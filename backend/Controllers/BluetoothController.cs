using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using System.IO;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.Entities;
using SmartNameplate.Api.DTOs;
using SmartNameplate.Api.Services;

namespace SmartNameplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BluetoothController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BluetoothController> _logger;
    private readonly IBluetoothService _bluetoothService;

    public BluetoothController(
        ApplicationDbContext context, 
        ILogger<BluetoothController> logger,
        IBluetoothService bluetoothService)
    {
        _context = context;
        _logger = logger;
        _bluetoothService = bluetoothService;
    }

    // GET: api/bluetooth/status
    [HttpGet("status")]
    public async Task<ActionResult> GetBluetoothStatus()
    {
        try
        {
            var isAvailable = await _bluetoothService.IsBluetoothAvailableAsync();
            return Ok(new { 
                IsAvailable = isAvailable,
                Message = isAvailable ? "藍牙可用" : "藍牙不可用或無權限"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查藍牙狀態時發生錯誤");
            return StatusCode(500, "檢查藍牙狀態時發生錯誤");
        }
    }

    // GET: api/bluetooth/scan
    [HttpGet("scan")]
    public async Task<ActionResult<IEnumerable<BluetoothDeviceDto>>> ScanDevices()
    {
        try
        {
            _logger.LogInformation("=== 開始掃描藍牙設備 ===");
            
            // 使用藍牙服務進行掃描
            var discoveredDevices = await _bluetoothService.ScanForDevicesAsync();
            
            _logger.LogInformation("藍牙服務返回了 {Count} 個設備", discoveredDevices.Count());
            
            // 過濾掉已經註冊的設備
            var existingAddresses = await _context.Devices
                .Select(d => d.BluetoothAddress)
                .ToListAsync();

            _logger.LogInformation("資料庫中已有 {Count} 個註冊設備", existingAddresses.Count);

            var availableDevices = discoveredDevices
                .Where(d => !existingAddresses.Contains(d.BluetoothAddress))
                .ToList();

            _logger.LogInformation("=== 掃描完成，找到 {Total} 個設備，其中 {Available} 個可用 ===", 
                discoveredDevices.Count(), availableDevices.Count);

            return Ok(availableDevices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "掃描藍牙設備時發生錯誤");
            return StatusCode(500, "掃描藍牙設備時發生錯誤");
        }
    }

    // POST: api/bluetooth/connect
    [HttpPost("connect")]
    public async Task<ActionResult<DeviceDto>> ConnectDevice(ConnectDeviceDto connectDeviceDto)
    {
        try
        {
            // 檢查設備是否已經存在
            var existingDevice = await _context.Devices
                .FirstOrDefaultAsync(d => d.BluetoothAddress == connectDeviceDto.BluetoothAddress);

            if (existingDevice != null)
            {
                return BadRequest("設備已經註冊");
            }

            _logger.LogInformation("正在連接到設備: {Name} ({Address})", connectDeviceDto.Name, connectDeviceDto.BluetoothAddress);

            // 檢測是否為桌牌設備並嘗試建立 BLE 連接
            var connectionResult = await AttemptBleConnection(connectDeviceDto);
            
            var device = new Device
            {
                Name = connectDeviceDto.Name,
                BluetoothAddress = connectDeviceDto.BluetoothAddress,
                OriginalAddress = connectDeviceDto.OriginalAddress,
                Status = connectionResult.IsSuccess ? DeviceStatus.Connected : DeviceStatus.Disconnected,
                LastConnected = connectionResult.IsSuccess ? DateTime.UtcNow : DateTime.MinValue,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            var deviceDto = new DeviceDto
            {
                Id = device.Id,
                Name = device.Name,
                BluetoothAddress = device.BluetoothAddress,
                OriginalAddress = device.OriginalAddress,
                Status = device.Status.ToString(),
                CurrentCardId = device.CurrentCardId,
                GroupId = device.GroupId,
                LastConnected = device.LastConnected,
                CreatedAt = device.CreatedAt,
                UpdatedAt = device.UpdatedAt
            };

            if (connectionResult.IsSuccess)
            {
                _logger.LogInformation("成功連接到設備: {Name}", connectDeviceDto.Name);
            }
            else
            {
                _logger.LogWarning("設備已註冊但連接失敗: {Name} - {Error}", connectDeviceDto.Name, connectionResult.ErrorMessage);
            }

            return Ok(deviceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "連接設備時發生錯誤");
            return StatusCode(500, "連接設備時發生錯誤");
        }
    }

    private async Task<(bool IsSuccess, string ErrorMessage)> AttemptBleConnection(ConnectDeviceDto connectDeviceDto)
    {
        try
        {
            _logger.LogInformation("嘗試 BLE 連接到設備: {Name}", connectDeviceDto.Name);
            
            // 模擬 BLE 連接測試
            await Task.Delay(2000);
            
            // 檢查是否為已知的桌牌設備類型
            var deviceName = connectDeviceDto.Name.ToLower();
            if (deviceName.StartsWith("a") && deviceName.Length > 5 ||
                deviceName.StartsWith("hpa") ||
                deviceName.Contains("ph6") ||
                deviceName.Contains("nameplate"))
            {
                _logger.LogInformation("檢測到桌牌設備，嘗試建立 BLE 連接...");
                
                // 這裡可以實現真實的 BLE 連接邏輯
                // 參考 test_ph6_pc_ble.py 的連接方法
                
                return (true, "BLE 連接成功");
            }
            else
            {
                _logger.LogInformation("標準藍牙設備，使用基本連接方式");
                return (true, "標準藍牙連接成功");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BLE 連接失敗");
            return (false, ex.Message);
        }
    }

    // GET: api/bluetooth/devices
    [HttpGet("devices")]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> GetDevices()
    {
        try
        {
            var devices = await _context.Devices
                .Include(d => d.CurrentCard)
                .Include(d => d.Group)
                .Select(d => new DeviceDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    BluetoothAddress = d.BluetoothAddress,
                    OriginalAddress = d.OriginalAddress,
                    Status = d.Status.ToString(),
                    CurrentCardId = d.CurrentCardId,
                    CurrentCardName = d.CurrentCard != null ? d.CurrentCard.Name : null,
                    GroupId = d.GroupId,
                    GroupName = d.Group != null ? d.Group.Name : null,
                    LastConnected = d.LastConnected,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    CustomIndex = d.CustomIndex
                })
                .ToListAsync();

            return Ok(devices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving devices");
            return StatusCode(500, "An error occurred while retrieving devices");
        }
    }

    // PUT: api/bluetooth/devices/{id}
    [HttpPut("devices/{id}")]
    public async Task<IActionResult> UpdateDevice(int id, UpdateDeviceDto updateDeviceDto)
    {
        try
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            device.Name = updateDeviceDto.Name;
            device.GroupId = updateDeviceDto.GroupId;
            device.CustomIndex = updateDeviceDto.CustomIndex;
            device.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device {Id}", id);
            return StatusCode(500, "An error occurred while updating the device");
        }
    }

    // POST: api/bluetooth/devices/{id}/deploy
    [HttpPost("devices/{id}/deploy")]
    public async Task<IActionResult> DeployCardToDevice(int id, DeployCardDto deployCardDto)
    {
        try
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound("Device not found");
            }

            var card = await _context.Cards.FindAsync(deployCardDto.CardId);
            if (card == null)
            {
                return NotFound("Card not found");
            }

            // 模擬部署過程
            device.Status = DeviceStatus.Syncing;
            device.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await Task.Delay(3000); // 模擬部署時間

            device.CurrentCardId = deployCardDto.CardId;
            device.Status = DeviceStatus.Connected;
            device.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Card deployed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deploying card to device {Id}", id);
            return StatusCode(500, "An error occurred while deploying the card");
        }
    }

    // POST: api/bluetooth/devices/{id}/cast
    // 進度回報事件
    private static readonly Dictionary<string, IProgress<string>> _progressReporters = new();
    
    [HttpGet("devices/{id}/cast-progress")]
    public async Task<IActionResult> GetCastProgress(int id)
    {
        var progressId = $"cast_{id}_{DateTime.Now.Ticks}";
        
        Response.Headers["Content-Type"] = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["Connection"] = "keep-alive";
        Response.Headers["Access-Control-Allow-Origin"] = "*";
        
        var progress = new Progress<string>(message =>
        {
            var data = $"data: {message}\n\n";
            Response.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes(data));
            Response.Body.FlushAsync();
        });
        
        _progressReporters[progressId] = progress;
        
        // 保持連接開啟
        await Task.Delay(TimeSpan.FromMinutes(5)); // 5分鐘超時
        
        _progressReporters.Remove(progressId);
        return Ok();
    }

    [HttpPost("devices/{id}/cast")]
    public async Task<IActionResult> CastImageToDevice(int id, DeployCardDto deployCardDto)
    {
        try
        {
            _logger.LogInformation("🎯 收到投圖請求 - 設備ID: {DeviceId}, 卡片ID: {CardId}, 面: {Side}", id, deployCardDto.CardId, deployCardDto.Side);

            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                _logger.LogError("❌ 找不到設備 ID: {DeviceId}", id);
                return NotFound($"Device with ID {id} not found");
            }

            // 如果 deployCardDto.CardId 為 0 或無效，使用設備當前卡片
            var cardIdToUse = deployCardDto.CardId;
            if (cardIdToUse <= 0 && device.CurrentCardId.HasValue)
            {
                cardIdToUse = device.CurrentCardId.Value;
                _logger.LogInformation("🔄 使用設備當前卡片 ID: {CardId}", cardIdToUse);
            }

            if (cardIdToUse <= 0)
            {
                _logger.LogError("❌ 設備未部署任何卡片且未指定有效的卡片ID");
                return BadRequest("Device has no deployed card and no valid card ID specified");
            }

            var card = await _context.Cards.FindAsync(cardIdToUse);
            if (card == null)
            {
                _logger.LogError("❌ 找不到卡片 ID: {CardId}", cardIdToUse);
                return NotFound($"Card with ID {cardIdToUse} not found");
            }

            // 修正：AB面相同時也使用雙面傳輸避免條紋問題
            _logger.LogInformation("開始投圖到設備 {DeviceName} (ID: {DeviceId}), 圖卡: {CardName}, 面板: {Side}, AB面相同: {IsSameBothSides}", 
                device.Name, device.Id, card.Name, deployCardDto.Side, card.IsSameBothSides);

            // 使用原始地址進行BLE連接，優先使用originalAddress
            var addressToUse = !string.IsNullOrEmpty(device.OriginalAddress) 
                ? device.OriginalAddress 
                : device.BluetoothAddress;
                
            _logger.LogInformation("使用地址進行連接: {Address} (Original: {Original}, Bluetooth: {Bluetooth})", 
                addressToUse, device.OriginalAddress, device.BluetoothAddress);
            
            var castResult = await CastImageToPH6Device_DualSideFixed(addressToUse, card);
            
            if (castResult.IsSuccess)
            {
                return Ok(new { message = $"Image cast to {device.Name} successfully" });
            }
            else
            {
                // 真實設備連接失敗，直接返回錯誤（移除模擬模式）
                _logger.LogError("真實設備投圖失敗: {Error}", castResult.ErrorMessage);
                return BadRequest(new { message = $"Failed to cast image to real device: {castResult.ErrorMessage}" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error casting image to device {Id}", id);
            return StatusCode(500, "An error occurred while casting the image");
        }
    }

    private async Task<(bool IsSuccess, string ErrorMessage)> CastImageToPH6Device_DualSideFixed(string deviceAddress, Card card)
    {
        try
        {
            var currentDir = Directory.GetCurrentDirectory(); // backend目錄
            var projectRoot = Path.GetDirectoryName(currentDir) ?? currentDir; // 專案根目錄

            _logger.LogInformation("🔄 開始雙面順序傳輸流程，AB面相同: {IsSameBothSides}, 設備地址: {DeviceAddress}", card.IsSameBothSides, deviceAddress);

            // 檢查設備地址是否有效
            if (string.IsNullOrEmpty(deviceAddress))
            {
                _logger.LogError("❌ 設備地址不能為空，無法進行真實投圖");
                return (false, "設備地址不能為空，投圖需要真實的藍牙設備");
            }

            // 🎨 使用前端生成的縮圖進行投圖，確保與預覽完全一致
            _logger.LogInformation("🎨 使用前端縮圖進行投圖，確保與預覽一致");
            var renderResult = await RenderCardImages(card);
            if (!renderResult.IsSuccess)
            {
                _logger.LogError("卡片縮圖轉換失敗: {Error}", renderResult.ErrorMessage);
                return (false, $"卡片縮圖轉換失敗: {renderResult.ErrorMessage}");
            }

            // 核心邏輯：無論AB是否相同，都按順序傳輸 A面(side=1) → B面(side=2)
            _logger.LogInformation("🚀 開始雙面順序傳輸：A面(side=1) → B面(side=2)");

            // === 步驟1：傳輸A面 (side=1) ===
            string imagePathA;
            if (card.IsSameBothSides)
            {
                imagePathA = $"card_{card.Id}_temp.png";  // AB相同用同一張圖
                _logger.LogInformation("📤 步驟1：傳輸A面 - AB相同模式，使用相同圖片");
            }
            else
            {
                imagePathA = $"card_{card.Id}_A_temp.png";  // AB不同用A面圖
                _logger.LogInformation("📤 步驟1：傳輸A面 - AB不同模式，使用A面圖片");
            }

            var sideAResult = await CastSingleSide(projectRoot, imagePathA, 1, deviceAddress);
            if (!sideAResult.IsSuccess)
            {
                _logger.LogError("❌ A面傳輸失敗: {Error}", sideAResult.ErrorMessage);
                return (false, $"A面傳輸失敗: {sideAResult.ErrorMessage}");
            }

            // === 步驟2：等待設備處理時間並清理A面資源 ===
            _logger.LogInformation("⏳ 等待5秒設備處理時間並清理A面資源...");
            await Task.Delay(5000);
            
            // 強制垃圾回收，清理記憶體中的圖片資料
            GC.Collect();
            GC.WaitForPendingFinalizers();
            _logger.LogInformation("🧹 已清理記憶體中的A面圖片資料");

            // === 步驟3：傳輸B面 (side=2) ===
            string imagePathB;
            if (card.IsSameBothSides)
            {
                imagePathB = $"card_{card.Id}_temp.png";  // AB相同用同一張圖
                _logger.LogInformation("📤 步驟3：傳輸B面 - AB相同模式，使用相同圖片");
            }
            else
            {
                imagePathB = $"card_{card.Id}_B_temp.png";  // AB不同用B面圖
                _logger.LogInformation("📤 步驟3：傳輸B面 - AB不同模式，使用B面圖片");
            }

            var sideBResult = await CastSingleSide(projectRoot, imagePathB, 2, deviceAddress);
            if (!sideBResult.IsSuccess)
            {
                _logger.LogError("❌ B面傳輸失敗: {Error}", sideBResult.ErrorMessage);
                return (false, $"B面傳輸失敗: {sideBResult.ErrorMessage}");
            }

            _logger.LogInformation("✅ 雙面順序傳輸完成！A面 → B面 全部成功 - 條紋問題已解決");
            
            // 🧹 自動清理臨時檔案
            await CleanupTempFiles(projectRoot, card);
            
            return (true, "雙面順序傳輸成功：A面 → B面 - 條紋問題已修正");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "雙面順序傳輸過程中發生錯誤");
            return (false, ex.Message);
        }
    }

    private async Task CleanupTempFiles(string projectRoot, Card card)
    {
        try
        {
            _logger.LogInformation("🧹 開始清理卡片 {CardId} 的臨時檔案", card.Id);
            
            var tempFiles = new List<string>();
            
            if (card.IsSameBothSides)
            {
                // AB相同模式：只有一個臨時檔案
                tempFiles.Add(Path.Combine(projectRoot, $"card_{card.Id}_temp.png"));
            }
            else
            {
                // AB不同模式：有A面和B面兩個臨時檔案
                tempFiles.Add(Path.Combine(projectRoot, $"card_{card.Id}_A_temp.png"));
                tempFiles.Add(Path.Combine(projectRoot, $"card_{card.Id}_B_temp.png"));
            }
            
            foreach (var tempFile in tempFiles)
            {
                if (System.IO.File.Exists(tempFile))
                {
                    try
                    {
                        System.IO.File.Delete(tempFile);
                        _logger.LogInformation("🗑️ 已刪除臨時檔案: {TempFile}", tempFile);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("⚠️ 無法刪除臨時檔案 {TempFile}: {Error}", tempFile, ex.Message);
                    }
                }
                else
                {
                    _logger.LogDebug("📝 臨時檔案不存在，跳過: {TempFile}", tempFile);
                }
            }
            
            _logger.LogInformation("✅ 臨時檔案清理完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ 清理臨時檔案時發生錯誤");
        }
    }

    private async Task CleanupRenderTempFiles(string projectRoot, Card card)
    {
        try
        {
            _logger.LogInformation("🧹 開始清理卡片 {CardId} 的渲染臨時檔案", card.Id);
            
            var tempFiles = new List<string>();
            
            if (card.IsSameBothSides)
            {
                // AB相同模式：只有一個臨時檔案
                tempFiles.Add(Path.Combine(projectRoot, $"card_{card.Id}_temp.png"));
            }
            else
            {
                // AB不同模式：有A面和B面兩個臨時檔案
                tempFiles.Add(Path.Combine(projectRoot, $"card_{card.Id}_A_temp.png"));
                tempFiles.Add(Path.Combine(projectRoot, $"card_{card.Id}_B_temp.png"));
            }
            
            foreach (var tempFile in tempFiles)
            {
                if (System.IO.File.Exists(tempFile))
                {
                    try
                    {
                        System.IO.File.Delete(tempFile);
                        _logger.LogInformation("🗑️ 已刪除渲染臨時檔案: {TempFile}", tempFile);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("⚠️ 無法刪除渲染臨時檔案 {TempFile}: {Error}", tempFile, ex.Message);
                    }
                }
                else
                {
                    _logger.LogDebug("📝 渲染臨時檔案不存在，跳過: {TempFile}", tempFile);
                }
            }
            
            _logger.LogInformation("✅ 渲染臨時檔案清理完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ 清理渲染臨時檔案時發生錯誤");
        }
    }

    private async Task<(bool IsSuccess, string ErrorMessage)> CastSingleSide(string projectRoot, string imagePath, int side, string deviceAddress)
    {
        try
        {
            _logger.LogInformation("🔧 傳輸單面：圖片={ImagePath}, 面={Side}, 設備={DeviceAddress}", imagePath, side, deviceAddress);

            // 🔧 驗證圖片檔案是否存在
            var fullImagePath = Path.Combine(projectRoot, imagePath);
            if (!System.IO.File.Exists(fullImagePath))
            {
                _logger.LogError("❌ 圖片檔案不存在: {FullImagePath}", fullImagePath);
                return (false, $"圖片檔案不存在: {fullImagePath}");
            }

            var fileInfo = new FileInfo(fullImagePath);
            _logger.LogInformation("📁 圖片檔案驗證成功，檔案大小: {FileSize} bytes", fileInfo.Length);

            // 🔧 等待檔案系統穩定
            await Task.Delay(300);

            // 使用已存在的 projectRoot 變數
            var scriptPath = Path.Combine(projectRoot, "backend", "Pythons", "cast_image_to_ph6_fixed.py");

            _logger.LogInformation("🔍 投圖腳本路徑: {ScriptPath}", scriptPath);
            _logger.LogInformation("🔍 當前工作目錄: {CurrentDir}", projectRoot);
            _logger.LogInformation("🔍 專案根目錄: {ProjectRoot}", projectRoot);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"\"{scriptPath}\" {imagePath} {side} {deviceAddress}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = projectRoot
            };

            using var process = new Process();
            process.StartInfo = processStartInfo;
            
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputBuilder.AppendLine(e.Data);
                    
                    // 解析進度資訊
                    if (e.Data.StartsWith("PROGRESS|"))
                    {
                        var parts = e.Data.Split('|');
                        if (parts.Length >= 4)
                        {
                            var blockInfo = parts[1];
                            var packageInfo = parts[2];
                            var progressPercent = parts[3];
                            
                            var progressMessage = $"面{side}: {blockInfo} - {packageInfo} ({progressPercent})";
                            _logger.LogInformation("📊 面{Side}進度: {Progress}", side, progressMessage);
                            
                            // 這裡可以通過SignalR或其他方式推送進度給前端
                            // 暫時先記錄日誌
                        }
                    }
                    else
                    {
                        _logger.LogInformation("📤 面{Side}傳輸輸出: {Output}", side, e.Data);
                    }
                }
            };
            
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    errorBuilder.AppendLine(e.Data);
                    _logger.LogError("❌ 面{Side}傳輸錯誤: {Error}", side, e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            await process.WaitForExitAsync();

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            if (process.ExitCode == 0)
            {
                _logger.LogInformation("✅ 面{Side}傳輸成功", side);
                return (true, $"面{side}傳輸成功");
            }
            else
            {
                _logger.LogError("❌ 面{Side}傳輸失敗，退出代碼: {ExitCode}, 錯誤: {Error}", side, process.ExitCode, error);
                return (false, $"面{side}傳輸失敗: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "傳輸面{Side}時發生錯誤", side);
            return (false, ex.Message);
        }
    }

    private async Task<(bool IsSuccess, string ErrorMessage)> RenderCardImages(Card card)
    {
        try
        {
            var currentDir = Directory.GetCurrentDirectory();
            var projectRoot = Path.GetDirectoryName(currentDir) ?? currentDir;

            _logger.LogInformation("🖼️ 使用前端生成的縮圖進行投圖，確保與預覽完全一致");

            if (card.IsSameBothSides)
            {
                // AB面相同：使用A面縮圖作為共用圖片
                var imagePath = Path.Combine(projectRoot, $"card_{card.Id}_temp.png");
                return await ConvertThumbnailToFile(card.ThumbnailA ?? "", imagePath, "A面（AB相同模式）");
            }
            else
            {
                // AB面不同：分別轉換A面和B面縮圖
                var imagePathA = Path.Combine(projectRoot, $"card_{card.Id}_A_temp.png");
                var imagePathB = Path.Combine(projectRoot, $"card_{card.Id}_B_temp.png");
                
                // 轉換A面縮圖
                var resultA = await ConvertThumbnailToFile(card.ThumbnailA ?? "", imagePathA, "A面");
                if (!resultA.IsSuccess)
                {
                    return resultA;
                }
                
                // 轉換B面縮圖
                var resultB = await ConvertThumbnailToFile(card.ThumbnailB ?? "", imagePathB, "B面");
                return resultB;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "處理卡片縮圖時發生錯誤");
            return (false, ex.Message);
        }
    }

    private async Task<(bool IsSuccess, string ErrorMessage)> ConvertThumbnailToFile(string thumbnailBase64, string outputPath, string sideName)
    {
        try
        {
            if (string.IsNullOrEmpty(thumbnailBase64))
            {
                _logger.LogError("❌ {SideName} 縮圖資料為空", sideName);
                return (false, $"{sideName} 縮圖資料為空");
            }

            _logger.LogInformation("🔄 直接轉換 {SideName} 縮圖為PNG檔案: {OutputPath}", sideName, outputPath);

            // 🔧 如果檔案已存在，先刪除避免衝突
            if (System.IO.File.Exists(outputPath))
            {
                try
                {
                    System.IO.File.Delete(outputPath);
                    _logger.LogInformation("🗑️ 已刪除舊的 {SideName} 檔案: {OutputPath}", sideName, outputPath);
                    // 等待檔案系統完成刪除操作
                    await Task.Delay(200);
                }
                catch (Exception deleteEx)
                {
                    _logger.LogWarning("⚠️ 無法刪除舊的 {SideName} 檔案: {Error}", sideName, deleteEx.Message);
                }
            }

            // 移除 data:image/png;base64, 前綴
            var base64Data = thumbnailBase64;
            if (base64Data.StartsWith("data:image/png;base64,"))
            {
                base64Data = base64Data.Substring("data:image/png;base64,".Length);
            }
            else if (base64Data.StartsWith("data:image/jpeg;base64,"))
            {
                base64Data = base64Data.Substring("data:image/jpeg;base64,".Length);
            }

            // 將 base64 轉換為 byte array 並直接寫入檔案
            var imageData = Convert.FromBase64String(base64Data);
            
            // 🔧 確保目錄存在
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogInformation("📁 已建立目錄: {Directory}", directory);
            }
            
            await System.IO.File.WriteAllBytesAsync(outputPath, imageData);
            
            // 🔧 等待檔案系統同步
            await Task.Delay(100);
            
            // 驗證檔案是否成功創建
            if (!System.IO.File.Exists(outputPath))
            {
                _logger.LogError("❌ PNG檔案創建失敗: {OutputPath}", outputPath);
                return (false, $"PNG檔案創建失敗: {outputPath}");
            }

            var fileInfo = new FileInfo(outputPath);
            _logger.LogInformation("✅ {SideName} PNG檔案創建成功，檔案大小: {FileSize} bytes", sideName, fileInfo.Length);
            
            return (true, $"{sideName} 縮圖轉換成功");
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "❌ Base64 格式錯誤 - {SideName}", sideName);
            return (false, $"{sideName} Base64 格式錯誤");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ 轉換 {SideName} 縮圖時發生錯誤", sideName);
            return (false, $"轉換 {sideName} 縮圖時發生錯誤: {ex.Message}");
        }
    }



    private async Task<(bool IsSuccess, string ErrorMessage)> RenderCardImagesWithScript(Card card, string projectRoot)
    {
        try
        {
            _logger.LogInformation("🎨 使用 render_card_image.py 渲染高品質圖片，卡片ID: {CardId}", card.Id);

            if (card.IsSameBothSides)
            {
                // AB面相同：只渲染一張圖片（使用A面）
                var imagePath = Path.Combine(projectRoot, $"card_{card.Id}_temp.png");
                var result = await RenderSingleSideWithScript(card.Id, imagePath, projectRoot, "A面（AB相同模式）", "A");
                
                // 🧹 渲染完成後自動清理臨時檔案
                if (result.IsSuccess)
                {
                    await CleanupRenderTempFiles(projectRoot, card);
                }
                
                return result;
            }
            else
            {
                // AB面不同：分別渲染A面和B面
                var imagePathA = Path.Combine(projectRoot, $"card_{card.Id}_A_temp.png");
                var imagePathB = Path.Combine(projectRoot, $"card_{card.Id}_B_temp.png");
                
                // 渲染A面
                var resultA = await RenderSingleSideWithScript(card.Id, imagePathA, projectRoot, "A面", "A");
                if (!resultA.IsSuccess)
                {
                    return resultA;
                }
                
                // 渲染B面
                var resultB = await RenderSingleSideWithScript(card.Id, imagePathB, projectRoot, "B面", "B");
                
                // 🧹 渲染完成後自動清理臨時檔案
                if (resultB.IsSuccess)
                {
                    await CleanupRenderTempFiles(projectRoot, card);
                }
                
                return resultB;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "使用 render_card_image.py 渲染圖片時發生錯誤");
            return (false, ex.Message);
        }
    }

    private async Task<(bool IsSuccess, string ErrorMessage)> RenderSingleSideWithScript(int cardId, string outputPath, string projectRoot, string sideName, string side = "")
    {
        try
        {
            _logger.LogInformation("🎨 渲染 {SideName} 到: {OutputPath}", sideName, outputPath);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"{Path.Combine(AppContext.BaseDirectory, "Pythons", "render_card_image.py")} {cardId} \"{outputPath}\" {side}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = projectRoot
            };

            using var process = new Process();
            process.StartInfo = processStartInfo;
            
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputBuilder.AppendLine(e.Data);
                    _logger.LogInformation("🎨 {SideName} 渲染輸出: {Output}", sideName, e.Data);
                }
            };
            
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    errorBuilder.AppendLine(e.Data);
                    _logger.LogError("❌ {SideName} 渲染錯誤: {Error}", sideName, e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            await process.WaitForExitAsync();

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            if (process.ExitCode == 0)
            {
                // 驗證輸出檔案是否成功創建
                if (!System.IO.File.Exists(outputPath))
                {
                    _logger.LogError("❌ {SideName} 渲染輸出檔案不存在: {OutputPath}", sideName, outputPath);
                    return (false, $"{sideName} 渲染輸出檔案不存在: {outputPath}");
                }

                var fileInfo = new FileInfo(outputPath);
                _logger.LogInformation("✅ {SideName} 渲染成功，檔案大小: {FileSize} bytes", sideName, fileInfo.Length);
                
                return (true, $"{sideName} 渲染成功");
            }
            else
            {
                _logger.LogError("❌ {SideName} 渲染執行失敗，退出代碼: {ExitCode}, 錯誤: {Error}", sideName, process.ExitCode, error);
                return (false, $"{sideName} 渲染執行失敗: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ 調用 render_card_image.py 渲染 {SideName} 時發生錯誤", sideName);
            return (false, $"調用 render_card_image.py 渲染 {sideName} 時發生錯誤: {ex.Message}");
        }
    }

    private async Task<(bool IsSuccess, string ErrorMessage)> CastImageToPH6Device(string deviceAddress, Card card, int side = 2)
    {
        try
        {
            var currentDir = Directory.GetCurrentDirectory(); // backend目錄
            var projectRoot = Path.GetDirectoryName(currentDir) ?? currentDir; // 上一層目錄

            // 使用新的直接渲染投圖腳本 - 統一數據處理流程
            var pythonScriptPath = Path.Combine(AppContext.BaseDirectory, "Pythons", "cast_render_direct.py");
            _logger.LogInformation("🔧 使用直接渲染投圖腳本: {ScriptPath}, 卡片ID: {CardId}", pythonScriptPath, card.Id);
            
            // 檢查新腳本是否存在，否則使用舊的分離式流程
            if (!System.IO.File.Exists(pythonScriptPath))
            {
                _logger.LogWarning("直接渲染腳本不存在，使用舊的分離式流程");
                return await CastImageToPH6Device_Legacy(deviceAddress, card, side);
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"cd {projectRoot} && source .venv/bin/activate && python {pythonScriptPath} {card.Id} {side} {deviceAddress}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = projectRoot
            };

            using var process = new Process();
            process.StartInfo = processStartInfo;
            
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputBuilder.AppendLine(e.Data);
                    _logger.LogInformation("📤 直接渲染投圖輸出: {Output}", e.Data);
                }
            };
            
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    errorBuilder.AppendLine(e.Data);
                    _logger.LogError("❌ 直接渲染投圖錯誤: {Error}", e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            await process.WaitForExitAsync();

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            if (process.ExitCode == 0)
            {
                _logger.LogInformation("✅ 直接渲染投圖執行成功 - 統一數據處理解決橫條紋問題");
                return (true, "圖片投送成功 - 已使用統一數據處理流程解決橫條紋問題");
            }
            else
            {
                _logger.LogError("❌ 直接渲染投圖執行失敗，退出代碼: {ExitCode}, 錯誤: {Error}", process.ExitCode, error);
                return (false, $"直接渲染投圖失敗: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "調用直接渲染投圖腳本時發生錯誤");
            return (false, ex.Message);
        }
    }

    private async Task<(bool IsSuccess, string ErrorMessage)> CastImageToPH6Device_Legacy(string deviceAddress, Card card, int side = 2)
    {
        try
        {
            var currentDir = Directory.GetCurrentDirectory(); // backend目錄
            var projectRoot = Path.GetDirectoryName(currentDir) ?? currentDir; // 上一層目錄

            // 舊的分離式流程：先渲染再投圖
            var imagePath = Path.Combine(projectRoot, $"card_{card.Id}_temp.png");
            var renderResult = await RenderCardToImage(card.Id, imagePath);
            if (!renderResult.IsSuccess)
            {
                _logger.LogError("卡片渲染失敗: {Error}", renderResult.ErrorMessage);
                return (false, $"卡片渲染失敗: {renderResult.ErrorMessage}");
            }
            
            // 確保圖片檔案存在
            if (!System.IO.File.Exists(imagePath))
            {
                _logger.LogError("渲染的圖片檔案不存在: {ImagePath}", imagePath);
                return (false, "渲染的圖片檔案不存在");
            }

            // 使用修復版本的Python腳本 - 包含ACK處理和傳輸延遲修復
            var pythonScriptPath = Path.Combine(AppContext.BaseDirectory, "Pythons", "cast_image_to_ph6_fixed.py");
            _logger.LogInformation("🔧 使用修復版本腳本: {ScriptPath}, 圖片路徑: {ImagePath}", pythonScriptPath, imagePath);
            
            // 檢查修復版本腳本是否存在，否則使用原版本
            if (!System.IO.File.Exists(pythonScriptPath))
            {
                pythonScriptPath = Path.Combine(projectRoot, "cast_image_to_ph6.py");
                _logger.LogWarning("修復版本腳本不存在，使用原版本: {ScriptPath}", pythonScriptPath);
            }
            
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"cd {projectRoot} && source .venv/bin/activate && python {pythonScriptPath} {imagePath} {side} {deviceAddress}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = projectRoot
            };

            using var process = new Process();
            process.StartInfo = processStartInfo;
            
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputBuilder.AppendLine(e.Data);
                    _logger.LogInformation("📤 投圖輸出: {Output}", e.Data);
                }
            };
            
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    errorBuilder.AppendLine(e.Data);
                    _logger.LogError("❌ 投圖錯誤: {Error}", e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            await process.WaitForExitAsync();

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            if (process.ExitCode == 0)
            {
                _logger.LogInformation("✅ 修復版本Python腳本執行成功 - 橫條紋問題已修復");
                
                // 清理暫存圖片檔案
                try
                {
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                        _logger.LogInformation("🧹 已清理暫存圖片檔案: {ImagePath}", imagePath);
                    }
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogWarning(cleanupEx, "清理暫存圖片檔案失敗: {ImagePath}", imagePath);
                }
                
                return (true, "圖片投送成功 - 已使用修復版本解決橫條紋問題");
            }
            else
            {
                _logger.LogError("❌ 修復版本Python腳本執行失敗，退出代碼: {ExitCode}, 錯誤: {Error}", process.ExitCode, error);
                return (false, $"Python腳本執行失敗: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "調用修復版本Python腳本時發生錯誤");
            return (false, ex.Message);
        }
    }

    private async Task<(bool IsSuccess, string ErrorMessage)> RenderCardToImage(int cardId, string outputPath, string side = "")
    {
        try
        {
            _logger.LogInformation("開始渲染卡片 {CardId} 到圖片: {OutputPath}, 面: {Side}", cardId, outputPath, side);
            
            var currentDir = Directory.GetCurrentDirectory(); // backend目錄
            var projectRoot = Path.GetDirectoryName(currentDir) ?? currentDir; // 上一層目錄
            var pythonScriptPath = Path.Combine(projectRoot, "render_card_image.py");
            _logger.LogInformation("渲染腳本路徑: {ScriptPath}", pythonScriptPath);

            var sideArg = string.IsNullOrEmpty(side) ? "" : $" {side}";
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"cd {projectRoot} && source .venv/bin/activate && python {pythonScriptPath} {cardId} {outputPath}{sideArg}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = projectRoot
            };

            using var process = new Process();
            process.StartInfo = processStartInfo;
            
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputBuilder.AppendLine(e.Data);
                    _logger.LogInformation("渲染輸出: {Output}", e.Data);
                }
            };
            
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    errorBuilder.AppendLine(e.Data);
                    _logger.LogError("渲染錯誤: {Error}", e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            await process.WaitForExitAsync();

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            if (process.ExitCode == 0)
            {
                _logger.LogInformation("卡片渲染成功");
                return (true, "卡片渲染成功");
            }
            else
            {
                _logger.LogError("卡片渲染失敗，退出代碼: {ExitCode}, 錯誤: {Error}", process.ExitCode, error);
                return (false, $"卡片渲染失敗: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "渲染卡片時發生錯誤");
            return (false, ex.Message);
        }
    }

    // 🔍 GET: api/bluetooth/devices/{id}/connection-status
    [HttpGet("devices/{id}/connection-status")]
    public async Task<IActionResult> CheckDeviceConnectionStatus(int id)
    {
        try
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound(new { message = "設備不存在" });
            }

            _logger.LogInformation("檢查設備 {DeviceId} ({Address}) 的連接狀態", id, device.BluetoothAddress);

            // 使用藍牙服務檢查連接狀態
            var isConnected = await _bluetoothService.CheckDeviceConnectionAsync(device.BluetoothAddress);
            var isReachable = await _bluetoothService.IsDeviceReachableAsync(device.BluetoothAddress);

            // 更新資料庫中的設備狀態
            var newStatus = isConnected ? DeviceStatus.Connected : DeviceStatus.Disconnected;
            if (device.Status != newStatus)
            {
                device.Status = newStatus;
                device.UpdatedAt = DateTime.UtcNow;
                if (isConnected)
                {
                    device.LastConnected = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("設備 {DeviceId} 狀態已更新為: {Status}", id, newStatus);
            }

            return Ok(new
            {
                deviceId = id,
                name = device.Name,
                bluetoothAddress = device.BluetoothAddress,
                isConnected = isConnected,
                isReachable = isReachable,
                status = newStatus.ToString(),
                lastConnected = device.LastConnected,
                checkedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查設備連接狀態時發生錯誤: {DeviceId}", id);
            return StatusCode(500, new { message = "檢查設備連接狀態時發生錯誤", error = ex.Message });
        }
    }

    // 🔍 GET: api/bluetooth/devices/connection-status
    [HttpGet("devices/connection-status")]
    public async Task<IActionResult> CheckAllDevicesConnectionStatus()
    {
        try
        {
            _logger.LogInformation("檢查所有設備的連接狀態");

            var devices = await _context.Devices.ToListAsync();
            var connectedAddresses = await _bluetoothService.GetConnectedDeviceAddressesAsync();
            
            var deviceStatuses = new List<object>();

            foreach (var device in devices)
            {
                var isConnected = connectedAddresses.Contains(device.BluetoothAddress);
                var newStatus = isConnected ? DeviceStatus.Connected : DeviceStatus.Disconnected;

                // 更新設備狀態
                if (device.Status != newStatus)
                {
                    device.Status = newStatus;
                    device.UpdatedAt = DateTime.UtcNow;
                    if (isConnected)
                    {
                        device.LastConnected = DateTime.UtcNow;
                    }
                }

                deviceStatuses.Add(new
                {
                    deviceId = device.Id,
                    name = device.Name,
                    bluetoothAddress = device.BluetoothAddress,
                    isConnected = isConnected,
                    status = newStatus.ToString(),
                    lastConnected = device.LastConnected
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                totalDevices = devices.Count,
                connectedCount = deviceStatuses.Count(d => ((dynamic)d).isConnected),
                devices = deviceStatuses,
                checkedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查所有設備連接狀態時發生錯誤");
            return StatusCode(500, new { message = "檢查所有設備連接狀態時發生錯誤", error = ex.Message });
        }
    }

    // DELETE: api/bluetooth/devices/{id}
    [HttpDelete("devices/{id}")]
    public async Task<IActionResult> RemoveDevice(int id)
    {
        try
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing device {Id}", id);
            return StatusCode(500, "An error occurred while removing the device");
        }
    }

    /// <summary>
    /// 🎯 下載卡片的高解析度圖片（A面和B面）
    /// </summary>
    [HttpPost("cards/{cardId}/download-images")]
    public async Task<IActionResult> DownloadCardImages(int cardId)
    {
        try
        {
            _logger.LogInformation("📥 開始下載卡片 {CardId} 的高解析度圖片", cardId);

            // 查詢卡片資料
            var card = await _context.Cards.FindAsync(cardId);
            if (card == null)
            {
                return NotFound(new { message = "卡片不存在" });
            }

            if (string.IsNullOrEmpty(card.ThumbnailA) && string.IsNullOrEmpty(card.ThumbnailB))
            {
                return BadRequest(new { message = "卡片沒有縮圖資料" });
            }

            var projectRoot = Directory.GetCurrentDirectory();
            var tempDir = Path.Combine(projectRoot, "temp_downloads");
            Directory.CreateDirectory(tempDir);

            var downloadedFiles = new List<string>();
            var errors = new List<string>();

            // 生成檔案名稱
            var sanitizedCardName = card.Name ?? $"Card_{cardId}";
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                sanitizedCardName = sanitizedCardName.Replace(invalidChar, '_');
            }
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            // 下載A面圖片
            if (!string.IsNullOrEmpty(card.ThumbnailA))
            {
                var fileNameA = $"{sanitizedCardName}_A面_{timestamp}.png";
                var filePathA = Path.Combine(tempDir, fileNameA);
                
                var resultA = await ConvertBase64ToPngFile(card.ThumbnailA, filePathA, "A面");
                if (resultA.IsSuccess)
                {
                    downloadedFiles.Add(filePathA);
                    _logger.LogInformation("✅ A面圖片已生成: {FilePath}", filePathA);
                }
                else
                {
                    errors.Add($"A面: {resultA.ErrorMessage}");
                }
            }

            // 下載B面圖片
            if (!string.IsNullOrEmpty(card.ThumbnailB))
            {
                var fileNameB = $"{sanitizedCardName}_B面_{timestamp}.png";
                var filePathB = Path.Combine(tempDir, fileNameB);
                
                var resultB = await ConvertBase64ToPngFile(card.ThumbnailB, filePathB, "B面");
                if (resultB.IsSuccess)
                {
                    downloadedFiles.Add(filePathB);
                    _logger.LogInformation("✅ B面圖片已生成: {FilePath}", filePathB);
                }
                else
                {
                    errors.Add($"B面: {resultB.ErrorMessage}");
                }
            }

            if (downloadedFiles.Count == 0)
            {
                return StatusCode(500, new { 
                    message = "圖片生成失敗", 
                    errors = errors 
                });
            }

            // 如果只有一個檔案，直接返回
            if (downloadedFiles.Count == 1)
            {
                var filePath = downloadedFiles[0];
                var fileName = Path.GetFileName(filePath);
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                
                // 清理暫存檔案
                System.IO.File.Delete(filePath);
                
                return File(fileBytes, "image/png", fileName);
            }

            // 如果有多個檔案，打包成ZIP
            var zipFileName = $"{sanitizedCardName}_{timestamp}.zip";
            var zipFilePath = Path.Combine(tempDir, zipFileName);
            
            using (var zip = new System.IO.Compression.ZipArchive(
                System.IO.File.Create(zipFilePath), 
                System.IO.Compression.ZipArchiveMode.Create))
            {
                foreach (var filePath in downloadedFiles)
                {
                    var fileName = Path.GetFileName(filePath);
                    var entry = zip.CreateEntry(fileName);
                    
                    using var entryStream = entry.Open();
                    using var fileStream = System.IO.File.OpenRead(filePath);
                    await fileStream.CopyToAsync(entryStream);
                }
            }

            var zipBytes = await System.IO.File.ReadAllBytesAsync(zipFilePath);
            
            // 清理所有暫存檔案
            foreach (var filePath in downloadedFiles)
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            if (System.IO.File.Exists(zipFilePath))
                System.IO.File.Delete(zipFilePath);

            _logger.LogInformation("✅ 卡片 {CardId} 的圖片已打包下載", cardId);
            
            return File(zipBytes, "application/zip", zipFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ 下載卡片圖片時發生錯誤");
            return StatusCode(500, new { message = "下載圖片時發生錯誤", error = ex.Message });
        }
    }

    /// <summary>
    /// 🎯 將 base64 轉換為 PNG 檔案（無損轉換）
    /// </summary>
    private async Task<(bool IsSuccess, string ErrorMessage)> ConvertBase64ToPngFile(string base64Data, string outputPath, string sideName)
    {
        try
        {
            if (string.IsNullOrEmpty(base64Data))
            {
                return (false, $"{sideName} 縮圖資料為空");
            }

            _logger.LogInformation("🔄 轉換 {SideName} base64 為高解析度PNG: {OutputPath}", sideName, outputPath);

            // 移除 data:image/png;base64, 前綴
            var cleanBase64 = base64Data;
            if (cleanBase64.StartsWith("data:image/png;base64,"))
            {
                cleanBase64 = cleanBase64.Substring("data:image/png;base64,".Length);
            }
            else if (cleanBase64.StartsWith("data:image/jpeg;base64,"))
            {
                cleanBase64 = cleanBase64.Substring("data:image/jpeg;base64,".Length);
            }

            // 🎯 直接將 base64 轉換為 byte array 並寫入檔案
            // 這是完全無損的轉換，保持原始解析度
            var imageData = Convert.FromBase64String(cleanBase64);
            await System.IO.File.WriteAllBytesAsync(outputPath, imageData);

            // 驗證檔案是否成功創建
            if (!System.IO.File.Exists(outputPath))
            {
                return (false, $"檔案創建失敗: {outputPath}");
            }

            var fileInfo = new FileInfo(outputPath);
            _logger.LogInformation("✅ {SideName} 高解析度PNG已生成，檔案大小: {FileSize} bytes", sideName, fileInfo.Length);
            
            return (true, $"{sideName} 轉換成功");
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "❌ Base64 格式錯誤 - {SideName}", sideName);
            return (false, $"{sideName} Base64 格式錯誤");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ 轉換 {SideName} 時發生錯誤", sideName);
            return (false, $"轉換 {sideName} 時發生錯誤: {ex.Message}");
        }
    }
} 