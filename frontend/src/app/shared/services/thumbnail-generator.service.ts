import { Injectable } from '@angular/core';
import { CanvasData } from '../../features/cards/models/card-design.models';
import * as htmlToImage from 'html-to-image';

@Injectable({
  providedIn: 'root'
})
export class ThumbnailGeneratorService {

  constructor() {}

  /**
   * 根據 CanvasData 生成縮圖 - 使用 html-to-image 實現精確轉換
   * @param canvasData 畫布資料
   * @param width 縮圖寬度，預設800
   * @param height 縮圖高度，預設480 
   * @returns Promise<string> base64格式的縮圖
   */
  async generateThumbnail(canvasData: CanvasData, width: number = 3200, height: number = 1920): Promise<string> {
    try {
      console.log('🎯 開始生成縮圖，目標尺寸:', width, 'x', height);
      
      // 尋找畫布容器
      const canvasContainer = document.querySelector('.card-designer__canvas-container') as HTMLElement;
      if (!canvasContainer) {
        console.warn('找不到畫布容器，使用備用方案');
        return this.generateThumbnailFallback(canvasData, width, height);
      }

      // 尋找畫布本身
      const canvasElement = canvasContainer.querySelector('.card-designer__canvas') as HTMLElement;
      if (!canvasElement) {
        console.warn('找不到畫布元素，使用備用方案');
        return this.generateThumbnailFallback(canvasData, width, height);
      }

      console.log('✅ 找到畫布元素，開始使用 html-to-image 轉換');

      // 獲取精確的畫布尺寸
      const canvasRect = canvasElement.getBoundingClientRect();
      const actualWidth = canvasRect.width;
      const actualHeight = canvasRect.height;
      
      console.log(`📏 畫布實際尺寸: ${actualWidth}x${actualHeight}, 目標尺寸: ${width}x${height}`);

      // 使用 html-to-image 轉換整個畫布，使用精確的尺寸與邊界處理
      const dataUrl = await htmlToImage.toPng(canvasElement, {
        width: actualWidth,
        height: actualHeight,
        quality: 1, // 最高品質
        style: {
          transform: 'scale(1)',
          transformOrigin: 'top left',
          margin: '0',
          padding: '0',
          // 強制使用 border-box 模式，確保邊界精確
          boxSizing: 'border-box',
          // 防止溢出，確保邊界精確
          overflow: 'hidden',
          // 確保不會有意外的邊距
          border: 'none',
          outline: 'none'
        },
        backgroundColor: canvasData.background || '#ffffff',
        cacheBust: true, // 避免緩存問題
        pixelRatio: 1, // 固定像素比例，避免縮放問題
        filter: (node: HTMLElement) => {
          // 過濾掉不需要的元素（如選中框、控制點等）
          if (node.classList) {
            const excludeClasses = [
              'draggable-element__handles',
              'draggable-element__handle',
              'crop-selection',
              'crop-handle',
              'crop-actions',
              'selected',
              'resize-handle',
              'rotation-handle'
            ];
            return !excludeClasses.some(cls => node.classList.contains(cls));
          }
          return true;
        }
      });

      // 如果目標尺寸與實際尺寸不同，進行縮放
      if (actualWidth !== width || actualHeight !== height) {
        console.log('🔄 需要縮放縮圖到目標尺寸');
        return this.resizeImageDataUrl(dataUrl, width, height);
      }

      console.log('🎉 html-to-image 轉換成功');
      return dataUrl;

    } catch (error) {
      console.error('html-to-image 轉換失敗，使用備用方案:', error);
      return this.generateThumbnailFallback(canvasData, width, height);
    }
  }

  /**
   * 精確轉換單個元素
   */
  async generateElementThumbnail(elementId: string, width: number = 200, height: number = 200): Promise<string | null> {
    try {
      const elementContainer = document.querySelector(`#element-${elementId}`) as HTMLElement;
      if (!elementContainer) {
        console.warn(`找不到元素 #element-${elementId}`);
        return null;
      }

      // 獲取元素的精確邊界
      const elementRect = elementContainer.getBoundingClientRect();
      const actualWidth = elementRect.width;
      const actualHeight = elementRect.height;

      console.log(`📏 元素 ${elementId} 實際尺寸: ${actualWidth}x${actualHeight}, 目標尺寸: ${width}x${height}`);

      // 使用 html-to-image 轉換單個元素，保持精確邊界
      const dataUrl = await htmlToImage.toPng(elementContainer, {
        width: actualWidth,
        height: actualHeight,
        backgroundColor: 'transparent',
        cacheBust: true,
        pixelRatio: 1,
        style: {
          boxSizing: 'border-box',
          overflow: 'hidden',
          border: 'none',
          outline: 'none'
        },
        filter: (node: HTMLElement) => {
          // 過濾掉控制元素
          if (node.classList) {
            const excludeClasses = [
              'draggable-element__handles',
              'draggable-element__handle',
              'resize-handle',
              'rotation-handle'
            ];
            return !excludeClasses.some(cls => node.classList.contains(cls));
          }
          return true;
        }
      });

      // 如果需要調整尺寸
      if (actualWidth !== width || actualHeight !== height) {
        return this.resizeImageDataUrl(dataUrl, width, height);
      }

      console.log(`✅ 元素 ${elementId} 轉換成功`);
      return dataUrl;

    } catch (error) {
      console.error(`元素 ${elementId} 轉換失敗:`, error);
      return null;
    }
  }

  /**
   * 批量轉換畫布區域
   */
  async generateCanvasRegionThumbnail(selector: string, width: number, height: number): Promise<string | null> {
    try {
      const regionElement = document.querySelector(selector) as HTMLElement;
      if (!regionElement) {
        console.warn(`找不到區域 ${selector}`);
        return null;
      }

      // 獲取區域的精確邊界
      const regionRect = regionElement.getBoundingClientRect();
      const actualWidth = regionRect.width;
      const actualHeight = regionRect.height;

      console.log(`📏 區域 ${selector} 實際尺寸: ${actualWidth}x${actualHeight}, 目標尺寸: ${width}x${height}`);

      const dataUrl = await htmlToImage.toPng(regionElement, {
        width: actualWidth,
        height: actualHeight,
        backgroundColor: 'transparent',
        cacheBust: true,
        pixelRatio: 1, // 固定像素比例
        quality: 1, // 最高品質
        style: {
          boxSizing: 'border-box',
          overflow: 'hidden',
          border: 'none',
          outline: 'none'
        },
        filter: (node: HTMLElement) => {
          // 只包含實際內容，排除UI控制元素
          if (node.classList) {
            const excludeClasses = [
              'draggable-element__handles',
              'draggable-element__handle', 
              'crop-selection',
              'crop-handle',
              'crop-actions',
              'selected',
              'resize-handle',
              'rotation-handle'
            ];
            return !excludeClasses.some(cls => node.classList.contains(cls));
          }
          return true;
        }
      });

      // 如果需要調整尺寸
      if (actualWidth !== width || actualHeight !== height) {
        return this.resizeImageDataUrl(dataUrl, width, height);
      }

      console.log(`✅ 區域 ${selector} 轉換成功`);
      return dataUrl;

    } catch (error) {
      console.error(`區域 ${selector} 轉換失敗:`, error);
      return null;
    }
  }

