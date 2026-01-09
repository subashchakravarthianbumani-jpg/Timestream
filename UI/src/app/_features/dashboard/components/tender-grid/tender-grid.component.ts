import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { dE } from '@fullcalendar/core/internal-common';

import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';
import { saveAs } from 'file-saver';
import { Subscription } from 'rxjs';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { SuccessStatus } from 'src/app/_models/ResponseStatus';
import { ITenderMbookDisplayType } from 'src/app/_models/configuration/tenderdisplay-type';
import {
  DashboardDivisionCountModel,
  DashboardFilterModel,
  TenderChartModel,
} from 'src/app/_models/dashboard.model';
import {
  TenderFilterModel,
  TableFilterModel,
  ColumnSearchModel,
} from 'src/app/_models/filterRequest';
import { TCModel } from 'src/app/_models/user/usermodel';
import { DashboardService } from 'src/app/_services/dashboard.service';
import { MBookService } from 'src/app/_services/mbook.service';
import { ReportsService } from 'src/app/_services/report.service';
import { TenderService } from 'src/app/_services/tender.service';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { getBtnSeverity, privileges } from 'src/app/shared/commonFunctions';

@Component({
  selector: 'app-tender-grid',
  templateUrl: './tender-grid.component.html',
  styleUrls: ['./tender-grid.component.scss'],
})
export class TenderGridComponent {
  @Input() year: string[] = [];
  chartOptions: any;

  chartType!: boolean;

  privlegess = privileges;
  tenders!: any[];
  divisionCount!: any[];
  isMimimizedoftenders: boolean = true;
  tenderSearchString!: string;
  first: number = 0;
  rows: number = 5;
  total!: number;
  defaultSortField: string = 'startDate';
  chartData: any;
  filtermodel!: any;
  tablefiltermodel!: TableFilterModel;
  divisioncount!: DashboardDivisionCountModel;
  subscription!: Subscription;

  divisions!: TCModel[];

  schemes!: TCModel[];
  dtistrictsList!: TCModel[];
  selecteddivisions!: any[];

  types!: ITenderMbookDisplayType[];

  //new changes

  selectedtype: string = 'DIVISION';
  selectedLabel: string | undefined = 'Work By Division';
  dashboardfiltermodel!: DashboardFilterModel;
  constructor(
    private mbookService: MBookService,
    public layoutService: LayoutService,
    private router: Router,
    private tenderService: TenderService,
    private dashboardService: DashboardService,
    private reportsService: ReportsService
  ) {
    this.subscription = this.layoutService.configUpdate$.subscribe(() => {
      this.initChart();
    });
  }
  //modified by Indu on 3/12/2025 for summary by division /district and scheme records and excel for exporting
  ngOnInit() {
    this.initChart();
    this.types = [
      { id: 'DIVISION', Name: 'Work By Division' },
      { id: 'WORK_TYPE', Name: 'Work By Work Type' },
      //{ id: 'SUMMARY_BY_DIVISION', Name: 'Abstract By Division' },
      {
        id: 'SUMMARY_BY_DIVISION/DISTRICT',
        Name: 'Abstract By Division/District',
      },
      { id: 'SUMMARY_BY_SCHEME', Name: 'Abstract By Scheme' },
    ];

    this.filtermodel = {
      districtList: null,
      divisionList: null,
      selectionType: this.selectedtype,
      fromDate: null,
      searchString: null,
      skip: 0,
      sorting: { fieldName: 'startDate', sort: 'ASC' },
      toDate: null,
      take: 5,
      where: null,
      workType: null,
      year: this.year,
      columnSearch: null,
    };

    // this.filtermodel.divisionList=this.selecteddivisions;
    this.reportsService.getreportform().subscribe(
      (x) => {
        this.divisions = x.data.divisions;

        this.dtistrictsList = x.data.districts;
        this.schemes = x.data.schemeName;
      },
      (error) => {}
    );
  }

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
      year: this.year,
      columnSearch: null,
    };

    if (this.selectedtype == 'WORK_TYPE' || this.selectedtype == 'DIVISION') {
      this.calltenders();
    }
    this.calltenderchartdetails();
    if (this.selectedtype == 'SUMMARY_BY_DIVISION') {
      this.calltenderDivisionCount();
    }
    if (this.selectedtype == 'SUMMARY_BY_DIVISION/DISTRICT') {
      this.calltenderDivisionDistrictCount();
    }
    if (this.selectedtype == 'SUMMARY_BY_SCHEME') {
      this.calltenderSchemeCount();
    }
  }
  changetype() {
    this.calltenderchartdetails();
  }

  calltenders() {
    this.tenderService.getTenders(this.filtermodel).subscribe(
      (data) => {
        if (data && data.status == SuccessStatus) {
          this.tenders = data.data;
          this.total = data.totalRecordCount;
        }
      },
      (error) => {}
    );
  }
  calltenderDivisionCount() {
    this.dashboardService.getDashboard_Count(this.filtermodel).subscribe(
      (data) => {
        if (data && data.status == SuccessStatus) {
          this.divisionCount = data.data;
          this.total = data.totalRecordCount;
        }
      },
      (error) => {}
    );
  }

  calltenderSchemeCount() {
    this.dashboardService.getDashboard_Scheme_Count(this.filtermodel).subscribe(
      (data) => {
        if (data && data.status == SuccessStatus) {
          console.log(data);
          this.divisionCount = data.data;
          this.total = data.totalRecordCount;
        }
      },
      (error) => {}
    );
  }


