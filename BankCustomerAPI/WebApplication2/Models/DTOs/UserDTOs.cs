using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models.DTOs
{
    public class CreateUserRequest
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        
        [Required, MaxLength(100)]
        public string LastName { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required, Phone]
        public string PhoneNumber { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public string Address { get; set; }
        
        [Required]
        public string IdentificationNumber { get; set; }
        
        public string UserType { get; set; } = "Normal"; // Normal, Bank
        
        // For bank users
        public int? BankId { get; set; }
        public string EmployeeId { get; set; }
        public string Department { get; set; }
    }
    
    public class UserResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public bool IsMinor { get; set; }
        public bool IsActive { get; set; }
        public List<RoleResponse> Roles { get; set; } = new List<RoleResponse>();
    }
    
    public class RoleResponse
    {
        public string RoleName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
    
    public class UpdateUserRequest
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }
        
        [Required, StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }
        
        [Required, RegularExpression(@"^(\+91|91|0)?[6-9]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number")]
        public string PhoneNumber { get; set; }
        
        [Required, StringLength(500, MinimumLength = 10)]
        public string Address { get; set; }
    }
}