  /**
   * 根據 CanvasData 生成縮圖
   * @param canvasData 畫布資料
   * @param width 縮圖寬度，預設800
   * @param height 縮圖高度，預設480 
   * @returns Promise<string> base64格式的縮圖
   */
  async generateThumbnailFallback(canvasData: CanvasData, width: number, height: number): Promise<string> {
    return new Promise((resolve, reject) => {
      try {
        // 創建離屏canvas
        const canvas = document.createElement('canvas');
        canvas.width = width;
        canvas.height = height;
        const ctx = canvas.getContext('2d');
        
        if (!ctx) {
          reject(new Error('無法創建 2D 上下文'));
          return;
        }

        // 計算縮放比例
        const scaleX = width / (canvasData.width || 800);
        const scaleY = height / (canvasData.height || 480);

        // 設置背景
        this.drawBackground(ctx, canvasData.background || '#ffffff', width, height);

        // 按照正確的z-index順序繪製所有元素
        const sortedElements = [...canvasData.elements].sort((a, b) => (a.zIndex || 0) - (b.zIndex || 0));
        
        // 並行繪製所有元素，確保完全復制畫布樣式
        const drawPromises = sortedElements.map(element => this.drawElementExact(ctx, element, scaleX, scaleY));

        Promise.all(drawPromises).then(() => {
          // 生成 base64 圖片
          const dataURL = canvas.toDataURL('image/jpeg', 0.8);
          resolve(dataURL);
        }).catch(error => {
          console.error('繪製元素時發生錯誤:', error);
          // 即使部分元素繪製失敗，也返回目前的結果
          const dataURL = canvas.toDataURL('image/jpeg', 0.8);
          resolve(dataURL);
        });

      } catch (error) {
        console.error('生成縮圖時發生錯誤:', error);
        reject(error);
      }
    });
  }

  /**
   * 精確繪製單個元素，完全復制畫布樣式
   */
  private async drawElementExact(ctx: CanvasRenderingContext2D, element: any, scaleX: number, scaleY: number): Promise<void> {
    const x = (element.position?.x || 0) * scaleX;
    const y = (element.position?.y || 0) * scaleY;
    const width = (element.size?.width || 0) * scaleX;
    const height = (element.size?.height || 0) * scaleY;

    // 首先嘗試從DOM獲取精確的樣式
    const domElement = await this.getElementFromDOMExact(element.id);
    if (domElement) {
      ctx.drawImage(domElement, x, y, width, height);
      return;
    }

    // 如果DOM獲取失敗，使用改進的手動繪製，確保樣式完全一致
    switch (element.type) {
      case 'text':
        this.drawTextExact(ctx, element, x, y, width, height, scaleX);
        break;
      case 'image':
        await this.drawImageExact(ctx, element, x, y, width, height);
        break;
      case 'shape':
        this.drawShapeExact(ctx, element, x, y, width, height);
        break;
      case 'qrcode':
        await this.drawQRCodeExact(ctx, element, x, y, width, height);
        break;
    }
  }

  /**
   * 從DOM精確獲取元素（改進版）
   */
  private async getElementFromDOMExact(elementId: string): Promise<HTMLCanvasElement | null> {
    try {
      // 尋找畫布上的元素容器
      const elementContainer = document.querySelector(`#element-${elementId}`) as HTMLElement;
      if (!elementContainer) {
        console.warn(`找不到元素 #element-${elementId}`);
        return null;
      }

      // 獲取元素的實際尺寸
      const rect = elementContainer.getBoundingClientRect();
      if (rect.width === 0 || rect.height === 0) {
        console.warn(`元素 ${elementId} 尺寸為0`);
        return null;
      }

      // 創建canvas
      const canvas = document.createElement('canvas');
      canvas.width = Math.ceil(rect.width);
      canvas.height = Math.ceil(rect.height);
      const ctx = canvas.getContext('2d');
      
      if (!ctx) {
        return null;
      }

      // 使用html2canvas風格轉換DOM元素
      const success = await this.renderDOMElementToCanvas(ctx, elementContainer, canvas.width, canvas.height);
      
      if (success) {
        console.log(`成功從DOM獲取元素 ${elementId}，尺寸: ${canvas.width}x${canvas.height}`);
        return canvas;
      } else {
        console.warn(`DOM渲染失敗，元素 ${elementId}`);
        return null;
      }
      
    } catch (error) {
      console.warn(`從DOM獲取元素 ${elementId} 失敗:`, error);
      return null;
    }
  }

  /**
   * 將DOM元素精確渲染到Canvas
   */
  private async renderDOMElementToCanvas(ctx: CanvasRenderingContext2D, element: HTMLElement, width: number, height: number): Promise<boolean> {
    return new Promise((resolve) => {
      try {
        // 找到內容區域
        const contentElement = element.querySelector('.draggable-element__content') as HTMLElement;
        if (!contentElement) {
          console.warn('找不到 .draggable-element__content');
          resolve(false);
          return;
        }

        // 對於QR碼特殊處理
        const qrElement = contentElement.querySelector('qrcode canvas') as HTMLCanvasElement;
        if (qrElement) {
          console.log('找到QR碼canvas，直接複製');
          // 獲取QR碼的父容器樣式（邊框等）
          const qrContainer = contentElement.querySelector('.draggable-element__qrcode') as HTMLElement;
          if (qrContainer) {
            const containerStyle = window.getComputedStyle(qrContainer);
            this.drawElementBackground(ctx, containerStyle, width, height);
          }
          
          // 繪製QR碼內容，考慮邊框偏移
          const borderWidth = qrContainer ? parseFloat(window.getComputedStyle(qrContainer).borderWidth) || 0 : 0;
          const contentX = borderWidth;
          const contentY = borderWidth;
          const contentWidth = width - borderWidth * 2;
          const contentHeight = height - borderWidth * 2;
          
          ctx.drawImage(qrElement, contentX, contentY, contentWidth, contentHeight);
          resolve(true);
          return;
        }

        // 對於SVG元素特殊處理
        const svgElement = contentElement.querySelector('svg') as SVGElement;
        if (svgElement) {
          console.log('找到SVG元素，準備渲染');
          // 先繪製容器背景和邊框
          const containerStyle = window.getComputedStyle(contentElement);
          this.drawElementBackground(ctx, containerStyle, width, height);
          
          this.renderSVGToCanvas(ctx, svgElement, width, height).then((success) => {
            resolve(success);
          });
          return;
        }

        // 對於其他元素，獲取計算樣式並繪製
        const computedStyle = window.getComputedStyle(contentElement);
        this.renderStyledElementToCanvas(ctx, contentElement, computedStyle, width, height);
        
        resolve(true);
      } catch (error) {
        console.warn('渲染DOM元素到Canvas失敗:', error);
        resolve(false);
      }
    });
  }

  /**
   * 繪製元素背景和邊框
   */
  private drawElementBackground(ctx: CanvasRenderingContext2D, style: CSSStyleDeclaration, width: number, height: number): void {
    // 繪製背景
    if (style.backgroundColor && style.backgroundColor !== 'rgba(0, 0, 0, 0)' && style.backgroundColor !== 'transparent') {
      ctx.fillStyle = style.backgroundColor;
      ctx.fillRect(0, 0, width, height);
    }

    // 繪製邊框
    const borderWidth = parseFloat(style.borderWidth) || 0;
    if (borderWidth > 0 && style.borderColor && style.borderColor !== 'rgba(0, 0, 0, 0)' && style.borderColor !== 'transparent') {
      ctx.strokeStyle = style.borderColor;
      ctx.lineWidth = borderWidth;
      
      const borderRadius = parseFloat(style.borderRadius) || 0;
      if (borderRadius > 0) {
        this.drawRoundedRectStroke(ctx, borderWidth/2, borderWidth/2, width - borderWidth, height - borderWidth, borderRadius);
      } else {
        ctx.strokeRect(borderWidth/2, borderWidth/2, width - borderWidth, height - borderWidth);
      }
    }
  }

