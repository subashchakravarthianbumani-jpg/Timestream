import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { NotfoundComponent } from './demo/components/notfound/notfound.component';
import { AppLayoutComponent } from './layout/app.layout.component';
import { AuthGuard } from './_helpers/auth.guard';
import { NgxPermissionsGuard } from 'ngx-permissions';

import { AccessComponent } from './_auth/access/access.component';
import { PrivacyPolicyComponent } from './_auth/privacy-policy/privacy-policy.component';
import { TermsConditionsComponent } from './_auth/terms-conditions/terms-conditions.component';

@NgModule({
  imports: [
    RouterModule.forRoot(
      [
        {
          path: '',
          component: AppLayoutComponent,
          canActivate: [AuthGuard],
          children: [
            {
              path: '',
              loadChildren: () =>
                import('./_features/dashboard/dashboard.module').then(
                  (m) => m.DashboardModule
                ),
              canActivate: [AuthGuard],
            },
            {
              path: 'dashboard',
              loadChildren: () =>
                import('./demo/components/dashboard/dashboard.module').then(
                  (m) => m.DashboardModule
                ),
              canActivate: [AuthGuard, NgxPermissionsGuard],
              data: {
                permissions: {
                  only: ['DASHBOARD_TENqDER_COUNT', 'TENqDER_sVIEW'],
                },
                redirectTo: ['nor'],
              },
            },
            {
              path: 'operations',
              loadChildren: () =>
                import('./_features/work-log/work-log.module').then(
                  (m) => m.WorkLogModule
                ),
              canActivate: [AuthGuard],
            },
            {
              path: 'tender',
              loadChildren: () =>
                import('./_features/tenders/tenders.module').then(
                  (m) => m.TendersModule
                ),
              canActivate: [AuthGuard],
            },
            {
              path: 'm-book-manage',
              loadChildren: () =>
                import('./_features/manage-work/manage-work.module').then(
                  (m) => m.ManageWorkModule
                ),
              canActivate: [AuthGuard],
            },
            {
              path: 'm-book-create',
              loadChildren: () =>
                import('./_features/manage-work/manage-work.module').then(
                  (m) => m.ManageWorkModule
                ),
              canActivate: [AuthGuard],
            },
            {
              path: 'alerts',
              loadChildren: () =>
                import('./_features/alerts/alerts.module').then(
                  (m) => m.AlertsModule
                ),
            },
            {
              path: 'user',
              loadChildren: () =>
                import('./_features/user/user.module').then(
                  (m) => m.UserModule
                ),
              canActivate: [AuthGuard],
            },
            {
              path: 'InProgress',
              loadChildren: () =>
                import('./_features/reports/reports.module').then(
                  (m) => m.ReportsModule
                ),
              canActivate: [AuthGuard],
            },
            {
              path: 'configuration',
              loadChildren: () =>
                import('./_features/configuration/configuration.module').then(
                  (m) => m.ConfigurationModule
                ),
            },
            {
              path: 'pages',
              loadChildren: () =>
                import('./demo/components/pages/pages.module').then(
                  (m) => m.PagesModule
                ),
            },
            {
              path: 'logs',
              loadChildren: () =>
                import('./_features/logs/logs.module').then(
                  (m) => m.LogsModule
                ),
              canActivate: [AuthGuard],
            },
            {
              path: 'logs/emaillogs',
              loadChildren: () =>
                import('./_features/logs/logs.module').then(
                  (m) => m.LogsModule
                ),
              canActivate: [AuthGuard],
            },
            {
              path: 'reports',
              loadChildren: () =>
                import('./_features/reports/reports.module').then(
                  (m) => m.ReportsModule
                ),
              canActivate: [AuthGuard],
            },
          ],
        },
        {
          path: 'auth',
          loadChildren: () =>
            import('./_auth/auth.module').then((m) => m.AuthModule),
        },
        {
          path: 'landing',
          loadChildren: () =>
            import('./demo/components/landing/landing.module').then(
              (m) => m.LandingModule
            ),
        },
        { path: 'notfound', component: NotfoundComponent },
        { path: 'notaccess', component: AccessComponent },
        { path: 'privacy', component: PrivacyPolicyComponent },
        { path: 'termsandconditions', component: TermsConditionsComponent },
        { path: '**', redirectTo: '/notfound' },
      ],
      {
        scrollPositionRestoration: 'enabled',
        anchorScrolling: 'enabled',
        onSameUrlNavigation: 'reload',
        useHash: false,
      }
    ),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule {}
