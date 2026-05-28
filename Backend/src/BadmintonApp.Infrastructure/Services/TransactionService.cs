using BadmintonApp.Application.DTOs.Transactions;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Domain.Entities;
using BadmintonApp.Domain.Enums;
using BadmintonApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BadmintonApp.Infrastructure.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TransactionDto>> GetMyTransactionsAsync(int userId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return transactions.Select(t => new TransactionDto
        {
            Id = t.Id,
            UserId = t.UserId,
            Amount = t.Amount,
            CreditsAdded = t.CreditsAdded,
            Status = t.Status,
            CreatedAt = t.CreatedAt
        });
    }

    public async Task<IEnumerable<TransactionDto>> GetPendingTransactionsAsync()
    {
        var transactions = await _context.Transactions
            .Include(t => t.User)
            .Where(t => t.Status == TransactionStatus.Pending)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();

        return transactions.Select(t => new TransactionDto
        {
            Id = t.Id,
            UserId = t.UserId,
            Username = t.User?.Username ?? "Unknown",
            Amount = t.Amount,
            CreditsAdded = t.CreditsAdded,
            Status = t.Status,
            CreatedAt = t.CreatedAt
        });
    }

    public async Task<TransactionDto> CreateTransactionAsync(int userId, CreateTransactionDto dto)
    {
        // Tự động hóa 100% duyệt giao dịch nạp tiền để giả lập PayOS webhook trên môi trường test
        var transaction = new Transaction
        {
            UserId = userId,
            Amount = dto.Amount,
            CreditsAdded = dto.CreditsAdded,
            Status = TransactionStatus.Approved, // Duyệt tự động luôn
            CreatedAt = DateTime.UtcNow
        };

        var user = await _context.Users.FindAsync(userId)
            ?? throw new InvalidOperationException("User not found");
        user.Credits += dto.CreditsAdded; // Tự động cộng lượt nạp ngay lập tức

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return new TransactionDto
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            Amount = transaction.Amount,
            CreditsAdded = transaction.CreditsAdded,
            Status = transaction.Status,
            CreatedAt = transaction.CreatedAt
        };
    }

    public async Task UpdateTransactionStatusAsync(int transactionId, TransactionStatus status)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId)
            ?? throw new KeyNotFoundException("Transaction not found");

        if (transaction.Status != TransactionStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be updated");

        transaction.Status = status;

        if (status == TransactionStatus.Approved)
        {
            var user = await _context.Users.FindAsync(transaction.UserId)
                ?? throw new InvalidOperationException("User not found");
            user.Credits += transaction.CreditsAdded;
        }

        await _context.SaveChangesAsync();
    }
}
