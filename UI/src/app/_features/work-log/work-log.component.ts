import { Component } from '@angular/core';
import { CustomerService } from 'src/app/demo/service/customer.service';
import { ProductService } from 'src/app/demo/service/product.service';
import { WorkLogConfigFacade } from './state/worklog.facades';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { GOMasterViewModel } from 'src/app/_models/go/tender';
import {
  dateconvertionwithOnlyDate,
  getYearList,
  privileges,
} from 'src/app/shared/commonFunctions';
import { GoFilterModel, TableFilterModel } from 'src/app/_models/filterRequest';
import { SortEvent } from 'primeng/api';
import { Router } from '@angular/router';

interface expandedRows {
  [key: string]: boolean;
}
@UntilDestroy()
@Component({
  selector: 'app-work-log',
  templateUrl: './work-log.component.html',
  styleUrls: ['./work-log.component.scss'],
})
export class WorkLogComponent {
  title: string = 'Government Order';
  first: number = 0;
  rows: number = 10;
  total: number = 0;

  selectedYear: string[] = [new Date().getFullYear().toString()];
  yearList: string[] = getYearList();
  filtermodel!: GoFilterModel;
  tableFilterModel!: TableFilterModel;
  defaultSortField!: string;

  products: GOMasterViewModel[] = [];

  privlegess = privileges;
  rowGroupMetadata: any;

  expandedRows: expandedRows = {};

  isExpanded: boolean = false;

  cols: any[] = [];
records: any[] = [];
fileName: string = 'WorkLog';

exportColumns: any[] = [];

ngAfterViewInit() {
  this.cols = [
    { field: 'goNumber', header: 'GO Number' },
    { field: 'goDepartment', header: 'Department' },
    { field: 'goDate', header: 'GO Date' },
    { field: 'goTotalAmount', header: 'Awarded Value' },
    { field: 'isActive', header: 'Status' },
    { field: 'tenderNumber', header: 'Work Number' },
    { field: 'bidType', header: 'Bid Type' },
    { field: 'mainCategory', header: 'Main Category' },
    { field: 'workPublishedDate', header: 'Work Published Date' },
    { field: 'awardedDate', header: 'Awarded Date' },
    { field: 'workValue', header: 'Work Value' },
  ];
}



  constructor(
    private customerService: CustomerService,
    private router: Router,
    private productService: ProductService,
    private workLogConfigFacade: WorkLogConfigFacade
  ) {}


 private prepareExportData() {
  this.records = [];

  this.products.forEach((go) => {
    // ✅ Push main GO row
    this.records.push({
      goNumber: go.goNumber,
      goDepartment: go.goDepartment,
      goDate: go.goDate ? this.dc(go.goDate) : '',
      goTotalAmount: go.goTotalAmount,
      isActive: go.isActive ? 'Active' : 'In-Active',
      tenderNumber: '',
      bidType: '',
      mainCategory: '',
      workPublishedDate: '',
      awardedDate: '',
      workValue: '',
    });

    // ✅ Add each tender row (child)
    if (go.tenderList && go.tenderList.length > 0) {
      go.tenderList.forEach((tender) => {
        this.records.push({
          goNumber: '', // keep GO columns blank for child rows
          goDepartment: '',
          goDate: '',
          goTotalAmount: '',
          isActive: '',
          tenderNumber: tender.tenderNumber,
          bidType: tender.bidType,
          mainCategory: tender.mainCategory,
          workPublishedDate: tender.startDate ? this.dc(tender.startDate) : '',
          awardedDate: tender.endDate ? this.dc(tender.endDate) : '',
          workValue: tender.workValue,
        });
      });
    }

    // ✅ Add separator row
    this.records.push({
      goNumber: '--------------------------------------------------',
      goDepartment: '',
      goDate: '',
      goTotalAmount: '',
      isActive: '',
      tenderNumber: '',
      bidType: '',
      mainCategory: '',
      workPublishedDate: '',
      awardedDate: '',
      workValue: '',
    });
  });
}

  ngOnInit() {
    this.filtermodel = {
      searchString: null,
      skip: 0,
      sorting: { fieldName: 'goNumber', sort: 'ASC' },
      take: 10,
      where: null,
      year: [this.selectedYear.toString()],
      columnSearch: null,
    };
    this.workLogConfigFacade.getWorkLogs(this.filtermodel);

    this.workLogConfigFacade.selectWorkLogs$
      .pipe(untilDestroyed(this))
      .subscribe((x) => {
        if (x) {
          this.products = x.workLog;
          this.total = x.totalRecords;
          // x.forEach((t) => {
          //   this.products.push({ ...t, tenderList: t.tenderList.slice() });
          // });
          this.prepareExportData(); // ✅ refresh export records
        }
      });
  }
  onPageChange(event: any) {
    this.first = event.first;
    this.rows = event.rows;
    const start = Math.min(this.total - 1, this.first);
    const end = Math.min(this.total, this.first + this.rows);

    this.tableFilterModel = {
      ...this.tableFilterModel,
      skip: start,
      take: end,
    };

    this.filtermodel = { ...this.filtermodel, ...this.tableFilterModel };
    this.workLogConfigFacade.getWorkLogs(this.filtermodel);
  }

