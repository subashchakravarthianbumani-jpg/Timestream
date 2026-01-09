import { TitleCasePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import {
  FormGroup,
  FormArray,
  FormBuilder,
  FormControl,
  Validators,
} from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { Guid } from 'guid-typescript';
import { MessageService, ConfirmationService } from 'primeng/api';
import { FileUploadHandlerEvent } from 'primeng/fileupload';
import { MBookMasterViewModel } from 'src/app/_models/mbook/mbook';
import { TCModel } from 'src/app/_models/user/usermodel';
import { GeneralService } from 'src/app/_services/general.service';
import { environment } from 'src/environments/environment';
import { MBookConfigFacade } from '../state/mbook.facade';
import * as moment from 'moment';
import {
  ErrorStatus,
  FailedStatus,
  SuccessStatus,
} from 'src/app/_models/ResponseStatus';
import { Location } from '@angular/common';
import {
  convertoWords,
  dateconvertion,
  dateconvertionwithOnlyDate,
  getCommentType,
} from 'src/app/shared/commonFunctions';
import { MilestoneFileModel } from 'src/app/_models/go/tender';
import { TenderService } from 'src/app/_services/tender.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';


@UntilDestroy()
@Component({
  selector: 'app-view-mbook',
  templateUrl: './view-mbook.component.html',
  styleUrls: ['./view-mbook.component.scss'],
  providers: [TitleCasePipe],
})
export class ViewMbookComponent 
{
  mBookId!: string;
  mBookapproveId!: string;
  title: string = 'M-Book View';
  mBookForm!: FormGroup;
  approvalForm!: FormGroup;
  mBookDetails?: MBookMasterViewModel;
  filetypes!: TCModel[];
  approvaltypes!: TCModel[];
  Otherfiles!: any[];
  isapprovepage: boolean = false;
  canapprove: boolean = false;
  mbookapproveId!: string;
  filesList: MilestoneFileModel[] = [];

  
  //pdfSrc: SafeResourceUrl | null = null;
 // imgSrc: SafeResourceUrl | null = null;
//  viewerVisible = false;


//new model for showing pdf
  showViewer = false;
  fileType: 'pdf' | 'image' | 'video' | null = null;
  pdfSrc!: SafeResourceUrl;
  imgSrc!: SafeResourceUrl;
  videoSrc!: SafeResourceUrl;
  currentFileId!: string;
  currentFileName!: string;

   Qualityfiletypes = [
  { text: 'Quality Testing Document', value: 'MBookTestingDocument' },
  { text: 'Other Doc', value: 'MBookOtherDoc' },
  // … more …
];

  
  
  Documentname!: string;
  fileerror: boolean = false;
  fileselected: boolean = false;
  
  get f() {
    return this.mBookForm?.controls;
  }
  get t() {
    return this.f['documents'] as FormArray;
  }
  get fileListd() {
    return this.t.controls as FormGroup[];
  }
  constructor(
    private mbookFacade: MBookConfigFacade,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private location: Location,
    private router: Router,
    private route: ActivatedRoute,
    private generalService: GeneralService,
    private tenderService: TenderService,
    private formBuilder: FormBuilder,
    private http: HttpClient,
    //added for pdf view 
    private sanitizer: DomSanitizer,
  ) {}

  ngOnInit() {
    this.mbookFacade.updateApproveStatus();
    this.route.params.pipe(untilDestroyed(this)).subscribe((params) => {
      
      this.mBookId = params['id']; //log the value of id
      this.isapprovepage = params['isapprove']; //log the value of id
      if (this.mBookId) {
        this.mbookFacade.getmbookbyId(this.mBookId);
        this.mbookFacade.getApprovalTypes(this.mBookId);
      }
      if (this.isapprovepage) {
        this.title = 'M-Book Review';
      } else {
        this.title = 'M-Book View';
      }
    });
    this.mbookFacade.selectMBookbyId$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.mBookDetails = x;

          this.getMilestoneFiles(this.mBookDetails.workTemplateMilestoneId);
          this.canapprove = this.mBookDetails.isActionable; //log the value of id

          if (this.mBookDetails.files) {
            this.generateDefaultRows(this.mBookDetails.files);
            this.setvaluesofortheForm();
          }
        }
        
      });
    this.mbookFacade.selectapprovaltypes$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.approvaltypes = x;
        }
      });
    this.mbookFacade.selectapprovestatus$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x && (x.Status == FailedStatus || x.Status == ErrorStatus)) {
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
          this.mbookFacade.updateApproveStatus();
          this.location.back();
        }
      });
    this.mBookForm = new FormGroup({
      id: new FormControl(this.mBookId),
      date: new FormControl('', [Validators.required]),
      actuals: new FormControl('', [Validators.required]),
      notes: new FormControl('', [Validators.required]),
      name: new FormControl('', [Validators.required]),
      documents: new FormArray([]),
    });
    this.approvalForm = new FormGroup({
      id: new FormControl(this.mBookId),
      notes: new FormControl('', [Validators.required]),
      approvaltype: new FormControl('', [Validators.required]),

      name: new FormControl('', [Validators.required]),
    });
  }
  getQualityFileText(value: string): string {
  const match = this.Qualityfiletypes.find(f => f.value === value);
  return match ? match.text : value;
}
  generateDefaultRows(files: any[]) {
    this.t.clear();
    if (files) {
      files.forEach((x) => {
     
        this.t.push(
          this.formBuilder.group({
            id: [x.id],
            type: [x.type, [Validators.required]],
            typeName: [x.typeName, [Validators.required]],
            originalFileName: [x.originalFileName],
            savedFileName: [x.savedFileName],
            fileType: [x.fileType],
            typeId: [this.mBookId, [Validators.required]],
            savedDate: [x.createdDate],
            savedby: [x.createdByUserName],
            savedbyrole: [x.createdbyRole],
            savedbyuserdivision: [x.createdbydivion],
          })
        );
      });
    }
  }
  setvaluesofortheForm() {
    this.mBookForm
      .get('date')
      ?.patchValue(
        this.mBookDetails?.date
          ? moment(this.mBookDetails?.date).toDate()
          : null
      );
    this.mBookForm.get('actuals')?.patchValue(this.mBookDetails?.actualAmount);
    this.mBookForm.get('notes')?.patchValue(this.mBookDetails?.workNotes);
  }
  submit() {
    if (
      this.approvalForm.valid &&
      this.Otherfiles?.length > 0
    ) {
      id: Guid.create().toString();
     const fakeEvent = { files: this.Otherfiles };
      
     // this.approvefileupload(fakeEvent, 'MBookApproveFile');
   // } else {
      //this.fileerror = true;
     // this.messageService.add({
      //  severity: 'warn',
      //  summary: 'Missing Details',
      //  detail: 'Please fill all required fields.',
      //});
    }
    //this.approvefileupload(event);
    this.mbookFacade.ApproveMBook({
      comments: this.approvalForm.get('notes')?.value,
      mbookId: this.mBookId,
      statusCode: this.approvalForm.get('approvaltype')?.value,
      mbookApprovHistoryeId: this.mbookapproveId,
      //documentName: this.approvalForm.get('name')?.value,
    });
  }
  resetForm() {
    this.mbookFacade.updateApproveStatus();
    this.location.back();
  }
  back() {
    this.mbookFacade.updateApproveStatus();
    this.location.back();
  }

  download(id: string, filename: string) {
    //added for dhow pdf
    this.currentFileId = id;
    this.currentFileName = filename;
    this.generalService.downloads(id, filename);
  }

   //added for show pdf or image by vijay  
