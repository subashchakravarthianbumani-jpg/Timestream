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
import { ConfirmationService, MessageService, SortEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import {
  ActionModel,
  Actions,
  Column,
  ExportColumn,
} from 'src/app/_models/datatableModel';
import {
  dangerStatusList,
  dateconvertion,
  getBtnSeverity,
  successStatusList,
  warningStatusList,
} from '../commonFunctions';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-datatable',
  templateUrl: './datatable.component.html',
  styleUrls: ['./datatable.component.scss'],
  providers: [ConfirmationService, MessageService],
})
export class DatatableComponent implements OnInit {
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
  @Input() canShowAction: boolean = true;
  @Input() displaySize: string = 'FULL';

  @Output() invokeAction = new EventEmitter<ActionModel>();
  @Output() changeStatus = new EventEmitter<boolean>();
  constructor(
    private confirmationService: ConfirmationService,
    private cookieService: CookieService
  ) {}

  selectedProducts!: any[];
  filteredRecords: any[] = [];
  entirerecords: any[] = [];
  rowsPerPageOptions: number[] = [];

  exportColumns!: ExportColumn[];

  userPermissions!: string[];

  first: number = 0;
  onLabel: string = 'Show Active';
  offLabel: string = 'Show In-Active';

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
    this.entirerecords?.sort((data1, data2) => {
      let value1 = data1[event.field ?? ''];
      let value2 = data2[event.field ?? ''];
      let result = null;

      if (value1 == null && value2 != null) result = -1;
      else if (value1 != null && value2 == null) result = 1;
      else if (value1 == null && value2 == null) result = 0;
      else if (typeof value1 === 'string' && typeof value2 === 'string')
        result = value1.localeCompare(value2);
      else result = value1 < value2 ? -1 : value1 > value2 ? 1 : 0;

      return event.order ?? 0 * result;
    });
  }

  exportPdf() {
    import('jspdf').then((jsPDF) => {
      import('jspdf-autotable').then((x) => {
        const doc = new jsPDF.default('p', 'px', 'a4');
        (doc as any).autoTable(this.exportColumns, this.entirerecords);
        doc.save('products.pdf');
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
      this.saveAsExcelFile(excelBuffer, this.fileName);
    });
  }

  saveAsExcelFile(buffer: any, fileName: string): void {
    let EXCEL_TYPE =
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    let EXCEL_EXTENSION = '.xlsx';
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE,
    });
    FileSaver.saveAs(
      data,
      fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION
    );
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
    const prod = this.entirerecords;
    const start = Math.min(this.total - 1, this.first);
    const end = Math.min(this.total, this.first + this.rows);
    this.filteredRecords = prod.slice(start, end);
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
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
        reject: () => {
          // this.messageService.add({
          //   severity: 'error',
          //   summary: 'Rejected',
          //   detail: 'You have rejected',
          // });
        },
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
  dc(date: any) {
    return dateconvertion(date);
  }

  getBtnSeverity(statusName: string) {
    return getBtnSeverity(statusName);
  }
  getStyle() {
    if (!this.canShowAction) {
      return 'extradttable';
    }
    return 'dttable';
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
}
