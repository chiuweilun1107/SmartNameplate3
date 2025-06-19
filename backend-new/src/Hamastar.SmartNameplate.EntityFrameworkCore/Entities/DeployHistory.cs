//-----
// <copyright file="DeployHistory.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Hamastar.SmartNameplate.Entities;

/// <summary>
/// 🤖 部署狀態列舉 (對應原始 DeployStatus)
/// </summary>
public enum DeployStatus
{
    Pending = 0,
    Success = 1,
    Failed = 2,
    Cancelled = 3
}

/// <summary>
/// 🤖 部署歷史實體 (對應原始 DeployHistory)
/// 記錄卡片部署到裝置的歷史記錄
/// </summary>
[Table("DeployHistories")]
[Comment("🤖 部署歷史資料表")]
public class DeployHistory : AuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 裝置 ID
    /// </summary>
    [Required]
    [Comment("裝置 ID")]
    public Guid DeviceId { get; set; }

    /// <summary>
    /// 裝置名稱
    /// </summary>
    [StringLength(100)]
    [Comment("裝置名稱")]
    public string? DeviceName { get; set; }

    /// <summary>
    /// 卡片 ID
    /// </summary>
    [Required]
    [Comment("卡片 ID")]
    public Guid CardId { get; set; }

    /// <summary>
    /// 卡片名稱
    /// </summary>
    [StringLength(100)]
    [Comment("卡片名稱")]
    public string? CardName { get; set; }

    /// <summary>
    /// 部署狀態
    /// </summary>
    [Comment("部署狀態")]
    [Required]
    public DeployStatus Status { get; set; } = DeployStatus.Pending;

    /// <summary>
    /// 部署時間
    /// </summary>
    [Comment("實際部署時間")]
    public DateTime? DeployedAt { get; set; }

    /// <summary>
    /// 預定時間
    /// </summary>
    [Comment("預定部署時間")]
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    [StringLength(500)]
    [Comment("錯誤訊息")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 部署者
    /// </summary>
    [StringLength(100)]
    [Comment("部署執行者")]
    public string? DeployedBy { get; set; }

    /// <summary>
    /// 是否預定部署
    /// </summary>
    [Comment("是否為預定部署")]
    public bool IsScheduled { get; set; } = false;

    /// <summary>
    /// 重試次數
    /// </summary>
    [Comment("重試次數")]
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// 部署開始時間
    /// </summary>
    [Comment("部署開始時間")]
    public DateTime? DeployTime { get; set; }

    /// <summary>
    /// 部署完成時間
    /// </summary>
    [Comment("部署完成時間")]
    public DateTime? CompletedTime { get; set; }

    /// <summary>
    /// 裝置
    /// </summary>
    public virtual Device Device { get; set; } = null!;

    /// <summary>
    /// 卡片
    /// </summary>
    public virtual Card Card { get; set; } = null!;
} 