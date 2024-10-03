import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ValidationError } from '../../models/validation-error';
import { Role } from '../../models/role';
import { Observable } from 'rxjs';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { AsyncPipe, CommonModule } from '@angular/common';

/**
 * RegisterComponent is responsible for user registration functionality.
 * It manages the registration form and handles user input and validation.
 */
@Component({
  selector: 'app-user-register',
  standalone: true,
  imports: [
    MatInputModule,      // Material input fields for form inputs
    MatSelectModule,     // Material select dropdown for role selection
    MatIconModule,       // Material icons for visual elements
    MatSnackBarModule,   // Material Snackbar for displaying messages
    ReactiveFormsModule,  // Reactive forms module for form handling
    RouterLink,          // Angular RouterLink directive for navigation
    AsyncPipe,           // Async pipe for handling observables in templates
    CommonModule         // Common directives and pipes
  ],
  templateUrl: './user-register.component.html', // Template URL for the component's HTML
  styleUrls: ['./user-register.component.css']    // Styles URL for the component's CSS
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;              // FormGroup instance for registration form
  errors: ValidationError[] = [];         // Array to hold validation errors
  passwordHide: boolean = true;           // Flag to toggle password visibility
  confirmPasswordHide: boolean = true;    // Flag to toggle confirm password visibility

  // Dependency injection for required services
  authService = inject(AuthService);      // Service for handling authentication
  router = inject(Router);                 // Angular router for navigation
  matSnackBar = inject(MatSnackBar);      // Service for displaying snack bar notifications
  fb = inject(FormBuilder);                // FormBuilder service for creating reactive forms

  ngOnInit(): void {
    // Initialize the registration form with validation rules
    this.registerForm = this.fb.group(
      {
        userName: ['', [Validators.required, Validators.minLength(3)]], // Username must be at least 3 characters long
        email: ['', [Validators.required, Validators.email]],           // Email must be valid format
        firstName: ['', Validators.required],                            // First name is required
        lastName: ['', Validators.required],                             // Last name is required
        companyName: [''],                                              // Optional company name
        role: ['Property Manager'],                                     // Default role selection
        address: ['', Validators.required],                             // Address is required
        password: ['', [Validators.required, Validators.minLength(6)]], // Password must be at least 6 characters long
        confirmPassword: ['', Validators.required],                     // Confirm password is required
      },
      { validators: this.passwordMatchValidator }                     // Custom validator to check password match
    );
  }

  /**
   * Custom validator to check if the password and confirm password fields match.
   * 
   * @param control - AbstractControl instance of the form group
   * @returns An object indicating a mismatch error or null if no error
   */
  passwordMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('password')?.value;                 // Get password value
    const confirmPassword = control.get('confirmPassword')?.value;   // Get confirm password value
    return password !== confirmPassword ? { passwordMismatch: true } : null; // Return error if they do not match
  }

  /**
   * Handles the user registration process.
   * If the form is valid, it submits the registration request.
   */
  register(): void {
    if (this.registerForm.valid) {
      // If the form is valid, call the register method from AuthService
      this.authService.register(this.registerForm.value).subscribe({
        next: (response) => {
          // Show success message upon successful registration
          this.matSnackBar.open(response.message, 'Close', {
            duration: 5000,                 // Duration for which the snackbar will be displayed
            horizontalPosition: 'center',    // Snackbar positioning
          });
          this.router.navigate(['/login']); // Navigate to login page after registration
        },
        error: (err: HttpErrorResponse) => {
          // Handle errors based on response status
          if (err.status === 400) {
            this.errors = err.error; // Capture validation errors
            this.matSnackBar.open('Validation error', 'Close', {
              duration: 5000,
              horizontalPosition: 'center',
            });
          } else {
            // Handle generic error
            this.matSnackBar.open('An error occurred', 'Close', {
              duration: 5000,
              horizontalPosition: 'center',
            });
          }
        },
        complete: () => console.log('Register success'), 
      });
    } else {
      // Show error message if the form is invalid
      this.matSnackBar.open('Please correct the form errors', 'Close', {
        duration: 5000,
        horizontalPosition: 'center',
      });
    }
  }
}
