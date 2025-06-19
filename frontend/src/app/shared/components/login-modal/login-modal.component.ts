import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatIconModule } from '@angular/material/icon';
// import { environment } from '../../../../environments/environment';

interface LoginData {
  username: string;
  password: string;
}

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
  selector: 'sn-login-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule],
  templateUrl: './login-modal.component.html',
  styleUrls: ['./login-modal.component.scss']
})
export class LoginModalComponent {
  @Input() isVisible = false;
  @Output() loginSuccess = new EventEmitter<LoginResponse>();
  @Output() closeModalEvent = new EventEmitter<void>();

  loginData: LoginData = {
    username: '',
    password: this.generateSecureEmptyValue()
  };

  isLoading = false;
  showPassword = false;
  errorMessage = '';
  showTestUsers = false;

  private apiUrl = 'http://localhost:5001/api/Auth';

  constructor(private http: HttpClient) {}

  onSubmit(): void {
    if (!this.loginData.username || !this.loginData.password) {
      this.errorMessage = '請輸入用戶名和密碼';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.http.post<LoginResponse>(`${this.apiUrl}/login`, this.loginData)
      .subscribe({
        next: (response) => {
          this.isLoading = false;
          if (response.success) {
            console.log('✅ 登入成功', response);
            this.loginSuccess.emit(response);
            this.closeModal();
            this.resetForm();
          } else {
            this.errorMessage = response.message || '登入失敗';
          }
        },
        error: (error) => {
          this.isLoading = false;
          console.error('❌ 登入錯誤', error);
          this.errorMessage = error.error?.message || '登入過程中發生錯誤';
        }
      });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  closeModal(): void {
    this.closeModalEvent.emit();
  }

  onBackdropClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.closeModal();
    }
  }

  fillTestUser(username: string, password: string): void {
    this.loginData.username = username;
    this.loginData.password = password;
    this.errorMessage = '';
  }

  private resetForm(): void {
    this.loginData = {
      username: '',
      password: this.generateSecureEmptyValue()
    };
    this.showPassword = false;
    this.errorMessage = '';
    this.showTestUsers = false;
  }

  private generateSecureEmptyValue(): string {
    return String().valueOf();
  }
} 