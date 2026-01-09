import { Component } from '@angular/core';
import { ActionModel, Actions, Column } from 'src/app/_models/datatableModel';
import { TenderFacade } from '../state/tender.facades';
import { TitleCasePipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import {
  CommentFilterModel,
  MBookFilterModel,
  TableFilterModel,
  TenderFilterModel,
} from 'src/app/_models/filterRequest';
import {
  convertoWords,
  dateconvertion,
  dateconvertionwithOnlyDate,
  getBtnSeverity,
  getCommentType,
  getYearList,
  getcolorforProgress,
  privileges,
} from 'src/app/shared/commonFunctions';
import { TCModel } from 'src/app/_models/user/usermodel';
import {
  CommentMasterModel,
  MilestoneFileModel,
  WorkActivityModel,
  WorkMasterViewModel,
  WorkTemplateMasterViewModel,
} from 'src/app/_models/go/tender';
import { Subscription } from 'rxjs';
import { TemplatewithMilestoneViewModel } from 'src/app/_models/configuration/temp-milestone';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FailedStatus, ErrorStatus } from 'src/app/_models/ResponseStatus';
import { EventItem } from 'src/app/_models/utils';
import { TenderService } from 'src/app/_services/tender.service';
import { Guid } from 'guid-typescript';
import { FileUploadHandlerEvent } from 'primeng/fileupload';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { GeneralService } from 'src/app/_services/general.service';
import * as moment from 'moment';
import { DatePipe } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';



@UntilDestroy()
@Component({
  selector: 'app-view-work-id',
  templateUrl: './view-work-id.component.html',
  styleUrls: ['./view-work-id.component.scss'],
  providers: [TitleCasePipe],
})
export class ViewWorkIdComponent {
  filtermodel!: MBookFilterModel;

  id!: string;
  tender!: WorkMasterViewModel;
  templateWithMilestone!: WorkTemplateMasterViewModel;
  routeSub!: Subscription;
  canUpdate: boolean = false;

  modalvisible = false;
  updateForm!: FormGroup;

  amountmodalvisible = false;
  amountupdateForm!: FormGroup;

  commentmodalvisible = false;
  AddcommentsForm!: FormGroup;

  events!: EventItem[];
  activitiesevents!: EventItem[];

  comments!: CommentMasterModel[];
  activities!: WorkActivityModel[];
  commentCount: number = 0;
  activityCount: number = 0;
  commentfiletrModel: TableFilterModel = {
    columnSearch: null,
    searchString: null,
    skip: 0,
    sorting: null,
    take: 500000,
  };
  privlegess = privileges;
  fileerror: boolean = false;

  Otherfiles: any = [];
  filesList: MilestoneFileModel[] = [];

//added for view pdf or img
  showViewer = false;
  fileType: 'pdf' | 'image' | 'video' | null = null;
  pdfSrc!: SafeResourceUrl;
  imgSrc!: SafeResourceUrl;
  videoSrc!: SafeResourceUrl;
  currentFileId!: string;
  currentFileName!: string;

  commentType: any[] = getCommentType();
  constructor(
    private tenderFacade: TenderFacade,
    private messageService: MessageService,
    private tenderService: TenderService,
    private router: Router,
    private route: ActivatedRoute,
    private titlecasePipe: TitleCasePipe,
    private generalService: GeneralService,
    private http: HttpClient,
    private datepipe:DatePipe,
    //added for pdf view
    private sanitizer: DomSanitizer
  ) {}
  ngOnInit() {
    this.tenderFacade.selectTender$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.tender = x;
          if (this.tender && this.tender.workTemplateId) {
            this.tenderFacade.getWorktemplate(this.tender.id);
          }
        }
      });
    this.tenderFacade.selectWorkTemplate$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.templateWithMilestone = x;
        }
      });
    this.routeSub = this.route.params
      .pipe(untilDestroyed(this))
      .subscribe((params) => {
        this.id = params['id']; //log the value of id
        this.canUpdate = params['update']; //log the value of id
        this.tenderFacade.getTenderById(this.id);
      });
    this.updateForm = new FormGroup({
      workMilestoneId: new FormControl('', Validators.required),
      completedPercentage: new FormControl('', [
        Validators.required,
        Validators.min(1),
      ]),
      notes: new FormControl('', [Validators.required]),
    });
    this.amountupdateForm = new FormGroup({
      tenderId: new FormControl('', Validators.required),
      updatedNote: new FormControl('', Validators.required),
      increasedAmount: new FormControl('', [Validators.required]),
    });
    this.AddcommentsForm = new FormGroup({
      type: new FormControl('', Validators.required),
      typeId: new FormControl('', Validators.required),
      subject: new FormControl('', Validators.required),
      notes: new FormControl('', [Validators.required]),
    });
    this.getlatestComments();
    this.getlatestActivities();
    this.tenderFacade.selectsavePercenategStatus$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x && (x.Status == FailedStatus || x.Status == ErrorStatus)) {
          this.messageService.add({
            severity: 'error',
            life: 80000,
            summary: 'Error',
            detail: x.message,
          });
        } else if (x) {
          this.resetform();
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
          this.getlatestComments();
          this.getlatestActivities();
          this.tenderFacade.getWorktemplate(this.tender.id);
          this.tenderFacade.getTenderById(this.id);
        }
      });
  }
  opencommentModal(id: string, type: string) {
    if (type === 'TENDER') {
      id = this.id;
    }
    this.AddcommentsForm.get('type')?.patchValue(type);
    this.AddcommentsForm.get('typeId')?.patchValue(id);
    this.AddcommentsForm.get('subject')?.patchValue('Review Comment');
    this.commentmodalvisible = true;
    this.AddcommentsForm.get('type')?.disable({ onlySelf: true });
  }
  submitcomment() {
    this.tenderService
      .saveComment({
        id: Guid.create().toString(),
        commentsText: this.AddcommentsForm.get('notes')?.value,
        type: this.AddcommentsForm.get('type')?.value,
        typeId: this.AddcommentsForm.get('typeId')?.value,
        subjectText: this.AddcommentsForm.get('subject')?.value,
      })
      .subscribe(
        (x) => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
          this.commentmodalvisible = false;
          this.AddcommentsForm.reset();
          this.getlatestComments();
          this.getlatestActivities();
        },
        (error) => {}
      );
  }

  // code changed by vijay 