  actioInvoked(val: any) {
    this.router.navigate(['tender', 'view', val.id]);
  }
  changeyear(val: any) {
    this.filtermodel = { ...this.filtermodel, year: val };
    this.workLogConfigFacade.getWorkLogs(this.filtermodel);
  }
  expandAll() {
    if (!this.isExpanded) {
      this.products.forEach((product) =>
        product && product.id ? (this.expandedRows[product.id] = true) : ''
      );
    } else {
      this.expandedRows = {};
    }
    this.isExpanded = !this.isExpanded;
  }
  next() {
    this.first = this.first + this.rows;
  }

  prev() {
    this.first = this.first - this.rows;
  }
  isLastPage(): boolean {
    return this.products
      ? this.first === this.products.length - this.rows
      : true;
  }

  isFirstPage(): boolean {
    return this.products ? this.first === 0 : true;
  }
  customSort(event: SortEvent) {
    this.tableFilterModel = {
      ...this.tableFilterModel,
      sorting: {
        fieldName: event.field ? event.field : this.defaultSortField,
        sort: event.order == 1 ? 'ASC' : 'DESC',
      },
    };
    this.filtermodel = { ...this.filtermodel, ...this.tableFilterModel };
    this.workLogConfigFacade.getWorkLogs(this.filtermodel);
  }
  lazyload(event: any) {
    this.tableFilterModel = {
      ...this.tableFilterModel,
      sorting: {
        fieldName:
          event.sortField && event.sortField !== ''
            ? event.sortField
            : this.defaultSortField,
        sort: event.sortOrder == 1 ? 'ASC' : 'DESC',
      },
    };
    this.filtermodel = { ...this.filtermodel, ...this.tableFilterModel };
    this.workLogConfigFacade.getWorkLogs(this.filtermodel);
  }
  dc(date: any) {
    return dateconvertionwithOnlyDate(date);
  }

  ngOnDestroy() {
    this.workLogConfigFacade.reset();
  }


  // ===== CSV Export =====
exportCSV() {
  const csvRows = [];
  const headers = this.cols.map((col) => col.header);
  csvRows.push(headers.join(','));

  this.records.forEach((row) => {
    const values = this.cols.map((col) => `"${row[col.field] ?? ''}"`);
    csvRows.push(values.join(','));
  });

  const blob = new Blob([csvRows.join('\n')], { type: 'text/csv;charset=utf-8;' });
  const link = document.createElement('a');
  const url = URL.createObjectURL(blob);
  link.href = url;
  link.download = `${'Goverment Order'}_Report_${new Date().getTime()}.csv`;
  link.click();
  URL.revokeObjectURL(url);
}


// ===== Excel Export =====
exportExcel() {
  import('xlsx').then((xlsx) => {
    const formattedData = this.records.map((row: any) => {
      const newRow: any = {};
      this.cols.forEach((col: any) => {
        newRow[col.header] = row[col.field];
      });
      return newRow;
    });

    const worksheet = xlsx.utils.json_to_sheet(formattedData);
    const workbook = {
      Sheets: { [this.fileName]: worksheet },
      SheetNames: [this.fileName],
    };

    const excelBuffer: any = xlsx.write(workbook, {
      bookType: 'xlsx',
      type: 'array',
    });

    import('file-saver').then((FileSaverModule: any) => {
      const saveAs = FileSaverModule.default || FileSaverModule.saveAs;
      const data: Blob = new Blob([excelBuffer], {
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8',
      });
      saveAs(data, `${'Goverment Order'}_Report_${new Date().getTime()}.xlsx`);
    });
  });
}


// ===== PDF Export =====
exportPdf() {
  this.exportColumns = this.cols.map((col) => ({
    title: col.header,
    dataKey: col.field,
  }));

  import('jspdf').then((jsPDF) => {
    import('jspdf-autotable').then(() => {
      const doc = new jsPDF.default('l', 'px', 'a3');
      const fileName = this.fileName;

      (doc as any).autoTable({
        columns: this.exportColumns,
        body: this.records,
        theme: 'striped',
        headStyles: { fillColor: [41, 128, 185] },
        bodyStyles: { fontSize: 9 },
        margin: { top: 40 },
        didDrawPage: function (data: any) {
          doc.setFontSize(16);
          doc.text(fileName + ' Report', data.settings.margin.left, 30);
        },
      });

      doc.save(`${'Goverment Order'}_Report_${new Date().getTime()}.pdf`);
    });
  });
}

}
