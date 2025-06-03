import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { NotificationService, NotificationMessage } from '../../services/notification.service';

// 文字內容常數
export const NOTIFICATION_TEXT = {
  buttons: {
    cancel: '取消',
    confirm: '確定'
  },
  types: {
    success: '✅',
    error: '❌',
    warning: '⚠️',
    info: 'ℹ️',
    loading: '⏳',
    confirm: '❓'
  }
};

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss']
})
export class NotificationComponent implements OnInit, OnDestroy {
  notifications: NotificationMessage[] = [];
  private subscription!: Subscription;
  
  // 文字常數
  text = NOTIFICATION_TEXT;

  constructor(private notificationService: NotificationService) {}

  ngOnInit() {
    this.subscription = this.notificationService.notifications.subscribe(
      notifications => {
        this.notifications = notifications;
      }
    );
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  getIcon(type: string): string {
    return this.text.types[type as keyof typeof this.text.types] || this.text.types.info;
  }

  confirm(notification: NotificationMessage) {
    if (notification.confirmCallback) {
      notification.confirmCallback();
    }
    this.remove(notification.id);
  }

  cancel(notification: NotificationMessage) {
    if (notification.cancelCallback) {
      notification.cancelCallback();
    }
    this.remove(notification.id);
  }

  remove(id: string) {
    this.notificationService.remove(id);
  }

  trackById(index: number, item: NotificationMessage) {
    return item.id;
  }
} 