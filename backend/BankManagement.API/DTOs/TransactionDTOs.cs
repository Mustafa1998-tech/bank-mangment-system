using System.ComponentModel.DataAnnotations;

namespace BankManagement.API.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountOwnerName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public string? RecipientAccount { get; set; }
        public string? RecipientName { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public decimal? Fee { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class DepositDto
    {
        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        public decimal Amount { get; set; }

        [StringLength(255, ErrorMessage = "الوصف يجب أن يكون أقل من 255 حرف")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "المرجع يجب أن يكون أقل من 100 حرف")]
        public string? Reference { get; set; }
    }

    public class WithdrawDto
    {
        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        public decimal Amount { get; set; }

        [StringLength(255, ErrorMessage = "الوصف يجب أن يكون أقل من 255 حرف")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "المرجع يجب أن يكون أقل من 100 حرف")]
        public string? Reference { get; set; }
    }

    public class TransferDto
    {
        [Required(ErrorMessage = "رقم الحساب المستقبل مطلوب")]
        [StringLength(20, ErrorMessage = "رقم الحساب يجب أن يكون أقل من 20 حرف")]
        public string ToAccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        public decimal Amount { get; set; }

        [StringLength(255, ErrorMessage = "الوصف يجب أن يكون أقل من 255 حرف")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "المرجع يجب أن يكون أقل من 100 حرف")]
        public string? Reference { get; set; }
    }

    public class TransferResultDto
    {
        public TransactionDto FromTransaction { get; set; } = null!;
        public TransactionDto ToTransaction { get; set; } = null!;
        public decimal FromAccountNewBalance { get; set; }
        public decimal ToAccountNewBalance { get; set; }
        public decimal TotalFee { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class TransactionSearchDto
    {
        public string? TransactionType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? Status { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "Timestamp";
        public string SortDirection { get; set; } = "desc";
    }

    public class TransactionStatisticsDto
    {
        public int TotalTransactions { get; set; }
        public decimal TotalAmount { get; set; }
        public int TodayTransactions { get; set; }
        public decimal TodayAmount { get; set; }
        public int ThisMonthTransactions { get; set; }
        public decimal ThisMonthAmount { get; set; }
        public Dictionary<string, int> TransactionTypeCount { get; set; } = new();
        public Dictionary<string, decimal> TransactionTypeAmount { get; set; } = new();
        public Dictionary<string, int> TransactionStatusCount { get; set; } = new();
        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }
        public decimal TotalTransfers { get; set; }
        public decimal TotalFees { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public List<DailyTransactionSummary> DailyTransactions { get; set; } = new();
    }

    public class DailyTransactionSummary
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }
}

