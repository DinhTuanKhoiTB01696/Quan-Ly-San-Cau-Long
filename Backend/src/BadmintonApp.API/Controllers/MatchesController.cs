using BadmintonApp.Application.DTOs.Matches;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BadmintonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Area? area, [FromQuery] Level? level, [FromQuery] MatchStatus? status)
    {
        // Kích hoạt tính năng auto-expire mỗi khi có người get danh sách
        await _matchService.AutoExpireMatchesAsync();

        var matches = await _matchService.GetAllAsync(area, level, status);
        return Ok(matches);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var match = await _matchService.GetByIdAsync(id);
        if (match == null) return NotFound();
        return Ok(match);
    }

    [Authorize]
    [HttpGet("my-matches")]
    public async Task<IActionResult> GetMyMatches()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var matches = await _matchService.GetByHostAsync(userId);
        return Ok(matches);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateMatchDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hostName = User.FindFirstValue("FullName") ?? User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        try
        {
            var match = await _matchService.CreateAsync(userId, hostName, dto);
            return CreatedAtAction(nameof(GetById), new { id = match.Id }, match);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] MatchStatus status)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        try
        {
            await _matchService.UpdateStatusAsync(id, userId, status);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        bool isAdmin = role == "Admin";

        try
        {
            await _matchService.DeleteAsync(id, userId, isAdmin);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
