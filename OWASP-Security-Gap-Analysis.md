# 🛡️ SmartNameplate 專案 OWASP Top 10 2021 安全缺失分析報告

## 📋 **執行摘要**

本報告基於 OWASP Top 10 2021 安全標準，對 SmartNameplate 專案進行全面的安全評估。經分析發現，專案存在多項重大安全缺失，需要立即修復以符合生產環境安全要求。

**整體安全評級：🔴 高風險**
- **嚴重缺失：6 項**
- **中等缺失：3 項**  
- **輕微缺失：1 項**

---

## 🚨 **A01: Broken Access Control - 嚴重缺失**

### **當前狀況：❌ 不合規**

#### **發現的問題：**

1. **缺少身份驗證機制**
```csharp
// backend/Controllers/AuthController.cs - 第 62 行
[HttpGet("status")]
public ActionResult<object> GetStatus()
{
    return Ok(new { isAuthenticated = false, message = "未實現會話管理" });
}
```

2. **API 端點無授權保護**
```csharp
// backend/Controllers/UsersController.cs - 所有端點都缺少 [Authorize] 屬性
[HttpGet]
public async Task<ActionResult<List<User>>> GetAllUsers() // 無權限檢查
[HttpPost]
public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request) // 無權限檢查
```

3. **缺少資源級別存取控制**
```csharp
// backend/Controllers/CardsController.cs - 缺少用戶資源隔離
// 用戶可能存取其他用戶的卡片資料
```

#### **風險等級：🔴 嚴重**
- 任何人都可以存取所有 API 端點
- 用戶資料可能被未授權存取
- 管理功能暴露給所有用戶

#### **修復建議：**

```csharp
// 1. 實施 JWT 認證
[Authorize]
[HttpGet]
public async Task<ActionResult<List<User>>> GetAllUsers()

// 2. 角色基礎存取控制
[Authorize(Roles = "Admin")]
[HttpPost]
public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)

// 3. 資源級別權限檢查
[HttpGet("{id}")]
public async Task<ActionResult<Card>> GetCard(int id)
{
    var card = await _cardService.GetByIdAsync(id, CurrentUserId);
    if (card == null || card.UserId != CurrentUserId)
        return Forbid();
    return Ok(card);
}
```

---

## 🔐 **A02: Cryptographic Failures - 中等缺失**

### **當前狀況：⚠️ 部分合規**

#### **已實施的安全措施：**
```csharp
// backend/Services/UserService.cs - 使用 BCrypt 密碼散列 ✅
public string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password);
}
```

#### **發現的問題：**

1. **缺少敏感資料加密**
```csharp
// backend/Entities/User.cs - 敏感資料未加密
public class User
{
    public string UserName { get; set; } = null!; // 明文儲存
    public string Role { get; set; } = null!; // 明文儲存
}
```

2. **資料庫連接字串明文儲存**
```json
// backend/appsettings.Development.json
"ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=smart_nameplate;Username=postgres;Password=password"
}
```

#### **風險等級：⚠️ 中等**
- 敏感資料可能在資料庫洩漏時被讀取
- 配置檔案包含明文密碼

#### **修復建議：**

```csharp
// 1. 實施資料加密服務
public class DataEncryptionService
{
    public string EncryptSensitiveData(string data)
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = 256;
            // 實施 AES-256 加密
        }
    }
}

// 2. 使用 Azure Key Vault 或環境變數
"ConnectionStrings": {
    "DefaultConnection": "${CONNECTION_STRING}"
}
```

---

## 💉 **A03: Injection - 嚴重缺失**

### **當前狀況：❌ 不合規**

#### **發現的問題：**

1. **缺少輸入驗證**
```csharp
// backend/Controllers/AuthController.cs - 無輸入驗證
[HttpPost("login")]
public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
{
    // 直接使用用戶輸入，無驗證
    var user = await _userService.GetUserByUsernameAsync(request.Username);
}
```

