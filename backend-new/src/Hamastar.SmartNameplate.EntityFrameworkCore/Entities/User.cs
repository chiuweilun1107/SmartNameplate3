//-----
// <copyright file="User.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Hamastar.SmartNameplate.Entities;

/// <summary>
/// 🤖 使用者實體 (對應原始 User)
/// </summary>
[Table("Users")]
[Comment("🤖 使用者資料表")]
public class User : AuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 使用者名稱
    /// </summary>
    [Required]
    [StringLength(50)]
    [Comment("使用者名稱")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密碼雜湊
    /// </summary>
    [Required]
    [StringLength(255)]
    [Comment("密碼雜湊值")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    [Required]
    [StringLength(100)]
    [Comment("電子郵件")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 顯示名稱
    /// </summary>
    [StringLength(100)]
    [Comment("顯示名稱")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// 電話號碼
    /// </summary>
    [StringLength(20)]
    [Comment("電話號碼")]
    public string? Phone { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Comment("是否啟用")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 最後登入時間
    /// </summary>
    [Comment("最後登入時間")]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 角色名稱
    /// </summary>
    [StringLength(50)]
    [Comment("角色名稱")]
    public string? Role { get; set; }

    // Navigation properties
    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
    public virtual ICollection<Template> Templates { get; set; } = new List<Template>();
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
} 