namespace BadmintonApp.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // "User" or "Admin"
    public bool IsLocked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int Credits { get; set; } = 3; // Mặc định tặng 3 lượt đăng

    public ICollection<MatchParticipant> JoinedMatches { get; set; } = new List<MatchParticipant>();
}
