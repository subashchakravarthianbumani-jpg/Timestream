import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import * as FileSaver from 'file-saver';
import * as moment from 'moment';
import { ConfirmationService, MessageService } from 'primeng/api';
import {
  ActionModel,
  Actions,
  Column,
  ExportColumn,
} from 'src/app/_models/datatableModel';
import { TableFilterModel } from 'src/app/_models/filterRequest';
import {
  CommentMasterModel,
  MilestoneFileModel,
  WorkActivityModel,
  WorkMasterViewModel,
  WorkTemplateMasterViewModel,
} from 'src/app/_models/go/tender';
import { MBookMasterViewModel } from 'src/app/_models/mbook/mbook';
import { EventItem } from 'src/app/_models/utils';
import { GeneralService } from 'src/app/_services/general.service';
import { MBookService } from 'src/app/_services/mbook.service';
import { TenderService } from 'src/app/_services/tender.service';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import {
  convertoWords,
  dateconvertion,
  dateconvertionwithOnlyDate,
  getBtnSeverity,
  getcolorforProgress,
  imgExtension,
} from 'src/app/shared/commonFunctions';
import { environment } from 'src/environments/environment';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

@Component({
  selector: 'app-report-milestone',
  templateUrl: './report-milestone.component.html',
  styleUrls: ['./report-milestone.component.scss'],
})
export class ReportMilestoneComponent {
  @Input() records!: any[];
  @Input() cols!: Column[];
  @Input() filterColumns!: Column[];
  @Input() actions!: Actions[];
  @Input() fileName!: string;
  @Input() rows: number = 10;
  @Input() total: number = 0;
  @Input() defaultSortField!: string;
  @Input() defaultSortOrder: number = 1;
  @Input() filterModel!: TableFilterModel;
  @Input() canShowAction: boolean = true;

  @Output() invokeAction = new EventEmitter<ActionModel>();
  @Output() changecolFilter = new EventEmitter();
  @Output() changefilter = new EventEmitter<TableFilterModel>();

  entirerecords: any[] = [];
  first: number = 0;
  exportColumns!: ExportColumn[];
  rowsPerPageOptions!: any[];
  tenderVisible: boolean = false;
  bookVisible: boolean = false;
  imgVisible: boolean = false;
  tender!: WorkMasterViewModel;
  templateWithMilestone!: WorkTemplateMasterViewModel;
  comments!: CommentMasterModel[];
  activities!: WorkActivityModel[];
  events!: EventItem[];
  activitiesevents!: EventItem[];
  mBookDetails?: MBookMasterViewModel;
  filesList: MilestoneFileModel[] = [];
  MbbokfilesList: MilestoneFileModel[] = [];
  mBookForm!: FormGroup;
  imgExtension: string[] = imgExtension;
  commentfiletrModel: TableFilterModel = {
    columnSearch: null,
    searchString: null,
    skip: 0,
    sorting: null,
    take: 500000,
  };
  tenderSearchString: string = '';
  imgDetails!: { name: string; url: string; fileId: string };
  get f() {
    return this.mBookForm?.controls;
  }
  get t() {
    return this.f['documents'] as FormArray;
  }
  get fileListd() {
    return this.t.controls as FormGroup[];
  }
  constructor(
    private confirmationService: ConfirmationService,
    private generalService: GeneralService,
    private mbookService: MBookService,
    private tenderService: TenderService
  ) {
    this.mBookForm = new FormGroup({
      id: new FormControl(''),
      date: new FormControl('', [Validators.required]),
      actuals: new FormControl('', [Validators.required]),
      notes: new FormControl('', [Validators.required]),
    });
  }

  ngOnChanges() {
    this.rowsPerPageOptions = [5, 10, 25, 50, this.total];
  }
  lazyload(event: any) {
    this.filterModel = {
      ...this.filterModel,
      sorting: {
        fieldName:
          event.sortField && event.sortField !== ''
            ? event.sortField
            : this.defaultSortField,
        sort: event.sortOrder == 1 ? 'ASC' : 'DESC',
      },
    };
    this.changefilter.emit(this.filterModel);
  }
  next() {
    this.first = this.first + this.rows;
  }

