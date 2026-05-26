using BadmintonApp.Domain.Enums;
using BadmintonApp.Application.DTOs.Matches;
using System.ComponentModel.DataAnnotations;

namespace BadmintonApp.Application.DTOs.Reports;

public class ReportDto
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public MatchDto? Match { get; set; }
    public int? ReportedByUserId { get; set; }
    public ReportReason Reason { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReportDto
{
    [Required]
    public int MatchId { get; set; }
    [Required]
    public ReportReason Reason { get; set; }
}
