import { Store } from '@ngrx/store';
import * as TwoColActions from '../actions/two.col.actions';
import * as TwoColSelectors from '../selectors/two.col.selectors';
import { Injectable } from '@angular/core';
import { IConfigurationModel } from 'src/app/_models/configuration/configuration';

@Injectable()
export class TwoColConfigFacade {
  constructor(private store: Store) {}

  selectCategories$ = this.store.select(TwoColSelectors.selectCategories);
  selectDepartments$ = this.store.select(TwoColSelectors.selectDepartments);
  selectSaveStatus$ = this.store.select(TwoColSelectors.selectSaveStatus);
  selectParentconfigurations$ = this.store.select(
    TwoColSelectors.selectParentconfigurations
  );
  selectConfigurationList$ = this.store.select(
    TwoColSelectors.selectConfigurationList
  );

  getCategories() {
    this.store.dispatch(TwoColActions.getCategoryList());
  }
  getDepartments() {
    this.store.dispatch(TwoColActions.getDepartmentList());
  }
  updatesaveStatus() {
    this.store.dispatch(TwoColActions.updatesaveStatus());
  }
  getConfigurationList(
    departmentId: string,
    configId?: string,
    categoryId?: string,
    parentConfigId?: string,
    isActive?: boolean
  ) {
    this.store.dispatch(
      TwoColActions.getConfigurationDetailsbyId({
        departmentId,
        configId,
        categoryId,
        parentConfigId,
        isActive,
      })
    );
  }
  getParentConfigurationList(
    departmentId: string,
    configId?: string,
    categoryId?: string,
    parentConfigId?: string
  ) {
    this.store.dispatch(
      TwoColActions.getParentConfigurationDetailsbyId({
        departmentId,
        configId,
        categoryId,
        parentConfigId,
      })
    );
  }
  saveConfiguration(configDetail: IConfigurationModel) {
    this.store.dispatch(
      TwoColActions.saveConfiguration({ configDetails: configDetail })
    );
  }

  reset() {
    this.store.dispatch(TwoColActions.Reset());
  }
}
