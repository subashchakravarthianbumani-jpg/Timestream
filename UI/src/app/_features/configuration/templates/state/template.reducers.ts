import { Action, createReducer, on } from '@ngrx/store';
import { ResponseStatus } from 'src/app/_models/utils';
import * as TemplatesActions from '../state/template.actions';
import { TemplateViewModel } from 'src/app/_models/configuration/templates';
import { TCModel } from 'src/app/_models/user/usermodel';
import { TemplatewithMilestoneViewModel } from 'src/app/_models/configuration/temp-milestone';

export const TemplatesStateKey = 'TemplatesState';

export interface TemplatesState {
  Templates: TemplateViewModel[];
  saveStatus: ResponseStatus | null;
  milestoneSaveStatus: ResponseStatus | null;
  publishStatus: ResponseStatus | null;
  WorkTypes: TCModel[];
  subWorkTypes: TCModel[];
  ServiceTypes: TCModel[];
  CategoryTypes:TCModel[];
  TemplateWithMilestones: TemplatewithMilestoneViewModel | null;
}

export const TemplatesinitialState: TemplatesState = {
  Templates: [],
  saveStatus: null,
  WorkTypes: [],
  subWorkTypes: [],
   ServiceTypes: [],
  CategoryTypes: [],
  milestoneSaveStatus: null,
  publishStatus: null,
  TemplateWithMilestones: null,
};
export const TemplatesReducer = createReducer(
  TemplatesinitialState,
  on(TemplatesActions.templateListGetSuccess, (state, { templates }) => ({
    ...state,
    Templates: templates,
  })),
  on(TemplatesActions.workTypeListGetSuccess, (state, { worktypes }) => ({
    ...state,
    WorkTypes: worktypes,
  })),
  on(TemplatesActions.serviceTypeListGetSuccess, (state, { servicetypes }) => ({
    ...state,
    ServiceTypes: servicetypes,
  })),
  on(TemplatesActions.categoryTypeListGetSuccess, (state, { categorytypes }) => ({
    ...state,
    CategoryTypes: categorytypes,
  })),
  on(TemplatesActions.subworkTypeListGetSuccess, (state, { subworktypes }) => ({
    ...state,
    subWorkTypes: subworktypes,
  })),
  on(TemplatesActions.savetemplateSuccess, (state, { template }) => ({
    ...state,
    saveStatus: { Status: template.status, message: template.message },
  })),
  on(TemplatesActions.updatesaveStatus, (state) => ({
    ...state,
    saveStatus: null,
  })),

  on(
    TemplatesActions.templateWithMilestoneListGetSuccess,
    (state, { templates }) => ({
      ...state,
      TemplateWithMilestones: templates,
    })
  ),
  on(TemplatesActions.savemilestonesSuccess, (state, { milestones }) => ({
    ...state,
    milestoneSaveStatus: {
      Status: milestones.status,
      message: milestones.message,
    },
  })),
  on(TemplatesActions.updatemilestonessaveStatus, (state) => ({
    ...state,
    milestoneSaveStatus: null,
  })),
  on(TemplatesActions.publishmilestonesSuccess, (state, { milestones }) => ({
    ...state,
    publishStatus: { Status: milestones.status, message: milestones.message },
  })),
  on(TemplatesActions.publishmilestonessaveStatus, (state) => ({
    ...state,
    publishStatus: null,
  })),
  on(TemplatesActions.reset, (state) => ({
    ...state,
    Templates: [],
    saveStatus: null,
    WorkTypes: [],
    subWorkTypes: [],
    ServiceTypes: [],
    CategoryTypes: [],
    milestoneSaveStatus: null,
    publishStatus: null,
    TemplateWithMilestones: null,
  }))
);
