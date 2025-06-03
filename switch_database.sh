#!/bin/bash

# 🔄 SmartNameplate 資料庫切換工具
echo "🔄 SmartNameplate 資料庫切換工具"

# 顯示目前配置
echo ""
echo "📋 目前配置："
grep -A 5 '"Database"' backend/appsettings.json

echo ""
echo "🎯 選擇資料庫提供者："
echo "1) PostgreSQL (本地)"
echo "2) PostgreSQL (Neon 雲端)"
echo "3) SQL Server"
echo "4) 還原預設值"

read -p "請選擇 (1-4): " choice

case $choice in
    1)
        echo "🐘 切換到 PostgreSQL (本地)..."
        # 更新 appsettings.json
        sed -i '' 's/"UseNeon": true/"UseNeon": false/g' backend/appsettings.json
        sed -i '' 's/"UseSqlServer": true/"UseSqlServer": false/g' backend/appsettings.json
        echo "✅ 已切換到 PostgreSQL (本地)"
        echo "📋 連接字串: DefaultConnection"
        ;;
    2)
        echo "☁️ 切換到 PostgreSQL (Neon 雲端)..."
        sed -i '' 's/"UseNeon": false/"UseNeon": true/g' backend/appsettings.json
        sed -i '' 's/"UseSqlServer": true/"UseSqlServer": false/g' backend/appsettings.json
        echo "✅ 已切換到 PostgreSQL (Neon 雲端)"
        echo "📋 連接字串: NeonConnection"
        ;;
    3)
        echo "🗄️ 切換到 SQL Server..."
        sed -i '' 's/"UseNeon": true/"UseNeon": false/g' backend/appsettings.json
        sed -i '' 's/"UseSqlServer": false/"UseSqlServer": true/g' backend/appsettings.json
        echo "✅ 已切換到 SQL Server"
        echo "📋 連接字串: SqlServerConnection"
        echo ""
        echo "⚠️ 注意：請確保已安裝 SQL Server 並建立資料庫"
        echo "💡 建立 SQL Server 資料庫："
        echo "   sqlcmd -S localhost -Q \"CREATE DATABASE SmartNameplateDB\""
        echo "   sqlcmd -S localhost -d SmartNameplateDB -i database-schemas/sqlserver_schema.sql"
        ;;
    4)
        echo "🔄 還原預設配置..."
        sed -i '' 's/"UseNeon": true/"UseNeon": false/g' backend/appsettings.json
        sed -i '' 's/"UseSqlServer": true/"UseSqlServer": false/g' backend/appsettings.json
        echo "✅ 已還原為 PostgreSQL (本地)"
        ;;
    *)
        echo "❌ 無效選擇"
        exit 1
        ;;
esac

echo ""
echo "📋 更新後配置："
grep -A 5 '"Database"' backend/appsettings.json

echo ""
echo "🚀 重啟應用程式以套用更改："
echo "   cd backend && dotnet run --project SmartNameplate.Api.csproj --urls http://localhost:5001" 