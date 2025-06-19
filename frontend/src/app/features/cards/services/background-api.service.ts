import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface BackgroundImage {
  id: number;
  name: string;
  description?: string;
  imageUrl: string;
  thumbnailUrl?: string;
  category?: string;
  isPublic: boolean;
  createdBy?: number;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}

export interface CreateBackgroundImageDto {
  name: string;
  description?: string;
  imageUrl: string;
  thumbnailUrl?: string;
  category?: string;
  isPublic?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class BackgroundApiService {
  private baseUrl = '/api/backgroundimages';

  constructor(private http: HttpClient) {}

  // 獲取所有公開背景圖片
  getBackgroundImages(category?: string): Observable<BackgroundImage[]> {
    const params: { category?: string } = {};
    if (category) {
      params.category = category;
    }
    return this.http.get<BackgroundImage[]>(this.baseUrl, { params });
  }

  // 根據 ID 獲取背景圖片
  getBackgroundImage(id: number): Observable<BackgroundImage> {
    return this.http.get<BackgroundImage>(`${this.baseUrl}/${id}`);
  }

  // 創建新背景圖片
  createBackgroundImage(backgroundImage: CreateBackgroundImageDto): Observable<BackgroundImage> {
    return this.http.post<BackgroundImage>(this.baseUrl, backgroundImage);
  }

  // 更新背景圖片
  updateBackgroundImage(id: number, backgroundImage: Partial<CreateBackgroundImageDto>): Observable<BackgroundImage> {
    return this.http.put<BackgroundImage>(`${this.baseUrl}/${id}`, backgroundImage);
  }

  // 刪除背景圖片
  deleteBackgroundImage(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
