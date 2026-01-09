import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { QuickLinkModel } from '../_models/configuration/quickLink';
import { httpOptions } from '../_models/utils';
import { ResponseModel } from '../_models/ResponseStatus';
import { AlertConfigurationPrimaryModel } from '../_models/alert.model';

@Injectable({ providedIn: 'root' })
export class AlertConfigService {
  constructor(private router: Router, private http: HttpClient) {}
  getAlertconfigForm() {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Alert_Configuration_Form`
    );
  }
  getAlertconfig(
    Id: string = '',
    Type: string = '',
    Object: string = '',
    IsActive: boolean = true,
    Department: string = ''
  ) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Alert_Primary_Get?Id=${Id}&Type=${Type}&Object=${Object}&IsActive=${IsActive}&Department=${Department}`
    );
  }
  getAlertSecodaryconfig(primaryId: string = '', IsActive: boolean = true) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Alert_Secondary_Get/?PrimaryId=${primaryId}&IsActive=${IsActive}`
    );
  }
  saveAlertCongig(model: any) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/AlertPrimary_Config_SaveUpdate`,
      model,
      httpOptions
    );
  }
}
