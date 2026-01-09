import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as templateActions from '../state/template.actions';
import { Templateservice } from 'src/app/_services/templates.service';
import { ResponseModel } from 'src/app/_models/ResponseStatus';

@Injectable()
export class templateEffects {
  gettemplates$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(templateActions.templateListGet),
      exhaustMap(({ isActive }) =>
        this.templateService.getTemplates(isActive).pipe(
          map((data: ResponseModel) =>
            templateActions.templateListGetSuccess({ templates: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getWorkTypes$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(templateActions.workTypeListGet),
      exhaustMap(() =>
        this.templateService.getWorkTypes().pipe(
          map((data: ResponseModel) =>
            templateActions.workTypeListGetSuccess({ worktypes: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getServiceTypes$ = createEffect(() => {
   
    return this.actions$.pipe(
      ofType(templateActions.serviceTypeListGet),
      exhaustMap(() =>
        this.templateService.getServiceTypes().pipe(
          map((data: ResponseModel) =>
            templateActions.serviceTypeListGetSuccess({ servicetypes: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
   getCategoryTypes$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(templateActions.categoryTypeListGet),
      exhaustMap(() =>
        this.templateService.getCategoryTypes().pipe(
          map((data: ResponseModel) =>
            templateActions.categoryTypeListGetSuccess({ categorytypes: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  subworkTypeListGet$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(templateActions.subworkTypeListGet),
      exhaustMap(({ workTypeId }) =>
        this.templateService.getConfigurationDetailsbyId(workTypeId).pipe(
          map((data: ResponseModel) =>
            templateActions.subworkTypeListGetSuccess({
              subworktypes: data.data,
            })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  savetemplate$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(templateActions.savetemplate),
      exhaustMap(({ template }) =>
        this.templateService.saveTemplates(template).pipe(
          map((data: ResponseModel) =>
            templateActions.savetemplateSuccess({ template: data })
          ),
          catchError((error) => {
            return of();
          })
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });

  getTemplateWithMilestone$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(templateActions.templateWithMilestoneListGet),
      exhaustMap(({ IsActive_template, IsActive_milestone, Id, WorkTypeId }) =>
        this.templateService
          .get_Template_With_Milestone(
            Id,
            IsActive_template,
            IsActive_milestone,
            WorkTypeId
          )
          .pipe(
            map((data: ResponseModel) =>
              templateActions.templateWithMilestoneListGetSuccess({
                templates: data.data[0],
              })
            ),
            catchError((error) => of())
          )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  save_Milestone$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(templateActions.savemilestones),
      exhaustMap(({ milestones }) =>
        this.templateService.save_Milestone(milestones).pipe(
          map((data: ResponseModel) =>
            templateActions.savemilestonesSuccess({ milestones: data })
          ),
          catchError((error) => {
            return of();
          })
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  publishMilestone$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(templateActions.publishmilestones),
      exhaustMap(({ id }) =>
        this.templateService.publishTemplate(id).pipe(
          map((data: ResponseModel) =>
            templateActions.publishmilestonesSuccess({ milestones: data })
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
    private templateService: Templateservice
  ) {}
}
