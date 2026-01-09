import { TitleCasePipe } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { guid } from '@fullcalendar/core/internal';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { Guid } from 'guid-typescript';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { SuccessStatus } from 'src/app/_models/ResponseStatus';
import {
  AlertConfigurationFormModel,
  AlertConfigurationPrimaryModel,
  AlertConfigurationSecondaryViewModel,
} from 'src/app/_models/alert.model';
import { TCModel } from 'src/app/_models/user/usermodel';
import { AlertConfigService } from 'src/app/_services/alert-config.sevice';
import { privileges } from 'src/app/shared/commonFunctions';

@UntilDestroy()
@Component({
  selector: 'app-alert-config-create',
  templateUrl: './alert-config-create.component.html',
  styleUrls: ['./alert-config-create.component.scss'],
  providers: [TitleCasePipe],
})
export class AlertConfigCreateComponent {
  alertconfigId!: string;
  title: string = 'Alert Config Create';
  alertconfigForm!: FormGroup;
  alertDetails?: AlertConfigurationPrimaryModel;
  alertSecondaryDetails?: AlertConfigurationSecondaryViewModel;
  departmentList!: TCModel[];
  typeList!: TCModel[];
  objectList!: TCModel[];
  severityList!: TCModel[];
  fieldList!: TCModel[];
  baseList!: TCModel[];
  calculationList!: TCModel[];
  frequencyTypeList!: TCModel[];
  usergroupList!: TCModel[];
  formdets!: AlertConfigurationFormModel;
  isNew: boolean = true;

  routeSub!: Subscription;
  privleges = privileges;

  get ef() {
    return this.alertconfigForm.controls;
  }
  get et() {
    return this.ef['alertConfigurationSecondary'] as FormArray;
  }
  get severityBasedList() {
    return this.et.controls as FormGroup[];
  }
  get subtypeErrorMsg() {
    var error: any;
    this.severityBasedList.map((x) => {
      if (x.controls['severity'].errors) {
        error = x.controls['severity'].errors;
      }
    });
    if (error && error['required']) {
      return 'Required';
    }
    return null;
  }
  get fieldErrorMsg() {
    var error: any;
    this.severityBasedList.map((x) => {
      if (x.controls['field'].errors) {
        error = x.controls['field'].errors;
      }
    });
    if (error && error['required']) {
      return 'Required';
    }
    return null;
  }
  get baseFieldErrorMsg() {
    var error: any;
    this.severityBasedList.map((x) => {
      if (x.controls['baseField'].errors) {
        error = x.controls['baseField'].errors;
      }
    });
    if (error && error['required']) {
      return 'Required';
    }
    return null;
  }
  get calculationTypeErrorMsg() {
    var error: any;
    this.severityBasedList.map((x) => {
      if (x.controls['calculationType'].errors) {
        error = x.controls['calculationType'].errors;
      }
    });
    if (error && error['required']) {
      return 'Required';
    }
    return null;
  }
  get calculationNoErrorMsg() {
    var error: any;
    this.severityBasedList.map((x) => {
      if (x.controls['calculationNo'].errors) {
        error = x.controls['calculationNo'].errors;
      }
    });
    if (error && error['required']) {
      return 'Required';
    }
    return null;
  }

  constructor(
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private titlecasePipe: TitleCasePipe,
    private cdr: ChangeDetectorRef,
    private alertConfigService: AlertConfigService
  ) {}

