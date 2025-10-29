using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models.Entities
{
    public abstract class Account
    {
        public int Id { get; set; }
        
        [Required, StringLength(20, MinimumLength = 10)]
        public string AccountNumber { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int UserId { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int BankId { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int BranchId { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int CurrencyId { get; set; }
        
        [Column(TypeName = "decimal(18,2)"), Range(0, 999999999999999.99)]
        public decimal Balance { get; set; }
        
        [Column(TypeName = "decimal(18,2)"), Range(0, 99999999.99)]
        public decimal MinimumBalance { get; set; }
        
        [Required]
        public DateTime OpenedDate { get; set; }
        
        public DateTime? ClosedDate { get; set; }
        
        public bool IsActive { get; set; }
        public bool IsNRIAccount { get; set; }
        
        public virtual User User { get; set; }
        public virtual Bank Bank { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Currency Currency { get; set; }
    }
    
    public class SavingAccount : Account
    {
        [Column(TypeName = "decimal(5,2)"), Range(0, 50)]
        public decimal InterestRate { get; set; }
        
        [Column(TypeName = "decimal(18,2)"), Range(0, 9999999.99)]
        public decimal WithdrawalLimit { get; set; }
        
        [Range(1, 50)]
        public int MaxWithdrawalsPerMonth { get; set; } = 6;
    }
    
    public class CurrentAccount : Account
    {
        [Column(TypeName = "decimal(18,2)"), Range(0, 99999999.99)]
        public decimal OverdraftLimit { get; set; }
        
        [Column(TypeName = "decimal(5,2)"), Range(0, 50)]
        public decimal OverdraftInterestRate { get; set; }
    }
    
    public class TermDepositAccount : SavingAccount
    {
        [Required]
        public DateTime MaturityDate { get; set; }
        
        [Required, Range(1, 120)]
        public int TermInMonths { get; set; }
        
        [Column(TypeName = "decimal(5,2)"), Range(0, 50)]
        public decimal MaturityInterestRate { get; set; }
        
        public bool IsAutoRenewal { get; set; }
        public bool IsMatured => DateTime.Now >= MaturityDate;
    }
}
