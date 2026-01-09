import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { MenuItem, SortEvent } from 'primeng/api';
import { Subscription } from 'rxjs';
import { SuccessStatus } from 'src/app/_models/ResponseStatus';
import { DashboardRecordCountCardModel } from 'src/app/_models/dashboard.model';
import {
  ColumnSearchModel,
  MBookFilterModel,
  TableFilterModel,
  TenderFilterModel,
} from 'src/app/_models/filterRequest';
import { DashboardService } from 'src/app/_services/dashboard.service';
import { MBookService } from 'src/app/_services/mbook.service';
import { TenderService } from 'src/app/_services/tender.service';
import { ProductService } from 'src/app/demo/service/product.service';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { privileges } from 'src/app/shared/commonFunctions';
import { AppLayoutComponent } from 'src/app/layout/app.layout.component';
import { Templateservice } from 'src/app/_services/templates.service';
import { CameraFilter } from 'src/app/_models/dashboard.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent {
  filters: CameraFilter = {};
  @Input() showFilter = false;
  // @Output() closeFilterEvent = new EventEmitter<void>();

  butnOptions: any[] = [
    { name: 'Current yr', value: 'CURRENT' },
    { name: 'Past yr', value: 'PAST_YR' },
    { name: 'Past 5 yrs', value: 'PAST_5_YR' },
  ];
  selectedOptions: any[] = ['PAST_5_YR'];
  yearList: string[] = [new Date().getFullYear().toString()];

  // category = '';
  // subcategory = '';
  // workStatus = '';
  // tenderNumber = '';
  // divisionName = '';
  // districtName = '';

  userPermissions!: string[];
  // divisionList: any[] = [];
  // divisionid: string | null = null;
  // districtList: any[] = [];
  // districtid: string | null = null;
  // WorkCategoryList: any[] = [];
  // subCategoryList: any[] = [];
  // workStatusList: any[] = [];
  // TenderNumberList: any[] = [];

  privleges = privileges;
  recordCount!: DashboardRecordCountCardModel;
  constructor(
    public layoutService: LayoutService,
    private cookieService: CookieService,
    private dashboardService: DashboardService,
    private layout: AppLayoutComponent,
    private service: Templateservice
  ) {}

  changeyear() {
    localStorage.setItem(
      'selectedYearOptions',
      JSON.stringify(this.selectedOptions)
    );

    this.yearList = [];

    if (this.selectedOptions.includes('PAST_5_YR')) {
      var startyear = new Date().getFullYear() - 5;
      var currentyear = new Date().getFullYear();
      for (var i = startyear + 1; i <= currentyear; i++) {
        this.yearList = [...this.yearList, i.toString()];
      }
    } else if (this.selectedOptions.includes('PAST_YR')) {
      this.yearList = [
        ...this.yearList,
        (new Date().getFullYear() - 1).toString(),
      ];
    }
    if (this.selectedOptions.includes('CURRENT')) {
      this.yearList = [...this.yearList, new Date().getFullYear().toString()];
    }
    if (this.selectedOptions && this.selectedOptions.length == 0) {
      this.yearList = [new Date().getFullYear().toString()];
    }
    this.yearList = this.yearList.filter(
      (item, i, arr) => arr.findIndex((t) => t === item) === i
    );
    this.getcountdata();
  }
  ngOnInit() {
    // this.loadDivision();
    //  this.initializeTableFilters();

    //  THIS FIXES YOUR POPUP
    this.layout.filterState$.subscribe((value) => {
      this.showFilter = value;
    });

    this.getcountdata();
    const privillage: any = this.cookieService.get('privillage');

    if (privillage) {
      this.userPermissions = privillage.split(',');
    }
    const storedOptions = localStorage.getItem('selectedYearOptions');
    if (storedOptions) {
      try {
        console.log('storedOptions:', storedOptions);
        this.selectedOptions = JSON.parse(storedOptions);
      } catch {
        this.selectedOptions = ['CURRENT'];
      }
    }
    this.changeyear();
  }
  getcountdata() {
    this.dashboardService
      .getDashboardRecordCount({
        departmentIds: null,
        divisionIds: null,
        year: this.yearList,
        costOrCount: 'COUNT',
        selectionType: null,
      })
      .subscribe(
        (x) => {
          this.recordCount = x.data;
        },
        (error) => {}
      );
  }
  getPerm(privleges: string | string[]) {
    if (typeof privleges == 'string') {
      return this.userPermissions.includes(privleges);
    } else if (typeof privleges == 'object') {
      return this.userPermissions.find((x) => privleges.includes(x));
    } else {
      return true;
    }
  }

  closeFilter() {
    this.layout.closeFilterPanel();
  }

  // loadDivision() {
  //   this.service.GetAllDivision().subscribe({
  //     next: (res) => {
  //       if (res?.status === 'SUCCESS') {
  //         this.divisionList = res.data || [];
  //       } else {
  //         this.divisionList = [];
  //       }
  //     },
  //     error: (err) => console.error('Error loading division list', err),
  //   });
  // }

  // onDivisionChange(event: any) {
  //     this.divisionid = event.value || '';
  //     const selectedDivision = this.divisionList.find(
  //       (x) => x.division === this.divisionid
  //     );
  //     this.divisionName = selectedDivision?.divisionName || '';

  //     // Only reset relevant lower-level filters
  //     if (!this.divisionid) {
  //       this.districtid = '';
  //       this.category = '';
  //       this.subcategory = '';
  //       this.workStatus = '';
  //       this.tenderNumber = '';
  //     }

  //     this.updateCameraFilters();

  //     // this.reloadFilters();
  //     this.loadDistrict(this.divisionid);
  //     this.loadTenderNumber(
  //       this.divisionid,
  //       this.districtid,
  //       this.category,
  //       this.subcategory,
  //       this.workStatus
  //     );
  //   }
  //   loadDistrict(divisionId: string | null | undefined): Promise<void> {
  //     return new Promise((resolve, reject) => {
  //       this.service.GetAllDistrict(divisionId || '').subscribe({
  //         next: (res) => {
  //           this.districtList = res?.status === 'SUCCESS' ? res.data || [] : [];
  //           resolve();
  //         },
  //         error: (err) => {
  //           console.error('Error loading district list', err);
  //           reject(err);
  //         },
  //       });
  //     });
  //   }

  //   onDistrictChange(event: any) {
  //     this.districtid = event.value || '';
  //     const selectedDistrict = this.districtList.find(
  //       (x) => x.district === this.districtid
  //     );
  //     this.districtName = selectedDistrict?.districtName || '';

  //     if (!this.districtid) {
  //       this.category = '';
  //       this.subcategory = '';
  //       this.workStatus = '';
  //       this.tenderNumber = '';
  //     }

  //     this.updateCameraFilters();

  //     // this.reloadFilters();
  //     this.loadWorkCategory(this.divisionid, this.districtid);
  //     this.loadTenderNumber(
  //       this.divisionid,
  //       this.districtid,
  //       this.category,
  //       this.subcategory,
  //       this.workStatus
  //     );
  //   }

  //   loadWorkCategory(
  //     divisionId: string | null | undefined,
  //     districtId: string | null | undefined
  //   ) {
  //     this.service.GetAllWorkType(divisionId || '', districtId || '').subscribe({
  //     next: (res) => {
  //       if (res?.status === 'SUCCESS') {
  //         this.WorkCategoryList = res.data || [];
  //       } else {
  //         this.WorkCategoryList = [];
  //       }
  //     },
  //     error: (err) => console.error('Error loading division list', err),
  //   });
  // }

  // onWorkcategoryChange(event: any) {
  //     this.category = event.value || '';

  //     if (!this.category) {
  //       this.subcategory = '';
  //       this.workStatus = '';
  //       this.tenderNumber = '';
  //     }

  //   this.updateCameraFilters();

  //     // this.reloadFilters();
  //     this.loadSubWorkType(this.divisionid, this.districtid, this.category);
  //     this.loadTenderNumber(
  //       this.divisionid,
  //       this.districtid,
  //       this.category,
  //       this.subcategory,
  //       this.workStatus
  //     );
  //   }

  // loadSubWorkType(
  //     divisionId: string | null | undefined,
  //     districtId: string | null | undefined,
  //     mainCategory: string | null | undefined
  //   ) {
  //   this.service.GetAllSubWorkType(divisionId || '', districtId || '', mainCategory || '').subscribe({
  //     next: (res) => {
  //       if (res?.status === 'SUCCESS') {
  //         this.subCategoryList = res.data || [];
  //       } else {
  //         this.subCategoryList = [];
  //       }
  //     },
  //     error: (err) => console.error('Error loading division list', err),
  //   });
  // }

  // onSubCategoryChange(event: any) {
  //     this.subcategory = event.value || '';
  //     if (!this.subcategory) {
  //       this.workStatus = '';
  //       this.tenderNumber = '';
  //     }

  //     this.updateCameraFilters();

  //     // this.reloadFilters();
  //     this.loadWorkStatus(
  //       this.divisionid,
  //       this.districtid,
  //       this.category,
  //       this.subcategory
  //     );
  //     this.loadTenderNumber(
  //       this.divisionid,
  //       this.districtid,
  //       this.category,
  //       this.subcategory,
  //       this.workStatus
  //     );
  //   }

  // loadWorkStatus(  divisionId: string | null | undefined,
  //     districtId: string | null | undefined,
  //     mainCategory: string | null | undefined,
  //     subCategory: string | null | undefined) {
  //   this.service.GetAllWorkStatus( divisionId || '',
  //         districtId || '',
  //         mainCategory || '',
  //         subCategory || '').subscribe({
  //     next: (res) => {
  //       if (res?.status === 'SUCCESS') {
  //         this.workStatusList= res.data || [];
  //       } else {
  //         this.workStatusList = [];
  //       }
  //     },
  //     error: (err) => console.error('Error loading division list', err),
  //   });
  // }

  //   onWorkStatusChange(event: any) {
  //     this.workStatus = event.value || '';
  //     this.updateCameraFilters();

  //     this.loadTenderNumber(
  //       this.divisionid,
  //       this.districtid,
  //       this.category,
  //       this.subcategory,
  //       this.workStatus
  //     );

  //   }

  // loadTenderNumber(  divisionId: string | null | undefined,
  //     districtId: string | null | undefined,
  //     mainCategory: string | null | undefined,
  //     subCategory: string | null | undefined,
  //     workStatus: string | null | undefined) {
  //   this.service.GetAllTenderNumber( divisionId || '',
  //         districtId || '',
  //         mainCategory || '',
  //         subCategory || '',
  //         workStatus || '').subscribe({
  //     next: (res) => {
  //       if (res?.status === 'SUCCESS') {
  //         this.TenderNumberList= res.data || [];
  //       } else {
  //         this.TenderNumberList = [];
  //       }
  //     },
  //     error: (err) => console.error('Error loading division list', err),
  //   });
  // }

  // onTenderChange(event: any) {
  //  this.tenderNumber = event?.value || '';

  //  this.updateCameraFilters();

  //     if (!this.tenderNumber) {
  //       this.loadTenderNumber('', '', '', '', '');
  //       return;
  //     }
  // }

  //   // ------------------ TABLE FILTER LOAD FUNCTIONS ------------------ //

  //   initializeTableFilters() {

  //     this.loadDistrict('');
  //     this.loadWorkCategory('', '');
  //     this.loadSubWorkType('', '', '');
  //     this.loadWorkStatus('', '', '', '');
  //     this.loadTenderNumber('', '', '', '', '');
  //   }

  //   updateCameraFilters() {
  //   this.filters = {
  //     divisionId: this.divisionid || '',
  //     districtId: this.districtid || '',
  //     mainCategory: this.category || '',
  //     subCategory: this.subcategory || '',
  //     workStatus: this.workStatus || '',
  //     tenderNumber: this.tenderNumber || ''
  //   };
  // }

  // onSearchChange() {
  //   this.updateCameraFilters();
  // }

  updateFilters(filters: CameraFilter) {
    console.log(' Dashboard received:', filters);

    // clone object to trigger change detection
    this.filters = { ...filters };
  }
}
