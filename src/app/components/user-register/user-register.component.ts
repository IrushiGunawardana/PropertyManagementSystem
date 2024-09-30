import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ValidationError } from '../../models/validation-error';

// Import Angular Material and CommonModule modules
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AsyncPipe, CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-register',
  standalone: true, // Use standalone component
  templateUrl: './user-register.component.html',
  styleUrls: ['./user-register.component.css'],
  imports: [
    // Add all required modules to the imports array
    MatInputModule,
    ReactiveFormsModule,
    RouterLink,
    MatSelectModule,
    MatIconModule,
    MatSnackBarModule,
    AsyncPipe,
    CommonModule
  ]
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  errors: ValidationError[] = [];
  passwordHide: boolean = true;
  confirmPasswordHide: boolean = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private matSnackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group(
      {
        userName: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        companyName: [''],
        address: ['', Validators.required],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
      },
      { validator: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;
    if (password !== confirmPassword) {
      return { passwordMismatch: true };
    }
    return null;
  }

  register(): void {
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe({
        next: (response: any) => {
          this.matSnackBar.open(response.message, 'Close', {
            duration: 5000,
            horizontalPosition: 'center',
          });
          this.router.navigate(['/login']);
        },
        error: (err: HttpErrorResponse) => {
          if (err.status === 400 && err.error) {
            this.errors = err.error;
            this.matSnackBar.open('Validation error', 'Close', {
              duration: 5000,
              horizontalPosition: 'center',
            });
          }
        },
      });
    } else {
      this.matSnackBar.open('Please correct the form errors', 'Close', {
        duration: 5000,
        horizontalPosition: 'center',
      });
    }
  }
}
