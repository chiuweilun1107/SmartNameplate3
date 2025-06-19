using SmartNameplate.Api.DTOs;
using System.Text.Json;

namespace SmartNameplate.Api.Services;

public class BluetoothService : IBluetoothService
{
    private readonly ILogger<BluetoothService> _logger;

    public BluetoothService(ILogger<BluetoothService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> IsBluetoothAvailableAsync()
    {
        try
        {
            // 在實際環境中，這裡應該檢查藍牙適配器狀態
            // 目前為了測試，先返回 true
            _logger.LogInformation("檢查藍牙可用性...");
            await Task.Delay(100); // 模擬檢查時間
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查藍牙可用性時發生錯誤");
            return false;
        }
    }

    public async Task<IEnumerable<BluetoothDeviceDto>> ScanForDevicesAsync(CancellationToken cancellationToken = default)
    {
        var discoveredDevices = new List<BluetoothDeviceDto>();

        try
        {
            _logger.LogInformation("開始掃描附近的藍牙設備...");
            
            // 在 macOS 上使用系統命令掃描藍牙設備
            if (OperatingSystem.IsMacOS())
            {
                await ScanBluetoothOnMacOS(discoveredDevices, cancellationToken);
            }
            else
            {
                _logger.LogWarning("當前作業系統不支援藍牙掃描，請在 macOS、Windows 或 Linux 上運行");
                await Task.Delay(1000, cancellationToken); // 模擬掃描時間
            }
            
            _logger.LogInformation("藍牙掃描完成，找到 {Count} 個設備", discoveredDevices.Count);
            
            if (discoveredDevices.Count == 0)
            {
                _logger.LogInformation("未找到任何藍牙設備。請確保：");
                _logger.LogInformation("1. 藍牙已啟用");
                _logger.LogInformation("2. 附近有可發現的藍牙設備");
                _logger.LogInformation("3. 應用程式有藍牙權限");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("藍牙掃描被取消");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "藍牙掃描過程中發生錯誤");
        }

        return discoveredDevices;
    }

    private async Task ScanBluetoothOnMacOS(List<BluetoothDeviceDto> discoveredDevices, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("在 macOS 上掃描藍牙設備...");
            
            // 優先使用 Python BLE 掃描來找到真實的 PH6 桌牌設備
            await ScanWithPythonBLE(discoveredDevices, cancellationToken);
            
            // 如果沒有找到 PH6 設備，再使用傳統方法
            if (discoveredDevices.Count == 0)
            {
                _logger.LogInformation("未找到 PH6 設備，使用傳統掃描方法...");
                
                // 方法 1: 使用 system_profiler 獲取已知設備
                await ScanWithSystemProfiler(discoveredDevices, cancellationToken);
                
                // 方法 2: 使用 blueutil 進行主動掃描發現新設備
                await ScanWithBlueutil(discoveredDevices, cancellationToken);
            }
            
            _logger.LogInformation("macOS 藍牙掃描完成，共發現 {Count} 個設備", discoveredDevices.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "macOS 藍牙掃描失敗");
        }
    }

    private async Task ScanWithPythonBLE(List<BluetoothDeviceDto> discoveredDevices, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("使用 Python BLE 掃描 PH6 桌牌設備...");
            
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "python3",
                Arguments = "backend_ble_scanner.py",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = "/Users/chiuyongren/Desktop/SmartNameplateC" // 設置工作目錄
            };

            using var process = new System.Diagnostics.Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                try
                {
                    var devices = JsonSerializer.Deserialize<List<BluetoothDeviceDto>>(output);
                    if (devices != null && devices.Count > 0)
                    {
                        discoveredDevices.AddRange(devices);
                        _logger.LogInformation("Python BLE 掃描發現 {Count} 個 PH6 設備", devices.Count);
                        
                        foreach (var device in devices)
                        {
                            _logger.LogInformation("發現 PH6 設備: {Name} ({Address})", device.Name, device.BluetoothAddress);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Python BLE 掃描未發現任何 PH6 設備");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "解析 Python BLE 掃描結果失敗: {Output}", output);
                }
            }
            else
            {
                var error = await process.StandardError.ReadToEndAsync();
                _logger.LogWarning("Python BLE 掃描失敗: {Error}", error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Python BLE 掃描出錯: {Message}", ex.Message);
            _logger.LogInformation("請確保已安裝 Python 3 和 bleak 庫: pip3 install bleak");
        }
    }

    private async Task ScanWithSystemProfiler(List<BluetoothDeviceDto> discoveredDevices, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("使用 system_profiler 掃描已知設備...");
            
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "system_profiler",
                Arguments = "SPBluetoothDataType -detailLevel full",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new System.Diagnostics.Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode == 0)
            {
                ParseMacOSBluetoothOutput(output, discoveredDevices);
            }
            else
            {
                var error = await process.StandardError.ReadToEndAsync();
                _logger.LogWarning("system_profiler 掃描失敗: {Error}", error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "system_profiler 掃描出錯");
        }
    }

