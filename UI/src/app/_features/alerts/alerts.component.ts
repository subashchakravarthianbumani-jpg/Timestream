import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ConfirmationService, MenuItem, MessageService } from 'primeng/api';
import {
  FailedStatus,
  ErrorStatus,
  ResponseModel,
} from 'src/app/_models/ResponseStatus';
import { AlertMasterModel } from 'src/app/_models/dashboard.model';
import { AlertFilterModel } from 'src/app/_models/filterRequest';
import { DashboardService } from 'src/app/_services/dashboard.service';
import { LogHistoryService } from 'src/app/_services/logs.service';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.scss'],
})
export class AlertsComponent {
  title: string = 'Alerts';

  filterModel: AlertFilterModel = {
    columnSearch: null,
    divisionIds: null,
    roleId: null,
    searchString: null,
    skip: 0,
    take: 99999999,
    sorting: null,
    typeIds: null,
    types: null,
  };
  result: AlertMasterModel[] = [];
  highAlerts: AlertMasterModel[] = [];
  moderateAlerts: AlertMasterModel[] = [];
  divisionIds!: string[];

  amountupdateForm!: FormGroup;
  amountmodalvisible = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private dashboardService: DashboardService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.getalerts();
    this.amountupdateForm = new FormGroup({
      Id: new FormControl('', Validators.required),
      updatedNote: new FormControl('', Validators.required),
    });
  }
  getalerts() {
    this.dashboardService.getAlerts(this.filterModel).subscribe((x) => {
      this.result = x.data;
      this.highAlerts = this.result.filter((x) => x.severity == 'Red');
      this.moderateAlerts = this.result.filter((x) => x.severity == 'Yellow');
    });
  }
  resolve(val: any, event: Event) {
    this.confirmationService.confirm({
      key: 'confirm2',
      target: (event.target as HTMLInputElement) || new EventTarget(),
      message: 'Are you sure that you want to resolve?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.amountupdateForm.get('Id')?.patchValue(val);
        this.amountmodalvisible = true;
      },
      reject: () => {
        // this.messageService.add({
        //   severity: 'error',
        //   summary: 'Rejected',
        //   detail: 'You have rejected',
        // });
      },
    });
  }
  amountUpdateSubmit() {
    this.dashboardService
      .alert_Resolve({
        id: this.amountupdateForm.get('Id')?.value,
        updatedNote: this.amountupdateForm.get('updatedNote')?.value,
      })
      .subscribe((x: ResponseModel) => {
        if (
          x &&
          x.data &&
          (x.status == FailedStatus || x.status == ErrorStatus)
        ) {
          this.messageService.add({
            severity: 'error',
            life: 80000,
            summary: 'Error',
            detail: x.message,
          });
          this.amountmodalvisible = false;
          this.amountupdateForm.reset();
          this.getalerts();
        } else {
          this.messageService.add({
            severity: 'success',
            life: 80000,
            summary: 'Success',
            detail: x.message,
          });
          this.amountmodalvisible = false;
          this.amountupdateForm.reset();
          this.getalerts();
        }
      });
  }

  resetform() {
    this.amountmodalvisible = false;
    this.amountupdateForm.reset();
  }
}