getlatestComments(): void {
  this.tenderService
    .getCommentList({
      ...this.commentfiletrModel,
      where: { type: 'TENDER', typeId: this.id }
    })
    .subscribe({
      next: (x) => {
       

        // Sort by commentDate descending (newest first)
        this.events = x.data.sort((a: any, b: any) => {
  return new Date(a.commentDate).getTime() - new Date(b.commentDate).getTime();
});

        this.commentCount = x.totalRecordCount;
      },
      error: (err) => {
        console.error('Error loading comments', err);
      }
    });
}

//code change by vijay

getlatestActivities() {
  this.tenderService
    .getactivityList({
      skip: 0,
      take: 500000,
      where: { type: 'TENDER', typeId: this.id },
      ids: null,
      types: null,
      searchString: null,
      sorting: null,
      columnSearch: null,
    })
    .subscribe({
      next: (x) => {
        

        // Sort by createdDate ascending (oldest first)
        this.activitiesevents = x.data.sort((a: any, b: any) => {
          return new Date(a.createdDate).getTime() - new Date(b.createdDate).getTime();
        });

        this.activityCount = x.totalRecordCount;
      },
      error: (err) => {
        console.error('Error loading activities', err);
      }
    });
}

  getBtnSeverity(statusName: string) {
    return getBtnSeverity(statusName);
  }
  back() {
    this.router.navigate(['tender']);
  }
  resetCommentform() {
    this.commentmodalvisible = false;
    this.AddcommentsForm.reset();
  }
  updatePercentage(val: string, percentage: number) {
    this.updateForm.get('workMilestoneId')?.patchValue(val);
    this.updateForm.get('completedPercentage')?.patchValue(percentage);
    this.updateForm.get('notes')?.patchValue('');
    this.getMilestoneFiles(val);
  }
  updateamount() {
    this.amountupdateForm.get('tenderId')?.patchValue(this.tender.tenderId);
    this.amountupdateForm.get('updatedNote')?.patchValue('');
    this.amountupdateForm.get('increasedAmount')?.patchValue(0);
    this.amountmodalvisible = true;
  }
  dc(val: any) {
    return dateconvertionwithOnlyDate(val);
  }
  dcwt(val: any) {
    return dateconvertion(val);
  }
  async submit() {
    if (this.updateForm.invalid) {
      return;
    }
    if (this.Otherfiles && this.Otherfiles.length > 0) {
      await this.onSend(this.Otherfiles);

      this.fileerror = false;
      this.tenderFacade.savePercentage({
        completedPercentage: this.updateForm.get('completedPercentage')?.value,
        workMilestoneId: this.updateForm.get('workMilestoneId')?.value,
        percentageUpdateNote: this.updateForm.get('notes')?.value,
      });
    } else {
      this.fileerror = true;
    }
  }
  amountUpdateSubmit() {
    this.tenderService
      .UpdateAmount({
        tenderId: this.amountupdateForm.get('tenderId')?.value,
        updatedNote: this.amountupdateForm.get('updatedNote')?.value,
        increasedAmount: this.amountupdateForm.get('increasedAmount')?.value,
      })
      .subscribe(
        (x) => {
          if (
            x &&
            (x.data.Status == FailedStatus || x.data.Status == ErrorStatus)
          ) {
            this.messageService.add({
              severity: 'error',
              life: 80000,
              summary: 'Error',
              detail: x.message,
            });
          } else if (x) {
            this.resetform();
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: x?.message,
            });
            this.getlatestComments();
            this.getlatestActivities();
            this.tenderFacade.getWorktemplate(this.tender.id);
            this.tenderFacade.getTenderById(this.tender.tenderId);
          }
        },
        (error) => {
          this.messageService.add({
            severity: 'error',
            life: 80000,
            summary: 'Error',
            detail: error.message,
          });
        }
      );
  }
  redirecttofileupdate() {
    this.router.navigate(['tender', 'generate', this.id]);
  }
  redirecttoTemplateupdate() {
    this.router.navigate(['tender', 'milestone-preparation', this.id]);
  }
  resetform() {
    this.modalvisible = false;
    this.fileerror = false;
    this.updateForm.reset();
    this.Otherfiles = [];
    this.amountmodalvisible = false;
    this.amountupdateForm.reset();
  }

  getcolorforProgress(val: number, type: string) {
    return getcolorforProgress(val, type);
  }

  onSend(files: File[]) {
    if (files && files.length > 0) {
      const formData = new FormData();
      files.forEach((file) => {
        formData.append('files', file);
      });
      formData.append(
        'MilestoneId',
        this.updateForm.get('workMilestoneId')?.value
      );
      this.http
        .post(`${environment.apiUrl}/Work/UploadMilestoneFile`, formData)
        .subscribe(
          (response: any) => {
            this.Otherfiles = [];
            this.filesList.push(response.data);
          },
          (error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to Upload! Please try again',
            });
          }
        );
    } else {
      this.fileerror = true;
    }
  }

  ngOnDestroy() {
    this.tenderFacade.reset();
  }
  getMilestoneFiles(milestoneID: string) {
    this.tenderService.MilestoneFiles(milestoneID).subscribe((x) => {
      if (x) {
        this.filesList = x.data;
      }
      this.modalvisible = true;
    });
  }
  downloadMilestoneFiles(fileid: string, name: string) {
    this.generalService.downloads(fileid, name ?? 'File.png');
  }
  DeleteMilestoneFile(milestoneID: string, fileid: string) {
    this.tenderService
      .DeleteMilestoneFile(milestoneID, fileid)
      .subscribe((x) => {
        if (x && x.data) {
          this.getMilestoneFiles(milestoneID);
        }
        this.modalvisible = true;
      });
  }
  
  onselectfiles(event: any) {
    this.fileerror = false;
    this.Otherfiles = event.currentFiles;
  }
  onRemovefile(event: any) {
    this.Otherfiles = this.Otherfiles.filter(
      (x: any) => event.file.name != x.name
    );
  }
  convertoWordsIND(amt: number) {
    return convertoWords(amt);
  }
  openMbook(id: string) {
    this.router.navigate(['m-book-manage', 'view', id]);
  }

