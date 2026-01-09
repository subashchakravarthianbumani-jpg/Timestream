import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserRoutingModule } from './user-routing.module';
import { UserListComponent } from './user-list/user-list.component';
import { UserCreateComponent } from './user-create/user-create.component';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TableModule } from 'primeng/table';
import { UiModule } from 'src/app/shared/ui/ui.module';
import { MessagesModule } from 'primeng/messages';
import { ToastModule } from 'primeng/toast';
import { DatatableModule } from 'src/app/shared/datatable/datatable.module';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { userEffects } from './state/user.effects';
import { UserStateKey, UserReducer } from './state/user.reducers';
import { UserConfigFacade } from './state/user.facades';
import { NgxPermissionsModule } from 'ngx-permissions';
import { DatatablePaginationModule } from '../../shared/datatable-pagination/datatable-pagination.module';
import { PasswordModule } from 'primeng/password';

@NgModule({
  declarations: [UserListComponent, UserCreateComponent],
  providers: [UserConfigFacade],
  imports: [
    CommonModule,
    UserRoutingModule,
    UiModule,
    TableModule,
    ButtonModule,
    ProgressSpinnerModule,
    DatatableModule,
    StoreModule.forFeature(UserStateKey, UserReducer),
    EffectsModule.forFeature([userEffects]),
    MessagesModule,
    ToastModule,
    NgxPermissionsModule.forChild(),
    DatatablePaginationModule,
    PasswordModule,
  ],
})
export class UserModule {}
