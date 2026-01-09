import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { SuccessStatus } from 'src/app/_models/ResponseStatus';
import { ITenderMbookDisplayType } from 'src/app/_models/configuration/tenderdisplay-type';
import { DashboardDivisionCountModel } from 'src/app/_models/dashboard.model';
import { ActionModel } from 'src/app/_models/datatableModel';
import {
  ColumnSearchModel,
  MBookFilterModel,
  TableFilterModel,
} from 'src/app/_models/filterRequest';
import { DashboardService } from 'src/app/_services/dashboard.service';
import { MBookService } from 'src/app/_services/mbook.service';
import { TenderService } from 'src/app/_services/tender.service';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import {
  dangerStatusList,
  getBtnSeverity,
  privileges,
  successStatusList,
  warningStatusList,
} from 'src/app/shared/commonFunctions';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

@Component({
  selector: 'app-mbook-grid',
  templateUrl: './mbook-grid.component.html',
  styleUrls: ['./mbook-grid.component.scss'],
})
export class MbookGridComponent {
  @Input() year: string[] = [];
  chartOptions: any;

  privlegess = privileges;
  chartType!: boolean;
  mbooks!: any[];
  isMimimized: boolean = true;
  SearchString!: string;
  first: number = 0;
  rows: number = 5;
  total!: number;
  defaultSortField: string = 'tenderNumber';
  chartData: any;
  filtermodel!: MBookFilterModel;
  tablefiltermodel!: TableFilterModel;

  subscription!: Subscription;

  types!: ITenderMbookDisplayType[];
  selectedtype: string = 'DIVISION';
  selectedLabel: string | undefined = 'M-Books By Division';

  divisionCount!: any[];
    divisioncount!: DashboardDivisionCountModel;
  constructor(
    private mbookService: MBookService,
    private router: Router,
    public layoutService: LayoutService,
    private dashboardService: DashboardService
  ) {
    this.subscription = this.layoutService.configUpdate$.subscribe(() => {
      this.initChart();
    });
  }

//modified by Indu on 4/1/2025 for mbook dropdowns  -->
  ngOnInit() {
    this.initChart();
    this.types = [
      { id: 'DIVISION', Name: 'M-Books By Division' },
      { id: 'WORK_TYPE', Name: 'M-Books By Work Type' },
      //  { id: 'SUMMARY_BY_DIVISION', Name: 'Abstract Division' },
       { id: 'SUMMARY_BY_DIVISION/DISTRICT', Name: 'Abstract Division/District' },
      
       
    ];
    this.filtermodel = {
      divisionIds: null,
      searchString: null,
      selectionType: this.selectedtype,
      skip: 0,
      sorting: { fieldName: 'tenderNumber', sort: 'ASC' },
      take: 5,
      where: { isActive: true },
      year: this.year,
      isForApproval: false,
      columnSearch: null,
    };
    this.callMbooks();

   
  }
  ngOnChanges() {
    this.filtermodel = {
      divisionIds: null,
      searchString: null,
      selectionType: this.selectedtype,
      skip: 0,
      sorting: { fieldName: 'tenderNumber', sort: 'ASC' },
      take: 5,
      where: { isActive: true },
      year: this.year,
      isForApproval: false,
      columnSearch: null,
    };

   
    this.callMbooks();
    this.callmbookchartdetails();
    this.callMbookDivisionDistrictCount();
    this.callMbookDivisionCount();
     
  }

  changetype() {

    this.callmbookchartdetails();

    if (this.selectedtype == 'SUMMARY_BY_DIVISION/DISTRICT') {
    this.callMbookDivisionDistrictCount();
  }
  }

