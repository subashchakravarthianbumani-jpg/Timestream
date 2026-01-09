using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.CustomeAttributes;

namespace Model.DomainModel
{
    [TableInfo( tableName: "account_role", keyFieldName: "Id")]
    public class AccountRoleModel
    {
        [LogField]
        public string Id { get; set; } = null!;
        [LogField]
        public string RoleName { get; set; } = null!;
        [LogField]
        public string RoleCode { get; set; } = null!;
        [LogField]
        public bool IsActive { get; set; }
        public bool IsChangeable { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByUserName { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
