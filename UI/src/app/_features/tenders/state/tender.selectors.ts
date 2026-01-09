import { createFeatureSelector, createSelector } from '@ngrx/store';
import { TenderState, TenderStateKey } from './tender.reducers';

export const selectTwoColConfigState =
  createFeatureSelector<TenderState>(TenderStateKey);

export const selectTenders = createSelector(
  selectTwoColConfigState,
  (state) => ({ tenders: state.tenders, totalRecords: state.totalRecordCount })
);

export const selectTender = createSelector(
  selectTwoColConfigState,
  (state) => state.tender
);

export const selectTemplates = createSelector(
  selectTwoColConfigState,
  (state) => state.Templates
);
export const selectTemplateWithMilestones = createSelector(
  selectTwoColConfigState,
  (state) => state.TemplateWithMilestones
);

export const selectSaveStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.saveStatus
);
export const selectmilestoneSaveStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.milestoneSaveStatus
);
export const selectsavePercenategStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.savePercenategStatus
);

export const selectWorkTemplates = createSelector(
  selectTwoColConfigState,
  (state) => state.WorkTemplate
);
export const selecttWorkTemplateWithMilestones = createSelector(
  selectTwoColConfigState,
  (state) => state.WorkTemplateWithMilestones
);
export const selectDivisions = createSelector(
  selectTwoColConfigState,
  (state) => state.Divisions
);
