import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { User, UserLogin, UserRegister, LoginResponse, RegisterResponse } from '../models/user.model';
import { environment } from '../../environments/environment';

/**
 * Kimlik doğrulama servisi
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    // Local storage'dan kullanıcı bilgisini yükle
    const savedUser = localStorage.getItem('currentUser');
    if (savedUser) {
      this.currentUserSubject.next(JSON.parse(savedUser));
    }
  }

  /**
   * Mevcut kullanıcıyı getirir
   */
  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  /**
   * Kullanıcı girişi
   */
  login(userLogin: UserLogin): Observable<LoginResponse> {
    console.log('AuthService.login called with:', userLogin);
    console.log('API URL:', `${this.apiUrl}/login`);
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, userLogin)
      .pipe(
        tap(response => {
          // Token'ı local storage'a kaydet
          localStorage.setItem('token', response.token);
          localStorage.setItem('currentUser', JSON.stringify(response.user));
          
          // Current user'ı güncelle
          this.currentUserSubject.next(response.user);
        })
      );
  }

  /**
   * Kullanıcı kaydı
   */
  register(userRegister: UserRegister): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/register`, userRegister);
  }

  /**
   * Kullanıcı çıkışı
   */
  logout(): Observable<void> {
    // Local storage'ı temizle
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    
    // Current user'ı temizle
    this.currentUserSubject.next(null);
    
    return of(void 0);
  }

  /**
   * Token'ın geçerli olup olmadığını kontrol eder
   */
  validateToken(): Observable<any> {
    return this.http.get(`${this.apiUrl}/validate-token`);
  }

  /**
   * E-posta adresinin kullanımda olup olmadığını kontrol eder
   */
  checkEmail(email: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/check-email?email=${email}`);
  }

  /**
   * Token'ı getirir
   */
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  /**
   * Kullanıcının giriş yapmış olup olmadığını kontrol eder
   */
  isLoggedIn(): boolean {
    const token = this.getToken();
    const user = this.currentUserValue;
    return !!(token && user && !this.isTokenExpired());
  }

  /**
   * Token'ın süresinin dolup dolmadığını kontrol eder
   */
  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp * 1000; // Convert to milliseconds
      return Date.now() >= exp;
    } catch (error) {
      return true;
    }
  }
}