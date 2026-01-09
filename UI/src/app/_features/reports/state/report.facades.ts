import { Store } from '@ngrx/store';
import * as ReportSelectors from './report.selectors';
import { Injectable } from '@angular/core';
import * as ReportActions from '../state/report.actions';
import {
  ReportBreadcrumbModel,
  WorkFilterModel,
} from 'src/app/_models/filterRequest';

@Injectable()
export class ReportFacade {
  constructor(private store: Store) {}

  selectReports$ = this.store.select(ReportSelectors.selectReports);
  selectBreadCrumb$ = this.store.select(ReportSelectors.selectBreadCrumb);

  getReports(model: any, selectedType: string) {
    if (selectedType == 'TENDER') {
      this.store.dispatch(ReportActions.getReports({ model, selectedType }));
    } else if (selectedType == 'MILESTONE') {
      this.store.dispatch(ReportActions.getMilestoneReports({ model }));
    } else if (selectedType == 'GO') {
      //TODO
      this.store.dispatch(ReportActions.getGOReports({ model }));
    } else {
      this.store.dispatch(ReportActions.getMbookReports({ model }));
    }
  }
  getbreadcrumbsList() {
    this.store.dispatch(ReportActions.getBreadcrumbs());
  }
  saveBreadcrumbs(breadcrumb: ReportBreadcrumbModel[]) {
    this.store.dispatch(ReportActions.saveBreadcrumbs({ breadcrumb }));
  }
  reset() {
    this.store.dispatch(ReportActions.reset());
    //window.location.reload();
  }
}
