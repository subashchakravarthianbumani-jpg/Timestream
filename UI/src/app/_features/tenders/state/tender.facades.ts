import { Store } from '@ngrx/store';
import * as TenderActions from '../state/tender.actions';
import * as TenderSelectors from './tender.selectors';
import { Injectable } from '@angular/core';
import { TenderFilterModel } from 'src/app/_models/filterRequest';
import { TemplateViewModel } from 'src/app/_models/configuration/templates';
import { TemplateMilestoneModel } from 'src/app/_models/configuration/temp-milestone';
import { UpdatePercentageModel } from 'src/app/_models/go/tender';

@Injectable()
export class TenderFacade {
  constructor(private store: Store) {}

  selectTenders$ = this.store.select(TenderSelectors.selectTenders);
  selectTender$ = this.store.select(TenderSelectors.selectTender);

  selectTemplates$ = this.store.select(TenderSelectors.selectTemplates);
  selectTemplateWithMilestones$ = this.store.select(
    TenderSelectors.selectTemplateWithMilestones
  );
  selectSaveStatus$ = this.store.select(TenderSelectors.selectSaveStatus);

  selectWorkTemplate$ = this.store.select(TenderSelectors.selectWorkTemplates);
  selecttWorkTemplateWithMilestones$ = this.store.select(
    TenderSelectors.selecttWorkTemplateWithMilestones
  );
  selectmilestoneSaveStatus$ = this.store.select(
    TenderSelectors.selectmilestoneSaveStatus
  );
  selectsavePercenategStatus$ = this.store.select(
    TenderSelectors.selectsavePercenategStatus
  );

  selectDivisions$ = this.store.select(TenderSelectors.selectDivisions);

  getTenders(filterRequest: TenderFilterModel) {
    this.store.dispatch(TenderActions.tenderListGet({ filterRequest }));
  }
  getTenderById(id: string) {
    this.store.dispatch(TenderActions.getTenderByID({ id }));
  }

  getTemplates(isActive: boolean,subcategory?: string,mainCategory?: string,serviceType?:string,categoryType?:string) {
   
    this.store.dispatch(TenderActions.templateListGet({ isActive,subcategory,mainCategory,serviceType,categoryType }));
  }
  templateWithMilestoneListGet(
    Id: string,
    IsActive_milestone: boolean = true,
    IsActive_template: boolean = true,
    WorkTypeId?: string
  ) {
    this.store.dispatch(
      TenderActions.templateWithMilestoneListGet({
        IsActive_template,
        IsActive_milestone,
        Id,
        WorkTypeId,
      })
    );
  }
  updatesaveStatus() {
    this.store.dispatch(TenderActions.updatesaveStatus());
  }
  saveTemplates(id: string, TemplateID: string) {
    this.store.dispatch(
      TenderActions.savetemplate({ id: id, TemplateID: TemplateID })
    );
  }
  updatemilestonessaveStatus() {
    this.store.dispatch(TenderActions.updatesaveMilestoneStatus());
  }
  savemilestones(milestone: TemplateMilestoneModel[]) {
    this.store.dispatch(
      TenderActions.savetemplateMilestone({ milestone: milestone })
    );
  }
  getWorktemplate(WorkId: string) {
    this.store.dispatch(TenderActions.WorktemplateGet({ WorkId }));
  }
  WorktemplateWithMilestoneListGet(
    Id: string = '',
    IsActive?: boolean,
    WorkTemplateId?: string,
    WorkId?: string
  ) {
    this.store.dispatch(
      TenderActions.WorktemplateWithMilestoneListGet({
        IsActive,
        Id,
        WorkTemplateId,
        WorkId,
      })
    );
  }

  divisionListGet() {
    this.store.dispatch(TenderActions.divisionListGet());
  }

  updatesavePercentageStatus() {
    this.store.dispatch(TenderActions.updatesavePercentageStatus());
  }
  savePercentage(model: UpdatePercentageModel) {
    this.store.dispatch(TenderActions.savePercentage({ model }));
  }
  reset() {
    this.store.dispatch(TenderActions.reset());
  }
}
