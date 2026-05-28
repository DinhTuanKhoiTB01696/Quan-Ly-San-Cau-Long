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
    public IEnumerable<ParticipantDto> Participants { get; set; } = new List<ParticipantDto>();
    public int ReportCount { get; set; }
}

public class ParticipantDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string SkillLevel { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
}

public class PendingJoinDto
{
    public int MatchId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string SkillLevel { get; set; } = string.Empty;
    public string CourtName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan TimeStart { get; set; }
    public decimal Cost { get; set; }
    public DateTime JoinedAt { get; set; }
}

public class CreateMatchDto
{
    [Required(ErrorMessage = "Vui lòng chọn Sân cầu lông")]
    public int CourtId { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn Ngày đánh")]
    public DateTime Date { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn Giờ bắt đầu")]
    public TimeSpan TimeStart { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn Giờ kết thúc")]
    public TimeSpan TimeEnd { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập số Zalo liên hệ")]
    public string Zalo { get; set; } = string.Empty;
    
    public int SlotsTotal { get; set; }
    public Level Level { get; set; }
    public decimal Cost { get; set; }
    public string Note { get; set; } = string.Empty;
}
