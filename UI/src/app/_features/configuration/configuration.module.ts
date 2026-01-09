import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ConfigurationRoutingModule } from './configuration-routing.module';
import { TwoColumnConfigComponent } from './two-column-config/two-column-config.component';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { PaginatorModule } from 'primeng/paginator';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { StoreModule } from '@ngrx/store';
import {
  TwoColConfigStateKey,
  twoColConfigReducer,
} from './two-column-config/state/reducers/two.col.reducers';
import { TwoColConfigFacade } from './two-column-config/state/facades/two.col.facades';
import { EffectsModule } from '@ngrx/effects';
import { TwoColEffects } from './two-column-config/state/effects/two.col.effects';
import { DatatableModule } from 'src/app/shared/datatable/datatable.module';
import { UiModule } from 'src/app/shared/ui/ui.module';
import { MessagesModule } from 'primeng/messages';
import { ToastModule } from 'primeng/toast';
import { RoleComponent } from './role/role.component';
import { PrivilegesComponent } from './privileges/privileges.component';
import { RoleConfigFacade } from './role/state/role.facades';
import { RoleReducer, RoleStateKey } from './role/state/role.reducers';
import { roleEffects } from './role/state/role.effects';
import { QuickLinksComponent } from './quick-links/quick-links.component';
import {
  QuickLinkReducer,
  QuickLinkStateKey,
} from './quick-links/state/quicklink.reducers';
import { quicklinkEffects } from './quick-links/state/quicklink.effects';
import { QuickLinkConfigFacade } from './quick-links/state/quicklink.facades';
import { TemplatesComponent } from './templates/templates.component';
import { TemplateMilestoneComponent } from './template-milestone/template-milestone.component';
import { TemplatesConfigFacade } from './templates/state/template.facades';
import {
  TemplatesReducer,
  TemplatesStateKey,
} from './templates/state/template.reducers';
import { templateEffects } from './templates/state/template.effects';
import { ApprovalFlowComponent } from './approval-flow/approval-flow.component';
import { ApprovalFlowConfigFacade } from './approval-flow/state/approvalflow.facades';
import {
  ApprovalFlowReducer,
  ApprovalFlowStateKey,
} from './approval-flow/state/approvalflow.reducers';
import { approvalFlowEffects } from './approval-flow/state/appflow.effects';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { AlertConfigComponent } from './alert-config/alert-config.component';
import { AlertConfigCreateComponent } from './alert-config-create/alert-config-create.component';
import { NgxPermissionsModule } from 'ngx-permissions';
@NgModule({
  declarations: [
    TwoColumnConfigComponent,
    RoleComponent,
    PrivilegesComponent,
    QuickLinksComponent,
    TemplatesComponent,
    TemplateMilestoneComponent,
    ApprovalFlowComponent,
    AlertConfigComponent,
    AlertConfigCreateComponent,
  ],
  providers: [
    TwoColConfigFacade,
    RoleConfigFacade,
    QuickLinkConfigFacade,
    TemplatesConfigFacade,
    ApprovalFlowConfigFacade,
  ],
  imports: [
    CommonModule,
    ConfigurationRoutingModule,
    UiModule,
    TableModule,
    ButtonModule,
    ProgressSpinnerModule,
    StoreModule.forFeature(TwoColConfigStateKey, twoColConfigReducer),
    StoreModule.forFeature(RoleStateKey, RoleReducer),
    StoreModule.forFeature(QuickLinkStateKey, QuickLinkReducer),
    StoreModule.forFeature(TemplatesStateKey, TemplatesReducer),
    StoreModule.forFeature(ApprovalFlowStateKey, ApprovalFlowReducer),
    EffectsModule.forFeature([
      TwoColEffects,
      roleEffects,
      quicklinkEffects,
      templateEffects,
      approvalFlowEffects,
    ]),
    DatatableModule,
    MessagesModule,
    ToastModule,
    DragDropModule,
    NgxPermissionsModule.forChild(),
  ],
})
export class ConfigurationModule {}
