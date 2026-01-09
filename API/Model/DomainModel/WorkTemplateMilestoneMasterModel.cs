using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "work_template_milestone_master", keyFieldName: "Id")]
    public class WorkTemplateMilestoneMasterModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string WorkTemplateId { get; set; } = string.Empty;
        [LogField]
        public string MilestoneName { get; set; } = string.Empty;
        [LogField]
        public string OrderNumber { get; set; } = string.Empty;
        [LogField]
        public int DurationInDays { get; set; }
        [LogField]
        public bool IsPaymentRequired { get; set; }
        [LogField]
        public decimal PaymentPercentage { get; set; }
        [LogField]
        public DateTime StartDate { get; set; }
        [LogField]
        public DateTime EndDate { get; set; }
        [LogField]

       
        public DateTime CompletedDate { get; set; }
        [LogField]
        public decimal PercentageCompleted { get; set; }
        [LogField]
        public string PaymentStatus { get; set; } = string.Empty;
        [LogField]
        public string MilestoneStatus { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; }
        [LogField]
        public decimal MilestoneAmount { get; set; }
        [LogField]
        public bool IsCompleted { get; set; }
        [LogField]
        public decimal ActualAmount { get; set; }

        public string Division { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;

        public bool IsSubmitted { get; set; }
        public string MilestoneCode { get; set; } = string.Empty;
        public string WorkTemplateName { get; set; } = string.Empty;
        public string TenderNumber { get; set; } = string.Empty;

        public string PaymentStatusName { get; set; } = string.Empty;
        public string MilestoneStatusName { get; set; } = string.Empty;

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

        public string WorkTemplateMilestoneMbookId { get; set; } = string.Empty;
    }
}
