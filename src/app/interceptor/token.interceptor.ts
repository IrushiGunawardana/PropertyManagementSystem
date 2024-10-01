import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import jwtDecode from 'jwt-decode';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
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
        accessToken = newAccessToken;
        localStorage.setItem('token', newAccessToken); // Save the new access token in local storage

        const cloned = req.clone({
          headers: req.headers.set('Authorization', 'Bearer ' + newAccessToken),
        });

        return next(cloned); // Continue with the new token
      });
    } else {
      const cloned = req.clone({
        headers: req.headers.set('Authorization', 'Bearer ' + accessToken),
      });
      return next(cloned); // Continue with the existing token if it's valid
    }
  }

  return next(req); // Continue without token if not present
};
