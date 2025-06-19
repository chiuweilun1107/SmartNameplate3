//-----
// <copyright file="CardRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Cards;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Cards.Request;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Cards.Response;
using Hamastar.SmartNameplate.EntityFrameworkCore;
using Hamastar.SmartNameplate.Entities;
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

namespace Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Cards
{
    /// <summary>
    /// 🤖 卡片儲存庫實作
    /// </summary>
    public class CardRepository : EfCoreRepository<SmartNameplateDbContext, Card, Guid>, ICardRepository
    {
        #region Fields

        /// <summary>
        /// 應用程式配置
        /// </summary>
        private readonly IConfiguration _appConfiguration;

        /// <summary>
        /// 工作單元管理器
        /// </summary>
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// 日誌記錄器
        /// </summary>
        private readonly ILogger log = Log.ForContext<CardRepository>();

        /// <summary>
        /// 當前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="appConfiguration"> 應用程式配置 </param>
        /// <param name="contextProvider"> 資料庫上下文提供者 </param>
        /// <param name="unitOfWorkManager"> 工作單元管理器 </param>
        /// <param name="currentUser"> 當前使用者 </param>
        public CardRepository(IConfiguration appConfiguration,
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
        /// 查詢：卡片列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        public async Task<CardListResponse> GetListByPage(CardListRequest request)
        {
            var dbContext = await GetDbContextAsync();
            IQueryable<Card> query = dbContext.Cards;

            // 關鍵字搜尋
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword) || 
                                        (x.Description != null && x.Description.Contains(request.Keyword)));
            }

            // 狀態篩選
            if (request.Status.HasValue)
            {
                query = query.Where(x => (int)x.Status == request.Status.Value);
            }

            // 總數計算
            var totalCount = await query.CountAsync();

            // 分頁處理
            var items = await query
                .OrderByDescending(x => x.LastModificationTime)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new CardItemForListByPage
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Status = (int)x.Status,
                    ThumbnailA = x.ThumbnailA,
                    ThumbnailB = x.ThumbnailB,
                    IsSameBothSides = x.IsSameBothSides,
                    CreationTime = x.CreationTime,
                    LastModificationTime = x.LastModificationTime ?? DateTime.UtcNow,
                    CreatorId = x.CreatorId
                })
                .ToListAsync();

            return new CardListResponse(items, totalCount, request.Page - 1, request.PageSize);
        }

        /// <summary>
        /// 查詢：根據狀態取得卡片列表
        /// </summary>
        /// <param name="status"> 卡片狀態 </param>
        /// <returns> 卡片列表 </returns>
        public async Task<IEnumerable<Card>> GetCardsByStatusAsync(CardStatus status)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.Cards
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.LastModificationTime)
                .ToListAsync();
        }

        #endregion Public Methods

        #region Private Methods

        // 私有輔助方法

        #endregion Private Methods
    }
} 