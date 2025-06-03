namespace SmartNameplate.Api.DTOs;

public class DeviceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BluetoothAddress { get; set; } = string.Empty;
    public string? OriginalAddress { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? CurrentCardId { get; set; }
    public string? CurrentCardName { get; set; }
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
    public DateTime LastConnected { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CustomIndex { get; set; }
}

public class CreateDeviceDto
{
    public string Name { get; set; } = string.Empty;
    public string BluetoothAddress { get; set; } = string.Empty;
    public int? GroupId { get; set; }
}
