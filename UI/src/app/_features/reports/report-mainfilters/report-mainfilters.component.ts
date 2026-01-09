import { ChangeDetectorRef, Component } from '@angular/core';
import { Router, ActivatedRoute,NavigationEnd } from '@angular/router';
import { Actions, Column } from 'src/app/_models/datatableModel';
import { untilDestroyed } from '@ngneat/until-destroy';
import {
  GoFilterModel,
  GoReportFilterModel,
  MBookFilterModel,
  MBookReportFilterModel,
  MilestoneFilterModel,
  ReportBreadcrumbModel,
  TableFilterModel,
  TenderFilterModel,
  WorkFilterModel,
} from 'src/app/_models/filterRequest';
import { TCModel } from 'src/app/_models/user/usermodel';
import {
  GoCols,
  MbookCols,
  dateconvertion,
  dateconvertionwithOnlyDate,
  getYearList,
  milestoneCols,
  reportCols,
  workKeyValueMbookModel,
  workKeyValueMilestoneModel,
  workKeyValueModelMbookarray,
  workKeyValueModelMilestonearray,
  workKeyValueModelTenderarray,
  workKeyValueTenderModel,
} from 'src/app/shared/commonFunctions';
import { ReportFacade } from '../state/report.facades';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { MenuItem } from 'primeng/api';
import { BreadcrumbItemClickEvent } from 'primeng/breadcrumb';
import { ChangeDetectionStrategy } from '@angular/compiler';
import { ReportsService } from 'src/app/_services/report.service';
import { TenderService } from 'src/app/_services/tender.service';
import { SuccessStatus } from 'src/app/_models/ResponseStatus';
import * as moment from 'moment';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators'; 


@Component({
  selector: 'app-report-mainfilters',
  templateUrl: './report-mainfilters.component.html',
  styleUrls: ['./report-mainfilters.component.scss'],
})
export class ReportMainfiltersComponent {
  selecteddivisions: string[] = [];

  dashboardQueryParams: any = {};
  selectedStatuses: string[] = [];
  selectedDistricts: string[] = [];
  selectedPacakges: string[] = [];
  types: string[] = ['GO', 'Tender', 'Milestone', 'M-Book'];
  selectedtype: string = 'TENDER';
  selecteddepartments: string[] = [];
  selectedYear: string[] = [new Date().getFullYear().toString()];
  yearList: string[] = [];
  divisions!: TCModel[];
  statusList!: TCModel[];
  dtistrictsList!: TCModel[];
  departments!: TCModel[];
  rangeDates: Date[] = [];
  schemes!: TCModel[];
  goPackageNo!: TCModel[];
  selectedscheme: string[] = [];

  title: string = 'Reports';

  first: number = 0;
  rows: number = 25;
  total: number = 0;
  defaultSortField: string = 'startDate';
  defaultSortOrder: number = 1;
  tableFilterModel!: TableFilterModel;
  actions: Actions[] = [];
  canShowAction: boolean = true;

  filtermodel!: TenderFilterModel;
  tenderSearchString!: string;
  tenders!: any[];

  gridList!: any[];
  filterColumns!: Column[];
  reportBreadcrumbModel!: ReportBreadcrumbModel[];
  cols!: Column[];

  filterModel!: any;
  filterModelTender!: WorkFilterModel;
  filterModelMbook!: MBookReportFilterModel;
  filterModelMilestone!: MilestoneFilterModel;

  freshfilterModelTender!: WorkFilterModel;
  freshfilterModelMilestone!: MilestoneFilterModel;
  freshfilterModelMbook!: MBookReportFilterModel;

  breadcrumbItems: MenuItem[] = [];
  showscheme: boolean = true;
  from!: any;
  to!: any;
  daysRange: { label: string; value: string }[] = [
    { label: '0-20', value: '0-20' },
    { label: '20-80', value: '20-80' },
    { label: '80-1000', value: '80-1000' },
  ];
  selectedDays = null;
  previousType: string = '';
colsInitialized: boolean = false;


  type: any;
   private routerSub!: Subscription;

  
  
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private tenderService: TenderService,
    private reportFacade: ReportFacade,
    private reportsService: ReportsService,
    public layoutService: LayoutService
    
  ) {}

  ngOnInit() {

    this.cols=[];
    this.route.queryParams.subscribe((params: any) => {
      if (params['Type']) {
        this.selectedtype = params['Type'];
      }
      if (params['Year']) {
  this.yearList = Array.isArray(params['Year']) ? params['Year'] : [params['Year']];
}
      if (params['Status']) {
        this.selectedStatuses = Array.isArray(params['Status'])
          ? params['Status']
          : [params['Status']];
      }
      if (params['days']) {
        this.selectedDays = params['days'];
      }
      if (params['division']) {
        this.selecteddivisions = Array.isArray(params['division'])
          ? params['division']
          : [params['division']];
      }
      if (params['scheme']) {
        this.selectedscheme = Array.isArray(params['scheme'])
          ? params['scheme']
          : [params['scheme']];
      }
      if (params['district']) {
        this.selectedDistricts = Array.isArray(params['district'])
          ? params['district']
          : [params['district']];
      }
      

    });

    //console.log("rangeDates after refresh:", this.rangeDates);

    this.filterModelTender = {
      columnSearch: null,
      contractor: null,
      contractorDistrict: null,
      cost: null,
      days: this.selectedDays ?? null,
      delay: null,
      districtId: null,
      divisionId: null,
      duration: null,
      goNumber: null,
      searchString: null,
      skip: 0,
      sorting: { fieldName: 'startDate', sort: 'ASC' },
      workStatus: null,
      strength: null,
      subWorkTypeId: null,
      take: 25,
      where: null,
      workTypeId: null,
      mainCategory:null,
      subcategory:null,
      contractorCompanyName: null,
      contractorMobile: null,
      departmentList: null,
      divisionList: this.selecteddivisions ?? null,
      statusList: this.selectedStatuses ?? null,
      districtList: this.selectedDistricts ?? null,
      schemeList: this.selectedscheme ?? null,
      packageList: this.selectedPacakges ?? null,
      workNumber: null,
      fromDate: null,
      toDate: null,
    };

    // // On page refresh, reset values
    // this.filterModelTender.fromDate = null;
    // this.filterModelTender.toDate = null;
    // this.rangeDates = [];

    if (this.yearList && this.yearList.length > 0) {
      const firstYear = Number(this.yearList[0]);
      const lastYear = Number(this.yearList[this.yearList.length - 1]);

      if (!isNaN(firstYear) && !isNaN(lastYear)) {
        this.filterModelTender.fromDate = moment(
          new Date(firstYear, 0, 1)
        ).format('YYYY-MM-DD');
        this.filterModelTender.toDate = moment(
          new Date(lastYear, 11, 31)
        ).format('YYYY-MM-DD');

        this.rangeDates = [
          moment(this.filterModelTender.fromDate, 'YYYY-MM-DD').toDate(),
          moment(this.filterModelTender.toDate, 'YYYY-MM-DD').toDate(),
        ];
        this.from = this.filterModelTender.fromDate;
    this.to   = this.filterModelTender.toDate;
      }
    } else {
      this.rangeDates = [];
    }

    // if (this.yearList && this.yearList.length > 0) {
    //   (this.filterModelTender.fromDate = moment(
    //     new Date(Number(this.yearList[0]), 0, 1)
    //   ).format('YYYY-MM-DD')),
    //     (this.filterModelTender.toDate = moment(
    //       new Date(Number(this.yearList[this.yearList.length - 1]), 11, 31)
    //     ).format('YYYY-MM-DD'));
    //    // this.rangeDates = [moment(this.filterModelTender.fromDate).toDate(), moment(this.filterModelTender.toDate).toDate()] ;
    // }

    this.filterModelMilestone = {
      columnSearch: null,
      cost: null,
      districtId: null,
      divisionId: null,
      searchString: null,
      skip: 0,
      sorting: null,
      strength: null,
      subWorkTypeId: null,
      take: 25,
      workTypeId: null,
      departmentList: null,
      divisionList: null,
      statusList: null,
      actualCost: null,
      approvalStatusName: null,
      approvalStatusId: null,
      fromDate: null,
      paymentStatusId: null,
      paymentStatusName: null,
      districtList: null,
      toDate: null,
      workId: null,
    };
    this.filterModelMbook = {
      columnSearch: null,
      districtId: null,
      divisionId: null,
      searchString: null,
      skip: 0,
      sorting: null,
      strength: null,
      subWorkTypeId: null,
      take: 25,
      workTypeId: null,
      departmentList: null,
      divisionList: null,
      districtList: null,
      statusList: null,
      fromDate: null,
      paymentStatusId: null,
      paymentStatusName: null,
      toDate: null,
      workId: null,
      actionableRoleId: null,
      actualAmount: null,
      amount: null,
      statusId: null,
      statusName: null,
    };
    this.filterModel = this.filterModelTender;
    this.freshfilterModelTender = this.filterModelTender;
    this.freshfilterModelMilestone = this.filterModelMilestone;
    this.freshfilterModelMbook = this.filterModelMbook;
    this.reportFacade.getReports(this.filterModel, this.selectedtype);

    this.reportsService.getreportform().subscribe(
      (x) => {
        this.types = x.data.types;
        this.divisions = x.data.divisions;
        this.statusList = x.data.statusList;
        this.departments = x.data.departments;
        this.dtistrictsList = x.data.districts;
        this.schemes = x.data.schemes;
        this.goPackageNo = x.data.goPackageNo;
      },
      (error) => {}
    );
    console.log("init" ,this.cols  );
    this.cols = reportCols.map(c => ({ ...c }));;
    console.log("init comp" ,this.cols  );
    this.filterColumns = this.cols.filter(
      (x) => x.isAnchortagforFilter == true
    );
    this.actions = [
      {
        icon: 'pi pi-info',
        title: 'View Details',
        type: 'VIEW',
        isIcon: true,
      },
    ];

    this.reportFacade.selectBreadCrumb$.subscribe((x) => {
      console.log(x);
      this.breadcrumbItems = [];
      this.reportBreadcrumbModel = x

        .slice()
        .sort((y: { orderNumber: any }) => y.orderNumber);

      this.reportBreadcrumbModel.map((y) => {
        this.breadcrumbItems.push({
          label: `<h5> ${y.labelName}<span> (${y.label}1)</span></h5>`,
          escape: false,
          queryParams: { field: y.fieldName, value: y.value },
        });
      });
      this.filterbasedonBreadcrumb();
    });
 

    //updated by Vijay 10-11-25 
    this.reportFacade.selectReports$.subscribe((x) => {
      
      this.gridList = x.list;
      
      this.gridList = []; // x.data;
      x.list?.forEach((x: any) => {
        this.gridList.push({
          ...x,
          startDateString: this.dc(x.startDate),
          endDateString: this.dc(x.endDate),
          dateString: this.dc(x.date),
          WorkCommencementDate: x.workCompletionDate
            ? this.dc(x.workCommencementDate)
            : '',
          WorkCompletionDate: x.workCompletionDate
            ? this.dc(x.workCompletionDate)
            : '',
        });
      });

      this.total = x.totalRecords;
if (!this.colsInitialized || this.previousType !== this.selectedtype) {
     
      if (this.selectedtype == 'TENDER') {
        this.cols = reportCols.map(c => ({ ...c }));;
      } else if (this.selectedtype == 'MILESTONE') {
        this.cols = milestoneCols.map(c => ({ ...c }));;
      } else if (this.selectedtype == 'GO') {
        this.cols = GoCols.map(c => ({ ...c }));;
      } else  {
        this.cols = MbookCols.map(c => ({ ...c }));;
        this.showscheme=false;
      }

         this.colsInitialized = true;
    this.previousType = this.selectedtype;
      }
    this.reportBreadcrumbModel?.forEach(b => {
    const col = this.cols.find(c => c.field === b.fieldName);
     if (col) col.isAnchortagforFilter = false;
   });


    });

//     this.reportFacade.selectReports$.subscribe((x) => {
//   // 🔹 Step 1: Refresh grid data only
//   this.gridList = (x.list ?? []).map((item: any) => ({
//     ...item,
//     startDateString: this.dc(item.startDate),
//     endDateString: this.dc(item.endDate),
//     dateString: this.dc(item.date),
//     WorkCommencementDate: item.workCommencementDate
//       ? this.dc(item.workCommencementDate)
//       : '',
//     WorkCompletionDate: item.workCompletionDate
//       ? this.dc(item.workCompletionDate)
//       : '',
//   }));

//   this.total = x.totalRecords ?? 0;

//   // 🔹 Step 2: Update columns only if type changed OR not initialized yet
//   if (!this.colsInitialized || this.previousType !== this.selectedtype) {
//     if (this.selectedtype === 'TENDER') {
//       this.cols = reportCols.map(c => ({ ...c }));
//     } else if (this.selectedtype === 'MILESTONE') {
//       this.cols = milestoneCols.map(c => ({ ...c }));
//     } else if (this.selectedtype === 'GO') {
//       this.cols = GoCols.map(c => ({ ...c }));
//     } else {
//       this.cols = MbookCols.map(c => ({ ...c }));
//       this.showscheme = false;
//     }

//     this.colsInitialized = true;
//     this.previousType = this.selectedtype;
//   }

//   // 🔹 Step 3: Keep breadcrumb filter column states
//   this.reportBreadcrumbModel?.forEach(b => {
//     const col = this.cols.find(c => c.field === b.fieldName);
//     if (col) col.isAnchortagforFilter = false;
//   });

//   // 🔹 Step 4: Trigger UI update
//   this.cdr.detectChanges();
// });

    
  }

  
  // onGlobalFilter(event: any) {
  //   this.filterModel = { ...this.filterModel, searchString: event };
  //   this.reportFacade.getReports(this.filterModel, this.selectedtype);
  //   //this.callreports();
  // }

  ngOnChanges() {
    this.filtermodel = {
      districtList: null,
      divisionList: null,
      fromDate: null,
      selectionType: this.selectedtype,
      searchString: null,
      skip: 0,
      sorting: { fieldName: 'startDate', sort: 'ASC' },
      toDate: null,
      take: 5,
      where: null,
      workType: null,
      year: null,
      columnSearch: null,
      
    };
  }

  filterbasedonBreadcrumb() {
    var filteredModel: any = this.getFilterBasedonType();
    this.filterModel = {
    ...this.filterModel, // keep existing ones like fromDate/toDate
    ...filteredModel,
    divisionList: this.selecteddivisions ?? [],
    districtList: this.selectedDistricts ?? [],
    schemeList: this.selectedscheme ?? [],
    departmentList: this.selecteddepartments ?? [],
    statusList: this.selectedStatuses ?? [],
    packageList: this.selectedPacakges ?? [],
    days: this.selectedDays ?? null,
  };
    this.reportFacade.getReports(this.filterModel, this.selectedtype);
  }

  getFilterBasedonType() {
    if (this.selectedtype == 'TENDER') {
      const filter: WorkFilterModel = {
        goNumber:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.goNumber
          )?.value ?? null,
        contractorDistrict:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.contractorDistrict
          )?.value ?? null,
        contractor:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.contractorName
          )?.value ?? null,
        cost:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.workValue
          )?.value ?? null,
        districtId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.districtName
          )?.value ?? null,

        divisionId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.divisionName
          )?.value ?? null,
        duration:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.workDurationInDays
          )?.value ?? null,
        subWorkTypeId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.subWorkType
          )?.value ?? null,
        searchString: null,
        skip: 0,
        sorting: { fieldName: 'startDate', sort: 'ASC' },
        take: 25,
        workTypeId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.workType
          )?.value ?? null,
        columnSearch: null,
        delay: null,
        mainCategory:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.mainCategory
          )?.value ?? null,
       subcategory:
         this.reportBreadcrumbModel.find(
          (x) => x.fieldName == workKeyValueTenderModel.subcategory
         )?.value ?? null,
          
        workStatus:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.workStatusName
          )?.value ?? null,

        strength:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.strength
          )?.value ?? null,
        where: null,
        contractorCompanyName:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.contractorCompanyName
          )?.value ?? null,
        contractorMobile:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.contractorMobile
          )?.value ?? null,
        departmentList: this.selecteddepartments,
        divisionList: this.selecteddivisions,
        statusList: this.selectedStatuses,
        districtList: this.selectedDistricts,
        schemeList: this.selectedscheme,
        packageList: this.selectedPacakges,
        workNumber:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueTenderModel.workNumber
          )?.value ?? null,
        fromDate: this.from,
        toDate: this.to,
        days: null,
      };
      return filter;
    } else if (this.selectedtype == 'MILESTONE') {
      const filter: MilestoneFilterModel = {
        columnSearch: null,
        cost:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMilestoneModel.cost
          )?.value ?? null,
        districtId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMilestoneModel.districtName
          )?.value ?? null,
        divisionId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMilestoneModel.divisionName
          )?.value ?? null,
        searchString: null,
        skip: 0,
        sorting: null,
        strength:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMilestoneModel.strength
          )?.value ?? null,
        subWorkTypeId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMilestoneModel.subWorkType
          )?.value ?? null,
        take: 25,
        workTypeId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMilestoneModel.workType
          )?.value ?? null,
        departmentList: this.selecteddepartments,
        divisionList: this.selecteddivisions,
        statusList: this.selectedStatuses,
        districtList: this.selectedDistricts,
        actualCost:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMilestoneModel.actualCost
          )?.value ?? null,
        approvalStatusName: null,
        approvalStatusId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMilestoneModel.approvalStatusName
          )?.value ?? null,
        fromDate: this.from,
        paymentStatusId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMilestoneModel.paymentStatusName
          )?.value ?? null,
        paymentStatusName: null,
        toDate: this.to,
        workId: null,
      };
      return filter;
    } else if (this.selectedtype == 'GO') {
      const filter: GoReportFilterModel = {
        columnSearch: null,
        districtId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMilestoneModel.districtName
          )?.value ?? null,
        divisionId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMilestoneModel.divisionName
          )?.value ?? null,
        searchString: null,
        skip: 0,
        sorting: null,
        take: 25,
        departmentList: this.selecteddepartments,
        fromDate: this.from,
        toDate: this.to,
        where: null,
        divisionList: null,
        statusList: null,
      };
      return filter;
    } else {
      const filter: MBookReportFilterModel = {
        columnSearch: null,
        districtId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMbookModel.districtName
          )?.value ?? null,
        divisionId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMbookModel.divisionName
          )?.value ?? null,
        searchString: null,
        skip: 0,
        sorting: null,
        strength:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMbookModel.strength
          )?.value ?? null,
        subWorkTypeId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMbookModel.subWorkType
          )?.value ?? null,
        take: 25,
        workTypeId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMbookModel.workType
          )?.value ?? null,
        departmentList: this.selecteddepartments,
        divisionList: this.selecteddivisions,
        statusList: this.selectedStatuses,
        districtList: this.selectedDistricts,
        fromDate: this.from,
        paymentStatusId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMbookModel.paymentStatusName
          )?.value ?? null,
        paymentStatusName: null,
        toDate: this.to,
        workId: null,
        actionableRoleId: null,
        actualAmount:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMbookModel.actualAmount
          )?.value ?? null,
        amount:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMbookModel.milestoneAmount
          )?.value ?? null,
        statusId:
          this.reportBreadcrumbModel.find(
            (x) => x.fieldName == workKeyValueMbookModel.statusName
          )?.value ?? null,
        statusName: null,
      };
      return filter;
    }
  }
  changecolFilter(val: any) {
    console.log("cols:",this.cols);
    var count = this.reportBreadcrumbModel.length;
    var dd:Column[] = [];
    this.cols.map((x) => {
      if (x.field == val.type.field) {
        dd = [...dd, {
          ...x,isAnchortagforFilter:false
        }];
        x.isAnchortagforFilter = false;
        var d: string = val.type.field;
        var name = val.record[d];
        // var value = this.getfieldname(val, d);
        // var name = this.getfieldname(val, d);
        var value = this.getfieldname(val, d);

        this.reportFacade.saveBreadcrumbs([
          ...this.reportBreadcrumbModel,

          {
            fieldName: val.type.field,
            label: val.type.header,
            labelName: name,
            orderNumber: count + 1,
            value: value,
          },
        ]);
      }else{
        dd = [...dd, {
          ...x
        }];
      }
    });
    this.cols = dd;
    console.log("cols compss:",this.cols);
  }
  getfieldname(val: any, d: string) {
    if (this.selectedtype == 'TENDER')
      return val.record[
        workKeyValueModelTenderarray.find((x) => x.fieldName == d)
          ?.fieldNameId ?? ''
      ];
    else if (this.selectedtype == 'MILESTONE') {
      return val.record[
        workKeyValueModelMilestonearray.find((s) => s.fieldName == d)
          ?.fieldNameId ?? ''
      ];
    } else {
      return val.record[
        workKeyValueModelMbookarray.find((t) => t.fieldName == d)
          ?.fieldNameId ?? ''
      ];
    }
  }
  onbreadCrumbClick(val: BreadcrumbItemClickEvent) {
    var final: ReportBreadcrumbModel[] = [];
    if (val?.item?.queryParams) {
      var fieldName = (
        val?.item?.queryParams as { field: string; value: string }
      ).field;
      var orderNumber =
        this.reportBreadcrumbModel.find((x) => x.fieldName == fieldName)
          ?.orderNumber ?? 0;
      var num = 0;
      this.reportBreadcrumbModel.map((y) => {
        var ccvd: Column[] = this.cols;
        if (orderNumber >= y.orderNumber) {
          num = num + 1;
          ccvd.map((x) => {
            if (x.field == y.fieldName) {
              x.isAnchortagforFilter = false;
            }
          });
          final.push({ ...y, orderNumber: num });
        } else {
          ccvd.map((x) => {
            if (x.field == y.fieldName) {
              x.isAnchortagforFilter = true;
            }
          });
        }
        this.cols = ccvd;
      });
    } else {
      this.reportBreadcrumbModel.map((y) => {
        var ccvd: Column[] = this.cols;
        ccvd.map((x) => {
          if (x.field == y.fieldName) {
            x.isAnchortagforFilter = true;
          }
        });
        this.cols = ccvd;
      });
    }
    console.log("onckij",this.cols)
    this.reportFacade.saveBreadcrumbs([...final]);
  }
  resetflag() {}

  divisionChanged(val: any) {

    this.filterModel = { ...this.filterModel, divisionList: val };
    this.reportFacade.getReports(this.filterModel, this.selectedtype);
  }
  sttausChanged(val: any) {
    this.filterModel = { ...this.filterModel, statusList: val };
    this.reportFacade.getReports(this.filterModel, this.selectedtype);
  }
  districtChanged(val: any) {
    this.filterModel = { ...this.filterModel, districtList: val };
    this.reportFacade.getReports(this.filterModel, this.selectedtype);
  }
  departmentChanged(val: any) {
    this.filterModel = { ...this.filterModel, departmentList: val };
    this.reportFacade.getReports(this.filterModel, this.selectedtype);
  }
  schemechanged(val: any) {
    this.filterModel = { ...this.filterModel, schemeList: val };
    this.reportFacade.getReports(this.filterModel, this.selectedtype);
  }

  packagechanged(val: any) {
    this.filterModel = { ...this.filterModel, packageList: val };
    this.reportFacade.getReports(this.filterModel, this.selectedtype);
  }

  changeyear(val: any) {
  if (val && val[0] && val[1]) {
    const from = moment(val[0]).format('YYYY-MM-DD');
    const to   = moment(val[1]).format('YYYY-MM-DD');

    this.filterModel = {
      ...this.filterModel,
      fromDate: from,
      toDate: to,
    };
    this.from = from;
    this.to   = to;
    this.reportFacade.getReports(this.filterModel, this.selectedtype);
  } else {
    this.filterModel = {
      ...this.filterModel,
      fromDate: null,
      toDate: null,
    };
    this.from = null;
    this.to   = null;
    this.reportFacade.getReports(this.filterModel, this.selectedtype);
  }
}

  changefilter(val: any) {
    this.filterModel = { ...this.filterModel, ...val };
    this.reportFacade.getReports(this.filterModel, this.selectedtype);
  }
  typeChanged(val: any) {
    this.selectedtype = val;
    if (val == 'TENDER') {
      this.filterModel = {
        ...this.freshfilterModelTender,
        departmentList: this.selecteddepartments,
        divisionList: this.selecteddivisions,
        statusList: this.selectedStatuses,
        districtList: this.selectedDistricts,
        fromDate: this.from,
        toDate: this.to,
      };
      this.showscheme = true;
    } else if (val == 'MILESTONE') {
      this.filterModel = {
        ...this.freshfilterModelMilestone,
        departmentList: this.selecteddepartments,
        divisionList: this.selecteddivisions,
        statusList: this.selectedStatuses,
        districtList: this.selectedDistricts,
        fromDate: this.from,
        toDate: this.to,
      };
      this.showscheme = false;
    } else if (val == 'GO') {
      this.filterModel = {
        ...this.freshfilterModelMilestone,
        departmentList: this.selecteddepartments,
        divisionList: this.selecteddivisions,
        statusList: this.selectedStatuses,
        districtList: this.selectedDistricts,
        fromDate: this.from,
        toDate: this.to,
      };
      this.showscheme = false;
    } else {
      this.filterModel = {
        ...this.freshfilterModelMbook,
        departmentList: this.selecteddepartments,
        divisionList: this.selecteddivisions,
        statusList: this.selectedStatuses,
        districtList: this.selectedDistricts,
        fromDate: this.from,
        toDate: this.to,
      };
      this.showscheme = false;
    }
    this.reportFacade.saveBreadcrumbs([]);
    this.reportFacade.getReports(this.filterModel, val);
  }

  ngOnDestroy() {
    this.reportFacade.reset();
   
      
  }
  dc(val: any) {
    return dateconvertionwithOnlyDate(val);
  }
  dcwt(val: any) {
    return dateconvertion(val);
  }
  dayschanged(val: any) {
    this.filterModel = { ...this.filterModel, days: val };
    this.reportFacade.getReports(this.filterModel, this.selectedtype);
  }

// resetFilter(){
//   // Reset all component filters
//   this.selecteddivisions = [];
//   this.selectedStatuses = [];
//   this.selectedDistricts = [];
//   this.selectedPacakges = [];
//   this.selectedtype = 'TENDER';
//   this.selecteddepartments = [];
//   this.selectedYear = [new Date().getFullYear().toString()];
//   this.yearList = [];
//   this.rangeDates = [];
//   this.selectedscheme = [];
//   this.selectedDays = null;
//   this.dashboardQueryParams = {};
 
//   this.from = null;
//   this.to = null;
 
//   // Reset models
//   this.filterModel = null as any;
//   this.filterModelTender = { ...this.freshfilterModelTender };
//   this.filterModelMilestone = { ...this.freshfilterModelMilestone };
//   this.filterModelMbook = { ...this.freshfilterModelMbook };
//   this.reportBreadcrumbModel = [];
//   this.breadcrumbItems = [];
// }

}