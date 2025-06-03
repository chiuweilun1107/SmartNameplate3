import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import {
  CanvasElement,
  CanvasData,
  CardDesign,
  Position,
  Size,
  TextElement,
  ImageElement,
  ShapeElement,
  QRCodeElement
} from '../models/card-design.models';
import { CardApiService, CreateCardDto } from './card-api.service';

@Injectable({
  providedIn: 'root'
})
export class CardDesignerService {
  private currentDesignSubject = new BehaviorSubject<CardDesign | null>(null);
  private selectedElementSubject = new BehaviorSubject<string | null>(null);
  private currentSideSubject = new BehaviorSubject<'A' | 'B'>('A');
  private isSyncABSubject = new BehaviorSubject<boolean>(false);
  private isSyncAB = false;

  currentDesign$ = this.currentDesignSubject.asObservable();
  selectedElement$ = this.selectedElementSubject.asObservable();
  currentSide$ = this.currentSideSubject.asObservable();
  isSyncAB$ = this.isSyncABSubject.asObservable();

  constructor(private cardApiService: CardApiService) {}

  // 設計管理
  createNewDesign(name: string = '新圖卡'): CardDesign {
    // 確保每次都創建全新的設計，避免重複使用舊資料
    const timestamp = Date.now();
    const randomId = Math.random().toString(36).substr(2, 9);
    
    const newDesign = {
      id: `new_${timestamp}_${randomId}`, // 更複雜的ID避免衝突
      name,
      A: {
        elements: [], // 確保元素陣列是全新的
        background: '#ffffff',
        width: 800,
        height: 480
      },
      B: {
        elements: [], // 確保元素陣列是全新的
        background: '#ffffff',
        width: 800,
        height: 480
      },
      createdAt: new Date(),
      updatedAt: new Date(),
      createdBy: 'current_user',
      isTemplate: false
    };
    
    // 設置新設計並清空所有狀態
    this.currentDesignSubject.next(newDesign);
    this.clearAllState();
    
    return newDesign;
  }

  /**
   * 清空所有狀態，用於創建新設計時
   */
  clearAllState(): void {
    this.selectedElementSubject.next(null);
    this.currentSideSubject.next('A');
    this.isSyncABSubject.next(false);
    this.isSyncAB = false;
  }

  loadDesign(id: string): void {
    this.cardApiService.getCard(parseInt(id)).subscribe({
      next: (card) => {
        // 設定同步狀態
        this.isSyncAB = card.isSameBothSides;
        this.isSyncABSubject.next(card.isSameBothSides);
        
        const design: CardDesign = {
          id: card.id.toString(),
          name: card.name,
          description: card.description,
          A: card.contentA || {
            elements: [],
            background: '#ffffff',
            width: 800,
            height: 480
          },
          B: card.contentB || {
            elements: [],
            background: '#ffffff',
            width: 800,
            height: 480
          },
          createdAt: new Date(card.createdAt),
          updatedAt: new Date(card.updatedAt),
          createdBy: 'user',
          isTemplate: false
        };
        this.currentDesignSubject.next(design);
      },
      error: (error) => {
        console.error('載入設計失敗:', error);
        // 如果載入失敗，創建一個新的設計
        const mockDesign = this.createNewDesign(`會議室 ${id}`);
        this.currentDesignSubject.next(mockDesign);
      }
    });
  }

  saveDesign(thumbnailA?: string, thumbnailB?: string): Observable<any> {
    const design = this.currentDesignSubject.value;
    if (design) {
      design.updatedAt = new Date();

      const cardDto: CreateCardDto = {
        name: design.name,
        description: design.description,
        status: 1, // Active
        contentA: design.A,
        contentB: design.B,
        thumbnailA: thumbnailA || '',
        thumbnailB: thumbnailB || '',
        isSameBothSides: this.isSyncAB
      };

      console.log('儲存設計:', cardDto);

      // 如果有 ID 且不是臨時 ID，則更新現有桌牌
      if (design.id && !design.id.startsWith('el_') && !isNaN(Number(design.id))) {
        return this.cardApiService.updateCard(parseInt(design.id), cardDto);
      } else {
        // 否則創建新桌牌
        return this.cardApiService.createCard(cardDto);
      }
    }

    throw new Error('沒有設計可以儲存');
  }

  // 側面管理
  switchSide(side: 'A' | 'B'): void {
    this.currentSideSubject.next(side);
    this.clearSelection();
  }

