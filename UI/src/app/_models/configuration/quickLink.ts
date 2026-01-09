export interface QuickLinkModel {
  id: string;
  name: string;
  link: string;
  fileType: string;
  isActive: boolean;
  userGroupIds?: string;
  userGroupIdList: string[] | null;
  lastUpdatedBy?: string;
  lastUpdatedUserName?: string;
  lastUpdatedDate?: string;
}
