using BadmintonApp.Application.DTOs.Courts;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BadmintonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourtsController : ControllerBase
{
    private readonly ICourtService _courtService;

    public CourtsController(ICourtService courtService)
    {
        _courtService = courtService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Area? area)
    {
        var courts = await _courtService.GetAllAsync(area);
        return Ok(courts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var court = await _courtService.GetByIdAsync(id);
        if (court == null) return NotFound();
        return Ok(court);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CourtCreateDto dto)
    {
        var court = await _courtService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = court.Id }, court);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CourtCreateDto dto)
    {
        try
        {
            await _courtService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _courtService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
