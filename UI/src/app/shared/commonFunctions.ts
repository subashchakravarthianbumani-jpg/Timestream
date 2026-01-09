import * as moment from 'moment';
import { Column } from '../_models/datatableModel';
import { ToWords } from 'to-words';

export function dateconvertion(date: any) {
  return moment(date).format('DD-MM-YYYY hh:mm a');
}
export function dateconvertionwithOnlyDate(date: any) {
  return moment(date).format('DD-MM-YYYY');
}

export function getYearList() {
  var startyear = 2022;
  var yearlist = [(2022).toString()];
  var currentyear = new Date().getFullYear();
  for (var i = startyear + 1; i <= currentyear; i++) {
    yearlist = [...yearlist, i.toString()];
  }
  return yearlist;
}
export const successStatusList = [
  'published',
  'approved',
  'saved',
  'submitted',
  'completed',
  'payment done',
];
export const warningStatusList = [
  'in-progress',
  'returned',
  'new',
  'payment pending',
  'payment in progress',
  'Not started',
  'Slow Progress',
  'Started but stilled',
];
export const dangerStatusList = ['rejected'];

export const wordFileTypes = ['docx', 'doc'];
export const xlFileTypes = ['xlsx', 'xls'];
export const pdfFileTypes = ['pdf'];
export const imgFileTypes = ['jpeg', 'png'];

export function getBtnSeverity(statusName: string) {
  if (!statusName) {
    return 'grey';
  }
  const name = String(statusName).toLowerCase();
  if (successStatusList.includes(statusName.toLowerCase())) {
    return 'success';
  } else if (warningStatusList.includes(statusName.toLowerCase())) {
    return 'warning';
  } else if (dangerStatusList.includes(statusName.toLowerCase())) {
    return 'danger';
  }
  return 'grey';
}

export function getCommentType() {
  return [
    { value: 'MBOOK', text: 'M-Book' },
    { value: 'MILESTONE', text: 'Milestone' },
    { value: 'TENDER', text: 'Tender' },
  ];
}

export function getcolorforProgress(val: number, type: string) {
  if (type == 'paymentPercentage') {
    return '#8888ea';
  }
  if (val > 0 && val <= 30) {
    return '#8888ea';
  } else if (val > 30 && val <= 70) {
    return '#caca29';
  } else if (val > 70) {
    return '#4dbc4d';
  }
  return '#b2b2b2';
}

