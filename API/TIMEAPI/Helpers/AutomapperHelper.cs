using AutoMapper;
using Model.DomainModel;
using Model.ViewModel;
using Newtonsoft.Json;

namespace TIMEAPI.Helpers
{
    public class AutomapperHelper : Profile
    {
        public AutomapperHelper()
        {
            // Account / Login

            CreateMap<LoginModel, LoginViewModel>();
            CreateMap<LoginViewModel, LoginModel>();

            CreateMap<AccountUserModel, AccountUserViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<AccountUserViewModel, AccountUserModel>();

            // TCC

            CreateMap<ConfigurationModel, ConfigurationViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<ConfigurationViewModel, ConfigurationModel>();

            CreateMap<ConfigCategoryModel, ConfigCategoryViewModel>();
            CreateMap<ConfigCategoryViewModel, ConfigCategoryModel>();

            // Role

            CreateMap<AccountRoleModel, AccountRoleViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<AccountRoleViewModel, AccountRoleModel>();

            CreateMap<AccountRolePrivilegeModel, AccountRolePrivilegeViewModel>();
            CreateMap<AccountRolePrivilegeViewModel, AccountRolePrivilegeModel>();

            CreateMap<AccountPrivilegeModel, AccountPrivilegeViewModel>();
            CreateMap<AccountPrivilegeViewModel, AccountPrivilegeModel>();

            // Role Privilege

            CreateMap<AccountPrivilegeSaveModel, AccountPrivilegeSaveViewModel>();
            CreateMap<AccountPrivilegeSaveViewModel, AccountPrivilegeSaveModel>();

            // Quick Link

            CreateMap<QuickLinkMasterModel, QuickLinkMasterViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<QuickLinkMasterViewModel, QuickLinkMasterModel>();

            // Template

            CreateMap<TemplateModel, TemplateViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.IsPublished ? "Published" : "In-Progress"));
            CreateMap<TemplateViewModel, TemplateModel>();

            CreateMap<TemplateMilestoneModel, TemplateMilestoneViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<TemplateMilestoneViewModel, TemplateMilestoneModel>();


            // Approval Flow

            CreateMap<ApprovalFlowMaster, ApprovalFlowViewMaster>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<ApprovalFlowViewMaster, ApprovalFlowMaster>();

            // Tender API

