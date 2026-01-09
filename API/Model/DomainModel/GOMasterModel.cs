using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "go_master", keyFieldName: "Id")]
    public class GOMasterModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string GONumber { get; set; } = string.Empty;
        [LogField]
        public DateTime GODate { get; set; }
        [LogField]
        public decimal GOCost { get; set; }
        [LogField]
        public string GOName { get; set; } = string.Empty;
        [LogField]
        public string GODepartment { get; set; } = string.Empty;
        public string Go_no_works { get; set; } = string.Empty;
        public string Go_package_count { get; set; } = string.Empty;
       
        [LogField]
        public string DepartmentId { get; set; } = string.Empty;
        [LogField]
        public string DepartmentCode { get; set; } = string.Empty;

        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string DivisionName { get; set; } = string.Empty;
        [LogField]
        public decimal GORevisedAmount { get; set; }
        [LogField]
        public decimal GOTotalAmount { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        public string LocalGONumber { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; }

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
