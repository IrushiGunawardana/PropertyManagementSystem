import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { MatMenuModule } from '@angular/material/menu';
import { CommonModule } from '@angular/common';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../services/auth.service';

/**
 * NavbarComponent is responsible for rendering the navigation bar of the application.
 * It includes login/logout functionality and provides access to different routes.
 */
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    MatToolbarModule, // Material toolbar for the top navigation bar
    RouterLink,       // Angular RouterLink directive for navigation
    MatMenuModule,    // Material menu for dropdown options
    MatButtonModule,  // Material button for actions
    MatSnackBarModule, // Material Snackbar for notifications
    MatIconModule,    // Material icons for visual elements
    CommonModule,     // Common directives and pipes
  ],
  templateUrl: './navbar.component.html', // Template URL for the component's HTML
  styleUrl: './navbar.component.css',      // Styles URL for the component's CSS
})
export class NavbarComponent {
  
  // Dependency injection for required services
  authService = inject(AuthService); // Service for handling authentication
  matSnackBar = inject(MatSnackBar);  // Service for displaying snack bar notifications
  router = inject(Router);             // Angular router for navigation

  /**
   * Checks if the user is logged in.
   * 
   * @returns true if the user is logged in, otherwise false.
   */
  isLoggedIn() {
    return this.authService.isLoggedIn(); // Calls the AuthService to check login status
  }

  /**
   * Logs the user out of the application.
   * Displays a snack bar message upon successful logout and navigates to the login page.
   */
  logout = () => {
    this.authService.logout(); // Calls the AuthService to log out the user
    this.matSnackBar.open('Logout success', 'Close', {
      duration: 5000,         // Duration for which the snack bar will be displayed
      horizontalPosition: 'center', // Positioning of the snack bar
    });
    this.router.navigate(['/login']); // Navigates to the login page
  };
}
