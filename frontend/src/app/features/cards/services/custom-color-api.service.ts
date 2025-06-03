import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CustomColor {
  id: number;
  name: string;
  colorValue: string;
  createdAt: string;
  updatedAt: string;
  createdBy?: string;
  isPublic: boolean;
}

export interface CreateCustomColorDto {
  name: string;
  colorValue: string;
  createdBy?: string;
  isPublic?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class CustomColorApiService {
  private readonly apiUrl = '/api/customcolors';

  constructor(private http: HttpClient) {}

  getCustomColors(): Observable<CustomColor[]> {
    return this.http.get<CustomColor[]>(this.apiUrl);
  }

  getCustomColor(id: number): Observable<CustomColor> {
    return this.http.get<CustomColor>(`${this.apiUrl}/${id}`);
  }

  createCustomColor(data: CreateCustomColorDto): Observable<CustomColor> {
    return this.http.post<CustomColor>(this.apiUrl, data);
  }

  updateCustomColor(id: number, data: Partial<CreateCustomColorDto>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }

  deleteCustomColor(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}