import { createFeatureSelector, createSelector } from '@ngrx/store';
import {
  ApprovalFlowState,
  ApprovalFlowStateKey,
} from './approvalflow.reducers';

export const selectTwoColConfigState =
  createFeatureSelector<ApprovalFlowState>(ApprovalFlowStateKey);

export const selectApprovalFlow = createSelector(
  selectTwoColConfigState,
  (state) => state.ApprovalFlow
);
export const selectSaveStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.saveStatus
);
export const selectRoles = createSelector(
  selectTwoColConfigState,
  (state) => state.Roles
);
export const selectAddRoleStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.AddRoleStatus
);
