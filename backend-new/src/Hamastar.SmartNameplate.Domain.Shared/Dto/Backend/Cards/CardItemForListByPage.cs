//-----
// <copyright file="CardItemForListByPage.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Cards;

/// <summary>
/// 🤖 卡片分頁列表項目 DTO (對應原始 CardDto)
/// </summary>
public class CardItemForListByPage
{
    /// <summary>
    /// 卡片 ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 卡片名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 卡片描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 模板 ID
    /// </summary>
    public Guid? TemplateId { get; set; }

    /// <summary>
    /// 卡片狀態 (0=Draft, 1=Active, 2=Inactive)
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 卡片資料 JSON
    /// </summary>
    public string? Data { get; set; }

    /// <summary>
    /// 預覽圖片路徑
    /// </summary>
    public string? PreviewImagePath { get; set; }

    /// <summary>
    /// 標籤
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// 是否為公開模板
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 版本號
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 標記 A、B 面是否相同 (true = side 0, false = side 1/2)
    /// </summary>
    public bool IsSameBothSides { get; set; }

    /// <summary>
    /// A面內容
    /// </summary>
    public string? ContentA { get; set; }

    /// <summary>
    /// B面內容
    /// </summary>
    public string? ContentB { get; set; }

    /// <summary>
    /// A面縮圖
    /// </summary>
    public string? ThumbnailA { get; set; }

    /// <summary>
    /// B面縮圖
    /// </summary>
    public string? ThumbnailB { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? LastModificationTime { get; set; }

    /// <summary>
    /// 建立者 ID
    /// </summary>
    public Guid? CreatorId { get; set; }
} 