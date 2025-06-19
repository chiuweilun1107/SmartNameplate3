# 智慧桌牌系統 (Smart Nameplate System)

這是一個基於 Angular + ASP.NET Core 的智慧桌牌管理系統。

## 🚀 功能特色

- 📱 桌牌設計器
- 👥 群組管理
- 📄 範本管理
- 🔄 部署歷史
- 🔐 用戶認證與權限管理

## 🛠️ 技術棧

### 前端
- Angular 17
- Angular Material
- PrimeNG
- SCSS (BEM方法論)
- TypeScript

### 後端
- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL/SQLite

## 📦 部署

### Vercel 部署 (前端)

1. 將專案推送到 GitHub
2. 在 Vercel 中連接 GitHub repository
3. 設定建置命令：`cd frontend && npm run build`
4. 設定輸出目錄：`frontend/dist/frontend`

### 本地開發

#### 啟動後端
```bash
dotnet run --project backend/SmartNameplate.Api.csproj --urls http://localhost:5001
```

#### 啟動前端
```bash
cd frontend
ng serve --proxy-config proxy.conf.json
```

## 🔧 環境變數

建立 `.env` 檔案並設定：

```
DATABASE_CONNECTION_STRING=your_database_connection
JWT_SECRET_KEY=your_jwt_secret
```

## 📄 API 文檔

API 運行於 `http://localhost:5001/swagger`

## 🤝 貢獻

歡迎提交 Issue 和 Pull Request！

## 📝 授權

MIT License