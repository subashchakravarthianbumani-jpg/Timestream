import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, exhaustMap, map } from 'rxjs/operators';
import * as roleActions from '../state/role.actions';
import { IConfigCategoryModel } from 'src/app/_models/configuration/configuration';
import { RoleService } from 'src/app/_services/role.service';
import { ResponseModel } from 'src/app/_models/ResponseStatus';

@Injectable()
export class roleEffects {
  getRoles$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(roleActions.roleListGet),
      exhaustMap(({ isActive }) =>
        this.roleService.getRoles(isActive).pipe(
          map((data: ResponseModel) =>
            roleActions.roleListGetSuccess({ roles: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  saverole$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(roleActions.saverole),
      exhaustMap(({ role }) =>
        this.roleService.saveRole(role).pipe(
          map((data: ResponseModel) =>
            roleActions.saveroleSuccess({ role: data })
          ),
          catchError((error) => {
            return of();
          })
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });

  getPrivileages$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(roleActions.PrivilegeGet),
      exhaustMap(({ roleId }) =>
        this.roleService.getPrivileges(roleId).pipe(
          map((data: ResponseModel) =>
            roleActions.PrivilegeGetSuccess({ privileges: data.data })
          ),
          catchError((error) => of())
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  savePrivilegee$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(roleActions.savePrivilege),
      exhaustMap(({ privilege }) =>
        this.roleService.savePrivileges(privilege).pipe(
          map((data: ResponseModel) =>
            roleActions.savePrivilegeSuccess({ privilege: data })
          ),
          catchError((error) => {
            return of();
          })
        )
      )
      // Errors are handled and it is safe to disable resubscription
    );
  });
  constructor(private actions$: Actions, private roleService: RoleService) {}
}
