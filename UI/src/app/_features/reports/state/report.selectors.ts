import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ReportState, ReportStateKey } from './report.reducers';

export const selectTwoColConfigState =
  createFeatureSelector<ReportState>(ReportStateKey);

export const selectReports = createSelector(
  selectTwoColConfigState,
  (state) => ({ list: state.list, totalRecords: state.recordCount })
);

export const selectBreadCrumb = createSelector(
  selectTwoColConfigState,
  (state) => state.breadcrumbs
);
