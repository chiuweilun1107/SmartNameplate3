//-----
// <copyright file="IGroupRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend.Groups;
using Hamastar.SmartNameplate.Dto.Backend.Groups.Request;
using Hamastar.SmartNameplate.Dto.Backend.Groups.Response;
using Hamastar.SmartNameplate.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Hamastar.SmartNameplate.Repositories.Groups
{
    /// <summary>
    /// 群組儲存庫介面
    /// </summary>
    public interface IGroupRepository : IRepository<Group, Guid>
    {
        #region Methods

        /// <summary>
        /// 查詢：群組列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        Task<GroupListResponse> GetListByPage(GroupListRequest request);

        /// <summary>
        /// 新增：卡片到群組
        /// </summary>
        /// <param name="groupId"> 群組 ID </param>
        /// <param name="cardId"> 卡片 ID </param>
        /// <returns> 操作結果 </returns>
        Task<bool> AddCardToGroupAsync(Guid groupId, Guid cardId);

        /// <summary>
        /// 移除：卡片從群組
        /// </summary>
        /// <param name="groupId"> 群組 ID </param>
        /// <param name="cardId"> 卡片 ID </param>
        /// <returns> 操作結果 </returns>
        Task<bool> RemoveCardFromGroupAsync(Guid groupId, Guid cardId);

        #endregion Methods
    }
} 