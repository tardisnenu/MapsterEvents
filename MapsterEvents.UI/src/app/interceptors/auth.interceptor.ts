import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

/**
 * Authentication Interceptor
 * Automatically adds JWT token to requests
 * Created by Hamza Canturk - https://hamzacanturk.com/
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  // Skip auth for login/register endpoints
  const isAuthEndpoint = req.url.includes('/auth/');
  console.log('Auth interceptor - URL:', req.url, 'isAuthEndpoint:', isAuthEndpoint);
  
  // Always add Content-Type for POST/PUT requests
  let headers: { [key: string]: string } = {};
  
  if (req.method === 'POST' || req.method === 'PUT') {
    headers['Content-Type'] = 'application/json';
  }
  
  if (token && !isAuthEndpoint) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  
  if (Object.keys(headers).length > 0) {
    const authReq = req.clone({
      setHeaders: headers
    });
    return next(authReq);
  }

  return next(req);
}; 