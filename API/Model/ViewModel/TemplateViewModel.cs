using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class TemplateViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string WorkTypeId { get; set; } = string.Empty;
        public string WorkId { get; set; } = string.Empty;
        public string workTemplateId { get; set; } = string.Empty;
        public string SubWorkTypeId { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        public string TemplateNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsPublished { get; set; }
        public string DepartmentId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public string WorkType { get; set; } = string.Empty;
        public string SubWorkType { get; set; } = string.Empty;
        public string categoryTypeId { get; set; } = string.Empty;
        public string serviceTypeId { get; set; } = string.Empty;
        public string serviceType { get; set; } = string.Empty;
        public string categoryType { get; set; } = string.Empty;

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime LastUpdatedDate { get; set; }

        public List<TemplateMilestoneViewModel>? templateMilestones { get; set; }
    }
}
