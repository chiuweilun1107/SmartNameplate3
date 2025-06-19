import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'sn-home-feature-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="home__feature-card" [ngClass]="customClass">
      <div class="home__feature-content">
        <div class="home__feature-icon" [ngClass]="iconClass">
          <img [src]="icon" [alt]="title" class="home__feature-icon-image">
        </div>
        <h3 class="home__feature-title">{{ title }}</h3>
        <p class="home__feature-description" [innerHTML]="desc"></p>
        <a *ngIf="link" [routerLink]="link" class="home__feature-link">
          {{ linkText }}
          <svg class="home__feature-arrow" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
          </svg>
        </a>
      </div>
    </div>
  `,
  styleUrls: ['./card.scss']
})
export class HomeFeatureCardComponent {
  @Input() icon = '';
  @Input() iconClass = '';
  @Input() title = '';
  @Input() desc = '';
  @Input() link = '';
  @Input() linkText = '';
  @Input() customClass = '';
} 