using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class DashboardDivisionCountModel
    {
        public string DivisionName { get; set; } =string.Empty;
        public string DistrictName { get; set; } =string.Empty;
        public string SchemeName { get; set; } =string.Empty;
        public string SchemeId { get; set; } =string.Empty;
        public int TotalDivisionCount { get; set; }
        public decimal TotalWorkValue { get; set; }
        public int Completed { get; set; }
        public int NotStarted { get; set; }
        public int InProgress { get; set; }
        public int SlowProgress { get; set; }
        public int ONHold { get; set; }
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
       
        
        public decimal MbookAmount { get; set; } 
        public string MbookNotUploaded { get; set; } = string.Empty;
        public string MbookUploaded { get; set; } = string.Empty;
        public string NoActionTaken { get; set; } = string.Empty;
        public string PaymentPending { get; set; } = string.Empty;
       




    }
}
