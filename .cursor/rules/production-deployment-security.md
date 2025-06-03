# 🛡️ SmartNameplate 生產部署安全規則

## 🎯 **核心原則**

- 🤖 **回覆時給予機器人符號**
- 🔍 **優先查找 context7 mcp 工具搜尋問題的解決方案**
- ⚡ **發現 linter error 要馬上修正**
- 🏗️ **使用 Angular/SASS/TS 架構，利用 BEM 寫法**
- 🚨 **遇到編譯錯誤必須優先解決**
- 🗄️ **以實際資料庫資料實作**
- 🔄 **修正 API 或後端相關文件後，要停止後端服務器然後重啟**

---

## 🏭 **Windows Server IIS 部署配置**

### 📦 **IIS 配置需求**

```powershell
# 啟用必要的 IIS 功能
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Security
Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestFiltering
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Performance
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerManagementTools
Enable-WindowsOptionalFeature -Online -FeatureName IIS-IIS6ManagementCompatibility
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Metabase
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ASPNET45
```

### 🔧 **web.config 安全配置**

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <!-- 安全標頭配置 -->
    <httpProtocol>
      <customHeaders>
        <add name="X-Frame-Options" value="DENY" />
        <add name="X-Content-Type-Options" value="nosniff" />
        <add name="X-XSS-Protection" value="1; mode=block" />
        <add name="Strict-Transport-Security" value="max-age=31536000; includeSubDomains" />
        <add name="Content-Security-Policy" value="default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline';" />
        <add name="Referrer-Policy" value="strict-origin-when-cross-origin" />
        <add name="Permissions-Policy" value="geolocation=(), microphone=(), camera=()" />
      </customHeaders>
    </httpProtocol>

    <!-- 移除不必要的 HTTP 標頭 -->
    <httpProtocol>
      <customHeaders>
        <remove name="Server" />
        <remove name="X-Powered-By" />
        <remove name="X-AspNet-Version" />
        <remove name="X-AspNetMvc-Version" />
      </customHeaders>
    </httpProtocol>

    <!-- 請求過濾 -->
    <security>
      <requestFiltering>
        <requestLimits maxAllowedContentLength="104857600" maxQueryString="2048" maxUrl="4096" />
        <hiddenSegments>
          <add segment="bin" />
          <add segment="App_code" />
          <add segment="App_Data" />
          <add segment="logs" />
        </hiddenSegments>
        <fileExtensions>
          <add fileExtension=".config" allowed="false" />
          <add fileExtension=".log" allowed="false" />
          <add fileExtension=".dll" allowed="false" />
          <add fileExtension=".cs" allowed="false" />
          <add fileExtension=".vb" allowed="false" />
        </fileExtensions>
      </requestFiltering>
    </security>

    <!-- URL 重寫規則 -->
    <rewrite>
      <rules>
        <!-- 強制 HTTPS -->
        <rule name="Redirect to HTTPS" stopProcessing="true">
          <match url=".*" />
          <conditions>
            <add input="{HTTPS}" pattern="off" ignoreCase="true" />
          </conditions>
          <action type="Redirect" url="https://{HTTP_HOST}/{R:0}" redirectType="Permanent" />
        </rule>
        
        <!-- Angular 路由支援 -->
        <rule name="Angular Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
            <add input="{REQUEST_URI}" pattern="^/api/" negate="true" />
          </conditions>
          <action type="Rewrite" url="/" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

---

## 🛡️ **OWASP Top 10 2021 安全規範**

### **A01: Broken Access Control**
```csharp
// 實施最小權限原則
[Authorize(Roles = "Admin")]
[RequirePermission("ManageUsers")]
public async Task<IActionResult> DeleteUser(int userId)
{
    // 驗證當前用戶是否有權限操作此用戶
    if (!await _authService.CanAccessUser(CurrentUserId, userId))
    {
        return Forbid();
    }
    
    // 實際操作
    return Ok();
}

// API 端點權限驗證
[HttpGet("{id}")]
[ValidateResourceAccess]
public async Task<IActionResult> GetCard(int id)
{
    var card = await _cardService.GetByIdAsync(id, CurrentUserId);
    return Ok(card);
}
```

### **A02: Cryptographic Failures**
```csharp
// 強密碼散列
public class PasswordHashService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }
    
    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}

// 敏感資料加密
public class DataEncryptionService
{
    public string EncryptSensitiveData(string data)
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.GenerateKey();
            // 實施 AES-256 加密
            return Convert.ToBase64String(aes.Key);
        }
    }
}
```

