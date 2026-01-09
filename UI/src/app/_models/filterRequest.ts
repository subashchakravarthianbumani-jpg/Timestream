export interface TableFilterModel {
  skip: number;
  take: number;
  searchString: string | null;
  sorting: ColumnSortingModel | null;
  columnSearch: ColumnSearchModel[] | null;
}

export interface ColumnSortingModel {
  fieldName: string;
  sort: string;
}

export interface ColumnSearchModel {
  fieldName: string;
  searchString: string;
}
export interface ColumnSearchModel {
  fieldName: string;
  searchString: string;
}
export interface TenderFilterModel extends TableFilterModel {
  where: TenderWhereClauseProperties | null;
  divisionList: string[] | null;
  districtList: string[] | null;
  workType: string[] | null;
  fromDate: string | null;
  toDate: string | null;
  selectionType: string | null;
  year: string[] | null;
}

export interface TenderWhereClauseProperties {
  isActive: boolean;
  id: string;
  tenderNumber: string;
  localTenderNumber: string;
}
export interface GOWhereClauseProperties {
  isActive: boolean;
}
export interface MBookFilterModel extends TableFilterModel {
  where: MBookWhereClauseProperties | null;
  year: string[] | null;
  divisionIds: string[] | null;
  departmentId?: string;
  selectionType: string | null;
  isForApproval: boolean;
}

export interface CameraFilterModel extends TableFilterModel {
  where: CameraWhereClauseProperties | null;
}
export interface UserFilterModel extends TableFilterModel {
  where: UserWhereClauseProperties | null;
}

export interface UserWhereClauseProperties {
  isActive: boolean;
}
export interface MBookWhereClauseProperties {
  isActive: boolean;
}

export interface CameraWhereClauseProperties {
  isActive: boolean;
}

export interface GoFilterModel extends TableFilterModel {
  where: GoWhereClauseProperties | null;
  year: string[] | null;
}
export interface GoReportFilterModel extends TableFilterModel {
  where: GoWhereClauseProperties | null;
  fromDate?: string[] | null;
  toDate?: string[] | null;
  divisionList: string[] | null;
  statusList: string[] | null;
  departmentList: string[] | null;
  divisionId: string | null;
  districtId: string | null;
}

export interface GoWhereClauseProperties {
  isActive: boolean;
  id: string;
  gONumber: string;
  localGONumber: string;
  departmentId: string;
}

export interface CommentFilterModel extends TableFilterModel {
  where: CommentWhereClauseProperties | null;
}
export interface CommentWhereClauseProperties {
  id?: string | null;
  type: string | null;
  typeId: string | null;
  commentsFrom?: string | null;
  commentsText?: string | null;
  subjectText?: string | null;
  commentNumber?: string | null;
  createdByUserName?: string | null;
  commentDate?: string | null;
}

export interface ReportBreadcrumbModel {
  orderNumber: number;
  fieldName: string;
  value: string;
  labelName: string;
  label: string;
  ismook?: boolean;
  ismilestone?: boolean;
}

export interface ActivityFilterModel extends TableFilterModel {
  where: ActivityWhereClauseProperties | null;
  ids: string[] | null;
  types: string[] | null;
}

export interface ActivityWhereClauseProperties {
  type: string | null;
  typeId: string | null;
}

export interface AlertFilterModel extends TableFilterModel {
  roleId: string | null;
  divisionIds: string[] | null;
  types: string[] | null;
  typeIds: string[] | null;
}

//updated by vijay 13-11-2025 for fetch mainCategory and subcategory in report
export interface WorkFilterModel extends TableFilterModel {
  where: WorkWhereClauseProperties | null;
  goNumber: string | null;
  workNumber: string | null;
  workTypeId: string | null;
  subWorkTypeId: string | null;
  mainCategory: string | null;
  subcategory: string | null;
  strength: string | null;
  divisionId: string | null;
  districtId: string | null;
  duration: string | null;
  workStatus: string | null;
  cost: string | null;
  days: string | null;
  delay: string | null;
  contractor: string | null;
  contractorDistrict: string | null;
  contractorCompanyName: string | null;
  contractorMobile: string | null;
  divisionList: string[] | null;
  districtList: string[] | null;
  schemeList: string[] | null;
  packageList: string[] | null;
  statusList: string[] | null;
  fromDate?: string | null;
  toDate?: string | null;
  departmentList: string[] | null;
}

export interface WorkWhereClauseProperties {
  isActive: boolean;
  id: string;
  tenderId: string;
  workNumber: string;
}

export interface MilestoneFilterModel extends TableFilterModel {
  workId: string | null;
  workTypeId: string | null;
  subWorkTypeId: string | null;
  strength: string | null;
  divisionId: string | null;
  districtId: string | null;
  approvalStatusName: string | null;
  paymentStatusName: string | null;
  approvalStatusId: string | null;
  paymentStatusId: string | null;
  cost: string | null;
  actualCost: string | null;
  divisionList: string[] | null;
  statusList: string[] | null;
  departmentList: string[] | null;
  districtList: string[] | null;
  fromDate: string | null;
  toDate: string | null;
}

export interface MBookReportFilterModel extends TableFilterModel {
  workId: string | null;
  workTypeId: string | null;
  subWorkTypeId: string | null;
  strength: string | null;
  divisionId: string | null;
  districtId: string | null;
  statusName: string | null;
  paymentStatusName: string | null;
  statusId: string | null;
  paymentStatusId: string | null;
  actionableRoleId: string | null;
  amount: string | null;
  actualAmount: string | null;
  divisionList: string[] | null;
  statusList: string[] | null;
  departmentList: string[] | null;
  districtList: string[] | null;
  fromDate: string | null;
  toDate: string | null;
}
