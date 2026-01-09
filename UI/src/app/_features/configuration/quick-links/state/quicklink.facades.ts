import { Store } from '@ngrx/store';
import * as QuickLinkActions from '../state/quicklink.actions';
import * as QuickLinkSelectors from './quicklink.selectors';
import { Injectable } from '@angular/core';
import { QuickLinkModel } from 'src/app/_models/configuration/quickLink';

@Injectable()
export class QuickLinkConfigFacade {
  constructor(private store: Store) {}

  selectQuickLinks$ = this.store.select(QuickLinkSelectors.selectQuickLinks);
  selectSaveStatus$ = this.store.select(QuickLinkSelectors.selectSaveStatus);
  selectuserGroups$ = this.store.select(QuickLinkSelectors.selectuserGroups);

  getQuickLinks(isActive: boolean) {
    this.store.dispatch(QuickLinkActions.quicklinkListGet({ isActive }));
  }
  userGroupListGet(isActive: boolean) {
    this.store.dispatch(QuickLinkActions.userGroupListGet());
  }
  updatesaveStatus() {
    this.store.dispatch(QuickLinkActions.updatesaveStatus());
  }
  saveQuickLink(quicklink: QuickLinkModel) {
    this.store.dispatch(
      QuickLinkActions.savequicklink({ quicklink: quicklink })
    );
  }
  reset() {
    this.store.dispatch(QuickLinkActions.reset());
  }
}
