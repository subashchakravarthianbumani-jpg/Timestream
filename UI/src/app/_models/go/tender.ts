import { FileMasterModel } from '../mbook/mbook';

export interface GOMasterViewModel {
  id: string;
  goNumber: string;
  goDate: string;
  goCost: number;
  goName: string;
  goDepartment: string;
  goRevisedAmount: number;
  goTotalAmount: number;
  prefix: string;
  suffix: string;
  runningNumber: number;
  localGONumber: string;
  isActive: boolean;
  tenderList: TenderMasterViewModel[];
  lastUpdatedBy: string;
  lastUpdatedUserName: string;
  lastUpdatedDate: string | null;
}
export interface TenderMasterViewModel {
  id: string;
  goId: string;
  tenderNumber: string;
  startDate: string;
  endDate: string;
  division: string;
  district: string;
  divisionName: string;
  districtName: string;
  class: string;
  category: string;
  workValue: string;
  mainCategory: string;
  subcategory: string;
  bidType: string;
  contractorName: string;
  contractorDivision: string;
  contractorDistrict: string;
  contractorCategory: string;
  tenderFinalAmount: string;
  prefix: string;
  suffix: string;
  runningNumber: number;
  localTenderNumber: string;
  isActive: boolean;
  goNumber: string;
  goName: string;
  canCreateWork: boolean;
  canCreateTemplate: boolean;
  canEditWork: boolean;
  milestones: TemplateMilestoneViewModel | null;
  lastUpdatedBy: string;
  lastUpdatedUserName: string;
  lastUpdatedDate: string | null;
}
export interface TemplateMilestoneViewModel {
  id: string;
  templateId: string;
  milestoneName: string;
  milestoneCode: string;
  orderNumber: number;
  durationInDays: number;
  isPaymentRequired: boolean;
  paymentPercentage: number;
  isActive: boolean;
  isPublished: boolean;
  templateName: string;
  lastUpdatedBy: string;
  lastUpdatedUserName: string;
  lastUpdatedDate: string;
}

export interface WorkMasterViewModel {
  id: string;
  tenderId: string;
  workOrderId: string;
  agreementCopyId: string;
  workTemplateId: string;
  calenderLeaveTypes: string;
  letterOfAcceptanceId: string;
  otherFileId: string;
  workCommencementDate :string;
  workCompletionDate: string;
  dateDifference:string;
  prefix: string;
  suffix: string;
  runningNumber: number;
  workNumber: string;
  isActive: boolean;
  goId: string;
  tenderStatus: string;
  workValueIncreasedAmount: number;
  tenderNumber: string;
  startDate: string;
  endDate: string;
  division: string;
  district: string;
  divisionName: string;
  districtName: string;
  class: string;
  category: string;
  workValue: number;
  mainCategory: string;
  subcategory: string;
  bidType: string;
  contractorName: string;
  contractorDivision: string;
  contractorDistrict: string;
  contractorCategory: string;
  contractorMobile: string;
  contractorEmail: string;
  contractorAddress: string;
  contractorAltMobile: string;
  tenderFinalAmount: number;
  localTenderNumber: string;
  contractorCompanyName: string;
  contractorCompanyAddress: string;
  contractorCompanyPhone: string;
  aPI_Responce: string;
  goNumber: string;
  goDate: string;
  goCost: number;
  goName: string;
  goDepartment: string;
  goRevisedAmount: number;
  goTotalAmount: number;
  localGONumber: string;
  work_Template_Id: string;
  templateId: string;
  workId: string;
  workTemplateName: string;
  workTypeId: string;
  workDurationInDays: number;
  workTemplateNumber: string;
  workType: string;
  subWorkTypeId: string;
  strength: string;
  templateCode: string;
  subWorkType: string;
  files: FileMasterModel[] | null;
  lastUpdatedBy: string;
  lastUpdatedUserName: string;
  lastUpdatedDate: string | null;

  negotiation_signed_doc: string;
  others_doc: string;
  isVerified?: boolean;
  workorder: string;
  schemeName:string;
  category_type_main:string;
  service_type_main:string;
  tenderOpenedDate:string;
  workSerialNumber :string;
  
}
export interface WorkTemplateMasterViewModel {
  id: string;
  templateId: string;
  workId: string;
  workTemplateName: string;
  workTypeId: string;
  workDurationInDays: number;
  workTemplateNumber: string;
  workType: string;
  isActive: boolean;
  subWorkTypeId: string;
  strength: string;
  templateCode: string;
  subWorkType: string;
  serviceType: string;
  serviceTypeId: string;
  categoryType: string;
  categoryTypeId: string;
  milestones: WorkTemplateMilestoneMasterViewModel[] | null;
  lastUpdatedBy: string;
  lastUpdatedUserName: string;
  lastUpdatedDate: string;
}
export interface WorkTemplateMilestoneMasterViewModel {
  id: string;
  workId: string;
  workTemplateId: string;
  milestoneName: string;
  orderNumber: number;
  durationInDays: number;
  isPaymentRequired: boolean;
  paymentPercentage: number;
  startDate: string;
  endDate: string;
  workCompletionDate:string;
  percentageCompleted: number;
  paymentStatus: string;
  milestoneStatus: string;
  isActive: boolean;
  isSubmitted: boolean;
  milestoneAmount: number;
  isCompleted: boolean;
  milestoneCode: string;
  workTemplateName: string;
  strength: string;
  templateCode: string;
  paymentStatusName: string;
  milestoneStatusName: string;
  lastUpdatedBy: string;
  lastUpdatedUserName: string;
  lastUpdatedDate: string;
  workTemplateMilestoneId: string;
  workTemplateMilestoneMbookId: string;
}
export interface UpdatePercentageModel {
  workMilestoneId: string;
  completedPercentage: string;
  percentageUpdateNote: string;
}
export interface CommentMasterModel {
  id: string;
  type: string;
  typeId: string;
  commentsFrom?: string;
  commentsText: string;
  subjectText: string;
  suffix?: string;
  prefix?: string;
  runningNumber?: number;
  commentNumber?: string;
  commentDate?: string | null;
  createdBy?: string;
  createdByUserName?: string;
  createdDate?: string | null;
}
export interface WorkActivityModel {
  id?: string;
  type: string;
  typeId: string;
  parentType?: string;
  parentId?: string;
  activitySubject?: string;
  activityMessage?: string;
  createdBy?: string;
  createdByUserName?: string;
  createdDate?: string | null;
  savedBy?: string;
  savedByUserName?: string;
  savedDate?: string;
}
export interface TenderAmountUpdateModel {
  tenderId: string;
  updatedNote: string;
  increasedAmount: number;
  savedBy?: string | null;
  savedByUserName?: string | null;
  savedDate?: string | null;
}
export interface MilestoneFileModel {
  id: string;
  type: string;
  originalFileName: string;
  savedFileName: string;
  fileType: string;
  typeId: string;
  createdByUserName: string;
  createdDate: string;
}
export interface CameraStreamModel{
    ipAddress: string;
    userName: string;
    password: string;
    channel: string;
    subType: string;
    workId: string;
}