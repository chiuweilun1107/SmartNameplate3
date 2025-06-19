//-----
// <copyright file="DeployListResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using System.Collections.Generic;
using Newtonsoft.Json;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy.Response;

/// <summary>
/// 🤖 部署列表回應 DTO
/// </summary>
public class DeployListResponse : PageResponse<DeployHistoryItem>
{
    /// <summary>
    /// 部署歷史列表
    /// </summary>
    [JsonProperty("items")]
    public List<DeployHistoryItem> Items { get; set; } = new();
} 