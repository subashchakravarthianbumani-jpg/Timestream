import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { Location } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import * as moment from 'moment';
import { ConfirmationService, MessageService } from 'primeng/api';
import {
  convertoWords,
  dateconvertionwithOnlyDate,
  privileges,
} from 'src/app/shared/commonFunctions';
import { TemplateMilestoneModel } from 'src/app/_models/configuration/temp-milestone';
import { TemplateViewModel } from 'src/app/_models/configuration/templates';
import { Actions } from 'src/app/_models/datatableModel';
import { WorkMasterViewModel } from 'src/app/_models/go/tender';
import { ErrorStatus, FailedStatus, ResponseModel } from 'src/app/_models/ResponseStatus';
import { TenderFacade } from '../state/tender.facades';
import { Templateservice } from 'src/app/_services/templates.service';
import { TenderService } from 'src/app/_services/tender.service';
import { TemplatesConfigFacade } from '../../configuration/templates/state/template.facades';
import { TwoColConfigService } from 'src/app/_services/two.col.service';


@UntilDestroy()
@Component({
  selector: 'app-milestone-preparation',
  templateUrl: './milestone-preparation.component.html',
  styleUrls: ['./milestone-preparation.component.scss'],
  providers: [ConfirmationService,TemplatesConfigFacade],
})
export class MilestonePreparationComponent {  
  title: string = 'Milestone Preparation';
  milestoneForm!: FormGroup;
  defaultstatus = false;
  actions: Actions[] = [];
  showDetail = false;
  isNew = true;
  isUpdating = false;

  selectedTemplate = '';
  configurationList!: TemplateViewModel[];
  gworkId!: string;
  workTemplateId!: string;
  startDate!: string;

  workType!: string;
  workTypeId!: string;
  tenderID!: string;
  tender!: WorkMasterViewModel;
  durationInDays!: number;
  name!: string;
  templateNumber!: string;
  templateCode!: string;
  strength!: string;
  subWorkType!: string;
  serviceType!: string;
  serviceTypeId!: string;
  categoryType!: string;
  categoryTypeId!: string;
  subWorkTypeId!: string;
  templateId!:string;
  privlegess = privileges;
  isMilestoneSubmit: boolean = false;
  templateWithMilestone?: any;

  isEditMode:boolean=false;
  templateForm!: FormGroup;
  currentStatus:boolean=true;

  get f() {
    return this.milestoneForm.controls;
  }
  get t() {
    return this.f['milestones'] as FormArray;
  }
  get milestoneList() {
    return this.t.controls as FormGroup[];
  }
  get milestoneNameErrorMsg() {
    var error: any;
    this.milestoneList.map((x) => {
      if (x.controls['milestoneName'].errors) {
        error = x.controls['milestoneName'].errors;
      }
    });

    if (this.defaultstatus) {
      return null;
    }
    if (error && error['required']) {
      return 'Required';
    }

    return null;
  }
  get milestoneCodeErrorMsg() {
    var error: any;
    this.milestoneList.map((x) => {
      if (x.controls['milestoneCode'].errors) {
        error = x.controls['milestoneCode'].errors;
      }
    });

    if (this.defaultstatus) {
      return null;
    }
    if (error && error['required']) {
      return 'Required';
    }

    return null;
  }
  get startdateErrorMsg() {
    var error: any;
    var colloideerror: any;
    var index: number = 0;
    var secindex: number = 0;
    this.milestoneList.map((x) => {
      index = index + 1;
      if (x.controls['startDate'].errors) {
        error = x.controls['startDate'].errors;
      }
      this.milestoneList.map((y) => {
        secindex = secindex + 1;
        var startDate = x.controls['startDate'].value;
        if (
          secindex != index &&
          y.controls['startDate'].value < startDate &&
          startDate < y.controls['endDate'].value
        ) {
          colloideerror = true;
        }
      });
      secindex = 0;
    });

    if (error && error['required']) {
      return 'Required';
    } else if (colloideerror) {
      return 'Start Date is colloiding';
    }

    return null;
  }
  get durationErrorMsg() {
    var error: any;
    this.milestoneList.map((x) => {
      if (x.controls['durationInDays'].errors) {
        error = x.controls['durationInDays'].errors;
      }
    });
    if (this.defaultstatus) {
      return null;
    }

    if (error && error['required']) {
      return 'Required';
    } else if (error && error['min']) {
      return 'Minimun (day)s should be 1';
    } else if (this.totaldays != (this.durationInDays ?? 0)) {
      return `Sum of days should be ${this.durationInDays ?? 0}`;
    }

    return null;
  }
  get paymentreqErrorMsg() {
    var error: any;
    var isgreaterthanzero: any;
    this.milestoneList.map((x) => {
      if (x.controls['paymentPercentage'].errors) {
        error = x.controls['paymentPercentage'].errors;
      }
      if (
        x.controls['isPaymentRequired'].value &&
        x.controls['paymentPercentage'].value <= 0
      ) {
        isgreaterthanzero = true;
      }
    });

    if (this.defaultstatus) {
      return null;
    }
    if (error && error['required']) {
      return 'Required';
    } else if (isgreaterthanzero) {
      return '% should be greater than 0';
    } else if (this.totalpercenatge != 100) {
      return `Sum of % should be 100`;
    }

    return null;
  }
  get totaldays() {
    let total = 0;
    this.milestoneList.map(
      (x) => (total += x.controls['durationInDays'].value)
    );
    return total;
  }
  get totalpercenatge() {
    let total = 0;
    this.milestoneList.map(
      (x) => (total += x.controls['paymentPercentage'].value)
    );
    return total;
  }
  constructor(
    private messageService: MessageService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private confirmationService: ConfirmationService,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private tenderFacade: TenderFacade,
    private _location: Location,
    private service:Templateservice,
    private Tservice:TenderService,
    private templateConfigFacade: TemplatesConfigFacade,
     
  ) {}

