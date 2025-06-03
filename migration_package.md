# 🚀 SmartNameplate 系統遷移包

## 📋 **遷移方案選擇**

### 🎯 **快速遷移（PostgreSQL → PostgreSQL）**
適用於：同類型系統間的快速遷移

### 🎯 **升級遷移（PostgreSQL → SQL Server）**
適用於：企業級部署或效能優化需求

---

## 📦 **遷移包內容**

### 📁 **核心文件**
- `SmartNameplateC/` - 完整專案目錄
- `SmartNameplateC.sln` - Visual Studio 解決方案文件 ⚠️ **必須保留**
- `package.json` & `package-lock.json` - 前端依賴

### 📁 **資料庫遷移文件**
- `.cursor/rules/SmartNameplate_SQLServer_Schema.sql` - SQL Server 完整架構
- `.cursor/rules/Database_Migration_Guide.md` - 詳細遷移指南
- `.cursor/rules/Database_Comparison.sql` - 驗證腳本

### 📁 **配置文件**
- `backend/appsettings.json` - 後端配置
- `backend/appsettings.Development.json` - 開發環境配置
- `frontend/proxy.conf.json` - 前端代理配置

---

## 🚀 **方案A：PostgreSQL 遷移步驟**

### 1️⃣ **匯出目前資料庫**
```bash
# 在目前設備上執行
pg_dump -h localhost -U postgres smart_nameplate > smartnameplate_backup.sql
```

### 2️⃣ **準備遷移文件**
```bash
# 建立遷移包
mkdir smartnameplate_migration
cp -r SmartNameplateC smartnameplate_migration/
cp smartnameplate_backup.sql smartnameplate_migration/
```

### 3️⃣ **在新設備上還原**
```bash
# 安裝 PostgreSQL
# 建立資料庫
createdb -U postgres smart_nameplate

# 還原資料
psql -U postgres -d smart_nameplate < smartnameplate_backup.sql

# 安裝後端依賴
cd SmartNameplateC/backend
dotnet restore

# 安裝前端依賴
cd ../frontend
npm install
```

### 4️⃣ **啟動系統**
```bash
# 啟動後端
cd backend
dotnet run --project SmartNameplate.Api.csproj --urls http://localhost:5001

# 啟動前端
cd frontend
ng serve --proxy-config proxy.conf.json
```

---

## 🎯 **方案B：SQL Server 遷移步驟**

### 1️⃣ **準備 SQL Server 環境**
- 安裝 SQL Server 2019+ 或 SQL Server Express
- 安裝 SQL Server Management Studio (SSMS)

### 2️⃣ **執行架構腳本**
```sql
-- 在 SSMS 中執行
-- 檔案：.cursor/rules/SmartNameplate_SQLServer_Schema.sql
```

### 3️⃣ **更新連接字串**
```json
// backend/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartNameplateDB;Trusted_Connection=true;TrustServerCertificate=true;",
    "SqlServerConnection": "Server=localhost;Database=SmartNameplateDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
  },
  "Database": {
    "Provider": "SqlServer",
    "UseSqlServer": true,
    "UseNeon": false
  }
}
```

### 4️⃣ **更新 NuGet 套件**
```bash
# 移除 PostgreSQL 套件
dotnet remove package Npgsql.EntityFrameworkCore.PostgreSQL

# 安裝 SQL Server 套件
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### 5️⃣ **更新程式碼**
```csharp
// Program.cs 更新
var connectionString = builder.Configuration.GetValue<bool>("Database:UseSqlServer")
    ? builder.Configuration.GetConnectionString("SqlServerConnection")
    : builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)); // 改為 UseSqlServer
```

---

## ✅ **驗證遷移成功**

### 🔍 **資料庫驗證**
```sql
-- 檢查表格數量
SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
-- 應該回傳 13

-- 檢查預設用戶
SELECT Id, UserName, Role FROM Users;
-- 應該有 5 個用戶
```

### 🔍 **API 驗證**
```bash
# 測試 API 連接
curl http://localhost:5001/api/Users

# 測試登入
curl -X POST "http://localhost:5001/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "hamaadmin", "password": "hamaadmin*"}'
```

### 🔍 **前端驗證**
- 訪問 http://localhost:4200
- 確認登入功能正常
- 檢查卡片和群組功能

---

## 📂 **遷移檢查清單**

### ✅ **遷移前檢查**
- [ ] 備份目前資料庫
- [ ] 確認所有文件完整
- [ ] 記錄目前系統配置

### ✅ **遷移中檢查**
- [ ] 資料庫架構建立成功
- [ ] 預設資料插入完成
- [ ] 依賴套件安裝成功
- [ ] 連接字串配置正確

### ✅ **遷移後檢查**
- [ ] 資料庫連接正常
- [ ] API 回應正確
- [ ] 前端功能正常
- [ ] 登入驗證成功

---

## 🚨 **故障排除**

### **常見問題**

1. **連接字串錯誤**
   - 檢查伺服器名稱
   - 確認認證方式
   - 驗證憑證設定

2. **套件相依性問題**
   - 執行 `dotnet restore`
   - 清除 `bin/` 和 `obj/` 資料夾
   - 重新建置專案

3. **前端代理問題**
   - 檢查 `proxy.conf.json`
   - 確認後端埠號
   - 重啟 ng serve

---

## 📞 **技術支援**

如遇到問題，請提供：
1. 錯誤訊息截圖
2. 系統環境資訊
3. 遷移步驟執行狀況

---

## 🔐 **預設帳號資訊**

| 用戶名 | 密碼 | 角色 |
|--------|------|------|
| hamaadmin | hamaadmin* | SuperAdmin |
| managera | managera* | OrgAdmin |
| managerb | managerb* | OrgAdmin |
| usera | usera* | User |
| userb | userb* | User | 