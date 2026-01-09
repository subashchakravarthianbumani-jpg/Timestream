import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportsRoutingModule } from './reports-routing.module';
import { ReportWorkComponent } from './report-work/report-work.component';
import { ReportMilestoneComponent } from './report-milestone/report-milestone.component';
import { ReportMainfiltersComponent } from './report-mainfilters/report-mainfilters.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ButtonModule } from 'primeng/button';
import { FileUploadModule } from 'primeng/fileupload';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { DatatablePaginationModule } from 'src/app/shared/datatable-pagination/datatable-pagination.module';
import { UiModule } from 'src/app/shared/ui/ui.module';
import {
  QuickLinkReducer,
  QuickLinkStateKey,
} from '../configuration/quick-links/state/quicklink.reducers';
import { quicklinkEffects } from '../configuration/quick-links/state/quicklink.effects';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { ReportReducer, ReportStateKey } from './state/report.reducers';
import { ReportsEffects } from './state/reportw.effects';
import { ReportFacade } from './state/report.facades';
import { PipeModuleModule } from '../../shared/pipe-module/pipe-module.module';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { ManageWorkModule } from '../manage-work/manage-work.module';

@NgModule({
  declarations: [
    ReportWorkComponent,
    ReportMilestoneComponent,
    ReportMainfiltersComponent,
  ],
  providers: [ReportsEffects, ReportFacade],
  imports: [
    CommonModule,
    ReportsRoutingModule,
    UiModule,
    TableModule,
    ButtonModule,
    ProgressSpinnerModule,
    DatatablePaginationModule,
    MessagesModule,
    ToastModule,
    TableModule,
    FileUploadModule,
    DragDropModule,
    ScrollPanelModule,
    NgScrollbarModule,
    StoreModule.forFeature(ReportStateKey, ReportReducer),
    EffectsModule.forFeature([ReportsEffects]),
    PipeModuleModule,
    ManageWorkModule,
  ],
})
export class ReportsModule {}
