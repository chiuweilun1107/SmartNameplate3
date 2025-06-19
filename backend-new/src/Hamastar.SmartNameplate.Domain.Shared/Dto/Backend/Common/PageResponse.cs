using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

/// <summary>
/// 🤖 分頁回應基類
/// 所有分頁查詢回應的基礎類別
/// </summary>
public class PageResponse<T>
{
    /// <summary>
    /// 資料列表
    /// </summary>
    [JsonProperty("data")]
    public virtual List<T> Data { get; set; } = new();

    /// <summary>
    /// 總筆數
    /// </summary>
    [JsonProperty("totalCount")]
    public virtual int TotalCount { get; set; }

    /// <summary>
    /// 每頁筆數
    /// </summary>
    [JsonProperty("pageSize")]
    public virtual int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    [JsonProperty("totalPages")]
    public int TotalPages => PageSize > 0 ? (TotalCount + PageSize - 1) / PageSize : 0;
} 