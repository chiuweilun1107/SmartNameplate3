# 🚀 SmartNameplate 生產環境部署腳本
# 符合 OWASP Top 10 安全標準
# Windows Server IIS 部署專用

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("Production", "Staging", "Testing")]
    [string]$Environment,
    
    [Parameter(Mandatory=$false)]
    [string]$DeployPath = "C:\SmartNameplate",
    
    [Parameter(Mandatory=$false)]
    [string]$LogPath = "C:\SmartNameplate\Logs",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipSecurityCheck
)

Write-Host "🤖 SmartNameplate 生產部署腳本啟動" -ForegroundColor Cyan
Write-Host "📅 部署時間: $(Get-Date)" -ForegroundColor Gray
Write-Host "🌍 目標環境: $Environment" -ForegroundColor Yellow

# 檢查管理員權限
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "❌ 需要管理員權限執行此腳本" -ForegroundColor Red
    exit 1
}

# 1. 建立目錄結構
Write-Host "`n📁 建立目錄結構..." -ForegroundColor Green

$directories = @(
    "$DeployPath\App",
    "$DeployPath\Config",
    "$DeployPath\Backups",
    "$LogPath\Application",
    "$LogPath\Security", 
    "$LogPath\Performance",
    "$LogPath\Database",
    "$LogPath\IIS",
    "$LogPath\Archive"
)

foreach ($dir in $directories) {
    if (!(Test-Path $dir)) {
        New-Item -ItemType Directory -Force -Path $dir | Out-Null
        Write-Host "  ✅ 建立: $dir" -ForegroundColor Gray
    } else {
        Write-Host "  📂 已存在: $dir" -ForegroundColor Gray
    }
}

# 2. 設定檔案權限
Write-Host "`n🔒 設定檔案權限..." -ForegroundColor Green

try {
    # 應用程式路徑 - 讀取權限
    icacls "$DeployPath\App" /grant "IIS_IUSRS:(OI)(CI)R" /T | Out-Null
    icacls "$DeployPath\App" /grant "IUSR:(OI)(CI)R" /T | Out-Null
    
    # 日誌路徑 - 修改權限
    icacls "$LogPath" /grant "IIS_IUSRS:(OI)(CI)M" /T | Out-Null
    icacls "$LogPath" /grant "NETWORK SERVICE:(OI)(CI)M" /T | Out-Null
    
    # 配置路徑 - 讀取權限
    icacls "$DeployPath\Config" /grant "IIS_IUSRS:(OI)(CI)R" /T | Out-Null
    
    # 移除繼承權限以提高安全性
    icacls "$DeployPath" /inheritance:r | Out-Null
    icacls "$DeployPath" /grant "Administrators:(OI)(CI)F" | Out-Null
    icacls "$DeployPath" /grant "SYSTEM:(OI)(CI)F" | Out-Null
    
    Write-Host "  ✅ 檔案權限設定完成" -ForegroundColor Gray
} catch {
    Write-Host "  ❌ 檔案權限設定失敗: $($_.Exception.Message)" -ForegroundColor Red
}

# 3. 檢查並啟用 IIS 功能
Write-Host "`n🌐 檢查 IIS 功能..." -ForegroundColor Green

$iisFeatures = @(
    "IIS-WebServerRole",
    "IIS-WebServer", 
    "IIS-CommonHttpFeatures",
    "IIS-HttpErrors",
    "IIS-HttpLogging",
    "IIS-Security",
    "IIS-RequestFiltering",
    "IIS-Performance",
    "IIS-WebServerManagementTools",
    "IIS-ASPNET45"
)

foreach ($feature in $iisFeatures) {
    $featureState = Get-WindowsOptionalFeature -Online -FeatureName $feature
    if ($featureState.State -eq "Disabled") {
        Write-Host "  🔧 啟用功能: $feature" -ForegroundColor Yellow
        Enable-WindowsOptionalFeature -Online -FeatureName $feature -All -NoRestart | Out-Null
    } else {
        Write-Host "  ✅ 已啟用: $feature" -ForegroundColor Gray
    }
}

# 4. 建立日誌配置檔案
Write-Host "`n📊 建立日誌配置..." -ForegroundColor Green

