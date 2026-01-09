using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class MilestoneUpdateModel
    {
        public string WorkMilestoneId { get; set; } = string.Empty;
        public decimal CompletedPercentage { get; set; }
        public string PercentageUpdateNote { get; set; } = string.Empty;
        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
    public class MilestoneUpdateViewModel
    {
        public string WorkMilestoneId { get; set; } = string.Empty;
        public string PercentageUpdateNote { get; set; } = string.Empty;
        public decimal CompletedPercentage { get; set; }
    }
}
