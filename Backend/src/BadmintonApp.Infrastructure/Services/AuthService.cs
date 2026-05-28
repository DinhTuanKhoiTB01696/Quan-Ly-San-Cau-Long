using BadmintonApp.Application.DTOs.Auth;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Application.DTOs.Users;
using BadmintonApp.Domain.Entities;
using BadmintonApp.Infrastructure.Data;
using BadmintonApp.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;

namespace BadmintonApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không chính xác");
        }

        if (user.IsLocked)
        {
            throw new UnauthorizedAccessException("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Admin.");
        }

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
        {
            throw new ArgumentException("Tên đăng nhập đã tồn tại");
        }

        if (ProfanityFilter.ContainsProfanity(registerDto.FullName) || ProfanityFilter.ContainsProfanity(registerDto.Username))
        {
            throw new ArgumentException("Tên đăng nhập hoặc Họ tên chứa từ ngữ không phù hợp.");
        }

        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            FullName = registerDto.FullName,
            Phone = registerDto.Phone,
            SkillLevel = registerDto.SkillLevel ?? "Trung bình",
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return GenerateAuthResponse(user);
    }

    public async Task ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("Không tìm thấy người dùng");

        if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Mật khẩu cũ không chính xác");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _context.SaveChangesAsync();
    }

    public async Task<AuthResponseDto> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("Không tìm thấy người dùng");

        if (ProfanityFilter.ContainsProfanity(dto.FullName))
        {
            throw new ArgumentException("Họ tên chứa từ ngữ không phù hợp.");
        }

        user.FullName = dto.FullName;
        user.Phone = dto.Phone;
        user.SkillLevel = dto.SkillLevel ?? "Trung bình";

        await _context.SaveChangesAsync();

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> GetMeAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("Không tìm thấy người dùng");

        return GenerateAuthResponse(user);
    }

    private AuthResponseDto GenerateAuthResponse(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("FullName", user.FullName),
            new Claim("Phone", user.Phone ?? ""),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return new AuthResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            UserId = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Phone = user.Phone,
            Role = user.Role,
            Credits = user.Credits,
            SkillLevel = user.SkillLevel
        };
    }

    public async Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginDto dto)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { "1008910270642-flh9lb3sb1241tpvs9ssj495uhvhhmgb.apps.googleusercontent.com" }
            };
            
            var payload = await GoogleJsonWebSignature.ValidateAsync(dto.Token, settings);
            if (payload == null)
            {
                throw new ArgumentException("Token không hợp lệ.");
            }
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == payload.Email);
            
            if (user == null)
            {
                user = new User
                {
                    Username = payload.Email,
                    FullName = payload.Name,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), 
                    Credits = 10,
                    Role = "User"
                };
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else if (user.IsLocked)
            {
                throw new UnauthorizedAccessException("Tài khoản đã bị khóa");
            }
            
            return GenerateAuthResponse(user);
        }
        catch (InvalidJwtException)
        {
            throw new ArgumentException("Token Google không hợp lệ hoặc đã hết hạn.");
        }
    }
}