  ngOnInit() {
 //this.templateConfigFacade.getTemplates(this.currentStatus);
    this.route.params.pipe(untilDestroyed(this)).subscribe((params) => {
      this.tenderID = params['id']; //log the value of id
      if (this.tenderID) {
        this.tenderFacade.getTenderById(this.tenderID);
      }
    });
    this.tenderFacade.selectTender$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.tender = x;
          this.gworkId = this.tender.id;
          this.workTemplateId = this.tender.workTemplateId;
          if (this.workTemplateId) {
            
            this.isNew = false;
            this.showDetail = true;
            this.tenderFacade.getWorktemplate(this.gworkId);
          } else {
           
            this.tenderFacade.getTemplates(true,this.tender.subcategory,this.tender.mainCategory,this.tender.service_type_main,this.tender.category_type_main);
          }
        }
      });
    this.milestoneForm = this.formBuilder.group({
      milestones: new FormArray([]),
    });

    this.tenderFacade.selectSaveStatus$
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
          this.resetForm();
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
          this.tenderFacade.updatesaveStatus();
          this.isNew = false;
          this.showDetail = true;
          this.tenderFacade.getWorktemplate(this.gworkId);
        }
      });
    this.tenderFacade.selectmilestoneSaveStatus$
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
          this.tenderFacade.updatemilestonessaveStatus();
          if (this.isMilestoneSubmit) {
            this.router.navigate(['tender']);
          }
          this.resetForm();
          this.tenderFacade.getWorktemplate(this.gworkId);
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
        }
      });

    this.tenderFacade.selectTemplateWithMilestones$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          
          this.templateWithMilestone = x;
          this.workType = x.workType;
          this.workTypeId = x.workTypeId;
          this.templateNumber = x.templateNumber;
          this.name = x.name;
          this.templateCode = x.templateCode;
          this.strength = x.strength;
          this.subWorkType = x.subWorkType;
          this.subWorkTypeId = x.subWorkTypeId;
          this.durationInDays = x.durationInDays;
          this.serviceType = x.serviceType;
      this.serviceTypeId = x.serviceTypeId;
      this.categoryType = x.categoryType;
      this.categoryTypeId = x.categoryTypeId;
          this.generateDefaultRows(
            this.templateWithMilestone.templateMilestones
          );
          this.cdr.markForCheck();
        }
      });

    this.tenderFacade.selectTemplates$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          
          this.configurationList = x.filter((x) => x.isPublished == true);
        }
      });

    this.tenderFacade.selectWorkTemplate$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
         
          this.templateWithMilestone = x;
          this.workType = x.workType;
          this.workTypeId=x.workTypeId
          this.templateNumber = x.workTemplateNumber;
          this.workTemplateId = x.id;
          this.name = x.workTemplateName;
          this.templateCode = x.templateCode;
          this.strength = x.strength;
          this.subWorkType = x.subWorkType;
          this.subWorkTypeId=x.subWorkTypeId;
          this.templateId=x.templateId;
          this.durationInDays = x.workDurationInDays;
          this.generateDefaultRows(this.templateWithMilestone.milestones);
          this.disablecompletedmilestone();
        }
      });

    this.actions = [
      { icon: 'pi pi-trash', title: 'Delete', type: 'DELETE' },
      { icon: 'pi pi-times', title: 'Delete', type: 'INACTIVATE' },
    ];

