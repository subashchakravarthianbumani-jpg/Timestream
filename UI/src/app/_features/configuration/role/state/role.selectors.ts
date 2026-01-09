import { createFeatureSelector, createSelector } from '@ngrx/store';
import { RoleState, RoleStateKey } from '../state/role.reducers';

export const selectTwoColConfigState =
  createFeatureSelector<RoleState>(RoleStateKey);

export const selectRoles = createSelector(
  selectTwoColConfigState,
  (state) => state.roles
);
export const selectSaveStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.saveStatus
);
export const selectPrivileges = createSelector(
  selectTwoColConfigState,
  (state) => state.privileges
);
export const selectSavePrivilegesStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.savePrivilegeStatus
);
