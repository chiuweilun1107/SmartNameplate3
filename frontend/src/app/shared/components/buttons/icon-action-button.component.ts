import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'sn-icon-action-button',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <button class="sn-icon-action-btn" [ngClass]="color" [attr.aria-label]="ariaLabel" (click)="onClick($event)">
      <mat-icon *ngIf="icon">{{ icon }}</mat-icon>
      <ng-content *ngIf="!icon"></ng-content>
    </button>
  `,
  styles: [`
    .sn-icon-action-btn {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      border-radius: 50%;
      width: 32px;
      height: 32px;
      border: none;
      outline: none;
      cursor: pointer;
      font-size: 22px;
      background: #f5f5f5;
      color: #333;
      transition: background 0.2s, color 0.2s, transform 0.18s cubic-bezier(.4,2,.6,1), box-shadow 0.18s cubic-bezier(.4,2,.6,1);
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      margin: 0 2px;
    }
    .sn-icon-action-btn.green {
      background: #4caf50;
      color: #fff;
    }
    .sn-icon-action-btn.green:hover {
      background: #388e3c;
    }
    .sn-icon-action-btn.red {
      background: #f44336;
      color: #fff;
    }
    .sn-icon-action-btn.red:hover {
      background: #b71c1c;
    }
    .sn-icon-action-btn.gray {
      background: #e0e0e0;
      color: #333;
    }
    .sn-icon-action-btn.gray:hover {
      background: #bdbdbd;
    }
    .sn-icon-action-btn.green:hover, .sn-icon-action-btn.red:hover {
      transform: scale(1.18);
      box-shadow: 0 4px 16px rgba(0,0,0,0.18);
    }
    .sn-icon-action-btn.green:hover mat-icon {
      color: #b9ffb9;
    }
    .sn-icon-action-btn.red:hover mat-icon {
      color: #ffd6d6;
    }
    mat-icon {
      font-size: 22px;
      width: 22px;
      height: 22px;
      line-height: 22px;
    }
  `]
})
export class IconActionButtonComponent {
  @Input() icon = '';
  @Input() color: 'green' | 'red' | 'gray' = 'gray';
  @Input() ariaLabel = '';
  @Output() clicked = new EventEmitter<MouseEvent>();

  onClick(event: MouseEvent) {
    this.clicked.emit(event);
  }
}