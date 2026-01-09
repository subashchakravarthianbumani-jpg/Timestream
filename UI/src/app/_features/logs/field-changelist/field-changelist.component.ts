import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Column } from 'src/app/_models/datatableModel';
import { RecordHistoryModel } from 'src/app/_models/log';
import { UserConfigFacade } from '../../user/state/user.facades';
import { TableFilterModel } from 'src/app/_models/filterRequest';
import { LogHistoryService } from 'src/app/_services/logs.service';
import {
  dateconvertion,
  dateconvertionwithOnlyDate,
  privileges,
} from 'src/app/shared/commonFunctions';

@Component({
  selector: 'app-field-changelist',
  templateUrl: './field-changelist.component.html',
  styleUrls: ['./field-changelist.component.scss'],
})
export class FieldChangelistComponent {
  configurationList!: RecordHistoryModel[];
  cols!: Column[];
  catgories!: any[];
  searchableColumns!: string[];
  currentStatus: boolean = true;

  title: string = 'Change History';
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
        field: 'action',
        header: 'Action',
        sortablefield: 'action',
        isSearchable: true,
        isSortable: true,
      },
      {
        field: 'tableName',
        header: 'Table Name',
        sortablefield: 'tableName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'tableUniqueId',
        header: 'Unique Id',
        sortablefield: 'tableUniqueId',
      },
      {
        field: 'columnName',
        header: 'Column Name',
        sortablefield: 'columnName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'oldValue',
        header: 'Old Value',
        sortablefield: 'oldValue',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'newValue',
        header: 'New Value',
        sortablefield: 'newValue',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'createdByUserName',
        header: 'Saved By',
        sortablefield: 'createdByUserName',
        isSortable: true,
      },
      {
        field: 'lastUpdatedDatestring',
        header: 'Saved Date',
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
    this.logService.getlogs(filter).subscribe(
      (x) => {
        this.configurationList = []; // x.data;
        x.data?.forEach((x: RecordHistoryModel) => {
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
