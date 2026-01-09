import { Action, createReducer, on } from '@ngrx/store';
import { ResponseStatus } from 'src/app/_models/utils';
import * as ApprovalFlowActions from '../state/approvalflow.actions';
import { TCModel } from 'src/app/_models/user/usermodel';
import { ApprovalFlowModel } from 'src/app/_models/configuration/approval.flow';

export const ApprovalFlowStateKey = 'ApprovalFlowState';

export interface ApprovalFlowState {
  ApprovalFlow: ApprovalFlowModel[];
  saveStatus: ResponseStatus | null;
  AddRoleStatus: ResponseStatus | null;
  Roles: TCModel[];
}

export const ApprovalFlowinitialState: ApprovalFlowState = {
  ApprovalFlow: [],
  saveStatus: null,
  Roles: [],
  AddRoleStatus: null,
};
export const ApprovalFlowReducer = createReducer(
  ApprovalFlowinitialState,
  on(ApprovalFlowActions.approvalFlowGetSuccess, (state, { approvalFlow }) => ({
    ...state,
    ApprovalFlow: approvalFlow,
  })),
  on(
    ApprovalFlowActions.approvalFlowRoleListGetSuccess,
    (state, { roles }) => ({
      ...state,
      Roles: roles,
    })
  ),
  on(
    ApprovalFlowActions.saveapprovalFlowSuccess,
    (state, { approvalFlow }) => ({
      ...state,
      saveStatus: {
        Status: approvalFlow.status,
        message: approvalFlow.message,
      },
    })
  ),
  on(ApprovalFlowActions.updatesaveStatus, (state) => ({
    ...state,
    saveStatus: null,
  })),
  on(ApprovalFlowActions.saveaddRoleSuccess, (state, { adddedrole }) => ({
    ...state,
    saveStatus: { Status: adddedrole.status, message: adddedrole.message },
  })),
  on(ApprovalFlowActions.updateaddRoleStatus, (state) => ({
    ...state,
    milestoneSaveStatus: null,
  })),
  on(ApprovalFlowActions.reset, (state) => ({
    ...state,
    ApprovalFlow: [],
    saveStatus: null,
    Roles: [],
    AddRoleStatus: null,
  }))
);
