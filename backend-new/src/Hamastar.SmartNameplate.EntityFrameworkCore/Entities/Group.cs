//-----
// <copyright file="Group.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Hamastar.SmartNameplate.Entities;

/// <summary>
/// 群組實體
/// </summary>
[Table("AppGroups")]
[Comment("群組主表")]
public class Group : Volo.Abp.Domain.Entities.BasicAggregateRoot<Guid>
{
    #region Properties

    /// <summary>
    /// 群組 ID
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Comment("群組 ID")]
    public override Guid Id { get; protected set; }

    /// <summary>
    /// 群組名稱
    /// </summary>
    [MaxLength(100)]
    [Column("Name", TypeName = "nvarchar")]
    [Comment("群組名稱")]
    [Required]
    public string Name { get; set; } = "";

    /// <summary>
    /// 群組描述
    /// </summary>
    [MaxLength(500)]
    [Column("Description", TypeName = "nvarchar")]
    [Comment("群組描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 群組顏色
    /// </summary>
    [MaxLength(7)]
    [Column("Color", TypeName = "nvarchar")]
    [Comment("群組顏色")]
    public string? Color { get; set; } = "#007ACC";

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Column("Enable", TypeName = "bit")]
    [Comment("是否啟用")]
    [Required]
    public bool Enable { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Column("CreationTime", TypeName = "datetime2(7)")]
    [Comment("建立時間")]
    [Required]
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    [Column("CreatorUserId", TypeName = "uniqueidentifier")]
    [Comment("建立者")]
    [Required]
    public Guid CreatorUserId { get; set; }

    /// <summary>
    /// 最後修改時間
    /// </summary>
    [Column("LastModificationTime", TypeName = "datetime2(7)")]
    [Comment("最後修改時間")]
    [Required]
    public DateTime LastModificationTime { get; set; }

    /// <summary>
    /// 最後修改者
    /// </summary>
    [Column("LastModifierUserId", TypeName = "uniqueidentifier")]
    [Comment("最後修改者")]
    [Required]
    public Guid LastModifierUserId { get; set; }

    #endregion Properties

    #region Navigation Properties

    /// <summary>
    /// 群組卡片關聯
    /// </summary>
    public virtual ICollection<GroupCard> GroupCards { get; set; } = new List<GroupCard>();

    /// <summary>
    /// 群組裝置
    /// </summary>
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    #endregion Navigation Properties
} 