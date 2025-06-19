import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HttpSecurityService {
  
  /**
   * 🛡️ 檢查 URL 是否為安全連接
   */
  isSecureConnection(): boolean {
    return location.protocol === 'https:';
  }

  /**
   * 🛡️ 取得安全的 API URL
   */
  getSecureApiUrl(endpoint: string): string {
    if (environment.production && environment.enableHttps) {
      // 生產環境強制使用 HTTPS
      return endpoint.replace(/^http:\/\//, 'https://');
    }
    return endpoint;
  }

  /**
   * 🛡️ 驗證 API 回應的安全性
   */
  validateApiResponse(response: { headers?: { has: (header: string) => boolean } }): boolean {
    // 檢查回應是否包含安全標頭
    const securityHeaders = ['x-content-type-options', 'x-frame-options'];
    return securityHeaders.some(header => 
      response.headers && response.headers.has(header)
    );
  }

  /**
   * 🛡️ 記錄安全事件
   */
  logSecurityEvent(event: string, details?: unknown): void {
    if (environment.enableLogging) {
      // 修復Format String漏洞：分離訊息和數據
      console.warn('[Security]', event, details);
    }
  }
}

/**
 * 🛡️ HTTP 安全攔截器
 * 自動添加安全標頭並處理 HTTPS 重定向
 */
@Injectable()
export class HttpSecurityInterceptor implements HttpInterceptor {
  
  constructor(private securityService: HttpSecurityService) {}

  intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // 🛡️ 複製請求並添加安全標頭
    let secureReq = req.clone({
      setHeaders: {
        'X-Requested-With': 'XMLHttpRequest',
        'Cache-Control': 'no-cache, no-store, must-revalidate',
        'Pragma': 'no-cache'
      }
    });

    // 🛡️ 在生產環境中確保使用 HTTPS
    if (environment.production && environment.enableHttps) {
      const secureUrl = this.securityService.getSecureApiUrl(secureReq.url);
      if (secureUrl !== secureReq.url) {
        secureReq = secureReq.clone({ url: secureUrl });
        this.securityService.logSecurityEvent('HTTP URL upgraded to HTTPS', {
          original: req.url,
          secure: secureUrl
        });
      }
    }

    return next.handle(secureReq);
  }
} 