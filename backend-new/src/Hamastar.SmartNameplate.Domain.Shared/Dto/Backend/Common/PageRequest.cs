using System;
using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

/// <summary>
/// 🤖 分頁請求基類
/// 所有分頁查詢的基礎類別
/// </summary>
public class PageRequest
{
    /// <summary>
    /// 頁數 (從 1 開始)
    /// </summary>
    [JsonProperty("page")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    [JsonProperty("pageSize")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 排序欄位
    /// </summary>
    [JsonProperty("sortBy")]
    public string? SortBy { get; set; }

    /// <summary>
    /// 排序方向 (asc/desc)
    /// </summary>
    [JsonProperty("sortOrder")]
    public string SortOrder { get; set; } = "asc";
} 