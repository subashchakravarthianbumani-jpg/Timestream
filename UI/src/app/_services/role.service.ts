import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { IRoleModel } from '../_models/configuration/role';
import { httpOptions } from '../_models/utils';
import { AccountPrivilegeSaveViewModel } from '../_models/configuration/privilege';
import { ResponseModel } from '../_models/ResponseStatus';

@Injectable({ providedIn: 'root' })
export class RoleService {
  constructor(private router: Router, private http: HttpClient) {}
  getRoles(isActive: boolean) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Role_Get?IsActive=${isActive}`
    );
  }
  saveRole(role: IRoleModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/Role_SaveUpdate`,
      role,
      httpOptions
    );
  }

  getPrivileges(roleid: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Role_Privilege_Get?RoleId=${roleid}`
    );
  }
  savePrivileges(privilege: AccountPrivilegeSaveViewModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/Role_Privilege_Save`,
      privilege,
      httpOptions
    );
  }
}
