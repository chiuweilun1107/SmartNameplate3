import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSliderModule } from '@angular/material/slider';
import { MatTabsModule } from '@angular/material/tabs';
import { FormsModule } from '@angular/forms';

export interface ImageEditSettings {
  src: string;
  filter?: string;
  cropData?: {
    x: number;
    y: number;
    width: number;
    height: number;
  };
}

export interface FilterSettings {
  brightness: number;
  contrast: number;
  saturation: number;
  blur: number;
  grayscale: number;
  sepia: number;
}

@Component({
  selector: 'sn-image-editor-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatSliderModule,
    MatTabsModule,
    FormsModule
  ],
  template: `
    <div class="modal-overlay" (click)="onOverlayClick($event)" *ngIf="isVisible">
      <div class="modal-container" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2 class="modal-title">圖片編輯器</h2>
          <button mat-icon-button class="modal-close-btn" (click)="closeModal()">
            <mat-icon>close</mat-icon>
          </button>
        </div>

        <div class="modal-content">
          <div class="filter-panel">
            <div class="filter-controls">
              <!-- 亮度 -->
              <div class="filter-control">
                <label>亮度：{{ filterSettings.brightness }}%</label>
                <input type="range" 
                       min="0" 
                       max="200" 
                       step="5" 
                       [(ngModel)]="filterSettings.brightness" 
                       (input)="onFilterChange()"
                       class="filter-slider">
              </div>

              <!-- 對比度 -->
              <div class="filter-control">
                <label>對比度：{{ filterSettings.contrast }}%</label>
                <input type="range" 
                       min="0" 
                       max="200" 
                       step="5" 
                       [(ngModel)]="filterSettings.contrast" 
                       (input)="onFilterChange()"
                       class="filter-slider">
              </div>

              <!-- 飽和度 -->
              <div class="filter-control">
                <label>飽和度：{{ filterSettings.saturation }}%</label>
                <input type="range" 
                       min="0" 
                       max="200" 
                       step="5" 
                       [(ngModel)]="filterSettings.saturation" 
                       (input)="onFilterChange()"
                       class="filter-slider">
              </div>

              <!-- 模糊 -->
              <div class="filter-control">
                <label>模糊：{{ filterSettings.blur }}px</label>
                <input type="range" 
                       min="0" 
                       max="10" 
                       step="0.5" 
                       [(ngModel)]="filterSettings.blur" 
                       (input)="onFilterChange()"
                       class="filter-slider">
              </div>

              <!-- 灰階 -->
              <div class="filter-control">
                <label>灰階：{{ filterSettings.grayscale }}%</label>
                <input type="range" 
                       min="0" 
                       max="100" 
                       step="5" 
                       [(ngModel)]="filterSettings.grayscale" 
                       (input)="onFilterChange()"
                       class="filter-slider">
              </div>

              <!-- 懷舊 -->
              <div class="filter-control">
                <label>懷舊：{{ filterSettings.sepia }}%</label>
                <input type="range" 
                       min="0" 
                       max="100" 
                       step="5" 
                       [(ngModel)]="filterSettings.sepia" 
                       (input)="onFilterChange()"
                       class="filter-slider">
              </div>

              <!-- 預設濾鏡 -->
              <div class="preset-filters">
                <h3>預設濾鏡</h3>
                <div class="filter-presets">
                  <button mat-button (click)="applyPreset('normal')" class="preset-btn">正常</button>
                  <button mat-button (click)="applyPreset('vintage')" class="preset-btn">復古</button>
                  <button mat-button (click)="applyPreset('warm')" class="preset-btn">暖色</button>
                  <button mat-button (click)="applyPreset('cool')" class="preset-btn">冷色</button>
                  <button mat-button (click)="applyPreset('dramatic')" class="preset-btn">戲劇化</button>
                </div>
              </div>
            </div>

            <!-- 預覽區域 -->
            <div class="preview-area">
              <h3>預覽</h3>
              <div class="image-preview">
                <img [src]="currentSettings.src" 
                     [style.filter]="getFilterString()"
                     alt="預覽圖片">
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button mat-raised-button color="primary" (click)="confirm()">
            確認
          </button>
          <button mat-button (click)="resetChanges()">
            重設
          </button>
          <button mat-button (click)="closeModal()">
            取消
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./image-editor-modal.component.scss']
})
export class ImageEditorModalComponent implements OnInit {
  @Input() isVisible = false;
  @Input() currentSettings: ImageEditSettings = { src: '' };
  @Output() settingsChanged = new EventEmitter<ImageEditSettings>();
  @Output() close = new EventEmitter<void>();

  filterSettings: FilterSettings = {
    brightness: 100,
    contrast: 100,
    saturation: 100,
    blur: 0,
    grayscale: 0,
    sepia: 0
  };

  originalFilterSettings: FilterSettings = { ...this.filterSettings };

  ngOnInit(): void {
    this.parseExistingFilter();
    this.originalFilterSettings = { ...this.filterSettings };
  }

  parseExistingFilter(): void {
    if (this.currentSettings.filter) {
      const filterString = this.currentSettings.filter;

      // 解析各種濾鏡參數
      const brightnessMatch = filterString.match(/brightness\((\d+(?:\.\d+)?)%?\)/);
      if (brightnessMatch) {
        this.filterSettings.brightness = parseFloat(brightnessMatch[1]);
      }

      const contrastMatch = filterString.match(/contrast\((\d+(?:\.\d+)?)%?\)/);
      if (contrastMatch) {
        this.filterSettings.contrast = parseFloat(contrastMatch[1]);
      }

      const saturateMatch = filterString.match(/saturate\((\d+(?:\.\d+)?)%?\)/);
      if (saturateMatch) {
        this.filterSettings.saturation = parseFloat(saturateMatch[1]);
      }

      const blurMatch = filterString.match(/blur\((\d+(?:\.\d+)?)px\)/);
      if (blurMatch) {
        this.filterSettings.blur = parseFloat(blurMatch[1]);
      }

      const grayscaleMatch = filterString.match(/grayscale\((\d+(?:\.\d+)?)%?\)/);
      if (grayscaleMatch) {
        this.filterSettings.grayscale = parseFloat(grayscaleMatch[1]);
      }

      const sepiaMatch = filterString.match(/sepia\((\d+(?:\.\d+)?)%?\)/);
      if (sepiaMatch) {
        this.filterSettings.sepia = parseFloat(sepiaMatch[1]);
      }
    }
  }

  onFilterChange(): void {
    // 即時更新預覽，確保濾鏡滑桿變更時能立即看到效果
    console.log('濾鏡設定變更:', this.filterSettings);
  }

  getFilterString(): string {
    const filters = [];
    
    if (this.filterSettings.brightness !== 100) {
      filters.push(`brightness(${this.filterSettings.brightness}%)`);
    }
    if (this.filterSettings.contrast !== 100) {
      filters.push(`contrast(${this.filterSettings.contrast}%)`);
    }
    if (this.filterSettings.saturation !== 100) {
      filters.push(`saturate(${this.filterSettings.saturation}%)`);
    }
    if (this.filterSettings.blur > 0) {
      filters.push(`blur(${this.filterSettings.blur}px)`);
    }
    if (this.filterSettings.grayscale > 0) {
      filters.push(`grayscale(${this.filterSettings.grayscale}%)`);
    }
    if (this.filterSettings.sepia > 0) {
      filters.push(`sepia(${this.filterSettings.sepia}%)`);
    }

    return filters.join(' ');
  }

  applyPreset(preset: string): void {
    switch (preset) {
      case 'normal':
        this.filterSettings = {
          brightness: 100,
          contrast: 100,
          saturation: 100,
          blur: 0,
          grayscale: 0,
          sepia: 0
        };
        break;
      case 'vintage':
        this.filterSettings = {
          brightness: 110,
          contrast: 120,
          saturation: 80,
          blur: 0,
          grayscale: 0,
          sepia: 30
        };
        break;
      case 'warm':
        this.filterSettings = {
          brightness: 105,
          contrast: 105,
          saturation: 120,
          blur: 0,
          grayscale: 0,
          sepia: 10
        };
        break;
      case 'cool':
        this.filterSettings = {
          brightness: 95,
          contrast: 110,
          saturation: 90,
          blur: 0,
          grayscale: 0,
          sepia: 0
        };
        break;
      case 'dramatic':
        this.filterSettings = {
          brightness: 90,
          contrast: 150,
          saturation: 130,
          blur: 0,
          grayscale: 0,
          sepia: 0
        };
        break;
    }
    // 套用預設濾鏡後觸發變更
    this.onFilterChange();
  }

  confirm(): void {
    const updatedSettings: ImageEditSettings = {
      ...this.currentSettings,
      filter: this.getFilterString()
    };
    
    this.settingsChanged.emit(updatedSettings);
    this.closeModal();
  }

  resetChanges(): void {
    this.filterSettings = { ...this.originalFilterSettings };
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