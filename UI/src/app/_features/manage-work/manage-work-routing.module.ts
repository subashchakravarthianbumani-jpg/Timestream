import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageWorkComponent } from './manage-work.component';
import { CreateWorkComponent } from './create-work/create-work.component';
import { ViewMbookComponent } from './view-mbook/view-mbook.component';

const routes: Routes = [
  { path: '', component: ManageWorkComponent },
  { path: ':type', component: ManageWorkComponent },
  { path: 'edit/:id', component: CreateWorkComponent },
  { path: 'view/:id/:isapprove', component: ViewMbookComponent },
  { path: 'view/:id', component: ViewMbookComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ManageWorkRoutingModule {}
