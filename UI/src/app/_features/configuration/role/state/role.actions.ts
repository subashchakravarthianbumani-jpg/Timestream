import { createAction, props } from '@ngrx/store';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import {
  IConfigCategoryModel,
  IConfigurationModel,
} from 'src/app/_models/configuration/configuration';
import {
  AccountPrivilegeByGroupModel,
  AccountPrivilegeSaveViewModel,
} from 'src/app/_models/configuration/privilege';
import { IRoleModel } from 'src/app/_models/configuration/role';

export const roleListGet = createAction(
  '[Master Configuration] Get Role List',
  props<{ isActive: boolean }>()
);

export const roleListGetSuccess = createAction(
  '[Master Configuration] Get Role List Success',
  props<{ roles: IRoleModel[] }>()
);

export const saverole = createAction(
  '[Master Configuration]  Save Role ',
  props<{ role: IRoleModel }>()
);
export const saveroleSuccess = createAction(
  '[Master Configuration] Save Role Success',
  props<{ role: ResponseModel }>()
);
export const updatesaveStatus = createAction(
  '[Master Configuration] Save Role Status'
);

export const PrivilegeGet = createAction(
  '[Master Configuration] Get Privilege List',
  props<{ roleId: string }>()
);

export const PrivilegeGetSuccess = createAction(
  '[Master Configuration] Get Privilege List Success',
  props<{ privileges: AccountPrivilegeByGroupModel[] }>()
);

export const savePrivilege = createAction(
  '[Master Configuration]  Save Privilege ',
  props<{ privilege: AccountPrivilegeSaveViewModel }>()
);
export const reset = createAction('[Master Configuration]  Reset ');
export const savePrivilegeSuccess = createAction(
  '[Master Configuration] Save Privilege Success',
  props<{ privilege: ResponseModel }>()
);
export const updatesPrivilegeaveStatus = createAction(
  '[Master Configuration] Save Role Status'
);
