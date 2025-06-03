using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.Entities;

public class Group
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public string? Color { get; set; } = "#007ACC";

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<GroupCard> GroupCards { get; set; } = new List<GroupCard>();
    public ICollection<Device> Devices { get; set; } = new List<Device>();
} 