  /**
   * 將SVG渲染到Canvas
   */
  private async renderSVGToCanvas(ctx: CanvasRenderingContext2D, svg: SVGElement, width: number, height: number): Promise<boolean> {
    return new Promise((resolve) => {
      try {
        const svgData = new XMLSerializer().serializeToString(svg);
        const svgBlob = new Blob([svgData], { type: 'image/svg+xml;charset=utf-8' });
        const url = URL.createObjectURL(svgBlob);
        
        const img = new Image();
        img.onload = () => {
          ctx.drawImage(img, 0, 0, width, height);
          URL.revokeObjectURL(url);
          resolve(true);
        };
        img.onerror = () => {
          console.warn('SVG載入失敗');
          URL.revokeObjectURL(url);
          resolve(false);
        };
        img.src = url;
      } catch (error) {
        console.warn('SVG渲染失敗:', error);
        resolve(false);
      }
    });
  }

  /**
   * 渲染帶樣式的元素到Canvas
   */
  private renderStyledElementToCanvas(ctx: CanvasRenderingContext2D, element: HTMLElement, style: CSSStyleDeclaration, width: number, height: number): void {
    // 繪製背景
    if (style.backgroundColor && style.backgroundColor !== 'rgba(0, 0, 0, 0)') {
      ctx.fillStyle = style.backgroundColor;
      ctx.fillRect(0, 0, width, height);
    }

    // 繪製邊框
    const borderWidth = parseFloat(style.borderWidth) || 0;
    if (borderWidth > 0 && style.borderColor && style.borderColor !== 'rgba(0, 0, 0, 0)') {
      ctx.strokeStyle = style.borderColor;
      ctx.lineWidth = borderWidth;
      
      const borderRadius = parseFloat(style.borderRadius) || 0;
      if (borderRadius > 0) {
        this.drawRoundedRectStroke(ctx, borderWidth/2, borderWidth/2, width - borderWidth, height - borderWidth, borderRadius);
      } else {
        ctx.strokeRect(borderWidth/2, borderWidth/2, width - borderWidth, height - borderWidth);
      }
    }

    // 繪製文字內容
    if (element.textContent && element.textContent.trim()) {
      this.renderTextContent(ctx, element.textContent, style, width, height);
    }
  }

  /**
   * 渲染文字內容
   */
  private renderTextContent(ctx: CanvasRenderingContext2D, text: string, style: CSSStyleDeclaration, width: number, height: number): void {
    ctx.fillStyle = style.color || '#000000';
    ctx.font = `${style.fontStyle} ${style.fontWeight} ${style.fontSize} ${style.fontFamily}`;
    ctx.textAlign = (style.textAlign as CanvasTextAlign) || 'left';
    ctx.textBaseline = 'top';

    const padding = parseFloat(style.padding) || 0;
    const lineHeight = parseFloat(style.fontSize) * 1.2;
    const lines = text.split('\n');

    lines.forEach((line, index) => {
      const y = padding + (index * lineHeight);
      let x = padding;
      
      if (style.textAlign === 'center') {
        x = width / 2;
      } else if (style.textAlign === 'right') {
        x = width - padding;
      }
      
      ctx.fillText(line, x, y);
    });
  }

  /**
   * 繪製圓角邊框
   */
  private drawRoundedRectStroke(ctx: CanvasRenderingContext2D, x: number, y: number, width: number, height: number, radius: number): void {
    ctx.beginPath();
    if (typeof (ctx as any).roundRect === 'function') {
      (ctx as any).roundRect(x, y, width, height, radius);
    } else {
      this.drawRoundedRect(ctx, x, y, width, height, radius);
    }
    ctx.stroke();
  }

