using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class TenderMasterViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string GoId { get; set; } = string.Empty;
        public string TenderNumber { get; set; } = string.Empty;

        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }

        [JsonIgnore]
        public DateTime StartDate { get; set; }

        [JsonIgnore]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("startDate")]
        public string StartDateFormatted =>
            StartDate.ToLocalTime().ToString("dd-MMM-yyyy hh:mm tt");

        [JsonPropertyName("endDate")]
        public string EndDateFormatted =>
            EndDate.ToLocalTime().ToString("dd-MMM-yyyy hh:mm tt");

        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string DivisionName { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string WorkValue { get; set; } = string.Empty;
        public string MainCategory { get; set; } = string.Empty;
        public string Subcategory { get; set; } = string.Empty;
        public string BidType { get; set; } = string.Empty;
        public string ContractorName { get; set; } = string.Empty;
        public string ContractorDivision { get; set; } = string.Empty;
        public string ContractorDistrict { get; set; } = string.Empty;
        public string ContractorCategory { get; set; } = string.Empty;
        public string ContractorMobile { get; set; } = string.Empty;
        public string ContractorEmail { get; set; } = string.Empty;
        public string ContractorAddress { get; set; } = string.Empty;
        public string ContractorAltMobile { get; set; } = string.Empty;
        public string TenderFinalAmount { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public string SchemeName {  get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        public string LocalTenderNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string TenderStatus { get; set; } = string.Empty;
        public string GoNumber { get; set; } = string.Empty;
        public string GoName { get; set; } = string.Empty;
        public bool CanCreateWork { get; set; }
        public bool CanCreateTemplate { get; set; }
        public bool CanEditWork { get; set; }

        public string WorkId { get; set; } = string.Empty;
        public string WorkTemplateId { get; set; } = string.Empty;

        public string WorkType { get; set; } = string.Empty;
        public string SubWorkType { get; set; } = string.Empty;
        public string WorkAmount { get; set; } = string.Empty;
        public string WorkStatus { get; set; } = string.Empty;
        public string WorkProgress { get; set; } = string.Empty;
        public decimal ActualAmount { get; set; }
        public decimal TenderRevisedAmount { get; set; }

        public string ContractorCompanyName { get; set; } = string.Empty;
        public string ContractorCompanyAddress { get; set; } = string.Empty;
        public string ContractorCompanyPhone { get; set; } = string.Empty;
        public string API_Responce { get; set; } = string.Empty;

        public TemplateMilestoneViewModel? Milestones { get; set; }

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime? LastUpdatedDate { get; set; }

        public decimal? PannedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? PaymentValue { get; set; }
        public string? WorkCommencementDate { get; set; } 
        public string? WorkCompletionDate { get; set; } 
    }
}
