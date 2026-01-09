import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ResponseModel } from '../_models/ResponseStatus';
import { httpFileUploadOptions, httpOptions } from '../_models/utils';
import {
  ActivityFilterModel,
  CommentFilterModel,
  TenderFilterModel,
} from '../_models/filterRequest';
import { TemplateViewModel } from '../_models/configuration/templates';
import { TemplateMilestoneModel } from '../_models/configuration/temp-milestone';
import {
  CommentMasterModel,
  TenderAmountUpdateModel,
  UpdatePercentageModel,
  WorkActivityModel,
} from '../_models/go/tender';
import { FileMasterModel } from '../_models/mbook/mbook';

@Injectable({ providedIn: 'root' })
export class TenderService {
  constructor(private router: Router, private http: HttpClient) {}
  getGos(
    IsActive: boolean,
    Id?: string,
    gONumber?: string,
    localGONumber?: string
  ) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/GO_Get?IsActive=${IsActive}&Id=${Id}&gONumber=${gONumber}&localGONumber=${localGONumber}`
    );
  }

  getTenders(filterRequest: TenderFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/Tender_Get`,
      filterRequest,
      httpOptions
    );
  }
  getTenderById(id: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/Tender_Create_Work?TenderId=${id}`
    );
  }
  fileupload(formdata: any) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/Work_UploadFile`,
      formdata,
      httpOptions
    );
  }

  workTemplateCreate(id: string, TemplateId: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/Work_Template_Create?WorkId=${id}&TemplateId=${TemplateId}`
    );
  }
  DeleteWorkTemplate(WorkId: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/DeleteWorkTemplate?WorkId=${WorkId}`
    );
  }
  
  IstenderVerified(TenderId: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/Tender_Verified?TenderId=${TenderId}`
    );
  }
  
  GetWorkTemplateMilestone(
    IsActive: boolean,
    id: string,
    WorkTemplateId: string,
    WorkId: string
  ) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/Work_Template_Milestone_Get?IsActive=${IsActive}&Id=${id}&WorkTemplateId=${WorkTemplateId}&WorkId=${WorkId}`
    );
  }
  SaveWorkTemplateMilestone(templateMilestone: TemplateMilestoneModel[]) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/Work_Template_Milestone_SaveUpdate`,
      templateMilestone,
      httpOptions
    );
  }
  GetWorkTemplate(WorkId: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/Work_Template_Get?WorkId=${WorkId}`
    );
  }
  downloadFile(id: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Common/DownloadImage?fileId=${id}`
    );
  }
  UpdatePercentage(model: UpdatePercentageModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/Work_Template_Milestone_UpdatePercentage`,
      model,
      httpOptions
    );
  }
  getCommentList(model: CommentFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/Comment_Get`,
      model,
      httpOptions
    );
  }
  saveComment(model: CommentMasterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/Comment_SaveUpdate`,
      model,
      httpOptions
    );
  }
  filemasterSaveUpdate(model: FileMasterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Common/FileMaster_SaveUpdate`,
      model,
      httpOptions
    );
  }
  getactivityList(model: ActivityFilterModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/Work_Activity_Get_Post`,
      model,
      httpOptions
    );
  }
  UpdateAmount(model: TenderAmountUpdateModel) {
    return this.http.post<ResponseModel>(
      `${environment.apiUrl}/Work/Tender_Update_Amount`,
      model,
      httpOptions
    );
  }
  MilestoneFiles(milestoneId: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/GetMilestoneFiles?MilestoneId=${milestoneId}`
    );
  }




  DeleteMilestoneFile(milestoneId: string, fileId: string) {
    return this.http.get<ResponseModel>(
      `${environment.apiUrl}/Work/DeleteMilestoneFile?MilestoneId=${milestoneId}&FileId=${fileId}`
    );
  }
}
