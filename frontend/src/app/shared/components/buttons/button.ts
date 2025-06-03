import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'sn-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button class="sn-btn" [ngClass]="type">
      <ng-content></ng-content>
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
    .sn-btn.primary {
      background: #407cff;
      color: #fff;
      border: none;
    }
    .sn-btn.primary:hover {
      background: #225be6;
    }
    .sn-btn.outline {
      background: #fff;
      color: #407cff;
      border: 3px solid #407cff;
    }
    .sn-btn.outline:hover {
      background: #eaf1ff;
      color: #225be6;
      border-color: #225be6;
    }
  `]
})
export class Button {
  @Input() type: 'primary' | 'outline' = 'primary';
} 