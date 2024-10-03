import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../environments/environment.development';
import { Observable, of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { JobsListResponseDto } from '../models/job-list-response';
import { JobDetailsResponseDto } from '../models/job-entity-response';
import { PropertyListResponseDto } from '../models/property-list-response';
import { JobTypeListResponseDto } from '../models/job-type-response';
import { ServiceProviderResponseDto } from '../models/service-provider-response';
import { CreateJobRequestDto } from '../models/create-job-request';
import { CreateJobResponseDto } from '../models/create-job-response';

/**
 * JobService is responsible for handling job-related API requests,
 * including fetching jobs, job details, properties, job types,
 * and creating new jobs.
 */
@Injectable({
  providedIn: 'root'
})
export class JobService {

  apiUrl: string = environment.apiUrl; // Base URL for the API
  private tokenKey = 'token'; // Key for accessing the authentication token in localStorage

  /**
   * Constructs the JobService.
   * 
   * @param http - HttpClient for making HTTP requests.
   * @param platformId - Object to check the platform (browser or server).
   */
  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object // Inject platformId to check if it's a browser
  ) { }

  /**
   * Fetches all jobs from the API.
   * 
   * @returns Observable of JobsListResponseDto containing the list of jobs.
   */
  getJobs(): Observable<JobsListResponseDto> {
    if (isPlatformBrowser(this.platformId)) {
      const token = localStorage.getItem(this.tokenKey); // Retrieve the token from localStorage
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}` // Set the Authorization header with the token
      });

      return this.http.get<JobsListResponseDto>(`${this.apiUrl}job/getalljobs`, { headers });
    } else {
      // Handle the case where localStorage is not available (e.g., during SSR)
      console.warn('LocalStorage is not available on the server.');
      return of({ message: '', data: [] }); // Return an empty observable or handle this case appropriately
    }
  }

  /**
   * Fetches details of a specific job by its ID.
   * 
   * @param jobId - The ID of the job to retrieve details for.
   * @returns Observable of JobDetailsResponseDto containing the job details.
   */
  getJobDetails(jobId: string): Observable<JobDetailsResponseDto> {
    const token = localStorage.getItem(this.tokenKey); // Retrieve the token from localStorage
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Set the Authorization header with the token
    });
    return this.http.get<JobDetailsResponseDto>(`${this.apiUrl}Job/getjobdetails/${jobId}`, { headers });
  }

  /**
   * Fetches a list of properties from the API.
   * 
   * @returns Observable of PropertyListResponseDto containing the list of properties.
   */
  getProperties(): Observable<PropertyListResponseDto> {
    const token = localStorage.getItem(this.tokenKey); // Retrieve the token from localStorage
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Set the Authorization header with the token
    });
    return this.http.get<PropertyListResponseDto>(`${this.apiUrl}property/getpropertydetails`, { headers });
  }

  /**
   * Fetches a list of job types from the API.
   * 
   * @returns Observable of JobTypeListResponseDto containing the list of job types.
   */
  getJobTypes(): Observable<JobTypeListResponseDto> {
    const token = localStorage.getItem(this.tokenKey); // Retrieve the token from localStorage
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Set the Authorization header with the token
    });
    return this.http.get<JobTypeListResponseDto>(`${this.apiUrl}job/getjobtypes`, { headers });
  }

  /**
   * Fetches a list of service providers based on the selected job type.
   * 
   * @param jobTypeId - The ID of the job type for which to retrieve service providers.
   * @returns Observable of ServiceProviderResponseDto containing the list of service providers.
   */
  getServiceProviders(jobTypeId: string): Observable<ServiceProviderResponseDto> {
    const token = localStorage.getItem(this.tokenKey); // Retrieve the token from localStorage
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Set the Authorization header with the token
    });
    return this.http.get<ServiceProviderResponseDto>(`${this.apiUrl}serviceprovider/getserviceproviderdetails?jobType=${jobTypeId}`, { headers });
  }

  /**
   * Creates a new job with the provided request data.
   * 
   * @param request - The data for the new job to be created.
   * @returns Observable of CreateJobResponseDto containing the result of the creation operation.
   */
  createJob(request: CreateJobRequestDto): Observable<CreateJobResponseDto> {
    const token = localStorage.getItem(this.tokenKey); // Retrieve the token from localStorage
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`, // Set the Authorization header with the token
      'Content-Type': 'application/json' // Set content type to JSON
    });

    return this.http.post<CreateJobResponseDto>(`${this.apiUrl}Job/createnewjob`, request, { headers });
  }
}
