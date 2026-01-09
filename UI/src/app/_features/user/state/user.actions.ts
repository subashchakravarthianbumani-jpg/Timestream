import { createAction, props } from '@ngrx/store';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { IRoleModel } from 'src/app/_models/configuration/role';
import { TableFilterModel } from 'src/app/_models/filterRequest';
import {
  AccountUserViewModel,
  UserFormListModel,
} from 'src/app/_models/user/usermodel';

export const userListGet = createAction(
  '[Master Configuration] Get User List',
  props<{ isActive: boolean; userId?: string }>()
);
export const userListGetServerPaging = createAction(
  '[Master Configuration] Get User List Server',
  props<{ filtermodel: TableFilterModel }>()
);

export const userListGetSuccess = createAction(
  '[Master Configuration] Get User List Success',
  props<{ users: ResponseModel }>()
);

export const saveuser = createAction(
  '[Master Configuration]  Save User ',
  props<{ user: AccountUserViewModel }>()
);
export const saveuserSuccess = createAction(
  '[Master Configuration] Save User Success',
  props<{ user: ResponseModel }>()
);
export const updatesaveStatus = createAction(
  '[Master Configuration] Save User Status'
);

export const userFormGet = createAction('[Master Configuration] Get User Form');

export const userFormGetSuccess = createAction(
  '[Master Configuration] Get User Form Success',
  props<{ formsDetails: UserFormListModel }>()
);

export const userRoleListGet = createAction(
  '[Master Configuration] Get User Role List',
  props<{ isActive: boolean }>()
);

export const reset = createAction('[Master Configuration] reset');

export const userRoleListGetSuccess = createAction(
  '[Master Configuration] Get User Role List Success',
  props<{ roles: IRoleModel[] }>()
);

export const sendMail = createAction(
  '[Master Configuration]  Send Mail ',
  props<{ user: AccountUserViewModel }>()
);
export const sendMailSuccess = createAction(
  '[Master Configuration] Send Mail Success',
  props<{ user: ResponseModel }>()
);
