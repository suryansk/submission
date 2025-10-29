using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models.Entities
{
    public class Currency
    {
        public int Id { get; set; }
        
        [Required, StringLength(3, MinimumLength = 3), RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency code must be 3 uppercase letters")]
        public string Code { get; set; } // USD, EUR, INR
        
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }
        
        [Required, StringLength(5, MinimumLength = 1)]
        public string Symbol { get; set; } // $, €, ₹
        
        [Column(TypeName = "decimal(10,6)"), Range(0.000001, 999999)]
        public decimal ExchangeRateToINR { get; set; } // Base currency INR for Indian banking
        
        public bool IsActive { get; set; }
        
        [Required]
        public DateTime LastUpdated { get; set; }
        
        // Navigation properties
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
