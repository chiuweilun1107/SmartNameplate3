//-----
// <copyright file="BluetoothAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Bluetooth;
using Hamastar.SmartNameplate.Dto.Backend.Bluetooth.Request;
using Hamastar.SmartNameplate.Dto.Backend.Bluetooth.Response;
using Hamastar.SmartNameplate.IApplication.Bluetooth;
using Hamastar.SmartNameplate.IApplication.AuditTrails;
using Hamastar.SmartNameplate.Permissions;
using Hamastar.SmartNameplate.Repositories.Bluetooth;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.Bluetooth
{
    /// <summary>
    /// 藍牙 App
    /// </summary>
    public class BluetoothAppService : ApplicationService, IBluetoothAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<BluetoothAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 藍牙儲存庫
        /// </summary>
        private readonly IBluetoothRepository _bluetoothRepository;

        /// <summary>
        /// 審計軌跡資料 APP
        /// </summary>
        private readonly IAuditTrailService _auditTrailService;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="bluetoothRepository"> 藍牙儲存庫 </param>
        /// <param name="auditTrailService"> 審計軌跡資料 APP </param>
        public BluetoothAppService(
            ICurrentUser currentUser,
            IBluetoothRepository bluetoothRepository,
            IAuditTrailService auditTrailService)
        {
            _currentUser = currentUser;
            _bluetoothRepository = bluetoothRepository;
            _auditTrailService = auditTrailService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 掃描：藍牙裝置
        /// </summary>
        /// <returns> 藍牙裝置列表 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.BluetoothMgmt)]
        public async Task<BusinessLogicResponse<ScanDevicesResponse>> ScanDevices()
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<ScanDevicesResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                ScanDevicesResponse scanResult = await _bluetoothRepository.ScanDevicesAsync();

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("藍牙裝置掃描", "DATA_READ", "掃描了" + scanResult.Devices.Count + "個藍牙裝置");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "掃描完成";
                response.Data = scanResult;
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
        /// 連接：藍牙裝置
        /// </summary>
        /// <param name="connectRequest"> 連接請求 </param>
        /// <returns> 連接結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.BluetoothMgmt)]
        public async Task<BusinessLogicResponse<ConnectDeviceResponse>> ConnectDevice(ConnectDeviceRequest connectRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<ConnectDeviceResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                connectRequest.DeviceAddress = sanitizer.Sanitize(connectRequest.DeviceAddress);

                // ========= 業務邏輯執行 =========
                ConnectDeviceResponse connectResult = await _bluetoothRepository.ConnectDeviceAsync(connectRequest.DeviceAddress);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("藍牙裝置連接", "DATA_UPDATE", "連接藍牙裝置：" + connectRequest.DeviceAddress + " 且 " + (connectResult.Success ? "成功" : "失敗"));
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = connectResult.Success ? "success" : "error";
                response.Message = connectResult.Success ? "連接成功" : "連接失敗";
                response.Data = connectResult;
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
        /// 斷開：藍牙裝置
        /// </summary>
        /// <param name="disconnectRequest"> 斷開請求 </param>
        /// <returns> 斷開結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.BluetoothMgmt)]
        public async Task<BusinessLogicResponse<DisconnectDeviceResponse>> DisconnectDevice(DisconnectDeviceRequest disconnectRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DisconnectDeviceResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                disconnectRequest.DeviceAddress = sanitizer.Sanitize(disconnectRequest.DeviceAddress);

                // ========= 業務邏輯執行 =========
                DisconnectDeviceResponse disconnectResult = await _bluetoothRepository.DisconnectDeviceAsync(disconnectRequest.DeviceAddress);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("藍牙裝置斷開", "DATA_UPDATE", "斷開藍牙裝置：" + disconnectRequest.DeviceAddress + " 且 " + (disconnectResult.Success ? "成功" : "失敗"));
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = disconnectResult.Success ? "success" : "error";
                response.Message = disconnectResult.Success ? "斷開成功" : "斷開失敗";
                response.Data = disconnectResult;
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
        /// 部署：卡片到裝置
        /// </summary>
        /// <param name="deployRequest"> 部署請求 </param>
        /// <returns> 部署結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.BluetoothMgmt)]
        public async Task<BusinessLogicResponse<DeployCardResponse>> DeployCard(DeployCardRequest deployRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeployCardResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                DeployCardResponse deployResult = await _bluetoothRepository.DeployCardAsync(deployRequest);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("藍牙卡片部署", "DATA_CREATE", "部署卡片到藍牙裝置 且 " + (deployResult.Success ? "成功" : "失敗"));
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

        /// <summary>
        /// 檢查：裝置連接狀態
        /// </summary>
        /// <param name="statusRequest"> 狀態查詢請求 </param>
        /// <returns> 連接狀態 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.BluetoothMgmt)]
        public async Task<BusinessLogicResponse<DeviceStatusResponse>> CheckDeviceStatus(DeviceStatusRequest statusRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeviceStatusResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                statusRequest.DeviceAddress = sanitizer.Sanitize(statusRequest.DeviceAddress);

                // ========= 業務邏輯執行 =========
                DeviceStatusResponse statusResult = await _bluetoothRepository.CheckDeviceStatusAsync(statusRequest.DeviceAddress);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("藍牙裝置狀態查詢", "DATA_READ", "查詢藍牙裝置狀態：" + statusRequest.DeviceAddress);
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = statusResult;
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