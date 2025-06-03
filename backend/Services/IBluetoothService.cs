using SmartNameplate.Api.DTOs;

namespace SmartNameplate.Api.Services;

public interface IBluetoothService
{
    Task<IEnumerable<BluetoothDeviceDto>> ScanForDevicesAsync(CancellationToken cancellationToken = default);
    Task<bool> IsBluetoothAvailableAsync();
} 