  /**
   * 繪製背景
   */
  private drawBackground(ctx: CanvasRenderingContext2D, background: string, width: number, height: number): void {
    if (background.startsWith('url(')) {
      // 處理圖片背景
      const imageUrl = background.match(/url\(([^)]+)\)/)?.[1]?.replace(/['"]/g, '');
      if (imageUrl) {
        const img = new Image();
        img.crossOrigin = 'anonymous';
        img.onload = () => {
          ctx.drawImage(img, 0, 0, width, height);
        };
        img.onerror = () => {
          // 如果圖片載入失敗，使用白色背景
          ctx.fillStyle = '#ffffff';
          ctx.fillRect(0, 0, width, height);
        };
        img.src = imageUrl;
      } else {
        ctx.fillStyle = '#ffffff';
        ctx.fillRect(0, 0, width, height);
      }
    } else {
      // 純色背景
      ctx.fillStyle = background;
      ctx.fillRect(0, 0, width, height);
    }
  }

  /**
   * 繪製單個元素
   */
  private async drawElement(ctx: CanvasRenderingContext2D, element: any, scaleX: number, scaleY: number): Promise<void> {
    const x = (element.position?.x || 0) * scaleX;
    const y = (element.position?.y || 0) * scaleY;
    const width = (element.size?.width || 0) * scaleX;
    const height = (element.size?.height || 0) * scaleY;

    switch (element.type) {
      case 'text':
        this.drawText(ctx, element, x, y, width, height, scaleX);
        break;
      case 'image':
        await this.drawImage(ctx, element, x, y, width, height);
        break;
      case 'shape':
        this.drawShape(ctx, element, x, y, width, height);
        break;
      case 'qrcode':
        await this.drawQRCode(ctx, element, x, y, width, height);
        break;
    }
  }

  /**
   * 繪製文字元素
   */
  private drawText(ctx: CanvasRenderingContext2D, element: any, x: number, y: number, width: number, height: number, scale: number): void {
    const style = element.style || {};
    const fontSize = (style.fontSize || 16) * scale;
    const fontFamily = style.fontFamily || 'Arial, sans-serif';
    const fontWeight = style.fontWeight || 'normal';
    const fontStyle = style.fontStyle || 'normal';
    const color = style.color || '#000000';
    const textAlign = style.textAlign || 'left';
    const backgroundColor = style.backgroundColor;
    const textDecoration = style.textDecoration || 'none';
    const borderWidth = (style.borderWidth || 0) * scale;
    const borderColor = style.borderColor || '#000000';
    const borderRadius = (style.borderRadius || 0) * scale;
    const padding = (style.padding || 0) * scale;

    ctx.save();

    // 設置字體（包含斜體）
    ctx.font = `${fontStyle} ${fontWeight} ${fontSize}px ${fontFamily}`;
    ctx.fillStyle = color;
    ctx.textBaseline = 'top';

    // 繪製背景色和邊框（如果有）
    if ((backgroundColor && backgroundColor !== 'transparent') || borderWidth > 0) {
      if (backgroundColor && backgroundColor !== 'transparent') {
        ctx.fillStyle = backgroundColor;
        if (borderRadius > 0) {
          ctx.beginPath();
          if (typeof (ctx as any).roundRect === 'function') {
            (ctx as any).roundRect(x, y, width, height, borderRadius);
          } else {
            this.drawRoundedRect(ctx, x, y, width, height, borderRadius);
          }
          ctx.fill();
        } else {
          ctx.fillRect(x, y, width, height);
        }
      }

      // 繪製邊框
      if (borderWidth > 0) {
        ctx.strokeStyle = borderColor;
        ctx.lineWidth = borderWidth;
        if (borderRadius > 0) {
          ctx.beginPath();
          if (typeof (ctx as any).roundRect === 'function') {
            (ctx as any).roundRect(x, y, width, height, borderRadius);
          } else {
            this.drawRoundedRect(ctx, x, y, width, height, borderRadius);
          }
          ctx.stroke();
        } else {
          ctx.strokeRect(x, y, width, height);
        }
      }

      // 重設文字顏色
      ctx.fillStyle = color;
    }

    // 繪製文字
    const content = element.content || '';
    const lines = content.split('\n');
    const lineHeight = fontSize * 1.2;
    
    lines.forEach((line: string, index: number) => {
      let textX = x;
      
      if (textAlign === 'center') {
        textX = x + width / 2;
        ctx.textAlign = 'center';
      } else if (textAlign === 'right') {
        textX = x + width;
        ctx.textAlign = 'right';
      } else {
        ctx.textAlign = 'left';
      }
      
      const textY = y + (index * lineHeight) + padding;
      
      // 繪製文字
      ctx.fillText(line, textX, textY);
      
      // 處理文字裝飾（底線、刪除線等）
      if (textDecoration && textDecoration !== 'none') {
        const textMetrics = ctx.measureText(line);
        const textWidth = textMetrics.width;
        let decorationY = textY;
        let decorationX = textX;
        
        // 根據對齊方式調整底線起始位置
        if (textAlign === 'center') {
          decorationX = textX - textWidth / 2;
        } else if (textAlign === 'right') {
          decorationX = textX - textWidth;
        }
        
        ctx.strokeStyle = color;
        ctx.lineWidth = Math.max(1, fontSize * 0.05); // 動態線條寬度
        
        if (textDecoration === 'underline') {
          decorationY = textY + fontSize + 2; // 底線位置
          ctx.beginPath();
          ctx.moveTo(decorationX, decorationY);
          ctx.lineTo(decorationX + textWidth, decorationY);
          ctx.stroke();
        } else if (textDecoration === 'line-through') {
          decorationY = textY + fontSize / 2; // 刪除線位置
          ctx.beginPath();
          ctx.moveTo(decorationX, decorationY);
          ctx.lineTo(decorationX + textWidth, decorationY);
          ctx.stroke();
        }
      }
    });

    ctx.restore();
  }

  /**
   * 繪製圖片元素
   */
  private async drawImage(ctx: CanvasRenderingContext2D, element: any, x: number, y: number, width: number, height: number): Promise<void> {
    return new Promise((resolve) => {
      if (!element.src) {
        resolve();
        return;
      }

      const img = new Image();
      img.crossOrigin = 'anonymous';
      
      img.onload = () => {
        try {
          // 應用樣式
          const style = element.style || {};
          const borderRadius = style.borderRadius || 0;
          const opacity = style.opacity !== undefined ? style.opacity : 1;
          const borderWidth = style.borderWidth || 0;
          const borderColor = style.borderColor || '#000000';

          ctx.save();
          ctx.globalAlpha = opacity;

          // 繪製邊框（如果有）
          if (borderWidth > 0) {
            ctx.strokeStyle = borderColor;
            ctx.lineWidth = borderWidth;
            
            if (borderRadius > 0) {
              ctx.beginPath();
              if (typeof (ctx as any).roundRect === 'function') {
                (ctx as any).roundRect(x, y, width, height, borderRadius);
              } else {
                this.drawRoundedRect(ctx, x, y, width, height, borderRadius);
              }
              ctx.stroke();
            } else {
              ctx.strokeRect(x, y, width, height);
            }
          }

          // 繪製圖片內容（考慮邊框內縮）
          const contentX = x + borderWidth;
          const contentY = y + borderWidth;
          const contentWidth = width - borderWidth * 2;
          const contentHeight = height - borderWidth * 2;

          if (borderRadius > 0) {
            // 繪製圓角 - 增加相容性檢查
            ctx.beginPath();
            const innerRadius = Math.max(0, borderRadius - borderWidth);
            if (typeof (ctx as any).roundRect === 'function') {
              (ctx as any).roundRect(contentX, contentY, contentWidth, contentHeight, innerRadius);
            } else {
              // 舊瀏覽器的圓角實現
              this.drawRoundedRect(ctx, contentX, contentY, contentWidth, contentHeight, innerRadius);
            }
            ctx.clip();
          }

          ctx.drawImage(img, contentX, contentY, contentWidth, contentHeight);
          ctx.restore();
        } catch (error) {
          console.error('繪製圖片時發生錯誤:', error);
        }
        resolve();
      };

      img.onerror = () => {
        console.error('載入圖片失敗:', element.src);
        // 繪製佔位符
        ctx.fillStyle = '#f0f0f0';
        ctx.fillRect(x, y, width, height);
        ctx.fillStyle = '#999';
        ctx.font = '12px Arial';
        ctx.textAlign = 'center';
        ctx.fillText('圖片', x + width/2, y + height/2);
        resolve();
      };

      img.src = element.src;
    });
  }

  /**
   * 繪製形狀元素
   */
  private drawShape(ctx: CanvasRenderingContext2D, element: any, x: number, y: number, width: number, height: number): void {
    const style = element.style || {};
    const backgroundColor = style.backgroundColor || '#e3f2fd';
    const borderColor = style.borderColor || '#2196f3';
    const borderWidth = style.borderWidth || 0;
    const borderRadius = style.borderRadius || 0;

    ctx.save();

    // 設置樣式
    ctx.fillStyle = backgroundColor;
    
    // 只有在有邊框寬度且邊框顏色不是 'none' 或 'transparent' 時才設置邊框
    const shouldDrawBorder = borderWidth > 0 && borderColor && 
                            borderColor !== 'none' && 
                            borderColor !== 'transparent' && 
                            borderColor !== 'rgba(0,0,0,0)';
    
    if (shouldDrawBorder) {
      ctx.strokeStyle = borderColor;
      ctx.lineWidth = borderWidth;
    }

    switch (element.shapeType) {
      case 'rectangle':
        if (borderRadius > 0) {
          ctx.beginPath();
          if (typeof (ctx as any).roundRect === 'function') {
            (ctx as any).roundRect(x, y, width, height, borderRadius);
          } else {
            // 舊瀏覽器的圓角實現
            this.drawRoundedRect(ctx, x, y, width, height, borderRadius);
          }
          ctx.fill();
          if (shouldDrawBorder) ctx.stroke();
        } else {
          ctx.fillRect(x, y, width, height);
          if (shouldDrawBorder) ctx.strokeRect(x, y, width, height);
        }
        break;
      case 'circle':
        ctx.beginPath();
        ctx.arc(x + width/2, y + height/2, Math.min(width, height)/2, 0, 2 * Math.PI);
        ctx.fill();
        if (shouldDrawBorder) ctx.stroke();
        break;
      case 'line':
        // 直線特殊處理，使用背景色作為線條顏色
        ctx.strokeStyle = backgroundColor;
        ctx.lineWidth = Math.max(height, 2);
        ctx.beginPath();
        ctx.moveTo(x, y + height/2);
        ctx.lineTo(x + width, y + height/2);
        ctx.stroke();
        break;
      case 'triangle':
        ctx.beginPath();
        // 頂點 (50%, 5%)，左下 (5%, 95%)，右下 (95%, 95%)
        ctx.moveTo(x + width / 2, y + height * 0.05);
        ctx.lineTo(x + width * 0.05, y + height * 0.95);
        ctx.lineTo(x + width * 0.95, y + height * 0.95);
        ctx.closePath();
        ctx.fill();
        if (shouldDrawBorder) ctx.stroke();
        break;
      case 'star':
        this.drawStar(ctx, x + width/2, y + height/2, Math.min(width, height)/2, Math.min(width, height)/4);
        ctx.fill();
        if (shouldDrawBorder) ctx.stroke();
        break;
      case 'polygon':
        this.drawHexagon(ctx, x + width/2, y + height/2, Math.min(width, height)/2);
        ctx.fill();
        if (shouldDrawBorder) ctx.stroke();
        break;
      default:
        // 預設矩形
        ctx.fillRect(x, y, width, height);
        if (shouldDrawBorder) ctx.strokeRect(x, y, width, height);
        break;
    }

    ctx.restore();
  }

  /**
   * 舊瀏覽器相容的圓角矩形繪製方法
   */
  private drawRoundedRect(ctx: CanvasRenderingContext2D, x: number, y: number, width: number, height: number, radius: number): void {
    ctx.moveTo(x + radius, y);
    ctx.lineTo(x + width - radius, y);
    ctx.quadraticCurveTo(x + width, y, x + width, y + radius);
    ctx.lineTo(x + width, y + height - radius);
    ctx.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
    ctx.lineTo(x + radius, y + height);
    ctx.quadraticCurveTo(x, y + height, x, y + height - radius);
    ctx.lineTo(x, y + radius);
    ctx.quadraticCurveTo(x, y, x + radius, y);
  }

  /**
   * 繪製星形
   */
  private drawStar(ctx: CanvasRenderingContext2D, centerX: number, centerY: number, outerRadius: number, innerRadius: number): void {
    ctx.beginPath();
    for (let i = 0; i < 10; i++) {
      const angle = -Math.PI / 2 + i * Math.PI / 5; // 從正上方開始
      const radius = i % 2 === 0 ? outerRadius : innerRadius;
      const x = centerX + radius * Math.cos(angle);
      const y = centerY + radius * Math.sin(angle);
      if (i === 0) {
        ctx.moveTo(x, y);
      } else {
        ctx.lineTo(x, y);
      }
    }
    ctx.closePath();
  }

  /**
   * 繪製六邊形
   */
  private drawHexagon(ctx: CanvasRenderingContext2D, centerX: number, centerY: number, radius: number): void {
    ctx.beginPath();
    for (let i = 0; i < 6; i++) {
      const angle = i * Math.PI / 3; // 每60度一個頂點
      const x = centerX + radius * Math.cos(angle);
      const y = centerY + radius * Math.sin(angle);
      if (i === 0) {
        ctx.moveTo(x, y);
      } else {
        ctx.lineTo(x, y);
      }
    }
    ctx.closePath();
  }

  /**
   * 繪製QR碼元素
   */
  private async drawQRCode(ctx: CanvasRenderingContext2D, element: any, x: number, y: number, width: number, height: number): Promise<void> {
    return new Promise((resolve) => {
      const style = element.style || {};
      const backgroundColor = style.backgroundColor || '#ffffff';
      const foregroundColor = style.foregroundColor || '#000000';
      const borderWidth = style.borderWidth || 0;
      const borderColor = style.borderColor || '#000000';
      const borderRadius = style.borderRadius || 0;
      const margin = element.margin || 4;

      ctx.save();

      // 繪製背景
      ctx.fillStyle = backgroundColor;
      ctx.fillRect(x, y, width, height);

      // 繪製邊框（如果有）
      const shouldDrawBorder = borderWidth > 0 && borderColor && 
                              borderColor !== 'none' && 
                              borderColor !== 'transparent' && 
                              borderColor !== 'rgba(0,0,0,0)';

      if (shouldDrawBorder) {
        ctx.strokeStyle = borderColor;
        ctx.lineWidth = borderWidth;
        
        if (borderRadius > 0) {
          ctx.beginPath();
          if (typeof (ctx as any).roundRect === 'function') {
            (ctx as any).roundRect(x + borderWidth/2, y + borderWidth/2, width - borderWidth, height - borderWidth, borderRadius);
          } else {
            this.drawRoundedRect(ctx, x + borderWidth/2, y + borderWidth/2, width - borderWidth, height - borderWidth, borderRadius);
          }
          ctx.stroke();
        } else {
          ctx.strokeRect(x + borderWidth/2, y + borderWidth/2, width - borderWidth, height - borderWidth);
        }
      }

      // 計算內容區域（考慮邊框）
      const contentX = x + borderWidth;
      const contentY = y + borderWidth;
      const contentWidth = width - borderWidth * 2;
      const contentHeight = height - borderWidth * 2;

      // 嘗試從DOM獲取真正的QR碼
      this.getQRCodeFromDOM(element.id).then((qrCanvas) => {
        if (qrCanvas) {
          // 直接使用從DOM獲取的QR碼
          const qrSize = Math.min(contentWidth, contentHeight);
          const qrX = contentX + (contentWidth - qrSize) / 2;
          const qrY = contentY + (contentHeight - qrSize) / 2;
          
          ctx.drawImage(qrCanvas, qrX, qrY, qrSize, qrSize);
        } else {
          // 如果無法從DOM獲取，使用備用繪製方法
          this.drawQRCodeFallback(ctx, contentX, contentY, contentWidth, contentHeight, backgroundColor, foregroundColor, margin);
        }
        
        ctx.restore();
        resolve();
      }).catch(() => {
        // 發生錯誤時使用備用方案
        this.drawQRCodeFallback(ctx, contentX, contentY, contentWidth, contentHeight, backgroundColor, foregroundColor, margin);
        ctx.restore();
        resolve();
      });
    });
  }

  /**
   * 從DOM獲取真正的QR碼
   */
  private async getQRCodeFromDOM(elementId: string): Promise<HTMLCanvasElement | null> {
    try {
      // 尋找畫布上的元素
      const elementContainer = document.querySelector(`#element-${elementId}`);
      if (!elementContainer) {
        console.warn(`找不到元素 #element-${elementId}`);
        return null;
      }

      // 尋找QR碼組件
      const qrElement = elementContainer.querySelector('qrcode');
      if (!qrElement) {
        console.warn(`在元素 ${elementId} 中找不到 qrcode 組件`);
        return null;
      }

      // 尋找QR碼的canvas元素
      const qrCanvas = qrElement.querySelector('canvas') as HTMLCanvasElement;
      if (!qrCanvas) {
        console.warn(`在 qrcode 組件中找不到 canvas`);
        return null;
      }

      // 創建新的canvas並複製內容
      const resultCanvas = document.createElement('canvas');
      resultCanvas.width = qrCanvas.width;
      resultCanvas.height = qrCanvas.height;
      const resultCtx = resultCanvas.getContext('2d');
      
      if (!resultCtx) {
        return null;
      }

      // 複製QR碼內容
      resultCtx.drawImage(qrCanvas, 0, 0);
      
      console.log(`成功從DOM獲取QR碼，尺寸: ${qrCanvas.width}x${qrCanvas.height}`);
      return resultCanvas;
      
    } catch (error) {
      console.error('從DOM獲取QR碼時發生錯誤:', error);
      return null;
    }
  }

  /**
   * QR碼備用繪製方案
   */
  private drawQRCodeFallback(ctx: CanvasRenderingContext2D, x: number, y: number, width: number, height: number, 
                           backgroundColor: string, foregroundColor: string, margin: number): void {
    // 繪製背景
    ctx.fillStyle = backgroundColor;
    ctx.fillRect(x, y, width, height);

    // 計算QR碼區域（考慮margin）
    const qrX = x + margin;
    const qrY = y + margin;
    const qrWidth = width - margin * 2;
    const qrHeight = height - margin * 2;

    if (qrWidth > 0 && qrHeight > 0) {
      ctx.fillStyle = foregroundColor;
      
      // 使用29x29的網格（更接近真實QR碼）
      const gridSize = 29;
      const cellSize = Math.min(qrWidth, qrHeight) / gridSize;
      
      // 繪製三個定位點 (7x7)
      this.drawFinderPattern(ctx, qrX, qrY, cellSize);                           // 左上
      this.drawFinderPattern(ctx, qrX + (gridSize - 7) * cellSize, qrY, cellSize);  // 右上
      this.drawFinderPattern(ctx, qrX, qrY + (gridSize - 7) * cellSize, cellSize);  // 左下
      
      // 繪製分隔線（定位點周圍的白色邊框）
      ctx.fillStyle = backgroundColor;
      // 左上分隔線
      ctx.fillRect(qrX + 7 * cellSize, qrY, cellSize, 8 * cellSize);
      ctx.fillRect(qrX, qrY + 7 * cellSize, 8 * cellSize, cellSize);
      // 右上分隔線
      ctx.fillRect(qrX + (gridSize - 8) * cellSize, qrY, cellSize, 8 * cellSize);
      ctx.fillRect(qrX + (gridSize - 8) * cellSize, qrY + 7 * cellSize, 8 * cellSize, cellSize);
      // 左下分隔線
      ctx.fillRect(qrX, qrY + (gridSize - 8) * cellSize, 8 * cellSize, cellSize);
      ctx.fillRect(qrX + 7 * cellSize, qrY + (gridSize - 8) * cellSize, cellSize, 8 * cellSize);
      
      ctx.fillStyle = foregroundColor;
      
      // 繪製時序模式（深色線條）
      for (let i = 8; i < gridSize - 8; i += 2) {
        ctx.fillRect(qrX + 6 * cellSize, qrY + i * cellSize, cellSize, cellSize);  // 垂直時序線
        ctx.fillRect(qrX + i * cellSize, qrY + 6 * cellSize, cellSize, cellSize);  // 水平時序線
      }
      
      // 繪製定位點
      ctx.fillRect(qrX + 6 * cellSize, qrY + 6 * cellSize, cellSize, cellSize);
      
      // 繪製數據模塊（模擬真實的QR碼數據）
      const dataPattern = this.generateDataPattern(gridSize);
      for (let row = 0; row < gridSize; row++) {
        for (let col = 0; col < gridSize; col++) {
          // 跳過定位點和分隔線區域
          if (this.isReservedArea(row, col, gridSize)) {
            continue;
          }
          
          // 根據數據模式決定是否填充
          if (dataPattern[row] && dataPattern[row][col]) {
            ctx.fillRect(qrX + col * cellSize, qrY + row * cellSize, cellSize, cellSize);
          }
        }
      }
    }
  }

  /**
   * 生成模擬的QR碼數據模式
   */
  private generateDataPattern(gridSize: number): boolean[][] {
    const pattern: boolean[][] = [];
    
    for (let row = 0; row < gridSize; row++) {
      pattern[row] = [];
      for (let col = 0; col < gridSize; col++) {
        // 使用複雜的模式生成算法，模擬真實QR碼的數據分佈
        const hash1 = this.simpleHash(`${row},${col}`);
        const hash2 = this.simpleHash(`${col},${row}`);
        const combined = (hash1 + hash2) % 100;
        
        // 創建不同密度的區域
        let threshold = 50;
        if ((row + col) % 3 === 0) threshold = 40;
        if ((row + col) % 5 === 0) threshold = 60;
        if (row % 2 === col % 2) threshold = 45;
        
        pattern[row][col] = combined < threshold;
      }
    }
    
    return pattern;
  }

  /**
   * 檢查是否為保留區域（定位點、分隔線、時序等）
   */
  private isReservedArea(row: number, col: number, gridSize: number): boolean {
    // 定位點區域 (9x9 包含分隔線)
    if ((row < 9 && col < 9) ||                              // 左上
        (row < 9 && col >= gridSize - 9) ||                 // 右上
        (row >= gridSize - 9 && col < 9)) {                 // 左下
      return true;
    }
    
    // 時序線
    if (row === 6 || col === 6) {
      return true;
    }
    
    // 暗模塊
    if (row === 4 * gridSize / 5 && col === 4 * gridSize / 5) {
      return true;
    }
    
    return false;
  }

  /**
   * 簡單的哈希函數
   */
  private simpleHash(str: string): number {
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
      const char = str.charCodeAt(i);
      hash = ((hash << 5) - hash) + char;
      hash = hash & hash;
    }
    return Math.abs(hash);
  }