2. **前端缺少 XSS 防護**
```typescript
// frontend/src/app/shared/components/login-modal/login-modal.component.ts
// 直接綁定用戶輸入到 DOM，可能導致 XSS
```

3. **缺少 HTML 清理**
```csharp
// 所有 Controller 都缺少輸入清理機制
```

#### **風險等級：🔴 嚴重**
- SQL 注入攻擊風險
- XSS 攻擊風險
- 命令注入風險

#### **修復建議：**

```csharp
// 1. 實施輸入驗證
[HttpPost("login")]
public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    // 清理輸入
    request.Username = _htmlSanitizer.Sanitize(request.Username);
}

// 2. 使用參數化查詢
public async Task<Card> GetCardByNameAsync(string name)
{
    return await _context.Cards
        .Where(c => c.Name == name) // EF Core 自動參數化
        .FirstOrDefaultAsync();
}
```

---

## 🏗️ **A04: Insecure Design - 嚴重缺失**

### **當前狀況：❌ 不合規**

#### **發現的問題：**

1. **缺少速率限制**
```csharp
// backend/Controllers/AuthController.cs - 登入端點無速率限制
[HttpPost("login")]
public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
// 可能遭受暴力破解攻擊
```

2. **缺少審計日誌**
```csharp
// 所有敏感操作都缺少審計記錄
// 無法追蹤安全事件
```

3. **缺少威脅建模**
- 無安全設計文檔
- 缺少攻擊面分析

#### **風險等級：🔴 嚴重**
- 暴力破解攻擊
- 無法檢測安全事件
- 缺少安全防護機制

#### **修復建議：**

```csharp
// 1. 實施速率限制
[RateLimit(requests: 5, timeWindow: 60)]
[HttpPost("login")]
public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)

// 2. 實施審計日誌
public class SecurityAuditService
{
    public void LogSecurityEvent(SecurityEventType eventType, string message, object details = null)
    {
        _logger.LogWarning("SECURITY_EVENT: {EventType} - {Message}", eventType, message);
    }
}
```

---

## ⚙️ **A05: Security Misconfiguration - 中等缺失**

### **當前狀況：⚠️ 部分合規**

#### **已實施的安全措施：**
```csharp
// backend/Program.cs - 已配置 CORS ✅
builder.Services.AddCors(options => { ... });
```

#### **發現的問題：**

1. **開發環境配置暴露**
```csharp
// backend/Program.cs - Swagger 在生產環境可能暴露
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // 需要確保生產環境關閉
}
```

2. **缺少安全標頭**
```csharp
// Program.cs 缺少安全標頭配置
// 無 HSTS、CSP、X-Frame-Options 等
```

3. **錯誤資訊洩漏**
```csharp
// backend/Controllers/AuthController.cs - 第 50 行
return StatusCode(500, new { message = "登入過程中發生錯誤", error = ex.Message });
// 洩漏詳細錯誤資訊
```

#### **風險等級：⚠️ 中等**
- API 文檔可能在生產環境暴露
- 缺少瀏覽器安全防護
- 錯誤資訊可能洩漏系統資訊

#### **修復建議：**

```csharp
// 1. 環境特定配置
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
}

// 2. 安全標頭
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    await next();
});

// 3. 通用錯誤回應
catch (Exception ex)
{
    _logger.LogError(ex, "Login error");
    return StatusCode(500, new { message = "登入失敗，請稍後再試" });
}
```

---

## 📦 **A06: Vulnerable and Outdated Components - 輕微缺失**

### **當前狀況：✅ 基本合規**

