import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgxPermissionsGuard } from 'ngx-permissions';
import { AuthGuard } from 'src/app/_helpers/auth.guard';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: 'crud',
        loadChildren: () =>
          import('./crud/crud.module').then((m) => m.CrudModule),
        canActivate: [AuthGuard, NgxPermissionsGuard],
        data: {
          permissions: {
            only: ['ADMIN', 'MODERATOR'],
            except: ['GUEST'],
          },
        },
      },
      {
        path: 'empty',
        loadChildren: () =>
          import('./empty/emptydemo.module').then((m) => m.EmptyDemoModule),
      },
      {
        path: 'timeline',
        loadChildren: () =>
          import('./timeline/timelinedemo.module').then(
            (m) => m.TimelineDemoModule
          ),
      },
      { path: '**', redirectTo: '/notfound' },
    ]),
  ],
  exports: [RouterModule],
})
export class PagesRoutingModule {}
