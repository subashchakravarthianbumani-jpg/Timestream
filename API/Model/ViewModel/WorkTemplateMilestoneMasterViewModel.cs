using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class WorkTemplateMilestoneMasterViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string WorkId { get; set; } = string.Empty;
        public string WorkTemplateId { get; set; } = string.Empty;
        public string MilestoneName { get; set; } = string.Empty;
        public int OrderNumber { get; set; }
        public int DurationInDays { get; set; }
        public bool IsPaymentRequired { get; set; }
        public decimal PaymentPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PercentageCompleted { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string MilestoneStatus { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsSubmitted { get; set; }
        public decimal MilestoneAmount { get; set; }
        public bool IsCompleted { get; set; }
        public string MilestoneCode { get; set; } = string.Empty;
        public string WorkTemplateName { get; set; } = string.Empty;
        public string TenderNumber { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;

        public string PaymentStatusName { get; set; } = string.Empty;
        public string MilestoneStatusName { get; set; } = string.Empty;
        public decimal ActualAmount { get; set; }


        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime LastUpdatedDate { get; set; }
        public string WorkTemplateMilestoneMbookId { get; set; } = string.Empty;
        
    }
}
