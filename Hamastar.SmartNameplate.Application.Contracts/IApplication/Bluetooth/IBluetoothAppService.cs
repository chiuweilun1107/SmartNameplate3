using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Response;

namespace Hamastar.SmartNameplate.Application.Contracts.IApplication.Bluetooth
{
    /// <summary>
    /// 🤖 藍牙應用服務介面
    /// </summary>
    public interface IBluetoothAppService : IApplicationService
    {
        /// <summary>
        /// 掃描設備
        /// </summary>
        Task<ScanDevicesResponse> ScanDevicesAsync();

        /// <summary>
        /// 連接設備
        /// </summary>
        Task<ConnectDeviceResponse> ConnectDeviceAsync(string bluetoothAddress);
    }
} 