import { Store } from '@ngrx/store';
import * as RoleActions from '../state/role.actions';
import * as RoleSelectors from '../state/role.selectors';
import { Injectable } from '@angular/core';
import { IConfigurationModel } from 'src/app/_models/configuration/configuration';
import { IRoleModel } from 'src/app/_models/configuration/role';
import { AccountPrivilegeSaveViewModel } from 'src/app/_models/configuration/privilege';

@Injectable()
export class RoleConfigFacade {
  constructor(private store: Store) {}

  selectRoles$ = this.store.select(RoleSelectors.selectRoles);
  selectSaveStatus$ = this.store.select(RoleSelectors.selectSaveStatus);

  selectPrivileges$ = this.store.select(RoleSelectors.selectPrivileges);
  selectSavePrivilegesStatus$ = this.store.select(
    RoleSelectors.selectSavePrivilegesStatus
  );

  getRoles(isActive: boolean) {
    this.store.dispatch(RoleActions.roleListGet({ isActive: isActive }));
  }
  updatesaveStatus() {
    this.store.dispatch(RoleActions.updatesaveStatus());
  }
  saverole(role: IRoleModel) {
    this.store.dispatch(RoleActions.saverole({ role: role }));
  }

  getPrivileges(roleId: string) {
    this.store.dispatch(RoleActions.PrivilegeGet({ roleId }));
  }
  updatesPrivilegeaveStatus() {
    this.store.dispatch(RoleActions.updatesPrivilegeaveStatus());
  }
  savePrivilege(privilege: AccountPrivilegeSaveViewModel) {
    this.store.dispatch(RoleActions.savePrivilege({ privilege: privilege }));
  }
  reset() {
    this.store.dispatch(RoleActions.reset());
  }
}