    private async Task ScanWithBlueutil(List<BluetoothDeviceDto> discoveredDevices, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("使用 blueutil 掃描附近可發現的設備...");
            
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "blueutil",
                Arguments = "--inquiry 10", // 掃描 10 秒
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new System.Diagnostics.Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode == 0)
            {
                ParseBlueutilOutput(output, discoveredDevices);
            }
            else
            {
                var error = await process.StandardError.ReadToEndAsync();
                _logger.LogWarning("blueutil 掃描失敗: {Error}", error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "blueutil 掃描出錯: {Message}", ex.Message);
            _logger.LogInformation("如果 blueutil 未安裝，請執行: brew install blueutil");
        }
    }

    private void ParseMacOSBluetoothOutput(string output, List<BluetoothDeviceDto> discoveredDevices)
    {
        try
        {
            var lines = output.Split('\n');
            string currentDeviceName = null;
            string currentDeviceAddress = null;
            bool isConnected = false;
            int signalStrength = -50;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // 檢查是否進入已連接或未連接的設備區域
                if (trimmedLine.Equals("Connected:"))
                {
                    isConnected = true;
                    continue;
                }
                else if (trimmedLine.Equals("Not Connected:"))
                {
                    isConnected = false;
                    continue;
                }
                
                // 查找設備名稱（縮進的行，以冒號結尾，不是屬性行）
                if (trimmedLine.EndsWith(":") && 
                    line.StartsWith("          ") && // 設備名稱有固定縮進
                    !trimmedLine.Contains("Bluetooth") && 
                    !trimmedLine.Contains("Controller") &&
                    !trimmedLine.Contains("Address:") &&
                    !trimmedLine.Contains("Connected:"))
                {
                    currentDeviceName = trimmedLine.TrimEnd(':').Trim();
                    currentDeviceAddress = null;
                    signalStrength = -50; // 重置信號強度
                    
                    _logger.LogInformation("找到設備名稱: {Name}", currentDeviceName);
                }
                
                // 查找MAC地址
                if (trimmedLine.StartsWith("Address:") && currentDeviceName != null)
                {
                    currentDeviceAddress = trimmedLine.Replace("Address:", "").Trim();
                    _logger.LogInformation("找到設備地址: {Address}", currentDeviceAddress);
                }
                
                // 查找信號強度
                if (trimmedLine.StartsWith("RSSI:") && currentDeviceName != null)
                {
                    var rssiStr = trimmedLine.Replace("RSSI:", "").Trim();
                    if (int.TryParse(rssiStr, out int rssi))
                    {
                        signalStrength = rssi;
                    }
                }
                
                // 當我們有設備名稱和地址時，添加到列表
                if (!string.IsNullOrEmpty(currentDeviceName) && !string.IsNullOrEmpty(currentDeviceAddress))
                {
                    discoveredDevices.Add(new BluetoothDeviceDto
                    {
                        Name = currentDeviceName,
                        BluetoothAddress = currentDeviceAddress,
                        SignalStrength = signalStrength,
                        IsConnected = isConnected,
                        DeviceType = DetermineDeviceType(currentDeviceName)
                    });
                    
                    _logger.LogInformation("成功添加藍牙設備: {Name} ({Address}) - 連接狀態: {Connected}", 
                        currentDeviceName, currentDeviceAddress, isConnected ? "已連接" : "未連接");
                    
                    currentDeviceName = null;
                    currentDeviceAddress = null;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解析藍牙掃描結果時發生錯誤");
        }
    }

    private void ParseBlueutilOutput(string output, List<BluetoothDeviceDto> discoveredDevices)
    {
        try
        {
            var lines = output.Split('\n');
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;
                
                // blueutil 輸出格式: address: XX-XX-XX-XX-XX-XX, status, name: "DeviceName"
                if (trimmedLine.StartsWith("address:"))
                {
                    var parts = trimmedLine.Split(',');
                    if (parts.Length >= 2)
                    {
                        // 提取 MAC 地址
                        var addressPart = parts[0].Replace("address:", "").Trim();
                        var bluetoothAddress = addressPart.Replace("-", ":");
                        
                        // 提取設備名稱
                        var deviceName = "Unknown Device";
                        var namePart = parts.FirstOrDefault(p => p.Trim().StartsWith("name:"));
                        if (!string.IsNullOrEmpty(namePart))
                        {
                            var nameValue = namePart.Replace("name:", "").Trim();
                            if (nameValue.StartsWith("\"") && nameValue.EndsWith("\""))
                            {
                                deviceName = nameValue.Substring(1, nameValue.Length - 2);
                            }
                            else
                            {
                                deviceName = nameValue;
                            }
                        }
                        
                        // 檢查是否已經存在相同的設備
                        if (!discoveredDevices.Any(d => d.BluetoothAddress.Equals(bluetoothAddress, StringComparison.OrdinalIgnoreCase)))
                        {
                            discoveredDevices.Add(new BluetoothDeviceDto
                            {
                                Name = deviceName,
                                BluetoothAddress = bluetoothAddress,
                                SignalStrength = -60, // blueutil 不提供信號強度，使用預設值
                                IsConnected = false, // blueutil 發現的都是未連接設備
                                DeviceType = DetermineDeviceType(deviceName)
                            });
                            
                            _logger.LogInformation("發現新的藍牙設備 (blueutil): {Name} ({Address})", deviceName, bluetoothAddress);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解析 blueutil 掃描結果時發生錯誤");
        }
    }

    private string DetermineDeviceType(string deviceName)
    {
        var name = deviceName.ToLower();
        
        if (name.Contains("airpods") || name.Contains("headphones") || name.Contains("earbuds"))
            return "Headphones";
        else if (name.Contains("mouse") || name.Contains("logi"))
            return "Mouse";
        else if (name.Contains("keyboard"))
            return "Keyboard";
        else if (name.Contains("iphone") || name.Contains("phone"))
            return "Phone";
        else if (name.Contains("laptop") || name.Contains("computer"))
            return "Computer";
        else if (name.Contains("nameplate") || name.Contains("eink") || name.Contains("epaper"))
            return "Smart Nameplate";
        else if (name.StartsWith("a1") || name.StartsWith("hpa") || name.Contains("ph6") || 
                 name.Contains("smart") || IsLikelyNameplateDevice(name))
            return "Smart Device";
        else
            return "Bluetooth Device";
    }

    private bool IsLikelyNameplateDevice(string deviceName)
    {
        // 根據 PH6 桌牌的命名模式判斷是否為桌牌設備
        var name = deviceName.ToLower();
        
        // 檢查是否符合常見的桌牌命名模式
        return name.StartsWith("a") && name.Length > 5 ||  // A109012002 類型
               name.StartsWith("hpa") ||                    // HPA110042801 類型
               name.Contains("ph") ||                       // PH6 系列
               name.Contains("eink") ||                     // 電子墨水屏
               name.Contains("epd") ||                      // Electronic Paper Display
               name.Contains("nameplate");                  // 桌牌
    }

    // 🔍 藍牙連接監控實作
    public async Task<bool> CheckDeviceConnectionAsync(string bluetoothAddress)
    {
        try
        {
            _logger.LogInformation("檢查設備連接狀態: {Address}", bluetoothAddress);

            if (OperatingSystem.IsMacOS())
            {
                return await CheckMacOSDeviceConnectionAsync(bluetoothAddress);
            }
            else if (OperatingSystem.IsWindows())
            {
                return await CheckWindowsDeviceConnectionAsync(bluetoothAddress);
            }
            else
            {
                _logger.LogWarning("不支援的作業系統，無法檢查設備連接狀態");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查設備連接狀態時發生錯誤: {Address}", bluetoothAddress);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetConnectedDeviceAddressesAsync()
    {
        try
        {
            _logger.LogInformation("取得所有已連接的藍牙設備地址");

            if (OperatingSystem.IsMacOS())
            {
                return await GetMacOSConnectedDevicesAsync();
            }
            else if (OperatingSystem.IsWindows())
            {
                return await GetWindowsConnectedDevicesAsync();
            }
            else
            {
                _logger.LogWarning("不支援的作業系統，無法取得連接設備列表");
                return new List<string>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得連接設備列表時發生錯誤");
            return new List<string>();
        }
    }

    public async Task<bool> IsDeviceReachableAsync(string bluetoothAddress)
    {
        try
        {
            _logger.LogInformation("檢查設備是否可達: {Address}", bluetoothAddress);

            // 方法1：檢查是否在已連接設備列表中
            var connectedDevices = await GetConnectedDeviceAddressesAsync();
            if (connectedDevices.Contains(bluetoothAddress))
            {
                _logger.LogInformation("設備在已連接列表中: {Address}", bluetoothAddress);
                return true;
            }

            // 方法2：嘗試 ping 設備（使用 Python BLE 腳本）
            return await PingBluetoothDeviceAsync(bluetoothAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查設備可達性時發生錯誤: {Address}", bluetoothAddress);
            return false;
        }
    }

    private async Task<bool> CheckMacOSDeviceConnectionAsync(string bluetoothAddress)
    {
        try
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "system_profiler",
                Arguments = "SPBluetoothDataType -detailLevel full",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new System.Diagnostics.Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                // 解析輸出，檢查特定設備是否在 "Connected:" 區域
                var lines = output.Split('\n');
                bool inConnectedSection = false;
                string currentDeviceAddress = null;

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    
                    if (trimmedLine.Equals("Connected:"))
                    {
                        inConnectedSection = true;
                        continue;
                    }
                    else if (trimmedLine.Equals("Not Connected:"))
                    {
                        inConnectedSection = false;
                        continue;
                    }

                    if (inConnectedSection && trimmedLine.StartsWith("Address:"))
                    {
                        currentDeviceAddress = trimmedLine.Replace("Address:", "").Trim();
                        if (currentDeviceAddress.Equals(bluetoothAddress, StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation("設備已連接: {Address}", bluetoothAddress);
                            return true;
                        }
                    }
                }
            }

            _logger.LogInformation("設備未連接: {Address}", bluetoothAddress);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查 macOS 設備連接狀態失敗");
            return false;
        }
    }

    private async Task<bool> CheckWindowsDeviceConnectionAsync(string bluetoothAddress)
    {
        try
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-Command \"Get-PnpDevice -Class Bluetooth | Where-Object {{$_.Status -eq 'OK' -and $_.InstanceId -like '*{bluetoothAddress}*'}}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new System.Diagnostics.Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查 Windows 設備連接狀態失敗");
            return false;
        }
    }

    private async Task<IEnumerable<string>> GetMacOSConnectedDevicesAsync()
    {
        var connectedAddresses = new List<string>();

        try
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "system_profiler",
                Arguments = "SPBluetoothDataType -detailLevel full",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new System.Diagnostics.Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                var lines = output.Split('\n');
                bool inConnectedSection = false;

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    
                    if (trimmedLine.Equals("Connected:"))
                    {
                        inConnectedSection = true;
                        continue;
                    }
                    else if (trimmedLine.Equals("Not Connected:"))
                    {
                        inConnectedSection = false;
                        continue;
                    }

                    if (inConnectedSection && trimmedLine.StartsWith("Address:"))
                    {
                        var address = trimmedLine.Replace("Address:", "").Trim();
                        connectedAddresses.Add(address);
                        _logger.LogDebug("找到已連接設備: {Address}", address);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得 macOS 連接設備列表失敗");
        }

        return connectedAddresses;
    }

    private async Task<IEnumerable<string>> GetWindowsConnectedDevicesAsync()
    {
        var connectedAddresses = new List<string>();

        try
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"Get-PnpDevice -Class Bluetooth | Where-Object {$_.Status -eq 'OK'} | Select-Object InstanceId\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new System.Diagnostics.Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                // 解析 Windows PowerShell 輸出，提取藍牙地址
                var lines = output.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains("BTHENUM"))
                    {
                        // 從 InstanceId 中提取藍牙地址
                        // 格式通常是: BTHENUM\{...}\{MAC_ADDRESS}&...
                        var parts = line.Split('\\', '&');
                        foreach (var part in parts)
                        {
                            if (part.Length == 12 && part.All(c => char.IsLetterOrDigit(c)))
                            {
                                // 轉換為標準 MAC 地址格式
                                var formattedAddress = string.Join(":", 
                                    Enumerable.Range(0, 6)
                                    .Select(i => part.Substring(i * 2, 2)));
                                connectedAddresses.Add(formattedAddress);
                                break;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得 Windows 連接設備列表失敗");
        }

        return connectedAddresses;
    }

    private async Task<bool> PingBluetoothDeviceAsync(string bluetoothAddress)
    {
        try
        {
            _logger.LogInformation("嘗試 ping 藍牙設備: {Address}", bluetoothAddress);

            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"backend_ble_scanner.py --ping {bluetoothAddress}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = "/Users/chiuyongren/Desktop/SmartNameplateC"
            };

            using var process = new System.Diagnostics.Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                _logger.LogInformation("設備 ping 成功: {Address}", bluetoothAddress);
                return true;
            }
            else
            {
                var error = await process.StandardError.ReadToEndAsync();
                _logger.LogWarning("設備 ping 失敗: {Address}, 錯誤: {Error}", bluetoothAddress, error);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ping 藍牙設備時發生錯誤: {Address}", bluetoothAddress);
            return false;
        }
    }
} 