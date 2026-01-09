using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class TemplateMilestoneViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TemplateId { get; set; } = string.Empty;
        public string MilestoneName { get; set; } = string.Empty;
        public string MilestoneCode { get; set; } = string.Empty;
        public int OrderNumber { get; set; }
        public int DurationInDays { get; set; }
        public bool IsPaymentRequired { get; set; }
        public int PaymentPercentage { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublished { get; set; }

        public string TemplateName { get; set; } = string.Empty;

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime LastUpdatedDate { get; set; }
    }
}
