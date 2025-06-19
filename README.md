# 智慧桌牌系統 (SmartNameplateC)

電子紙智慧桌牌系統，提供桌牌設計、管理與投放功能。

## 🏗️ 技術棧

### 前端
- **框架**: Angular 17 + TypeScript 5.4
- **樣式**: SCSS (採用 BEM 方法論)
- **UI 套件**: Angular Material + PrimeNG + Ng-Zorro
- **圖表**: Chart.js + ng2-charts
- **工具**: Angular CLI 17、RxJS 7.8

### 後端
- **框架**: ASP.NET Core 8 + C#
- **ORM**: Entity Framework Core 8
- **資料庫**: PostgreSQL / SQL Server (雙主要支援)
- **驗證**: FluentValidation
- **API 文檔**: Swagger/OpenAPI
- **日誌**: Serilog (每日滾動日誌)

## 🚨 **重要架構需求**

> **⚠️ 請務必閱讀 [ARCHITECTURE-REQUIREMENTS.md](./ARCHITECTURE-REQUIREMENTS.md)**

### 🗄️ **雙資料庫主要支援**
- ✅ **PostgreSQL** (主要支援)
- ✅ **SQL Server** (主要支援)  
- 🔄 **平等地位**: 兩種資料庫具有相同的支援優先級
- 🔄 **動態切換**: 透過配置檔案即時切換資料庫

### 🖥️ **IIS 部署準備**
- ✅ **web.config** 完整配置
- ✅ **Angular SPA** 路由支援
- ✅ **安全標頭** 與請求過濾
- ✅ **自動化部署腳本**

### 📊 **每日日誌系統**
- ✅ **應用程式日誌** (保留30天)
- ✅ **安全事件日誌** (保留90天)
- ✅ **Windows 事件日誌** 整合
- ✅ **Serilog** 每日滾動配置

## 📁 專案結構

```
SmartNameplateC/
├── frontend/          # Angular 17 前端應用
│   ├── src/app/
│   │   ├── features/  # 功能模組 (懶加載)
│   │   └── shared/    # 共享元件
│   └── src/styles/    # SCSS 樣式系統
├── backend/           # ASP.NET Core 8 API
│   ├── Controllers/   # Web API 控制器
│   ├── Services/      # 業務邏輯服務
│   ├── Data/          # Entity Framework
│   └── DTOs/          # 資料傳輸物件
├── TECH_STACK.md      # 技術棧詳細說明
├── ARCHITECTURE.md    # 架構設計文檔
└── test-system.sh     # 系統測試腳本
```

## 🚀 快速開始

### 💻 **新電腦部署** 

#### 🤖 **自動化部署 (推薦)**
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

#### 📖 **手動部署指南**
詳細部署步驟請參考：[**backend/deploy-instructions.md**](./backend/deploy-instructions.md)

### 環境需求

#### 必要環境
- **Node.js**: 18.x 或更高版本
- **npm**: 9.x 或更高版本  
- **.NET SDK**: 8.x 或更高版本
- **PostgreSQL**: 14+ 或 **SQL Server**: 2019+

#### 開發工具 (推薦)
- Visual Studio Code 或 Visual Studio 2022
- Angular Language Service 擴充功能
- C# Dev Kit 擴充功能

### 🔄 **原電腦開發**

### 1. 克隆專案
```bash
# 專案已在桌面的 SmartNameplate 2 資料夾
cd "/Users/chiuyongren/Desktop/SmartNameplate 2"
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

### 4. 資料庫設置

#### 🔍 **快速檢查資料庫**
```bash
cd backend
./check-database.sh  # macOS/Linux
```

#### 選項 A: 使用 PostgreSQL (推薦)
1. 安裝 PostgreSQL
2. 創建資料庫：`smart_nameplate`
3. 設定環境變數：
   ```bash
   export DATABASE_PROVIDER=PostgreSQL
   export DATABASE_HOST=localhost
   export DATABASE_NAME=smart_nameplate
   export DATABASE_USERNAME=postgres
   export DATABASE_PASSWORD=your_password
   ```

#### 選項 B: 使用 SQL Server
1. 安裝 SQL Server
2. 創建資料庫：`smart_nameplate`
3. 設定環境變數：
   ```bash
   export DATABASE_PROVIDER=SqlServer
   export SQL_SERVER_HOST=localhost
   export SQL_DATABASE_NAME=smart_nameplate
   export SQL_USERNAME=sa
   export SQL_PASSWORD=your_password
   ```

#### 執行 Migration
```bash
cd backend
dotnet ef database update
```

### 5. 啟動服務

#### 後端 API (端口 5001)
```bash
cd backend
dotnet run --project SmartNameplate.Api.csproj --urls http://localhost:5001
```

#### 前端應用 (端口 4200)
```bash
cd frontend
ng serve --proxy-config proxy.conf.json
```

### 6. 訪問應用

- **前端應用**: http://localhost:4200
- **API 文檔**: http://localhost:5001/api
- **Swagger UI**: http://localhost:5001/api

## 🧪 測試

### 自動測試
```bash
# 執行完整系統測試
./test-system.sh

