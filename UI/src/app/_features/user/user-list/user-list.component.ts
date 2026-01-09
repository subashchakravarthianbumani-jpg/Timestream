import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ActionModel, Actions, Column } from 'src/app/_models/datatableModel';
import { AccountUserViewModel } from 'src/app/_models/user/usermodel';
import { UserConfigFacade } from '../state/user.facades';
import { MessageService } from 'primeng/api';
import { FailedStatus, ErrorStatus } from 'src/app/_models/ResponseStatus';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import {
  dateconvertion,
  dateconvertionwithOnlyDate,
  privileges,
} from 'src/app/shared/commonFunctions';
import {
  MBookFilterModel,
  TableFilterModel,
  UserFilterModel,
} from 'src/app/_models/filterRequest';
import { d } from 'src/assets/base64img/logo';

@UntilDestroy()
@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss'],
  providers: [],
})
export class UserListComponent {
  configurationList!: AccountUserViewModel[];
  cols!: Column[];
  catgories!: any[];
  searchableColumns!: string[];
  currentStatus: boolean = true;

  actions: Actions[] = [];
  title: string = 'User';
  first: number = 0;
  rows: number = 25;
  total: number = 0;
  defaultSortField: string = 'userNumber';
  defaultSortOrder: number = 1;
  privleges = privileges;

  filtermodel!: UserFilterModel;
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private userConfigFacade: UserConfigFacade,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.filtermodel = {
      skip: 0,
      sorting: { fieldName: 'userNumber', sort: 'ASC' },
      take: 25,
      where: { isActive: true },
      searchString: null,
      columnSearch: null,
    };
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
          this.userConfigFacade.updatesaveStatus();
          this.filtermodel = {
            ...this.filtermodel,
            where: { isActive: this.currentStatus },
          };
          this.userConfigFacade.getUsersServerPaging(this.filtermodel);
        }
      });

    this.filtermodel = {
      ...this.filtermodel,
      where: { isActive: this.currentStatus },
    };
    this.userConfigFacade.getUsersServerPaging(this.filtermodel);
    this.cols = [
      {
        field: 'userNumber',
        header: 'User Id',
        customExportHeader: 'User Id',
        sortablefield: 'userNumber',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'firstName',
        header: 'First Name',
        sortablefield: 'firstName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'lastName',
        header: 'Last Name',
        sortablefield: 'lastName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'userGroupName',
        header: 'User Group',
        sortablefield: 'userGroupName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'roleName',
        header: 'Role',
        sortablefield: 'roleName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'department',
        header: 'Department',
        sortablefield: 'department',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'division',
        header: 'Division',
        sortablefield: 'division',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'email',
        header: 'Email',
        sortablefield: 'email',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'mobile',
        header: 'Mobile',
        sortablefield: 'mobile',
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
        field: 'lastUpdatedDatestring',
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
        isIcon: true,
        privilege: this.privleges.USER_UPDATE,
      },
      {
        icon: 'pi pi-times',
        title: 'In-Activate',
        type: 'INACTIVATE',
        isIcon: true,
        privilege: this.privleges.USER_Delete,
      },
    ];

    this.userConfigFacade.selectUsers$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.configurationList = []; // x.data;
          x.data?.forEach((x: AccountUserViewModel) => {
            this.configurationList.push({
              ...x,
              lastUpdatedDatestring: this.dcwt(x.lastUpdatedDate),
            });
          });
          this.total = x.totalRecordCount;
        }
      });
  }

  changefilter(val: TableFilterModel) {
    this.filtermodel = { ...this.filtermodel, ...val };
    this.userConfigFacade.getUsersServerPaging(this.filtermodel);
  }
  changeStatus(val: boolean) {
    this.filtermodel = { ...this.filtermodel, where: { isActive: !val } };
    this.userConfigFacade.getUsersServerPaging(this.filtermodel);
    this.currentStatus = !val;
    if (!val) {
      this.actions = [
        {
          icon: 'pi pi-pencil',
          title: 'Edit',
          type: 'EDIT',
          isIcon: true,
          privilege: this.privleges.USER_UPDATE,
        },
        {
          icon: 'pi pi-times',
          title: 'In-Activate',
          type: 'INACTIVATE',
          isIcon: true,
          privilege: this.privleges.USER_Delete,
        },
      ];
    } else {
      this.actions = [
        {
          icon: 'pi pi-undo',
          title: 'Activate',
          type: 'ACTIVATE',
          isIcon: true,
          privilege: this.privleges.USER_UPDATE,
        },
      ];
    }
  }
  actioInvoked(val: ActionModel) {
    if (val && val.type == 'INACTIVATE') {
      this.userConfigFacade.updatesaveStatus();
      this.userConfigFacade.saveuser({ ...val.record, isActive: false });
      this.filtermodel = {
        ...this.filtermodel,
        where: { isActive: this.currentStatus },
      };
      this.userConfigFacade.getUsersServerPaging(this.filtermodel);
    } else if (val && val.type == 'EDIT') {
      this.router.navigate(['create', val.record.userId], {
        relativeTo: this.route,
      });
    } else if (val && val.type == 'ACTIVATE') {
      this.userConfigFacade.updatesaveStatus();
      this.userConfigFacade.saveuser({ ...val.record, isActive: true });
      this.filtermodel = {
        ...this.filtermodel,
        where: { isActive: this.currentStatus },
      };
      this.userConfigFacade.getUsersServerPaging(this.filtermodel);
    }
  }
  usercreate() {
    this.router.navigate(['create', 0], { relativeTo: this.route });
  }
  ngOnDestroy() {
    this.userConfigFacade.reset();
  }
  dc(val: any) {
    return dateconvertionwithOnlyDate(val);
  }
  dcwt(val: any) {
    return dateconvertion(val);
  }
}
