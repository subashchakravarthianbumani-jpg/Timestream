import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ManageWorkRoutingModule } from './manage-work-routing.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ButtonModule } from 'primeng/button';
import { FileUploadModule } from 'primeng/fileupload';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { DatatablePaginationModule } from 'src/app/shared/datatable-pagination/datatable-pagination.module';
import { UiModule } from 'src/app/shared/ui/ui.module';
import { ManageWorkComponent } from './manage-work.component';
import { CreateWorkComponent } from './create-work/create-work.component';
import { mbookEffects } from './state/mbook.effects';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { MBookStateKey, MBookReducer } from './state/mbook.reducer';
import { MBookConfigFacade } from './state/mbook.facade';
import { ViewMbookComponent } from './view-mbook/view-mbook.component';
import { ViewMbookHistoryComponent } from './view-mbook-history/view-mbook-history.component';
import { ViewMbookRejectionComponent } from './view-mbook-rejection/view-mbook-rejection.component';

@NgModule({
  declarations: [
    ManageWorkComponent,
    CreateWorkComponent,
    ViewMbookComponent,
    ViewMbookHistoryComponent,
    ViewMbookRejectionComponent,
  ],
  imports: [
    CommonModule,
    ManageWorkRoutingModule,
    UiModule,
    TableModule,
    ButtonModule,
    ProgressSpinnerModule,
    DatatablePaginationModule,
    MessagesModule,
    StoreModule.forFeature(MBookStateKey, MBookReducer),
    EffectsModule.forFeature([mbookEffects]),
    ToastModule,
    TableModule,
    FileUploadModule,
    DragDropModule,
  ],
  exports: [ViewMbookHistoryComponent],
  providers: [MBookConfigFacade],
})
export class ManageWorkModule {}
