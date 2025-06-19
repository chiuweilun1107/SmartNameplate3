using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.Entities;

public enum CardStatus
{
    Draft,
    Active,
    Inactive
}

public class Card
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public CardStatus Status { get; set; } = CardStatus.Draft;

    public string? ThumbnailA { get; set; }
    
    public string? ThumbnailB { get; set; }

    public string? ContentA { get; set; }

    public string? ContentB { get; set; }

    /// <summary>
    /// 標記 A、B 面是否相同 (true = side 0, false = side 1/2)
    /// </summary>
    public bool IsSameBothSides { get; set; } = false;

    /// <summary>
    /// 文字元素標籤資訊 (姓名、職稱、電話、地址、公司等)
    /// </summary>
    [StringLength(500)]
    public string? Tags { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}