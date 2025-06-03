import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'sn-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderComponent {
  @Input() title = 'Magic E-paper';
  @Input() isLoggedIn = false;
  @Input() userName = '';
  @Input() userAvatar = '';
  
  @Output() loginClick = new EventEmitter<void>();
  @Output() logoutClick = new EventEmitter<void>();
  @Output() profileClick = new EventEmitter<void>();

  onLoginClick(): void {
    this.loginClick.emit();
  }

  onLogoutClick(): void {
    this.logoutClick.emit();
  }

  onProfileClick(): void {
    this.profileClick.emit();
  }
} 