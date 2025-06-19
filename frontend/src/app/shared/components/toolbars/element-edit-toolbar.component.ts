import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { ElementToolbarComponent } from './element-toolbar.component';
import { ToolbarPositioningService, DropdownPosition } from '../../services/toolbar-positioning.service';

export interface ElementEditOptions {
  canDelete?: boolean;
  canDuplicate?: boolean;
  canReorder?: boolean;
  customActions?: {
    icon: string;
    label: string;
    action: string;
  }[];
}

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

interface ShapeStyle {
  backgroundColor?: string;
  borderColor?: string;
  borderWidth?: number;
  borderRadius?: number;
}

@Component({
  selector: 'sn-element-edit-toolbar',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatMenuModule,
    ElementToolbarComponent,
    FormsModule
  ],
  template: `
    <sn-element-toolbar
      [position]="position"
      [targetElement]="targetElement"
      (close)="onClose()">
      
      <!-- 圖片元素工具 -->
      <ng-container *ngIf="elementType === 'image'">
        <button mat-icon-button
                (click)="onAction('replace')"
                matTooltip="替換圖片">
          <mat-icon>swap_horiz</mat-icon>
        </button>
        <button mat-icon-button
                (click)="onAction('crop')"
                matTooltip="裁剪圖片">
          <mat-icon>crop</mat-icon>
        </button>
        <button mat-icon-button
                (click)="onAction('filter')"
                matTooltip="濾鏡效果">
          <mat-icon>palette</mat-icon>
        </button>
      </ng-container>

      <!-- 形狀元素工具 -->
      <ng-container *ngIf="elementType === 'shape'">
        <button mat-icon-button
                (click)="onAction('changeShape')"
                matTooltip="變更形狀">
          <mat-icon>category</mat-icon>
        </button>
        <button mat-icon-button
                (click)="onAction('fillColor')"
                [matTooltip]="shapeType === 'line' ? '變更顏色' : '填充顏色'">
          <mat-icon [style.color]="getFillColor()">format_color_fill</mat-icon>
        </button>
        <!-- 直線專屬：粗細調整 -->
        <button *ngIf="shapeType === 'line'"
                mat-icon-button
                (click)="toggleLineThicknessMenu()"
                matTooltip="線條粗細">
          <mat-icon>line_weight</mat-icon>
        </button>
        <ng-container *ngIf="shapeType !== 'line'">
          <button mat-icon-button
                  (click)="onAction('borderColor')"
                  matTooltip="邊框顏色">
            <mat-icon [style.color]="getBorderColor()">border_color</mat-icon>
          </button>
        </ng-container>
      </ng-container>

      <!-- QR碼元素工具 -->
      <ng-container *ngIf="elementType === 'qrcode'">
        <button mat-icon-button
                (click)="onAction('editContent')"
                matTooltip="編輯內容">
          <mat-icon>edit</mat-icon>
        </button>
        <button mat-icon-button
                (click)="onAction('regenerate')"
                matTooltip="重新生成">
          <mat-icon>refresh</mat-icon>
        </button>
      </ng-container>

      <!-- 文字元素工具 -->
      <ng-container *ngIf="elementType === 'text'">
        <!-- 粗體 -->
        <button mat-icon-button
                [class.active]="currentTextStyle?.fontWeight === 'bold'"
                (click)="toggleBold()"
                matTooltip="粗體">
          <mat-icon>format_bold</mat-icon>
        </button>
        <!-- 斜體 -->
        <button mat-icon-button
                [class.active]="currentTextStyle?.fontStyle === 'italic'"
                (click)="toggleItalic()"
                matTooltip="斜體">
          <mat-icon>format_italic</mat-icon>
        </button>
        <!-- 底線 -->
        <button mat-icon-button
                [class.active]="currentTextStyle?.textDecoration === 'underline'"
                (click)="toggleUnderline()"
                matTooltip="底線">
          <mat-icon>format_underlined</mat-icon>
        </button>
        
        <!-- 分隔線 -->
        <div style="width: 1px; height: 24px; background: #e0e0e0; margin: 0 4px;"></div>
        
        <!-- 對齊 (整合按鈕) -->
        <button mat-icon-button
                #alignButton
                (click)="toggleDropdown('align')"
                matTooltip="文字對齊">
          <mat-icon>{{ getCurrentAlignIcon() }}</mat-icon>
        </button>
        <!-- 文字顏色 -->
        <button mat-icon-button
                #colorButton
                (click)="toggleDropdown('color')"
                matTooltip="文字顏色">
          <mat-icon [style.color]="currentTextStyle?.color || '#000'">format_color_text</mat-icon>
        </button>
        <!-- 字體大小 -->
        <button mat-icon-button
                #sizeButton
                (click)="toggleDropdown('size')"
                matTooltip="字體大小">
          <mat-icon>format_size</mat-icon>
        </button>

        <!-- 分隔線 -->
        <div style="width: 1px; height: 24px; background: #e0e0e0; margin: 0 4px;"></div>

        <!-- 標籤化 -->
        <button mat-icon-button
                #tagButton
                (click)="toggleDropdown('tag')"
                matTooltip="標籤化">
          <mat-icon>{{ getCurrentTagIcon() }}</mat-icon>
        </button>
      </ng-container>

      <!-- 分隔線 -->
      <div style="width: 1px; height: 24px; background: #e0e0e0; margin: 0 8px;"></div>

      <!-- 通用操作 -->
      <button mat-icon-button
              (click)="onAction('moveUp')"
              matTooltip="上移一層">
        <mat-icon>keyboard_arrow_up</mat-icon>
      </button>
      <button mat-icon-button
              (click)="onAction('moveDown')"
              matTooltip="下移一層">
        <mat-icon>keyboard_arrow_down</mat-icon>
      </button>
      <button mat-icon-button
              (click)="onAction('duplicate')"
              matTooltip="複製元素">
        <mat-icon>content_copy</mat-icon>
      </button>
      <button mat-icon-button
              (click)="onAction('delete')"
              matTooltip="刪除元素"
              color="warn">
        <mat-icon>delete</mat-icon>
      </button>
    </sn-element-toolbar>

    <!-- 對齊下拉選單 -->
    <div *ngIf="elementType === 'text' && showAlignMenu" 
         class="dropdown-menu"
         [style.left.px]="dropdownPositions.align?.x"
         [style.top.px]="dropdownPositions.align?.y">
             <button style="display: flex; align-items: center; width: 100%; padding: 8px 12px; border: none; background: none; text-align: left; cursor: pointer;"
               (click)="setAlignment('left'); showAlignMenu = false"
               (mouseover)="onMenuItemHover($event, true)"
               (mouseout)="onMenuItemHover($event, false)"
               (focus)="onMenuItemHover($event, true)"
               (blur)="onMenuItemHover($event, false)">
         <mat-icon style="margin-right: 8px; font-size: 18px;">format_align_left</mat-icon>
         <span>靠左對齊</span>
       </button>
       <button style="display: flex; align-items: center; width: 100%; padding: 8px 12px; border: none; background: none; text-align: left; cursor: pointer;"
               (click)="setAlignment('center'); showAlignMenu = false"
               (mouseover)="onMenuItemHover($event, true)"
               (mouseout)="onMenuItemHover($event, false)"
               (focus)="onMenuItemHover($event, true)"
               (blur)="onMenuItemHover($event, false)">
         <mat-icon style="margin-right: 8px; font-size: 18px;">format_align_center</mat-icon>
         <span>置中對齊</span>
       </button>
       <button style="display: flex; align-items: center; width: 100%; padding: 8px 12px; border: none; background: none; text-align: left; cursor: pointer;"
               (click)="setAlignment('right'); showAlignMenu = false"
               (mouseover)="onMenuItemHover($event, true)"
               (mouseout)="onMenuItemHover($event, false)"
               (focus)="onMenuItemHover($event, true)"
               (blur)="onMenuItemHover($event, false)">
         <mat-icon style="margin-right: 8px; font-size: 18px;">format_align_right</mat-icon>
         <span>靠右對齊</span>
       </button>
    </div>

    <!-- 顏色選擇下拉選單 -->
    <div *ngIf="elementType === 'text' && showColorMenu" 
         class="dropdown-menu color-menu"
         [style.left.px]="dropdownPositions.color?.x"
         [style.top.px]="dropdownPositions.color?.y">
      <div class="color-grid">
        <!-- 第一行：黑白灰系列 -->
        <div class="color-row">
          <button *ngFor="let color of colorRows[0]; trackBy: trackByColor"
                  class="color-swatch"
                  [style.background-color]="color"
                  [class.selected]="color === (currentTextStyle?.color || '#000000')"
                  (click)="setColor(color); showColorMenu = false"
                  (mouseover)="onColorSwatchHover($event, true, color)"
                  (mouseout)="onColorSwatchHover($event, false, color)"
                  (focus)="onColorSwatchHover($event, true, color)"
                  (blur)="onColorSwatchHover($event, false, color)"
                  [title]="getColorName(color)">
          </button>
        </div>
        <!-- 第二行：基礎色彩系列 -->
        <div class="color-row">
          <button *ngFor="let color of colorRows[1]; trackBy: trackByColor"
                  class="color-swatch"
                  [style.background-color]="color"
                  [class.selected]="color === (currentTextStyle?.color || '#000000')"
                  (click)="setColor(color); showColorMenu = false"
                  (mouseover)="onColorSwatchHover($event, true, color)"
                  (mouseout)="onColorSwatchHover($event, false, color)"
                  (focus)="onColorSwatchHover($event, true, color)"
                  (blur)="onColorSwatchHover($event, false, color)"
                  [title]="getColorName(color)">
          </button>
        </div>
        <!-- 第三行：進階色彩系列 -->
        <div class="color-row">
          <button *ngFor="let color of colorRows[2]; trackBy: trackByColor"
                  class="color-swatch"
                  [style.background-color]="color"
                  [class.selected]="color === (currentTextStyle?.color || '#000000')"
                  (click)="setColor(color); showColorMenu = false"
                  (mouseover)="onColorSwatchHover($event, true, color)"
                  (mouseout)="onColorSwatchHover($event, false, color)"
                  (focus)="onColorSwatchHover($event, true, color)"
                  (blur)="onColorSwatchHover($event, false, color)"
                  [title]="getColorName(color)">
          </button>
        </div>
      </div>
    </div>

    <!-- 字體大小下拉選單 -->
    <div *ngIf="elementType === 'text' && showSizeMenu" 
         class="dropdown-menu"
         [style.left.px]="dropdownPositions.size?.x"
         [style.top.px]="dropdownPositions.size?.y">
      <!-- 自定義輸入區域 -->
      <div style="padding: 8px 12px; border-bottom: 1px solid #e0e0e0;">
        <div style="display: flex; align-items: center; gap: 8px;">
          <input type="number" 
                 [(ngModel)]="customFontSize"
                 (keydown.enter)="applyCustomFontSize()"
                 min="8" max="72"
                 style="width: 60px; padding: 4px 6px; border: 1px solid #ddd; border-radius: 4px; font-size: 14px;">
          <span style="font-size: 12px; color: #666;">px</span>
          <button style="padding: 4px 8px; background: #007aff; color: white; border: none; border-radius: 4px; font-size: 12px; cursor: pointer;"
                  (click)="applyCustomFontSize()">
            套用
          </button>
        </div>
      </div>
              <!-- 預設大小選項 -->
       <button *ngFor="let size of sizeOptions" 
               class="dropdown-item"
               [style.background-color]="size === (currentTextStyle?.fontSize || 16) ? '#e3f2fd' : 'transparent'"
               (click)="setFontSize(size); showSizeMenu = false"
               (mouseover)="onMenuItemHover($event, true)"
               (mouseout)="onMenuItemHover($event, false)"
               (focus)="onMenuItemHover($event, true)"
               (blur)="onMenuItemHover($event, false)">
         {{size}}px
       </button>
    </div>

    <!-- 標籤下拉選單 -->
    <div *ngIf="elementType === 'text' && showTagMenu"
         class="dropdown-menu"
         [style.left.px]="dropdownPositions.tag?.x"
         [style.top.px]="dropdownPositions.tag?.y">
             <button *ngFor="let tag of tagOptions"
               class="dropdown-item"
               [style.background-color]="tag.id === currentTextStyle?.tag ? '#e3f2fd' : 'transparent'"
               (click)="setTag(tag.id); showTagMenu = false"
               (mouseover)="onMenuItemHover($event, true)"
               (mouseout)="onMenuItemHover($event, false)"
               (focus)="onMenuItemHover($event, true)"
               (blur)="onMenuItemHover($event, false)">
         <mat-icon style="margin-right: 8px; font-size: 18px;">{{ tag.icon }}</mat-icon>
         <span>{{ tag.label }}</span>
       </button>
      <div style="height: 1px; background: #e0e0e0; margin: 4px 0;"></div>
      <button class="dropdown-item"
              [style.background-color]="!currentTextStyle?.tag || currentTextStyle?.tag === '' ? '#e3f2fd' : 'transparent'"
              (click)="clearTag(); showTagMenu = false"
              (mouseover)="onMenuItemHover($event, true)"
              (mouseout)="onMenuItemHover($event, false)">
        <mat-icon style="margin-right: 8px; font-size: 18px;">clear</mat-icon>
        <span>清除標籤</span>
      </button>
    </div>

    <!-- 線條粗細下拉選單 -->
    <div *ngIf="showLineThicknessMenu && shapeType === 'line'" class="dropdown-menu" [style.left.px]="position.x + 40" [style.top.px]="position.y + 40">
             <button *ngFor="let thickness of lineThicknessOptions"
               class="dropdown-item"
               (click)="setLineThickness(thickness)"
               (mouseover)="showTooltip($event, 'thickness-' + thickness)"
               (mouseout)="hideTooltip()"
               (focus)="showTooltip($event, 'thickness-' + thickness)"
               (blur)="hideTooltip()">
         <span style="display:inline-block;width:32px;height:{{thickness}}px;background:#1565c0;"></span>
         <span style="margin-left:8px;">{{thickness}} px</span>
       </button>
    </div>
  `,
  styleUrls: ['./element-toolbar.component.scss'],
  styles: [`
    .dropdown-menu {
      position: fixed;
      background: white;
      border: 1px solid #ddd;
      border-radius: 4px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.15);
      z-index: 1001;
      min-width: 120px;
    }
    
    .dropdown-menu.color-menu {
      padding: 8px;
      min-width: 150px;
    }
    
    .color-grid {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    
    .color-row {
      display: flex;
      gap: 4px;
      justify-content: center;
    }
    
    .color-swatch {
      width: 24px;
      height: 24px;
      border-radius: 4px;
      border: 2px solid transparent;
      cursor: pointer;
      transition: all 0.2s ease;
      padding: 0;
    }
    
    .color-swatch.selected {
      border-color: #2196f3;
      box-shadow: 0 0 0 2px rgba(33, 150, 243, 0.3);
    }
    
    .dropdown-item {
      display: flex;
      align-items: center;
      width: 100%;
      padding: 8px 12px;
      border: none;
      background: none;
      text-align: left;
      cursor: pointer;
      font-size: 14px;
    }
    
    .dropdown-item:hover {
      background-color: #f5f5f5;
    }
    
    .dropdown-item mat-icon {
      margin-right: 8px;
      font-size: 18px;
    }
  `]
})
export class ElementEditToolbarComponent implements OnInit, OnDestroy, OnChanges {
  @Input() position: { x: number; y: number } = { x: 0, y: 0 };
  @Input() targetElement: HTMLElement | null = null;
  @Input() elementType: 'image' | 'shape' | 'qrcode' | 'text' = 'image';
  @Input() shapeType = '';
  @Input() options: ElementEditOptions = {};
  @Input() currentTextStyle?: TextStyle;
  @Input() currentShapeStyle: ShapeStyle = {};
  
