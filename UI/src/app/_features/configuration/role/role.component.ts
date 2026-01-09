import { Component } from '@angular/core';
import { RoleConfigFacade } from './state/role.facades';
import { MessageService } from 'primeng/api';
import { IRoleModel } from 'src/app/_models/configuration/role';
import { ActionModel, Actions, Column } from 'src/app/_models/datatableModel';
import { ErrorStatus, FailedStatus } from 'src/app/_models/ResponseStatus';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Guid } from 'guid-typescript';
import { ActivatedRoute, Router } from '@angular/router';
import { TitleCasePipe } from '@angular/common';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { privileges } from 'src/app/shared/commonFunctions';

@UntilDestroy()
@Component({
  selector: 'app-role',
  templateUrl: './role.component.html',
  styleUrls: ['./role.component.scss'],
  providers: [TitleCasePipe],
})
export class RoleComponent {
  configurationList!: IRoleModel[];
  cols!: Column[];
  catgories!: any[];
  searchableColumns!: string[];
  currentStatus: boolean = true;

  actions: Actions[] = [];
  title: string = 'Role';
  first: number = 0;
  rows: number = 25;
  total: number = 0;
  defaultSortField: string = 'roleName';
  defaultSortOrder: number = 1;

  privleges = privileges;
  roleForm!: FormGroup;
  constructor(
    private roleConfigFacade: RoleConfigFacade,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private titlecasePipe: TitleCasePipe
  ) {}
  ngOnInit() {
    this.roleConfigFacade.getRoles(this.currentStatus);

    this.roleForm = new FormGroup({
      id: new FormControl(Guid.raw()),
      roleName: new FormControl('', [
        Validators.required,
        Validators.maxLength(200),
        Validators.minLength(3),
      ]),
      code: new FormControl('', [
        Validators.required,
        Validators.maxLength(5),
        Validators.minLength(2),
      ]),
    });
    this.cols = [
      {
        field: 'roleName',
        header: 'Role Name',
        customExportHeader: 'Role Name',
        sortablefield: 'roleName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'roleCode',
        header: 'Role Code',
        sortablefield: 'roleCode',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'lastUpdatedUserName',
        header: 'Last Updated By',
        sortablefield: 'lastUpdatedUserName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'lastUpdatedDate',
        header: 'Last Updated Date',
        sortablefield: 'lastUpdatedDate',
        isSortable: true,
      },
    ];
    this.searchableColumns = this.cols
      .filter((x) => x.isSearchable == true)
      .flatMap((x) => x.field);

    this.actions = [
      {
        icon: 'pi pi-pencil',
        title: 'Edit',
        type: 'EDIT',
        privilege: this.privleges.ROLE_UPDATE,
        visibilityCheckFeild: 'isChangeable',
      },
      {
        icon: 'pi pi-times',
        title: 'In-Activate',
        type: 'INACTIVATE',
        privilege: this.privleges.ROLE_Delete,
        visibilityCheckFeild: 'isChangeable',
      },
    ];
    this.roleConfigFacade.selectRoles$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.configurationList = x;
        }
      });
    this.roleConfigFacade.selectSaveStatus$
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
          this.roleForm.get('id')?.patchValue(Guid.raw());
          this.roleForm.get('roleName')?.reset();
          this.roleForm.get('code')?.reset();
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
          this.roleConfigFacade.getRoles(this.currentStatus);
        }
      });
  }
  changeStatus(val: boolean) {
    this.roleConfigFacade.getRoles(!val);
    this.currentStatus = !val;
    if (!val) {
      this.actions = [
        {
          icon: 'pi pi-pencil',
          title: 'Edit',
          type: 'EDIT',
          privilege: this.privleges.ROLE_UPDATE,
          visibilityCheckFeild: 'isChangeable',
        },
        {
          icon: 'pi pi-times',
          title: 'In-Activate',
          type: 'INACTIVATE',
          privilege: this.privleges.ROLE_Delete,
          visibilityCheckFeild: 'isChangeable',
        },
      ];
    } else {
      this.actions = [
        {
          icon: 'pi pi-undo',
          title: 'Activate',
          type: 'ACTIVATE',
          privilege: this.privleges.ROLE_UPDATE,
        },
      ];
    }
  }
  resetForm() {
    this.roleForm.reset();
    this.roleForm.get('id')?.patchValue(Guid.raw());
  }
  actioInvoked(val: ActionModel) {
    if (val && val.type == 'INACTIVATE') {
      this.roleConfigFacade.updatesaveStatus();
      this.roleConfigFacade.saverole({
        ...val.record,
        isActive: false,
      });
    } else if (val && val.type == 'EDIT') {
      this.roleForm.get('id')?.patchValue(val.record.id);
      this.roleForm.get('roleName')?.patchValue(val.record.roleName);
      this.roleForm.get('code')?.patchValue(val.record.roleCode);
    } else if (val && val.type == 'ACTIVATE') {
      this.roleConfigFacade.updatesaveStatus();
      this.roleConfigFacade.saverole({
        ...val.record,
        isActive: true,
      });
    }
  }
  submit() {
    this.roleConfigFacade.updatesaveStatus();
    this.roleConfigFacade.saverole({
      id: this.roleForm.get('id')?.value,
      roleName: this.roleForm.get('roleName')?.value,
      roleCode: this.roleForm.get('code')?.value?.toUpperCase(),
      isActive: true,
    });
  }
  setPrivileges() {
    this.router.navigateByUrl('configuration/role/privileges');
  }
  ngOnDestroy() {
    this.roleConfigFacade.reset();
  }
}
