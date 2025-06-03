# 🗄️ SmartNameplate 通用資料庫架構指南

## 🎯 **概述**

SmartNameplate 系統現在支援**雙資料庫架構**，可在 PostgreSQL 和 SQL Server 之間無縫切換。

## 📋 **支援的資料庫**

| 資料庫 | 用途 | 優勢 |
|--------|------|------|
| **PostgreSQL (本地)** | 開發環境 | 免費、JSONB 支援、全文搜尋 |
| **PostgreSQL (Neon)** | 雲端部署 | 無伺服器、自動縮放、備份 |
| **SQL Server** | 企業環境 | 企業級功能、整合 Microsoft 生態系 |

---

## 🚀 **快速開始**

### 1️⃣ **切換資料庫**
```bash
# 使用互動式切換工具
./switch_database.sh

# 或手動編輯 appsettings.json
```

### 2️⃣ **生成架構**
```bash
# 生成所有架構
cd tools
npm run generate-schemas

# 僅生成特定資料庫
npm run generate-postgresql
npm run generate-sqlserver
```

---

## ⚙️ **配置說明**

### 📁 **appsettings.json 配置**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=smart_nameplate;Username=postgres;Password=password",
    "NeonConnection": "postgresql://...",
    "SqlServerConnection": "Server=localhost;Database=SmartNameplateDB;Trusted_Connection=true;"
  },
  "Database": {
    "Provider": "PostgreSQL",
    "UseNeon": false,        // true = 使用 Neon 雲端
    "UseSqlServer": false    // true = 使用 SQL Server
  }
}
```

### 🔀 **切換邏輯**

| UseNeon | UseSqlServer | 結果 |
|---------|--------------|------|
| false   | false        | PostgreSQL (本地) |
| true    | false        | PostgreSQL (Neon) |
| false   | true         | SQL Server |

---

## 🗂️ **資料庫差異處理**

### 📊 **資料類型對應**

| 功能 | PostgreSQL | SQL Server |
|------|------------|------------|
| **主鍵** | `SERIAL PRIMARY KEY` | `INT IDENTITY(1,1) PRIMARY KEY` |
| **JSON** | `JSONB` | `NVARCHAR(MAX)` |
| **文字** | `TEXT` | `NVARCHAR(MAX)` |
| **字串** | `VARCHAR(100)` | `NVARCHAR(100)` |
| **布林** | `BOOLEAN` | `BIT` |
| **時間** | `TIMESTAMP WITH TIME ZONE` | `DATETIME2` |

### 🔧 **自動處理機制**

系統使用 **DatabaseProvider** 抽象層自動處理：

```csharp
// 自動選擇適當的 JSON 欄位類型
entity.Property(e => e.ContentA)
    .HasColumnType(_databaseConfig.Provider.GetJsonColumnType());

// PostgreSQL: "jsonb"
// SQL Server: "nvarchar(max)"
```

---

## 📦 **部署選項**

### 🐘 **PostgreSQL 部署**

#### **本地開發**
```bash
# 1. 安裝 PostgreSQL
brew install postgresql  # macOS
sudo apt-get install postgresql  # Ubuntu

# 2. 建立資料庫
createdb -U postgres smart_nameplate

# 3. 匯入架構
psql -U postgres -d smart_nameplate < database-schemas/postgresql_schema.sql

# 4. 啟動應用
./switch_database.sh  # 選擇選項 1
cd backend && dotnet run
```

#### **Neon 雲端**
```bash
# 1. 在 Neon 建立專案
# 2. 更新 NeonConnection 連接字串
# 3. 切換設定
./switch_database.sh  # 選擇選項 2
```

### 🗄️ **SQL Server 部署**

#### **本地安裝**
```bash
# 1. 安裝 SQL Server
# https://www.microsoft.com/sql-server/

# 2. 建立資料庫
sqlcmd -S localhost -Q "CREATE DATABASE SmartNameplateDB"

# 3. 匯入架構
sqlcmd -S localhost -d SmartNameplateDB -i database-schemas/sqlserver_schema.sql

# 4. 切換設定
./switch_database.sh  # 選擇選項 3
```

#### **Azure SQL**
```bash
# 1. 建立 Azure SQL Database
# 2. 更新 SqlServerConnection 連接字串
# 3. 匯入架構並切換設定
```

---

## 🔄 **遷移策略**

### 📥 **從 PostgreSQL 遷移到 SQL Server**

```bash
# 1. 匯出 PostgreSQL 資料
pg_dump -h localhost -U postgres smart_nameplate > postgres_data.sql

# 2. 轉換資料格式（手動或使用工具）
# 3. 匯入到 SQL Server
# 4. 切換配置
./switch_database.sh
```

### 📤 **從 SQL Server 遷移到 PostgreSQL**

```bash
# 1. 使用 SQL Server Migration Assistant (SSMA)
# 2. 或手動匯出/轉換資料
# 3. 匯入到 PostgreSQL
# 4. 切換配置
```

---

## 🛠️ **開發工具**

### 📋 **可用腳本**

```bash
cd tools

# 生成通用架構
npm run generate-schemas

# 提取縮略圖（需要 PostgreSQL）
npm run extract-thumbnails

# 修復邊界元素（通用）
npm run fix-boundary
```

### 🗂️ **生成的檔案**

```
database-schemas/
├── postgresql_schema.sql    # PostgreSQL 架構
├── sqlserver_schema.sql     # SQL Server 架構
├── database_config.json     # 配置範例
└── README.md               # 詳細說明
```

---

## 🚨 **注意事項**

### ⚠️ **重要提醒**

1. **JSON 欄位**：PostgreSQL 使用 JSONB，SQL Server 使用 NVARCHAR(MAX)
2. **資料轉換**：切換資料庫時需要轉換現有資料
3. **功能差異**：某些 PostgreSQL 特定功能（如數組）在 SQL Server 中不可用
4. **效能**：PostgreSQL 的 JSONB 查詢通常比 SQL Server 的 JSON 字串快

### 💡 **最佳實踐**

1. **開發環境**：使用 PostgreSQL (本地)
2. **測試環境**：使用 PostgreSQL (Neon)  
3. **生產環境**：根據需求選擇
4. **資料備份**：定期備份，尤其在切換前

---

## 🆘 **故障排除**

### 🔧 **常見問題**

#### **連接失敗**
```bash
# 檢查資料庫是否運行
pg_isready  # PostgreSQL
sqlcmd -S localhost -Q "SELECT 1"  # SQL Server

# 檢查連接字串
grep -A 5 "ConnectionStrings" backend/appsettings.json
```

#### **架構不匹配**
```bash
# 重新生成架構
cd tools && npm run generate-schemas

# 檢查資料庫版本
SELECT version();  -- PostgreSQL
SELECT @@VERSION;  -- SQL Server
```

#### **JSON 欄位錯誤**
- PostgreSQL: 確保使用 `JSONB` 類型
- SQL Server: 確保使用 `NVARCHAR(MAX)` 並處理 JSON 字串

---

## 📞 **支援聯絡**

遇到問題時：
1. 檢查本指南的故障排除部分
2. 查看 `database-schemas/README.md`
3. 檢查應用程式日誌
4. 確認資料庫服務狀態

---

## 🎉 **總結**

SmartNameplate 現在具備**完整的多資料庫支援**：

✅ **自動類型對應**  
✅ **一鍵切換**  
✅ **通用架構生成**  
✅ **完整的遷移工具**  

選擇最適合您環境的資料庫，享受無縫的開發體驗！ 