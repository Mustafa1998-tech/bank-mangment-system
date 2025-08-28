using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManagement.API.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string OwnerName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(120)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        [Required]
        [StringLength(20)]
        public string AccountType { get; set; } = "Savings"; // Savings, Checking, Business

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Suspended, Closed

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

        // Methods
        public string GenerateAccountNumber()
        {
            var random = new Random();
            return $"ACC{random.Next(100000000, 999999999)}";
        }

        public bool CanWithdraw(decimal amount)
        {
            return Status == "Active" && Balance >= amount && amount > 0;
        }

        public bool CanDeposit(decimal amount)
        {
            return Status == "Active" && amount > 0;
        }
    }
}

