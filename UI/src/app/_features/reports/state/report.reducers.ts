import { Action, createReducer, on } from '@ngrx/store';
import { ResponseStatus } from 'src/app/_models/utils';
import * as ReportActions from '../state/report.actions';
import { ReportBreadcrumbModel } from 'src/app/_models/filterRequest';

export const ReportStateKey = 'ReportState';

export interface ReportState {
  list: any[];
  recordCount: number;
  breadcrumbs: ReportBreadcrumbModel[];
}

export const ReportinitialState: ReportState = {
  list: [],
  recordCount: 0,
  breadcrumbs: [],
};
export const ReportReducer = createReducer(
  ReportinitialState,
  on(ReportActions.getReportSuccess, (state, { list }) => ({
    ...state,
    list: list.data,
    recordCount: list.totalRecordCount,
  })
  
),

  on(ReportActions.saveBreadcrumbs, (state, { breadcrumb }) => ({
    ...state,
    breadcrumbs: breadcrumb,
  })),
  on(ReportActions.reset, (state) => ({
    ...state,
    list: [],
    recordCount: 0,
    breadcrumbs: [],
  }))
);
