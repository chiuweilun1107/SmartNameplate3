//-----
// <copyright file="GroupAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Groups;
using Hamastar.SmartNameplate.Dto.Backend.Groups.Request;
using Hamastar.SmartNameplate.Dto.Backend.Groups.Response;
using Hamastar.SmartNameplate.IApplication.Groups;
using Hamastar.SmartNameplate.IApplication.AuditTrails;
using Hamastar.SmartNameplate.Permissions;
using Hamastar.SmartNameplate.Repositories.Groups;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.Groups
{
    /// <summary>
    /// 群組 App
    /// </summary>
    public class GroupAppService : ApplicationService, IGroupAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<GroupAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 群組儲存庫
        /// </summary>
        private readonly IGroupRepository _groupRepository;

        /// <summary>
        /// 審計軌跡資料 APP
        /// </summary>
        private readonly IAuditTrailService _auditTrailService;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="groupRepository"> 群組儲存庫 </param>
        /// <param name="auditTrailService"> 審計軌跡資料 APP </param>
        public GroupAppService(
            ICurrentUser currentUser,
            IGroupRepository groupRepository,
            IAuditTrailService auditTrailService)
        {
            _currentUser = currentUser;
            _groupRepository = groupRepository;
            _auditTrailService = auditTrailService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 查詢：群組列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.GroupMgmt)]
        public async Task<BusinessLogicResponse<GroupListResponse>> GetGroupListByPage(GroupListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<GroupListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Keyword = sanitizer.Sanitize(request.Keyword);
                request.Keyword = HttpUtility.UrlDecode(request.Keyword);

                // ========= 業務邏輯執行 =========
                GroupListResponse groupList = await _groupRepository.GetListByPage(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("群組查詢", "DATA_READ", "查詢了" + groupList.ItemTotalCount + "筆群組資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = groupList;
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
        /// 新增：群組
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.GroupMgmt)]
        public async Task<BusinessLogicResponse<CreateGroupResponse>> CreateGroup(CreateGroupRequest createRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<CreateGroupResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (createRequest.Name.Length > 200) throw new BusinessException(message: "群組名稱過長 Max 200");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();

                // ========= 建立實體 =========
                Entities.Group createGroup = new()
                {
                    Name = sanitizer.Sanitize(createRequest.Name),
                    Description = sanitizer.Sanitize(createRequest.Description),
                    CreationTime = DateTime.Now,
                    CreatorUserId = CurrentUser.Id ?? Guid.Empty,
                    LastModificationTime = DateTime.Now,
                    LastModifierUserId = CurrentUser.Id ?? Guid.Empty
                };

                // ========= 執行新增 =========
                CreateGroupResponse createGroupResponse = new();
                var result = await _groupRepository.InsertAsync(createGroup);
                if (result != null)
                {
                    await CreateAuditTrail("群組新增", "DATA_CREATE", "新增了群組：" + createGroup.Name + " 且 成功");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Status = "success";
                    response.Message = "新增成功";
                    createGroupResponse.Result = true;
                    createGroupResponse.GroupId = result.Id;
                }
                else
                {
                    await CreateAuditTrail("群組新增", "DATA_CREATE", "新增了群組：" + createGroup.Name + " 且 失敗");
                    response.Status = "error";
                    response.Message = "新增失敗";
                    createGroupResponse.Result = false;
                    await uow.RollbackAsync();
                }
                await uow.CompleteAsync();
                response.Data = createGroupResponse;
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
        /// 更新：群組
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.GroupMgmt)]
        public async Task<BusinessLogicResponse<UpdateGroupResponse>> UpdateGroup(UpdateGroupRequest updateRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<UpdateGroupResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (!string.IsNullOrEmpty(updateRequest.Name) && updateRequest.Name.Length > 200) 
                    throw new BusinessException(message: "群組名稱過長 Max 200");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();

                // ========= 取得實體 =========
                var group = await _groupRepository.GetAsync(updateRequest.Id);
                if (group == null)
                {
                    throw new BusinessException(message: "群組不存在");
                }

                // ========= 更新實體 =========
                if (!string.IsNullOrEmpty(updateRequest.Name))
                    group.Name = sanitizer.Sanitize(updateRequest.Name);
                
                if (updateRequest.Description != null)
                    group.Description = sanitizer.Sanitize(updateRequest.Description);

                group.LastModificationTime = DateTime.Now;
                group.LastModifierUserId = CurrentUser.Id ?? Guid.Empty;

                // ========= 執行更新 =========
                UpdateGroupResponse updateGroupResponse = new();
                var result = await _groupRepository.UpdateAsync(group);
                if (result != null)
                {
                    await CreateAuditTrail("群組更新", "DATA_UPDATE", "更新了群組：" + group.Name + " 且 成功");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Status = "success";
                    response.Message = "更新成功";
                    updateGroupResponse.Result = true;
                }
                else
                {
                    await CreateAuditTrail("群組更新", "DATA_UPDATE", "更新了群組：" + group.Name + " 且 失敗");
                    response.Status = "error";
                    response.Message = "更新失敗";
                    updateGroupResponse.Result = false;
                    await uow.RollbackAsync();
                }
                await uow.CompleteAsync();
                response.Data = updateGroupResponse;
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
        /// 新增：卡片到群組
        /// </summary>
        /// <param name="addCardRequest"> 新增卡片請求 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.GroupMgmt)]
        public async Task<BusinessLogicResponse<AddCardToGroupResponse>> AddCardToGroup(AddCardToGroupRequest addCardRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<AddCardToGroupResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                var result = await _groupRepository.AddCardToGroupAsync(addCardRequest.GroupId, addCardRequest.CardId);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("群組卡片新增", "DATA_CREATE", "新增卡片到群組 且 " + (result ? "成功" : "失敗"));
                await CurrentUnitOfWork.SaveChangesAsync();
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                AddCardToGroupResponse addCardResponse = new()
                {
                    Result = result
                };
                response.Status = result ? "success" : "error";
                response.Message = result ? "新增成功" : "新增失敗";
                response.Data = addCardResponse;
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
        /// 刪除：群組
        /// </summary>
        /// <param name="deleteRequest"> 刪除條件 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.GroupMgmt)]
        public async Task<BusinessLogicResponse<DeleteGroupResponse>> DeleteGroup(DeleteGroupRequest deleteRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeleteGroupResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                var group = await _groupRepository.GetAsync(deleteRequest.Id);
                if (group == null)
                {
                    throw new BusinessException(message: "群組不存在");
                }

                string groupName = group.Name;
                await _groupRepository.DeleteAsync(group);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("群組刪除", "DATA_DELETE", "刪除了群組：" + groupName + " 且 成功");
                await CurrentUnitOfWork.SaveChangesAsync();
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                DeleteGroupResponse deleteGroupResponse = new()
                {
                    Result = true
                };
                response.Status = "success";
                response.Message = "刪除成功";
                response.Data = deleteGroupResponse;
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