  /**
   * 繪製QR碼定位點
   */
  private drawFinderPattern(ctx: CanvasRenderingContext2D, x: number, y: number, cellSize: number): void {
    // 外框 (7x7)
    ctx.fillRect(x, y, cellSize * 7, cellSize * 7);
    
    // 內部白色 (5x5)
    ctx.fillStyle = '#ffffff';
    ctx.fillRect(x + cellSize, y + cellSize, cellSize * 5, cellSize * 5);
    
    // 中心黑色 (3x3)
    ctx.fillStyle = '#000000';
    ctx.fillRect(x + cellSize * 2, y + cellSize * 2, cellSize * 3, cellSize * 3);
  }

  /**
   * 批量生成縮圖（A面和B面） - 使用卡面切換確保DOM正確顯示
   */
  async generateBothThumbnails(designA: CanvasData, designB: CanvasData, designerService?: any): Promise<{thumbnailA: string, thumbnailB: string}> {
    console.log('🔄 開始批量生成 A 面和 B 面縮圖（卡面切換模式）');
    
    try {
      // 記錄當前卡面
      const originalSide = designerService?.getCurrentSide() || 'A';
      console.log(`📍 當前卡面: ${originalSide}`);

      // 生成A面縮圖
      console.log('📸 切換到A面生成縮圖...');
      if (designerService) {
        designerService.switchSide('A');
        // 等待DOM更新
        await this.waitForDOMUpdate();
      }
      
      const thumbnailA = await this.generateThumbnail(designA, 3200, 1920);
      console.log('✅ A面縮圖生成完成 (超高解析度)');

      // 生成B面縮圖
      console.log('📸 切換到B面生成縮圖...');
      if (designerService) {
        designerService.switchSide('B');
        // 等待DOM更新
        await this.waitForDOMUpdate();
      }
      
      const thumbnailB = await this.generateThumbnail(designB, 3200, 1920);
      console.log('✅ B面縮圖生成完成');

      // 恢復原始卡面
      if (designerService && originalSide !== designerService.getCurrentSide()) {
        console.log(`🔄 恢復到原始卡面: ${originalSide}`);
        designerService.switchSide(originalSide);
        await this.waitForDOMUpdate();
      }

      console.log('✅ 批量生成完成 - 卡面切換方式');
      return { thumbnailA, thumbnailB };
      
    } catch (error) {
      console.warn('卡面切換生成失敗，使用備用方案:', error);
      
      // 備用方案：使用手動繪製
      console.log('🔄 使用備用方案手動繪製（超高解析度）...');
      const thumbnailA = await this.generateThumbnailFallback(designA, 3200, 1920);
      const thumbnailB = await this.generateThumbnailFallback(designB, 3200, 1920);

      return { thumbnailA, thumbnailB };
    }
  }

