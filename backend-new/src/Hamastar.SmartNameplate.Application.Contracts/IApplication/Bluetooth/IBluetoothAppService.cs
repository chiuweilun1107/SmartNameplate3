//-----
// <copyright file="IBluetoothAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Request;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Response;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

namespace Hamastar.SmartNameplate.Application.Contracts.IApplication.Bluetooth
{
    /// <summary>
    /// 🤖 藍牙應用服務介面
    /// </summary>
    public interface IBluetoothAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 掃描：藍牙裝置
        /// </summary>
        /// <returns> 掃描結果 </returns>
        Task<BusinessLogicResponse> ScanDevices();

        /// <summary>
        /// 連接：藍牙裝置
        /// </summary>
        /// <param name="connectRequest"> 連接請求 </param>
        /// <returns> 連接結果 </returns>
        Task<BusinessLogicResponse> ConnectDevice(ConnectDeviceRequest connectRequest);

        /// <summary>
        /// 斷開：藍牙裝置
        /// </summary>
        /// <param name="disconnectRequest"> 斷開請求 </param>
        /// <returns> 斷開結果 </returns>
        Task<BusinessLogicResponse> DisconnectDevice(DisconnectDeviceRequest disconnectRequest);

        /// <summary>
        /// 部署：卡片到裝置
        /// </summary>
        /// <param name="deployRequest"> 部署請求 </param>
        /// <returns> 部署結果 </returns>
        Task<BusinessLogicResponse> DeployCard(DeployCardRequest deployRequest);

        /// <summary>
        /// 檢查：裝置連接狀態
        /// </summary>
        /// <param name="statusRequest"> 狀態查詢請求 </param>
        /// <returns> 連接狀態 </returns>
        Task<BusinessLogicResponse> CheckDeviceStatus(DeviceStatusRequest statusRequest);

        #endregion Methods
    }
}