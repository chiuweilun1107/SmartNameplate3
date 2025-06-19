import { Injectable } from '@angular/core';
import { CanvasData, CanvasElement as ImportedCanvasElement } from '../../features/cards/models/card-design.models';
import * as htmlToImage from 'html-to-image';

export type RenderCallback = (canvas: HTMLCanvasElement) => void;
export type ErrorCallback = (error: Error) => void;

@Injectable({
  providedIn: 'root'
})
export class ThumbnailGeneratorService {
  // 移除空構造函數

  /**
   * 根據 CanvasData 生成縮圖 - 使用 html-to-image 實現精確轉換
   * @param canvasData 畫布資料
   * @param width 縮圖寬度，預設800
   * @param height 縮圖高度，預設480 
   * @returns Promise<string> base64格式的縮圖
   */
  async generateThumbnail(canvasData: CanvasData, width = 3200, height = 1920): Promise<string> {
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

      // 🎯 計算高解析度的 pixelRatio
      const targetPixelRatio = Math.max(width / actualWidth, height / actualHeight);
      const finalPixelRatio = Math.min(targetPixelRatio, 4); // 限制最大為4倍，避免記憶體問題
      
      console.log(`🔍 計算 pixelRatio: 目標=${targetPixelRatio.toFixed(2)}, 最終=${finalPixelRatio.toFixed(2)}`);

      // 使用 html-to-image 轉換整個畫布，使用高解析度設定
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
        pixelRatio: finalPixelRatio, // 🎯 使用計算出的高解析度比例
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
              'rotation-handle',
              // 🚫 過濾掉空白提示訊息元素
              'live-preview__empty',
              'live-preview__empty-icon',
              'live-preview__empty-text',
              'live-preview__empty-hint',
              'card-designer__empty-canvas'
            ];
            return !excludeClasses.some(cls => node.classList.contains(cls));
          }
          return true;
        }
      });

      // 🎯 檢查是否需要進一步縮放
      const generatedWidth = actualWidth * finalPixelRatio;
      const generatedHeight = actualHeight * finalPixelRatio;
      
      console.log(`📐 生成的圖片尺寸: ${generatedWidth}x${generatedHeight}`);
      
      // 如果生成的尺寸與目標尺寸差異很大，才進行縮放
      const widthDiff = Math.abs(generatedWidth - width) / width;
      const heightDiff = Math.abs(generatedHeight - height) / height;
      
      if (widthDiff > 0.1 || heightDiff > 0.1) {
        console.log('🔄 需要微調縮圖到精確目標尺寸');
        return this.resizeImageDataUrl(dataUrl, width, height);
      }

      console.log('🎉 html-to-image 高解析度轉換成功，無需額外縮放');
      return dataUrl;

    } catch (error) {
      console.error('html-to-image 轉換失敗，使用備用方案:', error);
      return this.generateThumbnailFallback(canvasData, width, height);
    }
  }

  /**
   * 精確轉換單個元素
   */
  async generateElementThumbnail(elementId: string, width = 200, height = 200): Promise<string | null> {
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
              'rotation-handle',
              // 🚫 過濾掉空白提示訊息元素
              'live-preview__empty',
              'live-preview__empty-icon',
              'live-preview__empty-text',
              'live-preview__empty-hint',
              'card-designer__empty-canvas'
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
      // 修復Format String漏洞：分離訊息和數據
      console.error('元素轉換失敗:', { elementId, error });
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
              'rotation-handle',
              // 🚫 過濾掉空白提示訊息元素
              'live-preview__empty',
              'live-preview__empty-icon',
              'live-preview__empty-text',
              'live-preview__empty-hint',
              'card-designer__empty-canvas'
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
      // 修復Format String漏洞：分離訊息和數據
      console.error('區域轉換失敗:', { selector, error });
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

        console.log('🎨 開始繪製縮圖，尺寸:', width, 'x', height, '縮放比例:', scaleX.toFixed(2), 'x', scaleY.toFixed(2));

        // 設置背景 - 等待背景完成後再繪製元素
        this.drawBackground(ctx, canvasData.background || '#ffffff', width, height).then(() => {
          console.log('🖼️ 背景繪製完成，開始繪製元素，元素數量:', canvasData.elements.length);
          
          // 按照正確的z-index順序繪製所有元素
          const sortedElements = [...canvasData.elements].sort((a, b) => (a.zIndex || 0) - (b.zIndex || 0));
          
          // 並行繪製所有元素，確保完全復制畫布樣式
          const drawPromises = sortedElements.map(element => this.drawElementExact(ctx, element, scaleX, scaleY));

          Promise.all(drawPromises).then(() => {
            console.log('✅ 所有元素繪製完成，生成縮圖');
            // 生成 base64 圖片
            const dataURL = canvas.toDataURL('image/png', 1.0); // 使用PNG格式和最高品質
            console.log('📸 縮圖生成完成，大小:', Math.round(dataURL.length / 1024), 'KB');
            resolve(dataURL);
          }).catch((error) => {
            console.warn('⚠️ 繪製元素時發生錯誤:', error);
            // 即使部分元素繪製失敗，也返回目前的結果
            const dataURL = canvas.toDataURL('image/png', 1.0);
            console.log('📸 部分失敗但仍生成縮圖，大小:', Math.round(dataURL.length / 1024), 'KB');
            resolve(dataURL);
          });
        }).catch((error) => {
          console.error('❌ 背景繪製失敗:', error);
          // 背景繪製失敗時，使用白色背景繼續
          ctx.fillStyle = '#ffffff';
          ctx.fillRect(0, 0, width, height);
          
          const sortedElements = [...canvasData.elements].sort((a, b) => (a.zIndex || 0) - (b.zIndex || 0));
          const drawPromises = sortedElements.map(element => this.drawElementExact(ctx, element, scaleX, scaleY));

          Promise.all(drawPromises).then(() => {
            const dataURL = canvas.toDataURL('image/png', 1.0);
            resolve(dataURL);
          }).catch(() => {
            const dataURL = canvas.toDataURL('image/png', 1.0);
            resolve(dataURL);
          });
        });

      } catch {
        // 🛡️ 安全日誌：避免洩露敏感錯誤資訊
        console.warn('生成縮圖時發生錯誤 - 詳細資訊已記錄');
        reject(new Error('縮圖生成失敗'));
      }
    });
  }

  /**
   * 精確繪製單個元素，完全復制畫布樣式
   */
  private async drawElementExact(ctx: CanvasRenderingContext2D, element: ImportedCanvasElement, scaleX: number, scaleY: number): Promise<void> {
    if (!element || typeof element !== 'object') return;
    const el = element as { position?: { x?: number; y?: number } };
    const position = (el as ImportedCanvasElement & { position?: { x?: number; y?: number } }).position;
    const size = (el as ImportedCanvasElement & { size?: { width?: number; height?: number } }).size;
    const x = (position?.x || 0) * scaleX;
    const y = (position?.y || 0) * scaleY;
    const width = (size?.width || 0) * scaleX;
    const height = (size?.height || 0) * scaleY;

    // 首先嘗試從DOM獲取精確的樣式
    const domElement = await this.getElementFromDOMExact((element as ImportedCanvasElement & { id?: string }).id || '');
    if (domElement) {
      ctx.drawImage(domElement, x, y, width, height);
      return;
    }

    // 如果DOM獲取失敗，使用改進的手動繪製，確保樣式完全一致
    switch ((el as ImportedCanvasElement).type) {
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
      
    } catch {
      // 修復Format String漏洞：分離訊息和數據
      console.warn('從DOM獲取元素失敗');
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
      } catch {
        console.warn('渲染DOM元素到Canvas失敗');
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
    if (typeof ((ctx as unknown) as { roundRect?: (x: number, y: number, width: number, height: number, radius: number) => void }).roundRect === 'function') {
      ((ctx as unknown) as { roundRect?: (x: number, y: number, width: number, height: number, radius: number) => void }).roundRect?.(x, y, width, height, radius);
    } else {
      this.drawRoundedRect(ctx, x, y, width, height, radius);
    }
    ctx.stroke();
  }

  /**
   * 繪製背景 - 修正版本，確保圖片背景正確載入
   */
  private async drawBackground(ctx: CanvasRenderingContext2D, background: string, width: number, height: number): Promise<void> {
    if (background.startsWith('url(')) {
      // 處理圖片背景
      const imageUrl = background.match(/url\(([^)]+)\)/)?.[1]?.replace(/['"]/g, '');
      if (imageUrl) {
        try {
          await new Promise<void>((resolve, reject) => {
            const img = new Image();
            img.crossOrigin = 'anonymous';
            img.onload = () => {
              try {
                ctx.drawImage(img, 0, 0, width, height);
                console.log('✅ 背景圖片載入成功:', imageUrl);
                resolve();
              } catch (error) {
                console.warn('❌ 背景圖片繪製失敗:', error);
                // 如果繪製失敗，使用白色背景
                ctx.fillStyle = '#ffffff';
                ctx.fillRect(0, 0, width, height);
                resolve();
              }
            };
            img.onerror = () => {
              console.warn('❌ 背景圖片載入失敗:', imageUrl);
              // 如果圖片載入失敗，使用白色背景
              ctx.fillStyle = '#ffffff';
              ctx.fillRect(0, 0, width, height);
              resolve();
            };
            img.src = imageUrl;
          });
        } catch (error) {
          console.warn('❌ 背景圖片處理異常:', error);
          // 異常情況下使用白色背景
          ctx.fillStyle = '#ffffff';
          ctx.fillRect(0, 0, width, height);
        }
      } else {
        ctx.fillStyle = '#ffffff';
        ctx.fillRect(0, 0, width, height);
      }
    } else {
      // 純色背景
      ctx.fillStyle = background || '#ffffff';
      ctx.fillRect(0, 0, width, height);
      console.log('✅ 純色背景設定完成:', background);
    }
  }

  /**
   * 繪製單個元素
   */
  private async drawElement(ctx: CanvasRenderingContext2D, element: ImportedCanvasElement, scaleX: number, scaleY: number): Promise<void> {
    if (!element || typeof element !== 'object') return;
    const el = element as { position?: { x?: number; y?: number } };
    const position = (el as ImportedCanvasElement & { position?: { x?: number; y?: number } }).position;
    const size = (el as ImportedCanvasElement & { size?: { width?: number; height?: number } }).size;
    const x = (position?.x || 0) * scaleX;
    const y = (position?.y || 0) * scaleY;
    const width = (size?.width || 0) * scaleX;
    const height = (size?.height || 0) * scaleY;

    switch ((el as ImportedCanvasElement).type) {
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
  private drawText(ctx: CanvasRenderingContext2D, element: ImportedCanvasElement, x: number, y: number, width: number, height: number, scale: number): void {
    if (!element || typeof element !== 'object') return;
    const textEl = element as { 
      text?: string; 
      style?: { 
        fontSize?: number; 
        fontFamily?: string; 
        color?: string; 
        textAlign?: string; 
        fontWeight?: string; 
        fontStyle?: string; 
        textDecoration?: string; 
        lineHeight?: number 
      } 
    };

    const text = textEl.text || '';
    const style = textEl.style || {};
    const fontSize = (style.fontSize || 16) * scale;
    const fontFamily = style.fontFamily || 'Arial';
    const color = style.color || '#000000';
    const textAlign = style.textAlign || 'left';
    const fontWeight = style.fontWeight || 'normal';
    const fontStyle = style.fontStyle || 'normal';
    const lineHeight = style.lineHeight || 1.2;

    ctx.save();
    ctx.font = `${fontStyle} ${fontWeight} ${fontSize}px ${fontFamily}`;
    ctx.fillStyle = color;
    ctx.textAlign = textAlign as CanvasTextAlign;
    ctx.textBaseline = 'top';

    // 處理多行文字
    const lines = text.split('\n');
    const lineHeightPx = fontSize * lineHeight;
    
    lines.forEach((line, index) => {
      const lineY = y + (index * lineHeightPx);
      let lineX = x;
      
      if (textAlign === 'center') {
        lineX = x + width / 2;
      } else if (textAlign === 'right') {
        lineX = x + width;
      }
      
      ctx.fillText(line, lineX, lineY);
    });

    ctx.restore();
  }

  /**
   * 繪製圖片元素 - 修正版本，確保圖片正確載入
   */
  private async drawImage(ctx: CanvasRenderingContext2D, element: ImportedCanvasElement, x: number, y: number, width: number, height: number): Promise<void> {
    const imageEl = element as { src?: string; alt?: string; style?: { opacity?: number; borderRadius?: number } };
    if (!imageEl.src) {
      console.warn('⚠️ 圖片元素沒有src屬性');
      return;
    }

    try {
      await new Promise<void>((resolve, reject) => {
        const img = new Image();
        img.crossOrigin = 'anonymous';
        
        img.onload = () => {
          try {
            ctx.save();
            
            // 設置透明度
            if (imageEl.style?.opacity !== undefined) {
              ctx.globalAlpha = imageEl.style.opacity;
            }

            // 處理圓角
            if (imageEl.style?.borderRadius && imageEl.style.borderRadius > 0) {
              this.drawRoundedRect(ctx, x, y, width, height, imageEl.style.borderRadius);
              ctx.clip();
            }

            ctx.drawImage(img, x, y, width, height);
            ctx.restore();
            console.log('✅ 圖片繪製成功:', imageEl.src || 'unknown');
            resolve();
          } catch (error) {
            console.warn('❌ 圖片繪製失敗:', error);
            ctx.restore();
            resolve();
          }
        };

        img.onerror = () => {
          console.warn('❌ 圖片載入失敗:', imageEl.src || 'unknown');
          resolve();
        };
        
        img.src = imageEl.src!; // 我們已經在函數開頭檢查過 src 不為空
      });
    } catch (error) {
      console.warn('❌ 繪製圖片異常:', error);
    }
  }

  /**
   * 繪製形狀元素
   */
  private drawShape(ctx: CanvasRenderingContext2D, element: ImportedCanvasElement, x: number, y: number, width: number, height: number): void {
    const shapeEl = element as { 
      shapeType?: string; 
      style?: { 
        backgroundColor?: string; 
        borderColor?: string; 
        borderWidth?: number; 
        borderRadius?: number 
      } 
    };
    const style = shapeEl.style || {};

    ctx.save();

    // 設置填充色
    if (style.backgroundColor) {
      ctx.fillStyle = style.backgroundColor;
    }
    
    // 設置邊框
    if (style.borderColor && style.borderWidth) {
      ctx.strokeStyle = style.borderColor;
      ctx.lineWidth = style.borderWidth;
    }
    
    // 根據形狀類型繪製
    switch (shapeEl.shapeType) {
      case 'circle':
        this.drawCircle(ctx, x + width/2, y + height/2, Math.min(width, height)/2);
        break;
      case 'triangle':
        this.drawTriangle(ctx, x, y, width, height);
        break;
      case 'star':
        this.drawStar(ctx, x + width/2, y + height/2, width/2, width/4);
        break;
      case 'hexagon':
        this.drawHexagon(ctx, x + width/2, y + height/2, width/2);
        break;
      case 'line':
        this.drawLine(ctx, x, y + height/2, x + width, y + height/2);
        break;
      default: // rectangle
        if (style.borderRadius && style.borderRadius > 0) {
          this.drawRoundedRect(ctx, x, y, width, height, style.borderRadius);
      } else {
          ctx.fillRect(x, y, width, height);
          if (style.borderWidth) {
            ctx.strokeRect(x, y, width, height);
      }
    }
        break;
    }
    
    ctx.restore();
  }

  /**
   * 繪製QR碼元素
   */
  private async drawQRCode(ctx: CanvasRenderingContext2D, element: ImportedCanvasElement, x: number, y: number, width: number, height: number): Promise<void> {
    const qrEl = element as { 
      data?: string; 
      style?: { 
        backgroundColor?: string; 
        foregroundColor?: string; 
        borderColor?: string; 
        borderWidth?: number; 
        borderRadius?: number 
      } 
    };
    
    const data = qrEl.data || 'https://example.com';
    const style = qrEl.style || {};
      const backgroundColor = style.backgroundColor || '#ffffff';
      const foregroundColor = style.foregroundColor || '#000000';
    const margin = 4;

    try {
      // 先嘗試從DOM獲取QR碼
      const qrCanvas = await this.getQRCodeFromDOM(element.id || '');
        if (qrCanvas) {
        ctx.drawImage(qrCanvas, x, y, width, height);
        return;
      }
      
      // 降級方案：繪製QR碼佔位符
      this.drawQRCodeFallback(ctx, x, y, width, height, backgroundColor, foregroundColor, margin);
    } catch {
      console.warn('QR碼繪製失敗，使用降級方案');
      this.drawQRCodeFallback(ctx, x, y, width, height, backgroundColor, foregroundColor, margin);
    }
  }

  /**
   * 從DOM獲取真正的QR碼
   */
  private async getQRCodeFromDOM(elementId: string): Promise<HTMLCanvasElement | null> {
    try {
      if (!elementId) return null;
      
      const element = document.getElementById(elementId);
      if (!element) return null;
      
      const canvas = element.querySelector('canvas') as HTMLCanvasElement;
      if (!canvas) return null;
      
      return canvas;
    } catch {
      console.warn('無法從DOM獲取QR碼');
      return null;
    }
  }

  /**
   * QR碼備用繪製方案
   */
  private drawQRCodeFallback(ctx: CanvasRenderingContext2D, x: number, y: number, width: number, height: number, backgroundColor: string, foregroundColor: string, margin: number): void {
    // 繪製背景
    ctx.fillStyle = backgroundColor;
    ctx.fillRect(x, y, width, height);

    // 繪製QR碼圖案（簡化版本）
      ctx.fillStyle = foregroundColor;
    const contentSize = Math.min(width, height) - (margin * 2);
    const contentX = x + (width - contentSize) / 2;
    const contentY = y + (height - contentSize) / 2;
    const cellSize = contentSize / 21; // 標準QR碼 21x21 格子
    
    // 繪製定位標記（三個角落）
    this.drawFinderPattern(ctx, contentX, contentY, cellSize);
    this.drawFinderPattern(ctx, contentX + cellSize * 14, contentY, cellSize);
    this.drawFinderPattern(ctx, contentX, contentY + cellSize * 14, cellSize);
    
    // 繪製一些隨機的數據點
    for (let i = 0; i < 50; i++) {
      const randomX = contentX + Math.floor(Math.random() * 21) * cellSize;
      const randomY = contentY + Math.floor(Math.random() * 21) * cellSize;
      ctx.fillRect(randomX, randomY, cellSize, cellSize);
    }
  }

  /**
   * 輔助方法
   */
  private drawCircle(ctx: CanvasRenderingContext2D, centerX: number, centerY: number, radius: number): void {
    ctx.beginPath();
    ctx.arc(centerX, centerY, radius, 0, 2 * Math.PI);
    ctx.fill();
    if (ctx.lineWidth > 0) {
      ctx.stroke();
    }
  }

  private drawTriangle(ctx: CanvasRenderingContext2D, x: number, y: number, width: number, height: number): void {
    ctx.beginPath();
    ctx.moveTo(x + width / 2, y);
    ctx.lineTo(x + width, y + height);
    ctx.lineTo(x, y + height);
    ctx.closePath();
    ctx.fill();
    if (ctx.lineWidth > 0) {
      ctx.stroke();
    }
  }

  private drawLine(ctx: CanvasRenderingContext2D, x1: number, y1: number, x2: number, y2: number): void {
    ctx.beginPath();
    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);
    ctx.stroke();
  }

  /**
   * 舊瀏覽器相容的圓角矩形繪製方法
   */
  private drawRoundedRect(ctx: CanvasRenderingContext2D, x: number, y: number, width: number, height: number, radius: number): void {
    ctx.beginPath();
    ctx.moveTo(x + radius, y);
    ctx.lineTo(x + width - radius, y);
    ctx.quadraticCurveTo(x + width, y, x + width, y + radius);
    ctx.lineTo(x + width, y + height - radius);
    ctx.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
    ctx.lineTo(x + radius, y + height);
    ctx.quadraticCurveTo(x, y + height, x, y + height - radius);
    ctx.lineTo(x, y + radius);
    ctx.quadraticCurveTo(x, y, x + radius, y);
    ctx.fill();
    if (ctx.lineWidth > 0) {
      ctx.stroke();
        }
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
    ctx.fill();
    if (ctx.lineWidth > 0) {
      ctx.stroke();
    }
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
    ctx.fill();
    if (ctx.lineWidth > 0) {
      ctx.stroke();
    }
  }

  /**
   * 批量生成縮圖（A面和B面） - 使用卡面切換確保DOM正確顯示
   */
  async generateBothThumbnails(designA: CanvasData, designB: CanvasData, designerService?: unknown): Promise<{thumbnailA: string, thumbnailB: string}> {
    console.log('🔄 開始批量生成 A 面和 B 面縮圖（卡面切換模式）');
    
    try {
      // 記錄當前卡面
      const originalSide = (designerService as { getCurrentSide?: () => string })?.getCurrentSide?.() || 'A';
      console.log(`📍 當前卡面: ${originalSide}`);

      // 生成A面縮圖
      console.log('📸 切換到A面生成縮圖...');
      if (designerService) {
        (designerService as { switchSide?: (side: string) => void })?.switchSide?.('A');
        // 等待DOM更新
        await this.waitForDOMUpdate();
      }
      
      const thumbnailA = await this.generateThumbnail(designA, 3200, 1920);
      console.log('✅ A面縮圖生成完成 (超高解析度)');

      // 生成B面縮圖
      console.log('📸 切換到B面生成縮圖...');
      if (designerService) {
        (designerService as { switchSide?: (side: string) => void })?.switchSide?.('B');
        // 等待DOM更新
        await this.waitForDOMUpdate();
      }
      
      const thumbnailB = await this.generateThumbnail(designB, 3200, 1920);
      console.log('✅ B面縮圖生成完成');

      // 恢復原始卡面
      if (designerService && originalSide !== (designerService as { getCurrentSide?: () => string })?.getCurrentSide?.()) {
        console.log(`🔄 恢復到原始卡面: ${originalSide}`);
        (designerService as { switchSide?: (side: string) => void })?.switchSide?.(originalSide);
        await this.waitForDOMUpdate();
      }

      console.log('✅ 批量生成完成 - 卡面切換方式');
      return { thumbnailA, thumbnailB };
      
    } catch {
      console.warn('卡面切換生成失敗，使用備用方案');
      
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
  private drawTextExact(ctx: CanvasRenderingContext2D, element: ImportedCanvasElement, x: number, y: number, width: number, height: number, scale: number): void {
    // 使用原有的drawText方法，它已經很準確了
    this.drawText(ctx, element, x, y, width, height, scale);
  }

  /**
   * 精確繪製圖片元素，完全復制畫布樣式
   */
  private async drawImageExact(ctx: CanvasRenderingContext2D, element: ImportedCanvasElement, x: number, y: number, width: number, height: number): Promise<void> {
    // 使用原有的drawImage方法，它已經很準確了
    await this.drawImage(ctx, element, x, y, width, height);
  }

  /**
   * 精確繪製形狀元素，完全復制畫布樣式
   */
  private drawShapeExact(ctx: CanvasRenderingContext2D, element: ImportedCanvasElement, x: number, y: number, width: number, height: number): void {
    this.drawShape(ctx, element, x, y, width, height);
  }

  /**
   * 精確繪製QR碼元素，完全復制畫布樣式
   */
  private async drawQRCodeExact(ctx: CanvasRenderingContext2D, element: ImportedCanvasElement, x: number, y: number, width: number, height: number): Promise<void> {
    await this.drawQRCode(ctx, element, x, y, width, height);
  }

  /**
   * 高品質縮放圖片DataURL到指定尺寸
   */
  private async resizeImageDataUrl(dataUrl: string, targetWidth: number, targetHeight: number): Promise<string> {
    return new Promise((resolve, reject) => {
      const img = new Image();
      img.onload = () => {
        // 創建高解析度canvas進行縮放
        const canvas = document.createElement('canvas');
        canvas.width = targetWidth;
        canvas.height = targetHeight;
        const ctx = canvas.getContext('2d');
        
        if (!ctx) {
          reject(new Error('無法創建2D上下文'));
          return;
        }

        // 🎯 使用高品質縮放設定
        ctx.imageSmoothingEnabled = true;
        ctx.imageSmoothingQuality = 'high';
        
        // 🎯 如果是放大操作，使用分階段縮放以提高品質
        const scaleX = targetWidth / img.width;
        const scaleY = targetHeight / img.height;
        const maxScale = Math.max(scaleX, scaleY);
        
        if (maxScale > 2) {
          // 分階段放大，提高品質
          console.log('🔍 使用分階段放大提高品質');
          let currentWidth = img.width;
          let currentHeight = img.height;
          let tempCanvas = document.createElement('canvas');
          let tempCtx = tempCanvas.getContext('2d');
          
          if (!tempCtx) {
            reject(new Error('無法創建臨時2D上下文'));
            return;
          }
          
          // 先繪製原圖到臨時canvas
          tempCanvas.width = currentWidth;
          tempCanvas.height = currentHeight;
          tempCtx.drawImage(img, 0, 0);
          
          // 分階段放大
          while (currentWidth < targetWidth || currentHeight < targetHeight) {
            const nextWidth = Math.min(currentWidth * 2, targetWidth);
            const nextHeight = Math.min(currentHeight * 2, targetHeight);
            
            const nextCanvas = document.createElement('canvas');
            const nextCtx = nextCanvas.getContext('2d');
            if (!nextCtx) break;
            
            nextCanvas.width = nextWidth;
            nextCanvas.height = nextHeight;
            nextCtx.imageSmoothingEnabled = true;
            nextCtx.imageSmoothingQuality = 'high';
            nextCtx.drawImage(tempCanvas, 0, 0, nextWidth, nextHeight);
            
            tempCanvas = nextCanvas;
            tempCtx = nextCtx;
            currentWidth = nextWidth;
            currentHeight = nextHeight;
          }
          
          // 最終繪製到目標canvas
          ctx.drawImage(tempCanvas, 0, 0, targetWidth, targetHeight);
        } else {
          // 直接縮放
          ctx.drawImage(img, 0, 0, targetWidth, targetHeight);
        }
        
        // 轉換為高品質PNG
        const resizedDataUrl = canvas.toDataURL('image/png', 1.0);
        console.log(`✅ 圖片縮放完成: ${img.width}x${img.height} → ${targetWidth}x${targetHeight}`);
        resolve(resizedDataUrl);
      };
      
      img.onerror = () => {
        reject(new Error('圖片載入失敗'));
      };
      
      img.src = dataUrl;
    });
  }

  private async loadImageToCanvas(url: string, width: number, height: number): Promise<HTMLCanvasElement | null> {
    return new Promise((resolve) => {
      const img = new Image();
      img.crossOrigin = 'anonymous';
      
      img.onload = () => {
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');
        
        if (!ctx) {
          console.warn('無法獲取 canvas context');
          resolve(null);
          return;
        }
        
        canvas.width = width;
        canvas.height = height;
        
        // 繪製圖片到 canvas
        ctx.drawImage(img, 0, 0, width, height);
        resolve(canvas);
      };
      
      img.onerror = () => {
        console.warn(`圖片載入失敗: ${url}`);
        resolve(null);
      };
      
      img.src = url;
    });
  }

  private async loadFontToCanvas(font: string, text: string, fontSize: number): Promise<HTMLCanvasElement> {
    return new Promise((resolve) => {
      const canvas = document.createElement('canvas');
      const ctx = canvas.getContext('2d')!;
      
      // 設定 canvas 大小
      const padding = 20;
      ctx.font = `${fontSize}px ${font}`;
      const textMetrics = ctx.measureText(text);
      
      canvas.width = textMetrics.width + padding * 2;
      canvas.height = fontSize + padding * 2;
      
      // 重新設定字體（因為 canvas 大小改變會重置樣式）
      ctx.font = `${fontSize}px ${font}`;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      
      // 繪製文字
      ctx.fillText(text, canvas.width / 2, canvas.height / 2);
      
      resolve(canvas);
    });
  }

  private applyCanvasFilters(ctx: CanvasRenderingContext2D, filters: Record<string, number>): void {
    if (!filters || typeof filters !== 'object') return;
    
    const filterString = Object.entries(filters)
      .map(([key, value]) => `${key}(${value})`)
      .join(' ');
    
    if (filterString) {
      ctx.filter = filterString;
    }
  }

  private setupCanvasContext(canvas: HTMLCanvasElement, width: number, height: number): CanvasRenderingContext2D | null {
    const ctx = canvas.getContext('2d');
    if (!ctx) return null;
    
    canvas.width = width;
    canvas.height = height;
    ctx.clearRect(0, 0, width, height);
    
    return ctx;
  }

  private validateElementType(element: ImportedCanvasElement, expectedType: string): boolean {
    return element && typeof element === 'object' && 'type' in element && element.type === expectedType;
  }

  private getElementPosition(element: ImportedCanvasElement): { x: number; y: number; width: number; height: number } {
    const defaultPosition = { x: 0, y: 0, width: 100, height: 100 };
    
    if (!element || typeof element !== 'object') return defaultPosition;
    
    const positionElement = element as { position?: { x?: number; y?: number }; size?: { width?: number; height?: number } };
    
    return {
      x: positionElement.position?.x || 0,
      y: positionElement.position?.y || 0,
      width: positionElement.size?.width || 100,
      height: positionElement.size?.height || 100
    };
  }

  private calculateThumbnailDimensions(originalWidth: number, originalHeight: number, maxSize: number): { width: number; height: number } {
    const aspectRatio = originalWidth / originalHeight;
    
    if (originalWidth > originalHeight) {
      return {
        width: maxSize,
        height: Math.round(maxSize / aspectRatio)
      };
    } else {
      return {
        width: Math.round(maxSize * aspectRatio),
        height: maxSize
      };
         }
   }

   /**
    * 繪製QR碼定位點
    */
   private drawFinderPattern(ctx: CanvasRenderingContext2D, x: number, y: number, cellSize: number): void {
     // 外框 (7x7)
     ctx.fillStyle = '#000000';
     ctx.fillRect(x, y, cellSize * 7, cellSize * 7);
     
     // 內部白色 (5x5)
     ctx.fillStyle = '#ffffff';
     ctx.fillRect(x + cellSize, y + cellSize, cellSize * 5, cellSize * 5);
     
     // 中心黑色 (3x3)
     ctx.fillStyle = '#000000';
     ctx.fillRect(x + cellSize * 2, y + cellSize * 2, cellSize * 3, cellSize * 3);
   }

} 