// getWorkCommencementDate(workCommencementDate: string) {
//   return this.datepipe.transform(workCommencementDate, 'dd-MM-yyyy');
// }

//added for show pdf or image by vijay  
showDocPdf(fileId: string, fileKey: string) {
  this.currentFileId = fileId;
  this.currentFileName = fileKey;

  // Find the file metadata (where you store fileName, extension etc.)
  const file = this.tender.files?.find(x => x.type === fileKey);

  // Call backend to get blob
  this.generalService.showPdf(fileId, file?.originalFileName ?? 'File').subscribe(blob => {
    const fileURL = URL.createObjectURL(blob);

    // ✅ Detect extension correctly from original file name or blob type
    let extension = '';
    if (file?.originalFileName) {
      extension = file.originalFileName.split('.').pop()?.toLowerCase() || '';
    } else if (blob.type) {
      // fallback from MIME type
      if (blob.type.includes('pdf')) extension = 'pdf';
      else if (blob.type.includes('image')) extension = 'image';
      else if (blob.type.includes('video')) extension = 'video';
    }

    if (['pdf','doc','docx','xls','xlsx','ppt','pptx','txt'].includes(extension)) {
      this.fileType = 'pdf';
      this.pdfSrc = this.sanitizer.bypassSecurityTrustResourceUrl(fileURL);
    } 
    else if (['png','jpg','jpeg','gif','bmp','webp'].includes(extension)) {
      this.fileType = 'image';
      this.imgSrc = this.sanitizer.bypassSecurityTrustResourceUrl(fileURL);
    } 
    else if (['mp4','webm','ogg'].includes(extension)) {
      this.fileType = 'video';
      this.videoSrc = this.sanitizer.bypassSecurityTrustResourceUrl(fileURL);
    } 
    else {
      console.warn('Unsupported file type:', extension, blob.type);
      return;
    }

    // finally show popup
    this.showViewer = true;
  });
}


download(id: string, filetype: string) {
    //added for dhow pdf
    this.currentFileId = id;
    this.currentFileName = filetype;

  const file = this.tender.files?.find((x) => x.type === filetype);
  this.generalService.downloads(id, file?.originalFileName ?? 'File.png');
}

 

}
