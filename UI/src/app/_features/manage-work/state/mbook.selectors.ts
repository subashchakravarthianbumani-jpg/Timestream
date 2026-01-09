import { createFeatureSelector, createSelector } from '@ngrx/store';
import { MBookState, MBookStateKey } from './mbook.reducer';

export const selectTwoColConfigState =
  createFeatureSelector<MBookState>(MBookStateKey);

export const selectMBooks = createSelector(
  selectTwoColConfigState,
  (state) => ({ mbooks: state.mbooks, totalRecords: state.totalRecordCount })
);
export const selectMBookbyId = createSelector(
  selectTwoColConfigState,
  (state) => state.mbook
);
export const selectSaveStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.saveStatus
);

export const selectDivisions = createSelector(
  selectTwoColConfigState,
  (state) => state.Divisions
);

export const selectfiletypes = createSelector(
  selectTwoColConfigState,
  (state) => state.filetypes
);
export const selectapprovaltypes = createSelector(
  selectTwoColConfigState,
  (state) => state.approvaltypes
);
export const selectapprovestatus = createSelector(
  selectTwoColConfigState,
  (state) => state.approvestatus
);
