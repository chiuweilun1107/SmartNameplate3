//-----
// <copyright file="Card.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Hamastar.SmartNameplate.Entities;

/// <summary>
/// 🤖 卡片狀態列舉 (對應原始 CardStatus)
/// </summary>
public enum CardStatus
{
    Draft = 0,
    Active = 1,
    Inactive = 2
}

/// <summary>
/// 🤖 卡片實體 (對應原始 Card)
/// </summary>
[Table("Cards")]
[Comment("🤖 卡片資料表")]
public class Card : AuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 卡片名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    [Comment("卡片名稱")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 卡片描述
    /// </summary>
    [StringLength(500)]
    [Comment("卡片描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 模板 ID
    /// </summary>
    [Comment("模板 ID")]
    public Guid? TemplateId { get; set; }

    /// <summary>
    /// 卡片狀態
    /// </summary>
    [Comment("卡片狀態")]
    public CardStatus Status { get; set; } = CardStatus.Draft;

    /// <summary>
    /// 卡片資料 JSON
    /// </summary>
    [Comment("卡片資料 JSON")]
    public string? Data { get; set; }

    /// <summary>
    /// 預覽圖片路徑
    /// </summary>
    [StringLength(500)]
    [Comment("預覽圖片路徑")]
    public string? PreviewImagePath { get; set; }

    /// <summary>
    /// 標籤
    /// </summary>
    [StringLength(500)]
    [Comment("標籤")]
    public string? Tags { get; set; }

    /// <summary>
    /// 是否為公開模板
    /// </summary>
    [Comment("是否為公開模板")]
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// 版本號
    /// </summary>
    [Comment("版本號")]
    public int Version { get; set; } = 1;

    /// <summary>
    /// 標記 A、B 面是否相同 (true = side 0, false = side 1/2)
    /// </summary>
    [Comment("標記 A、B 面是否相同 (true = side 0, false = side 1/2)")]
    public bool IsSameBothSides { get; set; } = false;

    /// <summary>
    /// 文字元素標籤資訊 (姓名、職稱、電話、地址、公司等)
    /// </summary>
    [StringLength(500)]
    [Comment("文字元素標籤資訊 (姓名、職稱、電話、地址、公司等)")]
    public string? ContentA { get; set; }

    /// <summary>
    /// 文字元素標籤資訊 (姓名、職稱、電話、地址、公司等)
    /// </summary>
    [StringLength(500)]
    [Comment("文字元素標籤資訊 (姓名、職稱、電話、地址、公司等)")]
    public string? ContentB { get; set; }

    /// <summary>
    /// 預覽圖片路徑
    /// </summary>
    [StringLength(500)]
    [Comment("預覽圖片路徑")]
    public string? ThumbnailA { get; set; }

    /// <summary>
    /// 預覽圖片路徑
    /// </summary>
    [StringLength(500)]
    [Comment("預覽圖片路徑")]
    public string? ThumbnailB { get; set; }

    // Navigation properties
    public virtual Template? Template { get; set; }
    public virtual User? Creator { get; set; }
    public virtual ICollection<DeployHistory> DeployHistories { get; set; } = new List<DeployHistory>();
    public virtual ICollection<GroupCard> GroupCards { get; set; } = new List<GroupCard>();
} 