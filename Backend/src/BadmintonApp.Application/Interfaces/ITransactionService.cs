using BadmintonApp.Application.DTOs.Transactions;
using BadmintonApp.Domain.Enums;

namespace BadmintonApp.Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetMyTransactionsAsync(int userId);
    Task<IEnumerable<TransactionDto>> GetPendingTransactionsAsync();
    Task<TransactionDto> CreateTransactionAsync(int userId, CreateTransactionDto dto);
    Task UpdateTransactionStatusAsync(int transactionId, TransactionStatus status);
}
