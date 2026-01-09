using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "template_milestone_master", keyFieldName: "Id")]
    public class TemplateMilestoneModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string TemplateId { get; set; } = string.Empty;
        [LogField]
        public string MilestoneName { get; set; } = string.Empty;
        [LogField]
        public string MilestoneCode { get; set; } = string.Empty;
        [LogField]
        public int OrderNumber { get; set; }
        [LogField]
        public int DurationInDays { get; set; }
        [LogField]
        public bool IsPaymentRequired { get; set; }
        [LogField]
        public int PaymentPercentage { get; set; }
        [LogField]
        public bool IsActive { get; set; }

        public string TemplateName { get; set; } = string.Empty;

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
