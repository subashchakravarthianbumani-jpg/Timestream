using Model.CustomeAttributes;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "work_mbook_master", keyFieldName: "Id")]
    public class MBookMasterModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string WorkTemplateMilestoneId { get; set; } = string.Empty;
        [LogField]
        public string PaymentStatusId { get; set; } = string.Empty;
        [LogField]
        public string PaymentStatusName { get; set; } = string.Empty;
        [LogField]
        public string PaymentStatusCode { get; set; } = string.Empty;
        [LogField]
        public string StatusId { get; set; } = string.Empty;
        [LogField]
        public string StatusName { get; set; } = string.Empty;
        [LogField]
        public string StatusCode { get; set; } = string.Empty;
        [LogField]
        public decimal Length { get; set; }
        [LogField]
        public decimal Width { get; set; }
        [LogField]
        public decimal Heigth { get; set; }
        [LogField]
        public string ActionableRoleId { get; set; } = string.Empty;
        [LogField]
        public string DivisionId { get; set; } = string.Empty;
        [LogField]
        public string MilestoneName { get; set; } = string.Empty;
        [LogField]
        public string WorkTemplateId { get; set; } = string.Empty;
        [LogField]
        public string WorkId { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; } = true;
        [LogField]
        public bool? IsReturned { get; set; }

        [LogField]
        public string WorkNotes { get; set;} = string.Empty;
        [LogField]
        public DateTime? Date { get; set; }
        [LogField]
        public DateTime? SubmittedDate { get; set; }
        [LogField]
        public DateTime? CompletedDate { get; set; }

        [LogField]
        public decimal ActualAmount { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        public string MBookNumber { get; set; } = string.Empty;
        public string MBookId { get; set; } = string.Empty;
        public DateTime? LastApprovedDate { get; set; }

        public bool IsActionable { get; set; }
        public bool IsEditable { get; set; }
        public bool IsSubmitted { get; set; }
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

        public int DurationInDays { get; set; }
        public decimal PaymentPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string MilestoneStatus { get; set; } = string.Empty;

        public List<FileMasterModel>? Files { get; set; }
        public List<MBookMasterModel>? RejectedMbooks { get; set; }

        public string Strength { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;
        public string MileStoneId { get; set; } = string.Empty;
        public string MilestoneCode { get; set; } = string.Empty;
        public decimal MilestoneAmount { get; set; }
        public bool IsCompleted { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByUserName { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string DeletedBy { get; set; } = string.Empty;
        public string DeletedByUserName { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public string SavedByRoleName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }

        public decimal? PannedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? PaymentValue { get; set; }
    }
}
