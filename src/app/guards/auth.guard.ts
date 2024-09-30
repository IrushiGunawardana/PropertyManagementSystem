import { CanActivateFn } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const matSnackbar = inject(MatSnackBar);

  if (inject(AuthService).isLoggedIn()) {
    return true;
  }

  matSnackbar.open('You must be logged in to view this page', 'Ok', {
    duration: 3000,
  });
  inject(Router).navigate(['/login']);
  return false;
};
