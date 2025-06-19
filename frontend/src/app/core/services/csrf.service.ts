import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CsrfService {

  constructor(private http: HttpClient) {}

  /**
   * 🛡️ 初始化 CSRF Token
   * 從伺服器獲取 CSRF Token 並設置為 Cookie
   */
  initializeCsrfToken(): Observable<{ token: string }> {
    return this.http.get<{ token: string }>('/api/antiforgery/token');
  }

  /**
   * 🛡️ 驗證 CSRF Token (測試用)
   */
  validateCsrfToken(): Observable<{ valid: boolean }> {
    return this.http.post<{ valid: boolean }>('/api/antiforgery/validate', {});
  }

  /**
   * 🛡️ 從 Cookie 獲取 CSRF Token
   */
  getCsrfTokenFromCookie(): string | null {
    const name = 'XSRF-TOKEN=';
    const decodedCookie = decodeURIComponent(document.cookie);
    const ca = decodedCookie.split(';');
    
    for (let i = 0; i < ca.length; i++) {
      let c = ca.at(i) || '';
      while (c.charAt(0) === ' ') {
        c = c.substring(1);
      }
      if (c.indexOf(name) === 0) {
        return c.substring(name.length, c.length);
      }
    }
    return null;
  }
} 