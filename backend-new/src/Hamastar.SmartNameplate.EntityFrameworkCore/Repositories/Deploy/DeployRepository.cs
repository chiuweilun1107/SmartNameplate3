//-----
// <copyright file="DeployRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Hamastar.SmartNameplate.Entities;
using Hamastar.SmartNameplate.EntityFrameworkCore;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy.Request;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy.Response;
using Microsoft.Extensions.Configuration;
using Serilog;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Deploy
{
    /// <summary>
    /// 🤖 部署 Repository 實作
    /// 處理部署歷史資料存取邏輯
    /// </summary>
    public class DeployRepository : EfCoreRepository<SmartNameplateDbContext, DeployHistory, Guid>, IDeployRepository
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
        private readonly ILogger log = Log.ForContext<DeployRepository>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DeployRepository" /> class
        /// </summary>
        /// <param name="appConfiguration"> SettingProvider </param>
        /// <param name="contextProvider"> dbContext </param>
        /// <param name="unitOfWorkManager"> Unit of Work Manager </param>
        /// <param name="currentUser"> 目前登入的使用者 </param>
        public DeployRepository(IConfiguration appConfiguration,
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
        /// 獲取部署歷史分頁列表
        /// </summary>
        public async Task<DeployHistoryListResponse> GetDeployHistoryListAsync(DeployHistoryListRequest request)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.DeployHistories.AsQueryable();

            // 篩選條件
            if (request.DeviceId.HasValue)
            {
                query = query.Where(x => x.DeviceId == request.DeviceId.Value);
            }

            if (request.CardId.HasValue)
            {
                query = query.Where(x => x.CardId == request.CardId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => (int)x.Status == request.Status.Value);
            }

            // 總數量
            var totalCount = await query.CountAsync();

            // 分頁
            var items = await query
                .OrderByDescending(x => x.CreationTime)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new DeployHistoryItem
                {
                    Id = x.Id,
                    DeviceId = x.DeviceId,
                    CardId = x.CardId,
                    Status = (int)x.Status,
                    DeployedAt = x.DeployedAt,
                    ScheduledAt = x.ScheduledAt,
                    ErrorMessage = x.ErrorMessage,
                    DeployedBy = x.DeployedBy,
                    DeviceName = x.DeviceName ?? "",
                    CardName = x.CardName ?? "",
                    DeployTime = x.DeployTime,
                    CompletedTime = x.CompletedTime,
                    CreationTime = x.CreationTime
                })
                .ToListAsync();

            return new DeployHistoryListResponse
            {
                Success = true,
                Data = items,
                TotalCount = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }

        /// <summary>
        /// 依據裝置獲取部署歷史
        /// </summary>
        public async Task<DeployHistoryListResponse> GetDeployHistoryByDeviceAsync(Guid deviceId)
        {
            var request = new DeployHistoryListRequest
            {
                DeviceId = deviceId,
                PageIndex = 1,
                PageSize = 100
            };
            return await GetDeployHistoryListAsync(request);
        }

        /// <summary>
        /// 依據卡片獲取部署歷史
        /// </summary>
        public async Task<DeployHistoryListResponse> GetDeployHistoryByCardAsync(Guid cardId)
        {
            var request = new DeployHistoryListRequest
            {
                CardId = cardId,
                PageIndex = 1,
                PageSize = 100
            };
            return await GetDeployHistoryListAsync(request);
        }

        /// <summary>
        /// 依據狀態獲取部署歷史
        /// </summary>
        public async Task<DeployHistoryListResponse> GetDeployHistoryByStatusAsync(int status)
        {
            var request = new DeployHistoryListRequest
            {
                Status = status,
                PageIndex = 1,
                PageSize = 100
            };
            return await GetDeployHistoryListAsync(request);
        }

        /// <summary>
        /// 執行部署
        /// </summary>
        public async Task<DeployResultResponse> ExecuteDeployAsync(DeployRequest request)
        {
            try
            {
                var deployHistory = new DeployHistory
                {
                    Id = Guid.NewGuid(),
                    DeviceId = request.DeviceId,
                    CardId = request.CardId,
                    Status = DeployStatus.Pending,
                    ScheduledAt = request.ScheduledAt,
                    DeployedBy = request.DeployedBy,
                    IsScheduled = request.IsScheduled,
                    DeviceName = request.DeviceName,
                    CardName = request.CardName,
                    DeployTime = DateTime.UtcNow
                };

                await InsertAsync(deployHistory);

                return new DeployResultResponse
                {
                    Success = true,
                    Message = "部署請求已建立",
                    DeployId = deployHistory.Id,
                    Status = DeployStatus.Pending.ToString(),
                    DeviceId = request.DeviceId,
                    CardId = request.CardId,
                    DeployTime = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new DeployResultResponse
                {
                    Success = false,
                    Message = $"部署失敗: {ex.Message}",
                    Status = DeployStatus.Failed.ToString()
                };
            }
        }

        /// <summary>
        /// 依據 ID 獲取部署歷史
        /// </summary>
        public async Task<DeployHistoryItem?> GetDeployHistoryByIdAsync(Guid id)
        {
            var dbContext = await GetDbContextAsync();
            var entity = await dbContext.DeployHistories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            return new DeployHistoryItem
            {
                Id = entity.Id,
                DeviceId = entity.DeviceId,
                CardId = entity.CardId,
                Status = (int)entity.Status,
                DeployedAt = entity.DeployedAt,
                ScheduledAt = entity.ScheduledAt,
                ErrorMessage = entity.ErrorMessage,
                DeployedBy = entity.DeployedBy,
                DeviceName = entity.DeviceName,
                CardName = entity.CardName,
                DeployTime = entity.DeployTime,
                CompletedTime = entity.CompletedTime,
                CreationTime = entity.CreationTime
            };
        }

        /// <summary>
        /// 依據使用者獲取部署歷史
        /// </summary>
        public async Task<DeployHistoryItem?> GetDeployHistoryByUserAsync(string userId)
        {
            var dbContext = await GetDbContextAsync();
            var entity = await dbContext.DeployHistories
                .Where(x => x.DeployedBy == userId)
                .OrderByDescending(x => x.CreationTime)
                .FirstOrDefaultAsync();

            if (entity == null) return null;

            return new DeployHistoryItem
            {
                Id = entity.Id,
                DeviceId = entity.DeviceId,
                CardId = entity.CardId,
                Status = (int)entity.Status,
                DeployedAt = entity.DeployedAt,
                ScheduledAt = entity.ScheduledAt,
                ErrorMessage = entity.ErrorMessage,
                DeployedBy = entity.DeployedBy,
                DeviceName = entity.DeviceName,
                CardName = entity.CardName,
                DeployTime = entity.DeployTime,
                CompletedTime = entity.CompletedTime,
                CreationTime = entity.CreationTime
            };
        }

        #endregion Public Methods

        #region Private Methods

        // 私有輔助方法

        #endregion Private Methods
    }

    /// <summary>
    /// 部署統計資訊
    /// </summary>
    public class DeployStatistics
    {
        /// <summary>
        /// 總部署次數
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 成功次數
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 失敗次數
        /// </summary>
        public int FailureCount { get; set; }

        /// <summary>
        /// 進行中次數
        /// </summary>
        public int InProgressCount { get; set; }

        /// <summary>
        /// 成功率
        /// </summary>
        public double SuccessRate { get; set; }
    }
} 