import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'sn-card-selection-item',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div 
      class="card-selection-item"
      [class.selected]="isSelected"
      (click)="onItemClick()">
      <div class="card-selection-checkbox">
        <input 
          type="checkbox" 
          [id]="'card-' + card.id"
          [checked]="isSelected"
          (change)="onSelectionChange($event)"
          (click)="$event.stopPropagation()">
      </div>
      <label [for]="'card-' + card.id" class="card-selection-label">
        <div class="card-preview">
          <div class="side-toggle-buttons">
            <button 
              class="side-toggle" 
              [class.active]="currentSide !== 'B'"
              (click)="toggleSide('A', $event)">
              A
            </button>
            <button 
              class="side-toggle" 
              [class.active]="currentSide === 'B'"
              (click)="toggleSide('B', $event)">
              B
            </button>
          </div>
          <img 
            *ngIf="getCurrentSideImage()" 
            [src]="getCurrentSideImage()" 
            [alt]="card.name + ' - ' + currentSide + '面'"
            class="card-image">
          <div 
            *ngIf="!getCurrentSideImage()" 
            class="card-placeholder">
            <span>{{ card.name.charAt(0) }}</span>
          </div>
        </div>
        <div class="card-info">
          <div class="card-info-content">
            <div class="card-text-info">
              <h4 class="card-name">{{ card.name }}</h4>
              <p class="card-desc">{{ card.description || '無描述' }}</p>
            </div>
            <div class="card-ab-preview">
              <div class="side-preview" [title]="getOtherSideTitle()">
                <img [src]="getOtherSidePreview()" [alt]="getOtherSideTitle()">
              </div>
            </div>
          </div>
        </div>
      </label>
    </div>
  `,
  styleUrls: ['./card-selection-item.component.scss']
})
export class CardSelectionItemComponent {
  @Input() card: any = {};
  @Input() isSelected = false;

  @Output() selectionChange = new EventEmitter<boolean>();
  @Output() itemClick = new EventEmitter<void>();

  currentSide: 'A' | 'B' = 'A';

  onItemClick(): void {
    this.itemClick.emit();
  }

  onSelectionChange(event: any): void {
    this.selectionChange.emit(event.target.checked);
  }

  toggleSide(side: 'A' | 'B', event: Event): void {
    event.stopPropagation();
    event.preventDefault();
    this.currentSide = side;
  }

  getCurrentSideImage(): string | undefined {
    if (this.currentSide === 'B' && this.card.thumbnailB) {
      return this.card.thumbnailB;
    }
    return this.card.thumbnailA || this.card.thumbnail;
  }

  getOtherSidePreview(): string | undefined {
    const otherSide = this.currentSide === 'B' ? 'A' : 'B';
    if (otherSide === 'B' && this.card.thumbnailB) {
      return this.card.thumbnailB;
    }
    return this.card.thumbnailA || this.card.thumbnail;
  }

  getOtherSideTitle(): string {
    const otherSide = this.currentSide === 'B' ? 'A' : 'B';
    return `${otherSide}面預覽`;
  }
} 