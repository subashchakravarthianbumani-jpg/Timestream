import { Action, createReducer, on } from '@ngrx/store';
import { ResponseStatus } from 'src/app/_models/utils';
import * as TenderActions from '../state/tender.actions';
import {
  TenderMasterViewModel,
  WorkMasterViewModel,
} from 'src/app/_models/go/tender';
import { TemplatewithMilestoneViewModel } from 'src/app/_models/configuration/temp-milestone';
import { TemplateViewModel } from 'src/app/_models/configuration/templates';
import { TCModel } from 'src/app/_models/user/usermodel';

export const TenderStateKey = 'TenderState';

export interface TenderState {
  tenders: any[];
  totalRecordCount: number;
  tender: WorkMasterViewModel | null;
  Templates: TemplateViewModel[];
  saveStatus: ResponseStatus | null;
  milestoneSaveStatus: ResponseStatus | null;
  TemplateWithMilestones: any | null;
  WorkTemplateWithMilestones: TemplatewithMilestoneViewModel | null;
  WorkTemplate: any | null;
  Divisions: TCModel[];
  savePercenategStatus: ResponseStatus | null;
}

export const TenderinitialState: TenderState = {
  tenders: [],
  totalRecordCount: 0,
  tender: null,
  Templates: [],
  saveStatus: null,
  milestoneSaveStatus: null,
  TemplateWithMilestones: null,
  WorkTemplate: null,
  WorkTemplateWithMilestones: null,
  Divisions: [],
  savePercenategStatus: null,
};
export const TenderReducer = createReducer(
  TenderinitialState,
  on(TenderActions.tenderListGetSuccess, (state, { tenders }) => ({
    ...state,
    tenders: tenders.data,
    totalRecordCount: tenders.totalRecordCount,
  })),
  on(TenderActions.getTenderByID, (state, { id }) => ({
    ...state,
    tender: null,
  })),
  on(TenderActions.getTenderByIDSuccess, (state, { tender }) => ({
    ...state,
    tender,
  })),

  on(TenderActions.templateListGetSuccess, (state, { templates }) => ({
    ...state,
    Templates: templates,
  })),
  on(
    TenderActions.templateWithMilestoneListGetSuccess,
    (state, { templates }) => ({
      ...state,
      TemplateWithMilestones: templates,
    })
  ),
  on(TenderActions.savetemplateSuccess, (state, { template }) => ({
    ...state,
    saveStatus: {
      Status: template.status,
      message: template.message,
      id: template?.data?.id,
    },
  })),
  on(TenderActions.updatesaveStatus, (state) => ({
    ...state,
    saveStatus: null,
  })),
  on(TenderActions.savetemplateMilestoneSuccess, (state, { milestone }) => ({
    ...state,
    milestoneSaveStatus: {
      Status: milestone.status,
      message: milestone.message,
      id: milestone?.data?.id,
    },
  })),
  on(TenderActions.updatesaveMilestoneStatus, (state) => ({
    ...state,
    milestoneSaveStatus: null,
  })),
  on(TenderActions.WorktemplateGet, (state) => ({
    ...state,
    WorkTemplate: null,
    TemplateWithMilestones: null,
  })),
  on(TenderActions.WorktemplateGetSuccess, (state, { template }) => ({
    ...state,
    WorkTemplate: template,
  })),
  on(TenderActions.divisionListGetSuccess, (state, { divisions }) => ({
    ...state,
    Divisions: divisions,
  })),

  on(TenderActions.savePercentageSuccess, (state, { response }) => ({
    ...state,
    savePercenategStatus: {
      Status: response.status,
      message: response.message,
      id: response.data.id,
    },
  })),
  on(TenderActions.updatesavePercentageStatus, (state) => ({
    ...state,
    savePercenategStatus: null,
  })),
  on(TenderActions.reset, (state) => ({
    ...state,
    tenders: [],
    totalRecordCount: 0,
    tender: null,
    Templates: [],
    saveStatus: null,
    milestoneSaveStatus: null,
    TemplateWithMilestones: null,
    WorkTemplate: null,
    WorkTemplateWithMilestones: null,
    Divisions: [],
    savePercenategStatus: null,
  }))
);
