using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.Entities
{
    public class TextTag
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ElementId { get; set; } = string.Empty;
        
        [Required]
        public int CardId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string TagType { get; set; } = string.Empty; // name, title, phone, address, company, custom
        
        [MaxLength(200)]
        public string? CustomLabel { get; set; }
        
        [MaxLength(1000)]
        public string? Content { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // 導航屬性
        public virtual Card? Card { get; set; }
    }
} 