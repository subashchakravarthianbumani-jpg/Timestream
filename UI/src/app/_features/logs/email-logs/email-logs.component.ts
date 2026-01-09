import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Column } from 'src/app/_models/datatableModel';
import { TableFilterModel } from 'src/app/_models/filterRequest';
import { MailSMSLog, RecordHistoryModel } from 'src/app/_models/log';
import { LogHistoryService } from 'src/app/_services/logs.service';
import {
  dateconvertion,
  dateconvertionwithOnlyDate,
  privileges,
} from 'src/app/shared/commonFunctions';

@Component({
  selector: 'app-email-logs',
  templateUrl: './email-logs.component.html',
  styleUrls: ['./email-logs.component.scss'],
})
export class EmailLogsComponent {
  configurationList!: MailSMSLog[];
  cols!: Column[];
  catgories!: any[];
  searchableColumns!: string[];
  currentStatus: boolean = true;

  title: string = 'Email / SMS History';
  first: number = 0;
  rows: number = 10;
  total: number = 0;
  defaultSortField: string = 'createdDate';
  defaultSortOrder: number = -1;
  tableFilterModel!: TableFilterModel;
  privleges = privileges;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private logService: LogHistoryService
  ) {}

  ngOnInit() {
    this.tableFilterModel = {
      searchString: null,
      skip: 0,
      sorting: { fieldName: 'createdDate', sort: 'DESC' },
      take: 10,
      columnSearch: null,
    };

    this.cols = [
      {
        field: 'recordType',
        header: 'Record Type',
        sortablefield: 'recordType',
        isSearchable: true,
        isSortable: true,
      },
      {
        field: 'sentAddress',
        header: 'To Email',
        sortablefield: 'sentAddress',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'subject',
        header: 'Subject',
        sortablefield: 'subject',
      },
      {
        field: 'body',
        header: 'Body',
        sortablefield: 'body',
      },
      {
        field: 'type',
        header: 'Module',
        sortablefield: 'type',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'createdByUserName',
        header: 'Sent By',
        sortablefield: 'createdByUserName',
        isSortable: true,
      },
      {
        field: 'lastUpdatedDatestring',
        header: 'Sent Date',
        sortablefield: 'createdDate',
        isSortable: true,
      },
    ];
    this.searchableColumns = this.cols
      .filter((x) => x.isSearchable == true)
      .flatMap((x) => x.field);
  }

  changefilter(val: any) {
    this.tableFilterModel = { ...this.tableFilterModel, ...val };
    this.getlogs(this.tableFilterModel);
  }
  getlogs(filter: TableFilterModel) {
    this.logService.getEmailSmsLog(filter).subscribe(
      (x) => {
        this.configurationList = []; // x.data;
        x.data?.forEach((x: MailSMSLog) => {
          this.configurationList.push({
            ...x,
            lastUpdatedDatestring: this.dcwt(x.createdDate),
          });
        });
        this.total = x.totalRecordCount;
      },
      (errr) => {}
    );
  }
  dc(val: any) {
    return dateconvertionwithOnlyDate(val);
  }
  dcwt(val: any) {
    return dateconvertion(val);
  }
}
