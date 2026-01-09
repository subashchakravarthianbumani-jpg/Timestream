import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as reportActions from './report.actions';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { ReportsService } from 'src/app/_services/report.service';

@Injectable()
export class ReportsEffects {
  getQuicklinks$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(reportActions.getReports),
      exhaustMap(({ model }) =>
        this.reportService.getreports(model).pipe(
          map((data: ResponseModel) =>
            reportActions.getReportSuccess({ list: data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  wwf$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(reportActions.getMbookReports),
      exhaustMap(({ model }) =>
        this.reportService.getMbookreports(model).pipe(
          map((data: ResponseModel) =>
            reportActions.getReportSuccess({ list: data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getQuics$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(reportActions.getMilestoneReports),
      exhaustMap(({ model }) =>
        this.reportService.getMilesonereports(model).pipe(
          map((data: ResponseModel) =>
            reportActions.getReportSuccess({ list: data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getQuicws$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(reportActions.getGOReports),
      exhaustMap(({ model }) =>
        this.reportService.getGoreports(model).pipe(
          map((data: ResponseModel) =>
            reportActions.getReportSuccess({ list: data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  constructor(
    private actions$: Actions,
    private reportService: ReportsService
  ) {}
}
