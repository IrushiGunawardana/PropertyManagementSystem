import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCardModule } from '@angular/material/card';
import { ActivatedRoute } from '@angular/router';
import { JobService } from '../../services/job.service'; 
import { ServiceProviderDto } from '../../models/service-provider-response';
import { Router } from '@angular/router';
import { Observable, startWith, map } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { CreateJobRequestDto } from '../../models/create-job-request';
import { NzNotificationService } from 'ng-zorro-antd/notification';


@Component({
  selector: 'app-job-post-wizards-service-provider',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatAutocompleteModule,
    MatSelectModule,
  ],
  templateUrl: './job-post-wizards-service-provider.component.html',
  styleUrls: ['./job-post-wizards-service-provider.component.css']
})
export class JobPostWizardsServiceProviderComponent implements OnInit{
  serviceProviderForm: FormGroup;
  filteredServiceProviders$: Observable<ServiceProviderDto[]> | undefined;
  propertyId: string | null = null;
  description: string | null = null;
  jobType: string | null = null;
  serviceProviders: ServiceProviderDto[] = [];


  constructor(
    private fb: FormBuilder, 
    private route: ActivatedRoute,  
    private jobService: JobService, 
    private authService: AuthService, 
    private router: Router,
    private notification: NzNotificationService) {

    this.serviceProviderForm = this.fb.group({
      serviceProvider: ['', Validators.required],
      selectedServiceProvider: [''],
      companyName: [''],
      email: ['']
    });
  }

  ngOnInit(): void {

    if(this.authService.isLoggedIn()){

      this.route.queryParams.subscribe(params => {
        this.propertyId = params['propertyId'] || null;
        this.description = params['description'] || null;
        this.jobType = params['jobType'] || null; 
      });

      if(this.jobType !=null) {

        this.jobService.getServiceProviders(this.jobType).subscribe((response) => {
          this.serviceProviders = response.data;

          this.filteredServiceProviders$ = this.serviceProviderForm.get('serviceProvider')!.valueChanges.pipe(
            startWith(''),
            map(value => this.filterServiceProviders(value || ''))
          );

        });
      }
    }
    else{
      this.router.navigate(['/login']);
    }
  }
  onSubmit() {
    
    const jobRequest: CreateJobRequestDto = {
      PropertyId: this.propertyId ? this.propertyId : "",
      Description: this.description ? this.description : "",
      Type: this.jobType ? this.jobType: "",
      ServiceProviderId: this.serviceProviderForm.value.serviceProvider.id
    };

    this.jobService.createJob(jobRequest).subscribe(response => {
      this.OpenSuccessNotification(response.data.jobNumber.toString());
    }, error => {
      this.OpenFailedNotification(error);
    });

  }

  displayServiceProvider(serviceProvider: ServiceProviderDto): string {
    return serviceProvider && serviceProvider.companyName ? serviceProvider.companyName : '';
  }

  private filterServiceProviders(value: string): ServiceProviderDto[] {
    const filterValue = value.toLowerCase();
    return this.serviceProviders.filter(property =>
      property.companyName.toLowerCase().includes(filterValue)
    );
  }

  onServiceProviderSelected(event: MatOptionSelectionChange, selectedServiceprovider: ServiceProviderDto): void {
    if (event.source.selected) {
      console.log(selectedServiceprovider);

      this.serviceProviderForm.patchValue({
        companyName: selectedServiceprovider.companyName,
        email: selectedServiceprovider.email
      });
      //
      
    }
  }

  OpenSuccessNotification(JobNumber: string): void {
    this.notification
      .success(
        'Job Posted Successfully !',
        `Requested Job has been created successfully. Note this is your job number : ${JobNumber} \n Redirecting to dashboard within 3 seconds.`
      )
      .onClick.subscribe(() => {
        console.log('notification clicked!');//delete console.logs
      });
      this.redirectAfterDelay();
      
  }

  OpenFailedNotification(error: string): void {
    this.notification
      .error(
        'Job Posting failed !',
        `Requested Job posting unsucessfull. \n Redirecting to dashboard within 3 seconds. \nError : ${error}`
      )
      .onClick.subscribe(() => {
        console.log('notification clicked!');
      });
      this.redirectAfterDelay();
      
  }

  redirectAfterDelay(): void {
    setTimeout(() => {
      this.router.navigate(['/dashboard']);
    }, 3000);
  }

}
