//-----
// <copyright file="CardListResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System.Collections.Generic;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Cards.Response
{
    /// <summary>
    /// 🤖 卡片列表回應 (對應原始 CardListResponse)
    /// </summary>
    public class CardListResponse : PageResponse<CardItemForListByPage>
    {
        /// <summary>
        /// 建構函式
        /// </summary>
        public CardListResponse()
        {
            Data = new List<CardItemForListByPage>();
        }

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="items">項目列表</param>
        /// <param name="totalCount">總數量</param>
        /// <param name="pageIndex">頁面索引</param>
        /// <param name="pageSize">頁面大小</param>
        public CardListResponse(List<CardItemForListByPage> items, int totalCount, int pageIndex, int pageSize)
        {
            Data = items;
            TotalCount = totalCount;
            PageSize = pageSize;
        }
    }
} 