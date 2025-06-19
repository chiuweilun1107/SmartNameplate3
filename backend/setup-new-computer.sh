#!/bin/bash

# 🤖 智慧桌牌系統 - 新電腦自動化部署腳本
# 使用方法: chmod +x setup-new-computer.sh && ./setup-new-computer.sh

set -e  # 遇到錯誤立即停止

echo "🚀 智慧桌牌系統 - 新電腦部署開始"
echo "=================================="

# 顏色定義
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 日誌函數
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

# 檢查系統環境
check_requirements() {
    log_info "檢查系統需求..."
    
    # 檢查 .NET SDK
    if command -v dotnet >/dev/null 2>&1; then
        DOTNET_VERSION=$(dotnet --version)
        log_success ".NET SDK 已安裝: $DOTNET_VERSION"
    else
        log_error ".NET SDK 未安裝，請先安裝 .NET 8 SDK"
        exit 1
    fi
    
    # 檢查 Node.js
    if command -v node >/dev/null 2>&1; then
        NODE_VERSION=$(node --version)
        log_success "Node.js 已安裝: $NODE_VERSION"
    else
        log_error "Node.js 未安裝，請先安裝 Node.js 18+"
        exit 1
    fi
    
    # 檢查 npm
    if command -v npm >/dev/null 2>&1; then
        NPM_VERSION=$(npm --version)
        log_success "npm 已安裝: $NPM_VERSION"
    else
        log_error "npm 未安裝"
        exit 1
    fi
}

# 安裝專案依賴
install_dependencies() {
    log_info "安裝專案依賴..."
    
    # 後端依賴
    log_info "安裝後端依賴..."
    dotnet restore
    log_success "後端依賴安裝完成"
    
    # 前端依賴
    log_info "安裝前端依賴..."
    cd ../frontend
    if npm install; then
        log_success "前端依賴安裝完成"
    else
        log_warning "前端依賴安裝失敗，嘗試使用 --legacy-peer-deps..."
        npm install --legacy-peer-deps
        log_success "前端依賴安裝完成 (使用 legacy peer deps)"
    fi
    cd ../backend
}

# 設定環境變數
setup_environment() {
    log_info "設定環境變數..."
    
    if [ ! -f .env ]; then
        if [ -f ../env.example ]; then
            cp ../env.example .env
            log_success "已建立 .env 檔案，請編輯填入實際值"
        else
            log_warning "找不到 env.example 檔案"
        fi
    else
        log_info ".env 檔案已存在"
    fi
}

# 檢查資料庫連線
check_database() {
    log_info "檢查資料庫設定..."
    
    # 載入環境變數
    if [ -f .env ]; then
        export $(grep -v '^#' .env | xargs)
    fi
    
    DB_PROVIDER=${DATABASE_PROVIDER:-PostgreSQL}
    log_info "使用資料庫提供者: $DB_PROVIDER"
    
    case $DB_PROVIDER in
        "PostgreSQL")
            if command -v psql >/dev/null 2>&1; then
                log_success "PostgreSQL 客戶端已安裝"
                # 測試連線（可選）
                if [ ! -z "$DATABASE_HOST" ] && [ ! -z "$DATABASE_USERNAME" ]; then
                    log_info "測試 PostgreSQL 連線..."
                    if PGPASSWORD="$DATABASE_PASSWORD" psql -h "$DATABASE_HOST" -U "$DATABASE_USERNAME" -d postgres -c "SELECT 1;" >/dev/null 2>&1; then
                        log_success "PostgreSQL 連線測試成功"
                    else
                        log_warning "PostgreSQL 連線測試失敗，請檢查設定"
                    fi
                fi
            else
                log_warning "PostgreSQL 客戶端未安裝"
            fi
            ;;
        "SqlServer")
            if command -v sqlcmd >/dev/null 2>&1; then
                log_success "SQL Server 客戶端已安裝"
            else
                log_warning "SQL Server 客戶端未安裝"
            fi
            ;;
        *)
            log_error "不支援的資料庫提供者: $DB_PROVIDER"
            exit 1
            ;;
    esac
}

# 執行 Migration
run_migrations() {
    log_info "執行資料庫 Migration..."
    
    # 檢查 Migration 列表
    log_info "檢查 Migration 列表..."
    dotnet ef migrations list --no-build
    
    # 執行 Migration
    log_info "更新資料庫結構..."
    dotnet ef database update
    log_success "資料庫 Migration 完成"
}

# 編譯測試
build_project() {
    log_info "編譯專案..."
    
    # 後端編譯
    dotnet build
    log_success "後端編譯成功"
    
    # 前端編譯測試
    cd ../frontend
    npm run build >/dev/null 2>&1 || log_warning "前端編譯測試跳過"
    cd ../backend
}

# 驗證安裝
verify_installation() {
    log_info "驗證安裝..."
    
    # 檢查關鍵檔案
    local files=(
        "SmartNameplate.Api.csproj"
        "../frontend/package.json"
        "../frontend/angular.json"
        "appsettings.json"
        "appsettings.Development.json"
    )
    
    for file in "${files[@]}"; do
        if [ -f "$file" ]; then
            log_success "檔案存在: $file"
        else
            log_error "檔案缺失: $file"
        fi
    done
}

# 主執行流程
main() {
    echo
    log_info "開始執行部署流程..."
    echo
    
    check_requirements
    echo
    
    install_dependencies
    echo
    
    setup_environment
    echo
    
    check_database
    echo
    
    build_project
    echo
    
    # 詢問是否執行 Migration
    read -p "是否要執行資料庫 Migration? (y/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        run_migrations
    else
        log_warning "跳過 Migration，請稍後手動執行: dotnet ef database update"
    fi
    echo
    
    verify_installation
    echo
    
    log_success "部署完成！"
    echo
    echo "🎉 接下來的步驟:"
    echo "1. 編輯 .env 檔案，填入實際的資料庫連線資訊"
    echo "2. 如果跳過了 Migration，請執行: dotnet ef database update"
    echo "3. 啟動後端: dotnet run --project SmartNameplate.Api.csproj --urls http://localhost:5001"
    echo "4. 啟動前端: cd ../frontend && npm start"
    echo "5. 開啟瀏覽器訪問: http://localhost:4200"
    echo
}

# 執行主程式
main "$@" 