this.templateForm = new FormGroup({
      
      name: new FormControl('', [
        Validators.required,
        Validators.maxLength(200),
        Validators.minLength(3),
      ]),
      durationInDays: new FormControl('', [
        Validators.required,
        Validators.min(1),
        Validators.max(3650),
      ]),
      workTypeId: new FormControl('', [Validators.required]),
      strength: new FormControl('', [Validators.required]),
      templateCode: new FormControl('', [Validators.required]),
      subWorkTypeId: new FormControl('', [Validators.required]),
      
    });

  }


  disablecompletedmilestone() {
    this.milestoneList.map((x) => {
      if (
        x.controls['percentageCompleted'].value &&
        x.controls['percentageCompleted'].value === 100
      ) {
        x.disable();
         this.isEditMode = true;
      }
    });
  }
  datechanged(Incomingindex: number) {
    let startdateglobal: any = null;
    let index = 0;
    if (!this.isUpdating) {
      this.isUpdating = true;
      this.milestoneList.map((x) => {
        index++;
        if (index > Incomingindex) {
          if (
            !x.controls['percentageCompleted'].value ||
            x.controls['percentageCompleted'].value !== 100
          ) {
            if (index != 1 && index != Incomingindex + 1) {
              x.controls['startDate'].patchValue(
                moment(startdateglobal).add(1, 'day').toDate()
              );
            }
            const startdate = x.controls['startDate'].value;
            startdateglobal = startdate;
            const durationInDays = x.controls['durationInDays'].value ?? 0;
            const ed = moment(startdate).add(durationInDays, 'days');
            x.controls['endDate'].patchValue(ed);
            startdateglobal = ed;
          } else {
            startdateglobal = x.controls['endDate'].value;
          }
        }
      });

      this.isUpdating = false;
    }
  }
  daychanged() {
    let startdateglobal: any = null;
    let index = 0;
    if (!this.isUpdating) {
      this.isUpdating = true;
      this.milestoneList.map((x) => {
        index++;
        if (
          !x.controls['percentageCompleted'].value ||
          x.controls['percentageCompleted'].value !== 100
        ) {
          if (index != 1) {
            x.controls['startDate'].patchValue(
              moment(startdateglobal).add(1, 'day').toDate()
            );
          }
          const startdate = x.controls['startDate'].value;
          startdateglobal = startdate;
          const durationInDays = x.controls['durationInDays'].value ?? 0;
          const ed = moment(startdate).add(durationInDays, 'days');
          x.controls['endDate'].patchValue(ed);
          startdateglobal = ed;
        } else {
          startdateglobal = x.controls['endDate'].value;
        }
      });

      this.isUpdating = false;
    }
  }
  generate() {
    this.tenderFacade.saveTemplates(this.gworkId, this.selectedTemplate);
  }
  changeRole(val: any) {
    this.showDetail = true;
    this.tenderFacade.templateWithMilestoneListGet(
      this.selectedTemplate,
      !this.defaultstatus,
      true,
      ''
    );
    this.milestoneForm.disable();
  }
  drop(event: CdkDragDrop<string[]>) {
    this.moveItemInArrayIfAllowed(
      this.milestoneList,
      event.previousIndex,
      event.currentIndex
    );
    let index = 0;
    this.t.controls.map((x) => {
      index++;
      (x as FormGroup).controls['orderNumber'].setValue(index);
    });
    this.daychanged();
  }
  private moveItemInArrayIfAllowed(
    array: any[],
    fromIndex: number,
    toIndex: number
  ): void {
    const from = this.clamp(fromIndex, array.length - 1);
    const to = this.clamp(toIndex, array.length - 1);

    if (from === to) {
      return;
    }

    const target = array[from];
    const delta = to < from ? -1 : 1;

    const affectedItems = array.filter((item, index) =>
      delta > 0 ? index >= from && index <= to : index >= to && index <= from
    );

    // If any of the items affected by the index changes is disabled
    // don't move any of the items.
    if (affectedItems.some((i) => i.disabled)) {
      return;
    }

    for (let i = from; i !== to; i += delta) {
      array[i] = array[i + delta];
    }

    array[to] = target;
  }
  private clamp(value: number, max: number): number {
    return Math.max(0, Math.min(max, value));
  }

  changeEvent(val: any) {
    if (!val.checked) {
      this.actions = [
        { icon: 'pi pi-trash', title: 'Delete', type: 'DELETE' },
        { icon: 'pi pi-times', title: 'Delete', type: 'INACTIVATE' },
      ];
    } else {
      this.actions = [
        { icon: 'pi pi-undo', title: 'Activate', type: 'ACTIVATE' },
      ];
    }
  }

  generateDefaultRows(templateMilestones: any[]) {
    this.t.clear();
    if (templateMilestones) {
      templateMilestones.forEach((x) => {
        this.t.push(
          this.formBuilder.group({
            id: [x.id],
            workTemplateId: [x.workTemplateId, [Validators.required]],
            milestoneName: [x.milestoneName, [Validators.required]],
            milestoneCode: [x.milestoneCode, [Validators.required]],
            orderNumber: [x.orderNumber, [Validators.required]],
            durationInDays: [
              x.durationInDays,
              [Validators.required, Validators.min(1)],
            ],
            startDate: [moment(x.startDate).toDate(), [Validators.required]],
            endDate: [moment(x.endDate).toDate(), [Validators.required]],
            isPaymentRequired: [x.isPaymentRequired, [Validators.required]],
            paymentPercentage: [x.paymentPercentage, [Validators.required]],
            percentageCompleted: [x.percentageCompleted, [Validators.required]],
            isActive: [true, [Validators.required]],
            isSubmitted: [false],
            workId: [x.workId],
            paymentStatus: [x.paymentStatus],
            milestoneStatus: [x.milestoneStatus],
            milestoneAmount: [x.milestoneAmount],
            isCompleted: [x.isCompleted],
            workTemplateName: [x.workTemplateName],
            strength: [x.strength],
            templateCode: [x.templateCode],
          })
        );
      });
    }
  }

  dc(date: any) {
    return dateconvertionwithOnlyDate(date);
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
            workId: [this.gworkId, [Validators.required]],
            workTemplateId: [this.workTemplateId, [Validators.required]],
            milestoneName: [null, [Validators.required]],
            milestoneCode: [null, [Validators.required]],
            orderNumber: [1, [Validators.required]],
            startDate: [null, [Validators.required]],
            endDate: [null, [Validators.required]],
            durationInDays: [null, [Validators.required, Validators.min(1)]],
            isPaymentRequired: [false, [Validators.required]],
            paymentPercentage: [0, [Validators.required]],
            percentageCompleted: [0],
            isActive: [true],
            isSubmitted: [false],
            paymentStatus: [null],
            milestoneStatus: [null],
            milestoneAmount: [0],
            isCompleted: [false],
            workTemplateName: [null],
            strength: [null],
            templateCode: [null],
          })
        );
      },
      reject: () => {},
    });
  }

  isPaymentRequired(event: any) {}

