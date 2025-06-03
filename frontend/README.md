# 智慧桌牌系統 (Smart Nameplate)

電子紙智慧桌牌系統，提供桌牌設計、管理與投放功能。

## 技術棧

- **前端**：Angular 17、SCSS、TypeScript
- **後端**：.NET 8 (或 NestJS)
- **資料庫**：Neon (PostgreSQL相容雲端資料庫)

## 開發規範

本專案遵循編碼規範，主要包括：

### 樣式規範

- 使用 SCSS 進行樣式開發，不使用 Tailwind
- 每個元件對應一個 `.scss` 檔案
- 避免使用 inline 樣式
- 採用 BEM 命名方法論
- 巢狀不超過3層

### 元件規範

- 使用 Angular 獨立元件
- 元件命名使用 `kebab-case`
- 元件前綴使用 `sn-` (SmartNameplate)

### 專案架構

- 核心模組 (Core)
- 共享模組 (Shared)
- 特徵模組 (Feature)：
  - 首頁 (Home)
  - 圖卡 (Cards)
  - 群組 (Groups)
  - 投圖 (Deploy)
  - 帳號 (Account)
  - 後台 (Admin)

### UI 元件庫

主要採用以下元件庫：
- PrimeNG
- Angular Material

## 運行專案

```bash
# 安裝依賴
npm install

# 開發環境運行
npm start

# 建構生產環境
npm run build

# 執行測試
npm test
```