  prev() {
    this.first = this.first - this.rows;
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

  maketableFilter(col: any, record: any) {
    this.changecolFilter.emit({ type: col, record });
  }
  showPopup(action: string, record: any) {
    if (action == 'WORK' || action == 'MILESTONE') {
      this.tenderService.getTenderById(record.tenderId).subscribe((x) => {
        this.tender = x.data;
        this.tenderService.GetWorkTemplate(x.data.id).subscribe((y) => {
          this.templateWithMilestone = y.data[0];
          this.tenderVisible = true;
          this.bookVisible = false;
        });
        this.tenderService
          .getCommentList({
            ...this.commentfiletrModel,
            where: { type: 'TENDER', typeId: record.tenderId },
          })
          .subscribe((x) => {
            this.events = x.data;
          });

        this.tenderService
          .getactivityList({
            ...this.commentfiletrModel,
            where: { type: 'TENDER', typeId: record.tenderId },
            ids: null,
            types: null,
          })
          .subscribe((x) => {
            this.activitiesevents = x.data;
          });
      });
    } else {
      this.mbookService.getMBookbyId(record.id).subscribe((x) => {
        if (x) {
          this.mBookDetails = x.data;
          this.tenderService
            .MilestoneFiles(this.mBookDetails?.workTemplateMilestoneId ?? '')
            .subscribe((x) => {
              if (x) {
                this.filesList = x.data;
                this.mBookForm
                  .get('date')
                  ?.patchValue(
                    this.mBookDetails?.date
                      ? moment(this.mBookDetails?.date).toDate()
                      : null
                  );
                this.mBookForm.get('id')?.patchValue(this.mBookDetails?.id);
                this.mBookForm
                  .get('actuals')
                  ?.patchValue(this.mBookDetails?.actualAmount);
                this.mBookForm
                  .get('notes')
                  ?.patchValue(this.mBookDetails?.workNotes);
                this.tenderVisible = false;
                this.bookVisible = true;
              }
            });
        }
      });
    }
  }

  onGlobalFilter(event: any) {
    this.filterModel = {
      ...this.filterModel,
      searchString: this.tenderSearchString,
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
  // dc(date: any, field?: string) {
  //   if(!field){
  //     return null;
  //   }
  //   else if (field === 'startDate' || field === 'endDate') {
  //     return dateconvertionwithOnlyDate(date);
  //   }
  //   else {
  //     return dateconvertion(date);
  //   }
  // }
  dc(date: any, field?: string) {
    if (
      field === 'startDate' ||
      field === 'endDate' ||
      field === 'tenderOpenedDate'
    ) {
      return date ? dateconvertionwithOnlyDate(date) : '';
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

  exportPdf() {
    this.exportColumns = this.cols.map((col) => ({
      title: col.header,
      dataKey: col.field,
    }));
    var totalPagesExp = '{total_pages_count_string}';
    import('jspdf').then((jsPDF) => {
      import('jspdf-autotable').then((x) => {
        const doc = new jsPDF.default('l', 'px', 'a3');
        var fileName = this.fileName;
        (doc as any).autoTable(this.exportColumns, this.records);
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

  // exportExcel() {
  //   import('xlsx').then((xlsx) => {
  //     const worksheet = xlsx.utils.json_to_sheet(this.records);
  //     const workbook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
  //     const excelBuffer: any = xlsx.write(workbook, {
  //       bookType: 'xlsx',
  //       type: 'array',
  //     });
  //     this.saveAsExcelFile(
  //       excelBuffer,
  //       this.fileName + '_Report_' + new Date().getTime()
  //     );
  //   });
  // }

  exportExcel() {
    import('xlsx').then((xlsx) => {
      const headerMap = this.cols.reduce((acc: any, col: any) => {
        acc[col.field] = col.header;
        return acc;
      }, {});

      const formattedData = this.records.map((row: any) => {
        const newRow: any = {};
        this.cols.forEach((col: any) => {
          newRow[col.header] = row[col.field];
        });
        return newRow;
      });

      const worksheet = xlsx.utils.json_to_sheet(formattedData);
      const workbook = {
        Sheets: { [this.fileName || 'Sheet1']: worksheet },
        SheetNames: [this.fileName || 'Sheet1'],
      };

      const excelBuffer: any = xlsx.write(workbook, {
        bookType: 'xlsx',
        type: 'array',
      });

      import('file-saver').then((FileSaverModule: any) => {
        const saveAs = FileSaverModule.default || FileSaverModule.saveAs; // ✅ handles both cases
        const data: Blob = new Blob([excelBuffer], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8',
        });
        saveAs(
          data,
          (this.fileName || 'Export') +
            '_Report_' +
            new Date().getTime() +
            '.xlsx'
        );
      });
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
  resetform() {}

  convertoWordsIND(amt: number) {
    return convertoWords(amt);
  }
  dcwt(val: any) {
    return dateconvertion(val);
  }
  download(id: string, filename: string) {
    this.generalService.downloads(id, filename);
  }
  downloadMilestoneFiles(fileid: string, name: string) {
    this.generalService.downloads(fileid, name ?? 'File.png');
  }
  downloadFile(feild: string, record: any) {
    this.generalService.downloads(
      feild == 'milestoneFile1Original'
        ? (record.milestoneFile1Saved as string).split('|')[0]
        : feild == 'milestoneFile2Original'
        ? (record.milestoneFile2Saved as string).split('|')[0]
        : '',
      feild == 'milestoneFile1Original'
        ? record.milestoneFile1Original
        : feild == 'milestoneFile2Original'
        ? record.milestoneFile2Original
        : 'File.png'
    );
  }
  geturl(feild: string, record: any) {
    var sd =
      feild == 'milestoneFile1Original'
        ? (record.milestoneFile1Saved as string).split('|')[1]
        : feild == 'milestoneFile2Original'
        ? (record.milestoneFile2Saved as string).split('|')[1]
        : '';
    return `${environment.apiUrl.replace('/api/', '')}/images/${sd}`;
  }
  isImage(data: string) {
    if (data && data != '') {
      var finalstr = data.substring(data.lastIndexOf('.'), data.length);
      return this.imgExtension.includes(finalstr.toLowerCase());
    }
    return false;
  }
  showimg(feild: string, record: any) {
    var sd =
      feild == 'milestoneFile1Original'
        ? (record.milestoneFile1Saved as string)
        : feild == 'milestoneFile2Original'
        ? (record.milestoneFile2Saved as string)
        : '';
    this.imgVisible = true;
    var url = `${environment.apiUrl.replace('/api/', '')}/images/${
      sd.split('|')[1]
    }`;
    this.imgDetails = {
      name: record[feild],
      url: url,
      fileId: sd.split('|')[0],
    };
  }
  resetimg() {}
  getAbs(value: number): number {
    return Math.abs(value);
  }
}
