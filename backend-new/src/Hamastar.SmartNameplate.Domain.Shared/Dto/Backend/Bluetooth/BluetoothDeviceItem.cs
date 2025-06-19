//-----
// <copyright file="BluetoothDeviceItem.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System;
using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth;

/// <summary>
/// 🤖 藍牙裝置項目 DTO
/// 對應原始 BluetoothDeviceDto
/// </summary>
public class BluetoothDeviceItem
{
    /// <summary>
    /// 裝置名稱
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 藍牙地址
    /// </summary>
    [JsonProperty("bluetoothAddress")]
    public string BluetoothAddress { get; set; } = string.Empty;

    /// <summary>
    /// 原始地址
    /// </summary>
    [JsonProperty("originalAddress")]
    public string? OriginalAddress { get; set; }

    /// <summary>
    /// 訊號強度
    /// </summary>
    [JsonProperty("signalStrength")]
    public int SignalStrength { get; set; }

    /// <summary>
    /// 是否已連接
    /// </summary>
    [JsonProperty("isConnected")]
    public bool IsConnected { get; set; }

    /// <summary>
    /// 裝置類型
    /// </summary>
    [JsonProperty("deviceType")]
    public string DeviceType { get; set; } = string.Empty;
} 