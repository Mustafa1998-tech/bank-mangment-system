using Microsoft.EntityFrameworkCore;
using BankManagement.API.Data;
using BankManagement.API.Interfaces;
using BankManagement.API.Models;

namespace BankManagement.API.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankDbContext _context;
        private readonly ILogger<TransactionRepository> _logger;

        public TransactionRepository(BankDbContext context, ILogger<TransactionRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            try
            {
                return await _context.Transactions
                    .Include(t => t.Account)
                    .OrderByDescending(t => t.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all transactions");
                throw;
            }
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            try
            {
                return await _context.Transactions
                    .Include(t => t.Account)
                    .FirstOrDefaultAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transaction by id: {TransactionId}", id);
                throw;
            }
        }

        public async Task<Transaction?> GetTransactionByTransactionIdAsync(string transactionId)
        {
            try
            {
                return await _context.Transactions
                    .Include(t => t.Account)
                    .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transaction by transaction id: {TransactionId}", transactionId);
                throw;
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId)
        {
            try
            {
                return await _context.Transactions
                    .Where(t => t.AccountId == accountId)
                    .Include(t => t.Account)
                    .OrderByDescending(t => t.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transactions for account: {AccountId}", accountId);
                throw;
            }
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            try
            {
                // Generate unique transaction ID
                string transactionId;
                do
                {
                    transactionId = transaction.GenerateTransactionId();
                } while (await _context.Transactions.AnyAsync(t => t.TransactionId == transactionId));

                transaction.TransactionId = transactionId;
                transaction.Timestamp = DateTime.UtcNow;

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Transaction created successfully with ID: {TransactionId}", transaction.TransactionId);
                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating transaction for account: {AccountId}", transaction.AccountId);
                throw;
            }
        }

        public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
        {
            try
            {
                _context.Transactions.Update(transaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Transaction updated successfully with ID: {TransactionId}", transaction.TransactionId);
                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating transaction: {TransactionId}", transaction.TransactionId);
                throw;
            }
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            try
            {
                var transaction = await _context.Transactions.FindAsync(id);
                if (transaction == null)
                    return false;

                // Only allow deletion of failed or cancelled transactions
                if (transaction.Status == "Completed")
                {
                    _logger.LogWarning("Cannot delete completed transaction: {TransactionId}", transaction.TransactionId);
                    return false;
                }

                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Transaction deleted successfully with ID: {TransactionId}", transaction.TransactionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting transaction with ID: {TransactionId}", id);
                throw;
            }
        }

        public async Task<(IEnumerable<Transaction> transactions, int totalCount)> GetPagedTransactionsByAccountAsync(
            int accountId, int page, int pageSize, string? transactionType = null, 
            DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.Transactions
                    .Where(t => t.AccountId == accountId)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(transactionType))
                {
                    query = query.Where(t => t.TransactionType == transactionType);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(t => t.Timestamp >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(t => t.Timestamp <= endDate.Value);
                }

                var totalCount = await query.CountAsync();

                var transactions = await query
                    .OrderByDescending(t => t.Timestamp)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(t => t.Account)
                    .ToListAsync();

                return (transactions, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paged transactions for account: {AccountId}", accountId);
                throw;
            }
        }

        public async Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(int count = 10)
        {
            try
            {
                return await _context.Transactions
                    .Include(t => t.Account)
                    .OrderByDescending(t => t.Timestamp)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting recent transactions");
                throw;
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(string transactionType)
        {
            try
            {
                return await _context.Transactions
                    .Where(t => t.TransactionType == transactionType)
                    .Include(t => t.Account)
                    .OrderByDescending(t => t.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transactions by type: {TransactionType}", transactionType);
                throw;
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Transactions
                    .Where(t => t.Timestamp >= startDate && t.Timestamp <= endDate)
                    .Include(t => t.Account)
                    .OrderByDescending(t => t.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transactions by date range");
                throw;
            }
        }

        public async Task<decimal> GetTotalTransactionAmountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.Transactions.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(t => t.Timestamp >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(t => t.Timestamp <= endDate.Value);

                return await query
                    .Where(t => t.Status == "Completed")
                    .SumAsync(t => t.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting total transaction amount");
                throw;
            }
        }

        public async Task<int> GetTransactionCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.Transactions.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(t => t.Timestamp >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(t => t.Timestamp <= endDate.Value);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transaction count");
                throw;
            }
        }

        public async Task<Dictionary<string, decimal>> GetTransactionTypeStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.Transactions.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(t => t.Timestamp >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(t => t.Timestamp <= endDate.Value);

                return await query
                    .Where(t => t.Status == "Completed")
                    .GroupBy(t => t.TransactionType)
                    .ToDictionaryAsync(g => g.Key, g => g.Sum(t => t.Amount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transaction type statistics");
                throw;
            }
        }

        public async Task<IEnumerable<Transaction>> GetPendingTransactionsAsync()
        {
            try
            {
                return await _context.Transactions
                    .Where(t => t.Status == "Pending")
                    .Include(t => t.Account)
                    .OrderBy(t => t.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting pending transactions");
                throw;
            }
        }

        public async Task<bool> TransactionExistsAsync(string transactionId)
        {
            try
            {
                return await _context.Transactions.AnyAsync(t => t.TransactionId == transactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if transaction exists: {TransactionId}", transactionId);
                throw;
            }
        }
    }
}

