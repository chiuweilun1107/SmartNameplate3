import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TagButtonComponent } from '../../shared/components/tags/tag-button.component';
import { CardItemComponent, CardItem } from '../../shared/components/cards/card-item.component';
import { CardApiService } from '../cards/services/card-api.service';
import { TemplateApiService } from '../cards/services/template-api.service';
import { forkJoin } from 'rxjs';

export interface GroupFormData {
  name: string;
  description: string;
  color: string;
}

interface GroupMember {
  id: number;
  name: string;
  tag: string;
}

interface Card {
  id: number;
  name: string;
  description?: string;
  thumbnail?: string;
  thumbnailA?: string;
  thumbnailB?: string;
  category?: string;
  type: 'card' | 'template'; // 用來區分是卡片還是樣板
}

  @Component({
  selector: 'sn-group-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    TagButtonComponent,
    CardItemComponent
  ],
  template: `
    <div class="group-form-container">
      <div class="group-form-header">
        <button class="back-btn" (click)="goBack()"
          (keydown.enter)="goBack()"
          (keydown.space)="goBack()"
          tabindex="0" role="button">
          <mat-icon>arrow_back</mat-icon>
          <span>返回群組列表</span>
        </button>
      </div>

      <div class="group-form-content">
        <!-- 群組名稱區塊 -->
        <div class="group-name-section">
          <label for="group-name-input" class="group-name-label">群組名稱：</label>
          <div class="group-name-input-wrapper">
            <input 
              id="group-name-input"
              type="text" 
              [(ngModel)]="formData.name"
              placeholder="請輸入群組名稱"
              class="group-name-input">
          </div>
        </div>

        <!-- 工具欄按鈕 -->
        <div class="toolbar-buttons">
          <button class="toolbar-btn toolbar-btn--select-card" (click)="openCardSelectionModal()"
            (keydown.enter)="openCardSelectionModal()"
            (keydown.space)="openCardSelectionModal()"
            tabindex="0" role="button">
            <mat-icon>credit_card</mat-icon>
            <span>選擇圖卡</span>
          </button>
          <button class="toolbar-btn toolbar-btn--add"
            tabindex="0" role="button">
            <mat-icon>add</mat-icon>
            <span>新增成員</span>
          </button>
          <button class="toolbar-btn toolbar-btn--delete"
            tabindex="0" role="button">
            <mat-icon>delete</mat-icon>
            <span>刪除成員</span>
          </button>
          <button class="toolbar-btn toolbar-btn--download"
            tabindex="0" role="button">
            <mat-icon>download</mat-icon>
            <span>下載成員</span>
          </button>
          <button class="toolbar-btn toolbar-btn--import"
            tabindex="0" role="button">
            <mat-icon>file_upload</mat-icon>
            <span>成員匯入</span>
          </button>
        </div>

        <!-- 成員列表 -->
        <div class="members-section">
          <div class="members-header">
            <div class="member-column member-column--name">成員名稱</div>
            <div class="member-column member-column--tag">標籤</div>
          </div>

          <div class="members-list">
            <div 
              *ngFor="let member of members; let i = index" 
              class="member-row">
              <div class="member-column member-column--name">
                <mat-icon class="member-edit-icon"
                  (click)="editMember(member)"
                  (keydown.enter)="editMember(member)"
                  (keydown.space)="editMember(member)"
                  tabindex="0" role="button"
                  title="編輯成員">edit</mat-icon>
                <span>{{ member.name }}</span>
              </div>
              <div class="member-column member-column--tag">
                {{ member.tag }}
              </div>
            </div>

            <!-- 空狀態 -->
            <div *ngIf="members.length === 0" class="empty-members">
              <p>尚無成員資料</p>
              <p>請先點擊選擇圖卡</p>
            </div>
          </div>
        </div>

        <!-- 底部操作按鈕 -->
        <div class="form-actions">
          <button 
            type="button"
            class="btn btn--secondary"
            (click)="goBack()">
            取消
          </button>
          <button 
            type="button"
            class="btn btn--primary"
            [disabled]="!isFormValid()"
            (click)="onSubmit()">
            {{ isEdit ? '儲存變更' : '建立群組' }}
          </button>
        </div>
      </div>
    </div>
      
    <!-- 卡片選擇彈窗 - 使用樣板modal共通組件樣式 -->
    <div 
      *ngIf="showCardSelectionModal" 
      class="modal-overlay"
      (click)="closeCardSelection()"
      (keydown.enter)="closeCardSelection()"
      (keydown.space)="closeCardSelection()"
      tabindex="0" role="button">
      <div 
        class="modal-container"
        (click)="$event.stopPropagation()"
        (keydown.enter)="$event.stopPropagation()"
        (keydown.space)="$event.stopPropagation()"
        tabindex="0" 
        role="dialog"
        aria-label="卡片選擇對話框">
        <div class="modal-header">
          <h2 class="modal-title">選擇圖卡</h2>
          <button 
            class="modal-close-btn"
            (click)="closeCardSelection()"
            (keydown.enter)="closeCardSelection()"
            (keydown.space)="closeCardSelection()"
            tabindex="0" role="button">
            <span class="close-icon">×</span>
          </button>
        </div>
        
        <div class="modal-content">
          <div class="template-categories">
            <sn-tag-button
              *ngFor="let category of categoryOptions"
              [label]="category.label"
              [icon]="category.icon"
              [isActive]="selectedCategory === category.value"
              [value]="category.value"
              (tagClick)="selectCategory($event)">
            </sn-tag-button>
          </div>

          <div class="template-grid">
            <sn-card-item
              *ngFor="let card of filteredCards"
              [card]="convertToCardItem(card)"
              [isSelected]="tempSelectedCardIds.includes(card.id)"
              [showDeleteButton]="false"

              (cardClick)="selectCard($event)"
              (sideToggle)="onSideToggle($event)">
            </sn-card-item>
          </div>
        </div>
        
        <div class="modal-footer">
          <button 
            class="modal-btn modal-btn--secondary"
            (click)="closeCardSelection()"
            (keydown.enter)="closeCardSelection()"
            (keydown.space)="closeCardSelection()"
            tabindex="0" role="button">
            取消
          </button>
          <button 
            class="modal-btn modal-btn--primary"
            (click)="confirmCardSelection()"
            (keydown.enter)="confirmCardSelection()"
            (keydown.space)="confirmCardSelection()"
            tabindex="0" role="button">
            確認選擇
          </button>
        </div>
      </div>
    </div>
    `,
    styleUrls: ['./group-form.component.scss']
  })
  export class GroupFormComponent implements OnInit {
    isEdit = false;
    groupId: number | null = null;
    showCardSelectionModal = false;
    tempSelectedCardIds: number[] = [];
    
    formData: GroupFormData = {
      name: '',
      description: '',
      color: '#000000'
    };

    members: GroupMember[] = [];
    availableCards: Card[] = [];
    selectedCategory = '全部';
    loading = false;

    categoryOptions = [
      { value: '全部', label: '全部', icon: 'apps' },
      { value: '名片', label: '名片', icon: 'badge' },
      { value: '會議室', label: '會議室', icon: 'meeting_room' },
      { value: '活動', label: '活動', icon: 'event' },
      { value: '公告', label: '公告', icon: 'announcement' }
    ];

    colorOptions = ['#000000', '#ff0000', '#00ff00', '#0000ff', '#ffff00', '#ff00ff', '#00ffff'];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private cardApiService: CardApiService,
    private templateApiService: TemplateApiService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEdit = true;
        this.groupId = Number(params['id']);
        this.loadGroupData();
      } else {
        // 新增模式，清空成員列表
        this.members = [];
      }
    });
    
    // 載入卡片和樣板資料
    this.loadCardsAndTemplates();
  }

  get filteredCards(): Card[] {
    if (this.selectedCategory === '全部') {
      return this.availableCards;
    }
    return this.availableCards.filter(card => card.category === this.selectedCategory);
  }

  loadGroupData() {
    if (this.groupId) {
      // 模擬載入資料
      this.formData = {
        name: '1111_群組',
        description: '這是一個測試群組',
        color: '#000000'
      };
    }
  }

  // 載入卡片和樣板資料
  loadCardsAndTemplates() {
    this.loading = true;
    
    forkJoin({
      cards: this.cardApiService.getCards(),
      templates: this.templateApiService.getTemplates()
    }).subscribe({
      next: (result) => {
        // 轉換卡片資料
        const cards: Card[] = result.cards.map(card => ({
           id: card.id,
           name: card.name,
           description: card.description || '',
           thumbnailA: card.thumbnailA,
           thumbnailB: card.thumbnailB,
           category: '卡片', // 給卡片一個預設分類
           type: 'card'
         }));

        // 轉換樣板資料
        const templates: Card[] = result.templates.map(template => ({
           id: template.id + 10000, // 避免ID衝突
           name: template.name,
           description: template.description || '',
           thumbnailA: template.thumbnailA,
           thumbnailB: template.thumbnailB,
           category: template.category,
           type: 'template'
         }));

        this.availableCards = [...cards, ...templates];
        this.loading = false;
      },
      error: () => {
        console.error('載入卡片和樣板失敗');
        this.loading = false;
      }
    });
  }

  // 分類選擇
  selectCategory(category: string): void {
    this.selectedCategory = category;
  }

  // 卡片選擇相關方法
  openCardSelectionModal() {
    this.showCardSelectionModal = true;
    this.tempSelectedCardIds = []; // 初始化選擇
    this.loadCardsAndTemplates(); // 重新載入資料
  }

  closeCardSelection() {
    this.showCardSelectionModal = false;
    this.tempSelectedCardIds = [];
  }

  // 選擇卡片（單選）
  selectCard(cardItem: CardItem) {
    const isSelected = this.tempSelectedCardIds.includes(cardItem.id);
    if (isSelected) {
      // 如果已選中，取消選擇
      this.tempSelectedCardIds = [];
    } else {
      // 單選：清空之前的選擇，只選擇當前項目
      this.tempSelectedCardIds = [cardItem.id];
    }
  }

  // 編輯成員
  editMember(member: GroupMember): void {
    console.log('編輯成員:', member);
    // TODO: 實作編輯成員邏輯
  }

  // A/B面切換
  onSideToggle(event: { card: CardItem, side: 'A' | 'B' }) {
    const card = this.availableCards.find(c => c.id === event.card.id);
    if (card) {
      // 🛡️ 安全的屬性設置
      Object.defineProperty(card, '_currentSide', {
        value: event.side,
        writable: true,
        enumerable: false,
        configurable: true
      });
    }
  }

     // 轉換為CardItem格式
   convertToCardItem(card: Card): CardItem {
     return {
       id: card.id,
       name: card.name,
       description: card.description || '',
       thumbnailA: card.thumbnailA,
       thumbnailB: card.thumbnailB,
       category: card.category || '',
       status: 1, // 預設為已發布狀態
       _currentSide: Object.prototype.hasOwnProperty.call(card, '_currentSide') 
         ? (card as Card & { _currentSide: 'A' | 'B' })._currentSide 
         : 'A',
       isPublic: true,
       createdAt: new Date().toISOString()
     };
   }

  confirmCardSelection() {
    // 根據選擇的卡片更新成員列表（單選模式）
    const selectedCards = this.availableCards.filter(card => 
      this.tempSelectedCardIds.includes(card.id)
    );
    
    // 單選模式：只會有一張卡片被選中
    if (selectedCards.length > 0) {
      const selectedCard = selectedCards[0];
      this.members = [{
        id: selectedCard.id,
        name: selectedCard.name,
        tag: `標籤_${selectedCard.id}`
      }];
    } else {
      this.members = [];
    }
    
    this.closeCardSelection();
  }

  isFormValid(): boolean {
    return this.formData.name.trim().length > 0;
  }

  onSubmit() {
    if (this.isFormValid()) {
      if (this.isEdit) {
        console.log('更新群組:', this.formData);
      } else {
        console.log('建立群組:', this.formData);
      }
      this.goBack();
    }
  }

  goBack() {
    this.router.navigate(['/groups']);
  }

  selectColor(color: string) {
    this.formData.color = color;
  }
} 