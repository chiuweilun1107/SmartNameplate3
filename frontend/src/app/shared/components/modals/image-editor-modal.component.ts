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
    <div class="modal-overlay" 
         (click)="onOverlayClick($event)" 
         (keydown.enter)="onOverlayClick($event)"
         (keydown.space)="onOverlayClick($event)"
         tabindex="0" 
         role="button" 
         *ngIf="isVisible">
      <div class="modal-container" 
           (click)="$event.stopPropagation()"
           (keydown.enter)="$event.stopPropagation()"
           (keydown.space)="$event.stopPropagation()"
           tabindex="0" 
           role="dialog"
           aria-label="圖片編輯對話框">
        <div class="modal-header">
          <h2 class="modal-title">圖片編輯器</h2>
          <button mat-icon-button class="modal-close-btn" (click)="closeModal()" (keydown.enter)="closeModal()" (keydown.space)="closeModal()" tabindex="0" role="button">
            <mat-icon>close</mat-icon>
          </button>
        </div>

        <div class="modal-content">
          <div class="filter-panel">
            <div class="filter-controls">
              <!-- 亮度 -->
              <div class="filter-control">
                <label for="brightness-slider">亮度：{{ filterSettings.brightness }}%</label>
                <input type="range" 
                       id="brightness-slider"
                       min="0" 
                       max="200" 
                       step="5" 
                       [(ngModel)]="filterSettings.brightness" 
                       (input)="onFilterChange()"
                       class="filter-slider">
              </div>

              <!-- 對比度 -->
              <div class="filter-control">
                <label for="contrast-slider">對比度：{{ filterSettings.contrast }}%</label>
                <input type="range" 
                       id="contrast-slider"
                       min="0" 
                       max="200" 
                       step="5" 
                       [(ngModel)]="filterSettings.contrast" 
                       (input)="onFilterChange()"
                       class="filter-slider">
              </div>

              <!-- 飽和度 -->
              <div class="filter-control">
                <label for="saturation-slider">飽和度：{{ filterSettings.saturation }}%</label>
                <input type="range" 
                       id="saturation-slider"
                       min="0" 
                       max="200" 
                       step="5" 
                       [(ngModel)]="filterSettings.saturation" 
                       (input)="onFilterChange()"
                       class="filter-slider">
              </div>

              <!-- 模糊 -->
              <div class="filter-control">
                <label for="blur-slider">模糊：{{ filterSettings.blur }}px</label>
                <input type="range" 
                       id="blur-slider"
                       min="0" 
                       max="10" 
                       step="0.5" 
                       [(ngModel)]="filterSettings.blur" 
                       (input)="onFilterChange()"
                       class="filter-slider">
              </div>

              <!-- 灰階 -->
              <div class="filter-control">
                <label for="grayscale-slider">灰階：{{ filterSettings.grayscale }}%</label>
                <input type="range" 
                       id="grayscale-slider"
                       min="0" 
                       max="100" 
                       step="5" 
                       [(ngModel)]="filterSettings.grayscale" 
                       (input)="onFilterChange()"
                       class="filter-slider">
              </div>

              <!-- 懷舊 -->
              <div class="filter-control">
                <label for="sepia-slider">懷舊：{{ filterSettings.sepia }}%</label>
                <input type="range" 
                       id="sepia-slider"
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
                  <button mat-button (click)="applyPreset('normal')" class="preset-btn" (keydown.enter)="applyPreset('normal')" (keydown.space)="applyPreset('normal')" tabindex="0" role="button">正常</button>
                  <button mat-button (click)="applyPreset('vintage')" class="preset-btn" (keydown.enter)="applyPreset('vintage')" (keydown.space)="applyPreset('vintage')" tabindex="0" role="button">復古</button>
                  <button mat-button (click)="applyPreset('warm')" class="preset-btn" (keydown.enter)="applyPreset('warm')" (keydown.space)="applyPreset('warm')" tabindex="0" role="button">暖色</button>
                  <button mat-button (click)="applyPreset('cool')" class="preset-btn" (keydown.enter)="applyPreset('cool')" (keydown.space)="applyPreset('cool')" tabindex="0" role="button">冷色</button>
                  <button mat-button (click)="applyPreset('dramatic')" class="preset-btn" (keydown.enter)="applyPreset('dramatic')" (keydown.space)="applyPreset('dramatic')" tabindex="0" role="button">戲劇化</button>
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
          <button mat-raised-button color="primary" (click)="confirm()" (keydown.enter)="confirm()" (keydown.space)="confirm()" tabindex="0" role="button">
            確認
          </button>
          <button mat-button (click)="resetChanges()" (keydown.enter)="resetChanges()" (keydown.space)="resetChanges()" tabindex="0" role="button">
            重設
          </button>
          <button mat-button (click)="closeModal()" (keydown.enter)="closeModal()" (keydown.space)="closeModal()" tabindex="0" role="button">
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
  @Output() modalClose = new EventEmitter<void>();

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

      // 解析各種濾鏡參數 - 使用安全的正則表達式（避免ReDoS攻擊）
      if (filterString.length < 1000) { // 限制長度
        const brightnessMatch = filterString.match(/brightness\(([0-9]{1,3}(?:\.[0-9]{1,2})?)%?\)/);
        if (brightnessMatch && brightnessMatch[1]) {
          this.filterSettings.brightness = Math.min(500, parseFloat(brightnessMatch[1]));
      }

        const contrastMatch = filterString.match(/contrast\(([0-9]{1,3}(?:\.[0-9]{1,2})?)%?\)/);
        if (contrastMatch && contrastMatch[1]) {
          this.filterSettings.contrast = Math.min(500, parseFloat(contrastMatch[1]));
      }

        const saturateMatch = filterString.match(/saturate\(([0-9]{1,3}(?:\.[0-9]{1,2})?)%?\)/);
        if (saturateMatch && saturateMatch[1]) {
          this.filterSettings.saturation = Math.min(500, parseFloat(saturateMatch[1]));
      }

        const blurMatch = filterString.match(/blur\(([0-9]{1,2}(?:\.[0-9]{1,2})?)px\)/);
        if (blurMatch && blurMatch[1]) {
          this.filterSettings.blur = Math.min(20, parseFloat(blurMatch[1]));
      }

        const grayscaleMatch = filterString.match(/grayscale\(([0-9]{1,3}(?:\.[0-9]{1,2})?)%?\)/);
        if (grayscaleMatch && grayscaleMatch[1]) {
          this.filterSettings.grayscale = Math.min(100, parseFloat(grayscaleMatch[1]));
      }

        const sepiaMatch = filterString.match(/sepia\(([0-9]{1,3}(?:\.[0-9]{1,2})?)%?\)/);
        if (sepiaMatch && sepiaMatch[1]) {
          this.filterSettings.sepia = Math.min(100, parseFloat(sepiaMatch[1]));
        }
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
    this.modalClose.emit();
  }

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.closeModal();
    }
  }

  // 修正不安全的正則表達式 - 使用邊界和長度檢查
  private validateFilterValue(value: string): boolean {
    if (!value || value.length > 3) return false;
    return /^[0-9]{1,3}$/.test(value);
  }

  private validateHslValue(value: string): boolean {
    if (!value || value.length > 3) return false;
    return /^[0-9]{1,3}$/.test(value);
  }

  private validatePercentValue(value: string): boolean {
    if (!value || value.length > 4) return false;
    return /^[0-9]{1,3}%$/.test(value);
  }

  private validatePixelValue(value: string): boolean {
    if (!value || value.length > 6) return false;
    return /^[0-9]{1,4}px$/.test(value);
  }

  private validateRotateValue(value: string): boolean {
    if (!value || value.length > 6) return false;
    return /^[0-9]{1,3}deg$/.test(value);
  }

  private validateScaleValue(value: string): boolean {
    if (!value || value.length > 5) return false;
    return /^[0-9]{1,2}(?:\.[0-9]{1,2})?$/.test(value);
  }
} 