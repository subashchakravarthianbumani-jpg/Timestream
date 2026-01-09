import { createFeatureSelector, createSelector } from '@ngrx/store';
import { QuickLinkState, QuickLinkStateKey } from './quicklink.reducers';

export const selectTwoColConfigState =
  createFeatureSelector<QuickLinkState>(QuickLinkStateKey);

export const selectQuickLinks = createSelector(
  selectTwoColConfigState,
  (state) => state.quickLinks
);
export const selectuserGroups = createSelector(
  selectTwoColConfigState,
  (state) => state.userGroups
);
export const selectSaveStatus = createSelector(
  selectTwoColConfigState,
  (state) => state.saveStatus
);
