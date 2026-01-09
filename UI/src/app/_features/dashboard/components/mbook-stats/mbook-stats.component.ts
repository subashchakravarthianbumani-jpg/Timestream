import { HttpClient } from '@angular/common/http';
import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { DashboardRecordCountCardModel } from 'src/app/_models/dashboard.model';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { environment } from 'src/environments/environment';
 
@Component({
  selector: 'app-mbook-stats',
  templateUrl: './mbook-stats.component.html',
  styleUrls: ['./mbook-stats.component.scss'],
})
export class MbookStatsComponent {
  @Input() year: string[] = [];
  @Input() recordCount!: DashboardRecordCountCardModel;
 
  constructor(
    private router: Router,
  private http: HttpClient
  )
  {}
 
  Type="MBOOK";
  Saved!: string;
  NotUpload!: string;
  Paymentpendin!: string;
  noActiontaken!: string;
 
  // Saved="8e6b29fa-7b20-11ee-b363-fa163e14116e";
  // NotUpload="8e6b2e55-7b20-11ee-b363-fa163e14116e";
  // Paymentpendin="42b8efcc-50e6-11f0-9806-2c98117e76d0";
  // //noActiontaken=["3e9e2493-7afb-11ee-b363-fa163e14116e", "3e9e2783-7afb-11ee-b363-fa163e14116e"];
  // noActiontaken="febc5537-50ec-11f0-9806-2c98117e76d0";
 ngOnInit() {
    this.http
      .get<ResponseModel>(`${environment.apiUrl}/Report/Alert_Filter_Form`)
      .subscribe((res) => {
        if (res.status === 'SUCCESS' && res.data?.statusList) {
          const statuses = res.data.statusList;
 
          this.Saved = statuses.find((x: any) => x.text === 'Saved')?.value;
          this.NotUpload = statuses.find((x: any) => x.text === 'Submitted')?.value;
          this.Paymentpendin = statuses.find((x: any) => x.text === 'Waiting For Payment')?.value;
          this.noActiontaken = statuses.find((x: any) => x.text === 'No Action Taken')?.value;
        }
      });
  }
 
  Uploaded()
{
  this.router.navigate(['/reports'],
    {
   queryParams: { Type: this.Type,
     Status:this.Saved,
     Year:this.year
 
   }
 });
}
 
 
NotUploaded()
{
  this.router.navigate(['/reports'],
    {
   queryParams: { Type: this.Type,
     Status:this.NotUpload,
     Year:this.year
 
   }
 });
}
 
 
 
Paymentpending()
{
  this.router.navigate(['/reports'],
    {
   queryParams: { Type: this.Type,
     Status:this.Paymentpendin,
     Year:this.year
 
   }
 });
}
noAction()
{
  this.router.navigate(['/reports'],
    {
   queryParams: { Type: this.Type,
     Status:this.noActiontaken,
     Year:this.year
 
   }
 });
}
Mbooks()
{
  this.router.navigate(['/reports'],
    {
   queryParams: {
    Type: this.Type,
     Year:this.year
 
   }
 });
}
}