import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as quicklinkActions from '../state/quicklink.actions';
import { QuickLinkService } from 'src/app/_services/quicklinks.service';
import { ResponseModel } from 'src/app/_models/ResponseStatus';

@Injectable()
export class quicklinkEffects {
  getQuicklinks$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(quicklinkActions.quicklinkListGet),
      exhaustMap(({ isActive }) =>
        this.quicklinkService.getQuickLinks(isActive).pipe(
          map((data: ResponseModel) =>
            quicklinkActions.quicklinkListGetSuccess({ quicklinks: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getUserGroups$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(quicklinkActions.userGroupListGet),
      exhaustMap(({}) =>
        this.quicklinkService.getUserGroups().pipe(
          map((data: ResponseModel) =>
            quicklinkActions.userGroupListGetSuccess({ userGroups: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  savequicklink$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(quicklinkActions.savequicklink),
      exhaustMap(({ quicklink }) =>
        this.quicklinkService.saveQuickLink(quicklink).pipe(
          map((data: ResponseModel) =>
            quicklinkActions.savequicklinkSuccess({ quicklink: data })
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
    private quicklinkService: QuickLinkService
  ) {}
}
