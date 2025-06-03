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
- **資料庫**: PostgreSQL (支援 Neon 雲端)
- **驗證**: FluentValidation
- **API 文檔**: Swagger/OpenAPI
- **日誌**: Serilog

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

### 環境需求

#### 必要環境
- **Node.js**: 18.x 或更高版本
- **npm**: 9.x 或更高版本  
- **.NET SDK**: 8.x 或更高版本
- **PostgreSQL**: 14+ 或 Neon 帳戶

#### 開發工具 (推薦)
- Visual Studio Code 或 Visual Studio 2022
- Angular Language Service 擴充功能
- C# Dev Kit 擴充功能

### 1. 克隆專案
```bash
# 專案已在桌面的 SmartNameplateC 資料夾
cd /Users/chiuyongren/Desktop/SmartNameplateC
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

#### 選項 A: 使用 Neon 雲端資料庫 (推薦)
1. 訪問 [Neon Console](https://console.neon.tech)
2. 創建新專案
3. 複製連接字串到 `backend/appsettings.json`

#### 選項 B: 本地 PostgreSQL
1. 安裝 PostgreSQL
2. 創建資料庫：`smart_nameplate_dev`
3. 更新 `backend/appsettings.Development.json` 中的連接字串

### 5. 啟動服務

#### 後端 API (端口 5000/5001)
```bash
cd backend
dotnet run
```

#### 前端應用 (端口 4200)
```bash
cd frontend
npm start
```

### 6. 訪問應用

- **前端應用**: http://localhost:4200
- **API 文檔**: https://localhost:5001/api
- **Swagger UI**: https://localhost:5001/api

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

- [技術棧詳細說明](./TECH_STACK.md)
- [架構設計文檔](./ARCHITECTURE.md)
- [開發規範指南](./frontend/README.md)
- [API 文檔](https://localhost:5001/api) (啟動後端後可存取)

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