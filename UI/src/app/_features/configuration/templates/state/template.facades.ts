import { Store } from '@ngrx/store';
import * as TemplatesActions from '../state/template.actions';
import * as TemplatesSelectors from './template.selectors';
import { Injectable } from '@angular/core';
import { TemplateViewModel } from 'src/app/_models/configuration/templates';
import { TemplateMilestoneModel } from 'src/app/_models/configuration/temp-milestone';

@Injectable()
export class TemplatesConfigFacade {
  constructor(private store: Store) {}

  selectTemplates$ = this.store.select(TemplatesSelectors.selectTemplates);
  selectSaveStatus$ = this.store.select(TemplatesSelectors.selectSaveStatus);
  selectWorkTypes$ = this.store.select(TemplatesSelectors.selectWorkTypes);
   selectServiceTypes$ = this.store.select(TemplatesSelectors.selectServiceTypes);
   selectCategoryTypes$ = this.store.select(TemplatesSelectors.selectCategoryTypes);
  selectsubWorkTypes$ = this.store.select(
    TemplatesSelectors.selectsubWorkTypes
  );
  selectpublishStatus$ = this.store.select(
    TemplatesSelectors.selectpublishStatus
  );
  selectTemplateWithMilestones$ = this.store.select(
    TemplatesSelectors.selectTemplateWithMilestones
  );
  selectmilestoneSaveStatus$ = this.store.select(
    TemplatesSelectors.selectmilestoneSaveStatus
  );

  getTemplates(isActive: boolean) {
    this.store.dispatch(TemplatesActions.templateListGet({ isActive }));
  }
  getworkTypeList() {
    this.store.dispatch(TemplatesActions.workTypeListGet());
  }
  getserviceList() {
    this.store.dispatch(TemplatesActions.serviceTypeListGet());
  }
  getcategoryTypeList() {
    this.store.dispatch(TemplatesActions.categoryTypeListGet());
  }
  updatesaveStatus() {
    this.store.dispatch(TemplatesActions.updatesaveStatus());
  }
  saveTemplates(Templates: TemplateViewModel) {
    this.store.dispatch(TemplatesActions.savetemplate({ template: Templates }));
  }
  getsubworkTypeList(workTypeId: string) {
    this.store.dispatch(
      TemplatesActions.subworkTypeListGet({ workTypeId: workTypeId })
    );
  }

  templateWithMilestoneListGet(
    Id: string,
    IsActive_milestone: boolean = true,
    IsActive_template: boolean = true,
    WorkTypeId?: string
  ) {
    this.store.dispatch(
      TemplatesActions.templateWithMilestoneListGet({
        IsActive_template,
        IsActive_milestone,
        Id,
        WorkTypeId,
      })
    );
  }
  updatemilestonessaveStatus() {
    this.store.dispatch(TemplatesActions.updatemilestonessaveStatus());
  }
  savemilestones(milestone: TemplateViewModel[]) {
    this.store.dispatch(
      TemplatesActions.savemilestones({ milestones: milestone })
    );
  }
  publishmilestonessaveStatus() {
    this.store.dispatch(TemplatesActions.publishmilestonessaveStatus());
  }
  publishmilestones(id: string) {
    this.store.dispatch(TemplatesActions.publishmilestones({ id: id }));
  }
  reset() {
    this.store.dispatch(TemplatesActions.reset());
  }
}