exportToExcel(fileType: 'xlsx' | 'csv' | 'pdf') {
  let exportData: any[] = [];
  let fileName = 'Tender Details';

  // 1️⃣ Work Type / Division
  if (this.selectedtype === 'WORK_TYPE' || this.selectedtype === 'DIVISION') {
    exportData = this.tenders?.map((item, index) => ({
      SNo: index + 1,
      WorkType: item.workType || '-',
      Division: item.divisionName || '-',
      District: item.districtName || '-',
      Scheme: item.schemeName || '-',
      WorkId: item.tenderNumber || '-',
      GO: item.goNumber || '-',
      StartDate: item.startDate
        ? new Date(item.startDate).toLocaleDateString('en-GB')
        : '-',
      EndDate: item.endDate
        ? new Date(item.endDate).toLocaleDateString('en-GB')
        : '-',
      Status: item.workStatus || '-',
    })) || [];
  }

  // 2️⃣ Division/District Summary
  else if (
    this.selectedtype === 'SUMMARY_BY_DIVISION' ||
    this.selectedtype === 'SUMMARY_BY_DIVISION/DISTRICT'
  ) {
    exportData = this.divisionCount?.map((item, index) => ({
      SNo: index + 1,
      Division: item.divisionName || '-',
      District:
        this.selectedtype === 'SUMMARY_BY_DIVISION/DISTRICT'
          ? item.districtName || '-'
          : '-',
      TotalWorks: item.totalDivisionCount || 0,
      TotalValue: item.totalWorkValue || 0,
      Completed: item.completed || 0,
      InProgress: item.inProgress || 0,
      SlowProgress: item.slowProgress || 0,
      OnHold: item.onHold || 0,
      NotStarted: item.notStarted || 0,
    })) || [];
  }

  // 3️⃣ Scheme Summary
  else if (this.selectedtype === 'SUMMARY_BY_SCHEME') {
    exportData = this.divisionCount?.map((item, index) => ({
      SNo: index + 1,
      SchemeName: item.schemeName || '-',
      TotalWorks: item.totalDivisionCount || 0,
      TotalValue: item.totalWorkValue || 0,
      Completed: item.completed || 0,
      InProgress: item.inProgress || 0,
      SlowProgress: item.slowProgress || 0,
      OnHold: item.onHold || 0,
      NotStarted: item.notStarted || 0,
    })) || [];
  }

  if (!exportData.length) {
    console.warn('⚠️ No data available for export.');
    return;
  }

  // 🔹 Common filename base
  let baseName = '';
  switch (this.selectedtype) {
    case 'WORK_TYPE':
      baseName = 'WorkType_Report';
      break;
    case 'DIVISION':
      baseName = 'Division_Report';
      break;
    case 'SUMMARY_BY_SCHEME':
      baseName = 'Scheme_Summary';
      break;
    case 'SUMMARY_BY_DIVISION/DISTRICT':
      baseName = 'Division_District_Summary';
      break;
    default:
      baseName = 'WorkSummary';
  }

  // 🔹 Create worksheet
  const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(exportData);
  const workbook: XLSX.WorkBook = { Sheets: { Data: worksheet }, SheetNames: ['Data'] };

  // 🔹 Export by fileType
  if (fileType === 'xlsx') {
    XLSX.writeFile(workbook, `${baseName}.xlsx`);
  } 
  else if (fileType === 'csv') {
    const csvOutput = XLSX.utils.sheet_to_csv(worksheet);
    const blob = new Blob([csvOutput], { type: 'text/csv;charset=utf-8;' });
    saveAs(blob, `${baseName}.csv`);
  } 
   else if (fileType === 'pdf') {
    const doc = new jsPDF('l', 'mm', 'a4'); // landscape mode
    doc.text(`${fileName} Report`, 14, 10);

    const headers = [Object.keys(exportData[0])];
    const data = exportData.map(obj => Object.values(obj));

    autoTable(doc, {
      head: headers,
      body: data as any,
      startY: 15,
      styles: { fontSize: 8 },
      headStyles: { fillColor: [22, 160, 133] },
      alternateRowStyles: { fillColor: [240, 240, 240] }
    });

    doc.save(`${fileName}.pdf`);
  }
}


