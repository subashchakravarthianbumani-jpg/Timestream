import { ChangeDetectorRef, Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Guid } from 'guid-typescript';
import { UserConfigFacade } from '../state/user.facades';
import { MessageService } from 'primeng/api';
import { FailedStatus, ErrorStatus } from 'src/app/_models/ResponseStatus';
import { Router, ActivatedRoute } from '@angular/router';
import { TitleCasePipe } from '@angular/common';
import { Subscription } from 'rxjs';
import { AccountUserViewModel } from 'src/app/_models/user/usermodel';
import * as moment from 'moment';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { privileges } from 'src/app/shared/commonFunctions';

@UntilDestroy()
@Component({
  selector: 'app-user-create',
  templateUrl: './user-create.component.html',
  styleUrls: ['./user-create.component.scss'],
  providers: [TitleCasePipe],
})
export class UserCreateComponent {
  userId!: string;
  title: string = 'User Create';
  userForm!: FormGroup;
  userDetails?: any;
  roleList!: any[];
  divisionList!: any[];
  userGroupList!: any[];
  departmentList!: any[];
  districtList!: any[];
  defaultDate = moment(new Date(1990, 1 - 1, 1)).toDate();

  routeSub!: Subscription;

  privleges = privileges;
  isNewForm: boolean = false;
  constructor(
    private userConfigFacade: UserConfigFacade,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private titlecasePipe: TitleCasePipe,
    private cdr: ChangeDetectorRef
  ) {}
  back() {
    this.userConfigFacade.updatesaveStatus();
    this.router.navigate(['user']);
  }
  ngOnDestroy() {
    this.routeSub.unsubscribe();
    this.userForm.reset();
  }
  ngOnInit() {
    this.routeSub = this.route.params
      .pipe(untilDestroyed(this))
      .subscribe((params) => {
        this.userId = params['id']; //log the value of id
        if (this.userId !== '0') {
          this.userConfigFacade.getUsers(true, this.userId);
        } else {
          this.isNewForm = true;
          this.userConfigFacade.getUsers(true, Guid.raw());
        }
      });

    this.userForm = new FormGroup({
      id: new FormControl(Guid.raw()),
      userNumber: new FormControl(''),
      firstName: new FormControl('', [
        Validators.required,
        Validators.maxLength(100),
        Validators.minLength(3),
      ]),
      lastName: new FormControl('', [
        Validators.required,
        Validators.maxLength(100),
        Validators.minLength(1),
      ]),
      email: new FormControl('', [Validators.required, Validators.email]),
      isActive: new FormControl(''),
      roleId: new FormControl('', [Validators.required]),
      mobile: new FormControl('', [
        Validators.required,
        Validators.maxLength(10),
        Validators.minLength(10),
      ]),
      divisionIdList: new FormControl('', [Validators.required]),
      userGroup: new FormControl('', [Validators.required]),
      dOB: new FormControl('', [Validators.required]),
      districtIdList: new FormControl('', [Validators.required]),
      departmentIdList: new FormControl('', [Validators.required]),
      password: new FormControl('', [
        Validators.required,
        Validators.maxLength(15),
        Validators.minLength(8),
      ]),
      pofileImageId: new FormControl(''),
    });

    this.userConfigFacade.getRoles(true);
    this.userConfigFacade.userFormGet();

    this.userConfigFacade.selectUsers$
      .pipe(untilDestroyed(this))
      .subscribe((x: any) => {
        if (x) {
          this.userDetails = x[0];

          this.userForm.get('id')?.patchValue(this.userDetails.userId);
          this.userForm
            .get('userNumber')
            ?.patchValue(this.userDetails.userNumber);
          this.userForm
            .get('firstName')
            ?.patchValue(this.userDetails.firstName);
          this.userForm.get('lastName')?.patchValue(this.userDetails.lastName);
          this.userForm.get('email')?.patchValue(this.userDetails.email);
          this.userForm.get('isActive')?.patchValue(this.userDetails.isActive);
          this.userForm.get('roleId')?.patchValue(this.userDetails.roleId);
          this.userForm.get('mobile')?.patchValue(this.userDetails.mobile);
          this.userForm
            .get('departmentIdList')
            ?.patchValue(this.userDetails.departmentIdList);
          this.userForm
            .get('divisionIdList')
            ?.patchValue(this.userDetails.divisionIdList);
          this.userForm
            .get('userGroup')
            ?.patchValue(this.userDetails.userGroup);
          this.userForm
            .get('dOB')
            ?.patchValue(moment(this.userDetails.dob).toDate());
          this.userForm
            .get('districtIdList')
            ?.patchValue(this.userDetails.districtIdList);
          this.userForm
            .get('pofileImageId')
            ?.patchValue(this.userDetails.pofileImageId);
          this.userForm.get('password')?.patchValue(this.userDetails.password);
          this.cdr.detectChanges();
        } else {
          this.userDetails = null;
          this.userForm.reset();
          this.userForm.get('id')?.patchValue(Guid.raw());
        }
      });
    this.userConfigFacade.selectUserRoles$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.roleList = x;
        }
      });
    this.userConfigFacade.selectdistrictList$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.districtList = x;
        }
      });
    this.userConfigFacade.selectdivisionList$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.divisionList = x;
        }
      });
    this.userConfigFacade.selectuserGroupList$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.userGroupList = x;
        }
      });
    this.userConfigFacade.selectdepartmentList$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.departmentList = x;
        }
      });

    this.userConfigFacade.selectSaveStatus$
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
          this.resetForm();
        }
      });
    this.userConfigFacade.selectEmailStatus$
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
            detail: 'Sent Successfully',
          });
        }
      });
  }
  submit() {
    this.userConfigFacade.updatesaveStatus();
    this.userConfigFacade.saveuser({
      userId: this.userForm.get('id')?.value,
      firstName: this.userForm.get('firstName')?.value,
      lastName: this.userForm.get('lastName')?.value,
      divisionIdList: this.userForm.get('divisionIdList')?.value,
      dOB: this.userForm.get('dOB')?.value,
      email: this.userForm.get('email')?.value,
      departmentIdList: this.userForm.get('departmentIdList')?.value,
      roleId: this.userForm.get('roleId')?.value,
      mobile: this.userForm.get('mobile')?.value.toString(),
      userGroup: this.userForm.get('userGroup')?.value,
      districtIdList: this.userForm.get('districtIdList')?.value,
      password: this.userForm.get('password')?.value,
      isActive: true,
    });
  }
  resetForm() {
    this.userConfigFacade.updatesaveStatus();
    this.router.navigate(['user']);
  }
  sendRegistration() {
    this.userConfigFacade.sendEmail({
      userId: this.userForm.get('id')?.value,
      firstName: this.userForm.get('firstName')?.value,
      lastName: this.userForm.get('lastName')?.value,
      divisionIdList: this.userForm.get('divisionIdList')?.value,
      dOB: this.userForm.get('dOB')?.value,
      email: this.userForm.get('email')?.value,
      departmentIdList: this.userForm.get('departmentIdList')?.value,
      roleId: this.userForm.get('roleId')?.value,
      mobile: this.userForm.get('mobile')?.value.toString(),
      userGroup: this.userForm.get('userGroup')?.value,
      password: this.userForm.get('password')?.value,
      isActive: true,
      districtIdList: this.userForm.get('districtIdList')?.value,
    });
  }
}
