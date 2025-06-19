//-----
// <copyright file="GroupRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend.Groups;
using Hamastar.SmartNameplate.Dto.Backend.Groups.Request;
using Hamastar.SmartNameplate.Dto.Backend.Groups.Response;
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

namespace Hamastar.SmartNameplate.Repositories.Groups
{
    /// <summary>
    /// 群組儲存庫
    /// </summary>
    public class GroupRepository : EfCoreRepository<SmartNameplateDbContext, Entities.Group, Guid>, IGroupRepository
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
        private readonly ILogger log = Log.ForContext<GroupRepository>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupRepository" /> class
        /// </summary>
        /// <param name="appConfiguration"> SettingProvider </param>
        /// <param name="contextProvider"> dbContext </param>
        /// <param name="unitOfWorkManager"> Unit of Work Manager </param>
        /// <param name="currentUser"> 目前登入的使用者 </param>
        public GroupRepository(IConfiguration appConfiguration,
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
        /// 查詢：群組列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        public async Task<GroupListResponse> GetListByPage(GroupListRequest request)
        {
            var dbContext = await GetDbContextAsync();
            IQueryable<Entities.Group> query = dbContext.Groups;

            // 基本篩選條件 - 只顯示啟用的群組
            query = query.Where(g => g.IsActive);

            // 啟用狀態篩選
            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            // 關鍵字搜尋
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword) || 
                                        (x.Description != null && x.Description.Contains(request.Keyword)));
            }

            // 使用者篩選
            if (request.UserId.HasValue)
            {
                query = query.Where(g => g.CreatorUserId == request.UserId.Value);
            }

            // 總數計算
            var totalCount = await query.CountAsync();

            // 分頁處理
            var items = await query
                .OrderByDescending(x => x.LastModificationTime)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new GroupItemForListByPage
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Color = x.Color ?? "",
                    CardCount = x.GroupCards.Count,
                    DeviceCount = 0, // TODO: 實作裝置數量計算
                    CreationTime = x.CreationTime,
                    LastModificationTime = x.LastModificationTime ?? DateTime.UtcNow,
                    CreatorId = x.CreatorId
                })
                .ToListAsync();

            return new GroupListResponse(items, totalCount, request.Page - 1, request.PageSize);
        }

        /// <summary>
        /// 查詢：根據ID取得群組
        /// </summary>
        /// <param name="id"> 群組ID </param>
        /// <returns> 群組資料 </returns>
        public async Task<GroupItem?> GetGroupById(Guid id)
        {
            var dbContext = await GetDbContextAsync();
            
            return await dbContext.Groups
                .Where(g => g.Id == id && g.IsActive)
                .Select(g => new GroupItem
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    CreationTime = g.CreationTime,
                    CreatorId = g.CreatorId,
                    IsActive = g.IsActive
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 查詢：群組的卡片列表
        /// </summary>
        /// <param name="groupId"> 群組ID </param>
        /// <returns> 卡片列表 </returns>
        public async Task<List<Guid>> GetGroupCardIds(Guid groupId)
        {
            var dbContext = await GetDbContextAsync();
            
            return await dbContext.GroupCards
                .Where(gc => gc.GroupId == groupId)
                .Select(gc => gc.CardId)
                .ToListAsync();
        }

        /// <summary>
        /// 新增：卡片到群組
        /// </summary>
        /// <param name="groupId"> 群組ID </param>
        /// <param name="cardId"> 卡片ID </param>
        /// <returns> 新增結果 </returns>
        public async Task<bool> AddCardToGroup(Guid groupId, Guid cardId)
        {
            var dbContext = await GetDbContextAsync();
            
            // 檢查是否已存在
            var exists = await dbContext.GroupCards
                .AnyAsync(gc => gc.GroupId == groupId && gc.CardId == cardId);
            
            if (exists)
                return false;

            // 新增關聯
            var groupCard = new Entities.GroupCard
            {
                GroupId = groupId,
                CardId = cardId
            };

            dbContext.GroupCards.Add(groupCard);
            await dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 移除：群組中的卡片
        /// </summary>
        /// <param name="groupId"> 群組ID </param>
        /// <param name="cardId"> 卡片ID </param>
        /// <returns> 移除結果 </returns>
        public async Task<bool> RemoveCardFromGroup(Guid groupId, Guid cardId)
        {
            var dbContext = await GetDbContextAsync();
            
            var groupCard = await dbContext.GroupCards
                .FirstOrDefaultAsync(gc => gc.GroupId == groupId && gc.CardId == cardId);
            
            if (groupCard == null)
                return false;

            dbContext.GroupCards.Remove(groupCard);
            await dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 查詢：使用者的群組列表
        /// </summary>
        /// <param name="userId"> 使用者ID </param>
        /// <returns> 群組列表 </returns>
        public async Task<List<GroupItem>> GetUserGroups(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            
            return await dbContext.Groups
                .Where(g => g.CreatorId == userId && g.IsActive)
                .OrderByDescending(g => g.CreationTime)
                .Select(g => new GroupItem
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    CreationTime = g.CreationTime,
                    CreatorId = g.CreatorId,
                    IsActive = g.IsActive
                })
                .ToListAsync();
        }

        /// <summary>
        /// 查詢：群組統計資訊
        /// </summary>
        /// <param name="groupId"> 群組ID </param>
        /// <returns> 統計資訊 </returns>
        public async Task<GroupStatistics> GetGroupStatistics(Guid groupId)
        {
            var dbContext = await GetDbContextAsync();
            
            var cardCount = await dbContext.GroupCards
                .CountAsync(gc => gc.GroupId == groupId);

            return new GroupStatistics
            {
                GroupId = groupId,
                CardCount = cardCount
            };
        }

        /// <summary>
        /// 新增卡片到群組
        /// </summary>
        /// <param name="groupId"> 群組 ID </param>
        /// <param name="cardId"> 卡片 ID </param>
        /// <returns> 操作結果 </returns>
        public async Task<bool> AddCardToGroupAsync(Guid groupId, Guid cardId)
        {
            var dbContext = await GetDbContextAsync();
            
            // 檢查群組是否存在
            var group = await dbContext.Groups.FindAsync(groupId);
            if (group == null || !group.IsActive)
                return false;

            // 檢查卡片是否存在
            var card = await dbContext.Cards.FindAsync(cardId);
            if (card == null)
                return false;

            // 檢查關聯是否已存在
            var existingRelation = await dbContext.GroupCards
                .FirstOrDefaultAsync(gc => gc.GroupId == groupId && gc.CardId == cardId);
            
            if (existingRelation != null)
                return true; // 已存在，視為成功

            // 建立新的群組卡片關聯
            var groupCard = new Entities.GroupCard
            {
                GroupId = groupId,
                CardId = cardId
            };

            await dbContext.GroupCards.AddAsync(groupCard);
            await dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 從群組移除卡片
        /// </summary>
        /// <param name="groupId"> 群組 ID </param>
        /// <param name="cardId"> 卡片 ID </param>
        /// <returns> 操作結果 </returns>
        public async Task<bool> RemoveCardFromGroupAsync(Guid groupId, Guid cardId)
        {
            var dbContext = await GetDbContextAsync();
            
            // 查找群組卡片關聯
            var groupCard = await dbContext.GroupCards
                .FirstOrDefaultAsync(gc => gc.GroupId == groupId && gc.CardId == cardId);
            
            if (groupCard == null)
                return true; // 不存在，視為成功

            // 移除關聯
            dbContext.GroupCards.Remove(groupCard);
            await dbContext.SaveChangesAsync();
            return true;
        }

        #endregion Public Methods

        #region Private Methods

        // 私有輔助方法

        #endregion Private Methods
    }

    /// <summary>
    /// 群組統計資訊
    /// </summary>
    public class GroupStatistics
    {
        /// <summary>
        /// 群組ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 卡片數量
        /// </summary>
        public int CardCount { get; set; }
    }
} 