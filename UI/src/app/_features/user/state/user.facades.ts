import { Store } from '@ngrx/store';
import * as UserActions from './user.actions';
import * as UserSelectors from '../state/user.selectors';
import { Injectable } from '@angular/core';
import { IConfigurationModel } from 'src/app/_models/configuration/configuration';
import { AccountPrivilegeSaveViewModel } from 'src/app/_models/configuration/privilege';
import { AccountUserViewModel } from 'src/app/_models/user/usermodel';
import { TableFilterModel } from 'src/app/_models/filterRequest';

@Injectable()
export class UserConfigFacade {
  constructor(private store: Store) {}

  selectUsers$ = this.store.select(UserSelectors.selectUsers);
  selectSaveStatus$ = this.store.select(UserSelectors.selectSaveStatus);
  selectEmailStatus$ = this.store.select(UserSelectors.selectEmailStatus);
  selectUserRoles$ = this.store.select(UserSelectors.selectUserRoles);
  selectdistrictList$ = this.store.select(UserSelectors.selectdistrictList);
  selectdivisionList$ = this.store.select(UserSelectors.selectdivisionList);
  selectuserGroupList$ = this.store.select(UserSelectors.selectuserGroupList);
  selectdepartmentList$ = this.store.select(UserSelectors.selectdepartmentList);

  getUsersServerPaging(filtermodel: TableFilterModel) {
    this.store.dispatch(UserActions.userListGetServerPaging({ filtermodel }));
  }
  getUsers(isActive: boolean, userId: string) {
    this.store.dispatch(UserActions.userListGet({ isActive, userId }));
  }
  updatesaveStatus() {
    this.store.dispatch(UserActions.updatesaveStatus());
  }
  saveuser(user: AccountUserViewModel) {
    this.store.dispatch(UserActions.saveuser({ user: user }));
  }
  sendEmail(user: AccountUserViewModel) {
    this.store.dispatch(UserActions.sendMail({ user: user }));
  }
  userFormGet() {
    this.store.dispatch(UserActions.userFormGet());
  }
  getRoles(isActive: boolean) {
    this.store.dispatch(UserActions.userRoleListGet({ isActive }));
  }
  reset() {
    this.store.dispatch(UserActions.reset());
  }
}
