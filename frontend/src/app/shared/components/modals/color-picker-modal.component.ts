import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'sn-color-picker-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    FormsModule
  ],
  template: `
    <div class="modal-overlay" (click)="onOverlayClick($event)"
      (keydown.enter)="onOverlayClick($event)"
      (keydown.space)="onOverlayClick($event)"
      tabindex="0" role="button" *ngIf="isVisible">
      <div class="modal-container" 
           (click)="$event.stopPropagation()"
           (keydown.enter)="$event.stopPropagation()"
           (keydown.space)="$event.stopPropagation()"
           tabindex="0" 
           role="dialog"
           aria-label="顏色選擇對話框">
        <div class="modal-header">
          <h2 class="modal-title">{{ getSafeTitle() }}</h2>
          <button mat-icon-button class="modal-close-btn" (click)="closeModal()"
            (keydown.enter)="closeModal()"
            (keydown.space)="closeModal()"
            tabindex="0" role="button">
            <mat-icon>close</mat-icon>
          </button>
        </div>

        <div class="modal-content">
          <!-- 預設顏色 -->
          <div class="color-section">
            <h3>預設顏色</h3>
            <div class="color-grid">
              <div
                *ngFor="let color of presetColors"
                class="color-preset"
                [style.background-color]="color"
                [class.selected]="selectedColor === color"
                (click)="selectColor(color)"
                (keydown.enter)="selectColor(color)"
                (keydown.space)="selectColor(color)"
                tabindex="0"
                role="button"
                [attr.aria-label]="'選擇顏色 ' + color">
                <mat-icon *ngIf="selectedColor === color">check</mat-icon>
              </div>
            </div>
          </div>

          <!-- 自訂顏色 -->
          <div class="color-section">
            <h3>自訂顏色</h3>
            <div class="custom-color-picker">
              <input 
                type="color" 
                [(ngModel)]="customColor" 
                (input)="onCustomColorChange($event)"
                class="color-input">
              <div class="color-value" [innerHTML]="getSafeColorDisplay(customColor)"></div>
            </div>
          </div>

          <!-- 透明度設定 -->
          <div class="color-section" *ngIf="showOpacity">
            <h3>透明度</h3>
            <div class="opacity-slider">
              <input 
                type="range" 
                min="0" 
                max="100" 
                [(ngModel)]="opacity"
                (input)="updatePreview()"
                class="slider"
                [style.background]="getSafeOpacityGradient()">
              <span class="opacity-value">{{ getSafeOpacity() }}%</span>
            </div>
          </div>

          <!-- 顏色預覽 -->
          <div class="color-section">
            <h3>預覽</h3>
            <div class="color-preview" [style.background-color]="getSafePreviewColor()">
              <span>範例文字</span>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button mat-raised-button color="primary" (click)="confirm()"
            (keydown.enter)="confirm()"
            (keydown.space)="confirm()"
            tabindex="0" role="button">確認</button>
          <button mat-button (click)="closeModal()"
            (keydown.enter)="closeModal()"
            (keydown.space)="closeModal()"
            tabindex="0" role="button">取消</button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./color-picker-modal.component.scss']
})
export class ColorPickerModalComponent implements OnInit {
  @Input() isVisible = false;
  @Input() title = '選擇顏色';
  @Input() currentColor = '#000000';
  @Input() showOpacity = false;
  @Output() colorSelected = new EventEmitter<string>();
  @Output() modalClose = new EventEmitter<void>();

  selectedColor = '#000000';
  customColor = '#000000';
  opacity = 100;

  presetColors = [
    '#000000', '#ffffff', '#ff0000', '#00ff00', '#0000ff',
    '#ffff00', '#ff00ff', '#00ffff', '#808080', '#800000',
    '#008000', '#000080', '#808000', '#800080', '#008080',
    '#c0c0c0', '#ffa500', '#a52a2a', '#dda0dd', '#98fb98',
    '#f0e68c', '#deb887', '#5f9ea0', '#ff1493', '#00bfff',
    '#ffd700', '#adff2f', '#ff69b4', '#b22222', '#228b22'
  ];

  constructor(private sanitizer: DomSanitizer) {}

  ngOnInit(): void {
    this.selectedColor = this.getSafeColor(this.currentColor);
    this.customColor = this.getSafeColor(this.currentColor);
    
    // 🛡️ 安全的透明度提取 - 防止 ReDoS 攻擊
    if (this.currentColor.includes('rgba') && this.currentColor.length < 50) {
      const rgba = this.currentColor.match(/^rgba\(([0-9]{1,3}),\s*([0-9]{1,3}),\s*([0-9]{1,3}),\s*([01](?:\.[0-9]{1,2})?)\)$/);
      if (rgba && rgba[4]) {
        this.opacity = Math.round(parseFloat(rgba[4]) * 100);
      }
    }
  }

  /**
   * 取得安全的標題
   */
  getSafeTitle(): string {
    return this.sanitizeInput(this.title) || '選擇顏色';
  }

  /**
   * 取得安全的顏色值
   */
  getSafeColor(color: string): string {
    if (!color) return '#000000';
    
    // 驗證顏色格式（hex、rgb、rgba）- 使用安全的正則表達式
    const hexPattern = /^#[A-Fa-f0-9]{3}$|^#[A-Fa-f0-9]{6}$/;
    const rgbPattern = /^rgb\([ \t]*[0-9]{1,3}[ \t]*,[ \t]*[0-9]{1,3}[ \t]*,[ \t]*[0-9]{1,3}[ \t]*\)$/;
    const rgbaPattern = /^rgba\([ \t]*[0-9]{1,3}[ \t]*,[ \t]*[0-9]{1,3}[ \t]*,[ \t]*[0-9]{1,3}[ \t]*,[ \t]*[01](?:\.[0-9]+)?[ \t]*\)$/;
    
    const cleanColor = this.sanitizeInput(color);
    
    if (hexPattern.test(cleanColor) || rgbPattern.test(cleanColor) || rgbaPattern.test(cleanColor)) {
      return cleanColor;
    }
    
    return '#000000'; // 預設安全顏色
  }

  /**
   * 取得安全的顏色顯示文字
   */
  getSafeColorDisplay(color: string): SafeHtml {
    const safeColor = this.getSafeColor(color);
    return this.sanitizer.sanitize(1, safeColor) || '#000000';
  }

  /**
   * 取得安全的透明度值
   */
  getSafeOpacity(): number {
    return Math.max(0, Math.min(100, Math.round(this.opacity || 0)));
  }

  /**
   * 取得安全的透明度漸層
   */
  getSafeOpacityGradient(): string {
    const hex = this.getSafeColor(this.selectedColor);
    
    // 提取 RGB 值
    let r = 0, g = 0, b = 0;
    
    if (hex.startsWith('#')) {
      const result = /^#([a-f0-9]{2})([a-f0-9]{2})([a-f0-9]{2})$/i.exec(hex);
      if (result) {
        r = parseInt(result[1], 16);
        g = parseInt(result[2], 16);
        b = parseInt(result[3], 16);
      }
    }
    
    return `linear-gradient(to right, rgba(${r},${g},${b},0), rgba(${r},${g},${b},1))`;
  }

  /**
   * 取得安全的預覽顏色
   */
  getSafePreviewColor(): string {
    const safeColor = this.getSafeColor(this.selectedColor);
    const safeOpacity = this.getSafeOpacity();
    
    if (this.showOpacity && safeOpacity < 100) {
      // 提取 RGB 值
      let r = 0, g = 0, b = 0;
      
      if (safeColor.startsWith('#')) {
        const result = /^#([a-f0-9]{2})([a-f0-9]{2})([a-f0-9]{2})$/i.exec(safeColor);
        if (result) {
          r = parseInt(result[1], 16);
          g = parseInt(result[2], 16);
          b = parseInt(result[3], 16);
        }
      }
      
      const alpha = safeOpacity / 100;
      return `rgba(${r}, ${g}, ${b}, ${alpha})`;
    }
    
    return safeColor;
  }

  /**
   * 清理用戶輸入
   */
  private sanitizeInput(input: string): string {
    if (!input) return '';
    
    // 移除潛在的危險字符和限制長度
    return input
      .replace(/[<>"']/g, '') // 移除 HTML 特殊字符
      .replace(/javascript:/gi, '') // 移除 javascript: 協議
      .replace(/data:/gi, '') // 移除 data: 協議
      .trim()
      .substring(0, 100); // 限制長度
  }

  selectColor(color: string): void {
    const safeColor = this.getSafeColor(color);
    this.selectedColor = safeColor;
    this.customColor = safeColor;
  }

  onCustomColorChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    const safeColor = this.getSafeColor(target.value);
    this.selectedColor = safeColor;
    this.customColor = safeColor;
  }

  updatePreview(): void {
    // 更新預覽時自動選擇自訂顏色
    this.selectedColor = this.getSafeColor(this.customColor);
  }

  getPreviewColor(): string {
    return this.getSafePreviewColor();
  }

  getOpacityGradient(): string {
    return this.getSafeOpacityGradient();
  }

  confirm(): void {
    const finalColor = this.getSafePreviewColor();
    this.colorSelected.emit(finalColor);
    this.closeModal();
  }

  closeModal(): void {
    this.modalClose.emit();
  }

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.closeModal();
    }
  }

  // 修正不安全的正則表達式 - 使用更安全的模式
  private isValidHex(color: string): boolean {
    if (!color || color.length !== 7) return false;
    return /^#[0-9A-Fa-f]{6}$/.test(color);
  }

  private isValidRgb(color: string): boolean {
    if (!color || color.length > 30) return false;
    return /^rgb\(\s*(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\s*,\s*(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\s*,\s*(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\s*\)$/.test(color);
  }

  // 修正轉義字符問題 - 移除不必要的轉義
  private formatColorValue(value: string): string {
    return value.replace(/"/g, '"');
  }
} 