import { ChangeDetectorRef, Component } from '@angular/core';
import { ConfirmationService, MessageService, SortEvent } from 'primeng/api';
import { Product } from 'src/app/demo/api/product';
import { ProductService } from 'src/app/demo/service/product.service';
import { TwoColConfigFacade } from './state/facades/two.col.facades';
import * as FileSaver from 'file-saver';
import { ActionModel, Actions, Column } from 'src/app/_models/datatableModel';
import { first } from 'rxjs';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import {
  IConfigCategoryModel,
  IConfigurationModel,
} from 'src/app/_models/configuration/configuration';
import { guid } from '@fullcalendar/core/internal';
import { Guid } from 'guid-typescript';
import { ErrorStatus, FailedStatus } from 'src/app/_models/ResponseStatus';
import { TitleCasePipe, UpperCasePipe } from '@angular/common';
import { TCModel } from 'src/app/_models/user/usermodel';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { privileges } from 'src/app/shared/commonFunctions';

@UntilDestroy()
@Component({
  selector: 'app-two-column-config',
  templateUrl: './two-column-config.component.html',
  styleUrls: ['./two-column-config.component.scss'],
  providers: [ConfirmationService, MessageService, TitleCasePipe],
})
export class TwoColumnConfigComponent {
  configurationList!: IConfigurationModel[];
  departments!: TCModel[];
  cols!: Column[];
  catgories!: any[];
  originalcatgories!: any[];
  configurations!: any[];
  searchableColumns!: string[];

  actions: Actions[] = [];
  isDependent: boolean = false;
  title: string = 'Configuration';
  first: number = 0;
  rows: number = 10;
  total: number = 0;
  hasCode: boolean = false;
  defaultSortField: string = 'value';
  defaultSortOrder: number = 1;
  currentStatus: boolean = true;
  canShowAction: boolean = true;
  parentconfigtext!: string;

  privleges = privileges;
  configurationForm!: FormGroup;
  constructor(
    private twoColConfigFacade: TwoColConfigFacade,
    private messageService: MessageService,
    private titlecasePipe: TitleCasePipe
  ) {}

