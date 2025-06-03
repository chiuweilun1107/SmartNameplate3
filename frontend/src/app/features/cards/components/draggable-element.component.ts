import { Component, Input, Output, EventEmitter, ElementRef, OnInit, OnDestroy, OnChanges, SimpleChanges, HostListener, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CanvasElement, Position, Size } from '../models/card-design.models';
import { Subject, fromEvent, takeUntil } from 'rxjs';
import { QRCodeModule } from 'angularx-qrcode';
import { TextToolbarRedesignedComponent, TextStyle } from '../../../shared/components/toolbars/text-toolbar-redesigned.component';
import { ElementEditToolbarComponent } from '../../../shared/components/toolbars/element-edit-toolbar.component';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { IconActionButtonComponent } from '../../../shared/components/buttons/icon-action-button.component';

@Component({
  selector: 'sn-draggable-element',
  standalone: true,
  imports: [CommonModule, QRCodeModule, TextToolbarRedesignedComponent, ElementEditToolbarComponent, FormsModule, MatButtonModule, MatIconModule, IconActionButtonComponent],
  templateUrl: './draggable-element.component.html',
  styleUrls: ['./draggable-element.component.scss']
})
export class DraggableElementComponent implements OnInit, OnDestroy, OnChanges {
  @Input() element!: CanvasElement;
  @Input() isSelected = false;
  @Input() canvasElement: HTMLDivElement | null = null;
  @Input() shouldCloseToolbar = 0;
  @Input() isCropping = false; // 是否處於裁剪模式

  @Output() elementSelected = new EventEmitter<string>();
  @Output() elementMoved = new EventEmitter<{id: string, position: Position}>();
  @Output() elementResized = new EventEmitter<{id: string, size: Size}>();
  @Output() elementUpdated = new EventEmitter<{id: string, updates: Partial<CanvasElement>}>();
  @Output() elementDeleted = new EventEmitter<string>();
  @Output() cropChanged = new EventEmitter<{id: string, cropData: any}>();

  isDragging = false;
  isResizing = false;
  isEditingText = false;
  showTextToolbar = false;
  textToolbarPosition = { x: -9999, y: -9999 };
  editTextValue = '';

  // 裁剪相關屬性
  cropSelection = {
    x: 10,
    y: 10,
    width: 100,
    height: 100
  };
  private isCropResizing = false;
  private cropResizeHandle = '';
  private cropResizeStart = { x: 0, y: 0 };
  private cropSelectionStart = { x: 0, y: 0, width: 0, height: 0 };

  private destroy$ = new Subject<void>();
  private dragStart = { x: 0, y: 0 };
  private elementStart = { x: 0, y: 0 };
  private resizeHandle = '';
  private resizeStartSize = { width: 0, height: 0 };
  private initialFontSize = 16; // 記錄縮放開始時的字體大小

  constructor(public elementRef: ElementRef, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.setupResizeHandlers();
    // 視窗resize/scroll時自動更新工具列
    window.addEventListener('resize', this.windowUpdateToolbarPosition, true);
    window.addEventListener('scroll', this.windowUpdateToolbarPosition, true);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    window.removeEventListener('resize', this.windowUpdateToolbarPosition, true);
    window.removeEventListener('scroll', this.windowUpdateToolbarPosition, true);
  }

  ngOnChanges(changes: SimpleChanges): void {
    // 監聽外部關閉工具列信號（數值變化時觸發）
    if (changes['shouldCloseToolbar'] &&
        changes['shouldCloseToolbar'].currentValue !== changes['shouldCloseToolbar'].previousValue &&
        changes['shouldCloseToolbar'].currentValue > 0) {
      this.showTextToolbar = false;
    }
  }

  onElementClick(event: MouseEvent): void {
    event.stopPropagation();

    // 發送選中事件
    this.elementSelected.emit(this.element.id);
  }

