export interface TemplateViewModel {
  id: string;
  name: string;
  workTypeId: string;
  serviceTypeId?: string;
  categoryTypeId?: string;
  departmentId?: string;
  durationInDays: string;
  subWorkTypeId: string;
  strength: string;
  templateCode: string;
  prefix?: string;
  suffix?: string;
  runningNumber?: string;
  templateNumber?: string;
  isActive: boolean;
  status?: string;
  isPublished?: boolean;
  workId?:string;
  workTemplateId?:string;
}
