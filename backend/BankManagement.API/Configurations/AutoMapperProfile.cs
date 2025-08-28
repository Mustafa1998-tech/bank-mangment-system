using AutoMapper;
using BankManagement.API.DTOs;
using BankManagement.API.Models;

namespace BankManagement.API.Configurations
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Account mappings
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.Transactions.Count))
                .ForMember(dest => dest.CardCount, opt => opt.MapFrom(src => src.Cards.Count))
                .ForMember(dest => dest.LoanCount, opt => opt.MapFrom(src => src.Loans.Count));

            CreateMap<CreateAccountDto, Account>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.InitialBalance))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore())
                .ForMember(dest => dest.Cards, opt => opt.Ignore())
                .ForMember(dest => dest.Loans, opt => opt.Ignore());

            CreateMap<UpdateAccountDto, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Balance, opt => opt.Ignore())
                .ForMember(dest => dest.AccountType, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore())
                .ForMember(dest => dest.Cards, opt => opt.Ignore())
                .ForMember(dest => dest.Loans, opt => opt.Ignore());

            // Transaction mappings
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Account.AccountNumber))
                .ForMember(dest => dest.AccountOwnerName, opt => opt.MapFrom(src => src.Account.OwnerName));

            CreateMap<DepositDto, Transaction>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => "Deposit"))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .ForMember(dest => dest.BalanceAfter, opt => opt.Ignore())
                .ForMember(dest => dest.RecipientAccount, opt => opt.Ignore())
                .ForMember(dest => dest.RecipientName, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Fee, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.Account, opt => opt.Ignore());

            CreateMap<WithdrawDto, Transaction>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => "Withdrawal"))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .ForMember(dest => dest.BalanceAfter, opt => opt.Ignore())
                .ForMember(dest => dest.RecipientAccount, opt => opt.Ignore())
                .ForMember(dest => dest.RecipientName, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Fee, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.Account, opt => opt.Ignore());

            CreateMap<TransferDto, Transaction>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => "Transfer"))
                .ForMember(dest => dest.RecipientAccount, opt => opt.MapFrom(src => src.ToAccountNumber))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .ForMember(dest => dest.BalanceAfter, opt => opt.Ignore())
                .ForMember(dest => dest.RecipientName, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Fee, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.Account, opt => opt.Ignore());

            // Card mappings
            CreateMap<Card, CardDto>()
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Account.AccountNumber))
                .ForMember(dest => dest.AccountOwnerName, opt => opt.MapFrom(src => src.Account.OwnerName));

            // Loan mappings
            CreateMap<Loan, LoanDto>()
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Account.AccountNumber))
                .ForMember(dest => dest.AccountOwnerName, opt => opt.MapFrom(src => src.Account.OwnerName))
                .ForMember(dest => dest.RemainingPayments, opt => opt.MapFrom(src => src.GetRemainingPayments()))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue()))
                .ForMember(dest => dest.TotalInterest, opt => opt.MapFrom(src => src.CalculateTotalInterest()));

            CreateMap<LoanPayment, LoanPaymentDto>()
                .ForMember(dest => dest.LoanNumber, opt => opt.MapFrom(src => src.Loan.LoanNumber));
        }
    }

    // Additional DTOs for Cards and Loans
    public class CardDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountOwnerName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public decimal? CreditLimit { get; set; }
        public decimal AvailableCredit { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime? BlockedDate { get; set; }
        public string? BlockReason { get; set; }
    }

    public class LoanDto
    {
        public int Id { get; set; }
        public string LoanNumber { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountOwnerName { get; set; } = string.Empty;
        public string LoanType { get; set; } = string.Empty;
        public decimal PrincipalAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TermInMonths { get; set; }
        public decimal MonthlyPayment { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public string? Purpose { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int RemainingPayments { get; set; }
        public bool IsOverdue { get; set; }
        public decimal TotalInterest { get; set; }
    }

    public class LoanPaymentDto
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public string LoanNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
    }
}

