import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Role } from '../models/role';

/**
 * RoleService is responsible for handling role-related API requests,
 * including fetching the list of roles available in the application.
 */
@Injectable({
  providedIn: 'root',
})
export class RoleService {
  apiUrl = environment.apiUrl; // Base URL for the API

  /**
   * Constructs the RoleService.
   * 
   * @param http - HttpClient for making HTTP requests.
   */
  constructor(private http: HttpClient) { }

  /**
   * Fetches the list of roles from the API.
   * 
   * @returns Observable of Role[] containing the list of roles.
   */
  getRoles = (): Observable<Role[]> =>
    this.http.get<Role[]>(`${this.apiUrl}roles`); // HTTP GET request to fetch roles
}
