import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaginatorModule } from 'primeng/paginator';
import { DatatableComponent } from './datatable.component';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { MessagesModule } from 'primeng/messages';
import { ToastModule } from 'primeng/toast';
import { UiModule } from '../ui/ui.module';
import { TagModule } from 'primeng/tag';
import { PipeModuleModule } from '../pipe-module/pipe-module.module';
 
@NgModule({
  declarations: [DatatableComponent],
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
  ],
  exports: [DatatableComponent],
})
export class DatatableModule {}
 
 