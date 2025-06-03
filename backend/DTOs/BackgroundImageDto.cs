using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.DTOs;

public class CreateBackgroundImageDto
{
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
}

public class UpdateBackgroundImageDto
{
    [StringLength(255)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    public bool? IsPublic { get; set; }

    public bool? IsActive { get; set; }
}

public class BackgroundImageResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public bool IsPublic { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
