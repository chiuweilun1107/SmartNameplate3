//-----
// <copyright file="DeviceRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Hamastar.SmartNameplate.Entities;
using Hamastar.SmartNameplate.EntityFrameworkCore;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Devices;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Devices.Request;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Devices.Response;

namespace Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Devices
{
    /// <summary>
    /// 🤖 裝置儲存庫實作
    /// 基於原始 DeviceService 重構
    /// </summary>
    public class DeviceRepository : EfCoreRepository<SmartNameplateDbContext, Device, Guid>, IDeviceRepository
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
        private readonly ILogger log = Log.ForContext<DeviceRepository>();

        /// <summary>
        /// 當前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="dbContextProvider"> 資料庫上下文提供者 </param>
        /// <param name="appConfiguration"> 應用程式配置 </param>
        /// <param name="unitOfWorkManager"> 工作單元管理器 </param>
        /// <param name="currentUser"> 當前使用者 </param>
        public DeviceRepository(IDbContextProvider<SmartNameplateDbContext> dbContextProvider,
            IConfiguration appConfiguration,
            IUnitOfWorkManager unitOfWorkManager,
            ICurrentUser currentUser)
            : base(dbContextProvider)
        {
            _appConfiguration = appConfiguration;
            _unitOfWorkManager = unitOfWorkManager;
            _currentUser = currentUser;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 獲取裝置分頁列表
        /// </summary>
        /// <param name="request"> 查詢條件 </param>
        /// <returns> 裝置列表 </returns>
        public async Task<DeviceListResponse> GetDeviceListAsync(DeviceListRequest request)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var query = dbContext.Devices.AsQueryable();

                // 關鍵字搜尋
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(x => x.Name.Contains(request.Keyword) || 
                                           x.BluetoothAddress.Contains(request.Keyword));
                }

                // 狀態篩選
                if (request.Status.HasValue)
                {
                    query = query.Where(x => (int)x.Status == request.Status.Value);
                }

                // 群組篩選
                if (request.GroupId.HasValue)
                {
                    query = query.Where(x => x.GroupId == request.GroupId.Value);
                }

                // 使用者篩選
                if (request.UserId.HasValue)
                {
                    query = query.Where(x => x.CreatorId == request.UserId.Value);
                }

                // 啟用狀態篩選
                if (request.Enable.HasValue)
                {
                    query = query.Where(x => x.Enable == request.Enable.Value);
                }

                var totalCount = await query.CountAsync();

