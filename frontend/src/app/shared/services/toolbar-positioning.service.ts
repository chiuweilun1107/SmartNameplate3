import { Injectable } from '@angular/core';

export interface ToolbarPosition {
  x: number;
  y: number;
}

export interface DropdownPosition {
  x: number;
  y: number;
}

@Injectable({
  providedIn: 'root'
})
export class ToolbarPositioningService {

  /**
   * 計算主工具列的位置
   * @param targetElement 目標元素
   * @param toolbarHeight 工具列高度（默認60px）
   * @returns 工具列位置座標
   */
  calculateToolbarPosition(targetElement: HTMLElement, toolbarHeight = 60): ToolbarPosition {
    const rect = targetElement.getBoundingClientRect();
    
    // 水平置中於元素中心
    const x = rect.left + rect.width / 2;

    // 計算上方位置
    let y = rect.top - toolbarHeight;

    // 如果工具列會超出頁面上緣，則改為顯示在元素下方
    if (y < 10) {
      y = rect.bottom + 10; // 顯示在元素下方
    }

    return { x, y };
  }

  /**
   * 計算下拉選單的位置
   * @param basePosition 基礎工具列位置
   * @param buttonType 按鈕類型
   * @param toolbarHeight 工具列高度（默認42px）
   * @param gap 間距（默認2px）
   * @returns 下拉選單位置座標
   */
  calculateDropdownPosition(
    basePosition: ToolbarPosition, 
    buttonType: 'align' | 'color' | 'size' | 'tag' | 'custom',
    toolbarHeight = 42,
    gap = 2,
    customOffset?: number
  ): DropdownPosition {
    const baseX = basePosition.x;
    const baseY = basePosition.y;
    
    // 不同按鈕在工具列中的相對位置
    let buttonOffset = 0;
    
    if (customOffset !== undefined) {
      buttonOffset = customOffset;
    } else {
      switch (buttonType) {
        case 'align':
          buttonOffset = -85;
          break;
        case 'color':
          buttonOffset = -45;
          break;
        case 'size':
          buttonOffset = -5;
          break;
        case 'tag':
          buttonOffset = 35;
          break;
        default:
          buttonOffset = 0;
      }
    }
    
    return {
      x: baseX + buttonOffset,
      y: baseY + toolbarHeight + gap
    };
  }

  /**
   * 確保位置在視窗範圍內
   * @param position 原始位置
   * @param elementWidth 元素寬度
   * @param elementHeight 元素高度
   * @returns 調整後的位置
   */
  constrainToViewport(
    position: ToolbarPosition | DropdownPosition, 
    elementWidth: number, 
    elementHeight: number
  ): ToolbarPosition | DropdownPosition {
    const viewport = {
      width: window.innerWidth,
      height: window.innerHeight
    };

    let { x, y } = position;

    // 確保不超出右邊界
    if (x + elementWidth > viewport.width) {
      x = viewport.width - elementWidth - 10;
    }

    // 確保不超出左邊界
    if (x < 10) {
      x = 10;
    }

    // 確保不超出下邊界
    if (y + elementHeight > viewport.height) {
      y = viewport.height - elementHeight - 10;
    }

    // 確保不超出上邊界
    if (y < 10) {
      y = 10;
    }

    return { x, y };
  }

  /**
   * 計算元素相對於另一個元素的位置
   * @param referenceElement 參考元素
   * @param targetWidth 目標元素寬度
   * @param targetHeight 目標元素高度
   * @param placement 放置位置
   * @returns 位置座標
   */
  calculateRelativePosition(
    referenceElement: HTMLElement,
    targetWidth: number,
    targetHeight: number,
    placement: 'top' | 'bottom' | 'left' | 'right' | 'top-start' | 'top-end' | 'bottom-start' | 'bottom-end' = 'bottom'
  ): ToolbarPosition {
    const rect = referenceElement.getBoundingClientRect();
    let x = 0;
    let y = 0;

    switch (placement) {
      case 'top':
        x = rect.left + rect.width / 2 - targetWidth / 2;
        y = rect.top - targetHeight - 8;
        break;
      case 'bottom':
        x = rect.left + rect.width / 2 - targetWidth / 2;
        y = rect.bottom + 8;
        break;
      case 'left':
        x = rect.left - targetWidth - 8;
        y = rect.top + rect.height / 2 - targetHeight / 2;
        break;
      case 'right':
        x = rect.right + 8;
        y = rect.top + rect.height / 2 - targetHeight / 2;
        break;
      case 'top-start':
        x = rect.left;
        y = rect.top - targetHeight - 8;
        break;
      case 'top-end':
        x = rect.right - targetWidth;
        y = rect.top - targetHeight - 8;
        break;
      case 'bottom-start':
        x = rect.left;
        y = rect.bottom + 8;
        break;
      case 'bottom-end':
        x = rect.right - targetWidth;
        y = rect.bottom + 8;
        break;
    }

    return this.constrainToViewport({ x, y }, targetWidth, targetHeight);
  }
} 