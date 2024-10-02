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

@Injectable({
  providedIn: 'root'
})
export class JobService {

  apiUrl: string = environment.apiUrl;
  private tokenKey = 'token';

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object // Inject platformId to check if it's a browser
  ) { }

  getJobs(): Observable<JobsListResponseDto> {
    if (isPlatformBrowser(this.platformId)) {

      const token = localStorage.getItem(this.tokenKey);
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`
      });

      return this.http.get<JobsListResponseDto>(`${this.apiUrl}job/getalljobs`, { headers });
    } else {
      // Handle the case where localStorage is not available (e.g., during SSR)
      console.warn('LocalStorage is not available on the server.');
      return of({ message: '', data: [] }); // Return an empty observable or handle this case appropriately
    }
  }


  getJobDetails(jobId: string): Observable<JobDetailsResponseDto> {
    const token = localStorage.getItem(this.tokenKey);
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.get<JobDetailsResponseDto>(`${this.apiUrl}Job/getjobdetails/${jobId}`, { headers });
  }

  getProperties(): Observable<PropertyListResponseDto> {
    const token = localStorage.getItem(this.tokenKey);
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.get<PropertyListResponseDto>(`${this.apiUrl}property/getpropertydetails`, { headers });
  }

  getJobTypes(): Observable<JobTypeListResponseDto> {
    const token = localStorage.getItem(this.tokenKey);
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.get<JobTypeListResponseDto>(`${this.apiUrl}job/getjobtypes`, { headers });
  }

  getServiceProviders(jobTypeId: string): Observable<ServiceProviderResponseDto> {
    const token = localStorage.getItem(this.tokenKey);
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.get<ServiceProviderResponseDto>(`${this.apiUrl}serviceprovider/getserviceproviderdetails?jobType=${jobTypeId}`, { headers });
  }

  createJob(request: CreateJobRequestDto): Observable<CreateJobResponseDto> {
    const token = localStorage.getItem(this.tokenKey);
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    return this.http.post<CreateJobResponseDto>(`${this.apiUrl}Job/createnewjob`, request, { headers });
  }




}
