//-----
// <copyright file="UserAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Users;
using Hamastar.SmartNameplate.Dto.Backend.Users.Request;
using Hamastar.SmartNameplate.Dto.Backend.Users.Response;
using Hamastar.SmartNameplate.IApplication.Users;
using Hamastar.SmartNameplate.IApplication.AuditTrails;
using Hamastar.SmartNameplate.Permissions;
using Hamastar.SmartNameplate.Repositories.Users;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.Users
{
    /// <summary>
    /// 使用者 App
    /// </summary>
    public class UserAppService : ApplicationService, IUserAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<UserAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 使用者儲存庫
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// 審計軌跡資料 APP
        /// </summary>
        private readonly IAuditTrailService _auditTrailService;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="userRepository"> 使用者儲存庫 </param>
        /// <param name="auditTrailService"> 審計軌跡資料 APP </param>
        public UserAppService(
            ICurrentUser currentUser,
            IUserRepository userRepository,
            IAuditTrailService auditTrailService)
        {
            _currentUser = currentUser;
            _userRepository = userRepository;
            _auditTrailService = auditTrailService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 查詢：使用者列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.UserMgmt)]
        public async Task<BusinessLogicResponse<UserListResponse>> GetUserListByPage(UserListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<UserListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Keyword = sanitizer.Sanitize(request.Keyword);
                request.Keyword = HttpUtility.UrlDecode(request.Keyword);

                // ========= 業務邏輯執行 =========
                UserListResponse userList = await _userRepository.GetListByPage(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("使用者查詢", "DATA_READ", "查詢了" + userList.ItemTotalCount + "筆使用者資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = userList;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        /// <summary>
        /// 查詢：單一使用者
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 使用者資訊 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.UserMgmt)]
        public async Task<BusinessLogicResponse<UserResponse>> GetUser(UserRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<UserResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                var user = await _userRepository.GetAsync(request.Id);
                if (user == null)
                {
                    throw new BusinessException(message: "使用者不存在");
                }

                UserResponse userResponse = new()
                {
                    User = new UserItem
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Role = user.Role,
                        CreationTime = user.CreationTime,
                        LastModificationTime = user.LastModificationTime
                    }
                };

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("使用者查詢", "DATA_READ", "查詢了使用者：" + user.UserName);
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = userResponse;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        /// <summary>
        /// 新增：使用者
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.UserMgmt)]
        public async Task<BusinessLogicResponse<CreateUserResponse>> CreateUser(CreateUserRequest createRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<CreateUserResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (createRequest.UserName.Length > 100) throw new BusinessException(message: "使用者名稱過長 Max 100");
                if (createRequest.Password.Length < 6) throw new BusinessException(message: "密碼長度至少 6 位");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();

                // ========= 檢查重複 =========
                var existingUser = await _userRepository.GetUserByUsernameAsync(createRequest.UserName);
                if (existingUser != null)
                {
                    throw new BusinessException(message: $"使用者 '{createRequest.UserName}' 已存在");
                }

                // ========= 建立實體 =========
                Entities.User createUser = new()
                {
                    UserName = sanitizer.Sanitize(createRequest.UserName),
                    PasswordHash = _userRepository.HashPassword(createRequest.Password),
                    Role = sanitizer.Sanitize(createRequest.Role),
                    CreationTime = DateTime.Now,
                    CreatorUserId = CurrentUser.Id ?? Guid.Empty,
                    LastModificationTime = DateTime.Now,
                    LastModifierUserId = CurrentUser.Id ?? Guid.Empty
                };

                // ========= 執行新增 =========
                CreateUserResponse createUserResponse = new();
                var result = await _userRepository.InsertAsync(createUser);
                if (result != null)
                {
                    await CreateAuditTrail("使用者新增", "DATA_CREATE", "新增了使用者：" + createUser.UserName + " 且 成功");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Status = "success";
                    response.Message = "新增成功";
                    createUserResponse.Result = true;
                    createUserResponse.UserId = result.Id;
                }
                else
                {
                    await CreateAuditTrail("使用者新增", "DATA_CREATE", "新增了使用者：" + createUser.UserName + " 且 失敗");
                    response.Status = "error";
                    response.Message = "新增失敗";
                    createUserResponse.Result = false;
                    await uow.RollbackAsync();
                }
                await uow.CompleteAsync();
                response.Data = createUserResponse;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        /// <summary>
        /// 更新：使用者密碼
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.UserMgmt)]
        public async Task<BusinessLogicResponse<UpdateUserResponse>> UpdateUserPassword(UpdateUserRequest updateRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<UpdateUserResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (updateRequest.NewPassword.Length < 6) throw new BusinessException(message: "密碼長度至少 6 位");

                // ========= 業務邏輯執行 =========
                var result = await _userRepository.UpdateUserPasswordAsync(updateRequest.UserName, updateRequest.NewPassword);
                
                UpdateUserResponse updateUserResponse = new();
                if (result != null)
                {
                    await CreateAuditTrail("使用者密碼更新", "DATA_UPDATE", "更新了使用者密碼：" + updateRequest.UserName + " 且 成功");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Status = "success";
                    response.Message = "密碼更新成功";
                    updateUserResponse.Result = true;
                }
                else
                {
                    await CreateAuditTrail("使用者密碼更新", "DATA_UPDATE", "更新了使用者密碼：" + updateRequest.UserName + " 且 失敗");
                    response.Status = "error";
                    response.Message = "密碼更新失敗";
                    updateUserResponse.Result = false;
                    await uow.RollbackAsync();
                }
                await uow.CompleteAsync();
                response.Data = updateUserResponse;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 儲存操作紀錄至審計軌跡
        /// </summary>
        /// <param name="target"> 操作目標 </param>
        /// <param name="type"> 操作類型 </param>
        /// <param name="description"> 操作描述 </param>
        /// <returns> Task </returns>
        private async Task CreateAuditTrail(string target, string type, string description)
        {
            await _auditTrailService.CreateAsync(
                target: "SmartNameplate-系統管理-" + target,
                type: type,
                description: description
                );
        }

        #endregion Private Methods
    }
} 