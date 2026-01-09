import { TitleCasePipe } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { TemplateViewModel } from 'src/app/_models/configuration/templates';
import { ActionModel, Actions, Column } from 'src/app/_models/datatableModel';
import { TemplatesConfigFacade } from './state/template.facades';
import { Router, ActivatedRoute } from '@angular/router';
import { Guid } from 'guid-typescript';
import { FailedStatus, ErrorStatus } from 'src/app/_models/ResponseStatus';
import { TCModel } from 'src/app/_models/user/usermodel';
import { TwoColConfigService } from 'src/app/_services/two.col.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { privileges } from 'src/app/shared/commonFunctions';

@UntilDestroy()
@Component({
  selector: 'app-templates',
  templateUrl: './templates.component.html',
  styleUrls: ['./templates.component.scss'],
  providers: [TitleCasePipe],
})
export class TemplatesComponent {
  configurationList!: TemplateViewModel[];
  cols!: Column[];
  workTypes!: any[];
  departments!: TCModel[];
  subWorkTypes!: any[];
  serviceTypes!: any[];
  categoryTypes!: any[];
  searchableColumns!: string[];
  currentStatus: boolean = true;

  actions: Actions[] = [];
  title: string = 'Templates';
  first: number = 0;
  rows: number = 10;
  total: number = 0;
  defaultSortField: string = 'templateNumber';
  defaultSortOrder: number = 1;

  privleges = privileges;
  templateForm!: FormGroup;
  constructor(
    private templateConfigFacade: TemplatesConfigFacade,
    private twoColConfigService: TwoColConfigService,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private titlecasePipe: TitleCasePipe
  ) {}

