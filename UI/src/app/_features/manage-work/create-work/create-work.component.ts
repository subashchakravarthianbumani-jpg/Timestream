import { TitleCasePipe } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Guid } from 'guid-typescript';
import { ConfirmationService, MessageService } from 'primeng/api';
import { MBookConfigFacade } from '../state/mbook.facade';
import { MBookMasterViewModel } from 'src/app/_models/mbook/mbook';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TCModel } from 'src/app/_models/user/usermodel';
import { GeneralService } from 'src/app/_services/general.service';
import { FileUploadHandlerEvent } from 'primeng/fileupload';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import {
  ErrorStatus,
  FailedStatus,
  ResponseModel,
  SuccessStatus,
} from 'src/app/_models/ResponseStatus';
import * as moment from 'moment';
import {
  convertoWords,
  dateconvertion,
  dateconvertionwithOnlyDate,
} from 'src/app/shared/commonFunctions';
import { TenderService } from 'src/app/_services/tender.service';
import { MilestoneFileModel } from 'src/app/_models/go/tender';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';


@UntilDestroy()
@Component({
  selector: 'app-create-work',
  templateUrl: './create-work.component.html',
  styleUrls: ['./create-work.component.scss'],
  providers: [TitleCasePipe],
})
export class CreateWorkComponent {
  mBookId!: string;
  title: string = 'M-Book Edit';
  Savelabel: string = 'Save';
  mBookForm!: FormGroup;
  approvalForm!: FormGroup;
  mBookDetails?: MBookMasterViewModel;
  filetypes!: TCModel[];
  Otherfiles: any[] = [];

  //added for view pdf or img
  showViewer = false;
  fileType: 'pdf' | 'image' | 'video' | null = null;
  pdfSrc!: SafeResourceUrl;
  imgSrc!: SafeResourceUrl;
  videoSrc!: SafeResourceUrl;
  currentFileId!: string;
  currentFileName!: string;

  isSubmitted: boolean = false;
  canSubmit: boolean = false;
  filesList: MilestoneFileModel[] = [];

  QualityTestingFileTypes = [
     { text: 'Quality Testing Documents', value: 'Quality Testing Documents' },
  { text: 'Chemical & Physical Properties of Lime', value: 'Chemical & Physical Properties of Lime' },
  { text: 'Bulking of Sand', value: 'Bulking of Sand' },
  { text: 'Silt Content', value: 'Silt Content' },
  { text: 'Particle size distribution', value: 'Particle size distribution' },
 
];

  get f() {
    return this.mBookForm.controls;
  }
  get t() {
    return this.f['documents'] as FormArray;
  }
  get fileListd() {
    return this.t.controls as FormGroup[];
  }