showDocPdf(fileId: string, fileName: string) {

  this.currentFileId = fileId;
  this.currentFileName = fileName;

  this.generalService.ViewPdf(fileId).subscribe(blob => {
    const fileURL = URL.createObjectURL(blob);
    const extension = fileName.split('.').pop()?.toLowerCase();

    if (['pdf','doc','docx','xls','xlsx','clsx','ppt','pptx','txt'].includes(extension!)) {
      this.fileType = 'pdf';
     // Mark blob URL as safe for Angular
      this.pdfSrc = this.sanitizer.bypassSecurityTrustResourceUrl(fileURL);

    } 
    else if (['png', 'jpg', 'jpeg', 'gif','bmp','webp'].includes(extension!)) {
      this.fileType = 'image';
      this.imgSrc = this.sanitizer.bypassSecurityTrustResourceUrl(fileURL);

    } 
    else if (['mp4', 'webm', 'ogg'].includes(extension!)) {
      this.fileType = 'video';
      this.videoSrc = this.sanitizer.bypassSecurityTrustResourceUrl(fileURL);

    } 
    else {
      console.warn('Unsupported file type:', extension);
      return;
    }

    // Show the popup
    this.showViewer = true;
  });
}



  ngOnDestroy() {
    this.mbookFacade.reset();
  }
  dc(val: any) {
    return dateconvertionwithOnlyDate(val);
  }
  dcwt(val: any) {
    return dateconvertion(val);
  }
  getMilestoneFiles(milestoneID: string) {
    this.tenderService.MilestoneFiles(milestoneID).subscribe((x) => {
      if (x) {
        this.filesList = x.data;
      }
    });
  }

  //added for show pdf or image by vijay 

