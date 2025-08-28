using BankManagement.API.Models;

namespace BankManagement.API.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task<Transaction?> GetTransactionByTransactionIdAsync(string transactionId);
        Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId);
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<Transaction> UpdateTransactionAsync(Transaction transaction);
        Task<bool> DeleteTransactionAsync(int id);
        Task<(IEnumerable<Transaction> transactions, int totalCount)> GetPagedTransactionsByAccountAsync(
            int accountId, int page, int pageSize, string? transactionType = null, 
            DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(int count = 10);
        Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(string transactionType);
        Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalTransactionAmountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetTransactionCountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, decimal>> GetTransactionTypeStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Transaction>> GetPendingTransactionsAsync();
        Task<bool> TransactionExistsAsync(string transactionId);
    }
}

