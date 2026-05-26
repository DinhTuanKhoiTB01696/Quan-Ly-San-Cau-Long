using BadmintonApp.Application.DTOs.Auth;

namespace BadmintonApp.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task ChangePasswordAsync(int userId, string oldPassword, string newPassword);
}
