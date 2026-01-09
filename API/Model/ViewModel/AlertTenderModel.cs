using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class AlertTenderModel
    {
        public string TenderId { get; set; } = string.Empty;
        public string TenderNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime WorkModifiedDate { get; set; }
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string WorkValue { get; set; } = string.Empty;
        public decimal WorkValueConverted { get; set; }
        public decimal WorkValueIncreasedAmount { get; set; }
        public bool IsWorkStarted { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class AlertTenderMilestoneModel
    {
        public string MilestoneId { get; set; } = string.Empty;
        public string WorkId { get; set; } = string.Empty;
        public string WorkTemplateId { get; set; } = string.Empty;
        public string MilestoneName { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
        public decimal PaymentPercentage { get; set; }
        public decimal PercentageCompleted { get; set; }
        public bool IsPaymentRequired { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime MilestoneModifiedDate { get; set; }
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string MilestoneStatus { get; set; } = string.Empty;
        public string MilestoneCode { get; set; } = string.Empty;
        public string MileStoneId { get; set; } = string.Empty;
        public decimal MilestoneAmount { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
    }


    public class AlertTenderMBookModel
    {
        public string MBookId { get; set; } = string.Empty;
        public string WorkId { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string PaymentStatusCode { get; set; } = string.Empty;
        public string MBookNumber { get; set; } = string.Empty;
        public string WorkNotes { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string MilestoneId { get; set; } = string.Empty;
        public string MilestoneName { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
        public decimal PaymentPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PercentageCompleted { get; set; }
        public bool IsActive { get; set; }
        public string MilestoneCode { get; set; } = string.Empty;
        public string MileStoneId { get; set; } = string.Empty;
        public decimal MilestoneAmount { get; set; }
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime MBookModifiedDate { get; set; }
    }
}
