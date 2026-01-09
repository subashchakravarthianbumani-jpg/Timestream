import { createAction, props } from '@ngrx/store';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { QuickLinkModel } from 'src/app/_models/configuration/quickLink';
import {
  GoReportFilterModel,
  ReportBreadcrumbModel,
  WorkFilterModel,
} from 'src/app/_models/filterRequest';
import { TCModel } from 'src/app/_models/user/usermodel';

export const getReports = createAction(
  '[Reports] Get Reports',
  props<{ model: any; selectedType: string }>()
);
export const getMilestoneReports = createAction(
  '[Reports] Get Reports Milestone',
  props<{ model: WorkFilterModel }>()
);

export const getGOReports = createAction(
  '[Reports] Get Reports GO',
  props<{ model: GoReportFilterModel }>()
);

export const getMbookReports = createAction(
  '[Reports] Get Reports Mbook',
  props<{ model: WorkFilterModel }>()
);

export const getReportSuccess = createAction(
  '[Reports] Get Reports Success',
  props<{ list: ResponseModel }>()
);
export const getBreadcrumbs = createAction('[Reports] Get Breadcrumbs');

export const saveBreadcrumbs = createAction(
  '[Reports] Save Breadcrumb',
  props<{ breadcrumb: ReportBreadcrumbModel[] }>()
);
export const reset = createAction('[Reports] s Reset');
