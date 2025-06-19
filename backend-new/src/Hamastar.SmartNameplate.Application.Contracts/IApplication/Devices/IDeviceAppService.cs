//-----
// <copyright file="IDeviceAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Devices.Request;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Devices.Response;

namespace Hamastar.SmartNameplate.Application.Contracts.IApplication.Devices
{
    /// <summary>
    /// 🤖 裝置應用服務介面
    /// </summary>
    public interface IDeviceAppService : IApplicationService
    {
        /// <summary>
        /// 獲取裝置分頁列表
        /// </summary>
        Task<DeviceListResponse> GetDeviceListAsync(DeviceListRequest request);
    }
} 