export const reportCols: Column[] = [
  {
    field: 'divisionName',
    header: 'Division',
    sortablefield: 'divisionName',
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'districtName',
    header: 'District',
    sortablefield: 'districtName',
    isSortable: true,
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'mainCategory',
    header: 'Work Type',
    sortablefield: 'mainCategory',
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'subcategory',
    header: 'Sub Work Type',
    sortablefield: 'subcategory',
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'goNumber',
    header: 'GO',
    sortablefield: 'goNumber',
    isSearchable: true,
    //isAnchortagforFilter: true,
  },
  {
    field: 'schemeName',
    header: 'Scheme Name',
    sortablefield: 'schemeName',
    isSearchable: true,
    //isAnchortagforFilter: true,
  },
  {
    field: 'go_Package_No',
    header: 'Package Number',
    sortablefield: 'go_Package_No',
    isSearchable: true,
  },
  {
    field: 'tenderNumber',
    header: 'Work Id',
    sortablefield: 'tenderNumber',
    isSearchable: true,
    isPopup: true,
    popupType: 'WORK',
  },
  {
    field: 'strength',
    header: 'Strength',
    sortablefield: 'strength',
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'workDurationInDays',
    header: 'Work Duration',
    sortablefield: 'workDurationInDays',
    isSortable: true,
    isSearchable: true,
  },
  {
    field: 'workStatusName',
    header: 'Work Status',
    sortablefield: 'workStatusName',
    isSortable: true,
    isSearchable: true,
    isBadge: true,
  },
  // {
  //   field: 'workProgress',
  //   header: 'Progress',
  //   sortablefield: 'workProgress',
  //   isSortable: true,
  //   isSearchable: true,
  // },
  // {
  //   field: 'health',
  //   header: 'Health',
  //   sortablefield: 'health',
  //   isSortable: true,
  //   isSearchable: true,
  //   //isAnchortagforFilter: true,
  // },
  // {
  //   field: 'durationLeft',
  //   header: 'Duration Left',
  //   sortablefield: 'durationLeft',
  //   isSortable: true,
  //   isSearchable: true,
  // },
  // {
  //   field: 'publishedDate',
  //   header: 'Published Date',
  //   sortablefield: 'publishedDate',
  //   isSortable: true,
  //   isSearchable: true,
  // },
  // {
  //   field: 'awardedDate',
  //   header: 'Awarded Date',
  //   sortablefield: 'awardedDate',
  //   isSearchable: true,
  // },
  {
    field: 'workValue',
    header: 'Work value',
    sortablefield: 'workValue',
    isSearchable: true,
  },
  {
    field: 'startDateString',
    header: 'Work Published Date',
    sortablefield: 'startDate',
    isSearchable: true,
  },
  {
    field: 'tenderOpenedDate',
    header: 'Tender Opened Date',
    sortablefield: 'tenderOpenedDate',
    isSearchable: true,
  },
  {
    field: 'endDateString',
    header: 'Awarded Date',
    sortablefield: 'endDate',
    isSearchable: true,
  },

  {
    field: 'WorkCommencementDate',
    header: 'Work Commencement Date',
    sortablefield: 'WorkCommencementDate',
    isSearchable: true,
  },
  {
    field: 'WorkCompletionDate',
    header: 'Work Completion Date',
    sortablefield: 'WorkCompletionDate',
    isSearchable: true,
  },
  {
    field: 'dateDifference',
    header: 'Difference in Days',
    sortablefield: 'dateDifference',
    isSearchable: true,
  },
  // {
  //   field: 'pannedValue',
  //   header: 'Planned Value',
  //   sortablefield: 'pannedValue',
  //   isSearchable: true,
  // },
  // {
  //   field: 'actualValue',
  //   header: 'Actual Value',
  //   sortablefield: 'actualValue',
  //   isSearchable: true,
  // },
  // {
  //   field: 'paymentValue',
  //   header: 'Payment Value',
  //   sortablefield: 'paymentValue',
  //   isSearchable: true,
  // },
  // {
  //   field: 'workEMD',
  //   header: 'Work EMD',
  //   isSearchable: true,
  //   sortablefield: 'workEMD',
  // },
  // {
  //   field: 'workASD',
  //   header: 'Work ASD',
  //   isSearchable: true,
  //   sortablefield: 'workASD',
  // },
  // {
  //   field: 'amountSpent',
  //   header: 'Amount Spent',
  //   sortablefield: 'amountSpent',
  //   isSearchable: true,
  // },
  {
    field: 'contractorName',
    header: 'Contractor Name',
    sortablefield: 'contractorName',
    isSearchable: true,
    //isAnchortagforFilter: true,
  },
  {
    field: 'contractorDistrict',
    header: 'Contractor District',
    sortablefield: 'contractorDistrict',
    isSearchable: true,
    //isAnchortagforFilter: true,
  },
  {
    field: 'contractorCompanyName',
    header: 'Conractor Company Name',
    sortablefield: 'contractorCompanyName',
    isSearchable: true,
    //isAnchortagforFilter: true,
  },
  {
    field: 'contractorMobile',
    header: 'Contractor Mobile',
    sortablefield: 'contractorMobile',
    isSearchable: true,
  },
];

