using BadmintonApp.Domain.Enums;
using BadmintonApp.Application.DTOs.Courts;
using System.ComponentModel.DataAnnotations;

namespace BadmintonApp.Application.DTOs.Matches;

public class MatchDto
{
    public int Id { get; set; }
    public int CourtId { get; set; }
    public CourtDto? Court { get; set; }
    
    public int HostUserId { get; set; }
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
    
    public MatchStatus Status { get; set; }
    public IEnumerable<int> ParticipantIds { get; set; } = new List<int>();
    public int ReportCount { get; set; }
}

public class CreateMatchDto
{
    [Required]
    public int CourtId { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public TimeSpan TimeStart { get; set; }
    [Required]
    public TimeSpan TimeEnd { get; set; }
    [Required]
    public string Zalo { get; set; } = string.Empty;
    
    public int SlotsTotal { get; set; }
    public Level Level { get; set; }
    public decimal Cost { get; set; }
    public string Note { get; set; } = string.Empty;
}
