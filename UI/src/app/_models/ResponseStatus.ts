export const SuccessStatus = 'SUCCESS';
export const FailedStatus = 'FAILED';
export const ErrorStatus = 'ERROR';

export interface ResponseModel {
  data: any;
  errorCode: string;
  message: string;
  status: string;
  totalRecordCount: number;
}