export const milestoneCols: Column[] = [
  {
    field: 'divisionName',
    header: 'Division',
    sortablefield: 'divisionName',
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'districtName',
    header: 'District',
    sortablefield: 'districtName',
    isSortable: true,
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'tenderNumber',
    header: 'Work Id',
    sortablefield: 'tenderNumber',
    isSearchable: true,
  },
  {
    field: 'workType',
    header: 'Work Type',
    sortablefield: 'workType',
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'subWorkType',
    header: 'Sub Work Type',
    sortablefield: 'subWorkType',
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'strength',
    header: 'Strength',
    sortablefield: 'strength',
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'durationInDays',
    header: 'Work Duration',
    sortablefield: 'durationInDays',
    isSortable: true,
    isSearchable: true,
    //isAnchortagforFilter: true,
  },
  // {
  //   field: 'milestoneStatusName',
  //   header: 'Status',
  //   sortablefield: 'milestoneStatusName',
  //   isSortable: true,
  //   isSearchable: true,
  //   isBadge: true,
  // },
  // {
  //   field: 'workProgress',
  //   header: 'Progress',
  //   sortablefield: 'workProgress',
  //   isSortable: true,
  //   isSearchable: true,
  // },
  // {
  //   field: 'health',
  //   header: 'Health',
  //   sortablefield: 'health',
  //   isSortable: true,
  //   isSearchable: true,
  //   isAnchortagforFilter: true,
  // },
  // {
  //   field: 'durationLeft',
  //   header: 'Duration Left',
  //   sortablefield: 'durationLeft',
  //   isSortable: true,
  //   isSearchable: true,
  //   isAnchortagforFilter: true,
  // },
  {
    field: 'milestoneName',
    header: 'Milestone Name',
    sortablefield: 'milestoneName',
    isSearchable: true,
    //isAnchortagforFilter: true,
  },
  {
    field: 'milestoneCode',
    header: 'Milestone Code',
    sortablefield: 'milestoneCode',
    isSearchable: true,
    //isAnchortagforFilter: true,
  },
  {
    field: 'startDateString',
    header: 'Start Date',
    sortablefield: 'startDate',
    isSearchable: true,
  },
  {
    field: 'durationInDays',
    header: 'Duration in Days',
    sortablefield: 'durationInDays',
    isSearchable: true,
  },
  {
    field: 'endDateString',
    header: 'End Date',
    sortablefield: 'endDate',
    isSearchable: true,
  },
  {
    field: 'percentageCompleted',
    header: 'Percentage Completed',
    sortablefield: 'percentageCompleted',
    isProgress: true,
    isSearchable: true,
  },
  {
    field: 'paymentPercentage',
    header: 'Payment Percentage',
    sortablefield: 'paymentPercentage',
    isProgress: true,
    isSearchable: true,
  },
  {
    field: 'pannedValue',
    header: 'Planned Value',
    sortablefield: 'pannedValue',
    isSearchable: true,
  },
  {
    field: 'actualValue',
    header: 'Actual Value',
    sortablefield: 'actualValue',
    isSearchable: true,
  },
  {
    field: 'paymentValue',
    header: 'Payment Value',
    sortablefield: 'paymentValue',
    isSearchable: true,
  },
  {
    field: 'paymentStatusName',
    header: 'Payment Status',
    sortablefield: 'paymentStatusName',
    isBadge: true,
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'milestoneStatusName',
    header: 'Milestone Status',
    sortablefield: 'milestoneStatusName',
    isBadge: true,
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'milestoneFile1Original',
    header: 'File 1',
    sortablefield: 'milestoneFile1Original',
    isSortable: false,
    isSearchable: false,
    isDownloadable: true,
  },
  {
    field: 'milestoneFile2Original',
    header: 'File 2',
    sortablefield: 'milestoneFile2Original',
    isSortable: false,
    isSearchable: false,
    isDownloadable: true,
  },
];