# 前端測試
cd frontend && npm test

# 後端測試
cd backend && dotnet test
```

### 建構測試
```bash
# 前端建構
cd frontend && npm run build

# 後端建構  
cd backend && dotnet build
```

## 📖 功能模組

### 🎨 圖卡設計
- 拖拉式設計介面
- 多種預設模板
- 自訂元素與樣式
- 即時預覽功能

### 👥 群組管理  
- 桌牌群組分類
- 批次操作功能
- 權限管理
- 群組狀態監控

### 📡 投圖管理
- 即時內容推送
- 排程投放功能
- 遠端設備管理
- 投放狀態追蹤

### ⚙️ 後台管理
- 用戶權限管理
- 系統設定配置
- 設備狀態監控
- 日誌審計功能

## 🔧 開發指南

### 前端開發規範
- 元件前綴：`sn-` (SmartNameplate)
- 使用 Angular 獨立元件
- SCSS 採用 BEM 命名方法論
- 巢狀層級不超過 3 層

### 後端開發規範
- RESTful API 設計
- 依賴注入模式
- 非同步處理
- 完整的錯誤處理

### 程式碼風格
- TypeScript/C# 嚴格模式
- ESLint + Prettier (前端)
- EditorConfig 統一格式
- 完整的 JSDoc/XML 文檔

## 📊 專案統計

### 前端
- **套件數量**: 967 個
- **UI 元件庫**: 3 個 (Angular Material, PrimeNG, Ng-Zorro)
- **建構大小**: ~1.48 MB
- **支援瀏覽器**: Chrome, Firefox, Safari, Edge

### 後端  
- **NuGet 套件**: 16 個
- **API 端點**: REST API
- **資料庫**: PostgreSQL 14+
- **部署目標**: Linux, Windows, macOS

## 🔄 CI/CD 準備

專案結構已準備好整合：
- GitHub Actions
- Azure DevOps
- Docker 容器化
- 自動化測試
- 部署管道

## 📚 文檔

### 🚨 **核心文檔 (必讀)**
- **[架構需求文件](./ARCHITECTURE-REQUIREMENTS.md)** - 多資料庫支援 & IIS 部署需求
- **[ASP.NET Core 最佳實踐](./ASP.NET-Core-Best-Practices.md)** - 開發規範與程式碼範例
- **[IIS 部署故障排除](./IIS-Deployment-Troubleshooting.md)** - 部署問題解決指南

### 📖 **技術文檔**
- [技術棧詳細說明](./TECH_STACK.md)
- [架構設計文檔](./ARCHITECTURE.md)
- [開發規範指南](./frontend/README.md)
- [API 文檔](https://localhost:5001/api) (啟動後端後可存取)

### 🛠️ **部署文檔**
- [自動化部署腳本](./Build-And-Deploy-IIS.ps1)
- [生產環境部署](./Deploy-Production.ps1)
- [資料庫切換腳本](./backend/switch_database.sh)

## 🤝 貢獻指南

1. Fork 專案
2. 創建功能分支：`git checkout -b feature/amazing-feature`
3. 提交變更：`git commit -m 'Add amazing feature'`
4. 推送分支：`git push origin feature/amazing-feature`
5. 提交 Pull Request

## 📄 授權

本專案採用 MIT 授權條款 - 詳見 [LICENSE](LICENSE) 檔案

## 📞 支援

如有問題或建議，請：
- 提交 Issue
- 發送 Email
- 查看文檔

---

**智慧桌牌系統** - 現代化的電子紙桌牌管理解決方案 🚀