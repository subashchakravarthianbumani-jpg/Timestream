using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "template_master", keyFieldName: "Id")]
    public class TemplateModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string Name { get; set; } = string.Empty;
        [LogField]
        public string WorkTypeId { get; set; } = string.Empty;
        [LogField]
        public string SubWorkTypeId { get; set; } = string.Empty;
        [LogField]
        public string Strength { get; set; } = string.Empty;
        [LogField]
        public string TemplateCode { get; set; } = string.Empty;
        [LogField]
        public int DurationInDays { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        public string TemplateNumber { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; }
        [LogField]
        public bool IsPublished { get; set; }
        [LogField]
        public string DepartmentId { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;
        public string WorkType { get; set; } = string.Empty;
        public string SubWorkType { get; set; } = string.Empty;
        public string categoryTypeId { get; set; } = string.Empty;
        public string serviceTypeId { get; set; } = string.Empty;
        public string serviceType { get; set; } = string.Empty;
        public string categoryType { get; set; } = string.Empty;
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
