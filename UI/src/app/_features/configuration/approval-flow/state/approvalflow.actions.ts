import { createAction, props } from '@ngrx/store';
import { ResponseModel } from 'src/app/_models/ResponseStatus';

import {
  ApprovalFlowAddRoleModel,
  ApprovalFlowModel,
} from 'src/app/_models/configuration/approval.flow';
import { TCModel } from 'src/app/_models/user/usermodel';

export const approvalFlowRoleListGet = createAction(
  '[Approval Flow Configuration] Get Approval Flow Role List',
  props<{ departmentId: string }>()
);

export const approvalFlowRoleListGetSuccess = createAction(
  '[Approval Flow Configuration] Get Approval Flow Role List Success',
  props<{ roles: TCModel[] }>()
);

export const approvalFlowGet = createAction(
  '[Approval Flow Configuration] Get Approval Flow List',
  props<{ departmentId: string }>()
);

export const approvalFlowGetSuccess = createAction(
  '[Approval Flow Configuration] Get Work Type Success',
  props<{ approvalFlow: ApprovalFlowModel[] }>()
);

export const saveapprovalFlow = createAction(
  '[Approval Flow Configuration]  Save Approval Flow ',
  props<{ approvalFlow: ApprovalFlowModel[] }>()
);
export const saveapprovalFlowSuccess = createAction(
  '[Approval Flow Configuration] Save Approval Flow Success',
  props<{ approvalFlow: ResponseModel }>()
);
export const updatesaveStatus = createAction(
  '[Approval Flow Configuration] Save Approval Flow Status'
);

export const addRoleforApprovalFlow = createAction(
  '[Approval Flow Configuration] Get Approval Flow With Milestone List',
  props<{ roles: ApprovalFlowAddRoleModel }>()
);

export const saveaddRoleSuccess = createAction(
  '[Approval Flow Configuration] Save Approval Flow Role Success',
  props<{ adddedrole: ResponseModel }>()
);
export const updateaddRoleStatus = createAction(
  '[Approval Flow Configuration] Save Approval Flow Role Status'
);
export const reset = createAction('[Approval Flow Configuration] Reset');
