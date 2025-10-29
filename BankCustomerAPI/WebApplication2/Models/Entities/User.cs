using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models.Entities
{
    public abstract class User
    {
        public int Id { get; set; }
        
        [Required, StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }
        
        [Required, StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }
        
        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; }
        
        [Required, RegularExpression(@"^(\+91|91|0)?[6-9]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number"), StringLength(15)]
        public string PhoneNumber { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required, StringLength(500, MinimumLength = 10)]
        public string Address { get; set; }
        
        [Required, StringLength(20, MinimumLength = 5)]
        public string IdentificationNumber { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        [Required]
        public DateTime UpdatedAt { get; set; }
        
        public bool IsActive { get; set; }
        
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
    
    public class NormalUser : User
    {
        public bool IsMinor => DateTime.Now.Year - DateOfBirth.Year < 18;
        
        public virtual ICollection<GuardianRelationship> AsGuardian { get; set; } = new List<GuardianRelationship>();
        public virtual ICollection<GuardianRelationship> AsMinor { get; set; } = new List<GuardianRelationship>();
    }
    
    public class ViewOnlyUser : User
    {
        // ViewOnly users have read-only access to their own data
        // No additional properties needed - access controlled via roles
    }
    
    public class Admin : User
    {
        [StringLength(100)]
        public string? Department { get; set; }
        
        [StringLength(100)]
        public string? Position { get; set; }
        
        [Range(1, 50)]
        public int? YearsOfExperience { get; set; }
        
        // Admin can manage users and basic system operations
        public DateTime? LastAdminActionAt { get; set; }
    }
    
    public class SYSAdmin : User
    {
        [StringLength(100)]
        public string? Department { get; set; }
        
        [StringLength(100)]
        public string? Position { get; set; }
        
        [Range(1, 50)]
        public int? YearsOfExperience { get; set; }
        
        // SYSAdmin has full system access and can manage everything
        public DateTime? LastSystemActionAt { get; set; }
        
        [StringLength(200)]
        public string? AdminLevel { get; set; } // Super, Senior, Junior
    }
}
