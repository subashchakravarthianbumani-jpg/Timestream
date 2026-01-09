import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import {
  IConfigCategoryModel,
  IConfigurationModel,
} from '../_models/configuration/configuration';
import { httpOptions } from '../_models/utils';
import { ResponseModel } from '../_models/ResponseStatus';

@Injectable({ providedIn: 'root' })
export class TwoColConfigService {
  constructor(private router: Router, private http: HttpClient) {}
  getAllCategory() {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Configuration_Category_Get`
    );
  }
  getConfigurationDetailsbyId(
    departmentId: string,
    configId?: string,
    categoryId?: string,
    parentConfigId?: string,
    isActive: boolean = true
  ) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Configuration_Get?ConfigurationId=${configId}&CategoryId=${categoryId}&ParentConfigurationId=${parentConfigId}&IsActive=${isActive}&DepartmentId=${departmentId}`
    );
  }
  saveConfiguration(configDetails: IConfigurationModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/Configuration_SaveUpdate`,
      configDetails,
      httpOptions
    );
  }
  getAllDepartments() {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Configuration_Department_Get`
    );
  }
  getAllDivisions() {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Configuration_Division_Get`
    );
  }
}