  getCurrentSide(): 'A' | 'B' {
    return this.currentSideSubject.value;
  }

  getCurrentCanvasData(): CanvasData | null {
    const design = this.currentDesignSubject.value;
    const side = this.getCurrentSide();
    return design ? design[side] : null;
  }

  // 元素管理
  addElement(element: CanvasElement): void {
    const design = this.currentDesignSubject.value;
    const side = this.getCurrentSide();

    if (design) {
      design[side].elements.push(element);
      // 雙向同步
      if (this.isSyncAB) {
        if (side === 'A') {
          design.B = JSON.parse(JSON.stringify(design.A));
        } else {
          design.A = JSON.parse(JSON.stringify(design.B));
        }
      }
      this.currentDesignSubject.next(design);
      this.selectElement(element.id);
    }
  }

  updateElement(id: string, updates: Partial<CanvasElement>): void {
    const design = this.currentDesignSubject.value;
    const side = this.getCurrentSide();

    if (design) {
      const elementIndex = design[side].elements.findIndex(el => el.id === id);
      if (elementIndex !== -1) {
        const currentElement = design[side].elements[elementIndex];

        design[side].elements[elementIndex] = {
          ...currentElement,
          ...updates
        } as CanvasElement;

        // 讓 elements 陣列 reference 變動，強制 Angular 偵測
        design[side].elements = [...design[side].elements];

        // 雙向同步
        if (this.isSyncAB) {
          if (side === 'A') {
            design.B = JSON.parse(JSON.stringify(design.A));
          } else {
            design.A = JSON.parse(JSON.stringify(design.B));
          }
        }
        this.currentDesignSubject.next(design);
      }
    }
  }

  deleteElement(id: string): void {
    const design = this.currentDesignSubject.value;
    const side = this.getCurrentSide();

    if (design) {
      design[side].elements = design[side].elements.filter(el => el.id !== id);
      // 雙向同步
      if (this.isSyncAB) {
        if (side === 'A') {
          design.B = JSON.parse(JSON.stringify(design.A));
        } else {
          design.A = JSON.parse(JSON.stringify(design.B));
        }
      }
      this.currentDesignSubject.next(design);
      this.clearSelection();
    }
  }

  moveElement(id: string, position: Position): void {
    this.updateElement(id, { position });
  }

  resizeElement(id: string, size: Size): void {
    this.updateElement(id, { size });
  }

  // 選擇管理
  selectElement(id: string): void {
    this.selectedElementSubject.next(id);
  }

  clearSelection(): void {
    this.selectedElementSubject.next(null);
  }

  getSelectedElement(): CanvasElement | null {
    const selectedId = this.selectedElementSubject.value;
    const canvasData = this.getCurrentCanvasData();

    if (selectedId && canvasData) {
      return canvasData.elements.find(el => el.id === selectedId) || null;
    }

    return null;
  }

  // 元素創建工廠方法
  createTextElement(content: string = '文字', position?: Position): TextElement {
    return {
      type: 'text',
      id: this.generateId(),
      content,
      position: position || { x: 100, y: 100 },
      size: { width: 200, height: 50 },
      style: {
        fontSize: 16,
        fontFamily: 'Noto Sans TC, PingFang TC, Microsoft JhengHei, Microsoft YaHei, sans-serif',
        fontWeight: 'normal',
        color: '#000000',
        textAlign: 'left',
        backgroundColor: 'transparent',
        borderRadius: 0,
        padding: 10
      },
      zIndex: this.getNextZIndex()
    };
  }

  createImageElement(src: string, alt: string = '圖片', position?: Position): ImageElement {
    return {
      type: 'image',
      id: this.generateId(),
      src,
      alt,
      position: position || { x: 100, y: 100 },
      size: { width: 200, height: 150 },
      style: {
        borderRadius: 0,
        opacity: 1
      },
      zIndex: this.getNextZIndex()
    };
  }

  createShapeElement(shapeType: 'rectangle' | 'circle' | 'line' | 'triangle' | 'star' | 'polygon' = 'rectangle', position?: Position): ShapeElement {
    const isCircle = shapeType === 'circle';
    const isLine = shapeType === 'line';
    return {
      type: 'shape',
      id: this.generateId(),
      shapeType,
      position: position || { x: 100, y: 100 },
      size: isCircle
        ? { width: 120, height: 120 }
        : isLine
        ? { width: 200, height: 6 }
        : shapeType === 'polygon'
        ? { width: 120, height: 120 }
        : { width: 150, height: 100 },
      style: {
        backgroundColor: isLine ? '#1565c0' : '#e3f2fd',
        borderColor: '#2196f3',
        borderWidth: isLine ? 0 : 2,
        borderRadius: isCircle ? 50 : 0
      },
      zIndex: this.getNextZIndex()
    };
  }