  /**
   * 等待DOM更新完成
   */
  private async waitForDOMUpdate(): Promise<void> {
    return new Promise(resolve => {
      // 使用 requestAnimationFrame 確保DOM已更新
      requestAnimationFrame(() => {
        // 雙重確保DOM已完全更新
        setTimeout(() => {
          resolve();
        }, 100); // 等待100ms確保元素已渲染
      });
    });
  }

  /**
   * 檢查畫布是否可用於轉換
   */
  private isCanvasAvailable(): boolean {
    const canvasContainer = document.querySelector('.card-designer__canvas-container');
    const canvasElement = canvasContainer?.querySelector('.card-designer__canvas');
    
    if (!canvasContainer || !canvasElement) {
      console.warn('畫布元素不可用');
      return false;
    }
    
    // 檢查畫布是否有內容
    const elements = canvasElement.querySelectorAll('[id^="element-"]');
    console.log(`畫布包含 ${elements.length} 個元素`);
    
    return true;
  }

  /**
   * 獲取畫布的實際尺寸
   */
  private getCanvasSize(): { width: number, height: number } {
    const canvasElement = document.querySelector('.card-designer__canvas') as HTMLElement;
    if (!canvasElement) {
      return { width: 800, height: 480 };
    }
    
    const rect = canvasElement.getBoundingClientRect();
    return {
      width: rect.width || 800,
      height: rect.height || 480
    };
  }

  /**
   * 精確繪製文字元素，完全復制畫布樣式
   */
  private drawTextExact(ctx: CanvasRenderingContext2D, element: any, x: number, y: number, width: number, height: number, scale: number): void {
    // 使用原有的drawText方法，它已經很準確了
    this.drawText(ctx, element, x, y, width, height, scale);
  }

