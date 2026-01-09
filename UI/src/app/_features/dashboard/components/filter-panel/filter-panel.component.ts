import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { Component ,Input, Output, EventEmitter } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { MenuItem, SortEvent } from 'primeng/api';
import { Subscription } from 'rxjs';
import { SuccessStatus } from 'src/app/_models/ResponseStatus';
import { DashboardRecordCountCardModel } from 'src/app/_models/dashboard.model';
import { DashboardService } from 'src/app/_services/dashboard.service';
import { MBookService } from 'src/app/_services/mbook.service';
import { TenderService } from 'src/app/_services/tender.service';
import { ProductService } from 'src/app/demo/service/product.service';
import { privileges } from 'src/app/shared/commonFunctions';
import { AppLayoutComponent } from 'src/app/layout/app.layout.component';
import { Templateservice } from 'src/app/_services/templates.service';
import { CameraFilter } from 'src/app/_models/dashboard.model';
import { FormsModule } from '@angular/forms';
import { ViewChildren, QueryList } from '@angular/core';
import { Dropdown } from 'primeng/dropdown';



@Component({
  selector: 'app-filter-panel',
  templateUrl: './filter-panel.component.html',
  styleUrls: ['./filter-panel.component.scss']
})
export class FilterPanelComponent {

@Input() filters!: CameraFilter;
@Input() showFilter = false;    
@Output() filtersChanged = new EventEmitter<CameraFilter>();
@ViewChildren(Dropdown) dropdowns!: QueryList<Dropdown>;



   constructor(
      public layoutService: LayoutService,
      private cookieService: CookieService,
      private dashboardService: DashboardService,
      private layout: AppLayoutComponent,
       private service: Templateservice,
  
    ) {}

  // filters: CameraFilter = {};
    category = '';
    subcategory = '';
    workStatus = '';
    tenderNumber = '';
    divisionName = '';  
    districtName = '';
  
    userPermissions!: string[];
    divisionList: any[] = [];
    divisionid: string | null = null;
    districtList: any[] = [];
    districtid: string | null = null;
    WorkCategoryList: any[] = [];
    subCategoryList: any[] = [];
    workStatusList: any[] = [];
    TenderNumberList: any[] = [];
  



  ngOnInit() {

const saved = this.loadFiltersFromLocalStorage();
if (saved) {
  this.filters = saved;

  this.divisionid   = saved.divisionId ?? null;
  this.districtid   = saved.districtId ?? null;
  this.category     = saved.mainCategory ?? '';
  this.subcategory  = saved.subCategory ?? '';
  this.workStatus   = saved.workStatus ?? '';
  this.tenderNumber = saved.tenderNumber ?? '';

  console.log("📌 Loaded saved filters:", saved);

      // ⭐ VERY IMPORTANT: Emit restored filters
    setTimeout(() => {
      this.updateCameraFilters();  // this triggers filtersChanged.emit()
    }, 300);
}

    this.loadDivision();
     this.initializeTableFilters();


      // ✅ THIS FIXES YOUR POPUP
  this.layout.filterState$.subscribe(value => {
    this.showFilter = value;
  });

  }

  ngOnChanges() {
  if (!this.filters) return;

  // ✅ push incoming values into form controls
  this.divisionid   = this.filters.divisionId || null;
  this.districtid   = this.filters.districtId || null;
  this.category     = this.filters.mainCategory || '';
  this.subcategory  = this.filters.subCategory || '';
  this.workStatus   = this.filters.workStatus || '';
  this.tenderNumber = this.filters.tenderNumber || '';

  console.log('✅ Filter panel pre-filled from status card:', this.filters);
}


loadDivision() {
  this.service.GetAllDivision().subscribe({
    next: (res) => {
      if (res?.status === 'SUCCESS') {
        this.divisionList = res.data || [];
      } else {
        this.divisionList = [];
      }
    },
    error: (err) => console.error('Error loading division list', err),
  });
}

onDivisionChange(event: any) {
    this.divisionid = event.value || '';
    const selectedDivision = this.divisionList.find(
      (x) => x.division === this.divisionid
    );
    this.divisionName = selectedDivision?.divisionName || '';


    // Only reset relevant lower-level filters
    if (!this.divisionid) {
      this.districtid = '';
      this.category = '';
      this.subcategory = '';
      this.workStatus = '';
      this.tenderNumber = '';
    }

    this.updateCameraFilters();


    // this.reloadFilters();
    this.loadDistrict(this.divisionid);
    this.loadTenderNumber(
      this.divisionid,
      this.districtid,
      this.category,
      this.subcategory,
      this.workStatus
    );
  }
  loadDistrict(divisionId: string | null | undefined): Promise<void> {
    return new Promise((resolve, reject) => {
      this.service.GetAllDistrict(divisionId || '').subscribe({
        next: (res) => {
          this.districtList = res?.status === 'SUCCESS' ? res.data || [] : [];
          resolve();
        },
        error: (err) => {
          console.error('Error loading district list', err);
          reject(err);
        },
      });
    });
  }

  onDistrictChange(event: any) {
    this.districtid = event.value || '';
    const selectedDistrict = this.districtList.find(
      (x) => x.district === this.districtid
    );
    this.districtName = selectedDistrict?.districtName || '';
   

    

    if (!this.districtid) {
      this.category = '';
      this.subcategory = '';
      this.workStatus = '';
      this.tenderNumber = '';
    }


    this.updateCameraFilters();

    // this.reloadFilters();
    this.loadWorkCategory(this.divisionid, this.districtid);
    this.loadTenderNumber(
      this.divisionid,
      this.districtid,
      this.category,
      this.subcategory,
      this.workStatus
    );
  }