  createQRCodeElement(data: string = '@https://example.com', position?: Position): QRCodeElement {
    return {
      type: 'qrcode',
      id: this.generateId(),
      data,
      position: position || { x: 100, y: 100 },
      size: { width: 100, height: 100 },
      style: {
        backgroundColor: '#ffffff',
        foregroundColor: '#000000'
      },
      zIndex: this.getNextZIndex()
    };
  }

  // 工具方法
  private generateId(): string {
    return 'el_' + Math.random().toString(36).substr(2, 9);
  }

  private getNextZIndex(): number {
    const canvasData = this.getCurrentCanvasData();
    if (!canvasData || canvasData.elements.length === 0) {
      return 1;
    }

    const maxZ = Math.max(...canvasData.elements.map(el => el.zIndex));
    return maxZ + 1;
  }

  // 背景管理
  setBackground(color: string): void {
    const design = this.currentDesignSubject.value;
    const side = this.getCurrentSide();

    if (design) {
      design[side].background = color;
      this.currentDesignSubject.next(design);
    }
  }

  setBackgroundBothSides(color: string): void {
    const design = this.currentDesignSubject.value;

    if (design) {
      design.A.background = color;
      design.B.background = color;
      this.currentDesignSubject.next(design);
    }
  }

  // 複製B面功能
  copyAToB(): void {
    const design = this.currentDesignSubject.value;
    if (design) {
      design.B = JSON.parse(JSON.stringify(design.A));
      // 重新生成ID避免衝突
      design.B.elements = design.B.elements.map(el => ({
        ...el,
        id: this.generateId()
      }));
      this.currentDesignSubject.next(design);
    }
  }

  // 清除當前面板內容（包含背景）
  clearCurrentSide(): void {
    const design = this.currentDesignSubject.value;
    const side = this.getCurrentSide();

    if (design) {
      design[side].elements = [];
      design[side].background = '#ffffff';
      this.currentDesignSubject.next(design);
    }
  }

  // 清除AB兩面的內容（包含背景）
  clearBothSides(): void {
    const design = this.currentDesignSubject.value;

    if (design) {
      design.A.elements = [];
      design.A.background = '#ffffff';
      design.B.elements = [];
      design.B.background = '#ffffff';
      this.currentDesignSubject.next(design);
    }
  }

  setDesign(design: CardDesign): void {
    this.currentDesignSubject.next(design);
  }

  setSyncAB(sync: boolean) {
    this.isSyncAB = sync;
    this.isSyncABSubject.next(sync);
    if (sync) {
      this.copyAToB();
    }
  }

  getSyncAB(): boolean {
    return this.isSyncAB;
  }

  // 新增：元素複製功能
  duplicateElement(id: string): CanvasElement | null {
    const design = this.currentDesignSubject.value;
    const side = this.getCurrentSide();

    if (design) {
      const element = design[side].elements.find(el => el.id === id);
      if (element) {
        // 創建複製的元素
        const duplicatedElement: CanvasElement = {
          ...element,
          id: this.generateId(),
          position: {
            x: element.position.x + 20, // 稍微偏移位置
            y: element.position.y + 20
          },
          zIndex: this.getNextZIndex()
        };

        // 添加到元素列表
        design[side].elements.push(duplicatedElement);

        // 雙向同步
        if (this.isSyncAB) {
          if (side === 'A') {
            design.B = JSON.parse(JSON.stringify(design.A));
          } else {
            design.A = JSON.parse(JSON.stringify(design.B));
          }
        }

        this.currentDesignSubject.next(design);
        return duplicatedElement;
      }
    }
    return null;
  }

