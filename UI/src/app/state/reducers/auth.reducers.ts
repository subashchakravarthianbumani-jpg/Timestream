import { Action, createReducer, on } from '@ngrx/store';
import * as AuthActions from '../actions/auth.actions';
import { UserModel } from 'src/app/_models/user';

export const AuthStateKey = 'AuthState';

export interface AuthState {
  userDetails: any;
}

export const authinitialState: AuthState = {
  userDetails: null,
};
export const authReducer = createReducer(
  authinitialState,
  on(AuthActions.AutheticateSuccess, (state, { UserModel }) => ({
    ...state,
    userDetails: UserModel,
  }))
);
