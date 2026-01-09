import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { httpOptions } from '../_models/utils';
import {
  ApprovalFlowAddRoleModel,
  ApprovalFlowModel,
} from '../_models/configuration/approval.flow';
import { ResponseModel } from '../_models/ResponseStatus';

@Injectable({ providedIn: 'root' })
export class ApprovalFlowservice {
  constructor(private router: Router, private http: HttpClient) {}
  getApprovalFlows(DepartmentId: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/ApprovalFlow_Get?DepartmentId=${DepartmentId}`
    );
  }
  getRoles(DepartmentId: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/ApprovalFlow_GetRoleList?DepartmentId=${DepartmentId}`
    );
  }
  saveApprovalFlows(approvalFlows: ApprovalFlowModel[]) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/ApprovalFlow_SaveUpdate
      `,
      approvalFlows,
      httpOptions
    );
  }

  save_roles(roles: ApprovalFlowAddRoleModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/ApprovalFlow_Add_Role
      `,
      roles,
      httpOptions
    );
  }
}
