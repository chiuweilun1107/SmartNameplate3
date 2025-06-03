using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.Entities;

public class BackgroundImage
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    public string? ThumbnailUrl { get; set; }

    [StringLength(50)]
    public string? Category { get; set; } = "general";

    public bool IsPublic { get; set; } = true;

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User? Creator { get; set; }
}
