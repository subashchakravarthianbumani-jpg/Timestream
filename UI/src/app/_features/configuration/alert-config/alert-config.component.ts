import { TitleCasePipe } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TwoColConfigFacade } from '../two-column-config/state/facades/two.col.facades';
import { Router, ActivatedRoute } from '@angular/router';
import { ActionModel, Actions, Column } from 'src/app/_models/datatableModel';
import { TCModel } from 'src/app/_models/user/usermodel';
import { AlertConfigurationPrimaryModel } from 'src/app/_models/alert.model';
import { AlertConfigService } from 'src/app/_services/alert-config.sevice';
import { SuccessStatus } from 'src/app/_models/ResponseStatus';
import { privileges } from 'src/app/shared/commonFunctions';

@Component({
  selector: 'app-alert-config',
  templateUrl: './alert-config.component.html',
  styleUrls: ['./alert-config.component.scss'],
  providers: [ConfirmationService, MessageService, TitleCasePipe],
})
export class AlertConfigComponent {
  title: string = 'Alert Configuration';

  alertconfigs!: AlertConfigurationPrimaryModel[];
  cols!: Column[];
  catgories!: any[];
  searchableColumns!: string[];
  currentStatus: boolean = true;
  userGroups: TCModel[] = [];

  actions: Actions[] = [];
  first: number = 0;
  rows: number = 10;
  total: number = 0;
  defaultSortField: string = 'name';
  defaultSortOrder: number = 1;

  privleges = privileges;
  constructor(
    private messageService: MessageService,
    private titlecasePipe: TitleCasePipe,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private confirmationService: ConfirmationService,
    private alertConfigService: AlertConfigService,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder
  ) {}
  ngOnInit() {
    this.getAlertsconfig(true);
    this.cols = [
      {
        field: 'departmentName',
        header: 'Department Name',
        sortablefield: 'departmentName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'type',
        header: 'Type',
        sortablefield: 'type',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'object',
        header: 'Object',
        sortablefield: 'object',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'name',
        header: 'Name',
        sortablefield: 'name',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'alertNumber',
        header: 'Code',
        sortablefield: 'alertNumber',
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
        privilege: this.privleges.ALERT_CONFIG_UPDATE,
      },
      {
        icon: 'pi pi-times',
        title: 'In-Activate',
        type: 'INACTIVATE',
        privilege: this.privleges.ALERT_CONFIG_DELETE,
      },
    ];
  }
  submit() {}
  resetForm() {}
  addnewRow(event: any) {
    this.router.navigate(['configuration', 'alert-config-create', 0], {});
  }

  actioInvoked(val: ActionModel) {
    if (val && val.type == 'INACTIVATE') {
      this.saveupdateAlertconfig({ ...val.record, isActive: false });
    } else if (val && val.type == 'EDIT') {
      this.router.navigate([
        'configuration',
        'alert-config-create',
        val.record.id,
      ]);
    } else if (val && val.type == 'ACTIVATE') {
      this.saveupdateAlertconfig({ ...val.record, isActive: true });
    }
  }
  changeStatus(val: boolean) {
    this.getAlertsconfig(!val);
    this.currentStatus = !val;
    if (!val) {
      this.actions = [
        {
          icon: 'pi pi-pencil',
          title: 'Edit',
          type: 'EDIT',
          privilege: this.privleges.ALERT_CONFIG_UPDATE,
        },
        {
          icon: 'pi pi-times',
          title: 'In-Activate',
          type: 'INACTIVATE',
          privilege: this.privleges.ALERT_CONFIG_DELETE,
        },
      ];
    } else {
      this.actions = [
        {
          icon: 'pi pi-undo',
          title: 'Activate',
          type: 'ACTIVATE',
          privilege: this.privleges.ALERT_CONFIG_UPDATE,
        },
      ];
    }
  }
  getAlertsconfig(val: boolean) {
    this.alertConfigService.getAlertconfig('', '', '', val).subscribe(
      (x) => {
        if (x.status == SuccessStatus) {
          this.alertconfigs = x.data;
          //this.getalertSecondarydetails(this.alertDetails?.id ?? '');
        }
      },
      (error) => {}
    );
  }
  saveupdateAlertconfig(model: AlertConfigurationPrimaryModel) {
    this.alertConfigService.saveAlertCongig(model).subscribe(
      (x) => {
        if (x.status == SuccessStatus) {
          this.getAlertsconfig(this.currentStatus);
        }
      },
      (error) => {}
    );
  }
}
