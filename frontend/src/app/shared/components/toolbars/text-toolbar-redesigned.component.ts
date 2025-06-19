import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatMenuModule, MatMenu } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { FormsModule } from '@angular/forms';
import { ElementToolbarComponent } from './element-toolbar.component';
import { MatButtonModule } from '@angular/material/button';

// 將TextStyle接口從這裡導出，替代原有的text-editing-toolbar.component.ts
export interface TextStyle {
  fontWeight?: 'normal' | 'bold';
  fontStyle?: 'normal' | 'italic';
  textDecoration?: 'none' | 'underline' | 'line-through';
  color?: string;
  fontSize?: number;
  textAlign?: 'left' | 'center' | 'right';
  fontFamily?: string;
  tag?: string; // 新增標籤屬性
}

// 文字標籤類型
export interface TextTag {
  id: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'sn-text-toolbar-redesigned',
  standalone: true,
  imports: [
    CommonModule,
    MatMenuModule,
    MatIconModule,
    MatDividerModule,
    MatButtonModule,
    FormsModule,
    ElementToolbarComponent
  ],
  template: `
    <sn-element-toolbar
      [position]="position"
      [targetElement]="targetElement"
      (close)="toolbarClose.emit()">

      <!-- 格式化按鈕組 -->
      <div class="toolbar-group">
        <!-- 粗體 -->
        <button type="button"
                mat-icon-button
                class="text-format-btn"
                [class.active]="currentStyle.fontWeight === 'bold'"
                matTooltip="粗體"
                (click)="toggleBold()">
          <span class="mat-icon-wrapper">
            <mat-icon>format_bold</mat-icon>
          </span>
        </button>

        <!-- 斜體 -->
        <button type="button"
                mat-icon-button
                class="text-format-btn"
                [class.active]="currentStyle.fontStyle === 'italic'"
                matTooltip="斜體"
                (click)="toggleItalic()">
          <span class="mat-icon-wrapper">
            <mat-icon>format_italic</mat-icon>
          </span>
        </button>

        <!-- 底線 -->
        <button type="button"
                mat-icon-button
                class="text-format-btn"
                [class.active]="currentStyle.textDecoration === 'underline'"
                matTooltip="底線"
                (click)="toggleUnderline()">
          <span class="mat-icon-wrapper">
            <mat-icon>format_underlined</mat-icon>
          </span>
        </button>
      </div>

      <!-- 分隔線 -->
      <div class="toolbar-divider"></div>

      <!-- 對齊按鈕 -->
      <div class="toolbar-group">
        <button type="button"
                mat-icon-button
                class="text-format-btn"
                [class.active]="true"
                matTooltip="文字對齊"
                [matMenuTriggerFor]="alignMenu">
          <span class="mat-icon-wrapper">
            <mat-icon>{{ getCurrentAlignIcon() }}</mat-icon>
          </span>
        </button>
      </div>

      <!-- 分隔線 -->
      <div class="toolbar-divider"></div>

      <!-- 顏色和字體大小 -->
      <div class="toolbar-group">
        <!-- 文字顏色 -->
        <button type="button"
                mat-icon-button
                class="text-format-btn"
                matTooltip="文字顏色"
                [matMenuTriggerFor]="colorMenu">
          <span class="mat-icon-wrapper">
            <mat-icon [style.color]="currentStyle.color || '#000'">format_color_text</mat-icon>
          </span>
        </button>

        <!-- 字體大小 -->
        <button type="button"
                mat-icon-button
                class="text-format-btn"
                matTooltip="字體大小"
                [matMenuTriggerFor]="sizeMenu">
          <span class="mat-icon-wrapper">
            <mat-icon>format_size</mat-icon>
          </span>
        </button>
      </div>

      <!-- 分隔線 -->
      <div class="toolbar-divider"></div>

      <!-- 標籤化功能 -->
      <div class="toolbar-group">
        <button type="button"
                mat-icon-button
                class="text-format-btn"
                matTooltip="標籤化"
                [matMenuTriggerFor]="tagMenu">
          <span class="mat-icon-wrapper">
            <mat-icon>{{ getCurrentTagIcon() }}</mat-icon>
          </span>
        </button>
      </div>

      <!-- 分隔線 -->
      <div class="toolbar-divider"></div>

      <!-- 刪除按鈕 -->
      <div class="toolbar-group">
        <button type="button"
                mat-icon-button
                class="text-format-btn delete-button"
                matTooltip="刪除文字"
                (click)="delete.emit()">
          <span class="mat-icon-wrapper">
            <mat-icon>delete</mat-icon>
          </span>
        </button>
      </div>
    </sn-element-toolbar>

    <!-- 對齊選單 -->
    <mat-menu #alignMenu="matMenu">
      <button *ngFor="let option of alignOptions"
              mat-menu-item
              (click)="setAlignment(option.value)"
              [class.selected]="currentStyle.textAlign === option.value">
        <mat-icon>{{ option.icon }}</mat-icon>
        <span>{{ option.label }}</span>
      </button>
    </mat-menu>

    <!-- 顏色選單 -->
    <mat-menu #colorMenu="matMenu">
      <div class="color-palette" 
           (click)="$event.stopPropagation()"
           (keydown.enter)="$event.stopPropagation()"
           (keydown.space)="$event.stopPropagation()"
           tabindex="0" 
           role="grid"
           aria-label="顏色選擇區域">
                 <button *ngFor="let color of colorOptions"
                 class="color-option"
                 [style.background-color]="color"
                 (click)="selectColor(color)"
                 (keydown.enter)="selectColor(color)"
                 (keydown.space)="selectColor(color)"
                 tabindex="0" 
                 role="button"
                 [class.selected]="currentStyle.color === color"
                 [attr.aria-label]="'選擇顏色 ' + color">
         </button>
      </div>
    </mat-menu>

    <!-- 字體大小選單 -->
    <mat-menu #sizeMenu="matMenu">
      <button *ngFor="let size of sizeOptions"
              mat-menu-item
              (click)="setFontSize(size)"
              [class.selected]="currentStyle.fontSize === size">
        {{size}}px
      </button>
    </mat-menu>

    <!-- 標籤選單 -->
    <mat-menu #tagMenu="matMenu">
      <button *ngFor="let tag of tagOptions"
              mat-menu-item
              (click)="setTag(tag.id)"
              [class.selected]="currentStyle.tag === tag.id">
        <mat-icon>{{ tag.icon }}</mat-icon>
        <span>{{ tag.label }}</span>
      </button>
      <mat-divider></mat-divider>
      <button mat-menu-item
              (click)="clearTag()"
              [class.selected]="!currentStyle.tag">
        <mat-icon>clear</mat-icon>
        <span>清除標籤</span>
      </button>
    </mat-menu>
  `,
  styles: [`
    .color-palette {
      display: grid;
      grid-template-columns: repeat(5, 1fr);
      gap: 4px;
      padding: 8px;
    }

    .color-option {
      width: 24px;
      height: 24px;
      border-radius: 4px;
      border: 2px solid transparent;
      cursor: pointer;
      transition: all 0.2s ease;
    }

    .color-option:hover {
      transform: scale(1.1);
      border-color: rgba(0, 0, 0, 0.2);
    }

    .color-option.selected {
      border-color: #007aff;
      transform: scale(1.1);
    }

    ::ng-deep .mat-menu-item.selected {
      background-color: rgba(0, 122, 255, 0.1);
    }

    ::ng-deep .mat-menu-item.selected .mat-icon {
      color: #007aff;
    }

    .toolbar-group {
      display: flex;
      align-items: center;
      gap: 2px;
    }

    .toolbar-divider {
      width: 1px;
      height: 24px;
      background: rgba(0, 0, 0, 0.1);
      margin: 0 4px;
    }

    .text-format-btn {
      width: 36px !important;
      height: 36px !important;
      padding: 6px !important;
      display: inline-flex !important;
      align-items: center !important;
      justify-content: center !important;
      transition: background-color 0.2s cubic-bezier(0.4, 0, 0.2, 1);
    }

    .text-format-btn:hover {
      background-color: rgba(0, 122, 255, 0.1);
    }

    .text-format-btn.active {
      background-color: #007aff;
      color: white;
    }

    .text-format-btn.active .mat-icon {
      color: white !important;
    }

    .text-format-btn.delete-button:hover {
      background-color: rgba(255, 68, 68, 0.1);
    }

    .text-format-btn.delete-button:hover .mat-icon {
      color: #ff4444 !important;
    }

    .mat-icon-wrapper {
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .mat-icon {
      font-size: 18px;
      width: 18px !important;
      height: 18px !important;
      line-height: 18px !important;
    }
  `]
})
export class TextToolbarRedesignedComponent implements OnInit, OnDestroy {
  @Input() position = { x: 0, y: 0 };
  @Input() currentStyle: TextStyle = { textAlign: 'left' }; // 預設為靠左對齊
  @Input() targetElement: HTMLElement | null = null;
  @Output() styleChange = new EventEmitter<TextStyle>();
  @Output() toolbarClose = new EventEmitter<void>();
  @Output() delete = new EventEmitter<void>();
  @Output() textSubmit = new EventEmitter<void>(); // 修正submit綁定衝突

