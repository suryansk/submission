using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models.DTOs
{
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }
    
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public UserInfo User { get; set; }
        public List<RoleInfo> Roles { get; set; }
    }
    
    public class UserInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public bool IsMinor { get; set; }
    }
    
    public class RoleInfo
    {
        public string RoleName { get; set; }
        public string BankName { get; set; }
        public int? BankId { get; set; }
        public string AccountNumber { get; set; }
        public List<string> Permissions { get; set; }
    }
    
    public class RegisterRequest
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }
        
        [Required, StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required, RegularExpression(@"^(\+91[-\s]?|91[-\s]?|0)?[6-9]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number (10 digits starting with 6-9, optionally prefixed with +91 or 91)")]
        public string PhoneNumber { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required, StringLength(500, MinimumLength = 10)]
        public string Address { get; set; }
        
        [Required, StringLength(20, MinimumLength = 5)]
        public string IdentificationNumber { get; set; }
        
        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }
        
        public string UserType { get; set; } = "NormalUser"; // NormalUser, ViewOnlyUser, Admin, SYSAdmin
        
        // Admin/SYSAdmin-specific fields (optional)
        public string? Department { get; set; }
        public string? Position { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? AdminLevel { get; set; } // Super, Senior, Junior (for SYSAdmin)
    }
}
