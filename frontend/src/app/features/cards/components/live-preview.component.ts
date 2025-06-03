import { Component, Input, OnInit, OnDestroy, AfterViewInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Subject, takeUntil } from 'rxjs';
import { CardDesign, CanvasData } from '../models/card-design.models';
import { CardDesignerService } from '../services/card-designer.service';

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

  constructor(private designerService: CardDesignerService) {}

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

  private updateCurrentCanvasData(): void {
    if (this.design) {
      this.currentCanvasData = this.design[this.previewSide];
    }
  }

  get sortedElements() {
    if (!this.currentCanvasData?.elements) return [];
    return [...this.currentCanvasData.elements].sort((a, b) => a.zIndex - b.zIndex);
  }

  // 根據側面獲取排序後的元素
  getSortedElements(side: 'A' | 'B') {
    if (!this.design || !this.design[side]?.elements) return [];
    return [...this.design[side].elements].sort((a, b) => a.zIndex - b.zIndex);
  }

  // 根據側面獲取畫布數據
  getCanvasData(side: 'A' | 'B') {
    return this.design ? this.design[side] : null;
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
  getTextElement(element: any): any {
    return element.type === 'text' ? element : { style: {}, content: '' };
  }

  getImageElement(element: any): any {
    return element.type === 'image' ? element : { style: {}, src: '', alt: '' };
  }

  getShapeElement(element: any): any {
    return element.type === 'shape' ? element : { style: {}, shapeType: 'rectangle' };
  }

  getQRCodeElement(element: any): any {
    return element.type === 'qrcode' ? element : {
      style: {
        backgroundColor: '#fff',
        foregroundColor: '#000',
        borderColor: '#000',
        borderWidth: 0,
        borderRadius: 0
      }
    };
  }

  // 生成QR碼模擬圖案
  private generateQRPattern(): boolean[][] {
    const size = 21; // 簡化的QR碼尺寸
    const pattern: boolean[][] = [];
    
    for (let i = 0; i < size; i++) {
      pattern[i] = [];
      for (let j = 0; j < size; j++) {
        // 簡單的模擬圖案
        if (i < 7 && j < 7) pattern[i][j] = (i + j) % 2 === 0; // 左上角
        else if (i < 7 && j >= size - 7) pattern[i][j] = (i + j) % 2 === 1; // 右上角
        else if (i >= size - 7 && j < 7) pattern[i][j] = (i + j) % 2 === 0; // 左下角
        else pattern[i][j] = Math.random() > 0.5; // 隨機填充
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