  @ViewChild('alignMenu') alignMenu!: MatMenu;
  @ViewChild('colorMenu') colorMenu!: MatMenu;
  @ViewChild('sizeMenu') sizeMenu!: MatMenu;
  @ViewChild('tagMenu') tagMenu!: MatMenu;

  alignOptions = [
    { value: 'left' as const, icon: 'format_align_left', label: '靠左對齊' },
    { value: 'center' as const, icon: 'format_align_center', label: '置中對齊' },
    { value: 'right' as const, icon: 'format_align_right', label: '靠右對齊' }
  ];

  colorOptions = [
    '#000000', '#333333', '#666666', '#999999', '#cccccc',
    '#ff0000', '#ff6600', '#ffcc00', '#33cc00', '#0099cc',
    '#6600cc', '#cc0066', '#ffffff', '#f5f5f5', '#e0e0e0'
  ];

  sizeOptions = [12, 24, 36, 48];

  tagOptions: TextTag[] = [
    { id: 'name', label: '姓名', icon: 'person' },
    { id: 'title', label: '職稱', icon: 'work' },
    { id: 'phone', label: '電話', icon: 'phone' },
    { id: 'address', label: '地址', icon: 'location_on' },
    { id: 'company', label: '公司', icon: 'business' },
    { id: 'custom', label: '自訂', icon: 'edit' }
  ];

