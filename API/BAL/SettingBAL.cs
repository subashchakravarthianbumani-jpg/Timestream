using AutoMapper;
using BAL.Interface;
using DAL;
using Dapper;
using Microsoft.Extensions.Configuration;
using Model.Constants;
using Model.CustomeAttributes;
using Model.DomainModel;
using Model.ViewModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Interface;

namespace BAL
{
    public class SettingBAL : ISettingBAL
    {
        private readonly SettingsDAL _settingDAL;
        private readonly GeneralDAL _generalDAL;
        private readonly IMapper _mapper;
        private readonly IFTPHelpers _fTPHelpers;

        public SettingBAL(IMySqlDapperHelper mySqlDapperHelper, IMySqlHelper mySqlHelper, IMapper mapper, IConfiguration configuration, IFTPHelpers fTPHelpers)
        {
            _settingDAL = new SettingsDAL(mySqlDapperHelper, mySqlHelper, configuration);
            _generalDAL = new GeneralDAL(mySqlDapperHelper, mySqlHelper, configuration);
            _mapper = mapper;
            _fTPHelpers = fTPHelpers;
        }

        #region Two Column Configuration
        public List<ConfigurationModel> Configuration_Get(bool IsActive, string ConfigurationId = "", string CategoryId = "", string ParentConfigurationId = "", string Value = "", string CategoryCode = "", string DepartmentId = "")
        {
            return _settingDAL.Configuration_Get(IsActive, ConfigurationId, CategoryId, ParentConfigurationId, Value, CategoryCode, DepartmentId);
        }
        public List<ConfigCategoryModel> Configuration_Category_Get(string CategoryCode = "")
        {
            return _settingDAL.Configuration_Category_Get(CategoryCode);
        }
        public string Configuration_SaveUpdate(ConfigurationModel Configuration)
        {
            #region Save Difference
            ConfigurationModel? exist = Configuration_Get(true, ConfigurationId: Configuration.Id)?.FirstOrDefault() ?? new ConfigurationModel();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = Configuration_Get(false, ConfigurationId: Configuration.Id)?.FirstOrDefault() ?? new ConfigurationModel();
            }
            ObjectDifference diff = new ObjectDifference(Configuration, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<ConfigurationModel, LogFieldAttribute>();
            if (Configuration.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = Configuration.SavedBy;
            diff.SavedByUserName = Configuration.SavedByUserName;
            diff.SavedDate = Configuration.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _settingDAL.Configuration_SaveUpdate(Configuration);
        }
        #endregion Two Column Configuration

        #region Role

        public string Account_Role_Save(AccountRoleModel model)
        {
            #region Save Difference
            AccountRoleModel exist = Account_Role_Get(model.Id)?.FirstOrDefault() ?? new AccountRoleModel();
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<AccountRoleModel, LogFieldAttribute>();
            if (model.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _settingDAL.Account_Role_Save(model);
        }
        public List<AccountRoleModel> Account_Role_Get(string Id = "", bool IsActive = true, string RoleName = "", string RoleCode = "")
        {
            return _settingDAL.Account_Role_Get(Id, IsActive, RoleName, RoleCode);
        }

        #endregion Role

        #region Role Privilege
        public List<AccountPrivilegeFormModel> Account_Role_Privilege_Raw_Get(string RoleId, string PrivilegeId)
        {
            return _settingDAL.Account_Role_Privilege_Get(RoleId, PrivilegeId);
        }
        public List<AccountPrivilegeByGroupModel> Account_Role_Privilege_Get(string RoleId)
        {
            List<AccountPrivilegeFormModel> privilegeList = _settingDAL.Account_Role_Privilege_Get(RoleId);

            List<AccountPrivilegeByGroupModel> privilegeListGrouped = privilegeList
                .GroupBy(x => x.ModuleName)
                .Select(y => new AccountPrivilegeByGroupModel()
                {
                    RoleId = RoleId,
                    ModuleName = y.Key,
                    Privilege = y.OrderBy(x => x.OrderNumber).ToList()
                }).ToList();

            return privilegeListGrouped;
        }
        public List<AccountPrivilegeFormModel> Account_Role_Privilege_Login_Get(string RoleId)
        {
            List<AccountPrivilegeFormModel> privilegeList = _settingDAL.Account_Role_Privilege_Get(RoleId);
            privilegeList = privilegeList.Where(x => x.IsSelected == true).ToList();
            return privilegeList;
        }
        public List<AccountPrivilegeFormModel> Account_Role_Privilege_Get_All(string RoleId)
        {
            List<AccountPrivilegeFormModel> privilegeList = _settingDAL.Account_Role_Privilege_Get(RoleId);
            return privilegeList;
        }
        public string Account_Role_Privilege_Save(AccountPrivilegeSaveModel model)
        {
            #region Save Difference
            AccountPrivilegeFormModel? exist = Account_Role_Privilege_Raw_Get(model.RoleId, model.PrivilegeId)?.FirstOrDefault() ?? new AccountPrivilegeFormModel();
            if (string.IsNullOrWhiteSpace(exist.PrivilegeId))
            {
                exist = Account_Role_Privilege_Raw_Get(model.RoleId, model.PrivilegeId)?.FirstOrDefault() ?? new AccountPrivilegeFormModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<ConfigurationModel, LogFieldAttribute>();
            diff.IsDeleted = false;
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _settingDAL.Account_Role_Privilege_Save(model);
        }
        #endregion Role Privilege

        #region User
        public List<AccountUserModel> User_Get(bool IsActive, string UserId = "", string DistrictId = "", string DivisionId = "",
            string UserGroup = "", string RoleId = "", string MobileNumber = "", string Email = "", string DepartmentId = "")
        {
            List<AccountUserModel> list = _settingDAL.User_Get(IsActive, UserId, DistrictId, DivisionId, UserGroup, RoleId, MobileNumber, Email, DepartmentId);
            list.ForEach(x =>
            {
                x.Password = EncryptDecrypt.Decrypt(x.Password);

                if (!string.IsNullOrWhiteSpace(x.DivisionId))
                {
                    x.DivisionIdList = x.DivisionId.Split(',').ToList();
                }
                if (!string.IsNullOrWhiteSpace(x.DepartmentId))
                {
                    x.DepartmentIdList = x.DepartmentId.Split(',').ToList();
                }
                if (!string.IsNullOrWhiteSpace(x.DistrictId))
                {
                    x.DistrictIdList = x.DistrictId.Split(',').ToList();
                }
            });
            return list;
        }
        public List<AccountUserModel> User_Get(UserFilterModel model, string departmentId, out int TotalCount)
        {
            List<AccountUserModel> list = _settingDAL.User_Get(model, departmentId, out TotalCount);
            list.ForEach(x =>
            {
                x.Password = EncryptDecrypt.Decrypt(x.Password);

                if (!string.IsNullOrWhiteSpace(x.DivisionId))
                {
                    x.DivisionIdList = x.DivisionId.Split(',').ToList();
                }
                if (!string.IsNullOrWhiteSpace(x.DepartmentId))
                {
                    x.DepartmentIdList = x.DepartmentId.Split(',').ToList();
                }
                if (!string.IsNullOrWhiteSpace(x.DistrictId))
                {
                    x.DistrictIdList = x.DistrictId.Split(',').ToList();
                }
            });
            return list;
        }
        public List<UserNextNumberModel> User_GetNextNumber()
        {
            return _settingDAL.User_GetNextNumber();
        }
        public AccountUserModel User_SaveUpdate(AccountUserModel model)
        {
            #region Save Difference
            AccountUserModel exist = User_Get(true, UserId: model.UserId)?.FirstOrDefault() ?? new AccountUserModel();
            if (string.IsNullOrWhiteSpace(exist.UserId))
            {
                exist = User_Get(false, UserId: model.UserId)?.FirstOrDefault() ?? new AccountUserModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<AccountUserModel, LogFieldAttribute>();
            if (model.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            //if (string.IsNullOrWhiteSpace(exist.UserId))
            //{
            //    if (string.IsNullOrWhiteSpace(model.Password))
            //    {
            //        model.Password = EncryptDecrypt.Encrypt(StringFunctions.CreateRandomPassword(8));
            //    }
            //}
            //else
            //{
            //    model.Password = exist.Password;
            //}

            model.UserId = _settingDAL.User_SaveUpdate(model);

            return model;
        }
        public string User_Activate(AccountUserModel model)
        {
            return _settingDAL.User_Activate(model);
        }
        public List<SelectListItem> GetDepartmentSelectList()
        {
            return _settingDAL.Configuration_Get(CategoryCode: ConfigurationCategory.Department)?.Select(x =>
            new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false })?.OrderBy(o => o.Text)?.ToList() ?? new List<SelectListItem>();
        }
        public List<SelectListItem> GetUserGroupSelectList()
        {
            return _settingDAL.Configuration_Get(CategoryCode: ConfigurationCategory.UserGroup)?.Select(x =>
            new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false })?.OrderBy(o => o.Text)?.ToList() ?? new List<SelectListItem>();
        }
        public List<SelectListItem> GetDivisionSelectList()
        {
            return _settingDAL.Configuration_Get(CategoryCode: ConfigurationCategory.Division)?.Select(x =>
            new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false })?.OrderBy(o => o.Text)?.ToList() ?? new List<SelectListItem>();
        }
        public List<SelectListItem> GetDistrictSelectList()
        {
            return _settingDAL.Configuration_Get(CategoryCode: ConfigurationCategory.District)?.Select(x =>
            new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false })?.OrderBy(o => o.Text)?.ToList() ?? new List<SelectListItem>();
        }

        public AccountUserFormDetailModel User_Form_Get()
        {
            AccountUserFormDetailModel model = new AccountUserFormDetailModel();

            model.DistrictList = _settingDAL.Configuration_Get(CategoryCode: ConfigurationCategory.District)?.Select(x =>
            new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false })?.OrderBy(o => o.Text)?.ToList() ?? new List<SelectListItem>();
            model.DivisionList = _settingDAL.Configuration_Get(CategoryCode: ConfigurationCategory.Division)?.Select(x =>
            new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false })?.OrderBy(o => o.Text)?.ToList() ?? new List<SelectListItem>();
            model.UserGroupList = _settingDAL.Configuration_Get(CategoryCode: ConfigurationCategory.UserGroup)?.Select(x =>
            new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false })?.OrderBy(o => o.Text)?.ToList() ?? new List<SelectListItem>();
            model.DepartmentList = _settingDAL.Configuration_Get(CategoryCode: ConfigurationCategory.Department)?.Select(x =>
            new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false })?.Where(d => d.Text != "General")?.OrderBy(o => o.Text)?.ToList() ?? new List<SelectListItem>();

            return model;
        }
        #endregion User

        #region QuickLink
        public List<QuickLinkMasterModel> QuickLink_Get_For_View(string UserGroupId = "")
        {
            List<QuickLinkMasterModel> list = _settingDAL.QuickLink_Get(UserGroupId: UserGroupId);
            list.ForEach(x =>
            {
                x.UserGroupIds = "";
            });
            return list;
        }
        public List<QuickLinkMasterModel> QuickLink_Get(bool IsActive = true, string Id = "", string FileType = "", string UserGroupId = "")
        {
            List<QuickLinkMasterModel> list = _settingDAL.QuickLink_Get(IsActive, Id, FileType, UserGroupId);
            list.ForEach(x =>
            {
                x.UserGroupIdList = x.UserGroupIds.Split(',').ToList();
            });
            return list;
        }
        public string QuickLink_SaveUpdate(QuickLinkMasterModel model)
        {
            #region Save Difference
            QuickLinkMasterModel? exist = QuickLink_Get(true, Id: model.Id)?.FirstOrDefault() ?? new QuickLinkMasterModel();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = QuickLink_Get(false, model.Id)?.FirstOrDefault() ?? new QuickLinkMasterModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<QuickLinkMasterModel, LogFieldAttribute>();
            if (model.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            if (model.UserGroupIdList?.Count > 0)
            {
                model.UserGroupIds = string.Join(",", model.UserGroupIdList).ToString();
            }

            return _settingDAL.QuickLink_SaveUpdate(model);
        }

        #endregion QuickLink

        #region WorkTemplate

        public List<TemplateModel> Template_Get(bool IsActive = true, string Id = "", string WorkTypeId = "", string DepartmentId = "",string subcategory = "", string mainCategory = "", string serviceType = "", string categoryType = "")
        {
            return _settingDAL.Template_Get(IsActive, Id, WorkTypeId, DepartmentId, subcategory, mainCategory, serviceType, categoryType);
        }
        public string Template_SaveUpdate(TemplateModel model)
        {
            #region Save Difference
            TemplateModel? exist = Template_Get(true, Id: model.Id)?.FirstOrDefault() ?? new TemplateModel();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = Template_Get(false, model.Id)?.FirstOrDefault() ?? new TemplateModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<TemplateModel, LogFieldAttribute>();
            if (model.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _settingDAL.Template_SaveUpdate(model);
        }
        public List<TemplateMilestoneModel> TemplateMilestone_Get(bool IsActive = true, string Id = "", string TemplateId = "")
        {
            return _settingDAL.TemplateMilestone_Get(IsActive, Id, TemplateId);
        }
        public string TemplateMilestone_SaveUpdate(TemplateMilestoneModel model)
        {
            return _settingDAL.TemplateMilestone_SaveUpdate(model);
        }

        public string Work_Template_Edit(TemplateViewModel model)
        {
            return _settingDAL.Work_Template_Edit(model);
        }

        public List<SelectListItem> GetWorkTypeList()
        {
            return _settingDAL.Configuration_Get(CategoryCode: "WORKTYPE")?.Select(x => new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false }).ToList() ?? new List<SelectListItem>();
        }

        public List<SelectListItem> Template_ServiceTypeList()
        {
            return _settingDAL.Configuration_Get(CategoryCode: "SERVICETYPE")?.Select(x => new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false }).ToList() ?? new List<SelectListItem>();
        }
        public List<SelectListItem> Template_CategoryTypeList()
        {
            return _settingDAL.Configuration_Get(CategoryCode: "CATEGORYTYPE")?.Select(x => new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false }).ToList() ?? new List<SelectListItem>();
        }
        public string Template_Publish(TemplateModel model)
        {
            return _settingDAL.Template_Publish(model);
        }
        #endregion WorkTemplate

        #region ApprovalFlow

        public List<SelectListItem> GetApprovalflowRoleIdList(string DepartmentId)
        {
            List<SelectListItem> list = _settingDAL.Account_Role_Get(IsActive: true).Select(x => new SelectListItem() { Text = x.RoleName, Value = x.Id }).ToList();

            List<string> approvalflow_list = _settingDAL.ApprovalFlow_Get(DepartmentId)?.Select(x => x.RoleId)?.ToList() ?? new List<string>();

            list.ForEach(x =>
            {
                if (approvalflow_list.Contains(x.Value))
                {
                    x.Selected = true;
                }
                else
                {
                    x.Selected = false;
                }
            });

            return list;
        }
        public List<ApprovalFlowMaster> ApprovalFlow_Get(string DepartmentId, string RoleId = "")
        {
            return _settingDAL.ApprovalFlow_Get(DepartmentId, RoleId);
        }
        public string ApprovalFlow_Add_Role(ApprovalFlowAddRoleModel_API model)
        {
            return _settingDAL.ApprovalFlow_Add_Role(model);
        }
        public string ApprovalFlow_Remove_Role(ApprovalFlowMaster model)
        {
            return _settingDAL.ApprovalFlow_Remove_Role(model);
        }
        public string ApprovalFlow_SaveUpdate(ApprovalFlowMaster model)
        {
            #region Save Difference
            ApprovalFlowMaster? exist = ApprovalFlow_Get(model.DepartmentId, model.RoleId)?.FirstOrDefault() ?? new ApprovalFlowMaster();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = ApprovalFlow_Get(model.DepartmentId, model.RoleId)?.FirstOrDefault() ?? new ApprovalFlowMaster();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<ApprovalFlowMaster, LogFieldAttribute>();
            diff.IsDeleted = false;
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _settingDAL.ApprovalFlow_SaveUpdate(model);
        }

        #endregion ApprovalFlow

        #region Alert
        public AlertConfigurationFormModel Alert_Configuration_Form()
        {
            AlertConfigurationFormModel model = new AlertConfigurationFormModel();

            model.FieldList = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "Work Start date", Value = AlertConfigConstants.Field_WorkStartDate },
                new SelectListItem(){ Text = "Work end date", Value = AlertConfigConstants.Field_WorkEndDate },
                new SelectListItem(){ Text = "Work Total Cost", Value = AlertConfigConstants.Field_WorkTotalCost },

                new SelectListItem(){ Text = "Milestone Start Date", Value = AlertConfigConstants.Field_MilestoneStartDate },
                new SelectListItem(){ Text = "Milestone End Date", Value = AlertConfigConstants.Field_MilestoneEndDate },
               // new SelectListItem(){ Text = "Milestone Progress", Value = AlertConfigConstants.Field_MilestoneProgress },

                new SelectListItem(){ Text = " Mbook(Active) Submitted date", Value = AlertConfigConstants.Field_ActiveMbookSubmittedDate }
            };

            model.TypeList = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "Schedule", Value = AlertConfigConstants.Type_Schedule },
                new SelectListItem(){ Text = "Cost", Value = AlertConfigConstants.Type_Cost },
                new SelectListItem(){ Text = "Approval", Value = AlertConfigConstants.Type_Approval }
            };

            model.ObjectList = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "Work", Value = AlertConfigConstants.Object_Work },
                new SelectListItem(){ Text = "Milestone", Value = AlertConfigConstants.Object_Milestone },
                new SelectListItem(){ Text = "Mbook", Value = AlertConfigConstants.Object_Mbook }
            };

            model.SeverityList = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "Red", Value = AlertConfigConstants.Severity_Red },
                new SelectListItem(){ Text = "Yellow", Value = AlertConfigConstants.Severity_Yellow }
            };

            model.BaseFieldList = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "Current Date", Value = AlertConfigConstants.BaseField_CurrentDate },
                //new SelectListItem(){ Text = "Current Date or actual Completion Date", Value = AlertConfigConstants.BaseField_CurrentDateOrActualCompletionDate },
                //new SelectListItem(){ Text = "Current date and Milestone End Date", Value = AlertConfigConstants.BaseField_CurrentDateAndMilestoneEndDate },
                //new SelectListItem(){ Text = "Current Date or Mbook Submitted Date", Value = AlertConfigConstants.BaseField_CurrentDateAndMbookSubmittedDate },
                new SelectListItem(){ Text = "Work Cost", Value = AlertConfigConstants.BaseField_WorkCost }
            };

            model.CalculationTypeList = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = ">", Value = AlertConfigConstants.CalculationType_GreaterThan },
                new SelectListItem(){ Text = ">=", Value = AlertConfigConstants.CalculationType_GreaterThanEqual },
                new SelectListItem(){ Text = "<", Value = AlertConfigConstants.CalculationType_LessThan },
                new SelectListItem(){ Text = "<=", Value = AlertConfigConstants.CalculationType_LessThanEqual },
            };

            model.FrequencyTypeList = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "Daily", Value = AlertConfigConstants.Frequency_Daily }
            };

            model.UserGroupList = _settingDAL.Configuration_Get(CategoryCode: ConfigurationCategory.UserGroup)?.Select(x =>
            new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false })?.OrderBy(o => o.Text)?.ToList() ?? new List<SelectListItem>();

            model.DepartmentList = _settingDAL.Configuration_Get(CategoryCode: ConfigurationCategory.Department)?.Select(x =>
            new SelectListItem() { Text = x.Value, Value = x.Id, Selected = false })?.OrderBy(o => o.Text)?.ToList() ?? new List<SelectListItem>();

            return model;
        }
        public List<AlertConfigurationPrimaryModel> Alert_Primary_Get(string Id = "", string Type = "", string Object = "", string Name = "", string Department = "", bool IsActive = true)
        {
            return _settingDAL.Alert_Primary_Get(Id, Type, Object, Name, Department, IsActive);
        }
        public string AlertPrimary_Config_SaveUpdate(AlertConfigurationPrimaryModel model)
        {
            #region Save Difference
            AlertConfigurationPrimaryModel? exist = Alert_Primary_Get(Id: model.Id, IsActive: true)?.FirstOrDefault() ?? new AlertConfigurationPrimaryModel();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = Alert_Primary_Get(Id: model.Id, IsActive: false)?.FirstOrDefault() ?? new AlertConfigurationPrimaryModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<AlertConfigurationPrimaryModel, LogFieldAttribute>();
            if (model.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _settingDAL.AlertPrimary_Config_SaveUpdate(model);
        }
        public List<AlertConfigurationSecondaryModel> Alert_Secondary_Get(string Id = "", string PrimaryId = "")
        {
            return _settingDAL.Alert_Secondary_Get(Id, PrimaryId);
        }
        public string AlertSecondary_Config_SaveUpdate(AlertConfigurationSecondaryModel model)
        {
            #region Save Difference
            AlertConfigurationSecondaryModel? exist = Alert_Secondary_Get(model.Id)?.FirstOrDefault() ?? new AlertConfigurationSecondaryModel();
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<AlertConfigurationSecondaryModel, LogFieldAttribute>();
            diff.IsDeleted = false;
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _settingDAL.AlertSecondary_Config_SaveUpdate(model);
        }
        #endregion Alert

        public List<StatusMaster> Status_Get(string StatusCode = "", string Id = "", string Type = "")
        {
            return _settingDAL.Status_Get(StatusCode, Id, Type);
        }
        public List<schemeModel> Scheme_GeT()
        {
         return _settingDAL.Scheme_Get();
        }
        public List<schemeModel> PacakgeNo_Get()
        {
            return _settingDAL.PacakgeNo_Get();
        }
    }
}