// exportToExcel() {
//   let exportData: any[] = [];

//   if (this.selectedtype === 'WORK_TYPE' || this.selectedtype === 'DIVISION') {
//     // 🔹 Export data from tenders table
//     exportData = this.tenders?.map((item, index) => ({
//       SNo: index + 1,
//       WorkType: item.workType || '-',
//       Division: item.divisionName || '-',
//       District: item.districtName || '-',
//       Scheme: item.schemeName || '-',
//       WorkId: item.tenderNumber || '-',
//       GO: item.goNumber || '-',
//       StartDate: item.startDate
//         ? new Date(item.startDate).toLocaleDateString('en-GB')
//         : '-',
//       EndDate: item.endDate
//         ? new Date(item.endDate).toLocaleDateString('en-GB')
//         : '-',
//       Status: item.workStatus || '-',
//     })) || [];
//   } 
//   else if (
//     this.selectedtype === 'SUMMARY_BY_DIVISION' ||
//     this.selectedtype === 'SUMMARY_BY_DIVISION/DISTRICT'
//   ) {
//     // 🔹 Export Division/District Summary data
//     exportData = this.divisionCount?.map((item, index) => ({
//       SNo: index + 1,
//       Division: item.divisionName || '-',
//       District:
//         this.selectedtype === 'SUMMARY_BY_DIVISION/DISTRICT'
//           ? item.districtName || '-'
//           : '-',
//       TotalWorks: item.totalDivisionCount || 0,
//       TotalValue: item.totalWorkValue || 0,
//       Completed: item.completed || 0,
//       InProgress: item.inProgress || 0,
//       SlowProgress: item.slowProgress || 0,
//       OnHold: item.onHold || 0,
//       NotStarted: item.notStarted || 0,
//     })) || [];
//   } 
//   else if (this.selectedtype === 'SUMMARY_BY_SCHEME') {
//     // 🔹 Export Scheme Summary data
//     exportData = this.divisionCount?.map((item, index) => ({
//       SNo: index + 1,
//       SchemeName: item.schemeName || '-',
//       TotalWorks: item.totalDivisionCount || 0,
//       TotalValue: item.totalWorkValue || 0,
//       Completed: item.completed || 0,
//       InProgress: item.inProgress || 0,
//       SlowProgress: item.slowProgress || 0,
//       OnHold: item.onHold || 0,
//       NotStarted: item.notStarted || 0,
//     })) || [];
//   }

//   if (!exportData.length) {
//     console.warn('⚠️ No data available for export.');
//     return;
//   }

//   // ✅ Create Excel sheet and file
//   const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(exportData);
//   const workbook: XLSX.WorkBook = {
//     Sheets: { Data: worksheet },
//     SheetNames: ['Data'],
//   };

//   let filename = '';
//   switch (this.selectedtype) {
//     case 'WORK_TYPE':
//       filename = 'WorkType_Report.xlsx';
//       break;
//     case 'DIVISION':
//       filename = 'Division_Report.xlsx';
//       break;
//     case 'SUMMARY_BY_SCHEME':
//       filename = 'Scheme_Summary.xlsx';
//       break;
//     case 'SUMMARY_BY_DIVISION/DISTRICT':
//       filename = 'Division_District_Summary.xlsx';
//       break;
//     default:
//       filename = 'WorkSummary.xlsx';
//   }

