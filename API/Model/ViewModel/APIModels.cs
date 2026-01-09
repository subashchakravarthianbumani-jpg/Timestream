using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class APIModels
    {
    }

    public class TenderAwardedRequestModel
    {
        public string Fdate { get; set; } = string.Empty;
        public string Tdate { get; set; } = string.Empty;
    }

    public class AwardedTender
    {
        public string tender_initiation_id { get; set; } = string.Empty;
        public string tender_awarded_date { get; set; } = string.Empty;
        public string class_name { get; set; } = string.Empty;
        public string category_name { get; set; } = string.Empty;
        public string tender_ref_number { get; set; } = string.Empty;
        public string tender_id { get; set; } = string.Empty;
        public string tender_slug { get; set; } = string.Empty;
        public string department_id { get; set; } = string.Empty;
        public string tender_govt_id { get; set; } = string.Empty;
        public string tender_title { get; set; } = string.Empty;
        public string tender_document_cost { get; set; } = string.Empty;
        public string tender_desciption { get; set; } = string.Empty;
        public string tender_emd { get; set; } = string.Empty;
        public string tender_announcement_date { get; set; } = string.Empty;
        public string tender_publication_date_on_portal { get; set; } = string.Empty;
        public string tender_go_number { get; set; } = string.Empty;
        public string tender_go_amount { get; set; } = string.Empty;
        public string tender_go_document { get; set; } = string.Empty;
        public string tender_go_date { get; set; } = string.Empty;
        public string tender_division { get; set; } = string.Empty;
        public string tender_district { get; set; } = string.Empty;
        public string tender_worktype { get; set; } = string.Empty;
        public string wt_batchid { get; set; } = string.Empty;
        public string work_type_main_cat_id { get; set; } = string.Empty;
        public string work_type_sub_cat_id { get; set; } = string.Empty;
        public string tender_boq_amount { get; set; } = string.Empty;
        public string tender_bid_type { get; set; } = string.Empty;
        public string tender_available_from_date { get; set; } = string.Empty;
        public string tender_available_to_date { get; set; } = string.Empty;
        public string tender_last_date_submission { get; set; } = string.Empty;
        public string tender_pre_bid_meeting_date { get; set; } = string.Empty;
        public string tender_pre_bid_meeting_invite_link { get; set; } = string.Empty;
        public string tender_financial_bid_opening_date { get; set; } = string.Empty;
        public string tender_financial_bid_end_date { get; set; } = string.Empty;
        public string tender_technical_bid_opening_date { get; set; } = string.Empty;
        public string tender_opening_date { get; set; } = string.Empty;
        public string tender_contact_name { get; set; } = string.Empty;
        public string tender_contact_designation { get; set; } = string.Empty;
        public string tender_contact_mobile { get; set; } = string.Empty;
        public string tender_contact_address { get; set; } = string.Empty;
        public string tender_contact_email { get; set; } = string.Empty;
        public string tender_boq_document { get; set; } = string.Empty;
        public string tender_document { get; set; } = string.Empty;
        public string tender_rfp_document { get; set; } = string.Empty;
        public string tender_paper_ad_document { get; set; } = string.Empty;
        public string tender_misc_document { get; set; } = string.Empty;
        public string tender_conditions_document { get; set; } = string.Empty;
        public string tender_name { get; set; } = string.Empty;
        public string tender_created_by { get; set; } = string.Empty;
        public string tender_created_at { get; set; } = string.Empty;
        public string tender_status { get; set; } = string.Empty;
        public string is_verified { get; set; } = string.Empty;
        public string is_published { get; set; } = string.Empty;
        public string contractor_id { get; set; } = string.Empty;
        public string contractor_community { get; set; } = string.Empty;
        public string contractor_caste { get; set; } = string.Empty;
        public string contractor_category { get; set; } = string.Empty;
        public string contractor_class { get; set; } = string.Empty;
        public string contractor_number { get; set; } = string.Empty;
        public string contractor_division { get; set; } = string.Empty;
        public string contractor_district { get; set; } = string.Empty;
        public string contractor_gender { get; set; } = string.Empty;
        public string contractor_companyname { get; set; } = string.Empty;
        public string contractor_company_reg_no { get; set; } = string.Empty;
        public string contractor_company_reg_date { get; set; } = string.Empty;
        public string contractor_company_contact { get; set; } = string.Empty;
        public string contractor_company_state { get; set; } = string.Empty;
        public string contractor_company_division { get; set; } = string.Empty;
        public string contractor_company_district { get; set; } = string.Empty;
        public string con_division { get; set; } = string.Empty;
        public string con_district { get; set; } = string.Empty;
        public string con_category { get; set; } = string.Empty;
        public string contractor_company_city { get; set; } = string.Empty;
        public string contractor_company_pincode { get; set; } = string.Empty;
        public string contractor_company_mobile1 { get; set; } = string.Empty;
        public string contractor_company_mobile2 { get; set; } = string.Empty;
        public string contractor_partnersname1 { get; set; } = string.Empty;
        public string contractor_partnersname2 { get; set; } = string.Empty;
        public string contractor_partnersname3 { get; set; } = string.Empty;
        public string contractor_companystreet1 { get; set; } = string.Empty;
        public string contractor_companystreet2 { get; set; } = string.Empty;
        public string contractor_name { get; set; } = string.Empty;
        public string contractor_partnersname { get; set; } = string.Empty;
        public string contractor_address { get; set; } = string.Empty;
        public string contractor_mobile { get; set; } = string.Empty;
        public string contractor_alt_mobile { get; set; } = string.Empty;
        public string contractor_email { get; set; } = string.Empty;
        public string contractor_solvency { get; set; } = string.Empty;
        public string contractor_tin { get; set; } = string.Empty;
        public string contractor_cin { get; set; } = string.Empty;
        public string contractor_pan { get; set; } = string.Empty;
        public string contractor_gstin { get; set; } = string.Empty;
        public string contractor_date_of_registration { get; set; } = string.Empty;
        public string contractor_community_ctf { get; set; } = string.Empty;
        public string contractor_community_ctf_number { get; set; } = string.Empty;
        public string contractor_eligibility { get; set; } = string.Empty;
        public string contract_division { get; set; } = string.Empty;
        public string contract_district { get; set; } = string.Empty;
        public string contractor_created_at { get; set; } = string.Empty;
        public string contractor_updated_at { get; set; } = string.Empty;
        public string contractor_created_by { get; set; } = string.Empty;
        public string contractor_updated_by { get; set; } = string.Empty;
        public string contractor_is_deleted { get; set; } = string.Empty;
        public string contractor_status { get; set; } = string.Empty;
        public string contractor_userid { get; set; } = string.Empty;
        public string contractor_registration_source { get; set; } = string.Empty;
        public string govt_order_id { get; set; } = string.Empty;
        public string govt_order_name { get; set; } = string.Empty;
        public string go_number { get; set; } = string.Empty;
        public string go_amount { get; set; } = string.Empty;
        public string go_date { get; set; } = string.Empty;
        public string go_final_amount { get; set; } = string.Empty;
        public string go_revised_amount { get; set; } = string.Empty;
        public string gopackageNo { get; set; } = string.Empty;
        public string go_package_count { get; set; } = string.Empty;
        public string Go_no_works { get; set; } = string.Empty;
        public string department_name { get; set; } = string.Empty;
        public string division_id { get; set; } = string.Empty;
        public string division_name { get; set; } = string.Empty;
        public string district_id { get; set; } = string.Empty;
        public string district_name { get; set; } = string.Empty;
        public string work_type_id { get; set; } = string.Empty;
        public string work_type_main { get; set; } = string.Empty;
        public string work_type_sub { get; set; } = string.Empty;
        public string bid_type { get; set; } = string.Empty;
        public string dept_id { get; set; } = string.Empty;
        public string dept_name { get; set; } = string.Empty;
        public string dept_code { get; set; } = string.Empty;
        public string dept_id_time { get; set; } = string.Empty;    
        public string workorder { get; set; } = string.Empty;    
        public string negotiation_signed_doc { get; set; } = string.Empty;    
        public string others_doc { get; set; } = string.Empty;

        public string TSchemeID { get; set; } = string.Empty;
        public string TSchemeName { get; set; } = string.Empty;

        public string TSchemeCode { get; set; } = string.Empty;

        public string Workserialno { get; set; } = string.Empty;
        public string category_type_main { get; set; } = string.Empty;
        public string service_type_main { get; set; } = string.Empty;
        public string category_type_main_id { get; set; } = string.Empty;
        public string service_type_main_id { get; set; } = string.Empty;
        public string contractor_tender_quote_final_amount { get; set; } = string.Empty;

    }

    public class AwardedTenderAPIResponce
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public List<AwardedTender>? data { get; set; }
    }
}
