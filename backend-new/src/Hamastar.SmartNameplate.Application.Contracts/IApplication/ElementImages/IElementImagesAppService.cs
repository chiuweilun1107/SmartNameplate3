//-----
// <copyright file="IElementImagesAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.ElementImages.Request;
using Hamastar.SmartNameplate.Dto.Backend.ElementImages.Response;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Hamastar.SmartNameplate.IApplication.ElementImages
{
    /// <summary>
    /// 元素圖片 App Service 介面
    /// </summary>
    public interface IElementImagesAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 查詢：元素圖片列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        Task<BusinessLogicResponse<ElementImageListResponse>> GetElementImageListByPage(ElementImageListRequest request);

        /// <summary>
        /// 查詢：公開元素圖片列表
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 公開圖片列表 </returns>
        Task<BusinessLogicResponse<ElementImageListResponse>> GetPublicElementImages(ElementImageListRequest request);

        /// <summary>
        /// 查詢：依分類取得元素圖片
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 分類圖片列表 </returns>
        Task<BusinessLogicResponse<ElementImageListResponse>> GetElementImagesByCategory(ElementImageCategoryRequest request);

        /// <summary>
        /// 查詢：單一元素圖片
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 圖片詳情 </returns>
        Task<BusinessLogicResponse<ElementImageResponse>> GetElementImageById(ElementImageRequest request);

        /// <summary>
        /// 新增：元素圖片
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 新增結果 </returns>
        Task<BusinessLogicResponse<CreateElementImageResponse>> CreateElementImage(CreateElementImageRequest createRequest);

        /// <summary>
        /// 更新：元素圖片
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 更新結果 </returns>
        Task<BusinessLogicResponse<UpdateElementImageResponse>> UpdateElementImage(UpdateElementImageRequest updateRequest);

        /// <summary>
        /// 刪除：元素圖片
        /// </summary>
        /// <param name="request"> 刪除條件 </param>
        /// <returns> 刪除結果 </returns>
        Task<BusinessLogicResponse<DeleteElementImageResponse>> DeleteElementImage(DeleteElementImageRequest request);

        #endregion Methods
    }
} 