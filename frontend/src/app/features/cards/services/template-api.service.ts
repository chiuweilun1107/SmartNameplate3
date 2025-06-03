import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Template {
  id: number;
  name: string;
  description?: string;
  thumbnailUrl?: string;
  thumbnailA?: string;   // A面縮圖
  thumbnailB?: string;   // B面縮圖
  layoutDataA: any;
  layoutDataB: any;
  dimensions: any;
  organizationId?: number;
  createdBy?: number;
  isPublic: boolean;
  category: string;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}

export interface CreateTemplateDto {
  name: string;
  description?: string;
  thumbnailUrl?: string;
  thumbnailA?: string;   // A面縮圖
  thumbnailB?: string;   // B面縮圖
  layoutDataA: any;
  layoutDataB: any;
  dimensions: any;
  isPublic?: boolean;
  category?: string;
}

export interface TemplateListItem {
  id: number;
  name: string;
  description?: string;
  thumbnailUrl?: string;
  thumbnailA?: string;   // A面縮圖
  thumbnailB?: string;   // B面縮圖
  category: string;
  isPublic: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class TemplateApiService {
  private baseUrl = '/api/templates';

  constructor(private http: HttpClient) {}

  // 獲取所有公開樣板
  getTemplates(category?: string): Observable<TemplateListItem[]> {
    let params: any = {};
    if (category) {
      params.category = category;
    }
    return this.http.get<TemplateListItem[]>(this.baseUrl, { params });
  }

  // 根據 ID 獲取樣板
  getTemplate(id: number): Observable<Template> {
    return this.http.get<Template>(`${this.baseUrl}/${id}`);
  }

  // 創建新樣板
  createTemplate(template: CreateTemplateDto): Observable<Template> {
    return this.http.post<Template>(this.baseUrl, template);
  }

  // 更新樣板
  updateTemplate(id: number, template: Partial<CreateTemplateDto>): Observable<Template> {
    return this.http.put<Template>(`${this.baseUrl}/${id}`, template);
  }

  // 刪除樣板
  deleteTemplate(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
