import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'sn-simple-color-picker-modal',
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
          <!-- 預設顏色面板 -->
          <div class="color-palette">
            <h3>常用顏色</h3>
            <div class="color-grid">
              <div 
                *ngFor="let color of presetColors"
                class="color-option"
                [style.backgroundColor]="color"
                [class.selected]="getColorHex() === color"
                (click)="selectColor(color)"
                [title]="color">
              </div>
            </div>
          </div>

          <!-- 自定義顏色輸入 -->
          <div class="custom-color">
            <h3>自定義顏色</h3>
            <div class="color-input-group">
              <input 
                type="color" 
                [ngModel]="getColorHex()"
                (ngModelChange)="updateColor($event)"
                class="color-picker-input">
              <input 
                type="text" 
                [ngModel]="getColorHex()"
                (ngModelChange)="updateColor($event)"
                class="color-text-input"
                placeholder="#ffffff">
            </div>
          </div>

          <!-- 透明度設定 -->
          <div class="opacity-section">
            <h3>透明度</h3>
            <div class="opacity-slider">
              <input 
                type="range" 
                min="0" 
                max="100" 
                [(ngModel)]="opacity"
                (input)="updateOpacity()"
                class="slider"
                [style.background]="getOpacityGradient()">
              <span class="opacity-value">{{ opacity }}%</span>
            </div>
          </div>

          <!-- 目前選擇的顏色預覽 -->
          <div class="current-color">
            <h3>目前選擇</h3>
            <div 
              class="color-preview"
              [style.backgroundColor]="getFinalColor()">
              {{ getFinalColor() }}
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
  styleUrls: ['./simple-color-picker-modal.component.scss']
})
export class SimpleColorPickerModalComponent {
  @Input() isVisible = false;
  @Input() title = '選擇顏色';
  @Input() currentColor = '#e3f2fd';
  @Output() colorSelected = new EventEmitter<string>();
  @Output() close = new EventEmitter<void>();

  selectedColor = '#e3f2fd';
  opacity = 100;

  presetColors = [
    '#000000', '#ffffff', '#ff0000', '#00ff00', '#0000ff', '#ffff00',
    '#ff00ff', '#00ffff', '#808080', '#800000', '#008000', '#000080',
    '#808000', '#800080', '#008080', '#c0c0c0', '#ffa500', '#ffc0cb',
    '#e3f2fd', '#f3e5f5', '#e8f5e8', '#fff3e0', '#ffebee', '#f1f8e9'
  ];

  ngOnInit(): void {
    this.parseCurrentColor();
  }

  private parseCurrentColor(): void {
    if (this.currentColor.includes('rgba')) {
      // 解析 rgba 格式
      const rgba = this.currentColor.match(/rgba?\((\d+),\s*(\d+),\s*(\d+)(?:,\s*([\d.]+))?\)/);
      if (rgba) {
        const r = parseInt(rgba[1]);
        const g = parseInt(rgba[2]);
        const b = parseInt(rgba[3]);
        this.opacity = rgba[4] ? Math.round(parseFloat(rgba[4]) * 100) : 100;
        this.selectedColor = `#${r.toString(16).padStart(2, '0')}${g.toString(16).padStart(2, '0')}${b.toString(16).padStart(2, '0')}`;
      }
    } else {
      this.selectedColor = this.currentColor;
      this.opacity = 100;
    }
  }

  getColorHex(): string {
    return this.selectedColor;
  }

  updateColor(color: string): void {
    this.selectedColor = color;
  }

  updateOpacity(): void {
    // 透明度變更時的回調
  }

  selectColor(color: string): void {
    this.selectedColor = color;
  }

  getFinalColor(): string {
    if (this.opacity < 100) {
      const hex = this.selectedColor;
      const r = parseInt(hex.slice(1, 3), 16);
      const g = parseInt(hex.slice(3, 5), 16);
      const b = parseInt(hex.slice(5, 7), 16);
      const alpha = this.opacity / 100;
      return `rgba(${r}, ${g}, ${b}, ${alpha})`;
    }
    return this.selectedColor;
  }

  confirm(): void {
    this.colorSelected.emit(this.getFinalColor());
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

  getOpacityGradient(): string {
    const hex = this.getColorHex();
    const r = parseInt(hex.slice(1, 3), 16);
    const g = parseInt(hex.slice(3, 5), 16);
    const b = parseInt(hex.slice(5, 7), 16);
    return `linear-gradient(to right, rgba(${r},${g},${b},0), rgba(${r},${g},${b},1))`;
  }
} 