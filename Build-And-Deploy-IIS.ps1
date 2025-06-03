# 🚀 SmartNameplate IIS 自動化部署腳本
# 解決所有常見的 IIS 部署問題

param(
    [Parameter(Mandatory=$false)]
    [string]$TargetPath = "C:\inetpub\wwwroot\SmartNameplate",
    
    [Parameter(Mandatory=$false)]
    [string]$SiteName = "SmartNameplate",
    
    [Parameter(Mandatory=$false)]
    [string]$AppPoolName = "SmartNameplatePool",
    
    [Parameter(Mandatory=$false)]
    [int]$Port = 80,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild
)

Write-Host "🤖 SmartNameplate IIS 自動化部署腳本啟動" -ForegroundColor Cyan
Write-Host "📅 部署時間: $(Get-Date)" -ForegroundColor Gray

# 檢查管理員權限
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "❌ 需要管理員權限執行此腳本" -ForegroundColor Red
    exit 1
}

# 1. 檢查必要元件
Write-Host "`n🔍 檢查系統需求..." -ForegroundColor Green

# 檢查 .NET Runtime
try {
    $dotnetInfo = dotnet --list-runtimes 2>$null
    $aspNetCoreRuntime = $dotnetInfo | Where-Object { $_ -match "Microsoft.AspNetCore.App 8\." }
    
    if ($aspNetCoreRuntime) {
        Write-Host "  ✅ ASP.NET Core Runtime 8.0 已安裝" -ForegroundColor Gray
    } else {
        Write-Host "  ❌ 缺少 ASP.NET Core Runtime 8.0" -ForegroundColor Red
        Write-Host "  請從此連結下載安裝：https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host "  ❌ 無法檢查 .NET Runtime，請確保已安裝" -ForegroundColor Red
    exit 1
}

# 檢查 IIS
$iisService = Get-Service -Name "W3SVC" -ErrorAction SilentlyContinue
if (-not $iisService) {
    Write-Host "  ❌ IIS 未安裝或未啟動" -ForegroundColor Red
    Write-Host "  請安裝 IIS 角色" -ForegroundColor Yellow
    exit 1
} else {
    Write-Host "  ✅ IIS 服務正常" -ForegroundColor Gray
}

# 2. 停止相關服務
Write-Host "`n🛑 停止相關服務..." -ForegroundColor Green

try {
    Import-Module WebAdministration -ErrorAction SilentlyContinue
    
    # 停止應用程式池（如果存在）
    if (Get-WebAppPool -Name $AppPoolName -ErrorAction SilentlyContinue) {
        Stop-WebAppPool -Name $AppPoolName
        Write-Host "  ✅ 已停止應用程式池: $AppPoolName" -ForegroundColor Gray
    }
    
    # 停止網站（如果存在）
    if (Get-Website -Name $SiteName -ErrorAction SilentlyContinue) {
        Stop-Website -Name $SiteName
        Write-Host "  ✅ 已停止網站: $SiteName" -ForegroundColor Gray
    }
} catch {
    Write-Host "  ⚠️ 停止服務時遇到問題，繼續執行..." -ForegroundColor Yellow
}

# 3. 建置應用程式
if (-not $SkipBuild) {
    Write-Host "`n🔨 建置應用程式..." -ForegroundColor Green
    
    # 確保在正確的目錄
    $originalLocation = Get-Location
    
    try {
        # 建置後端
        Write-Host "  📦 建置 ASP.NET Core 後端..." -ForegroundColor Gray
        Set-Location "backend"
        
        $publishResult = dotnet publish -c Release -o "$TargetPath" --runtime win-x64 --self-contained false
        if ($LASTEXITCODE -ne 0) {
            throw "後端建置失敗"
        }
        Write-Host "  ✅ 後端建置完成" -ForegroundColor Gray
        
        # 建置前端
        Write-Host "  📦 建置 Angular 前端..." -ForegroundColor Gray
        Set-Location "..\frontend"
        
        # 安裝依賴
        npm ci --silent
        if ($LASTEXITCODE -ne 0) {
            throw "前端依賴安裝失敗"
        }
        
        # 建置
        ng build --configuration=production
        if ($LASTEXITCODE -ne 0) {
            throw "前端建置失敗"
        }
        Write-Host "  ✅ 前端建置完成" -ForegroundColor Gray
        
        # 複製前端檔案到 wwwroot
        $frontendDist = "dist\smart-nameplate"
        $wwwrootPath = "$TargetPath\wwwroot"
        
        if (Test-Path $wwwrootPath) {
            Remove-Item $wwwrootPath -Recurse -Force
        }
        New-Item -ItemType Directory -Path $wwwrootPath -Force | Out-Null
        Copy-Item "$frontendDist\*" -Destination $wwwrootPath -Recurse -Force
        Write-Host "  ✅ 前端檔案複製完成" -ForegroundColor Gray
        
    } catch {
        Write-Host "  ❌ 建置失敗: $($_.Exception.Message)" -ForegroundColor Red
        Set-Location $originalLocation
        exit 1
    } finally {
        Set-Location $originalLocation
    }
}

# 4. 建立目錄結構
Write-Host "`n📁 建立目錄結構..." -ForegroundColor Green

$directories = @(
    "$TargetPath\logs",
    "$TargetPath\wwwroot",
    "$TargetPath\wwwroot\assets"
)

foreach ($dir in $directories) {
    if (!(Test-Path $dir)) {
        New-Item -ItemType Directory -Force -Path $dir | Out-Null
        Write-Host "  ✅ 建立: $dir" -ForegroundColor Gray
    }
}

# 5. 建立 web.config
Write-Host "`n📄 建立 web.config..." -ForegroundColor Green

$webConfig = @"
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
          <environmentVariable name="ASPNETCORE_HTTP_PORTS" value="$Port" />
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
            <match url="^.*\.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot|json|txt|map)$" />
            <action type="None" />
          </rule>
          
          <!-- Angular 路由重寫到 index.html -->
          <rule name="Angular Routes" stopProcessing="true">
            <match url=".*" />
            <conditions logicalGrouping="MatchAll">
              <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
              <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
            </conditions>
            <action type="Rewrite" url="/index.html" />
          </rule>
        </rules>
      </rewrite>
      
      <!-- 安全標頭 -->
      <httpProtocol>
        <customHeaders>
          <add name="X-Frame-Options" value="DENY" />
          <add name="X-Content-Type-Options" value="nosniff" />
          <add name="X-XSS-Protection" value="1; mode=block" />
          <add name="Referrer-Policy" value="strict-origin-when-cross-origin" />
          <remove name="Server" />
          <remove name="X-Powered-By" />
          <remove name="X-AspNet-Version" />
          <remove name="X-AspNetMvc-Version" />
        </customHeaders>
      </httpProtocol>
      
      <!-- 靜態檔案處理 -->
      <staticContent>
        <mimeMap fileExtension=".json" mimeType="application/json" />
        <mimeMap fileExtension=".woff" mimeType="application/font-woff" />
        <mimeMap fileExtension=".woff2" mimeType="application/font-woff2" />
        <mimeMap fileExtension=".map" mimeType="application/json" />
      </staticContent>
      
      <!-- 錯誤頁面 -->
      <httpErrors errorMode="Custom" existingResponse="Replace">
        <remove statusCode="404" subStatusCode="-1" />
        <error statusCode="404" path="/index.html" responseMode="ExecuteURL" />
      </httpErrors>
    </system.webServer>
  </location>
</configuration>
"@

$webConfig | Out-File -FilePath "$TargetPath\web.config" -Encoding UTF8
Write-Host "  ✅ web.config 已建立" -ForegroundColor Gray

# 6. 設定檔案權限
Write-Host "`n🔒 設定檔案權限..." -ForegroundColor Green

try {
    # 設定主目錄權限
    icacls "$TargetPath" /grant "IIS_IUSRS:(OI)(CI)R" /T | Out-Null
    icacls "$TargetPath" /grant "IUSR:(OI)(CI)R" /T | Out-Null
    
    # 設定日誌目錄權限
    icacls "$TargetPath\logs" /grant "IIS_IUSRS:(OI)(CI)M" /T | Out-Null
    icacls "$TargetPath\logs" /grant "NETWORK SERVICE:(OI)(CI)M" /T | Out-Null
    
    Write-Host "  ✅ 檔案權限設定完成" -ForegroundColor Gray
} catch {
    Write-Host "  ⚠️ 權限設定警告: $($_.Exception.Message)" -ForegroundColor Yellow
}

# 7. 設定 IIS 應用程式池
Write-Host "`n🏊 設定應用程式池..." -ForegroundColor Green

try {
    # 刪除現有應用程式池
    if (Get-WebAppPool -Name $AppPoolName -ErrorAction SilentlyContinue) {
        Remove-WebAppPool -Name $AppPoolName
        Write-Host "  🗑️ 已刪除現有應用程式池" -ForegroundColor Gray
    }
    
    # 建立新應用程式池
    New-WebAppPool -Name $AppPoolName
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "managedRuntimeVersion" -Value ""
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "processModel.identityType" -Value "ApplicationPoolIdentity"
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "recycling.periodicRestart.time" -Value "00:00:00"
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "processModel.idleTimeout" -Value "00:00:00"
    
    Write-Host "  ✅ 應用程式池 '$AppPoolName' 已建立" -ForegroundColor Gray
} catch {
    Write-Host "  ❌ 應用程式池設定失敗: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 8. 設定 IIS 網站
Write-Host "`n🌐 設定 IIS 網站..." -ForegroundColor Green

try {
    # 刪除現有網站
    if (Get-Website -Name $SiteName -ErrorAction SilentlyContinue) {
        Remove-Website -Name $SiteName
        Write-Host "  🗑️ 已刪除現有網站" -ForegroundColor Gray
    }
    
    # 建立新網站
    New-Website -Name $SiteName -PhysicalPath $TargetPath -Port $Port -ApplicationPool $AppPoolName
    
    Write-Host "  ✅ 網站 '$SiteName' 已建立在埠口 $Port" -ForegroundColor Gray
} catch {
    Write-Host "  ❌ 網站設定失敗: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 9. 啟動服務
Write-Host "`n🚀 啟動服務..." -ForegroundColor Green

try {
    # 啟動應用程式池
    Start-WebAppPool -Name $AppPoolName
    Start-Sleep -Seconds 2
    
    # 啟動網站
    Start-Website -Name $SiteName
    Start-Sleep -Seconds 3
    
    Write-Host "  ✅ 服務已啟動" -ForegroundColor Gray
} catch {
    Write-Host "  ⚠️ 服務啟動警告: $($_.Exception.Message)" -ForegroundColor Yellow
}

# 10. 驗證部署
Write-Host "`n🔍 驗證部署..." -ForegroundColor Green

try {
    # 檢查應用程式池狀態
    $poolState = (Get-WebAppPool -Name $AppPoolName).State
    Write-Host "  📊 應用程式池狀態: $poolState" -ForegroundColor Gray
    
    # 檢查網站狀態
    $siteState = (Get-Website -Name $SiteName).State
    Write-Host "  📊 網站狀態: $siteState" -ForegroundColor Gray
    
    # 測試連線
    Start-Sleep -Seconds 5
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:$Port" -UseBasicParsing -TimeoutSec 10
        if ($response.StatusCode -eq 200) {
            Write-Host "  ✅ 網站回應正常 (HTTP 200)" -ForegroundColor Gray
        } else {
            Write-Host "  ⚠️ 網站回應異常 (HTTP $($response.StatusCode))" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  ⚠️ 無法連線到網站: $($_.Exception.Message)" -ForegroundColor Yellow
        Write-Host "  💡 請檢查防火牆設定和埠口 $Port 是否被佔用" -ForegroundColor Blue
    }
    
} catch {
    Write-Host "  ⚠️ 驗證過程中發生錯誤: $($_.Exception.Message)" -ForegroundColor Yellow
}

# 11. 建立診斷腳本
Write-Host "`n🛠️ 建立診斷工具..." -ForegroundColor Green

$diagScript = @"
# SmartNameplate 診斷腳本
Write-Host "🔍 SmartNameplate 診斷報告" -ForegroundColor Cyan
Write-Host "📅 檢查時間: `$(Get-Date)" -ForegroundColor Gray

# 檢查服務狀態
Write-Host "`n📊 服務狀態:"
Write-Host "  應用程式池: `$((Get-WebAppPool -Name '$AppPoolName').State)"
Write-Host "  網站: `$((Get-Website -Name '$SiteName').State)"

# 檢查檔案
Write-Host "`n📁 檔案檢查:"
if (Test-Path "$TargetPath\SmartNameplate.Api.dll") {
    Write-Host "  ✅ API DLL 存在"
} else {
    Write-Host "  ❌ API DLL 缺失"
}

if (Test-Path "$TargetPath\wwwroot\index.html") {
    Write-Host "  ✅ 前端檔案存在"
} else {
    Write-Host "  ❌ 前端檔案缺失"
}

# 檢查日誌
Write-Host "`n📄 最新日誌:"
`$logFiles = Get-ChildItem "$TargetPath\logs\stdout*.log" -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending
if (`$logFiles) {
    Get-Content `$logFiles[0].FullName -Tail 5
} else {
    Write-Host "  無日誌檔案"
}

# 檢查連線
Write-Host "`n🌐 連線測試:"
try {
    `$response = Invoke-WebRequest -Uri "http://localhost:$Port" -UseBasicParsing -TimeoutSec 5
    Write-Host "  ✅ HTTP `$(`$response.StatusCode)"
} catch {
    Write-Host "  ❌ 連線失敗: `$(`$_.Exception.Message)"
}
"@

$diagScript | Out-File -FilePath "$TargetPath\Diagnose-SmartNameplate.ps1" -Encoding UTF8
Write-Host "  ✅ 診斷腳本已建立: $TargetPath\Diagnose-SmartNameplate.ps1" -ForegroundColor Gray

# 12. 完成部署
Write-Host "`n🎉 部署完成!" -ForegroundColor Green
Write-Host "📁 部署路徑: $TargetPath" -ForegroundColor Cyan
Write-Host "🌐 網站地址: http://localhost:$Port" -ForegroundColor Cyan
Write-Host "📊 應用程式池: $AppPoolName" -ForegroundColor Cyan
Write-Host "🏷️ 網站名稱: $SiteName" -ForegroundColor Cyan

Write-Host "`n💡 使用提示:" -ForegroundColor Yellow
Write-Host "  • 執行診斷: .\Diagnose-SmartNameplate.ps1" -ForegroundColor White
Write-Host "  • 查看日誌: Get-Content '$TargetPath\logs\stdout*.log'" -ForegroundColor White
Write-Host "  • 重啟應用: Restart-WebAppPool -Name '$AppPoolName'" -ForegroundColor White

Write-Host "`n🚨 如果遇到問題:" -ForegroundColor Red
Write-Host "  1. 檢查 Windows Event Log" -ForegroundColor White
Write-Host "  2. 檢查應用程式日誌" -ForegroundColor White
Write-Host "  3. 確認 ASP.NET Core Runtime 8.0 已安裝" -ForegroundColor White
Write-Host "  4. 檢查防火牆和埠口設定" -ForegroundColor White

Write-Host "`n🤖 SmartNameplate IIS 部署腳本執行完成!" -ForegroundColor Cyan 