//   XLSX.writeFile(workbook, filename);
// }

  calltenderDivisionDistrictCount() {
    this.dashboardService
      .getDashboard_district_Count(this.filtermodel)
      .subscribe(
        (data) => {
          if (data && data.status == SuccessStatus) {
            this.divisionCount = data.data;
            this.total = data.totalRecordCount;
          }
        },
        (error) => {}
      );
  }
  divisionChanged(val: any) {
    //this.filtermodel = { ...this.filtermodel, DivisionIds: val };
    this.filtermodel.divisionIds = val;

    this.calltenderDivisionDistrictCount();
    this.filtermodel.divisionIds = [];
  }
  districtChanged(val: any) {
    this.filtermodel = { ...this.filtermodel, DistrictIds: val };

    this.calltenderDivisionDistrictCount();
    this.filtermodel.DistrictIds = [];
  }

  calltenderchartdetails() {
    this.dashboardService
      .getDashboardTenderChart({
        departmentIds: null,
        divisionIds: null,
        year: this.year,
        selectionType: this.selectedtype,
        costOrCount: this.chartType ? 'COST' : 'COUNT',
      })
      .subscribe(
        (x) => {
          this.chartData = x.data;
        },
        (error) => {}
      );
  }
  initChart() {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--text-color');
    const textColorSecondary = documentStyle.getPropertyValue(
      '--text-color-secondary'
    );
    const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

    this.chartOptions = {
      maintainAspectRatio: false,
      aspectRatio: 0.8,
      plugins: {
        legend: {
          labels: {
            color: textColor,
          },
        },
      },
      exportEnabled: true,
      scales: {
        x: {
          ticks: {
            color: textColorSecondary,
            font: {
              weight: 500,
            },
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false,
          },
        },
        y: {
          ticks: {
            color: textColorSecondary,
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false,
          },
        },
      },
    };
  }
  getStyleClass() {
    if (this.isMimimizedoftenders) {
      return 'col-12 xl:col-6';
    } else {
      return 'col-12 xl:col-12';
    }
  }
  actioInvoked(val: any) {
    this.router.navigate(['tender', 'view', val.id]);
  }
  onGlobalFilter(event: any) {
    this.filtermodel = {
      ...this.filtermodel,
      searchString: this.tenderSearchString,
    };
    this.calltenders();
  }
  lazyload(event: any) {
    var list: ColumnSearchModel[] = [];
    if (event.filters) {
      this.filtermodel = {
        ...this.filtermodel,
        sorting: {
          fieldName: event.field ? event.field : this.defaultSortField,
          sort: event.order == 1 ? 'ASC' : 'DESC',
        },
        columnSearch: list,
      };
    }

    this.filtermodel = {
      ...this.filtermodel,
      sorting: {
        fieldName:
          event.sortField && event.sortField !== ''
            ? event.sortField
            : this.defaultSortField,
        sort: event.sortOrder == 1 ? 'ASC' : 'DESC',
      },
    };
    this.calltenders();
  }
  toggletable() {
    if (this.isMimimizedoftenders) {
      this.isMimimizedoftenders = false;
    } else {
      this.isMimimizedoftenders = true;
    }
  }
  onPageChange(event: any) {
    this.first = event.first;
    this.rows = event.rows;
    const start = Math.min(this.total - 1, this.first);
    const end = Math.min(this.total, this.first + this.rows);
    this.filtermodel = {
      ...this.filtermodel,
      skip: start,
      take: event.rows,
    };
    this.calltenders();
  }
  getBtnSeverity(statusName: string) {
    return getBtnSeverity(statusName);
  }

  changeType(val: any) {
    this.filtermodel = {
      ...this.filtermodel,
      selectionType: val.value,
    };

    this.selectedLabel = this.types.find((x) => x.id == val.value)?.Name;
    this.calltenders();
    this.calltenderchartdetails();
    if (this.selectedtype == 'SUMMARY_BY_DIVISION') {
      this.calltenderDivisionCount();
    }
    if (this.selectedtype == 'SUMMARY_BY_DIVISION/DISTRICT') {
      this.calltenderDivisionDistrictCount();
    }
    if (this.selectedtype == 'SUMMARY_BY_SCHEME') {
      this.calltenderSchemeCount();
    }
  }
  goToReports(division: any, status: any) {
    this.router.navigate(['/reports'], {
      queryParams: {
        Status: status,
        division: division,
        Year: this.year,
      },
    });
  }

  SchemeCompleted(schemeId: any, status: any) {
    this.router.navigate(['/reports'], {
      queryParams: {
        Status: status,
        scheme: schemeId,
        Year: this.year,
      },
    });
  }

  totalworks(division: any) {
    this.router.navigate(['/reports'], {
      queryParams: {
        division: division,

        Year: this.year,
      },
    });
  }

  totalworksScheme(scheme: any) {
    this.router.navigate(['/reports'], {
      queryParams: {
        scheme: scheme,
        Year: this.year,
      },
    });
  }

  districtreport(district: any, status: any) {
    this.router.navigate(['/reports'], {
      queryParams: {
        district: district,

        Status: status,

        Year: this.year,
      },
    });
  }

  districttotal(district: any) {
    this.router.navigate(['/reports'], {
      queryParams: {
        district: district,

        Year: this.year,
      },
    });
  }
}