  ngOnInit() {
    this.getalertcreateFormdetails();
    this.routeSub = this.route.params
      .pipe(untilDestroyed(this))
      .subscribe((params) => {
        this.alertconfigId = params['id']; //log the value of id
        if (this.alertconfigId !== '0') {
          this.isNew = false;
          this.getalertcreatedetails(this.alertconfigId);
        } else {
          this.isNew = true;
          this.alertconfigId = Guid.raw();
          this.getalertcreatedetails(this.alertconfigId);
        }
      });
    this.alertconfigForm = new FormGroup({
      id: new FormControl(Guid.raw()),
      department: new FormControl('', [Validators.required]),
      type: new FormControl('', [Validators.required]),
      object: new FormControl('', [Validators.required]),
      name: new FormControl('', [
        Validators.required,
        Validators.maxLength(200),
        Validators.minLength(3),
      ]),
      alertNumber: new FormControl(''),
      emailfrequency: new FormControl(''),
      smsfrequency: new FormControl(''),
      emailuserGroupList: new FormControl(''),
      smsuserGroupsList: new FormControl(''),
      isActive: new FormControl(true),
      alertConfigurationSecondary: new FormArray([]),
    });
  }
  generateseverityrows(serverities: AlertConfigurationSecondaryViewModel[]) {
    this.et.clear();
    if (serverities) {
      serverities.forEach((x) => {
        this.et.push(
          this.formBuilder.group({
            id: [x.id],
            primaryId: [x.primaryId, [Validators.required]],
            severity: [x.severity, [Validators.required]],
            field: [x.field, [Validators.required]],
            baseField: [x.baseField, [Validators.required]],
            calculationType: [x.calculationType, [Validators.required]],
            calculationNo: [x.calculationNo, [Validators.required]],
            isActive: [true, [Validators.required]],
          })
        );
        this.cdr.markForCheck();
        this.cdr.detectChanges();
      });
    }
  }
  Generatedefaultrows() {
    this.et.clear();
    if (this.severityList) {
      this.severityList.forEach((x) => {
        this.et.push(
          this.formBuilder.group({
            id: [Guid.raw()],
            primaryId: [this.alertconfigId, [Validators.required]],
            severity: [x.value, [Validators.required]],
            field: [null, [Validators.required]],
            baseField: [null, [Validators.required]],
            calculationType: [null, [Validators.required]],
            calculationNo: [null, [Validators.required]],
            isActive: [true, [Validators.required]],
          })
        );
      });
      this.cdr.reattach();
      this.cdr.detectChanges();
    }
  }
  getalertcreatedetails(id: string) {
    this.alertConfigService.getAlertconfig(id).subscribe(
      (x) => {
        if (x.status == SuccessStatus && x && x.data.length > 0) {
          this.alertDetails = x.data[0];
          this.setformDetails();
          if (
            this.alertDetails?.alertConfigurationSecondary &&
            this.alertDetails?.alertConfigurationSecondary.length > 0
          ) {
            this.generateseverityrows(
              this.alertDetails?.alertConfigurationSecondary
            );
          } else {
            this.Generatedefaultrows();
          }
        } else {
          this.Generatedefaultrows();
        }
      },
      (error) => {}
    );
  }
  setformDetails() {
    this.alertconfigForm.get('id')?.patchValue(this.alertDetails?.id);
    this.alertconfigForm
      .get('department')
      ?.patchValue(this.alertDetails?.department);
    this.alertconfigForm.get('type')?.patchValue(this.alertDetails?.type);
    this.alertconfigForm.get('object')?.patchValue(this.alertDetails?.object);
    this.alertconfigForm.get('name')?.patchValue(this.alertDetails?.name);
    this.alertconfigForm
      .get('alertNumber')
      ?.patchValue(this.alertDetails?.alertNumber);
    this.alertconfigForm
      .get('emailfrequency')
      ?.patchValue(this.alertDetails?.emailFrequency);
    this.alertconfigForm
      .get('smsfrequency')
      ?.patchValue(this.alertDetails?.smsFrequency);
    this.alertconfigForm
      .get('emailuserGroupList')
      ?.patchValue(this.alertDetails?.emailuserGroupList);
    this.alertconfigForm
      .get('smsuserGroupsList')
      ?.patchValue(this.alertDetails?.smsuserGroupsList);
  }
  getalertcreateFormdetails() {
    this.alertConfigService.getAlertconfigForm().subscribe(
      (x) => {
        if (x.status == SuccessStatus) {
          this.formdets = x.data;
          this.departmentList = this.formdets.departmentList;
          this.typeList = this.formdets.typeList;
          this.objectList = this.formdets.objectList;
          this.severityList = this.formdets.severityList;
          this.fieldList = this.formdets.fieldList;
          this.baseList = this.formdets.baseFieldList;
          this.calculationList = this.formdets.calculationTypeList;
          this.usergroupList = this.formdets.userGroupList;
          this.frequencyTypeList = this.formdets.frequencyTypeList;
          if (this.et.length == 0) {
            this.Generatedefaultrows();
          }
        }
      },
      (error) => {}
    );
  }

  back() {
    this.router.navigate(['configuration', 'alert-config']);
  }
  submit() {
    var df = this.alertconfigForm.value;
    df.emailuserGroupList =
      df.emailuserGroupList == '' ? null : df.emailuserGroupList;
    df.smsuserGroupsList =
      df.smsuserGroupsList == '' ? null : df.smsuserGroupsList;

    this.alertConfigService.saveAlertCongig(df).subscribe(
      (x) => {
        if (x.status == SuccessStatus) {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
          this.router.navigate(['configuration', 'alert-config']);
        }
      },
      (error) => {}
    );
  }
  resetForm() {
    this.router.navigate(['configuration', 'alert-config']);
  }
}
