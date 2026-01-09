import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatatablePaginationComponent } from './datatable-pagination.component';
import { ButtonModule } from 'primeng/button';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { MessagesModule } from 'primeng/messages';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { UiModule } from '../ui/ui.module';
import { NgxPermissionsModule } from 'ngx-permissions';
import { PipeModuleModule } from '../pipe-module/pipe-module.module';

@NgModule({
  declarations: [DatatablePaginationComponent],
  providers: [],
  imports: [
    CommonModule,
    TableModule,
    UiModule,
    PaginatorModule,
    CommonModule,
    PipeModuleModule,
    ButtonModule,
    ConfirmPopupModule,
    MessagesModule,
    ToastModule,
    TagModule,
    NgxPermissionsModule.forChild(),
  ],
  exports: [DatatablePaginationComponent],
})
export class DatatablePaginationModule {}
