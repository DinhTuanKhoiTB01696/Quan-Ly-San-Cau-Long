namespace BadmintonApp.Domain.Entities;

public class MatchParticipant
{
    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
