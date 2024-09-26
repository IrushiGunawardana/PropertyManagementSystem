import { Component, OnInit, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RoleService } from '../../services/role.service';
import { Observable } from 'rxjs';

import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { AsyncPipe, CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { ValidationError } from '../../models/validation-error';
import { Role } from '../../models/role';

@Component({
  selector: 'app-user-register',
  standalone: true,
  imports: [
    MatInputModule,
    ReactiveFormsModule,
    RouterLink,
    MatSelectModule,
    MatIconModule,
    MatSnackBarModule,
    AsyncPipe,
    CommonModule,
  ],
  templateUrl: './user-register.component.html',
  styleUrls: ['./user-register.component.css'],
})
export class RegisterComponent implements OnInit {
  roleService = inject(RoleService);
  authService = inject(AuthService);
  matSnackbar = inject(MatSnackBar);
  roles$!: Observable<Role[]>;
  fb = inject(FormBuilder);
  registerForm!: FormGroup;
  router = inject(Router);
  confirmPasswordHide: boolean = true;
  passwordHide: boolean = true;
  errors!: ValidationError[];

  register() {
    this.authService.register(this.registerForm.value).subscribe({
      next: (response) => {
        console.log(response);
        this.matSnackbar.open(response.message, 'Close', {
          duration: 5000,
          horizontalPosition: 'center',
        });
        this.router.navigate(['/login']);
      },
      error: (err: HttpErrorResponse) => {
        if (err!.status === 400) {
          this.errors = err!.error;
          this.matSnackbar.open('Validation error', 'Close', {
            duration: 5000,
            horizontalPosition: 'center',
          });
        }
      },
      complete: () => console.log('Register success'),
    });
  }

  ngOnInit(): void {
    this.registerForm = this.fb.group(
      {
        userName: ['', Validators.required],
        password: ['', Validators.required],
        confirmPassword: ['', Validators.required],
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        role: [''],
        companyName: [''],
        address: ['', Validators.required],
      },
      {
        validator: this.passwordMatchValidator,
      }
    );

    this.roles$ = this.roleService.getRoles();
  }

  private passwordMatchValidator(
    control: AbstractControl
  ): { [key: string]: boolean } | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      return { passwordMismatch: true };
    }

    return null;
  }
}
