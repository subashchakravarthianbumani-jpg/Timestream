import {
  Component,
  OnInit,
  AfterViewInit,
  QueryList,
  ViewChildren,
  ElementRef,
} from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';

import { CameraService } from 'src/app/_services/camera.service';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { Input, OnChanges, SimpleChanges } from '@angular/core';
import {
  CameraData,
  CameraFilter,
  DashboardCameraModel,
} from 'src/app/_models/dashboard.model';
import { GeneralService } from 'src/app/_services/general.service';
import { Actions, Column } from 'src/app/_models/datatableModel';
import { TableFilterModel } from 'src/app/_models/filterRequest';
import { InputSwitchModule } from 'primeng/inputswitch';

declare var WebRtcStreamer: any;

type DisplayCamera = {
  divisionName: string;
  districtName: string;
  workStatus: string;
  liveUrl: SafeResourceUrl | null;
  rtspUrl: string;
    rtmpUrl?: string;
  tenderNumber: string;
  error: string;
  lastUpdated: string;
};

@Component({
  selector: 'app-camera-grid',
  templateUrl: './camera-grid.component.html',
  styleUrls: ['./camera-grid.component.scss'],
})
export class CameraGridComponent implements OnInit, OnChanges {
  @Input() filters!: CameraFilter;

  searchableColumns!: string[];

  cameras: DisplayCamera[] = [];
  visibleCameras: DisplayCamera[] = [];
  headerTitle: string = '';
  pageSize = 9;
  currentIndex = 0;
  domReset = false;

  chartType: boolean = true;
  // false = Report, true = Camera

  Loadcamera: boolean = true;

  cols: Column[] = [];
  configurationList: CameraData[] = [];
  actions: Actions[] = [];
  title: string = 'M-Book';
  first: number = 0;
  rows: number = 25;
  total: number = 0;
  defaultSortField: string = 'tenderNumber';
  defaultSortOrder: number = 1;
  filtermodel!: DashboardCameraModel;
  tableFilterModel!: TableFilterModel;
  showToggle: boolean = true;

  constructor(
    private cameraService: CameraService,
    private sanitizer: DomSanitizer,
    private router: Router,
    private route: ActivatedRoute,
    private state: GeneralService
  ) {}

  workStatus: any;
  selectedStatuses: string = '';
  alertCameras: DisplayCamera[] = [];
  selectedAlerts: string = '';

