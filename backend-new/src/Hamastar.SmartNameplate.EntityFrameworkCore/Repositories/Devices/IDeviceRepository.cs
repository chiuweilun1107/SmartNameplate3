//-----
// <copyright file="IDeviceRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Hamastar.SmartNameplate.Entities;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Devices;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Devices.Request;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Devices.Response;

namespace Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Devices;

/// <summary>
/// 🤖 裝置 Repository 介面
/// 處理裝置資料存取邏輯
/// </summary>
public interface IDeviceRepository : IRepository<Device, Guid>
{
    /// <summary>
    /// 獲取裝置分頁列表
    /// </summary>
    Task<DeviceListResponse> GetDeviceListAsync(DeviceListRequest request);

    /// <summary>
    /// 依據 ID 獲取裝置資訊
    /// </summary>
    Task<DeviceItem?> GetDeviceByIdAsync(Guid id);

    /// <summary>
    /// 依據名稱獲取裝置資訊
    /// </summary>
    Task<DeviceItem?> GetDeviceByNameAsync(string name);

    /// <summary>
    /// 查詢：裝置列表(頁數)
    /// </summary>
    /// <param name="request"> 查詢條件及頁數 </param>
    /// <returns> 結果及頁數資訊 </returns>
    Task<DeviceListResponse> GetListByPage(DeviceListRequest request);

    /// <summary>
    /// 查詢：單一裝置
    /// </summary>
    /// <param name="id"> 裝置ID </param>
    /// <returns> 裝置資料 </returns>
    Task<DeviceItem?> GetDeviceById(Guid id);

    /// <summary>
    /// 查詢：依藍牙地址取得裝置
    /// </summary>
    /// <param name="bluetoothAddress"> 藍牙地址 </param>
    /// <returns> 裝置資料 </returns>
    Task<DeviceItem?> GetDeviceByBluetoothAddress(string bluetoothAddress);

    /// <summary>
    /// 更新：裝置狀態
    /// </summary>
    /// <param name="deviceId"> 裝置ID </param>
    /// <param name="status"> 新狀態 </param>
    /// <returns> 更新結果 </returns>
    Task<bool> UpdateDeviceStatus(Guid deviceId, int status);

    /// <summary>
    /// 更新：裝置電池電量
    /// </summary>
    /// <param name="deviceId"> 裝置ID </param>
    /// <param name="batteryLevel"> 電池電量 </param>
    /// <returns> 更新結果 </returns>
    Task<bool> UpdateDeviceBatteryLevel(Guid deviceId, int batteryLevel);

    /// <summary>
    /// 更新：裝置最後連線時間
    /// </summary>
    /// <param name="deviceId"> 裝置ID </param>
    /// <returns> 更新結果 </returns>
    Task<bool> UpdateDeviceLastConnected(Guid deviceId);

    /// <summary>
    /// 查詢：根據藍牙地址取得裝置實體
    /// </summary>
    /// <param name="bluetoothAddress"> 藍牙地址 </param>
    /// <returns> 裝置實體 </returns>
    Task<Device?> GetDeviceByBluetoothAddressAsync(string bluetoothAddress);

    /// <summary>
    /// 查詢：根據群組取得裝置列表
    /// </summary>
    /// <param name="groupId"> 群組 ID </param>
    /// <returns> 裝置列表 </returns>
    Task<List<Device>> GetDevicesByGroupAsync(Guid groupId);
} 