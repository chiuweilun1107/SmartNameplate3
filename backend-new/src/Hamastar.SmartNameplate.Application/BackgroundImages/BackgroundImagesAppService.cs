//-----
// <copyright file="BackgroundImagesAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Request;
using Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Response;
using Hamastar.SmartNameplate.IApplication.BackgroundImages;
using Hamastar.SmartNameplate.IApplication.AuditTrails;
using Hamastar.SmartNameplate.Permissions;
using Hamastar.SmartNameplate.Repositories.BackgroundImages;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.BackgroundImages
{
    /// <summary>
    /// 背景圖片 App Service
    /// </summary>
    public class BackgroundImagesAppService : ApplicationService, IBackgroundImagesAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<BackgroundImagesAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 背景圖片儲存庫
        /// </summary>
        private readonly IBackgroundImagesRepository _backgroundImagesRepository;

        /// <summary>
        /// 審計軌跡資料 APP
        /// </summary>
        private readonly IAuditTrailService _auditTrailService;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundImagesAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="backgroundImagesRepository"> 背景圖片儲存庫 </param>
        /// <param name="auditTrailService"> 審計軌跡資料 APP </param>
        public BackgroundImagesAppService(
            ICurrentUser currentUser,
            IBackgroundImagesRepository backgroundImagesRepository,
            IAuditTrailService auditTrailService)
        {
            _currentUser = currentUser;
            _backgroundImagesRepository = backgroundImagesRepository;
            _auditTrailService = auditTrailService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 查詢：背景圖片列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<BackgroundImageListResponse>> GetBackgroundImageListByPage(BackgroundImageListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<BackgroundImageListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Keyword = sanitizer.Sanitize(request.Keyword);
                request.Keyword = HttpUtility.UrlDecode(request.Keyword);

                // ========= 業務邏輯執行 =========
                BackgroundImageListResponse backgroundImageList = await _backgroundImagesRepository.GetBackgroundImageListByPage(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("背景圖片查詢", "DATA_READ", "查詢了" + backgroundImageList.ItemTotalCount + "筆背景圖片資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = backgroundImageList;
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
        /// 查詢：公開背景圖片列表
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 公開圖片列表 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<BackgroundImageListResponse>> GetPublicBackgroundImages(BackgroundImageListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<BackgroundImageListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Keyword = sanitizer.Sanitize(request.Keyword);
                request.Keyword = HttpUtility.UrlDecode(request.Keyword);

                // ========= 業務邏輯執行 =========
                BackgroundImageListResponse backgroundImageList = await _backgroundImagesRepository.GetPublicBackgroundImages(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("公開背景圖片查詢", "DATA_READ", "查詢了" + backgroundImageList.ItemTotalCount + "筆公開背景圖片資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = backgroundImageList;
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
        /// 查詢：依分類取得背景圖片
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 分類圖片列表 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<BackgroundImageListResponse>> GetBackgroundImagesByCategory(BackgroundImageCategoryRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<BackgroundImageListResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (string.IsNullOrEmpty(request.Category)) throw new BusinessException(message: "分類不能為空");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Category = sanitizer.Sanitize(request.Category);
                request.Category = HttpUtility.UrlDecode(request.Category);

                // ========= 業務邏輯執行 =========
                BackgroundImageListResponse backgroundImageList = await _backgroundImagesRepository.GetBackgroundImagesByCategory(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("分類背景圖片查詢", "DATA_READ", "查詢了分類[" + request.Category + "]的" + backgroundImageList.ItemTotalCount + "筆背景圖片資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = backgroundImageList;
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
        /// 查詢：單一背景圖片
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 圖片詳情 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<BackgroundImageResponse>> GetBackgroundImageById(BackgroundImageRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<BackgroundImageResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (request.Id <= 0) throw new BusinessException(message: "ID無效");

                // ========= 業務邏輯執行 =========
                BackgroundImageResponse backgroundImage = await _backgroundImagesRepository.GetBackgroundImageById(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("背景圖片詳情查詢", "DATA_READ", "查詢了背景圖片ID[" + request.Id + "]的詳情");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = backgroundImage;
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
        /// 新增：背景圖片
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 新增結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<CreateBackgroundImageResponse>> CreateBackgroundImage(CreateBackgroundImageRequest createRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<CreateBackgroundImageResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (string.IsNullOrEmpty(createRequest.Name)) throw new BusinessException(message: "名稱不能為空");
                if (string.IsNullOrEmpty(createRequest.ImageUrl)) throw new BusinessException(message: "圖片URL不能為空");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                createRequest.Name = sanitizer.Sanitize(createRequest.Name);
                createRequest.Description = sanitizer.Sanitize(createRequest.Description);
                createRequest.ImageUrl = sanitizer.Sanitize(createRequest.ImageUrl);
                createRequest.ThumbnailUrl = sanitizer.Sanitize(createRequest.ThumbnailUrl);
                createRequest.Category = sanitizer.Sanitize(createRequest.Category);

                // ========= 業務邏輯執行 =========
                CreateBackgroundImageResponse createBackgroundImageResponse = await _backgroundImagesRepository.CreateBackgroundImage(createRequest);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("背景圖片新增", "DATA_CREATE", "新增了背景圖片[" + createRequest.Name + "]");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "新增成功";
                response.Data = createBackgroundImageResponse;
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
        /// 更新：背景圖片
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 更新結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<UpdateBackgroundImageResponse>> UpdateBackgroundImage(UpdateBackgroundImageRequest updateRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<UpdateBackgroundImageResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (updateRequest.Id <= 0) throw new BusinessException(message: "ID無效");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                updateRequest.Name = sanitizer.Sanitize(updateRequest.Name);
                updateRequest.Description = sanitizer.Sanitize(updateRequest.Description);
                updateRequest.ImageUrl = sanitizer.Sanitize(updateRequest.ImageUrl);
                updateRequest.ThumbnailUrl = sanitizer.Sanitize(updateRequest.ThumbnailUrl);
                updateRequest.Category = sanitizer.Sanitize(updateRequest.Category);

                // ========= 業務邏輯執行 =========
                UpdateBackgroundImageResponse updateBackgroundImageResponse = await _backgroundImagesRepository.UpdateBackgroundImage(updateRequest);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("背景圖片更新", "DATA_UPDATE", "更新了背景圖片ID[" + updateRequest.Id + "]");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "更新成功";
                response.Data = updateBackgroundImageResponse;
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
        /// 刪除：背景圖片
        /// </summary>
        /// <param name="request"> 刪除條件 </param>
        /// <returns> 刪除結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<DeleteBackgroundImageResponse>> DeleteBackgroundImage(DeleteBackgroundImageRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeleteBackgroundImageResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (request.Id <= 0) throw new BusinessException(message: "ID無效");

                // ========= 業務邏輯執行 =========
                DeleteBackgroundImageResponse deleteBackgroundImageResponse = await _backgroundImagesRepository.DeleteBackgroundImage(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("背景圖片刪除", "DATA_DELETE", "刪除了背景圖片ID[" + request.Id + "]");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "刪除成功";
                response.Data = deleteBackgroundImageResponse;
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