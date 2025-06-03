using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.Entities;

public class GroupCard
{
    public int Id { get; set; }

    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public int CardId { get; set; }
    public Card Card { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
} 