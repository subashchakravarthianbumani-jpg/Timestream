import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as forgetPasswordActions from '../state/forget-password.action';
import { ForgetPasswordService } from 'src/app/_services/forget-password.service';
import { ResponseModel } from 'src/app/_models/ResponseStatus';

@Injectable()
export class quicklinkEffects {
  sendForgetPasswordOtp$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(forgetPasswordActions.forgetPasswordSendOTP),
      exhaustMap(({ mobileNumber }) =>
        this.forgetPasswordService.sendOTP(mobileNumber).pipe(
          map((data: ResponseModel) =>
            forgetPasswordActions.forgetPasswordSendOTPSuccess({
              responce: data,
            })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });

  verifyForgetPasswordOtp$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(forgetPasswordActions.forgetPasswordVerifyOTP),
      exhaustMap(({ mobileNumber, otp }) =>
        this.forgetPasswordService.vaerifyOtp(mobileNumber, otp).pipe(
          map((data: ResponseModel) =>
            forgetPasswordActions.forgetPasswordVerifyOTPSuccess({
              responce: data,
            })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });

  saveNewPassword$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(forgetPasswordActions.saveNewPassword),
      exhaustMap(({ token, otp, password }) =>
        this.forgetPasswordService.savePasword(token, otp, password).pipe(
          map((data: ResponseModel) =>
            forgetPasswordActions.saveNewPasswordSuccess({ responce: data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });

  constructor(
    private actions$: Actions,
    private forgetPasswordService: ForgetPasswordService
  ) {}
}
