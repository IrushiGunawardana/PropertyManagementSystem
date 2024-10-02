import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { UserLoginComponent } from './components/user-login/user-login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { JobPostWizardsJobDetailsComponent } from './components/job-post-wizards-job-details/job-post-wizards-job-details.component';
import { JobPostWizardsServiceProviderComponent } from './components/job-post-wizards-service-provider/job-post-wizards-service-provider.component';
import { JobDetailsComponent } from './components/job-details/job-details.component';
import { RegisterComponent } from './components/user-register/user-register.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard],
  },
  {
    path: 'login',
    component: UserLoginComponent,

  },
  {
    path: 'register',
    component: RegisterComponent,

  },

  {
    path: 'jobpostdetails',
    component: JobPostWizardsJobDetailsComponent,
    canActivate: [authGuard],
  },
  {
    path: 'serviceproviderdetails',
    component: JobPostWizardsServiceProviderComponent,
    canActivate: [authGuard],
  },
  {
    path: 'jobdetails/:id',
    component: JobDetailsComponent,
    canActivate: [authGuard],
  },
  {
    path: '',
    component: HomeComponent,
  }

];
