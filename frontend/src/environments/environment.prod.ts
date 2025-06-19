export const environment = {
  production: true,
  apiUrl: '/api',
  // 🛡️ 生產環境安全配置
  enableHttps: true,   // 生產環境必須啟用 HTTPS
  enableCSRFProtection: true,
  enableSecurityHeaders: true,
  enableSTSHeaders: true,  // 啟用 HSTS
  // 🔧 生產工具
  enableDebugMode: false,
  enableLogging: false
}; 