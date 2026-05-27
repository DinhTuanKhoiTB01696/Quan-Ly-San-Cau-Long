using BadmintonApp.Domain.Enums;

namespace BadmintonApp.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public decimal Amount { get; set; }
    public int CreditsAdded { get; set; }
    
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
