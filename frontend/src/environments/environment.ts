/**
 * 開發環境設定
 * 包含安全措施以降低開發工具漏洞風險
 */
export const environment = {
  production: false,
  
  // 🌐 開發 API 設定
  apiUrl: 'http://localhost:5001/api',
  
  // 🔧 開發資源路徑
  assetsPath: '/assets',
  
  // 📡 WebSocket 設定
  wsUrl: 'ws://localhost:5001/ws',
  
  // 🔐 開發環境安全設定
  enableHttps: false,
  cookieSecure: false,
  
  // 🛡️ 開發伺服器安全措施
  devServer: {
    // 限制開發伺服器只監聽本地
    host: 'localhost',
    // 禁用外部存取
    allowedHosts: ['localhost', '127.0.0.1'],
    // 啟用 CORS 保護
    corsEnabled: true
  },
  
  // 📊 開發工具設定
  enableAnalytics: false,
  
  // 🐛 除錯設定
  enableLogging: true,
  enableConsoleLog: true,
  enableErrorReporting: false,
  
  // 🏗️ 建置資訊
  buildTarget: 'development',
  version: '1.0.0-dev'
}; 