  // 新增：元素圖層管理
  moveElementUp(id: string): void {
    const design = this.currentDesignSubject.value;
    const side = this.getCurrentSide();

    if (design) {
      const elements = design[side].elements;
      const elementIndex = elements.findIndex(el => el.id === id);
      
      if (elementIndex !== -1) {
        const element = elements[elementIndex];
        const currentZ = element.zIndex;
        
        // 找到下一個更高的z-index
        const higherElements = elements.filter(el => el.zIndex > currentZ);
        if (higherElements.length > 0) {
          const nextZ = Math.min(...higherElements.map(el => el.zIndex));
          
          // 找到該z-index的元素並交換
          const targetElement = elements.find(el => el.zIndex === nextZ);
          if (targetElement) {
            element.zIndex = nextZ;
            targetElement.zIndex = currentZ;
          }
        }

        // 雙向同步
        if (this.isSyncAB) {
          if (side === 'A') {
            design.B = JSON.parse(JSON.stringify(design.A));
          } else {
            design.A = JSON.parse(JSON.stringify(design.B));
          }
        }

        this.currentDesignSubject.next(design);
      }
    }
  }

  moveElementDown(id: string): void {
    const design = this.currentDesignSubject.value;
    const side = this.getCurrentSide();

    if (design) {
      const elements = design[side].elements;
      const elementIndex = elements.findIndex(el => el.id === id);
      
      if (elementIndex !== -1) {
        const element = elements[elementIndex];
        const currentZ = element.zIndex;
        
        // 找到下一個更低的z-index
        const lowerElements = elements.filter(el => el.zIndex < currentZ);
        if (lowerElements.length > 0) {
          const nextZ = Math.max(...lowerElements.map(el => el.zIndex));
          
          // 找到該z-index的元素並交換
          const targetElement = elements.find(el => el.zIndex === nextZ);
          if (targetElement) {
            element.zIndex = nextZ;
            targetElement.zIndex = currentZ;
          }
        }

        // 雙向同步
        if (this.isSyncAB) {
          if (side === 'A') {
            design.B = JSON.parse(JSON.stringify(design.A));
          } else {
            design.A = JSON.parse(JSON.stringify(design.B));
          }
        }

        this.currentDesignSubject.next(design);
      }
    }
  }

  moveElementToTop(id: string): void {
    const design = this.currentDesignSubject.value;
    const side = this.getCurrentSide();

    if (design) {
      const elements = design[side].elements;
      const elementIndex = elements.findIndex(el => el.id === id);
      
      if (elementIndex !== -1) {
        const element = elements[elementIndex];
        element.zIndex = this.getNextZIndex();

        // 雙向同步
        if (this.isSyncAB) {
          if (side === 'A') {
            design.B = JSON.parse(JSON.stringify(design.A));
          } else {
            design.A = JSON.parse(JSON.stringify(design.B));
          }
        }

        this.currentDesignSubject.next(design);
      }
    }
  }

  moveElementToBottom(id: string): void {
    const design = this.currentDesignSubject.value;
    const side = this.getCurrentSide();

    if (design) {
      const elements = design[side].elements;
      const elementIndex = elements.findIndex(el => el.id === id);
      
      if (elementIndex !== -1) {
        const element = elements[elementIndex];
        // 將所有其他元素的z-index加1，將此元素設為1
        elements.forEach(el => {
          if (el.id !== id) {
            el.zIndex += 1;
          }
        });
        element.zIndex = 1;

        // 雙向同步
        if (this.isSyncAB) {
          if (side === 'A') {
            design.B = JSON.parse(JSON.stringify(design.A));
          } else {
            design.A = JSON.parse(JSON.stringify(design.B));
          }
        }

        this.currentDesignSubject.next(design);
      }
    }
  }

  // 新增：更新形狀類型
  updateShapeType(id: string, shapeType: 'rectangle' | 'circle' | 'line' | 'triangle' | 'star' | 'polygon'): void {
    const updates: any = { shapeType };
    
    // 根據形狀類型設定適當的樣式
    if (shapeType === 'circle') {
      updates.style = {
        ...updates.style,
        borderRadius: '50%'
      };
    } else if (shapeType === 'line') {
      updates.size = { width: 200, height: 2 };
    }
    
    this.updateElement(id, updates);
  }

  // 新增：更新QR碼數據
  updateQRCodeData(id: string, data: string): void {
    this.updateElement(id, { data });
  }

  // 新增：更新QR碼樣式
  updateQRCodeStyle(id: string, style: any): void {
    this.updateElement(id, { style });
  }

  // 新增：替換圖片
  replaceImage(id: string, newSrc: string, newAlt?: string): void {
    const updates: any = { src: newSrc };
    if (newAlt) {
      updates.alt = newAlt;
    }
    this.updateElement(id, updates);
  }

  createEmptyCanvasData(): CanvasData {
    return {
      elements: [],
      background: '#ffffff',
      width: 800,
      height: 480
    };
  }
}
