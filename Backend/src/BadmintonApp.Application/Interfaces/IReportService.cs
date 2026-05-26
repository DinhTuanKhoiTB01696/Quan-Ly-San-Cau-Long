using BadmintonApp.Application.DTOs.Reports;

namespace BadmintonApp.Application.Interfaces;

public interface IReportService
{
    Task<IEnumerable<ReportDto>> GetAllPendingAsync();
    Task<ReportDto> CreateAsync(int? userId, CreateReportDto createDto);
    Task ResolveAsync(int id);
}
