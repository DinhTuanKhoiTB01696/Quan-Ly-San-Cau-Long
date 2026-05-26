using BadmintonApp.Domain.Enums;

namespace BadmintonApp.Domain.Entities;

public class Match
{
    public int Id { get; set; }
    public int CourtId { get; set; }
    public Court Court { get; set; } = null!;
    
    public int HostUserId { get; set; }
    public User HostUser { get; set; } = null!;
    public string HostName { get; set; } = string.Empty;
    public string Zalo { get; set; } = string.Empty;

    public DateTime Date { get; set; }
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    
    public int SlotsTotal { get; set; }
    public int SlotsFilled { get; set; }
    
    public Level Level { get; set; }
    public decimal Cost { get; set; }
    public string Note { get; set; } = string.Empty;
    
    public MatchStatus Status { get; set; } = MatchStatus.Open;
    public int ReportCount { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<MatchParticipant> Participants { get; set; } = new List<MatchParticipant>();
}
