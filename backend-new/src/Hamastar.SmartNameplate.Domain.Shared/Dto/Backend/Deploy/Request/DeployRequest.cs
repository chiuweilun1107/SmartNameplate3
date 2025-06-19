//-----
// <copyright file="DeployRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System;
using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy.Request;

/// <summary>
/// 🤖 部署請求 DTO
/// 用於卡片部署到裝置的請求參數
/// </summary>
public class DeployRequest
{
    #region Properties

    /// <summary>
    /// 裝置 ID
    /// </summary>
    [JsonProperty("deviceId")]
    public Guid DeviceId { get; set; }

    /// <summary>
    /// 裝置名稱
    /// </summary>
    [JsonProperty("deviceName")]
    public string? DeviceName { get; set; }

    /// <summary>
    /// 卡片 ID
    /// </summary>
    [JsonProperty("cardId")]
    public Guid CardId { get; set; }

    /// <summary>
    /// 卡片名稱
    /// </summary>
    [JsonProperty("cardName")]
    public string? CardName { get; set; }

    /// <summary>
    /// 是否預定部署
    /// </summary>
    [JsonProperty("isScheduled")]
    public bool IsScheduled { get; set; } = false;

    /// <summary>
    /// 預定時間
    /// </summary>
    [JsonProperty("scheduledAt")]
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// 部署者
    /// </summary>
    [JsonProperty("deployedBy")]
    public string? DeployedBy { get; set; }

    #endregion Properties
} 