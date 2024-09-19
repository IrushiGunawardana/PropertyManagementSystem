import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './components/navbar/navbar.component';
import { HomeComponent } from './components/home/home.component';
import { UserLoginComponent } from './components/user-login/user-login.component';

import { NZ_I18N, en_US } from 'ng-zorro-antd/i18n';
import { JobPostWizardsJobDetailsComponent } from './components/job-post-wizards-job-details/job-post-wizards-job-details.component';
import { JobPostWizardsServiceProviderComponent } from './components/job-post-wizards-service-provider/job-post-wizards-service-provider.component';
import { RegisterComponent } from './components/user-register/user-register.component';

import { CookieService } from 'ngx-cookie-service';


@Component({
  selector: 'app-root',
  standalone: true,
  providers: [
    { provide: NZ_I18N, useValue: en_US }

  ],
  imports: [
    RouterOutlet,
    NavbarComponent ,
    HomeComponent,
    UserLoginComponent,
    RegisterComponent,
    JobPostWizardsJobDetailsComponent,
    JobPostWizardsServiceProviderComponent
    ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'PropertyManagementSystem';

  private cookie_name='';
  private all_cookies:any='';

  constructor(private cookieService:CookieService){

  }
    setCookie(){
      this.cookieService.set('name','PropertyManagementSystem');
    }
     
    deleteCookie(){
      this.cookieService.delete('name');
    }
     
    deleteAll(){
      this.cookieService.deleteAll();
      
    }
     
    ngOnInit(): void {
    this.cookie_name=this.cookieService.get('name');
    this.all_cookies=this.cookieService.getAll();  // get all cookies object
        }
    }