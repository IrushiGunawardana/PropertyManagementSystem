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

@Component({
  selector: 'app-user-register',
  standalone: true,
  imports: [
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatSnackBarModule,
    ReactiveFormsModule,
    RouterLink,
    AsyncPipe,
    CommonModule
  ],
  templateUrl: './user-register.component.html',
  styleUrls: ['./user-register.component.css']
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  errors: ValidationError[] = [];
  passwordHide: boolean = true;
  confirmPasswordHide: boolean = true;

  authService = inject(AuthService);
  router = inject(Router);
  matSnackBar = inject(MatSnackBar);
  fb = inject(FormBuilder);

  ngOnInit(): void {
    this.registerForm = this.fb.group(
      {
        userName: ['', [Validators.required, Validators.minLength(3)]],
        email: ['', [Validators.required, Validators.email]],
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        companyName: [''],
        role: ['Property Manager'], // Set default role
        address: ['', Validators.required],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
      },
      { validators: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;
    return password !== confirmPassword ? { passwordMismatch: true } : null;
  }

  register(): void {
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe({
        next: (response) => {
          this.matSnackBar.open(response.message, 'Close', {
            duration: 5000,
            horizontalPosition: 'center',
          });
          this.router.navigate(['/login']);
        },
        error: (err: HttpErrorResponse) => {
          if (err.status === 400) {
            this.errors = err.error; // Assuming the server returns an array of errors
            this.matSnackBar.open('Validation error', 'Close', {
              duration: 5000,
              horizontalPosition: 'center',
            });
          } else {
            this.matSnackBar.open('An error occurred', 'Close', {
              duration: 5000,
              horizontalPosition: 'center',
            });
          }
        },
        complete: () => console.log('Register success'),
      });
    } else {
      this.matSnackBar.open('Please correct the form errors', 'Close', {
        duration: 5000,
        horizontalPosition: 'center',
      });
    }
  }
}
