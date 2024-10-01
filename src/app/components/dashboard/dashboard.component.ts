import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatSortModule } from '@angular/material/sort';
import { JobService } from '../../services/job.service';
import { JobDto } from '../../models/job-list-response';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginatorModule } from '@angular/material/paginator';
import { JobDetailsComponent } from '../job-details/job-details.component';
import { AuthService } from '../../services/auth.service';
import { JobPostWizardsJobDetailsComponent } from '../job-post-wizards-job-details/job-post-wizards-job-details.component';
import { Router } from '@angular/router';
import { ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

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
  
  jobs: JobDto[] = [];
  displayedColumns: string[] = ['jobNumber', 'description', 'postedDate'];
  selectedJob: JobDto | null = null;
  dataSource = new MatTableDataSource<JobDto>(this.jobs);

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(private router: Router, private jobService: JobService, private authService: AuthService, private dialog: MatDialog) {}

  ngOnInit(): void {

    if(this.authService.isLoggedIn()){
      this.loadJobs();
    }
    else{
      this.router.navigate(['/login']);
    }

  }

  loadJobs() {
    this.jobService.getJobs().subscribe({
      next: (response) => {
        this.jobs = response.data;
        this.dataSource.data = this.jobs;
        this.dataSource.paginator = this.paginator;
      },
      error: (err) => {
        console.error('Failed to load jobs', err);
      }
    });
  }

  openJobDetailModal(job: JobDto) {
    this.selectedJob = job;

    const url = `/jobdetails/${job.id}`;
    window.open(url, '_self');

    // const dialogRef = this.dialog.open(JobDetailsComponent, {
    //   width: '100px',
    //   data: { job }
    // });

    // dialogRef.afterClosed().subscribe(result => {
    //   this.selectedJob = null;
    // });
  }

  openNewJobModal() {
    // const dialogRef = this.dialog.open(JobPostWizardsJobDetailsComponent, {
    //   width: '1000px',
    // });

    // dialogRef.afterClosed().subscribe(result => {
    //   if (result === 'submitted') {
    //     this.loadJobs();
    //   }
    // });
    const url = `/jobpostdetails`;
    window.open(url, '_self');
  }
}
