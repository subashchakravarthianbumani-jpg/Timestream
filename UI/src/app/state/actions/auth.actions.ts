import { createAction, props } from '@ngrx/store';
export const getUserDetails = createAction('[Auth] Get User Details');
export const Authenticate = createAction(
  '[Auth] Authenticate',
  props<{ username: string; password: string }>()
);
export const AutheticateSuccess = createAction(
  '[Auth] Authenticate Success',
  props<{ UserModel: any }>()
);
