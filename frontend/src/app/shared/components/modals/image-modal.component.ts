import { Component, EventEmitter, Input, OnInit, Output, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TagButtonComponent } from '../tags/tag-button.component';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';
import { ElementImageApiService, ElementImage } from '../../../features/cards/services/element-image-api.service';

export interface ImageOption {
  id?: number;
  name: string;
  url: string;
  category: string;
  preview?: string;
  isTemporary?: boolean;
}

@Component({
  selector: 'sn-image-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    TagButtonComponent,
    DeleteButtonComponent
  ],
  template: `
    <div class="modal-overlay" (click)="onOverlayClick($event)">
      <div class="modal-container" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2 class="modal-title">選擇圖片</h2>
          <button mat-icon-button class="modal-close-btn" (click)="closeModal()">
            <mat-icon>close</mat-icon>
          </button>
        </div>

        <div class="modal-content">
          <!-- 分類標籤 -->
          <div class="modal-categories">
            <sn-tag-button
              *ngFor="let category of categories"
              [label]="category.label"
              [icon]="category.icon"
              [isActive]="selectedCategory === category.label"
              (tagClick)="selectCategory($event)">
            </sn-tag-button>
          </div>

          <!-- 圖片網格 -->
          <div class="modal-grid">
            <!-- 上傳區域放在第一個位置 -->
            <div class="modal-item upload-item" (click)="triggerFileInput()">
              <div class="upload-area">
                <mat-icon>cloud_upload</mat-icon>
                <p>點擊上傳圖片</p>
              </div>
              <input
                #fileInput
                type="file"
                accept="image/*"
                (change)="onFileSelected($event)"
                style="display: none;">
            </div>

            <!-- 已儲存的圖片 -->
            <div
              *ngFor="let image of filteredImages"
              class="modal-item"
              [class.selected]="selectedImage?.id === image.id"
              (click)="selectImage(image)">
              <div class="modal-item-actions">
                <sn-delete-button
                  *ngIf="!image.isTemporary"
                  size="small"
                  tooltip="刪除圖片"
                  (delete)="deleteImage(image)">
                </sn-delete-button>
              </div>
              <div class="modal-item-thumbnail">
                <img [src]="image.preview || image.url" [alt]="image.name" />
              </div>
              <div class="modal-item-info">
                <h3 class="modal-item-name">{{ image.name }}</h3>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button
            mat-raised-button
            color="primary"
            [disabled]="!selectedImage"
            (click)="confirm()">
            載入圖片
          </button>
          <button mat-button class="apple-confirm-btn" (click)="closeModal()">確認</button>
        </div>
      </div>
    </div>
  `
})
export class ImageModalComponent implements OnInit {
  @Input() isVisible = false;
  @Output() imageSelected = new EventEmitter<ImageOption>();
  @Output() close = new EventEmitter<void>();
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  images: ImageOption[] = [];
  filteredImages: ImageOption[] = [];
  categories = [
    { label: '全部', icon: 'apps' },
    { label: '人物', icon: 'person' },
    { label: '標誌', icon: 'business' },
    { label: '產品', icon: 'inventory' },
    { label: '背景', icon: 'wallpaper' },
    { label: '裝飾', icon: 'star' }
  ];
  selectedCategory = '全部';
  selectedImage: ImageOption | null = null;

  constructor(private elementImageApiService: ElementImageApiService) {}

  ngOnInit(): void {
    this.loadImages();
  }

