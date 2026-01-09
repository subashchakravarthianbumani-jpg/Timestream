using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class TenderDataModel
    {
        public string TenderId { get; set; } = string.Empty;
        public string TenderNumber { get; set; } = string.Empty;
        public string WorkType { get; set; } = string.Empty;
        public DateTime AwardedDate { get; set; }
        public DateTime WorkCommencementDate { get; set; }
        public DateTime WorkCompletionDate { get; set; }
        public string TenderStatus { get; set; } = string.Empty;
        public string WorkStatus { get; set; } = string.Empty;
        public int WorkDurationInDays { get; set; }
        public int DateDifference { get; set;}
       
       
    }
}
