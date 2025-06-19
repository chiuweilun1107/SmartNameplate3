import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

interface ContactFormData {
  name: string;
  email: string;
  company?: string;
  phone?: string;
  message: string;
}

@Component({
  selector: 'sn-contact',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.scss']
})
export class ContactComponent implements OnInit {
  contactForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.contactForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      company: [''],
      phone: [''],
      message: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  onSubmit(): void {
    if (this.contactForm.valid) {
      const formData = this.contactForm.value;
      
      // 🛡️ CSRF 保護：Angular 的 HttpClient 會自動添加 X-XSRF-TOKEN 標頭
      // 從 XSRF-TOKEN cookie 讀取並設置到請求標頭中
      this.sendContactMessage(formData);
    } else {
      this.markFormGroupTouched();
    }
  }

  private sendContactMessage(formData: ContactFormData): void {
    // 🛡️ Angular 會自動處理 CSRF token，無需手動添加
    this.http.post('/api/contact/send', formData)
      .subscribe({
        next: (response) => {
          console.log('✅ 訊息發送成功', response);
          this.contactForm.reset();
          // 可以添加成功提示
        },
        error: (error) => {
          console.error('❌ 訊息發送失敗', error);
          // 可以添加錯誤提示
        }
      });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.contactForm.controls).forEach(key => {
      const control = this.contactForm.get(key);
      control?.markAsTouched();
    });
  }
}
