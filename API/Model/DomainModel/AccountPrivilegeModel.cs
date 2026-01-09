using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "account_privilege", keyFieldName: "PrivilegeId")]
    public class AccountPrivilegeModel
    {
        [LogField]
        public string PrivilegeId { get; set; } = null!;
        [LogField]
        public string ModuleName { get; set; } = null!;
        [LogField]
        public string PrivilegeCode { get; set; } = null!;
        [LogField]
        public string Privilege { get; set; } = null!;
        [LogField]
        public string PrivilegeType { get; set; } = null!;
        [LogField]
        public bool IsActive { get; set; }

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
