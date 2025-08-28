using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManagement.API.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        public int AccountId { get; set; }

        [Required]
        [StringLength(20)]
        public string TransactionType { get; set; } = string.Empty; // Deposit, Withdrawal, Transfer, Payment

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfter { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [StringLength(20)]
        public string? RecipientAccount { get; set; }

        [StringLength(100)]
        public string? RecipientName { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Completed"; // Pending, Completed, Failed, Cancelled

        [StringLength(100)]
        public string? Reference { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Fee { get; set; } = 0;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; } = null!;

        // Methods
        public string GenerateTransactionId()
        {
            return $"TXN{Guid.NewGuid().ToString("N")[..12].ToUpper()}";
        }

        public bool IsTransfer()
        {
            return TransactionType.Equals("Transfer", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsDebit()
        {
            return TransactionType.Equals("Withdrawal", StringComparison.OrdinalIgnoreCase) ||
                   TransactionType.Equals("Transfer", StringComparison.OrdinalIgnoreCase) ||
                   TransactionType.Equals("Payment", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsCredit()
        {
            return TransactionType.Equals("Deposit", StringComparison.OrdinalIgnoreCase);
        }
    }
}

