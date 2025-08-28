using System.ComponentModel.DataAnnotations;

namespace BankManagement.API.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public decimal Balance { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TransactionCount { get; set; }
        public int CardCount { get; set; }
        public int LoanCount { get; set; }
    }

    public class CreateAccountDto
    {
        [Required(ErrorMessage = "اسم المالك مطلوب")]
        [StringLength(100, ErrorMessage = "اسم المالك يجب أن يكون أقل من 100 حرف")]
        public string OwnerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        [StringLength(120, ErrorMessage = "البريد الإلكتروني يجب أن يكون أقل من 120 حرف")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [StringLength(20, ErrorMessage = "رقم الهاتف يجب أن يكون أقل من 20 حرف")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "نوع الحساب مطلوب")]
        [RegularExpression("^(Savings|Checking|Business)$", ErrorMessage = "نوع الحساب يجب أن يكون: Savings, Checking, أو Business")]
        public string AccountType { get; set; } = "Savings";

        [Range(0, double.MaxValue, ErrorMessage = "الرصيد الأولي يجب أن يكون أكبر من أو يساوي صفر")]
        public decimal InitialBalance { get; set; } = 0;

        [StringLength(500, ErrorMessage = "الملاحظات يجب أن تكون أقل من 500 حرف")]
        public string? Notes { get; set; }
    }

    public class UpdateAccountDto
    {
        [Required(ErrorMessage = "اسم المالك مطلوب")]
        [StringLength(100, ErrorMessage = "اسم المالك يجب أن يكون أقل من 100 حرف")]
        public string OwnerName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [StringLength(20, ErrorMessage = "رقم الهاتف يجب أن يكون أقل من 20 حرف")]
        public string? PhoneNumber { get; set; }

        [RegularExpression("^(Active|Suspended|Closed)$", ErrorMessage = "حالة الحساب يجب أن تكون: Active, Suspended, أو Closed")]
        public string? Status { get; set; }

        [StringLength(500, ErrorMessage = "الملاحظات يجب أن تكون أقل من 500 حرف")]
        public string? Notes { get; set; }
    }

    public class AccountSearchDto
    {
        public string? SearchTerm { get; set; }
        public string? AccountType { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortDirection { get; set; } = "desc";
    }

    public class AccountStatisticsDto
    {
        public int TotalAccounts { get; set; }
        public int ActiveAccounts { get; set; }
        public int SuspendedAccounts { get; set; }
        public int ClosedAccounts { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal AverageBalance { get; set; }
        public Dictionary<string, int> AccountTypeDistribution { get; set; } = new();
        public Dictionary<string, decimal> AccountTypeBalances { get; set; } = new();
        public int NewAccountsThisMonth { get; set; }
        public decimal TotalDepositsThisMonth { get; set; }
        public decimal TotalWithdrawalsThisMonth { get; set; }
    }
}

