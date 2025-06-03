import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatIconModule } from '@angular/material/icon';
// import { environment } from '../../../../environments/environment';

interface LoginRequest {
  username: string;
  password: string;
}

interface LoginResponse {
  success: boolean;
  message: string;
  user?: {
    id: number;
    userName: string;
    role: string;
  };
}

@Component({
  selector: 'app-login-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule],
  templateUrl: './login-modal.component.html',
  styleUrls: ['./login-modal.component.scss']
})
export class LoginModalComponent {
  @Input() isVisible: boolean = false;
  @Output() loginSuccess = new EventEmitter<any>();
  @Output() closeModalEvent = new EventEmitter<void>();

  loginData: LoginRequest = {
    username: '',
    password: ''
  };

  isLoading: boolean = false;
  showPassword: boolean = false;
  errorMessage: string = '';
  showTestUsers: boolean = false;

  private apiUrl = 'http://localhost:5001/api/Auth';

  constructor(private http: HttpClient) {}

  onSubmit(): void {
    if (this.isLoading || !this.loginData.username || !this.loginData.password) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.http.post<LoginResponse>(`${this.apiUrl}/login`, this.loginData)
      .subscribe({
        next: (response) => {
          this.isLoading = false;
          if (response.success) {
            console.log('登入成功:', response);
            this.loginSuccess.emit(response.user);
            this.closeModal();
            this.resetForm();
          } else {
            this.errorMessage = response.message || '登入失敗';
          }
        },
        error: (error) => {
          this.isLoading = false;
          console.error('登入錯誤:', error);
          
          if (error.error?.message) {
            this.errorMessage = error.error.message;
          } else if (error.status === 400) {
            this.errorMessage = '用戶名或密碼錯誤';
          } else if (error.status === 0) {
            this.errorMessage = '無法連接到服務器，請檢查網路連接';
          } else {
            this.errorMessage = '登入過程中發生錯誤，請稍後再試';
          }
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
      password: ''
    };
    this.showPassword = false;
    this.errorMessage = '';
    this.showTestUsers = false;
  }
} 