import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as userActions from '../state/user.actions';
import { IConfigCategoryModel } from 'src/app/_models/configuration/configuration';
import { UserService } from 'src/app/_services/user.service';
import { RoleService } from 'src/app/_services/role.service';
import { ResponseModel } from 'src/app/_models/ResponseStatus';

@Injectable()
export class userEffects {
  getUsers$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(userActions.userListGet),
      exhaustMap(({ isActive, userId }) =>
        this.userService.getUsers(isActive, userId).pipe(
          map((data: ResponseModel) =>
            userActions.userListGetSuccess({ users: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getUsersList$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(userActions.userListGetServerPaging),
      exhaustMap(({ filtermodel }) =>
        this.userService.getUsersList(filtermodel).pipe(
          map((data: ResponseModel) =>
            userActions.userListGetSuccess({ users: data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  saveuser$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(userActions.saveuser),
      exhaustMap(({ user }) =>
        this.userService.saveUser(user).pipe(
          map((data: ResponseModel) =>
            userActions.saveuserSuccess({ user: data })
          ),
          catchError((error) => {
            return of();
          })
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  sendEmail$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(userActions.sendMail),
      exhaustMap(({ user }) =>
        this.userService.saveUser(user).pipe(
          map((data: ResponseModel) =>
            userActions.sendMailSuccess({ user: data })
          ),
          catchError((error) => {
            return of();
          })
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });

  getRoles$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(userActions.userRoleListGet),
      exhaustMap(({ isActive }) =>
        this.roleService.getRoles(isActive).pipe(
          map((data: ResponseModel) =>
            userActions.userRoleListGetSuccess({ roles: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  getUserForm$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(userActions.userFormGet),
      exhaustMap(() =>
        this.userService.getUserForm().pipe(
          map((data: ResponseModel) =>
            userActions.userFormGetSuccess({ formsDetails: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  constructor(
    private actions$: Actions,
    private userService: UserService,
    private roleService: RoleService
  ) {}
}
