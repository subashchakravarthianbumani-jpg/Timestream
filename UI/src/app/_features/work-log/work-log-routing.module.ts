import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WorkLogComponent } from './work-log.component';

const routes: Routes = [{ path: '', component: WorkLogComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class WorkLogRoutingModule {}
