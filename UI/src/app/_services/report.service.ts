import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { QuickLinkModel } from '../_models/configuration/quickLink';
import { httpOptions } from '../_models/utils';
import { ResponseModel } from '../_models/ResponseStatus';
import { DashboardFilterModel } from '../_models/dashboard.model';
import { WorkFilterModel } from '../_models/filterRequest';

@Injectable({ providedIn: 'root' })
export class ReportsService {
  constructor(private router: Router, private http: HttpClient) {}
  getreports(model: WorkFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Report/Work_Get`,
      model,
      httpOptions
    );
  }
  getMilesonereports(model: any) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Report/Milestone_Get`,
      model,
      httpOptions
    );
  }
  getGoreports(model: any) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Report/GO_Get`,
      model,
      httpOptions
    );
  }
  getMbookreports(model: any) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Report/MBook_Get`,
      model,
      httpOptions
    );
  }
  getreportform() {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Report/Alert_Filter_Form`,
      httpOptions
    );
  }
}
