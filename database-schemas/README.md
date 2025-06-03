# SmartNameplate 通用資料庫架構

## 🎯 概述

本資料夾包含 SmartNameplate 系統的通用資料庫架構，支援 PostgreSQL 和 SQL Server。

## 📁 檔案說明

- `postgresql_schema.sql` - PostgreSQL 資料庫架構
- `sqlserver_schema.sql` - SQL Server 資料庫架構  
- `database_config.json` - 資料庫配置範例
- `README.md` - 本說明文件

## 🚀 使用方式

### PostgreSQL
```bash
psql -U postgres -d smart_nameplate < postgresql_schema.sql
```

### SQL Server
```bash
sqlcmd -S localhost -i sqlserver_schema.sql
```

## ⚙️ 配置

在 `appsettings.json` 中設定：

```json
{
  "Database": {
    "UseSqlServer": false,  // 設為 true 使用 SQL Server
    "UseNeon": false        // 設為 true 使用 Neon 雲端 PostgreSQL
  }
}
```

## 🗂️ 資料表結構

系統包含以下核心表格：
- Users - 用戶管理
- Groups - 群組管理
- Cards - 卡片管理
- Templates - 模板管理
- Devices - 設備管理

所有表格都包含適當的索引和外鍵約束。
