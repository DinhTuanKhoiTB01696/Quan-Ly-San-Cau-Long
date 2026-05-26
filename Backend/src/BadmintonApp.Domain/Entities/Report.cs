using BadmintonApp.Domain.Enums;

namespace BadmintonApp.Domain.Entities;

public class Report
{
    public int Id { get; set; }
    
    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;
    
    public int? ReportedByUserId { get; set; }
    public User? ReportedByUser { get; set; }
    
    public ReportReason Reason { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