  loadImages(): void {
    // 從ElementImage API載入圖片
    this.elementImageApiService.getElementImages().subscribe({
      next: (elementImages) => {
        // 按照創建時間倒序排列（新的在前面）
        const sortedImages = elementImages.sort((a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );

        this.images = sortedImages.map(ei => ({
          id: ei.id,
          name: ei.name,
          url: ei.imageUrl,
          category: ei.category || '全部',
          preview: ei.thumbnailUrl || ei.imageUrl
        }));
        this.filterImages();
      },
      error: (error) => {
        console.error('載入圖片失敗:', error);
        // fallback到示例數據
        this.images = [
          {
            id: 1,
            name: '商務人士',
            url: '/assets/images/business-person.jpg',
            category: '人物',
            preview: '/assets/images/business-person-thumb.jpg'
          },
          {
            id: 2,
            name: '公司標誌',
            url: '/assets/images/logo.jpg',
            category: '標誌',
            preview: '/assets/images/logo-thumb.jpg'
          },
          {
            id: 3,
            name: '產品圖片',
            url: '/assets/images/product.jpg',
            category: '產品',
            preview: '/assets/images/product-thumb.jpg'
          }
        ];
        this.filterImages();
      }
    });
  }

  selectCategory(category: string): void {
    this.selectedCategory = category;
    this.filterImages();
  }

  filterImages(): void {
    if (this.selectedCategory === '全部') {
      this.filteredImages = this.images;
    } else {
      this.filteredImages = this.images.filter(img => img.category === this.selectedCategory);
    }
  }

  selectImage(image: ImageOption): void {
    this.selectedImage = image;
  }

  triggerFileInput(): void {
    // 清除先前的值以允許選擇相同檔案
    if (this.fileInput?.nativeElement) {
      this.fileInput.nativeElement.value = '';
      this.fileInput.nativeElement.click();
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      return;
    }

    // 檢查文件類型
    if (!file.type.startsWith('image/')) {
      alert('請選擇圖片文件！');
      input.value = ''; // 清除輸入
      return;
    }

    // 檢查文件大小 (限制為2MB)
    const maxSize = 2 * 1024 * 1024; // 2MB
    if (file.size > maxSize) {
      alert('圖片文件過大，請選擇小於2MB的圖片！');
      input.value = ''; // 清除輸入
      return;
    }

    console.log('開始上傳圖片:', file.name, '大小:', (file.size / 1024).toFixed(2) + 'KB');

    const reader = new FileReader();
    reader.onload = (e) => {
      const imageUrl = e.target?.result as string;
      
      // 檢查base64是否過大
      if (imageUrl.length > 500000) { // 約500KB的base64
        alert('圖片轉換後過大，請選擇更小的圖片！');
        input.value = ''; // 清除輸入
        return;
      }

      const imageOption: ImageOption = {
        name: file.name,
        url: imageUrl,
        category: '上傳',
        preview: imageUrl,
        isTemporary: true // 先標記為臨時，儲存成功後會更新
      };

      // 添加到圖片列表的最前面（新的在左邊）
      this.images.unshift(imageOption);
      this.filterImages();
      this.selectImage(imageOption);

      console.log('圖片已添加到UI，準備儲存到資料庫...');

      // 直接儲存到資料庫
      this.saveImageToDatabase(file.name, imageUrl, imageOption);
      
      // 清除輸入以允許重新選擇
      input.value = '';
    };

    reader.onerror = (error) => {
      console.error('讀取文件失敗:', error);
      alert('讀取圖片失敗，請重試！');
      input.value = ''; // 清除輸入
    };

    reader.readAsDataURL(file);
  }

  private saveImageToDatabase(fileName: string, imageUrl: string, image: ImageOption): void {
    const imageData = {
      name: fileName,
      description: `上傳的圖片: ${fileName}`,
      imageUrl: imageUrl,
      thumbnailUrl: imageUrl,
      category: '上傳',
      isPublic: true
    };

    console.log('發送圖片儲存請求到API...', {
      name: fileName,
      imageUrlLength: imageUrl.length,
      category: '上傳'
    });

    this.elementImageApiService.createElement(imageData).subscribe({
      next: (savedImage) => {
        console.log('圖片儲存成功:', savedImage);
        // 更新圖片物件的ID和移除臨時標記
        image.id = savedImage.id;
        image.isTemporary = false;
        
        // 顯示成功消息
        alert(`圖片「${fileName}」上傳成功！`);
      },
      error: (error) => {
        console.error('儲存圖片失敗:', error);
        
        let errorMessage = '儲存圖片失敗，請稍後再試。';
        if (error.status === 413) {
          errorMessage = '圖片文件過大，請選擇更小的圖片。';
        } else if (error.status === 400) {
          errorMessage = '圖片格式不正確，請選擇有效的圖片文件。';
        } else if (error.status === 0) {
          errorMessage = '網路連接失敗，請檢查網路連接。';
        }
        
        alert(errorMessage);
        
        // 儲存失敗時從列表中移除
        this.images = this.images.filter(img => img !== image);
        this.filterImages();
        if (this.selectedImage === image) {
          this.selectedImage = null;
        }
      }
    });
  }

  deleteImage(image: ImageOption): void {
    if (image.isTemporary) {
      // 直接從列表中移除臨時圖片
      if (confirm(`確定要刪除圖片「${image.name}」嗎？`)) {
        this.images = this.images.filter(img => img !== image);
        this.filterImages();
        if (this.selectedImage === image) {
          this.selectedImage = null;
        }
      }
    } else if (image.id && confirm(`確定要刪除圖片「${image.name}」嗎？`)) {
      this.elementImageApiService.deleteElement(image.id).subscribe({
        next: () => {
          this.images = this.images.filter(img => img.id !== image.id);
          this.filterImages();
          if (this.selectedImage?.id === image.id) {
            this.selectedImage = null;
          }
        },
        error: (error) => {
          console.error('刪除圖片失敗:', error);
          alert('刪除圖片失敗，請稍後再試。');
        }
      });
    }
  }

  confirm(): void {
    if (this.selectedImage) {
      this.imageSelected.emit(this.selectedImage);
      this.closeModal();
    }
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