import { Action, createReducer, on } from '@ngrx/store';
import { ResponseStatus } from 'src/app/_models/utils';
import * as WorkLogActions from '../state/worklog.actions';
import { GOMasterViewModel } from 'src/app/_models/go/tender';

export const WorkLogStateKey = 'WorkLogState';

export interface WorkLogState {
  workLog: GOMasterViewModel[];
  saveStatus: ResponseStatus | null;
  totalRecordCount: number;
}

export const WorkLoginitialState: WorkLogState = {
  workLog: [],
  saveStatus: null,
  totalRecordCount: 0,
};
export const WorkLogReducer = createReducer(
  WorkLoginitialState,
  on(WorkLogActions.workLogListGetSuccess, (state, { workLogs }) => ({
    ...state,
    workLog: workLogs.data,
    totalRecordCount: workLogs.totalRecordCount,
  })),
  on(WorkLogActions.saveworkLogSuccess, (state, { workLog }) => ({
    ...state,
    saveStatus: { Status: workLog.status, message: workLog.message },
  })),
  on(WorkLogActions.updatesaveStatus, (state) => ({
    ...state,
    saveStatus: null,
  })),
  on(WorkLogActions.reset, (state) => ({
    ...state,
    workLog: [],
    saveStatus: null,
    totalRecordCount: 0,
  }))
);
