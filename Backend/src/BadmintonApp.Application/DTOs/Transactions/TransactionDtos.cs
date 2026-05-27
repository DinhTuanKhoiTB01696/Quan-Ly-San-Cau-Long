using BadmintonApp.Domain.Enums;

namespace BadmintonApp.Application.DTOs.Transactions;

public class TransactionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int CreditsAdded { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTransactionDto
{
    public decimal Amount { get; set; }
    public int CreditsAdded { get; set; }
}
