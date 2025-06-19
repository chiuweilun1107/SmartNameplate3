# 🤖 資料庫操作指令

## 📊 支援的資料庫

本系統支援兩種主要資料庫，地位相等：
- **PostgreSQL** (預設)
- **SQL Server**

## ⚙️ 切換資料庫提供者

### 方法 1: 修改設定檔案
```json
// appsettings.json 或 appsettings.Development.json
{
  "Database": {
    "Provider": "PostgreSQL"  // 或 "SqlServer"
  }
}
```

### 方法 2: 環境變數
```bash
export DATABASE_PROVIDER=PostgreSQL
# 或
export DATABASE_PROVIDER=SqlServer
```

## 🗄️ Migration 指令

### PostgreSQL Migration
```bash
# 添加新的 Migration
dotnet ef migrations add InitialCreate --context ApplicationDbContext

# 更新資料庫
dotnet ef database update --context ApplicationDbContext

# 移除最後一個 Migration
dotnet ef migrations remove --context ApplicationDbContext
```

### SQL Server Migration
```bash
# 設定 SQL Server 環境變數
export DATABASE_PROVIDER=SqlServer

# 添加新的 Migration (會自動使用 SQL Server)
dotnet ef migrations add InitialCreate_SqlServer --context ApplicationDbContext

# 更新 SQL Server 資料庫
dotnet ef database update --context ApplicationDbContext
```

## 🔄 資料庫連線字串

### PostgreSQL
```bash
# 環境變數
DATABASE_HOST=localhost
DATABASE_NAME=smart_nameplate
DATABASE_USERNAME=postgres
DATABASE_PASSWORD=your_password
```

### SQL Server
```bash
# 環境變數
SQL_SERVER_HOST=localhost
SQL_DATABASE_NAME=smart_nameplate
SQL_USERNAME=sa
SQL_PASSWORD=your_password
```

## 🚀 啟動指令

### 使用 PostgreSQL
```bash
export DATABASE_PROVIDER=PostgreSQL
dotnet run --project backend/SmartNameplate.Api.csproj --urls http://localhost:5001
```

### 使用 SQL Server
```bash
export DATABASE_PROVIDER=SqlServer
dotnet run --project backend/SmartNameplate.Api.csproj --urls http://localhost:5001
```

## 🧪 測試不同資料庫

在開發環境中，您可以輕鬆切換資料庫來測試：

```bash
# 測試 PostgreSQL
export DATABASE_PROVIDER=PostgreSQL
dotnet ef database update
dotnet run

# 測試 SQL Server
export DATABASE_PROVIDER=SqlServer
dotnet ef database update
dotnet run
``` 