  onTextStyleChange(newStyle: TextStyle): void {
    const textElement = this.getTextElement();

    const updates: any = {
      style: {
        ...textElement.style,
        ...newStyle
      }
    };

    // 如果字體大小變更，自動調整元素高度
    if (newStyle.fontSize && newStyle.fontSize !== textElement.style?.fontSize) {
      const fontSize = newStyle.fontSize;
      const padding = textElement.style?.padding || 10;
      const lineHeight = 1.2;

      // 計算合適的高度（字體大小 * 行高 + 上下padding）
      const newHeight = Math.max(fontSize * lineHeight + padding * 2, 30);

      updates.size = {
        ...this.element.size,
        height: newHeight
      };
    }

    this.elementUpdated.emit({
      id: this.element.id,
      updates
    });
  }

  onCloseTextToolbar(): void {
    this.showTextToolbar = false;
  }

  onDeleteElement(): void {
    this.showTextToolbar = false;
    this.elementDeleted.emit(this.element.id);
  }

  getCurrentTextStyle(): TextStyle {
    const textElement = this.getTextElement();
    const style = textElement.style || {};

    return {
      fontWeight: style.fontWeight || 'normal',
      fontStyle: style.fontStyle || 'normal',
      textDecoration: style.textDecoration || 'none',
      color: style.color || '#000000',
      fontSize: style.fontSize || 16,
      textAlign: style.textAlign || 'left',
      fontFamily: style.fontFamily || 'Arial'
    };
  }

  onMouseDown(event: MouseEvent): void {
    event.preventDefault();
    event.stopPropagation();

    const target = event.target as HTMLElement;

    // 修正 class 名稱判斷，確保能正確觸發 resize
    if (target.classList.contains('draggable-element__handle')) {
      this.startResize(event, target.dataset['handle'] || '');
      return;
    }

    // 開始拖拽
    this.startDrag(event);
  }