  @Output() toolbarClose = new EventEmitter<void>();
  @Output() action = new EventEmitter<string | { type: string; value: number }>();
  @Output() styleChange = new EventEmitter<TextStyle>();

  showAlignMenu = false;
  showColorMenu = false;
  showSizeMenu = false;
  showTagMenu = false;
  showLineThicknessMenu = false;

  // 重新組織的顏色選項 - 按UI/UX最佳實踐排列
  colorRows: string[][] = [
    // 第一行：黑白灰系列（最常用）
    ['#000000', '#333333', '#666666', '#999999', '#ffffff'],
    // 第二行：基礎色彩系列（RGB三原色 + 常用顏色）
    ['#ff0000', '#ff6600', '#ffcc00', '#33cc00', '#0099cc'],
    // 第三行：進階色彩系列（紫色系 + 輔助色）
    ['#6600cc', '#cc0066', '#f5f5f5', '#e0e0e0', '#cccccc']
  ];

  // 保持向後兼容的平面陣列
  get colorOptions(): string[] {
    return this.colorRows.flat();
  }

  sizeOptions = [12, 16, 20, 24, 28, 32, 36, 48];

  tagOptions: TextTag[] = [
    { id: 'name', label: '姓名', icon: 'person' },
    { id: 'title', label: '職稱', icon: 'work' },
    { id: 'phone', label: '電話', icon: 'phone' },
    { id: 'address', label: '地址', icon: 'location_on' },
    { id: 'company', label: '公司', icon: 'business' },
    { id: 'custom', label: '自訂', icon: 'edit' }
  ];

