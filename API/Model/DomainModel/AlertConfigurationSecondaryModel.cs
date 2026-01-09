using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "Alert_Configuration_Secondary", keyFieldName: "Id")]
    public class AlertConfigurationSecondaryModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string PrimaryId { get; set; } = string.Empty;
        [LogField]
        public string Severity { get; set; } = string.Empty;
        [LogField]
        public string Field { get; set; } = string.Empty;
        [LogField]
        public string BaseField { get; set; } = string.Empty;
        [LogField]
        public string CalculationType { get; set; } = string.Empty;
        [LogField]
        public string FrequencyType { get; set; } = string.Empty;
        [LogField]
        public decimal CalculationNo { get; set; }



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