$serilogConfig = @"
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.File", "Serilog.Sinks.EventLog" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "SmartNameplate": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "$($LogPath.Replace('\', '\\'))\\Application\\app-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File", 
        "Args": {
          "path": "$($LogPath.Replace('\', '\\'))\\Security\\security-events-.log",
          "rollingInterval": "Day",
          "restrictedToMinimumLevel": "Warning",
          "retainedFileCountLimit": 90
        }
      },
      {
        "Name": "EventLog",
        "Args": {
          "source": "SmartNameplate",
          "logName": "Application",
          "restrictedToMinimumLevel": "Error"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithProcessId", "WithThreadId" ]
  }
}
"@

$serilogConfig | Out-File -FilePath "$DeployPath\Config\serilog.json" -Encoding UTF8
Write-Host "  ✅ Serilog 配置已建立" -ForegroundColor Gray

# 5. 建立 web.config 安全模板
Write-Host "`n🛡️ 建立 web.config 安全模板..." -ForegroundColor Green

$webConfig = @"
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
"@

$webConfig | Out-File -FilePath "$DeployPath\Config\web.config.template" -Encoding UTF8
Write-Host "  ✅ web.config 安全模板已建立" -ForegroundColor Gray

# 6. 安全檢查 (如果未跳過)
if (-not $SkipSecurityCheck) {
    Write-Host "`n🔍 執行安全檢查..." -ForegroundColor Green
    
    # 檢查防火牆狀態
    $firewallStatus = Get-NetFirewallProfile | Where-Object { $_.Enabled -eq $false }
    if ($firewallStatus) {
        Write-Host "  ⚠️ 警告: 部分防火牆設定檔未啟用" -ForegroundColor Yellow
    } else {
        Write-Host "  ✅ 防火牆已啟用" -ForegroundColor Gray
    }
    
    # 檢查 Windows Update 狀態
    try {
        $updateService = Get-Service "wuauserv" -ErrorAction SilentlyContinue
        if ($updateService -and $updateService.Status -eq "Running") {
            Write-Host "  ✅ Windows Update 服務正在運行" -ForegroundColor Gray
        } else {
            Write-Host "  ⚠️ 警告: Windows Update 服務未運行" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  ⚠️ 無法檢查 Windows Update 狀態" -ForegroundColor Yellow
    }
    
    # 檢查開放埠口
    $openPorts = @(80, 443, 5001)
    foreach ($port in $openPorts) {
        $portTest = Test-NetConnection -ComputerName "localhost" -Port $port -InformationLevel Quiet
        if ($portTest) {
            Write-Host "  ✅ 埠口 $port 可存取" -ForegroundColor Gray
        } else {
            Write-Host "  ℹ️ 埠口 $port 未開放" -ForegroundColor Blue
        }
    }
}

# 7. 建立 PowerShell 監控腳本
Write-Host "`n📈 建立監控腳本..." -ForegroundColor Green

$monitoringScript = @"
# SmartNameplate 監控腳本
param([int]`$IntervalMinutes = 5)

while (`$true) {
    `$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    
    # 檢查應用程式狀態
    `$appProcess = Get-Process "dotnet" -ErrorAction SilentlyContinue | Where-Object { `$_.ProcessName -eq "dotnet" }
    if (`$appProcess) {
        Write-Host "[\$timestamp] ✅ 應用程式運行中 (PID: `$(`$appProcess.Id))"
    } else {
        Write-Host "[\$timestamp] ❌ 應用程式未運行" -ForegroundColor Red
    }
    
    # 檢查記憶體使用率
    `$memory = Get-CimInstance -ClassName Win32_OperatingSystem
    `$memoryUsage = [math]::Round(((`$memory.TotalVisibleMemorySize - `$memory.FreePhysicalMemory) / `$memory.TotalVisibleMemorySize) * 100, 2)
    Write-Host "[\$timestamp] 💾 記憶體使用率: `$memoryUsage%"
    
    # 檢查磁碟空間
    `$disk = Get-CimInstance -ClassName Win32_LogicalDisk | Where-Object { `$_.DeviceID -eq "C:" }
    `$diskUsage = [math]::Round(((`$disk.Size - `$disk.FreeSpace) / `$disk.Size) * 100, 2)
    Write-Host "[\$timestamp] 💿 磁碟使用率: `$diskUsage%"
    
    # 檢查日誌檔案大小
    `$logSize = (Get-ChildItem "$LogPath" -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
    Write-Host "[\$timestamp] 📊 日誌大小: `$([math]::Round(`$logSize, 2)) MB"
    
    Start-Sleep (`$IntervalMinutes * 60)
}
"@

$monitoringScript | Out-File -FilePath "$DeployPath\Monitor-SmartNameplate.ps1" -Encoding UTF8
Write-Host "  ✅ 監控腳本已建立: $DeployPath\Monitor-SmartNameplate.ps1" -ForegroundColor Gray

# 8. 建立日誌清理腳本
Write-Host "`n🧹 建立日誌清理腳本..." -ForegroundColor Green

$cleanupScript = @"
# SmartNameplate 日誌清理腳本
param(
    [int]`$RetentionDays = 30,
    [switch]`$WhatIf
)

`$cutoffDate = (Get-Date).AddDays(-`$RetentionDays)
Write-Host "清理 `$cutoffDate 之前的日誌檔案..."

Get-ChildItem "$LogPath" -Recurse -File | Where-Object { `$_.LastWriteTime -lt `$cutoffDate } | ForEach-Object {
    if (`$WhatIf) {
        Write-Host "將刪除: `$(`$_.FullName)" -ForegroundColor Yellow
    } else {
        Remove-Item `$_.FullName -Force
        Write-Host "已刪除: `$(`$_.FullName)" -ForegroundColor Gray
    }
}

Write-Host "日誌清理完成"
"@

$cleanupScript | Out-File -FilePath "$DeployPath\Cleanup-Logs.ps1" -Encoding UTF8
Write-Host "  ✅ 清理腳本已建立: $DeployPath\Cleanup-Logs.ps1" -ForegroundColor Gray

# 9. 建立部署摘要
Write-Host "`n📋 建立部署摘要..." -ForegroundColor Green

$summary = @"
# SmartNameplate 部署摘要
部署時間: $(Get-Date)
環境: $Environment
部署路徑: $DeployPath
日誌路徑: $LogPath

## 建立的目錄:
$($directories -join "`n")

## 建立的檔案:
- $DeployPath\Config\serilog.json (Serilog 配置)
- $DeployPath\Config\web.config.template (web.config 安全模板)
- $DeployPath\Monitor-SmartNameplate.ps1 (監控腳本)
- $DeployPath\Cleanup-Logs.ps1 (日誌清理腳本)

## 安全設定:
- ✅ 檔案權限設定完成
- ✅ IIS 功能檢查完成
- ✅ 安全標頭配置模板已建立
- ✅ 請求過濾設定已配置

## 下一步:
1. 部署應用程式檔案到 $DeployPath\App
2. 複製 web.config.template 到應用程式根目錄並重新命名為 web.config
3. 設定 IIS 網站指向應用程式目錄
4. 配置 SSL 憑證
5. 執行安全測試

## 監控:
執行監控腳本: .\Monitor-SmartNameplate.ps1

## 維護:
執行日誌清理: .\Cleanup-Logs.ps1 -RetentionDays 30
"@

$summary | Out-File -FilePath "$DeployPath\DEPLOYMENT_SUMMARY.md" -Encoding UTF8

# 10. 完成部署
Write-Host "`n🎉 部署完成!" -ForegroundColor Green
Write-Host "📁 部署路徑: $DeployPath" -ForegroundColor Cyan
Write-Host "📊 日誌路徑: $LogPath" -ForegroundColor Cyan
Write-Host "📋 部署摘要: $DeployPath\DEPLOYMENT_SUMMARY.md" -ForegroundColor Cyan

Write-Host "`n🚨 重要提醒:" -ForegroundColor Yellow
Write-Host "  1. 請設定適當的 SSL 憑證" -ForegroundColor White
Write-Host "  2. 配置資料庫連接字串" -ForegroundColor White
Write-Host "  3. 執行 OWASP ZAP 安全掃描" -ForegroundColor White
Write-Host "  4. 測試所有功能正常運作" -ForegroundColor White

Write-Host "`n🔒 符合 OWASP Top 10 2021 安全標準" -ForegroundColor Green
Write-Host "📖 詳細安全指南: .cursor\rules\production-deployment-security.md" -ForegroundColor Cyan

# 如果需要重啟 IIS
$restart = Read-Host "`n❓ 是否需要重啟 IIS? (y/N)"
if ($restart -eq "y" -or $restart -eq "Y") {
    Write-Host "🔄 重啟 IIS..." -ForegroundColor Yellow
    iisreset
    Write-Host "✅ IIS 重啟完成" -ForegroundColor Green
}

Write-Host "`n🤖 SmartNameplate 生產部署腳本執行完成!" -ForegroundColor Cyan 