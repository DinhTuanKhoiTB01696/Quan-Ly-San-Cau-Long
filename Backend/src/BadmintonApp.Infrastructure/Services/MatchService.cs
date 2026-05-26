using BadmintonApp.Application.DTOs.Matches;
using BadmintonApp.Application.DTOs.Courts;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Domain.Entities;
using BadmintonApp.Domain.Enums;
using BadmintonApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BadmintonApp.Infrastructure.Services;

public class MatchService : IMatchService
{
    private readonly AppDbContext _context;

    public MatchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MatchDto>> GetAllAsync(Area? area, Level? level, MatchStatus? status)
    {
        var query = _context.Matches.Include(m => m.Court).Include(m => m.Participants).AsQueryable();

        if (area.HasValue)
            query = query.Where(m => m.Court.Area == area.Value);
        
        if (level.HasValue)
            query = query.Where(m => m.Level == level.Value);
            
        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        var matches = await query
            .OrderBy(m => m.Status) // Open (1), Full (2), Expired (3)
            .ThenBy(m => m.Date)
            .ThenBy(m => m.TimeStart)
            .ToListAsync();

        return matches.Select(MapToDto);
    }

    public async Task<IEnumerable<MatchDto>> GetByHostAsync(int hostUserId)
    {
        var matches = await _context.Matches
            .Include(m => m.Court)
            .Include(m => m.Participants)
            .Where(m => m.HostUserId == hostUserId)
            .OrderByDescending(m => m.Date)
            .ToListAsync();

        return matches.Select(MapToDto);
    }

