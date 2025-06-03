using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.Entities;

public enum DeviceStatus
{
    Connected,
    Disconnected,
    Syncing,
    Error
}

public class Device
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string BluetoothAddress { get; set; } = string.Empty;

    /// <summary>
    /// 原始的 BLE UUID 地址，用於實際的 BLE 連接
    /// </summary>
    [StringLength(200)]
    public string? OriginalAddress { get; set; }

    public DeviceStatus Status { get; set; } = DeviceStatus.Disconnected;

    public int? CurrentCardId { get; set; }
    public Card? CurrentCard { get; set; }

    public int? GroupId { get; set; }
    public Group? Group { get; set; }

    public DateTime LastConnected { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 使用者自訂排序編號
    /// </summary>
    public int? CustomIndex { get; set; }

    // Navigation properties
    public virtual ICollection<DeployHistory> DeployHistories { get; set; } = new List<DeployHistory>();
}
