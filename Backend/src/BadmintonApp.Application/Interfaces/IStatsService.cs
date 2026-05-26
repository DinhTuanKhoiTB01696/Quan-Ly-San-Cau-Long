using BadmintonApp.Application.DTOs.Stats;

namespace BadmintonApp.Application.Interfaces;

public interface IStatsService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
