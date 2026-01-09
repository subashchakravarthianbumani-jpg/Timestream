using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class GOReportViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string GONumber { get; set; } = string.Empty;
        public DateTime GODate { get; set; }
        public string GOName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;

        public decimal GOCost { get; set; }
        public decimal GORevisedAmount { get; set; }
        public decimal GOTotalAmount { get; set; }
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string DivisionName { get; set; } = string.Empty;

        public int NumberOfTenders { get; set; }
        public decimal PannedValue { get; set; }
        public decimal ActualValue { get; set; }
        public decimal PaymentAmount { get; set; }
        public int TotalWork { get; set; }
        public int CompletedWork { get; set; }
        public int RemainingWorks { get; set; }
    }
}
