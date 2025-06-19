//-----
// <copyright file="ICardAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Cards.Request;
using Hamastar.SmartNameplate.Dto.Backend.Cards.Response;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Hamastar.SmartNameplate.IApplication.Cards
{
    /// <summary>
    /// 卡片 App Service 介面
    /// </summary>
    public interface ICardAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 查詢：卡片列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        Task<BusinessLogicResponse<CardListResponse>> GetCardListByPage(CardListRequest request);

        /// <summary>
        /// 查詢：單一卡片
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 卡片資訊 </returns>
        Task<BusinessLogicResponse<CardResponse>> GetCard(CardRequest request);

        /// <summary>
        /// 新增：卡片
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<CreateCardResponse>> CreateCard(CreateCardRequest createRequest);

        /// <summary>
        /// 更新：卡片
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<UpdateCardResponse>> UpdateCard(UpdateCardRequest updateRequest);

        /// <summary>
        /// 刪除：卡片
        /// </summary>
        /// <param name="deleteRequest"> 刪除條件 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<DeleteCardResponse>> DeleteCard(DeleteCardRequest deleteRequest);

        #endregion Methods
    }
} 