using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    public class AccountPrivilegeFormModel
    {
        public string PrivilegeId { get; set; } = string.Empty;
        public string Privilege { get; set; } = string.Empty;
        public string PrivilegeCode { get; set; } = string.Empty;
        public string PrivilegeType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public int OrderNumber { get; set; }
    }

    public class AccountPrivilegeByGroupModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public List<AccountPrivilegeFormModel> Privilege { get; set; }
    }

    public class AccountPrivilegeSaveViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string PrivilegeId { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class AccountPrivilegeSaveModel
    {
        public string RolePrivilegeId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string PrivilegeId { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }

}
