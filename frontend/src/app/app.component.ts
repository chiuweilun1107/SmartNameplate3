import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { HeaderComponent } from './shared/layouts/header/header.component';
import { NotificationComponent } from './shared/components/notifications/notification.component';
import { FooterComponent } from './shared/layouts/footer/footer.component';
import { LoginModalComponent } from './shared/components/login-modal/login-modal.component';
import { filter } from 'rxjs/operators';
import { CsrfService } from './core/services/csrf.service';

interface LoginResponse {
  success: boolean;
  message: string;
  user?: {
    id: number;
    username: string;
    role: string;
  };
  token?: string;
}

@Component({
  selector: 'sn-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HeaderComponent, NotificationComponent, FooterComponent, LoginModalComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Magic E-paper';
  isLoggedIn = false;
  userName = '';
  userAvatar = '';
  showFooter = true;
  showLoginModal = false;

  constructor(private router: Router, private csrfService: CsrfService) {
    // 監聽路由變化，決定是否顯示頁腳
    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event) => {
        // cards/new 和 cards/edit/:id 頁面不顯示頁腳
        this.showFooter = !event.url.includes('/cards/new') && !event.url.includes('/cards/edit');
      });
  }

  ngOnInit(): void {
    // 🛡️ 初始化 CSRF Token
    this.initializeCsrfProtection();
  }

  private initializeCsrfProtection(): void {
    this.csrfService.initializeCsrfToken().subscribe({
      next: (response) => {
        console.log('🛡️ CSRF Token 初始化成功', response);
      },
      error: (error) => {
        console.error('❌ CSRF Token 初始化失敗', error);
        // 在實際應用中，可能需要重試或顯示錯誤訊息
      }
    });
  }

  onLogin(): void {
    console.log('登入按鈕被點擊');
    this.showLoginModal = true;
  }

  onLogout(): void {
    console.log('登出按鈕被點擊');
    this.isLoggedIn = false;
    this.userName = '';
    this.userAvatar = '';
  }

  onProfile(): void {
    console.log('個人資料被點擊');
  }

  onLoginSuccess(response: LoginResponse): void {
    console.log('登入成功:', response);
    this.isLoggedIn = true;
    this.userName = response.user?.username || '';
    this.userAvatar = '';  // LoginResponse 沒有 avatar 欄位，保持空字串
    this.showLoginModal = false;
  }

  onCloseLoginModal(): void {
    this.showLoginModal = false;
  }
}