import { Component, inject, OnInit } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { JobService } from '../../services/job.service';
import { JobDetailsDto } from '../../models/job-entity-response';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';

@Component({
  selector: 'app-job-details',
  standalone: true,
  imports: [
    MatInputModule,
    ReactiveFormsModule,
  ],
  templateUrl: './job-details.component.html',
})
export class JobDetailsComponent implements OnInit {

  form!: FormGroup;
  fb = inject(FormBuilder);
  jobs: JobDetailsDto | null = null;
  jobService = inject(JobService);
  authService = inject(AuthService);
  route = inject(ActivatedRoute);
  router = inject(Router);

  ngOnInit(): void {
    this.form = this.fb.group({
      property: [''],
      owner: [''],
      tenant: [''],
      description: [''],
      type: [''],
      provider: ['']
    });

    if (this.authService.isLoggedIn()) {
      this.route.paramMap.subscribe(params => {
        const jobId = params.get('id');
        if (jobId) {
          this.loadJobDetails(jobId);
        }
      });
    } else {
      this.router.navigate(['/login']);
    }
  }

  loadJobDetails(jobId: string) {
    this.jobService.getJobDetails(jobId).subscribe({
      next: (response) => {
        this.jobs = response.data;
        this.form.setValue({
          property: this.jobs.property.address,
          owner: this.jobs.ownerDetails.map(owner => `${owner.firstName} ${owner.lastName}`).join('\n'),
          tenant: this.jobs.tenantDetails.map(tenant => `${tenant.firstName} ${tenant.lastName}`).join('\n'),
          description: this.jobs.description,
          type: this.jobs.jobType.name,
          provider: this.jobs.provider.companyName
        });
      },
      error: (err) => {
        console.error('Failed to load jobs', err);
      }
    });
  }

  back() {
    this.router.navigate(['/dashboard']);
  }
}
