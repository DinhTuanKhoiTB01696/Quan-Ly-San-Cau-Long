namespace BadmintonApp.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Admin, User
    public bool IsLocked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string SkillLevel { get; set; } = "Trung bình"; // Mới chơi, Trung bình, Khá, Tốt
    
    public int Credits { get; set; } = 10;

    public ICollection<MatchParticipant> JoinedMatches { get; set; } = new List<MatchParticipant>();
}
