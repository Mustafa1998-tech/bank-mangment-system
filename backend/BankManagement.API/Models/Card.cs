using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManagement.API.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        [StringLength(19)]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CardHolderName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string CardType { get; set; } = string.Empty; // Debit, Credit, Prepaid

        [Required]
        [StringLength(7)]
        public string ExpiryDate { get; set; } = string.Empty; // MM/YYYY

        [Required]
        [StringLength(3)]
        public string CVV { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CreditLimit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AvailableCredit { get; set; } = 0;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Blocked, Expired, Cancelled

        public bool IsBlocked { get; set; } = false;

        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        public DateTime? BlockedDate { get; set; }

        [StringLength(255)]
        public string? BlockReason { get; set; }

        // Navigation property
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; } = null!;

        // Methods
        public string GenerateCardNumber()
        {
            var random = new Random();
            return $"4{random.Next(100, 999)}-{random.Next(1000, 9999)}-{random.Next(1000, 9999)}-{random.Next(1000, 9999)}";
        }

        public string GenerateCVV()
        {
            var random = new Random();
            return random.Next(100, 999).ToString();
        }

        public string GenerateExpiryDate()
        {
            var futureDate = DateTime.UtcNow.AddYears(4);
            return futureDate.ToString("MM/yyyy");
        }

        public bool IsExpired()
        {
            if (DateTime.TryParseExact(ExpiryDate, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime expiry))
            {
                return DateTime.UtcNow > expiry.AddMonths(1).AddDays(-1);
            }
            return true;
        }

        public bool CanUse()
        {
            return Status == "Active" && !IsBlocked && !IsExpired();
        }
    }
}

