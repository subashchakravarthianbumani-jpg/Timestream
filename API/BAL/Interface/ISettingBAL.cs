using DAL;
using Dapper;
using Model.DomainModel;
using Model.ViewModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface ISettingBAL
    {
        #region Two Column Configuration
        public List<ConfigurationModel> Configuration_Get(bool IsActive, string ConfigurationId = "", string CategoryId = "", string ParentConfigurationId = "", string Value = "", string CategoryCode = "", string DepartmentId = "");
        public List<ConfigCategoryModel> Configuration_Category_Get(string CategoryCode = "");
        public string Configuration_SaveUpdate(ConfigurationModel Configuration);
        public List<SelectListItem> GetUserGroupSelectList();
        public List<SelectListItem> GetDivisionSelectList();
        public List<SelectListItem> GetDistrictSelectList();
        #endregion Two Column Configuration

        #region Role

        public string Account_Role_Save(AccountRoleModel model);
        public List<AccountRoleModel> Account_Role_Get(string Id = "", bool IsActive = true, string RoleName = "", string RoleCode = "");

        #endregion Role


        #region Role Privilege
        public List<AccountPrivilegeByGroupModel> Account_Role_Privilege_Get(string RoleId);
        public List<AccountPrivilegeFormModel> Account_Role_Privilege_Login_Get(string RoleId);
        public List<AccountPrivilegeFormModel> Account_Role_Privilege_Get_All(string RoleId);
        public string Account_Role_Privilege_Save(AccountPrivilegeSaveModel model);
        #endregion Role Privilege

        #region User
        public List<AccountUserModel> User_Get(bool IsActive, string UserId = "", string DistrictId = "", string DivisionId = "", 
            string UserGroup = "", string RoleId = "", string MobileNumber = "", string Email = "", string DepartmentId = "");
        public List<UserNextNumberModel> User_GetNextNumber();
        public AccountUserModel User_SaveUpdate(AccountUserModel model);
        public string User_Activate(AccountUserModel model);
        public List<SelectListItem> GetDepartmentSelectList();
        public AccountUserFormDetailModel User_Form_Get();
        public List<AccountUserModel> User_Get(UserFilterModel model, string departmentId, out int TotalCount);
        #endregion User

        #region QuickLink
        public List<QuickLinkMasterModel> QuickLink_Get_For_View(string UserGroupId = "");
        public List<QuickLinkMasterModel> QuickLink_Get(bool IsActive = true, string Id = "", string FileType = "", string UserGroupId = "");
        public string QuickLink_SaveUpdate(QuickLinkMasterModel model);

        #endregion QuickLink

        #region WorkTemplate

        public List<TemplateModel> Template_Get(bool IsActive = true, string Id = "", string WorkTypeId = "", string DepartmentId = "",string subcategory="",string mainCategory = "", string serviceType = "", string categoryType = "");
        public string Template_SaveUpdate(TemplateModel model);
        public string Work_Template_Edit(TemplateViewModel model);
        public List<TemplateMilestoneModel> TemplateMilestone_Get(bool IsActive = true, string Id = "", string TemplateId = "");
        public string TemplateMilestone_SaveUpdate(TemplateMilestoneModel model);
        public List<SelectListItem> GetWorkTypeList();
        public List<SelectListItem> Template_ServiceTypeList();
        public List<SelectListItem> Template_CategoryTypeList();
        public string Template_Publish(TemplateModel model);

        #endregion WorkTemplate

        #region ApprovalFlow
        public List<SelectListItem> GetApprovalflowRoleIdList(string DepartmentId);
        public List<ApprovalFlowMaster> ApprovalFlow_Get(string DepartmentId, string RoleId = "");
        public string ApprovalFlow_Add_Role(ApprovalFlowAddRoleModel_API model);
        public string ApprovalFlow_Remove_Role(ApprovalFlowMaster model);
        public string ApprovalFlow_SaveUpdate(ApprovalFlowMaster model);

        #endregion ApprovalFlow

        #region Alert
        public AlertConfigurationFormModel Alert_Configuration_Form();
        public List<AlertConfigurationPrimaryModel> Alert_Primary_Get(string Id = "", string Type = "", string Object = "", string Name = "", string Department = "", bool IsActive = true);
        public string AlertPrimary_Config_SaveUpdate(AlertConfigurationPrimaryModel model);
        public List<AlertConfigurationSecondaryModel> Alert_Secondary_Get(string Id = "", string PrimaryId = "");
        public string AlertSecondary_Config_SaveUpdate(AlertConfigurationSecondaryModel model);
        #endregion Alert

        public List<StatusMaster> Status_Get(string StatusCode = "", string Id = "", string Type = "");
        public List<schemeModel> Scheme_GeT();
        public List<schemeModel> PacakgeNo_Get();
    }
}
