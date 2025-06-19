//-----
// <copyright file="ICardRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Hamastar.SmartNameplate.Entities;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Cards.Request;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Cards.Response;

namespace Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Cards
{
    /// <summary>
    /// 🤖 卡片儲存庫介面
    /// </summary>
    public interface ICardRepository : IRepository<Card, Guid>
    {
        /// <summary>
        /// 查詢：卡片列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        Task<CardListResponse> GetListByPage(CardListRequest request);

        /// <summary>
        /// 查詢：根據狀態取得卡片列表
        /// </summary>
        /// <param name="status"> 卡片狀態 </param>
        /// <returns> 卡片列表 </returns>
        Task<IEnumerable<Card>> GetCardsByStatusAsync(CardStatus status);
    }
} 