  get typeErrorMsg() {
    var error: any;
    this.fileListd.map((x) => {
      if (x.controls['type'].errors) {
        error = x.controls['type'].errors;
      }
    });
    if (error && error['required']) {
      return 'Required';
    }

    return null;
  }
  get validreqDocErrorMsg() {
    var error: any;
    var reqDoc = [
      'MBookEntryDocument',
      'MBookTestingDocument',
      'MBookPhotos',
      'MBookVideos',
    ];
    var currDocs: string[] = [];
    this.fileListd.map((x) => {
      if (x.controls['id'].value) {
        currDocs = [...currDocs, x.controls['type'].value];
      }
    });
    if (!reqDoc.every((x) => currDocs.includes(x))) {
      return 'Please provide  mandatory documents to submit the MBook';
    }
    return null;
  }
  get typeNameErrorMsg() {
    var error: any;
    this.fileListd.map((x) => {
      if (x.controls['typeName'].errors) {
        error = x.controls['typeName'].errors;
      }
    });
    if (error && error['required']) {
      return 'Required';
    }

    return null;
  }
  constructor(
    private mbookFacade: MBookConfigFacade,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private router: Router,
    private route: ActivatedRoute,
    private generalService: GeneralService,
    private tenderService: TenderService,
    private formBuilder: FormBuilder,
    private http: HttpClient,
    //added for pdf view 
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit() {
    this.mbookFacade.getFileTypes();
    this.route.params.pipe(untilDestroyed(this)).subscribe((params) => {
      this.mBookId = params['id']; //log the value of id
    });
    this.mbookFacade.selectMBookbyId$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.mBookDetails = x;
          this.canSubmit = x.isActionable == false && x.isEditable == true;
          // false && true
          if (this.mBookDetails) {
            this.getMilestoneFiles(this.mBookDetails.workTemplateMilestoneId);
            this.setvaluesofortheForm();
            if (this.mBookDetails.files && this.mBookDetails.files.length > 0) {
              this.generateDefaultRows(this.mBookDetails.files);
              this.filetypes.forEach((x) => {
                if (!this.mBookDetails?.files?.find((y) => y.type == x.value)) {
                  this.createDummyrows(x.value, x.text);
                }
              });
            } else {
              this.filetypes.forEach((x) => {
                this.createDummyrows(x.value, x.text);
              });
            }
          } else {
            this.filetypes.forEach((x) => {
              this.createDummyrows(x.value, x.text);
            });
          }
          this.disablecontroles();
        }
      });
    this.mbookFacade.selectfiletypes$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.filetypes = x;
          if (this.mBookId) {
            this.mbookFacade.getmbookbyId(this.mBookId);
          }
        }
      });
    this.mbookFacade.selectSaveStatus$
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
          if (this.isSubmitted) {
            this.router.navigate(['m-book-manage']);
          }
        }
      });
    this.mBookForm = new FormGroup({
      id: new FormControl(this.mBookId),
      isSubmitted: new FormControl(false),
      actuals: new FormControl(0, [Validators.required]),
      date: new FormControl('', [Validators.required]),
      notes: new FormControl('', [Validators.required]),
      documents: new FormArray([]),
    });
    this.approvalForm = new FormGroup({
      id: new FormControl(Guid.raw()),
      date: new FormControl('', [Validators.maxLength(200)]),
      notes: new FormControl('', [Validators.required]),
      type: new FormControl('', [Validators.required]),
    });
  }


  createDummyrows(type: string, typeName: string) {
    this.t.push(
      this.formBuilder.group({
        id: [null],
        type: [type, [Validators.required]],
        typeName: [typeName, [Validators.required]],
        originalFileName: [null],
        savedFileName: [null],
        fileType: [null],
        typeId: [this.mBookId, [Validators.required]],
      })
    );
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
          })
        );
      });
    }
  }
  addnewRow(event: Event) {
    this.confirmationService.confirm({
      key: 'confirm2',
      target: (event.target as HTMLInputElement) || new EventTarget(),
      message: 'Are you sure that you want to Add?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.t.push(
          this.formBuilder.group({
            id: [null],
            type: [null, [Validators.required]],
            typeName: [null, [Validators.required]],
            originalFileName: [null],
            savedFileName: [null],
            fileType: [null],
            typeId: [this.mBookId, [Validators.required]],
          })
        );
      },
      reject: () => {},
    });
  }
  submit(val: boolean = true) {
    this.mbookFacade.updatesaveStatus();
    this.mbookFacade.saveMBook({
      date: this.mBookForm.get('date')?.value,
      actualAmount: this.mBookForm.get('actuals')?.value,
      id: this.mBookForm.get('id')?.value,
      workNotes: this.mBookForm.get('notes')?.value,
      isSubmitted: val,
      statusCode: !this.canSubmit ? 'ONLYSAVE' : '',
    });
    this.isSubmitted = val;
  }
  disablecontroles() {
    this.fileListd.map((x) => {
      if (x.controls['type'].value && x.controls['id'].value) {
        x.controls['type'].disable({ onlySelf: true });
        x.controls['typeName'].disable({ onlySelf: true });
      } else if (
        x.controls['type'].value &&
        !x.controls['id'].value &&
        x.controls['type'].value !== ('MBookOtherDoc') &&  x.controls['type'].value !== ('MBookTestingDocument')
      ) {
        x.controls['typeName'].disable({ onlySelf: true });
      } else {
        x.controls['type'].enable({ onlySelf: true });
        x.controls['typeName'].enable({ onlySelf: true });
      }
    });
  }
  setvaluesofortheForm() {
    this.mBookForm
      .get('date')
      ?.patchValue(
        this.mBookDetails?.date
          ? moment(this.mBookDetails?.date).toDate()
          : null
      );
    this.mBookForm.get('notes')?.patchValue(this.mBookDetails?.workNotes);
    this.mBookForm.get('actuals')?.patchValue(this.mBookDetails?.actualAmount);
  }
  changeRole(s: any, i: number) {
    const file = this.filetypes.find((x) => x.value == s.value);
    this.fileListd[i].controls['typeName'].patchValue(file?.text);
    if (file?.value !== 'MBookOtherDoc') {
      this.fileListd[i].controls['typeName'].disable({ onlySelf: true });
    } else {
      this.fileListd[i].controls['typeName'].enable({ onlySelf: true });
    }
     if (file?.value !== 'MBookTestingDocument') {
      this.fileListd[i].controls['typeName'].disable({ onlySelf: true });
    } else {
      this.fileListd[i].controls['typeName'].enable({ onlySelf: true });
    }
  }
  testDoc(s: any, i: number){
    const file = this.filetypes.find((x) => x.value == s.value);
    this.fileListd[i].controls['typeName'].patchValue(file?.text);
    if (file?.value !== 'MBookTestingDocument') {
      this.fileListd[i].controls['typeName'].disable({ onlySelf: true });
    } else {
      this.fileListd[i].controls['typeName'].enable({ onlySelf: true });
    }
  }

  resetForm() {
    this.mbookFacade.updatesaveStatus();
    this.router.navigate(['m-book-manage']);
  }
  back() {
    this.mbookFacade.updatesaveStatus();
    this.router.navigate(['m-book-manage']);
  }
  onSend(event: FileUploadHandlerEvent, i: number) {
    if (event.files && event.files.length > 0) {
      const formData = new FormData();
      formData.append('File', event.files[0]);
      formData.append('Type', this.fileListd[i].controls['type'].value);
      formData.append('TypeId', this.mBookId);
      formData.append('typeName', this.fileListd[i].controls['typeName'].value);
      this.http
        .post(`${environment.apiUrl}/Common/UploadFile`, formData)
        .subscribe(
          (response: any) => {
            this.Otherfiles = [];

            this.fileListd[i].controls['id'].patchValue(response.data.id);
            this.fileListd[i].controls['originalFileName'].patchValue(
              response.data.originalFileName
            );
            this.fileListd[i].controls['savedFileName'].patchValue(
              response.data.savedFileName
            );
            this.fileListd[i].controls['fileType'].patchValue(
              response.data.fileType
            );
            this.fileListd[i].controls['typeName'].patchValue(
              response.data.typeName
            );
            this.fileListd[i].controls['type'].disable({ onlySelf: true });
            this.fileListd[i].controls['typeName'].disable({ onlySelf: true });
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
  download(id: string, filename: string) {
        //added for show pdf
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

    if (['pdf','doc','docx','xlsx','xls','clsx','ppt','pptx','txt'].includes(extension!)) {
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
  removefiledetails(i: number) {
    if (this.fileListd[i].controls['id'].value) {
      this.tenderService
        .filemasterSaveUpdate({
          ...this.fileListd[i].value,
          isActive: false,
        })
        .subscribe((x) => {
          if (x.status == SuccessStatus) {
            this.t.removeAt(i);
          }
        });
    } else {
      this.t.removeAt(i);
    }
  }
  savefiledetails(i: number) {}
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

  //add for show pdf or image by vijay 

showFileInPopup(fileId: string, fileName: string) {

   // Store current file info for download button by vijay

  this.currentFileId = fileId;
  this.currentFileName = fileName;

  this.generalService.ViewPdf(fileId).subscribe(blob => {
    const fileURL = URL.createObjectURL(blob);
    const extension = fileName.split('.').pop()?.toLowerCase();

    if (['pdf','doc','docx','clsx','xlsx','xls','ppt','pptx','txt'].includes(extension!)) {
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
}
