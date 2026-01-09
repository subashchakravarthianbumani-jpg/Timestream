import { Action, createReducer, on } from '@ngrx/store';
import { ResponseStatus } from 'src/app/_models/utils';
import * as ForgetPasswordActions from '../state/forget-password.action';

export const ForgetPasswordStateKey = 'ForgetPasswordState';

export interface ForgetPasswordState {
  saveStatus: ResponseStatus | null;
  token: string;
  otpVerificationStatus: ResponseStatus | null;
  savePasswordStatus: ResponseStatus | null;
}

export const ForgetPasswordinitialState: ForgetPasswordState = {
  saveStatus: null,
  token: "",
  otpVerificationStatus: null,
  savePasswordStatus: null
};
export const ForgetPasswordReducer = createReducer(
    ForgetPasswordinitialState,
  on(ForgetPasswordActions.forgetPasswordSendOTP, (state) => ({
    ...state,
    saveStatus: null,
  })),
  on(ForgetPasswordActions.forgetPasswordSendOTPSuccess, (state, { responce }) => ({
    ...state,
    saveStatus: { Status: responce.status, message: responce.message },
  })),
  on(ForgetPasswordActions.forgetPasswordVerifyOTPSuccess, (state, { responce }) => ({
    ...state,
    otpVerificationStatus: { Status: responce.status, message: responce.message },
    token: responce.data
  })),
  on(ForgetPasswordActions.saveNewPasswordSuccess, (state, { responce }) => ({
    ...state,
    savePasswordStatus: { Status: responce.status, message: responce.message }
  }))
);
