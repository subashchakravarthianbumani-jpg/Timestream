import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ForgetPasswordComponent } from './forget-password.component';
import { ForgetPasswordRoutingModule } from './forget-password-routing.module';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { FormsModule } from '@angular/forms';
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { Message, MessageService } from 'primeng/api';
import { MessagesModule } from 'primeng/messages';
import { ToastModule } from 'primeng/toast';
import { ForgetPasswordService } from 'src/app/_services/forget-password.service';
import { ForgetPasswordFacade } from './state/forget-password.facades';
import { StoreModule } from '@ngrx/store';
import { ForgetPasswordReducer, ForgetPasswordStateKey } from './state/forget-password.reducers';
import { EffectsModule } from '@ngrx/effects';
import { quicklinkEffects } from './state/forget-password.effects';

@NgModule({
  declarations: [ForgetPasswordComponent],
  imports: [
    CommonModule,
    ForgetPasswordRoutingModule,
    ButtonModule,
    CheckboxModule,
    InputTextModule,
    FormsModule,
    ToastModule,
    MessagesModule,
    PasswordModule,

    StoreModule.forFeature(ForgetPasswordStateKey, ForgetPasswordReducer),
    EffectsModule.forFeature([
      quicklinkEffects,
    ]),
  ],
  providers: [
    ForgetPasswordFacade,
    ForgetPasswordService
  ]
})
export class ForgetPasswordModule { }
