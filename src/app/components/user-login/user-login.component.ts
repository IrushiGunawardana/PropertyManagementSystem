import { Component, inject, OnInit } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-login',
  standalone: true,
  imports: [
    MatInputModule,
    RouterLink,
    MatSnackBarModule,
    MatIconModule,
    ReactiveFormsModule,
    RouterLink,
  ],
  templateUrl: './user-login.component.html',
  styleUrls: ['./user-login.component.css']
})
export class UserLoginComponent implements OnInit {
  authService = inject(AuthService);
  router = inject(Router);
  hide = true;
  form!: FormGroup;
  fb = inject(FormBuilder);

  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', [Validators.required]],  
      password: ['', Validators.required],
    });
  }

  login() {
    this.authService.login(this.form.value).subscribe({
      next: (response) => {
        if (response.token) {
          localStorage.setItem('token', response.token);
          console.log('User logged in successfully:', response);

          this.router.navigate(['/dashboard']); 
        } else {
          console.log('Login failed: Token not received.');
        }
      },
      error: (err) => {
        console.error('Login error:', err);
      }
    });
  }
}
