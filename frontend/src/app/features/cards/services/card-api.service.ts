import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CanvasData } from '../models/card-design.models';

export interface Card {
  id: number;
  name: string;
  description?: string;
  status: number; // 0=Draft, 1=Active, 2=Inactive
  thumbnailA?: string;
  thumbnailB?: string;
  contentA?: CanvasData;
  contentB?: CanvasData;
  isSameBothSides: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateCardDto {
  name: string;
  description?: string;
  status?: number;
  thumbnailA?: string;
  thumbnailB?: string;
  contentA?: CanvasData;
  contentB?: CanvasData;
  isSameBothSides?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class CardApiService {
  private baseUrl = '/api/cards';

  constructor(private http: HttpClient) {}

  // 獲取所有桌牌
  getCards(): Observable<Card[]> {
    return this.http.get<Card[]>(this.baseUrl);
  }

  // 根據 ID 獲取桌牌
  getCard(id: number): Observable<Card> {
    return this.http.get<Card>(`${this.baseUrl}/${id}`);
  }

  // 創建新桌牌
  createCard(card: CreateCardDto): Observable<Card> {
    return this.http.post<Card>(this.baseUrl, card);
  }

  // 更新桌牌
  updateCard(id: number, card: Partial<CreateCardDto>): Observable<Card> {
    return this.http.put<Card>(`${this.baseUrl}/${id}`, card);
  }

  // 刪除桌牌
  deleteCard(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  // 🛡️ 安全的狀態文字轉換 - 防止 Object Injection
  getStatusText(status: number): string {
    if (typeof status !== 'number' || !Number.isInteger(status)) {
      return '未知';
    }
    
    const statusMap = new Map([
      [0, '草稿'],
      [1, '已啟用'], 
      [2, '未啟用']
    ]);
    
    return statusMap.get(status) || '未知';
  }
}