import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';

export interface NotificationMessage {
  id: string;
  type: 'success' | 'error' | 'info' | 'warning' | 'loading' | 'confirm';
  title: string;
  message?: string;
  duration?: number;
  progress?: number;
  showProgress?: boolean;
  confirmCallback?: () => void;
  cancelCallback?: () => void;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private notifications$ = new BehaviorSubject<NotificationMessage[]>([]);
  private progressSubject = new Subject<{ id: string; progress: number }>();

  get notifications() {
    return this.notifications$.asObservable();
  }

  get progressUpdates() {
    return this.progressSubject.asObservable();
  }

  private generateId(): string {
    return Math.random().toString(36).substr(2, 9);
  }

  show(notification: Omit<NotificationMessage, 'id'>): string {
    const id = this.generateId();
    const newNotification: NotificationMessage = {
      id,
      duration: notification.duration || 3000,
      ...notification
    };

    const current = this.notifications$.value;
    this.notifications$.next([...current, newNotification]);

    // 自動移除（除非是loading類型）
    if (notification.type !== 'loading' && newNotification.duration! > 0) {
      setTimeout(() => {
        this.remove(id);
      }, newNotification.duration);
    }

    return id;
  }

  success(title: string, message?: string, duration?: number): string {
    return this.show({
      type: 'success',
      title,
      message,
      duration
    });
  }

  error(title: string, message?: string, duration?: number): string {
    return this.show({
      type: 'error',
      title,
      message,
      duration: duration || 5000
    });
  }

  info(title: string, message?: string, duration?: number): string {
    return this.show({
      type: 'info',
      title,
      message,
      duration
    });
  }

  warning(title: string, message?: string, duration?: number): string {
    return this.show({
      type: 'warning',
      title,
      message,
      duration
    });
  }

  loading(title: string, message?: string, showProgress = false): string {
    return this.show({
      type: 'loading',
      title,
      message,
      showProgress,
      duration: 0 // loading不自動消失
    });
  }

  updateProgress(id: string, progress: number): void {
    const current = this.notifications$.value;
    const updated = current.map(notification => 
      notification.id === id 
        ? { ...notification, progress }
        : notification
    );
    this.notifications$.next(updated);
    this.progressSubject.next({ id, progress });
  }

  remove(id: string): void {
    const current = this.notifications$.value;
    const updated = current.filter(notification => notification.id !== id);
    this.notifications$.next(updated);
  }

  clear(): void {
    this.notifications$.next([]);
  }

  confirm(title: string, message?: string, confirmCallback?: () => void, cancelCallback?: () => void): string {
    return this.show({
      type: 'confirm',
      title,
      message,
      duration: 0, // 確認對話框不自動消失
      confirmCallback,
      cancelCallback
    });
  }
} 