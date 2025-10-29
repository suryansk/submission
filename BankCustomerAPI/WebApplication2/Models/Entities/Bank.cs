using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models.Entities
{
    public class Bank
    {
        public int Id { get; set; }
        
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }
        
        [Required, StringLength(20, MinimumLength = 2)]
        public string BankCode { get; set; }
        
        [Required, StringLength(11, MinimumLength = 8)]
        public string SwiftCode { get; set; }
        
        [Required, StringLength(500, MinimumLength = 10)]
        public string HeadOfficeAddress { get; set; }
        
        [Required, RegularExpression(@"^(\+91|91|0)?[6-9]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number"), StringLength(15)]
        public string ContactNumber { get; set; }
        
        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; }
        
        public bool IsActive { get; set; }
        
        [Required]
        public DateTime EstablishedDate { get; set; }
        
        public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
    
    public class Branch
    {
        public int Id { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int BankId { get; set; }
        
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }
        
        [Required, StringLength(20, MinimumLength = 3)]
        public string BranchCode { get; set; }
        
        [Required, StringLength(500, MinimumLength = 10)]
        public string Address { get; set; }
        
        [Required, StringLength(100, MinimumLength = 2)]
        public string City { get; set; }
        
        [Required, StringLength(100, MinimumLength = 2)]
        public string State { get; set; }
        
        [Required, RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "Please enter a valid 6-digit postal code"), StringLength(6, MinimumLength = 6)]
        public string PostalCode { get; set; }
        
        [Required, RegularExpression(@"^(\+91|91|0)?[6-9]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number"), StringLength(15)]
        public string ContactNumber { get; set; }
        
        public bool IsActive { get; set; }
        
        public virtual Bank Bank { get; set; }
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
