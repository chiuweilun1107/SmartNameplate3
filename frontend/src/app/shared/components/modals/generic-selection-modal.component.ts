import { Component, Input, Output, EventEmitter, TemplateRef, ContentChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface SelectionItem {
  id: number | string;
  name: string;
  description?: string;
  [key: string]: any; // 允許額外屬性
}

@Component({
  selector: 'sn-generic-selection-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="modal-overlay" (click)="onOverlayClick($event)" *ngIf="isVisible">
      <div class="modal-container" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2 class="modal-title">{{ title }}</h2>
          <button 
            class="modal-close-btn"
            (click)="close.emit()">
            <span class="close-icon">×</span>
          </button>
        </div>

        <div class="modal-content">
          <div class="selection-grid">
            <ng-content></ng-content>
          </div>
        </div>

        <div class="modal-footer">
          <button 
            class="modal-btn modal-btn--secondary"
            (click)="close.emit()">
            {{ cancelText }}
          </button>
          <button 
            *ngIf="showConfirmButton"
            class="modal-btn modal-btn--primary"
            [disabled]="!hasSelection"
            (click)="confirmSelection()">
            {{ confirmText }}
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./generic-selection-modal.component.scss']
})
export class GenericSelectionModalComponent {
  @Input() isVisible = false;
  @Input() title = '選擇項目';
  @Input() items: SelectionItem[] = [];
  @Input() selectedItems: (string | number)[] = [];
  @Input() multiSelect = false;
  @Input() showConfirmButton = true;
  @Input() confirmText = '確認選擇';
  @Input() cancelText = '取消';

  @Output() close = new EventEmitter<void>();
  @Output() selectionChange = new EventEmitter<SelectionItem[]>();
  @Output() itemSelect = new EventEmitter<SelectionItem>();

  get hasSelection(): boolean {
    return this.selectedItems.length > 0;
  }

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.close.emit();
    }
  }

  confirmSelection(): void {
    const selected = this.items.filter(item => 
      this.selectedItems.includes(item.id)
    );
    this.selectionChange.emit(selected);
    this.close.emit();
  }

  selectItem(item: SelectionItem): void {
    if (this.multiSelect) {
      // 多選模式
      const index = this.selectedItems.indexOf(item.id);
      if (index > -1) {
        this.selectedItems.splice(index, 1);
      } else {
        this.selectedItems.push(item.id);
      }
    } else {
      // 單選模式
      this.selectedItems = [item.id];
      if (!this.showConfirmButton) {
        // 如果不顯示確認按鈕，直接選擇
        this.itemSelect.emit(item);
        this.close.emit();
      }
    }
  }

  isSelected(item: SelectionItem): boolean {
    return this.selectedItems.includes(item.id);
  }
} 