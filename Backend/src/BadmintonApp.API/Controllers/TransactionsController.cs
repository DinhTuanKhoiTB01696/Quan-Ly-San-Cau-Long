using BadmintonApp.Application.DTOs.Transactions;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BadmintonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [Authorize]
    [HttpGet("my-transactions")]
    public async Task<IActionResult> GetMyTransactions()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var result = await _transactionService.GetMyTransactionsAsync(userId);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _transactionService.GetPendingTransactionsAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var result = await _transactionService.GetAllTransactionsAsync();
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var result = await _transactionService.CreateTransactionAsync(userId, dto);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] TransactionStatus status)
    {
        try
        {
            await _transactionService.UpdateTransactionStatusAsync(id, status);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
