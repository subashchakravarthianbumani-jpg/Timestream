import { createAction, props } from '@ngrx/store';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { QuickLinkModel } from 'src/app/_models/configuration/quickLink';
import { TCModel } from 'src/app/_models/user/usermodel';

export const quicklinkListGet = createAction(
  '[Master Configuration] Get Quick Link List',
  props<{ isActive: boolean }>()
);

export const quicklinkListGetSuccess = createAction(
  '[Master Configuration] Get Quick Link List Success',
  props<{ quicklinks: QuickLinkModel[] }>()
);

export const savequicklink = createAction(
  '[Master Configuration]  Save Quick Link ',
  props<{ quicklink: QuickLinkModel }>()
);
export const savequicklinkSuccess = createAction(
  '[Master Configuration] Save Quick Link Success',
  props<{ quicklink: ResponseModel }>()
);
export const updatesaveStatus = createAction(
  '[Master Configuration] Save Quick Link Status'
);

export const userGroupListGet = createAction(
  '[Master Configuration] Get user Group List'
);

export const userGroupListGetSuccess = createAction(
  '[Master Configuration] Get userGroup List Success',
  props<{ userGroups: TCModel[] }>()
);

export const reset = createAction('[Master Configuration] asc Reset ');
