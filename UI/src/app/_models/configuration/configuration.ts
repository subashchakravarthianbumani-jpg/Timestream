export interface IConfigurationModel {
  id: string;
  categoryId: string;
  configurationId: string;
  value: string;
  code: string | null;
  departmentId: string | null;
  departmentName?: string | null;
  departmentCode?: string | null;
  canDelete?: boolean;
  isGeneral?: boolean;
  isActive?: boolean;
  lastUpdatedBy?: string;
  lastUpdatedUserName?: string;
  lastUpdatedDate?: string | null;
}

export interface IConfigCategoryModel {
  id: string;
  parentId: string;
  category: string;
  categoryCode: string;
  categoryType: string;
  isActive: boolean;
  isEditable: boolean;
  isDependent: boolean;
  hasCode: boolean;
}