### **A03: Injection**
```csharp
// 使用參數化查詢
public async Task<Card> GetCardByNameAsync(string name)
{
    var query = "SELECT * FROM Cards WHERE Name = @name";
    return await _context.Cards
        .FromSqlRaw(query, new SqlParameter("@name", name))
        .FirstOrDefaultAsync();
}

// 輸入驗證和清理
[HttpPost]
public async Task<IActionResult> CreateCard([FromBody] CreateCardDto dto)
{
    // 驗證輸入
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    
    // 清理 HTML 內容
    dto.Description = _htmlSanitizer.Sanitize(dto.Description);
    
    var card = await _cardService.CreateAsync(dto);
    return CreatedAtAction(nameof(GetCard), new { id = card.Id }, card);
}
```

### **A04: Insecure Design**
```csharp
// 威脅建模和安全設計模式
public class SecureCardService
{
    // 實施速率限制
    [RateLimit(requests: 10, timeWindow: 60)]
    public async Task<Card> CreateCardAsync(CreateCardDto dto)
    {
        // 業務邏輯驗證
        if (!await ValidateBusinessRules(dto))
        {
            throw new BusinessRuleViolationException();
        }
        
        return await _repository.CreateAsync(dto);
    }
    
    // 實施審計日誌
    [AuditLog]
    public async Task<bool> DeleteCardAsync(int cardId)
    {
        _logger.LogWarning("Card deletion attempt: {CardId} by {UserId}", 
            cardId, _currentUser.Id);
        
        return await _repository.DeleteAsync(cardId);
    }
}
```

### **A05: Security Misconfiguration**
```json
// appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "System": "Warning",
      "SmartNameplate": "Information"
    }
  },
  "AllowedHosts": "smartnameplate.com;*.smartnameplate.com",
  "Database": {
    "UseSqlServer": true,
    "ConnectionTimeout": 30
  },
  "Security": {
    "RequireHttps": true,
    "AllowedOrigins": ["https://smartnameplate.com"],
    "JwtExpireMinutes": 15,
    "MaxLoginAttempts": 5,
    "LockoutMinutes": 30
  }
}
```

### **A06: Vulnerable and Outdated Components**
```xml
<!-- 定期更新套件版本 -->
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>

<ItemGroup>
  <!-- 使用最新穩定版本 -->
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
  <PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
  <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
</ItemGroup>
```

### **A07: Identification and Authentication Failures**
```csharp
// JWT 安全配置
public void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true
            };
        });
}

// 多因素認證
[HttpPost("verify-2fa")]
public async Task<IActionResult> Verify2FA([FromBody] Verify2FADto dto)
{
    var isValid = await _authService.Verify2FACode(dto.UserId, dto.Code);
    if (!isValid)
    {
        await _authService.RecordFailedAttempt(dto.UserId);
        return Unauthorized("Invalid 2FA code");
    }
    
    return Ok(await _authService.GenerateJwtToken(dto.UserId));
}
```

### **A08: Software and Data Integrity Failures**
```csharp
// 檔案上傳驗證
[HttpPost("upload")]
public async Task<IActionResult> UploadFile(IFormFile file)
{
    // 檔案類型驗證
    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
    if (!allowedTypes.Contains(file.ContentType))
    {
        return BadRequest("Invalid file type");
    }
    
    // 檔案大小限制
    if (file.Length > 5 * 1024 * 1024) // 5MB
    {
        return BadRequest("File too large");
    }
    
    // 病毒掃描
    var scanResult = await _virusScanService.ScanAsync(file);
    if (!scanResult.IsClean)
    {
        return BadRequest("File failed security scan");
    }
    
    // 計算檔案雜湊值以確保完整性
    var hash = await _hashService.ComputeFileHashAsync(file);
    
    return Ok(new { Hash = hash });
}
```

### **A09: Security Logging and Monitoring Failures**
```csharp
// 全面的安全日誌記錄
public class SecurityLogger
{
    private readonly ILogger<SecurityLogger> _logger;
    
    public void LogSecurityEvent(SecurityEventType eventType, string message, object details = null)
    {
        _logger.LogWarning("SECURITY_EVENT: {EventType} - {Message} - {Details} - {Timestamp}", 
            eventType, message, JsonSerializer.Serialize(details), DateTime.UtcNow);
    }
    
    public void LogLoginAttempt(string username, bool success, string ipAddress)
    {
        _logger.LogInformation("LOGIN_ATTEMPT: User={Username}, Success={Success}, IP={IP}, Time={Time}",
            username, success, ipAddress, DateTime.UtcNow);
    }
    
    public void LogPrivilegeEscalation(string userId, string attemptedAction)
    {
        _logger.LogCritical("PRIVILEGE_ESCALATION: User={UserId}, Action={Action}, Time={Time}",
            userId, attemptedAction, DateTime.UtcNow);
    }
}
```

