import { Component, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';

export interface TemplateCategorySelection {
  category: string;
  name: string;
}

@Component({
  selector: 'sn-template-category-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    FormsModule
  ],
  template: `
    <div class="modal-overlay" 
         *ngIf="isVisible" 
         (click)="onOverlayClick($event)"
         (keydown.enter)="onOverlayClick($event)"
         (keydown.space)="onOverlayClick($event)"
         tabindex="0" 
         role="button">
      <div class="modal-container" 
           (click)="$event.stopPropagation()"
           (keydown.enter)="$event.stopPropagation()"
           (keydown.space)="$event.stopPropagation()"
           tabindex="0" 
           role="dialog"
           aria-label="樣板分類對話框">
        <div class="modal-header">
          <h2 class="modal-title">儲存樣板</h2>
          <button mat-icon-button class="modal-close-btn" (click)="modalClose.emit()" (keydown.enter)="modalClose.emit()" (keydown.space)="modalClose.emit()" tabindex="0" role="button">
            <mat-icon>close</mat-icon>
          </button>
        </div>

        <div class="modal-content">
          <div class="form-group">
            <label for="templateName">樣板名稱</label>
            <input
              id="templateName"
              type="text"
              [(ngModel)]="templateName"
              placeholder="請輸入樣板名稱"
              class="form-input">
          </div>

          <div class="form-group">
            <label for="category-select" class="form-label">選擇分類</label>
                         <select 
               id="category-select"
               [(ngModel)]="selectedCategory"
               class="form-select">
               <option value="">請選擇分類</option>
               <option *ngFor="let category of categories" [value]="category.value">{{ category.label }}</option>
             </select>
          </div>
        </div>

        <div class="modal-footer">
          <button mat-button (click)="modalClose.emit()" (keydown.enter)="modalClose.emit()" (keydown.space)="modalClose.emit()" tabindex="0" role="button">取消</button>
          <button
            mat-raised-button
            color="primary"
            [disabled]="!templateName.trim()"
            (click)="saveTemplate()"
            (keydown.enter)="saveTemplate()"
            (keydown.space)="saveTemplate()"
            tabindex="0" role="button">
            儲存樣板
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./template-category-modal.component.scss']
})
export class TemplateCategoryModalComponent implements OnChanges {
  @Input() isVisible = false;
  @Input() currentCardName = '';
  @Output() modalClose = new EventEmitter<void>();
  @Output() templateSaved = new EventEmitter<TemplateCategorySelection>();

  templateName = '';
  selectedCategory = '';

  ngOnChanges(): void {
    // 當模態框顯示且有桌牌名稱時，自動帶入
    if (this.isVisible && this.currentCardName && !this.templateName) {
      this.templateName = this.currentCardName;
    }
  }

  categories = [
    { value: '名片', label: '名片', icon: 'badge' },
    { value: '會議室', label: '會議室', icon: 'meeting_room' },
    { value: '活動', label: '活動', icon: 'event' },
    { value: '公告', label: '公告', icon: 'announcement' }
  ];

  selectCategory(category: string): void {
    this.selectedCategory = category;
  }

  saveTemplate(): void {
    if (this.templateName.trim()) {
      this.templateSaved.emit({
        category: this.selectedCategory,
        name: this.templateName.trim()
      });
      this.modalClose.emit();
      this.resetForm();
    }
  }

  private resetForm(): void {
    this.templateName = '';
    this.selectedCategory = '';
  }

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.modalClose.emit();
    }
  }
}