#### **已實施的安全措施：**
```xml
<!-- backend/SmartNameplate.Api.csproj - 使用最新版本 ✅ -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

#### **發現的問題：**

1. **部分套件版本較舊**
```xml
<PackageReference Include="Microsoft.AspNetCore.Cors" Version="2.2.0" />
<!-- 建議升級到最新版本 -->
```

#### **風險等級：🟡 輕微**
- 部分套件可能存在已知漏洞

#### **修復建議：**

```xml
<!-- 升級所有套件到最新穩定版本 -->
<PackageReference Include="Microsoft.AspNetCore.Cors" Version="8.0.0" />
```

---

## 🔑 **A07: Identification and Authentication Failures - 嚴重缺失**

### **當前狀況：❌ 不合規**

#### **發現的問題：**

1. **缺少 JWT 實作**
```csharp
// backend/Controllers/AuthController.cs - 登入成功後無 Token 生成
var response = new LoginResponse
{
    Success = true,
    Message = "登入成功",
    User = new UserInfo { ... }
    // 缺少 JWT Token
};
```

2. **無會話管理**
```csharp
// backend/Controllers/AuthController.cs - 第 62 行
return Ok(new { isAuthenticated = false, message = "未實現會話管理" });
```

3. **缺少密碼強度要求**
```csharp
// backend/Services/UserService.cs - 無密碼複雜度驗證
public async Task<User> CreateUserAsync(string username, string password, string role)
{
    // 直接接受任何密碼
}
```

4. **缺少登入失敗保護**
```csharp
// 無帳戶鎖定機制
// 無登入嘗試次數限制
```

#### **風險等級：🔴 嚴重**
- 無有效的身份驗證機制
- 容易遭受暴力破解攻擊
- 弱密碼風險

#### **修復建議：**

```csharp
// 1. 實施 JWT 認證
public class JwtService
{
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

// 2. 密碼強度驗證
public bool ValidatePasswordStrength(string password)
{
    return password.Length >= 8 && 
           Regex.IsMatch(password, @"[A-Z]") && 
           Regex.IsMatch(password, @"[a-z]") && 
           Regex.IsMatch(password, @"\d") && 
           Regex.IsMatch(password, @"[!@#$%^&*]");
}

// 3. 登入失敗保護
public class LoginAttemptService
{
    public async Task<bool> IsAccountLocked(string username)
    {
        var attempts = await GetFailedAttempts(username);
        return attempts >= 5;
    }
}
```

---

## 🔒 **A08: Software and Data Integrity Failures - 嚴重缺失**

### **當前狀況：❌ 不合規**

#### **發現的問題：**

1. **缺少檔案上傳驗證**
```csharp
// 專案中有檔案上傳功能但缺少安全驗證
// 無檔案類型檢查
// 無檔案大小限制
// 無病毒掃描
```

2. **缺少資料完整性檢查**
```csharp
// 無資料雜湊驗證
// 無數位簽章機制
```

#### **風險等級：🔴 嚴重**
- 惡意檔案上傳風險
- 資料篡改風險

#### **修復建議：**

```csharp
// 檔案上傳安全驗證
[HttpPost("upload")]
public async Task<IActionResult> UploadFile(IFormFile file)
{
    // 檔案類型驗證
    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
    if (!allowedTypes.Contains(file.ContentType))
        return BadRequest("Invalid file type");
    
    // 檔案大小限制
    if (file.Length > 5 * 1024 * 1024) // 5MB
        return BadRequest("File too large");
    
    // 計算檔案雜湊值
    var hash = await _hashService.ComputeFileHashAsync(file);
    
    return Ok(new { Hash = hash });
}
```

---

## 📊 **A09: Security Logging and Monitoring Failures - 嚴重缺失**

### **當前狀況：❌ 不合規**

#### **已實施的安全措施：**
```csharp
// backend/Program.cs - 基本日誌配置 ✅
builder.Host.UseSerilog();
```

#### **發現的問題：**

1. **缺少安全事件日誌**
```csharp
// backend/Controllers/AuthController.cs - 登入事件無記錄
[HttpPost("login")]
public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
{
    // 無登入嘗試記錄
    // 無失敗登入警報
}
```

2. **缺少監控機制**
```csharp
// 無即時安全監控
// 無異常行為檢測
// 無自動警報系統
```

#### **風險等級：🔴 嚴重**
- 無法檢測安全攻擊
- 事件回應能力不足
- 合規性問題

#### **修復建議：**

```csharp
// 安全事件日誌
public class SecurityLogger
{
    public void LogLoginAttempt(string username, bool success, string ipAddress)
    {
        _logger.LogInformation("LOGIN_ATTEMPT: User={Username}, Success={Success}, IP={IP}",
            username, success, ipAddress);
    }
    
