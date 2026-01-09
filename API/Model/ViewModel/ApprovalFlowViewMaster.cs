using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class ApprovalFlowViewMaster
    {
        public string Id { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public int OrderNumber { get; set; }
        public bool IsNA { get; set; }
        public string ApprovalFlowId { get; set; } = string.Empty;
        public string ReturnFlowId { get; set; } = string.Empty;
        public bool IsFinal { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;

        public string ApprovalFlowRoleName { get; set; } = string.Empty;
        public string ApprovalFlowRoleCode { get; set; } = string.Empty;

        public string ReturnFlowRoleName { get; set; } = string.Empty;
        public string ReturnFlowRoleCode { get; set; } = string.Empty;

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime? LastUpdatedDate { get; set; }
    }

    public class ApprovalFlowAddRoleModel
    {
        public string DepartmentId { get; set; } = string.Empty;
        public List<string> RoleIds { get; set; } = null!;
    }

    public class ApprovalFlowAddRoleModel_API
    {
        public string DepartmentId { get; set; } = string.Empty;
        public List<string> RoleIds { get; set; } = null!;
        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