### **A10: Server-Side Request Forgery (SSRF)**
```csharp
// SSRF 防護
public class HttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly string[] _allowedHosts = { "api.smartnameplate.com", "cdn.smartnameplate.com" };
    
    public async Task<string> GetExternalResourceAsync(string url)
    {
        // 驗證 URL
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("Invalid URL");
        }
        
        // 檢查是否為允許的主機
        if (!_allowedHosts.Contains(uri.Host))
        {
            throw new UnauthorizedAccessException("Host not allowed");
        }
        
        // 防止內部網路存取
        if (IsInternalIP(uri.Host))
        {
            throw new UnauthorizedAccessException("Internal network access denied");
        }
        
        return await _httpClient.GetStringAsync(uri);
    }
}
```

---

## 📊 **日誌管理系統**

### 🗂️ **日誌資料夾結構**
```
C:\SmartNameplate\Logs\
├── Application\
│   ├── app-{yyyy-MM-dd}.log
│   └── error-{yyyy-MM-dd}.log
├── Security\
│   ├── auth-{yyyy-MM-dd}.log
│   ├── access-{yyyy-MM-dd}.log
│   └── security-events-{yyyy-MM-dd}.log
├── Performance\
│   ├── performance-{yyyy-MM-dd}.log
│   └── api-metrics-{yyyy-MM-dd}.log
├── Database\
│   ├── queries-{yyyy-MM-dd}.log
│   └── migrations-{yyyy-MM-dd}.log
└── IIS\
    ├── access.log
    ├── error.log
    └── trace.log
```

### 🔧 **Serilog 配置**
```csharp
public static void ConfigureLogging(IHostBuilder hostBuilder)
{
    hostBuilder.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .WriteTo.Console()
            .WriteTo.File(
                path: @"C:\SmartNameplate\Logs\Application\app-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: @"C:\SmartNameplate\Logs\Security\security-events-.log",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Warning,
                retainedFileCountLimit: 90)
            .WriteTo.EventLog(
                source: "SmartNameplate",
                logName: "Application",
                restrictedToMinimumLevel: LogEventLevel.Error);
    });
}
```

### 📈 **效能監控**
```csharp
// 自訂效能計數器
public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            var responseTime = stopwatch.ElapsedMilliseconds;
            
            if (responseTime > 1000) // 超過 1 秒記錄警告
            {
                _logger.LogWarning("SLOW_REQUEST: {Method} {Path} took {ResponseTime}ms",
                    context.Request.Method,
                    context.Request.Path,
                    responseTime);
            }
            
            // 記錄到效能日誌
            _logger.LogInformation("API_METRICS: {Method} {Path} {StatusCode} {ResponseTime}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                responseTime);
        }
    }
}
```

---

## 🔒 **SSL/TLS 配置**

### 📜 **SSL 憑證配置**
```xml
<!-- IIS SSL 設定 -->
<system.webServer>
  <security>
    <access sslFlags="Ssl,SslNegotiateCert,SslRequireCert,Ssl128" />
  </security>
  <httpProtocol>
    <customHeaders>
      <add name="Strict-Transport-Security" value="max-age=31536000; includeSubDomains; preload" />
    </customHeaders>
  </httpProtocol>
</system.webServer>
```

