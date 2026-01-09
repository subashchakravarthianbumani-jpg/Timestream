import { TitleCasePipe } from '@angular/common';
import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ActionModel, Actions, Column } from 'src/app/_models/datatableModel';
import {
  MBookFilterModel,
  TableFilterModel,
  TenderFilterModel,
} from 'src/app/_models/filterRequest';
import { TenderFacade } from '../tenders/state/tender.facades';
import { MBookConfigFacade } from './state/mbook.facade';
import { TCModel } from 'src/app/_models/user/usermodel';
import {
  dateconvertion,
  dateconvertionwithOnlyDate,
  getYearList,
  privileges,
} from 'src/app/shared/commonFunctions';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { Booleanish } from 'primeng/ts-helpers';

@UntilDestroy()
@Component({
  selector: 'app-manage-work',
  templateUrl: './manage-work.component.html',
  styleUrls: ['./manage-work.component.scss'],
  providers: [TitleCasePipe],
})
export class ManageWorkComponent {
  configurationList!: any[];
  cols!: Column[];
  catgories!: any[];
  searchableColumns!: string[];
  currentStatus: boolean = false;

  actions: Actions[] = [];
  title: string = 'M-Book';
  first: number = 0;
  rows: number = 25;
  total: number = 0;
  defaultSortField: string = 'tenderNumber';
  defaultSortOrder: number = 1;
  filtermodel!: MBookFilterModel;
  tableFilterModel!: TableFilterModel;

  selecteddivisions: string[] = [];
  selectedYear: string[] = [new Date().getFullYear().toString()];
  yearList: string[] = getYearList();
  divisions!: TCModel[];
  isdiffnav: boolean = false;

