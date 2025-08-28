using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManagement.API.Models
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string LoanNumber { get; set; } = string.Empty;

        [Required]
        public int AccountId { get; set; }

        [Required]
        [StringLength(50)]
        public string LoanType { get; set; } = string.Empty; // Personal, Home, Car, Business

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrincipalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OutstandingAmount { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }

        public int TermInMonths { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyPayment { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Paid, Defaulted, Cancelled

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }

        [StringLength(500)]
        public string? Purpose { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<LoanPayment> Payments { get; set; } = new List<LoanPayment>();

        // Methods
        public string GenerateLoanNumber()
        {
            var random = new Random();
            return $"LOAN{random.Next(100000, 999999)}";
        }

        public decimal CalculateMonthlyPayment()
        {
            if (InterestRate == 0) return PrincipalAmount / TermInMonths;
            
            var monthlyRate = (double)(InterestRate / 100) / 12;
            var numPayments = TermInMonths;
            
            var monthlyPayment = (decimal)(((double)PrincipalAmount * monthlyRate * Math.Pow(1 + monthlyRate, numPayments)) / 
                                          (Math.Pow(1 + monthlyRate, numPayments) - 1));
            
            return Math.Round(monthlyPayment, 2);
        }

        public decimal CalculateTotalInterest()
        {
            return (MonthlyPayment * TermInMonths) - PrincipalAmount;
        }

        public bool IsOverdue()
        {
            return NextPaymentDate.HasValue && DateTime.UtcNow > NextPaymentDate.Value && Status == "Active";
        }

        public int GetRemainingPayments()
        {
            var paidPayments = Payments.Count(p => p.Status == "Completed");
            return Math.Max(0, TermInMonths - paidPayments);
        }
    }

    public class LoanPayment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LoanId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrincipalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal InterestAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Completed"; // Completed, Failed, Pending

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        // Navigation property
        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; } = null!;
    }
}

