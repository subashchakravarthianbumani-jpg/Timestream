using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "work_master", keyFieldName: "Id")]
    public class WorkMasterModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string TenderId { get; set; } = string.Empty;
        [LogField]
        public string WorkOrderId { get; set; } = string.Empty;
        [LogField]
        public string AgreementCopyId { get; set; } = string.Empty;
        [LogField]
        public string WorkTemplateId { get; set; } = string.Empty;
        [LogField]
        public string CalenderLeaveTypes { get; set; } = string.Empty;
        [LogField]
        public string LetterOfAcceptanceId { get; set; } = string.Empty;
        [LogField]
        public string OtherFileId { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        public string WorkNumber { get; set; } = string.Empty;
        [LogField]
        public string GoId { get; set; } = string.Empty;
        [LogField]
        public string GoName { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public string WorkStatus { get; set; } = string.Empty;
        public string WorkStatusName { get; set; } = string.Empty;
        public string WorkProgress { get; set; } = string.Empty;

        public DateTime? WorkCommencementDate { get; set; }
        public DateTime? WorkCompletionDate { get; set; }
        public int? DateDifference { get; set; }
        public string SchemeName {  get; set; } = string.Empty;
        public string TenderOpenedDate {  get; set; } = string.Empty;
        public string WorkSerialNumber {  get; set; } = string.Empty;
        public string Go_Package_No {  get; set; } = string.Empty;

        public string category_type_main { get; set; } = string.Empty;
        public string service_type_main { get; set; } = string.Empty;
        public string category_type_main_id { get; set; } = string.Empty;
        public string service_type_main_id { get; set; } = string.Empty;



        public string TenderStatus { get; set; } = string.Empty;
        public decimal WorkValueIncreasedAmount { get; set; }

        // Tender

        public string TenderNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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
        public string ContractorId { get; set; } = string.Empty;
        public string TenderFinalAmount { get; set; } = string.Empty;
        public string LocalTenderNumber { get; set; } = string.Empty;

        public string ContractorCompanyName { get; set; } = string.Empty;
        public string ContractorCompanyAddress { get; set; } = string.Empty;
        public string ContractorCompanyPhone { get; set; } = string.Empty;
        public string API_Responce { get; set; } = string.Empty;

        public string Workorder { get; set; } = string.Empty;
        public string Negotiation_signed_doc { get; set; } = string.Empty;
        public string Others_doc { get; set; } = string.Empty;
        public string Days {  get; set; } = string.Empty;
        public bool IsVerified {  get; set; }


        // Go
        public string GONumber { get; set; } = string.Empty;
        public DateTime GODate { get; set; }
        public decimal GOCost { get; set; }
        public string GOName { get; set; } = string.Empty;
        public string GODepartment { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public decimal GORevisedAmount { get; set; }
        public decimal GOTotalAmount { get; set; }
        public string LocalGONumber { get; set; } = string.Empty;
        public int Go_no_works { get; set; } 
        public int Go_package_count { get; set; }

        // Template

        public string Work_Template_Id { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public string WorkId { get; set; } = string.Empty;
        public string WorkTemplateName { get; set; } = string.Empty;
        public string WorkTypeId { get; set; } = string.Empty;
        public int? WorkDurationInDays { get; set; }
        public string WorkTemplateNumber { get; set; } = string.Empty;
        public string WorkType { get; set; } = string.Empty;
        public string SubWorkTypeId { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;
        public string SubWorkType { get; set; } = string.Empty;



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
        public DateTime SavedDate { get; set; }

        public decimal? PannedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? PaymentValue { get; set; }
    }
}
