import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { QuickLinkModel } from '../_models/configuration/quickLink';
import { httpOptions } from '../_models/utils';
import { ResponseModel } from '../_models/ResponseStatus';
import {
  DashboardCameraCountModel,
  DashboardFilterModel,
} from '../_models/dashboard.model';
import { AlertFilterModel } from '../_models/filterRequest';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  constructor(private router: Router, private http: HttpClient) {}
  getDashboardRecordCount(model: DashboardFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Dashboard/Dashboard_RecordCount_Get`,
      model,
      httpOptions
    );
  }
  getDashboardCameraStatusCount(model: DashboardCameraCountModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Dashboard/DashboardcameraCount`,
      model,
      httpOptions
    );
  }
  getDashboardTenderChart(model: DashboardFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Dashboard/Dashboard_Tender_Chart`,
      model,
      httpOptions
    );
  }
  getDashboardMbookChart(model: DashboardFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Dashboard/Dashboard_Mbook_Chart`,
      model,
      httpOptions
    );
  }
  getAlerts(model: AlertFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Dashboard/Alert_Get`,
      model,
      httpOptions
    );
  }

  getDashboard_Count(model: any) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Dashboard/Dashboard_Division_Count_Get`,
      model,
      httpOptions
    );
  }

  getDashboard_Mbook_Count(model: any) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Dashboard/Dashboard_Mbook_Count_Get`,
      model,
      httpOptions
    );
  }
  getDashboard_Mbookdivision_Count(model: any) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Dashboard/GetDivision_Mbook_Count`,
      model,
      httpOptions
    );
  }

  getDashboard_Scheme_Count(model: any) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Dashboard/Dashboard_Scheme_Count_Get`,
      model,
      httpOptions
    );
  }

  getDashboard_district_Count(model: any) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Dashboard/Dashboard_Division_district_Count_Get`,
      model,
      httpOptions
    );
  }

  alert_Resolve(model: any) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Dashboard/Alert_Resolve`,
      model,
      httpOptions
    );
  }
}
