//-----
// <copyright file="DeviceListResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System.Collections.Generic;
using Newtonsoft.Json;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Devices.Response;

/// <summary>
/// 🤖 裝置列表回應 DTO
/// 用於裝置列表查詢的回應
/// </summary>
public class DeviceListResponse : PageResponse<DeviceItemForListByPage>
{
    #region Properties
    
    /// <summary>
    /// 裝置列表資料
    /// </summary>
    [JsonProperty("data")]
    public override List<DeviceItemForListByPage> Data { get; set; } = new();

    /// <summary>
    /// 總筆數
    /// </summary>
    [JsonProperty("totalCount")]
    public override int TotalCount { get; set; }

    /// <summary>
    /// 每頁筆數
    /// </summary>
    [JsonProperty("pageSize")]
    public override int PageSize { get; set; }

    /// <summary>
    /// 當前頁數
    /// </summary>
    [JsonProperty("page")]
    public int Page { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    [JsonProperty("pageTotalCount")]
    public int PageTotalCount { get; set; }

    #endregion
} 