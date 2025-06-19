import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TemplateApiService, TemplateListItem } from '../../../features/cards/services/template-api.service';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';
import { TagButtonComponent } from '../tags/tag-button.component';
import { CardItemComponent, CardItem } from '../cards/card-item.component';

export interface Template {
  id: number;
  name: string;
  description: string;
  thumbnail: string;
  category: string;
}

@Component({
  selector: 'sn-template-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    DeleteButtonComponent,
    TagButtonComponent,
    CardItemComponent
  ],
  template: `
    <div class="modal-overlay" (click)="onOverlayClick($event)"
      (keydown.enter)="onOverlayClick($event)"
      (keydown.space)="onOverlayClick($event)"
      tabindex="0" role="button">
      <div class="modal-container" 
           (click)="$event.stopPropagation()"
           (keydown.enter)="$event.stopPropagation()"
           (keydown.space)="$event.stopPropagation()"
           tabindex="0" 
           role="dialog"
           aria-label="樣板選擇對話框">
        <div class="modal-header">
          <h2 class="modal-title">選擇樣板</h2>
          <button mat-icon-button class="modal-close-btn" (click)="modalClose.emit()"
            (keydown.enter)="modalClose.emit()"
            (keydown.space)="modalClose.emit()"
            tabindex="0" role="button">
            <mat-icon>close</mat-icon>
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
                         <div class="template-item" 
                  *ngFor="let template of filteredTemplates"
                                     [class.selected]="selectedTemplate?.id === template.id"
                  (click)="selectTemplate(template)"
                  (keydown.enter)="selectTemplate(template)"
                  (keydown.space)="selectTemplate(template)"
                  tabindex="0" 
                  role="button"
                  [attr.aria-label]="'選擇模板 ' + template.name">
               <img [src]="getCurrentSideImage(template)" [alt]="template.name">
               <div class="template-info">
                 <h4>{{ template.name }}</h4>
                 <p>{{ template.description }}</p>
               </div>
               <!-- 使用共通刪除按鈕元件 -->
               <sn-delete-button
                 size="small"
                 [tooltip]="'刪除樣板：' + template.name"
                 (delete)="deleteTemplate(template)">
               </sn-delete-button>
             </div>
          </div>
        </div>

        <div class="modal-footer">
          <button
            mat-raised-button
            color="primary"
            [disabled]="!selectedTemplate"
            (click)="applyTemplate()"
            (keydown.enter)="applyTemplate()"
            (keydown.space)="applyTemplate()"
            tabindex="0" role="button">
            套用樣板
          </button>
          <button mat-button class="apple-confirm-btn" (click)="modalClose.emit()"
            (keydown.enter)="modalClose.emit()"
            (keydown.space)="modalClose.emit()"
            tabindex="0" role="button">確認</button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./template-modal.component.scss']
})
export class TemplateModalComponent implements OnInit {
  @Input() isVisible = false;
  @Output() modalClose = new EventEmitter<void>();
  @Output() templateSelected = new EventEmitter<Template>();

  selectedCategory = '全部';
  selectedTemplate: Template | null = null;
  templates: (TemplateListItem & { _currentSide?: 'A' | 'B' })[] = [];
  loading = false;

  categoryOptions = [
    { value: '全部', label: '全部', icon: 'apps' },
    { value: '名片', label: '名片', icon: 'badge' },
    { value: '會議室', label: '會議室', icon: 'meeting_room' },
    { value: '活動', label: '活動', icon: 'event' },
    { value: '公告', label: '公告', icon: 'announcement' }
  ];

  constructor(private templateApiService: TemplateApiService) {}

  ngOnInit(): void {
    this.loadTemplates();
  }

  get filteredTemplates(): (TemplateListItem & { _currentSide?: 'A' | 'B' })[] {
    if (this.selectedCategory === '全部') {
      return this.templates;
    }
    return this.templates.filter(template => template.category === this.selectedCategory);
  }

  loadTemplates(): void {
    this.loading = true;
    const category = this.selectedCategory === '全部' ? undefined : this.selectedCategory;

    this.templateApiService.getTemplates(category).subscribe({
      next: (templates) => {
        this.templates = templates;
        this.loading = false;
      },
      error: (error) => {
        console.error('載入樣板失敗:', error);
        this.loading = false;
      }
    });
  }

  selectCategory(category: string): void {
    this.selectedCategory = category;
    this.filterTemplates();
  }

  filterTemplates(): void {
    this.selectedTemplate = null;
    this.loadTemplates();
  }

  selectTemplate(cardOrTemplate: TemplateListItem | CardItem): void {
    let template: TemplateListItem;
    
    // 如果是從 CardItem 來的
    if ('_currentSide' in cardOrTemplate) {
      template = this.templates.find(t => t.id === cardOrTemplate.id)!;
    } else {
      template = cardOrTemplate as TemplateListItem;
    }
    
    // 轉換為 Template 格式
    this.selectedTemplate = {
      id: template.id,
      name: template.name,
      description: template.description || '',
      thumbnail: template.thumbnailUrl || '',
      category: template.category
    };
  }

  deleteTemplate(cardOrTemplate: TemplateListItem | CardItem): void {
    let template: TemplateListItem;
    
    // 如果是從 CardItem 來的
    if ('_currentSide' in cardOrTemplate) {
      template = this.templates.find(t => t.id === cardOrTemplate.id)!;
    } else {
      template = cardOrTemplate as TemplateListItem;
    }
    
    if (confirm(`確定要刪除樣板「${template.name}」嗎？`)) {
      this.templateApiService.deleteTemplate(template.id).subscribe({
        next: () => {
          this.templates = this.templates.filter(t => t.id !== template.id);
          if (this.selectedTemplate?.id === template.id) {
            this.selectedTemplate = null;
          }
        },
        error: (error) => {
          console.error('刪除樣板失敗:', error);
          alert('刪除樣板失敗，請稍後再試。');
        }
      });
    }
  }

  applyTemplate(): void {
    if (this.selectedTemplate) {
      this.templateSelected.emit(this.selectedTemplate);
      this.modalClose.emit();
    }
  }

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.modalClose.emit();
    }
  }

  toggleTemplateSide(template: TemplateListItem & { _currentSide?: 'A' | 'B' }, side: 'A' | 'B', event: Event): void {
    event.stopPropagation();
    template._currentSide = side;
  }

  getCurrentSideImage(template: TemplateListItem & { _currentSide?: 'A' | 'B' }): string {
    const currentSide = template._currentSide || 'A';
    if (currentSide === 'A') {
      return template.thumbnailA || template.thumbnailUrl || '/assets/templates/default.png';
    } else {
      return template.thumbnailB || template.thumbnailUrl || '/assets/templates/default.png';
    }
  }

  getOtherSidePreview(template: TemplateListItem & { _currentSide?: 'A' | 'B' }): string {
    const currentSide = template._currentSide || 'A';
    if (currentSide === 'A') {
      return template.thumbnailB || template.thumbnailUrl || '/assets/templates/default.png';
    } else {
      return template.thumbnailA || template.thumbnailUrl || '/assets/templates/default.png';
    }
  }

  convertToCardItem(template: TemplateListItem & { _currentSide?: 'A' | 'B' }): CardItem {
    return {
      id: template.id,
      name: template.name,
      description: template.description || '',
      thumbnailA: template.thumbnailA,
      thumbnailB: template.thumbnailB,
      category: template.category,
      status: 1,
      _currentSide: template._currentSide,
      isPublic: template.isPublic || false,
      createdAt: template.createdAt
    };
  }

  onSideToggle(event: { card: CardItem, side: 'A' | 'B' }): void {
    const template = this.templates.find(t => t.id === event.card.id);
    if (template) {
      template._currentSide = event.side;
    }
  }
}
