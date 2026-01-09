import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ResponseModel } from '../_models/ResponseStatus';
import { GoFilterModel } from '../_models/filterRequest';
import { httpOptions } from '../_models/utils';

@Injectable({ providedIn: 'root' })
export class WorkLogService {
  constructor(private router: Router, private http: HttpClient) {}

  getGos(filterModel: GoFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/GO_Get`,
      filterModel,
      httpOptions
    );
  }
}
