import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as AuthActions from '../actions/auth.actions';
import { IConfigCategoryModel } from 'src/app/_models/configuration/configuration';
import { AuthService } from 'src/app/_services/auth.service';

@Injectable()
export class AuthEffects {
  // authenticateList$ = createEffect(() => {
  //   return this.actions$.pipe(
  //     ofType(AuthActions.Authenticate),
  //     exhaustMap(({ username, password }) =>
  //       this.authService.login(username, password).pipe(
  //         map((data: ResponseModel) => {
  //           localStorage.setItem('user', JSON.stringify(data.data));
  //           localStorage.setItem(
  //             'token',
  //             JSON.stringify(data.data.accessToken)
  //           );
  //           return AuthActions.AutheticateSuccess({ UserModel: data });
  //         }),
  //         catchError((error) => of())
  //       )
  //     )
  //   );
  // });
  authentiscateList$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(AuthActions.Authenticate),
      exhaustMap((action) =>
        this.authService.login(action.username, action.password).pipe(
          map((response: any) =>
            AuthActions.AutheticateSuccess({ UserModel: response.data })
          ),
          catchError((error: any) => of())
        )
      )
    );
  });

  constructor(private actions$: Actions, private authService: AuthService) {}
}
