import { ColumnSortingModel } from './filterRequest';
export interface DashboardCameraCountModel {
  notStarted: string;
  inProgress: string;
  completed: string;
  slowProgress: string;
  startedButStilled: string;
  total: string;
}
export interface DashboardRecordCountCardModel {
  project_Finished: number;
  project_OnGoing: number;
  project_OnHold: number;
  project_Slowprogress: number;
  total_Project: number;
  project_Upcoming: number;
  project_Finished_Amount: number;
  project_OnGoing_Amount: number;
  project_OnHold_Amount: number;
  project_Slowprogress_Amount: number;
  project_Upcoming_Amount: number;
  total_Project_Amount: number;
  project_Finished_Amount_Text: string;
  project_OnGoing_Amount_Text: string;
  project_OnHold_Amount_Text: string;
  project_Slowprogress_Amount_Text: number;
  total_Project_Amount_Text: string;
  project_Upcoming_Amount_Text: string;
  mbook_Approved: number;
  mbook_InApproval: number;
  mbook_Upcoming: number;
  mbook_Rejected: number;
  totalMbooks: number;
  mbook_NotUploaded: number;
  mbook_Uploaded: number;
  mbook_No_Action_Taken: number;
  mbook_PaymentPending: number;

  mbook_NotUploaded_Amount: number;
  mbook_Uploaded_Amount: number;
  mbook_No_Action_Taken_Amount: number;
  mbook_PaymentPending_Amount: number;

  mbook_NotUploaded_Amount_Text: string;
  mbook_Uploaded_Amount_Text: string;
  mbook_No_Action_Taken_Amount_Text: string;
  mbook_PaymentPending_Amount_Text: string;
  mbook_Total_Amount: number;
  mbook_Total_Amount_Text: string;

  mbook_Approved_Amount: number;
  mbook_InApproval_Amount: number;
  mbook_Upcoming_Amount: number;
  mbook_Rejected_Amount: number;
  mbook_Approved_Amount_Text: string;
  mbook_InApproval_Amount_Text: string;
  mbook_Upcoming_Amount_Text: string;
  mbook_Rejected_Amount_Text: string;
}
export interface TenderChartModel {
  labels: string[];
  datasets: TenderChartDatasetModel[];
}

export interface TenderChartDatasetModel {
  label: string;
  backgroundColor: string;
  borderColor: string;
  borderWidth: number;
  borderRadius: number;
  data: number[];
}

export interface DashboardFilterModel {
  divisionIds: string[] | null;
  departmentIds: string[] | null;
  selectionType: string | null;
  year: string[] | null;
  costOrCount: string | null;
}
export interface DashboardDivisionCountModel {
  divisionName: string;
  districtName: string;
  schemeName: string;
  schemeId: string;
  totalDivisionCount: number;
  totalWorkValue: number;
  completed: number;
  notStarted: number;
  inProgress: number;
  slowProgress: number;
  onHold: number;
  division: string;
  district: string;
  departmentId: string;

  NoActionTaken: string;
  MbookUploaded: string;
  MbookNotUploaded: string;
  MbookAmount: number;
}
export interface AlertMasterModel {
  id: string;
  alertId: string;
  alertCode: string;
  severity: string;
  message: string;
  type: string;
  typeId: string;
  roleId: string;
  divisionId: string;
  districtId: string;
  resolvedBy: string;
  resolvedByUserName: string;
  resolvedDate: string;
  createdBy: string;
  createdByUserName: string;
  createdDate: string;
}

export interface CameraData {
  divisionName: string;
  districtName: string;
  tenderNumber: string;
  rtspUrl: string;
  liveUrl: string;

  // From API
  mainCategory?: string;
  subcategory?: string;
  schemeName?: string;
  go_Package_No?: string;
  contractorCompanyName?: string;
  workCommencementDate?: string;
  dateDifference?: number;
  workStatus?: string;

  // UI Fields
  lastUpdated?: string;
  status?: 'In progress' | 'Completed' | 'Slow Progress' | 'No Data';
  thumbnail?: string;
}

export interface DashboardCameraModel {
  divisionIds: string[] | null;
  districtIds: string[] | null;
  tenderId: string;
  divisionName: string;
  workStatus: any;
  mainCategory: string;
  subcategory: string;
  districtName: string;
  tenderNumber: string;
  rtspUrl: string;
  liveUrl: string;
  channel: string;
  type: string;

  skip?: number;
  take?: number;
  SearchString?: string;
  sortField?: string;
  sortOrder?: string;

  sorting?: ColumnSortingModel;
}

export interface CameraFilter {
  divisionId?: string;
  districtId?: string;
  mainCategory?: string;
  subCategory?: string;
  workStatus?: string;
  tenderNumber?: string;
}
