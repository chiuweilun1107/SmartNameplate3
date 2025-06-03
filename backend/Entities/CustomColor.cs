using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.Entities
{
    public class CustomColor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(7)] // HEX color format #RRGGBB
        public string ColorValue { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public bool IsPublic { get; set; } = true;
    }
} 