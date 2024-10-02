import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { jwtDecode } from 'jwt-decode';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  /**
   * Skip interception for requests targeting the refresh token endpoint.
   * This is done to avoid triggering an infinite loop by intercepting the refresh token call itself.
   * @returns the original request to continue without modification.
   */
  if (req.url.includes('/account/refresh-token')) {
    return next(req);
  }

  // Inject the AuthService to access token management and authentication logic
  const authService = inject(AuthService);
  let accessToken = authService.getToken(); // Retrieve the current access token from local storage

  /**
   * If an access token is available, decode it to check for expiration.
   * If the token is about to expire, a new access token is requested using the refresh token.
   * The request is then cloned with the updated Authorization header.
   * 
   * @returns a cloned request with the 'Authorization' header set, or the original request if no token is found.
   */
  if (accessToken) {
    // Decode the JWT token to retrieve its expiration time ('exp' claim)
    const decodedToken: any = jwtDecode(accessToken);
    const currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
    const expirationTime = decodedToken['exp']; // Token expiration time in seconds

    /**
     * If the token is set to expire within the next hour (3600 seconds),
     * a call to the refresh token endpoint is made to get a new access token.
     * The new token is then stored in local storage for future requests.
     */
    if (expirationTime - currentTime <= 3600) {
      authService.refreshAccessToken().subscribe((newAccessToken) => {
        // Save the new access token in local storage
        localStorage.setItem('token', newAccessToken);
      });
    }

    /**
     * Clone the current HTTP request and add the 'Authorization' header with the
     * existing or newly fetched access token before passing it to the next handler.
     */
    const cloned = req.clone({
      headers: req.headers.set('Authorization', 'Bearer ' + authService.getToken()),
    });

    // @returns cloned request with updated Authorization header
    return next(cloned);
  }

  /**
   * If no access token is present in local storage, proceed with the request as is,
   * without adding the Authorization header.
   * 
   * @returns the original request unmodified.
   */
  return next(req);
};
