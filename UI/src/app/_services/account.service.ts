import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { UserModel } from '../_models/user';
import { ResponseModel, SuccessStatus } from '../_models/ResponseStatus';
import { CookieService } from 'ngx-cookie-service';

@Injectable({ providedIn: 'root' })
export class AccountService {
  // private userSubject: BehaviorSubject<UserModel | null>;
  // public user: Observable<UserModel | null>;

  constructor(
    private router: Router,
    private http: HttpClient,
    private cookieService: CookieService
  ) {
    // this.userSubject = new BehaviorSubject(
    //   JSON.parse(this.cookieService.get('user')!)
    // );
    // this.user = this.userSubject.asObservable();
  }

  public get userValue() {
    if (this.cookieService.check('user')) {
      return JSON.parse(this.cookieService.get('user'));
    } else null;
  }

  login(username: string, password: string) {
    return this.http
      .post<ResponseModel>(`${environment.apiUrl}/Account/Login`, {
        username,
        password,
      })
      .pipe(
        map((user: ResponseModel) => {
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          if (user.status === SuccessStatus) {
            let userdetails = user.data;
            let privillage = userdetails.privillage;
            let accessToken: string = userdetails.accessToken;
            let refreshToken = userdetails.refreshToken;
            userdetails.privillage = null;
            userdetails.accessToken = null;
            userdetails.refreshToken = null;
            accessToken = accessToken.replace('Bearer ', '');
            this.cookieService.set('privillage', privillage, 1);
            this.cookieService.set('user', JSON.stringify(userdetails), 1);
            this.cookieService.set('token', accessToken, 1);
            this.cookieService.set('refreshToken', refreshToken, 1);
            //this.userSubject.next(user.data);
          }
          return user;
        })
      );
  }

  logout() {
    // remove user from local storage and set current user to null
    this.cookieService.delete('user');
    this.cookieService.delete('token');
    this.cookieService.delete('privillage');
    this.cookieService.delete('refreshToken');
    localStorage.removeItem('selectedYearOptions');
    // this.userSubject.next(null);
  }
}
