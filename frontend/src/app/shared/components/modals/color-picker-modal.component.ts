import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';

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
    <div class="modal-overlay" (click)="onOverlayClick($event)" *ngIf="isVisible">
      <div class="modal-container" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2 class="modal-title">{{ title }}</h2>
          <button mat-icon-button class="modal-close-btn" (click)="closeModal()">
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
                class="color-item"
                [style.background-color]="color"
                [class.selected]="selectedColor === color"
                (click)="selectColor(color)">
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
              <div class="color-value">{{ customColor }}</div>
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
                [style.background]="getOpacityGradient()">
              <span class="opacity-value">{{ opacity }}%</span>
            </div>
          </div>

          <!-- 顏色預覽 -->
          <div class="color-section">
            <h3>預覽</h3>
            <div class="color-preview" [style.background-color]="getPreviewColor()">
              <span>範例文字</span>
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
  styleUrls: ['./color-picker-modal.component.scss']
})
export class ColorPickerModalComponent implements OnInit {
  @Input() isVisible = false;
  @Input() title = '選擇顏色';
  @Input() currentColor = '#000000';
  @Input() showOpacity = false;
  @Output() colorSelected = new EventEmitter<string>();
  @Output() close = new EventEmitter<void>();

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

  ngOnInit(): void {
    this.selectedColor = this.currentColor;
    this.customColor = this.currentColor;
    
    // 如果當前顏色包含透明度，提取透明度值
    if (this.currentColor.includes('rgba')) {
      const rgba = this.currentColor.match(/rgba?\((\d+),\s*(\d+),\s*(\d+)(?:,\s*([\d.]+))?\)/);
      if (rgba && rgba[4]) {
        this.opacity = Math.round(parseFloat(rgba[4]) * 100);
      }
    }
  }

  selectColor(color: string): void {
    this.selectedColor = color;
    this.customColor = color;
  }

  onCustomColorChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.selectedColor = target.value;
    this.customColor = target.value;
  }

  updatePreview(): void {
    // 更新預覽時自動選擇自訂顏色
    this.selectedColor = this.customColor;
  }

  getPreviewColor(): string {
    if (this.showOpacity && this.opacity < 100) {
      const hex = this.selectedColor;
      const r = parseInt(hex.slice(1, 3), 16);
      const g = parseInt(hex.slice(3, 5), 16);
      const b = parseInt(hex.slice(5, 7), 16);
      const alpha = this.opacity / 100;
      return `rgba(${r}, ${g}, ${b}, ${alpha})`;
    }
    return this.selectedColor;
  }

  getOpacityGradient(): string {
    const hex = this.selectedColor;
    const r = parseInt(hex.slice(1, 3), 16);
    const g = parseInt(hex.slice(3, 5), 16);
    const b = parseInt(hex.slice(5, 7), 16);
    return `linear-gradient(to right, rgba(${r},${g},${b},0), rgba(${r},${g},${b},1))`;
  }

  confirm(): void {
    const finalColor = this.getPreviewColor();
    this.colorSelected.emit(finalColor);
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