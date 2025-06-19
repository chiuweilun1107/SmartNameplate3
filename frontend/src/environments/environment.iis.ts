/**
 * IIS 部署環境設定
 * 用於 Windows Server + IIS 的生產環境
 */
export const environment = {
  production: true,
  
  // 🌐 IIS 部署 API 端點設定
  apiUrl: 'https://your-domain.com/SmartNameplate/api',
  
  // 🔧 IIS 靜態資源路徑
  assetsPath: '/SmartNameplate/assets',
  
  // 📡 WebSocket 設定（用於藍牙即時更新）
  wsUrl: 'wss://your-domain.com/SmartNameplate/ws',
  
  // 🔐 安全設定
  enableHttps: true,
  cookieSecure: true,
  
  // 📊 效能監控
  enableAnalytics: true,
  
  // 🐛 除錯設定
  enableLogging: false,
  enableConsoleLog: false,
  enableErrorReporting: true,
  
  // 🏗️ 建置資訊
  buildTarget: 'iis',
  version: '1.0.0'
}; 