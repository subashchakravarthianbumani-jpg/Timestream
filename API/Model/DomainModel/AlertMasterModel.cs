using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    public class AlertMasterModel
    {
        public string Id { get; set; } = null!;
        public string AlertId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string AlertCode { get; set; } = null!;
        public string Severity { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string TypeId { get; set; } = null!;
        public string RoleId { get; set; } = null!;
        public string DivisionId { get; set; } = null!;
        public string DistrictId { get; set; } = null!;
        public string ResolvedBy { get; set; } = string.Empty;
        public string ResolvedByUserName { get; set; } = string.Empty;
        public DateTime ResolvedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
    public class alertSaveList
    {
        public string AlertId { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string TypeId { get; set; } = string.Empty;
    }
}
