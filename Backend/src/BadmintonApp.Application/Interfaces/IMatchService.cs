using BadmintonApp.Application.DTOs.Matches;
using BadmintonApp.Domain.Enums;

namespace BadmintonApp.Application.Interfaces;

public interface IMatchService
{
    Task<IEnumerable<MatchDto>> GetAllAsync(Area? area, Level? level, MatchStatus? status);
    Task<IEnumerable<MatchDto>> GetByHostAsync(int hostUserId);
    Task<MatchDto?> GetByIdAsync(int id);
    Task<MatchDto> CreateAsync(int hostUserId, string hostName, CreateMatchDto createDto);
    Task UpdateStatusAsync(int id, int userId, MatchStatus status);
    Task DeleteAsync(int id, int userId, bool isAdmin);
    Task AutoExpireMatchesAsync();

    Task JoinMatchAsync(int matchId, int userId);
    Task LeaveMatchAsync(int matchId, int userId);
}
