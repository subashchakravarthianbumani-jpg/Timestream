using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "account_user", keyFieldName: "UserId")]
    public class AccountUserModel
    {
        [LogField]
        public string UserId { get; set; } = null!;
        public string UserNumber { get; set; } = string.Empty;
        [LogField]
        public string FirstName { get; set; } = string.Empty;
        [LogField]
        public string LastName { get; set; } = string.Empty;
        [LogField]
        public string Email { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; }
        [LogField]
        public bool IsContractor { get; set; }
        [LogField]
        public string RoleId { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        [LogField]
        public string Mobile { get; set; } = string.Empty;
        [LogField]
        public string DivisionId { get; set; } = string.Empty;
        [LogField]
        public string UserGroup { get; set; } = string.Empty;
        [LogField]
        public DateTime DOB { get; set; }
        [LogField]
        public string DistrictId { get; set; } = string.Empty;
        [LogField]
        public string PofileImageId { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string UserGroupName { get; set; } = string.Empty;
        public string UserGroupCode { get; set; } = string.Empty;

        public string DepartmentId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public List<string>? DistrictIdList { get; set; }
        public List<string>? DivisionIdList { get; set; }
        public List<string>? DepartmentIdList { get; set; }
        public List<string>? DepartmentNameList { get; set; }

        public string Password { get; set; } = string.Empty;

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
