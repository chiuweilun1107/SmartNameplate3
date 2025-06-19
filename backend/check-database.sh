#!/bin/bash

# 🤖 資料庫連線檢查腳本
# 用於快速驗證資料庫設定是否正確

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

echo "🔍 智慧桌牌系統 - 資料庫連線檢查"
echo "====================================="

# 載入環境變數
if [ -f .env ]; then
    export $(grep -v '^#' .env | xargs)
    log_success "已載入 .env 環境變數"
else
    log_error "找不到 .env 檔案"
    exit 1
fi

DB_PROVIDER=${DATABASE_PROVIDER:-PostgreSQL}
log_info "資料庫提供者: $DB_PROVIDER"

case $DB_PROVIDER in
    "PostgreSQL")
        log_info "檢查 PostgreSQL 設定..."
        
        # 檢查必要環境變數
        if [ -z "$DATABASE_HOST" ]; then
            log_error "DATABASE_HOST 未設定"
            exit 1
        fi
        
        if [ -z "$DATABASE_USERNAME" ]; then
            log_error "DATABASE_USERNAME 未設定"
            exit 1
        fi
        
        if [ -z "$DATABASE_NAME" ]; then
            log_error "DATABASE_NAME 未設定"
            exit 1
        fi
        
        log_success "PostgreSQL 環境變數檢查通過"
        
        # 測試連線
        log_info "測試 PostgreSQL 連線..."
        if command -v psql >/dev/null 2>&1; then
            if PGPASSWORD="$DATABASE_PASSWORD" psql -h "$DATABASE_HOST" -U "$DATABASE_USERNAME" -d "$DATABASE_NAME" -c "SELECT version();" >/dev/null 2>&1; then
                log_success "PostgreSQL 連線成功"
                
                # 檢查資料表
                log_info "檢查資料表..."
                TABLE_COUNT=$(PGPASSWORD="$DATABASE_PASSWORD" psql -h "$DATABASE_HOST" -U "$DATABASE_USERNAME" -d "$DATABASE_NAME" -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';" 2>/dev/null || echo "0")
                log_info "資料表數量: $TABLE_COUNT"
                
                if [ "$TABLE_COUNT" -gt "0" ]; then
                    log_success "資料庫結構已建立"
                else
                    log_warning "資料庫為空，需要執行 Migration"
                fi
            else
                log_error "PostgreSQL 連線失敗"
                log_info "檢查項目:"
                log_info "1. PostgreSQL 服務是否啟動"
                log_info "2. 連線資訊是否正確"
                log_info "3. 資料庫是否存在"
                log_info "4. 使用者權限是否足夠"
                exit 1
            fi
        else
            log_error "psql 命令未找到，請安裝 PostgreSQL 客戶端"
            exit 1
        fi
        ;;
        
    "SqlServer")
        log_info "檢查 SQL Server 設定..."
        
        # 檢查必要環境變數
        if [ -z "$SQL_SERVER_HOST" ]; then
            log_error "SQL_SERVER_HOST 未設定"
            exit 1
        fi
        
        if [ -z "$SQL_USERNAME" ]; then
            log_error "SQL_USERNAME 未設定"
            exit 1
        fi
        
        if [ -z "$SQL_DATABASE_NAME" ]; then
            log_error "SQL_DATABASE_NAME 未設定"
            exit 1
        fi
        
        log_success "SQL Server 環境變數檢查通過"
        
        # 測試連線
        log_info "測試 SQL Server 連線..."
        if command -v sqlcmd >/dev/null 2>&1; then
            if sqlcmd -S "$SQL_SERVER_HOST" -U "$SQL_USERNAME" -P "$SQL_PASSWORD" -d "$SQL_DATABASE_NAME" -Q "SELECT @@VERSION" >/dev/null 2>&1; then
                log_success "SQL Server 連線成功"
                
                # 檢查資料表
                log_info "檢查資料表..."
                TABLE_COUNT=$(sqlcmd -S "$SQL_SERVER_HOST" -U "$SQL_USERNAME" -P "$SQL_PASSWORD" -d "$SQL_DATABASE_NAME" -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h -1 2>/dev/null | tr -d ' ' || echo "0")
                log_info "資料表數量: $TABLE_COUNT"
                
                if [ "$TABLE_COUNT" -gt "0" ]; then
                    log_success "資料庫結構已建立"
                else
                    log_warning "資料庫為空，需要執行 Migration"
                fi
            else
                log_error "SQL Server 連線失敗"
                log_info "檢查項目:"
                log_info "1. SQL Server 服務是否啟動"
                log_info "2. 連線資訊是否正確"
                log_info "3. 資料庫是否存在"
                log_info "4. 使用者權限是否足夠"
                exit 1
            fi
        else
            log_error "sqlcmd 命令未找到，請安裝 SQL Server 客戶端工具"
            exit 1
        fi
        ;;
        
    *)
        log_error "不支援的資料庫提供者: $DB_PROVIDER"
        exit 1
        ;;
esac

# 檢查 Migration 狀態
log_info "檢查 Migration 狀態..."
if dotnet ef migrations list --no-build >/dev/null 2>&1; then
    MIGRATION_COUNT=$(dotnet ef migrations list --no-build | grep -c "^[0-9]" || echo "0")
    log_info "Migration 數量: $MIGRATION_COUNT"
    
    if [ "$MIGRATION_COUNT" -gt "0" ]; then
        log_success "Migration 檔案已就緒"
    else
        log_warning "未發現 Migration 檔案"
    fi
else
    log_error "無法檢查 Migration 狀態"
fi

echo
log_success "資料庫檢查完成！"
echo
echo "📋 檢查摘要:"
echo "• 資料庫提供者: $DB_PROVIDER"
echo "• 連線狀態: 正常"
echo "• 資料表數量: $TABLE_COUNT"
echo "• Migration 數量: $MIGRATION_COUNT"
echo
echo "🚀 後續步驟:"
if [ "$TABLE_COUNT" -eq "0" ]; then
    echo "執行 Migration 建立資料庫結構:"
    echo "dotnet ef database update"
else
    echo "資料庫已就緒，可以啟動應用程式:"
    echo "dotnet run --project SmartNameplate.Api.csproj --urls http://localhost:5001"
fi
echo 