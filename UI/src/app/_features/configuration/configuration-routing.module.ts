import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TwoColumnConfigComponent } from './two-column-config/two-column-config.component';
import { RoleComponent } from './role/role.component';
import { PrivilegesComponent } from './privileges/privileges.component';
import { AuthGuard } from 'src/app/_helpers/auth.guard';
import { QuickLinksComponent } from './quick-links/quick-links.component';
import { TemplatesComponent } from './templates/templates.component';
import { TemplateMilestoneComponent } from './template-milestone/template-milestone.component';
import { ApprovalFlowComponent } from './approval-flow/approval-flow.component';
import { AlertConfigComponent } from './alert-config/alert-config.component';
import { AlertConfigCreateComponent } from './alert-config-create/alert-config-create.component';

const routes: Routes = [
  { path: '', component: TwoColumnConfigComponent },
  { path: 'role', component: RoleComponent },
  {
    path: 'role/privileges',
    component: PrivilegesComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'quicklinks',
    component: QuickLinksComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'templates',
    component: TemplatesComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'alert-config',
    component: AlertConfigComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'alert-config-create/:id',
    component: AlertConfigCreateComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'milestones/:id',
    component: TemplateMilestoneComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'approval-flow',
    component: ApprovalFlowComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ConfigurationRoutingModule {}
