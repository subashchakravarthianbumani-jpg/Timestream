import { TitleCasePipe } from '@angular/common';
import { Component } from '@angular/core';
import { MessageService } from 'primeng/api';
import { ActionModel, Actions, Column } from 'src/app/_models/datatableModel';
import { TenderMasterViewModel } from 'src/app/_models/go/tender';
import { TenderFacade } from '../state/tender.facades';
import { ActivatedRoute, Router } from '@angular/router';
import {
  TableFilterModel,
  TenderFilterModel,
} from 'src/app/_models/filterRequest';
import { TCModel } from 'src/app/_models/user/usermodel';
import {
  dateconvertion,
  dateconvertionwithOnlyDate,
  getYearList,
  privileges,
} from 'src/app/shared/commonFunctions';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

@UntilDestroy()
@Component({
  selector: 'app-tenderlist',
  templateUrl: './tenderlist.component.html',
  styleUrls: ['./tenderlist.component.scss'],
  providers: [TitleCasePipe],
})
export class TenderlistComponent {
  configurationList!: any[];
  cols!: Column[];
  catgories!: any[];
  searchableColumns!: string[];
  currentStatus: boolean = false;

  privlegess = privileges;
  selecteddivisions: string[] = [];
  selectedYear: string[] = [new Date().getFullYear().toString()];
  yearList: string[] = getYearList();
  divisions!: TCModel[];
  actions: Actions[] = [];
  title: string = 'Works';
  first: number = 0;
  rows: number = 25;
  total: number = 0;
  defaultSortField: string = 'startDate';
  defaultSortOrder: number = 1;
  filtermodel!: TenderFilterModel;
  tableFilterModel!: TableFilterModel;

  constructor(
    private tenderFacade: TenderFacade,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private titlecasePipe: TitleCasePipe
  ) {}

  ngOnInit() {
    this.filtermodel = {
      districtList: null,
      divisionList: null,
      selectionType: null,
      fromDate: null,
      searchString: null,
      skip: 0,
      sorting: { fieldName: 'startDate', sort: 'ASC' },
      toDate: null,
      take: 25,
      where: null,
      workType: null,
      year: [this.selectedYear.toString()],
      columnSearch: null,
    };
    this.tenderFacade.getTenders(this.filtermodel);
    this.tenderFacade.divisionListGet();

    this.cols = [
      {
        field: 'workProgress',
        header: 'Work Progress',
        sortablefield: 'workProgress',
        isSearchable: true,
        isProgress: true,
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
        field: 'workType',
        header: 'Work Type',
        sortablefield: 'workType',
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
        field: 'schemeName',
        header: 'Scheme Name',
        sortablefield: 'schemeName',
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
      // {
      //   field: 'ContractAcceptedDate',
      //   header: 'Contract Accepted Date',
      //   sortablefield: 'ContractAcceptedDate',
      //   isSortable: true,
      //   isSearchable: true,
      // },
      {
        field: 'bidType',
        header: 'Bid Type',
        sortablefield: 'bidType',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'startDateString',
        header: 'Work Published date',
        sortablefield: 'startDate',
        isSortable: true,
        isSearchable: false,
      },
      {
        field: 'endDateString',
        header: 'Awarded Date',
        sortablefield: 'endDate',
        isSearchable: false,
      },
      {
        field: 'workCommencementDate',
        header: 'Work Commencement Date',
        sortablefield: 'workCommencementDate',
        isSearchable: true,
      },
      {
        field: 'workCompletionDate',
        header: 'Work Completion Date',
        sortablefield: 'workCompletionDate',
        isSearchable: true,
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
        field: 'tenderStatus',
        header: 'Status',
        sortablefield: 'tenderStatus',
        isSearchable: true,
        isBadge: true,
      },
      // {
      //   field: 'lastUpdatedUserName',
      //   header: 'Last Updated By',
      //   sortablefield: 'lastUpdatedUserName',
      //   isSortable: true,
      // },
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
        icon: 'pi pi-plus',
        title: 'Create Work',
        type: 'CREATE',
        visibilityCheckFeild: 'canCreateWork',
        privilege: this.privlegess.TENDER_WORK_CREATE,
        isIcon: true,
      },
      {
        icon: 'pi pi-pencil',
        title: 'Edit Work',
        type: 'EDIT',
        privilege: this.privlegess.TENDER_WORK_CREATE,
        visibilityCheckFeild: 'canEditWork',
        isIcon: true,
      },
      {
        icon: 'pi pi-arrow-right',
        title: 'Set Milestone',
        type: 'SET_MILESTONE',
        privilege: this.privlegess.TENDER_MILESTONE_SET,
        visibilityCheckFeild: 'canCreateTemplate',
        isIcon: true,
      },
      {
        icon: 'pi pi-info',
        title: 'View Details',
        type: 'VIEW',
        privilege: this.privlegess.TENDER_VIEW,
        isIcon: true,
      },
    ];
    this.tenderFacade.selectTenders$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.configurationList = []; // x.data;
          x.tenders?.forEach((x) => {
            this.configurationList.push({
              ...x,
              startDateString: this.dc(x.startDate),
              endDateString: this.dc(x.endDate),
              lastUpdatedDateString: this.dcwt(x.lastUpdatedDate),
              workCompletionDate:x.workCompletionDate?this.dc(x.workCompletionDate):'',
              workCommencementDate:x.workCommencementDate?this.dc(x.workCommencementDate):'',
            });
          });
          this.total = x.totalRecords;
        }
      });
    this.tenderFacade.selectDivisions$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.divisions = x;
        }
      });
  }
  changeyear(val: any) {
    this.filtermodel = { ...this.filtermodel, year: val };
    this.tenderFacade.getTenders(this.filtermodel);
  }
  divisionChanged(val: any) {
    this.filtermodel = { ...this.filtermodel, divisionList: val };
    this.tenderFacade.getTenders(this.filtermodel);
  }
  changefilter(val: any) {
    this.filtermodel = { ...this.filtermodel, ...val };
    this.tenderFacade.getTenders(this.filtermodel);
  }
  actioInvoked(val: ActionModel) {
    if (val && val.type == 'CREATE') {
      this.router.navigate(['generate', val.record.id], {
        relativeTo: this.route,
      });
    } else if (val && val.type == 'SET_MILESTONE') {
      this.router.navigate(['milestone-preparation', val.record.id], {
        relativeTo: this.route,
      });
    } else if (val && val.type == 'EDIT') {
      this.router.navigate(['generate', val.record.id], {
        relativeTo: this.route,
      });
    } else if (val && val.type == 'VIEW') {
      this.router.navigate(['view', val.record.id], {
        relativeTo: this.route,
      });
    }
  }
  changeStatus(val: boolean) {
    this.tenderFacade.getTenders(this.filtermodel);
    this.currentStatus = !val;
  }
  ngOnDestroy() {
    this.tenderFacade.reset();
  }
  dc(val: any) {
    return dateconvertionwithOnlyDate(val);
  }
  dcwt(val: any) {
    return dateconvertion(val);
  }
}
