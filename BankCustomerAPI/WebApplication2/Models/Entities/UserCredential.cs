using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models.Entities
{
    public class UserCredential
    {
        public int Id { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int UserId { get; set; }
        
        [Required, StringLength(500)]
        public string PasswordHash { get; set; }
        
        [Required, StringLength(200)]
        public string PasswordSalt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        public bool IsLocked { get; set; }
        
        public virtual User User { get; set; }
    }
}
