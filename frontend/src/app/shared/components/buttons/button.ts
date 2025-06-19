import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'sn-button',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  template: `
    <button
      mat-button
      [color]="color"
      [disabled]="disabled"
      [class]="buttonClass"
      (click)="onClick()"
      (keydown.enter)="onClick()"
      (keydown.space)="onClick()"
      tabindex="0" role="button">
      <mat-icon *ngIf="icon">{{ icon }}</mat-icon>
      <span>{{ label }}</span>
    </button>
  `,
  styles: [`
    .sn-btn {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      border-radius: 24px;
      font-size: 22px;
      font-weight: 600;
      padding: 0 40px;
      height: 72px;
      min-width: 180px;
      border: none;
      outline: none;
      cursor: pointer;
      transition: background 0.2s, color 0.2s, border 0.2s;
      user-select: none;
    }
  `]
})
export class ButtonComponent {
  @Input() label = '';
  @Input() icon = '';
  @Input() color: 'primary' | 'accent' | 'warn' | '' = '';
  @Input() disabled = false;
  @Input() buttonClass = '';

  @Output() buttonClick = new EventEmitter<void>();

  onClick(): void {
    if (!this.disabled) {
      this.buttonClick.emit();
    }
  }
} 