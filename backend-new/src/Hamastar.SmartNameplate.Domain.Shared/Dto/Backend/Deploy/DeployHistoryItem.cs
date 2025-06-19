//-----
// <copyright file="DeployHistoryItem.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System;
using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy;

/// <summary>
/// 🤖 部署歷史項目 DTO
/// 對應原始 DeployHistoryDto
/// </summary>
public class DeployHistoryItem
{
    /// <summary>
    /// ID
    /// </summary>
    [JsonProperty("id")]
    public Guid Id { get; set; }

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
    /// 狀態
    /// </summary>
    [JsonProperty("status")]
    public int Status { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [JsonProperty("createdAt")]
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 部署時間
    /// </summary>
    [JsonProperty("deployedAt")]
    public DateTime? DeployedAt { get; set; }

    /// <summary>
    /// 預定時間
    /// </summary>
    [JsonProperty("scheduledAt")]
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 部署者
    /// </summary>
    [JsonProperty("deployedBy")]
    public string? DeployedBy { get; set; }

    /// <summary>
    /// 部署開始時間
    /// </summary>
    [JsonProperty("deployTime")]
    public DateTime? DeployTime { get; set; }

    /// <summary>
    /// 部署完成時間
    /// </summary>
    [JsonProperty("completedTime")]
    public DateTime? CompletedTime { get; set; }
} 