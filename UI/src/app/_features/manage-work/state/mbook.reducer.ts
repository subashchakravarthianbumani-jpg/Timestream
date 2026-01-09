import { Action, createReducer, on } from '@ngrx/store';
import { ResponseStatus } from 'src/app/_models/utils';
import * as MBookActions from '../state/mbook.actions';
import { MBookMasterViewModel } from 'src/app/_models/mbook/mbook';
import { TCModel } from 'src/app/_models/user/usermodel';

export const MBookStateKey = 'MBookState';

export interface MBookState {
  mbooks: MBookMasterViewModel[];
  mbook: MBookMasterViewModel | null;
  totalRecordCount: number;
  saveStatus: ResponseStatus | null;
  Divisions: TCModel[];
  approvaltypes: TCModel[];
  filetypes: TCModel[];
  approvestatus: ResponseStatus | null;
}

export const MBookinitialState: MBookState = {
  mbooks: [],
  mbook: null,
  totalRecordCount: 0,
  saveStatus: null,
  Divisions: [],
  approvaltypes: [],
  filetypes: [],
  approvestatus: null,
};
export const MBookReducer = createReducer(
  MBookinitialState,
  on(MBookActions.mbookListGetSuccess, (state, { mbooks }) => ({
    ...state,
    mbooks: mbooks.data,
    totalRecordCount: mbooks.totalRecordCount,
  })),
  on(MBookActions.savembookSuccess, (state, { mbook }) => ({
    ...state,
    saveStatus: { Status: mbook.status, message: mbook.message },
  })),
  on(MBookActions.mbookbyIdSuccess, (state, { mbook }) => ({
    ...state,
    mbook,
  })),
  on(MBookActions.updatesaveStatus, (state) => ({
    ...state,
    saveStatus: null,
  })),
  on(MBookActions.divisionListGetSuccess, (state, { divisions }) => ({
    ...state,
    Divisions: divisions,
  })),
  on(MBookActions.getApprovalTypesSuccess, (state, { approvaltypes }) => ({
    ...state,
    approvaltypes,
  })),
  on(MBookActions.getFileTypesSuccess, (state, { filetypes }) => ({
    ...state,
    filetypes,
  })),
  on(MBookActions.approvembookSuccess, (state, { mbook }) => ({
    ...state,
    approvestatus: { Status: mbook.status, message: mbook.message },
  })),
  on(MBookActions.updateapprovetatus, (state, {}) => ({
    ...state,
    approvestatus: null,
  })),
  on(MBookActions.reset, (state, {}) => ({
    ...state,
    mbooks: [],
    mbook: null,
    totalRecordCount: 0,
    saveStatus: null,
    Divisions: [],
    approvaltypes: [],
    filetypes: [],
    approvestatus: null,
  }))
);
