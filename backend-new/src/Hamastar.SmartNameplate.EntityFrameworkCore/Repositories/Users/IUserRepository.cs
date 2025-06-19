//-----
// <copyright file="IUserRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Hamastar.SmartNameplate.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Users
{
    /// <summary>
    /// 使用者儲存庫介面
    /// </summary>
    public interface IUserRepository : IRepository<User, Guid>
    {
        #region Methods

        /// <summary>
        /// 驗證：使用者密碼
        /// </summary>
        /// <param name="userName"> 使用者名稱 </param>
        /// <param name="password"> 密碼 </param>
        /// <returns> 驗證結果 </returns>
        Task<bool> ValidateUserPasswordAsync(string userName, string password);

        /// <summary>
        /// 更新：使用者最後登入時間
        /// </summary>
        /// <param name="userId"> 使用者 ID </param>
        /// <returns> 更新結果 </returns>
        Task<bool> UpdateLastLoginTimeAsync(Guid userId);

        /// <summary>
        /// 檢查：使用者名稱是否存在
        /// </summary>
        /// <param name="userName"> 使用者名稱 </param>
        /// <returns> 是否存在 </returns>
        Task<bool> IsUserNameExistsAsync(string userName);

        /// <summary>
        /// 檢查：電子郵件是否存在
        /// </summary>
        /// <param name="email"> 電子郵件 </param>
        /// <returns> 是否存在 </returns>
        Task<bool> IsEmailExistsAsync(string email);

        #endregion Methods
    }
} 