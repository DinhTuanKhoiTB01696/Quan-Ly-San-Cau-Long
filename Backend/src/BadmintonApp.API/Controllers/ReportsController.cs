using BadmintonApp.Application.DTOs.Reports;
using BadmintonApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BadmintonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllPending()
    {
        var reports = await _reportService.GetAllPendingAsync();
        return Ok(reports);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReportDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? userId = null;
        if (int.TryParse(userIdStr, out int id))
        {
            userId = id;
        }

        try
        {
            var report = await _reportService.CreateAsync(userId, dto);
            return Ok(report);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Match not found" });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/resolve")]
    public async Task<IActionResult> Resolve(int id)
    {
        try
        {
            await _reportService.ResolveAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
