import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
  ViewContainerRef,
} from '@angular/core';
import * as FileSaver from 'file-saver';
import * as moment from 'moment';
import {
  ConfirmationService,
  LazyLoadEvent,
  MessageService,
  SortEvent,
} from 'primeng/api';
import { Table, TableFilterEvent } from 'primeng/table';
import {
  ActionModel,
  Actions,
  Column,
  ExportColumn,
} from 'src/app/_models/datatableModel';
import {
  ColumnSearchModel,
  ColumnSortingModel,
  TableFilterModel,
  TenderFilterModel,
} from 'src/app/_models/filterRequest';
import {
  dangerStatusList,
  dateconvertion,
  dateconvertionwithOnlyDate,
  getBtnSeverity,
  getcolorforProgress,
  successStatusList,
  warningStatusList,
} from '../commonFunctions';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-datatable-pagination',
  templateUrl: './datatable-pagination.component.html',
  styleUrls: ['./datatable-pagination.component.scss'],
  providers: [ConfirmationService, MessageService],
})
export class DatatablePaginationComponent implements OnInit {
  // @Input() set records(recordslist: any[]) {
  //   if (recordslist) {
  //     this.filteredRecords = recordslist;
  //     this.entirerecords = recordslist;
  //   }
  // }
  @Input() records!: any[];
  @Input() cols!: Column[];
  @Input() searchableColumns!: string[];
  @Input() actions!: Actions[];
  @Input() fileName!: string;
  @Input() rows: number = 10;
  @Input() total: number = 0;
  @Input() defaultSortField!: string;
  @Input() defaultSortOrder: number = 1;
  @Input() hasActiveInactive: boolean = false;
  @Input() defaultstatus: boolean = false;
  @Input() filterModel!: TableFilterModel;
  @Input() canShowAction: boolean = true;
  @Input() displaySize: string = 'FULL';

  @Output() invokeAction = new EventEmitter<ActionModel>();
  @Output() changeStatus = new EventEmitter<boolean>();
  @Output() changefilter = new EventEmitter<TableFilterModel>();
  @Output() iconAction = new EventEmitter<any>();

  constructor(
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private cookieService: CookieService
  ) {}

  selectedProducts!: any[];
  filteredRecords: any[] = [];
  entirerecords: any[] = [];

  exportColumns!: ExportColumn[];
  rowsPerPageOptions!: any[];

  first: number = 0;
  searchString: string = '';
  onLabel: string = 'Show Active';
  offLabel: string = 'Show In-Active';

  userPermissions!: string[];
  ngOnInit() {
    const privillage: any = this.cookieService.get('privillage');
    if (privillage) {
      this.userPermissions = privillage.split(',');
    }
    this.exportColumns = this.cols.map((col) => ({
      title: col.header,
      dataKey: col.field,
    }));
    this.reset();
  }
  ngOnChanges() {
    if (this.records) {
      this.entirerecords = [...this.records];
    }
    if (this.displaySize === 'FULL') {
      this.rowsPerPageOptions = [25, 50, 100, 500, this.total];
    } else if (this.displaySize === 'HALF') {
      this.rowsPerPageOptions = [10, 25, 50, 100, this.total];
    } else if (this.displaySize === 'LESS_HALF') {
      this.rowsPerPageOptions = [5, 10, 25, 50, this.total];
    }
  }
  customSort(event: SortEvent) {
    this.filterModel = {
      ...this.filterModel,
      sorting: {
        fieldName: event.field ? event.field : this.defaultSortField,
        sort: event.order == 1 ? 'ASC' : 'DESC',
      },
    };
    this.changefilter.emit(this.filterModel);
  }
  load() {
    alert();
  }
  lazyload(event: any) {
    var list: ColumnSearchModel[] = [];
    if (event.filters) {
      var keys: string[] = Object.keys(event.filters as object);
      keys.forEach((key) => {
        if (event.filters[key].value) {
          list.push({ fieldName: key, searchString: event.filters[key].value });
        }
      });
    }

    this.filterModel = {
      ...this.filterModel,
      sorting: {
        fieldName:
          event.sortField && event.sortField !== ''
            ? event.sortField
            : this.defaultSortField,
        sort: event.sortOrder == 1 ? 'ASC' : 'DESC',
      },
      columnSearch: list,
    };
    this.changefilter.emit(this.filterModel);
  }
  next() {
    this.first = this.first + this.rows;
  }

  prev() {
    this.first = this.first - this.rows;
  }

  reset() {
    this.first = 0;
    const start = Math.min(this.total - 1, this.first);
    const end = Math.min(this.total, this.first + this.rows);
    const prod = this.entirerecords;
    this.filteredRecords = prod.slice(start, end);
  }

  isLastPage(): boolean {
    return this.entirerecords
      ? this.first === this.entirerecords.length - this.rows
      : true;
  }

  isFirstPage(): boolean {
    return this.entirerecords ? this.first === 0 : true;
  }

