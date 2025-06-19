//-----
// <copyright file="ITextTagsAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.TextTags.Request;
using Hamastar.SmartNameplate.Dto.Backend.TextTags.Response;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Hamastar.SmartNameplate.IApplication.TextTags
{
    /// <summary>
    /// 文字標籤 App Service 介面
    /// </summary>
    public interface ITextTagsAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 查詢：卡片文字元素列表
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 文字元素列表 </returns>
        Task<BusinessLogicResponse<TextTagListResponse>> GetCardTextElements(TextTagListRequest request);

        /// <summary>
        /// 查詢：單一卡片文字元素
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 文字元素詳情 </returns>
        Task<BusinessLogicResponse<TextTagResponse>> GetCardTextElement(TextTagRequest request);

        /// <summary>
        /// 新增：卡片文字元素
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 新增結果 </returns>
        Task<BusinessLogicResponse<CreateTextTagResponse>> CreateCardTextElement(CreateTextTagRequest createRequest);

        /// <summary>
        /// 更新：卡片文字元素
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 更新結果 </returns>
        Task<BusinessLogicResponse<UpdateTextTagResponse>> UpdateCardTextElement(UpdateTextTagRequest updateRequest);

        /// <summary>
        /// 刪除：卡片文字元素
        /// </summary>
        /// <param name="request"> 刪除條件 </param>
        /// <returns> 刪除結果 </returns>
        Task<BusinessLogicResponse<DeleteTextTagResponse>> DeleteCardTextElement(DeleteTextTagRequest request);

        /// <summary>
        /// 查詢：卡片實例資料
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 實例資料列表 </returns>
        Task<BusinessLogicResponse<CardInstanceDataResponse>> GetCardInstanceData(CardInstanceDataRequest request);

        /// <summary>
        /// 儲存：卡片實例資料
        /// </summary>
        /// <param name="request"> 儲存資訊 </param>
        /// <returns> 儲存結果 </returns>
        Task<BusinessLogicResponse<SaveCardInstanceDataResponse>> SaveCardInstanceData(SaveCardInstanceDataRequest request);

        /// <summary>
        /// 刪除：卡片實例
        /// </summary>
        /// <param name="request"> 刪除條件 </param>
        /// <returns> 刪除結果 </returns>
        Task<BusinessLogicResponse<DeleteCardInstanceResponse>> DeleteCardInstance(DeleteCardInstanceRequest request);

        #endregion Methods
    }
} 