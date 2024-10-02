import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { PropertyDto } from '../../models/property-list-response';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSelectModule } from '@angular/material/select';
import { MatListModule } from '@angular/material/list';  // Import MatListModule
import { AuthService } from '../../services/auth.service';
import { JobService } from '../../services/job.service';
import { Observable, startWith, map } from 'rxjs';
import { MatOptionSelectionChange } from '@angular/material/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-job-post-wizards-job-details',
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
    MatListModule,  // Include MatListModule here
  ],
  templateUrl: './job-post-wizards-job-details.component.html',
  styleUrls: ['./job-post-wizards-job-details.component.css']
})
export class JobPostWizardsJobDetailsComponent implements OnInit {
  jobDetailsForm: FormGroup;
  filteredProperties$: Observable<PropertyDto[]> | undefined;
  properties: PropertyDto[] = [];
  jobTypes: { id: string; name: string }[] = [];
  owners: { firstName: string; lastName: string }[] = [];
  tenants: { firstName: string; lastName: string }[] = [];

  constructor(private fb: FormBuilder, private jobService: JobService, private authService: AuthService, private router: Router) {
    this.jobDetailsForm = this.fb.group({
      property: ['', Validators.required],
      description: ['', Validators.required],
      type: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.jobService.getProperties().subscribe((response) => {
        this.properties = response.data;

        this.filteredProperties$ = this.jobDetailsForm.get('property')!.valueChanges.pipe(
          startWith(''),
          map(value => this.filterProperties(value || ''))
        );
      });

      this.jobService.getJobTypes().subscribe((response) => {
        this.jobTypes = response.data;
      });
    } else {
      this.router.navigate(['/login']);
    }
  }

  private filterProperties(value: string): PropertyDto[] {
    const filterValue = value.toLowerCase();
    return this.properties.filter(property =>
      property.address.toLowerCase().includes(filterValue)
    );
  }

  displayProperty(property: PropertyDto): string {
    return property && property.address ? property.address : '';
  }

  onPropertySelected(event: MatOptionSelectionChange, selectedProperty: PropertyDto): void {
    if (event.source.selected) {
      console.log(selectedProperty);
      this.owners = selectedProperty.ownersDetails.map(o => ({ firstName: o.firstName, lastName: o.lastName }));
      this.tenants = selectedProperty.tenantsDetails.map(t => ({ firstName: t.firstName, lastName: t.lastName }));
    }
  }

  onSubmit(): void {
    if (this.jobDetailsForm.valid) {
      const propertyId = this.jobDetailsForm.value.property.id;
      const description = this.jobDetailsForm.value.description;
      const jobType = this.jobDetailsForm.value.type;

      console.log(this.jobDetailsForm.value);

      this.router.navigate(['/serviceproviderdetails'], { //singlton object - input output binding -> data pass
        queryParams: {
          propertyId,
          description,
          jobType
        }
      });
    } else {
      // Handle form invalid case
    }
  }
}
