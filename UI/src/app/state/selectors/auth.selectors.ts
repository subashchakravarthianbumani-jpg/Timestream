import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AuthStateKey, AuthState } from '../reducers/auth.reducers';

export const selectAuthConfigState =
  createFeatureSelector<AuthState>(AuthStateKey);

export const selectuserDetails = createSelector(
  selectAuthConfigState,
  (state) => state.userDetails
);