submit(isSubmit: boolean = true): void {
  // ✅ Step 1: Validate & process milestone form
  if (this.milestoneForm.valid) {
    this.isMilestoneSubmit = isSubmit;

    // Mark each milestone as submitted
    if (isSubmit) {
      this.milestoneList.forEach((milestone) => {
        milestone.controls['isSubmitted'].patchValue(isSubmit);
      });
    }

    // Extract & order milestones
    const milestones = this.milestoneForm.controls['milestones'].getRawValue();
    const orderedMilestones = milestones.map((m: TemplateMilestoneModel, index: number) => ({
      ...m,
      orderNumber: index + 1,
    }));

    // Save milestones
    this.tenderFacade.savemilestones(orderedMilestones);
  }

  // ✅ Step 2: Prepare role/template data
  const role: TemplateViewModel = {
    id: this.templateId || '',
    name: this.name,
    templateCode: this.templateCode,
    durationInDays: String(this.durationInDays),
    workId: this.gworkId,
    strength: this.strength,
    workTypeId: this.workTypeId || '',
    workTemplateId: this.workTemplateId,
    subWorkTypeId: this.subWorkTypeId || '',
    isActive: true
  };

  // Save template
  this.service.EditWorkTemplates(role).subscribe((response) => {
    

  
    if (response?.status === FailedStatus || response?.status === ErrorStatus) {
      this.messageService.add({
        severity: 'error',
        life: 8000,
        summary: 'Error',
        detail: response.message,
      });
      return;
    }


    // this.messageService.add({
    //   severity: 'success',
    //   summary: 'Success',
    //   detail: response?.message,
    // });
  
  });
}


  publish(event: Event) {
    this.confirmationService.confirm({
      key: 'confirm2',
      target: (event.target as HTMLInputElement) || new EventTarget(),
      message: 'Are you sure that you want to Publish?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {},
      reject: () => {},
    });
  }

  resetForm() {
    this.tenderFacade.getWorktemplate(this.gworkId);
  }
  backtoDoc() {
    this.router.navigate(['tender/generate', this.tender.tenderId], {});
  }
  back() {
    this.router.navigate(['tender']);
  }
  getdragdisableorNot(index: any) {
    var percentageCompleted = index.controls['percentageCompleted'];
    if (percentageCompleted.value && percentageCompleted.value === 100) {
      return true;
    }
    return false;
  }
  actioInvoked(action: string, record: number, event: Event) {
    if (action == 'INACTIVATE') {
      this.confirmationService.confirm({
        key: 'confirm2',
        target: (event.target as HTMLInputElement) || new EventTarget(),
        message: 'Are you sure that you want to Delete?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.t.removeAt(record);
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Deleted successfully',
          });
        },
        reject: () => {},
      });
    } else if (action == 'DELETE') {
      this.confirmationService.confirm({
        key: 'confirm2',
        target: (event.target as HTMLInputElement) || new EventTarget(),
        message: 'Are you sure that you want to Delete?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.t.removeAt(record);
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Deleted successfully',
          });
        },
        reject: () => {},
      });
    } else if (action == 'ACTIVATE') {
      this.confirmationService.confirm({
        key: 'confirm2',
        target: (event.target as HTMLInputElement) || new EventTarget(),
        message: 'Are you sure that you want to Activate?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          (this.t.controls[record] as FormGroup).controls['isActive'].setValue(
            true
          );
          this.submit(false);
        },
        reject: () => {},
      });
    }
  }
  ngOnDestroy() {
    this.tenderFacade.reset();
  }

  convertoWordsIND(amt: number) {
    return convertoWords(amt);
  }



  //Edit the template 
   Edit(event:any)
  {
   
        this.isEditMode = true;
  this.defaultstatus = false;
    
      }
     
  // TEMPLATE RE-ASSIGN

  //started
  delete(event:any)
  {
    this.confirmationService.confirm({
      key: 'confirm2',
      target: (event.target as HTMLInputElement) || new EventTarget(),
      message: '"Clicking on "Delete" will also delete milestones. Are you sure you want to proceed?"',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.Tservice.DeleteWorkTemplate(this.tender.workId).subscribe((x:ResponseModel)=>{
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
            location.reload();
            this.isNew=true;            
            this.tenderFacade.getTemplates(true);
            
          }
        })
    
      },
      reject: () => {},
    });
    
    
    
  }

  //ended
}