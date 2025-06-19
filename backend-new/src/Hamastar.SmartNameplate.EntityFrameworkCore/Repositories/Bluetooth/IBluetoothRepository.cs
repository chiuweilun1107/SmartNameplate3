//-----
// <copyright file="IBluetoothRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Hamastar.SmartNameplate.Entities;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Response;

namespace Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Bluetooth;

/// <summary>
/// 🤖 藍牙 Repository 介面
/// 處理藍牙裝置資料存取邏輯
/// </summary>
public interface IBluetoothRepository : IRepository<Device, Guid>
{
    /// <summary>
    /// 掃描藍牙裝置
    /// </summary>
    Task<ScanDevicesResponse> ScanDevicesAsync();

    /// <summary>
    /// 連接藍牙裝置
    /// </summary>
    Task<ConnectDeviceResponse> ConnectDeviceAsync(string bluetoothAddress);

    /// <summary>
    /// 獲取藍牙裝置列表
    /// </summary>
    Task<List<BluetoothDeviceItem>> GetBluetoothDevicesAsync();
} 