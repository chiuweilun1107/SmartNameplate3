using SmartNameplate.Api.DTOs;
using System.Text.Json;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace SmartNameplate.Api.Services;

public class NativeBluetoothService : IBluetoothService
{
    private readonly ILogger<NativeBluetoothService> _logger;
    
    // PH6 桌牌的識別模式
    private static readonly string[] PH6_PATTERNS = { "ph6", "ph-6", "nameplate", "eink", "epd", "epaper" };
    private static readonly string[] SMART_PATTERNS = { "smart", "display", "a1", "hpa" };

    public NativeBluetoothService(ILogger<NativeBluetoothService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> IsBluetoothAvailableAsync()
    {
        try
        {
            _logger.LogInformation("檢查藍牙可用性...");
            
            if (OperatingSystem.IsMacOS())
            {
                return await CheckMacOSBluetoothAsync();
            }
            else if (OperatingSystem.IsWindows())
            {
                return await CheckWindowsBluetoothAsync();
            }
            else
            {
                _logger.LogWarning("不支援的作業系統");
                return false;
            }
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
            _logger.LogInformation("開始使用原生 .NET 藍牙掃描...");

            // 檢查藍牙是否可用
            if (!await IsBluetoothAvailableAsync())
            {
                _logger.LogWarning("藍牙不可用，無法進行掃描");
                return discoveredDevices;
            }

            // 根據作業系統選擇掃描方法
            if (OperatingSystem.IsMacOS())
            {
                await ScanOnMacOSAsync(discoveredDevices, cancellationToken);
            }
            else if (OperatingSystem.IsWindows())
            {
                await ScanOnWindowsAsync(discoveredDevices, cancellationToken);
            }
            else
            {
                _logger.LogWarning("當前作業系統不支援藍牙掃描");
            }

            // 過濾和優先排序 PH6 設備
            var ph6Devices = discoveredDevices.Where(d => IsPotentialPH6Device(d.Name)).ToList();
            var otherDevices = discoveredDevices.Where(d => !IsPotentialPH6Device(d.Name)).ToList();

            // 重新組合，PH6 設備優先
            discoveredDevices.Clear();
            discoveredDevices.AddRange(ph6Devices);
            
            // 如果沒有找到 PH6 設備，添加其他可能的智能設備
            if (ph6Devices.Count == 0)
            {
                var smartDevices = otherDevices.Where(d => IsLikelySmartDevice(d.Name)).ToList();
                discoveredDevices.AddRange(smartDevices);
            }

            _logger.LogInformation("原生藍牙掃描完成，找到 {Total} 個設備，其中 {PH6Count} 個 PH6 設備", 
                discoveredDevices.Count, ph6Devices.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "藍牙掃描失敗");
        }

        return discoveredDevices;
    }

    private async Task<bool> CheckMacOSBluetoothAsync()
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "system_profiler",
                Arguments = "SPBluetoothDataType",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return process.ExitCode == 0 && output.Contains("Bluetooth:");
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> CheckWindowsBluetoothAsync()
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"Get-PnpDevice -FriendlyName '*Bluetooth*' | Select-Object Status\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return process.ExitCode == 0 && output.Contains("OK");
        }
        catch
        {
            return false;
        }
    }

    private async Task ScanOnMacOSAsync(List<BluetoothDeviceDto> discoveredDevices, CancellationToken cancellationToken)
    {
        // 方法 1: 掃描已知設備 (system_profiler)
        await ScanWithSystemProfilerAsync(discoveredDevices, cancellationToken);
        
        // 方法 2: 主動掃描新設備 (blueutil)
        await ScanWithBlueutilAsync(discoveredDevices, cancellationToken);
        
        // 方法 3: 如果還是沒找到 PH6，嘗試 BLE 掃描 (使用我們之前成功的 Python 方法)
        var ph6Devices = discoveredDevices.Where(d => IsPotentialPH6Device(d.Name)).ToList();
        if (ph6Devices.Count == 0)
        {
            _logger.LogInformation("未找到 PH6 設備，嘗試使用 Python BLE 掃描...");
            await ScanWithPythonBLEAsync(discoveredDevices, cancellationToken);
        }
    }

    private async Task ScanOnWindowsAsync(List<BluetoothDeviceDto> discoveredDevices, CancellationToken cancellationToken)
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"Get-PnpDevice -Class Bluetooth | Where-Object {$_.Status -eq 'OK'} | Select-Object FriendlyName, InstanceId\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode == 0)
            {
                ParseWindowsBluetoothOutput(output, discoveredDevices);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Windows 藍牙掃描失敗");
        }
    }

    private async Task ScanWithSystemProfilerAsync(List<BluetoothDeviceDto> discoveredDevices, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("使用 system_profiler 掃描已知設備...");
            
            var processInfo = new ProcessStartInfo
            {
                FileName = "system_profiler",
                Arguments = "SPBluetoothDataType -detailLevel full",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode == 0)
            {
                ParseSystemProfilerOutput(output, discoveredDevices);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "system_profiler 掃描失敗");
        }
    }

    private async Task ScanWithBlueutilAsync(List<BluetoothDeviceDto> discoveredDevices, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("使用 blueutil 掃描可發現設備...");
            
            var processInfo = new ProcessStartInfo
            {
                FileName = "blueutil",
                Arguments = "--inquiry 8",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode == 0)
            {
                ParseBlueutilOutput(output, discoveredDevices);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "blueutil 掃描失敗 - 可能未安裝 blueutil");
        }
    }

    private async Task ScanWithPythonBLEAsync(List<BluetoothDeviceDto> discoveredDevices, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("使用 Python BLE 掃描 PH6 設備...");
            
            // 取得專案根目錄（backend 的上一層）
            var currentDir = Directory.GetCurrentDirectory(); // backend 目錄
            var projectRoot = Path.GetDirectoryName(currentDir) ?? currentDir; // 專案根目錄
            var scriptPath = Path.Combine(projectRoot, "backend", "Pythons", "backend_ble_scanner.py");

            _logger.LogInformation("🔍 Python 腳本路徑: {ScriptPath}", scriptPath);
            _logger.LogInformation("🔍 當前工作目錄: {CurrentDir}", currentDir);
            _logger.LogInformation("🔍 專案根目錄: {ProjectRoot}", projectRoot);

            var processInfo = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"\"{scriptPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = projectRoot
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            var errorOutput = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);

            _logger.LogInformation("Python BLE 進程退出碼: {ExitCode}", process.ExitCode);
            if (!string.IsNullOrEmpty(errorOutput))
            {
                _logger.LogError("Python BLE 錯誤輸出: {ErrorOutput}", errorOutput);
            }

            if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                try
                {
                    _logger.LogInformation("Python BLE 原始輸出: {Output}", output);
                    
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    
                    var devices = JsonSerializer.Deserialize<List<BluetoothDeviceDto>>(output, options);
                    _logger.LogInformation("JSON 反序列化成功，設備數量: {Count}", devices?.Count ?? 0);
                    
                    if (devices != null && devices.Count > 0)
                    {
                        // 避免重複添加相同設備
                        foreach (var device in devices)
                        {
                            if (!discoveredDevices.Any(d => d.BluetoothAddress.Equals(device.BluetoothAddress, StringComparison.OrdinalIgnoreCase)))
                            {
                                _logger.LogInformation("Python BLE 發現設備: {Name} ({Address})", device.Name, device.BluetoothAddress);
                                discoveredDevices.Add(device);
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "解析 Python BLE 結果失敗");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Python BLE 掃描失敗");
        }
    }

    private void ParseSystemProfilerOutput(string output, List<BluetoothDeviceDto> discoveredDevices)
    {
        try
        {
            _logger.LogInformation("解析 system_profiler 輸出...");
            var lines = output.Split('\n');
            string? currentDeviceName = null;
            string? currentDeviceAddress = null;
            bool isConnected = false;
            int signalStrength = -50;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
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
                
                // 查找設備名稱
                if (trimmedLine.EndsWith(":") && 
                    line.StartsWith("          ") && 
                    !trimmedLine.Contains("Bluetooth") && 
                    !trimmedLine.Contains("Controller") &&
                    !trimmedLine.Contains("Address:"))
                {
                    currentDeviceName = trimmedLine.TrimEnd(':').Trim();
                    currentDeviceAddress = null;
                    signalStrength = -50;
                }
                
                // 查找MAC地址
                if (trimmedLine.StartsWith("Address:") && currentDeviceName != null)
                {
                    currentDeviceAddress = trimmedLine.Replace("Address:", "").Trim();
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
                
                // 添加設備
                if (!string.IsNullOrEmpty(currentDeviceName) && !string.IsNullOrEmpty(currentDeviceAddress))
                {
                    if (!discoveredDevices.Any(d => d.BluetoothAddress.Equals(currentDeviceAddress, StringComparison.OrdinalIgnoreCase)))
                    {
                        discoveredDevices.Add(new BluetoothDeviceDto
                        {
                            Name = currentDeviceName,
                            BluetoothAddress = FormatBluetoothAddress(currentDeviceAddress),
                            SignalStrength = signalStrength,
                            IsConnected = isConnected,
                            DeviceType = DetermineDeviceType(currentDeviceName)
                        });
                        
                        _logger.LogInformation("添加設備: {Name} ({Address}) - {Type}", 
                            currentDeviceName, currentDeviceAddress, DetermineDeviceType(currentDeviceName));
                    }
                    
                    currentDeviceName = null;
                    currentDeviceAddress = null;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解析 system_profiler 輸出時發生錯誤");
        }
    }

    private void ParseBlueutilOutput(string output, List<BluetoothDeviceDto> discoveredDevices)
    {
        try
        {
            _logger.LogInformation("解析 blueutil 輸出...");
            var lines = output.Split('\n');
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;
                
                if (trimmedLine.StartsWith("address:"))
                {
                    var parts = trimmedLine.Split(',');
                    if (parts.Length >= 2)
                    {
                        var addressPart = parts[0].Replace("address:", "").Trim();
                        var bluetoothAddress = addressPart.Replace("-", ":");
                        
                        var deviceName = "Unknown Device";
                        var namePart = parts.FirstOrDefault(p => p.Trim().StartsWith("name:"));
                        if (namePart != null)
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
                        
                        if (!discoveredDevices.Any(d => d.BluetoothAddress.Equals(bluetoothAddress, StringComparison.OrdinalIgnoreCase)))
                        {
                            discoveredDevices.Add(new BluetoothDeviceDto
                            {
                                Name = deviceName,
                                BluetoothAddress = FormatBluetoothAddress(bluetoothAddress),
                                SignalStrength = -60,
                                IsConnected = false,
                                DeviceType = DetermineDeviceType(deviceName)
                            });
                            
                            _logger.LogInformation("blueutil 發現設備: {Name} ({Address})", deviceName, bluetoothAddress);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解析 blueutil 輸出時發生錯誤");
        }
    }

    private string DetermineDeviceType(string deviceName)
    {
        var name = deviceName.ToLower();
        
        if (IsPotentialPH6Device(deviceName))
            return "Smart Nameplate";
        else if (name.Contains("airpods") || name.Contains("headphones") || name.Contains("earbuds"))
            return "Headphones";
        else if (name.Contains("mouse") || name.Contains("logi"))
            return "Mouse";
        else if (name.Contains("keyboard"))
            return "Keyboard";
        else if (name.Contains("iphone") || name.Contains("phone"))
            return "Phone";
        else if (name.Contains("laptop") || name.Contains("computer"))
            return "Computer";
        else if (IsLikelySmartDevice(deviceName))
            return "Smart Device";
        else
            return "Bluetooth Device";
    }

    private void ParseWindowsBluetoothOutput(string output, List<BluetoothDeviceDto> discoveredDevices)
    {
        _logger.LogInformation("解析 Windows 藍牙輸出...");
        // Windows 解析邏輯
    }

    private bool IsPotentialPH6Device(string? deviceName)
    {
        if (string.IsNullOrEmpty(deviceName))
            return false;

        var name = deviceName.ToLower();
        return PH6_PATTERNS.Any(pattern => name.Contains(pattern));
    }

    private bool IsLikelySmartDevice(string? deviceName)
    {
        if (string.IsNullOrEmpty(deviceName))
            return false;

        var name = deviceName.ToLower();
        return SMART_PATTERNS.Any(pattern => name.Contains(pattern)) ||
               (name.Length > 3 && (name.StartsWith("a1") || name.StartsWith("hpa")));
    }

    /// <summary>
    /// 格式化藍牙地址為標準 MAC 地址格式
    /// 例如: 6A422DCC-2730-B0E8-E8B8-1C513A0D7B10 -> 6A:42:2D:CC:27:30
    /// </summary>
    private string FormatBluetoothAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return address;

        try
        {
            _logger.LogInformation("格式化藍牙地址: 原始={Address}", address);
            
            // 移除所有分隔符號
            var cleanAddress = address.Replace("-", "").Replace(":", "").Replace(" ", "").ToUpper();
            
            _logger.LogInformation("清理後地址: {CleanAddress}, 長度: {Length}", cleanAddress, cleanAddress.Length);
            
            // 如果是長格式的 UUID (32位)，取前 12 位作為 MAC 地址
            if (cleanAddress.Length >= 12)
            {
                var macParts = new string[6];
                for (int i = 0; i < 6; i++)
                {
                    macParts[i] = cleanAddress.Substring(i * 2, 2);
                }
                var formattedAddress = string.Join(":", macParts);
                _logger.LogInformation("格式化結果: {FormattedAddress}", formattedAddress);
                return formattedAddress;
            }
            
            // 如果已經是標準 MAC 長度 (12位)，直接格式化
            if (cleanAddress.Length == 12)
            {
                var macParts = new string[6];
                for (int i = 0; i < 6; i++)
                {
                    macParts[i] = cleanAddress.Substring(i * 2, 2);
                }
                var formattedAddress = string.Join(":", macParts);
                _logger.LogInformation("格式化結果: {FormattedAddress}", formattedAddress);
                return formattedAddress;
            }
            
            // 如果格式不符，返回原始地址
            _logger.LogInformation("無法格式化，返回原始地址: {Address}", address);
            return address;
        }
        catch (Exception ex)
        {
            // 格式化失敗，返回原始地址
            _logger.LogError(ex, "格式化藍牙地址失敗: {Address}", address);
            return address;
        }
    }

    // 🔍 實現新的藍牙連接監控方法
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
            var processInfo = new ProcessStartInfo
            {
                FileName = "system_profiler",
                Arguments = "SPBluetoothDataType -detailLevel full",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                // 解析輸出，檢查特定設備是否在 "Connected:" 區域
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
                        var currentDeviceAddress = trimmedLine.Replace("Address:", "").Trim();
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
            var processInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-Command \"Get-PnpDevice -Class Bluetooth | Where-Object {{$_.Status -eq 'OK' -and $_.InstanceId -like '*{bluetoothAddress}*'}}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
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
            var processInfo = new ProcessStartInfo
            {
                FileName = "system_profiler",
                Arguments = "SPBluetoothDataType -detailLevel full",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
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
            var processInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"Get-PnpDevice -Class Bluetooth | Where-Object {$_.Status -eq 'OK'} | Select-Object InstanceId\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
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

            // 取得專案根目錄（backend 的上一層）
            var currentDir = Directory.GetCurrentDirectory(); // backend 目錄
            var projectRoot = Path.GetDirectoryName(currentDir) ?? currentDir; // 專案根目錄
            var scriptPath = Path.Combine(projectRoot, "backend", "Pythons", "backend_ble_scanner.py");

            _logger.LogInformation("🔍 Python 腳本路徑: {ScriptPath}", scriptPath);
            _logger.LogInformation("🔍 當前工作目錄: {CurrentDir}", currentDir);
            _logger.LogInformation("🔍 專案根目錄: {ProjectRoot}", projectRoot);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"\"{scriptPath}\" --ping {bluetoothAddress}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = projectRoot
            };

            using var process = new Process { StartInfo = processStartInfo };
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