  ngOnInit() {
    const savedFilters = this.loadFiltersFromLocalStorage();

    if (savedFilters) {
      console.log('CameraGrid loaded saved filters:', savedFilters);
      this.filters = savedFilters;
    }

    setInterval(() => {
      window.location.reload();
    }, 20 * 60 * 1000);
    this.chartType = false;

    this.route.queryParams.subscribe((params: any) => {
      this.selectedStatuses = params['Status'] || '';
      this.selectedAlerts = params['Alerts'] || '';

      this.chartType = params['chartType'] === 'true';

      //  Build header text
      if (this.selectedAlerts.toLowerCase() === 'alerts') {
        this.headerTitle = 'Alerts';
        this.showToggle = false;
      } else if (this.selectedStatuses) {
        this.showToggle = false;
        this.headerTitle = this.selectedStatuses + ' Works';
      }

      this.loadData();
    });

    this.tableFilterModel = {
      skip: 0,
      take: this.rows,

      sorting: {
        fieldName: this.defaultSortField,
        sort: this.defaultSortOrder === 1 ? 'ASC' : 'DESC',
      },

      searchString: null,
      columnSearch: null,
    };

    // ✅ Build searchable columns list
    this.searchableColumns = this.cols
      .filter((x) => x.isSearchable === true)
      .flatMap((x) => x.field);

    this.cols = [
      {
        field: 'rtspUrl',
        header: 'Live View',
        isIconButton: true,
        buttonIcon: 'pi pi-eye',
        actionType: 'VIEW_CAMERA',
      },
      {
        field: 'divisionName',
        header: 'Division',
        sortablefield: 'divisionName',
        isSearchable: true,
        isAnchortagforFilter: true,
      },
      {
        field: 'districtName',
        header: 'District',
        sortablefield: 'districtName',
        isSortable: true,
        isSearchable: true,
        isAnchortagforFilter: true,
      },
      {
        field: 'mainCategory',
        header: 'Work Type',
        sortablefield: 'mainCategory',
        isSearchable: true,
        isAnchortagforFilter: true,
      },
      {
        field: 'subcategory',
        header: 'Sub Work Type',
        sortablefield: 'subcategory',
        isSearchable: true,
        isAnchortagforFilter: true,
      },
      {
        field: 'tenderNumber',
        header: 'Work Id',
        sortablefield: 'tenderNumber',
        isSearchable: true,
        isPopup: true,
        popupType: 'WORK',
      },
      {
        field: 'schemeName',
        header: 'Scheme Name',
        sortablefield: 'schemeName',
        isSearchable: true,
        //isAnchortagforFilter: true,
      },
      {
        field: 'go_Package_No',
        header: 'Package Number',
        sortablefield: 'go_Package_No',
        isSearchable: true,
      },

      {
        field: 'contractorCompanyName',
        header: 'Contactor Company Name',
        sortablefield: 'contractorCompanyName',
        isSearchable: true,
        isAnchortagforFilter: true,
      },
      {
        field: 'workCommencementDate',
        header: 'Commentcement Date',
        sortablefield: 'workCommencementDate',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'workCompletionDate',
        header: ' Completion Date',
        sortablefield: 'workCompletionDate',
        isSortable: true,
        isSearchable: true,
      },
      {
        field: 'workStatus',
        header: 'Status',
        sortablefield: 'workStatus',
        isSortable: true,
        isSearchable: true,
      },
      // {
      //   field: 'dateDifference',
      //   header: 'Date Difference',
      //   sortablefield: 'dateDifference',
      //   isSortable: true,
      //   isSearchable: true,
      // },
    ];
  }

  ngOnChanges() {
    if (this.Loadcamera) {
      this.Loadcamera = false;
      return;
    }
    if (!this.filters) return;

    this.tableFilterModel = {
      ...this.tableFilterModel,
      skip: 0,
      take: this.rows,
    };

    this.currentIndex = 0; // reset camera paging

    this.loadData();
  }

  loadData() {
    const tf = this.tableFilterModel;

    this.cameraService
      .getAllCameras({
        divisionIds: this.filters?.divisionId
          ? [this.filters.divisionId]
          : null,

        districtIds: this.filters?.districtId
          ? [this.filters.districtId]
          : null,

        tenderId: this.filters?.tenderNumber || '',
        divisionName: '',
        districtName: '',

        mainCategory: this.filters?.mainCategory || '',
        subcategory: this.filters?.subCategory || '',

        workStatus: this.filters?.workStatus || this.selectedStatuses || '',

        tenderNumber: this.filters?.tenderNumber || '',

        channel: '',
        rtspUrl: '',
        liveUrl: '',
        type: this.selectedAlerts
          ? 'Alert'
          : this.chartType
          ? 'Camera'
          : 'Report',

        skip: this.currentIndex,
        take: this.pageSize,

        SearchString: tf?.searchString ?? '',

        sorting: {
          fieldName: tf?.sorting?.fieldName || this.defaultSortField,
          sort:
            tf?.sorting?.sort || (this.defaultSortOrder === 1 ? 'ASC' : 'DESC'),
        },
      })
      .subscribe((res) => {
        this.total = res?.totalRecordCount || 0;

        console.log('Cameras loaded:', this.total);
        // const data = res || [];
        const data: CameraData[] = res?.data || [];

        this.configurationList = data;

        const mapped = data.map((x: any) => {
          const url =
            x.liveUrl && x.liveUrl.endsWith('/') ? x.liveUrl : x.liveUrl + '/';

          return {
            ...x,
            isRtspValid: x.isRtspValid, // keep backend flag
            liveUrl: url
              ? this.sanitizer.bypassSecurityTrustResourceUrl(url)
              : null,
               rtspUrl: x.rtspUrl,
               rtmpUrl: x.rtmpUrl, 
            error: x.isRtspValid
              ? ''
              : 'Camera is offline or stream unreachable',
            lastUpdated: new Date().toLocaleString(),
          };
        });

        this.alertCameras = mapped.filter((cam) => !cam.isRtspValid);
        this.cameras = mapped.filter((cam) => cam.isRtspValid);

        if (this.selectedAlerts?.toLowerCase() === 'alerts') {
          this.visibleCameras = this.alertCameras;
        } else {
          this.visibleCameras = this.cameras;
        }
      });
  }

