using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.Entities;

public class Template
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? ThumbnailA { get; set; }  // A面縮圖

    public string? ThumbnailB { get; set; }  // B面縮圖

    public string? LayoutDataA { get; set; }

    public string? LayoutDataB { get; set; }

    public string? Dimensions { get; set; }

    public int? OrganizationId { get; set; }

    public int? CreatedBy { get; set; }

    public bool IsPublic { get; set; } = false;

    [StringLength(100)]
    public string Category { get; set; } = "general";

    /// <summary>
    /// 文字元素標籤資訊 (姓名、職稱、電話、地址、公司等)
    /// </summary>
    [StringLength(500)]
    public string? Tags { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User? Creator { get; set; }
}
