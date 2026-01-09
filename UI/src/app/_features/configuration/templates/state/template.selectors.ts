import { createFeatureSelector, createSelector } from '@ngrx/store';
import { TemplatesState, TemplatesStateKey } from './template.reducers';

export const selectTwoColConfigState =
  createFeatureSelector<TemplatesState>(TemplatesStateKey);

export const selectTemplates = createSelector(
  selectTwoColConfigState,
  (state) => state.Templates
);
export const selectSaveStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.saveStatus
);
export const selectWorkTypes = createSelector(
  selectTwoColConfigState,
  (state) => state.WorkTypes
);
export const selectServiceTypes = createSelector(
  selectTwoColConfigState,
  (state) => state.ServiceTypes
);
export const selectCategoryTypes = createSelector(
  selectTwoColConfigState,
  (state) => state.CategoryTypes
);
export const selectmilestoneSaveStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.milestoneSaveStatus
);
export const selectTemplateWithMilestones = createSelector(
  selectTwoColConfigState,
  (state) => state.TemplateWithMilestones
);
export const selectpublishStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.publishStatus
);
export const selectsubWorkTypes = createSelector(
  selectTwoColConfigState,
  (state) => state.subWorkTypes
);