showFileInPopup(fileId: string, fileName: string) {

   // Store current file info for download button by vijay

  this.currentFileId = fileId;
  this.currentFileName = fileName;

  this.generalService.ViewPdf(fileId).subscribe(blob => {
    const fileURL = URL.createObjectURL(blob);
    const extension = fileName.split('.').pop()?.toLowerCase();

    if (['pdf','doc','docx','xlsx','xls','clxs','ppt','pptx','txt'].includes(extension!)) {
      this.fileType = 'pdf';
     // Mark blob URL as safe for Angular
      this.pdfSrc = this.sanitizer.bypassSecurityTrustResourceUrl(fileURL);

    } 
    else if (['png', 'jpg', 'jpeg', 'gif','bmp','webp'].includes(extension!)) {
      this.fileType = 'image';
      this.imgSrc = this.sanitizer.bypassSecurityTrustResourceUrl(fileURL);

    } 
    else if (['mp4', 'webm', 'ogg'].includes(extension!)) {
      this.fileType = 'video';
      this.videoSrc = this.sanitizer.bypassSecurityTrustResourceUrl(fileURL);

    } 
    else {
      console.warn('Unsupported file type:', extension);
      return;
    }

    // Show the popup
    this.showViewer = true;
  });
}

  downloadMilestoneFiles(fileid: string, name: string) {
    // Store current file info for download button add by vijay
    this.currentFileId = fileid;
    this.currentFileName = name;
    this.generalService.downloads(fileid, name ?? 'File.png');
  }
  DeleteMilestoneFile(milestoneID: string, fileid: string) {
    this.tenderService
      .DeleteMilestoneFile(milestoneID, fileid)
      .subscribe((x) => {
        if (x && x.data) {
          this.getMilestoneFiles(milestoneID);
        }
      });
  }

  convertoWordsIND(amt: number) {
    return convertoWords(amt);
  }

  onselectfiles(event: any) {
    this.fileselected = true;
    this.Otherfiles = event.currentFiles;
  }
  /*onRemovefile(event: any) {
    this.fileselected = false;
    this.Otherfiles = this.Otherfiles.filter(
      (x: any) => event.file.name != x.name
    );
  }*/

/*approvefileupload(event: any, type: string) 
  {
    if (event?.files?.length > 0) 
      {
      this.mbookapproveId = Guid.create().toString();
      const formData = new FormData();
      formData.append('file', event.files[0]);
      formData.append('type', type);
      formData.append('typeId', this.mbookapproveId);

      this.http
  .post(`${environment.apiUrl}/Common/UploadFile`, formData)
  .subscribe(
    (x:any) => 
      {
      if (x && (x.Status == FailedStatus || x.Status == ErrorStatus)) 
        {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to Upload! Please try again.',
        });
      }
      else
       {
        
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Uploaded Successfully',
        });
        
        this.mbookFacade.updatesaveStatus();
      }
    
    });
   

    }
  }*/
  
  
  


}
