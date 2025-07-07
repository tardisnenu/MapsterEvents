import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Kimlik doğrulama guard'ı
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  /**
   * Route'a erişim kontrolü
   */
  canActivate(): boolean {
    if (this.authService.isLoggedIn() && !this.authService.isTokenExpired()) {
      return true;
    }

    // Token süresi dolmuşsa çıkış yap
    if (this.authService.isTokenExpired()) {
      this.authService.logout();
      console.warn('Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.');
    } else {
      console.error('Bu sayfaya erişmek için giriş yapmalısınız.');
    }

    // Login sayfasına yönlendir
    this.router.navigate(['/login']);
    return false;
  }
}