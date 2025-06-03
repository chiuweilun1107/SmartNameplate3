using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.Entities;

public enum DeployStatus
{
    Pending,
    Success,
    Failed,
    Cancelled
}

public class DeployHistory
{
    public int Id { get; set; }

    [Required]
    public int DeviceId { get; set; }

    [Required]
    public int CardId { get; set; }

    [Required]
    public DeployStatus Status { get; set; } = DeployStatus.Pending;

    public DateTime CreatedAt { get; set; }

    public DateTime? DeployedAt { get; set; }

    public DateTime? ScheduledAt { get; set; }

    [StringLength(500)]
    public string? ErrorMessage { get; set; }

    [StringLength(100)]
    public string? DeployedBy { get; set; }

    public bool IsScheduled { get; set; } = false;

    public int RetryCount { get; set; } = 0;

    // Navigation properties
    public virtual Device Device { get; set; } = null!;
    public virtual Card Card { get; set; } = null!;
}
