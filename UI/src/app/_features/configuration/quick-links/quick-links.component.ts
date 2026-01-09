import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { QuickLinkModel } from 'src/app/_models/configuration/quickLink';
import { ActionModel, Actions, Column } from 'src/app/_models/datatableModel';
import { QuickLinkConfigFacade } from './state/quicklink.facades';
import { MessageService } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';
import { TitleCasePipe } from '@angular/common';
import { Guid } from 'guid-typescript';
import { ErrorStatus, FailedStatus } from 'src/app/_models/ResponseStatus';
import { TCModel } from 'src/app/_models/user/usermodel';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { privileges } from 'src/app/shared/commonFunctions';

@UntilDestroy()
@Component({
  selector: 'app-quick-links',
  templateUrl: './quick-links.component.html',
  styleUrls: ['./quick-links.component.scss'],
  providers: [TitleCasePipe],
})
export class QuickLinksComponent {
  configurationList!: QuickLinkModel[];
  cols!: Column[];
  catgories!: any[];
  searchableColumns!: string[];
  currentStatus: boolean = true;
  userGroups: TCModel[] = [];

  actions: Actions[] = [];
  title: string = 'Quick Links';
  first: number = 0;
  rows: number = 10;
  total: number = 0;
  defaultSortField: string = 'name';
  defaultSortOrder: number = 1;

  privleges = privileges;
  quickLinkForm!: FormGroup;
  constructor(
    private quickLinkConfigFacade: QuickLinkConfigFacade,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private titlecasePipe: TitleCasePipe
  ) {}

  ngOnInit() {
    this.quickLinkConfigFacade.getQuickLinks(this.currentStatus);
    this.quickLinkConfigFacade.userGroupListGet(this.currentStatus);

    this.quickLinkForm = new FormGroup({
      id: new FormControl(Guid.raw()),
      name: new FormControl('', [
        Validators.required,
        Validators.maxLength(200),
        Validators.minLength(3),
      ]),
      fileType: new FormControl('', [Validators.required]),
      userGroupIdList: new FormControl(null, [Validators.required]),
      link: new FormControl('', [Validators.required]),
    });
    this.quickLinkForm.controls['link'].valueChanges.subscribe((x) => {
      if (x) {
        if (x.includes('.')) {
          this.quickLinkForm.get('fileType')?.patchValue(x.split('.').pop());
        }
      }
    });
    this.cols = [
      {
        field: 'name',
        header: 'File Name',
        customExportHeader: 'File Name',
        sortablefield: 'name',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'fileType',
        header: 'File Type',
        sortablefield: 'fileType',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'link',
        header: 'Link',
        sortablefield: 'link',
        isLink: true,
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
        privilege: this.privleges.QL_UPDATE,
      },
      {
        icon: 'pi pi-times',
        title: 'In-Activate',
        type: 'INACTIVATE',
        privilege: this.privleges.QL_DELETE,
      },
    ];
    this.quickLinkConfigFacade.selectQuickLinks$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.configurationList = x;
        }
      });
    this.quickLinkConfigFacade.selectuserGroups$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.userGroups = x;
        }
      });
    this.quickLinkConfigFacade.selectSaveStatus$
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
          this.quickLinkConfigFacade.getQuickLinks(this.currentStatus);
        }
      });
  }
  changeStatus(val: boolean) {
    this.quickLinkConfigFacade.getQuickLinks(!val);
    this.currentStatus = !val;
    if (!val) {
      this.actions = [
        {
          icon: 'pi pi-pencil',
          title: 'Edit',
          type: 'EDIT',
          privilege: this.privleges.QL_UPDATE,
        },
        {
          icon: 'pi pi-times',
          title: 'In-Activate',
          type: 'INACTIVATE',
          privilege: this.privleges.QL_DELETE,
        },
      ];
    } else {
      this.actions = [
        {
          icon: 'pi pi-undo',
          title: 'Activate',
          type: 'ACTIVATE',
          privilege: this.privleges.QL_UPDATE,
        },
      ];
    }
  }
  resetForm() {
    this.quickLinkForm.reset();
    this.quickLinkForm.get('id')?.patchValue(Guid.raw());
  }
  actioInvoked(val: ActionModel) {
    if (val && val.type == 'INACTIVATE') {
      this.quickLinkConfigFacade.updatesaveStatus();
      this.quickLinkConfigFacade.saveQuickLink({
        ...val.record,
        isActive: false,
      });
    } else if (val && val.type == 'EDIT') {
      this.quickLinkForm.get('id')?.patchValue(val.record.id);
      this.quickLinkForm.get('name')?.patchValue(val.record.name);
      this.quickLinkForm.get('fileType')?.patchValue(val.record.fileType);
      this.quickLinkForm
        .get('userGroupIdList')
        ?.patchValue(val.record.userGroupIdList);
      this.quickLinkForm.get('link')?.patchValue(val.record.link);
    } else if (val && val.type == 'ACTIVATE') {
      this.quickLinkConfigFacade.updatesaveStatus();
      this.quickLinkConfigFacade.saveQuickLink({
        ...val.record,
        isActive: true,
      });
    }
  }
  submit() {
    this.quickLinkConfigFacade.updatesaveStatus();
    this.quickLinkConfigFacade.saveQuickLink({
      id: this.quickLinkForm.get('id')?.value,
      name: this.quickLinkForm.get('name')?.value,
      fileType: this.quickLinkForm.get('fileType')?.value,
      userGroupIdList: this.quickLinkForm.get('userGroupIdList')?.value,
      link: this.quickLinkForm.get('link')?.value,
      isActive: true,
    });
  }
  ngOnDestroy() {
    this.quickLinkConfigFacade.reset();
  }
}
