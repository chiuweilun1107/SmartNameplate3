#!/bin/bash

# 🚀 SmartNameplate 資料庫匯出腳本
echo "🚀 開始匯出 SmartNameplate 資料庫..."

# 設定變數
DB_NAME="smart_nameplate"
DB_USER="postgres"
BACKUP_FILE="smartnameplate_backup_$(date +%Y%m%d_%H%M%S).sql"
MIGRATION_DIR="smartnameplate_migration"

# 檢查 PostgreSQL 是否安裝
if ! command -v pg_dump &> /dev/null; then
    echo "❌ 錯誤：找不到 pg_dump 命令。請確認 PostgreSQL 已安裝。"
    exit 1
fi

# 建立遷移目錄
echo "📁 建立遷移目錄..."
mkdir -p "$MIGRATION_DIR"

# 匯出資料庫
echo "💾 匯出資料庫 $DB_NAME..."
pg_dump -h localhost -U "$DB_USER" "$DB_NAME" > "$MIGRATION_DIR/$BACKUP_FILE"

if [ $? -eq 0 ]; then
    echo "✅ 資料庫匯出成功：$MIGRATION_DIR/$BACKUP_FILE"
else
    echo "❌ 資料庫匯出失敗"
    exit 1
fi

# 複製專案文件
echo "📋 複製專案文件..."
cp -r backend "$MIGRATION_DIR/"
cp -r frontend "$MIGRATION_DIR/"
cp -r tools "$MIGRATION_DIR/"
cp -r .cursor "$MIGRATION_DIR/"
cp SmartNameplateC.sln "$MIGRATION_DIR/"
cp global.json "$MIGRATION_DIR/" 2>/dev/null || true
cp migration_package.md "$MIGRATION_DIR/"

# 建立還原腳本
echo "📝 建立還原腳本..."
cat > "$MIGRATION_DIR/restore_database.sh" << 'EOF'
#!/bin/bash

# 🔄 SmartNameplate 資料庫還原腳本
echo "🔄 開始還原 SmartNameplate 資料庫..."

DB_NAME="smart_nameplate"
DB_USER="postgres"

# 找到備份文件
BACKUP_FILE=$(ls smartnameplate_backup_*.sql | head -n 1)

if [ -z "$BACKUP_FILE" ]; then
    echo "❌ 找不到備份文件"
    exit 1
fi

echo "📁 使用備份文件：$BACKUP_FILE"

# 檢查 PostgreSQL 是否安裝
if ! command -v createdb &> /dev/null; then
    echo "❌ 錯誤：找不到 PostgreSQL 命令。請先安裝 PostgreSQL。"
    exit 1
fi

# 建立資料庫
echo "🗄️ 建立資料庫 $DB_NAME..."
createdb -U "$DB_USER" "$DB_NAME" 2>/dev/null || echo "資料庫可能已存在"

# 還原資料
echo "📥 還原資料到資料庫..."
psql -U "$DB_USER" -d "$DB_NAME" < "$BACKUP_FILE"

if [ $? -eq 0 ]; then
    echo "✅ 資料庫還原成功"
else
    echo "❌ 資料庫還原失敗"
    exit 1
fi

# 安裝後端依賴
echo "📦 安裝後端依賴..."
cd backend
dotnet restore
cd ..

# 安裝前端依賴
echo "🌐 安裝前端依賴..."
cd frontend
npm install
cd ..

# 安裝工具依賴
echo "🛠️ 安裝工具依賴..."
cd tools
npm install
cd ..

echo "🎉 遷移完成！"
echo ""
echo "啟動說明："
echo "1. 啟動後端：cd backend && dotnet run --project SmartNameplate.Api.csproj --urls http://localhost:5001"
echo "2. 啟動前端：cd frontend && ng serve --proxy-config proxy.conf.json"
echo "3. 訪問網站：http://localhost:4200"
EOF

chmod +x "$MIGRATION_DIR/restore_database.sh"

# 壓縮遷移包
echo "📦 建立遷移包壓縮檔..."
tar -czf "${MIGRATION_DIR}.tar.gz" "$MIGRATION_DIR"

echo ""
echo "🎉 遷移包建立完成！"
echo ""
echo "📁 遷移文件："
echo "   - 遷移目錄：$MIGRATION_DIR/"
echo "   - 壓縮檔：${MIGRATION_DIR}.tar.gz"
echo "   - 資料庫備份：$MIGRATION_DIR/$BACKUP_FILE"
echo ""
echo "📋 下一步："
echo "1. 將 ${MIGRATION_DIR}.tar.gz 複製到新設備"
echo "2. 解壓：tar -xzf ${MIGRATION_DIR}.tar.gz"
echo "3. 執行：cd $MIGRATION_DIR && ./restore_database.sh"
echo ""
echo "💡 詳細說明請參考：$MIGRATION_DIR/migration_package.md" 