  ngOnInit() {
    this.templateConfigFacade.getTemplates(this.currentStatus);
    this.templateConfigFacade.getworkTypeList();
this.templateConfigFacade.getserviceList();
this.templateConfigFacade.getcategoryTypeList();
    this.twoColConfigService.getAllDepartments().subscribe(
      (x) => {
        this.departments = x.data;
      },
      (error) => {}
    );
    this.templateForm = new FormGroup({
      id: new FormControl(Guid.raw()),
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
      departmentId: new FormControl('', [Validators.required]),
      servicetypeId: new FormControl('', [Validators.required]),
      categorytypeId: new FormControl('', [Validators.required]),
    });
    this.cols = [
      {
        field: 'department',
        header: 'Department',
        customExportHeader: 'Department',
        sortablefield: 'department',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'name',
        header: 'Name',
        customExportHeader: 'Name',
        sortablefield: 'name',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'templateCode',
        header: 'Code',
        customExportHeader: 'Code',
        sortablefield: 'templateCode',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'strength',
        header: 'Strength',
        customExportHeader: 'Strength',
        sortablefield: 'strength',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'workType',
        header: 'Work',
        customExportHeader: 'Work Type',
        sortablefield: 'workType',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'subWorkType',
        header: 'Sub Work',
        customExportHeader: 'Sub Work Type',
        sortablefield: 'subWorkType',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'serviceType',
        header: 'serviceType',
        customExportHeader: 'Service Type',
        sortablefield: 'serviceType',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'categoryType',
        header: 'categoryType',
        customExportHeader: 'Category Type',
        sortablefield: 'categoryType',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'durationInDays',
        header: 'Days',
        sortablefield: 'durationInDays',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'status',
        header: 'Status',
        sortablefield: 'status',
        isSortable: true,
        isSearchable: true,
        isBadge: true,
        badgeColor: '#ff0000',
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
        privilege: this.privleges.TEMPLATE_UPDATE,
      },
      {
        icon: 'pi pi-times',
        title: 'In-Activate',
        type: 'INACTIVATE',
        privilege: this.privleges.TEMPLATE_DELETE,
      },
      {
        icon: 'pi pi-arrow-right',
        title: 'Set Milestone',
        type: 'MILESTONE',
        privilege: this.privleges.TEMPLATE_SET_MILESTONE,
      },
    ];
    this.templateConfigFacade.selectTemplates$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.configurationList = x;
        }
      });
    this.templateConfigFacade.selectWorkTypes$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.workTypes = x;
        }
      });
      this.templateConfigFacade.selectServiceTypes$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          
          this.serviceTypes = x;
        }
      });
       this.templateConfigFacade.selectCategoryTypes$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
         
          this.categoryTypes = x;
        }
      });
    this.templateConfigFacade.selectsubWorkTypes$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.subWorkTypes = x;
        }
      });
    this.templateConfigFacade.selectSaveStatus$
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
          this.resetForm();
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: x?.message,
          });
          this.templateConfigFacade.updatesaveStatus();
          this.templateConfigFacade.getTemplates(this.currentStatus);
        }
      });
    this.templateForm.get('workTypeId')?.valueChanges.subscribe((x) => {
      this.templateConfigFacade.getsubworkTypeList(x);
    });
  }
  changeStatus(val: boolean) {
    this.templateConfigFacade.getTemplates(!val);
    this.currentStatus = !val;
    if (!val) {
      this.actions = [
        {
          icon: 'pi pi-pencil',
          title: 'Edit',
          type: 'EDIT',
          privilege: this.privleges.TEMPLATE_UPDATE,
        },
        {
          icon: 'pi pi-times',
          title: 'In-Activate',
          type: 'INACTIVATE',
          privilege: this.privleges.TEMPLATE_DELETE,
        },
        {
          icon: 'pi pi-arrow-right',
          title: 'Set Milestone',
          type: 'MILESTONE',
          privilege: this.privleges.TEMPLATE_SET_MILESTONE,
        },
      ];
    } else {
      this.actions = [
        {
          icon: 'pi pi-undo',
          title: 'Activate',
          type: 'ACTIVATE',
          privilege: this.privleges.TEMPLATE_UPDATE,
        },
      ];
    }
  }
  resetForm() {
    this.templateConfigFacade.updatesaveStatus();
    this.templateForm.reset();
    this.templateForm.get('id')?.patchValue(Guid.raw());
  }
  actioInvoked(val: ActionModel) {
    if (val && val.type == 'INACTIVATE') {
      this.templateConfigFacade.updatesaveStatus();
      this.templateConfigFacade.saveTemplates({
        ...val.record,
        isActive: false,
      });
    } else if (val && val.type == 'EDIT') {
      
      this.templateForm.get('id')?.patchValue(val.record.id);
      this.templateForm.get('name')?.patchValue(val.record.name);
      this.templateForm
        .get('departmentId')
        ?.patchValue(val.record.departmentId);
      this.templateForm
        .get('durationInDays')
        ?.patchValue(val.record.durationInDays);
      this.templateForm.get('strength')?.patchValue(val.record.strength);
      this.templateForm
        .get('subWorkTypeId')
        ?.patchValue(val.record.subWorkTypeId);
      this.templateForm
        .get('templateCode')
        ?.patchValue(val.record.templateCode);
      this.templateForm.get('workTypeId')?.patchValue(val.record.workTypeId);
      this.templateForm.get('servicetypeId')?.patchValue(val.record.serviceTypeId);
      this.templateForm.get('categorytypeId')?.patchValue(val.record.categoryTypeId);
    } else if (val && val.type == 'ACTIVATE') {
      this.templateConfigFacade.updatesaveStatus();
      this.templateConfigFacade.saveTemplates({
        ...val.record,
        isActive: true,
      });
    } else if (val && val.type == 'MILESTONE') {
      this.router.navigate(['configuration/milestones', val.record.id]);
    }
  }
  submit() {
    this.templateConfigFacade.updatesaveStatus();
    this.templateConfigFacade.saveTemplates({
      id: this.templateForm.get('id')?.value,
      name: this.templateForm.get('name')?.value,
      durationInDays: this.templateForm.get('durationInDays')?.value,
      workTypeId: this.templateForm.get('workTypeId')?.value,
      serviceTypeId: this.templateForm.get('servicetypeId')?.value,
      categoryTypeId: this.templateForm.get('categorytypeId')?.value,
      isActive: true,
      subWorkTypeId: this.templateForm.get('subWorkTypeId')?.value,
      departmentId: this.templateForm.get('departmentId')?.value,
      strength: this.templateForm.get('strength')?.value,
      templateCode: this.templateForm.get('templateCode')?.value,
    });
  }
  ngOnDestroy() {
    this.templateConfigFacade.reset();
  }
}
