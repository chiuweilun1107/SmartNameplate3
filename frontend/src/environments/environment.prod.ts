export const environment = {
  production: true,
  apiUrl: 'https://smartnameplate3-backend.vercel.app/api', // 後端 API URL
  appUrl: 'https://smartnameplate3.vercel.app',
  // 🛡️ 生產環境安全配置
  enableHttps: true,   // 生產環境必須啟用 HTTPS
  enableCSRFProtection: true,
  enableSecurityHeaders: true,
  enableSTSHeaders: true,  // 啟用 HSTS
  // 🔧 生產工具
  enableDebugMode: false,
  enableLogging: false,
  enableDebugInfo: false,
  maxFileSize: 5 * 1024 * 1024, // 5MB
  supportedImageTypes: ['image/jpeg', 'image/png', 'image/gif', 'image/webp'],
  cacheTimeout: 300000, // 5分鐘
  version: '3.0.0'
}; 