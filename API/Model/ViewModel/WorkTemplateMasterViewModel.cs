using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class WorkTemplateMasterViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public string WorkId { get; set; } = string.Empty;
        public string WorkTemplateName { get; set; } = string.Empty;
        public string WorkTypeId { get; set; } = string.Empty;
        public int WorkDurationInDays { get; set; }
        public string WorkTemplateNumber { get; set; } = string.Empty;
        public string WorkType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsSubmitted { get; set; }

        public string SubWorkTypeId { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;
        public string SubWorkType { get; set; } = string.Empty;

        public string categoryTypeId { get; set; } = string.Empty;
        public string serviceTypeId { get; set; } = string.Empty;
        public string serviceType { get; set; } = string.Empty;
        public string categoryType { get; set; } = string.Empty;

        public List<WorkTemplateMilestoneMasterViewModel>? Milestones { get; set; }


        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime LastUpdatedDate { get; set; }
    }
}
