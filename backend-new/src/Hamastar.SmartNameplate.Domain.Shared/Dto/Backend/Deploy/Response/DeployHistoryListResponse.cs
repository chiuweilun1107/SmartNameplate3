//-----
// <copyright file="DeployHistoryListResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System.Collections.Generic;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy.Response
{
    /// <summary>
    /// 🤖 部署歷史列表回應
    /// </summary>
    public class DeployHistoryListResponse : BusinessLogicResponse
    {
        /// <summary>
        /// 部署歷史列表
        /// </summary>
        public List<DeployHistoryItem> Items { get; set; } = new();

        /// <summary>
        /// 總數量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 頁數
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每頁數量
        /// </summary>
        public int PageSize { get; set; }
    }
} 