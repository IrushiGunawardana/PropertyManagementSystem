import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import {jwtDecode } from 'jwt-decode';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  
  if (req.url.includes('/account/refresh-token')) {
    return next(req);
  }

  const authService = inject(AuthService);
  let accessToken = authService.getToken();

  if (accessToken) {
    const decodedToken: any = jwtDecode(accessToken);
    const currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
    const expirationTime = decodedToken['exp'];

    // Check if the token is going to expire within the next hour (3600 seconds)
    if (expirationTime - currentTime <= 3600) {
      // Call the refresh token endpoint if access token is about to expire
      authService.refreshAccessToken().subscribe((newAccessToken) => {
        localStorage.setItem('token', newAccessToken); // Save the new access token in local storage
      });
    } 
    const cloned = req.clone({
      headers: req.headers.set('Authorization', 'Bearer ' + authService.getToken()),
    });

    return next(cloned); // Continue with the new token
  }

  return next(req); // Continue without token if not present
};
