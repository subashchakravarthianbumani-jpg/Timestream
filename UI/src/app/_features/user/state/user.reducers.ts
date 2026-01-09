import { Action, createReducer, on } from '@ngrx/store';
import { UserService } from 'src/app/_services/user.service';
import {
  IConfigCategoryModel,
  IConfigurationModel,
} from 'src/app/_models/configuration/configuration';
import { ResponseStatus } from 'src/app/_models/utils';
import * as UserActions from '../state/user.actions';
import { AccountPrivilegeByGroupModel } from 'src/app/_models/configuration/privilege';
import {
  AccountUserViewModel,
  TCModel,
  UserFormListModel,
} from 'src/app/_models/user/usermodel';
import { IRoleModel } from 'src/app/_models/configuration/role';
import { ResponseModel } from 'src/app/_models/ResponseStatus';

export const UserStateKey = 'UserState';

export interface UserState {
  users: ResponseModel | null;
  saveStatus: ResponseStatus | null;
  emailStatus: ResponseStatus | null;
  districtList: TCModel[];
  divisionList: TCModel[];
  roleList: IRoleModel[];
  userGroupList: TCModel[];
  departmentList: TCModel[];
}

export const UserinitialState: UserState = {
  users: null,
  saveStatus: null,
  emailStatus: null,
  districtList: [],
  divisionList: [],
  roleList: [],
  userGroupList: [],
  departmentList: [],
};
export const UserReducer = createReducer(
  UserinitialState,
  on(UserActions.userListGetSuccess, (state, { users }) => ({
    ...state,
    users,
  })),
  on(UserActions.userListGet, (state, {}) => ({
    ...state,
    saveStatus: null,
  })),
  on(UserActions.saveuser, (state, { user }) => ({
    ...state,
    saveStatus: null,
  })),
  on(UserActions.saveuserSuccess, (state, { user }) => ({
    ...state,
    saveStatus: { Status: user.status, message: user.message },
  })),
  on(UserActions.sendMail, (state, { user }) => ({
    ...state,
    emailStatus: null,
  })),
  on(UserActions.sendMailSuccess, (state, { user }) => ({
    ...state,
    emailStatus: { Status: user.status, message: user.message },
  })),
  on(UserActions.updatesaveStatus, (state) => ({
    ...state,
    saveStatus: null,
  })),
  on(UserActions.userFormGetSuccess, (state, { formsDetails }) => ({
    ...state,
    districtList: formsDetails.districtList,
    divisionList: formsDetails.divisionList,
    userGroupList: formsDetails.userGroupList,
    departmentList: formsDetails.departmentList,
  })),
  on(UserActions.userRoleListGetSuccess, (state, { roles }) => ({
    ...state,
    roleList: roles,
  })),
  on(UserActions.reset, (state) => ({
    ...state,
    users: null,
    saveStatus: null,
    emailStatus: null,
    districtList: [],
    divisionList: [],
    roleList: [],
    userGroupList: [],
    departmentList: [],
  }))
);
