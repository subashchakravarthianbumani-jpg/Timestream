using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "Alert_Configuration_Primary", keyFieldName: "Id")]
    public class AlertConfigurationPrimaryModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string Type { get; set; } = string.Empty;
        [LogField]
        public string Object { get; set; } = string.Empty;
        [LogField]
        public string Name { get; set; } = string.Empty;
        [LogField]
        public string Department { get; set; } = string.Empty;
        [LogField]
        public string EmailFrequency { get; set; } = string.Empty;
        [LogField]
        public string SMSFrequency { get; set; } = string.Empty;
        [LogField]
        public string EmailuserGroups { get; set; } = string.Empty;
        [LogField]
        public string SMSuserGroups { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; }

        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public string RunningNumber { get; set; } = string.Empty;
        public string AlertNumber { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;

        public List<string>? EmailuserGroupList { get; set; }
        public List<string>? SMSuserGroupsList { get; set; }

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
