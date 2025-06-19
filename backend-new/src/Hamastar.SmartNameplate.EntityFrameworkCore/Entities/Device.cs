//-----
// <copyright file="Device.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Hamastar.SmartNameplate.Entities;

/// <summary>
/// 🤖 裝置狀態列舉 (對應原始 DeviceStatus)
/// </summary>
public enum DeviceStatus
{
    Connected = 0,
    Disconnected = 1,
    Syncing = 2,
    Error = 3
}

/// <summary>
/// 🤖 裝置實體 (對應原始 Device)
/// </summary>
[Table("Devices")]
[Comment("🤖 智慧名牌裝置資料表")]
public class Device : AuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 裝置名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    [Comment("裝置名稱")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 藍牙地址
    /// </summary>
    [Required]
    [StringLength(200)]
    [Comment("藍牙MAC地址")]
    public string BluetoothAddress { get; set; } = string.Empty;

    /// <summary>
    /// 原始的 BLE UUID 地址，用於實際的 BLE 連接
    /// </summary>
    [StringLength(200)]
    [Comment("原始 BLE UUID 地址")]
    public string? OriginalAddress { get; set; }

    /// <summary>
    /// 裝置狀態
    /// </summary>
    [Comment("裝置連接狀態")]
    public DeviceStatus Status { get; set; } = DeviceStatus.Disconnected;

    /// <summary>
    /// 當前卡片 ID
    /// </summary>
    [Comment("當前部署的卡片ID")]
    public Guid? CurrentCardId { get; set; }

    /// <summary>
    /// 群組 ID
    /// </summary>
    [Comment("所屬群組ID")]
    public Guid? GroupId { get; set; }

    /// <summary>
    /// 最後連線時間
    /// </summary>
    [Comment("最後連線時間")]
    public DateTime LastConnected { get; set; }

    /// <summary>
    /// 使用者自訂排序編號
    /// </summary>
    [Comment("使用者自訂排序編號")]
    public int? CustomIndex { get; set; }

    /// <summary>
    /// 裝置描述 (新增屬性)
    /// </summary>
    [StringLength(500)]
    [Comment("裝置描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 電池電量 (新增屬性)
    /// </summary>
    [Comment("電池電量百分比")]
    public int? BatteryLevel { get; set; }

    /// <summary>
    /// 是否啟用 (新增屬性)
    /// </summary>
    [Comment("是否啟用")]
    public bool Enable { get; set; } = true;

    // Navigation properties
    public virtual Card? CurrentCard { get; set; }
    public virtual Group? Group { get; set; }
    public virtual ICollection<DeployHistory> DeployHistories { get; set; } = new List<DeployHistory>();
} 