//-----
// <copyright file="DeviceAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Devices;
using Hamastar.SmartNameplate.Dto.Backend.Devices.Request;
using Hamastar.SmartNameplate.Dto.Backend.Devices.Response;
using Hamastar.SmartNameplate.IApplication.Devices;
using Hamastar.SmartNameplate.IApplication.AuditTrails;
using Hamastar.SmartNameplate.Permissions;
using Hamastar.SmartNameplate.Repositories.Devices;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.Devices
{
    /// <summary>
    /// 裝置 App
    /// </summary>
    public class DeviceAppService : ApplicationService, IDeviceAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<DeviceAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 裝置儲存庫
        /// </summary>
        private readonly IDeviceRepository _deviceRepository;

        /// <summary>
        /// 審計軌跡資料 APP
        /// </summary>
        private readonly IAuditTrailService _auditTrailService;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="deviceRepository"> 裝置儲存庫 </param>
        /// <param name="auditTrailService"> 審計軌跡資料 APP </param>
        public DeviceAppService(
            ICurrentUser currentUser,
            IDeviceRepository deviceRepository,
            IAuditTrailService auditTrailService)
        {
            _currentUser = currentUser;
            _deviceRepository = deviceRepository;
            _auditTrailService = auditTrailService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 查詢：裝置列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.DeviceMgmt)]
        public async Task<BusinessLogicResponse<DeviceListResponse>> GetDeviceListByPage(DeviceListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeviceListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Keyword = sanitizer.Sanitize(request.Keyword);
                request.Keyword = HttpUtility.UrlDecode(request.Keyword);

                // ========= 業務邏輯執行 =========
                DeviceListResponse deviceList = await _deviceRepository.GetListByPage(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("裝置查詢", "DATA_READ", "查詢了" + deviceList.ItemTotalCount + "筆裝置資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = deviceList;
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
        /// 新增：裝置
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.DeviceMgmt)]
        public async Task<BusinessLogicResponse<CreateDeviceResponse>> CreateDevice(CreateDeviceRequest createRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<CreateDeviceResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (createRequest.Name.Length > 200) throw new BusinessException(message: "裝置名稱過長 Max 200");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();

                // ========= 建立實體 =========
                Entities.Device createDevice = new()
                {
                    Name = sanitizer.Sanitize(createRequest.Name),
                    MacAddress = createRequest.MacAddress,
                    Status = createRequest.Status,
                    CreationTime = DateTime.Now,
                    CreatorUserId = CurrentUser.Id ?? Guid.Empty,
                    LastModificationTime = DateTime.Now,
                    LastModifierUserId = CurrentUser.Id ?? Guid.Empty
                };

                // ========= 執行新增 =========
                CreateDeviceResponse createDeviceResponse = new();
                var result = await _deviceRepository.InsertAsync(createDevice);
                if (result != null)
                {
                    await CreateAuditTrail("裝置新增", "DATA_CREATE", "新增了裝置：" + createDevice.Name + " 且 成功");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Status = "success";
                    response.Message = "新增成功";
                    createDeviceResponse.Result = true;
                    createDeviceResponse.DeviceId = result.Id;
                }
                else
                {
                    await CreateAuditTrail("裝置新增", "DATA_CREATE", "新增了裝置：" + createDevice.Name + " 且 失敗");
                    response.Status = "error";
                    response.Message = "新增失敗";
                    createDeviceResponse.Result = false;
                    await uow.RollbackAsync();
                }
                await uow.CompleteAsync();
                response.Data = createDeviceResponse;
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
        /// 更新：裝置
        /// </summary>
        /// <param name="updateRequest"> 更新資訊 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.DeviceMgmt)]
        public async Task<BusinessLogicResponse<UpdateDeviceResponse>> UpdateDevice(UpdateDeviceRequest updateRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<UpdateDeviceResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (!string.IsNullOrEmpty(updateRequest.Name) && updateRequest.Name.Length > 200) 
                    throw new BusinessException(message: "裝置名稱過長 Max 200");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();

                // ========= 取得實體 =========
                var device = await _deviceRepository.GetAsync(updateRequest.Id);
                if (device == null)
                {
                    throw new BusinessException(message: "裝置不存在");
                }

                // ========= 更新實體 =========
                if (!string.IsNullOrEmpty(updateRequest.Name))
                    device.Name = sanitizer.Sanitize(updateRequest.Name);
                
                if (updateRequest.MacAddress != null)
                    device.MacAddress = updateRequest.MacAddress;
                
                if (updateRequest.Status.HasValue)
                    device.Status = updateRequest.Status.Value;

                device.LastModificationTime = DateTime.Now;
                device.LastModifierUserId = CurrentUser.Id ?? Guid.Empty;

                // ========= 執行更新 =========
                UpdateDeviceResponse updateDeviceResponse = new();
                var result = await _deviceRepository.UpdateAsync(device);
                if (result != null)
                {
                    await CreateAuditTrail("裝置更新", "DATA_UPDATE", "更新了裝置：" + device.Name + " 且 成功");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Status = "success";
                    response.Message = "更新成功";
                    updateDeviceResponse.Result = true;
                }
                else
                {
                    await CreateAuditTrail("裝置更新", "DATA_UPDATE", "更新了裝置：" + device.Name + " 且 失敗");
                    response.Status = "error";
                    response.Message = "更新失敗";
                    updateDeviceResponse.Result = false;
                    await uow.RollbackAsync();
                }
                await uow.CompleteAsync();
                response.Data = updateDeviceResponse;
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
        /// 刪除：裝置
        /// </summary>
        /// <param name="deleteRequest"> 刪除條件 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.DeviceMgmt)]
        public async Task<BusinessLogicResponse<DeleteDeviceResponse>> DeleteDevice(DeleteDeviceRequest deleteRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DeleteDeviceResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                var device = await _deviceRepository.GetAsync(deleteRequest.Id);
                if (device == null)
                {
                    throw new BusinessException(message: "裝置不存在");
                }

                string deviceName = device.Name;
                await _deviceRepository.DeleteAsync(device);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("裝置刪除", "DATA_DELETE", "刪除了裝置：" + deviceName + " 且 成功");
                await CurrentUnitOfWork.SaveChangesAsync();
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                DeleteDeviceResponse deleteDeviceResponse = new()
                {
                    Result = true
                };
                response.Status = "success";
                response.Message = "刪除成功";
                response.Data = deleteDeviceResponse;
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