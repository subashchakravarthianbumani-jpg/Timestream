export interface ApprovalFlowModel {
  id: string;
  roleId: string;
  orderNumber: number;
  approvalFlowId: string;
  returnFlowId: string;
  isActive?: boolean;
  roleName?: string;
  roleCode?: string;
  departmentId: string;
}
export interface ApprovalFlowAddRoleModel {
  roleIds: string[];
  departmentId: string;
}
