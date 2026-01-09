import { createFeatureSelector, createSelector } from '@ngrx/store';
import {
  TwoColConfigState,
  TwoColConfigStateKey,
} from '../reducers/two.col.reducers';

export const selectTwoColConfigState =
  createFeatureSelector<TwoColConfigState>(TwoColConfigStateKey);

export const selectCategories = createSelector(
  selectTwoColConfigState,
  (state) => state.categories
);
export const selectConfigurationList = createSelector(
  selectTwoColConfigState,
  (state) => state.configurations
);
export const selectSaveStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.saveStatus
);
export const selectParentconfigurations = createSelector(
  selectTwoColConfigState,
  (state) => state.Parentconfigurations
);
export const selectDepartments = createSelector(
  selectTwoColConfigState,
  (state) => state.departments
);
