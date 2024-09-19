import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpErrorResponse, HttpEvent } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';  // Import CookieService

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  
  constructor(private router: Router, private cookieService: CookieService) {}  // Inject CookieService

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Retrieve the JWT token from the cookie
    const token = this.cookieService.get('token');  // Assuming the token is stored in a cookie named 'token'

    // Clone the request to add the Authorization header if the token exists
    let clonedReq = req;
    if (token) {
      clonedReq = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)  // Add the JWT token to the Authorization header
      });
    }

    // Handle the HTTP request and catch any errors
    return next.handle(clonedReq).pipe(
      catchError((error: HttpErrorResponse) => {
        // If the error is 401 Unauthorized, redirect the user to the home page
        if (error.status === 401) {
          console.log("Unauthorized request - redirecting to home.");
          this.router.navigate(['/home']);
        }
        // Return the error as an observable for further handling
        return throwError(error);
      })
    );
  }
}
