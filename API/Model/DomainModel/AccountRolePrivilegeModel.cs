using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "account_role_privilege", keyFieldName: "RolePrivilegeId")]
    public class AccountRolePrivilegeModel
    {
        [LogField]
        public string RolePrivilegeId { get; set; } = null!;
        [LogField]
        public string RoleId { get; set; } = null!;
        [LogField]
        public string PrivilegeId { get; set; } = null!;

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByUserName { get; set; } = string.Empty;
        public DateTime ModifiedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
