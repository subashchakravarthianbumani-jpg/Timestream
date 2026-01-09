import { Action, createReducer, on } from '@ngrx/store';
import { RoleService } from 'src/app/_services/role.service';
import {
  IConfigCategoryModel,
  IConfigurationModel,
} from 'src/app/_models/configuration/configuration';
import { ResponseStatus } from 'src/app/_models/utils';
import * as RoleActions from '../state/role.actions';
import { IRoleModel } from 'src/app/_models/configuration/role';
import { AccountPrivilegeByGroupModel } from 'src/app/_models/configuration/privilege';

export const RoleStateKey = 'RoleState';

export interface RoleState {
  roles: IRoleModel[];
  saveStatus: ResponseStatus | null;
  privileges: AccountPrivilegeByGroupModel[];
  savePrivilegeStatus: ResponseStatus | null;
}

export const RoleinitialState: RoleState = {
  roles: [],
  saveStatus: null,
  privileges: [],
  savePrivilegeStatus: null,
};
export const RoleReducer = createReducer(
  RoleinitialState,
  on(RoleActions.roleListGetSuccess, (state, { roles }) => ({
    ...state,
    roles,
  })),
  on(RoleActions.saveroleSuccess, (state, { role }) => ({
    ...state,
    saveStatus: { Status: role.status, message: role.message },
  })),
  on(RoleActions.updatesaveStatus, (state) => ({
    ...state,
    saveStatus: null,
  })),
  on(RoleActions.PrivilegeGetSuccess, (state, { privileges }) => ({
    ...state,
    privileges,
  })),
  on(RoleActions.savePrivilegeSuccess, (state, { privilege }) => ({
    ...state,
    savePrivilegeStatus: {
      Status: privilege.status,
      message: privilege.message,
    },
  })),
  on(RoleActions.updatesPrivilegeaveStatus, (state) => ({
    ...state,
    savePrivilegeStatus: null,
  })),
  on(RoleActions.reset, (state) => ({
    ...state,
    roles: [],
    saveStatus: null,
    privileges: [],
    savePrivilegeStatus: null,
  }))
);
