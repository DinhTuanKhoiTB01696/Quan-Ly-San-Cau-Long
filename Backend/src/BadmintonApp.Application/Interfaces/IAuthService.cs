using BadmintonApp.Application.DTOs.Auth;

namespace BadmintonApp.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    Task<AuthResponseDto> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task<AuthResponseDto> GetMeAsync(int userId);
}
