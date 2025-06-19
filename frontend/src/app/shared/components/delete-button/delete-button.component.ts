import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'sn-delete-button',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <div class="delete-button"
         [class]="'delete-button--' + size"
         [title]="tooltip"
         (click)="onDelete($event)"
         (keydown.enter)="onDelete($event)"
         (keydown.space)="onDelete($event)"
         tabindex="0" role="button">
      <mat-icon>delete</mat-icon>
    </div>
  `,
  styleUrls: ['./delete-button.component.scss']
})
export class DeleteButtonComponent {
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Input() tooltip = '刪除';
  @Output() delete = new EventEmitter<void>();

  onDelete(event: Event): void {
    event.stopPropagation();
    this.delete.emit();
  }
}
