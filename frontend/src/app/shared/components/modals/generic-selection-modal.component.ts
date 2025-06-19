import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface SelectionItem {
  id: string | number;
  name: string;
  description?: string;
  preview?: string;
  data?: unknown;
}

@Component({
  selector: 'sn-generic-selection-modal',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
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
           aria-label="通用選擇對話框">
        <div class="modal-header">
          <h2 class="modal-title">{{ title }}</h2>
          <button mat-icon-button class="modal-close-btn" (click)="modalClose.emit()"
            (keydown.enter)="modalClose.emit()"
            (keydown.space)="modalClose.emit()"
            tabindex="0" role="button">
            <mat-icon>close</mat-icon>
          </button>
        </div>

        <div class="modal-content">
          <div class="selection-grid">
            <div
              *ngFor="let item of items"
              class="selection-item"
              [class.selected]="selectedItem === item"
              (click)="selectItem(item)"
              (keydown.enter)="selectItem(item)"
              (keydown.space)="selectItem(item)"
              tabindex="0"
              role="button"
              [attr.aria-label]="'選擇項目 ' + item.name">
              <div class="item-preview" *ngIf="item.preview">
                <img [src]="item.preview" [alt]="item.name">
              </div>
              <div class="item-info">
                <h3 class="item-name">{{ item.name }}</h3>
                <p class="item-description" *ngIf="item.description">{{ item.description }}</p>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button
            mat-raised-button
            color="primary"
            [disabled]="!selectedItem"
            (click)="confirmSelection()">
            確認選擇
          </button>
          <button mat-button (click)="modalClose.emit()">取消</button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./generic-selection-modal.component.scss']
})
export class GenericSelectionModalComponent {
  @Input() title = '選擇項目';
  @Input() items: SelectionItem[] = [];
  @Input() isVisible = false;

  @Output() modalClose = new EventEmitter<void>();
  @Output() itemSelected = new EventEmitter<SelectionItem>();

  selectedItem: SelectionItem | null = null;

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.modalClose.emit();
    }
  }

  confirmSelection(): void {
    if (this.selectedItem) {
      this.itemSelected.emit(this.selectedItem);
      this.modalClose.emit();
    }
  }

  selectItem(item: SelectionItem): void {
    this.selectedItem = item;
  }
} 