            CreateMap<AwardedTender, TenderMasterModel>()
               .ForMember(d => d.TenderNumber, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.tender_ref_number) ? "" : s.tender_ref_number))
               .ForMember(d => d.StartDate, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.go_date) ? default : Convert.ToDateTime(s.go_date)))
               .ForMember(d => d.EndDate, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.tender_awarded_date) ? default : Convert.ToDateTime(s.tender_awarded_date)))
               .ForMember(d => d.DivisionName, o => o.MapFrom(s => s.division_name))
               .ForMember(d => d.DistrictName, o => o.MapFrom(s => s.district_name))
               .ForMember(d => d.Class, o => o.MapFrom(s => s.class_name))
               .ForMember(d => d.Category, o => o.MapFrom(s => s.category_name))
               .ForMember(d => d.WorkValue, o => o.MapFrom(s => s.tender_boq_amount))
               .ForMember(d => d.MainCategory, o => o.MapFrom(s => s.work_type_main))
               .ForMember(d => d.Subcategory, o => o.MapFrom(s => s.work_type_sub))
               .ForMember(d => d.BidType, o => o.MapFrom(s => s.bid_type))
               .ForMember(d => d.ContractorName, o => o.MapFrom(s => s.contractor_name))
               .ForMember(d => d.ContractorDivisionName, o => o.MapFrom(s => s.con_division))
               .ForMember(d => d.ContractorDistrictName, o => o.MapFrom(s => s.con_district))
               .ForMember(d => d.ContractorDivision, o => o.MapFrom(s => s.con_division))
               .ForMember(d => d.ContractorDistrict, o => o.MapFrom(s => s.con_district))
               .ForMember(d => d.ContractorCategory, o => o.MapFrom(s => s.con_category))

               .ForMember(d => d.ContractorMobile, o => o.MapFrom(s => s.contractor_mobile))
               .ForMember(d => d.ContractorEmail, o => o.MapFrom(s => s.contractor_email))
               .ForMember(d => d.ContractorAddress, o => o.MapFrom(s => s.contractor_address))
               .ForMember(d => d.ContractorAltMobile, o => o.MapFrom(s => s.contractor_alt_mobile))

               .ForMember(d => d.ContractorCompanyAddress, o => o.MapFrom(s => s.contractor_companystreet1))
               .ForMember(d => d.ContractorCompanyName, o => o.MapFrom(s => s.contractor_companyname))
               .ForMember(d => d.ContractorCompanyPhone, o => o.MapFrom(s => s.contractor_company_mobile1))
               .ForMember(d => d.API_Responce, o => o.MapFrom(s => JsonConvert.SerializeObject(s)))

               .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.dept_name))
               .ForMember(d => d.DepartmentCode, o => o.MapFrom(s => s.dept_code))
               .ForMember(d => d.DepartmentId, o => o.MapFrom(s => s.dept_id_time))

               .ForMember(d => d.TenderFinalAmount, o => o.MapFrom(s => s.tender_go_amount))
               .ForMember(d => d.GoName, o => o.MapFrom(s => s.govt_order_name))
               .ForMember(d => d.GoNumber, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.go_number) ? "" : s.go_number))

               .ForMember(d => d.Workorder, o => o.MapFrom(s => s.workorder))
               .ForMember(d => d.Negotiation_signed_doc, o => o.MapFrom(s => s.negotiation_signed_doc))
               .ForMember(d => d.Others_doc, o => o.MapFrom(s => s.others_doc))
               .ForMember(d => d.SchemeId, o => o.MapFrom(s => s.TSchemeID))
               .ForMember(d => d.SchemeName, o => o.MapFrom(s => s.TSchemeName))
               .ForMember(d => d.SchemeCode, o => o.MapFrom(s => s.TSchemeCode))
               .ForMember(d => d.WorkSerialNumber, o => o.MapFrom(s => s.Workserialno))
               .ForMember(d => d.TenderOpenedDate, o => o.MapFrom(s => s.tender_opening_date))
               .ForMember(d => d.category_type_main, o => o.MapFrom(s => s.category_type_main))
               .ForMember(d => d.service_type_main, o => o.MapFrom(s => s.service_type_main))
               .ForMember(d => d.category_type_main_id, o => o.MapFrom(s => s.category_type_main_id))
               .ForMember(d => d.service_type_main_id, o => o.MapFrom(s => s.service_type_main_id))
               .ForMember(d => d.Go_package_No, o => o.MapFrom(s => s.gopackageNo));



            CreateMap<TenderMasterModel, AwardedTender>();

            CreateMap<AwardedTender, GOMasterModel>()
               .ForMember(d => d.GONumber, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.go_number) ? "" : s.go_number))
               .ForMember(d => d.GODate, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.go_date) ? default : Convert.ToDateTime(s.go_date)))
               .ForMember(d => d.GOCost, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.go_amount) ? 0 : Convert.ToDecimal(s.go_amount)))
               .ForMember(d => d.GOName, o => o.MapFrom(s => s.govt_order_name))
               .ForMember(d => d.GORevisedAmount, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.go_revised_amount) ? 0 : Convert.ToDecimal(s.go_revised_amount)))
               .ForMember(d => d.GODepartment, o => o.MapFrom(s => s.dept_name))
               .ForMember(d => d.DepartmentId, o => o.MapFrom(s => s.dept_id_time))
               .ForMember(d => d.DepartmentCode, o => o.MapFrom(s => s.dept_code))
               .ForMember(d => d.Go_package_count, o => o.MapFrom(s => s.go_package_count))
               .ForMember(d => d.Go_no_works, o => o.MapFrom(s => s.Go_no_works))
               
               .ForMember(d => d.GOTotalAmount, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.go_final_amount) ? 0 : Convert.ToDecimal(s.go_final_amount)));
            CreateMap<GOMasterModel, AwardedTender>();

            // Tender / GO

            CreateMap<GOMasterModel, GOMasterViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<GOMasterViewModel, GOMasterModel>();


            CreateMap<TenderMasterModel, TenderMasterViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<TenderMasterViewModel, TenderMasterModel>();

            // Work

            CreateMap<WorkMasterModel, WorkMasterViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<WorkMasterViewModel, WorkMasterModel>();

            CreateMap<WorkTemplateMasterModel, WorkTemplateMasterViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<WorkTemplateMasterViewModel, WorkTemplateMasterModel>();

            CreateMap<WorkTemplateMilestoneMasterModel, WorkTemplateMilestoneMasterViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<WorkTemplateMilestoneMasterViewModel, WorkTemplateMilestoneMasterModel>();

             
            // M-Book

            CreateMap<MBookMasterModel, MBookMasterViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedByUserName) ? s.CreatedByUserName : s.ModifiedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ModifiedBy) ? s.CreatedBy : s.ModifiedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.ModifiedDate ?? s.CreatedDate));
            CreateMap<MBookMasterViewModel, MBookMasterModel>();

            CreateMap<MBookApprovalHistoryModel, MBookApprovalHistoryViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.CreatedByUserName) ? "" : s.CreatedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.CreatedBy) ? "" : s.CreatedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.CreatedDate));
            CreateMap<MBookApprovalHistoryViewModel, MBookApprovalHistoryModel>();


            CreateMap<AlertConfigurationPrimaryModel, AlertConfigurationPrimaryViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.CreatedByUserName) ? "" : s.CreatedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.CreatedBy) ? "" : s.CreatedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.CreatedDate))
                .ForMember(d => d.SmsFrequency, o => o.MapFrom(s => s.SMSFrequency))
                .ForMember(d => d.SmsuserGroups, o => o.MapFrom(s => s.SMSuserGroups));
            CreateMap<AlertConfigurationPrimaryViewModel, AlertConfigurationPrimaryModel>();


            CreateMap<AlertConfigurationSecondaryModel, AlertConfigurationSecondaryViewModel>()
                .ForMember(d => d.LastUpdatedUserName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.CreatedByUserName) ? "" : s.CreatedByUserName))
                .ForMember(d => d.LastUpdatedBy, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.CreatedBy) ? "" : s.CreatedBy))
                .ForMember(d => d.LastUpdatedDate, o => o.MapFrom(s => s.CreatedDate));
            CreateMap<AlertConfigurationSecondaryViewModel, AlertConfigurationSecondaryModel>();
        }
    }
}
