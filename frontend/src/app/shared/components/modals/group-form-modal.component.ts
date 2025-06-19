import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

export interface GroupFormData {
  name: string;
  description: string;
  color: string;
}

@Component({
  selector: 'sn-group-form-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule
  ],
  template: `
    <div class="modal-overlay" 
      (click)="onOverlayClick($event)"
      (keydown.enter)="onOverlayClick($event)"
      (keydown.space)="onOverlayClick($event)"
      tabindex="0" role="button" *ngIf="isVisible">
      <div class="modal-container" 
        (click)="$event.stopPropagation()"
        (keydown.enter)="$event.stopPropagation()"
        (keydown.space)="$event.stopPropagation()"
        tabindex="0" role="button">
        <div class="modal-header">
          <h2 class="modal-title">{{ isEdit ? '編輯群組' : '新增群組' }}</h2>
          <button 
            class="modal-close-btn"
            (click)="modalClose.emit()"
            (keydown.enter)="modalClose.emit()"
            (keydown.space)="modalClose.emit()"
            tabindex="0" role="button">
            <span class="close-icon">×</span>
          </button>
        </div>

        <div class="modal-content">
          <form class="group-form" (ngSubmit)="onSubmit()" #groupForm="ngForm">
            <div class="form-group">
              <label for="group-name-input" class="group-form-modal__label">名稱</label>
              <input
                id="group-name-input"
                type="text"
                [(ngModel)]="formData.name"
                placeholder="請輸入群組名稱"
                class="group-form-modal__input"
                required>
            </div>

            <div class="form-group">
              <label for="group-description-input" class="group-form-modal__label">描述</label>
              <textarea
                id="group-description-input"
                name="groupDescription"
                [(ngModel)]="formData.description"
                placeholder="請輸入群組描述（選填）"
                class="group-form-modal__textarea"
                rows="3">
              </textarea>
            </div>

            <div class="form-group">
              <label for="group-color-picker">群組顏色 *</label>
              <div class="color-picker" id="group-color-picker" role="radiogroup" aria-label="選擇群組顏色">
                <div 
                  *ngFor="let color of colorOptions"
                  class="color-option"
                  [class.selected]="formData.color === color.value"
                  [style.background-color]="color.value"
                  [title]="color.name"
                  (click)="selectColor(color.value)"
                  (keydown.enter)="selectColor(color.value)"
                  (keydown.space)="selectColor(color.value)"
                  tabindex="0" role="button">
                  <span *ngIf="formData.color === color.value" class="check-icon">✓</span>
                </div>
              </div>
            </div>
          </form>
        </div>

        <div class="modal-footer">
          <button 
            class="modal-btn modal-btn--secondary"
            (click)="modalClose.emit()"
            (keydown.enter)="modalClose.emit()"
            (keydown.space)="modalClose.emit()"
            tabindex="0" role="button">
            取消
          </button>
          <button 
            class="modal-btn modal-btn--primary"
            [disabled]="!isFormValid()"
            (click)="onSubmit()"
            (keydown.enter)="onSubmit()"
            (keydown.space)="onSubmit()"
            tabindex="0" role="button">
            {{ isEdit ? '儲存' : '建立群組' }}
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./group-form-modal.component.scss']
})
export class GroupFormModalComponent implements OnInit, OnChanges {
  @Input() isVisible = false;
  @Input() isEdit = false;
  @Input() initialData: Partial<GroupFormData> = {};

  @Output() modalClose = new EventEmitter<void>();
  @Output() formSubmit = new EventEmitter<GroupFormData>();

  formData: GroupFormData = {
    name: '',
    description: '',
    color: '#2196F3'
  };

  colorOptions = [
    { name: '藍色', value: '#2196F3' },
    { name: '綠色', value: '#4CAF50' },
    { name: '橙色', value: '#FF9800' },
    { name: '紅色', value: '#F44336' },
    { name: '紫色', value: '#9C27B0' },
    { name: '青色', value: '#00BCD4' },
    { name: '粉色', value: '#E91E63' },
    { name: '深綠色', value: '#388E3C' },
    { name: '深藍色', value: '#1976D2' },
    { name: '深橙色', value: '#F57C00' },
    { name: '深紫色', value: '#7B1FA2' },
    { name: '棕色', value: '#795548' }
  ];

  ngOnInit() {
    this.resetForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    // 只有在初始資料改變或者彈窗顯示/隱藏時才重置表單
    if (changes['initialData'] || (changes['isVisible'] && changes['isVisible'].currentValue)) {
      this.resetForm();
    }
  }

  resetForm() {
    this.formData = {
      name: this.initialData.name || '',
      description: this.initialData.description || '',
      color: this.initialData.color || '#2196F3'
    };
  }

  selectColor(color: string) {
    this.formData.color = color;
  }

  isFormValid(): boolean {
    return this.formData.name.trim().length > 0;
  }

  onSubmit() {
    if (this.isFormValid()) {
      this.formSubmit.emit({ ...this.formData });
    }
  }

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.modalClose.emit();
    }
  }
} 