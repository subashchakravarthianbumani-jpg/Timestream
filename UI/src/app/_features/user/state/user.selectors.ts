import { createFeatureSelector, createSelector } from '@ngrx/store';
import { UserState, UserStateKey } from './user.reducers';

export const selectTwoColConfigState =
  createFeatureSelector<UserState>(UserStateKey);

export const selectUsers = createSelector(
  selectTwoColConfigState,
  (state) => state.users
);
export const selectSaveStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.saveStatus
);
export const selectEmailStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.emailStatus
);
export const selectUserRoles = createSelector(
  selectTwoColConfigState,
  (state) => state.roleList
);
export const selectdistrictList = createSelector(
  selectTwoColConfigState,
  (state) => state.districtList
);
export const selectdivisionList = createSelector(
  selectTwoColConfigState,
  (state) => state.divisionList
);
export const selectuserGroupList = createSelector(
  selectTwoColConfigState,
  (state) => state.userGroupList
);
export const selectdepartmentList = createSelector(
  selectTwoColConfigState,
  (state) => state.departmentList
);
