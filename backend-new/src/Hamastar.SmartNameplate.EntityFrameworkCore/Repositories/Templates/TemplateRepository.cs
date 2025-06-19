//-----
// <copyright file="TemplateRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend.Templates;
using Hamastar.SmartNameplate.Dto.Backend.Templates.Request;
using Hamastar.SmartNameplate.Dto.Backend.Templates.Response;
using Hamastar.SmartNameplate.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.Repositories.Templates
{
    /// <summary>
    /// 模板儲存庫
    /// </summary>
    public class TemplateRepository : EfCoreRepository<SmartNameplateDbContext, Entities.Template, Guid>, ITemplateRepository
    {
        #region Fields

        /// <summary>
        /// SettingProvider
        /// </summary>
        private readonly IConfiguration _appConfiguration;

        /// <summary>
        /// UnitOfWorkManager
        /// </summary>
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger log = Log.ForContext<TemplateRepository>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateRepository" /> class
        /// </summary>
        /// <param name="appConfiguration"> SettingProvider </param>
        /// <param name="contextProvider"> dbContext </param>
        /// <param name="unitOfWorkManager"> Unit of Work Manager </param>
        /// <param name="currentUser"> 目前登入的使用者 </param>
        public TemplateRepository(IConfiguration appConfiguration,
            IDbContextProvider<SmartNameplateDbContext> contextProvider,
            IUnitOfWorkManager unitOfWorkManager,
            ICurrentUser currentUser) : base(contextProvider)
        {
            _appConfiguration = appConfiguration;
            _unitOfWorkManager = unitOfWorkManager;
            _currentUser = currentUser;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 查詢：模板列表(頁數) - 參考原始 GetTemplatesAsync
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        public async Task<TemplateListResponse> GetListByPage(TemplateListRequest request)
        {
            var dbContext = await GetDbContextAsync();
            IQueryable<Entities.Template> query = dbContext.Templates;

            // 基本篩選條件 - 只顯示啟用的模板
            query = query.Where(t => t.IsActive);

            // 關鍵字搜尋
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(t => t.Name.Contains(request.Keyword) || 
                                        (t.Description != null && t.Description.Contains(request.Keyword)) ||
                                        (t.Category != null && t.Category.Contains(request.Keyword)));
            }

            // 分類篩選 - 參考原始 GetTemplatesByCategoryAsync
            if (!string.IsNullOrWhiteSpace(request.Category) && request.Category != "全部")
            {
                query = query.Where(t => t.Category == request.Category);
            }

            // 公開狀態篩選
            if (request.IsPublicOnly.HasValue && request.IsPublicOnly.Value)
            {
                query = query.Where(t => t.IsPublic);
            }

            // 總筆數
            int totalCount = await query.CountAsync();

            // 排序 - 參考原始邏輯
            query = query.OrderByDescending(t => t.CreationTime);

            // 分頁
            var templates = await query
                .Skip((request.PageIndex) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TemplateItem
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description ?? "",
                    ThumbnailUrl = t.ThumbnailUrl ?? "",
                    ThumbnailA = t.ThumbnailA ?? "",
                    ThumbnailB = t.ThumbnailB ?? "",
                    Category = t.Category ?? "",
                    IsPublic = t.IsPublic,
                    Dimensions = t.Dimensions ?? "",
                    CreationTime = t.CreationTime,
                    CreatorId = t.CreatorId,
                    IsActive = t.IsActive
                })
                .ToListAsync();

            return new TemplateListResponse(templates, totalCount, request.PageIndex, request.PageSize);
        }

        /// <summary>
        /// 查詢：公開模板列表 - 參考原始 GetPublicTemplatesAsync
        /// </summary>
        /// <returns> 公開模板列表 </returns>
        public async Task<List<TemplateItem>> GetPublicTemplates()
        {
            var dbContext = await GetDbContextAsync();
            
            return await dbContext.Templates
                .Where(t => t.IsActive && t.IsPublic)
                .OrderByDescending(t => t.CreationTime)
                .Select(t => new TemplateItem
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description ?? "",
                    ThumbnailUrl = t.ThumbnailUrl ?? "",
                    ThumbnailA = t.ThumbnailA ?? "",
                    ThumbnailB = t.ThumbnailB ?? "",
                    LayoutDataA = t.LayoutDataA ?? "",
                    LayoutDataB = t.LayoutDataB ?? "",
                    Category = t.Category ?? "",
                    IsPublic = t.IsPublic,
                    Dimensions = t.Dimensions ?? "",
                    CreationTime = t.CreationTime,
                    CreatorId = t.CreatorId,
                    IsActive = t.IsActive
                })
                .ToListAsync();
        }

        /// <summary>
        /// 查詢：依分類取得模板 - 參考原始 GetTemplatesByCategoryAsync
        /// </summary>
        /// <param name="category"> 分類名稱 </param>
        /// <returns> 模板列表 </returns>
        public async Task<List<TemplateItem>> GetTemplatesByCategory(string category)
        {
            var dbContext = await GetDbContextAsync();
            IQueryable<Entities.Template> query = dbContext.Templates;

            // 基本篩選
            query = query.Where(t => t.IsActive && t.IsPublic);

            // 分類篩選
            if (!string.IsNullOrEmpty(category) && category != "全部")
            {
                query = query.Where(t => t.Category == category);
            }

            return await query
                .OrderByDescending(t => t.CreationTime)
                .Select(t => new TemplateItem
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description ?? "",
                    ThumbnailUrl = t.ThumbnailUrl ?? "",
                    ThumbnailA = t.ThumbnailA ?? "",
                    ThumbnailB = t.ThumbnailB ?? "",
                    LayoutDataA = t.LayoutDataA ?? "",
                    LayoutDataB = t.LayoutDataB ?? "",
                    Category = t.Category ?? "",
                    IsPublic = t.IsPublic,
                    Dimensions = t.Dimensions ?? "",
                    CreationTime = t.CreationTime,
                    CreatorId = t.CreatorId,
                    IsActive = t.IsActive
                })
                .ToListAsync();
        }

        /// <summary>
        /// 查詢：單一模板 - 參考原始 GetTemplateByIdAsync
        /// </summary>
        /// <param name="id"> 模板ID </param>
        /// <returns> 模板資料 </returns>
        public async Task<TemplateItem?> GetTemplateById(Guid id)
        {
            var dbContext = await GetDbContextAsync();
            
            return await dbContext.Templates
                .Where(t => t.Id == id && t.IsActive)
                .Select(t => new TemplateItem
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description ?? "",
                    ThumbnailUrl = t.ThumbnailUrl ?? "",
                    ThumbnailA = t.ThumbnailA ?? "",
                    ThumbnailB = t.ThumbnailB ?? "",
                    LayoutDataA = t.LayoutDataA ?? "",
                    LayoutDataB = t.LayoutDataB ?? "",
                    Category = t.Category ?? "",
                    IsPublic = t.IsPublic,
                    Dimensions = t.Dimensions ?? "",
                    CreationTime = t.CreationTime,
                    CreatorId = t.CreatorId,
                    IsActive = t.IsActive
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 查詢公開模板
        /// </summary>
        /// <returns> 公開模板列表 </returns>
        public async Task<List<TemplateItem>> GetPublicTemplatesAsync()
        {
            var dbContext = await GetDbContextAsync();
            
            return await dbContext.Templates
                .Where(t => t.IsPublic && t.IsActive)
                .OrderByDescending(t => t.CreationTime)
                .Select(t => new TemplateItem
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description ?? "",
                    Category = t.Category ?? "",
                    IsPublic = t.IsPublic,
                    CreationTime = t.CreationTime,
                    CreatorId = t.CreatorId,
                    IsActive = t.IsActive
                })
                .ToListAsync();
        }

        /// <summary>
        /// 根據分類查詢模板
        /// </summary>
        /// <param name="category"> 分類名稱 </param>
        /// <returns> 模板列表 </returns>
        public async Task<List<TemplateItem>> GetTemplatesByCategoryAsync(string category)
        {
            var dbContext = await GetDbContextAsync();
            IQueryable<Entities.Template> query = dbContext.Templates;

            // 基本篩選
            query = query.Where(t => t.IsActive);

            // 分類篩選
            if (!string.IsNullOrEmpty(category) && category != "全部")
            {
                query = query.Where(t => t.Category == category);
            }

            return await query
                .OrderByDescending(t => t.CreationTime)
                .Select(t => new TemplateItem
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description ?? "",
                    Category = t.Category ?? "",
                    IsPublic = t.IsPublic,
                    CreationTime = t.CreationTime,
                    CreatorId = t.CreatorId,
                    IsActive = t.IsActive
                })
                .ToListAsync();
        }

        #endregion Public Methods

        #region Private Methods

        // 私有輔助方法

        #endregion Private Methods
    }
} 