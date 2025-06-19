//-----
// <copyright file="ITemplateRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend.Templates;
using Hamastar.SmartNameplate.Dto.Backend.Templates.Request;
using Hamastar.SmartNameplate.Dto.Backend.Templates.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Hamastar.SmartNameplate.Repositories.Templates
{
    /// <summary>
    /// 模板儲存庫介面
    /// </summary>
    public interface ITemplateRepository : IRepository<Entities.Template, Guid>
    {
        #region Methods

        /// <summary>
        /// 查詢：模板列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        Task<TemplateListResponse> GetListByPage(TemplateListRequest request);

        /// <summary>
        /// 查詢：公開模板列表
        /// </summary>
        /// <returns> 公開模板列表 </returns>
        Task<List<TemplateItem>> GetPublicTemplates();

        /// <summary>
        /// 查詢：依分類取得模板
        /// </summary>
        /// <param name="category"> 分類名稱 </param>
        /// <returns> 模板列表 </returns>
        Task<List<TemplateItem>> GetTemplatesByCategory(string category);

        /// <summary>
        /// 查詢：單一模板
        /// </summary>
        /// <param name="id"> 模板ID </param>
        /// <returns> 模板資料 </returns>
        Task<TemplateItem?> GetTemplateById(Guid id);

        /// <summary>
        /// 查詢公開模板 (Async版本)
        /// </summary>
        /// <returns> 公開模板列表 </returns>
        Task<List<TemplateItem>> GetPublicTemplatesAsync();

        /// <summary>
        /// 根據分類查詢模板 (Async版本)
        /// </summary>
        /// <param name="category"> 分類名稱 </param>
        /// <returns> 模板列表 </returns>
        Task<List<TemplateItem>> GetTemplatesByCategoryAsync(string category);

        #endregion Methods
    }
} 