import { TCModel } from './user/usermodel';

export interface AlertConfigurationFormModel {
  typeList: TCModel[];
  objectList: TCModel[];
  departmentList: TCModel[];
  severityList: TCModel[];
  fieldList: TCModel[];
  baseFieldList: TCModel[];
  calculationTypeList: TCModel[];
  frequencyTypeList: TCModel[];
  userGroupList: TCModel[];
}
export interface AlertConfigurationPrimaryModel {
  id: string;
  type: string;
  object: string;
  name: string;
  alertNumber: string;
  department: string;
  emailFrequency: string;
  smsFrequency: string;
  emailuserGroups: string;
  sMSuserGroups: string;
  isActive: boolean;
  departmentName: string;
  emailuserGroupList: string[] | null;
  smsuserGroupsList: string[] | null;
  alertConfigurationSecondary: AlertConfigurationSecondaryViewModel[] | null;
  lastUpdatedBy: string;
  lastUpdatedUserName: string;
  lastUpdatedDate: string | null;
}
export interface AlertConfigurationSecondaryViewModel {
  id: string;
  primaryId: string;
  severity: string;
  field: string;
  baseField: string;
  calculationType: string;
  frequencyType: string;
  userGroupId: string;
  calculationNo: number;
  email: number;
  sMS: number;
  userGroups: string[] | null;
  lastUpdatedBy: string;
  lastUpdatedUserName: string;
  lastUpdatedDate: string | null;
}
