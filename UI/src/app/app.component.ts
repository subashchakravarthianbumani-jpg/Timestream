import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { NgxPermissionsService } from 'ngx-permissions';
import { PrimeNGConfig } from 'primeng/api';
import { UserModel } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  constructor(
    private primengConfig: PrimeNGConfig,
    private permissionsService: NgxPermissionsService,
    private http: HttpClient,
    private cookieService: CookieService
  ) {}

  ngOnInit() {
    this.primengConfig.ripple = true;
    if (this.cookieService.get('privillage')) {
      const privillage: any = this.cookieService.get('privillage');

      if (privillage && privillage) {
        this.permissionsService.loadPermissions(privillage.split(','));
      }
    }

    //  this.http.get('url').subscribe((permissions) => {
    //    //const perm = ["ADMIN", "EDITOR"]; example of permissions
    //    this.permissionsService.loadPermissions(permissions);
    // })
  }
}
