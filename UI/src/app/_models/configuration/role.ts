export interface IRoleModel {
  id: string;
  roleName: string;
  roleCode: string;
  isActive: boolean;
  isChangeable?: boolean;
}
