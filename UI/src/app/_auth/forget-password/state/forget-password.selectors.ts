import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ForgetPasswordState, ForgetPasswordStateKey } from './forget-password.reducers';

export const selectForgetPasswordState =
    createFeatureSelector<ForgetPasswordState>(ForgetPasswordStateKey);

export const getSentOtpStatus = createSelector(
    selectForgetPasswordState,
    (state) => state.saveStatus
);

export const getToken = createSelector(
    selectForgetPasswordState,
    (state) => state.token
);

export const getOtpVerificationStatus = createSelector(
    selectForgetPasswordState,
    (state) => state.otpVerificationStatus
);

export const saveNewPasswordStatus = createSelector(
    selectForgetPasswordState,
    (state) => state.savePasswordStatus
);


