import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { QuickLinkModel } from '../_models/configuration/quickLink';
import { httpOptions } from '../_models/utils';
import { ResponseModel } from '../_models/ResponseStatus';

@Injectable({ providedIn: 'root' })
export class QuickLinkService {
  constructor(private router: Router, private http: HttpClient) {}
  getQuickLinks(isActive: boolean) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/QuickLink_Get?IsActive=${isActive}`
    );
  }
  saveQuickLink(role: QuickLinkModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Settings/QuickLink_SaveUpdate`,
      role,
      httpOptions
    );
  }
  getUserGroups() {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Settings/Configuration_UserGroup_Get`
    );
  }
}
