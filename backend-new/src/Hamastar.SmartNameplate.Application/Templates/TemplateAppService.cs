//-----
// <copyright file="TemplateAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Templates;
using Hamastar.SmartNameplate.Dto.Backend.Templates.Request;
using Hamastar.SmartNameplate.Dto.Backend.Templates.Response;
using Hamastar.SmartNameplate.IApplication.Templates;
using Hamastar.SmartNameplate.IApplication.AuditTrails;
using Hamastar.SmartNameplate.Permissions;
using Hamastar.SmartNameplate.Repositories.Templates;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.Templates
{
    /// <summary>
    /// 模板 App
    /// </summary>
    public class TemplateAppService : ApplicationService, ITemplateAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<TemplateAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 模板儲存庫
        /// </summary>
        private readonly ITemplateRepository _templateRepository;

        /// <summary>
        /// 審計軌跡資料 APP
        /// </summary>
        private readonly IAuditTrailService _auditTrailService;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="templateRepository"> 模板儲存庫 </param>
        /// <param name="auditTrailService"> 審計軌跡資料 APP </param>
        public TemplateAppService(
            ICurrentUser currentUser,
            ITemplateRepository templateRepository,
            IAuditTrailService auditTrailService)
        {
            _currentUser = currentUser;
            _templateRepository = templateRepository;
            _auditTrailService = auditTrailService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 查詢：模板列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<TemplateListResponse>> GetTemplateListByPage(TemplateListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<TemplateListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Keyword = sanitizer.Sanitize(request.Keyword);
                request.Keyword = HttpUtility.UrlDecode(request.Keyword);

                // ========= 業務邏輯執行 =========
                TemplateListResponse templateList = await _templateRepository.GetListByPage(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("模板查詢", "DATA_READ", "查詢了" + templateList.ItemTotalCount + "筆模板資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = templateList;
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
        /// 查詢：公開模板列表
        /// </summary>
        /// <returns> 公開模板列表 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<TemplateListResponse>> GetPublicTemplates()
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<TemplateListResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                TemplateListResponse templateList = await _templateRepository.GetPublicTemplatesAsync();

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("公開模板查詢", "DATA_READ", "查詢了" + templateList.ItemTotalCount + "筆公開模板資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = templateList;
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
        /// 查詢：根據分類取得模板列表
        /// </summary>
        /// <param name="request"> 分類查詢請求 </param>
        /// <returns> 模板列表 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<TemplateListResponse>> GetTemplatesByCategory(TemplateCategoryRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<TemplateListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Category = sanitizer.Sanitize(request.Category);

                // ========= 業務邏輯執行 =========
                TemplateListResponse templateList = await _templateRepository.GetTemplatesByCategoryAsync(request.Category);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("分類模板查詢", "DATA_READ", "查詢了分類：" + request.Category + " 的" + templateList.ItemTotalCount + "筆模板資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = templateList;
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
        /// 新增：模板
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<CreateTemplateResponse>> CreateTemplate(CreateTemplateRequest createRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<CreateTemplateResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (createRequest.Name.Length > 200) throw new BusinessException(message: "模板名稱過長 Max 200");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();

                // ========= 建立實體 =========
                Entities.Template createTemplate = new()
                {
                    Name = sanitizer.Sanitize(createRequest.Name),
                    Description = sanitizer.Sanitize(createRequest.Description),
                    Category = sanitizer.Sanitize(createRequest.Category),
                    IsPublic = createRequest.IsPublic,
                    ThumbnailA = createRequest.ThumbnailA,
                    ThumbnailB = createRequest.ThumbnailB,
                    ContentA = createRequest.ContentA,
                    ContentB = createRequest.ContentB,
                    CreationTime = DateTime.Now,
                    CreatorUserId = CurrentUser.Id ?? Guid.Empty,
                    LastModificationTime = DateTime.Now,
                    LastModifierUserId = CurrentUser.Id ?? Guid.Empty
                };

                // ========= 執行新增 =========
                CreateTemplateResponse createTemplateResponse = new();
                var result = await _templateRepository.InsertAsync(createTemplate);
                if (result != null)
                {
                    await CreateAuditTrail("模板新增", "DATA_CREATE", "新增了模板：" + createTemplate.Name + " 且 成功");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Status = "success";
                    response.Message = "新增成功";
                    createTemplateResponse.Result = true;
                    createTemplateResponse.TemplateId = result.Id;
                }
                else
                {
                    await CreateAuditTrail("模板新增", "DATA_CREATE", "新增了模板：" + createTemplate.Name + " 且 失敗");
                    response.Status = "error";
                    response.Message = "新增失敗";
                    createTemplateResponse.Result = false;
                    await uow.RollbackAsync();
                }
                await uow.CompleteAsync();
                response.Data = createTemplateResponse;
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
        /// 更新：模板
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<UpdateTemplateResponse>> UpdateTemplate(UpdateTemplateRequest updateRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<UpdateTemplateResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (!string.IsNullOrEmpty(updateRequest.Name) && updateRequest.Name.Length > 200) 
                    throw new BusinessException(message: "模板名稱過長 Max 200");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();

                // ========= 取得實體 =========
                var template = await _templateRepository.GetAsync(updateRequest.Id);
                if (template == null)
                {
                    throw new BusinessException(message: "模板不存在");
                }

                // ========= 更新實體 =========
                if (!string.IsNullOrEmpty(updateRequest.Name))
                    template.Name = sanitizer.Sanitize(updateRequest.Name);
                
                if (updateRequest.Description != null)
                    template.Description = sanitizer.Sanitize(updateRequest.Description);
                
                if (updateRequest.Category != null)
                    template.Category = sanitizer.Sanitize(updateRequest.Category);
                
                if (updateRequest.IsPublic.HasValue)
                    template.IsPublic = updateRequest.IsPublic.Value;
                
                if (updateRequest.ThumbnailA != null)
                    template.ThumbnailA = updateRequest.ThumbnailA;
                
                if (updateRequest.ThumbnailB != null)
                    template.ThumbnailB = updateRequest.ThumbnailB;
                
                if (updateRequest.ContentA != null)
                    template.ContentA = updateRequest.ContentA;
                    
                if (updateRequest.ContentB != null)
                    template.ContentB = updateRequest.ContentB;

                template.LastModificationTime = DateTime.Now;
                template.LastModifierUserId = CurrentUser.Id ?? Guid.Empty;

                // ========= 執行更新 =========
                UpdateTemplateResponse updateTemplateResponse = new();
                var result = await _templateRepository.UpdateAsync(template);
                if (result != null)
                {
                    await CreateAuditTrail("模板更新", "DATA_UPDATE", "更新了模板：" + template.Name + " 且 成功");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Status = "success";
                    response.Message = "更新成功";
                    updateTemplateResponse.Result = true;
                }
                else
                {
                    await CreateAuditTrail("模板更新", "DATA_UPDATE", "更新了模板：" + template.Name + " 且 失敗");
                    response.Status = "error";
                    response.Message = "更新失敗";
                    updateTemplateResponse.Result = false;
                    await uow.RollbackAsync();
                }
                await uow.CompleteAsync();
                response.Data = updateTemplateResponse;
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
        /// 刪除：模板
        /// </summary>
        /// <param name="deleteRequest"> 刪除條件 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<DeleteTemplateResponse>> DeleteTemplate(DeleteTemplateRequest deleteRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeleteTemplateResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                var template = await _templateRepository.GetAsync(deleteRequest.Id);
                if (template == null)
                {
                    throw new BusinessException(message: "模板不存在");
                }

                string templateName = template.Name;
                await _templateRepository.DeleteAsync(template);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("模板刪除", "DATA_DELETE", "刪除了模板：" + templateName + " 且 成功");
                await CurrentUnitOfWork.SaveChangesAsync();
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                DeleteTemplateResponse deleteTemplateResponse = new()
                {
                    Result = true
                };
                response.Status = "success";
                response.Message = "刪除成功";
                response.Data = deleteTemplateResponse;
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