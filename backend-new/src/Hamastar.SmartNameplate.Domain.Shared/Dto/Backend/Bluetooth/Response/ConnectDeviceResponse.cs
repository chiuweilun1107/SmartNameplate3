//-----
// <copyright file="ConnectDeviceResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using System;
using Newtonsoft.Json;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Response;

/// <summary>
/// 🤖 連接裝置回應 DTO
/// 用於藍牙裝置連接操作的回應結果
/// </summary>
public class ConnectDeviceResponse : BusinessLogicResponse
{
    #region Properties
    
    /// <summary>
    /// 是否成功
    /// </summary>
    [JsonProperty("success")]
    public bool Success { get; set; }

    /// <summary>
    /// 裝置地址
    /// </summary>
    [JsonProperty("deviceAddress")]
    public string DeviceAddress { get; set; } = string.Empty;

    /// <summary>
    /// 裝置名稱
    /// </summary>
    [JsonProperty("deviceName")]
    public string? DeviceName { get; set; }

    /// <summary>
    /// 連接時間
    /// </summary>
    [JsonProperty("connectedAt")]
    public DateTime ConnectedAt { get; set; }

    /// <summary>
    /// 連接狀態
    /// </summary>
    [JsonProperty("connectionStatus")]
    public string ConnectionStatus { get; set; } = string.Empty;

    #endregion
} 