using Microsoft.EntityFrameworkCore;
using BankManagement.API.Models;

namespace BankManagement.API.Data
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Card> Cards { get; set; } = null!;
        public DbSet<Loan> Loans { get; set; } = null!;
        public DbSet<LoanPayment> LoanPayments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Account configuration
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.AccountNumber).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Balance).HasPrecision(18, 2);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Transaction configuration
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TransactionId).IsUnique();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.BalanceAfter).HasPrecision(18, 2);
                entity.Property(e => e.Fee).HasPrecision(18, 2);
                entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Account)
                    .WithMany(e => e.Transactions)
                    .HasForeignKey(e => e.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Card configuration
            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CardNumber).IsUnique();
                entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
                entity.Property(e => e.AvailableCredit).HasPrecision(18, 2);
                entity.Property(e => e.IssuedDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Account)
                    .WithMany(e => e.Cards)
                    .HasForeignKey(e => e.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Loan configuration
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.LoanNumber).IsUnique();
                entity.Property(e => e.PrincipalAmount).HasPrecision(18, 2);
                entity.Property(e => e.OutstandingAmount).HasPrecision(18, 2);
                entity.Property(e => e.InterestRate).HasPrecision(5, 2);
                entity.Property(e => e.MonthlyPayment).HasPrecision(18, 2);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Account)
                    .WithMany(e => e.Loans)
                    .HasForeignKey(e => e.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // LoanPayment configuration
            modelBuilder.Entity<LoanPayment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.PrincipalAmount).HasPrecision(18, 2);
                entity.Property(e => e.InterestAmount).HasPrecision(18, 2);
                entity.Property(e => e.PaymentDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Loan)
                    .WithMany(e => e.Payments)
                    .HasForeignKey(e => e.LoanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed sample accounts
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    OwnerName = "أحمد محمد علي",
                    AccountNumber = "ACC123456789",
                    Email = "ahmed.ali@example.com",
                    PhoneNumber = "+966501234567",
                    Balance = 5000.00m,
                    AccountType = "Savings",
                    Status = "Active",
                    Notes = "حساب توفير أساسي",
                    CreatedAt = DateTime.UtcNow.AddMonths(-6),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new Account
                {
                    Id = 2,
                    OwnerName = "فاطمة أحمد السالم",
                    AccountNumber = "ACC987654321",
                    Email = "fatima.salem@example.com",
                    PhoneNumber = "+966507654321",
                    Balance = 12500.00m,
                    AccountType = "Checking",
                    Status = "Active",
                    Notes = "حساب جاري للمعاملات اليومية",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Account
                {
                    Id = 3,
                    OwnerName = "شركة التقنية المتقدمة",
                    AccountNumber = "ACC555666777",
                    Email = "info@techadvanced.com",
                    PhoneNumber = "+966112345678",
                    Balance = 75000.00m,
                    AccountType = "Business",
                    Status = "Active",
                    Notes = "حساب تجاري للشركة",
                    CreatedAt = DateTime.UtcNow.AddMonths(-12),
                    UpdatedAt = DateTime.UtcNow.AddMonths(-12)
                }
            );

            // Seed sample transactions
            modelBuilder.Entity<Transaction>().HasData(
                new Transaction
                {
                    Id = 1,
                    TransactionId = "TXN001234567890",
                    AccountId = 1,
                    TransactionType = "Deposit",
                    Amount = 1000.00m,
                    BalanceAfter = 6000.00m,
                    Description = "إيداع نقدي",
                    Status = "Completed",
                    Fee = 0,
                    Timestamp = DateTime.UtcNow.AddDays(-5)
                },
                new Transaction
                {
                    Id = 2,
                    TransactionId = "TXN001234567891",
                    AccountId = 1,
                    TransactionType = "Withdrawal",
                    Amount = 500.00m,
                    BalanceAfter = 5500.00m,
                    Description = "سحب نقدي من الصراف",
                    Status = "Completed",
                    Fee = 5.00m,
                    Timestamp = DateTime.UtcNow.AddDays(-3)
                },
                new Transaction
                {
                    Id = 3,
                    TransactionId = "TXN001234567892",
                    AccountId = 2,
                    TransactionType = "Transfer",
                    Amount = 2000.00m,
                    BalanceAfter = 14500.00m,
                    Description = "تحويل من حساب خارجي",
                    RecipientAccount = "ACC123456789",
                    RecipientName = "أحمد محمد علي",
                    Status = "Completed",
                    Fee = 2.00m,
                    Timestamp = DateTime.UtcNow.AddDays(-2)
                }
            );
        }
    }
}

