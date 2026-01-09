import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { httpOptions } from '../_models/utils';
import { AccountPrivilegeSaveViewModel } from '../_models/configuration/privilege';
import { AccountUserViewModel } from '../_models/user/usermodel';
import { ResponseModel } from '../_models/ResponseStatus';
import { TableFilterModel } from '../_models/filterRequest';

@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private router: Router, private http: HttpClient) {}
  getUsers(IsActive: boolean, UserId?: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/User_Get?IsActive=${IsActive}&UserId=${UserId}`
    );
  }
  getUsersList(filtermodel: TableFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/User_GetList`,
      filtermodel,
      httpOptions
    );
  }
  getUserForm() {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/User_Form_Get`
    );
  }
  saveUser(user: AccountUserViewModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/User_SaveUpdate`,
      user,
      httpOptions
    );
  }
  getKeyContacts() {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Common/KeyContacts`
    );
  }
}