  loadWorkCategory(
    divisionId: string | null | undefined,
    districtId: string | null | undefined
  ) {
    this.service.GetAllWorkType(divisionId || '', districtId || '').subscribe({
    next: (res) => {
      if (res?.status === 'SUCCESS') {
        this.WorkCategoryList = res.data || [];
      } else {
        this.WorkCategoryList = [];
      }
    },
    error: (err) => console.error('Error loading division list', err),
  });
}

onWorkcategoryChange(event: any) {
    this.category = event.value || '';

    if (!this.category) {
      this.subcategory = '';
      this.workStatus = '';
      this.tenderNumber = '';
    }

  this.updateCameraFilters(); 

    // this.reloadFilters();
    this.loadSubWorkType(this.divisionid, this.districtid, this.category);
    this.loadTenderNumber(
      this.divisionid,
      this.districtid,
      this.category,
      this.subcategory,
      this.workStatus
    );
  }



loadSubWorkType(
    divisionId: string | null | undefined,
    districtId: string | null | undefined,
    mainCategory: string | null | undefined
  ) {
  this.service.GetAllSubWorkType(divisionId || '', districtId || '', mainCategory || '').subscribe({
    next: (res) => {
      if (res?.status === 'SUCCESS') {
        this.subCategoryList = res.data || [];
      } else {
        this.subCategoryList = [];
      }
    },
    error: (err) => console.error('Error loading division list', err),
  });
}

onSubCategoryChange(event: any) {
    this.subcategory = event.value || '';
    if (!this.subcategory) {
      this.workStatus = '';
      this.tenderNumber = '';
    }

    this.updateCameraFilters();

    // this.reloadFilters();
    this.loadWorkStatus(
      this.divisionid,
      this.districtid,
      this.category,
      this.subcategory
    );
    this.loadTenderNumber(
      this.divisionid,
      this.districtid,
      this.category,
      this.subcategory,
      this.workStatus
    );
  }

loadWorkStatus(  divisionId: string | null | undefined,
    districtId: string | null | undefined,
    mainCategory: string | null | undefined,
    subCategory: string | null | undefined) {
  this.service.GetAllWorkStatus( divisionId || '',
        districtId || '',
        mainCategory || '',
        subCategory || '').subscribe({
    next: (res) => {
      if (res?.status === 'SUCCESS') {
        this.workStatusList= res.data || [];
      } else {
        this.workStatusList = [];
      }
    },
    error: (err) => console.error('Error loading division list', err),
  });
}

  onWorkStatusChange(event: any) {
    this.workStatus = event.value || '';
    this.updateCameraFilters();

    this.loadTenderNumber(
      this.divisionid,
      this.districtid,
      this.category,
      this.subcategory,
      this.workStatus
    );
 

  }
  
loadTenderNumber(  divisionId: string | null | undefined,
    districtId: string | null | undefined,
    mainCategory: string | null | undefined,
    subCategory: string | null | undefined,
    workStatus: string | null | undefined) {
  this.service.GetAllTenderNumber( divisionId || '',
        districtId || '',
        mainCategory || '',
        subCategory || '',
        workStatus || '').subscribe({
    next: (res) => {
      if (res?.status === 'SUCCESS') {
        this.TenderNumberList= res.data || [];
      } else {
        this.TenderNumberList = [];
      }
    },
    error: (err) => console.error('Error loading division list', err),
  });
}

onTenderChange(event: any) {
 this.tenderNumber = event?.value || '';
 
 this.updateCameraFilters();

    if (!this.tenderNumber) {
      this.loadTenderNumber('', '', '', '', '');
      return;
    }
}

  // ------------------ TABLE FILTER LOAD FUNCTIONS ------------------ //

  initializeTableFilters() {
  

    this.loadDistrict('');
    this.loadWorkCategory('', '');
    this.loadSubWorkType('', '', '');
    this.loadWorkStatus('', '', '', '');
    this.loadTenderNumber('', '', '', '', '');
  }
updateCameraFilters() {
  const filters: CameraFilter = {
    divisionId: this.divisionid || '',
    districtId: this.districtid || '',
    mainCategory: this.category || '',
    subCategory: this.subcategory || '',
    workStatus: this.workStatus || '',
    tenderNumber: this.tenderNumber || ''
  };

  this.filtersChanged.emit(filters);

    this.saveFiltersToLocalStorage(filters);
}

onSearchChange() {
  this.updateCameraFilters();
}

 closeFilter() {
  this.layout.closeFilterPanel();   
}

// ------------------ LOCAL STORAGE HELPERS ------------------ //
saveFiltersToLocalStorage(filters: CameraFilter) {
  localStorage.setItem("cameraFilters", JSON.stringify(filters));
}

loadFiltersFromLocalStorage(): CameraFilter | null {
  const saved = localStorage.getItem("cameraFilters");
  return saved ? JSON.parse(saved) : null;
}


closeOtherDropdowns(active: Dropdown) {
  this.dropdowns.forEach(dd => {
    if (dd !== active) {
      dd.hide();
    }
  });
}



}
