import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LogsRoutingModule } from './logs-routing.module';
import { FieldChangelistComponent } from './field-changelist/field-changelist.component';
import { ButtonModule } from 'primeng/button';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { DatatableModule } from 'src/app/shared/datatable/datatable.module';
import { UiModule } from 'src/app/shared/ui/ui.module';
import { DatatablePaginationModule } from '../../shared/datatable-pagination/datatable-pagination.module';
import { EmailLogsComponent } from './email-logs/email-logs.component';

@NgModule({
  declarations: [FieldChangelistComponent, EmailLogsComponent],
  imports: [
    CommonModule,
    LogsRoutingModule,
    UiModule,
    TableModule,
    ButtonModule,
    ProgressSpinnerModule,
    DatatableModule,
    MessagesModule,
    ToastModule,
    DatatablePaginationModule,
  ],
})
export class LogsModule {}
