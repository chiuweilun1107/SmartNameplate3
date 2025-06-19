//-----
// <copyright file="UserRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Hamastar.SmartNameplate.Entities;
using Hamastar.SmartNameplate.EntityFrameworkCore;

namespace Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Users
{
    /// <summary>
    /// 🤖 使用者儲存庫實作
    /// 基於原始 UserService 重構
    /// </summary>
    public class UserRepository : EfCoreRepository<SmartNameplateDbContext, User, Guid>, IUserRepository
    {
        #region Fields

        /// <summary>
        /// 應用程式配置
        /// </summary>
        private readonly IConfiguration _appConfiguration;

        /// <summary>
        /// 工作單元管理器
        /// </summary>
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// 日誌記錄器
        /// </summary>
        private readonly ILogger log = Log.ForContext<UserRepository>();

        /// <summary>
        /// 當前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="dbContextProvider"> 資料庫上下文提供者 </param>
        /// <param name="appConfiguration"> 應用程式配置 </param>
        /// <param name="unitOfWorkManager"> 工作單元管理器 </param>
        /// <param name="currentUser"> 當前使用者 </param>
        public UserRepository(IDbContextProvider<SmartNameplateDbContext> dbContextProvider,
            IConfiguration appConfiguration,
            IUnitOfWorkManager unitOfWorkManager,
            ICurrentUser currentUser)
            : base(dbContextProvider)
        {
            _appConfiguration = appConfiguration;
            _unitOfWorkManager = unitOfWorkManager;
            _currentUser = currentUser;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 驗證使用者密碼
        /// </summary>
        /// <param name="userName"> 使用者名稱 </param>
        /// <param name="password"> 密碼 </param>
        /// <returns> 驗證結果 </returns>
        public async Task<bool> ValidateUserPasswordAsync(string userName, string password)
        {
            var dbContext = await GetDbContextAsync();
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName && x.IsActive);
            return user != null && user.PasswordHash == password;
        }

        /// <summary>
        /// 更新使用者最後登入時間
        /// </summary>
        /// <param name="userId"> 使用者 ID </param>
        /// <returns> 更新結果 </returns>
        public async Task<bool> UpdateLastLoginTimeAsync(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return false;
            user.LastLoginTime = DateTime.UtcNow;
            user.LastModificationTime = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 檢查使用者名稱是否存在
        /// </summary>
        /// <param name="userName"> 使用者名稱 </param>
        /// <returns> 是否存在 </returns>
        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.Users.AnyAsync(x => x.UserName == userName);
        }

        /// <summary>
        /// 檢查電子郵件是否存在
        /// </summary>
        /// <param name="email"> 電子郵件 </param>
        /// <returns> 是否存在 </returns>
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.Users.AnyAsync(x => x.Email == email);
        }

        #endregion Public Methods
    }
} 