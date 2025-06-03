# 🛠️ SmartNameplate IIS 部署問題診斷與解決方案

## 🚨 **常見問題清單**

### 1. **ASP.NET Core Runtime 問題**
```bash
# 錯誤訊息：HTTP Error 500.31 - ANCM Failed to Start Process
# 原因：伺服器缺少 ASP.NET Core Runtime
```

**解決方案：**
```powershell
# 下載並安裝 ASP.NET Core Runtime 8.0
# 訪問：https://dotnet.microsoft.com/download/dotnet/8.0
# 安裝：ASP.NET Core Runtime 8.0.x (includes .NET Runtime) - Windows x64

# 驗證安裝
dotnet --list-runtimes
# 應該看到：Microsoft.AspNetCore.App 8.0.x
```

### 2. **IIS 模組問題**
```bash
# 錯誤訊息：HTTP Error 500.19 - Internal Server Error
# 原因：缺少 ASP.NET Core Module v2 (ANCM)
```

**解決方案：**
```powershell
# 安裝 IIS ASP.NET Core Module
# 從 Microsoft 下載：ASP.NET Core Module v2
# 或通過 Server Manager 安裝：
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ASPNET45
```

### 3. **Angular SPA 路由問題**
```bash
# 錯誤：刷新頁面顯示 404 Not Found
# 原因：IIS 不知道如何處理 Angular 路由
```

### 4. **權限問題**
```bash
# 錯誤：Access denied 或 Permission denied
# 原因：IIS 應用程式池身分權限不足
```

### 5. **API 代理問題**
```bash
# 錯誤：API 請求失敗，CORS 錯誤
# 原因：生產環境中前端無法正確代理到後端 API
```

---

## 🔧 **完整解決方案**

### **步驟 1：建立正確的 web.config**
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <!-- ASP.NET Core Module 配置 -->
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\SmartNameplate.Api.dll" 
                  stdoutLogEnabled="true" 
                  stdoutLogFile=".\logs\stdout" 
                  hostingModel="inprocess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        </environmentVariables>
      </aspNetCore>
      
      <!-- Angular SPA 路由支援 -->
      <rewrite>
        <rules>
          <!-- API 請求不重寫 -->
          <rule name="API Routes" stopProcessing="true">
            <match url="^api/.*" />
            <action type="None" />
          </rule>
          
          <!-- 靜態檔案不重寫 -->
          <rule name="Static Files" stopProcessing="true">
            <match url="^.*\.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$" />
            <action type="None" />
          </rule>
          
          <!-- Angular 路由重寫到 index.html -->
          <rule name="Angular Routes" stopProcessing="true">
            <match url=".*" />
            <conditions logicalGrouping="MatchAll">
              <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
              <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
            </conditions>
            <action type="Rewrite" url="/" />
          </rule>
        </rules>
      </rewrite>
      
      <!-- 安全標頭 -->
      <httpProtocol>
        <customHeaders>
          <add name="X-Frame-Options" value="DENY" />
          <add name="X-Content-Type-Options" value="nosniff" />
          <add name="X-XSS-Protection" value="1; mode=block" />
          <add name="Strict-Transport-Security" value="max-age=31536000; includeSubDomains" />
        </customHeaders>
      </httpProtocol>
      
      <!-- 靜態檔案處理 -->
      <staticContent>
        <mimeMap fileExtension=".json" mimeType="application/json" />
        <mimeMap fileExtension=".woff" mimeType="application/font-woff" />
        <mimeMap fileExtension=".woff2" mimeType="application/font-woff2" />
      </staticContent>
    </system.webServer>
  </location>
</configuration>
```

### **步驟 2：修改 ASP.NET Core 程式配置**
```csharp
// Program.cs 中加入 IIS 配置
var builder = WebApplication.CreateBuilder(args);

// IIS 配置
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AutomaticAuthentication = false;
});

// 支援靜態檔案
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 設定靜態檔案
app.UseStaticFiles();

