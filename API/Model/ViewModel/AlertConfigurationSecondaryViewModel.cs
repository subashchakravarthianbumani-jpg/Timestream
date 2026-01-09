using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class AlertConfigurationSecondaryViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string PrimaryId { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public string BaseField { get; set; } = string.Empty;
        public string CalculationType { get; set; } = string.Empty;
        public string FrequencyType { get; set; } = string.Empty;
        public decimal CalculationNo { get; set; }

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime? LastUpdatedDate { get; set; }

    }
}
