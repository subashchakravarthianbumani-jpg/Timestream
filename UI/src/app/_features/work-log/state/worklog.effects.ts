import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as workLogActions from '../state/worklog.actions';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { WorkLogService } from 'src/app/_services/workLog.service';

@Injectable()
export class workLogEffects {
  getQuicklinks$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(workLogActions.workLogListGet),
      exhaustMap(({ filterModel }) =>
        this.workLogService.getGos(filterModel).pipe(
          map((data: ResponseModel) =>
            workLogActions.workLogListGetSuccess({ workLogs: data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });

  constructor(
    private actions$: Actions,
    private workLogService: WorkLogService
  ) {}
}
