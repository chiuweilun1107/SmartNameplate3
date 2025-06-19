//-----
// <copyright file="BluetoothRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
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
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Request;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Response;

namespace Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Bluetooth
{
    /// <summary>
    /// 🤖 藍牙服務儲存庫實作
    /// 基於原始 BluetoothService 重構
    /// </summary>
    public class BluetoothRepository : EfCoreRepository<SmartNameplateDbContext, Device, Guid>, IBluetoothRepository
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
        private readonly ILogger log = Log.ForContext<BluetoothRepository>();

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
        public BluetoothRepository(IConfiguration appConfiguration,
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
        /// 檢查藍牙是否可用
        /// </summary>
        /// <returns> 是否可用 </returns>
        public async Task<bool> IsBluetoothAvailable()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    var result = await ExecuteCommand("system_profiler SPBluetoothDataType");
                    return !string.IsNullOrEmpty(result) && result.Contains("Bluetooth");
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex, "檢查藍牙可用性時發生錯誤");
                return false;
            }
        }

        /// <summary>
        /// 掃描藍牙裝置
        /// </summary>
        /// <param name="cancellationToken"> 取消權杖 </param>
        /// <returns> 發現的裝置列表 </returns>
        public async Task<List<BluetoothDeviceItem>> ScanForDevices(CancellationToken cancellationToken = default)
        {
            var discoveredDevices = new List<BluetoothDeviceItem>();

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    await ScanBluetoothOnMacOS(discoveredDevices, cancellationToken);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Windows 藍牙掃描實作
                    log.Information("Windows 藍牙掃描尚未實作");
                }

                return discoveredDevices;
            }
            catch (Exception ex)
            {
                log.Error(ex, "掃描藍牙裝置時發生錯誤");
                return discoveredDevices;
            }
        }

        /// <summary>
        /// 檢查裝置連接狀態
        /// </summary>
        /// <param name="bluetoothAddress"> 藍牙地址 </param>
        /// <returns> 是否已連接 </returns>
        public async Task<bool> CheckDeviceConnection(string bluetoothAddress)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return await CheckMacOSDeviceConnection(bluetoothAddress);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return await CheckWindowsDeviceConnection(bluetoothAddress);
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex, "檢查裝置連接狀態時發生錯誤: {BluetoothAddress}", bluetoothAddress);
                return false;
            }
        }

        /// <summary>
        /// 取得已連接的裝置地址列表
        /// </summary>
        /// <returns> 已連接的裝置地址列表 </returns>
        public async Task<List<string>> GetConnectedDeviceAddresses()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    var connectedDevices = await GetMacOSConnectedDevices();
                    return connectedDevices.ToList();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var connectedDevices = await GetWindowsConnectedDevices();
                    return connectedDevices.ToList();
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                log.Error(ex, "取得已連接裝置地址時發生錯誤");
                return new List<string>();
            }
        }

        /// <summary>
        /// 檢查裝置是否可達
        /// </summary>
        /// <param name="bluetoothAddress"> 藍牙地址 </param>
        /// <returns> 是否可達 </returns>
        public async Task<bool> IsDeviceReachable(string bluetoothAddress)
        {
            try
            {
                // 先檢查是否已連接
                var isConnected = await CheckDeviceConnection(bluetoothAddress);
                if (isConnected)
                {
                    return true;
                }

                // 嘗試 ping 裝置
                return await PingBluetoothDevice(bluetoothAddress);
            }
            catch (Exception ex)
            {
                log.Error(ex, "檢查裝置可達性時發生錯誤: {BluetoothAddress}", bluetoothAddress);
                return false;
            }
        }

        /// <summary>
        /// 掃描藍牙裝置
        /// </summary>
        /// <returns> 掃描結果 </returns>
        public async Task<ScanDevicesResponse> ScanDevicesAsync()
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var devices = await ScanForDevices();
                var endTime = DateTime.UtcNow;

                return new ScanDevicesResponse
                {
                    Success = true,
                    Devices = devices,
                    ScanStartTime = startTime,
                    ScanEndTime = endTime,
                    ScanDuration = (int)(endTime - startTime).TotalMilliseconds,
                    Result = "success",
                    Message = $"成功掃描到 {devices.Count} 個藍牙裝置"
                };
            }
            catch (Exception ex)
            {
                log.Error(ex, "掃描藍牙裝置時發生錯誤");
                return new ScanDevicesResponse
                {
                    Success = false,
                    Devices = new List<BluetoothDeviceItem>(),
                    Result = "error",
                    Message = $"掃描失敗: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 連接藍牙裝置
        /// </summary>
        /// <param name="bluetoothAddress"> 藍牙地址 </param>
        /// <returns> 連接結果 </returns>
        public async Task<ConnectDeviceResponse> ConnectDeviceAsync(string bluetoothAddress)
        {
            try
            {
                // 實作藍牙連接邏輯
                var isConnected = await CheckDeviceConnection(bluetoothAddress);
                
                return new ConnectDeviceResponse
                {
                    Success = true,
                    DeviceAddress = bluetoothAddress,
                    DeviceName = "Unknown Device",
                    ConnectedAt = DateTime.UtcNow,
                    ConnectionStatus = isConnected ? "Connected" : "Disconnected"
                };
            }
            catch (Exception ex)
            {
                log.Error(ex, "連接藍牙裝置時發生錯誤: {BluetoothAddress}", bluetoothAddress);
                return new ConnectDeviceResponse
                {
                    Success = false,
                    DeviceAddress = bluetoothAddress,
                    ConnectionStatus = "Error"
                };
            }
        }

        /// <summary>
        /// 獲取藍牙裝置列表
        /// </summary>
        /// <returns> 藍牙裝置列表 </returns>
        public async Task<List<BluetoothDeviceItem>> GetBluetoothDevicesAsync()
        {
            try
            {
                return await ScanForDevices();
            }
            catch (Exception ex)
            {
                log.Error(ex, "獲取藍牙裝置列表時發生錯誤");
                return new List<BluetoothDeviceItem>();
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 在 macOS 上掃描藍牙裝置
        /// </summary>
        /// <param name="discoveredDevices"> 發現的裝置列表 </param>
        /// <param name="cancellationToken"> 取消權杖 </param>
        private async Task ScanBluetoothOnMacOS(List<BluetoothDeviceItem> discoveredDevices, CancellationToken cancellationToken)
        {
            try
            {
                // 嘗試多種掃描方法
                await ScanWithPythonBLE(discoveredDevices, cancellationToken);
                
                if (discoveredDevices.Count == 0)
                {
                    await ScanWithSystemProfiler(discoveredDevices, cancellationToken);
                }
                
                if (discoveredDevices.Count == 0)
                {
                    await ScanWithBlueutil(discoveredDevices, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "macOS 藍牙掃描時發生錯誤");
            }
        }

        /// <summary>
        /// 使用 Python BLE 掃描
        /// </summary>
        /// <param name="discoveredDevices"> 發現的裝置列表 </param>
        /// <param name="cancellationToken"> 取消權杖 </param>
        private async Task ScanWithPythonBLE(List<BluetoothDeviceItem> discoveredDevices, CancellationToken cancellationToken)
        {
            try
            {
                var pythonScript = @"
import asyncio
import json
from bleak import BleakScanner

async def scan_devices():
    devices = await BleakScanner.discover(timeout=10.0)
    result = []
    for device in devices:
        result.append({
            'name': device.name or 'Unknown',
            'address': device.address,
            'rssi': device.rssi
        })
    return result

if __name__ == '__main__':
    try:
        devices = asyncio.run(scan_devices())
        print(json.dumps(devices))
    except Exception as e:
        print(json.dumps([]))
";

                var tempFile = Path.GetTempFileName() + ".py";
                await File.WriteAllTextAsync(tempFile, pythonScript, cancellationToken);

                try
                {
                    var result = await ExecuteCommand($"python3 {tempFile}");
                    
                    if (!string.IsNullOrEmpty(result))
                    {
                        var devices = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(result);
                        
                        foreach (var device in devices ?? new List<Dictionary<string, object>>())
                        {
                            discoveredDevices.Add(new BluetoothDeviceItem
                            {
                                Name = device.GetValueOrDefault("name", "Unknown").ToString() ?? "Unknown",
                                BluetoothAddress = device.GetValueOrDefault("address", "").ToString() ?? "",
                                SignalStrength = Convert.ToInt32(device.GetValueOrDefault("rssi", -100)),
                                IsConnected = false,
                                DeviceType = "BLE"
                            });
                        }
                    }
                }
                finally
                {
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warning(ex, "Python BLE 掃描失敗");
            }
        }

        /// <summary>
        /// 使用 system_profiler 掃描
        /// </summary>
        /// <param name="discoveredDevices"> 發現的裝置列表 </param>
        /// <param name="cancellationToken"> 取消權杖 </param>
        private async Task ScanWithSystemProfiler(List<BluetoothDeviceItem> discoveredDevices, CancellationToken cancellationToken)
        {
            try
            {
                var result = await ExecuteCommand("system_profiler SPBluetoothDataType");
                ParseMacOSBluetoothOutput(result, discoveredDevices);
            }
            catch (Exception ex)
            {
                log.Warning(ex, "system_profiler 掃描失敗");
            }
        }

        /// <summary>
        /// 使用 blueutil 掃描
        /// </summary>
        /// <param name="discoveredDevices"> 發現的裝置列表 </param>
        /// <param name="cancellationToken"> 取消權杖 </param>
        private async Task ScanWithBlueutil(List<BluetoothDeviceItem> discoveredDevices, CancellationToken cancellationToken)
        {
            // blueutil 掃描實作
            log.Information("blueutil 掃描尚未實作");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 解析 macOS 藍牙輸出
        /// </summary>
        /// <param name="output"> 命令輸出 </param>
        /// <param name="discoveredDevices"> 發現的裝置列表 </param>
        private void ParseMacOSBluetoothOutput(string output, List<BluetoothDeviceItem> discoveredDevices)
        {
            // 解析 system_profiler 輸出
            // 這裡需要根據實際輸出格式進行解析
        }

        /// <summary>
        /// 檢查 macOS 裝置連接狀態
        /// </summary>
        /// <param name="bluetoothAddress"> 藍牙地址 </param>
        /// <returns> 是否已連接 </returns>
        private async Task<bool> CheckMacOSDeviceConnection(string bluetoothAddress)
        {
            try
            {
                var result = await ExecuteCommand($"system_profiler SPBluetoothDataType | grep -i {bluetoothAddress}");
                return !string.IsNullOrEmpty(result) && result.Contains("Connected");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查 Windows 裝置連接狀態
        /// </summary>
        /// <param name="bluetoothAddress"> 藍牙地址 </param>
        /// <returns> 是否已連接 </returns>
        private async Task<bool> CheckWindowsDeviceConnection(string bluetoothAddress)
        {
            // Windows 連接檢查實作
            await Task.CompletedTask;
            return false;
        }

        /// <summary>
        /// 取得 macOS 已連接裝置
        /// </summary>
        /// <returns> 已連接裝置地址列表 </returns>
        private async Task<IEnumerable<string>> GetMacOSConnectedDevices()
        {
            try
            {
                var result = await ExecuteCommand("system_profiler SPBluetoothDataType");
                // 解析已連接的裝置
                return new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// 取得 Windows 已連接裝置
        /// </summary>
        /// <returns> 已連接裝置地址列表 </returns>
        private async Task<IEnumerable<string>> GetWindowsConnectedDevices()
        {
            // Windows 實作
            await Task.CompletedTask;
            return new List<string>();
        }

        /// <summary>
        /// Ping 藍牙裝置
        /// </summary>
        /// <param name="bluetoothAddress"> 藍牙地址 </param>
        /// <returns> 是否可達 </returns>
        private async Task<bool> PingBluetoothDevice(string bluetoothAddress)
        {
            // 藍牙 ping 實作
            await Task.CompletedTask;
            return false;
        }

        /// <summary>
        /// 執行系統命令
        /// </summary>
        /// <param name="command"> 命令 </param>
        /// <returns> 命令輸出 </returns>
        private async Task<string> ExecuteCommand(string command)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{command}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                return output;
            }
            catch (Exception ex)
            {
                log.Error(ex, "執行命令時發生錯誤: {Command}", command);
                return string.Empty;
            }
        }

        #endregion Private Methods
    }
} 