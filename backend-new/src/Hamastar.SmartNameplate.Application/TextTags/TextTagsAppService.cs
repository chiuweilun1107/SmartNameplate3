//-----
// <copyright file="TextTagsAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.TextTags.Request;
using Hamastar.SmartNameplate.Dto.Backend.TextTags.Response;
using Hamastar.SmartNameplate.IApplication.TextTags;
using Hamastar.SmartNameplate.Permissions;
using Hamastar.SmartNameplate.Repositories.TextTags;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.TextTags
{
    /// <summary>
    /// 文字標籤 App Service
    /// </summary>
    public class TextTagsAppService : ApplicationService, ITextTagsAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<TextTagsAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 文字標籤儲存庫
        /// </summary>
        private readonly ITextTagsRepository _textTagsRepository;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTagsAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="textTagsRepository"> 文字標籤儲存庫 </param>
        public TextTagsAppService(
            ICurrentUser currentUser,
            ITextTagsRepository textTagsRepository)
        {
            _currentUser = currentUser;
            _textTagsRepository = textTagsRepository;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 查詢：卡片文字元素列表
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 文字元素列表 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<TextTagListResponse>> GetCardTextElements(TextTagListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<TextTagListResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (request.CardId <= 0) throw new BusinessException(message: "卡片ID無效");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Side = sanitizer.Sanitize(request.Side);
                request.Side = HttpUtility.UrlDecode(request.Side);

                // ========= 業務邏輯執行 =========
                TextTagListResponse textTagList = await _textTagsRepository.GetCardTextElements(request);

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = textTagList;
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
        /// 查詢：單一卡片文字元素
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 文字元素詳情 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<TextTagResponse>> GetCardTextElement(TextTagRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<TextTagResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (request.CardId <= 0) throw new BusinessException(message: "卡片ID無效");
                if (string.IsNullOrEmpty(request.ElementId)) throw new BusinessException(message: "元素ID不能為空");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Side = sanitizer.Sanitize(request.Side);
                request.ElementId = sanitizer.Sanitize(request.ElementId);

                // ========= 業務邏輯執行 =========
                TextTagResponse textTag = await _textTagsRepository.GetCardTextElement(request);

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = textTag;
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
        /// 新增：卡片文字元素
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 新增結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<CreateTextTagResponse>> CreateCardTextElement(CreateTextTagRequest createRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<CreateTextTagResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (createRequest.CardId <= 0) throw new BusinessException(message: "卡片ID無效");
                if (string.IsNullOrEmpty(createRequest.TagType)) throw new BusinessException(message: "標籤類型不能為空");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                createRequest.TagType = sanitizer.Sanitize(createRequest.TagType);
                createRequest.TagLabel = sanitizer.Sanitize(createRequest.TagLabel);
                createRequest.DefaultContent = sanitizer.Sanitize(createRequest.DefaultContent);

                // ========= 業務邏輯執行 =========
                CreateTextTagResponse createTextTagResponse = await _textTagsRepository.CreateCardTextElement(createRequest);

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "新增成功";
                response.Data = createTextTagResponse;
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
        /// 更新：卡片文字元素
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 更新結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<UpdateTextTagResponse>> UpdateCardTextElement(UpdateTextTagRequest updateRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<UpdateTextTagResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (updateRequest.Id <= 0) throw new BusinessException(message: "ID無效");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                updateRequest.TagType = sanitizer.Sanitize(updateRequest.TagType);
                updateRequest.TagLabel = sanitizer.Sanitize(updateRequest.TagLabel);
                updateRequest.DefaultContent = sanitizer.Sanitize(updateRequest.DefaultContent);

                // ========= 業務邏輯執行 =========
                UpdateTextTagResponse updateTextTagResponse = await _textTagsRepository.UpdateCardTextElement(updateRequest);

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "更新成功";
                response.Data = updateTextTagResponse;
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
        /// 刪除：卡片文字元素
        /// </summary>
        /// <param name="request"> 刪除條件 </param>
        /// <returns> 刪除結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<DeleteTextTagResponse>> DeleteCardTextElement(DeleteTextTagRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeleteTextTagResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (request.Id <= 0) throw new BusinessException(message: "ID無效");

                // ========= 業務邏輯執行 =========
                DeleteTextTagResponse deleteTextTagResponse = await _textTagsRepository.DeleteCardTextElement(request);

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "刪除成功";
                response.Data = deleteTextTagResponse;
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
        /// 查詢：卡片實例資料
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 實例資料列表 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<CardInstanceDataResponse>> GetCardInstanceData(CardInstanceDataRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<CardInstanceDataResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (request.CardId <= 0) throw new BusinessException(message: "卡片ID無效");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.InstanceName = sanitizer.Sanitize(request.InstanceName);

                // ========= 業務邏輯執行 =========
                CardInstanceDataResponse cardInstanceData = await _textTagsRepository.GetCardInstanceData(request);

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = cardInstanceData;
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
        /// 儲存：卡片實例資料
        /// </summary>
        /// <param name="request"> 儲存資訊 </param>
        /// <returns> 儲存結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<SaveCardInstanceDataResponse>> SaveCardInstanceData(SaveCardInstanceDataRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<SaveCardInstanceDataResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (request.CardId <= 0) throw new BusinessException(message: "卡片ID無效");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.InstanceName = sanitizer.Sanitize(request.InstanceName);
                request.ContentValue = sanitizer.Sanitize(request.ContentValue);

                // ========= 業務邏輯執行 =========
                SaveCardInstanceDataResponse saveCardInstanceDataResponse = await _textTagsRepository.SaveCardInstanceData(request);

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "儲存成功";
                response.Data = saveCardInstanceDataResponse;
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
        /// 刪除：卡片實例
        /// </summary>
        /// <param name="request"> 刪除條件 </param>
        /// <returns> 刪除結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<DeleteCardInstanceResponse>> DeleteCardInstance(DeleteCardInstanceRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeleteCardInstanceResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (request.CardId <= 0) throw new BusinessException(message: "卡片ID無效");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.InstanceName = sanitizer.Sanitize(request.InstanceName);

                // ========= 業務邏輯執行 =========
                DeleteCardInstanceResponse deleteCardInstanceResponse = await _textTagsRepository.DeleteCardInstance(request);

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "刪除成功";
                response.Data = deleteCardInstanceResponse;
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
    }
} 