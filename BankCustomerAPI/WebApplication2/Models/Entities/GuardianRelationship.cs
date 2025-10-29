using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models.Entities
{
    public class GuardianRelationship
    {
        public int Id { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int GuardianUserId { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int MinorUserId { get; set; }
        
        [Required, StringLength(50, MinimumLength = 3)]
        public string RelationshipType { get; set; }
        
        [Required]
        public DateTime EstablishedDate { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        
        public bool IsActive { get; set; }
        
        public virtual NormalUser Guardian { get; set; }
        public virtual NormalUser Minor { get; set; }
    }
}
