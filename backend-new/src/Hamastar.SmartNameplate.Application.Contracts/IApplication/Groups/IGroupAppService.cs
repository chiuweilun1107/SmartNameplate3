//-----
// <copyright file="IGroupAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Groups.Request;
using Hamastar.SmartNameplate.Dto.Backend.Groups.Response;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Hamastar.SmartNameplate.IApplication.Groups
{
    /// <summary>
    /// 群組 App Service 介面
    /// </summary>
    public interface IGroupAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 查詢：群組列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        Task<BusinessLogicResponse<GroupListResponse>> GetGroupListByPage(GroupListRequest request);

        /// <summary>
        /// 新增：群組
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<CreateGroupResponse>> CreateGroup(CreateGroupRequest createRequest);

        /// <summary>
        /// 更新：群組
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<UpdateGroupResponse>> UpdateGroup(UpdateGroupRequest updateRequest);

        /// <summary>
        /// 新增：卡片到群組
        /// </summary>
        /// <param name="addCardRequest"> 新增卡片請求 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<AddCardToGroupResponse>> AddCardToGroup(AddCardToGroupRequest addCardRequest);

        /// <summary>
        /// 刪除：群組
        /// </summary>
        /// <param name="deleteRequest"> 刪除條件 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<DeleteGroupResponse>> DeleteGroup(DeleteGroupRequest deleteRequest);

        #endregion Methods
    }
} 