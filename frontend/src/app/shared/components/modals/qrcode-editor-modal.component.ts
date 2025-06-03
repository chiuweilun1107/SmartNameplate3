import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatSliderModule } from '@angular/material/slider';
import { FormsModule } from '@angular/forms';
import { QRCodeModule } from 'angularx-qrcode';

export interface QRCodeSettings {
  data: string;
  size: number;
  backgroundColor: string;
  foregroundColor: string;
  errorCorrectionLevel: 'L' | 'M' | 'Q' | 'H';
  margin: number;
  borderColor?: string;
  borderWidth?: number;
  borderRadius?: number;
}

@Component({
  selector: 'sn-qrcode-editor-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatSliderModule,
    FormsModule,
    QRCodeModule
  ],
  template: `
    <div class="modal-overlay" (click)="onOverlayClick($event)" *ngIf="isVisible">
      <div class="modal-container" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2 class="modal-title">QR碼編輯器</h2>
          <button mat-icon-button class="modal-close-btn" (click)="closeModal()">
            <mat-icon>close</mat-icon>
          </button>
        </div>

        <div class="modal-content">
          <div class="editor-layout">
            <!-- 左側：設定區 -->
            <div class="settings-panel">
              <!-- QR碼內容 -->
              <div class="setting-group">
                <h3>QR碼內容</h3>
                <div class="content-type-tabs">
                  <button 
                    *ngFor="let type of contentTypes"
                    class="tab-button"
                    [class.active]="selectedContentType === type.value"
                    (click)="selectContentType(type.value)">
                    <mat-icon>{{ type.icon }}</mat-icon>
                    {{ type.label }}
                  </button>
                </div>
                
                <div class="content-input" [ngSwitch]="selectedContentType">
                  <!-- 網址 -->
                  <mat-form-field *ngSwitchCase="'url'" appearance="outline">
                    <mat-label>網址</mat-label>
                    <input matInput 
                           [(ngModel)]="settings.data" 
                           placeholder="https://example.com"
                           (input)="onContentChange()">
                  </mat-form-field>

                  <!-- 文字 -->
                  <mat-form-field *ngSwitchCase="'text'" appearance="outline">
                    <mat-label>文字內容</mat-label>
                    <textarea matInput 
                              [(ngModel)]="settings.data" 
                              rows="3"
                              placeholder="輸入文字內容"
                              (input)="onContentChange()"></textarea>
                  </mat-form-field>

                  <!-- 電話 -->
                  <mat-form-field *ngSwitchCase="'phone'" appearance="outline">
                    <mat-label>電話號碼</mat-label>
                    <input matInput 
                           [(ngModel)]="phoneNumber" 
                           placeholder="+886-912-345-678"
                           (input)="onPhoneChange()">
                  </mat-form-field>

                  <!-- 電子郵件 -->
                  <mat-form-field *ngSwitchCase="'email'" appearance="outline">
                    <mat-label>電子郵件</mat-label>
                    <input matInput 
                           [(ngModel)]="emailAddress" 
                           placeholder="example@email.com"
                           (input)="onEmailChange()">
                  </mat-form-field>

                  <!-- WiFi -->
                  <div *ngSwitchCase="'wifi'" class="wifi-settings">
                    <mat-form-field appearance="outline">
                      <mat-label>WiFi名稱 (SSID)</mat-label>
                      <input matInput [(ngModel)]="wifiSSID" (input)="onWifiChange()">
                    </mat-form-field>
                    <mat-form-field appearance="outline">
                      <mat-label>密碼</mat-label>
                      <input matInput [(ngModel)]="wifiPassword" type="password" (input)="onWifiChange()">
                    </mat-form-field>
                    <mat-form-field appearance="outline">
                      <mat-label>加密方式</mat-label>
                      <mat-select [(ngModel)]="wifiSecurity" (selectionChange)="onWifiChange()">
                        <mat-option value="WPA">WPA/WPA2</mat-option>
                        <mat-option value="WEP">WEP</mat-option>
                        <mat-option value="nopass">無密碼</mat-option>
                      </mat-select>
                    </mat-form-field>
                  </div>
                </div>
              </div>

              <!-- 外觀設定 -->
              <div class="setting-group">
                <h3>外觀設定</h3>
                
                <!-- 大小 -->
                <div class="setting-item">
                  <label>大小：{{ settings.size }}px</label>
                  <input type="range" 
                         min="50" 
                         max="200" 
                         step="10" 
                         [(ngModel)]="settings.size" 
                         (input)="onSettingChange()"
                         class="qr-slider">
                </div>

                <!-- 前景色 -->
                <div class="setting-item">
                  <label>前景色</label>
                  <div class="color-picker">
                    <input type="color" [(ngModel)]="settings.foregroundColor" (input)="onSettingChange()">
                    <span>{{ settings.foregroundColor }}</span>
                  </div>
                </div>

                <!-- 背景色 -->
                <div class="setting-item">
                  <label>背景色</label>
                  <div class="color-picker">
                    <input type="color" [(ngModel)]="settings.backgroundColor" (input)="onSettingChange()">
                    <span>{{ settings.backgroundColor }}</span>
                  </div>
                </div>

                <!-- 邊距 -->
                <div class="setting-item">
                  <label>邊距：{{ settings.margin }}px</label>
                  <input type="range" 
                         min="0" 
                         max="50" 
                         step="1" 
                         [(ngModel)]="settings.margin" 
                         (input)="onSettingChange()"
                         class="qr-slider">
                </div>

                <!-- 錯誤修正等級 -->
                <div class="setting-item">
                  <mat-form-field appearance="outline">
                    <mat-label>錯誤修正等級</mat-label>
                    <mat-select [(ngModel)]="settings.errorCorrectionLevel" (selectionChange)="onSettingChange()">
                      <mat-option value="L">低 (L) - 約7%</mat-option>
                      <mat-option value="M">中 (M) - 約15%</mat-option>
                      <mat-option value="Q">中高 (Q) - 約25%</mat-option>
                      <mat-option value="H">高 (H) - 約30%</mat-option>
                    </mat-select>
                  </mat-form-field>
                </div>

              </div>
            </div>

            <!-- 右側：預覽區 -->
            <div class="preview-panel">
              <h3>預覽</h3>
              <div class="qr-preview">
                <div class="qr-preview-container">
                  <qrcode 
                    [qrdata]="settings.data" 
                    [width]="settings.size"
                    [colorDark]="settings.foregroundColor" 
                    [colorLight]="settings.backgroundColor"
                    [errorCorrectionLevel]="settings.errorCorrectionLevel"
                    [margin]="settings.margin"
                    cssClass="qr-preview-code">
                  </qrcode>
                </div>
              </div>
              
              <div class="preview-info">
                <p><strong>內容：</strong> {{ getPreviewText() }}</p>
                <p><strong>大小：</strong> {{ settings.size }}x{{ settings.size }}px</p>
                <p><strong>錯誤修正：</strong> {{ getErrorCorrectionText() }}</p>
                <p><strong>邊距：</strong> {{ settings.margin }}px</p>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button mat-raised-button color="primary" (click)="confirm()">
            確認
          </button>
          <button mat-button (click)="closeModal()">
            取消
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./qrcode-editor-modal.component.scss']
})
export class QRCodeEditorModalComponent implements OnInit {
  @Input() isVisible = false;
  @Input() currentSettings: QRCodeSettings = {
    data: '@https://example.com',
    size: 100,
    backgroundColor: '#ffffff',
    foregroundColor: '#000000',
    errorCorrectionLevel: 'M',
    margin: 4
  };
  @Output() settingsChanged = new EventEmitter<QRCodeSettings>();
  @Output() close = new EventEmitter<void>();

  settings: QRCodeSettings = { ...this.currentSettings };
  selectedContentType = 'url';
  
  // 臨時變數用於特定格式
  phoneNumber = '';
  emailAddress = '';
  wifiSSID = '';
  wifiPassword = '';
  wifiSecurity = 'WPA';

  // 讓模板能訪問Math
  Math = Math;

  contentTypes = [
    { value: 'url', label: '網址', icon: 'link' },
    { value: 'text', label: '文字', icon: 'text_fields' },
    { value: 'phone', label: '電話', icon: 'phone' },
    { value: 'email', label: '郵件', icon: 'email' },
    { value: 'wifi', label: 'WiFi', icon: 'wifi' }
  ];

  ngOnInit(): void {
    this.settings = { ...this.currentSettings };
    this.detectContentType();
  }

  detectContentType(): void {
    const data = this.settings.data;
    
    if (data.startsWith('http://') || data.startsWith('https://')) {
      this.selectedContentType = 'url';
    } else if (data.startsWith('tel:')) {
      this.selectedContentType = 'phone';
      this.phoneNumber = data.replace('tel:', '');
    } else if (data.startsWith('mailto:')) {
      this.selectedContentType = 'email';
      this.emailAddress = data.replace('mailto:', '');
    } else if (data.startsWith('WIFI:')) {
      this.selectedContentType = 'wifi';
      this.parseWifiData(data);
    } else {
      this.selectedContentType = 'text';
    }
  }

  selectContentType(type: string): void {
    this.selectedContentType = type;
    
    // 根據類型設定預設內容
    switch (type) {
      case 'url':
        this.settings.data = 'https://example.com';
        break;
      case 'text':
        this.settings.data = '文字內容';
        break;
      case 'phone':
        this.phoneNumber = '+886-912-345-678';
        this.onPhoneChange();
        break;
      case 'email':
        this.emailAddress = 'example@email.com';
        this.onEmailChange();
        break;
      case 'wifi':
        this.wifiSSID = 'MyWiFi';
        this.wifiPassword = 'password123';
        this.wifiSecurity = 'WPA';
        this.onWifiChange();
        break;
    }
  }

  onContentChange(): void {
    // 一般內容變更時無需特殊處理
  }

  onPhoneChange(): void {
    this.settings.data = `tel:${this.phoneNumber}`;
  }

  onEmailChange(): void {
    this.settings.data = `mailto:${this.emailAddress}`;
  }

  onWifiChange(): void {
    this.settings.data = `WIFI:T:${this.wifiSecurity};S:${this.wifiSSID};P:${this.wifiPassword};;`;
  }

  parseWifiData(data: string): void {
    const matches = data.match(/WIFI:T:([^;]*);S:([^;]*);P:([^;]*);/);
    if (matches) {
      this.wifiSecurity = matches[1] || 'WPA';
      this.wifiSSID = matches[2] || '';
      this.wifiPassword = matches[3] || '';
    }
  }

  onSettingChange(): void {
    // 設定變更時觸發
  }

  getPreviewText(): string {
    const data = this.settings.data;
    if (data.length > 50) {
      return data.substring(0, 50) + '...';
    }
    return data;
  }

  getErrorCorrectionText(): string {
    const levels = {
      'L': '低 (約7%)',
      'M': '中 (約15%)',
      'Q': '中高 (約25%)',
      'H': '高 (約30%)'
    };
    return levels[this.settings.errorCorrectionLevel];
  }

  confirm(): void {
    this.settingsChanged.emit(this.settings);
    this.closeModal();
  }

  closeModal(): void {
    this.close.emit();
  }

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.closeModal();
    }
  }
} 