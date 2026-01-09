import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map, mergeMap } from 'rxjs/operators';
import * as tenderActions from '../state/tender.actions';
import { TenderService } from 'src/app/_services/tender.service';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { Templateservice } from 'src/app/_services/templates.service';
import { TwoColConfigService } from 'src/app/_services/two.col.service';

@Injectable()
export class tenderEffects {
  getQuicklinks$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(tenderActions.tenderListGet),
      mergeMap(({ filterRequest }) =>
        this.tenderService.getTenders(filterRequest).pipe(
          map((data: ResponseModel) =>
            tenderActions.tenderListGetSuccess({ tenders: data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getAllDivisions$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(tenderActions.divisionListGet),
      exhaustMap(({}) =>
        this.twoColConfigService.getAllDivisions().pipe(
          map((data: ResponseModel) =>
            tenderActions.divisionListGetSuccess({ divisions: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  savePercentage$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(tenderActions.savePercentage),
      exhaustMap(({ model }) =>
        this.tenderService.UpdatePercentage(model).pipe(
          map((data: ResponseModel) =>
            tenderActions.savePercentageSuccess({ response: data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getTenderbyID$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(tenderActions.getTenderByID),
      exhaustMap(({ id }) =>
        this.tenderService.getTenderById(id).pipe(
          map((data: ResponseModel) =>
            tenderActions.getTenderByIDSuccess({ tender: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  gettemplates$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(tenderActions.templateListGet),
      exhaustMap(({ isActive,subcategory,mainCategory,serviceType,categoryType }) =>
       
        this.templateService.getTemplates(isActive,subcategory,mainCategory,serviceType,categoryType).pipe(
          map((data: ResponseModel) =>
            tenderActions.templateListGetSuccess({ templates: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });

  getTemplateWithMilestone$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(tenderActions.templateWithMilestoneListGet),
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
              tenderActions.templateWithMilestoneListGetSuccess({
                templates: data.data[0],
              })
            ),
            catchError((error) => of())
          )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  GetWorkTemplate$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(tenderActions.WorktemplateGet),
      exhaustMap(({ WorkId }) =>
        this.tenderService.GetWorkTemplate(WorkId).pipe(
          map((data: ResponseModel) =>
            tenderActions.WorktemplateGetSuccess({
              template: data.data[0],
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
      ofType(tenderActions.savetemplate),
      exhaustMap(({ id, TemplateID }) =>
        this.tenderService.workTemplateCreate(id, TemplateID).pipe(
          map((data: ResponseModel) =>
            tenderActions.savetemplateSuccess({ template: data })
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
      ofType(tenderActions.savetemplateMilestone),
      exhaustMap(({ milestone }) =>
        this.tenderService.SaveWorkTemplateMilestone(milestone).pipe(
          map((data: ResponseModel) =>
            tenderActions.savetemplateMilestoneSuccess({ milestone: data })
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
    private tenderService: TenderService,
    private twoColConfigService: TwoColConfigService,
    private templateService: Templateservice
  ) {}
}
