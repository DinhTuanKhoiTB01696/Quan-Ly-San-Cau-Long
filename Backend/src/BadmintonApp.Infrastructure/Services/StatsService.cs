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

    public async Task<StatsDto> GetStatsAsync()
    {
        var vnTime = DateTime.UtcNow.AddHours(7);
        var today = vnTime.Date;

        var totalMatchesToday = await _context.Matches
            .CountAsync(m => m.Date == today);

        var totalCourts = await _context.Courts.CountAsync();

        var openMatches = await _context.Matches
            .CountAsync(m => m.Status == MatchStatus.Open);

        var pendingReports = await _context.Reports
            .CountAsync(r => r.Status == ReportStatus.Pending);

        return new StatsDto
        {
            TotalMatchesToday = totalMatchesToday,
            TotalCourts = totalCourts,
            OpenMatches = openMatches,
            PendingReports = pendingReports
        };
    }
}
