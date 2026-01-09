import { createAction, props } from '@ngrx/store';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { QuickLinkModel } from 'src/app/_models/configuration/quickLink';

export const forgetPasswordSendOTP = createAction(
  '[Forget Password] Send OTP',
  props<{ mobileNumber: string }>()
);

export const forgetPasswordSendOTPSuccess = createAction(
  '[Forget Password] Send OTP Success',
  props<{ responce: ResponseModel }>()
);

export const forgetPasswordVerifyOTP = createAction(
  '[Forget Password] Verify OTP',
  props<{ mobileNumber: string; otp: string }>()
);

export const forgetPasswordVerifyOTPSuccess = createAction(
  '[Forget Password] Verify OTP Success',
  props<{ responce: ResponseModel }>()
);

export const saveNewPassword = createAction(
  '[Forget Password] Save New Password',
  props<{ token: string; otp: string; password: string }>()
);

export const saveNewPasswordSuccess = createAction(
  '[Forget Password] Save New Password Status',
  props<{ responce: ResponseModel }>()
);
