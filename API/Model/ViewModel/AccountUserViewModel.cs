using Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class AccountUserViewModel
    {
        public string UserId { get; set; } = null!;
        public string UserNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsContractor { get; set; }
        public bool IsActive { get; set; }
        public string RoleId { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public string DivisionId { get; set; } = string.Empty;
        public string UserGroup { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string DistrictId { get; set; } = string.Empty;
        public string PofileImageId { get; set; } = string.Empty;
        public string LoginId { get; set; } = string.Empty;
        public List<string>? DivisionIdList { get; set; }
        public List<string>? DistrictIdList { get; set; }
        public List<string>? DepartmentIdList { get; set; }

        public string District { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string UserGroupName { get; set; } = string.Empty;
        public string UserGroupCode { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;

        public string DepartmentId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime? LastUpdatedDate { get; set; }
    }

    public class AccountUserFormDetailModel
    {
        public List<SelectListItem> DistrictList { get; set; } = null!;
        public List<SelectListItem> DivisionList { get; set; } = null!;
        public List<SelectListItem> UserGroupList { get; set; } = null!;
        public List<SelectListItem> DepartmentList { get; set; } = null!;
    }

}
