using AutoMapper;
using BankManagement.API.DTOs;
using BankManagement.API.Interfaces;
using BankManagement.API.Models;

namespace BankManagement.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper,
            ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResult<IEnumerable<AccountDto>>> GetAllAccountsAsync()
        {
            try
            {
                var accounts = await _accountRepository.GetAllAccountsAsync();
                var accountDtos = _mapper.Map<IEnumerable<AccountDto>>(accounts);
                
                return ServiceResult<IEnumerable<AccountDto>>.Success(accountDtos, "تم جلب الحسابات بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all accounts");
                return ServiceResult<IEnumerable<AccountDto>>.Failure("حدث خطأ أثناء جلب الحسابات", 500);
            }
        }

        public async Task<ServiceResult<AccountDto>> GetAccountByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ServiceResult<AccountDto>.Failure("معرف الحساب غير صحيح", 400);

                var account = await _accountRepository.GetAccountByIdAsync(id);
                if (account == null)
                    return ServiceResult<AccountDto>.Failure("الحساب غير موجود", 404);

                var accountDto = _mapper.Map<AccountDto>(account);
                return ServiceResult<AccountDto>.Success(accountDto, "تم جلب الحساب بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account by id: {AccountId}", id);
                return ServiceResult<AccountDto>.Failure("حدث خطأ أثناء جلب الحساب", 500);
            }
        }

        public async Task<ServiceResult<AccountDto>> GetAccountByNumberAsync(string accountNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountNumber))
                    return ServiceResult<AccountDto>.Failure("رقم الحساب مطلوب", 400);

                var account = await _accountRepository.GetAccountByNumberAsync(accountNumber);
                if (account == null)
                    return ServiceResult<AccountDto>.Failure("الحساب غير موجود", 404);

                var accountDto = _mapper.Map<AccountDto>(account);
                return ServiceResult<AccountDto>.Success(accountDto, "تم جلب الحساب بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account by number: {AccountNumber}", accountNumber);
                return ServiceResult<AccountDto>.Failure("حدث خطأ أثناء جلب الحساب", 500);
            }
        }

        public async Task<ServiceResult<AccountDto>> CreateAccountAsync(CreateAccountDto createAccountDto)
        {
            try
            {
                // Validate email uniqueness
                var existingAccount = await _accountRepository.GetAccountByEmailAsync(createAccountDto.Email);
                if (existingAccount != null)
                    return ServiceResult<AccountDto>.Failure("البريد الإلكتروني مستخدم بالفعل", 400);

                // Create new account
                var account = new Account
                {
                    OwnerName = createAccountDto.OwnerName,
                    Email = createAccountDto.Email,
                    PhoneNumber = createAccountDto.PhoneNumber,
                    AccountType = createAccountDto.AccountType,
                    Balance = createAccountDto.InitialBalance,
                    Notes = createAccountDto.Notes,
                    Status = "Active"
                };

                var createdAccount = await _accountRepository.CreateAccountAsync(account);

                // Create initial deposit transaction if balance > 0
                if (createAccountDto.InitialBalance > 0)
                {
                    var initialTransaction = new Transaction
                    {
                        AccountId = createdAccount.Id,
                        TransactionType = "Deposit",
                        Amount = createAccountDto.InitialBalance,
                        BalanceAfter = createAccountDto.InitialBalance,
                        Description = "رصيد أولي عند إنشاء الحساب",
                        Status = "Completed"
                    };

                    await _transactionRepository.CreateTransactionAsync(initialTransaction);
                }

                var accountDto = _mapper.Map<AccountDto>(createdAccount);
                return ServiceResult<AccountDto>.Success(accountDto, "تم إنشاء الحساب بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating account for: {OwnerName}", createAccountDto.OwnerName);
                return ServiceResult<AccountDto>.Failure("حدث خطأ أثناء إنشاء الحساب", 500);
            }
        }

        public async Task<ServiceResult<AccountDto>> UpdateAccountAsync(int id, UpdateAccountDto updateAccountDto)
        {
            try
            {
                var account = await _accountRepository.GetAccountByIdAsync(id);
                if (account == null)
                    return ServiceResult<AccountDto>.Failure("الحساب غير موجود", 404);

                // Update allowed fields
                account.OwnerName = updateAccountDto.OwnerName;
                account.PhoneNumber = updateAccountDto.PhoneNumber;
                account.Notes = updateAccountDto.Notes;

                if (!string.IsNullOrEmpty(updateAccountDto.Status))
                    account.Status = updateAccountDto.Status;

                var updatedAccount = await _accountRepository.UpdateAccountAsync(account);
                var accountDto = _mapper.Map<AccountDto>(updatedAccount);

                return ServiceResult<AccountDto>.Success(accountDto, "تم تحديث الحساب بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating account: {AccountId}", id);
                return ServiceResult<AccountDto>.Failure("حدث خطأ أثناء تحديث الحساب", 500);
            }
        }

        public async Task<ServiceResult<bool>> DeleteAccountAsync(int id)
        {
            try
            {
                var account = await _accountRepository.GetAccountByIdAsync(id);
                if (account == null)
                    return ServiceResult<bool>.Failure("الحساب غير موجود", 404);

                if (account.Balance > 0)
                    return ServiceResult<bool>.Failure("لا يمكن حذف حساب يحتوي على رصيد", 400);

                var deleted = await _accountRepository.DeleteAccountAsync(id);
                if (!deleted)
                    return ServiceResult<bool>.Failure("لا يمكن حذف الحساب - يحتوي على معاملات أو قروض نشطة", 400);

                return ServiceResult<bool>.Success(true, "تم حذف الحساب بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting account: {AccountId}", id);
                return ServiceResult<bool>.Failure("حدث خطأ أثناء حذف الحساب", 500);
            }
        }

        public async Task<ServiceResult<bool>> SuspendAccountAsync(int id, string reason)
        {
            try
            {
                var account = await _accountRepository.GetAccountByIdAsync(id);
                if (account == null)
                    return ServiceResult<bool>.Failure("الحساب غير موجود", 404);

                if (account.Status == "Suspended")
                    return ServiceResult<bool>.Failure("الحساب معلق بالفعل", 400);

                account.Status = "Suspended";
                account.Notes = $"{account.Notes}\nتم تعليق الحساب: {reason} - {DateTime.UtcNow:yyyy-MM-dd HH:mm}";

                await _accountRepository.UpdateAccountAsync(account);
                return ServiceResult<bool>.Success(true, "تم تعليق الحساب بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while suspending account: {AccountId}", id);
                return ServiceResult<bool>.Failure("حدث خطأ أثناء تعليق الحساب", 500);
            }
        }

        public async Task<ServiceResult<bool>> ActivateAccountAsync(int id)
        {
            try
            {
                var account = await _accountRepository.GetAccountByIdAsync(id);
                if (account == null)
                    return ServiceResult<bool>.Failure("الحساب غير موجود", 404);

                if (account.Status == "Active")
                    return ServiceResult<bool>.Failure("الحساب نشط بالفعل", 400);

                if (account.Status == "Closed")
                    return ServiceResult<bool>.Failure("لا يمكن تنشيط حساب مغلق", 400);

                account.Status = "Active";
                account.Notes = $"{account.Notes}\nتم تنشيط الحساب - {DateTime.UtcNow:yyyy-MM-dd HH:mm}";

                await _accountRepository.UpdateAccountAsync(account);
                return ServiceResult<bool>.Success(true, "تم تنشيط الحساب بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while activating account: {AccountId}", id);
                return ServiceResult<bool>.Failure("حدث خطأ أثناء تنشيط الحساب", 500);
            }
        }

        public async Task<ServiceResult<bool>> CloseAccountAsync(int id, string reason)
        {
            try
            {
                var account = await _accountRepository.GetAccountByIdAsync(id);
                if (account == null)
                    return ServiceResult<bool>.Failure("الحساب غير موجود", 404);

                if (account.Status == "Closed")
                    return ServiceResult<bool>.Failure("الحساب مغلق بالفعل", 400);

                if (account.Balance != 0)
                    return ServiceResult<bool>.Failure("لا يمكن إغلاق حساب يحتوي على رصيد", 400);

                account.Status = "Closed";
                account.Notes = $"{account.Notes}\nتم إغلاق الحساب: {reason} - {DateTime.UtcNow:yyyy-MM-dd HH:mm}";

                await _accountRepository.UpdateAccountAsync(account);
                return ServiceResult<bool>.Success(true, "تم إغلاق الحساب بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while closing account: {AccountId}", id);
                return ServiceResult<bool>.Failure("حدث خطأ أثناء إغلاق الحساب", 500);
            }
        }

        public async Task<ServiceResult<PagedResult<AccountDto>>> GetPagedAccountsAsync(AccountSearchDto searchDto)
        {
            try
            {
                var (accounts, totalCount) = await _accountRepository.GetPagedAccountsAsync(
                    searchDto.Page, searchDto.PageSize, searchDto.SearchTerm, 
                    searchDto.AccountType, searchDto.Status);

                var accountDtos = _mapper.Map<IEnumerable<AccountDto>>(accounts);

                var pagedResult = new PagedResult<AccountDto>
                {
                    Items = accountDtos,
                    TotalCount = totalCount,
                    Page = searchDto.Page,
                    PageSize = searchDto.PageSize
                };

                return ServiceResult<PagedResult<AccountDto>>.Success(pagedResult, "تم جلب الحسابات بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paged accounts");
                return ServiceResult<PagedResult<AccountDto>>.Failure("حدث خطأ أثناء جلب الحسابات", 500);
            }
        }

        public async Task<ServiceResult<AccountStatisticsDto>> GetAccountStatisticsAsync()
        {
            try
            {
                var totalAccounts = await _accountRepository.GetAccountsCountAsync();
                var activeAccounts = await _accountRepository.GetActiveAccountsCountAsync();
                var totalBalance = await _accountRepository.GetTotalBalanceAsync();
                var accountTypeStats = await _accountRepository.GetAccountTypeStatisticsAsync();

                var statistics = new AccountStatisticsDto
                {
                    TotalAccounts = totalAccounts,
                    ActiveAccounts = activeAccounts,
                    TotalBalance = totalBalance,
                    AverageBalance = totalAccounts > 0 ? totalBalance / totalAccounts : 0,
                    AccountTypeDistribution = accountTypeStats
                };

                return ServiceResult<AccountStatisticsDto>.Success(statistics, "تم جلب الإحصائيات بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account statistics");
                return ServiceResult<AccountStatisticsDto>.Failure("حدث خطأ أثناء جلب الإحصائيات", 500);
            }
        }

        public async Task<ServiceResult<bool>> ValidateAccountAsync(int id)
        {
            try
            {
                var exists = await _accountRepository.AccountExistsAsync(id);
                return ServiceResult<bool>.Success(exists, exists ? "الحساب موجود" : "الحساب غير موجود");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating account: {AccountId}", id);
                return ServiceResult<bool>.Failure("حدث خطأ أثناء التحقق من الحساب", 500);
            }
        }

        public async Task<ServiceResult<decimal>> GetAccountBalanceAsync(int id)
        {
            try
            {
                var account = await _accountRepository.GetAccountByIdAsync(id);
                if (account == null)
                    return ServiceResult<decimal>.Failure("الحساب غير موجود", 404);

                return ServiceResult<decimal>.Success(account.Balance, "تم جلب الرصيد بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account balance: {AccountId}", id);
                return ServiceResult<decimal>.Failure("حدث خطأ أثناء جلب الرصيد", 500);
            }
        }

        public async Task<ServiceResult<IEnumerable<AccountDto>>> SearchAccountsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return ServiceResult<IEnumerable<AccountDto>>.Failure("مصطلح البحث مطلوب", 400);

                var accounts = await _accountRepository.SearchAccountsAsync(searchTerm);
                var accountDtos = _mapper.Map<IEnumerable<AccountDto>>(accounts);

                return ServiceResult<IEnumerable<AccountDto>>.Success(accountDtos, "تم البحث بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching accounts with term: {SearchTerm}", searchTerm);
                return ServiceResult<IEnumerable<AccountDto>>.Failure("حدث خطأ أثناء البحث", 500);
            }
        }
    }
}

