using System.Text.Json;

namespace SmartNameplate.Api.DTOs;

public class CreateTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ThumbnailA { get; set; }   // A面縮圖
    public string? ThumbnailB { get; set; }   // B面縮圖
    public JsonElement LayoutDataA { get; set; }
    public JsonElement LayoutDataB { get; set; }
    public JsonElement Dimensions { get; set; }
    public bool IsPublic { get; set; } = false;
    public string Category { get; set; } = "general";
}

public class UpdateTemplateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ThumbnailA { get; set; }   // A面縮圖
    public string? ThumbnailB { get; set; }   // B面縮圖
    public JsonElement? LayoutDataA { get; set; }
    public JsonElement? LayoutDataB { get; set; }
    public JsonElement? Dimensions { get; set; }
    public bool? IsPublic { get; set; }
    public string? Category { get; set; }
    public bool? IsActive { get; set; }
}

public class TemplateResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ThumbnailA { get; set; }   // A面縮圖
    public string? ThumbnailB { get; set; }   // B面縮圖
    public JsonElement LayoutDataA { get; set; }
    public JsonElement LayoutDataB { get; set; }
    public JsonElement Dimensions { get; set; }
    public int? OrganizationId { get; set; }
    public int? CreatedBy { get; set; }
    public bool IsPublic { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class TemplateListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ThumbnailA { get; set; }   // A面縮圖
    public string? ThumbnailB { get; set; }   // B面縮圖
    public string Category { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
}
