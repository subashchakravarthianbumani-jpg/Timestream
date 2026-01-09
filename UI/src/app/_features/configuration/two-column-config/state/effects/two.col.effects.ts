import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as TwoColActions from '../actions/two.col.actions';
import { IConfigCategoryModel } from 'src/app/_models/configuration/configuration';
import { TwoColConfigService } from 'src/app/_services/two.col.service';
import { ResponseModel } from 'src/app/_models/ResponseStatus';

@Injectable()
export class TwoColEffects {
  getCategoryList$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(TwoColActions.getCategoryList),
      exhaustMap(() =>
        this.twoColConfigService.getAllCategory().pipe(
          map((data: ResponseModel) =>
            TwoColActions.getCategoryListSuccess({ categories: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getAllDepartments$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(TwoColActions.getDepartmentList),
      exhaustMap(() =>
        this.twoColConfigService.getAllDepartments().pipe(
          map((data: ResponseModel) =>
            TwoColActions.getDepartmentListSuccess({ departments: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getCategoryDetailsList$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(TwoColActions.getConfigurationDetailsbyId),
      exhaustMap(
        ({ departmentId, configId, categoryId, parentConfigId, isActive }) =>
          this.twoColConfigService
            .getConfigurationDetailsbyId(
              departmentId,
              configId,
              categoryId,
              parentConfigId,
              isActive
            )
            .pipe(
              map((data: ResponseModel) =>
                TwoColActions.getConfigurationDetailsbyIdSuccess({
                  configDetail: data.data,
                })
              ),
              catchError((error) => of())
            )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getParentDetailsList$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(TwoColActions.getParentConfigurationDetailsbyId),
      exhaustMap(({ departmentId, configId, categoryId, parentConfigId }) =>
        this.twoColConfigService
          .getConfigurationDetailsbyId(
            departmentId,
            configId,
            categoryId,
            parentConfigId
          )
          .pipe(
            map((data: ResponseModel) =>
              TwoColActions.getParentConfigurationDetailsbyIdSuccess({
                configDetail: data.data,
              })
            ),
            catchError((error) => of())
          )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  saveConfiguration$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(TwoColActions.saveConfiguration),
      exhaustMap(({ configDetails }) =>
        this.twoColConfigService.saveConfiguration(configDetails).pipe(
          map((data: ResponseModel) =>
            TwoColActions.saveConfigurationSuccess({ details: data })
          ),
          catchError((error) => {
            this.sdf;
            return of();
          })
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  constructor(
    private actions$: Actions,
    private twoColConfigService: TwoColConfigService
  ) {}

  sdf() {}
}