    public async Task<MatchDto?> GetByIdAsync(int id)
    {
        var m = await _context.Matches
            .Include(x => x.Court)
            .Include(x => x.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(x => x.Id == id);
        return m == null ? null : MapToDto(m);
    }

    public async Task<MatchDto> CreateAsync(int hostUserId, string hostName, CreateMatchDto createDto)
    {
        var targetDate = createDto.Date.Date;
        // Check trùng lặp
        var isDuplicate = await _context.Matches.AnyAsync(m => 
            m.CourtId == createDto.CourtId &&
            m.Date == targetDate &&
            ((createDto.TimeStart >= m.TimeStart && createDto.TimeStart < m.TimeEnd) ||
             (createDto.TimeEnd > m.TimeStart && createDto.TimeEnd <= m.TimeEnd) ||
             (createDto.TimeStart <= m.TimeStart && createDto.TimeEnd >= m.TimeEnd)) &&
            m.Status != MatchStatus.Expired);
            
        if (isDuplicate)
        {
            throw new InvalidOperationException("Đã có kèo tại sân này trong khoảng thời gian trên.");
        }

        var match = new Match
        {
            CourtId = createDto.CourtId,
            HostUserId = hostUserId,
            HostName = hostName,
            Zalo = createDto.Zalo,
            Date = createDto.Date.Date,
            TimeStart = createDto.TimeStart,
            TimeEnd = createDto.TimeEnd,
            SlotsTotal = createDto.SlotsTotal,
            SlotsFilled = 0,
            Level = createDto.Level,
            Cost = createDto.Cost,
            Note = createDto.Note,
            Status = MatchStatus.Open
        };

        _context.Matches.Add(match);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(match.Id) ?? throw new Exception("Create match failed");
    }

    public async Task UpdateStatusAsync(int id, int userId, MatchStatus status)
    {
        var match = await _context.Matches.FindAsync(id) 
            ?? throw new KeyNotFoundException("Match not found");

        if (match.HostUserId != userId)
            throw new UnauthorizedAccessException("Only host can update match status");

        match.Status = status;
        if (status == MatchStatus.Full)
        {
            match.SlotsFilled = match.SlotsTotal;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, int userId, bool isAdmin)
    {
        var match = await _context.Matches.FindAsync(id)
            ?? throw new KeyNotFoundException("Match not found");

        if (!isAdmin && match.HostUserId != userId)
            throw new UnauthorizedAccessException("Not allowed to delete this match");

        _context.Matches.Remove(match);
        await _context.SaveChangesAsync();
    }

    public async Task AutoExpireMatchesAsync()
    {
        var now = DateTime.UtcNow;
        // Chuyển sang giờ Việt Nam (UTC+7) cho logic đơn giản
        var vnTime = now.AddHours(7);
        var today = vnTime.Date;
        var currentTime = vnTime.TimeOfDay;
        var timeLimit = currentTime.Subtract(TimeSpan.FromHours(2));

        IQueryable<Match> query = _context.Matches.Where(m => m.Status != MatchStatus.Expired);

        if (timeLimit < TimeSpan.Zero)
        {
            // Nếu timeLimit < 0 (vd mới 1h sáng), thì không có kèo nào của ngày hôm nay có thể đã kết thúc quá 2 tiếng.
            query = query.Where(m => m.Date < today);
        }
        else
        {
            query = query.Where(m => m.Date < today || (m.Date == today && m.TimeEnd < timeLimit));
        }

        // Cập nhật các kèo có giờ kết thúc đã qua quá 2 tiếng
        var expiredMatches = await query.ToListAsync();

        foreach (var match in expiredMatches)
        {
            match.Status = MatchStatus.Expired;
        }

        if (expiredMatches.Any())
        {
            await _context.SaveChangesAsync();
        }
    }

    public async Task JoinMatchAsync(int matchId, int userId)
    {
        var match = await _context.Matches
            .Include(m => m.Participants)
            .FirstOrDefaultAsync(m => m.Id == matchId)
            ?? throw new KeyNotFoundException("Match not found");

        if (match.Status != MatchStatus.Open)
            throw new InvalidOperationException("This match is not open for joining.");

        if (match.HostUserId == userId)
            throw new InvalidOperationException("Host cannot join their own match as a participant.");

        if (match.Participants.Any(p => p.UserId == userId))
            throw new InvalidOperationException("You have already joined this match.");

        if (match.SlotsFilled >= match.SlotsTotal)
            throw new InvalidOperationException("This match is already full.");

        match.Participants.Add(new MatchParticipant
        {
            MatchId = matchId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        });

        match.SlotsFilled++;

        if (match.SlotsFilled >= match.SlotsTotal)
        {
            match.Status = MatchStatus.Full;
        }

        await _context.SaveChangesAsync();
    }

    public async Task LeaveMatchAsync(int matchId, int userId)
    {
        var match = await _context.Matches
            .Include(m => m.Participants)
            .FirstOrDefaultAsync(m => m.Id == matchId)
            ?? throw new KeyNotFoundException("Match not found");

        var participant = match.Participants.FirstOrDefault(p => p.UserId == userId)
            ?? throw new InvalidOperationException("You are not a participant of this match.");

        match.Participants.Remove(participant);
        match.SlotsFilled--;

        if (match.Status == MatchStatus.Full && match.SlotsFilled < match.SlotsTotal)
        {
            match.Status = MatchStatus.Open;
        }

        await _context.SaveChangesAsync();
    }

    private static MatchDto MapToDto(Match m)
    {
        return new MatchDto
        {
            Id = m.Id,
            CourtId = m.CourtId,
            Court = m.Court == null ? null : new CourtDto
            {
                Id = m.Court.Id,
                Name = m.Court.Name,
                Area = m.Court.Area,
                Address = m.Court.Address,
                Price = m.Court.Price,
                Rating = m.Court.Rating
            },
            HostUserId = m.HostUserId,
            HostName = m.HostName,
            Zalo = m.Zalo,
            Date = m.Date,
            TimeStart = m.TimeStart,
            TimeEnd = m.TimeEnd,
            SlotsTotal = m.SlotsTotal,
            SlotsFilled = m.SlotsFilled,
            Level = m.Level,
            Cost = m.Cost,
            Note = m.Note,
            Status = m.Status,
            ParticipantIds = m.Participants?.Select(p => p.UserId).ToList() ?? new List<int>(),
            Participants = m.Participants?.Where(p => p.User != null).Select(p => new ParticipantDto 
            {
                UserId = p.UserId,
                FullName = p.User.FullName,
                Username = p.User.Username
            }).ToList() ?? new List<ParticipantDto>()
        };
    }
}
