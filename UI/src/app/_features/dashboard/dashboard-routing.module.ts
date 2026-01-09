import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { NgxPermissionsGuard } from 'ngx-permissions';
import { AuthGuard } from 'src/app/_helpers/auth.guard';
import { KeyContactsComponent } from './components/key-contacts/key-contacts.component';
import { QuickLinksComponent } from './components/quick-links/quick-links.component';
import { CameraGridComponent } from './components/camera-grid/camera-grid.component';
import { LiveViewerComponent } from './components/live-viewer/live-viewer.component';
import { CameraGridPageComponent } from './components/camera-grid-page/camera-grid-page.component';

const routes: Routes = [
  {
    path: '',
    component: DashboardComponent,
  },
  {
    path: 'key-contacts',
    component: KeyContactsComponent,
  },
  {
    path: 'quick-links',
    component: QuickLinksComponent,
  },
  {
    path:'camera-grid',
    component: CameraGridComponent,
  },

  {
  path: 'camera-grid-page',
  component: CameraGridPageComponent
},

  { path: 'live-viewer',
     component:LiveViewerComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DashboardRoutingModule {}
