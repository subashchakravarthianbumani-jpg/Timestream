using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "tender_master", keyFieldName: "Id")]
    public class TenderMasterModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string GoId { get; set; } = string.Empty;
        [LogField]
        public string TenderNumber { get; set; } = string.Empty;
        [LogField]
        public DateTime StartDate { get; set; }
        [LogField]
        public DateTime EndDate { get; set; }
        [LogField]
        public string Division { get; set; } = string.Empty;
        [LogField]
        public string District { get; set; } = string.Empty;
        [LogField]
        public string DivisionName { get; set; } = string.Empty;
        [LogField]
        public string DistrictName { get; set; } = string.Empty;
        [LogField]
        public string Class { get; set; } = string.Empty;
        [LogField]
        public string Category { get; set; } = string.Empty;
        [LogField]
        public string WorkValue { get; set; } = string.Empty;
        [LogField]
        public string MainCategory { get; set; } = string.Empty;
        [LogField]
        public string Subcategory { get; set; } = string.Empty;
        [LogField]
        public string BidType { get; set; } = string.Empty;
        [LogField]
        public string ContractorName { get; set; } = string.Empty;
        [LogField]
        public string ContractorDivision { get; set; } = string.Empty;
        [LogField]
        public string ContractorDistrict { get; set; } = string.Empty;
        [LogField]
        public string ContractorCategory { get; set; } = string.Empty;
        [LogField]
        public string ContractorMobile { get; set; } = string.Empty;
        [LogField]
        public string ContractorEmail { get; set; } = string.Empty;
        [LogField]
        public string ContractorAddress { get; set; } = string.Empty;
        [LogField]
        public string ContractorAltMobile { get; set; } = string.Empty;
        [LogField]
        public string ContractorId { get; set; } = string.Empty;
        [LogField]
        public string DepartmentName { get; set; } = string.Empty;
        [LogField]
        public string DepartmentCode { get; set; } = string.Empty;
        [LogField]
        public string DepartmentId { get; set; } = string.Empty;
        [LogField]
        public string TenderFinalAmount { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        public string LocalTenderNumber { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; }
        [LogField]
        public string TenderStatus { get; set; } = string.Empty;

        [LogField]
        public string ContractorCompanyName { get; set; } = string.Empty;
        [LogField]
        public string ContractorCompanyAddress { get; set; } = string.Empty;
        [LogField]
        public string ContractorCompanyPhone { get; set; } = string.Empty;

        [LogField]
        public string Workorder { get; set; } = string.Empty;
        [LogField]
        public string Negotiation_signed_doc { get; set; } = string.Empty;
        [LogField]
        public string Others_doc { get; set; } = string.Empty;

        public string API_Responce { get; set; } = string.Empty;

        public string GoNumber { get; set; } = string.Empty;
        public string GoName { get; set; } = string.Empty;
        public bool CanCreateWork { get; set; }
        public bool CanCreateTemplate { get; set; }
        public bool CanEditWork { get; set; }
        public decimal ActualAmount { get; set; }

        public string ContractorDivisionName { get; set; } = string.Empty;
        public string ContractorDistrictName { get; set; } = string.Empty;

        public string WorkId { get; set; } = string.Empty;
        public string WorkTemplateId { get; set; } = string.Empty;

        public string WorkType { get; set; } = string.Empty;
        public string SubWorkType { get; set; } = string.Empty;
        public string WorkAmount { get; set; } = string.Empty;
        public string WorkStatus { get; set; } = string.Empty;
        public string WorkProgress { get; set; } = string.Empty;

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

        public string SchemeId { get; set; } =string.Empty;
        public string SchemeName { get; set; } = string.Empty;

        public string SchemeCode { get; set; } = string.Empty;

        public string WorkSerialNumber { get; set; }= string.Empty;
        public string TenderOpenedDate { get; set; }= string.Empty;
        public string? WorkCommencementDate { get; set; }
        public string? WorkCompletionDate { get; set; }

        public string Go_package_No { get; set; } = string.Empty;
        public string category_type_main { get; set; } = string.Empty;
        public string service_type_main { get; set; } = string.Empty;
        public string category_type_main_id { get; set; } = string.Empty;
        public string service_type_main_id { get; set; } = string.Empty;


    }


    public static class TenderrecordsModel
    {
        //public static int InsertedTenderCount { get; set; }
        //public static List<string> InsertedTenderNumbers { get; set; } = new List<string>();

        public static int InsertedTenderCount { get; set; }
        public static int UpdatedTenderCount { get; set; }

        public static List<string> InsertedTenderNumbers { get; set; } = new List<string>();
        public static List<string> UpdatedTenderNumbers { get; set; } = new List<string>();
    }

}
