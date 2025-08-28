using BankManagement.API.DTOs;

namespace BankManagement.API.Interfaces
{
    public interface ITransactionService
    {
        Task<ServiceResult<TransactionDto>> DepositAsync(int accountId, DepositDto depositDto);
        Task<ServiceResult<TransactionDto>> WithdrawAsync(int accountId, WithdrawDto withdrawDto);
        Task<ServiceResult<TransferResultDto>> TransferAsync(int fromAccountId, TransferDto transferDto);
        Task<ServiceResult<TransactionDto>> GetTransactionByIdAsync(int id);
        Task<ServiceResult<TransactionDto>> GetTransactionByTransactionIdAsync(string transactionId);
        Task<ServiceResult<IEnumerable<TransactionDto>>> GetTransactionsByAccountIdAsync(int accountId);
        Task<ServiceResult<PagedResult<TransactionDto>>> GetPagedTransactionsByAccountAsync(int accountId, TransactionSearchDto searchDto);
        Task<ServiceResult<IEnumerable<TransactionDto>>> GetRecentTransactionsAsync(int count = 10);
        Task<ServiceResult<TransactionStatisticsDto>> GetTransactionStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<ServiceResult<bool>> CancelTransactionAsync(int transactionId, string reason);
        Task<ServiceResult<bool>> ValidateTransactionAsync(string transactionId);
        Task<ServiceResult<decimal>> CalculateTransactionFeeAsync(string transactionType, decimal amount);
        Task<ServiceResult<IEnumerable<TransactionDto>>> GetPendingTransactionsAsync();
        Task<ServiceResult<bool>> ProcessPendingTransactionAsync(int transactionId);
    }
}

