import { createAction, props } from '@ngrx/store';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import {
  TemplateMilestoneModel,
  TemplatewithMilestoneViewModel,
} from 'src/app/_models/configuration/temp-milestone';
import { TemplateViewModel } from 'src/app/_models/configuration/templates';
import { TCModel } from 'src/app/_models/user/usermodel';

export const templateListGet = createAction(
  '[Master Configuration] Get Template List',
  props<{ isActive: boolean,TemplateId?:string }>()
);

export const templateListGetSuccess = createAction(
  '[Master Configuration] Get Template List Success',
  props<{ templates: TemplateViewModel[] }>()
);

export const workTypeListGet = createAction(
  '[Master Configuration] Get Work Type List'
);
export const serviceTypeListGet = createAction(
  '[Master Configuration] Get Service Type List'
);
export const categoryTypeListGet = createAction(
  '[Master Configuration] Get Category Type List'
);

export const workTypeListGetSuccess = createAction(
  '[Master Configuration] Get Work Type Success',
  props<{ worktypes: TCModel[] }>()
);

export const subworkTypeListGet = createAction(
  '[Master Configuration] Get sub Work Type List',
  props<{ workTypeId: string }>()
);
export const serviceTypeListGetSuccess = createAction(
  '[Master Configuration] Get Service Type Success',
  props<{ servicetypes: TCModel[] }>()
);
export const categoryTypeListGetSuccess = createAction(
  '[Master Configuration] Get Category Type Success',
  props<{ categorytypes: TCModel[] }>()
);



export const subworkTypeListGetSuccess = createAction(
  '[Master Configuration] Get sub Work Type Success',
  props<{ subworktypes: TCModel[] }>()
);

export const savetemplate = createAction(
  '[Master Configuration]  Save Template ',
  props<{ template: TemplateViewModel }>()
);
export const savetemplateSuccess = createAction(
  '[Master Configuration] Save Template Success',
  props<{ template: ResponseModel }>()
);
export const updatesaveStatus = createAction(
  '[Master Configuration] Save Template Status'
);

export const templateWithMilestoneListGet = createAction(
  '[Master Configuration] Get Template With Milestone List',
  props<{
    IsActive_template?: boolean;
    IsActive_milestone?: boolean;
    Id: string;
    WorkTypeId?: string;
  }>()
);

export const templateWithMilestoneListGetSuccess = createAction(
  '[Master Configuration] Get Template With Milestone List Success',
  props<{ templates: TemplatewithMilestoneViewModel }>()
);

export const savemilestones = createAction(
  '[Master Configuration]  Save Milestones ',
  props<{ milestones: TemplateViewModel[] }>()
);
export const savemilestonesSuccess = createAction(
  '[Master Configuration] Save Milestones Success',
  props<{ milestones: ResponseModel }>()
);
export const updatemilestonessaveStatus = createAction(
  '[Master Configuration] Save Milestones Status'
);

export const publishmilestones = createAction(
  '[Master Configuration]  publish Milestones ',
  props<{ id: string }>()
);
export const publishmilestonesSuccess = createAction(
  '[Master Configuration] publish Milestones Success',
  props<{ milestones: ResponseModel }>()
);
export const publishmilestonessaveStatus = createAction(
  '[Master Configuration] publish Milestones Status'
);
export const reset = createAction('[Master Configuration] Reset');