  ngOnInit() {
    this.twoColConfigFacade.getCategories();
    this.twoColConfigFacade.getDepartments();
    this.twoColConfigFacade.selectCategories$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.originalcatgories = x;
          this.isDependent = false;
          this.catgories = x.filter((x) => x.isDependent == false);
        }
      });
    this.twoColConfigFacade.selectDepartments$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.departments = x;
        }
      });
    this.twoColConfigFacade.selectSaveStatus$
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
          this.configurationForm.get('id')?.patchValue(Guid.raw());
          this.configurationForm.get('code')?.reset();
          this.configurationForm.get('value')?.reset();
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
          const categoyvalue = this.configurationForm.get('category')?.value;
          const configurationvalue =
            this.configurationForm.get('configuration')?.value;
          const departmentValue =
            this.configurationForm.get('department')?.value;
          this.twoColConfigFacade.getConfigurationList(
            departmentValue,
            '',
            categoyvalue,
            configurationvalue ? configurationvalue : '',
            this.currentStatus
          );
        }
      });
    this.twoColConfigFacade.selectConfigurationList$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.configurationList = x;
          this.total = x.length;
        }
      });
    this.twoColConfigFacade.selectParentconfigurations$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.configurations = x;
        }
      });
    this.configurationForm = new FormGroup({
      department: new FormControl('', Validators.required),
      category: new FormControl('', Validators.required),
      id: new FormControl(Guid.raw()),
      configuration: new FormControl(''),
      value: new FormControl('', [Validators.required]),
      code: new FormControl(''),
    });
    this.cols = [
      {
        field: 'value',
        header: 'Configuration Value',
        customExportHeader: 'Configuration Value',
        sortablefield: 'value',
        isSortable: true,
      },
      {
        field: 'code',
        header: 'Code',
        sortablefield: 'code',
        isSortable: true,
      },
    ];
    this.searchableColumns = ['value', 'code'];

    this.actions = [
      {
        icon: 'pi pi-pencil',
        title: 'Edit',
        type: 'EDIT',
        privilege: privileges.CONFIG_UPDATE,
      },
      {
        icon: 'pi pi-times',
        title: 'In-Activate',
        type: 'INACTIVATE',
        privilege: privileges.CONFIG_DELETE,
      },
    ];
    this.configurationForm.get('department')?.valueChanges.subscribe((x) => {
      this.configurationForm.get('category')?.reset();
      this.configurationForm.get('value')?.reset();
      this.configurationForm.get('configuration')?.reset();
      this.configurationForm.get('code')?.reset();
    });
    this.configurationForm.get('category')?.valueChanges.subscribe((x) => {
      const departmentValue = this.configurationForm.get('department')?.value;
      if (x && this.isDependent) {
        const curntvalue: IConfigCategoryModel = this.originalcatgories.find(
          (y: IConfigCategoryModel) => y.id == x
        );
        const parentvalue: IConfigCategoryModel = this.originalcatgories.find(
          (y: IConfigCategoryModel) => y.id == curntvalue.parentId
        );
        this.parentconfigtext = `(${parentvalue.category})`;
        if (parentvalue) {
          this.twoColConfigFacade.getParentConfigurationList(
            departmentValue,
            '',
            parentvalue.id,
            ''
          );
        } else {
          this.configurations = [];
        }
      } else if (!this.isDependent) {
        this.twoColConfigFacade.getConfigurationList(
          departmentValue,
          '',
          x,
          '',
          this.currentStatus
        );
        this.configurations = [];
      }
      var category: IConfigCategoryModel = this.originalcatgories.find(
        (y: IConfigCategoryModel) => y.id == x
      );
      if (category) {
        this.hasCode = category.hasCode;
        this.canShowAction = category.isEditable;
      } else {
        this.hasCode = false;
      }
      if (this.hasCode) {
        this.configurationForm.get('code')?.addValidators(Validators.required);
        this.cols = [
          {
            field: 'value',
            header: 'Configuration Value',
            customExportHeader: 'Configuration Value',
            sortablefield: 'value',
            isSortable: true,
          },
          {
            field: 'code',
            header: 'Code',
            sortablefield: 'code',
            isSortable: true,
          },
        ];
        this.searchableColumns = ['value', 'code'];
      } else {
        this.configurationForm
          .get('code')
          ?.removeValidators(Validators.required);
        this.cols = [
          {
            field: 'value',
            header: 'Configuration Value',
            customExportHeader: 'Configuration Value',
            sortablefield: 'value',
            isSortable: true,
          },
        ];
        this.searchableColumns = ['value'];
      }

      this.configurationForm.get('value')?.reset();
      this.configurationForm.get('code')?.reset();
    });

    this.configurationForm.get('configuration')?.valueChanges.subscribe((x) => {
      const departmentValue = this.configurationForm.get('department')?.value;
      this.configurationList = [];
      this.total = 0;
      if (x) {
        this.twoColConfigFacade.getConfigurationList(
          departmentValue,
          '',
          '',
          x,
          this.currentStatus
        );
        this.configurationForm.get('id')?.patchValue(Guid.raw());
        this.configurationForm.get('code')?.reset();
        this.configurationForm.get('value')?.reset();
      }
    });
  }

  changeEvent(val: any) {
    this.configurationForm.reset();
    this.configurationForm.get('id')?.patchValue(Guid.raw());

    this.configurationList = [];
    this.total = 0;
    this.parentconfigtext = '';
    if (val.checked) {
      this.title = 'Dependent Configuration';
      this.configurationForm
        .get('configuration')
        ?.addValidators(Validators.required);
      this.catgories = this.originalcatgories.filter(
        (x) => x.isDependent == true
      );
    } else {
      this.title = 'Configuration';
      this.configurationForm.get('configuration')?.clearValidators();
      this.catgories = this.originalcatgories.filter(
        (x) => x.isDependent == false
      );
    }
    this.configurationForm.get('configuration')?.updateValueAndValidity();
  }

  changeStatus(val: boolean) {
    const categoyvalue = this.configurationForm.get('category')?.value;
    const configurationvalue =
      this.configurationForm.get('configuration')?.value;
    const departmentValue = this.configurationForm.get('department')?.value;
    if (
      categoyvalue &&
      ((configurationvalue && this.isDependent) ||
        (!configurationvalue && !this.isDependent))
    ) {
      this.twoColConfigFacade.getConfigurationList(
        departmentValue,
        '',
        categoyvalue,
        configurationvalue ? configurationvalue : '',
        !val
      );
    }
    this.configurationList = [];
    this.total = 0;
    this.currentStatus = !val;
    if (!val) {
      this.actions = [
        {
          icon: 'pi pi-pencil',
          title: 'Edit',
          type: 'EDIT',
          privilege: privileges.CONFIG_UPDATE,
        },
        {
          icon: 'pi pi-times',
          title: 'In-Activate',
          type: 'INACTIVATE',
          privilege: privileges.CONFIG_DELETE,
        },
      ];
    } else {
      this.actions = [
        {
          icon: 'pi pi-undo',
          title: 'Activate',
          type: 'ACTIVATE',
          privilege: privileges.CONFIG_UPDATE,
        },
      ];
    }
  }
  submit() {
    this.twoColConfigFacade.updatesaveStatus();
    this.twoColConfigFacade.saveConfiguration({
      departmentId: this.configurationForm.get('department')?.value,
      categoryId: this.configurationForm.get('category')?.value,
      id: this.configurationForm.get('id')?.value,
      configurationId: this.configurationForm.get('configuration')?.value,
      value: this.configurationForm.get('value')?.value,
      code: this.configurationForm.get('code')?.value?.toUpperCase(),
      isActive: true,
    });
  }
  getcontrol(val: any) {
    return (
      (this.configurationForm.get(val) as FormControl) ?? new FormControl()
    );
  }
  resetForm() {
    this.configurationForm.reset();
    this.configurationForm.get('id')?.patchValue(Guid.raw());
    this.configurations = [];
    this.total = 0;
  }
  actioInvoked(val: ActionModel) {
    if (val && val.type == 'INACTIVATE') {
      this.twoColConfigFacade.updatesaveStatus();
      this.twoColConfigFacade.saveConfiguration({
        ...val.record,
        isActive: false,
      });
    } else if (val && val.type == 'EDIT') {
      this.configurationForm.get('id')?.patchValue(val.record.id);
      this.configurationForm
        .get('configurationId')
        ?.patchValue(val.record.configurationId);
      this.configurationForm.get('value')?.patchValue(val.record.value);
      this.configurationForm.get('code')?.patchValue(val.record.code);
    } else if (val && val.type == 'ACTIVATE') {
      this.twoColConfigFacade.updatesaveStatus();
      this.twoColConfigFacade.saveConfiguration({
        ...val.record,
        isActive: true,
      });
    }
  }
  ngOnDestroy() {
    this.twoColConfigFacade.reset();
  }
}
