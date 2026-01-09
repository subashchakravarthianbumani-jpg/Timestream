export interface AccountUserViewModel {
  userId: string;
  userNumber?: string;
  firstName: string;
  lastName: string;
  email: string;
  isContractor?: boolean;
  isActive?: boolean;
  roleId: string;
  prefix?: string;
  suffix?: string;
  runningNumber?: number;
  mobile: string;
  divisionId?: string;
  userGroup: string;
  dOB: string;
  password: string;
  districtId?: string;
  pofileImageId?: string;
  loginId?: string;
  divisionIdList: string[] | null;
  departmentIdList: string[] | null;
  districtIdList: string[] | null;
  district?: string;
  division?: string;
  userGroupName?: string;
  roleCode?: string;
  roleName?: string;
  departmentId?: string;
  department?: string;
  lastUpdatedBy?: string;
  lastUpdatedUserName?: string;
  lastUpdatedDate?: string | null;
  lastUpdatedDatestring?: string | null;
}

export interface UserFormListModel {
  districtList: TCModel[];
  divisionList: TCModel[];
  userGroupList: TCModel[];
  departmentList: TCModel[];
}
export interface TCModel {
  selected?: boolean;
  text: string;
  value: string;
}
