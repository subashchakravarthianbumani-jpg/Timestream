using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "approval_flow_master", keyFieldName: "Id")]
    public class ApprovalFlowMaster
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string RoleId { get; set; } = string.Empty;
        [LogField]
        public string DepartmentId { get; set; } = string.Empty;
        [LogField]
        public int OrderNumber { get; set; }
        [LogField]
        public bool IsNA { get; set; }
        [LogField]
        public string ApprovalFlowId { get; set; } = string.Empty;
        [LogField]
        public string ReturnFlowId { get; set; } = string.Empty;
        [LogField]
        public bool IsFinal { get; set; }
        [LogField]
        public bool IsActive { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;


        public string ApprovalFlowRoleName { get; set; } = string.Empty;
        public string ApprovalFlowRoleCode { get; set; } = string.Empty;

        public string ReturnFlowRoleName { get; set; } = string.Empty;
        public string ReturnFlowRoleCode { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByUserName { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string DeletedBy { get; set; } = string.Empty;
        public string DeletedByUserName { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
