using BadmintonApp.Application.DTOs.Reports;
using BadmintonApp.Application.DTOs.Matches;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Domain.Entities;
using BadmintonApp.Domain.Enums;
using BadmintonApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BadmintonApp.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReportDto>> GetAllPendingAsync()
    {
        return await _context.Reports
            .Include(r => r.Match)
            .Where(r => r.Status == ReportStatus.Pending)
            .Select(r => new ReportDto
            {
                Id = r.Id,
                MatchId = r.MatchId,
                Match = new MatchDto 
                {
                    Id = r.Match.Id,
                    HostName = r.Match.HostName,
                    Date = r.Match.Date,
                    ReportCount = r.Match.ReportCount
                },
                ReportedByUserId = r.ReportedByUserId,
                Reason = r.Reason,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ReportDto> CreateAsync(int? userId, CreateReportDto createDto)
    {
        var match = await _context.Matches.FindAsync(createDto.MatchId)
            ?? throw new KeyNotFoundException("Match not found");

        var report = new Report
        {
            MatchId = createDto.MatchId,
            ReportedByUserId = userId,
            Reason = createDto.Reason,
            Status = ReportStatus.Pending
        };

        _context.Reports.Add(report);
        
        // Tăng biến đếm ReportCount của Match
        match.ReportCount += 1;
        
        // Logic: Nếu >= 3 report thì tự động ẩn kèo
        if (match.ReportCount >= 3)
        {
            match.Status = MatchStatus.Expired;
        }

        await _context.SaveChangesAsync();

        return new ReportDto
        {
            Id = report.Id,
            MatchId = report.MatchId,
            ReportedByUserId = report.ReportedByUserId,
            Reason = report.Reason,
            Status = report.Status,
            CreatedAt = report.CreatedAt
        };
    }

    public async Task ResolveAsync(int id)
    {
        var report = await _context.Reports.FindAsync(id)
            ?? throw new KeyNotFoundException("Report not found");

        report.Status = ReportStatus.Resolved;
        await _context.SaveChangesAsync();
    }
}
