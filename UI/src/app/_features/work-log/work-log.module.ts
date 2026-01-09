import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { WorkLogRoutingModule } from './work-log-routing.module';
import { ToastModule } from 'primeng/toast';
import { MessagesModule } from 'primeng/messages';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TableModule } from 'primeng/table';
import { DatatableModule } from 'src/app/shared/datatable/datatable.module';
import { UiModule } from 'src/app/shared/ui/ui.module';
import { WorkLogComponent } from './work-log.component';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { userEffects } from '../user/state/user.effects';
import { WorkLogConfigFacade } from './state/worklog.facades';
import { WorkLogStateKey, WorkLogReducer } from './state/worklog.reducers';
import { workLogEffects } from './state/worklog.effects';
import { TagModule } from 'primeng/tag';
import { PaginatorModule } from 'primeng/paginator';
import { NgxPermissionsModule } from 'ngx-permissions';

@NgModule({
  declarations: [WorkLogComponent],
  providers: [WorkLogConfigFacade],
  imports: [
    CommonModule,
    WorkLogRoutingModule,
    MessagesModule,
    ToastModule,
    UiModule,
    StoreModule.forFeature(WorkLogStateKey, WorkLogReducer),
    EffectsModule.forFeature([workLogEffects]),
    TableModule,
    ButtonModule,
    ProgressSpinnerModule,
    DatatableModule,
    TagModule,
    PaginatorModule,
    NgxPermissionsModule.forChild(),
  ],
})
export class WorkLogModule {}
