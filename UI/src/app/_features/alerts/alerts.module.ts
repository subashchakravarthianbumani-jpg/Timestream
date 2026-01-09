import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AlertsRoutingModule } from './alerts-routing.module';
import { AlertsComponent } from './alerts.component';
import { UiModule } from 'src/app/shared/ui/ui.module';

@NgModule({
  declarations: [AlertsComponent],
  imports: [CommonModule, AlertsRoutingModule, UiModule],
})
export class AlertsModule {}
