namespace BadmintonApp.Application.DTOs.Stats;

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    
    public int TotalCourts { get; set; }
    public List<TopCourtDto> TopCourts { get; set; } = new();
    
    public int TotalMatches { get; set; }
    public int OpenMatches { get; set; }
    public int FullMatches { get; set; }
    public int ExpiredMatches { get; set; }
    
    public int PendingReports { get; set; }
    
    public List<MatchCountByDateDto> MatchCountsByDate { get; set; } = new();
}

public class MatchCountByDateDto
{
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TopCourtDto
{
    public int CourtId { get; set; }
    public string CourtName { get; set; } = string.Empty;
    public int MatchCount { get; set; }
}
