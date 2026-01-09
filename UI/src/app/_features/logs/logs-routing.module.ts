import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FieldChangelistComponent } from './field-changelist/field-changelist.component';
import { EmailLogsComponent } from './email-logs/email-logs.component';

const routes: Routes = [
  { path: 'fieldlog', component: FieldChangelistComponent },
  { path: 'emaillogs', component: EmailLogsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LogsRoutingModule {}
