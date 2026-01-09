import { HostListener, OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { LayoutService } from './service/app.layout.service';
import { NgxPermissionsService } from 'ngx-permissions';
import { privileges } from '../shared/commonFunctions';

@Component({
  selector: 'app-menu',
  templateUrl: './app.menu.component.html',
})
export class AppMenuComponent implements OnInit {
  model: any[] = [];
  permissions: any[] = [];

  permissionsObject: any = '';
  constructor(
    public layoutService: LayoutService,
    private permissionsService: NgxPermissionsService
  ) {}

  ngOnInit() {
    this.permissionsService.permissions$.subscribe(
      (x) => (this.permissionsObject = x)
    );
    this.permissions = Object.values(privileges);
    this.model = [
      {
        label: 'Home',
        ngxPermissionsOnly: [
          privileges.DASHBOARD_TENDER_COUNT,
          privileges.DASHBOARD_TENDER_RECORD,
          privileges.DASHBOARD_MBOOK_COUNT,
          privileges.DASHBOARD_MBOOK_RECORD,
          privileges.DASHBOARD_KEY_CONTACTS,
          privileges.DASHBOARD_QUICK_LINKS,
        ],
        items: [
          {
            ngxPermissionsOnly: [
              privileges.DASHBOARD_TENDER_COUNT,
              privileges.DASHBOARD_TENDER_RECORD,
              privileges.DASHBOARD_MBOOK_COUNT,
              privileges.DASHBOARD_MBOOK_RECORD,
              privileges.DASHBOARD_KEY_CONTACTS,
              privileges.DASHBOARD_QUICK_LINKS,
            ],
            label: 'Dashboard',
            icon: 'pi pi-fw pi-home',
            routerLink: ['/'],
          },
        ],
      },
      {
        label: 'Operations',
        items: [
          {
            label: 'Works',
            icon: 'pi pi-fw pi-slack',
            ngxPermissionsOnly: [privileges.GO_VIEW, privileges.TENDER_VIEW],
            items: [
              {
                ngxPermissionsOnly: [privileges.GO_VIEW],
                label: 'Government Orders',
                icon: 'pi pi-fw pi-database',
                routerLink: ['/operations'],
                routerLinkActiveOptions: '{exact:true}',
              },
              {
                ngxPermissionsOnly: [privileges.TENDER_VIEW],
                label: 'Manage',
                icon: 'pi pi-fw pi-table',
                routerLink: ['/tender'],
                routerLinkActiveOptions: '{exact:true}',
              },
              
            ],
          },
          {
            label: 'M-Book',
            icon: 'pi pi-fw pi-book',
            ngxPermissionsOnly: [
              privileges.MBOOK_VIEW,
              privileges.MBOOK_HISTORY,
            ],
            routerLink: ['/m-book-manage'],
            routerLinkActiveOptions: '{exact:true}',
          },
          // {
          //   label: 'M-Book',
          //   icon: 'pi pi-fw pi-book',
          //   ngxPermissionsOnly: [
          //     privileges.MBOOK_VIEW,
          //     privileges.MBOOK_HISTORY,
          //   ],
          //   items: [
          //     {
          //       label: 'Manage',
          //       ngxPermissionsOnly: [privileges.MBOOK_VIEW],
          //       routerLink: ['/m-book-manage'],
          //       icon: 'pi pi-fw pi-map',
          //     },
          //     // {
          //     //   label: 'History',
          //     //   ngxPermissionsOnly: [privileges.MBOOK_HISTORY],
          //     //   icon: 'pi pi-fw pi-history',
          //     //   routerLink: ['/m-book-manage/history'],
          //     // },
          //   ],
          // },

          {
            label: 'Approvals',
            ngxPermissionsOnly: [privileges.MBOOK_APPROVE_REJECT],
            icon: 'pi pi-fw pi-check',
            routerLink: ['/m-book-manage/approvals'],
            routerLinkActiveOptions: '{exact:true}',
          },
          {
            label: 'Reports',
            ngxPermissionsOnly: [privileges.REPORTS_VIEW],
            icon: 'pi pi-fw pi-file',
            routerLink: ['/reports'],
            routerLinkActiveOptions: '{exact:true}',
          },
          
          {
            label: 'Alerts',
            ngxPermissionsOnly: [privileges.ALERT_VIEW],
            icon: 'pi pi-fw pi-exclamation-triangle',
            routerLink: ['/alerts'],
            routerLinkActiveOptions: '{exact:true}',
          },
        ],
      },
      {
        label: 'Settings',
        icon: 'pi pi-fw pi-globe',
        ngxPermissionsOnly: [
          privileges.CONFIG_VIEW,
          privileges.ALERT_CONFIG_VIEW,
          privileges.ROLE_VIEW,
          privileges.USER_VIEW,
          privileges.USER_CREATE,
          privileges.QL_VIEW,
          privileges.TEMPLATE_VIEW,
          privileges.APPROVALFLOW_VIEW,
          privileges.RECORD_LOG_VIEW,
          privileges.EMAIL_SMS_LOG_VIEW,
        ],
        items: [
          {
            label: 'Configurations',
            ngxPermissionsOnly: [privileges.CONFIG_VIEW],
            icon: 'pi pi-fw pi-globe',
            routerLink: ['/configuration'],
          },
          {
            label: 'Alert Configurations',
            icon: 'pi pi-fw pi-bell',
            ngxPermissionsOnly: [privileges.ALERT_CONFIG_VIEW],
            routerLink: ['/configuration/alert-config'],
            routerLinkActiveOptions: '{exact:true}',
          },
          {
            label: 'Roles',
            ngxPermissionsOnly: [privileges.ROLE_VIEW],
            icon: 'pi pi-fw pi-th-large',
            routerLink: ['/configuration/role'],
            routerLinkActiveOptions: '{exact:true}',
          },
          {
            label: 'Users',
            ngxPermissionsOnly: [privileges.USER_VIEW],
            icon: 'pi pi-fw pi-users',
            items: [
              {
                label: 'Manage',
                ngxPermissionsOnly: [privileges.USER_VIEW],
                icon: 'pi pi-fw pi-users',
                routerLink: ['/user'],
              },
              {
                label: 'Create',
                ngxPermissionsOnly: [privileges.USER_CREATE],
                icon: 'pi pi-fw pi-user',
                routerLink: ['/user/create/0'],
              },
            ],
          },
          {
            label: 'Quick Links',
            ngxPermissionsOnly: [privileges.QL_VIEW],
            icon: 'pi pi-fw pi-paperclip',
            routerLink: ['/configuration/quicklinks'],
            routerLinkActiveOptions: '{exact:true}',
          },
          {
            label: 'Templates',
            ngxPermissionsOnly: [privileges.TEMPLATE_VIEW],
            icon: 'pi pi-fw pi-database',
            routerLink: ['/configuration/templates'],
            routerLinkActiveOptions: '{exact:true}',
          },
          {
            label: 'Approval Flow',
            ngxPermissionsOnly: [privileges.APPROVALFLOW_VIEW],
            icon: 'pi pi-fw pi-arrow-right-arrow-left',
            routerLink: ['/configuration/approval-flow'],
            routerLinkActiveOptions: '{exact:true}',
          },
          {
            label: 'Logs',
            ngxPermissionsOnly: [
              privileges.RECORD_LOG_VIEW,
              privileges.EMAIL_SMS_LOG_VIEW,
            ],
            icon: 'pi pi-fw pi-history',
            items: [
              {
                label: 'Change History',
                ngxPermissionsOnly: [privileges.RECORD_LOG_VIEW],
                icon: 'pi pi-fw pi-hourglass',
                routerLink: ['/logs/fieldlog'],
                routerLinkActiveOptions: '{exact:true}',
              },
              {
                label: 'Email / SMS Logs',
                ngxPermissionsOnly: [privileges.EMAIL_SMS_LOG_VIEW],
                icon: 'pi pi-fw pi-sitemap',
                routerLink: ['/logs/emaillogs'],
                routerLinkActiveOptions: '{exact:true}',
              },
            ],
          },
        ],
      },
    ];
  
  
  this.forceFullscreen();
  }

    forceFullscreen() {
    const elem = document.documentElement;
    if (elem.requestFullscreen) elem.requestFullscreen();
    else if ((elem as any).webkitRequestFullscreen) (elem as any).webkitRequestFullscreen();
    else if ((elem as any).msRequestFullscreen) (elem as any).msRequestFullscreen();
  }

  @HostListener("document:fullscreenchange", [])
  onExit() {
    if (!document.fullscreenElement) {
      this.forceFullscreen(); // Keep fullscreen always
    }
  }
}
