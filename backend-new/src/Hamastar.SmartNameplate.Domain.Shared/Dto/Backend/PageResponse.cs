//-----
// <copyright file="PageResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System;
using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend;

/// <summary>
/// 分頁回應基底類別
/// </summary>
public class PageResponse
{
    #region Properties

    /// <summary>
    /// 當前頁碼
    /// </summary>
    [JsonProperty("currentPage")]
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// 每頁資料筆數
    /// </summary>
    [JsonProperty("pageSize")]
    public int PageSize { get; set; } = SmartNameplateConsts.DefaultPageSize;

    /// <summary>
    /// 總資料筆數
    /// </summary>
    [JsonProperty("itemTotalCount")]
    public long ItemTotalCount { get; set; } = 0;

    /// <summary>
    /// 總頁數
    /// </summary>
    [JsonProperty("totalPages")]
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)ItemTotalCount / PageSize) : 0;

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    [JsonProperty("hasPreviousPage")]
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    [JsonProperty("hasNextPage")]
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// 是否為第一頁
    /// </summary>
    [JsonProperty("isFirstPage")]
    public bool IsFirstPage => CurrentPage == 1;

    /// <summary>
    /// 是否為最後一頁
    /// </summary>
    [JsonProperty("isLastPage")]
    public bool IsLastPage => CurrentPage == TotalPages || TotalPages == 0;

    /// <summary>
    /// 當前頁開始的資料索引 (從 1 開始)
    /// </summary>
    [JsonProperty("startIndex")]
    public long StartIndex => ItemTotalCount == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;

    /// <summary>
    /// 當前頁結束的資料索引
    /// </summary>
    [JsonProperty("endIndex")]
    public long EndIndex
    {
        get
        {
            if (ItemTotalCount == 0) return 0;
            var end = CurrentPage * PageSize;
            return end > ItemTotalCount ? ItemTotalCount : end;
        }
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// 從分頁請求設定分頁資訊
    /// </summary>
    /// <param name="request"> 分頁請求 </param>
    /// <param name="totalCount"> 總資料筆數 </param>
    public virtual void SetPageInfo(PageRequest request, long totalCount)
    {
        CurrentPage = request.Page;
        PageSize = request.PageSize;
        ItemTotalCount = totalCount;
    }

    /// <summary>
    /// 建立分頁資訊
    /// </summary>
    /// <param name="page"> 當前頁碼 </param>
    /// <param name="pageSize"> 每頁資料筆數 </param>
    /// <param name="totalCount"> 總資料筆數 </param>
    /// <returns> 分頁回應 </returns>
    public static PageResponse Create(int page, int pageSize, long totalCount)
    {
        return new PageResponse
        {
            CurrentPage = page,
            PageSize = pageSize,
            ItemTotalCount = totalCount
        };
    }

    #endregion Methods
} 