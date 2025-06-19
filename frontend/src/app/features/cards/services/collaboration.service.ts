import { Injectable } from '@angular/core';
import { BehaviorSubject, fromEvent } from 'rxjs';
import { map, throttleTime, distinctUntilChanged } from 'rxjs/operators';
import { CollaborationUser, CollaborationAction, Position } from '../models/card-design.models';
import { CryptoService } from '../../../core/services/crypto.service';

@Injectable({
  providedIn: 'root'
})
export class CollaborationService {
  private collaboratorsSubject = new BehaviorSubject<CollaborationUser[]>([]);
  private actionsSubject = new BehaviorSubject<CollaborationAction[]>([]);
  private currentUserSubject = new BehaviorSubject<CollaborationUser | null>(null);
  
  collaborators$ = this.collaboratorsSubject.asObservable();
  actions$ = this.actionsSubject.asObservable();
  currentUser$ = this.currentUserSubject.asObservable();

  // WebSocket 連接（模擬）
  private wsConnection: WebSocket | null = null;

  constructor(private cryptoService: CryptoService) {
    this.initializeCurrentUser();
    this.setupMouseTracking();
  }

  // 🛡️ 初始化當前用戶 - 使用安全的隨機選擇
  private initializeCurrentUser(): void {
    const colors = ['#ff6b6b', '#4ecdc4', '#45b7d1', '#f9ca24', '#f0932b', '#eb4d4b', '#6c5ce7'];
    const randomColor = this.cryptoService.selectSecureRandomColor(colors);
    
    const currentUser: CollaborationUser = {
      id: this.generateUserId(),
      name: '我',
      color: randomColor,
      cursor: { x: 0, y: 0 }
    };
    
    this.currentUserSubject.next(currentUser);
  }

  // 設置鼠標追蹤
  private setupMouseTracking(): void {
    const mouseMove$ = fromEvent<MouseEvent>(document, 'mousemove');
    const throttledMouse$ = mouseMove$.pipe(
      throttleTime(100),
      map(event => ({ x: event.clientX, y: event.clientY })),
      distinctUntilChanged((prev, curr) => prev.x === curr.x && prev.y === curr.y)
    );

    throttledMouse$.subscribe(position => {
      this.updateCursorPosition(position);
    });
  }

  // 連接協作會話
  connectToSession(cardId: string): void {
    console.log('連接到協作會話:', cardId);
    
    // TODO: 實際 WebSocket 連接
    // this.wsConnection = new WebSocket(`ws://localhost:3000/collaboration/${cardId}`);
  }

  // 斷開連接
  disconnect(): void {
    if (this.wsConnection) {
      this.wsConnection.close();
      this.wsConnection = null;
    }
    this.collaboratorsSubject.next([]);
  }

  // 發送協作動作
  sendAction(action: Omit<CollaborationAction, 'userId' | 'timestamp'>): void {
    const currentUser = this.currentUserSubject.value;
    if (!currentUser) return;

    const fullAction: CollaborationAction = {
      ...action,
      userId: currentUser.id,
      timestamp: new Date()
    };

    // 添加到本地動作歷史
    const currentActions = this.actionsSubject.value;
    this.actionsSubject.next([...currentActions, fullAction]);

    // TODO: 發送到 WebSocket
    console.log('發送協作動作:', fullAction);
  }

  // 更新光標位置
  private updateCursorPosition(position: Position): void {
    const currentUser = this.currentUserSubject.value;
    if (currentUser) {
      currentUser.cursor = position;
      this.currentUserSubject.next(currentUser);
      
      this.sendAction({
        type: 'cursor_move',
        data: position
      });
    }
  }

  // 更新選中元素
  updateSelectedElement(elementId: string | null): void {
    const currentUser = this.currentUserSubject.value;
    if (currentUser) {
      currentUser.selectedElement = elementId || undefined;
      this.currentUserSubject.next(currentUser);
    }
  }

  // 處理接收到的動作
  handleReceivedAction(action: CollaborationAction): void {
    switch (action.type) {
      case 'cursor_move':
        // 🛡️ 類型保護：確保 data 是 Position 類型
        if (this.isPosition(action.data)) {
          this.updateCollaboratorCursor(action.userId, action.data);
        }
        break;
      case 'element_add':
      case 'element_update':
      case 'element_delete':
      case 'element_move':
        // 這些動作應該由 CardDesignerService 處理
        break;
    }
  }

  // 🛡️ 類型保護：檢查是否為 Position 類型
  private isPosition(data: CollaborationAction['data']): data is Position {
    return typeof data === 'object' && 
           data !== null && 
           typeof (data as Position).x === 'number' && 
           typeof (data as Position).y === 'number';
  }

  // 更新協作者光標
  private updateCollaboratorCursor(userId: string, position: Position): void {
    const collaborators = this.collaboratorsSubject.value;
    const updatedCollaborators = collaborators.map(user => 
      user.id === userId ? { ...user, cursor: position } : user
    );
    this.collaboratorsSubject.next(updatedCollaborators);
  }

  // 🛡️ 工具方法 - 使用安全的ID生成
  private generateUserId(): string {
    return this.cryptoService.generateUserId();
  }

  // 獲取協作者顏色
  getCollaboratorColor(userId: string): string {
    const collaborators = this.collaboratorsSubject.value;
    const user = collaborators.find(u => u.id === userId);
    return user?.color || '#666';
  }

  // 檢查元素是否被其他人選中
  isElementSelectedByOthers(elementId: string): boolean {
    const collaborators = this.collaboratorsSubject.value;
    return collaborators.some(user => user.selectedElement === elementId);
  }

  // 獲取選中指定元素的用戶
  getUsersSelectingElement(elementId: string): CollaborationUser[] {
    const collaborators = this.collaboratorsSubject.value;
    return collaborators.filter(user => user.selectedElement === elementId);
  }
}
