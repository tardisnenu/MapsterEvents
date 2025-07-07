import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

/**
 * Error Handling Interceptor
 * Global error handling for HTTP requests
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Beklenmeyen bir hata oluştu';

      switch (error.status) {
        case 400:
          errorMessage = error.error?.message || 'Geçersiz istek';
          break;
        case 401:
          errorMessage = 'Oturumunuz sona erdi. Lütfen tekrar giriş yapın.';
          authService.logout();
          router.navigate(['/login']);
          break;
        case 403:
          errorMessage = 'Bu işlem için yetkiniz bulunmuyor';
          break;
        case 404:
          errorMessage = 'Aradığınız kaynak bulunamadı';
          break;
        case 422:
          errorMessage = error.error?.message || 'Doğrulama hatası';
          break;
        case 500:
          errorMessage = 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.';
          break;
        case 0:
          errorMessage = 'Bağlantı hatası. İnternet bağlantınızı kontrol edin.';
          break;
        default:
          errorMessage = error.error?.message || `Hata kodu: ${error.status}`;
      }

      // Log error to console for now
      console.error('HTTP Error:', errorMessage, error);

      return throwError(() => error);
    })
  );
}; 