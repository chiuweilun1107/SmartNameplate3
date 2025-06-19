//-----
// <copyright file="Template.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hamastar.SmartNameplate.Entities;

/// <summary>
/// 模板實體
/// </summary>
[Table("AppTemplates")]
[Comment("模板主表")]
public class Template : Volo.Abp.Domain.Entities.BasicAggregateRoot<Guid>
{
    #region Properties

    /// <summary>
    /// 模板 ID
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Comment("模板 ID")]
    public override Guid Id { get; protected set; }

    /// <summary>
    /// 模板名稱
    /// </summary>
    [MaxLength(255)]
    [Column("Name", TypeName = "nvarchar")]
    [Comment("模板名稱")]
    [Required]
    public string Name { get; set; } = "";

    /// <summary>
    /// 模板描述
    /// </summary>
    [MaxLength(500)]
    [Column("Description", TypeName = "nvarchar")]
    [Comment("模板描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 縮圖 URL
    /// </summary>
    [Column("ThumbnailUrl", TypeName = "nvarchar(MAX)")]
    [Comment("縮圖 URL")]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// A面縮圖
    /// </summary>
    [Column("ThumbnailA", TypeName = "nvarchar(MAX)")]
    [Comment("A面縮圖")]
    public string? ThumbnailA { get; set; }

    /// <summary>
    /// B面縮圖
    /// </summary>
    [Column("ThumbnailB", TypeName = "nvarchar(MAX)")]
    [Comment("B面縮圖")]
    public string? ThumbnailB { get; set; }

    /// <summary>
    /// A面佈局資料
    /// </summary>
    [Column("LayoutDataA", TypeName = "nvarchar(MAX)")]
    [Comment("A面佈局資料")]
    public string? LayoutDataA { get; set; }

    /// <summary>
    /// B面佈局資料
    /// </summary>
    [Column("LayoutDataB", TypeName = "nvarchar(MAX)")]
    [Comment("B面佈局資料")]
    public string? LayoutDataB { get; set; }

    /// <summary>
    /// 尺寸資訊
    /// </summary>
    [Column("Dimensions", TypeName = "nvarchar(MAX)")]
    [Comment("尺寸資訊")]
    public string? Dimensions { get; set; }

    /// <summary>
    /// 組織 ID
    /// </summary>
    [Column("OrganizationId", TypeName = "uniqueidentifier")]
    [Comment("組織 ID")]
    public Guid? OrganizationId { get; set; }

    /// <summary>
    /// 是否公開
    /// </summary>
    [Column("IsPublic", TypeName = "bit")]
    [Comment("是否公開")]
    [Required]
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// 分類
    /// </summary>
    [MaxLength(100)]
    [Column("Category", TypeName = "nvarchar")]
    [Comment("分類")]
    [Required]
    public string Category { get; set; } = "general";

    /// <summary>
    /// 文字元素標籤資訊
    /// </summary>
    [MaxLength(500)]
    [Column("Tags", TypeName = "nvarchar")]
    [Comment("文字元素標籤資訊")]
    public string? Tags { get; set; }

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
    /// 建立者
    /// </summary>
    public virtual User? Creator { get; set; }

    #endregion Navigation Properties
} 