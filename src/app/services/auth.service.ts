import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../environments/environment.development';
import { LoginRequest } from '../models/login-request';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { AuthResponse } from '../models/auth-response';
import { jwtDecode } from 'jwt-decode';
import { RegisterRequest } from '../models/register-request';

@Injectable({
  providedIn: 'root', // This service is provided at the root level, meaning it's a singleton and available throughout the application
})
export class AuthService {
  // Base API URL from environment config
  apiUrl: string = environment.apiUrl;
  
  // Key to store/retrieve token from local storage
  private tokenKey = 'token';
  
  // Flag to determine if code is running in a browser environment
  isBrowser: boolean = false;

  constructor(private http: HttpClient, @Inject(PLATFORM_ID) platformId: Object) {
    // Check if the code is running in a browser 
    this.isBrowser = isPlatformBrowser(platformId);
  }

  /**
   * Logs in the user by sending login credentials to the server.
   * On success, it stores the received JWT token in localStorage.
   * @param data - LoginRequest object containing user credentials
   * @returns Observable<AuthResponse> - The API response with authentication data
   */
  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}account/login`, data)
      .pipe(
        map((response) => {
          // On successful login, store the JWT token in localStorage
          if (response.isSuccess) {
            localStorage.setItem(this.tokenKey, response.access_token);
          }
          return response; // Return the response as an observable
        })
      );
  }

  /**
   * Registers a new user by sending registration data to the server.
   * @param data - RegisterRequest object containing registration details
   * @returns Observable<AuthResponse> - The API response with the result of the registration
   */
  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}account/register`, data);
  }

  /**
   * Extracts the user's details from the stored JWT token.
   * @returns userDetail object containing user id, full name, email, and roles
   */
  getUserDetail = () => {
    const token = this.getToken();
    if (!token) return null;

    // Decode the JWT token to retrieve user information
    const decodedToken: any = jwtDecode(token);
    const userDetail = {
      id: decodedToken.UserId,
      fullName: decodedToken.FirstName + " " + decodedToken.LastName,
      email: decodedToken.email,
      roles: decodedToken.role || [], // Roles may not always exist, fallback to an empty array
    };

    return userDetail; // Return the user details extracted from the token
  };

  /**
   * Checks if the user is logged in by verifying the presence and validity of the JWT token.
   * @returns boolean - True if the user is logged in, false otherwise
   */
  isLoggedIn = (): boolean => {
    const token = this.getToken();
    if (!token) return false;

    // Check if the token has expired
    return !this.isTokenExpired();
  };

  /**
   * Checks if the stored JWT token is expired based on the expiration timestamp.
   * If the token is expired, it logs the user out by removing the token from localStorage.
   * @returns boolean - True if the token is expired, false otherwise
   */
  private isTokenExpired() {
    const token = this.getToken();
    if (!token) return true;

    // Decode the JWT token and check the expiration time
    const decoded = jwtDecode(token);
    const isTokenExpired = Date.now() >= decoded['exp']! * 1000; // 'exp' is in seconds, Date.now() is in milliseconds

    // If the token is expired, log the user out
    if (isTokenExpired) this.logout();
    return isTokenExpired;
  }

  /**
   * Logs the user out by removing the JWT token from localStorage.
   * This is only done in a browser environment.
   */
  logout = (): void => {
    if (this.isBrowser) {
      localStorage.removeItem(this.tokenKey); // Remove the token to effectively log out the user
    }
  };

  /**
   * Retrieves the stored JWT token from localStorage, if available.
   * @returns string - The stored JWT token, or an empty string if not available
   */
  getToken = (): string |null => {
    
      return localStorage.getItem(this.tokenKey) || ''; // Fetch token or return an empty string if not found
    
  };


  refreshAccessToken(): Observable<string> {
    const refreshToken = localStorage.getItem('refresh_token'); // Assuming refresh token is stored separately
    return this.http
      .post<{ access_token: string }>(`${this.apiUrl}account/refreshtoken`, {
        refresh_token: refreshToken,
      })
      .pipe(
        map((response) => response.access_token) // Return the new access token
      );
  }
  
}
