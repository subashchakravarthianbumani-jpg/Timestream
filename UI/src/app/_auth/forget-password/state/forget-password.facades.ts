import { Store } from '@ngrx/store';
import * as ForgetPasswordActions from '../state/forget-password.action';
import * as ForgetPasswordSelectors from '../state/forget-password.selectors';
import { Injectable } from '@angular/core';

@Injectable()
export class ForgetPasswordFacade {
  constructor(private store: Store) {}

  getSentOtpStatus$ = this.store.select(ForgetPasswordSelectors.getSentOtpStatus);
  getToken$ = this.store.select(ForgetPasswordSelectors.getToken);
  getOtpVerificationStatus$ = this.store.select(ForgetPasswordSelectors.getOtpVerificationStatus);
  saveNewPasswordStatus$ = this.store.select(ForgetPasswordSelectors.saveNewPasswordStatus);


  sendOtp(mobileNumber: string) {
    this.store.dispatch(ForgetPasswordActions.forgetPasswordSendOTP({ mobileNumber }));
  }

  validateOtp(mobileNumber: string, otp: string) {
    this.store.dispatch(ForgetPasswordActions.forgetPasswordVerifyOTP({ mobileNumber, otp }));
  }

  saveNewPassword(token: string, otp: string, password: string) {
    this.store.dispatch(ForgetPasswordActions.saveNewPassword({ token, otp, password }));
  }

}