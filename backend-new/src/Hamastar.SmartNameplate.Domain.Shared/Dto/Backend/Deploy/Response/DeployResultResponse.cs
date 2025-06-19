//-----
// <copyright file="DeployResultResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy.Response;

/// <summary>
/// 🤖 部署結果回應 DTO
/// 用於部署操作的回應結果
/// </summary>
public class DeployResultResponse : BusinessLogicResponse
{
    #region Properties

    /// <summary>
    /// 部署 ID
    /// </summary>
    [JsonProperty("deployId")]
    public Guid DeployId { get; set; }

    /// <summary>
    /// 部署狀態
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 裝置 ID
    /// </summary>
    [JsonProperty("deviceId")]
    public Guid DeviceId { get; set; }

    /// <summary>
    /// 卡片 ID
    /// </summary>
    [JsonProperty("cardId")]
    public Guid CardId { get; set; }

    /// <summary>
    /// 部署時間
    /// </summary>
    [JsonProperty("deployTime")]
    public DateTime? DeployTime { get; set; }

    /// <summary>
    /// 完成時間
    /// </summary>
    [JsonProperty("completedTime")]
    public DateTime? CompletedTime { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    [JsonProperty("success")]
    public bool Success { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; } = "";

    /// <summary>
    /// 總裝置數
    /// </summary>
    [JsonProperty("totalDevices")]
    public int TotalDevices { get; set; }

    /// <summary>
    /// 成功部署數
    /// </summary>
    [JsonProperty("successfulDeploys")]
    public int SuccessfulDeploys { get; set; }

    /// <summary>
    /// 失敗部署數
    /// </summary>
    [JsonProperty("failedDeploys")]
    public int FailedDeploys { get; set; }

    /// <summary>
    /// 部署結果列表
    /// </summary>
    [JsonProperty("results")]
    public List<DeployItem> Results { get; set; } = new();

    #endregion Properties
} 