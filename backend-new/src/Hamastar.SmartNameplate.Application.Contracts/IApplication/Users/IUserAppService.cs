//-----
// <copyright file="IUserAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Users.Request;
using Hamastar.SmartNameplate.Dto.Backend.Users.Response;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Hamastar.SmartNameplate.IApplication.Users
{
    /// <summary>
    /// 使用者 App Service 介面
    /// </summary>
    public interface IUserAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 查詢：使用者列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        Task<BusinessLogicResponse<UserListResponse>> GetUserListByPage(UserListRequest request);

        /// <summary>
        /// 查詢：單一使用者
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 使用者資訊 </returns>
        Task<BusinessLogicResponse<UserResponse>> GetUser(UserRequest request);

        /// <summary>
        /// 新增：使用者
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<CreateUserResponse>> CreateUser(CreateUserRequest createRequest);

        /// <summary>
        /// 更新：使用者密碼
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<UpdateUserResponse>> UpdateUserPassword(UpdateUserRequest updateRequest);

        #endregion Methods
    }
} 