  constructor(
    private mbookFacade: MBookConfigFacade,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private titlecasePipe: TitleCasePipe
  ) {}
  ngOnInit() {
    this.filtermodel = {
      divisionIds: null,
      searchString: null,
      selectionType: null,
      skip: 0,
      sorting: { fieldName: 'tenderNumber', sort: 'ASC' },
      take: 25,
      where: { isActive: true },
      year: [this.selectedYear.toString()],
      isForApproval: false,
      columnSearch: null,
    };
    this.route.params.pipe(untilDestroyed(this)).subscribe((params) => {
      const type = params['type']; //log the value of id
      if (type) {
        if (type === 'history') {
          this.title = 'M-Book History';
          this.isdiffnav = true;
          this.filtermodel = {
            ...this.filtermodel,
            where: { isActive: false },
          };
        }
        if (type === 'approvals') {
          this.title = 'M-Book Approvals';
          this.isdiffnav = true;
          this.filtermodel = {
            ...this.filtermodel,
            isForApproval: true,
            where: { isActive: true },
          };
        }
      }
      this.mbookFacade.getMBooks(this.filtermodel);
    });

    

    this.mbookFacade.divisionListGet();
    this.cols = [
      {
        field: 'percentageCompleted',
        header: 'Progress',
        sortablefield: 'percentageCompleted',
        isSortable: true,
        isSearchable: true,
        isProgress: true,
      },
      {
        field: 'paymentPercentage',
        header: 'Payment Percentage',
        sortablefield: 'paymentPercentage',
        isSortable: true,
        isProgress: true,
      },
      {
        field: 'mBookNumber',
        header: 'M-Book Number',
        customExportHeader: 'M-Book Number',
        sortablefield: 'mBookNumber',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'tenderNumber',
        header: 'Work Number',
        customExportHeader: 'Work Number',
        sortablefield: 'tenderNumber',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'paymentStatusName',
        header: 'Payment Status',
        sortablefield: 'paymentStatusName',
        isSortable: true,
        isSearchable: true,
        isBadge: true,
      },
      {
        field: 'statusName',
        header: 'Status',
        sortablefield: 'statusName',
        isSortable: true,
        isSearchable: true,
        isBadge: true,
      },
      {
        field: 'workType',
        header: 'Work Type',
        sortablefield: 'workType',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'subWorkType',
        header: 'Sub Work Type',
        sortablefield: 'subWorkType',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'strength',
        header: 'Strength',
        isSearchable: true,
        sortablefield: 'strength',
        isSortable: true,
      },
      {
        field: 'milestoneCode',
        header: 'Code',
        isSearchable: true,
        sortablefield: 'milestoneCode',
        isSortable: true,
      },
      {
        field: 'milestoneName',
        header: 'Milestone Name',
        sortablefield: 'milestoneName',
      },
      {
        field: 'divisionName',
        header: 'Division',
        sortablefield: 'divisionName',
        isSearchable: true,
      },
      {
        field: 'districtName',
        header: 'District',
        sortablefield: 'districtName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'goNumber',
        header: 'GO',
        sortablefield: 'goNumber',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'startDateString',
        header: 'Work Published date',
        sortablefield: 'startDate',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'endDateString',
        header: 'Awarded Date',
        sortablefield: 'endDate',
      },
      {
        field: 'pannedValue',
        header: 'Planned Value',
        sortablefield: 'pannedValue',
        isSearchable: true,
      },
      {
        field: 'actualValue',
        header: 'Actual Value',
        sortablefield: 'actualValue',
        isSearchable: true,
      },
      {
        field: 'paymentValue',
        header: 'Payment Value',
        sortablefield: 'paymentValue',
        isSearchable: true,
      },
      {
        field: 'lastUpdatedDateString',
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
        title: 'Edit M-Book',
        type: 'EDIT',
        privilege: privileges.MBOOK_EDIT,
        visibilityCheckFeild: 'isEditable',
        isIcon: true,
      },
      {
        icon: 'pi pi-arrow-right',
        title: 'Approve',
        privilege: privileges.MBOOK_APPROVE_REJECT,
        type: 'APPROVE',
        visibilityCheckFeild: 'isActionable',
        isIcon: true,
      },
      {
        icon: 'pi pi-eye',
        privilege: privileges.MBOOK_VIEW,
        title: 'View',
        type: 'VIEW',
        isIcon: true,
      },
    ];
    this.mbookFacade.selectDivisions$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.divisions = x;
        }
      });
    this.mbookFacade.selectMBooks$.pipe(untilDestroyed(this)).subscribe((x) => {
      if (x) {
        this.configurationList = []; // x.data;
        x.mbooks?.forEach((x) => {
          this.configurationList.push({
            ...x,
            startDateString: this.dc(x.startDate),
            endDateString: this.dc(x.endDate),
            lastUpdatedDateString: this.dcwt(x.lastUpdatedDate),
          });
        });
        this.total = x.totalRecords;
      }
    });
  }
  changeyear(val: any) {
    this.filtermodel = { ...this.filtermodel, year: val };
    this.mbookFacade.getMBooks(this.filtermodel);
  }
  divisionChanged(val: any) {
    this.filtermodel = { ...this.filtermodel, divisionIds: val };
    this.mbookFacade.getMBooks(this.filtermodel);
  }
  changefilter(val: TableFilterModel) {
    this.filtermodel = { ...this.filtermodel, ...val };
    this.mbookFacade.getMBooks(this.filtermodel);
  }
  actioInvoked(val: ActionModel) {
    if (val && val.type == 'EDIT') {
      if (!this.isdiffnav) {
        this.router.navigate(['edit', val.record.id], {
          relativeTo: this.route,
        });
      } else {
        this.router.navigate(['m-book-manage/edit', val.record.id]);
      }
    } else if (val && val.type == 'APPROVE') {
      if (!this.isdiffnav) {
        this.router.navigate(['view', val.record.id, true], {
          relativeTo: this.route,
        });
      } else {
        this.router.navigate(['m-book-manage/view', val.record.id, true]);
      }
    } else if (val && val.type == 'VIEW') {
      if (!this.isdiffnav) {
        this.router.navigate(['view', val.record.id], {
          relativeTo: this.route,
        });
      } else {
        this.router.navigate(['m-book-manage/view', val.record.id]);
      }
    }
  }
  changeStatus(val: boolean) {
    this.mbookFacade.getMBooks(this.filtermodel);
    this.currentStatus = !val;
  }
  ngOnDestroy() {
    this.mbookFacade.reset();
  }
  dc(val: any) {
    return dateconvertionwithOnlyDate(val);
  }
  dcwt(val: any) {
    return dateconvertion(val);
  }
}
