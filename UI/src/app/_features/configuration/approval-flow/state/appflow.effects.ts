import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as ApprovalFlowActions from '../state/approvalflow.actions';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { ApprovalFlowservice } from 'src/app/_services/approval-flow.services';

@Injectable()
export class approvalFlowEffects {
  getApprovalFlows$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(ApprovalFlowActions.approvalFlowGet),
      exhaustMap(({ departmentId }) =>
        this.ApprovalFlowService.getApprovalFlows(departmentId).pipe(
          map((data: ResponseModel) =>
            ApprovalFlowActions.approvalFlowGetSuccess({
              approvalFlow: data.data,
            })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  approvalFlowRoleListGet$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(ApprovalFlowActions.approvalFlowRoleListGet),
      exhaustMap(({ departmentId }) =>
        this.ApprovalFlowService.getRoles(departmentId).pipe(
          map((data: ResponseModel) =>
            ApprovalFlowActions.approvalFlowRoleListGetSuccess({
              roles: data.data,
            })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  saveApprovalFlow$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(ApprovalFlowActions.saveapprovalFlow),
      exhaustMap(({ approvalFlow }) =>
        this.ApprovalFlowService.saveApprovalFlows(approvalFlow).pipe(
          map((data: ResponseModel) =>
            ApprovalFlowActions.saveapprovalFlowSuccess({ approvalFlow: data })
          ),
          catchError((error) => {
            return of();
          })
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });

  save_Milestone$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(ApprovalFlowActions.addRoleforApprovalFlow),
      exhaustMap(({ roles }) =>
        this.ApprovalFlowService.save_roles(roles).pipe(
          map((data: ResponseModel) =>
            ApprovalFlowActions.saveaddRoleSuccess({ adddedrole: data })
          ),
          catchError((error) => {
            return of();
          })
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  constructor(
    private actions$: Actions,
    private ApprovalFlowService: ApprovalFlowservice
  ) {}
}
