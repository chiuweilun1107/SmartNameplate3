//-----
// <copyright file="IDeployAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy.Request;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy.Response;

namespace Hamastar.SmartNameplate.Application.Contracts.IApplication.Deploy
{
    /// <summary>
    /// 🤖 部署應用服務介面
    /// </summary>
    public interface IDeployAppService : IApplicationService
    {
        /// <summary>
        /// 獲取部署歷史列表
        /// </summary>
        Task<DeployListResponse> GetDeployListAsync();

        /// <summary>
        /// 執行部署
        /// </summary>
        Task<DeployResultResponse> DeployAsync(DeployRequest request);
    }
} 