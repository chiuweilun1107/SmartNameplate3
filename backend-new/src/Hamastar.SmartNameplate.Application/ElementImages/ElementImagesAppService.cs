//-----
// <copyright file="ElementImagesAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.ElementImages.Request;
using Hamastar.SmartNameplate.Dto.Backend.ElementImages.Response;
using Hamastar.SmartNameplate.IApplication.ElementImages;
using Hamastar.SmartNameplate.IApplication.AuditTrails;
using Hamastar.SmartNameplate.Permissions;
using Hamastar.SmartNameplate.Repositories.ElementImages;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.ElementImages
{
    /// <summary>
    /// 元素圖片 App Service
    /// </summary>
    public class ElementImagesAppService : ApplicationService, IElementImagesAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<ElementImagesAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 元素圖片儲存庫
        /// </summary>
        private readonly IElementImagesRepository _elementImagesRepository;

        /// <summary>
        /// 審計軌跡資料 APP
        /// </summary>
        private readonly IAuditTrailService _auditTrailService;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementImagesAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="elementImagesRepository"> 元素圖片儲存庫 </param>
        /// <param name="auditTrailService"> 審計軌跡資料 APP </param>
        public ElementImagesAppService(
            ICurrentUser currentUser,
            IElementImagesRepository elementImagesRepository,
            IAuditTrailService auditTrailService)
        {
            _currentUser = currentUser;
            _elementImagesRepository = elementImagesRepository;
            _auditTrailService = auditTrailService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 查詢：元素圖片列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<ElementImageListResponse>> GetElementImageListByPage(ElementImageListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<ElementImageListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Keyword = sanitizer.Sanitize(request.Keyword);
                request.Keyword = HttpUtility.UrlDecode(request.Keyword);

                // ========= 業務邏輯執行 =========
                ElementImageListResponse elementImageList = await _elementImagesRepository.GetElementImageListByPage(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("元素圖片查詢", "DATA_READ", "查詢了" + elementImageList.ItemTotalCount + "筆元素圖片資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = elementImageList;
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
        /// 查詢：公開元素圖片列表
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 公開圖片列表 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<ElementImageListResponse>> GetPublicElementImages(ElementImageListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<ElementImageListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Keyword = sanitizer.Sanitize(request.Keyword);
                request.Keyword = HttpUtility.UrlDecode(request.Keyword);

                // ========= 業務邏輯執行 =========
                ElementImageListResponse elementImageList = await _elementImagesRepository.GetPublicElementImages(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("公開元素圖片查詢", "DATA_READ", "查詢了" + elementImageList.ItemTotalCount + "筆公開元素圖片資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = elementImageList;
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
        /// 查詢：依分類取得元素圖片
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 分類圖片列表 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<ElementImageListResponse>> GetElementImagesByCategory(ElementImageCategoryRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<ElementImageListResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (string.IsNullOrEmpty(request.Category)) throw new BusinessException(message: "分類不能為空");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Category = sanitizer.Sanitize(request.Category);
                request.Category = HttpUtility.UrlDecode(request.Category);

                // ========= 業務邏輯執行 =========
                ElementImageListResponse elementImageList = await _elementImagesRepository.GetElementImagesByCategory(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("分類元素圖片查詢", "DATA_READ", "查詢了分類[" + request.Category + "]的" + elementImageList.ItemTotalCount + "筆元素圖片資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = elementImageList;
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
        /// 查詢：單一元素圖片
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 圖片詳情 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<ElementImageResponse>> GetElementImageById(ElementImageRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<ElementImageResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (request.Id <= 0) throw new BusinessException(message: "ID無效");

                // ========= 業務邏輯執行 =========
                ElementImageResponse elementImage = await _elementImagesRepository.GetElementImageById(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("元素圖片詳情查詢", "DATA_READ", "查詢了元素圖片ID[" + request.Id + "]的詳情");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = elementImage;
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
        /// 新增：元素圖片
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 新增結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<CreateElementImageResponse>> CreateElementImage(CreateElementImageRequest createRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<CreateElementImageResponse> response = new();
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
                CreateElementImageResponse createElementImageResponse = await _elementImagesRepository.CreateElementImage(createRequest);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("元素圖片新增", "DATA_CREATE", "新增了元素圖片[" + createRequest.Name + "]");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "新增成功";
                response.Data = createElementImageResponse;
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
        /// 更新：元素圖片
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 更新結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<UpdateElementImageResponse>> UpdateElementImage(UpdateElementImageRequest updateRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<UpdateElementImageResponse> response = new();
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
                UpdateElementImageResponse updateElementImageResponse = await _elementImagesRepository.UpdateElementImage(updateRequest);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("元素圖片更新", "DATA_UPDATE", "更新了元素圖片ID[" + updateRequest.Id + "]");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "更新成功";
                response.Data = updateElementImageResponse;
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
        /// 刪除：元素圖片
        /// </summary>
        /// <param name="request"> 刪除條件 </param>
        /// <returns> 刪除結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.TemplateMgmt)]
        public async Task<BusinessLogicResponse<DeleteElementImageResponse>> DeleteElementImage(DeleteElementImageRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeleteElementImageResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (request.Id <= 0) throw new BusinessException(message: "ID無效");

                // ========= 業務邏輯執行 =========
                DeleteElementImageResponse deleteElementImageResponse = await _elementImagesRepository.DeleteElementImage(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("元素圖片刪除", "DATA_DELETE", "刪除了元素圖片ID[" + request.Id + "]");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "刪除成功";
                response.Data = deleteElementImageResponse;
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