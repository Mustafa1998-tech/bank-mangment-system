using BankManagement.API.Models;
using BankManagement.API.DTOs;

namespace BankManagement.API.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account?> GetAccountByIdAsync(int id);
        Task<Account?> GetAccountByNumberAsync(string accountNumber);
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Account> CreateAccountAsync(Account account);
        Task<Account> UpdateAccountAsync(Account account);
        Task<bool> DeleteAccountAsync(int id);
        Task<bool> AccountExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<Account>> SearchAccountsAsync(string searchTerm);
        Task<IEnumerable<Account>> GetAccountsByTypeAsync(string accountType);
        Task<IEnumerable<Account>> GetAccountsByStatusAsync(string status);
        Task<(IEnumerable<Account> accounts, int totalCount)> GetPagedAccountsAsync(int page, int pageSize, string? searchTerm = null, string? accountType = null, string? status = null);
        Task<decimal> GetTotalBalanceAsync();
        Task<int> GetAccountsCountAsync();
        Task<int> GetActiveAccountsCountAsync();
        Task<Dictionary<string, int>> GetAccountTypeStatisticsAsync();
    }
}

