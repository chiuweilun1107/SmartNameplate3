//-----
// <copyright file="IBackgroundImagesAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Request;
using Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Response;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Hamastar.SmartNameplate.IApplication.BackgroundImages
{
    /// <summary>
    /// 背景圖片 App Service 介面
    /// </summary>
    public interface IBackgroundImagesAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 查詢：背景圖片列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        Task<BusinessLogicResponse<BackgroundImageListResponse>> GetBackgroundImageListByPage(BackgroundImageListRequest request);

        /// <summary>
        /// 查詢：公開背景圖片列表
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 公開圖片列表 </returns>
        Task<BusinessLogicResponse<BackgroundImageListResponse>> GetPublicBackgroundImages(BackgroundImageListRequest request);

        /// <summary>
        /// 查詢：依分類取得背景圖片
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 分類圖片列表 </returns>
        Task<BusinessLogicResponse<BackgroundImageListResponse>> GetBackgroundImagesByCategory(BackgroundImageCategoryRequest request);

        /// <summary>
        /// 查詢：單一背景圖片
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 圖片詳情 </returns>
        Task<BusinessLogicResponse<BackgroundImageResponse>> GetBackgroundImageById(BackgroundImageRequest request);

        /// <summary>
        /// 新增：背景圖片
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 新增結果 </returns>
        Task<BusinessLogicResponse<CreateBackgroundImageResponse>> CreateBackgroundImage(CreateBackgroundImageRequest createRequest);

        /// <summary>
        /// 更新：背景圖片
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 更新結果 </returns>
        Task<BusinessLogicResponse<UpdateBackgroundImageResponse>> UpdateBackgroundImage(UpdateBackgroundImageRequest updateRequest);

        /// <summary>
        /// 刪除：背景圖片
        /// </summary>
        /// <param name="request"> 刪除條件 </param>
        /// <returns> 刪除結果 </returns>
        Task<BusinessLogicResponse<DeleteBackgroundImageResponse>> DeleteBackgroundImage(DeleteBackgroundImageRequest request);

        #endregion Methods
    }
} 