  /**
   * 精確繪製圖片元素，完全復制畫布樣式
   */
  private async drawImageExact(ctx: CanvasRenderingContext2D, element: any, x: number, y: number, width: number, height: number): Promise<void> {
    // 使用原有的drawImage方法，它已經很準確了
    return this.drawImage(ctx, element, x, y, width, height);
  }

  /**
   * 精確繪製形狀元素，完全復制畫布樣式
   */
  private drawShapeExact(ctx: CanvasRenderingContext2D, element: any, x: number, y: number, width: number, height: number): void {
    const style = element.style || {};
    
    // 使用與畫布完全相同的預設值和樣式邏輯
    const backgroundColor = style.backgroundColor || '#e3f2fd';
    const borderColor = style.borderColor || '#2196f3';
    const borderWidth = style.borderWidth || 2; // 預設邊框寬度為2，與畫布一致
    const borderRadius = style.borderRadius || 0;

    ctx.save();

    // 設置樣式
    ctx.fillStyle = backgroundColor;
    
    // 邊框判斷邏輯要與畫布HTML模板完全一致
    const shouldDrawBorder = borderWidth > 0 && borderColor && 
                            borderColor !== 'none' && 
                            borderColor !== 'transparent' && 
                            borderColor !== 'rgba(0,0,0,0)';
    
    if (shouldDrawBorder) {
      ctx.strokeStyle = borderColor;
      ctx.lineWidth = borderWidth;
    }

    // 確保與畫布SVG形狀樣式完全一致
    switch (element.shapeType) {
      case 'rectangle':
        if (borderRadius > 0) {
          ctx.beginPath();
          if (typeof (ctx as any).roundRect === 'function') {
            (ctx as any).roundRect(x, y, width, height, borderRadius);
          } else {
            this.drawRoundedRect(ctx, x, y, width, height, borderRadius);
          }
          ctx.fill();
          if (shouldDrawBorder) ctx.stroke();
        } else {
          ctx.fillRect(x, y, width, height);
          if (shouldDrawBorder) ctx.strokeRect(x, y, width, height);
        }
        break;
        
      case 'circle':
        ctx.beginPath();
        ctx.arc(x + width/2, y + height/2, Math.min(width, height)/2, 0, 2 * Math.PI);
        ctx.fill();
        if (shouldDrawBorder) ctx.stroke();
        break;
        
      case 'line':
        // 直線特殊處理，使用背景色作為線條顏色
        ctx.strokeStyle = backgroundColor;
        ctx.lineWidth = Math.max(height, 2);
        ctx.beginPath();
        ctx.moveTo(x, y + height/2);
        ctx.lineTo(x + width, y + height/2);
        ctx.stroke();
        break;
        
      case 'triangle':
        // 確保與SVG模板的points完全一致
        ctx.beginPath();
        // 使用與draggable-element.component.html中getTrianglePoints相同的邏輯
        ctx.moveTo(x + width / 2, y + height * 0.05);  // 頂點
        ctx.lineTo(x + width * 0.05, y + height * 0.95);  // 左下
        ctx.lineTo(x + width * 0.95, y + height * 0.95);  // 右下
        ctx.closePath();
        ctx.fill();
        if (shouldDrawBorder) ctx.stroke();
        break;
        
      case 'star':
        // 使用與SVG模板相同的星形繪製邏輯
        this.drawStarExact(ctx, x + width/2, y + height/2, Math.min(width, height)/2, Math.min(width, height)/4);
        ctx.fill();
        if (shouldDrawBorder) ctx.stroke();
        break;
        
      case 'polygon':
        // 六邊形，確保與SVG模板一致
        this.drawHexagonExact(ctx, x + width/2, y + height/2, Math.min(width, height)/2);
        ctx.fill();
        if (shouldDrawBorder) ctx.stroke();
        break;
        
      default:
        // 預設矩形
        ctx.fillRect(x, y, width, height);
        if (shouldDrawBorder) ctx.strokeRect(x, y, width, height);
        break;
    }

    ctx.restore();
  }

  /**
   * 精確繪製星形，與SVG版本保持一致
   */
  private drawStarExact(ctx: CanvasRenderingContext2D, centerX: number, centerY: number, outerRadius: number, innerRadius: number): void {
    ctx.beginPath();
    for (let i = 0; i < 10; i++) {
      const angle = -Math.PI / 2 + i * Math.PI / 5; // 從正上方開始，與SVG一致
      const radius = i % 2 === 0 ? outerRadius : innerRadius;
      const x = centerX + radius * Math.cos(angle);
      const y = centerY + radius * Math.sin(angle);
      if (i === 0) {
        ctx.moveTo(x, y);
      } else {
        ctx.lineTo(x, y);
      }
    }
    ctx.closePath();
  }

  /**
   * 精確繪製六邊形，與SVG版本保持一致
   */
  private drawHexagonExact(ctx: CanvasRenderingContext2D, centerX: number, centerY: number, radius: number): void {
    ctx.beginPath();
    for (let i = 0; i < 6; i++) {
      const angle = i * Math.PI / 3; // 每60度一個頂點，與SVG一致
      const x = centerX + radius * Math.cos(angle);
      const y = centerY + radius * Math.sin(angle);
      if (i === 0) {
        ctx.moveTo(x, y);
      } else {
        ctx.lineTo(x, y);
      }
    }
    ctx.closePath();
  }

  /**
   * 精確繪製QR碼元素，完全復制畫布樣式
   */
  private async drawQRCodeExact(ctx: CanvasRenderingContext2D, element: any, x: number, y: number, width: number, height: number): Promise<void> {
    return new Promise((resolve) => {
      const style = element.style || {};
      
      // 使用與畫布完全相同的樣式邏輯
      const backgroundColor = style.backgroundColor || '#ffffff';
      const foregroundColor = style.foregroundColor || '#000000';
      const borderWidth = style.borderWidth || 0;
      const borderColor = style.borderColor || '#000000';
      const borderRadius = style.borderRadius || 0;
      const margin = element.margin || 4;

      ctx.save();

      // 繪製背景
      ctx.fillStyle = backgroundColor;
      ctx.fillRect(x, y, width, height);

      // 繪製邊框（與畫布HTML模板邏輯一致）
      const shouldDrawBorder = borderWidth > 0;  // 簡化判斷，與HTML模板一致

      if (shouldDrawBorder) {
        ctx.strokeStyle = borderColor;
        ctx.lineWidth = borderWidth;
        
        if (borderRadius > 0) {
          ctx.beginPath();
          if (typeof (ctx as any).roundRect === 'function') {
            (ctx as any).roundRect(x + borderWidth/2, y + borderWidth/2, width - borderWidth, height - borderWidth, borderRadius);
          } else {
            this.drawRoundedRect(ctx, x + borderWidth/2, y + borderWidth/2, width - borderWidth, height - borderWidth, borderRadius);
          }
          ctx.stroke();
        } else {
          ctx.strokeRect(x + borderWidth/2, y + borderWidth/2, width - borderWidth, height - borderWidth);
        }
      }

      // 計算內容區域（考慮邊框）
      const contentX = x + borderWidth;
      const contentY = y + borderWidth;
      const contentWidth = width - borderWidth * 2;
      const contentHeight = height - borderWidth * 2;

      // 嘗試從DOM獲取真正的QR碼，如果失敗使用改進的備用方案
      this.getQRCodeFromDOM(element.id).then((qrCanvas) => {
        if (qrCanvas) {
          // 直接使用從DOM獲取的QR碼
          const qrSize = Math.min(contentWidth, contentHeight);
          const qrX = contentX + (contentWidth - qrSize) / 2;
          const qrY = contentY + (contentHeight - qrSize) / 2;
          
          ctx.drawImage(qrCanvas, qrX, qrY, qrSize, qrSize);
        } else {
          // 使用改進的備用繪製方法
          this.drawQRCodeFallbackExact(ctx, contentX, contentY, contentWidth, contentHeight, backgroundColor, foregroundColor, margin);
        }
        
        ctx.restore();
        resolve();
      }).catch(() => {
        // 發生錯誤時使用備用方案
        this.drawQRCodeFallbackExact(ctx, contentX, contentY, contentWidth, contentHeight, backgroundColor, foregroundColor, margin);
        ctx.restore();
        resolve();
      });
    });
  }

