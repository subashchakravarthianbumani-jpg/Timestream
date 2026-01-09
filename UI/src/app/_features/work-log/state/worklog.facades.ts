import { Store } from '@ngrx/store';
import * as WorkLogActions from '../state/worklog.actions';
import * as WorkLogSelectors from './worklog.selectors';
import { Injectable } from '@angular/core';
import { GOMasterViewModel } from 'src/app/_models/go/tender';
import { GoFilterModel } from 'src/app/_models/filterRequest';

@Injectable()
export class WorkLogConfigFacade {
  constructor(private store: Store) {}

  selectWorkLogs$ = this.store.select(WorkLogSelectors.selectWorkLogs);
  selectSaveStatus$ = this.store.select(WorkLogSelectors.selectSaveStatus);

  getWorkLogs(filterModel: GoFilterModel) {
    this.store.dispatch(WorkLogActions.workLogListGet({ filterModel }));
  }
  updatesaveStatus() {
    this.store.dispatch(WorkLogActions.updatesaveStatus());
  }
  saveWorkLog(workLog: GOMasterViewModel) {
    this.store.dispatch(WorkLogActions.saveworkLog({ workLog: workLog }));
  }
  reset() {
    this.store.dispatch(WorkLogActions.reset());
  }
}
