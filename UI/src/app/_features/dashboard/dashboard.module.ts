import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
 
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';
import { ButtonModule } from 'primeng/button';
import { SelectButtonModule } from 'primeng/selectbutton';
import { KeyContactsComponent } from './components/key-contacts/key-contacts.component';
import { QuickLinksComponent } from './components/quick-links/quick-links.component';
import { StatisticsComponent } from './components/statistics/statistics.component';
import { FormsModule } from '@angular/forms';
import { ChartModule } from 'primeng/chart';
import { MenuModule } from 'primeng/menu';
import { PanelMenuModule } from 'primeng/panelmenu';
import { StyleClassModule } from 'primeng/styleclass';
import { TableModule } from 'primeng/table';
import { DashboardsRoutingModule } from 'src/app/demo/components/dashboard/dashboard-routing.module';
import { UiModule } from 'src/app/shared/ui/ui.module';
import { TenderStatsComponent } from './components/tender-stats/tender-stats.component';
import { MbookStatsComponent } from './components/mbook-stats/mbook-stats.component';
import { MbookGridComponent } from './components/mbook-grid/mbook-grid.component';
import { TenderGridComponent } from './components/tender-grid/tender-grid.component';
import { DatatableModule } from '../../shared/datatable/datatable.module';
import { NgxPermissionsModule } from 'ngx-permissions';
import { CameraGridComponent } from './components/camera-grid/camera-grid.component';
import { LiveViewerComponent } from './components/live-viewer/live-viewer.component';
import { DatatablePaginationModule } from 'src/app/shared/datatable-pagination/datatable-pagination.module';
import { CameraGridPageComponent } from './components/camera-grid-page/camera-grid-page.component';
import { FilterPanelComponent } from './components/filter-panel/filter-panel.component';
 
@NgModule({
  declarations: [
    DashboardComponent,
    KeyContactsComponent,
    QuickLinksComponent,
    StatisticsComponent,
    TenderStatsComponent,
    MbookStatsComponent,
    MbookGridComponent,
    TenderGridComponent,
    CameraGridComponent,
    LiveViewerComponent,
    FilterPanelComponent,
    CameraGridPageComponent,
  ],
  imports: [
    UiModule,
    CommonModule,
    DashboardRoutingModule,
    ButtonModule,
    SelectButtonModule,
    CommonModule,
    FormsModule,
    ChartModule,
    MenuModule,
    TableModule,
    StyleClassModule,
    PanelMenuModule,
    ButtonModule,
    DashboardsRoutingModule,
    DatatableModule,
    NgxPermissionsModule.forChild(),
    DatatablePaginationModule,
  ],
})
export class DashboardModule {}
 
 