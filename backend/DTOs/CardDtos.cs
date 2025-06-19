using System.ComponentModel.DataAnnotations;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.DTOs;

public class CreateCardDto
{
    [Required]
    [StringLength(100, ErrorMessage = "桌牌名稱不能超過 100 個字符")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "描述不能超過 500 個字符")]
    public string? Description { get; set; }

    public CardStatus Status { get; set; } = CardStatus.Draft;

    public string? ThumbnailA { get; set; }
    
    public string? ThumbnailB { get; set; }

    public string? ContentA { get; set; }

    public string? ContentB { get; set; }

    public bool IsSameBothSides { get; set; } = false;
}

public class UpdateCardDto
{
    [StringLength(100, ErrorMessage = "桌牌名稱不能超過 100 個字符")]
    public string? Name { get; set; }

    [StringLength(500, ErrorMessage = "描述不能超過 500 個字符")]
    public string? Description { get; set; }

    public CardStatus? Status { get; set; }

    public string? ThumbnailA { get; set; }
    
    public string? ThumbnailB { get; set; }

    public string? ContentA { get; set; }

    public string? ContentB { get; set; }

    public bool? IsSameBothSides { get; set; }
}

public class CardResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CardStatus Status { get; set; }
    public string? ThumbnailA { get; set; }
    public string? ThumbnailB { get; set; }
    public string? ContentA { get; set; }
    public string? ContentB { get; set; }
    public bool IsSameBothSides { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}