using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class RoleViewModels
    {
    }

    public class AccountRoleViewModel
    {
        public string Id { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string RoleCode { get; set; } = null!;
        public bool IsActive { get; set; }

        public bool IsChangeable { get; set; }

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime LastUpdatedDate { get; set; }
    }
    public class AccountRolePrivilegeViewModel
    {
        public string RolePrivilegeId { get; set; } = null!;
        public string RoleId { get; set; } = null!;
        public string PrivilegeId { get; set; } = null!;
    }

    public class AccountPrivilegeViewModel
    {
        public string PrivilegeId { get; set; } = null!;
        public string ModuleName { get; set; } = null!;
        public string PrivilegeCode { get; set; } = null!;
        public string Privilege { get; set; } = null!;
        public string PrivilegeType { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