  private startDrag(event: MouseEvent): void {
    this.isDragging = true;
    this.dragStart = { x: event.clientX, y: event.clientY };
    this.elementStart = { ...this.element.position };

    const mousemove$ = fromEvent<MouseEvent>(document, 'mousemove');
    const mouseup$ = fromEvent<MouseEvent>(document, 'mouseup');

    mousemove$
      .pipe(takeUntil(mouseup$), takeUntil(this.destroy$))
      .subscribe(moveEvent => this.onDragMove(moveEvent));

    mouseup$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.stopDrag());
  }

  private onDragMove(event: MouseEvent): void {
    if (!this.isDragging) return;

    const deltaX = event.clientX - this.dragStart.x;
    const deltaY = event.clientY - this.dragStart.y;

    // 計算縮放比例：實際畫布尺寸 vs 邏輯畫布尺寸（800x480）
    const logicalCanvasWidth = 800;
    const logicalCanvasHeight = 480;
    const actualCanvasWidth = this.canvasElement?.clientWidth || 800;
    const actualCanvasHeight = this.canvasElement?.clientHeight || 480;
    
    // 縮放比例（邏輯尺寸 / 實際尺寸）
    const scaleX = logicalCanvasWidth / actualCanvasWidth;
    const scaleY = logicalCanvasHeight / actualCanvasHeight;

    // 將實際的鼠標移動距離轉換為邏輯座標系統的移動距離
    const logicalDeltaX = deltaX * scaleX;
    const logicalDeltaY = deltaY * scaleY;

    let newPosition = {
      x: this.elementStart.x + logicalDeltaX,
      y: this.elementStart.y + logicalDeltaY
    };
      
    // 使用邏輯畫布尺寸進行邊界檢查（統一使用800x480座標系統）
    newPosition.x = Math.max(0, Math.min(newPosition.x, logicalCanvasWidth - this.element.size.width));
    newPosition.y = Math.max(0, Math.min(newPosition.y, logicalCanvasHeight - this.element.size.height));

    this.elementMoved.emit({ id: this.element.id, position: newPosition });
  }

  private stopDrag(): void {
    this.isDragging = false;
  }

  private startResize(event: MouseEvent, handle: string): void {
    this.isResizing = true;
    this.resizeHandle = handle;
    this.dragStart = { x: event.clientX, y: event.clientY };
    this.resizeStartSize = { ...this.element.size };
    this.elementStart = { ...this.element.position };

    // 如果是文字元素，記錄初始字體大小
    if (this.element.type === 'text') {
      const textElement = this.getTextElement();
      this.initialFontSize = textElement.style?.fontSize || 16;
    }

    const mousemove$ = fromEvent<MouseEvent>(document, 'mousemove');
    const mouseup$ = fromEvent<MouseEvent>(document, 'mouseup');

    mousemove$
      .pipe(takeUntil(mouseup$), takeUntil(this.destroy$))
      .subscribe(moveEvent => this.onResizeMove(moveEvent));

    mouseup$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.stopResize());
  }

  private onResizeMove(event: MouseEvent): void {
    if (!this.isResizing) return;

    const deltaX = event.clientX - this.dragStart.x;
    const deltaY = event.clientY - this.dragStart.y;

    // 計算縮放比例：實際畫布尺寸 vs 邏輯畫布尺寸（800x480）
    const logicalCanvasWidth = 800;
    const logicalCanvasHeight = 480;
    const actualCanvasWidth = this.canvasElement?.clientWidth || 800;
    const actualCanvasHeight = this.canvasElement?.clientHeight || 480;
    
    // 縮放比例（邏輯尺寸 / 實際尺寸）
    const scaleX = logicalCanvasWidth / actualCanvasWidth;
    const scaleY = logicalCanvasHeight / actualCanvasHeight;

    // 將實際的鼠標移動距離轉換為邏輯座標系統的移動距離
    const logicalDeltaX = deltaX * scaleX;
    const logicalDeltaY = deltaY * scaleY;

    const newSize = { ...this.resizeStartSize };
    const newPosition = { ...this.elementStart };

    // === 圓形 shapeType: 'circle' ===
    if (this.element.type === 'shape' && this.getShapeElement().shapeType === 'circle') {
      const width = this.resizeStartSize.width;
      const height = this.resizeStartSize.height;
      let newWidth = width;
      let newHeight = height;
      let newX = this.elementStart.x;
      let newY = this.elementStart.y;
      const aspectRatio = width / height;
      switch (this.resizeHandle) {
        case 'e':
          newWidth = Math.max(20, width + logicalDeltaX);
          break;
        case 'w':
          newWidth = Math.max(20, width - logicalDeltaX);
          newX = this.elementStart.x + (width - newWidth);
          break;
        case 's':
          newHeight = Math.max(20, height + logicalDeltaY);
          break;
        case 'n':
          newHeight = Math.max(20, height - logicalDeltaY);
          newY = this.elementStart.y + (height - newHeight);
          break;
        case 'se':
        case 'sw':
        case 'ne':
        case 'nw': {
          // 角落控制點：依照當下比例等比縮放
          let sizeDeltaX = (this.resizeHandle === 'se' || this.resizeHandle === 'ne') ? logicalDeltaX : -logicalDeltaX;
          let sizeDeltaY = (this.resizeHandle === 'se' || this.resizeHandle === 'sw') ? logicalDeltaY : -logicalDeltaY;
          // 取最大變化量，維持比例
          let sizeDelta = Math.abs(sizeDeltaX) > Math.abs(sizeDeltaY) ? sizeDeltaX : sizeDeltaY;
          if (this.resizeHandle === 'nw') sizeDelta = -Math.max(Math.abs(sizeDeltaX), Math.abs(sizeDeltaY));
          else sizeDelta = Math.max(sizeDeltaX, sizeDeltaY);
          // 依照原始比例縮放
          if (aspectRatio >= 1) {
            newWidth = Math.max(20, width + sizeDelta);
            newHeight = Math.max(20, newWidth / aspectRatio);
          } else {
            newHeight = Math.max(20, height + sizeDelta);
            newWidth = Math.max(20, newHeight * aspectRatio);
          }
          // 修正位置，保持中心點不變
          newX = this.elementStart.x + (width - newWidth) / 2;
          newY = this.elementStart.y + (height - newHeight) / 2;
          break;
        }
      }
      newSize.width = newWidth;
      newSize.height = newHeight;
      newPosition.x = newX;
      newPosition.y = newY;
    } else if (this.element.type === 'image' || this.element.type === 'shape') {
      // 其他圖片/形狀維持單方向拉伸
      switch (this.resizeHandle) {
        case 'e': // 右邊
          newSize.width = Math.max(20, this.resizeStartSize.width + logicalDeltaX);
          break;
        case 'w': // 左邊
          newSize.width = Math.max(20, this.resizeStartSize.width - logicalDeltaX);
          newPosition.x = this.elementStart.x + (this.resizeStartSize.width - newSize.width);
          break;
        case 's': // 下邊
          newSize.height = Math.max(20, this.resizeStartSize.height + logicalDeltaY);
          break;
        case 'n': // 上邊
          newSize.height = Math.max(20, this.resizeStartSize.height - logicalDeltaY);
          newPosition.y = this.elementStart.y + (this.resizeStartSize.height - newSize.height);
          break;
        case 'se': // 右下角
          newSize.width = Math.max(20, this.resizeStartSize.width + logicalDeltaX);
          newSize.height = Math.max(20, this.resizeStartSize.height + logicalDeltaY);
          break;
        case 'sw': // 左下角
          newSize.width = Math.max(20, this.resizeStartSize.width - logicalDeltaX);
          newSize.height = Math.max(20, this.resizeStartSize.height + logicalDeltaY);
          newPosition.x = this.elementStart.x + (this.resizeStartSize.width - newSize.width);
          break;
        case 'ne': // 右上角
          newSize.width = Math.max(20, this.resizeStartSize.width + logicalDeltaX);
          newSize.height = Math.max(20, this.resizeStartSize.height - logicalDeltaY);
          newPosition.y = this.elementStart.y + (this.resizeStartSize.height - newSize.height);
          break;
        case 'nw': // 左上角
          newSize.width = Math.max(20, this.resizeStartSize.width - logicalDeltaX);
          newSize.height = Math.max(20, this.resizeStartSize.height - logicalDeltaY);
          newPosition.x = this.elementStart.x + (this.resizeStartSize.width - newSize.width);
          newPosition.y = this.elementStart.y + (this.resizeStartSize.height - newSize.height);
          break;
      }
    } else {
      // 文字等其他元素維持原本行為
      switch (this.resizeHandle) {
        case 'se': // 右下角
          newSize.width = Math.max(20, this.resizeStartSize.width + logicalDeltaX);
          newSize.height = Math.max(20, this.resizeStartSize.height + logicalDeltaY);
          break;
        case 'sw': // 左下角
          newSize.width = Math.max(20, this.resizeStartSize.width - logicalDeltaX);
          newSize.height = Math.max(20, this.resizeStartSize.height + logicalDeltaY);
          newPosition.x = this.elementStart.x + (this.resizeStartSize.width - newSize.width);
          break;
        case 'ne': // 右上角
          newSize.width = Math.max(20, this.resizeStartSize.width + logicalDeltaX);
          newSize.height = Math.max(20, this.resizeStartSize.height - logicalDeltaY);
          newPosition.y = this.elementStart.y + (this.resizeStartSize.height - newSize.height);
          break;
        case 'nw': // 左上角
          newSize.width = Math.max(20, this.resizeStartSize.width - logicalDeltaX);
          newSize.height = Math.max(20, this.resizeStartSize.height - logicalDeltaY);
          newPosition.x = this.elementStart.x + (this.resizeStartSize.width - newSize.width);
          newPosition.y = this.elementStart.y + (this.resizeStartSize.height - newSize.height);
          break;
        case 'e': // 右邊
          newSize.width = Math.max(20, this.resizeStartSize.width + logicalDeltaX);
          break;
        case 'w': // 左邊
          newSize.width = Math.max(20, this.resizeStartSize.width - logicalDeltaX);
          newPosition.x = this.elementStart.x + (this.resizeStartSize.width - newSize.width);
          break;
        case 's': // 下邊
          newSize.height = Math.max(20, this.resizeStartSize.height + logicalDeltaY);
          break;
        case 'n': // 上邊
          newSize.height = Math.max(20, this.resizeStartSize.height - logicalDeltaY);
          newPosition.y = this.elementStart.y + (this.resizeStartSize.height - newSize.height);
          break;
      }
    }

    // 確保元素位置和尺寸不超出邏輯畫布邊界（使用已宣告的邏輯畫布尺寸）
    newPosition.x = Math.max(0, Math.min(newPosition.x, logicalCanvasWidth - newSize.width));
    newPosition.y = Math.max(0, Math.min(newPosition.y, logicalCanvasHeight - newSize.height));
    newSize.width = Math.min(newSize.width, logicalCanvasWidth - newPosition.x);
    newSize.height = Math.min(newSize.height, logicalCanvasHeight - newPosition.y);

    // 如果是文字元素，計算新的字體大小
    let updates: any = { size: newSize };

    if (this.element.type === 'text') {
      const textElement = this.getTextElement();
      const containerWidth = newSize.width;
      const containerHeight = newSize.height;
      const widthBasedSize = containerWidth;
      const heightBasedSize = containerHeight;
      const finalFontSize = Math.min(widthBasedSize, heightBasedSize);
      updates.style = {
        ...textElement.style,
        fontSize: Math.round(finalFontSize)
      };
    }

    this.elementUpdated.emit({ id: this.element.id, updates });
    if (newPosition.x !== this.elementStart.x || newPosition.y !== this.elementStart.y) {
      this.elementMoved.emit({ id: this.element.id, position: newPosition });
    }
  }

  private stopResize(): void {
    this.isResizing = false;
    this.resizeHandle = '';
  }

  private setupResizeHandlers(): void {
    // 設置調整大小控制點的事件監聽
  }

  // 類型安全的getter方法
  getTextElement(): any {
    return this.element.type === 'text' ? this.element : {};
  }

  getTextElementFontFamily(): string {
    const textElement = this.getTextElement();
    const fontFamily = textElement.style?.fontFamily || 'Arial';

    // 如果已經包含中文字體，直接返回
    if (fontFamily.includes('Noto Sans TC') || fontFamily.includes('PingFang') || fontFamily.includes('Microsoft JhengHei')) {
      return fontFamily;
    }

    // 為所有字體添加中文字體回退
    return `'Noto Sans TC', 'PingFang TC', 'Microsoft JhengHei', 'Microsoft YaHei', ${fontFamily}, sans-serif`;
  }

  getImageElement(): any {
    return this.element.type === 'image' ? this.element : {};
  }

  getShapeElement(): any {
    return this.element.type === 'shape' ? this.element : {};
  }

  getQRCodeElement(): any {
    return this.element.type === 'qrcode' ? this.element : {};
  }

  enableTextEdit() {
    this.isEditingText = true;
    this.editTextValue = this.getTextElement().content;
  }

  saveTextEdit() {
    this.isEditingText = false;
    this.elementUpdated.emit({
      id: this.element.id,
      updates: { content: this.editTextValue }
    });
  }

  // 新增：視窗resize/scroll時自動更新
  windowUpdateToolbarPosition = () => {
  };

  // 裁剪功能方法
  startCropResize(event: MouseEvent, handle: string): void {
    if (!this.isCropping) return;
    
    event.preventDefault();
    event.stopPropagation();
    
    this.isCropResizing = true;
    this.cropResizeHandle = handle;
    this.cropResizeStart = { x: event.clientX, y: event.clientY };
    this.cropSelectionStart = { ...this.cropSelection };
    
    const mousemove = (e: MouseEvent) => this.onCropResize(e);
    const mouseup = () => {
      this.isCropResizing = false;
      document.removeEventListener('mousemove', mousemove);
      document.removeEventListener('mouseup', mouseup);
    };
    
    document.addEventListener('mousemove', mousemove);
    document.addEventListener('mouseup', mouseup);
  }
  
  onCropResize(event: MouseEvent): void {
    if (!this.isCropResizing) return;
    
    const deltaX = event.clientX - this.cropResizeStart.x;
    const deltaY = event.clientY - this.cropResizeStart.y;
    
    const startSelection = this.cropSelectionStart;
    
    switch (this.cropResizeHandle) {
      case 'nw':
        this.cropSelection.x = Math.max(0, startSelection.x + deltaX);
        this.cropSelection.y = Math.max(0, startSelection.y + deltaY);
        this.cropSelection.width = Math.max(20, startSelection.width - deltaX);
        this.cropSelection.height = Math.max(20, startSelection.height - deltaY);
        break;
      case 'ne':
        this.cropSelection.y = Math.max(0, startSelection.y + deltaY);
        this.cropSelection.width = Math.max(20, startSelection.width + deltaX);
        this.cropSelection.height = Math.max(20, startSelection.height - deltaY);
        break;
      case 'sw':
        this.cropSelection.x = Math.max(0, startSelection.x + deltaX);
        this.cropSelection.width = Math.max(20, startSelection.width - deltaX);
        this.cropSelection.height = Math.max(20, startSelection.height + deltaY);
        break;
      case 'se':
        this.cropSelection.width = Math.max(20, startSelection.width + deltaX);
        this.cropSelection.height = Math.max(20, startSelection.height + deltaY);
        break;
    }
    
    // 確保裁剪區域不超出元素邊界
    this.cropSelection.x = Math.min(this.cropSelection.x, this.element.size.width - this.cropSelection.width);
    this.cropSelection.y = Math.min(this.cropSelection.y, this.element.size.height - this.cropSelection.height);
    this.cropSelection.width = Math.min(this.cropSelection.width, this.element.size.width - this.cropSelection.x);
    this.cropSelection.height = Math.min(this.cropSelection.height, this.element.size.height - this.cropSelection.y);
    
    // 實時發送裁剪數據變更
    this.cropChanged.emit({
      id: this.element.id,
      cropData: { ...this.cropSelection }
    });
  }

  // 裁剪操作：完成
  applyCrop(event: Event): void {
    event.stopPropagation();
    const mouseEvent = event as MouseEvent;
    this.cropChanged.emit({ id: this.element.id, cropData: { ...this.cropSelection, apply: true } });
    // 通知父元件應用裁剪，並退出裁剪模式
    const customEvent = new CustomEvent('finishCrop', { bubbles: true });
    this.elementRef.nativeElement.dispatchEvent(customEvent);
  }

  // 裁剪操作：取消
  cancelCrop(event: Event): void {
    event.stopPropagation();
    // 通知父元件取消裁剪，並退出裁剪模式
    const customEvent = new CustomEvent('cancelCrop', { bubbles: true });
    this.elementRef.nativeElement.dispatchEvent(customEvent);
  }

  onTextToolbarAction(action: string) {
    switch (action) {
      case 'moveUp':
        this.elementUpdated.emit({
          id: this.element.id,
          updates: { zIndex: this.element.zIndex + 1 }
        });
        break;
      case 'moveDown':
        this.elementUpdated.emit({
          id: this.element.id,
          updates: { zIndex: Math.max(1, this.element.zIndex - 1) }
        });
        break;
      case 'duplicate':
        // 創建複製的元素
        const duplicatedElement = {
          ...this.element,
          id: 'text_' + Date.now(),
          position: {
            x: this.element.position.x + 20,
            y: this.element.position.y + 20
          },
          zIndex: this.element.zIndex + 1
        };
        this.elementUpdated.emit({
          id: 'new',
          updates: duplicatedElement
        });
        break;
      case 'delete':
        this.onDeleteElement();
        break;
      default:
        console.log('未處理的文字工具列動作:', action);
    }
  }

  // 動態產生三角形 points
  getTrianglePoints(width: number, height: number): string {
    // 頂點 (50%, 5%)，左下 (5%, 95%)，右下 (95%, 95%)
    const p1 = `${width / 2},${height * 0.05}`;
    const p2 = `${width * 0.05},${height * 0.95}`;
    const p3 = `${width * 0.95},${height * 0.95}`;
    return `${p1} ${p2} ${p3}`;
  }

  // 動態產生五角星 points（五個頂點貼齊外框）
  getStarPoints(width: number, height: number): string {
    // 以外接圓方式，讓五個頂點分別貼齊上下左右
    const cx = width / 2;
    const cy = height / 2;
    // 外圓半徑，讓頂點貼齊上下左右
    const rX = width / 2;
    const rY = height / 2;
    // 內圓半徑比例（正五角星的黃金比例）
    const innerRatio = Math.sin(Math.PI / 10) / Math.sin(7 * Math.PI / 10);
    const rInnerX = rX * innerRatio;
    const rInnerY = rY * innerRatio;
    const points = [];
    for (let i = 0; i < 10; i++) {
      const angle = -Math.PI / 2 + i * Math.PI / 5; // 從正上方開始
      const r = i % 2 === 0 ? { x: rX, y: rY } : { x: rInnerX, y: rInnerY };
      const x = cx + r.x * Math.cos(angle);
      const y = cy + r.y * Math.sin(angle);
      points.push(`${x},${y}`);
    }
    return points.join(' ');
  }

  // 動態產生六邊形 points（蜂巢角度，一邊朝上）
  getHexagonPoints(width: number, height: number): string {
    const cx = width / 2;
    const cy = height / 2;
    const rX = width / 2;
    const rY = height / 2;
    const points = [];
    for (let i = 0; i < 6; i++) {
      const angle = i * Math.PI / 3; // 從正右方開始，一邊朝上
      const x = cx + rX * Math.cos(angle);
      const y = cy + rY * Math.sin(angle);
      points.push(`${x},${y}`);
    }
    return points.join(' ');
  }

  // 縮放相關方法
  getScaledPosition(value: number): number {
    if (!this.canvasElement) return value;
    const logicalCanvasWidth = 800;
    const actualCanvasWidth = this.canvasElement.clientWidth;
    const scale = actualCanvasWidth / logicalCanvasWidth;
    return value * scale;
  }

  getScaledSize(value: number): number {
    if (!this.canvasElement) return value;
    const logicalCanvasWidth = 800;
    const actualCanvasWidth = this.canvasElement.clientWidth;
    const scale = actualCanvasWidth / logicalCanvasWidth;
    return value * scale;
  }

  getScaledFontSize(value: number): number {
    if (!this.canvasElement) return value;
    const logicalCanvasWidth = 800;
    const actualCanvasWidth = this.canvasElement.clientWidth;
    const scale = actualCanvasWidth / logicalCanvasWidth;
    return Math.max(8, value * scale); // 最小字體大小 8px
  }

  // 根據標籤 ID 取得對應的圖標
  getTagIcon(tagId: string): string {
    const tagIcons: { [key: string]: string } = {
      'name': 'person',
      'title': 'work',
      'phone': 'phone',
      'address': 'location_on',
      'company': 'business',
      'custom': 'edit'
    };
    return tagIcons[tagId] || 'label';
  }

  // 根據標籤 ID 取得對應的標籤名稱
  getTagLabel(tagId: string): string {
    const tagLabels: { [key: string]: string } = {
      'name': '姓名',
      'title': '職稱',
      'phone': '電話',
      'address': '地址',
      'company': '公司',
      'custom': '自訂'
    };
    return tagLabels[tagId] || '標籤';
  }
}