// CORS 配置（生產環境）
if (app.Environment.IsProduction())
{
    app.UseCors(policy => policy
        .WithOrigins("https://yourdomain.com") // 替換為您的域名
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
}

// SPA 回退路由
app.MapFallbackToFile("index.html");

app.Run();
```

### **步驟 3：正確的目錄結構**
```
C:\inetpub\wwwroot\SmartNameplate\
├── wwwroot\                    # Angular 建置檔案
│   ├── index.html
│   ├── main.js
│   ├── styles.css
│   └── assets\
├── SmartNameplate.Api.dll      # ASP.NET Core 檔案
├── SmartNameplate.Api.deps.json
├── SmartNameplate.Api.runtimeconfig.json
├── web.config                  # IIS 配置檔案
├── logs\                      # 日誌資料夾
└── appsettings.Production.json # 生產配置
```

---

## 🚀 **自動化部署腳本**

### **建置和部署腳本**
```powershell
# Build-And-Deploy.ps1
param(
    [string]$TargetPath = "C:\inetpub\wwwroot\SmartNameplate",
    [string]$Environment = "Production"
)

Write-Host "🤖 開始建置和部署 SmartNameplate..." -ForegroundColor Cyan

# 1. 建置後端
Write-Host "🔨 建置 ASP.NET Core 後端..." -ForegroundColor Green
Set-Location backend
dotnet publish -c Release -o "$TargetPath" --runtime win-x64 --self-contained false

# 2. 建置前端
Write-Host "🔨 建置 Angular 前端..." -ForegroundColor Green
Set-Location ..\frontend
npm ci
ng build --configuration=production

# 3. 複製前端檔案到 wwwroot
Write-Host "📁 複製前端檔案..." -ForegroundColor Green
$frontendDist = "dist\smart-nameplate"
$wwwrootPath = "$TargetPath\wwwroot"

if (Test-Path $wwwrootPath) {
    Remove-Item $wwwrootPath -Recurse -Force
}
New-Item -ItemType Directory -Path $wwwrootPath | Out-Null
Copy-Item "$frontendDist\*" -Destination $wwwrootPath -Recurse

# 4. 建立正確的 web.config
Write-Host "📄 建立 web.config..." -ForegroundColor Green
# (使用上面的 web.config 內容)

# 5. 設定權限
Write-Host "🔒 設定權限..." -ForegroundColor Green
icacls "$TargetPath" /grant "IIS_IUSRS:(OI)(CI)R" /T
icacls "$TargetPath\logs" /grant "IIS_IUSRS:(OI)(CI)M" /T

# 6. 重啟應用程式池
Write-Host "🔄 重啟應用程式池..." -ForegroundColor Green
Import-Module WebAdministration
Restart-WebAppPool -Name "DefaultAppPool"

Write-Host "✅ 部署完成!" -ForegroundColor Green
```

---

## 🔍 **診斷工具和指令**

### **檢查運行時**
```powershell
# 檢查 .NET Runtime
dotnet --list-runtimes

# 檢查 ASP.NET Core Module
Get-WindowsFeature -Name "IIS-ASPNET*"

# 檢查 IIS 狀態
Get-Service W3SVC
Get-Service WAS
```

### **檢查日誌**
```powershell
# 檢查 Windows Event Log
Get-EventLog -LogName Application -Source "ASP.NET Core*" -Newest 10

# 檢查 IIS 日誌
Get-Content "C:\inetpub\logs\LogFiles\W3SVC1\*.log" | Select-Object -Last 10

# 檢查應用程式 stdout 日誌
Get-Content "$TargetPath\logs\stdout*.log" | Select-Object -Last 10
```

### **測試連線**
```powershell
# 測試本地 API
Invoke-RestMethod -Uri "http://localhost/api/health" -Method GET

# 測試前端
Invoke-WebRequest -Uri "http://localhost" -UseBasicParsing
```

---

## ⚠️ **常見錯誤修復**

### **502.5 Process Failure**
```bash
# 檢查：
1. .NET Runtime 是否安裝
2. 應用程式檔案是否完整
3. appsettings.json 是否正確
4. 資料庫連線是否可用
```

### **500.31 ANCM Failed to Start**
```bash
# 解決：
1. 安裝正確版本的 ASP.NET Core Runtime
2. 檢查應用程式池 .NET CLR 版本設為 "No Managed Code"
3. 檢查 web.config 中的 processPath 和 arguments
```

### **404 Angular 路由錯誤**
```bash
# 解決：
1. 確保 web.config 中有正確的 URL 重寫規則
2. 檢查 Angular 建置是否正確
3. 確認 base href 設定
```

---

## 📋 **部署檢查清單**

- [ ] ✅ 安裝 ASP.NET Core Runtime 8.0
- [ ] ✅ 安裝 ASP.NET Core Module v2
- [ ] ✅ 建立正確的 web.config
- [ ] ✅ 設定應用程式池為 "No Managed Code"
- [ ] ✅ 建置 Angular 應用程式 (production)
- [ ] ✅ 發布 ASP.NET Core 應用程式
- [ ] ✅ 設定正確的檔案權限
- [ ] ✅ 建立日誌資料夾
- [ ] ✅ 測試 API 端點
- [ ] ✅ 測試 Angular 路由
- [ ] ✅ 檢查瀏覽器控制台錯誤
- [ ] ✅ 檢查 Windows Event Log

如果按照這些步驟還有問題，請提供具體的錯誤訊息，我可以進一步協助診斷！ 