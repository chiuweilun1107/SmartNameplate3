namespace SmartNameplate.Api.DTOs;

public class BluetoothDeviceDto
{
    public string Name { get; set; } = string.Empty;
    public string BluetoothAddress { get; set; } = string.Empty;
    public string? OriginalAddress { get; set; }
    public int SignalStrength { get; set; }
    public bool IsConnected { get; set; }
    public string DeviceType { get; set; } = string.Empty;
}

public class ConnectDeviceDto
{
    public string Name { get; set; } = string.Empty;
    public string BluetoothAddress { get; set; } = string.Empty;
    public string? OriginalAddress { get; set; }
}

public class UpdateDeviceDto
{
    public string Name { get; set; } = string.Empty;
    public int? GroupId { get; set; }
    public int? CustomIndex { get; set; }
}

public class DeployCardDto
{
    public int CardId { get; set; }
    public int Side { get; set; } = 2; // 預設投圖到 B 面
} 