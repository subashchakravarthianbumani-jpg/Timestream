import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule } from './auth-routing.module';
import { Message, MessageService } from 'primeng/api';
import { PrivacyPolicyComponent } from './privacy-policy/privacy-policy.component';
import { TermsConditionsComponent } from './terms-conditions/terms-conditions.component';

@NgModule({
  imports: [CommonModule, AuthRoutingModule],
  providers: [MessageService],
  declarations: [
    PrivacyPolicyComponent,
    TermsConditionsComponent
  ],
})
export class AuthModule {
  msgs: Message[] = [];
}
