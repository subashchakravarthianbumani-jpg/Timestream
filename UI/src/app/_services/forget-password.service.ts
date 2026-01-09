import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, of, timer } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ResponseModel } from '../_models/ResponseStatus';

@Injectable({ providedIn: 'root' })
export class ForgetPasswordService {
  constructor(private router: Router, private http: HttpClient) {}

  sendOTP(mobileNumber: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Account/SendOtp?MobileNumber=${mobileNumber}`
    );
  }

  vaerifyOtp(mobileNumber: string, otp: string) {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const options = { headers };
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Account/ValidateOtp`,
      {
        mobileNumber: mobileNumber,
        oTP: otp,
      },
      options
    );
  }

  savePasword(token: string, otp: string, password: string) {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const options = { headers };
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Account/SaveNewPassword`,
      {
        token: token,
        oTP: otp,
        password: password,
      },
      options
    );
  }

  getCounter(tick: number) {
    return timer(0, tick);
  }
}
