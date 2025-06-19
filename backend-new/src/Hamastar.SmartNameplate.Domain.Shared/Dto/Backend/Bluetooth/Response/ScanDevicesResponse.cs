//-----
// <copyright file="ScanDevicesResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Response;

/// <summary>
/// 🤖 掃描裝置回應 DTO
/// 用於藍牙裝置掃描操作的回應結果
/// </summary>
public class ScanDevicesResponse : BusinessLogicResponse
{
    #region Properties

    /// <summary>
    /// 是否成功
    /// </summary>
    [JsonProperty("success")]
    public bool Success { get; set; }

    /// <summary>
    /// 發現的裝置列表
    /// </summary>
    [JsonProperty("devices")]
    public List<BluetoothDeviceItem> Devices { get; set; } = new();

    /// <summary>
    /// 掃描開始時間
    /// </summary>
    [JsonProperty("scanStartTime")]
    public DateTime ScanStartTime { get; set; }

    /// <summary>
    /// 掃描結束時間
    /// </summary>
    [JsonProperty("scanEndTime")]
    public DateTime? ScanEndTime { get; set; }

    /// <summary>
    /// 掃描持續時間(秒)
    /// </summary>
    [JsonProperty("scanDuration")]
    public int ScanDuration { get; set; }

    /// <summary>
    /// 掃描結果
    /// </summary>
    [JsonProperty("result")]
    public bool Result { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; } = "";

    #endregion Properties
} 