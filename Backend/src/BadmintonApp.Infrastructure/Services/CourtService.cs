using BadmintonApp.Application.DTOs.Courts;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Domain.Entities;
using BadmintonApp.Domain.Enums;
using BadmintonApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BadmintonApp.Infrastructure.Services;

public class CourtService : ICourtService
{
    private readonly AppDbContext _context;

    public CourtService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CourtDto>> GetAllAsync(Area? area)
    {
        var query = _context.Courts.AsQueryable();

        if (area.HasValue)
        {
            query = query.Where(c => c.Area == area.Value);
        }

        return await query
            .OrderByDescending(c => c.IsFeatured)
            .ThenByDescending(c => c.Rating)
            .Select(c => new CourtDto
            {
                Id = c.Id,
                Name = c.Name,
                Area = c.Area,
                Address = c.Address,
                Price = c.Price,
                Ceiling = c.Ceiling,
                Light = c.Light,
                Surface = c.Surface,
                Rating = c.Rating,
                Phone = c.Phone,
                IsFeatured = c.IsFeatured,
                ImageUrl = c.ImageUrl
            })
            .ToListAsync();
    }

    public async Task<CourtDto?> GetByIdAsync(int id)
    {
        var c = await _context.Courts.FindAsync(id);
        if (c == null) return null;

        return new CourtDto
        {
            Id = c.Id,
            Name = c.Name,
            Area = c.Area,
            Address = c.Address,
            Price = c.Price,
            Ceiling = c.Ceiling,
            Light = c.Light,
            Surface = c.Surface,
            Rating = c.Rating,
            Phone = c.Phone,
            IsFeatured = c.IsFeatured,
            ImageUrl = c.ImageUrl
        };
    }

    public async Task<CourtDto> CreateAsync(CourtCreateDto createDto)
    {
        var court = new Court
        {
            Name = createDto.Name,
            Area = createDto.Area,
            Address = createDto.Address,
            Price = createDto.Price,
            Ceiling = createDto.Ceiling,
            Light = createDto.Light,
            Surface = createDto.Surface,
            Phone = createDto.Phone,
            ImageUrl = createDto.ImageUrl
        };

        _context.Courts.Add(court);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(court.Id) ?? throw new Exception("Create failed");
    }

    public async Task UpdateAsync(int id, CourtCreateDto updateDto)
    {
        var court = await _context.Courts.FindAsync(id) 
            ?? throw new KeyNotFoundException("Court not found");

        court.Name = updateDto.Name;
        court.Area = updateDto.Area;
        court.Address = updateDto.Address;
        court.Price = updateDto.Price;
        court.Ceiling = updateDto.Ceiling;
        court.Light = updateDto.Light;
        court.Surface = updateDto.Surface;
        court.Phone = updateDto.Phone;
        court.ImageUrl = updateDto.ImageUrl;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var court = await _context.Courts.FindAsync(id)
            ?? throw new KeyNotFoundException("Court not found");
            
        _context.Courts.Remove(court);
        await _context.SaveChangesAsync();
    }
}
