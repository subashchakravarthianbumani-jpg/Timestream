import { HttpClient } from '@angular/common/http';
import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { selectdepartmentList } from 'src/app/_features/user/state/user.selectors';
import { DashboardRecordCountCardModel } from 'src/app/_models/dashboard.model';
import { DashboardCameraCountModel } from 'src/app/_models/dashboard.model';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { DashboardService } from 'src/app/_services/dashboard.service';
import { privileges } from 'src/app/shared/commonFunctions';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-tender-stats',
  templateUrl: './tender-stats.component.html',
  styleUrls: ['./tender-stats.component.scss'],
})
export class TenderStatsComponent {
  @Input() year: string[] = [];
  @Input() recordCount!: DashboardRecordCountCardModel;
  cameraInstalledCount!: DashboardCameraCountModel;
  constructor(
    private router: Router,
    private http: HttpClient,
    private dashboardService: DashboardService
  ) {}
  Type = 'TENDER';
  InprogressId!: string;
  CompletedId!: string;
  slowprogres!: string;
  onhold!: string;
  notstarted: string[] = [];
  // InprogressId='198ccf78-8de2-11ee-aa46-fa163e14116e';//In progress
  // CompletedId='8e6b304b-7b20-11ee-b363-fa163e14116e';
  // slowprogres='40625423-7b36-11ee-b363-fa163e14116b';
  // onhold='40625423-7b36-11ee-b363-fa163e14116c';
  // Status='Not-Started';
  // notstarted=['40625423-7b36-11ee-b363-fa163e14116a','3e9e2926-7afb-11ee-b363-fa163e14116e'];

  ngOnInit() {
    this.http
      .get<ResponseModel>(`${environment.apiUrl}/Report/Alert_Filter_Form`)
      .subscribe((res) => {
        if (res.status === 'SUCCESS' && res.data?.statusList) {
          const statuses = res.data.statusList;

          this.InprogressId = statuses.find(
            (x: any) => x.text === 'In-progress'
          )?.value;
          this.CompletedId = statuses.find(
            (x: any) => x.text === 'Completed'
          )?.value;
          this.slowprogres = statuses.find(
            (x: any) => x.text === 'Slow Progress'
          )?.value;
          this.onhold = statuses.find(
            (x: any) => x.text === 'Started but Stilled'
          )?.value;

          // multiple values → array
          this.notstarted = [
            statuses.find((x: any) => x.text === 'Not-Started')?.value,
            statuses.find((x: any) => x.text === 'New')?.value,
          ].filter(Boolean);
        }
      });
    this.getcountdata();
  }
  getcountdata() {
    this.dashboardService
      .getDashboardCameraStatusCount({
        notStarted: '',
        startedButStilled: '',
        inProgress: '',
        slowProgress: '',
        completed: '',
        total: '',
      })
      .subscribe(
        (x) => {
          this.cameraInstalledCount = x.data[0];
        },
        (error) => {}
      );
  }

  Inprogress() {
    this.router.navigate(['/camera-grid-page'], {
      queryParams: {
        Type: this.Type,
        chartType: true,
        Status: 'In-progress',
        Year: this.year,
      },
    });
  }
  gotoreports() {
    this.router.navigate(['/camera-grid-page'], {
      queryParams: { Type: this.Type, Year: this.year, chartType: true },
    });
  }

  slowprogress() {
    this.router.navigate(['/camera-grid-page'], {
      queryParams: {
        Type: this.Type,
        Status: 'Slow Progress',
        Year: this.year,
        chartType: true,
      },
    });
  }
  onHold() {
    this.router.navigate(['/camera-grid-page'], {
      queryParams: {
        Type: this.Type,
        Status: 'Started but Stilled',
        Year: this.year,
        chartType: true,
      },
    });
  }
  Completed() {
    this.router.navigate(['/camera-grid-page'], {
      queryParams: {
        Status: 'Completed',
        chartType: true,
      },
    });
  }
  Notstarted() {
    this.router.navigate(['/camera-grid-page'], {
      queryParams: {
        Type: this.Type,
        Status: 'Not-Started',
        Year: this.year,
        chartType: true,
      },
    });
  }
}
