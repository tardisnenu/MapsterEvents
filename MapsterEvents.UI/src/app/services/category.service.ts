import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../models/category.model';
import { environment } from '../../environments/environment';

/**
 * Kategori servisi
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = `${environment.apiUrl}/categories`;

  constructor(private http: HttpClient) { }

  /**
   * Tüm kategorileri getirir
   */
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  /**
   * Kategori detayını getirir
   */
  getCategoryById(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/${id}`);
  }

  /**
   * Kategorileri etkinlik sayısıyla birlikte getirir
   */
  getCategoriesWithEventCount(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/with-event-count`);
  }

  /**
   * Yeni kategori oluşturur
   */
  createCategory(category: { name: string; description: string }): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, category);
  }

  /**
   * Kategori günceller
   */
  updateCategory(id: number, category: { id: number; name: string; description: string }): Observable<Category> {
    return this.http.put<Category>(`${this.apiUrl}/${id}`, category);
  }

  /**
   * Kategori siler
   */
  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Kategori adının kullanımda olup olmadığını kontrol eder
   */
  checkCategoryName(name: string, excludeId?: number): Observable<boolean> {
    let url = `${this.apiUrl}/check-name?name=${encodeURIComponent(name)}`;
    if (excludeId) {
      url += `&excludeId=${excludeId}`;
    }
    return this.http.get<boolean>(url);
  }
}