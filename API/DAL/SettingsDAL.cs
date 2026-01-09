using Dapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Model.Constants;
using Model.DomainModel;
using Model.ViewModel;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utils;
using Utils.Interface;

namespace DAL
{
    public class SettingsDAL
    {
        private readonly IMySqlHelper _mySqlHelper;
        private readonly IConfiguration _configuration;
        private readonly IMySqlDapperHelper _mySqlDapperHelper;

        private readonly string connectionId = "Default";
        public SettingsDAL(IMySqlDapperHelper mySqlDapperHelper, IMySqlHelper mySqlHelper, IConfiguration configuration)
        {
            _mySqlHelper = mySqlHelper;
            _configuration = configuration;
            _mySqlDapperHelper = mySqlDapperHelper;
        }

        #region Two Column Configuration

        public List<ConfigurationModel> Configuration_Get(bool IsActive = true, string ConfigurationId = "", string CategoryId = "", string ParentConfigurationId = "", string Value = "", string CategoryCode = "", string DepartmentId = "", string Code = "")
        {
            dynamic @params = new
            {
                pId = ConfigurationId?.Trim() ?? "",
                pIsActive = IsActive,
                pCategoryId = CategoryId?.Trim() ?? "",
                pDepartmentId = DepartmentId?.Trim() ?? "",
                pCategoryCode = CategoryCode?.Trim() ?? "",
                pConfigurationId = ParentConfigurationId?.Trim() ?? "",
                pValue = Value?.Trim() ?? "",
                pCode = Code?.Trim() ?? ""
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<ConfigurationModel>(connection, "Two_Column_Configuration_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<ConfigurationModel>();
        }
        public List<ConfigCategoryModel> Configuration_Category_Get(string CategoryCode = "")
        {
            dynamic @params = new
            {
                pCategoryCode = CategoryCode?.Trim() ?? ""
            };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<ConfigCategoryModel>(connection, "Two_Column_Configuration_Category_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<ConfigCategoryModel>();
        }

        public List<ConfigurationModel> Scheme_Category_Get(bool IsActive = true)
        {
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            string sql = @"
        SELECT 
     ifnull(SchemeName,'Others') AS SchemeName,
      ifnull(SchemeCode,'Others') AS SchemeCode,
       ifnull(SchemeId,'Others') AS SchemeId,
 COUNT(*) AS Count
 FROM tender_master
 WHERE IsActive = 1
 GROUP BY SchemeName, SchemeCode, SchemeId";

            return connection.Query<ConfigurationModel>(sql, new { IsActive }).ToList();
        }

        public string Configuration_SaveUpdate(ConfigurationModel Configuration)
        {
            dynamic @params = new
            {
                pId = Configuration.Id,
                pCategoryId = Configuration.CategoryId,
                pConfigurationId = Configuration.ConfigurationId,
                pDepartmentId = Configuration.DepartmentId ?? "",
                pValue = Configuration.Value,
                pCode = Configuration.Code ?? "",
                pIsActive = Configuration.IsActive,
                pSavedBy = Configuration.SavedBy,
                pSavedByUserName = Configuration.SavedByUserName,
                pSavedDate = Configuration.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Two_Column_Configuration_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }

        #endregion Two Column Configuration

        #region Role

        public string Account_Role_Save(AccountRoleModel model)
        {
            dynamic @params = new
            {
                pRoleId = model.Id,
                pRoleName = model.RoleName,
                pRoleCode = model.RoleCode,
                pIsActive = model.IsActive,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Account_Role_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }

        public List<AccountRoleModel> Account_Role_Get(string RoleId = "", bool IsActive = true, string RoleName = "", string RoleCode = "")
        {
            dynamic @params = new
            {
                pId = RoleId?.Trim() ?? "",
                pIsActive = IsActive,
                pRoleName = RoleName?.Trim() ?? "",
                pRoleCode = RoleCode?.Trim() ?? ""
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<AccountRoleModel>(connection, "Account_Role_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<AccountRoleModel>();
        }
        #endregion Role

        #region Role Privilege
        public List<AccountPrivilegeFormModel> Account_Role_Privilege_Get(string RoleId, string PrivilegeId = "")
        {
            dynamic @params = new
            {
                pRoleId = RoleId?.Trim() ?? "",
                pPrivilegeId = PrivilegeId?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<AccountPrivilegeFormModel>(connection, "Account_Role_Privilege_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<AccountPrivilegeFormModel>();
        }
        public string Account_Role_Privilege_Save(AccountPrivilegeSaveModel model)
        {
            dynamic @params = new
            {
                pRolePrivilegeId = Guid.NewGuid().ToString(),
                pRoleId = model.RoleId,
                pPrivilegeId = model.PrivilegeId,
                pIsSelected = model.IsSelected,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Account_Role_Privilege_Save", @params, commandType: CommandType.StoredProcedure);
        }
        #endregion Role Privilege

        #region User
        public List<string> User_GetContractorDivisions(string UserId)
        {
            dynamic @params = new
            {
                pUserId = UserId?.Trim() ?? ""
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<string>(connection, "Account_GetContractorDivisions", @params, commandType: CommandType.StoredProcedure) ?? new List<string>();
        }
        public List<AccountUserModel> User_Get(bool IsActive = true, string UserId = "", string DistrictId = "", string DivisionId = "",
            string UserGroup = "", string RoleId = "", string MobileNumber = "", string Email = "", string DepartmentId = "",
            string UserGroupName = "")
        {
            dynamic @params = new
            {
                pUserId = UserId?.Trim() ?? "",
                pIsActive = IsActive,
                pDistrictId = string.IsNullOrWhiteSpace(DistrictId) ? "" : ("%" + DistrictId?.Trim() + "%"),
                pDivisionId = string.IsNullOrWhiteSpace(DivisionId) ? "" : ("%" + DivisionId?.Trim() + "%"),
                pUserGroup = UserGroup?.Trim() ?? "",
                pUserGroupName = UserGroupName?.Trim() ?? "",
                pRoleId = RoleId?.Trim() ?? "",
                pMobile = MobileNumber?.Trim() ?? "",
                pEmail = Email?.Trim() ?? "",
                pDepartmentId = string.IsNullOrWhiteSpace(DepartmentId) ? "" : ("%" + DepartmentId?.Trim() + "%")
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<AccountUserModel>(connection, "Account_User_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<AccountUserModel>();
        }
        public List<UserNextNumberModel> User_GetNextNumber()
        {
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<UserNextNumberModel>(connection, "Account_User_GetNextNumber_Out", commandType: CommandType.StoredProcedure)?.ToList() ?? new List<UserNextNumberModel>();
        }
        public string User_SaveUpdate(AccountUserModel model, bool IsContractor = false)
        {
            DateTime? dob = model.DOB == DateTime.MinValue ? null : model.DOB;

            dynamic @params = new
            {
                pUserId = model.UserId,
                pFirstName = model.FirstName,
                pLastName = model.LastName,
                pEmail = model.Email,
                pIsActive = model.IsActive,
                pRoleId = model.RoleId,
                pMobile = model.Mobile,
                pDivisionId = model.DivisionId,
                pUserGroup = model.UserGroup,
                pDOB = dob,
                pDistrictId = model.DistrictId,
                pPassword = model.Password,
                pIsContractor = IsContractor,
                pDepartmentId = model.DepartmentId,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Account_User_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }

        public string User_Activate(AccountUserModel model)
        {
            dynamic @params = new
            {
                pUserId = model.UserId,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Account_User_Activate", @params, commandType: CommandType.StoredProcedure);
        }

        public List<AccountUserModel> User_Get(UserFilterModel model, string DepartmentId, out int TotalCount)
        {
            TotalCount = 0;
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            string Query = @"SELECT au.UserId, au.UserNumber, au.FirstName, au.LastName, au.Email, au.IsActive, au.RoleId, au.Prefix, au.Suffix, 
                        au.RunningNumber, au.Mobile, au.DivisionId, au.DOB, au.UserGroup, au.DistrictId, au.PofileImageId,
                        ug.Value as UserGroupName,
                        au.CreatedBy, au.CreatedDate, au.CreatedByUserName, 
                        au.ModifiedBy, au.ModifiedDate, au.ModifiedByUserName, au.DeletedBy, au.DeletedByUserName, au.DeletedDate,
                        ar.RoleCode,
                        ar.RoleName,
                        au.DepartmentId,
                        (select GROUP_CONCAT(Value separator ', ') from two_column_configuration_values where FIND_IN_SET(id, au.DistrictId)) as District,
                        (select GROUP_CONCAT(Value separator ', ') from two_column_configuration_values where FIND_IN_SET(id, au.DepartmentId)) as Department,
                        (select GROUP_CONCAT(Value separator ', ') from two_column_configuration_values where FIND_IN_SET(id, au.DivisionId)) as Division,
                        al.Password
                        FROM account_user au
                        LEFT JOIN two_column_configuration_values udiv ON udiv.Id = au.DivisionId
                        LEFT JOIN two_column_configuration_values udis ON udis.Id = au.DistrictId
                        LEFT JOIN two_column_configuration_values ug ON ug.Id = au.UserGroup
                        LEFT JOIN two_column_configuration_values ud ON ud.Id = au.DepartmentId
                        LEFT JOIN account_role ar ON ar.Id = au.RoleId
                        LEFT JOIN account_login al ON al.UserId = au.UserId";
            if (model != null)
            {

                #region Build Query Conditions

                string Condition = " WHERE ";

                if (model.Where != null)
                {
                    PropertyInfo[] whereProperties = typeof(UserWhereClauseProperties).GetProperties();
                    foreach (var property in whereProperties)
                    {
                        var value = property.GetValue(model.Where)?.ToString() ?? "";
                        if (value == "True")
                        {
                            value = "1";
                        }
                        else if (value == "False")
                        {
                            value = "0";
                        }
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            Condition += " au." + property.Name + "='" + value.Replace('\'', '%').Trim() + "' AND ";
                        }
                    }
                }
                if (model.ColumnSearch?.Count > 0)
                {
                    foreach (ColumnSearchModel item in model.ColumnSearch)
                    {
                        if (!string.IsNullOrWhiteSpace(item.SearchString?.Trim()) && !string.IsNullOrWhiteSpace(item.FieldName?.Trim()))
                        {
                            string columnName = "";

                            #region Field Name Select
                            if (string.Equals(item.FieldName, "UserNumber", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "au.UserNumber";
                            }
                            else if (string.Equals(item.FieldName, "FirstName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "au.FirstName";
                            }
                            else if (string.Equals(item.FieldName, "LastName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "au.LastName";
                            }
                            else if (string.Equals(item.FieldName, "Email", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "au.Email";
                            }
                            else if (string.Equals(item.FieldName, "UserGroupName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "ug.Value";
                            }
                            else if (string.Equals(item.FieldName, "Mobile", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "au.Mobile";
                            }
                            else if (string.Equals(item.FieldName, "ModifiedByUserName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "au.ModifiedByUserName";
                            }
                            else if (string.Equals(item.FieldName, "LastUpdatedUserName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "au.ModifiedByUserName";
                            }
                            else if (string.Equals(item.FieldName, "RoleName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "ar.RoleName";
                            }
                            else if (string.Equals(item.FieldName, "LastUpdatedDate", StringComparison.CurrentCultureIgnoreCase))
                            {
                                DateTime dd;

                                bool isOkay = DateTime.TryParseExact(item.SearchString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dd);

                                if (isOkay)
                                {
                                    item.SearchString = dd.ToString("yyyy-MM-dd");
                                }

                                columnName = "au.ModifiedDate";
                            }
                            else if (string.Equals(item.FieldName, "Department", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "(select GROUP_CONCAT(Value, ' ') from two_column_configuration_values where FIND_IN_SET(id, au.DepartmentId))";
                            }
                            else if (string.Equals(item.FieldName, "Division", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "(select GROUP_CONCAT(Value, ' ') from two_column_configuration_values where FIND_IN_SET(id, au.DivisionId))";
                            }
                            #endregion Field Name Select

                            //Condition += " " + columnName + "='" + item.SearchString.Replace('\'', '%').Trim() + "' AND ";
                            Condition += " " + columnName + " LIKE " + "'%" + item.SearchString.Replace('\'', '%').Trim() + "%' AND ";
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";
                    PropertyInfo[] whereProperties = typeof(UserWhereClauseProperties).GetProperties();
                    foreach (PropertyInfo property in whereProperties)
                    {
                        if (property.PropertyType.Name.ToLower() != "boolean")
                        {
                            searchCondition += " au." + property.Name + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                        }
                    }
                    int sub_pos = searchCondition.Length - 3;
                    if (!(sub_pos < 0) && searchCondition.Substring(searchCondition.Length - 3) == "OR ")
                    {
                        searchCondition = searchCondition.Remove(searchCondition.Length - 3);
                    }
                    searchCondition += ") AND ";

                    Condition += searchCondition;
                }

                if (Condition.Substring(Condition.Length - 5) == " AND ")
                {
                    Condition = Condition.Remove(Condition.Length - 5);
                }
                if (Condition == " WHERE ")
                {
                    Condition = "";
                }
                TotalCount = (SqlMapper.Query<TenderMasterModel>(connection, Query + Condition, commandType: CommandType.Text)?.ToList() ?? new List<TenderMasterModel>()).Count();

                if (model?.Sorting != null && !string.IsNullOrWhiteSpace(model?.Sorting.FieldName) && !string.IsNullOrWhiteSpace(model?.Sorting.Sort))
                {
                    string FieldName = "";

                    #region Field Name Select
                    if (string.Equals(model?.Sorting.FieldName, "UserNumber", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "au.UserNumber";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "FirstName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "au.FirstName";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "LastName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "au.LastName";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "Email", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "au.Email";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "UserGroupName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "ug.Value";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "Department", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "ar.RoleName";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "Mobile", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "au.Mobile";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "ModifiedByUserName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "au.ModifiedByUserName";
                    }
                    else
                    {
                        FieldName = "au.ModifiedDate";
                    }
                    #endregion Select Field

                    if (model?.Skip == 0 && model?.Take == 0)
                    {
                        Condition += " ORDER BY " + FieldName + " " + model?.Sorting.Sort + " ";
                    }
                    else
                    {
                        Condition += " ORDER BY " + FieldName + " " + model?.Sorting.Sort + " LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                    }
                }
                else if (model?.Skip == 0 && model?.Take == 0)
                {
                    Condition += " ORDER BY wtm.CreatedDate ";
                }
                else
                {
                    Condition += " ORDER BY wtm.CreatedDate LIMIT " + model?.Take + " OFFSET " + model?.Skip;
                }

                Query += Condition;

                #endregion Build Query Conditions
            }

            return SqlMapper.Query<AccountUserModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<AccountUserModel>();
        }
        #endregion User

        #region QuickLink

        public List<QuickLinkMasterModel> QuickLink_Get(bool IsActive = true, string Id = "", string FileType = "", string UserGroupId = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pIsActive = IsActive,
                pFileType = FileType?.Trim() ?? "",
                pUserGroupId = string.IsNullOrWhiteSpace(UserGroupId) ? "" : ("%" + UserGroupId.Trim() + "%")
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<QuickLinkMasterModel>(connection, "QuickLink_Master_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<QuickLinkMasterModel>();
        }
        public string QuickLink_SaveUpdate(QuickLinkMasterModel model)
        {
            dynamic @params = new
            {
                pId = model.Id,
                pName = model.Name,
                pLink = model.Link,
                pFileType = model.FileType,
                pIsActive = model.IsActive,
                pUserGroupIds = model.UserGroupIds,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "QuickLink_Master_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }

        #endregion QuickLink

        #region WorkTemplate

        public List<TemplateModel> Template_Get(bool IsActive = true, string Id = "", string WorkTypeId = "", string DepartmentId = "",string subcategory = "", string maincategory = "", string serviceType = "", string categoryType = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pIsActive = IsActive,
                pWorkTypeId = WorkTypeId?.Trim() ?? "",
                pDepartmentId = DepartmentId?.Trim() ?? "",
                psubcategory= subcategory??"",
                pmaincategory= maincategory??"",
                pcategoryType = categoryType ?? "",
                pserviceType = serviceType ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<TemplateModel>(connection, "Template_Master_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<TemplateModel>();
        }
        public string Template_SaveUpdate(TemplateModel model)
        {
            dynamic @params = new
            {
                pId = model.Id,
                pName = model.Name,
                pWorkTypeId = model.WorkTypeId,
                pServiceTypeId = model.serviceTypeId,
                pCategoryTypeId = model.categoryTypeId,
                pDurationInDays = model.DurationInDays,
                pIsActive = model.IsActive,
                pSubWorkTypeId = model.SubWorkTypeId,
                pStrength = model.Strength,
                pTemplateCode = model.TemplateCode,
                pDepartmentId = model.DepartmentId,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Template_Master_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }
        public List<TemplateMilestoneModel> TemplateMilestone_Get(bool IsActive = true, string Id = "", string TemplateId = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pIsActive = IsActive,
                pTemplateId = TemplateId?.Trim() ?? ""
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<TemplateMilestoneModel>(connection, "Template_Milestone_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<TemplateMilestoneModel>();
        }
        public string TemplateMilestone_SaveUpdate(TemplateMilestoneModel model)
        {
            dynamic @params = new
            {
                pId = model.Id,
                pTemplateId = model.TemplateId,
                pMilestoneName = model.MilestoneName,
                pOrderNumber = model.OrderNumber,
                pDurationInDays = model.DurationInDays,
                pIsPaymentRequired = model.IsPaymentRequired,
                pPaymentPercentage = model.PaymentPercentage,
                pMilestoneCode = model.MilestoneCode,
                pIsActive = model.IsActive,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Template_Milestone_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }

        public string Work_Template_Edit(TemplateViewModel model)
        {
            dynamic @params = new
            {

                pTemplateId = model.Id,
                pworkTemplateId = model.workTemplateId,
                pName = model.Name,
                pWorkTypeId = model.WorkTypeId,
                pWorkId = model.WorkId,
                pDurationInDays = model.DurationInDays,
                pIsActive = model.IsActive,
                pSubWorkTypeId = model.SubWorkTypeId,
                pStrength = model.Strength,
                pTemplateCode = model.TemplateCode,
                pSavedBy = model.LastUpdatedBy,
                pSavedByUserName = model.LastUpdatedUserName,
                pSavedDate = model.LastUpdatedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Work_Template_Edit", @params, commandType: CommandType.StoredProcedure);
        }
        public string Template_Publish(TemplateModel model)
        {
            dynamic @params = new
            {
                pId = model.Id,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Template_Master_Publish", @params, commandType: CommandType.StoredProcedure);
        }
        #endregion WorkTemplate

        #region ApprovalFlow

        public List<ApprovalFlowMaster> ApprovalFlow_Get(string DepartmentId, string RoleId = "")
        {
            dynamic @params = new
            {
                pDepartmentId = DepartmentId?.Trim() ?? "",
                pRoleId = RoleId?.Trim() ?? ""
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<ApprovalFlowMaster>(connection, "Approval_Flow_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<ApprovalFlowMaster>();
        }
        public string ApprovalFlow_Add_Role(ApprovalFlowAddRoleModel_API model)
        {
            if (model.RoleIds != null)
            {
                List<ApprovalFlowMaster> exist_role_id_list = ApprovalFlow_Get(model.DepartmentId);

                List<ApprovalFlowMaster> exist_role_id_to_remove = exist_role_id_list?.Where(x => !model.RoleIds.Contains(x.RoleId)).ToList() ?? new List<ApprovalFlowMaster>();

                List<string> role_id_to_stay_exist = new List<string>();
                if (exist_role_id_to_remove.Count > 0)
                {
                    role_id_to_stay_exist = exist_role_id_list?.Where(x => !(exist_role_id_to_remove.Select(x => x.RoleId).ToList().Contains(x.RoleId)))?.Select(x => x.RoleId)?.ToList() ?? new List<string>();
                }
                List<string> role_ids_to_add_new = new List<string>();
                if (role_id_to_stay_exist.Count > 0)
                {
                    role_ids_to_add_new = model.RoleIds.Where(x => !role_id_to_stay_exist.Contains(x))?.ToList() ?? new List<string>();
                }

                foreach (ApprovalFlowMaster remove_item in exist_role_id_to_remove)
                {
                    ApprovalFlow_Remove_Role(new ApprovalFlowMaster()
                    {
                        Id = remove_item.Id,
                        RoleId = remove_item.RoleId,
                        SavedBy = model.SavedBy,
                        SavedByUserName = model.SavedByUserName,
                        SavedDate = model.SavedDate,
                    });
                }

                foreach (string roleId in model.RoleIds)
                {
                    dynamic @params = new
                    {
                        pId = Guid.NewGuid().ToString(),
                        pRoleId = roleId,
                        pDepartmentId = model.DepartmentId,
                        pSavedBy = model.SavedBy,
                        pSavedByUserName = model.SavedByUserName,
                        pSavedDate = model.SavedDate,
                    };

                    using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
                    SqlMapper.ExecuteScalar<string>(connection, "Approval_Flow_Add_Role", @params, commandType: CommandType.StoredProcedure);
                }
            }
            return "";
        }
        public string ApprovalFlow_Remove_Role(ApprovalFlowMaster model)
        {
            dynamic @params = new
            {
                pId = model.Id,
                pRoleId = model.RoleId,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Approval_Flow_Remove_Role", @params, commandType: CommandType.StoredProcedure);
        }
        public string ApprovalFlow_SaveUpdate(ApprovalFlowMaster model)
        {
            dynamic @params = new
            {
                pId = model.Id,
                pRoleId = model.RoleId,
                pOrderNumber = model.OrderNumber,
                pIsNA = model.IsNA,
                pApprovalFlowId = model.ApprovalFlowId,
                pReturnFlowId = model.ReturnFlowId,
                pIsFinal = model.IsFinal,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Approval_Flow_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }

        #endregion ApprovalFlow

        #region Alert
        public List<AlertConfigurationPrimaryModel> Alert_Primary_Get(string Id = "", string Type = "", string Object = "", string Name = "", string Department = "", bool IsActive = true)
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pType = Type?.Trim() ?? "",
                pObject = Object?.Trim() ?? "",
                pName = Name?.Trim() ?? "",
                pDepartment = Department?.Trim() ?? "",
                pIsActive = IsActive
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<AlertConfigurationPrimaryModel>(connection, "Alert_Primary_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<AlertConfigurationPrimaryModel>();
        }
        public string AlertPrimary_Config_SaveUpdate(AlertConfigurationPrimaryModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
            }

            dynamic @params = new
            {
                pId = model.Id,
                pType = model.Type,
                pObject = model.Object,
                pName = model.Name,
                pDepartment = model.Department,

                pSMSFrequency = model.SMSFrequency,
                pSMSuserGroups = model.SMSuserGroups,
                pEmailFrequency = model.EmailFrequency,
                pEmailuserGroups = model.EmailuserGroups,

                pIsActive = model.IsActive,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Alert_Primary_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }
        public List<AlertConfigurationSecondaryModel> Alert_Secondary_Get(string Id = "", string PrimaryId = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pPrimaryId = PrimaryId?.Trim() ?? ""
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<AlertConfigurationSecondaryModel>(connection, "Alert_Secondary_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<AlertConfigurationSecondaryModel>();
        }
        public string AlertSecondary_Config_SaveUpdate(AlertConfigurationSecondaryModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
            }

            dynamic @params = new
            {
                pId = model.Id,
                pPrimaryId = model.PrimaryId,
                pSeverity = model.Severity,
                pField = model.Field,
                pBaseField = model.BaseField,
                pCalculationNo = model.CalculationNo,
                pCalculationType = model.CalculationType,
                pFrequencyType = model.FrequencyType,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Alert_Secondary_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }
        #endregion Alert

        public List<StatusMaster> Status_Get(string StatusCode = "", string Id = "", string Type = "")
        {
            dynamic @params = new
            {
                pStatusCode = StatusCode?.Trim() ?? "",
                pId = Id?.Trim() ?? "",
                pType = Type?.Trim() ?? ""
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<StatusMaster>(connection, "Status_Master_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<StatusMaster>();
        }

        // // Modified by indu for list of schemes
        public List<schemeModel> Scheme_Get()
        {
            dynamic @params = new
            {
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<schemeModel>(connection, "GetSchemeList", @params, commandType: CommandType.StoredProcedure) ?? new List<schemeModel>();
        }

        public List<schemeModel> PacakgeNo_Get()
        {
            dynamic @params = new
            {
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<schemeModel>(connection, "GetPacakgeNoList", @params, commandType: CommandType.StoredProcedure) ?? new List<schemeModel>();
        }
    }
}