export const MbookCols: Column[] = [
  {
    field: 'divisionName',
    header: 'Division',
    sortablefield: 'divisionName',
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'districtName',
    header: 'District',
    sortablefield: 'districtName',
    isSortable: true,
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  // {
  //   field: 'goNumber',
  //   header: 'GO',
  //   sortablefield: 'goNumber',
  //   isSortable: true,
  //   isSearchable: true,
  //   isAnchortagforFilter: true,
  // },
  {
    field: 'tenderNumber',
    header: 'Work Number',
    customExportHeader: 'Work Number',
    sortablefield: 'tenderNumber',
    isSortable: true,
    isSearchable: true,
    //isAnchortagforFilter: true,
  },
  {
    field: 'mBookNumber',
    header: 'M-Book Number',
    customExportHeader: 'M-Book Number',
    sortablefield: 'mBookNumber',
    isSortable: true,
    isSearchable: true,
    isPopup: true,
    popupType: 'MBOOK',
  },
  {
    field: 'workType',
    header: 'Work Type',
    sortablefield: 'workType',
    isSortable: true,
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'subWorkType',
    header: 'Sub Work Type',
    sortablefield: 'subWorkType',
    isSortable: true,
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'strength',
    header: 'Strength',
    isSearchable: true,
    sortablefield: 'strength',
    isSortable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'milestoneCode',
    header: 'Code',
    isSearchable: true,
    sortablefield: 'milestoneCode',
    isSortable: true,
    //isAnchortagforFilter: true,
  },
  {
    field: 'milestoneName',
    header: 'Milestone Name',
    sortablefield: 'milestoneName',
    //isAnchortagforFilter: true,
  },
  {
    field: 'startDate',
    header: 'Start Date',
    sortablefield: 'startDate',
    isSortable: true,
    isSearchable: true,
  },
  // {
  //   field: 'endDate',
  //   header: 'End Date',
  //   sortablefield: 'endDate',
  // },
  {
    field: 'percentageCompleted',
    header: 'Percentage',
    sortablefield: 'percentageCompleted',
    isSortable: true,
    isSearchable: true,
    isProgress: true,
  },
  {
    field: 'paymentPercentage',
    header: 'Payment Percentage',
    sortablefield: 'paymentPercentage',
    isSortable: true,
    isProgress: true,
  },
  {
    field: 'pannedValue',
    header: 'Planned Value',
    sortablefield: 'pannedValue',
    isSearchable: true,
  },
  {
    field: 'actualValue',
    header: 'Actual Value',
    sortablefield: 'actualValue',
    isSearchable: true,
  },
  {
    field: 'paymentValue',
    header: 'Payment Value',
    sortablefield: 'paymentValue',
    isSearchable: true,
  },
  {
    field: 'paymentStatusName',
    header: 'Payment Status',
    sortablefield: 'paymentStatusName',
    isSortable: true,
    isSearchable: true,
    isBadge: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'isWaitingForPayment',
    header: 'Is Waiting for Payement',
    sortablefield: 'isWaitingForPayment',
  },
  {
    field: 'statusName',
    header: 'Status',
    sortablefield: 'statusName',
    isSortable: true,
    isSearchable: true,
    isBadge: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'dateString',
    header: 'M-Book Date',
    sortablefield: 'date',
    isSortable: true,
  },
  {
    field: 'notes',
    header: 'Notes',
    sortablefield: 'notes',
    isSortable: true,
    isSearchable: true,
  },
  {
    field: 'milestoneFile1Original',
    header: 'File 1',
    sortablefield: 'milestoneFile1Original',
    isSortable: false,
    isSearchable: false,
    isDownloadable: true,
  },
  {
    field: 'milestoneFile2Original',
    header: 'File 2',
    sortablefield: 'milestoneFile2Original',
    isSortable: false,
    isSearchable: false,
    isDownloadable: true,
  },
];

export const GoCols: Column[] = [
  {
    field: 'divisionName',
    header: 'Division',
    sortablefield: 'divisionName',
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'districtName',
    header: 'District',
    sortablefield: 'districtName',
    isSortable: true,
    isSearchable: true,
    isAnchortagforFilter: true,
  },
  {
    field: 'goNumber',
    header: 'GO Number',
    sortablefield: 'goNumber',
    isSearchable: true,
  },
  {
    field: 'department',
    header: 'Department',
    sortablefield: 'department',
    isSortable: true,
  },
  {
    field: 'goDate',
    header: 'Date',
    sortablefield: 'goDate',
    isSortable: true,
  },
  {
    field: 'numberOfTenders',
    header: 'Number of Tenders',
    customExportHeader: 'Number of Tenders',
    sortablefield: 'numberOfTenders',
    isSortable: true,
  },
  {
    field: 'goCost',
    header: 'GO Cost',
    customExportHeader: 'M-Book Number',
    sortablefield: 'goCost',
    isSortable: true,
  },
  {
    field: 'goTotalAmount',
    header: 'GO Total Amount',
    sortablefield: 'goTotalAmount',
    isSortable: true,
  },
  {
    field: 'totalWork',
    header: 'Total Work',
    sortablefield: 'totalWork',
    isSortable: true,
  },
  {
    field: 'remainingWorks',
    header: 'Remaining Work',
    isSearchable: true,
    sortablefield: 'remainingWorks',
    isSortable: true,
  },
  {
    field: 'completedWork',
    header: 'Completed Work',
    isSearchable: true,
    sortablefield: 'completedWork',
    isSortable: true,
    //isAnchortagforFilter: true,
  },
  {
    field: 'pannedValue',
    header: 'Planned Value',
    sortablefield: 'pannedValue',
    //isAnchortagforFilter: true,
  },
  {
    field: 'actualValue',
    header: 'Actual Value',
    sortablefield: 'actualValue',
    isSortable: true,
  },
  {
    field: 'paymentAmount',
    header: 'Payment Value',
    sortablefield: 'paymentAmount',
    isSortable: true,
  },
];
export const privileges = {
  ROLE_UPDATE: 'ROLE_UPDATE',
  USER_CREATE: 'USER_CREATE',
  DASHBOARD_TENDER_COUNT: 'DASHBOARD_TENDER_COUNT',
  DASHBOARD_TENDER_RECORD: 'DASHBOARD_TENDER_RECORD',
  DASHBOARD_MBOOK_COUNT: 'DASHBOARD_MBOOK_COUNT',
  DASHBOARD_MBOOK_RECORD: 'DASHBOARD_MBOOK_RECORD',
  DASHBOARD_KEY_CONTACTS: 'DASHBOARD_KEY_CONTACTS',
  DASHBOARD_QUICK_LINKS: 'DASHBOARD_QUICK_LINKS',
  GO_VIEW: 'GO_VIEW',
  TENDER_VIEW: 'TENDER_VIEW',
  TENDER_EDIT: 'TENDER_EDIT',
  TENDER_TEMPLATE_EDIT: 'TENDER_TEMPLATE_EDIT',
  TENDER_MILESTONE_EDIT: 'TENDER_MILESTONE_EDIT',
  TENDER_WORK_CREATE: 'TENDER_WORK_CREATE',
  TENDER_MILESTONE_SET: 'TENDER_MILESTONE_SET',
  MBOOK_VIEW: 'MBOOK_VIEW',
  MBOOK_EDIT: 'MBOOK_EDIT',
  MBOOK_APPROVE_REJECT: 'MBOOK_APPROVE_REJECT',
  MBOOK_HISTORY: 'MBOOK_HISTORY',
  ALERT_VIEW: 'ALERT_VIEW',
  ALERT_RESOLVE: 'ALERT_RESOLVE',
  USER_UPDATE: 'USER_UPDATE',
  MBOOK_UPDATE: 'MBOOK_UPDATE',
  ROLE_CREATE: 'ROLE_CREATE',
  USER_VIEW: 'USER_VIEW',
  ROLE_VIEW: 'ROLE_VIEW',
  CONFIG_CREATE: 'CONFIG_CREATE',
  CONFIG_VIEW: 'CONFIG_VIEW',
  CONFIG_UPDATE: 'CONFIG_UPDATE',
  CONFIG_DELETE: 'CONFIG_DELETE',
  ROLE_SET_PRIVILEGE: 'ROLE_SET_PRIVILEGE',
  QL_CREATE: 'QL_CREATE',
  QL_VIEW: 'QL_VIEW',
  QL_UPDATE: 'QL_UPDATE',
  QL_DELETE: 'QL_DELETE',
  TEMPLATE_CREATE: 'TEMPLATE_CREATE',
  TEMPLATE_VIEW: 'TEMPLATE_VIEW',
  TEMPLATE_UPDATE: 'TEMPLATE_UPDATE',
  TEMPLATE_DELETE: 'TEMPLATE_DELETE',
  TEMPLATE_SET_MILESTONE: 'TEMPLATE_SET_MILESTONE',
  APPROVALFLOW_CREATE: 'APPROVALFLOW_CREATE',
  APPROVALFLOW_VIEW: 'APPROVALFLOW_VIEW',
  APPROVALFLOW_UPDATE: 'APPROVALFLOW_UPDATE',
  USER_Delete: 'USER_Delete',
  ROLE_Delete: 'ROLE_Delete',
  REPORTS_VIEW: 'REPORTS_VIEW',
  RECORD_LOG_VIEW: 'RECORD_LOG_VIEW',
  EMAIL_SMS_LOG_VIEW: 'EMAIL_SMS_LOG_VIEW',
  TENDER_AMOUNT_REVISION: 'TENDER_AMOUNT_REVISION',
  TENDER_ADD_COMMENT: 'TENDER_ADD_COMMENT',
  TENDER_REVIEW_COMMENT: 'TENDER_REVIEW_COMMENT',
  MILESTONE_PERCENTAGE_UPDATE: 'MILESTONE_PERCENTAGE_UPDATE',
  ALERT_CONFIG_VIEW: 'ALERT_CONFIG_VIEW',
  ALERT_CONFIG_CREATE: 'ALERT_CONFIG_CREATE',
  ALERT_CONFIG_UPDATE: 'ALERT_CONFIG_UPDATE',
  ALERT_CONFIG_DELETE: 'ALERT_CONFIG_DELETE',
  TEMPLATE_REASSIGN: 'TEMPLATE_REASSIGN',
};

export const workKeyValueTenderModel = {
  goNumber: 'goNumber',
  workType: 'workType',
  workNumber: 'workNumber',
  subWorkType: 'subWorkType',
  mainCategory: 'mainCategory',
  subcategory: 'subcategory',
  workStatusName: 'workStatusName',
  strength: 'strength',
  divisionName: 'divisionName',
  districtName: 'districtName',
  workProgress: 'workProgress',
  workDurationInDays: 'workDurationInDays',
  durationLeft: 'durationLeft',
  workValue: 'workValue',
  contractorName: 'contractorName',
  contractorDistrict: 'contractorDistrict',
  contractorCompanyName: 'contractorCompanyName',
  contractorMobile: 'contractorMobile',
};
export const workKeyValueModelTenderarray = [
  {
    fieldName: 'goNumber',
    fieldNameId: 'goNumber',
  },
  {
    fieldName: 'workType',
    fieldNameId: 'workTypeId',
  },
  {
    fieldName: 'subWorkType',
    fieldNameId: 'subWorkTypeId',
  },
  {
    fieldName: 'mainCategory',
    fieldNameId: 'mainCategory',
  },
  {
    fieldName: 'subcategory',
    fieldNameId: 'subcategory',
  },
  {
    fieldName: 'strength',
    fieldNameId: 'strength',
  },
  {
    fieldName: 'divisionName',
    fieldNameId: 'division',
  },
  {
    fieldName: 'districtName',
    fieldNameId: 'district',
  },
  {
    fieldName: 'workProgress',
    fieldNameId: 'workProgress',
  },
  {
    fieldName: 'workDurationInDays',
    fieldNameId: 'workDurationInDays',
  },
  {
    fieldName: 'durationLeft',
    fieldNameId: 'durationLeft',
  },
  {
    fieldName: 'workValue',
    fieldNameId: 'workValue',
  },
  {
    fieldName: 'contractorName',
    fieldNameId: 'contractorName',
  },
  {
    fieldName: 'contractorDistrict',
    fieldNameId: 'contractorDistrict',
  },
  {
    fieldName: 'contractorCompanyName',
    fieldNameId: 'contractorCompanyName',
  },
  {
    fieldName: 'contractorMobile',
    fieldNameId: 'contractorMobile',
  },
  {
    fieldName: 'workStatusName',
    fieldNameId: 'workStatus',
  },
];

export const workKeyValueMbookModel = {
  workNotes: 'workNotes',
  submittedDate: 'submittedDate',
  completedDate: 'completedDate',
  date: 'date',
  actualAmount: 'actualAmount',
  mBookNumber: 'mBookNumber',
  actionableRoleId: 'actionableRoleId',
  actionableRoleName: 'actionableRoleName',
  paymentStatusId: 'paymentStatusId',
  paymentStatusName: 'paymentStatusName',
  statusId: 'statusId',
  statusName: 'statusName',
  workId: 'workId',
  workTemplateId: 'workTemplateId',
  milestoneName: 'milestoneName',
  orderNumber: 'orderNumber',
  durationInDays: 'durationInDays',
  durationInDaysLeft: 'durationInDaysLeft',
  isPaymentRequired: 'isPaymentRequired',
  paymentPercentage: 'paymentPercentage',
  milestoneStartDate: 'milestoneStartDate',
  milestoneEndDate: 'milestoneEndDate',
  milestoneCompletedDate: 'milestoneCompletedDate',
  milestoneCode: 'milestoneCode',
  milestoneAmount: 'milestoneAmount',
  milestoneActualAmount: 'milestoneActualAmount',
  workTypeId: 'workTypeId',
  workType: 'workType',
  subWorkTypeId: 'subWorkTypeId',
  subWorkType: 'subWorkType',
  strength: 'strength',
  workNumber: 'workNumber',
  workTemplateName: 'workTemplateName',
  division: 'division',
  district: 'district',
  districtName: 'districtName',
  divisionName: 'divisionName',
  tenderNumber: 'tenderNumber',
};
export const workKeyValueModelMbookarray = [
  {
    fieldName: 'goNumber',
    fieldNameId: 'goNumber',
  },
  {
    fieldName: 'workType',
    fieldNameId: 'workTypeId',
  },
  {
    fieldName: 'subWorkType',
    fieldNameId: 'subWorkTypeId',
  },
  {
    fieldName: 'strength',
    fieldNameId: 'strength',
  },
  {
    fieldName: 'divisionName',
    fieldNameId: 'division',
  },
  {
    fieldName: 'districtName',
    fieldNameId: 'district',
  },
  {
    fieldName: 'paymentStatusName',
    fieldNameId: 'paymentStatusId',
  },
  {
    fieldName: 'workDurationInDays',
    fieldNameId: 'workDurationInDays',
  },
  {
    fieldName: 'actualAmount',
    fieldNameId: 'actualAmount',
  },
  {
    fieldName: 'milestoneAmount',
    fieldNameId: 'amount',
  },
  {
    fieldName: 'statusName',
    fieldNameId: 'statusId',
  },
];

export const workKeyValueMilestoneModel = {
  goNumber: 'goNumber',
  workType: 'workType',
  workNumber: 'workNumber',
  subWorkType: 'subWorkType',
  workStatusName: 'workStatusName',
  strength: 'strength',
  divisionName: 'divisionName',
  districtName: 'districtName',
  workProgress: 'workProgress',
  workDurationInDays: 'workDurationInDays',
  durationLeft: 'durationLeft',
  workValue: 'workValue',
  contractorName: 'contractorName',
  contractorDistrict: 'contractorDistrict',
  contractorCompanyName: 'contractorCompanyName',
  contractorMobile: 'contractorMobile',
  workId: 'workId',
  workTypeId: 'workTypeId',
  subWorkTypeId: 'subWorkTypeId',
  divisionId: 'divisionId',
  districtId: 'districtId',
  approvalStatusName: 'approvalStatusName',
  paymentStatusName: 'paymentStatusName',
  approvalStatusId: 'approvalStatusId',
  paymentStatusId: 'paymentStatusId',
  cost: 'cost',
  actualCost: 'actualCost',
};
export const workKeyValueModelMilestonearray = [
  {
    fieldName: 'cost',
    fieldNameId: 'cost',
  },
  {
    fieldName: 'districtName',
    fieldNameId: 'district',
  },
  {
    fieldName: 'divisionName',
    fieldNameId: 'division',
  },
  {
    fieldName: 'strength',
    fieldNameId: 'strength',
  },
  {
    fieldName: 'subWorkType',
    fieldNameId: 'subWorkTypeId',
  },
  {
    fieldName: 'workType',
    fieldNameId: 'workTypeId',
  },
  {
    fieldName: 'actualCost',
    fieldNameId: 'actualCost',
  },
  {
    fieldName: 'paymentStatusName',
    fieldNameId: 'paymentStatusId',
  },
  {
    fieldName: 'approvalStatusName',
    fieldNameId: 'approvalStatusId',
  },
];

const toWords = new ToWords({
  localeCode: 'en-IN',
  converterOptions: {
    currency: true,
    ignoreDecimal: false,
    ignoreZeroCurrency: false,
    doNotAddOnly: false,
    currencyOptions: {
      // can be used to override defaults for the selected locale
      name: 'Rupee',
      plural: 'Rupees',
      symbol: '₹',
      fractionalUnit: {
        name: 'Paisa',
        plural: 'Paise',
        symbol: '',
      },
    },
  },
});

export function convertoWords(amount: number) {
  return toWords.convert(amount);
}

export const imgExtension = [
  '.apng',
  '.png',
  '.avif',
  '.gif',
  '.jpg',
  '.jpeg',
  '.jfif',
  '.pjpeg',
  '.pjp',
  '.png',
  '.svg',
  '.webp',
];
