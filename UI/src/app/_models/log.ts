export interface RecordHistoryModel {
  id: string;
  action: string;
  tableName: string;
  tableUniqueId: string;
  columnName: string;
  oldValue: string;
  newValue: string;
  createdBy: string;
  createdByUserName: string;
  createdDate: string;
  savedBy: string;
  savedByUserName: string;
  lastUpdatedDatestring: string;
  savedDate: string;
}
export interface MailSMSLog {
  id: string;
  recordType: string;
  sentAddress: string;
  subject: string;
  body: string;
  type: string;
  typeId: string;
  receivedBy: string;
  createdBy: string;
  createdByUserName: string;
  lastUpdatedDatestring: string;
  createdDate: string | null;
}
