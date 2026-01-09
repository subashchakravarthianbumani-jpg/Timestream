using Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class MBookMasterViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string WorkTemplateMilestoneId { get; set; } = string.Empty;
        public string PaymentStatusId { get; set; } = string.Empty;
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Heigth { get; set; }
        public string ActionableRoleId { get; set; } = string.Empty;
        public string DivisionId { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string PaymentStatusName { get; set; } = string.Empty;
        public string PaymentStatusCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsActionable { get; set; }
        public bool IsEditable { get; set; }
        public bool IsSubmitted { get; set; }
        public string WorkNotes { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        public string MBookNumber { get; set; } = string.Empty;
        public decimal ActualAmount { get; set; }

        public string MilestoneName { get; set; } = string.Empty;
        public string WorkTemplateId { get; set; } = string.Empty;

        public string TenderNumber { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string DivisionName { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string GONumber { get; set; } = string.Empty;
        public string GODepartment { get; set; } = string.Empty;
        public string GOName { get; set; } = string.Empty;

        public decimal PercentageCompleted { get; set; }
        public string WorkType { get; set; } = string.Empty;
        public string SubWorkType { get; set; } = string.Empty;

        public List<FileMasterModel>? Files { get; set; }
        public List<MBookApprovalHistoryViewModel>? ApprovalHistory { get; set; }
        public List<MBookMasterViewModel>? RejectedMbooks { get; set; }

        public int DurationInDays { get; set; }
        public decimal PaymentPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string MilestoneStatus { get; set; } = string.Empty;

        public string Strength { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;
        public string MileStoneId { get; set; } = string.Empty;
        public string MilestoneCode { get; set; } = string.Empty;
        public decimal MilestoneAmount { get; set; }
        public bool IsCompleted { get; set; }

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime? LastUpdatedDate { get; set; }

        public decimal? PannedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? PaymentValue { get; set; }
    }
}
