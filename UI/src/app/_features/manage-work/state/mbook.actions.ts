import { createAction, props } from '@ngrx/store';
import { ResponseModel } from 'src/app/_models/ResponseStatus';
import { MBookFilterModel } from 'src/app/_models/filterRequest';
import { MBookMasterViewModel } from 'src/app/_models/mbook/mbook';
import { TCModel } from 'src/app/_models/user/usermodel';

export const mbookListGet = createAction(
  '[MBook] Get MBook List',
  props<{ mbookFiletr: MBookFilterModel }>()
);

export const mbookListGetSuccess = createAction(
  '[MBook] Get MBook List Success',
  props<{ mbooks: ResponseModel }>()
);

export const mbookbyId = createAction(
  '[MBook] Get MBook by Id',
  props<{ id: string }>()
);

export const mbookbyIdSuccess = createAction(
  '[MBook] Get MBook by Id Success',
  props<{ mbook: MBookMasterViewModel }>()
);

export const savembook = createAction(
  '[MBook]  Save MBook ',
  props<{
    mbook: {
      id: string;
      workNotes: string;
      date: string;
      isSubmitted: boolean;
      actualAmount: number;
      statusCode: string;
    };
  }>()
);
export const savembookSuccess = createAction(
  '[MBook] Save MBook Success',
  props<{ mbook: ResponseModel }>()
);
export const updatesaveStatus = createAction('[MBook] Save MBook Status');

export const divisionListGet = createAction('[MBook] Get division List');

export const divisionListGetSuccess = createAction(
  '[MBook] Get division List Success',
  props<{ divisions: TCModel[] }>()
);

export const getApprovalTypes = createAction(
  '[MBook] Get Approval Type',
  props<{ mbookId: string }>()
);

export const getApprovalTypesSuccess = createAction(
  '[MBook] Get Approval Type Success',
  props<{ approvaltypes: TCModel[] }>()
);

export const getFileTypes = createAction('[MBook] Get File Types');

export const getFileTypesSuccess = createAction(
  '[MBook] Get File Types Success',
  props<{ filetypes: TCModel[] }>()
);

export const approvembook = createAction(
  '[MBook]  approve MBook ',
  props<{
    mbook: {
      mbookId: string;
      statusCode: string;
      comments: string;
      mbookApprovHistoryeId:string;
      //documentName:string;
    };
  }>()
);
export const approvembookSuccess = createAction(
  '[MBook] approve MBook Success',
  props<{ mbook: ResponseModel }>()
);

export const updateapprovetatus = createAction(
  '[MBook] SAve approve MBook Status'
);

export const reset = createAction('[MBook] Reset');
