import { Component, OnInit, OnDestroy, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { Subject, takeUntil } from 'rxjs';

// 服務和模型
import { CardDesignerService } from './services/card-designer.service';
import { CardDesign, CanvasElement, CanvasData } from './models/card-design.models';
import { ToolbarPositioningService } from '../../shared/services/toolbar-positioning.service';
import { ThumbnailGeneratorService } from '../../shared/services/thumbnail-generator.service';

// 組件
import { DraggableElementComponent } from './components/draggable-element.component';
import { ElementToolbarComponent } from './components/element-toolbar.component';
import { LivePreviewComponent } from './components/live-preview.component';
import { TextToolbarRedesignedComponent, TextStyle } from '../../shared/components/toolbars/text-toolbar-redesigned.component';
import { TemplateModalComponent, Template } from '../../shared/components/modals/template-modal.component';
import { BackgroundModalComponent, BackgroundOption } from '../../shared/components/modals/background-modal.component';
import { TemplateCategoryModalComponent, TemplateCategorySelection } from '../../shared/components/modals/template-category-modal.component';
import { TemplateApiService } from './services/template-api.service';
import { ImageModalComponent, ImageOption } from '../../shared/components/modals/image-modal.component';
import { ElementEditToolbarComponent } from '../../shared/components/toolbars/element-edit-toolbar.component';
import { ColorPickerModalComponent } from '../../shared/components/modals/color-picker-modal.component';
import { ShapeSelectorModalComponent, ShapeOption } from '../../shared/components/modals/shape-selector-modal.component';
import { QRCodeEditorModalComponent, QRCodeSettings } from '../../shared/components/modals/qrcode-editor-modal.component';
import { ImageEditorModalComponent, ImageEditSettings } from '../../shared/components/modals/image-editor-modal.component';
import { SimpleColorPickerModalComponent } from '../../shared/components/modals/simple-color-picker-modal.component';

@Component({
  selector: 'sn-card-designer',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSlideToggleModule,
    MatTooltipModule,
    MatCheckboxModule,
    DraggableElementComponent,
    ElementToolbarComponent,
    LivePreviewComponent,
    TextToolbarRedesignedComponent,
    TemplateModalComponent,
    BackgroundModalComponent,
    ImageModalComponent,
    TemplateCategoryModalComponent,
    ElementEditToolbarComponent,
    ColorPickerModalComponent,
    ShapeSelectorModalComponent,
    QRCodeEditorModalComponent,
    ImageEditorModalComponent,
    SimpleColorPickerModalComponent
  ],
  templateUrl: './card-designer.component.html',
  styleUrls: ['./card-designer.component.scss']
})
export class CardDesignerComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('canvasElement', { static: false }) canvasElement?: ElementRef<HTMLDivElement>;

  // 基本屬性
  cardName = '新圖卡';
  isEditing = false;
  currentSide: 'A' | 'B' = 'A';
  cardId: string | null = null;

  // 模式設定
  isBSameAsA = false;

  // 當前設計和元素
  currentDesign: CardDesign | null = null;
  selectedElementId: string | null = null;

  // 畫布相關
  currentCanvasData: CanvasData | null = null;
  canvasElements: CanvasElement[] = [];
  closeToolbarSignal = 0; // 用於觸發工具列關閉

  showTextToolbar = false;
  toolbarPosition = { x: -9999, y: -9999 };
  currentTextStyle: TextStyle = {};
  selectedTextElementId: string | null = null;

  // 其他元素工具列
  showImageToolbar = false;
  showShapeToolbar = false;
  showQRCodeToolbar = false;
  selectedImageElementId: string | null = null;
  selectedShapeElementId: string | null = null;
  selectedQRCodeElementId: string | null = null;
  
  // 裁剪模式
  croppingElementId: string | null = null;

  // 彈跳視窗狀態
  showTemplateModal = false;
  showBackgroundModal = false;
  showImageModal = false;
  showTemplateCategoryModal = false;

  // 新增模態視窗狀態
  showColorPickerModal = false;
  showShapeSelectorModal = false;
  showQRCodeEditorModal = false;
  showImageEditorModal = false;
  
  // 簡化版顏色選擇器（用於形狀）
  showSimpleColorPickerModal = false;
  
  // 顏色選擇器設定
  colorPickerTitle = '';
  colorPickerCurrentColor = '#e3f2fd';
  colorPickerAction = '';
  
  // 形狀選擇器設定
  currentShapeType = 'rectangle';
  
  // QR碼編輯器設定
  currentQRCodeSettings: QRCodeSettings = {
    data: '@https://example.com',
    size: 100,
    backgroundColor: '#ffffff',
    foregroundColor: '#000000',
    errorCorrectionLevel: 'M',
    margin: 4,
    borderColor: '#000000',
    borderWidth: 0,
    borderRadius: 0
  };

  // 圖片編輯器相關
  currentImageEditSettings: ImageEditSettings = { src: '' };

  // 標籤配置相關
  showTagConfigModal = false;

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private designerService: CardDesignerService,
    private templateApiService: TemplateApiService,
    private positioningService: ToolbarPositioningService,
    private thumbnailGeneratorService: ThumbnailGeneratorService
  ) {}

  ngOnInit(): void {
    this.initializeDesigner();
    this.setupSubscriptions();
  }

  ngAfterViewInit(): void {
    // 設置畫布點擊事件
    this.setupCanvasEvents();

    // 監聽裁剪完成/取消事件
    const canvas = this.canvasElement?.nativeElement;
    if (canvas) {
      canvas.addEventListener('finishCrop', () => {
        this.endCropMode();
      });
      canvas.addEventListener('cancelCrop', () => {
        this.endCropMode();
      });
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeDesigner(): void {
    this.cardId = this.route.snapshot.paramMap.get('id');
    console.log('當前路由參數 - cardId:', this.cardId);
    console.log('當前 URL:', window.location.href);

    if (this.cardId) {
      // 編輯模式
      console.log('進入編輯模式，載入桌牌 ID:', this.cardId);
      this.isEditing = true;
      this.designerService.loadDesign(this.cardId);
      
      // 等待設計載入完成後同步 UI 狀態
      setTimeout(() => {
        this.isBSameAsA = this.designerService.getSyncAB();
        console.log('同步狀態已載入:', this.isBSameAsA);
      }, 500);
    } else {
      // 新建模式 - 清除所有狀態並創建全新設計
      console.log('進入新建模式，創建全新設計');
      this.isEditing = false;
      this.currentSide = 'A';
      this.isBSameAsA = false;
      this.selectedElementId = null;
      this.cardName = '新圖卡';
      
      // 關閉所有工具列和模態視窗
      this.closeAllToolbars();
      
      // 創建全新的設計
      this.currentDesign = this.designerService.createNewDesign('新圖卡');
      
      // 確保設計服務也清空之前的狀態
      this.designerService.clearSelection();
      this.designerService.setSyncAB(false);
      
      console.log('新設計已創建:', this.currentDesign);
    }
  }

  // 頂部工具欄方法
  startEditingName(): void {
    this.isEditing = true;
    // 等待DOM更新後聚焦
    setTimeout(() => {
      const input = document.querySelector('.card-designer__name-field') as HTMLInputElement;
      if (input) {
        input.focus();
        input.select();
      }
    });
  }

  enableNameEdit(event: Event): void {
    if (!this.isEditing) {
      this.isEditing = true;
      setTimeout(() => {
        const input = event.target as HTMLInputElement;
        input.focus();
        input.select();
      });
    }
  }

  saveCardName(): void {
    this.isEditing = false;
    if (this.currentDesign) {
      this.currentDesign.name = this.cardName;
      this.designerService.saveDesign();
    }
  }

  // 訂閱和事件
  private setupSubscriptions(): void {
    // 監聽設計變化
    this.designerService.currentDesign$
      .pipe(takeUntil(this.destroy$))
      .subscribe(design => {
        this.currentDesign = design;
        if (design) {
          this.cardName = design.name;
          this.updateCanvasElements();
        }
      });

    // 監聽當前側面變化
    this.designerService.currentSide$
      .pipe(takeUntil(this.destroy$))
      .subscribe(side => {
        this.currentSide = side;
        this.updateCanvasElements();
      });

    // 監聽選擇元素變化
    this.designerService.selectedElement$
      .pipe(takeUntil(this.destroy$))
      .subscribe(id => {
        this.selectedElementId = id;
      });

    // 監聽AB面同步狀態變化
    this.designerService.isSyncAB$
      .pipe(takeUntil(this.destroy$))
      .subscribe(isSyncAB => {
        this.isBSameAsA = isSyncAB;
      });
  }

  private setupCanvasEvents(): void {
    if (this.canvasElement?.nativeElement) {
      const canvas = this.canvasElement.nativeElement;

      // 畫布點擊事件（取消選擇）
      canvas.addEventListener('click', (event: MouseEvent) => {
        if (event.target === canvas) {
          this.closeToolbarSignal++; // 關閉工具列
          this.designerService.clearSelection();
          
          // 清除選中狀態並關閉所有工具列
          this.selectedElementId = null;
          this.closeAllToolbars();
          
          // 如果處於裁剪模式，結束裁剪
          if (this.croppingElementId) {
            this.endCropMode();
          }
        }
      });
    }
  }

  // 更新畫布元素
  private updateCanvasElements(): void {
    const canvasData = this.designerService.getCurrentCanvasData();
    this.currentCanvasData = canvasData;
    this.canvasElements = canvasData?.elements || [];
  }

  // 用於ngFor的跟蹤函數
  trackByElementId(index: number, element: CanvasElement): string {
    return element?.id || `element-${index}`;
  }

  // 元素操作方法
  onElementSelected(id: string): void {
    this.designerService.selectElement(id);
    this.selectedElementId = id;
    const el = document.getElementById('element-' + id);
    const element = this.canvasElements.find(e => e.id === id);
    
    // 先關閉所有工具列
    this.closeAllToolbars();
    
    if (el && element) {
      // 使用定位服務計算工具列位置
      const position = this.positioningService.calculateToolbarPosition(el);
      this.toolbarPosition = position;

      // 根據元素類型顯示對應的工具列
      switch (element.type) {
        case 'text':
          this.showTextToolbar = true;
          this.selectedTextElementId = id;
          this.currentTextStyle = element.style || {};
          break;
        case 'image':
          this.showImageToolbar = true;
          this.selectedImageElementId = id;
          break;
        case 'shape':
          this.showShapeToolbar = true;
          this.selectedShapeElementId = id;
          break;
        case 'qrcode':
          this.showQRCodeToolbar = true;
          this.selectedQRCodeElementId = id;
          break;
      }
    }
  }

  onElementMoved(data: { id: string, position: { x: number, y: number } }): void {
    this.designerService.moveElement(data.id, data.position);
    if (this.selectedElementId === data.id) {
      this.updateToolbarPositionById(data.id);
    }
  }

  onElementResized(data: { id: string, size: { width: number, height: number } }): void {
    this.designerService.resizeElement(data.id, data.size);
    if (this.selectedElementId === data.id) {
      this.updateToolbarPositionById(data.id);
    }
  }

  onElementUpdated(data: { id: string, updates: Partial<CanvasElement> }): void {
    console.log('元素更新:', data);
    this.designerService.updateElement(data.id, data.updates);
    this.updateCanvasElements();
  }

  // 處理裁剪數據變更
  async onCropChanged(data: { id: string, cropData: { x: number, y: number, width: number, height: number, apply?: boolean } }): Promise<void> {
    console.log('裁剪數據變更:', data);
    // 若是套用裁剪
    if (data.cropData && data.cropData.apply) {
      // 找到該圖片元素
      const element = this.canvasElements.find(e => e.id === data.id && e.type === 'image');
      if (element) {
        const img = new window.Image();
        img.crossOrigin = 'anonymous';
        img.onload = () => {
          // 取得原圖與元素顯示尺寸
          const naturalW = img.naturalWidth;
          const naturalH = img.naturalHeight;
          const displayW = element.size.width;
          const displayH = element.size.height;

          // 計算裁剪區域在原圖上的像素座標
          const scaleX = naturalW / displayW;
          const scaleY = naturalH / displayH;
          const cropX = Math.round(data.cropData.x * scaleX);
          const cropY = Math.round(data.cropData.y * scaleY);
          const cropW = Math.round(data.cropData.width * scaleX);
          const cropH = Math.round(data.cropData.height * scaleY);

          const canvas = document.createElement('canvas');
          canvas.width = cropW;
          canvas.height = cropH;
          const ctx = canvas.getContext('2d');
          if (ctx) {
            ctx.drawImage(
              img,
              cropX,
              cropY,
              cropW,
              cropH,
              0,
              0,
              cropW,
              cropH
            );
            const croppedDataUrl = canvas.toDataURL('image/png');
            this.designerService.updateElement(data.id, {
              src: croppedDataUrl,
              size: {
                width: data.cropData.width,
                height: data.cropData.height
              }
            });
            this.updateCanvasElements();
          }
        };
        img.src = (element as { src: string }).src;
      }
      // 結束裁剪模式
      this.endCropMode();
    }
    // 這裡可以實時更新裁剪預覽或儲存裁剪狀態
  }

  updateToolbarPositionById(id: string) {
    const el = document.getElementById('element-' + id);
    if (el) {
      const position = this.positioningService.calculateToolbarPosition(el);
      this.toolbarPosition = position;
    }
  }

  onTextStyleChange(newStyle: TextStyle) {
    const selectedElement = this.getSelectedCanvasElement();
    if (!selectedElement || selectedElement.type !== 'text') {
      return;
    }

    const updates: Partial<CanvasElement> = {
      style: {
        ...selectedElement.style,
        ...newStyle
      }
    };

    // 如果字體大小變更，自動調整元素高度
    if (newStyle.fontSize && newStyle.fontSize !== selectedElement.style?.fontSize) {
      const fontSize = newStyle.fontSize;
      const padding = selectedElement.style?.padding || 10;
      const lineHeight = 1.2;

      // 計算合適的高度（字體大小 * 行高 + 上下padding）
      const newHeight = Math.max(fontSize * lineHeight + padding * 2, 30);

      updates.size = {
        ...selectedElement.size,
        height: newHeight
      };
    }

    this.onElementUpdated({
      id: selectedElement.id,
      updates
    });

    this.currentTextStyle = { ...this.currentTextStyle, ...newStyle };
  }

  onCloseTextToolbar() {
    this.showTextToolbar = false;
    this.selectedTextElementId = null;
  }

  onDeleteElement() {
    if (this.selectedTextElementId) {
      this.designerService.deleteElement(this.selectedTextElementId);
      this.showTextToolbar = false;
      this.selectedTextElementId = null;
    }
  }

  // 新增的統一元素添加方法
  onAddElement(data: { type: string, options?: { shapeType?: string } }): void {
    let element: CanvasElement;

    switch (data.type) {
      case 'text':
        element = this.designerService.createTextElement();
        break;
      case 'image':
        element = this.designerService.createImageElement('');
        break;
      case 'shape':
        element = this.designerService.createShapeElement(
          (data.options?.shapeType as 'rectangle' | 'circle' | 'line' | 'triangle' | 'star' | 'polygon') || 'rectangle'
        );
        break;
      case 'qrcode':
        element = this.designerService.createQRCodeElement('@https://example.com');
        break;
      default:
        console.warn('未知元素類型:', data.type);
        return;
    }

    if (element) {
      this.designerService.addElement(element);
    }
  }

  onAddTemplate(templateId: string): void {
    // 根據模板ID套用預設內容
    let template: { name: string; A: CanvasData; B: CanvasData };
    if (templateId === 'namecard') {
      template = {
        name: '姓名牌',
        A: {
          elements: [
            {
              type: 'text',
              id: 'el_name',
              content: '王小明',
              position: { x: 300, y: 200 },
              size: { width: 200, height: 60 },
              style: {
                fontSize: 32,
                fontFamily: 'Noto Sans TC, PingFang TC, Microsoft JhengHei, Microsoft YaHei, sans-serif',
                fontWeight: 'bold',
                color: '#222',
                textAlign: 'center',
                backgroundColor: 'transparent',
                borderRadius: 0,
                padding: 0
              },
              zIndex: 1
            },
            {
              type: 'text',
              id: 'el_title',
              content: '工程師',
              position: { x: 300, y: 270 },
              size: { width: 200, height: 40 },
              style: {
                fontSize: 20,
                fontFamily: 'Noto Sans TC, PingFang TC, Microsoft JhengHei, Microsoft YaHei, sans-serif',
                fontWeight: 'normal',
                color: '#666',
                textAlign: 'center',
                backgroundColor: 'transparent',
                borderRadius: 0,
                padding: 0
              },
              zIndex: 2
            }
          ],
          background: '#fff',
          width: 800,
          height: 480 // 修正為480高度
        },
        B: {
          elements: [],
          background: '#fff',
          width: 800,
          height: 480 // 修正為480高度
        }
      };
    } else if (templateId === 'meeting') {
      template = {
        name: '會議室',
        A: {
          elements: [
            {
              type: 'text',
              id: 'el_room',
              content: '會議室 301',
              position: { x: 250, y: 180 },
              size: { width: 300, height: 60 },
              style: {
                fontSize: 30,
                fontFamily: 'Noto Sans TC, PingFang TC, Microsoft JhengHei, Microsoft YaHei, sans-serif',
                fontWeight: 'bold',
                color: '#1976d2',
                textAlign: 'center',
                backgroundColor: 'transparent',
                borderRadius: 0,
                padding: 0
              },
              zIndex: 1
            },
            {
              type: 'text',
              id: 'el_info',
              content: '預約時段：09:00-12:00',
              position: { x: 250, y: 260 },
              size: { width: 300, height: 40 },
              style: {
                fontSize: 18,
                fontFamily: 'Noto Sans TC, PingFang TC, Microsoft JhengHei, Microsoft YaHei, sans-serif',
                fontWeight: 'normal',
                color: '#333',
                textAlign: 'center',
                backgroundColor: 'transparent',
                borderRadius: 0,
                padding: 0
              },
              zIndex: 2
            }
          ],
          background: '#f5faff',
          width: 800,
          height: 480 // 修正為480高度
        },
        B: {
          elements: [],
          background: '#f5faff',
          width: 800,
          height: 480 // 修正為480高度
        }
      };
    } else if (templateId === 'event') {
      template = {
        name: '活動',
        A: {
          elements: [
            {
              type: 'text',
              id: 'el_event',
              content: 'AI 創新論壇',
              position: { x: 250, y: 180 },
              size: { width: 300, height: 60 },
              style: {
                fontSize: 28,
                fontFamily: 'Noto Sans TC, PingFang TC, Microsoft JhengHei, Microsoft YaHei, sans-serif',
                fontWeight: 'bold',
                color: '#7b1fa2',
                textAlign: 'center',
                backgroundColor: 'transparent',
                borderRadius: 0,
                padding: 0
              },
              zIndex: 1
            },
            {
              type: 'text',
              id: 'el_time',
              content: '2024/07/01 14:00',
              position: { x: 250, y: 260 },
              size: { width: 300, height: 40 },
              style: {
                fontSize: 18,
                fontFamily: 'Noto Sans TC, PingFang TC, Microsoft JhengHei, Microsoft YaHei, sans-serif',
                fontWeight: 'normal',
                color: '#333',
                textAlign: 'center',
                backgroundColor: 'transparent',
                borderRadius: 0,
                padding: 0
              },
              zIndex: 2
            }
          ],
          background: '#fff8f5',
          width: 800,
          height: 480 // 修正為480高度
        },
        B: {
          elements: [],
          background: '#fff8f5',
          width: 800,
          height: 480 // 修正為480高度
        }
      };
    } else {
      return;
    }
    // 套用模板內容到當前設計，並同步 service
    const newDesign = {
      id: 'template_' + templateId + '_' + Date.now(),
      name: template.name,
      A: template.A,
      B: template.B,
      createdAt: new Date(),
      updatedAt: new Date(),
      createdBy: 'template',
      isTemplate: false
    };
    this.designerService.setDesign(newDesign);
    this.currentSide = 'A';
    this.updateCanvasElements();
  }

  onAlignElements(alignType: string): void {
    console.log('對齊元素:', alignType);
    // TODO: 實現元素對齊
  }

  onLayerAction(action: string): void {
    console.log('圖層操作:', action);
    // TODO: 實現圖層操作
  }

  // 畫布輔助方法
  getCanvasNativeElement(): HTMLDivElement | null {
    return this.canvasElement?.nativeElement || null;
  }

  // 獲取選中的元素
  getSelectedElement(): HTMLElement | null {
    if (!this.selectedElementId) return null;
    return document.getElementById('element-' + this.selectedElementId);
  }

  // 獲取選中的畫布元素（用於數據操作）
  getSelectedCanvasElement(): CanvasElement | null {
    return this.canvasElements.find(el => el.id === this.selectedElementId) || null;
  }

  /**
   * 取得當前選中文字元素的樣式
   */
  getCurrentTextStyle(): TextStyle {
    const selectedElement = this.getSelectedCanvasElement();
    if (!selectedElement || selectedElement.type !== 'text') {
      return {};
    }
    
    const style = selectedElement.style || {};
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

  // 檢查是否有背景
  hasBackground(): boolean {
    const background = this.currentCanvasData?.background;
    return background !== undefined && background !== '#ffffff' && background !== 'transparent';
  }

  onSyncABChange(checked: boolean): void {
    if (checked) {
      if (confirm('啟用同步會清除B面現有內容，確定要同步嗎？')) {
        this.isBSameAsA = true;
        this.designerService.setSyncAB(true);
      }
    } else {
      this.isBSameAsA = false;
      this.designerService.setSyncAB(false);
    }
  }

  // 彈跳視窗處理方法
  onTemplateSelected(template: Template): void {
    console.log('選擇的樣板:', template);

    if (!this.currentDesign) return;

    // 從 API 獲取完整的樣板資料
    this.templateApiService.getTemplate(template.id).subscribe({
      next: (fullTemplate) => {
        // 套用樣板內容到當前設計
        this.currentDesign!.A = fullTemplate.layoutDataA;
        this.currentDesign!.B = fullTemplate.layoutDataB;

        // 更新設計服務
        this.designerService.setDesign(this.currentDesign!);

        // 強制更新畫布元素
        this.updateCanvasElements();

        // 觸發重新渲染
        setTimeout(() => {
          this.updateCanvasElements();
        }, 100);

        console.log('樣板套用成功');
      },
      error: (error) => {
        console.error('載入樣板詳細資料失敗:', error);
        alert('套用樣板失敗，請稍後再試。');
      }
    });
  }

  onBackgroundSelected(background: BackgroundOption): void {
    console.log('選擇的背景:', background);
    
    if (!this.currentDesign) return;

    // 更新當前設計的背景
    let backgroundStyle: string;
    
    if (background.type === 'image') {
      // 處理圖片背景：提取真實的圖片URL
      const imageUrl = background.preview || background.value;
      // 如果是url()格式，提取其中的URL
      const urlMatch = imageUrl.match(/url\(([^)]+)\)/);
      const actualUrl = urlMatch ? urlMatch[1].replace(/['"]/g, '') : imageUrl;
      backgroundStyle = `url("${actualUrl}") center/cover no-repeat`;
    } else {
      // 純色背景
      backgroundStyle = background.value;
    }

    // 如果啟用AB面同步，同時套用到兩面
    if (this.isBSameAsA) {
      this.currentDesign.A.background = backgroundStyle;
      this.currentDesign.B.background = backgroundStyle;
    } else {
      // 只套用到當前面
      if (this.currentSide === 'A') {
        this.currentDesign.A.background = backgroundStyle;
      } else {
        this.currentDesign.B.background = backgroundStyle;
      }
    }

    // 更新設計服務
    this.designerService.setDesign(this.currentDesign);

    // 強制更新畫布
    this.updateCanvasElements();

    console.log('背景套用成功:', backgroundStyle);
  }

  onImageSelected(image: ImageOption): void {
    console.log('選擇的圖片:', image);
    
    if (!this.currentDesign) return;

    // 如果是替換模式且有選中的圖片元素
    if (this.selectedImageElementId) {
      this.designerService.replaceImage(this.selectedImageElementId, image.url, image.name);
      this.updateCanvasElements();
      console.log('圖片已替換:', image.url);
      this.showImageModal = false;
      return;
    }

    // 原有的添加新圖片邏輯
    const newImageElement: CanvasElement = {
      type: 'image',
      id: 'img_' + Date.now(),
      src: image.url,
      alt: image.name || '圖片',
      position: { x: 100, y: 100 },
      size: { width: 200, height: 150 },
      style: {
        borderRadius: 0,
        opacity: 1
      },
      zIndex: this.getNextZIndex()
    };

    if (this.currentSide === 'A') {
      this.currentDesign.A.elements.push(newImageElement);
    } else {
      this.currentDesign.B.elements.push(newImageElement);
    }

    if (this.isBSameAsA) {
      const syncElement = { ...newImageElement, id: 'img_sync_' + Date.now() };
      if (this.currentSide === 'A') {
        this.currentDesign.B.elements.push(syncElement);
      } else {
        this.currentDesign.A.elements.push(syncElement);
      }
    }

    this.designerService.setDesign(this.currentDesign);
    this.updateCanvasElements();
    console.log('圖片元素添加成功');
  }

  private getNextZIndex(): number {
    const currentElements = this.currentSide === 'A' 
      ? this.currentDesign?.A.elements || []
      : this.currentDesign?.B.elements || [];
    
    const maxZ = Math.max(0, ...currentElements.map(el => el.zIndex || 0));
    return maxZ + 1;
  }

  // QR碼相關方法
  onEditQRContent(): void {
    console.log('編輯QR碼內容');
  }

  onQRCodeSettings(): void {
    console.log('QR碼設定');
  }

  onRegenerateQR(): void {
    console.log('重新生成QR碼');
  }

  // 關閉所有工具列
  closeAllToolbars(): void {
    this.showTextToolbar = false;
    this.showImageToolbar = false;
    this.showShapeToolbar = false;
    this.showQRCodeToolbar = false;
    
    this.selectedTextElementId = null;
    this.selectedImageElementId = null;
    this.selectedShapeElementId = null;
    this.selectedQRCodeElementId = null;
  }

  // 處理元素工具列動作
  onElementAction(action: string | { type: string; value: number }): void {
    console.log('元素動作:', action);
    const selectedId = this.selectedImageElementId || this.selectedShapeElementId || this.selectedQRCodeElementId || this.selectedTextElementId;

    // 新增：處理線條粗細調整
    if (typeof action === 'object' && action.type === 'lineThickness') {
      if (this.selectedShapeElementId) {
        const element = this.canvasElements.find(e => e.id === this.selectedShapeElementId);
        if (element && element.type === 'shape' && (element as { shapeType?: string }).shapeType === 'line') {
          // 以 style.height 控制線條粗細
          const currentStyle = (element as { style?: Record<string, unknown> }).style || {};
          const newStyle = { ...currentStyle };
          newStyle['height'] = Number(action.value);
          this.designerService.updateElement(this.selectedShapeElementId, { style: newStyle, size: { ...element.size, height: action.value } });
          this.updateCanvasElements();
          console.log('線條粗細已更新:', action.value);
        }
      }
      return;
    }

    switch (action) {
      case 'delete':
        if (selectedId) {
          this.designerService.deleteElement(selectedId);
          this.closeAllToolbars();
        }
        break;
      case 'duplicate':
        if (selectedId) {
          const duplicatedElement = this.designerService.duplicateElement(selectedId);
          if (duplicatedElement) {
            this.updateCanvasElements();
            console.log('元素複製成功:', duplicatedElement.id);
          }
        }
        break;
      case 'moveUp':
        if (selectedId) {
          this.designerService.moveElementUp(selectedId);
          this.updateCanvasElements();
          console.log('元素上移一層:', selectedId);
        }
        break;
      case 'moveDown':
        if (selectedId) {
          this.designerService.moveElementDown(selectedId);
          this.updateCanvasElements();
          console.log('元素下移一層:', selectedId);
        }
        break;
      // 圖片特定動作
      case 'replace':
        console.log('替換圖片');
        this.showImageModal = true;
        break;
      case 'crop':
        this.startCropMode();
        break;
      case 'filter':
        this.openImageEditor();
        break;
      // 形狀特定動作
      case 'changeShape':
        this.openShapeSelector(true);
        break;
      case 'fillColor':
        this.openFillColorPicker();
        break;
      case 'borderColor':
        this.openBorderColorPicker();
        break;
      // QR碼特定動作
      case 'editContent':
        this.openQRCodeEditor();
        break;
      case 'qrcodeSettings':
        this.openQRCodeEditor();
        break;
      case 'regenerate':
        this.regenerateQRCode();
        break;
      default:
        console.log('未知動作:', action);
    }
  }

  // 開啟形狀選擇器
  openShapeSelector(isAdd = false): void {
    if (isAdd) {
      this.selectedShapeElementId = null; // 強制清空，確保只新增
    } else if (this.selectedShapeElementId) {
      const element = this.canvasElements.find(e => e.id === this.selectedShapeElementId);
      if (element && element.type === 'shape') {
        this.currentShapeType = (element as { shapeType?: string }).shapeType ?? 'rectangle';
      }
    }
    this.showShapeSelectorModal = true;
  }

  // 處理形狀選擇
  onShapeSelected(shape: ShapeOption): void {
    if (this.selectedShapeElementId) {
      // 替換已選 shape
      this.designerService.updateShapeType(this.selectedShapeElementId, shape.type);
      this.updateCanvasElements();
      console.log('形狀已更新:', shape.type);
    } else {
      // 未選 shape，直接新增
      const element = this.designerService.createShapeElement(shape.type);
      this.designerService.addElement(element);
      this.updateCanvasElements();
      console.log('已新增形狀:', shape.type);
    }
    this.showShapeSelectorModal = false;
  }

  // 開啟填充顏色選擇器
  openFillColorPicker(): void {
    if (this.selectedShapeElementId) {
      const element = this.canvasElements.find(e => e.id === this.selectedShapeElementId);
      if (element && element.type === 'shape') {
        this.colorPickerCurrentColor = (element as { style?: { backgroundColor?: string } }).style?.backgroundColor ?? '#e3f2fd';
      }
    }
    this.colorPickerTitle = '選擇填充顏色';
    this.colorPickerAction = 'fillColor';
    this.showSimpleColorPickerModal = true;
  }

  // 開啟邊框顏色選擇器
  openBorderColorPicker(): void {
    if (this.selectedShapeElementId) {
      const element = this.canvasElements.find(e => e.id === this.selectedShapeElementId);
      if (element && element.type === 'shape') {
        this.colorPickerCurrentColor = (element as { style?: { borderColor?: string } }).style?.borderColor ?? '#2196f3';
      }
    }
    this.colorPickerTitle = '選擇邊框顏色';
    this.colorPickerAction = 'borderColor';
    this.showSimpleColorPickerModal = true;
  }

  // 處理顏色選擇
  onColorSelected(color: string): void {
    if (this.selectedShapeElementId) {
      const element = this.canvasElements.find(e => e.id === this.selectedShapeElementId);
      if (element && element.type === 'shape') {
        const currentStyle = (element as { style?: Record<string, unknown> }).style || {};
        const newStyle = { ...currentStyle };

        if (this.colorPickerAction === 'fillColor') {
          newStyle['backgroundColor'] = color;
        } else if (this.colorPickerAction === 'borderColor') {
          newStyle['borderColor'] = color;
        }

        this.designerService.updateElement(this.selectedShapeElementId, { style: newStyle });
        this.updateCanvasElements();
        console.log('顏色已更新:', color);
      }
    }
    this.showSimpleColorPickerModal = false;
  }

  // 開啟QR碼編輯器
  openQRCodeEditor(): void {
    if (this.selectedQRCodeElementId) {
      const element = this.canvasElements.find(e => e.id === this.selectedQRCodeElementId);
      if (element && element.type === 'qrcode') {
        const qr = element as { style?: Record<string, unknown>; data?: string; errorCorrectionLevel?: string; margin?: number };
        this.currentQRCodeSettings = {
          data: (qr.data ?? '@https://example.com') as string,
          size: element.size?.width || 100,
          backgroundColor: (qr.style?.['backgroundColor'] as string) ?? '#ffffff',
          foregroundColor: (qr.style?.['foregroundColor'] as string) ?? '#000000',
          errorCorrectionLevel: (qr.errorCorrectionLevel as 'M' | 'L' | 'Q' | 'H') ?? 'M',
          margin: (qr.margin as number) ?? 4,
          borderColor: (qr.style?.['borderColor'] as string) ?? '#000000',
          borderWidth: (qr.style?.['borderWidth'] as number) ?? 0,
          borderRadius: (qr.style?.['borderRadius'] as number) ?? 0
        };
      }
    }
    this.showQRCodeEditorModal = true;
  }

  // 處理QR碼設定變更
  onQRCodeSettingsChanged(settings: QRCodeSettings): void {
    if (this.selectedQRCodeElementId) {
      const updates = {
        data: settings.data,
        size: { width: settings.size, height: settings.size },
        style: {
          backgroundColor: settings.backgroundColor,
          foregroundColor: settings.foregroundColor,
          borderColor: settings.borderColor,
          borderWidth: settings.borderWidth,
          borderRadius: settings.borderRadius
        },
        errorCorrectionLevel: settings.errorCorrectionLevel,
        margin: settings.margin
      };

      // 添加調試信息
      console.log('🔍 QR碼更新調試:', {
        elementId: this.selectedQRCodeElementId,
        settings: settings,
        updates: updates,
        marginInSettings: settings.margin,
        marginInUpdates: updates.margin
      });

      this.designerService.updateElement(this.selectedQRCodeElementId, updates);
      this.updateCanvasElements();
      console.log('QR碼設定已更新:', settings);
    }
    this.showQRCodeEditorModal = false;
  }

  // 重新生成QR碼
  regenerateQRCode(): void {
    if (this.selectedQRCodeElementId) {
      // 添加時間戳來強制重新生成QR碼
      const element = this.canvasElements.find(e => e.id === this.selectedQRCodeElementId);
      if (element && element.type === 'qrcode') {
        const currentData = (element as { data?: string }).data || '@https://example.com';
        const timestamp = Date.now();
        
        // 暫時改變數據來觸發重新渲染
        this.designerService.updateElement(this.selectedQRCodeElementId, { 
          data: currentData + '?t=' + timestamp 
        });
        
        // 立即恢復原數據，但QR碼已經重新生成
        setTimeout(() => {
          if (this.selectedQRCodeElementId) {
            this.designerService.updateElement(this.selectedQRCodeElementId, { 
              data: currentData 
            });
            this.updateCanvasElements();
          }
        }, 100);
        
        console.log('QR碼已重新生成');
      }
    }
  }

  // 關閉顏色選擇器
  onColorPickerClose(): void {
    this.showSimpleColorPickerModal = false;
  }

  // 關閉形狀選擇器
  onShapeSelectorClose(): void {
    this.showShapeSelectorModal = false;
  }

  // 關閉QR碼編輯器
  onQRCodeEditorClose(): void {
    this.showQRCodeEditorModal = false;
  }

  // 開啟圖片編輯器
  openImageEditor(): void {
    if (this.selectedImageElementId) {
      const element = this.canvasElements.find(e => e.id === this.selectedImageElementId);
      if (element && element.type === 'image') {
        this.currentImageEditSettings = {
          src: (element as { src: string }).src,
          filter: (element as { style?: Record<string, unknown> }).style?.['filter'] as string
        };
        this.showImageEditorModal = true;
      }
    }
  }

  // 處理圖片編輯設定變更
  onImageEditSettingsChanged(settings: ImageEditSettings): void {
    if (this.selectedImageElementId) {
      const element = this.canvasElements.find(e => e.id === this.selectedImageElementId);
      if (element && element.type === 'image') {
        const updates: { style: Record<string, unknown> } = {
          style: {
            ...(element as { style?: Record<string, unknown> }).style,
            filter: settings.filter || undefined
          }
        };
      
      // 這裡可以添加裁剪邏輯
      if (settings.cropData) {
        // 實際應用中可能需要服務端處理裁剪
        console.log('裁剪設定:', settings.cropData);
      }

      this.designerService.updateElement(this.selectedImageElementId, updates);
      this.updateCanvasElements();
      console.log('圖片編輯設定已更新:', settings);
      }
    }
    this.showImageEditorModal = false;
  }

  // 關閉圖片編輯器
  onImageEditorClose(): void {
    this.showImageEditorModal = false;
  }

  startCropMode(): void {
    if (this.selectedImageElementId) {
      // 啟用裁剪模式
      this.croppingElementId = this.selectedImageElementId;
      
      // 關閉工具列，因為會進入裁剪模式
      this.closeAllToolbars();
      
      console.log('開始裁剪模式，元素ID:', this.selectedImageElementId);
      
      // 點擊畫布其他地方時結束裁剪模式
      const clickHandler = (event: MouseEvent) => {
        const target = event.target as HTMLElement;
        // 如果點擊的不是裁剪控制點，則結束裁剪模式
        if (!target.classList.contains('crop-handle')) {
          this.endCropMode();
          document.removeEventListener('click', clickHandler);
        }
      };
      
      // 延遲添加事件監聽器，避免立即觸發
      setTimeout(() => {
        document.addEventListener('click', clickHandler);
      }, 100);
      
      // 提示用戶如何使用裁剪功能
      setTimeout(() => {
        console.log('💡 裁剪提示：拖拽圖片四角的藍色控制點調整裁剪區域，點擊其他地方完成裁剪。');
      }, 200);
    }
  }
  
  // 結束裁剪模式
  endCropMode(): void {
    if (this.croppingElementId) {
      console.log('結束裁剪模式');
      this.croppingElementId = null;
    }
  }
  
  // 檢查元素是否處於裁剪模式
  isElementCropping(elementId: string): boolean {
    return this.croppingElementId === elementId;
  }

  // 新增：取得目前選中 shape 的 shapeType
  getSelectedShapeType(): string {
    const el = this.getSelectedCanvasElement();
    return el && el.type === 'shape' ? (el as { shapeType?: string }).shapeType ?? '' : '';
  }

  // 新增：取得目前選中 shape 的 style
  getSelectedShapeStyle(): Record<string, unknown> {
    const el = this.getSelectedCanvasElement();
    return el && el.type === 'shape' ? (el as { style?: Record<string, unknown> }).style ?? {} : {};
  }

  // 改良版儲存桌牌（包含縮圖生成）
  async saveCard(): Promise<void> {
    try {
      if (!this.currentDesign) {
        alert('沒有設計可以儲存');
        return;
      }

      console.log('🤖 開始儲存桌牌，包含縮圖生成...');

      // 生成A面和B面的縮圖
      console.log('📸 正在生成A面和B面縮圖...');
      const { thumbnailA, thumbnailB } = await this.thumbnailGeneratorService.generateBothThumbnails(
        this.currentDesign.A,
        this.currentDesign.B,
        this.designerService
      );

      console.log('✅ 縮圖生成完成');
      console.log('A面縮圖大小:', Math.round(thumbnailA.length / 1024), 'KB');
      console.log('B面縮圖大小:', Math.round(thumbnailB.length / 1024), 'KB');

      // 儲存設計（包含縮圖）
      this.designerService.saveDesign(thumbnailA, thumbnailB).subscribe({
        next: (response) => {
          console.log('💾 桌牌和縮圖儲存成功:', response);
          
          // 🎯 自動下載高解析度A面和B面圖片（透過後端API）
          this.downloadHighResolutionImages(response.id);
          
          alert('桌牌已儲存成功！\n✅ 包含A面和B面縮圖\n📥 A面和B面圖片已自動下載');
          
          // 如果是新建的卡片，更新URL以反映新的ID
          if (response.id && this.currentDesign?.id.startsWith('new_')) {
            this.currentDesign.id = response.id.toString();
          }
        },
        error: (error) => {
          console.error('❌ 桌牌儲存失敗:', error);
          
          let errorMessage = '桌牌儲存失敗，請稍後再試。';
          if (error.status === 413) {
            errorMessage = '縮圖檔案過大，請簡化設計後再試。';
          } else if (error.status === 400) {
            errorMessage = '資料格式錯誤，請檢查設計內容。';
          }
          
          alert(errorMessage);
        }
      });

    } catch (error) {
      console.error('🚨 生成縮圖或儲存過程中發生異常:', error);
      
      // 如果縮圖生成失敗，詢問是否要繼續保存（不含縮圖）
      const shouldContinue = confirm(
        '縮圖生成失敗，是否要繼續儲存桌牌（不含縮圖）？\n\n' +
        '選擇「確定」繼續儲存\n' +
        '選擇「取消」放棄儲存'
      );
      
      if (shouldContinue) {
        // 不含縮圖的保存
        this.designerService.saveDesign().subscribe({
          next: (response) => {
            console.log('💾 桌牌儲存成功（不含縮圖）:', response);
            alert('桌牌已儲存成功！\n⚠️ 縮圖生成失敗，僅儲存設計內容');
          },
          error: (saveError) => {
            console.error('❌ 桌牌儲存失敗:', saveError);
            alert('桌牌儲存失敗，請稍後再試。');
          }
        });
      }
    }
  }

  // 🎯 透過後端API下載高解析度圖片（無損轉換）
  private downloadHighResolutionImages(cardId: number): void {
    try {
      console.log('📥 開始透過後端API下載高解析度圖片...');
      
      // 使用 fetch API 正確處理二進制檔案下載
      const downloadUrl = `/api/bluetooth/cards/${cardId}/download-images`;
      
      fetch(downloadUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        }
      })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        // 從回應標頭獲取檔案名稱
        const contentDisposition = response.headers.get('content-disposition');
        let filename = `${this.cardName}_高解析度圖片.zip`;
        
        if (contentDisposition) {
          const filenameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
          if (filenameMatch && filenameMatch[1]) {
            filename = filenameMatch[1].replace(/['"]/g, '');
          }
        }
        
        // 將回應轉換為 Blob
        return response.blob().then(blob => ({ blob, filename }));
      })
      .then(({ blob, filename }) => {
        // 創建下載連結
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;
        link.style.display = 'none';
        
        // 觸發下載
        document.body.appendChild(link);
        link.click();
        
        // 清理
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
        
        console.log('✅ 高解析度圖片下載成功:', filename);
      })
      .catch(error => {
        console.error('❌ 透過後端API下載圖片失敗:', error);
        alert('下載高解析度圖片時發生錯誤，請稍後再試');
      });
      
    } catch (error) {
      console.error('❌ 下載圖片初始化失敗:', error);
      alert('下載高解析度圖片時發生錯誤，請稍後再試');
    }
  }

  // 🎯 自動下載A面和B面圖片（前端方式，備用）
  private downloadBothSideImages(thumbnailA: string, thumbnailB: string): void {
    try {
      console.log('📥 開始自動下載A面和B面圖片...');
      
      // 生成檔案名稱（使用桌牌名稱）
      const sanitizedCardName = this.cardName.replace(/[^a-zA-Z0-9\u4e00-\u9fff]/g, '_');
      const timestamp = new Date().toISOString().split('T')[0]; // YYYY-MM-DD格式
      
      // 下載A面圖片
      this.downloadImage(thumbnailA, `${sanitizedCardName}_A面_${timestamp}.png`);
      
      // 延遲一點下載B面，避免瀏覽器阻擋多重下載
      setTimeout(() => {
        this.downloadImage(thumbnailB, `${sanitizedCardName}_B面_${timestamp}.png`);
      }, 500);
      
      console.log('✅ A面和B面圖片下載指令已發送');
      
    } catch (error) {
      console.error('❌ 自動下載圖片失敗:', error);
      // 不顯示錯誤提示，避免干擾用戶體驗
    }
  }

  // 🎯 下載單張圖片的輔助方法
  private downloadImage(dataUrl: string, filename: string): void {
    try {
      const link = document.createElement('a');
      link.href = dataUrl;
      link.download = filename;
      link.style.display = 'none';
      
      // 添加到DOM，觸發下載，然後移除
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      
      console.log(`📥 已觸發下載: ${filename}`);
      
    } catch (error) {
      console.error(`❌ 下載 ${filename} 失敗:`, error);
    }
  }

  // 簡化版PNG匯出（暫時顯示提示）
  async exportAsPNG(): Promise<void> {
    alert('PNG 匯出功能正在開發中，敬請期待！');
    console.log('PNG 匯出功能尚未實現');
  }

  // 簡化版樣板保存
  async saveAsTemplate(): Promise<void> {
    if (!this.currentDesign) return;
    this.showTemplateCategoryModal = true;
  }

  async onTemplateSaved(selection: TemplateCategorySelection): Promise<void> {
    if (!this.currentDesign) {
      console.error('當前設計為空，無法保存樣板');
      alert('當前設計為空，無法保存樣板');
      return;
    }

    try {
      console.log('🤖 開始保存樣板，選擇:', selection);

      // 如果樣板名稱與當前桌牌名稱不同，更新桌牌名稱
      if (selection.name !== this.cardName) {
        this.cardName = selection.name;
        this.currentDesign.name = selection.name;
      }

      // 生成樣板縮圖 - 使用與桌牌相同的方法
      console.log('📸 正在生成樣板縮圖...');
      const { thumbnailA, thumbnailB } = await this.thumbnailGeneratorService.generateBothThumbnails(
        this.currentDesign.A,
        this.currentDesign.B,
        this.designerService
      );

      console.log('✅ 樣板縮圖生成完成');
      console.log('A面縮圖大小:', Math.round(thumbnailA.length / 1024), 'KB');
      console.log('B面縮圖大小:', Math.round(thumbnailB.length / 1024), 'KB');

      // 準備保存樣板資訊到資料庫的數據（包含縮圖）
      const templateData = {
        name: selection.name,
        description: `樣板：${selection.name}`,
        thumbnailUrl: thumbnailA, // 使用A面縮圖作為主縮圖
        thumbnailA: thumbnailA,   // A面縮圖
        thumbnailB: thumbnailB,   // B面縮圖
        layoutDataA: this.currentDesign.A || {},
        layoutDataB: this.currentDesign.B || {},
        dimensions: {
          width: 800,
          height: 480
        },
        isPublic: true,
        category: selection.category || 'general'
      };

      // 調用API保存樣板到資料庫
      this.templateApiService.createTemplate(templateData).subscribe({
        next: (savedTemplate) => {
          console.log('💾 樣板保存成功:', savedTemplate);
          alert('樣板保存成功！\n✅ 包含A面和B面縮圖');
        },
        error: (error) => {
          console.error('❌ 保存樣板失敗:', error);
          
          let errorMessage = '保存樣板失敗，請稍後再試。';
          if (error.status === 413) {
            errorMessage = '樣板縮圖檔案過大，請簡化設計後再試。';
          } else if (error.status === 400) {
            errorMessage = '樣板資料格式錯誤，請檢查設計內容。';
          }
          
          alert(errorMessage);
        }
      });

    } catch (error) {
      console.error('🚨 生成樣板縮圖或保存過程中發生異常:', error);
      
      // 如果縮圖生成失敗，詢問是否要繼續保存（不含縮圖）
      const shouldContinue = confirm(
        '樣板縮圖生成失敗，是否要繼續保存樣板（不含縮圖）？\n\n' +
        '選擇「確定」繼續保存\n' +
        '選擇「取消」放棄保存'
      );
      
      if (shouldContinue) {
        // 不含縮圖的保存
        const templateData = {
          name: selection.name,
          description: `樣板：${selection.name}`,
          thumbnailUrl: '',
          thumbnailA: '',
          thumbnailB: '',
          layoutDataA: this.currentDesign.A || {},
          layoutDataB: this.currentDesign.B || {},
          dimensions: {
            width: 800,
            height: 480
          },
          isPublic: true,
          category: selection.category || 'general'
        };

        this.templateApiService.createTemplate(templateData).subscribe({
          next: (savedTemplate) => {
            console.log('💾 樣板保存成功（不含縮圖）:', savedTemplate);
            alert('樣板保存成功！\n⚠️ 縮圖生成失敗，僅保存樣板內容');
          },
          error: (saveError) => {
            console.error('❌ 樣板保存失敗:', saveError);
            alert('樣板保存失敗，請稍後再試。');
          }
        });
      }
    }
  }

  // 修改後的重置方法：根據同步狀態決定重置範圍
  resetCurrentSide(): void {
    const resetMessage = this.isBSameAsA 
      ? '確定要重置A面和B面的所有內容嗎？此操作無法撤銷。'
      : `確定要重置${this.currentSide}面的所有內容嗎？此操作無法撤銷。`;
    
    if (confirm(resetMessage)) {
      if (this.isBSameAsA) {
        // 如果B面與A面相同，重置AB兩面
        this.designerService.clearBothSides();
      } else {
        // 否則只重置當前面
        this.designerService.clearCurrentSide();
      }
    }
  }

  // 模式切換
  switchToSide(side: 'A' | 'B'): void {
    this.designerService.switchSide(side);
  }

  toggleBSameAsA(): void {
    this.isBSameAsA = !this.isBSameAsA;
    this.designerService.setSyncAB(this.isBSameAsA);
    if (this.isBSameAsA) {
      this.designerService.copyAToB();
    }
  }

  // 基本互動方法
  applyTemplate(): void {
    console.log('應用模板');
    // TODO: 實現模板套用
  }

  setBackground(): void {
    console.log('設定背景');
    // TODO: 實現背景設定
  }

  addBlock(): void {
    console.log('添加區塊');
    // TODO: 實現添加區塊
  }

  // 標籤配置相關方法
  hasTextElements(): boolean {
    return this.canvasElements.some(element => element.type === 'text');
  }

  getTextElementsWithTags(): CanvasElement[] {
    return this.canvasElements.filter(element => element.type === 'text');
  }

  getTagIcon(tagId: string): string {
    if (!tagId || typeof tagId !== 'string') return 'label';
    
    const tagIcons = new Map([
      ['name', 'person'],
      ['title', 'work'], 
      ['phone', 'phone'],
      ['address', 'location_on'],
      ['company', 'business'],
      ['custom', 'edit']
    ]);
    
    return tagIcons.get(tagId) || 'label';
  }

  getTagLabel(tagId: string): string {
    if (!tagId || typeof tagId !== 'string') return '標籤';
    
    const tagLabels = new Map([
      ['name', '姓名'],
      ['title', '職稱'],
      ['phone', '電話'],
      ['address', '地址'],
      ['company', '公司'],
      ['custom', '自訂']
    ]);
    
    return tagLabels.get(tagId) || '標籤';
  }

  getElementDisplayName(element: CanvasElement): string {
    if (element.type === 'text') {
      return element.content ? `文字: ${element.content.substring(0, 10)}...` : '空白文字';
    }
    return element.type;
  }
}
