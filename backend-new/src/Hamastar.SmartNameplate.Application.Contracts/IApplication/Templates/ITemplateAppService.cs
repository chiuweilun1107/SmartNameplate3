//-----
// <copyright file="ITemplateAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Templates.Request;
using Hamastar.SmartNameplate.Dto.Backend.Templates.Response;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Hamastar.SmartNameplate.IApplication.Templates
{
    /// <summary>
    /// 模板 App Service 介面
    /// </summary>
    public interface ITemplateAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 查詢：模板列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        Task<BusinessLogicResponse<TemplateListResponse>> GetTemplateListByPage(TemplateListRequest request);

        /// <summary>
        /// 查詢：公開模板列表
        /// </summary>
        /// <returns> 公開模板列表 </returns>
        Task<BusinessLogicResponse<TemplateListResponse>> GetPublicTemplates();

        /// <summary>
        /// 查詢：根據分類取得模板列表
        /// </summary>
        /// <param name="request"> 分類查詢請求 </param>
        /// <returns> 模板列表 </returns>
        Task<BusinessLogicResponse<TemplateListResponse>> GetTemplatesByCategory(TemplateCategoryRequest request);

        /// <summary>
        /// 新增：模板
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<CreateTemplateResponse>> CreateTemplate(CreateTemplateRequest createRequest);

        /// <summary>
        /// 更新：模板
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<UpdateTemplateResponse>> UpdateTemplate(UpdateTemplateRequest updateRequest);

        /// <summary>
        /// 刪除：模板
        /// </summary>
        /// <param name="deleteRequest"> 刪除條件 </param>
        /// <returns> 成功與否 </returns>
        Task<BusinessLogicResponse<DeleteTemplateResponse>> DeleteTemplate(DeleteTemplateRequest deleteRequest);

        #endregion Methods
    }
} 