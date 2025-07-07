import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EventListItem, EventDetail, EventCreate } from '../models/event.model';
import { environment } from '../../environments/environment';

/**
 * Etkinlik servisi
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
@Injectable({
  providedIn: 'root'
})
export class EventService {
  private apiUrl = `${environment.apiUrl}/events`;

  constructor(private http: HttpClient) { }

  /**
   * Tüm etkinlikleri getirir
   */
  getAllEvents(): Observable<EventListItem[]> {
    return this.http.get<EventListItem[]>(this.apiUrl);
  }

  /**
   * Etkinlik detayını getirir
   */
  getEventById(id: number): Observable<EventDetail> {
    return this.http.get<EventDetail>(`${this.apiUrl}/${id}`);
  }

  /**
   * Yaklaşan etkinlikleri getirir
   */
  getUpcomingEvents(): Observable<EventListItem[]> {
    return this.http.get<EventListItem[]>(`${this.apiUrl}/upcoming`);
  }

  /**
   * Geçmiş etkinlikleri getirir
   */
  getPastEvents(): Observable<EventListItem[]> {
    return this.http.get<EventListItem[]>(`${this.apiUrl}/past`);
  }

  /**
   * Kategoriye göre etkinlikleri getirir
   */
  getEventsByCategory(categoryId: number): Observable<EventListItem[]> {
    return this.http.get<EventListItem[]>(`${this.apiUrl}/category/${categoryId}`);
  }

  /**
   * Kullanıcının organize ettiği etkinlikleri getirir
   */
  getMyOrganizedEvents(): Observable<EventListItem[]> {
    return this.http.get<EventListItem[]>(`${this.apiUrl}/my-organized`);
  }

  /**
   * Kullanıcının kayıtlı olduğu etkinlikleri getirir
   */
  getMyRegisteredEvents(): Observable<EventListItem[]> {
    return this.http.get<EventListItem[]>(`${this.apiUrl}/my-registrations`);
  }

  /**
   * Yeni etkinlik oluşturur
   */
  createEvent(event: EventCreate): Observable<EventDetail> {
    return this.http.post<EventDetail>(this.apiUrl, event);
  }

  /**
   * Etkinlik günceller
   */
  updateEvent(id: number, event: EventCreate): Observable<EventDetail> {
    return this.http.put<EventDetail>(`${this.apiUrl}/${id}`, event);
  }

  /**
   * Etkinlik siler
   */
  deleteEvent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Etkinliğe kayıt olur
   */
  registerToEvent(id: number): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/${id}/register`, {});
  }

  /**
   * Etkinlik kaydını iptal eder
   */
  unregisterFromEvent(id: number): Observable<string> {
    return this.http.delete<string>(`${this.apiUrl}/${id}/unregister`);
  }

  /**
   * Kullanıcının etkinliğe kayıtlı olup olmadığını kontrol eder
   */
  getRegistrationStatus(id: number): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/${id}/registration-status`);
  }
}