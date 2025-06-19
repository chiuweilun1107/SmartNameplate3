# 🤖 智慧桌牌系統 - 新電腦自動化部署腳本 (Windows)
# 使用方法: Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
#          .\setup-new-computer.ps1

param(
    [switch]$SkipMigration = $false
)

# 顏色函數
function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "[SUCCESS] $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# 檢查系統需求
function Test-Requirements {
    Write-Info "檢查系統需求..."
    
    # 檢查 .NET SDK
    try {
        $dotnetVersion = dotnet --version
        Write-Success ".NET SDK 已安裝: $dotnetVersion"
    }
    catch {
        Write-Error ".NET SDK 未安裝，請先安裝 .NET 8 SDK"
        exit 1
    }
    
    # 檢查 Node.js
    try {
        $nodeVersion = node --version
        Write-Success "Node.js 已安裝: $nodeVersion"
    }
    catch {
        Write-Error "Node.js 未安裝，請先安裝 Node.js 18+"
        exit 1
    }
    
    # 檢查 npm
    try {
        $npmVersion = npm --version
        Write-Success "npm 已安裝: $npmVersion"
    }
    catch {
        Write-Error "npm 未安裝"
        exit 1
    }
}

# 安裝專案依賴
function Install-Dependencies {
    Write-Info "安裝專案依賴..."
    
    # 後端依賴
    Write-Info "安裝後端依賴..."
    dotnet restore
    Write-Success "後端依賴安裝完成"
    
    # 前端依賴
    Write-Info "安裝前端依賴..."
    Push-Location "..\frontend"
    try {
        npm install
        Write-Success "前端依賴安裝完成"
    }
    catch {
        Write-Warning "前端依賴安裝失敗，嘗試使用 --legacy-peer-deps..."
        npm install --legacy-peer-deps
        Write-Success "前端依賴安裝完成 (使用 legacy peer deps)"
    }
    Pop-Location
}

# 設定環境變數
function Set-Environment {
    Write-Info "設定環境變數..."
    
    if (-not (Test-Path ".env")) {
        if (Test-Path "..\env.example") {
            Copy-Item "..\env.example" ".env"
            Write-Success "已建立 .env 檔案，請編輯填入實際值"
        }
        else {
            Write-Warning "找不到 env.example 檔案"
        }
    }
    else {
        Write-Info ".env 檔案已存在"
    }
}

# 檢查資料庫連線
function Test-Database {
    Write-Info "檢查資料庫設定..."
    
    # 載入環境變數
    if (Test-Path ".env") {
        Get-Content ".env" | ForEach-Object {
            if ($_ -match '^([^=]+)=(.*)$') {
                [System.Environment]::SetEnvironmentVariable($matches[1], $matches[2])
            }
        }
    }
    
    $dbProvider = $env:DATABASE_PROVIDER
    if (-not $dbProvider) {
        $dbProvider = "PostgreSQL"
    }
    
    Write-Info "使用資料庫提供者: $dbProvider"
    
    switch ($dbProvider) {
        "PostgreSQL" {
            if (Get-Command psql -ErrorAction SilentlyContinue) {
                Write-Success "PostgreSQL 客戶端已安裝"
                
                # 測試連線（可選）
                if ($env:DATABASE_HOST -and $env:DATABASE_USERNAME) {
                    Write-Info "測試 PostgreSQL 連線..."
                    try {
                        $env:PGPASSWORD = $env:DATABASE_PASSWORD
                        psql -h $env:DATABASE_HOST -U $env:DATABASE_USERNAME -d postgres -c "SELECT 1;" | Out-Null
                        Write-Success "PostgreSQL 連線測試成功"
                    }
                    catch {
                        Write-Warning "PostgreSQL 連線測試失敗，請檢查設定"
                    }
                }
            }
            else {
                Write-Warning "PostgreSQL 客戶端未安裝"
            }
        }
        "SqlServer" {
            if (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
                Write-Success "SQL Server 客戶端已安裝"
            }
            else {
                Write-Warning "SQL Server 客戶端未安裝"
            }
        }
        default {
            Write-Error "不支援的資料庫提供者: $dbProvider"
            exit 1
        }
    }
}

# 執行 Migration
function Invoke-Migrations {
    Write-Info "執行資料庫 Migration..."
    
    # 檢查 Migration 列表
    Write-Info "檢查 Migration 列表..."
    dotnet ef migrations list --no-build
    
    # 執行 Migration
    Write-Info "更新資料庫結構..."
    dotnet ef database update
    Write-Success "資料庫 Migration 完成"
}

# 編譯專案
function Build-Project {
    Write-Info "編譯專案..."
    
    # 後端編譯
    dotnet build
    Write-Success "後端編譯成功"
    
    # 前端編譯測試
    Push-Location "..\frontend"
    try {
        npm run build | Out-Null
        Write-Success "前端編譯測試成功"
    }
    catch {
        Write-Warning "前端編譯測試跳過"
    }
    Pop-Location
}

# 驗證安裝
function Test-Installation {
    Write-Info "驗證安裝..."
    
    # 檢查關鍵檔案
    $files = @(
        "SmartNameplate.Api.csproj",
        "..\frontend\package.json",
        "..\frontend\angular.json",
        "appsettings.json",
        "appsettings.Development.json"
    )
    
    foreach ($file in $files) {
        if (Test-Path $file) {
            Write-Success "檔案存在: $file"
        }
        else {
            Write-Error "檔案缺失: $file"
        }
    }
}

# 主執行流程
function Main {
    Write-Host "🚀 智慧桌牌系統 - 新電腦部署開始" -ForegroundColor Cyan
    Write-Host "==================================" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Info "開始執行部署流程..."
    Write-Host ""
    
    Test-Requirements
    Write-Host ""
    
    Install-Dependencies
    Write-Host ""
    
    Set-Environment
    Write-Host ""
    
    Test-Database
    Write-Host ""
    
    Build-Project
    Write-Host ""
    
    # 詢問是否執行 Migration
    if (-not $SkipMigration) {
        $response = Read-Host "是否要執行資料庫 Migration? (y/N)"
        if ($response -eq "y" -or $response -eq "Y") {
            Invoke-Migrations
        }
        else {
            Write-Warning "跳過 Migration，請稍後手動執行: dotnet ef database update"
        }
        Write-Host ""
    }
    
    Test-Installation
    Write-Host ""
    
    Write-Success "部署完成！"
    Write-Host ""
    Write-Host "🎉 接下來的步驟:" -ForegroundColor Cyan
    Write-Host "1. 編輯 .env 檔案，填入實際的資料庫連線資訊"
    Write-Host "2. 如果跳過了 Migration，請執行: dotnet ef database update"
    Write-Host "3. 啟動後端: dotnet run --project SmartNameplate.Api.csproj --urls http://localhost:5001"
    Write-Host "4. 啟動前端: cd ..\frontend; npm start"
    Write-Host "5. 開啟瀏覽器訪問: http://localhost:4200"
    Write-Host ""
}

# 執行主程式
try {
    Main
}
catch {
    Write-Error "部署過程發生錯誤: $_"
    exit 1
} 