    public void LogSecurityEvent(SecurityEventType eventType, string message)
    {
        _logger.LogWarning("SECURITY_EVENT: {EventType} - {Message}", eventType, message);
    }
}
```

---

## 🌐 **A10: Server-Side Request Forgery (SSRF) - 中等缺失**

### **當前狀況：⚠️ 部分合規**

#### **發現的問題：**

1. **缺少外部請求驗證**
```csharp
// 如果有外部 API 呼叫，缺少 URL 白名單驗證
// 缺少內部網路存取防護
```

#### **風險等級：⚠️ 中等**
- 可能被利用存取內部資源

#### **修復建議：**

```csharp
// SSRF 防護
public class HttpClientService
{
    private readonly string[] _allowedHosts = { "api.smartnameplate.com" };
    
    public async Task<string> GetExternalResourceAsync(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid URL");
        
        if (!_allowedHosts.Contains(uri.Host))
            throw new UnauthorizedAccessException("Host not allowed");
        
        return await _httpClient.GetStringAsync(uri);
    }
}
```

---

## 📋 **修復優先級建議**

### **🔴 立即修復（1-2 週）**
1. **A01: 實施 JWT 認證和授權**
2. **A07: 建立完整的身份驗證系統**
3. **A03: 實施輸入驗證和清理**
4. **A04: 加入速率限制和審計日誌**

### **⚠️ 短期修復（2-4 週）**
5. **A08: 實施檔案上傳安全驗證**
6. **A09: 建立安全監控系統**
7. **A02: 加強資料加密**

### **🟡 中期修復（1-2 個月）**
8. **A05: 完善安全配置**
9. **A10: 實施 SSRF 防護**
10. **A06: 更新所有套件版本**

---

## 🛠️ **實施計畫**

### **第一階段：核心安全（2 週）**
```csharp
// 1. JWT 認證實施
// 2. API 授權保護
// 3. 輸入驗證機制
// 4. 基本安全日誌
```

### **第二階段：進階防護（2 週）**
```csharp
// 1. 速率限制
// 2. 檔案上傳安全
// 3. 資料加密
// 4. 監控系統
```

### **第三階段：完善合規（4 週）**
```csharp
// 1. 全面安全測試
// 2. 滲透測試
// 3. 合規性驗證
// 4. 文檔完善
```

---

## 📊 **合規性檢查清單**

- [ ] **A01: Broken Access Control** - ❌ 不合規
- [ ] **A02: Cryptographic Failures** - ⚠️ 部分合規
- [ ] **A03: Injection** - ❌ 不合規
- [ ] **A04: Insecure Design** - ❌ 不合規
- [ ] **A05: Security Misconfiguration** - ⚠️ 部分合規
- [ ] **A06: Vulnerable Components** - 🟡 基本合規
- [ ] **A07: Authentication Failures** - ❌ 不合規
- [ ] **A08: Integrity Failures** - ❌ 不合規
- [ ] **A09: Logging Failures** - ❌ 不合規
- [ ] **A10: SSRF** - ⚠️ 部分合規

**總體合規率：10%** 🔴

---

## 🎯 **結論**

SmartNameplate 專案目前存在嚴重的安全缺失，**不適合直接部署到生產環境**。建議立即開始安全修復工作，優先處理身份驗證、授權控制和輸入驗證等核心安全問題。

完成所有修復後，預期可達到 **90%+ OWASP 合規率**，符合生產環境安全要求。

---

**報告生成時間：** $(Get-Date)  
**評估標準：** OWASP Top 10 2021  
**評估範圍：** SmartNameplate 完整專案 