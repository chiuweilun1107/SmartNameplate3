import { Component, Input, OnInit, OnDestroy, AfterViewInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Subject, takeUntil } from 'rxjs';
import { CardDesignerService } from '../services/card-designer.service';
import { CardDesign, CanvasData, CanvasElement, TextElement, ImageElement, ShapeElement, QRCodeElement } from '../models/card-design.models';
import { CryptoService } from '../../../core/services/crypto.service';

@Component({
  selector: 'sn-live-preview',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule
  ],
  templateUrl: './live-preview.component.html',
  styleUrls: ['./live-preview.component.scss']
})
export class LivePreviewComponent implements OnInit, OnDestroy, AfterViewInit, OnChanges {
  @Input() design: CardDesign | null = null;
  
  previewSide: 'A' | 'B' = 'A';
  scale = 0.3; // 預覽縮放比例
  isFullscreen = false;
  currentCanvasData: CanvasData | null = null;
  
  // QR碼模擬圖案
  qrCodePattern = this.generateQRPattern();

  private destroy$ = new Subject<void>();

  constructor(
    private designerService: CardDesignerService,
    private cryptoService: CryptoService
  ) {}

  ngOnInit(): void {
    // 監聽設計變更
    this.designerService.currentDesign$
      .pipe(takeUntil(this.destroy$))
      .subscribe(design => {
        this.design = design;
        this.updateCurrentCanvasData();
      });

    // 監聽當前側面變更
    this.designerService.currentSide$
      .pipe(takeUntil(this.destroy$))
      .subscribe(side => {
        this.previewSide = side;
        this.updateCurrentCanvasData();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  ngAfterViewInit(): void {
    this.updateCanvasScale();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['design'] && this.design) {
      this.updateCanvasScale();
    }
  }

  // 🛡️ 安全的更新當前畫布數據 - 防止 Object Injection
  private updateCurrentCanvasData(): void {
    if (this.design) {
      this.currentCanvasData = this.previewSide === 'A' ? this.design.A : this.design.B;
    }
  }

  get sortedElements() {
    if (!this.currentCanvasData?.elements) return [];
    return [...this.currentCanvasData.elements].sort((a, b) => a.zIndex - b.zIndex);
  }

  // 🛡️ 安全的根據側面獲取排序後的元素 - 防止 Object Injection
  getSortedElements(side: 'A' | 'B') {
    if (!this.design) return [];
    
    const sideData = side === 'A' ? this.design.A : this.design.B;
    if (!sideData?.elements) return [];
    
    return [...sideData.elements].sort((a, b) => a.zIndex - b.zIndex);
  }

  // 🛡️ 安全的根據側面獲取畫布數據 - 防止 Object Injection
  getCanvasData(side: 'A' | 'B') {
    if (!this.design) return null;
    
    return side === 'A' ? this.design.A : this.design.B;
  }

  togglePreviewSide(): void {
    this.previewSide = this.previewSide === 'A' ? 'B' : 'A';
    this.updateCurrentCanvasData();
  }

  toggleFullscreen(): void {
    this.isFullscreen = !this.isFullscreen;
    if (this.isFullscreen) {
      this.scale = 0.8; // 全屏時放大
    } else {
      this.scale = 0.3; // 恢復預覽比例
    }
  }

  // 縮放相關方法
  getScaledPosition(value: number): number {
    return value * this.scale;
  }

  getScaledSize(value: number): number {
    return value * this.scale;
  }

  getScaledFontSize(value: number): number {
    return Math.max(8, value * this.scale); // 最小字體大小
  }

  getScaledValue(value: number | undefined): number {
    if (value === undefined) return 0;
    return value * this.scale;
  }

  // 類型安全的元素獲取方法
  getTextElement(element: CanvasElement): TextElement {
    return element.type === 'text' ? (element as TextElement) : { 
      id: '',
      type: 'text',
      position: { x: 0, y: 0 },
      size: { width: 100, height: 50 },
      zIndex: 0,
      style: {
        fontSize: 16,
        fontFamily: 'Arial, sans-serif',
        fontWeight: 'normal',
        color: '#000000',
        textAlign: 'left'
      },
      content: ''
    };
  }

  getImageElement(element: CanvasElement): ImageElement {
    return element.type === 'image' ? (element as ImageElement) : { 
      id: '',
      type: 'image',
      position: { x: 0, y: 0 },
      size: { width: 100, height: 100 },
      zIndex: 0,
      style: {},
      src: '',
      alt: ''
    };
  }

  getShapeElement(element: CanvasElement): ShapeElement {
    return element.type === 'shape' ? (element as ShapeElement) : { 
      id: '',
      type: 'shape',
      position: { x: 0, y: 0 },
      size: { width: 100, height: 100 },
      zIndex: 0,
      style: {
        backgroundColor: '#e3f2fd'
      },
      shapeType: 'rectangle'
    };
  }

  getQRCodeElement(element: CanvasElement): QRCodeElement {
    return element.type === 'qrcode' ? (element as QRCodeElement) : {
      id: '',
      type: 'qrcode',
      position: { x: 0, y: 0 },
      size: { width: 100, height: 100 },
      zIndex: 0,
      style: {
        backgroundColor: '#fff',
        foregroundColor: '#000',
        borderColor: '#000',
        borderWidth: 0,
        borderRadius: 0
      },
      data: '',
      margin: 4,
      errorCorrectionLevel: 'M'
    };
  }

  // 🛡️ 生成QR碼模擬圖案 - 使用安全的隨機生成和陣列存取
  private generateQRPattern(): boolean[][] {
    const size = 21; // 簡化的QR碼尺寸
    const pattern: boolean[][] = [];
    
    for (let i = 0; i < size; i++) {
      // 🛡️ 安全的陣列初始化
      const row: boolean[] = [];
      pattern.push(row);
      
      for (let j = 0; j < size; j++) {
        let value: boolean;
        
        // 簡單的模擬圖案
        if (i < 7 && j < 7) {
          value = (i + j) % 2 === 0; // 左上角
        } else if (i < 7 && j >= size - 7) {
          value = (i + j) % 2 === 1; // 右上角
        } else if (i >= size - 7 && j < 7) {
          value = (i + j) % 2 === 0; // 左下角
        } else {
          value = this.cryptoService.generateSecureBoolean(); // 🛡️ 安全的隨機填充
        }
        
        // 🛡️ 安全的陣列賦值
        row.push(value);
      }
    }
    
    return pattern;
  }

  // 數學方法暴露給模板
  Math = Math;

  private updateCanvasScale(): void {
    // 計算畫布縮放比例，基準為 400px 寬度
    if (typeof document !== 'undefined') {
      const canvasElements = document.querySelectorAll('.live-preview__canvas');
      canvasElements.forEach((canvas: Element) => {
        const htmlCanvas = canvas as HTMLElement;
        const actualWidth = htmlCanvas.offsetWidth;
        const scale = actualWidth / 400; // 400px 為基準寬度
        htmlCanvas.style.setProperty('--canvas-scale', scale.toString());
      });
    }
  }
}
