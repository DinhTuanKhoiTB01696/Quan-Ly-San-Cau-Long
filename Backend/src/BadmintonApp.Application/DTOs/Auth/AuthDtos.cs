using System.ComponentModel.DataAnnotations;

namespace BadmintonApp.Application.DTOs.Auth;

public class LoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string FullName { get; set; } = string.Empty;
    [Required]
    public string Phone { get; set; } = string.Empty;
}

public class UpdateProfileDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;
    [Required]
    public string Phone { get; set; } = string.Empty;
}

public class GoogleLoginDto
{
    public string Token { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int Credits { get; set; }
}
