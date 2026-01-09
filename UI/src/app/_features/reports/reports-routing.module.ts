import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReportWorkComponent } from './report-work/report-work.component';
import { ReportMainfiltersComponent } from './report-mainfilters/report-mainfilters.component';

const routes: Routes = [
  {
    path: '',
    component: ReportMainfiltersComponent,
  },
  
  
  

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportsRoutingModule {}
