import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { QuickLinkModel } from '../_models/configuration/quickLink';
import { httpOptions } from '../_models/utils';
import { ResponseModel } from '../_models/ResponseStatus';
import { TableFilterModel } from '../_models/filterRequest';

@Injectable({ providedIn: 'root' })
export class LogHistoryService {
  constructor(private router: Router, private http: HttpClient) {}
  getlogs(model: TableFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Common/Record_History_Get`,
      model,
      httpOptions
    );
  }
  getEmailSmsLog(model: TableFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Common/Email_SMS_Log_Get`,
      model,
      httpOptions
    );
  }
}
