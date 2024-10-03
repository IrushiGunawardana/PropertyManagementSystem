import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatSortModule } from '@angular/material/sort';
import { JobService } from '../../services/job.service';
import { JobDto } from '../../models/job-list-response';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginatorModule } from '@angular/material/paginator';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

/**
 * DashboardComponent is responsible for displaying the job dashboard where users can view,
 * manage, and post maintenance jobs.
 */
@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatSortModule,
    MatPaginatorModule,
  ]
})
export class DashboardComponent implements OnInit {

  jobs: JobDto[] = []; // Array to hold the list of jobs
  displayedColumns: string[] = ['jobNumber', 'description', 'postedDate']; // Columns to display in the job table
  selectedJob: JobDto | null = null; // Holds the selected job for detail viewing
  dataSource = new MatTableDataSource<JobDto>(this.jobs); // Data source for the job table

  @ViewChild(MatPaginator) paginator!: MatPaginator; // Paginator for the job table

  /**
   * Constructs the DashboardComponent.
   * 
   * @param router - Angular router for navigation.
   * @param jobService - Service for job-related operations.
   * @param authService - Service for authentication-related operations.
   * 
   */
  constructor(private router: Router, private jobService: JobService, private authService: AuthService ) { }

  /**
   * Lifecycle hook that is called after the component is initialized.
   * Checks if the user is logged in and loads jobs if authenticated.
   */
  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.loadJobs(); // Load jobs if the user is logged in
    } else {
      this.router.navigate(['/login']); // Redirect to login if not authenticated
    }
  }

  /**
   * Loads jobs from the job service and populates the data source for the table.
   * Handles errors by logging them to the console.
   * 
   * @returns void
   */
  loadJobs() {
    this.jobService.getJobs().subscribe({
      next: (response) => {
        this.jobs = response.data; // Assign the retrieved job data to the jobs array
        this.dataSource.data = this.jobs; // Update the data source with the jobs array
        this.dataSource.paginator = this.paginator; // Assign paginator to the data source
      },
      error: (err) => {
        console.error('Failed to load jobs', err); // Log any errors encountered during loading
      }
    });
  }

  /**
   * Opens the job detail modal for a selected job by navigating to the job details route.
   * 
   * @param job - The job object to display details for.
   * @returns void
   */
  openJobDetailModal(job: JobDto) {
    this.selectedJob = job; // Set the selected job

    const url = `/jobdetails/${job.id}`; // Construct the URL for job details
    window.open(url, '_self'); // Navigate to the job details page
  }

  /**
   * Opens a new job posting modal by navigating to the job post wizard route.
   * 
   * @returns void
   */
  openNewJobModal() {
    const url = `/jobpostdetails`; // Construct the URL for posting a new job
    window.open(url, '_self'); // Navigate to the job posting page
  }
}
