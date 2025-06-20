import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Group {
  id: number;
  name: string;
  description?: string;
  color: string;
  cardCount?: number;
  deviceCount?: number;
}

@Component({
  selector: 'sn-group-selection-item',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div 
      class="group-selection-item"
      [class.selected]="isSelected"
      (click)="onItemClick()"
      (keydown.enter)="onItemClick()"
      (keydown.space)="onItemClick()"
      tabindex="0" role="button">
      <div class="group-preview">
        <div class="group-color" [style.background-color]="group.color"></div>
        <div class="group-info">
          <h4 class="group-name">{{ group.name }}</h4>
          <p class="group-desc">{{ group.description || '無描述' }}</p>
          <div class="group-stats">
            <span class="card-count">{{ group.cardCount || 0 }} 張圖卡</span>
            <span class="device-count">{{ group.deviceCount || 0 }} 個設備</span>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./group-selection-item.component.scss']
})
export class GroupSelectionItemComponent {
  @Input() group: Group = {
    id: 0,
    name: '',
    color: '#e3f2fd'
  };
  @Input() isSelected = false;

  @Output() itemClick = new EventEmitter<void>();

  onItemClick(): void {
    this.itemClick.emit();
  }
} 