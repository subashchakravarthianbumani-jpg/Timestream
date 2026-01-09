import { TitleCasePipe } from '@angular/common';
import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { TenderFacade } from '../state/tender.facades';

import {
  TenderMasterViewModel,
  WorkMasterViewModel,
} from 'src/app/_models/go/tender';
import { Subscription } from 'rxjs';
import { Location } from '@angular/common';
import { FileUploadHandlerEvent, UploadEvent } from 'primeng/fileupload';
import { TenderService } from 'src/app/_services/tender.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import {
  convertoWords,
  dateconvertion,
  dateconvertionwithOnlyDate,
  privileges,
} from 'src/app/shared/commonFunctions';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { GeneralService } from 'src/app/_services/general.service';
import { httpOptions } from 'src/app/_models/utils';
import { ErrorStatus, FailedStatus } from 'src/app/_models/ResponseStatus';

@UntilDestroy()
@Component({
  selector: 'app-gnrte-work-id',
  templateUrl: './gnrte-work-id.component.html',
  styleUrls: ['./gnrte-work-id.component.scss'],
  providers: [MessageService],
})
export class GnrteWorkIdComponent {
  id!: string;
  tender!: WorkMasterViewModel;
  routeSub!: Subscription;

  isVerified: boolean = false;

  privlegess = privileges;
  title: string = 'Manage Work';
  LetterOfAcceptancefiles: any = [];
  WorkOrderfiles: any = [];
  Otherfiles: any = [];
  AgreementCopyfiles: any = [];

  constructor(
    private tenderFacade: TenderFacade,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private _location: Location,
    private tenderService: TenderService,
    private generalService: GeneralService,
    private http: HttpClient
  ) {}

  back() {
    this._location.back();
  }
  ngOnDestroy() {
    this.routeSub.unsubscribe();
    this.tenderFacade.reset();
  }
  ngOnInit() {
    this.tenderFacade.selectTender$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.tender = x;
        }
      });
    this.routeSub = this.route.params
      .pipe(untilDestroyed(this))
      .subscribe((params) => {
        this.id = params['id']; //log the value of id
        this.tenderFacade.getTenderById(this.id);
      });
  }
  onUpload(event: UploadEvent) {}
  clear() {}
  onSend(event: FileUploadHandlerEvent, type: string) {
    if (event.files && event.files.length > 0) {
      const formData = new FormData();
      formData.append('file', event.files[0]);
      formData.append('type', type);
      formData.append('workId', this.tender.id);
      this.http
        .post(`${environment.apiUrl}/Work/Work_UploadFile`, formData)
        .subscribe(
          (response) => {
            if (type === 'LetterOfAcceptance') {
              this.LetterOfAcceptancefiles = [];
            }
            // else if (type === 'WorkOrder') {
            //   this.WorkOrderfiles = [];
            // }
            else if (type === 'AgreementCopy') {
              this.AgreementCopyfiles = [];
            } else if (type === 'Other') {
              this.Otherfiles = [];
            }
            this.tenderFacade.getTenderById(this.id);
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Uploaded Successfully',
            });
          },
          (error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to Upload! Please try again',
            });
          }
        );
    }
  }
  download(id: string, filetype: string) {
    const file = this.tender.files?.find((x) => x.type === filetype);
    this.generalService.downloads(id, file?.originalFileName ?? 'File.png');
  }
  downloaddoc(url: string, name: string) {
    this.generalService.DocumentsDownload(url).subscribe(async (event) => {
      let data = event as HttpResponse<Blob>;
      const downloadedFile = new Blob([data.body as BlobPart], {
        type: data.body?.type,
      });
      if (downloadedFile.type != '') {
        const a = document.createElement('a');
        a.setAttribute('style', 'display:none;');
        document.body.appendChild(a);
        a.download = name;
        a.href = URL.createObjectURL(downloadedFile);
        a.target = '_blank';
        a.click();
        document.body.removeChild(a);
      }
    });
  }
  get requiresVerification(): boolean {
    return !!(this.tender?.agreementCopyId || this.tender?.letterOfAcceptanceId || this.tender?.otherFileId);
  }
  dc(date: any) {
    return dateconvertionwithOnlyDate(date);
  }
  resetform() {
    this._location.back();
  }
  next() {
    
    this.router.navigate([
      'tender/milestone-preparation',
      this.tender.tenderId,
    ]);
    
  }

  convertoWordsIND(amt: number) {
    return convertoWords(amt);
  }
  verify()
  {
    this.tenderService.IstenderVerified(this.tender.tenderId).subscribe((x:any)=>{
      if (x && (x.status == FailedStatus || x.status == ErrorStatus)) {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          life: 80000,
          detail: x.message,
        });
      } else if (x) {
        
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: x?.message,

        });
        this.tenderFacade.getTenderById(this.id);
      
     
        
      }
    })


    
  }
}
