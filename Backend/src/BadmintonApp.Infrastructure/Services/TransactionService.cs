using BadmintonApp.Application.DTOs.Transactions;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Domain.Entities;
using BadmintonApp.Domain.Enums;
using BadmintonApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace BadmintonApp.Infrastructure.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public TransactionService(AppDbContext context, IEmailService emailService, IConfiguration config)
    {
        _context = context;
        _emailService = emailService;
        _config = config;
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

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
    {
        var transactions = await _context.Transactions
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
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
        var transaction = new Transaction
        {
            UserId = userId,
            Amount = dto.Amount,
            CreditsAdded = dto.CreditsAdded,
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Gửi email cho Admin thông báo có yêu cầu nạp tiền mới
        try
        {
            var user = await _context.Users.FindAsync(userId);
            var adminEmail = _config["EmailSettings:AdminEmail"] ?? "khoidttb01696@gmail.com";
            
            var subject = $"[Yêu Cầu Nạp Lượt] Giao dịch mới cần duyệt từ {user?.FullName ?? user?.Username}";
            var body = $@"
                <h2>YÊU CẦU NẠP LƯỢT ĐĂNG BÀI MỚI</h2>
                <p>Chào Admin,</p>
                <p>Hệ thống vừa nhận được yêu cầu nạp tiền mới từ người chơi. Dưới đây là thông tin chi tiết:</p>
                <table border='1' cellpadding='10' cellspacing='0' style='border-collapse: collapse; min-width: 400px; font-family: sans-serif; border-color: #e5e7eb;'>
                    <tr style='background-color: #10b981; color: white;'>
                        <td><strong>Thông tin</strong></td>
                        <td><strong>Chi tiết</strong></td>
                    </tr>
                    <tr>
                        <td>Người yêu cầu</td>
                        <td>{user?.FullName} ({user?.Username})</td>
                    </tr>
                    <tr>
                        <td>Số điện thoại</td>
                        <td>{user?.Phone}</td>
                    </tr>
                    <tr>
                        <td>Số tiền nạp</td>
                        <td><strong style='color: #16a34a;'>{dto.Amount:N0} VND</strong></td>
                    </tr>
                    <tr>
                        <td>Số lượt mua (Credits)</td>
                        <td><strong>{dto.CreditsAdded} lượt</strong></td>
                    </tr>
                    <tr>
                        <td>Mã giao dịch</td>
                        <td><strong style='font-family: monospace; font-size: 16px; background-color: #f3f4f6; padding: 4px 8px;'>LN{transaction.CreatedAt:yyMMddHHmmss}{transaction.Id}</strong></td>
                    </tr>
                    <tr>
                        <td>Trạng thái</td>
                        <td><span style='color: #eab308; font-weight: bold;'>Đang chờ duyệt (Pending)</span></td>
                    </tr>
                </table>
                <br/>
                <p>Vui lòng kiểm tra tài khoản ngân hàng và truy cập trang Admin Dashboard của Website để thực hiện duyệt lượt cho thành viên này.</p>
                <p>Trân trọng,<br/>Hệ thống Ghép Kèo Cầu Lông Biên Hòa</p>
            ";

            await _emailService.SendEmailAsync(adminEmail, subject, body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email to Admin: {ex.Message}");
        }

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
        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var transaction = await _context.Transactions.FindAsync(transactionId)
                ?? throw new KeyNotFoundException("Transaction not found");

            if (transaction.Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Only pending transactions can be updated");

            transaction.Status = status;
            _context.Transactions.Update(transaction);

            if (status == TransactionStatus.Approved)
            {
                var user = await _context.Users.FindAsync(transaction.UserId)
                    ?? throw new InvalidOperationException("User not found");
                
                user.Credits += transaction.CreditsAdded;
                user.AvailablePosts += transaction.CreditsAdded;
                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        }
        catch (Exception)
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }
}
