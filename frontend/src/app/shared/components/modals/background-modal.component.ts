import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { BackgroundApiService, BackgroundImage } from '../../../features/cards/services/background-api.service';
import { CustomColorApiService, CustomColor } from '../../../features/cards/services/custom-color-api.service';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';
import { TagButtonComponent } from '../tags/tag-button.component';

export interface BackgroundOption {
  type: 'color' | 'gradient' | 'image';
  value: string;
  name: string;
  preview?: string;
  isTemporary?: boolean;
  id?: number; // 用於刪除背景圖片或自訂顏色
  isCustomColor?: boolean; // 標記是否為自訂顏色
}

@Component({
  selector: 'sn-background-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    FormsModule,
    DeleteButtonComponent,
    TagButtonComponent
  ],
  template: `
    <div class="modal-overlay" (click)="onOverlayClick($event)">
      <div class="modal-container" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2 class="modal-title">設定背景</h2>
          <button mat-icon-button class="modal-close-btn" (click)="close.emit()">
            <mat-icon>close</mat-icon>
          </button>
        </div>

        <div class="modal-content">
          <div class="background-tabs">
            <sn-tag-button
              *ngFor="let tab of tabs"
              [label]="tab.label"
              [icon]="tab.icon"
              [isActive]="selectedTab === tab.type"
              [value]="tab.type"
              (tagClick)="selectTab($event)">
            </sn-tag-button>
          </div>

          <div class="background-options">
            <!-- 純色背景 -->
            <div *ngIf="selectedTab === 'color'" class="color-section">
              <div class="color-grid">
                <div
                  *ngFor="let color of colorOptions"
                  class="color-option"
                  [style.background-color]="color.value"
                  [class.selected]="selectedBackground?.value === color.value"
                  [class.white-border]="color.value === '#ffffff'"
                  (click)="selectBackground(color)">
                  <!-- 如果是自訂顏色，顯示刪除按鈕 -->
                  <sn-delete-button
                    *ngIf="color.isCustomColor"
                    size="small"
                    tooltip="刪除顏色"
                    class="custom-color-delete"
                    (click)="deleteCustomColor(color.id); $event.stopPropagation()">
                  </sn-delete-button>
                </div>
                
                <!-- 新增自訂顏色按鈕 -->
                <div class="add-custom-color" (click)="showCustomColorPicker()">
                  <mat-icon>add</mat-icon>
                </div>
              </div>

              <!-- 自訂顏色選擇器 -->
              <div *ngIf="showCustomColorConfirm" class="custom-color-picker">
                <div class="color-picker-container">
                  <input 
                    type="color" 
                    [(ngModel)]="customColor" 
                    class="color-picker-input">
                </div>
                <div class="custom-color-actions">
                  <button mat-button (click)="cancelCustomColor()">取消</button>
                  <button mat-raised-button color="primary" (click)="confirmCustomColor()">新增</button>
                </div>
              </div>
            </div>

            <!-- 圖片背景 -->
            <div *ngIf="selectedTab === 'image'" class="image-section">
              <div class="background-grid">
                <!-- 上傳區域放在第一個位置 -->
                <div class="background-item upload-item" (click)="triggerFileInput()">
                  <div class="upload-area">
                    <mat-icon>cloud_upload</mat-icon>
                    <p>點擊上傳背景圖片</p>
                  </div>
                  <input
                    #fileInput
                    type="file"
                    accept="image/*"
                    (change)="onFileSelected($event)"
                    style="display: none;">
                </div>

                <!-- 已儲存的背景圖片（按先右後下的順序排列） -->
                <div
                  *ngFor="let background of savedBackgrounds"
                  class="background-item"
                  [class.selected]="selectedBackground?.value === background.value"
                  (click)="selectBackground(background)">
                  <div class="background-thumbnail">
                    <img [src]="background.preview" [alt]="background.name">
                    <div class="background-actions">
                      <sn-delete-button
                        size="small"
                        tooltip="刪除背景"
                        (delete)="deleteBackground(background)">
                      </sn-delete-button>
                    </div>
                  </div>
                  <div class="background-info">
                    <h3 class="background-name">{{ background.name }}</h3>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button
            mat-raised-button
            color="primary"
            [disabled]="!selectedBackground"
            (click)="applyBackground()">
            套用背景
          </button>
          <button mat-button class="apple-confirm-btn" (click)="close.emit()">確認</button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./background-modal.component.scss']
})
export class BackgroundModalComponent implements OnInit {
  @Input() isVisible = false;
  @Output() close = new EventEmitter<void>();
  @Output() backgroundSelected = new EventEmitter<BackgroundOption>();

  selectedTab: 'color' | 'image' = 'color';
  selectedBackground: BackgroundOption | null = null;
  customColor = '#ffffff';
  showCustomColorConfirm = false;
  lastCustomColor = '#ffffff';

  constructor(
    private backgroundApiService: BackgroundApiService,
    private customColorApiService: CustomColorApiService
  ) {}

  tabs = [
    { type: 'color' as const, label: '純色', icon: 'palette' },
    { type: 'image' as const, label: '圖片', icon: 'image' }
  ];

  colorOptions: BackgroundOption[] = [
    // 第一行：基礎色彩
    { type: 'color', value: '#ffffff', name: '白色' },
    { type: 'color', value: '#f5f5f5', name: '淺灰' },
    { type: 'color', value: '#e0e0e0', name: '灰色' },
    { type: 'color', value: '#9e9e9e', name: '中灰' },
    { type: 'color', value: '#666666', name: '深灰' },
    { type: 'color', value: '#333333', name: '暗灰' },
    { type: 'color', value: '#000000', name: '黑色' },
    { type: 'color', value: '#f44336', name: '紅色' },
    { type: 'color', value: '#e91e63', name: '粉紅' },
    { type: 'color', value: '#9c27b0', name: '紫色' },
    { type: 'color', value: '#673ab7', name: '深紫' },
    { type: 'color', value: '#3f51b5', name: '靛藍' },
    // 第二行：亮色系
    { type: 'color', value: '#2196f3', name: '藍色' },
    { type: 'color', value: '#03a9f4', name: '淺藍' },
    { type: 'color', value: '#00bcd4', name: '青色' },
    { type: 'color', value: '#009688', name: '藍綠' },
    { type: 'color', value: '#4caf50', name: '綠色' },
    { type: 'color', value: '#8bc34a', name: '淺綠' },
    { type: 'color', value: '#cddc39', name: '黃綠' },
    { type: 'color', value: '#ffeb3b', name: '黃色' },
    { type: 'color', value: '#ffc107', name: '琥珀' },
    { type: 'color', value: '#ff9800', name: '橙色' },
    { type: 'color', value: '#ff5722', name: '深橙' },
    { type: 'color', value: '#795548', name: '棕色' }
  ];

  savedBackgrounds: BackgroundOption[] = [];

  ngOnInit(): void {
    this.loadSavedBackgrounds();
    this.loadCustomColors();
  }

  selectTab(tab: 'color' | 'image'): void {
    this.selectedTab = tab;
    this.selectedBackground = null;
  }

  selectBackground(background: BackgroundOption): void {
    this.selectedBackground = background;
  }

  onCustomColorChange(): void {
    // 檢查顏色是否有變化
    if (this.customColor !== this.lastCustomColor) {
      // 檢查是否已存在相同顏色
      const existingColor = this.colorOptions.find(color => color.value === this.customColor);
      if (!existingColor) {
        this.showCustomColorConfirm = true;
      } else {
        this.showCustomColorConfirm = false;
        this.selectBackground(existingColor);
      }
      this.lastCustomColor = this.customColor;
    }
  }

  showCustomColorPicker(): void {
    this.showCustomColorConfirm = true;
    this.customColor = '#ffffff';
  }

  cancelCustomColor(): void {
    this.showCustomColorConfirm = false;
    this.customColor = '#ffffff';
  }

  confirmCustomColor(): void {
    const colorName = `自訂色 ${this.customColor}`;
    
    // 調用API儲存到資料庫
    this.customColorApiService.createCustomColor({
      name: colorName,
      colorValue: this.customColor,
      createdBy: 'current-user'
    }).subscribe({
      next: (savedColor) => {
        // 添加到顏色選項
        const customBackground: BackgroundOption = {
          type: 'color',
          value: this.customColor,
          name: colorName,
          id: savedColor.id,
          isCustomColor: true
        };
        
        this.colorOptions.push(customBackground);
        this.selectBackground(customBackground);
        this.showCustomColorConfirm = false;
        
        console.log('自訂顏色儲存成功:', savedColor);
      },
      error: (error) => {
        console.error('儲存自訂顏色失敗:', error);
        if (error.error?.message === '此顏色已存在') {
          alert('此顏色已存在！');
        } else {
          alert('儲存自訂顏色失敗，請稍後再試。');
        }
        this.showCustomColorConfirm = false;
      }
    });
  }

  deleteCustomColor(colorId: number | undefined): void {
    if (colorId && confirm(`確定要刪除顏色嗎？`)) {
      this.customColorApiService.deleteCustomColor(colorId).subscribe({
        next: () => {
          this.colorOptions = this.colorOptions.filter(c => c.id !== colorId);
          if (this.selectedBackground?.id === colorId) {
            this.selectedBackground = null;
          }
        },
        error: (error) => {
          console.error('刪除自訂顏色失敗:', error);
          alert('刪除自訂顏色失敗，請稍後再試。');
        }
      });
    }
  }

  private loadCustomColors(): void {
    this.customColorApiService.getCustomColors().subscribe({
      next: (colors) => {
        const customColorOptions = colors.map(color => ({
          type: 'color' as const,
          value: color.colorValue,
          name: color.name,
          id: color.id,
          isCustomColor: true
        }));
        
        this.colorOptions = [...this.colorOptions, ...customColorOptions];
      },
      error: (error) => {
        console.error('載入自訂顏色失敗:', error);
      }
    });
  }

  triggerFileInput(): void {
    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    fileInput?.click();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => {
        const imageUrl = e.target?.result as string;
        const imageBackground: BackgroundOption = {
          type: 'image',
          value: `url(${imageUrl})`,
          name: file.name,
          preview: imageUrl,
          isTemporary: true // 先標記為臨時，儲存成功後會更新
        };

        // 添加到已儲存背景列表的最前面（新的在左邊）
        this.savedBackgrounds.unshift(imageBackground);
        this.selectBackground(imageBackground);

        // 直接儲存到資料庫
        this.saveBackgroundToDatabase(file.name, imageUrl, imageBackground);
      };
      reader.readAsDataURL(file);
    }
  }

  private loadSavedBackgrounds(): void {
    this.backgroundApiService.getBackgroundImages().subscribe({
      next: (backgrounds) => {
        // 按照創建時間倒序排列（新的在前面）
        const sortedBackgrounds = backgrounds.sort((a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );

        this.savedBackgrounds = sortedBackgrounds.map(bg => ({
          type: 'image' as const,
          value: `url(${bg.imageUrl})`,
          name: bg.name,
          preview: bg.imageUrl,
          id: bg.id
        }));
      },
      error: (error) => {
        console.error('載入背景圖片失敗:', error);
      }
    });
  }

  private saveBackgroundToDatabase(fileName: string, imageUrl: string, background: BackgroundOption): void {
    const backgroundData = {
      name: fileName,
      imageUrl: imageUrl,
      category: 'uploaded',
      isPublic: true
    };

    this.backgroundApiService.createBackgroundImage(backgroundData).subscribe({
      next: (savedBackground) => {
        console.log('背景圖片儲存成功:', savedBackground);
        // 更新背景物件的ID和移除臨時標記
        background.id = savedBackground.id;
        background.isTemporary = false;
      },
      error: (error) => {
        console.error('儲存背景圖片失敗:', error);
        alert('儲存背景圖片失敗，請稍後再試。');
        // 儲存失敗時從列表中移除
        this.savedBackgrounds = this.savedBackgrounds.filter(bg => bg !== background);
        if (this.selectedBackground === background) {
          this.selectedBackground = null;
        }
      }
    });
  }

  deleteBackground(background: BackgroundOption): void {
    if (background.isTemporary) {
      // 直接從列表中移除臨時背景
      if (confirm(`確定要刪除背景「${background.name}」嗎？`)) {
        this.savedBackgrounds = this.savedBackgrounds.filter(bg => bg !== background);
        if (this.selectedBackground === background) {
          this.selectedBackground = null;
        }
      }
    } else if (background.id && confirm(`確定要刪除背景「${background.name}」嗎？`)) {
      // 從資料庫刪除已儲存的背景
      this.backgroundApiService.deleteBackgroundImage(background.id).subscribe({
        next: () => {
          this.savedBackgrounds = this.savedBackgrounds.filter(bg => bg.id !== background.id);
          if (this.selectedBackground?.id === background.id) {
            this.selectedBackground = null;
          }
        },
        error: (error) => {
          console.error('刪除背景失敗:', error);
          alert('刪除背景失敗，請稍後再試。');
        }
      });
    }
  }

  applyBackground(): void {
    if (this.selectedBackground) {
      this.backgroundSelected.emit(this.selectedBackground);
      this.close.emit();
    }
  }

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.close.emit();
    }
  }
}
