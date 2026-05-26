using BadmintonApp.Application.DTOs.Stats;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Domain.Enums;
using BadmintonApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BadmintonApp.Infrastructure.Services;

public class StatsService : IStatsService
{
    private readonly AppDbContext _context;

    public StatsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var totalUsers = await _context.Users.CountAsync();
        var newUsersThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= startOfMonth);

        var totalCourts = await _context.Courts.CountAsync();

        var totalMatches = await _context.Matches.CountAsync();
        var openMatches = await _context.Matches.CountAsync(m => m.Status == MatchStatus.Open);
        var fullMatches = await _context.Matches.CountAsync(m => m.Status == MatchStatus.Full);
        var expiredMatches = await _context.Matches.CountAsync(m => m.Status == MatchStatus.Expired);

        var pendingReports = await _context.Reports.CountAsync(r => r.Status == ReportStatus.Pending);

        var topCourts = await _context.Matches
            .GroupBy(m => new { m.CourtId, m.Court.Name })
            .Select(g => new TopCourtDto
            {
                CourtId = g.Key.CourtId,
                CourtName = g.Key.Name,
                MatchCount = g.Count()
            })
            .OrderByDescending(x => x.MatchCount)
            .Take(5)
            .ToListAsync();

        return new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            NewUsersThisMonth = newUsersThisMonth,
            TotalCourts = totalCourts,
            TotalMatches = totalMatches,
            OpenMatches = openMatches,
            FullMatches = fullMatches,
            ExpiredMatches = expiredMatches,
            PendingReports = pendingReports,
            TopCourts = topCourts
        };
    }
}