### 🔐 **程式碼中的 HTTPS 強制**
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // 強制 HTTPS
    services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
        options.HttpsPort = 443;
    });
    
    // HSTS 配置
    services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });
}
```

---

## 🚨 **錯誤處理和安全回應**

### 🛡️ **全域錯誤處理**
```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            error = "An error occurred while processing your request.",
            traceId = Activity.Current?.Id ?? context.TraceIdentifier
        };
        
        // 不洩漏敏感資訊
        switch (exception)
        {
            case SecurityException:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                response = new { error = "Access denied." };
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = new { error = "Unauthorized access." };
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

---

## 🔍 **安全掃描和合規性檢查**

### 📋 **部署前檢查清單**
```bash
# 安全掃描腳本
function SecurityChecklist {
    Write-Host "🔍 執行安全合規性檢查..."
    
    # 1. 檢查 SSL 憑證
    Check-SSLCertificate -Domain "smartnameplate.com"
    
    # 2. 檢查安全標頭
    Test-SecurityHeaders -Url "https://smartnameplate.com"
    
    # 3. 檢查開放埠口
    Test-OpenPorts -Server "production-server"
    
    # 4. 檢查檔案權限
    Test-FilePermissions -Path "C:\SmartNameplate"
    
    # 5. 檢查日誌設定
    Test-LoggingConfiguration
    
    # 6. 檢查資料庫連接安全性
    Test-DatabaseSecurity
    
    Write-Host "✅ 安全檢查完成"
}
```

### 🛠️ **自動化部署腳本**
```powershell
# 部署腳本 Deploy-SmartNameplate.ps1
param(
    [Parameter(Mandatory=$true)]
    [string]$Environment
)

# 設定環境變數
$LogPath = "C:\SmartNameplate\Logs"
$AppPath = "C:\SmartNameplate\App"

# 建立日誌資料夾
New-Item -ItemType Directory -Force -Path "$LogPath\Application"
New-Item -ItemType Directory -Force -Path "$LogPath\Security" 
New-Item -ItemType Directory -Force -Path "$LogPath\Performance"
New-Item -ItemType Directory -Force -Path "$LogPath\Database"
New-Item -ItemType Directory -Force -Path "$LogPath\IIS"

# 設定檔案權限
icacls "$LogPath" /grant "IIS_IUSRS:(OI)(CI)M"
icacls "$AppPath" /grant "IIS_IUSRS:(OI)(CI)R"

# 重啟 IIS
iisreset

Write-Host "🚀 部署完成 - 環境: $Environment"
```

---

## 📧 **緊急通知系統**

### 🚨 **安全事件警報**
```csharp
public class SecurityAlertService
{
    public async Task SendSecurityAlert(SecurityIncident incident)
    {
        var alert = new
        {
            Severity = incident.Severity,
            EventType = incident.EventType,
            Timestamp = DateTime.UtcNow,
            Source = Environment.MachineName,
            Details = incident.Details
        };
        
        // 發送到監控系統
        await _monitoringService.SendAlert(alert);
        
        // 記錄到安全日誌
        _logger.LogCritical("SECURITY_INCIDENT: {@Alert}", alert);
        
        // 如果是高嚴重性，發送緊急通知
        if (incident.Severity >= SecuritySeverity.High)
        {
            await _notificationService.SendEmergencyNotification(alert);
        }
    }
}
```

---

## ✅ **部署後驗證**

### 🧪 **自動化測試**
```bash
# 部署後安全測試
npm run test:security
npm run test:e2e
npm run test:performance

# 檢查 OWASP ZAP 掃描
zap-cli quick-scan --self-contained https://smartnameplate.com

# 檢查 SSL 評級
ssllabs-scan smartnameplate.com
```

---

## 📚 **文檔和培訓要求**

### 📖 **必備文檔**
1. **安全部署指南**
2. **事件回應手冊**
3. **日誌分析指南**
4. **緊急聯絡清單**
5. **備份和復原程序**

### 👥 **團隊培訓**
- OWASP Top 10 安全意識培訓
- IIS 安全配置培訓
- 日誌監控和分析培訓
- 事件回應演練

---

## 🔄 **持續維護規則**

### 📅 **定期檢查**
- **每日**：檢查安全日誌和警報
- **每週**：檢查系統更新和漏洞掃描
- **每月**：安全配置審查
- **每季**：滲透測試和安全評估
- **每年**：完整安全審計

### 🛡️ **緊急回應程序**
1. **檢測** - 監控系統自動檢測異常
2. **隔離** - 立即隔離受影響的系統
3. **分析** - 分析事件範圍和影響
4. **修復** - 實施修復措施
5. **復原** - 安全復原服務
6. **檢討** - 事後檢討和改進

---

## 📊 **合規性報告**

### 📈 **定期報告**
```csharp
public class ComplianceReportService
{
    public async Task<ComplianceReport> GenerateOWASPComplianceReport()
    {
        return new ComplianceReport
        {
            A01_BrokenAccessControl = await CheckAccessControlCompliance(),
            A02_CryptographicFailures = await CheckCryptographyCompliance(),
            A03_Injection = await CheckInjectionProtection(),
            A04_InsecureDesign = await CheckSecureDesign(),
            A05_SecurityMisconfiguration = await CheckSecurityConfiguration(),
            A06_VulnerableComponents = await CheckComponentSecurity(),
            A07_AuthenticationFailures = await CheckAuthenticationSecurity(),
            A08_IntegrityFailures = await CheckDataIntegrity(),
            A09_LoggingFailures = await CheckLoggingCompliance(),
            A10_SSRF = await CheckSSRFProtection(),
            GeneratedAt = DateTime.UtcNow
        };
    }
}
```

這些規則確保 SmartNameplate 系統在 Windows Server IIS 環境中的安全部署，完全符合 OWASP Top 10 標準，並提供完整的日誌管理和監控功能。 