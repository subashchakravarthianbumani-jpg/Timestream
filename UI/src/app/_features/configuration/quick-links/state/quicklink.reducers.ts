import { Action, createReducer, on } from '@ngrx/store';
import { ResponseStatus } from 'src/app/_models/utils';
import * as QuickLinkActions from '../state/quicklink.actions';
import { QuickLinkModel } from 'src/app/_models/configuration/quickLink';
import { TCModel } from 'src/app/_models/user/usermodel';

export const QuickLinkStateKey = 'QuickLinkState';

export interface QuickLinkState {
  quickLinks: QuickLinkModel[];
  saveStatus: ResponseStatus | null;
  userGroups: TCModel[];
}

export const QuickLinkinitialState: QuickLinkState = {
  quickLinks: [],
  saveStatus: null,
  userGroups: [],
};
export const QuickLinkReducer = createReducer(
  QuickLinkinitialState,
  on(QuickLinkActions.quicklinkListGetSuccess, (state, { quicklinks }) => ({
    ...state,
    quickLinks: quicklinks,
  })),
  on(QuickLinkActions.userGroupListGetSuccess, (state, { userGroups }) => ({
    ...state,
    userGroups: userGroups,
  })),
  on(QuickLinkActions.savequicklinkSuccess, (state, { quicklink }) => ({
    ...state,
    saveStatus: { Status: quicklink.status, message: quicklink.message },
  })),
  on(QuickLinkActions.updatesaveStatus, (state) => ({
    ...state,
    saveStatus: null,
  })),
  on(QuickLinkActions.reset, (state) => ({
    ...state,
    quickLinks: [],
    saveStatus: null,
    userGroups: [],
  }))
);
