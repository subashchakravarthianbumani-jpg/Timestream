import { Component, ElementRef, OnInit, ViewChild, EventEmitter, Output, Input  } from '@angular/core';
import { MenuItem, MessageService } from 'primeng/api';
import { LayoutService } from './service/app.layout.service';
import { UserModel } from '../_models/user';
import { getUserDetails } from '../state/actions/auth.actions';
import { AccountService } from '../_services/account.service';
import { ActivatedRoute, Router, RouterStateSnapshot } from '@angular/router';
import { ThemeService } from 'src/app/_services/theme.service';
import { DashboardRecordCountCardModel } from '../_models/dashboard.model';
import { CameraService } from 'src/app/_services/camera.service';

@Component({
  selector: 'app-topbar',
  templateUrl: './app.topbar.component.html',
  styleUrls: ['./app.topbar.component.scss'],

})
export class AppTopBarComponent implements OnInit {
  items!: MenuItem[];
  user!: UserModel;
    showSettings: boolean = false;
settingsItems: MenuItem[] = [];

alertCount: number = 0;

  @ViewChild('menubutton') menuButton!: ElementRef;

  @ViewChild('topbarmenubutton') topbarMenuButton!: ElementRef;

  @ViewChild('topbarmenu') menu!: ElementRef;

  @Output() filterClick = new EventEmitter<void>(); 
  
   @Input() recordCount!: DashboardRecordCountCardModel;

  constructor(
    public layoutService: LayoutService,
    private accountService: AccountService,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private themeService: ThemeService,
    private cameraService: CameraService
    
  ) {}
  ngOnInit(): void {
    this.user = this.accountService.userValue;
    this.items = [
      {
        label: 'Profile',
        icon: 'pi pi-fw pi-user-edit',
        routerLink: ['/configuration'],
      },
      {
        label: 'Logout',
        icon: 'pi pi-fw pi-sign-out',
        command: (e) => this.log(e),
      },
    ];
  this.loadAlertCount();

  }
  log(e: any) {
    this.accountService.logout();

    const returnUrl = this.router.url;
    this.router.navigate(['/auth/login'], {
      //queryParams: { returnUrl: returnUrl },
    });
  }

  goToDashboard() {
  this.router.navigate(['/']);
}


  openFilter() {
    this.filterClick.emit();                          // ✅ EMIT EVENT
  }
  openSettings(){

  }


toggleSettingsDropdown() {
  this.showSettings = !this.showSettings;
}

navigateToUserCreate() {
  this.router.navigate(['/user']);
  this.showSettings = false;
}

toggleTheme() {
  this.themeService.toggle();
}

isDark() {
  return this.themeService.isDark();
}

Alert() {
    this.router.navigate(['/camera-grid-page'], {
      queryParams: {
        Alerts: 'Alerts',
        chartType: true,
      },
    });
  }

  loadAlertCount() {
  this.cameraService
    .getAllCameras({
      divisionIds: null,
      districtIds: null,
      tenderId: '',
      divisionName: '',
      districtName: '',
      mainCategory: '',
      subcategory: '',
      workStatus: '',
      tenderNumber: '',
      channel: '',
      rtspUrl: '',
      liveUrl: '',
      type: 'Alert',              
      skip: 0,
      take: 1,                    
      SearchString: '',
      sorting: {
        fieldName: 'tenderNumber',
        sort: 'DESC',
      },
    })
    .subscribe({
      next: (res) => {
        this.alertCount = res?.totalRecordCount || 0;
        console.log('🔔 Alert count:', this.alertCount);
      },
      error: (err) => {
        console.error('Failed to load alert count', err);
        this.alertCount = 0;
      },
    });
}


}



  
  
