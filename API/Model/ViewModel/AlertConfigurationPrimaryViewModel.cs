using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class AlertConfigurationPrimaryViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Object { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string EmailFrequency { get; set; } = string.Empty;
        public string SmsFrequency { get; set; } = string.Empty;
        public string EmailuserGroups { get; set; } = string.Empty;
        public string SmsuserGroups { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public string RunningNumber { get; set; } = string.Empty;
        public string AlertNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public List<string>? EmailuserGroupList { get; set; }
        public List<string>? SmsuserGroupsList { get; set; }

        public List<AlertConfigurationSecondaryViewModel>? AlertConfigurationSecondary { get; set; }

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime? LastUpdatedDate { get; set; }
    }
}
