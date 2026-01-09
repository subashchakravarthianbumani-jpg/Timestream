import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IConfigCategoryModel } from '../_models/configuration/configuration';
import { ResponseModel, SuccessStatus } from '../_models/ResponseStatus';
import { CookieService } from 'ngx-cookie-service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(
    private router: Router,
    private http: HttpClient,
    private cookieService: CookieService
  ) {}
  authenticate() {
    return this.http.get<ResponseModel>(`${environment.apiUrl}/Account/Login`);
  }
  login(username: string, password: string) {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const options = { headers };
    return this.http.post(
      environment.apiUrl,
      {
        username,
        password,
      },
      options
    );
  }

  private authToken: string | null = null;
  private refreshToken: string | null = null;

  setToken(authToken: string) {
    this.authToken = authToken;
  }

  setRefreshToken(refreshToken: string) {
    this.refreshToken = refreshToken;
  }

  getToken(): string | null {
    this.authToken = this.cookieService.get('token');
    return this.authToken;
  }

  getRefreshToken(): string | null {
    this.refreshToken = this.cookieService.get('refreshToken');
    return this.refreshToken;
  }

  isAuthenticated(): boolean {
    return !!this.authToken;
  }

  isRefreshToken(): boolean {
    return !!this.refreshToken;
  }

  refreshTokens(): Observable<any> {
    const httpOptions = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + this.getRefreshToken(),
      }),
    };

    return this.http.post('/api2/auth/refresh', {}, httpOptions).pipe(
      tap((tokens: any) => {
        this.cookieService.set('token', tokens.access_token);
        this.cookieService.set('refreshToken', tokens.refresh_token);
        this.setToken(tokens.access_token);
        this.setRefreshToken(tokens.refresh_token);
        
      })
    );
  }
}
