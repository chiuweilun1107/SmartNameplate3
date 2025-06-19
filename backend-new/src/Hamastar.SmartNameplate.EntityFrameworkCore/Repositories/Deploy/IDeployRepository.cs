//-----
// <copyright file="IDeployRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Hamastar.SmartNameplate.Entities;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy.Request;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy.Response;

namespace Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Deploy;

/// <summary>
/// 🤖 部署 Repository 介面
/// 處理部署歷史資料存取邏輯
/// </summary>
public interface IDeployRepository : IRepository<DeployHistory, Guid>
{
    /// <summary>
    /// 獲取部署歷史分頁列表
    /// </summary>
    Task<DeployHistoryListResponse> GetDeployHistoryListAsync(DeployHistoryListRequest request);

    /// <summary>
    /// 依據裝置獲取部署歷史
    /// </summary>
    Task<DeployHistoryListResponse> GetDeployHistoryByDeviceAsync(Guid deviceId);

    /// <summary>
    /// 依據卡片獲取部署歷史
    /// </summary>
    Task<DeployHistoryListResponse> GetDeployHistoryByCardAsync(Guid cardId);

    /// <summary>
    /// 依據狀態獲取部署歷史
    /// </summary>
    Task<DeployHistoryListResponse> GetDeployHistoryByStatusAsync(int status);

    /// <summary>
    /// 執行部署
    /// </summary>
    Task<DeployResultResponse> ExecuteDeployAsync(DeployRequest request);

    /// <summary>
    /// 依據 ID 獲取部署歷史
    /// </summary>
    Task<DeployHistoryItem?> GetDeployHistoryByIdAsync(Guid id);

    /// <summary>
    /// 依據使用者獲取部署歷史
    /// </summary>
    Task<DeployHistoryItem?> GetDeployHistoryByUserAsync(string userId);
} 