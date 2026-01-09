import { Store } from '@ngrx/store';
import * as MBookActions from '../state/mbook.actions';
import * as MBookSelectors from './mbook.selectors';
import { Injectable } from '@angular/core';
import { MBookMasterViewModel } from 'src/app/_models/mbook/mbook';
import { MBookFilterModel } from 'src/app/_models/filterRequest';

@Injectable()
export class MBookConfigFacade {
  constructor(private store: Store) {}

  selectMBooks$ = this.store.select(MBookSelectors.selectMBooks);
  selectSaveStatus$ = this.store.select(MBookSelectors.selectSaveStatus);
  selectDivisions$ = this.store.select(MBookSelectors.selectDivisions);
  selectMBookbyId$ = this.store.select(MBookSelectors.selectMBookbyId);
  selectfiletypes$ = this.store.select(MBookSelectors.selectfiletypes);
  selectapprovaltypes$ = this.store.select(MBookSelectors.selectapprovaltypes);
  selectapprovestatus$ = this.store.select(MBookSelectors.selectapprovestatus);

  getMBooks(mbookFiletr: MBookFilterModel) {
    this.store.dispatch(MBookActions.mbookListGet({ mbookFiletr }));
  }
  getmbookbyId(id: string) {
    this.store.dispatch(MBookActions.mbookbyId({ id: id }));
  }

  getapprovalTypes(id: string) {
    this.store.dispatch(MBookActions.mbookbyId({ id: id }));
  }
  getfileTypes(id: string) {
    this.store.dispatch(MBookActions.mbookbyId({ id: id }));
  }
  updatesaveStatus() {
    this.store.dispatch(MBookActions.updatesaveStatus());
  }
  saveMBook(mbook: {
    id: string;
    workNotes: string;
    date: string;
    isSubmitted: boolean;
    actualAmount: number;
    statusCode: string;
  }) {
    this.store.dispatch(MBookActions.savembook({ mbook: mbook }));
  }

  divisionListGet() {
    this.store.dispatch(MBookActions.divisionListGet());
  }
  getApprovalTypes(mbookId: string) {
    this.store.dispatch(MBookActions.getApprovalTypes({ mbookId }));
  }
  getFileTypes() {
    this.store.dispatch(MBookActions.getFileTypes());
  }
  updateApproveStatus() {
    this.store.dispatch(MBookActions.updateapprovetatus());
  }
  ApproveMBook(mbook: {
    mbookId: string;
    statusCode: string;
    comments: string;
    mbookApprovHistoryeId:string;
    //documentName:string;
    
  }) {
    this.store.dispatch(MBookActions.approvembook({ mbook: mbook }));
  }
  reset() {
    this.store.dispatch(MBookActions.reset());
  }
}