  ngOnInit() {
    // 確保初始有預設值
    if (!this.currentStyle.textAlign) {
      this.currentStyle.textAlign = 'left';
    }
  }

  ngOnDestroy(): void {
    // 清理工作
    document.removeEventListener('click', this.handleDocumentClick);
  }

  private handleDocumentClick = (event: Event) => {
    // 處理文檔點擊事件
    const target = event.target as HTMLElement;
    if (!target.closest('.text-toolbar-redesigned')) {
      this.toolbarClose.emit();
    }
  }

  toggleBold() {
    const newWeight = this.currentStyle.fontWeight === 'bold' ? 'normal' : 'bold';
    this.emitStyleChange({ fontWeight: newWeight });
  }

  toggleItalic() {
    const newStyle = this.currentStyle.fontStyle === 'italic' ? 'normal' : 'italic';
    this.emitStyleChange({ fontStyle: newStyle });
  }

  toggleUnderline() {
    const newDecoration = this.currentStyle.textDecoration === 'underline' ? 'none' : 'underline';
    this.emitStyleChange({ textDecoration: newDecoration });
  }

  setAlignment(align: 'left' | 'center' | 'right') {
    this.emitStyleChange({ textAlign: align });
  }

  setColor(color: string) {
    this.emitStyleChange({ color });
  }

  setFontSize(size: number) {
    this.emitStyleChange({ fontSize: size });
  }

  getCurrentAlignIcon(): string {
    const align = this.currentStyle.textAlign || 'left';
    const option = this.alignOptions.find(opt => opt.value === align);
    return option ? option.icon : 'format_align_left';
  }

  getCurrentTagIcon(): string {
    if (!this.currentStyle.tag) {
      return 'label';
    }
    const tag = this.tagOptions.find(opt => opt.id === this.currentStyle.tag);
    return tag ? tag.icon : 'label';
  }

  setTag(tagId: string) {
    this.emitStyleChange({ tag: tagId });
  }

  clearTag() {
    this.emitStyleChange({ tag: undefined });
  }

  private emitStyleChange(changes: Partial<TextStyle>) {
    const newStyle = { ...this.currentStyle, ...changes };
    this.styleChange.emit(newStyle);
  }

  selectColor(color: string) {
    this.setColor(color);
  }
}