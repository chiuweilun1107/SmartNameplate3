//-----
// <copyright file="PageRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Hamastar.SmartNameplate.Dto.Backend;

/// <summary>
/// 分頁請求基底類別
/// </summary>
public class PageRequest
{
    #region Properties

    /// <summary>
    /// 頁碼 (從 1 開始)
    /// </summary>
    [JsonProperty("page")]
    [Range(1, int.MaxValue, ErrorMessage = "頁碼必須大於 0")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每頁資料筆數
    /// </summary>
    [JsonProperty("pageSize")]
    [Range(1, 1000, ErrorMessage = "每頁資料筆數必須介於 1 到 1000 之間")]
    public int PageSize { get; set; } = SmartNameplateConsts.DefaultPageSize;

    /// <summary>
    /// 排序欄位
    /// </summary>
    [JsonProperty("sorting")]
    public string Sorting { get; set; } = "";

    /// <summary>
    /// 是否降序排列
    /// </summary>
    [JsonProperty("isDescending")]
    public bool IsDescending { get; set; } = false;

    #endregion Properties

    #region Calculated Properties

    /// <summary>
    /// 跳過的資料筆數
    /// </summary>
    [JsonIgnore]
    public int SkipCount => (Page - 1) * PageSize;

    /// <summary>
    /// 取得的資料筆數
    /// </summary>
    [JsonIgnore]
    public int TakeCount => PageSize;

    #endregion Calculated Properties

    #region Methods

    /// <summary>
    /// 驗證分頁參數
    /// </summary>
    /// <returns> 是否有效 </returns>
    public virtual bool IsValid()
    {
        return Page > 0 && PageSize > 0 && PageSize <= SmartNameplateConsts.MaxPageSize;
    }

    /// <summary>
    /// 標準化分頁參數
    /// </summary>
    public virtual void Normalize()
    {
        if (Page < 1)
            Page = 1;

        if (PageSize < 1)
            PageSize = SmartNameplateConsts.DefaultPageSize;

        if (PageSize > SmartNameplateConsts.MaxPageSize)
            PageSize = SmartNameplateConsts.MaxPageSize;

        // 清理排序欄位
        if (!string.IsNullOrWhiteSpace(Sorting))
        {
            Sorting = Sorting.Trim();
        }
    }

    #endregion Methods
} 