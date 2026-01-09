import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Guid } from 'guid-typescript';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { ActionModel, Actions } from 'src/app/_models/datatableModel';
import { TemplatesConfigFacade } from '../templates/state/template.facades';
import {
  TemplateMilestoneModel,
  TemplatewithMilestoneViewModel,
} from 'src/app/_models/configuration/temp-milestone';
import { ErrorStatus, FailedStatus } from 'src/app/_models/ResponseStatus';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { privileges } from 'src/app/shared/commonFunctions';

@UntilDestroy()
@Component({
  selector: 'app-template-milestone',
  templateUrl: './template-milestone.component.html',
  styleUrls: ['./template-milestone.component.scss'],
  providers: [ConfirmationService],
})
export class TemplateMilestoneComponent {
  title: string = 'Milestone';
  milestoneForm!: FormGroup;
  defaultstatus = false;
  onLabel: string = 'Show Active';
  offLabel: string = 'Show In-Active';
  actions: Actions[] = [];

  isPublish = false;
  templateId!: string;
  templateWithMilestone?: TemplatewithMilestoneViewModel;
  privleges = privileges;
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
    } else if (
      this.totaldays != (this.templateWithMilestone?.durationInDays ?? 0)
    ) {
      return `Sum of days should be ${
        this.templateWithMilestone?.durationInDays ?? 0
      }`;
    }

    return null;
  }
  get paymentreqErrorMsg() {
    var error: any;
    this.milestoneList.map((x) => {
      if (x.controls['paymentPercentage'].errors) {
        error = x.controls['paymentPercentage'].errors;
      }
    });

    if (this.defaultstatus) {
      return null;
    }
    if (error && error['required']) {
      return 'Required';
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
    private templateConfigFacade: TemplatesConfigFacade
  ) {}

  ngOnInit() {
    this.isPublish = false;
    this.route.params.subscribe((params) => {
      this.templateId = params['id'];
      //console.log(params); //log the entire params object
      //console.log(params['id']); //log the value of id
    });
    this.templateConfigFacade.templateWithMilestoneListGet(
      this.templateId,
      !this.defaultstatus,
      true,
      ''
    );
    this.milestoneForm = this.formBuilder.group({
      milestones: new FormArray([]),
    });
    this.templateConfigFacade.selectTemplateWithMilestones$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.templateWithMilestone = x;
          this.generateDefaultRows(
            this.templateWithMilestone.templateMilestones
          );
        }
      });
    this.actions = [
      { icon: 'pi pi-trash', title: 'Delete', type: 'DELETE' },
      { icon: 'pi pi-times', title: 'In-Activate', type: 'INACTIVATE' },
    ];
    this.templateConfigFacade.selectmilestoneSaveStatus$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x && (x.Status == FailedStatus || x.Status == ErrorStatus)) {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            life: 80000,
            detail: x.message,
          });

          this.templateConfigFacade.updatemilestonessaveStatus();
          this.templateConfigFacade.templateWithMilestoneListGet(
            this.templateId,
            !this.defaultstatus,
            true,
            ''
          );
        } else if (x) {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
          this.templateConfigFacade.updatemilestonessaveStatus();
          if (this.isPublish) {
            this.router.navigateByUrl('configuration/templates');
          }
          this.templateConfigFacade.templateWithMilestoneListGet(
            this.templateId,
            !this.defaultstatus,
            true,
            ''
          );
        }
      });
  }

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(
      this.milestoneList,
      event.previousIndex,
      event.currentIndex
    );
    let index = 0;
    this.t.controls.map((x) => {
      index++;
      (x as FormGroup).controls['orderNumber'].setValue(index);
    });
  }

  changeEvent(val: any) {
    this.templateConfigFacade.templateWithMilestoneListGet(
      this.templateId,
      !this.defaultstatus,
      true,
      ''
    );
    if (!val.checked) {
      this.actions = [
        { icon: 'pi pi-trash', title: 'Delete', type: 'DELETE' },
        { icon: 'pi pi-times', title: 'In-Activate', type: 'INACTIVATE' },
      ];
    } else {
      this.actions = [
        { icon: 'pi pi-undo', title: 'Activate', type: 'ACTIVATE' },
      ];
    }
  }

  generateDefaultRows(templateMilestones: TemplateMilestoneModel[]) {
    this.t.clear();
    if (templateMilestones) {
      templateMilestones.forEach((x) => {
        this.t.push(
          this.formBuilder.group({
            id: [x.id],
            templateId: [x.templateId, [Validators.required]],
            milestoneName: [x.milestoneName, [Validators.required]],
            orderNumber: [x.orderNumber, [Validators.required]],
            milestoneCode: [x.milestoneCode, [Validators.required]],
            durationInDays: [x.durationInDays, [Validators.required]],
            isPaymentRequired: [x.isPaymentRequired, [Validators.required]],
            paymentPercentage: [x.paymentPercentage, [Validators.required]],
            isActive: [x.isActive, [Validators.required]],
            isPublished: [false, [Validators.required]],
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
            templateId: [this.templateId, [Validators.required]],
            milestoneName: [null, [Validators.required]],
            milestoneCode: [null, [Validators.required]],
            orderNumber: [1, [Validators.required]],
            durationInDays: [null, [Validators.required]],
            isPaymentRequired: [false, [Validators.required]],
            paymentPercentage: [0, [Validators.required]],
            isActive: [true, [Validators.required]],
            isPublished: [false, [Validators.required]],
          })
        );
      },
      reject: () => {},
    });
  }

  isPaymentRequired(event: any) {}

  submit(ispublish: boolean = false) {
    const milestones = this.milestoneForm.controls['milestones'].value;
    var order = 1;
    this.isPublish = ispublish;
    milestones.forEach((x: TemplateMilestoneModel) => {
      x.orderNumber = order;
      x.isPublished = ispublish;
      order++;
    });
    this.templateConfigFacade.savemilestones(milestones);
  }

  publish(event: Event) {
    this.confirmationService.confirm({
      key: 'confirm2',
      target: (event.target as HTMLInputElement) || new EventTarget(),
      message: 'Are you sure that you want to Publish?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.submit(true);
      },
      reject: () => {},
    });
  }

  resetForm() {
    this.router.navigateByUrl('configuration/templates');
  }

  back() {
    this.router.navigateByUrl('configuration/templates');
  }

  actioInvoked(action: string, record: number, event: Event) {
    if (action == 'INACTIVATE') {
      this.confirmationService.confirm({
        key: 'confirm2',
        target: (event.target as HTMLInputElement) || new EventTarget(),
        message: 'Are you sure that you want to InActivate?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          (this.t.controls[record] as FormGroup).controls['isActive'].setValue(
            false
          );
          this.submit();
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
          this.submit();
        },
        reject: () => {},
      });
    }
  }
  ngOnDestroy() {
    this.templateConfigFacade.reset();
  }
}
