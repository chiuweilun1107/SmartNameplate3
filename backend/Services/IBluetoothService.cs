using SmartNameplate.Api.DTOs;

namespace SmartNameplate.Api.Services;

public interface IBluetoothService
{
    Task<IEnumerable<BluetoothDeviceDto>> ScanForDevicesAsync(CancellationToken cancellationToken = default);
    Task<bool> IsBluetoothAvailableAsync();
    Task<bool> CheckDeviceConnectionAsync(string bluetoothAddress);
    Task<IEnumerable<string>> GetConnectedDeviceAddressesAsync();
    Task<bool> IsDeviceReachableAsync(string bluetoothAddress);
} 