  actioInvoked(val: any) {
    this.router.navigate(['m-book-manage', 'view', val.id]);
  }
  callmbookchartdetails() {
    this.dashboardService
      .getDashboardMbookChart({
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
  callMbooks() {
    this.mbookService.getMBooks(this.filtermodel).subscribe(
      (data) => {
        if (data && data.status == SuccessStatus) {
          this.mbooks = data.data;
          this.total = data.totalRecordCount;
        }
      },
      (error) => {}
    );
  }
  callMbookDivisionDistrictCount()
  {
    
 this.dashboardService.getDashboard_Mbook_Count(this.filtermodel).subscribe(
      (data) => {
        if (data && data.status == SuccessStatus) {
          this.divisionCount = data.data;
          this.total = data.totalRecordCount;
        }
      },
      (error) => {}
    );
   
  }

 callMbookDivisionCount()
   {
this.dashboardService.getDashboard_Mbookdivision_Count(this.filtermodel).subscribe(
      (data) => {
        if (data && data.status == SuccessStatus) {
          this.divisionCount = data.data;
          this.total = data.totalRecordCount;
        }
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

    this.chartData = {
      labels: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '9', '10'],
      datasets: [
        {
          label: 'First Dataset',
          data: [65, 59, 80, 81, 56, 55, 40, 57, 58, 70, 58, 70],
          backgroundColor: '#a7e996',
          borderColor: '#a7e996',
          borderWidth: 2,
          borderRadius: 3,
        },
        {
          label: 'Second Dataset',
          data: [28, 48, 40, 19, 86, 27, 95, 66, 77, 30, 77, 30],
          backgroundColor: '#f7dd91',
          borderColor: '#f7dd91',
          borderWidth: 2,
          borderRadius: 3,
        },
      ],
    };

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
  getmbookStyleClass() {
    if (this.isMimimized) {
      return 'col-12 xl:col-6';
    } else {
      return 'col-12 xl:col-12';
    }
  }
  togglembookStable() {
    if (this.isMimimized) {
      this.isMimimized = false;
    } else {
      this.isMimimized = true;
    }
  }
  getStyleClass() {
    if (this.isMimimized) {
      return 'col-12 xl:col-6';
    } else {
      return 'col-12 xl:col-12';
    }
  }
  onGlobalFilter(event: any) {
    this.filtermodel = {
      ...this.filtermodel,
      searchString: this.SearchString,
    };
    this.callMbooks();
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
    this.callMbooks();
  }
  getBtnSeverity(statusName: string) {
    return getBtnSeverity(statusName);
  }
  toggletable() {
    if (this.isMimimized) {
      this.isMimimized = false;
    } else {
      this.isMimimized = true;
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
    this.callMbooks();
  }
  changeType(val: any) {
    this.filtermodel = {
      ...this.filtermodel,
      selectionType: val.value,
    };
    this.selectedLabel = this.types.find((x) => x.id == val.value)?.Name;
    this.callMbooks();
    this.callmbookchartdetails();
    if (this.selectedtype == 'SUMMARY_BY_DIVISION/DISTRICT') {
      this.callMbookDivisionDistrictCount();
    }
     if (this.selectedtype == 'SUMMARY_BY_DIVISION') {
      this.callMbookDivisionCount();
    }
  }

   Type="MBOOK";
  Saved="8e6b29fa-7b20-11ee-b363-fa163e14116e";
  Upload="8e6b2e55-7b20-11ee-b363-fa163e14116e";
  Paymentpendin="42b8efcc-50e6-11f0-9806-2c98117e76d0";
  //noActiontaken=["3e9e2493-7afb-11ee-b363-fa163e14116e", "3e9e2783-7afb-11ee-b363-fa163e14116e"];
  noActiontaken="febc5537-50ec-11f0-9806-2c98117e76d0";
  
  goToReports(division: any, status: any) {
    this.router.navigate(['/reports'], {
      queryParams: {
        Type:this.Type,
        Status: status,
        division: division,
        Year: this.year,
      },
    });
  }

  SchemeCompleted(schemeId: any, status: any) {
    this.router.navigate(['/reports'], {
      queryParams: {
        Type:this.Type,
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
 Type:this.Type,
        Year: this.year,
      },
    });
  }

  totalworksScheme(scheme: any) {
    this.router.navigate(['/reports'], {
      queryParams: {
         Type:this.Type,
        scheme: scheme,
        Year: this.year,
      },
      
    });
    
  }

  districtreport(district: any, status: any) {
    
    this.router.navigate(['/reports'], {
      queryParams: {
         Type:this.Type,
        district: district,

        Status: status,

        Year: this.year,
      },
    });
  }

  districttotal(district: any) {
    this.router.navigate(['/reports'], {
      queryParams: {
         Type:this.Type,
        district: district,

        Year: this.year,
      },
    });
  }


//   exportToExcel() {
//   let exportData: any[] = [];
//   let fileName = 'MBookData.xlsx';

//   // 1️⃣ Export for Work Type
//   if (this.selectedtype === 'WORK_TYPE') {
//     exportData = this.mbooks?.map((x, i) => ({
//       'S.No': i + 1,
//       'Work Type': x.workType,
//       'Division': x.divisionName,
//       'District': x.districtName,
//       'M-Book Number': x.mBookNumber,
//       'Milestone Name': x.milestoneName,
//       'Milestone Code': x.milestoneCode,
//       'GO': x.goNumber,
//       'Work ID': x.tenderNumber,
//       'Status': x.statusName
//     })) ?? [];
//     fileName = 'MBook_WorkType.xlsx';
//   }

//   // 2️⃣ Export for Division Summary
//   else if (this.selectedtype === 'DIVISION') {
//     exportData = this.mbooks?.map((x, i) => ({
//       'S.No': i + 1,
//       'Division': x.divisionName,
//       'District': x.districtName,
//       'M-Book Number': x.mBookNumber,
//       'Milestone Name': x.milestoneName,
//       'GO': x.goNumber,
//       'Work ID': x.tenderNumber,
//       'Status': x.statusName
//     })) ?? [];
//     fileName = 'MBook_Division.xlsx';
//   }

//   // 3️⃣ Export for Abstract by Division / District
//   else if (this.selectedtype === 'SUMMARY_BY_DIVISION/DISTRICT') {
//     exportData = this.divisionCount?.map((x, i) => ({
//       'S.No': i + 1,
//       'Division': x.divisionName,
//       'District': x.districtName,
//       'Total MBooks': x.totalDivisionCount,
//       'Total MBook Value (Cr)': (x.mbookAmount / 10000000).toFixed(2),
//       'MBook Not Uploaded': x.mbookNotUploaded,
//       'MBook Uploaded': x.mbookUploaded,
//       'No Action Taken': x.noActionTaken,
//       'Payment Pending': x.paymentPending
//     })) ?? [];
//     fileName = 'MBook_Abstract_Division_District.xlsx';
//   }

//   // 4️⃣ Export for Abstract by Division (if added later)
//   else if (this.selectedtype === 'SUMMARY_BY_DIVISION') {
//     exportData = this.divisionCount?.map((x, i) => ({
//       'S.No': i + 1,
//       'Division': x.divisionName,
//       'Total MBooks': x.totalDivisionCount,
//       'Total MBook Value (Cr)': (x.mbookAmount / 10000000).toFixed(2),
//       'MBook Not Uploaded': x.mbookNotUploaded,
//       'MBook Uploaded': x.mbookUploaded,
//       'No Action Taken': x.noActionTaken,
//       'Payment Pending': x.paymentPending
//     })) ?? [];
//     fileName = 'MBook_Abstract_Division.xlsx';
//   }

//   // 🧾 Export logic using XLSX
//   if (exportData.length === 0) {
//     console.warn('No data available for export.');
//     return;
//   }

//   const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(exportData);
//   const workbook: XLSX.WorkBook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
//   const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
//   const blob: Blob = new Blob([excelBuffer], { type: 'application/octet-stream' });
//   saveAs(blob, fileName);
// }


exportToFile(fileType: 'excel' | 'pdf' | 'csv') {
  let exportData: any[] = [];
  let fileName = 'MBookData';

  // 1️⃣ Export for Work Type
  if (this.selectedtype === 'WORK_TYPE') {
    exportData = this.mbooks?.map((x, i) => ({
      'S.No': i + 1,
      'Work Type': x.workType,
      'Division': x.divisionName,
      'District': x.districtName,
      'M-Book Number': x.mBookNumber,
      'Milestone Name': x.milestoneName,
      'Milestone Code': x.milestoneCode,
      'GO': x.goNumber,
      'Work ID': x.tenderNumber,
      'Status': x.statusName
    })) ?? [];
    fileName = 'MBook_WorkType';
  }

  // 2️⃣ Export for Division Summary
  else if (this.selectedtype === 'DIVISION') {
    exportData = this.mbooks?.map((x, i) => ({
      'S.No': i + 1,
      'Division': x.divisionName,
      'District': x.districtName,
      'M-Book Number': x.mBookNumber,
      'Milestone Name': x.milestoneName,
      'GO': x.goNumber,
      'Work ID': x.tenderNumber,
      'Status': x.statusName
    })) ?? [];
    fileName = 'MBook_Division';
  }

  // 3️⃣ Export for Abstract by Division / District
  else if (this.selectedtype === 'SUMMARY_BY_DIVISION/DISTRICT') {
    exportData = this.divisionCount?.map((x, i) => ({
      'S.No': i + 1,
      'Division': x.divisionName,
      'District': x.districtName,
      'Total MBooks': x.totalDivisionCount,
      'Total MBook Value (Cr)': (x.mbookAmount / 10000000).toFixed(2),
      'MBook Not Uploaded': x.mbookNotUploaded,
      'MBook Uploaded': x.mbookUploaded,
      'No Action Taken': x.noActionTaken,
      'Payment Pending': x.paymentPending
    })) ?? [];
    fileName = 'MBook_Abstract_Division_District';
  }

  // 4️⃣ Export for Abstract by Division
  else if (this.selectedtype === 'SUMMARY_BY_DIVISION') {
    exportData = this.divisionCount?.map((x, i) => ({
      'S.No': i + 1,
      'Division': x.divisionName,
      'Total MBooks': x.totalDivisionCount,
      'Total MBook Value (Cr)': (x.mbookAmount / 10000000).toFixed(2),
      'MBook Not Uploaded': x.mbookNotUploaded,
      'MBook Uploaded': x.mbookUploaded,
      'No Action Taken': x.noActionTaken,
      'Payment Pending': x.paymentPending
    })) ?? [];
    fileName = 'MBook_Abstract_Division';
  }

  // 🧾 If no data
  if (!exportData || exportData.length === 0) {
    console.warn('No data available for export.');
    return;
  }

  // =============================
  // 🔹 EXPORT BASED ON FILE TYPE
  // =============================

  // 🟢 1. EXCEL EXPORT
  if (fileType === 'excel') {
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(exportData);
    const workbook: XLSX.WorkBook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
    const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const blob: Blob = new Blob([excelBuffer], { type: 'application/octet-stream' });
    saveAs(blob, `${fileName}.xlsx`);
  }

  // 🟠 2. CSV EXPORT
  else if (fileType === 'csv') {
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(exportData);
    const csvOutput = XLSX.utils.sheet_to_csv(worksheet);
    const blob = new Blob([csvOutput], { type: 'text/csv;charset=utf-8;' });
    saveAs(blob, `${fileName}.csv`);
  }

  // 🔵 3. PDF EXPORT
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

}