                // 分頁
                var devices = await query
                    .OrderBy(x => x.CreationTime)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new DeviceItemForListByPage
                    {
                        Id = x.Id,
                        Name = x.Name,
                        BluetoothAddress = x.BluetoothAddress,
                        Status = (int)x.Status,
                        BatteryLevel = x.BatteryLevel ?? 0,
                        Enable = x.Enable,
                        CreationTime = x.CreationTime,
                        LastModificationTime = x.LastModificationTime
                    })
                    .ToListAsync();

                return new DeviceListResponse
                {
                    Data = devices,
                    TotalCount = totalCount,
                    PageSize = request.PageSize,
                    Page = request.Page,
                    PageTotalCount = (int)Math.Ceiling((double)totalCount / request.PageSize)
                };
            }
            catch (Exception ex)
            {
                log.Error(ex, "獲取裝置列表時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 依據 ID 獲取裝置資訊
        /// </summary>
        /// <param name="id"> 裝置 ID </param>
        /// <returns> 裝置資訊 </returns>
        public async Task<DeviceItem?> GetDeviceByIdAsync(Guid id)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var device = await dbContext.Devices
                    .Where(x => x.Id == id)
                    .Select(x => new DeviceItem
                    {
                        Id = x.Id,
                        Name = x.Name,
                        BluetoothAddress = x.BluetoothAddress,
                        OriginalAddress = x.OriginalAddress,
                        Description = x.Description,
                        Status = (int)x.Status,
                        BatteryLevel = x.BatteryLevel ?? 0,
                        Enable = x.Enable,
                        CurrentCardId = x.CurrentCardId,
                        GroupId = x.GroupId,
                        LastConnected = x.LastConnected,
                        CreationTime = x.CreationTime,
                        LastModificationTime = x.LastModificationTime,
                        CreatorUserId = x.CreatorId,
                        CustomIndex = x.CustomIndex
                    })
                    .FirstOrDefaultAsync();

                return device;
            }
            catch (Exception ex)
            {
                log.Error(ex, "依據 ID 獲取裝置資訊時發生錯誤: {DeviceId}", id);
                throw;
            }
        }

        /// <summary>
        /// 依據名稱獲取裝置資訊
        /// </summary>
        /// <param name="name"> 裝置名稱 </param>
        /// <returns> 裝置資訊 </returns>
        public async Task<DeviceItem?> GetDeviceByNameAsync(string name)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var device = await dbContext.Devices
                    .Where(x => x.Name == name)
                    .Select(x => new DeviceItem
                    {
                        Id = x.Id,
                        Name = x.Name,
                        BluetoothAddress = x.BluetoothAddress,
                        OriginalAddress = x.OriginalAddress,
                        Description = x.Description,
                        Status = (int)x.Status,
                        BatteryLevel = x.BatteryLevel ?? 0,
                        Enable = x.Enable,
                        CurrentCardId = x.CurrentCardId,
                        GroupId = x.GroupId,
                        LastConnected = x.LastConnected,
                        CreationTime = x.CreationTime,
                        LastModificationTime = x.LastModificationTime,
                        CreatorUserId = x.CreatorId,
                        CustomIndex = x.CustomIndex
                    })
                    .FirstOrDefaultAsync();

                return device;
            }
            catch (Exception ex)
            {
                log.Error(ex, "依據名稱獲取裝置資訊時發生錯誤: {DeviceName}", name);
                throw;
            }
        }

        /// <summary>
        /// 根據藍牙地址取得裝置實體
        /// </summary>
        /// <param name="bluetoothAddress"> 藍牙地址 </param>
        /// <returns> 裝置實體 </returns>
        public async Task<Device?> GetDeviceByBluetoothAddressAsync(string bluetoothAddress)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                return await dbContext.Devices
                    .FirstOrDefaultAsync(x => x.BluetoothAddress == bluetoothAddress);
            }
            catch (Exception ex)
            {
                log.Error(ex, "根據藍牙地址取得裝置時發生錯誤: {BluetoothAddress}", bluetoothAddress);
                throw;
            }
        }

        /// <summary>
        /// 根據群組取得裝置列表
        /// </summary>
        /// <param name="groupId"> 群組 ID </param>
        /// <returns> 裝置列表 </returns>
        public async Task<List<Device>> GetDevicesByGroupAsync(Guid groupId)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                return await dbContext.Devices
                    .Where(x => x.GroupId == groupId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                log.Error(ex, "根據群組取得裝置列表時發生錯誤: {GroupId}", groupId);
                throw;
            }
        }

        /// <summary>
        /// 查詢：裝置列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        public async Task<DeviceListResponse> GetListByPage(DeviceListRequest request)
        {
            return await GetDeviceListAsync(request);
        }

        /// <summary>
        /// 查詢：單一裝置
        /// </summary>
        /// <param name="id"> 裝置ID </param>
        /// <returns> 裝置資料 </returns>
        public async Task<DeviceItem?> GetDeviceById(Guid id)
        {
            return await GetDeviceByIdAsync(id);
        }

        /// <summary>
        /// 查詢：依藍牙地址取得裝置
        /// </summary>
        /// <param name="bluetoothAddress"> 藍牙地址 </param>
        /// <returns> 裝置資料 </returns>
        public async Task<DeviceItem?> GetDeviceByBluetoothAddress(string bluetoothAddress)
        {
            try
            {
                var device = await GetDeviceByBluetoothAddressAsync(bluetoothAddress);
                if (device == null) return null;

                return new DeviceItem
                {
                    Id = device.Id,
                    Name = device.Name,
                    BluetoothAddress = device.BluetoothAddress,
                    OriginalAddress = device.OriginalAddress,
                    Description = device.Description,
                    Status = (int)device.Status,
                    BatteryLevel = device.BatteryLevel ?? 0,
                    Enable = device.Enable,
                    CurrentCardId = device.CurrentCardId,
                    GroupId = device.GroupId,
                    LastConnected = device.LastConnected,
                    CreationTime = device.CreationTime,
                    LastModificationTime = device.LastModificationTime,
                    CreatorUserId = device.CreatorId,
                    CustomIndex = device.CustomIndex
                };
            }
            catch (Exception ex)
            {
                log.Error(ex, "依藍牙地址取得裝置時發生錯誤: {BluetoothAddress}", bluetoothAddress);
                throw;
            }
        }

        /// <summary>
        /// 更新：裝置狀態
        /// </summary>
        /// <param name="deviceId"> 裝置ID </param>
        /// <param name="status"> 新狀態 </param>
        /// <returns> 更新結果 </returns>
        public async Task<bool> UpdateDeviceStatus(Guid deviceId, int status)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var device = await dbContext.Devices.FirstOrDefaultAsync(x => x.Id == deviceId);
                
                if (device == null)
                {
                    log.Warning("找不到裝置: {DeviceId}", deviceId);
                    return false;
                }

                device.Status = (DeviceStatus)status;
                device.LastModificationTime = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex, "更新裝置狀態時發生錯誤: {DeviceId}, {Status}", deviceId, status);
                return false;
            }
        }

        /// <summary>
        /// 更新：裝置電池電量
        /// </summary>
        /// <param name="deviceId"> 裝置ID </param>
        /// <param name="batteryLevel"> 電池電量 </param>
        /// <returns> 更新結果 </returns>
        public async Task<bool> UpdateDeviceBatteryLevel(Guid deviceId, int batteryLevel)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var device = await dbContext.Devices.FirstOrDefaultAsync(x => x.Id == deviceId);
                
                if (device == null)
                {
                    log.Warning("找不到裝置: {DeviceId}", deviceId);
                    return false;
                }

                device.BatteryLevel = batteryLevel;
                device.LastModificationTime = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex, "更新裝置電池電量時發生錯誤: {DeviceId}, {BatteryLevel}", deviceId, batteryLevel);
                return false;
            }
        }

        /// <summary>
        /// 更新：裝置最後連線時間
        /// </summary>
        /// <param name="deviceId"> 裝置ID </param>
        /// <returns> 更新結果 </returns>
        public async Task<bool> UpdateDeviceLastConnected(Guid deviceId)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var device = await dbContext.Devices.FirstOrDefaultAsync(x => x.Id == deviceId);
                
                if (device == null)
                {
                    log.Warning("找不到裝置: {DeviceId}", deviceId);
                    return false;
                }

                device.LastConnected = DateTime.UtcNow;
                device.LastModificationTime = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex, "更新裝置最後連線時間時發生錯誤: {DeviceId}", deviceId);
                return false;
            }
        }

        #endregion Public Methods
    }
} 