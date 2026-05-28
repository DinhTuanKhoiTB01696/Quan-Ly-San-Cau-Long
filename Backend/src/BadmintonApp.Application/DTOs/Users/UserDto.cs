namespace BadmintonApp.Application.DTOs.Users;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
    public int Credits { get; set; }
    public int AvailablePosts { get; set; }
    public DateTime CreatedAt { get; set; }
}
