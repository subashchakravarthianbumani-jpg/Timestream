import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TendersComponent } from './tenders.component';
import { GnrteWorkIdComponent } from './gnrte-work-id/gnrte-work-id.component';
import { ViewWorkIdComponent } from './view-work-id/view-work-id.component';
import { MngeWorkIdComponent } from './mnge-work-id/mnge-work-id.component';
import { MilestonePreparationComponent } from './milestone-preparation/milestone-preparation.component';
import { TenderlistComponent } from './tenderlist/tenderlist.component';
import { ReportMainfiltersComponent } from '../reports/report-mainfilters/report-mainfilters.component';

const routes: Routes = [
  {
    path: '',
    component: TendersComponent,
    children: [
      { path: '', component: TenderlistComponent },
      { path: 'generate/:id', component: GnrteWorkIdComponent },
      { path: 'view/:id', component: ViewWorkIdComponent },
      { path: 'manage/:id', component: MngeWorkIdComponent },
      {
        path: 'milestone-preparation/:id',
        component: MilestonePreparationComponent,
      },
      { path: 'view/:id/:update', component: ViewWorkIdComponent },
      { path: 'reports/:status',component:ReportMainfiltersComponent},
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TendersRoutingModule {}
