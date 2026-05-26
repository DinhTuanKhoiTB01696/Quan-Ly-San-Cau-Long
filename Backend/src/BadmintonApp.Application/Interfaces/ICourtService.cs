using BadmintonApp.Application.DTOs.Courts;
using BadmintonApp.Domain.Enums;

namespace BadmintonApp.Application.Interfaces;

public interface ICourtService
{
    Task<IEnumerable<CourtDto>> GetAllAsync(Area? area);
    Task<CourtDto?> GetByIdAsync(int id);
    Task<CourtDto> CreateAsync(CourtCreateDto createDto);
    Task UpdateAsync(int id, CourtCreateDto updateDto);
    Task DeleteAsync(int id);
}
