import { Store } from '@ngrx/store';
import * as ApprovalFlowActions from '../state/approvalflow.actions';
import * as ApprovalFlowSelectors from './approvalflow.selectors';
import { Injectable } from '@angular/core';
import { TemplateViewModel } from 'src/app/_models/configuration/templates';
import { TemplateMilestoneModel } from 'src/app/_models/configuration/temp-milestone';
import {
  ApprovalFlowAddRoleModel,
  ApprovalFlowModel,
} from 'src/app/_models/configuration/approval.flow';

@Injectable()
export class ApprovalFlowConfigFacade {
  constructor(private store: Store) {}

  selectAddRoleStatus$ = this.store.select(
    ApprovalFlowSelectors.selectAddRoleStatus
  );
  selectSaveStatus$ = this.store.select(ApprovalFlowSelectors.selectSaveStatus);
  selectRoles$ = this.store.select(ApprovalFlowSelectors.selectRoles);
  selectApprovalFlow$ = this.store.select(
    ApprovalFlowSelectors.selectApprovalFlow
  );

  getApprovalFlowRoles(departmentId: string) {
    this.store.dispatch(
      ApprovalFlowActions.approvalFlowRoleListGet({ departmentId })
    );
  }
  approvalFlowGet(departmentId: string) {
    this.store.dispatch(ApprovalFlowActions.approvalFlowGet({ departmentId }));
  }
  updatesaveStatus() {
    this.store.dispatch(ApprovalFlowActions.updatesaveStatus());
  }
  saveApprovalFlow(ApprovalFlow: ApprovalFlowModel[]) {
    this.store.dispatch(
      ApprovalFlowActions.saveapprovalFlow({ approvalFlow: ApprovalFlow })
    );
  }

  addRoleforApprovalFlow(roles: ApprovalFlowAddRoleModel) {
    this.store.dispatch(
      ApprovalFlowActions.addRoleforApprovalFlow({ roles: roles })
    );
  }
  updateaddRoleStatus() {
    this.store.dispatch(ApprovalFlowActions.updateaddRoleStatus());
  }
  reset() {
    this.store.dispatch(ApprovalFlowActions.reset());
  }
}
