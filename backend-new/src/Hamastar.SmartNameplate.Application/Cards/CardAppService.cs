//-----
// <copyright file="CardAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Cards;
using Hamastar.SmartNameplate.Dto.Backend.Cards.Request;
using Hamastar.SmartNameplate.Dto.Backend.Cards.Response;
using Hamastar.SmartNameplate.IApplication.Cards;
using Hamastar.SmartNameplate.IApplication.AuditTrails;
using Hamastar.SmartNameplate.Permissions;
using Hamastar.SmartNameplate.Repositories.Cards;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.Cards
{
    /// <summary>
    /// 卡片 App
    /// </summary>
    public class CardAppService : ApplicationService, ICardAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<CardAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 卡片儲存庫
        /// </summary>
        private readonly ICardRepository _cardRepository;

        /// <summary>
        /// 審計軌跡資料 APP
        /// </summary>
        private readonly IAuditTrailService _auditTrailService;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CardAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="cardRepository"> 卡片儲存庫 </param>
        /// <param name="auditTrailService"> 審計軌跡資料 APP </param>
        public CardAppService(
            ICurrentUser currentUser,
            ICardRepository cardRepository,
            IAuditTrailService auditTrailService)
        {
            _currentUser = currentUser;
            _cardRepository = cardRepository;
            _auditTrailService = auditTrailService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 查詢：卡片列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<CardListResponse>> GetCardListByPage(CardListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<CardListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Keyword = sanitizer.Sanitize(request.Keyword);
                request.Keyword = HttpUtility.UrlDecode(request.Keyword);

                // ========= 業務邏輯執行 =========
                CardListResponse cardList = await _cardRepository.GetListByPage(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("卡片查詢", "DATA_READ", "查詢了" + cardList.ItemTotalCount + "筆卡片資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = cardList;
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
        /// 查詢：單一卡片
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 卡片資訊 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<CardResponse>> GetCard(CardRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<CardResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                var card = await _cardRepository.GetAsync(request.Id);
                if (card == null)
                {
                    throw new BusinessException(message: "卡片不存在");
                }

                CardResponse cardResponse = new()
                {
                    Card = new CardItem
                    {
                        Id = card.Id,
                        Name = card.Name,
                        Description = card.Description,
                        Status = card.Status,
                        ThumbnailA = card.ThumbnailA,
                        ThumbnailB = card.ThumbnailB,
                        ContentA = card.ContentA,
                        ContentB = card.ContentB,
                        IsSameBothSides = card.IsSameBothSides,
                        CreationTime = card.CreationTime,
                        LastModificationTime = card.LastModificationTime
                    }
                };

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("卡片查詢", "DATA_READ", "查詢了卡片：" + card.Name);
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = cardResponse;
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
        /// 新增：卡片
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<CreateCardResponse>> CreateCard(CreateCardRequest createRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<CreateCardResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (createRequest.Name.Length > 200) throw new BusinessException(message: "卡片名稱過長 Max 200");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();

                // ========= 建立實體 =========
                Entities.Card createCard = new()
                {
                    Name = sanitizer.Sanitize(createRequest.Name),
                    Description = sanitizer.Sanitize(createRequest.Description),
                    Status = createRequest.Status,
                    ThumbnailA = createRequest.ThumbnailA,
                    ThumbnailB = createRequest.ThumbnailB,
                    ContentA = createRequest.ContentA,
                    ContentB = createRequest.ContentB,
                    IsSameBothSides = createRequest.IsSameBothSides,
                    CreationTime = DateTime.Now,
                    CreatorUserId = CurrentUser.Id ?? Guid.Empty,
                    LastModificationTime = DateTime.Now,
                    LastModifierUserId = CurrentUser.Id ?? Guid.Empty
                };

                // ========= 執行新增 =========
                CreateCardResponse createCardResponse = new();
                var result = await _cardRepository.InsertAsync(createCard);
                if (result != null)
                {
                    await CreateAuditTrail("卡片新增", "DATA_CREATE", "新增了卡片：" + createCard.Name + " 且 成功");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Status = "success";
                    response.Message = "新增成功";
                    createCardResponse.Result = true;
                    createCardResponse.CardId = result.Id;
                }
                else
                {
                    await CreateAuditTrail("卡片新增", "DATA_CREATE", "新增了卡片：" + createCard.Name + " 且 失敗");
                    response.Status = "error";
                    response.Message = "新增失敗";
                    createCardResponse.Result = false;
                    await uow.RollbackAsync();
                }
                await uow.CompleteAsync();
                response.Data = createCardResponse;
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
        /// 更新：卡片
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<UpdateCardResponse>> UpdateCard(UpdateCardRequest updateRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<UpdateCardResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (!string.IsNullOrEmpty(updateRequest.Name) && updateRequest.Name.Length > 200) 
                    throw new BusinessException(message: "卡片名稱過長 Max 200");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();

                // ========= 取得實體 =========
                var card = await _cardRepository.GetAsync(updateRequest.Id);
                if (card == null)
                {
                    throw new BusinessException(message: "卡片不存在");
                }

                // ========= 更新實體 =========
                if (!string.IsNullOrEmpty(updateRequest.Name))
                    card.Name = sanitizer.Sanitize(updateRequest.Name);
                
                if (updateRequest.Description != null)
                    card.Description = sanitizer.Sanitize(updateRequest.Description);
                
                if (updateRequest.Status.HasValue)
                    card.Status = updateRequest.Status.Value;
                
                if (updateRequest.ThumbnailA != null)
                    card.ThumbnailA = updateRequest.ThumbnailA;
                
                if (updateRequest.ThumbnailB != null)
                    card.ThumbnailB = updateRequest.ThumbnailB;
                
                if (updateRequest.ContentA != null)
                    card.ContentA = updateRequest.ContentA;
                    
                if (updateRequest.ContentB != null)
                    card.ContentB = updateRequest.ContentB;

                if (updateRequest.IsSameBothSides.HasValue)
                    card.IsSameBothSides = updateRequest.IsSameBothSides.Value;

                card.LastModificationTime = DateTime.Now;
                card.LastModifierUserId = CurrentUser.Id ?? Guid.Empty;

                // ========= 執行更新 =========
                UpdateCardResponse updateCardResponse = new();
                var result = await _cardRepository.UpdateAsync(card);
                if (result != null)
                {
                    await CreateAuditTrail("卡片更新", "DATA_UPDATE", "更新了卡片：" + card.Name + " 且 成功");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Status = "success";
                    response.Message = "更新成功";
                    updateCardResponse.Result = true;
                }
                else
                {
                    await CreateAuditTrail("卡片更新", "DATA_UPDATE", "更新了卡片：" + card.Name + " 且 失敗");
                    response.Status = "error";
                    response.Message = "更新失敗";
                    updateCardResponse.Result = false;
                    await uow.RollbackAsync();
                }
                await uow.CompleteAsync();
                response.Data = updateCardResponse;
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
        /// 刪除：卡片
        /// </summary>
        /// <param name="deleteRequest"> 刪除條件 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.CardMgmt)]
        public async Task<BusinessLogicResponse<DeleteCardResponse>> DeleteCard(DeleteCardRequest deleteRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeleteCardResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                var card = await _cardRepository.GetAsync(deleteRequest.Id);
                if (card == null)
                {
                    throw new BusinessException(message: "卡片不存在");
                }

                string cardName = card.Name;
                await _cardRepository.DeleteAsync(card);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("卡片刪除", "DATA_DELETE", "刪除了卡片：" + cardName + " 且 成功");
                await CurrentUnitOfWork.SaveChangesAsync();
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                DeleteCardResponse deleteCardResponse = new()
                {
                    Result = true
                };
                response.Status = "success";
                response.Message = "刪除成功";
                response.Data = deleteCardResponse;
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