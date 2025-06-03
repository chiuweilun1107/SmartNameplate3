import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ElementImage {
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

export interface CreateElementImageDto {
  name: string;
  description?: string;
  imageUrl: string;
  thumbnailUrl?: string;
  category?: string;
  isPublic?: boolean;
}

export interface UpdateElementImageDto {
  name?: string;
  description?: string;
  imageUrl?: string;
  thumbnailUrl?: string;
  category?: string;
  isPublic?: boolean;
  isActive?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ElementImageApiService {
  private baseUrl = '/api/elementimages';

  constructor(private http: HttpClient) {}

  getElementImages(category?: string): Observable<ElementImage[]> {
    const url = category ? `${this.baseUrl}?category=${category}` : this.baseUrl;
    return this.http.get<ElementImage[]>(url);
  }

  getElementById(id: number): Observable<ElementImage> {
    return this.http.get<ElementImage>(`${this.baseUrl}/${id}`);
  }

  createElement(elementImage: CreateElementImageDto): Observable<ElementImage> {
    return this.http.post<ElementImage>(this.baseUrl, elementImage);
  }

  updateElement(id: number, elementImage: UpdateElementImageDto): Observable<ElementImage> {
    return this.http.put<ElementImage>(`${this.baseUrl}/${id}`, elementImage);
  }

  deleteElement(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}