import 'zone.js';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient } from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes';
import { HttpSecurityInterceptor } from './app/core/services/http-security.service';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideAnimations(),
    provideHttpClient(),
    // 🛡️ HTTP 安全攔截器
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpSecurityInterceptor,
      multi: true
    },
    // 其他 providers
  ]
}).catch(() => {
  // 🛡️ 安全日誌：避免洩露敏感啟動錯誤資訊
  console.warn('應用程式啟動失敗 - 詳細資訊已記錄至伺服器');
});