  getStatusClass(status: string): string {
    if (!status) return '';

    switch (status.toLowerCase()) {
      case 'in-progress':
      case 'inprogress':
      case 'started':
        return 'status-inprogress';

      case 'not-started':
      case 'not started':
        return 'status-notstarted';

      case 'completed':
        return 'status-completed';

      case 'slow-progress':
      case 'slow progress':
        return 'status-slow';

      default:
        return 'status-default';
    }
  }

  moveCameraToAlerts(cam: DisplayCamera, errorMsg: string) {
    const msg = errorMsg.toLowerCase();

    if (
      msg.includes('timed out') ||
      msg.includes('unreachable') ||
      msg.includes('source of path') ||
      msg.includes('failed')
    ) {
      console.warn('Camera moved to Alerts:', cam.tenderNumber);

      if (!this.alertCameras.some((c) => c.tenderNumber === cam.tenderNumber)) {
        this.alertCameras.push(cam);
      }

      this.cameras = this.cameras.filter(
        (x) => x.tenderNumber !== cam.tenderNumber
      );

      //    this.setVisibleCameras();
    }
  }

  changetype() {
    this.loadData();

    if (this.chartType) {
      setTimeout(() => {}, 300);
    }
  }

  openLiveStream(index?: number, camera?: any) {
    if (index !== undefined) {
      this.state.setState(this.cameras, index);
      this.router.navigate(['/live-viewer']);
      return;
    }

    if (camera) {
      const singleCam = [camera];

      this.state.setState(singleCam, 0);
      this.router.navigate(['/live-viewer']);
      return;
    }

    console.warn('openLiveStream called without index or camera data');
  }

  // get totalCameras(): number {
  //   return this.cameras.length;
  // }

  get totalCameras(): number {
    return this.total; // ✅ backend total
  }

  get startRecord(): number {
    return this.totalCameras === 0 ? 0 : this.currentIndex + 1;
  }

  // get endRecord(): number {
  //   return Math.min(this.currentIndex + this.pageSize, this.totalCameras);
  // }

  get endRecord(): number {
    return Math.min(this.currentIndex + this.pageSize, this.total);
  }

  nextPage() {
    if (this.currentIndex + this.pageSize < this.totalCameras) {
      this.currentIndex += this.pageSize;
      // this.setVisibleCameras();
      this.loadData();
    }
  }

  prevPage() {
    if (this.currentIndex - this.pageSize >= 0) {
      this.currentIndex -= this.pageSize;
      // this.setVisibleCameras();
      this.loadData();
    }
  }

  currentPage = 1;

  get totalPages(): number {
    return Math.ceil(this.totalCameras / this.pageSize);
  }

  goToFirst() {
    this.currentPage = 1;
    // this.setVisibleCameras();
  }

  goToPrev() {
    if (this.currentPage > 1) {
      this.currentPage--;
      // this.setVisibleCameras();
    }
  }

  goToNext() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      // this.setVisibleCameras();
    }
  }

  goToLast() {
    this.currentPage = this.totalPages;
    // this.setVisibleCameras();
  }

  onPageSizeChange() {
    this.currentPage = 1;
    //  this.setVisibleCameras();
  }

  changefilter(val: TableFilterModel) {
    this.tableFilterModel = {
      ...this.tableFilterModel,
      ...val,
    };

    this.loadData();
  }

  handleIconAction(event: any) {
    if (event.type === 'VIEW_CAMERA') {
      const camera = event.record;
      
if (!camera?.rtmpUrl && !camera?.rtspUrl) {
  alert('RTSP/RTMP not found');
  return;
}


      this.openLiveStream(undefined, camera);
    }
  }

  loadFiltersFromLocalStorage(): CameraFilter | null {
    const saved = localStorage.getItem('cameraFilters');
    return saved ? JSON.parse(saved) : null;
  }
}
