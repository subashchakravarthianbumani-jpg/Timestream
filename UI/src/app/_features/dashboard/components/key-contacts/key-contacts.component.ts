import { Component } from '@angular/core';
import { SuccessStatus } from 'src/app/_models/ResponseStatus';
import { Actions, Column } from 'src/app/_models/datatableModel';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-key-contacts',
  templateUrl: './key-contacts.component.html',
  styleUrls: ['./key-contacts.component.scss'],
})
export class KeyContactsComponent {
  contacts!: any[];
  cols!: Column[];
  searchableColumns!: string[];
  actions: Actions[] = [];
  isDependent: boolean = false;
  title: string = 'Configuration';
  first: number = 0;
  rows: number = 25;
  total: number = 0;
  hasCode: boolean = false;
  defaultSortField: string = 'value';
  defaultSortOrder: number = 1;

  constructor(private userService: UserService) {}
  ngOnInit() {
    this.userService.getKeyContacts().subscribe(
      (data) => {
        if (data && data.status == SuccessStatus) {
          this.contacts = data.data;
          this.total = data.totalRecordCount;
        }
      },
      (error) => {}
    );

    this.cols = [
      {
        field: 'department',
        header: 'Department',
        customExportHeader: 'Department',
        sortablefield: 'department',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'firstName',
        header: 'First Name',
        sortablefield: 'firstName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'lastName',
        header: 'Last Name',
        sortablefield: 'lastName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'roleName',
        header: 'Role',
        sortablefield: 'roleName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'userGroupName',
        header: 'Group',
        sortablefield: 'userGroupName',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'email',
        header: 'Email',
        sortablefield: 'email',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'mobile',
        header: 'Mobile',
        sortablefield: 'mobile',
        isSortable: true,
        isSearchable: true,
      },
    ];

    this.searchableColumns = this.cols
      .filter((x) => x.isSearchable == true)
      .flatMap((x) => x.field);
  }
}