  onPageChange(event: any) {
    this.first = event.first;
    this.rows = event.rows;
    const start = Math.min(this.total - 1, this.first);
    const end = Math.min(this.total, this.first + this.rows);

    this.filterModel = {
      ...this.filterModel,
      skip: start,
      take: event.rows,
    };
    this.changefilter.emit(this.filterModel);
  }

  onGlobalFilter(event: any) {
    this.filterModel = {
      ...this.filterModel,
      searchString: this.searchString,
    };
    this.changefilter.emit(this.filterModel);
  }

  makeAction(action: string, record: any, event: Event) {
    if (action == 'DELETE') {
      this.confirmationService.confirm({
        key: 'confirm2',
        target: (event.target as HTMLInputElement) || new EventTarget(),
        message: 'Are you sure that you want to delete?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.invokeAction.emit({ type: action, record });
        },
        reject: () => {},
      });
    } else if (action == 'INACTIVATE') {
      this.confirmationService.confirm({
        key: 'confirm2',
        target: (event.target as HTMLInputElement) || new EventTarget(),
        message: 'Are you sure that you want to InActivate?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.invokeAction.emit({ type: action, record });
        },
        reject: () => {},
      });
    } else if (action == 'ACTIVATE') {
      this.confirmationService.confirm({
        key: 'confirm2',
        target: (event.target as HTMLInputElement) || new EventTarget(),
        message: 'Are you sure that you want to Activate?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.invokeAction.emit({ type: action, record });
        },
        reject: () => {},
      });
    } else {
      this.invokeAction.emit({ type: action, record });
    }
  }
  changeEvent(val: any) {
    this.changeStatus.emit(val.checked);
  }
  dc(date: any, field?: string) {
    if (field === 'startDate' || field === 'endDate') {
      return dateconvertionwithOnlyDate(date);
    } else {
      return dateconvertion(date);
    }
  }
  getBtnSeverity(statusName: string) {
    return getBtnSeverity(statusName);
  }
  getcolorforProgress(val: number, type: string) {
    return getcolorforProgress(val, type);
  }
  getPerm(privleges: any) {
    if (typeof privleges == 'string') {
      return this.userPermissions.includes(privleges);
    } else if (typeof privleges == 'object') {
      return this.userPermissions.find((x) => privleges.includes(x));
    } else {
      return true;
    }
  }

  exportPdf() {
    var totalPagesExp = '{total_pages_count_string}';
    import('jspdf').then((jsPDF) => {
      import('jspdf-autotable').then((x) => {
        const doc = new jsPDF.default('l', 'px', 'a3');
        var fileName = this.fileName;
        (doc as any).autoTable(this.exportColumns, this.entirerecords);
        (doc as any).autoTable({
          willDrawPage: function (data: any) {
            // Header
            doc.setFontSize(20);
            doc.setTextColor(40);
            // if (base64Img) {
            //   doc.addImage(base64Img, 'JPEG', data.settings.margin.left, 15, 10, 10)
            // }
            doc.text(fileName + ' Report', data.settings.margin.left, 20);
          },
          didDrawPage: function (data: any) {
            // Footer
            var str = 'Page ' + (doc as any).internal.getNumberOfPages();
            // Total page number plugin only available in jspdf v1.0+
            if (typeof doc.putTotalPages === 'function') {
              str = str + ' of ' + totalPagesExp;
            }
            doc.setFontSize(10);

            // jsPDF 1.4+ uses getHeight, <1.4 uses .height
            var pageSize = doc.internal.pageSize;
            var pageHeight = pageSize.height
              ? pageSize.height
              : pageSize.getHeight();
            doc.text(str, data.settings.margin.left, pageHeight - 10);
          },
          theme: 'striped',
        });
        // Total page number plugin only available in jspdf v1.0+
        if (typeof doc.putTotalPages === 'function') {
          doc.putTotalPages(totalPagesExp);
        }
        doc.setProperties({
          title: this.fileName + '_Report',
        });

        doc.save(this.fileName + '_Report_' + new Date().getTime() + '.pdf');
      });
    });
  }

  exportExcel() {
    import('xlsx').then((xlsx) => {
      const worksheet = xlsx.utils.json_to_sheet(this.entirerecords);
      const workbook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
      const excelBuffer: any = xlsx.write(workbook, {
        bookType: 'xlsx',
        type: 'array',
      });
      this.saveAsExcelFile(
        excelBuffer,
        this.fileName + '_Report_' + new Date().getTime()
      );
    });
  }
  gettimestanp() {
    return new Date().getTime();
  }
  saveAsExcelFile(buffer: any, fileName: string): void {
    let EXCEL_TYPE =
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    let EXCEL_EXTENSION = '.xlsx';
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE,
    });
    FileSaver.saveAs(data, fileName + EXCEL_EXTENSION);
  }

  onIconAction(type: string, record: any) {
  this.iconAction.emit({ type, record });
}
}
