export interface TemplatewithMilestoneViewModel {
  id: string;
  name: string;
  workTypeId: string;
  subWorkTypeId: string;
  strength: string;
  templateCode: string;
  durationInDays: number;
  prefix: string;
  suffix: string;
  runningNumber: number;
  templateNumber: string;
  isActive: boolean;
  isPublished: boolean;
  status: string;
  workType: string;
  subWorkType: string;
  serviceType?: string;
  serviceTypeId?: string;
  categoryType?: string;
  categoryTypeId?: string;
  lastUpdatedBy: string;
  lastUpdatedUserName: string;
  lastUpdatedDate: string;
  templateMilestones: TemplateMilestoneModel[];
}

export interface TemplateMilestoneModel {
  id: string;
  templateId: string;
  milestoneName: string;
  milestoneCode: string;
  orderNumber: number;
  isPublished: boolean;
  durationInDays: number;
  isPaymentRequired: boolean;
  paymentPercentage: number;
  isActive: boolean;
  templateName: string;
  lastUpdatedBy: string;
  lastUpdatedUserName: string;
  lastUpdatedDate: string;
}
