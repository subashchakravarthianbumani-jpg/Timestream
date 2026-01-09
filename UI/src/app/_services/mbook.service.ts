import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { httpOptions } from '../_models/utils';
import { ResponseModel } from '../_models/ResponseStatus';
import { MBookMasterViewModel } from '../_models/mbook/mbook';
import { MBookFilterModel } from '../_models/filterRequest';

@Injectable({ providedIn: 'root' })
export class MBookService {
  constructor(private router: Router, private http: HttpClient) {}

  getMBookbyId(MBookId: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/MBook_GetById?MBookId=${MBookId}`
    );
  }
  getMBooks(mbookFiletr: MBookFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/MBook_Get`,
      mbookFiletr,
      httpOptions
    );
  }
  
  saveMbook(user: {
    id: string;
    workNotes: string;
    date: string;
    isSubmitted: boolean;
    actualAmount: number;
    statusCode: string;
  }) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/MBook_SaveUpdate`,
      user,
      httpOptions
    );
  }
  approveMbook(user: {
    mbookId: string;
    statusCode: string;
    comments: string;
    mbookApprovHistoryeId:string;
   // documentName:string;
  }) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/MBook_ApproveRejectReturn`,
      user,
      httpOptions
    );
  }

  getFileTypes() {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/MBook_Get_FileTypes`
    );
  }

  getApprovalTypes(mbookId: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/MBook_Get_ApprovalStatusList?mbookId=` +
        mbookId
    );
  }
}
