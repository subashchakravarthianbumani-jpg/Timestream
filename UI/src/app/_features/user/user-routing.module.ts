import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserCreateComponent } from './user-create/user-create.component';
import { UserListComponent } from './user-list/user-list.component';
import { DatatablePaginationModule } from 'src/app/shared/datatable-pagination/datatable-pagination.module';

const routes: Routes = [
  { path: '', component: UserListComponent },
  { path: 'create/:id', component: UserCreateComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UserRoutingModule {}
