import { Action, createReducer, on } from '@ngrx/store';
import * as TwoColActions from '../actions/two.col.actions';
import {
  IConfigCategoryModel,
  IConfigurationModel,
} from 'src/app/_models/configuration/configuration';
import { ResponseStatus } from 'src/app/_models/utils';
import { TCModel } from 'src/app/_models/user/usermodel';

export const TwoColConfigStateKey = 'TwoColConfigState';

export interface TwoColConfigState {
  categories: IConfigCategoryModel[];
  departments: TCModel[];
  configurations: IConfigurationModel[];
  Parentconfigurations: IConfigurationModel[];
  saveStatus: ResponseStatus | null;
}

export const twoColConfiginitialState: TwoColConfigState = {
  categories: [],
  departments: [],
  configurations: [],
  Parentconfigurations: [],
  saveStatus: null,
};
export const twoColConfigReducer = createReducer(
  twoColConfiginitialState,
  on(TwoColActions.getCategoryListSuccess, (state, { categories }) => ({
    ...state,
    categories,
  })),
  on(TwoColActions.getDepartmentListSuccess, (state, { departments }) => ({
    ...state,
    departments,
  })),
  on(
    TwoColActions.getConfigurationDetailsbyIdSuccess,
    (state, { configDetail }) => ({
      ...state,
      configurations: configDetail,
    })
  ),
  on(
    TwoColActions.getParentConfigurationDetailsbyIdSuccess,
    (state, { configDetail }) => ({
      ...state,
      Parentconfigurations: configDetail,
    })
  ),
  on(TwoColActions.saveConfigurationSuccess, (state, { details }) => ({
    ...state,
    saveStatus: { Status: details.status, message: details.message },
  })),
  on(TwoColActions.updatesaveStatus, (state) => ({
    ...state,
    saveStatus: null,
  })),
  on(TwoColActions.Reset, (state) => ({
    ...state,
    categories: [],
    departments: [],
    configurations: [],
    Parentconfigurations: [],
    saveStatus: null,
  }))
);
