import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as mbookActions from '../state/mbook.actions';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { MBookService } from 'src/app/_services/mbook.service';
import { TwoColConfigService } from 'src/app/_services/two.col.service';

@Injectable()
export class mbookEffects {
  getmbooks$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(mbookActions.mbookListGet),
      exhaustMap(({ mbookFiletr }) =>
        this.mBookService.getMBooks(mbookFiletr).pipe(
          map((data: ResponseModel) =>
            mbookActions.mbookListGetSuccess({ mbooks: data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getAllDivisions$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(mbookActions.divisionListGet),
      exhaustMap(({}) =>
        this.twoColConfigService.getAllDivisions().pipe(
          map((data: ResponseModel) =>
            mbookActions.divisionListGetSuccess({ divisions: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getmbookbyId$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(mbookActions.mbookbyId),
      exhaustMap(({ id }) =>
        this.mBookService.getMBookbyId(id).pipe(
          map((data: ResponseModel) =>
            mbookActions.mbookbyIdSuccess({ mbook: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getFileTypes$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(mbookActions.getFileTypes),
      exhaustMap(({}) =>
        this.mBookService.getFileTypes().pipe(
          map((data: ResponseModel) =>
            mbookActions.getFileTypesSuccess({ filetypes: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getApprovalTypes$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(mbookActions.getApprovalTypes),
      exhaustMap(({ mbookId }) =>
        this.mBookService.getApprovalTypes(mbookId).pipe(
          map((data: ResponseModel) =>
            mbookActions.getApprovalTypesSuccess({ approvaltypes: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  savembook$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(mbookActions.savembook),
      exhaustMap(({ mbook }) =>
        this.mBookService.saveMbook(mbook).pipe(
          map((data: ResponseModel) =>
            mbookActions.savembookSuccess({ mbook: data })
          ),
          catchError((error) => {
            return of();
          })
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });

  approveMbook$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(mbookActions.approvembook),
      exhaustMap(({ mbook }) =>
        this.mBookService.approveMbook(mbook).pipe(
          map((data: ResponseModel) =>
            mbookActions.approvembookSuccess({ mbook: data })
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
    private mBookService: MBookService,
    private twoColConfigService: TwoColConfigService
  ) {}
}
