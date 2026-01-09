using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class MBookReportModel
    {
        public string Id { get; set; } = string.Empty;
        public string WorkNotes { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public DateTime Date { get; set; }
        public decimal ActualAmount { get; set; }
        public string MBookNumber { get; set; } = string.Empty;
        public string ActionableRoleId { get; set; } = string.Empty;
        public string ActionableRoleName { get; set; } = string.Empty;
        public string PaymentStatusId { get; set; } = string.Empty;
        public string PaymentStatusName { get; set; } = string.Empty;
        public string StatusId { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;

        public string WorkId { get; set; } = string.Empty;
        public string WorkTemplateId { get; set; } = string.Empty;
        public string MilestoneName { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
        public int DurationInDaysLeft { get; set; }
        public bool IsPaymentRequired { get; set; }
        public decimal PaymentPercentage { get; set; }
        public DateTime MilestoneStartDate { get; set; }
        public DateTime MilestoneEndDate { get; set; }
        public DateTime MilestoneCompletedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PercentageCompleted { get; set; }
        public string MilestoneCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public decimal MilestoneAmount { get; set; }
        public decimal MilestoneActualAmount { get; set; }
        public bool MilestoneIsCompleted { get; set; }

        public string WorkTypeId { get; set; } = string.Empty;
        public string WorkType { get; set; } = string.Empty;
        public string SubWorkTypeId { get; set; } = string.Empty;
        public string SubWorkType { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string WorkNumber { get; set; } = string.Empty;
        public string WorkTemplateName { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string DivisionName { get; set; } = string.Empty;
        public string TenderNumber { get; set; } = string.Empty;

        public decimal? PannedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? PaymentValue { get; set; }
        public bool IsWaitingForPayment { get; set; }
        public bool IsNoAction { get; set; }
        public string MilestoneFile1Saved { get; set; } = string.Empty;
        public string MilestoneFile1Original { get; set; } = string.Empty;
        public string MilestoneFile2Saved { get; set; } = string.Empty;
        public string MilestoneFile2Original { get; set; } = string.Empty;
    }
}
