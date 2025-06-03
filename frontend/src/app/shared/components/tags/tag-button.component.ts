import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'sn-tag-button',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <button 
      class="tag-button"
      [class.active]="isActive"
      [class.tag-button--active]="isActive"
      [class.disabled]="isDisabled"
      [class.has-icon]="icon"
      (click)="onClick()">
      <mat-icon *ngIf="icon" class="tag-button__icon">{{ icon }}</mat-icon>
      <span class="tag-button__label">{{ label }}</span>
    </button>
  `,
  styleUrls: ['./tag-button.component.scss']
})
export class TagButtonComponent {
  @Input() label = '';
  @Input() icon?: string;
  @Input() isActive = false;
  @Input() isDisabled = false;
  @Input() value?: any;
  @Output() tagClick = new EventEmitter<any>();

  onClick(): void {
    if (!this.isDisabled) {
      this.tagClick.emit(this.value || this.label);
    }
  }
} 