import { createAction, props } from '@ngrx/store';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { GoFilterModel } from 'src/app/_models/filterRequest';
import { GOMasterViewModel } from 'src/app/_models/go/tender';

export const workLogListGet = createAction(
  '[Tender] Get Work Log List',
  props<{ filterModel: GoFilterModel }>()
);

export const workLogListGetSuccess = createAction(
  '[Tender] Get Work Log List Success',
  props<{ workLogs: ResponseModel }>()
);

export const saveworkLog = createAction(
  '[Tender]  Save Work Log ',
  props<{ workLog: GOMasterViewModel }>()
);
export const saveworkLogSuccess = createAction(
  '[Tender] Save Work Log Success',
  props<{ workLog: ResponseModel }>()
);
export const updatesaveStatus = createAction('[Tender] Save Work Log Status');

export const reset = createAction('[Tender] Reset');
