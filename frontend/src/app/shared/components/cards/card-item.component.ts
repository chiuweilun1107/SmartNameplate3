import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';

export interface CardItem {
  id: number;
  name: string;
  description?: string;
  thumbnailA?: string;
  thumbnailB?: string;
  category: string;
  status?: number;
  contentA?: string | object;
  contentB?: string | object;
  _currentSide?: 'A' | 'B';
  isPublic?: boolean;
  createdAt?: string;
}

@Component({
  selector: 'sn-card-item',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
    DeleteButtonComponent
  ],
  template: `
    <div class="card-item" [class.selected]="isSelected" (click)="onCardClick()"
      (keydown.enter)="onCardClick()"
      (keydown.space)="onCardClick()"
      tabindex="0" role="button">
      <sn-delete-button
        *ngIf="showDeleteButton"
        size="small"
        tooltip="刪除"
        class="card-delete"
        (delete)="onDelete()">
      </sn-delete-button>
      <div class="card-preview">
        <div class="side-toggle-buttons" *ngIf="showToggleButtons">
          <button 
            class="side-toggle" 
            [class.active]="card._currentSide !== 'B'"
            (click)="onToggleSide('A', $event)">
            A
          </button>
          <button 
            class="side-toggle" 
            [class.active]="card._currentSide === 'B'"
            (click)="onToggleSide('B', $event)">
            B
          </button>
        </div>
        <img [src]="getCurrentSideImage()" [alt]="card.name + ' - ' + (card._currentSide || 'A') + '面'">
      </div>
      <div class="card-info">
        <div class="card-info-content">
          <div class="card-text-info">
            <h3>{{ card.name }}</h3>
            <p>{{ card.description || '無描述' }}</p>
            <span 
              *ngIf="card.status !== undefined" 
              class="card-status" 
              [class]="'card-status--' + card.status">
              {{ getStatusText(card.status) }}
            </span>
            <span 
              *ngIf="card.category" 
              class="card-category">
              {{ card.category }}
            </span>
          </div>
          <div class="card-ab-preview" *ngIf="showOtherSidePreview">
            <div class="side-preview" [title]="getOtherSideTitle()">
              <img [src]="getOtherSidePreview()" [alt]="getOtherSideTitle()">
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./card-item.component.scss']
})
export class CardItemComponent {
  @Input() card!: CardItem;
  @Input() isSelected = false;
  @Input() showDeleteButton = true;
  @Input() showToggleButtons = true;
  @Input() showOtherSidePreview = true;

  @Output() cardClick = new EventEmitter<CardItem>();
  @Output() cardDelete = new EventEmitter<CardItem>();
  @Output() sideToggle = new EventEmitter<{ card: CardItem, side: 'A' | 'B' }>();

  onCardClick(): void {
    this.cardClick.emit(this.card);
  }

  onDelete(): void {
    this.cardDelete.emit(this.card);
  }

  onToggleSide(side: 'A' | 'B', event: MouseEvent): void {
    event.stopPropagation();
    this.card._currentSide = side;
    this.sideToggle.emit({ card: this.card, side });
  }

  getCurrentSideImage(): string {
    const currentSide = this.card._currentSide || 'A';
    
    // 優先使用對應面的縮圖
    if (currentSide === 'A' && this.card.thumbnailA) {
      return this.card.thumbnailA;
    }
    if (currentSide === 'B' && this.card.thumbnailB) {
      return this.card.thumbnailB;
    }
    
    // 否則生成基於內容的預覽圖
    return this.generateSidePreview(this.card, currentSide);
  }

  getOtherSidePreview(): string {
    const otherSide = this.card._currentSide === 'B' ? 'A' : 'B';
    
    // 優先使用對應面的縮圖
    if (otherSide === 'A' && this.card.thumbnailA) {
      return this.card.thumbnailA;
    }
    if (otherSide === 'B' && this.card.thumbnailB) {
      return this.card.thumbnailB;
    }
    
    // 否則生成基於內容的預覽圖
    return this.generateSidePreview(this.card, otherSide);
  }

  getOtherSideTitle(): string {
    const otherSide = this.card._currentSide === 'B' ? 'A' : 'B';
    return `${otherSide}面預覽`;
  }

  private generateSidePreview(item: CardItem, side: 'A' | 'B'): string {
    // 優先使用對應面的縮圖
    if (side === 'A' && item.thumbnailA) {
      return item.thumbnailA;
    }
    if (side === 'B' && item.thumbnailB) {
      return item.thumbnailB;
    }
    
    // 基於卡片內容生成預覽圖
    const content = side === 'A' ? item.contentA : item.contentB;
    if (!content) {
      return 'assets/images/default-card.png';
    }
    
    // 生成基於卡片內容的預覽
    return this.generatePreviewFromContent(content);
  }

  private generatePreviewFromContent(content: string | object): string {
    // 這裡可以根據內容類型生成不同的預覽圖
    // 暫時返回默認圖片，後續可以實作內容解析
    try {
      const parsedContent = typeof content === 'string' ? JSON.parse(content) : content;
      if (parsedContent && typeof parsedContent === 'object' && 'background' in parsedContent) {
        // 使用正確的background字段
        return this.generateColorPreview((parsedContent as { background: string }).background);
      }
    } catch (e) {
      console.log('解析內容失敗:', e);
    }
    
    return 'assets/images/default-card.png';
  }

  private generateColorPreview(color: string): string {
    // 生成簡單的色塊預覽 (SVG data URL)
    const svg = `
      <svg xmlns="http://www.w3.org/2000/svg" width="100" height="60" viewBox="0 0 100 60">
        <rect width="100" height="60" fill="${color}" stroke="#ddd" stroke-width="1"/>
        <text x="50" y="35" text-anchor="middle" font-family="Arial" font-size="10" fill="${color === '#ffffff' ? '#666' : '#fff'}">預覽</text>
      </svg>
    `;
    return `data:image/svg+xml;base64,${btoa(unescape(encodeURIComponent(svg)))}`;
  }

  getStatusText(status: number): string {
    switch (status) {
      case 0: return '草稿';
      case 1: return '已發布';
      case 2: return '已停用';
      default: return '未知';
    }
  }
} 