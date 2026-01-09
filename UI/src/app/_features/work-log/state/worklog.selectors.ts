import { createFeatureSelector, createSelector } from '@ngrx/store';
import { WorkLogState, WorkLogStateKey } from './worklog.reducers';

export const selectTwoColConfigState =
  createFeatureSelector<WorkLogState>(WorkLogStateKey);

export const selectWorkLogs = createSelector(
  selectTwoColConfigState,
  (state) => ({ workLog: state.workLog, totalRecords: state.totalRecordCount })
);
export const selectSaveStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.saveStatus
);
