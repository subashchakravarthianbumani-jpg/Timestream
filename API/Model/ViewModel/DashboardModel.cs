using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    internal class DashboardModel
    {
    }

    public class TenderWorkBaseModel
    {
        // Tender
        public string TenderId { get;set; } = string.Empty;
        public string Division { get;set; } = string.Empty;
        public string District { get;set; } = string.Empty;
        public string DistrictId { get;set; } = string.Empty;
        public string DepartmentId { get;set; } = string.Empty;
        public string WorkTemplateId { get;set; } = string.Empty;
        public string WorkTypeId { get;set; } = string.Empty;
        public string ContractorId { get;set; } = string.Empty;
        public decimal TenderFinalAmount { get;set; }
        public decimal TenderAmount { get;set; }
        public decimal AmountSpend { get;set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? WorkStatus { get; set; }

        // Work
        public string WorkId { get;set; } = string.Empty;
        public bool IsActive { get;set; }
        public bool IsCompleted { get;set; }


        public string? SchemeId { get; set; } = string.Empty;
        public string? SchemeName { get; set; } = string.Empty;
    }

    public class MbookBaseModel
    {
        // Mbook
        public string MbookId { get; set; } = string.Empty;
        public string WorkTypeId { get; set; } = string.Empty;
        public string TenderId { get; set; } = string.Empty;
        public string ActionableRoleId { get; set; } = string.Empty;
        public string PaymentStatusId { get; set; } = string.Empty;
        public string PaymentStatusName { get; set; } = string.Empty;
        public string PaymentStatusCode { get; set; } = string.Empty;
        public string StatusId { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string DivisionId { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string DistrictId { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public decimal PaymentPercentage { get; set; }
        public decimal PercentageCompleted { get; set; }
        public decimal TenderAmount { get; set; }
        public decimal MbookAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public bool IsWaitingForPayment { get; set; }
    }
}
