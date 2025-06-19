# 🚀 智慧桌牌系統 - 新電腦部署指南

## 📋 **移轉前準備**

### 在原電腦上執行
```bash
# 1. 清理建置檔案
dotnet clean
cd frontend && npm run clean || rm -rf node_modules
cd ..

# 2. 確認所有檔案已提交到版本控制
git add .
git commit -m "準備移轉到新電腦"

# 3. 建立環境變數範本
cp .env .env.example.backup  # 如果有 .env 檔案
```

## 🖥️ **新電腦環境需求**

### 必要軟體安裝
- **Node.js**: 18.x 或更高版本
- **npm**: 9.x 或更高版本
- **.NET SDK**: 8.x 或更高版本
- **Git**: 最新版本

### 資料庫選擇（擇一安裝）
- **PostgreSQL**: 14+ (推薦)
- **SQL Server**: 2019+ 或 SQL Server Express

## 🗄️ **資料庫安裝指南**

### 選項 A: PostgreSQL (推薦)

#### macOS 安裝
```bash
# 使用 Homebrew
brew install postgresql@15
brew services start postgresql@15

# 建立資料庫
createdb smart_nameplate
```

#### Windows 安裝
```bash
# 下載並安裝 PostgreSQL
# https://www.postgresql.org/download/windows/

# 或使用 Chocolatey
choco install postgresql

# 建立資料庫
psql -U postgres -c "CREATE DATABASE smart_nameplate;"
```

#### Ubuntu/Linux 安裝
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql

# 建立資料庫
sudo -u postgres createdb smart_nameplate
```

### 選項 B: SQL Server

#### Windows 安裝
```bash
# 下載 SQL Server Express
# https://www.microsoft.com/en-us/sql-server/sql-server-downloads

# 或使用 winget
winget install Microsoft.SQLServer.2022.Express
```

#### macOS/Linux 安裝 (使用 Docker)
```bash
# 安裝 Docker Desktop
# 啟動 SQL Server 容器
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Dev123456!" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

## 📁 **專案部署步驟**

### 1. 克隆或複製專案
```bash
# 方法 A: Git 克隆
git clone <your-repository-url>
cd SmartNameplate2

# 方法 B: 直接複製檔案
# 將整個專案資料夾複製到新電腦
```

### 2. 安裝前端依賴
```bash
cd frontend
npm install
```

### 3. 安裝後端依賴
```bash
cd ../backend
dotnet restore
```

### 4. 設定環境變數

#### 建立 .env 檔案
```bash
# 複製範例檔案
cp ../env.example .env

# 編輯 .env 檔案，填入實際值
nano .env  # 或使用其他編輯器
```

#### PostgreSQL 設定範例
```env
DATABASE_PROVIDER=PostgreSQL
DATABASE_HOST=localhost
DATABASE_NAME=smart_nameplate
DATABASE_USERNAME=postgres
DATABASE_PASSWORD=your_postgres_password

JWT_SECRET_KEY=your_32_character_jwt_secret_key_here
JWT_ISSUER=SmartNameplate
JWT_AUDIENCE=SmartNameplateUsers

ENCRYPTION_KEY=your_32_character_encryption_key
ENCRYPTION_IV=your_16_char_iv
```

#### SQL Server 設定範例
```env
DATABASE_PROVIDER=SqlServer
SQL_SERVER_HOST=localhost
SQL_DATABASE_NAME=smart_nameplate
SQL_USERNAME=sa
SQL_PASSWORD=Dev123456!

JWT_SECRET_KEY=your_32_character_jwt_secret_key_here
JWT_ISSUER=SmartNameplate
JWT_AUDIENCE=SmartNameplateUsers

ENCRYPTION_KEY=your_32_character_encryption_key
ENCRYPTION_IV=your_16_char_iv
```

### 5. 建立資料庫結構

#### 載入環境變數
```bash
# macOS/Linux
source .env

# Windows PowerShell
Get-Content .env | ForEach-Object {
    if ($_ -match '^([^=]+)=(.*)$') {
        [System.Environment]::SetEnvironmentVariable($matches[1], $matches[2])
    }
}
```

#### 執行 Migration
```bash
# 檢查 Migration 列表
dotnet ef migrations list

# 更新資料庫結構
dotnet ef database update
```

### 6. 驗證安裝

#### 編譯測試
```bash
dotnet build
```

#### 啟動後端
```bash
dotnet run --project SmartNameplate.Api.csproj --urls http://localhost:5001
```

#### 啟動前端（新終端）
```bash
cd frontend
npm start
```

#### 驗證頁面
- 前端：http://localhost:4200
- API 文檔：http://localhost:5001/api

## 🔧 **常見問題排除**

### 資料庫連線問題
```bash
# 測試 PostgreSQL 連線
psql -h localhost -U postgres -d smart_nameplate -c "SELECT 1;"

# 測試 SQL Server 連線
sqlcmd -S localhost -U sa -P "Dev123456!" -Q "SELECT 1"
```

### 端口衝突處理
```bash
# 檢查端口佔用
lsof -i :5001  # macOS/Linux
netstat -ano | findstr :5001  # Windows

# 終止佔用進程
kill -9 <PID>  # macOS/Linux
taskkill /PID <PID> /F  # Windows
```

### Migration 錯誤
```bash
# 重設 Migration（僅開發環境）
dotnet ef database drop --force
dotnet ef database update
```

## 📊 **驗證清單**

- [ ] 資料庫服務正常運行
- [ ] 環境變數設定正確
- [ ] Migration 執行成功
- [ ] 後端編譯無錯誤
- [ ] 前端依賴安裝完成
- [ ] API 服務正常啟動 (port 5001)
- [ ] 前端服務正常啟動 (port 4200)
- [ ] Swagger API 文檔可訪問

## 🚨 **重要提醒**

1. **密鑰安全**: 確保 JWT 和加密密鑰至少 32 字元
2. **資料庫權限**: 確認資料庫使用者有建立表格權限
3. **防火牆設定**: 檢查本地防火牆是否阻擋端口
4. **版本相容**: 確認 .NET 和 Node.js 版本符合需求 