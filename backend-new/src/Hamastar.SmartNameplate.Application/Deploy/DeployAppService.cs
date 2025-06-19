//-----
// <copyright file="DeployAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Deploy;
using Hamastar.SmartNameplate.Dto.Backend.Deploy.Request;
using Hamastar.SmartNameplate.Dto.Backend.Deploy.Response;
using Hamastar.SmartNameplate.IApplication.Deploy;
using Hamastar.SmartNameplate.IApplication.AuditTrails;
using Hamastar.SmartNameplate.Permissions;
using Hamastar.SmartNameplate.Repositories.Deploy;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.Deploy
{
    /// <summary>
    /// 部署 App
    /// </summary>
    public class DeployAppService : ApplicationService, IDeployAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<DeployAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 部署儲存庫
        /// </summary>
        private readonly IDeployRepository _deployRepository;

        /// <summary>
        /// 審計軌跡資料 APP
        /// </summary>
        private readonly IAuditTrailService _auditTrailService;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DeployAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="deployRepository"> 部署儲存庫 </param>
        /// <param name="auditTrailService"> 審計軌跡資料 APP </param>
        public DeployAppService(
            ICurrentUser currentUser,
            IDeployRepository deployRepository,
            IAuditTrailService auditTrailService)
        {
            _currentUser = currentUser;
            _deployRepository = deployRepository;
            _auditTrailService = auditTrailService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 查詢：部署歷史列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.DeployMgmt)]
        public async Task<BusinessLogicResponse<DeployListResponse>> GetDeployHistoryListByPage(DeployListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeployListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Keyword = sanitizer.Sanitize(request.Keyword);
                request.Keyword = HttpUtility.UrlDecode(request.Keyword);

                // ========= 業務邏輯執行 =========
                DeployListResponse deployList = await _deployRepository.GetListByPage(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("部署歷史查詢", "DATA_READ", "查詢了" + deployList.ItemTotalCount + "筆部署歷史資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = deployList;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        /// <summary>
        /// 執行：部署請求
        /// </summary>
        /// <param name="deployRequest"> 部署請求 </param>
        /// <returns> 部署結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.DeployMgmt)]
        public async Task<BusinessLogicResponse<DeployResultResponse>> ExecuteDeploy(DeployRequest deployRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeployResultResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                DeployResultResponse deployResult = await _deployRepository.ExecuteDeployAsync(deployRequest);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("部署執行", "DATA_CREATE", "執行了部署請求 且 " + (deployResult.Success ? "成功" : "失敗"));
                await CurrentUnitOfWork.SaveChangesAsync();
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = deployResult.Success ? "success" : "error";
                response.Message = deployResult.Success ? "部署成功" : "部署失敗";
                response.Data = deployResult;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 儲存操作紀錄至審計軌跡
        /// </summary>
        /// <param name="target"> 操作目標 </param>
        /// <param name="type"> 操作類型 </param>
        /// <param name="description"> 操作描述 </param>
        /// <returns> Task </returns>
        private async Task CreateAuditTrail(string target, string type, string description)
        {
            await _auditTrailService.CreateAsync(
                target: "SmartNameplate-系統管理-" + target,
                type: type,
                description: description
                );
        }

        #endregion Private Methods
    }
} 