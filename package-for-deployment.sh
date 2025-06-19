#!/bin/bash

# 🤖 智慧桌牌系統 - 專案打包腳本
# 用於準備專案移轉到新電腦

set -e

# 顏色定義
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

echo "📦 智慧桌牌系統 - 專案打包準備"
echo "================================"

# 專案目錄
PROJECT_NAME="SmartNameplate-deployment-$(date +%Y%m%d)"
PROJECT_DIR="../$PROJECT_NAME"

log_info "準備打包專案..."

# 清理建置檔案
log_info "清理建置檔案..."
cd backend
dotnet clean >/dev/null 2>&1 || true

cd ../frontend
if [ -d "node_modules" ]; then
    log_warning "node_modules 存在，複製時將自動排除"
fi

if [ -d "dist" ]; then
    rm -rf dist 2>/dev/null || true
fi

# 返回專案根目錄
cd ..

# 創建部署目錄
log_info "創建部署目錄: $PROJECT_NAME"
if [ -d "$PROJECT_DIR" ]; then
    rm -rf "$PROJECT_DIR"
fi
mkdir -p "$PROJECT_DIR"

# 複製專案檔案
log_info "複製專案檔案..."

# 複製後端
log_info "複製後端檔案..."
cp -r backend "$PROJECT_DIR/"

# 複製前端 (排除 node_modules)
log_info "複製前端檔案..."
rsync -av --exclude='node_modules' --exclude='dist' --exclude='.angular' frontend/ "$PROJECT_DIR/frontend/" 2>/dev/null || cp -r frontend "$PROJECT_DIR/"

# 複製根目錄檔案
log_info "複製設定檔案..."
cp README.md "$PROJECT_DIR/" 2>/dev/null || true
cp env.example "$PROJECT_DIR/" 2>/dev/null || true

# 清理部署目錄中的不需要檔案
log_info "清理不需要的檔案..."
cd "$PROJECT_DIR"

# 清理後端
if [ -d "backend/bin" ]; then
    rm -rf backend/bin
fi
if [ -d "backend/obj" ]; then
    rm -rf backend/obj
fi
if [ -d "backend/temp_downloads" ]; then
    rm -rf backend/temp_downloads
fi

# 清理前端
if [ -d "frontend/node_modules" ]; then
    rm -rf frontend/node_modules
fi
if [ -d "frontend/dist" ]; then
    rm -rf frontend/dist
fi
if [ -d "frontend/.angular" ]; then
    rm -rf frontend/.angular
fi

# 清理敏感檔案
find . -name ".env" -delete 2>/dev/null || true
find . -name "*.log" -delete 2>/dev/null || true
find . -name ".DS_Store" -delete 2>/dev/null || true
find . -name "Thumbs.db" -delete 2>/dev/null || true

cd ..

# 創建部署說明檔案
log_info "創建部署說明檔案..."
mkdir -p "$PROJECT_DIR"
cat > "$PROJECT_DIR/DEPLOYMENT-GUIDE.md" << 'EOF'
# 🚀 智慧桌牌系統 - 新電腦部署指南

## 📋 快速部署

### 🤖 自動化部署 (推薦)

```bash
# macOS/Linux
cd backend
chmod +x setup-new-computer.sh
./setup-new-computer.sh

# Windows PowerShell
cd backend
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
.\setup-new-computer.ps1
```

### 🔍 檢查資料庫連線

```bash
# macOS/Linux
cd backend
./check-database.sh
```

## 📖 詳細說明

詳細的部署指南請參考：
- `backend/deploy-instructions.md` - 完整部署說明
- `README.md` - 專案總覽

## ⚡ 快速啟動

1. **環境需求**:
   - Node.js 18+
   - .NET SDK 8+
   - PostgreSQL 14+ 或 SQL Server 2019+

2. **資料庫**:
   - 選擇 PostgreSQL 或 SQL Server
   - 創建資料庫：`smart_nameplate`

3. **啟動**:
   ```bash
   # 後端
   cd backend
   dotnet run --project SmartNameplate.Api.csproj --urls http://localhost:5001
   
   # 前端 (新終端)
   cd frontend
   npm start
   ```

4. **訪問**:
   - 前端：http://localhost:4200
   - API：http://localhost:5001/api

## 🚨 常見問題

- **依賴衝突**: 使用 `npm install --legacy-peer-deps`
- **端口佔用**: 使用 `lsof -i :5001` 檢查並終止進程
- **Migration**: 執行 `dotnet ef database update`

## 📞 支援

如遇問題，請檢查：
1. 環境變數設定是否正確
2. 資料庫服務是否啟動
3. 防火牆是否阻擋端口
EOF

# 創建檔案清單
log_info "創建檔案清單..."
find "$PROJECT_DIR" -type f > "$PROJECT_DIR/FILES-LIST.txt"

# 計算專案大小
PROJECT_SIZE=$(du -sh "$PROJECT_DIR" | cut -f1)

log_success "專案打包完成！"
echo
echo "📁 部署包位置: $PROJECT_DIR"
echo "📏 專案大小: $PROJECT_SIZE"
echo
echo "📋 部署包內容:"
echo "├── backend/                # ASP.NET Core API"
echo "├── frontend/               # Angular 應用"
echo "├── README.md               # 專案說明"
echo "├── env.example             # 環境變數範例"
echo "├── DEPLOYMENT-GUIDE.md     # 部署指南"
echo "└── FILES-LIST.txt          # 檔案清單"
echo
echo "🚀 下一步:"
echo "1. 將 '$PROJECT_DIR' 資料夾複製到新電腦"
echo "2. 在新電腦執行: cd $PROJECT_NAME/backend && ./setup-new-computer.sh"
echo "3. 或參考 DEPLOYMENT-GUIDE.md 進行手動部署"
echo

# 可選：創建壓縮檔案
read -p "是否要創建 ZIP 壓縮檔案? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    log_info "創建壓縮檔案..."
    zip -r "$PROJECT_NAME.zip" "$PROJECT_DIR" >/dev/null
    ZIP_SIZE=$(du -sh "$PROJECT_NAME.zip" | cut -f1)
    log_success "壓縮檔案已創建: $PROJECT_NAME.zip ($ZIP_SIZE)"
fi

echo
log_success "打包作業完成！" 