# 🤖 SmartNameplate 開發與部署規則

## 🎯 **基本開發規則**

- 🤖 **回覆時給予機器人符號**
- 🔍 **優先查找 context7 mcp 工具搜尋問題的解決方案**
- ⚡ **發現 linter error 要馬上修正**
- 🏗️ **使用 Angular/SASS/TS 架構，利用 BEM 寫法，HTML 專注架構、文字內容都寫在 TS、Scss 負責樣式**
- 🚨 **遇到編譯錯誤必須優先解決**
- 🗄️ **以實際資料庫資料實作**
- 🔄 **修正到 API 或是後端相關文件後，要停止後端服務器然後重啟**
- 🧩 **使用共通元件，如果沒有共通元件請建立**
- 📂 **共通元件資料夾路徑：/Users/chiuyongren/Desktop/SmartNameplateC/frontend/src/app/shared/components**
- 🛠️ **直接進行檢查以及修正，不需要請求文件或詢問是否需要修正**
- 🚪 **使用 lsof -i 以及 kill -9 [PID] 指令解決端口衝突，不要使用 kill -f "dotnet**

---

## 🚀 **啟動指令**

### 啟動後端（ASP.NET）
```bash
dotnet run --project backend/SmartNameplate.Api.csproj --urls http://localhost:5001
```
- 這會啟動 ASP.NET Core Web API，API 端點為 `http://localhost:5001`
- 若遇到 port 被佔用，請先關閉舊的 dotnet 服務

### 啟動前端（Angular）
```bash
cd frontend
ng serve --proxy-config proxy.conf.json
```
- 這會啟動 Angular 前端，預設網址為 `http://localhost:4200`
- 若 4200 port 被佔用，會自動詢問是否換 port
- 前端所有 `/api` 請求會自動代理到後端 5001 port

---

## 🏭 **生產部署安全規則**

### 🛡️ **OWASP Top 10 2021 合規要求**

1. **A01: Broken Access Control** - 實施最小權限原則，驗證每個 API 端點的存取權限
2. **A02: Cryptographic Failures** - 使用 BCrypt 密碼散列，AES-256 敏感資料加密
3. **A03: Injection** - 使用參數化查詢，驗證和清理所有用戶輸入
4. **A04: Insecure Design** - 實施威脅建模，速率限制，審計日誌
5. **A05: Security Misconfiguration** - 正確配置安全標頭，移除不必要的服務
6. **A06: Vulnerable Components** - 定期更新套件，使用最新穩定版本
7. **A07: Authentication Failures** - 實施 JWT 安全配置，多因素認證
8. **A08: Integrity Failures** - 檔案上傳驗證，病毒掃描，檔案雜湊驗證
9. **A09: Logging Failures** - 全面安全日誌記錄，監控異常活動
10. **A10: SSRF** - 驗證外部 URL，防止內部網路存取

### 🏭 **Windows Server IIS 部署配置**

#### web.config 必要安全設定：
```xml
<!-- 安全標頭 -->
<add name="X-Frame-Options" value="DENY" />
<add name="X-Content-Type-Options" value="nosniff" />
<add name="X-XSS-Protection" value="1; mode=block" />
<add name="Strict-Transport-Security" value="max-age=31536000; includeSubDomains" />
<add name="Content-Security-Policy" value="default-src 'self';" />

<!-- 移除服務器資訊 -->
<remove name="Server" />
<remove name="X-Powered-By" />
<remove name="X-AspNet-Version" />

<!-- 強制 HTTPS 重導向 -->
<!-- Angular 路由支援 -->
```

### 📊 **日誌管理系統**

#### 日誌資料夾結構：
```
C:\SmartNameplate\Logs\
├── Application\          # 應用程式日誌
├── Security\            # 安全事件日誌
├── Performance\         # 效能監控日誌
├── Database\           # 資料庫操作日誌
└── IIS\               # IIS 伺服器日誌
```

#### 必要的安全日誌：
- 登入嘗試和失敗記錄
- 權限提升嘗試
- 檔案存取記錄
- API 呼叫追蹤
- 安全事件警報

### 🔒 **SSL/TLS 要求**

- 強制 HTTPS 重導向
- HSTS 標頭設定
- SSL 憑證定期更新
- 禁用不安全的 SSL/TLS 版本

### 🚨 **錯誤處理規則**

- 全域異常處理中間件
- 不洩漏敏感錯誤資訊
- 記錄所有異常到安全日誌
- 提供用戶友善的錯誤訊息

### 🔍 **部署前檢查清單**

1. ✅ SSL 憑證驗證
2. ✅ 安全標頭檢查
3. ✅ 開放埠口掃描
4. ✅ 檔案權限設定
5. ✅ 日誌配置驗證
6. ✅ 資料庫連接安全性
7. ✅ OWASP ZAP 安全掃描
8. ✅ 效能測試
9. ✅ 滲透測試

### 📅 **維護計畫**

- **每日**：檢查安全日誌和警報
- **每週**：系統更新和漏洞掃描
- **每月**：安全配置審查
- **每季**：滲透測試和安全評估
- **每年**：完整安全審計

---

## 📋 **開發檢查清單**

### 🔧 **程式碼品質**
- [ ] 所有 Linter 錯誤已修正
- [ ] 編譯沒有警告
- [ ] 單元測試通過
- [ ] 程式碼審查完成

### 🛡️ **安全檢查**
- [ ] 輸入驗證實施
- [ ] 權限檢查到位
- [ ] 敏感資料加密
- [ ] 安全日誌記錄

### 🗄️ **資料庫**
- [ ] 使用參數化查詢
- [ ] 資料庫遷移測試
- [ ] 備份策略確認
- [ ] 效能優化檢查

### 🌐 **前端**
- [ ] Angular 元件使用共通組件
- [ ] BEM 命名規範遵循
- [ ] RWD 響應式設計
- [ ] 無障礙功能實施

---

## 🚨 **緊急回應程序**

1. **檢測** - 監控系統自動檢測異常
2. **隔離** - 立即隔離受影響的系統
3. **分析** - 分析事件範圍和影響
4. **修復** - 實施修復措施
5. **復原** - 安全復原服務
6. **檢討** - 事後檢討和改進

詳細的安全配置和部署指南請參考：`.cursor/rules/production-deployment-security.md` 