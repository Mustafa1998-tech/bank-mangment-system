using AutoMapper;
using BankManagement.API.DTOs;
using BankManagement.API.Interfaces;
using BankManagement.API.Models;

namespace BankManagement.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            IMapper mapper,
            ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResult<TransactionDto>> DepositAsync(int accountId, DepositDto depositDto)
        {
            try
            {
                var account = await _accountRepository.GetAccountByIdAsync(accountId);
                if (account == null)
                    return ServiceResult<TransactionDto>.Failure("الحساب غير موجود", 404);

                if (!account.CanDeposit(depositDto.Amount))
                    return ServiceResult<TransactionDto>.Failure("لا يمكن إجراء عملية الإيداع على هذا الحساب", 400);

                // Calculate fee (if any)
                var fee = await CalculateDepositFee(depositDto.Amount);

                // Create transaction
                var transaction = new Transaction
                {
                    AccountId = accountId,
                    TransactionType = "Deposit",
                    Amount = depositDto.Amount,
                    BalanceAfter = account.Balance + depositDto.Amount,
                    Description = depositDto.Description ?? "إيداع نقدي",
                    Reference = depositDto.Reference,
                    Fee = fee,
                    Status = "Completed"
                };

                // Update account balance
                account.Balance += depositDto.Amount;
                await _accountRepository.UpdateAccountAsync(account);

                // Save transaction
                var createdTransaction = await _transactionRepository.CreateTransactionAsync(transaction);
                var transactionDto = _mapper.Map<TransactionDto>(createdTransaction);

                _logger.LogInformation("Deposit completed successfully. Account: {AccountId}, Amount: {Amount}", 
                    accountId, depositDto.Amount);

                return ServiceResult<TransactionDto>.Success(transactionDto, "تم الإيداع بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during deposit. Account: {AccountId}, Amount: {Amount}", 
                    accountId, depositDto.Amount);
                return ServiceResult<TransactionDto>.Failure("حدث خطأ أثناء عملية الإيداع", 500);
            }
        }

        public async Task<ServiceResult<TransactionDto>> WithdrawAsync(int accountId, WithdrawDto withdrawDto)
        {
            try
            {
                var account = await _accountRepository.GetAccountByIdAsync(accountId);
                if (account == null)
                    return ServiceResult<TransactionDto>.Failure("الحساب غير موجود", 404);

                if (!account.CanWithdraw(withdrawDto.Amount))
                    return ServiceResult<TransactionDto>.Failure("لا يمكن إجراء عملية السحب - الرصيد غير كافي أو الحساب غير نشط", 400);

                // Calculate fee
                var fee = await CalculateWithdrawalFee(withdrawDto.Amount);
                var totalAmount = withdrawDto.Amount + fee;

                if (account.Balance < totalAmount)
                    return ServiceResult<TransactionDto>.Failure("الرصيد غير كافي لتغطية المبلغ والرسوم", 400);

                // Create transaction
                var transaction = new Transaction
                {
                    AccountId = accountId,
                    TransactionType = "Withdrawal",
                    Amount = withdrawDto.Amount,
                    BalanceAfter = account.Balance - totalAmount,
                    Description = withdrawDto.Description ?? "سحب نقدي",
                    Reference = withdrawDto.Reference,
                    Fee = fee,
                    Status = "Completed"
                };

                // Update account balance
                account.Balance -= totalAmount;
                await _accountRepository.UpdateAccountAsync(account);

                // Save transaction
                var createdTransaction = await _transactionRepository.CreateTransactionAsync(transaction);
                var transactionDto = _mapper.Map<TransactionDto>(createdTransaction);

                _logger.LogInformation("Withdrawal completed successfully. Account: {AccountId}, Amount: {Amount}", 
                    accountId, withdrawDto.Amount);

                return ServiceResult<TransactionDto>.Success(transactionDto, "تم السحب بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during withdrawal. Account: {AccountId}, Amount: {Amount}", 
                    accountId, withdrawDto.Amount);
                return ServiceResult<TransactionDto>.Failure("حدث خطأ أثناء عملية السحب", 500);
            }
        }

        public async Task<ServiceResult<TransferResultDto>> TransferAsync(int fromAccountId, TransferDto transferDto)
        {
            try
            {
                // Get source account
                var fromAccount = await _accountRepository.GetAccountByIdAsync(fromAccountId);
                if (fromAccount == null)
                    return ServiceResult<TransferResultDto>.Failure("الحساب المرسل غير موجود", 404);

                // Get destination account
                var toAccount = await _accountRepository.GetAccountByNumberAsync(transferDto.ToAccountNumber);
                if (toAccount == null)
                    return ServiceResult<TransferResultDto>.Failure("الحساب المستقبل غير موجود", 404);

                if (fromAccount.Id == toAccount.Id)
                    return ServiceResult<TransferResultDto>.Failure("لا يمكن التحويل إلى نفس الحساب", 400);

                if (!fromAccount.CanWithdraw(transferDto.Amount))
                    return ServiceResult<TransferResultDto>.Failure("لا يمكن إجراء التحويل من الحساب المرسل", 400);

                if (!toAccount.CanDeposit(transferDto.Amount))
                    return ServiceResult<TransferResultDto>.Failure("لا يمكن إجراء التحويل إلى الحساب المستقبل", 400);

                // Calculate fees
                var transferFee = await CalculateTransferFee(transferDto.Amount);
                var totalAmount = transferDto.Amount + transferFee;

                if (fromAccount.Balance < totalAmount)
                    return ServiceResult<TransferResultDto>.Failure("الرصيد غير كافي لتغطية المبلغ والرسوم", 400);

                // Create debit transaction (from account)
                var debitTransaction = new Transaction
                {
                    AccountId = fromAccount.Id,
                    TransactionType = "Transfer",
                    Amount = transferDto.Amount,
                    BalanceAfter = fromAccount.Balance - totalAmount,
                    Description = transferDto.Description ?? $"تحويل إلى {toAccount.AccountNumber}",
                    Reference = transferDto.Reference,
                    RecipientAccount = toAccount.AccountNumber,
                    RecipientName = toAccount.OwnerName,
                    Fee = transferFee,
                    Status = "Completed"
                };

                // Create credit transaction (to account)
                var creditTransaction = new Transaction
                {
                    AccountId = toAccount.Id,
                    TransactionType = "Transfer",
                    Amount = transferDto.Amount,
                    BalanceAfter = toAccount.Balance + transferDto.Amount,
                    Description = transferDto.Description ?? $"تحويل من {fromAccount.AccountNumber}",
                    Reference = transferDto.Reference,
                    RecipientAccount = fromAccount.AccountNumber,
                    RecipientName = fromAccount.OwnerName,
                    Fee = 0,
                    Status = "Completed"
                };

                // Update balances
                fromAccount.Balance -= totalAmount;
                toAccount.Balance += transferDto.Amount;

                await _accountRepository.UpdateAccountAsync(fromAccount);
                await _accountRepository.UpdateAccountAsync(toAccount);

                // Save transactions
                var createdDebitTransaction = await _transactionRepository.CreateTransactionAsync(debitTransaction);
                var createdCreditTransaction = await _transactionRepository.CreateTransactionAsync(creditTransaction);

                var result = new TransferResultDto
                {
                    FromTransaction = _mapper.Map<TransactionDto>(createdDebitTransaction),
                    ToTransaction = _mapper.Map<TransactionDto>(createdCreditTransaction),
                    FromAccountNewBalance = fromAccount.Balance,
                    ToAccountNewBalance = toAccount.Balance,
                    TotalFee = transferFee,
                    Message = "تم التحويل بنجاح"
                };

                _logger.LogInformation("Transfer completed successfully. From: {FromAccount}, To: {ToAccount}, Amount: {Amount}", 
                    fromAccount.AccountNumber, toAccount.AccountNumber, transferDto.Amount);

                return ServiceResult<TransferResultDto>.Success(result, "تم التحويل بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during transfer. From Account: {FromAccountId}, Amount: {Amount}", 
                    fromAccountId, transferDto.Amount);
                return ServiceResult<TransferResultDto>.Failure("حدث خطأ أثناء عملية التحويل", 500);
            }
        }

        public async Task<ServiceResult<TransactionDto>> GetTransactionByIdAsync(int id)
        {
            try
            {
                var transaction = await _transactionRepository.GetTransactionByIdAsync(id);
                if (transaction == null)
                    return ServiceResult<TransactionDto>.Failure("المعاملة غير موجودة", 404);

                var transactionDto = _mapper.Map<TransactionDto>(transaction);
                return ServiceResult<TransactionDto>.Success(transactionDto, "تم جلب المعاملة بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transaction by id: {TransactionId}", id);
                return ServiceResult<TransactionDto>.Failure("حدث خطأ أثناء جلب المعاملة", 500);
            }
        }

        public async Task<ServiceResult<TransactionDto>> GetTransactionByTransactionIdAsync(string transactionId)
        {
            try
            {
                var transaction = await _transactionRepository.GetTransactionByTransactionIdAsync(transactionId);
                if (transaction == null)
                    return ServiceResult<TransactionDto>.Failure("المعاملة غير موجودة", 404);

                var transactionDto = _mapper.Map<TransactionDto>(transaction);
                return ServiceResult<TransactionDto>.Success(transactionDto, "تم جلب المعاملة بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transaction by transaction id: {TransactionId}", transactionId);
                return ServiceResult<TransactionDto>.Failure("حدث خطأ أثناء جلب المعاملة", 500);
            }
        }

        public async Task<ServiceResult<IEnumerable<TransactionDto>>> GetTransactionsByAccountIdAsync(int accountId)
        {
            try
            {
                var transactions = await _transactionRepository.GetTransactionsByAccountIdAsync(accountId);
                var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);
                
                return ServiceResult<IEnumerable<TransactionDto>>.Success(transactionDtos, "تم جلب المعاملات بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transactions for account: {AccountId}", accountId);
                return ServiceResult<IEnumerable<TransactionDto>>.Failure("حدث خطأ أثناء جلب المعاملات", 500);
            }
        }

        public async Task<ServiceResult<PagedResult<TransactionDto>>> GetPagedTransactionsByAccountAsync(int accountId, TransactionSearchDto searchDto)
        {
            try
            {
                var (transactions, totalCount) = await _transactionRepository.GetPagedTransactionsByAccountAsync(
                    accountId, searchDto.Page, searchDto.PageSize, searchDto.TransactionType, 
                    searchDto.StartDate, searchDto.EndDate);

                var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);

                var pagedResult = new PagedResult<TransactionDto>
                {
                    Items = transactionDtos,
                    TotalCount = totalCount,
                    Page = searchDto.Page,
                    PageSize = searchDto.PageSize
                };

                return ServiceResult<PagedResult<TransactionDto>>.Success(pagedResult, "تم جلب المعاملات بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paged transactions for account: {AccountId}", accountId);
                return ServiceResult<PagedResult<TransactionDto>>.Failure("حدث خطأ أثناء جلب المعاملات", 500);
            }
        }

        public async Task<ServiceResult<IEnumerable<TransactionDto>>> GetRecentTransactionsAsync(int count = 10)
        {
            try
            {
                var transactions = await _transactionRepository.GetRecentTransactionsAsync(count);
                var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);
                
                return ServiceResult<IEnumerable<TransactionDto>>.Success(transactionDtos, "تم جلب المعاملات الأخيرة بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting recent transactions");
                return ServiceResult<IEnumerable<TransactionDto>>.Failure("حدث خطأ أثناء جلب المعاملات الأخيرة", 500);
            }
        }

        public async Task<ServiceResult<TransactionStatisticsDto>> GetTransactionStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var totalTransactions = await _transactionRepository.GetTransactionCountAsync(startDate, endDate);
                var totalAmount = await _transactionRepository.GetTotalTransactionAmountAsync(startDate, endDate);
                var typeStats = await _transactionRepository.GetTransactionTypeStatisticsAsync(startDate, endDate);

                var statistics = new TransactionStatisticsDto
                {
                    TotalTransactions = totalTransactions,
                    TotalAmount = totalAmount,
                    TransactionTypeAmount = typeStats,
                    AverageTransactionAmount = totalTransactions > 0 ? totalAmount / totalTransactions : 0
                };

                return ServiceResult<TransactionStatisticsDto>.Success(statistics, "تم جلب الإحصائيات بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transaction statistics");
                return ServiceResult<TransactionStatisticsDto>.Failure("حدث خطأ أثناء جلب الإحصائيات", 500);
            }
        }

        public async Task<ServiceResult<bool>> CancelTransactionAsync(int transactionId, string reason)
        {
            try
            {
                var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
                if (transaction == null)
                    return ServiceResult<bool>.Failure("المعاملة غير موجودة", 404);

                if (transaction.Status != "Pending")
                    return ServiceResult<bool>.Failure("لا يمكن إلغاء معاملة مكتملة", 400);

                transaction.Status = "Cancelled";
                transaction.Description = $"{transaction.Description} - تم الإلغاء: {reason}";

                await _transactionRepository.UpdateTransactionAsync(transaction);
                return ServiceResult<bool>.Success(true, "تم إلغاء المعاملة بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cancelling transaction: {TransactionId}", transactionId);
                return ServiceResult<bool>.Failure("حدث خطأ أثناء إلغاء المعاملة", 500);
            }
        }

        public async Task<ServiceResult<bool>> ValidateTransactionAsync(string transactionId)
        {
            try
            {
                var exists = await _transactionRepository.TransactionExistsAsync(transactionId);
                return ServiceResult<bool>.Success(exists, exists ? "المعاملة موجودة" : "المعاملة غير موجودة");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating transaction: {TransactionId}", transactionId);
                return ServiceResult<bool>.Failure("حدث خطأ أثناء التحقق من المعاملة", 500);
            }
        }

        public async Task<ServiceResult<decimal>> CalculateTransactionFeeAsync(string transactionType, decimal amount)
        {
            try
            {
                decimal fee = transactionType.ToLower() switch
                {
                    "deposit" => await CalculateDepositFee(amount),
                    "withdrawal" => await CalculateWithdrawalFee(amount),
                    "transfer" => await CalculateTransferFee(amount),
                    _ => 0
                };

                return ServiceResult<decimal>.Success(fee, "تم حساب الرسوم بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating transaction fee");
                return ServiceResult<decimal>.Failure("حدث خطأ أثناء حساب الرسوم", 500);
            }
        }

        public async Task<ServiceResult<IEnumerable<TransactionDto>>> GetPendingTransactionsAsync()
        {
            try
            {
                var transactions = await _transactionRepository.GetPendingTransactionsAsync();
                var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);
                
                return ServiceResult<IEnumerable<TransactionDto>>.Success(transactionDtos, "تم جلب المعاملات المعلقة بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting pending transactions");
                return ServiceResult<IEnumerable<TransactionDto>>.Failure("حدث خطأ أثناء جلب المعاملات المعلقة", 500);
            }
        }

        public async Task<ServiceResult<bool>> ProcessPendingTransactionAsync(int transactionId)
        {
            try
            {
                var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
                if (transaction == null)
                    return ServiceResult<bool>.Failure("المعاملة غير موجودة", 404);

                if (transaction.Status != "Pending")
                    return ServiceResult<bool>.Failure("المعاملة ليست معلقة", 400);

                transaction.Status = "Completed";
                await _transactionRepository.UpdateTransactionAsync(transaction);

                return ServiceResult<bool>.Success(true, "تم معالجة المعاملة بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing pending transaction: {TransactionId}", transactionId);
                return ServiceResult<bool>.Failure("حدث خطأ أثناء معالجة المعاملة", 500);
            }
        }

        // Private helper methods for fee calculation
        private async Task<decimal> CalculateDepositFee(decimal amount)
        {
            // Simple fee structure - can be made configurable
            return await Task.FromResult(0m); // No fee for deposits
        }

        private async Task<decimal> CalculateWithdrawalFee(decimal amount)
        {
            // Simple fee structure - can be made configurable
            if (amount <= 1000) return await Task.FromResult(5m);
            if (amount <= 5000) return await Task.FromResult(10m);
            return await Task.FromResult(amount * 0.002m); // 0.2% for large amounts
        }

        private async Task<decimal> CalculateTransferFee(decimal amount)
        {
            // Simple fee structure - can be made configurable
            if (amount <= 1000) return await Task.FromResult(2m);
            if (amount <= 10000) return await Task.FromResult(5m);
            return await Task.FromResult(amount * 0.001m); // 0.1% for large amounts
        }
    }
}