  /**
   * 改進的QR碼備用繪製方案，確保與真實QR碼更相似
   */
  private drawQRCodeFallbackExact(ctx: CanvasRenderingContext2D, x: number, y: number, width: number, height: number, 
                           backgroundColor: string, foregroundColor: string, margin: number): void {
    // 繪製背景
    ctx.fillStyle = backgroundColor;
    ctx.fillRect(x, y, width, height);

    // 計算QR碼區域（考慮margin）
    const qrX = x + margin;
    const qrY = y + margin;
    const qrWidth = width - margin * 2;
    const qrHeight = height - margin * 2;

    if (qrWidth > 0 && qrHeight > 0) {
      ctx.fillStyle = foregroundColor;
      
      // 使用29x29的網格（更接近真實QR碼）
      const gridSize = 29;
      const cellSize = Math.min(qrWidth, qrHeight) / gridSize;
      
      // 繪製三個定位點 (7x7)
      this.drawFinderPatternExact(ctx, qrX, qrY, cellSize, backgroundColor, foregroundColor);                           // 左上
      this.drawFinderPatternExact(ctx, qrX + (gridSize - 7) * cellSize, qrY, cellSize, backgroundColor, foregroundColor);  // 右上
      this.drawFinderPatternExact(ctx, qrX, qrY + (gridSize - 7) * cellSize, cellSize, backgroundColor, foregroundColor);  // 左下
      
      // 繪製分隔線（定位點周圍的白色邊框）
      ctx.fillStyle = backgroundColor;
      // 左上分隔線
      ctx.fillRect(qrX + 7 * cellSize, qrY, cellSize, 8 * cellSize);
      ctx.fillRect(qrX, qrY + 7 * cellSize, 8 * cellSize, cellSize);
      // 右上分隔線
      ctx.fillRect(qrX + (gridSize - 8) * cellSize, qrY, cellSize, 8 * cellSize);
      ctx.fillRect(qrX + (gridSize - 8) * cellSize, qrY + 7 * cellSize, 8 * cellSize, cellSize);
      // 左下分隔線
      ctx.fillRect(qrX, qrY + (gridSize - 8) * cellSize, 8 * cellSize, cellSize);
      ctx.fillRect(qrX + 7 * cellSize, qrY + (gridSize - 8) * cellSize, cellSize, 8 * cellSize);
      
      ctx.fillStyle = foregroundColor;
      
      // 繪製時序模式（深色線條）
      for (let i = 8; i < gridSize - 8; i += 2) {
        ctx.fillRect(qrX + 6 * cellSize, qrY + i * cellSize, cellSize, cellSize);  // 垂直時序線
        ctx.fillRect(qrX + i * cellSize, qrY + 6 * cellSize, cellSize, cellSize);  // 水平時序線
      }
      
      // 繪製定位點
      ctx.fillRect(qrX + 6 * cellSize, qrY + 6 * cellSize, cellSize, cellSize);
      
      // 繪製數據模塊（模擬真實的QR碼數據）
      const dataPattern = this.generateDataPatternExact(gridSize);
      for (let row = 0; row < gridSize; row++) {
        for (let col = 0; col < gridSize; col++) {
          // 跳過定位點和分隔線區域
          if (this.isReservedAreaExact(row, col, gridSize)) {
            continue;
          }
          
          // 根據數據模式決定是否填充
          if (dataPattern[row] && dataPattern[row][col]) {
            ctx.fillRect(qrX + col * cellSize, qrY + row * cellSize, cellSize, cellSize);
          }
        }
      }
    }
  }

  /**
   * 改進的QR碼定位點繪製
   */
  private drawFinderPatternExact(ctx: CanvasRenderingContext2D, x: number, y: number, cellSize: number, backgroundColor: string, foregroundColor: string): void {
    // 外框 (7x7)
    ctx.fillStyle = foregroundColor;
    ctx.fillRect(x, y, cellSize * 7, cellSize * 7);
    
    // 內部白色 (5x5)
    ctx.fillStyle = backgroundColor;
    ctx.fillRect(x + cellSize, y + cellSize, cellSize * 5, cellSize * 5);
    
    // 中心黑色 (3x3)
    ctx.fillStyle = foregroundColor;
    ctx.fillRect(x + cellSize * 2, y + cellSize * 2, cellSize * 3, cellSize * 3);
  }

  /**
   * 改進的數據模式生成
   */
  private generateDataPatternExact(gridSize: number): boolean[][] {
    const pattern: boolean[][] = [];
    
    for (let row = 0; row < gridSize; row++) {
      pattern[row] = [];
      for (let col = 0; col < gridSize; col++) {
        // 使用更複雜的模式生成算法，模擬真實QR碼的數據分佈
        const hash1 = this.simpleHashExact(`${row},${col}`);
        const hash2 = this.simpleHashExact(`${col},${row}`);
        const combined = (hash1 + hash2) % 100;
        
        // 創建不同密度的區域，使其看起來更像真實QR碼
        let threshold = 45;
        if ((row + col) % 3 === 0) threshold = 35;
        if ((row + col) % 5 === 0) threshold = 55;
        if (row % 2 === col % 2) threshold = 40;
        
        pattern[row][col] = combined < threshold;
      }
    }
    
    return pattern;
  }

  /**
   * 改進的保留區域檢查
   */
  private isReservedAreaExact(row: number, col: number, gridSize: number): boolean {
    // 定位點區域 (9x9 包含分隔線)
    if ((row < 9 && col < 9) ||                              // 左上
        (row < 9 && col >= gridSize - 9) ||                 // 右上
        (row >= gridSize - 9 && col < 9)) {                 // 左下
      return true;
    }
    
    // 時序線
    if (row === 6 || col === 6) {
      return true;
    }
    
    // 暗模塊
    if (row === Math.floor(4 * gridSize / 5) && col === Math.floor(4 * gridSize / 5)) {
      return true;
    }
    
    return false;
  }

  /**
   * 改進的哈希函數
   */
  private simpleHashExact(str: string): number {
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
      const char = str.charCodeAt(i);
      hash = ((hash << 5) - hash) + char;
      hash = hash & hash;
    }
    return Math.abs(hash);
  }

  /**
   * 縮放圖片DataURL到指定尺寸
   */
  private async resizeImageDataUrl(dataUrl: string, targetWidth: number, targetHeight: number): Promise<string> {
    return new Promise((resolve, reject) => {
      const img = new Image();
      img.onload = () => {
        // 創建canvas進行縮放
        const canvas = document.createElement('canvas');
        canvas.width = targetWidth;
        canvas.height = targetHeight;
        const ctx = canvas.getContext('2d');
        
        if (!ctx) {
          reject(new Error('無法創建2D上下文'));
          return;
        }

        // 繪製縮放後的圖片
        ctx.drawImage(img, 0, 0, targetWidth, targetHeight);
        
        // 轉換為DataURL
        const resizedDataUrl = canvas.toDataURL('image/png');
        resolve(resizedDataUrl);
      };
      
      img.onerror = () => {
        reject(new Error('圖片載入失敗'));
      };
      
      img.src = dataUrl;
    });
  }


} 