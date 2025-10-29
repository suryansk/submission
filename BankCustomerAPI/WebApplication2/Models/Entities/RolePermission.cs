using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models.Entities
{
    public class Role
    {
        public int Id { get; set; }
        
        [Required, StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } // ACCOUNT_HOLDER, POA, GUARDIAN, BANK_MANAGER, etc.
        
        [StringLength(200)]
        public string Description { get; set; }
        
        public bool IsActive { get; set; }
        
        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
    
    public class Permission
    {
        public int Id { get; set; }
        
        [Required, StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } // CREATE_ACCOUNT, WITHDRAW_MONEY, etc.
        
        [StringLength(200)]
        public string Description { get; set; }
        
        [Required, StringLength(50, MinimumLength = 2)]
        public string Module { get; set; } // USER, ACCOUNT, TRANSACTION
        
        public bool IsActive { get; set; }
        
        // Navigation properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
    
    public class UserRole
    {
        public int Id { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int UserId { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int RoleId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int? BankId { get; set; } // Role specific to a bank
        [Range(1, int.MaxValue)]
        public int? AccountId { get; set; } // Role specific to an account
        
        [Required]
        public DateTime AssignedDate { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        
        public bool IsActive { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
        public virtual Bank Bank { get; set; }
        public virtual Account Account { get; set; }
    }
    
    public class RolePermission
    {
        public int Id { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int RoleId { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int PermissionId { get; set; }
        
        public bool IsActive { get; set; }
        
        // Navigation properties
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
