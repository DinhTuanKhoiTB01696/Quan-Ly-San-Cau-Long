using BadmintonApp.Application.DTOs.Matches;
using BadmintonApp.Application.DTOs.Courts;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Domain.Entities;
using BadmintonApp.Domain.Enums;
using BadmintonApp.Infrastructure.Data;
using BadmintonApp.Application.Utils;
using Microsoft.EntityFrameworkCore;

namespace BadmintonApp.Infrastructure.Services;

public class MatchService : IMatchService
{
    private readonly AppDbContext _context;

    public MatchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MatchDto>> GetAllAsync(Area? area, Level? level, MatchStatus? status, DateTime? date)
    {
        var query = _context.Matches.Include(m => m.Court).Include(m => m.Participants).AsQueryable();

        if (area.HasValue)
            query = query.Where(m => m.Court.Area == area.Value);
        
        if (level.HasValue)
            query = query.Where(m => m.Level == level.Value);
            
        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        if (date.HasValue)
            query = query.Where(m => m.Date.Date == date.Value.Date);

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

    public async Task<IEnumerable<MatchDto>> GetJoinedMatchesAsync(int userId)
    {
        var matches = await _context.Matches
            .Include(m => m.Court)
            .Include(m => m.Participants)
            .Where(m => m.Participants.Any(p => p.UserId == userId))
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

        if (createDto.TimeStart >= createDto.TimeEnd)
        {
            throw new InvalidOperationException("Giờ bắt đầu phải nhỏ hơn giờ kết thúc");
        }

        if (createDto.SlotsTotal <= 0)
        {
            throw new InvalidOperationException("Tổng số người phải lớn hơn 0");
        }

        if (ProfanityFilter.ContainsProfanity(createDto.Note))
        {
            throw new InvalidOperationException("Nội dung ghi chú chứa từ ngữ không phù hợp. Vui lòng sử dụng ngôn từ văn minh.");
        }

        var user = await _context.Users.FindAsync(hostUserId) 
            ?? throw new InvalidOperationException("Không tìm thấy thông tin tài khoản");

        bool isAdmin = user.Role == "Admin";

        if (!isAdmin)
        {
            if (user.Credits <= 0)
            {
                throw new InvalidOperationException("Bạn đã hết lượt đăng kèo. Vui lòng nạp thêm để tiếp tục sử dụng dịch vụ.");
            }

            user.Credits -= 1;
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
            throw new InvalidOperationException("Kèo này đã đóng hoặc đầy, không thể tham gia.");

        if (match.HostUserId == userId)
            throw new InvalidOperationException("Chủ kèo không cần tham gia kèo của chính mình.");

        if (match.Participants.Any(p => p.UserId == userId))
            throw new InvalidOperationException("Bạn đã gửi yêu cầu tham gia kèo này rồi.");

        if (match.SlotsFilled >= match.SlotsTotal)
            throw new InvalidOperationException("Kèo này hiện tại đã đầy chỗ.");

        match.Participants.Add(new MatchParticipant
        {
            MatchId = matchId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            IsApproved = false // Mặc định là chờ duyệt (chờ đóng tiền cọc)
        });

        // Không tăng match.SlotsFilled ở đây. SlotsFilled chỉ tăng khi Admin duyệt đóng cọc!
        await _context.SaveChangesAsync();
    }

    public async Task LeaveMatchAsync(int matchId, int userId)
    {
        var match = await _context.Matches
            .Include(m => m.Participants)
            .FirstOrDefaultAsync(m => m.Id == matchId)
            ?? throw new KeyNotFoundException("Match not found");

        var participant = match.Participants.FirstOrDefault(p => p.UserId == userId)
            ?? throw new InvalidOperationException("Bạn chưa tham gia kèo này.");

        if (participant.IsApproved)
        {
            match.SlotsFilled--;
            if (match.Status == MatchStatus.Full && match.SlotsFilled < match.SlotsTotal)
            {
                match.Status = MatchStatus.Open;
            }
        }

        match.Participants.Remove(participant);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PendingJoinDto>> GetPendingJoinsAsync()
    {
        var pendingParticipants = await _context.MatchParticipants
            .Include(mp => mp.User)
            .Include(mp => mp.Match)
                .ThenInclude(m => m.Court)
            .Where(mp => !mp.IsApproved)
            .OrderByDescending(mp => mp.JoinedAt)
            .ToListAsync();

        return pendingParticipants.Select(mp => new PendingJoinDto
        {
            MatchId = mp.MatchId,
            UserId = mp.UserId,
            Username = mp.User.Username,
            FullName = mp.User.FullName,
            Phone = mp.User.Phone,
            SkillLevel = mp.User.SkillLevel,
            CourtName = mp.Match.Court?.Name ?? "Unknown",
            Date = mp.Match.Date,
            TimeStart = mp.Match.TimeStart,
            Cost = mp.Match.Cost,
            JoinedAt = mp.JoinedAt
        });
    }

    public async Task ApproveJoinAsync(int matchId, int userId)
    {
        var mp = await _context.MatchParticipants
            .Include(x => x.Match)
            .FirstOrDefaultAsync(x => x.MatchId == matchId && x.UserId == userId)
            ?? throw new KeyNotFoundException("Yêu cầu tham gia không tồn tại.");

        if (mp.IsApproved) return;

        var match = mp.Match;
        if (match.SlotsFilled >= match.SlotsTotal)
            throw new InvalidOperationException("Kèo này đã đầy chỗ, không thể duyệt thêm.");

        mp.IsApproved = true;
        match.SlotsFilled++;

        if (match.SlotsFilled >= match.SlotsTotal)
        {
            match.Status = MatchStatus.Full;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RejectJoinAsync(int matchId, int userId)
    {
        var mp = await _context.MatchParticipants
            .FirstOrDefaultAsync(x => x.MatchId == matchId && x.UserId == userId)
            ?? throw new KeyNotFoundException("Yêu cầu tham gia không tồn tại.");

        _context.MatchParticipants.Remove(mp);
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
                Username = p.User.Username,
                Phone = p.User.Phone,
                SkillLevel = p.User.SkillLevel,
                IsApproved = p.IsApproved
            }).ToList() ?? new List<ParticipantDto>()
        };
    }
}
