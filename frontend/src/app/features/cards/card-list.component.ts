import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CardApiService, Card } from './services/card-api.service';
import { MatIconModule } from '@angular/material/icon';
import { CardPreviewComponent } from './components/card-preview.component';
import { CardItemComponent, CardItem } from '../../shared/components/cards/card-item.component';

type CardWithSide = Card & { _currentSide: 'A' | 'B' };

@Component({
  selector: 'sn-card-list',
  standalone: true,
  imports: [CommonModule, MatIconModule, CardPreviewComponent, CardItemComponent],
  templateUrl: './card-list.component.html',
  styleUrls: ['./card-list.component.scss']
})
export class CardListComponent implements OnInit {
  cards: CardWithSide[] = [];
  loading = true;

  constructor(
    private router: Router,
    private cardApiService: CardApiService
  ) {}

  ngOnInit(): void {
    this.loadCards();
  }

  private loadCards(): void {
    this.cardApiService.getCards().subscribe({
      next: (cards) => {
        // 初始化每個卡片的當前顯示面為A面
        this.cards = cards.map(card => ({...card, _currentSide: 'A' as 'A' | 'B'}));
        this.loading = false;
      },
      error: (error) => {
        console.error('載入桌牌失敗:', error);
        this.loading = false;
      }
    });
  }

  trackByCardId(index: number, card: CardWithSide): number {
    return card.id;
  }

  selectCard(card: CardWithSide): void {
    console.log('選中桌牌:', card);
    console.log('導航到路徑:', `/cards/edit/${card.id}`);
    // 導航到編輯頁面
    this.router.navigate(['/cards/edit', card.id]);
  }

  createCard(): void {
    console.log('創建新桌牌');
    // 導航到設計器頁面
    this.router.navigate(['/cards/new']);
  }

  getStatusText(status: number): string {
    return this.cardApiService.getStatusText(status);
  }

  deleteCard(card: Card, event: MouseEvent): void {
    event.stopPropagation();
    
    const confirmMessage = `確定要刪除「${card.name}」嗎？\n此操作無法復原。`;
    if (confirm(confirmMessage)) {
      console.log('開始刪除卡片:', card.id, card.name);
      
      this.cardApiService.deleteCard(card.id).subscribe({
        next: () => {
          console.log('卡片刪除成功:', card.id);
          // 從本地陣列中移除
          this.cards = this.cards.filter(c => c.id !== card.id);
          
          // 可選：顯示成功訊息
          // alert(`「${card.name}」已成功刪除`);
        },
        error: (err) => {
          console.error('刪除卡片失敗:', err);
          
          // 根據錯誤類型提供更具體的錯誤訊息
          let errorMessage = '刪除失敗，請稍後再試';
          if (err.status === 404) {
            errorMessage = '該卡片不存在或已被刪除';
          } else if (err.status === 403) {
            errorMessage = '您沒有權限刪除此卡片';
          } else if (err.status === 500) {
            errorMessage = '伺服器錯誤，請稍後再試';
          }
          
          alert(errorMessage);
        }
      });
    }
  }

  generateSidePreview(card: Card, side: 'A' | 'B'): string {
    // 優先使用對應面的縮圖
    if (side === 'A' && card.thumbnailA) {
      return card.thumbnailA;
    }
    if (side === 'B' && card.thumbnailB) {
      return card.thumbnailB;
    }
    
    // 基於卡片內容生成預覽圖
    const content = side === 'A' ? card.contentA : card.contentB;
    if (!content) {
      return 'assets/images/default-card.png';
    }
    
    // 生成基於卡片內容的預覽
    return this.generatePreviewFromContent(content);
  }

  private generatePreviewFromContent(content: unknown): string {
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

  toggleSide(card: CardWithSide, side: 'A' | 'B', event: MouseEvent): void {
    event.stopPropagation(); // 防止觸發卡片選擇
    card._currentSide = side;
  }

  getCurrentSideImage(card: CardWithSide): string {
    const currentSide = card._currentSide || 'A';
    
    // 優先使用對應面的縮圖
    if (currentSide === 'A' && card.thumbnailA) {
      return card.thumbnailA;
    }
    if (currentSide === 'B' && card.thumbnailB) {
      return card.thumbnailB;
    }
    
    // 否則生成基於內容的預覽圖
    return this.generateSidePreview(card, currentSide);
  }

  convertToCardItem(card: CardWithSide): CardItem {
    return {
      id: card.id,
      name: card.name,
      description: card.description || undefined,
      thumbnailA: card.thumbnailA,
      thumbnailB: card.thumbnailB,
      category: '桌牌',
      status: card.status || undefined,
      contentA: card.contentA,
      contentB: card.contentB,
      _currentSide: card._currentSide,
      isPublic: false,
      createdAt: card.createdAt
    };
  }

  onCardClick(cardItem: CardItem): void {
    const card = this.cards.find(c => c.id === cardItem.id);
    if (card) {
      this.selectCard(card);
    }
  }

  onCardDelete(cardItem: CardItem): void {
    const card = this.cards.find(c => c.id === cardItem.id);
    if (card) {
      this.deleteCard(card, new MouseEvent('click'));
    }
  }

  onSideToggle(event: { card: CardItem, side: 'A' | 'B' }): void {
    const card = this.cards.find(c => c.id === event.card.id);
    if (card) {
      card._currentSide = event.side;
    }
  }
}