  customFontSize = 16;
  
  // 使用定位服務計算的下拉選單位置
  dropdownPositions: {
    align?: DropdownPosition;
    color?: DropdownPosition;
    size?: DropdownPosition;
    tag?: DropdownPosition;
  } = {};

  lineThicknessOptions = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

  constructor(private positioningService: ToolbarPositioningService) {}

  ngOnInit() {
    // 監聽點擊事件以關閉下拉選單
    document.addEventListener('click', this.handleDocumentClick.bind(this));
    // 初始化自定義字體大小為當前字體大小
    this.customFontSize = this.currentTextStyle?.fontSize || 16;
  }

  ngOnDestroy() {
    document.removeEventListener('click', this.handleDocumentClick.bind(this));
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['currentTextStyle']) {
      this.customFontSize = changes['currentTextStyle'].currentValue?.fontSize || 16;
    }
    
    // 當位置變更時，重新計算下拉選單位置
    if (changes['position']) {
      this.updateDropdownPositions();
    }
  }

  private handleDocumentClick(event: Event) {
    const target = event.target as HTMLElement;
    const toolbar = target.closest('sn-element-edit-toolbar');

    // 如果點擊不在工具列內，關閉所有下拉選單
    if (!toolbar) {
      this.showAlignMenu = false;
      this.showColorMenu = false;
      this.showSizeMenu = false;
      this.showTagMenu = false;
      this.showLineThicknessMenu = false;
    }
  }

  onClose(): void {
    this.toolbarClose.emit();
  }

  onAction(actionType: string): void {
    this.action.emit(actionType);
  }

  toggleBold() {
    const newWeight = this.currentTextStyle?.fontWeight === 'bold' ? 'normal' : 'bold';
    this.emitStyleChange({ fontWeight: newWeight });
  }

  toggleItalic() {
    const newStyle = this.currentTextStyle?.fontStyle === 'italic' ? 'normal' : 'italic';
    this.emitStyleChange({ fontStyle: newStyle });
  }

  toggleUnderline() {
    const newDecoration = this.currentTextStyle?.textDecoration === 'underline' ? 'none' : 'underline';
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

  onMenuItemHover(event: MouseEvent | FocusEvent, hover: boolean) {
    const target = event.target as HTMLElement;
    if (target) {
      target.style.backgroundColor = hover ? '#f5f5f5' : 'transparent';
    }
  }

  onColorSwatchHover(event: MouseEvent | FocusEvent, hover: boolean, originalColor: string) {
    const target = event.target as HTMLElement;
    if (target) {
      // 對於顏色按鈕，我們不改變背景顏色，只改變其他樣式效果
      if (hover) {
        target.style.transform = 'scale(1.15)';
        target.style.borderColor = '#2196f3';
        target.style.boxShadow = '0 2px 8px rgba(33, 150, 243, 0.3)';
      } else {
        target.style.transform = 'scale(1)';
        target.style.borderColor = 'transparent';
        target.style.boxShadow = 'none';
        // 確保背景顏色保持原樣
        target.style.backgroundColor = originalColor;
      }
    }
  }

  private emitStyleChange(changes: Partial<TextStyle>) {
    const newStyle = { ...this.currentTextStyle, ...changes };
    this.styleChange.emit(newStyle);
  }

  getCurrentAlignIcon(): string {
    if (this.currentTextStyle?.textAlign === 'left') {
      return 'format_align_left';
    } else if (this.currentTextStyle?.textAlign === 'center') {
      return 'format_align_center';
    } else if (this.currentTextStyle?.textAlign === 'right') {
      return 'format_align_right';
    } else {
      return 'format_align_left';
    }
  }

  toggleDropdown(type: 'align' | 'color' | 'size' | 'tag') {
    // 檢查當前選單是否已開啟
    const isCurrentlyOpen =
      (type === 'align' && this.showAlignMenu) ||
      (type === 'color' && this.showColorMenu) ||
      (type === 'size' && this.showSizeMenu) ||
      (type === 'tag' && this.showTagMenu);

    // 關閉所有下拉選單
    this.showAlignMenu = false;
    this.showColorMenu = false;
    this.showSizeMenu = false;
    this.showTagMenu = false;
    this.showLineThicknessMenu = false;

    // 如果當前選單沒有開啟，則開啟對應的選單
    if (!isCurrentlyOpen) {
      // 更新下拉選單位置
      this.updateDropdownPositions();

      switch (type) {
        case 'align':
          this.showAlignMenu = true;
          break;
        case 'color':
          this.showColorMenu = true;
          break;
        case 'size':
          this.showSizeMenu = true;
          break;
        case 'tag':
          this.showTagMenu = true;
          break;
      }
    }
  }

  private updateDropdownPositions(): void {
    this.dropdownPositions = {
      align: this.positioningService.calculateDropdownPosition(this.position, 'align'),
      color: this.positioningService.calculateDropdownPosition(this.position, 'color'),
      size: this.positioningService.calculateDropdownPosition(this.position, 'size'),
      tag: this.positioningService.calculateDropdownPosition(this.position, 'tag')
    };
  }

  applyCustomFontSize() {
    // 驗證輸入範圍
    if (this.customFontSize < 8) {
      this.customFontSize = 8;
    } else if (this.customFontSize > 72) {
      this.customFontSize = 72;
    }
    
    // 套用字體大小
    this.setFontSize(this.customFontSize);
    
    // 關閉下拉選單
    this.showSizeMenu = false;
  }

  /**
   * 🛡️ 安全的顏色名稱取得 - 防止 Object Injection
   */
  getColorName(color: string): string {
    if (!color || typeof color !== 'string') return '未知顏色';
    
    const colorNames = new Map([
      ['#000000', '黑色'],
      ['#333333', '深灰色'],
      ['#666666', '中灰色'],
      ['#999999', '淺灰色'],
      ['#ffffff', '白色'],
      ['#ff0000', '紅色'],
      ['#ff6600', '橘色'],
      ['#ffcc00', '黃色'],
      ['#33cc00', '綠色'],
      ['#0099cc', '藍色'],
      ['#6600cc', '紫色'],
      ['#cc0066', '粉紅色'],
      ['#f5f5f5', '極淺灰'],
      ['#e0e0e0', '淺灰'],
      ['#cccccc', '中淺灰']
    ]);
    
    return colorNames.get(color) || color;
  }

  toggleLineThicknessMenu() {
    this.showLineThicknessMenu = !this.showLineThicknessMenu;
  }

  setLineThickness(thickness: number) {
    // 只針對 shape/line，直接觸發 action，由父層處理 style 更新
    this.action.emit({ type: 'lineThickness', value: thickness });
    this.showLineThicknessMenu = false;
  }

  getFillColor(): string {
    return this.currentShapeStyle?.backgroundColor || (this.shapeType === 'line' ? '#1565c0' : '#e3f2fd');
  }

  getBorderColor(): string {
    return this.currentShapeStyle?.borderColor || '#2196f3';
  }

  getCurrentTagIcon(): string {
    if (!this.currentTextStyle?.tag || this.currentTextStyle.tag === '') {
      return 'label';
    }
    const tag = this.tagOptions.find(opt => opt.id === this.currentTextStyle?.tag);
    return tag ? tag.icon : 'label';
  }

  setTag(tagId: string) {
    this.emitStyleChange({ tag: tagId });
  }

  clearTag() {
    this.emitStyleChange({ tag: '' }); // 給空字符串而不是undefined
  }

  showTooltip(event: MouseEvent | FocusEvent, tooltip: string) {
    // Implementation of showTooltip method
    console.log('Show tooltip:', tooltip, 'at:', event.target);
  }

  hideTooltip() {
    // Implementation of hideTooltip method
    console.log('Hide tooltip');
  }

  trackByColor(index: number, color: string): string {
    return color;
  }
}