using BankManagement.API.DTOs;
using BankManagement.API.Models;

namespace BankManagement.API.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResult<IEnumerable<AccountDto>>> GetAllAccountsAsync();
        Task<ServiceResult<AccountDto>> GetAccountByIdAsync(int id);
        Task<ServiceResult<AccountDto>> GetAccountByNumberAsync(string accountNumber);
        Task<ServiceResult<AccountDto>> CreateAccountAsync(CreateAccountDto createAccountDto);
        Task<ServiceResult<AccountDto>> UpdateAccountAsync(int id, UpdateAccountDto updateAccountDto);
        Task<ServiceResult<bool>> DeleteAccountAsync(int id);
        Task<ServiceResult<bool>> SuspendAccountAsync(int id, string reason);
        Task<ServiceResult<bool>> ActivateAccountAsync(int id);
        Task<ServiceResult<bool>> CloseAccountAsync(int id, string reason);
        Task<ServiceResult<PagedResult<AccountDto>>> GetPagedAccountsAsync(AccountSearchDto searchDto);
        Task<ServiceResult<AccountStatisticsDto>> GetAccountStatisticsAsync();
        Task<ServiceResult<bool>> ValidateAccountAsync(int id);
        Task<ServiceResult<decimal>> GetAccountBalanceAsync(int id);
        Task<ServiceResult<IEnumerable<AccountDto>>> SearchAccountsAsync(string searchTerm);
    }
}

