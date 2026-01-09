import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TendersRoutingModule } from './tenders-routing.module';
import { GnrteWorkIdComponent } from './gnrte-work-id/gnrte-work-id.component';
import { MilestonePreparationComponent } from './milestone-preparation/milestone-preparation.component';
import { MngeWorkIdComponent } from './mnge-work-id/mnge-work-id.component';
import { TendersComponent } from './tenders.component';
import { ViewWorkIdComponent } from './view-work-id/view-work-id.component';
import { TenderlistComponent } from './tenderlist/tenderlist.component';
import { DatatablePaginationModule } from 'src/app/shared/datatable-pagination/datatable-pagination.module';
import { MessagesModule } from 'primeng/messages';
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { tenderEffects } from './state/tender.effects';
import { TenderReducer, TenderStateKey } from './state/tender.reducers';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { TenderFacade } from './state/tender.facades';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { UiModule } from 'src/app/shared/ui/ui.module';
import { FileUploadModule } from 'primeng/fileupload';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { NgxPermissionsModule } from 'ngx-permissions';
import { PipeModuleModule } from 'src/app/shared/pipe-module/pipe-module.module';



@NgModule({
  declarations: [
    TendersComponent,
    GnrteWorkIdComponent,
    MngeWorkIdComponent,
    MilestonePreparationComponent,
    ViewWorkIdComponent,
    TenderlistComponent,
   
  ],
  providers: [TenderFacade],
  imports: [
    CommonModule,
    TendersRoutingModule,
    PipeModuleModule,
    UiModule,
    TableModule,
    ButtonModule,
    ProgressSpinnerModule,
    DatatablePaginationModule,
    StoreModule.forFeature(TenderStateKey, TenderReducer),
    EffectsModule.forFeature([tenderEffects]),
    MessagesModule,
    ToastModule,
    TableModule,
    FileUploadModule,
    DragDropModule,
    ScrollPanelModule,
    NgScrollbarModule,
    NgxPermissionsModule.forChild(),
  ],
})
export class TendersModule {}
