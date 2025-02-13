using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccelokaAPI.Models
{
    public class Ticket
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CategoryId { get; set; }  // Foreign key

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }  // Navigation property
        
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }
        public decimal Price { get; set; }
        public int Quota { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "SYSTEM";
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; } = "SYSTEM";
    }
}
