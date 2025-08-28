using Microsoft.EntityFrameworkCore;
using BankManagement.API.Data;
using BankManagement.API.Interfaces;
using BankManagement.API.Models;
using BankManagement.API.DTOs;

namespace BankManagement.API.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankDbContext _context;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(BankDbContext context, ILogger<AccountRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            try
            {
                return await _context.Accounts
                    .Include(a => a.Transactions)
                    .Include(a => a.Cards)
                    .Include(a => a.Loans)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all accounts");
                throw;
            }
        }

        public async Task<Account?> GetAccountByIdAsync(int id)
        {
            try
            {
                return await _context.Accounts
                    .Include(a => a.Transactions.OrderByDescending(t => t.Timestamp).Take(10))
                    .Include(a => a.Cards)
                    .Include(a => a.Loans)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account by id: {AccountId}", id);
                throw;
            }
        }

        public async Task<Account?> GetAccountByNumberAsync(string accountNumber)
        {
            try
            {
                return await _context.Accounts
                    .Include(a => a.Transactions)
                    .Include(a => a.Cards)
                    .Include(a => a.Loans)
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account by number: {AccountNumber}", accountNumber);
                throw;
            }
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            try
            {
                return await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account by email: {Email}", email);
                throw;
            }
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            try
            {
                // Generate unique account number
                string accountNumber;
                do
                {
                    accountNumber = account.GenerateAccountNumber();
                } while (await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber));

                account.AccountNumber = accountNumber;
                account.CreatedAt = DateTime.UtcNow;
                account.UpdatedAt = DateTime.UtcNow;

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Account created successfully with ID: {AccountId}", account.Id);
                return account;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating account for: {OwnerName}", account.OwnerName);
                throw;
            }
        }

        public async Task<Account> UpdateAccountAsync(Account account)
        {
            try
            {
                account.UpdatedAt = DateTime.UtcNow;
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Account updated successfully with ID: {AccountId}", account.Id);
                return account;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating account with ID: {AccountId}", account.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(id);
                if (account == null)
                    return false;

                // Check if account has active transactions or loans
                var hasActiveTransactions = await _context.Transactions
                    .AnyAsync(t => t.AccountId == id && t.Status == "Pending");
                
                var hasActiveLoans = await _context.Loans
                    .AnyAsync(l => l.AccountId == id && l.Status == "Active");

                if (hasActiveTransactions || hasActiveLoans)
                {
                    _logger.LogWarning("Cannot delete account {AccountId} - has active transactions or loans", id);
                    return false;
                }

                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Account deleted successfully with ID: {AccountId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting account with ID: {AccountId}", id);
                throw;
            }
        }

        public async Task<bool> AccountExistsAsync(int id)
        {
            try
            {
                return await _context.Accounts.AnyAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if account exists: {AccountId}", id);
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                return await _context.Accounts.AnyAsync(a => a.Email.ToLower() == email.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if email exists: {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<Account>> SearchAccountsAsync(string searchTerm)
        {
            try
            {
                var lowerSearchTerm = searchTerm.ToLower();
                return await _context.Accounts
                    .Where(a => a.OwnerName.ToLower().Contains(lowerSearchTerm) ||
                               a.Email.ToLower().Contains(lowerSearchTerm) ||
                               a.AccountNumber.Contains(searchTerm) ||
                               (a.PhoneNumber != null && a.PhoneNumber.Contains(searchTerm)))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching accounts with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Account>> GetAccountsByTypeAsync(string accountType)
        {
            try
            {
                return await _context.Accounts
                    .Where(a => a.AccountType == accountType)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting accounts by type: {AccountType}", accountType);
                throw;
            }
        }

        public async Task<IEnumerable<Account>> GetAccountsByStatusAsync(string status)
        {
            try
            {
                return await _context.Accounts
                    .Where(a => a.Status == status)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting accounts by status: {Status}", status);
                throw;
            }
        }

        public async Task<(IEnumerable<Account> accounts, int totalCount)> GetPagedAccountsAsync(
            int page, int pageSize, string? searchTerm = null, string? accountType = null, string? status = null)
        {
            try
            {
                var query = _context.Accounts.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var lowerSearchTerm = searchTerm.ToLower();
                    query = query.Where(a => a.OwnerName.ToLower().Contains(lowerSearchTerm) ||
                                           a.Email.ToLower().Contains(lowerSearchTerm) ||
                                           a.AccountNumber.Contains(searchTerm));
                }

                if (!string.IsNullOrEmpty(accountType))
                {
                    query = query.Where(a => a.AccountType == accountType);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(a => a.Status == status);
                }

                var totalCount = await query.CountAsync();

                var accounts = await query
                    .OrderByDescending(a => a.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(a => a.Transactions)
                    .Include(a => a.Cards)
                    .Include(a => a.Loans)
                    .ToListAsync();

                return (accounts, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paged accounts");
                throw;
            }
        }

        public async Task<decimal> GetTotalBalanceAsync()
        {
            try
            {
                return await _context.Accounts
                    .Where(a => a.Status == "Active")
                    .SumAsync(a => a.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting total balance");
                throw;
            }
        }

        public async Task<int> GetAccountsCountAsync()
        {
            try
            {
                return await _context.Accounts.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting accounts count");
                throw;
            }
        }

        public async Task<int> GetActiveAccountsCountAsync()
        {
            try
            {
                return await _context.Accounts.CountAsync(a => a.Status == "Active");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting active accounts count");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetAccountTypeStatisticsAsync()
        {
            try
            {
                return await _context.Accounts
                    .GroupBy(a => a.AccountType)
                    .ToDictionaryAsync(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account type statistics");
                throw;
            }
        }
    }
}

