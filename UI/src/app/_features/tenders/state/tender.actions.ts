import { createAction, props } from '@ngrx/store';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import {
  TemplateMilestoneModel,
  TemplatewithMilestoneViewModel,
} from 'src/app/_models/configuration/temp-milestone';
import { TemplateViewModel } from 'src/app/_models/configuration/templates';
import { TenderFilterModel } from 'src/app/_models/filterRequest';
import {
  TenderMasterViewModel,
  UpdatePercentageModel,
  WorkMasterViewModel,
} from 'src/app/_models/go/tender';
import { TCModel } from 'src/app/_models/user/usermodel';

export const tenderListGet = createAction(
  '[Tender] Get Work List',
  props<{ filterRequest: TenderFilterModel }>()
);

export const tenderListGetSuccess = createAction(
  '[Tender] Get Work List Success',
  props<{ tenders: ResponseModel }>()
);

export const getTenderByID = createAction(
  '[Tender] Get Work By ID',
  props<{ id: string }>()
);

export const getTenderByIDSuccess = createAction(
  '[Tender] Get Work By ID Success',
  props<{ tender: WorkMasterViewModel }>()
);

export const templateListGet = createAction(
  '[Tender] Get Template List',
  props<{ isActive: boolean ; subcategory?: string,mainCategory?: string,serviceType?:string,categoryType?:string}>()
);

export const templateListGetSuccess = createAction(
  '[Tender] Get Template List Success',
  props<{ templates: TemplateViewModel[] }>()
);

export const templateWithMilestoneListGet = createAction(
  '[Tender] Get Template With Milestone List',
  props<{
    IsActive_template?: boolean;
    IsActive_milestone?: boolean;
    Id: string;
    WorkTypeId?: string;
  }>()
);
export const templateWithMilestoneListGetSuccess = createAction(
  '[Tender] Get Template With Milestone List Success',
  props<{ templates: TemplatewithMilestoneViewModel }>()
);
export const savetemplate = createAction(
  '[Tender]  Save Template ',
  props<{ id: string; TemplateID: string }>()
);
export const savetemplateSuccess = createAction(
  '[Tender] Save Template Success',
  props<{ template: ResponseModel }>()
);
export const updatesaveStatus = createAction('[Tender] Save Template Status');
export const savetemplateMilestone = createAction(
  '[Tender]  Save Template Milestone',
  props<{ milestone: TemplateMilestoneModel[] }>()
);
export const savetemplateMilestoneSuccess = createAction(
  '[Tender] Save Template Milestone Success',
  props<{ milestone: ResponseModel }>()
);
export const updatesaveMilestoneStatus = createAction(
  '[Tender] Save Template Milestone Status'
);

export const WorktemplateWithMilestoneListGet = createAction(
  '[Tender] Get Work Template With Milestone List',
  props<{
    IsActive?: boolean;
    Id: string;
    WorkTemplateId?: string;
    WorkId?: string;
  }>()
);
export const WorktemplateWithMilestoneListGetSuccess = createAction(
  '[Tender] Get Work Template With Milestone List Success',
  props<{ templates: TemplateMilestoneModel[] }>()
);
export const WorktemplateGet = createAction(
  '[Tender] Get Work Template ',
  props<{ WorkId: string }>()
);

export const WorktemplateGetSuccess = createAction(
  '[Tender] Get Work Template  Success',
  props<{ template: TemplatewithMilestoneViewModel }>()
);

export const divisionListGet = createAction('[Tender] Get division List');

export const divisionListGetSuccess = createAction(
  '[Tender] Get division List Success',
  props<{ divisions: TCModel[] }>()
);

export const savePercentage = createAction(
  '[Tender] Save Percentage',
  props<{ model: UpdatePercentageModel }>()
);
export const savePercentageSuccess = createAction(
  '[Tender] Save Percentage Success',
  props<{ response: ResponseModel }>()
);
export const updatesavePercentageStatus = createAction(
  '[Tender] Update Save Percentage Status'
);
export const reset = createAction('[Tender] s Reset');
