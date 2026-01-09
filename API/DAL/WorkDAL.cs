using AutoMapper;
using Dapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Model.Constants;
using Model.DomainModel;
using Model.ViewModel;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Utils;
using Utils.Interface;




//using static Google.Protobuf.Collections.MapField<TKey, TValue>;
using static System.Net.Mime.MediaTypeNames;

namespace DAL
{
    public class WorkDAL
    {
        private readonly IMySqlHelper _mySqlHelper;
        private readonly IConfiguration _configuration;
        private readonly IMySqlDapperHelper _mySqlDapperHelper;
        private readonly SettingsDAL _settingDAL;


        private readonly string connectionId = "Default";
        

        public WorkDAL(IMySqlDapperHelper mySqlDapperHelper, IMySqlHelper mySqlHelper, IConfiguration configuration)
        {
            _mySqlHelper = mySqlHelper;

            _configuration = configuration;
            _mySqlDapperHelper = mySqlDapperHelper;
            _settingDAL = new SettingsDAL(mySqlDapperHelper, mySqlHelper, configuration);
        }

        #region GO

        public List<GOMasterModel> GO_Get(bool IsActive = true, string Id = "", string GONumber = "", string LocalGONumber = "", string DepartmentId = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pIsActive = IsActive,
                pGONumber = GONumber?.Trim() ?? "",
                pLocalGONumber = LocalGONumber?.Trim() ?? "",
                pDepartmentId = DepartmentId?.Trim() ?? ""
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<GOMasterModel>(connection, "GO_Master_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<GOMasterModel>();
        }
        public List<GOMasterModel> GO_Get(GoFilterModel model, out int TotalCount)
        {
            TotalCount = 0;
            string Query = @"SELECT gm.Id, gm.GONumber, gm.GODate, gm.GOCost, gm.GOName, gm.GODepartment, gm.GOTotalAmount, gm.IsActive,
                                gm.Prefix, gm.Suffix, gm.RunningNumber, gm.LocalGONumber, gm.CreatedBy, gm.CreatedDate, gm.CreatedByUserName, gm.ModifiedBy, gm.ModifiedDate, 
                                gm.ModifiedByUserName, gm.DeletedBy, gm.DeletedByUserName, gm.DeletedDate, gm.DepartmentId, gm.DepartmentCode,
                                count(tm.Id) as TenderCount,
                                (select sum(ttm.TenderFinalAmount) from tender_master ttm where  ttm.IsActive= 1  and ttm.GoId=gm.Id) as GORevisedAmount
                                FROM go_master gm 
                                left join tender_master tm on tm.GoId = gm.Id";
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            if (model != null)
            {
                string Condition = " WHERE ";

                if (model.Where != null)
                {
                    PropertyInfo[] whereProperties = typeof(GoWhereClauseProperties).GetProperties();
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
                            Condition += "gm." + property.Name + "='" + value.Replace('\'', '%').Trim() + "' AND ";
                        }
                    }
                }
                if (model?.DepartmentList?.Count > 0)
                {
                    string departmentIds_str = "";
                    model.DepartmentList.ForEach(x =>
                    {
                        departmentIds_str += "'" + x + "',";
                    });
                    departmentIds_str = departmentIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(departmentIds_str))
                    {
                        Condition += " gm.DepartmentId IN (" + departmentIds_str + ") AND ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";
                    PropertyInfo[] whereProperties = typeof(GoWhereClauseProperties).GetProperties();
                    foreach (var property in whereProperties)
                    {
                        if (property.PropertyType.Name.ToLower() != "boolean")
                        {
                            searchCondition += "gm." + property.Name + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                        }
                    }
                    searchCondition += ") AND";

                    Condition += searchCondition;
                }

                if (model?.Year?.Count > 0)
                {
                    string searchCondition = " (";
                    foreach (string year in model.Year)
                    {
                        searchCondition += " (YEAR(gm.GODate) = '" + year + "' OR YEAR(gm.GODate) = '" + year + "') OR ";
                    }
                    if (searchCondition.Substring(searchCondition.Length - 3) == "OR ")
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

                string qqqq = Query + Condition + " group by gm.Id having count(tm.Id) > 0";

                TotalCount = (SqlMapper.Query<TenderMasterModel>(connection, qqqq, commandType: CommandType.Text)?.ToList() ?? new List<TenderMasterModel>()).Count();

                if (model?.Sorting != null && !string.IsNullOrWhiteSpace(model?.Sorting.FieldName) && !string.IsNullOrWhiteSpace(model?.Sorting.Sort))
                {
                    if (model?.Skip == 0 && model?.Take == 0)
                    {
                        Condition += " group by gm.Id having count(tm.Id) > 0 ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " ";
                    }
                    else
                    {
                        Condition += " group by gm.Id having count(tm.Id) > 0 ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                    }
                }
                else if (model?.Skip == 0 && model?.Take == 0)
                {
                    Condition += " group by gm.Id having count(tm.Id) > 0 ORDER BY CreatedDate ";
                }
                else
                {
                    Condition += " group by gm.Id having count(tm.Id) > 0 ORDER BY CreatedDate LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                }

                Query += Condition;
            }


            return SqlMapper.Query<GOMasterModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<GOMasterModel>();
        }
        public string GO_SaveUpdate(GOMasterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
            }

            dynamic @params = new
            {
                pId = model.Id,
                pGONumber = model.GONumber,
                pGODate = model.GODate,
                pGOCost = model.GOCost,
                pGOName = model.GOName,
                pGODepartment = model.GODepartment,
                pDepartmentId = model.DepartmentId,
                pDepartmentCode = model.DepartmentCode,
                pGORevisedAmount = model.GORevisedAmount,
                pGOTotalAmount = model.GOTotalAmount,
                pGo_package_count = model.Go_package_count,
                pGo_no_works = model.Go_no_works,
                pIsActive = model.IsActive,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = DateTime.Now,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "GO_Master_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }
        public List<GOMasterModel> GO_SaveUpdate_Bulk(List<GOMasterModel> list)
        {
            if (list?.Count > 0)
            {
                list.ForEach(model =>
                {
                    model.Id = Guid.NewGuid().ToString();
                    model.IsActive = true;
                    string Id = GO_SaveUpdate(model);
                    if (Id.Contains("@@1"))
                    {
                        model.Id = Id.Split("/")[0].ToString();
                        GO_SaveUpdate(model);
                    }
                    else
                    {
                        model.Id = Id;
                    }
                });

                return GO_Get();
            }

            return new List<GOMasterModel>();
        }

        public List<GOReportViewModel> GO_Report_Get(GoFilterModel model, out int TotalCount)
        {
            TotalCount = 0;
            string Query = @"select 
                            gom.Id, gom.GONumber, gom.GODate, gom.GOName, gom.GODepartment as 'Department', tm.Division, tm.District, tm.DistrictName, tm.DivisionName, 
                            gom.DepartmentId, gom.DepartmentCode, 
                            gom.GOCost, gom.GORevisedAmount, gom.GOTotalAmount,
                            COUNT(tm.Id) AS 'NumberOfTenders',
                            T.PannedValue, T.ActualValue, T.PaymentAmount, T.TotalWorkCount as 'TotalWork', T.CompletedWorkCount as 'CompletedWork',
                            (T.TotalWorkCount - T.CompletedWorkCount) as 'RemainingWorks'
                            from go_master gom
                            left join tender_master tm on tm.GoId = gom.Id
                            left join (select SUM(wtmm.MilestoneAmount) as 'PannedValue', SUM(wtmm.ActualAmount) as 'ActualValue',
                            COUNT(wtmm.MilestoneAmount) as 'TotalWorkCount',
                            SUM(CASE WHEN wtmm.IsCompleted = 1 THEN 1 ELSE 0 END) as 'CompletedWorkCount', 
                            SUM(CASE WHEN wtmm.IsCompleted = 1 THEN wtmm.ActualAmount ELSE 0 END) as 'PaymentAmount',
                            tm.GoId 
                            from work_template_milestone_master wtmm 
                            inner join work_master wm on wm.Id=wtmm.WorkId
                            inner join tender_master tm on tm.Id=wm.TenderId
                            group by tm.GoId) as T on T.GoId = gom.Id ";

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            if (model != null)
            {
                string Condition = " WHERE ";

                List<string> goProp = new List<string>() { "Id", "GONumber", "LocalGONumber", "GODate", "GOName",
                                 "DepartmentId", "GORevisedAmount", "GOTotalAmount", "GOCost" };

                List<string> goTProp = new List<string>() { "PannedValue", "ActualValue", "TotalWorkCount", "CompletedWorkCount", "PaymentAmount" };
                //Condition = Condition.Remove(Condition.Length - 3);
                if (model.Where != null)
                {
                    PropertyInfo[] whereProperties = typeof(GoWhereClauseProperties).GetProperties();
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
                            if (goProp.Contains(property.Name))
                            {
                                Condition += "gom." + property.Name + "='" + value.Replace('\'', '%').Trim() + "' AND ";
                            }
                            else if (property.Name == "NumberOfTenders")
                            {
                                Condition += "COUNT(tm.Id) = '" + value.Replace('\'', '%').Trim() + "' AND ";
                            }
                            else if (property.Name == "RemainingWorks")
                            {
                                Condition += "(T.TotalWorkCount - T.CompletedWorkCount) = '" + value.Replace('\'', '%').Trim() + "' AND ";
                            }
                            else if (goTProp.Contains(property.Name))
                            {
                                Condition += "T." + property.Name + "='" + value.Replace('\'', '%').Trim() + "' AND ";
                            }
                        }

                    }
                }
                //if (!string.IsNullOrWhiteSpace(model?.SearchString))
                //{
                //    string searchCondition = " (";
                //    List<string> whereProperties = new List<string>() { "Id", "GONumber", "LocalGONumber", "GODate", "GOName",
                //                "Department", "DepartmentId", "GORevisedAmount", "GOTotalAmount", "GOCost" };
                //    foreach (var property in whereProperties)
                //    {
                //        searchCondition += property + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                //    }
                //    searchCondition = searchCondition.Remove(searchCondition.Length - 3);
                //    searchCondition += ") AND";

                //    Condition += searchCondition;
                //}

                if (model?.DistrictList?.Count > 0)
                {
                    string districtIds_str = "";
                    model.DistrictList.ForEach(x =>
                    {
                        districtIds_str += "'" + x + "',";
                    });
                    districtIds_str = districtIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(districtIds_str))
                    {
                        Condition += " tm.District IN (" + districtIds_str + ") AND ";
                    }
                }
                if (model?.DivisionList?.Count > 0)
                {
                    string divisionIds_str = "";
                    model.DivisionList.ForEach(x =>
                    {
                        divisionIds_str += "'" + x + "',";
                    });
                    divisionIds_str = divisionIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(divisionIds_str))
                    {
                        Condition += " tm.Division IN (" + divisionIds_str + ") AND ";
                    }
                }
                if (model?.DepartmentList?.Count > 0)
                {
                    string departmentIds_str = "";
                    model.DepartmentList.ForEach(x =>
                    {
                        departmentIds_str += "'" + x + "',";
                    });
                    departmentIds_str = departmentIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(departmentIds_str))
                    {
                        Condition += " gom.DepartmentId IN (" + departmentIds_str + ") AND ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";
                    PropertyInfo[] whereProperties = typeof(GoWhereClauseProperties).GetProperties();
                    foreach (var property in whereProperties)
                    {
                        if (property.PropertyType.Name.ToLower() != "boolean")
                        {
                            if (goProp.Contains(property.Name))
                            {
                                searchCondition += "gom." + property.Name + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                            }
                            //else if (property.Name == "NumberOfTenders")
                            //{
                            //    searchCondition += "COUNT(tm.Id) LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                            //}
                            else if (property.Name == "RemainingWorks")
                            {
                                searchCondition += "(T.TotalWorkCount - T.CompletedWorkCount) LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                            }
                            else if (goTProp.Contains(property.Name))
                            {
                                searchCondition += "T." + property.Name + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                            }
                        }
                    }
                    searchCondition = searchCondition.Remove(searchCondition.Length - 3);
                    searchCondition += ") AND ";

                    Condition += searchCondition;
                }

                if (model?.Year?.Count > 0)
                {
                    string searchCondition = " (";
                    foreach (string year in model.Year)
                    {
                        searchCondition += " (YEAR(gom.GODate) = '" + year + "' OR YEAR(gom.GODate) = '" + year + "') OR ";
                    }
                    if (searchCondition.Substring(searchCondition.Length - 3) == "OR ")
                    {
                        searchCondition = searchCondition.Remove(searchCondition.Length - 3);
                    }
                    searchCondition += ") AND ";

                    Condition += searchCondition;
                }
                if (model?.DivisionId?.Count() > 0)
                {
                    if (!string.IsNullOrWhiteSpace(model?.DivisionId))
                    {
                        Condition += " tm.Division = '" + model.DivisionId + "' AND ";
                    }
                }

                if (model?.DistrictId?.Count() > 0)
                {

                    if (!string.IsNullOrWhiteSpace(model?.DistrictId))
                    {
                        Condition += " tm.District = '" + model.DistrictId + "' ";
                    }
                }

                if (Condition.Substring(Condition.Length - 5) == " AND ")
                {
                    Condition = Condition.Remove(Condition.Length - 5);
                }
                if (Condition == " WHERE ")
                {
                    Condition = "";
                }


                string qqqq = Query + Condition + " group by gom.Id having count(tm.Id) > 0";



                TotalCount = (SqlMapper.Query<TenderMasterModel>(connection, qqqq, commandType: CommandType.Text)?.ToList() ?? new List<TenderMasterModel>()).Count();

                if (model?.Sorting != null && !string.IsNullOrWhiteSpace(model?.Sorting.FieldName) && !string.IsNullOrWhiteSpace(model?.Sorting.Sort))
                {
                    string FieldName = "";

                    #region Select Field
                    if (string.Equals(model?.Sorting.FieldName, "Id", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "gom.Id";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "GONumber", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "gom.GONumber";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "DivisionName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "tm.DivisionName";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "DistrictName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "tm.DistrictName";
                    }

                    else if (string.Equals(model?.Sorting.FieldName, "GODate", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "gom.GODate";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "GOName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "gom.GOName";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "GODepartment", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "gom.GODepartment";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "DepartmentId", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "gom.DepartmentId";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "DepartmentCode", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "gom.DepartmentCode";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "GOCost", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "gom.GOCost";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "GORevisedAmount", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "gom.GORevisedAmount";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "GOTotalAmount", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "gom.GOTotalAmount";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "NumberOfTenders", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "COUNT(tm.Id)";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "PannedValue", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "T.PannedValue";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "ActualValue", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "T.ActualValue";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "PaymentAmount", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "T.PaymentAmount";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "TotalWorkCount", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "T.TotalWorkCount";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "CompletedWorkCount", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "T.CompletedWorkCount";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "RemainingWorks", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "(T.TotalWorkCount - T.CompletedWorkCount)";
                    }
                    else
                    {
                        FieldName = "gom.CreatedDate";
                    }
                    #endregion Select Field

                    if (model?.Skip == 0 && model?.Take == 0)
                    {
                        Condition += " group by gom.Id having count(tm.Id) > 0 ORDER BY " + FieldName + " " + model?.Sorting.Sort + " ";
                    }
                    else
                    {
                        Condition += " group by gom.Id having count(tm.Id) > 0 ORDER BY " + FieldName + " " + model?.Sorting.Sort + " LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                    }
                }
                else if (model?.Skip == 0 && model?.Take == 0)
                {
                    Condition += " group by gom.Id having count(tm.Id) > 0 ORDER BY gom.CreatedDate ";
                }
                else
                {
                    Condition += " group by gom.Id having count(tm.Id) > 0 ORDER BY gom.CreatedDate LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                }

                Query += Condition;
            }


            return SqlMapper.Query<GOReportViewModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<GOReportViewModel>();
        }

        #endregion GO

        #region Tender

        public List<TenderMasterModel> Tender_Get(bool IsActive = true, string Id = "", string TenderNumber = "", string LocalTenderNumber = "", string GOId = "", string ContractorId = "", string DepartmentId = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pIsActive = IsActive,
                pTenderNumber = TenderNumber?.Trim() ?? "",
                pLocalTenderNumber = LocalTenderNumber?.Trim() ?? "",
                pGOId = GOId?.Trim() ?? "",
                pContractorId = ContractorId?.Trim() ?? "",
                pDepartmentId = DepartmentId?.Trim() ?? ""
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<TenderMasterModel>(connection, "Tender_Master_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<TenderMasterModel>();
        }
        public List<TenderMasterModel> Tender_Get(TenderFilterModel model, out int TotalCount)
        {
            TotalCount = 0;
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            string Query = @"SELECT tm.Id, tm.TenderNumber, tm.StartDate, tm.EndDate, tm.Division, tm.District, tm.DivisionName, tm.DistrictName, tm.Class, tm.Category, tm.WorkValue, tm.MainCategory, tm.Subcategory,
                            tm.BidType, tm.ContractorName, tm.ContractorDivision, tm.ContractorDistrict, tm.ContractorCategory,
                            tm.ContractorAddress, tm.ContractorAltMobile, tm.ContractorEmail, tm.ContractorMobile, tm.TenderFinalAmount, 
                            tm.IsActive, tm.Prefix, tm.Suffix, tm.RunningNumber, tm.LocalTenderNumber,tm.SchemeName,wm.WorkCommencementDate,wm.WorkCompletionDate,
                            tm.CreatedBy, tm.CreatedDate, tm.CreatedByUserName, tm.ModifiedBy, tm.ModifiedDate, tm.ModifiedByUserName, tm.DeletedBy, tm.DeletedByUserName, tm.DeletedDate,
                            NOT EXISTS (SELECT 1 FROM work_master wm1 WHERE wm1.TenderId = tm.Id) as CanCreateWork,
	                        (IFNULL(wm.Id,'')='always false') as CanEditWork,

	                        (((IFNULL(wm.WorkTemplateId,'')='' OR IFNULL(wtm.IsSubmitted,0)=0)) AND (EXISTS (SELECT 1 FROM work_master wm1 WHERE wm1.TenderId = tm.Id))) as CanCreateTemplate,

                            wm.Id AS WorkId, wm.WorkTemplateId, tm.DepartmentName, tm.DepartmentCode, tm.DepartmentId,

                            wt.Value as WorkType,
                            swt.Value as SubWorkType,
                            go.GONumber as GoNumber,
                            go.Id as GoId,
                            tm.TenderFinalAmount as WorkAmount,
                            wm.WorkProgress,
                            ifnull(tm.TenderStatus,'Not-Started') AS TenderStatus,
                            ifnull(tm.TenderStatus,'Not-Started') AS WorkStatus,                            
                            (select sum(s.ActualAmount) from  work_template_milestone_master s
                             where  s.IsActive= 1  and s.WorkId=wm.id) as ActualAmount,
                            
                            tm.WorkValue AS PannedValue,
                            (select sum(ActualAmount) from work_template_milestone_master where WorkId=wm.Id) AS ActualValue,
                            (select sum(ActualAmount) from work_template_milestone_master where WorkId=wm.Id and IsCompleted = 1) AS PaymentValue

                            FROM tender_master tm 
                            LEFT JOIN go_master go ON go.Id = tm.GoId 
                            LEFT JOIN work_master wm ON wm.TenderId = tm.Id 
                            LEFT JOIN work_template_master wtm ON wtm.Id = wm.WorkTemplateId 
                            LEFT JOIN two_column_configuration_values wt ON wt.Id = wtm.WorkTypeId
                            LEFT JOIN two_column_configuration_values swt ON swt.Id = wtm.SubWorkTypeId ";

            if (model != null)
            {
                #region Build Query Conditions


                string Condition = " WHERE ";

                if (model.Where != null)
                {
                    PropertyInfo[] whereProperties = typeof(TenderWhereClauseProperties).GetProperties();
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
                            Condition += "tm." + property.Name + "='" + value.Replace('\'', '%').Trim() + "' AND ";
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
                            if (string.Equals(item.FieldName, "divisionName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.DivisionName";
                            }
                            else if (string.Equals(item.FieldName, "districtName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.DistrictName";
                            }
                            else if (string.Equals(item.FieldName, "workType", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "wt.Value";
                            }
                            else if (string.Equals(item.FieldName, "goNumber", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "go.GONumber";
                            }
                            else if (string.Equals(item.FieldName, "tenderNumber", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.TenderNumber";
                            }
                            else if (string.Equals(item.FieldName, "bidType", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.BidType";
                            }
                            else if (string.Equals(item.FieldName, "startDate", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.startDate";
                            }
                            else if (string.Equals(item.FieldName, "endDate", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.endDate";
                            }
                            else if (string.Equals(item.FieldName, "workCommencementDate", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "wm.WorkCommencementDate";
                            }
                            else if (string.Equals(item.FieldName, "WorkCompletionDate", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "wm.WorkCompletionDate";
                            }
                            else if (string.Equals(item.FieldName, "SchemeName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.SchemeName";
                            }
                            else if (string.Equals(item.FieldName, "workAmount", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.TenderFinalAmount";
                            }
                            else if (string.Equals(item.FieldName, "workProgress", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "wm.WorkProgress";
                            }
                            else if (string.Equals(item.FieldName, "WorkStatus", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "wm.WorkStatus";
                            }

                            else if (string.Equals(item.FieldName, "WorkValue", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.WorkValue";
                            }
                            else if (string.Equals(item.FieldName, "lastUpdatedDate", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.ModifiedDate";
                            }
                            else if (string.Equals(item.FieldName, "TenderStatus", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = " ifnull(tm.TenderStatus,'Not-Started') ";
                            }
                            #endregion Field Name Select

                            Condition += " " + columnName + "='" + item.SearchString.Replace('\'', '%').Trim() + "' AND ";
                            Condition += " " + columnName + " LIKE " + "'%" + item.SearchString.Replace('\'', '%').Trim() + "%' AND ";
                        }
                    }
                }

                if (string.Equals(model.RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (model?.TenderIds?.Count > 0)
                    {
                        string tenderIds_str = "";
                        model.TenderIds.ForEach(x =>
                        {
                            tenderIds_str += "'" + x + "',";
                        });
                        tenderIds_str = tenderIds_str.Trim().Trim(',');
                        if (!string.IsNullOrWhiteSpace(tenderIds_str))
                        {
                            Condition += " tm.Id IN (" + tenderIds_str + ") AND ";
                        }
                    }
                }

                if (model?.DivisionList?.Count > 0)
                {
                    string divisionIds_str = "";
                    model.DivisionList.ForEach(x =>
                    {
                        divisionIds_str += "'" + x + "',";
                    });
                    divisionIds_str = divisionIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(divisionIds_str))
                    {
                        Condition += " tm.Division IN (" + divisionIds_str + ") AND ";
                    }
                }
                if (model?.DepartmentList?.Count > 0)
                {
                    string departmentIds_str = "";
                    model.DepartmentList.ForEach(x =>
                    {
                        departmentIds_str += "'" + x + "',";
                    });
                    departmentIds_str = departmentIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(departmentIds_str))
                    {
                        Condition += " tm.DepartmentId IN (" + departmentIds_str + ") AND ";
                    }
                }



                if (model?.Year?.Count > 0)
                {
                    List<int> yearList = model.Year.Select(s => Convert.ToInt32(s)).OrderBy(x => x).ToList();
                    int fromYear = yearList?.FirstOrDefault() ?? 2020;
                    int toYear = yearList?.LastOrDefault() ?? 4000;

                    DateTime StartDate = new DateTime(Convert.ToInt32(fromYear), 4, 1);
                    DateTime EndDate = new DateTime(Convert.ToInt32(toYear), 3, 31);

                    //if (model.Year.Count > 1)
                    //{
                    //    Condition += " ((YEAR(tm.StartDate) >= "+ StartDate.Year+ " OR YEAR(tm.StartDate) <= "+ EndDate.Year+ ") AND (YEAR(tm.EndDate) >= "+ StartDate.Year+ " OR YEAR(tm.EndDate) <= "+ EndDate.Year+ ")) AND ";
                    //}
                    //else
                    //{
                    //    Condition += " ((YEAR(tm.StartDate) = " + StartDate.Year + " OR YEAR(tm.StartDate) = " + EndDate.Year + ") AND (YEAR(tm.EndDate) = " + StartDate.Year + " OR YEAR(tm.EndDate) = " + EndDate.Year + ")) AND ";
                    //}

                    Condition += " ((YEAR(tm.StartDate) BETWEEN " + StartDate.Year + " AND " + EndDate.Year + ") OR (YEAR(tm.EndDate)  BETWEEN " + StartDate.Year + " AND " + EndDate.Year + ") " +
                        " OR (YEAR(tm.StartDate) <= " + StartDate.Year + " AND (YEAR(tm.EndDate) >= " + EndDate.Year + "))) AND ";
                }

                if (model?.WorkType?.Count > 0)
                {
                    string workTypeIds_str = "";
                    model.WorkType.ForEach(x =>
                    {
                        workTypeIds_str += "'" + x + "',";
                    });
                    workTypeIds_str = workTypeIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(workTypeIds_str))
                    {
                        Condition += " wt.Value IN (" + workTypeIds_str + ") AND ";
                    }
                }

                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";
                    List<string> columnsToSearch = new List<string>() { "tm.TenderNumber", "tm.DivisionName",
                        "tm.DistrictName", "tm.BidType", "tm.WorkValue", "wt.Value", "swt.Value", "go.GONumber","tm.SchemeName","wm.WorkCompletionDate","wm.WorkCommencementDate", "tm.TenderStatus" };
                    foreach (var column in columnsToSearch)
                    {
                        if (!string.IsNullOrEmpty(column))
                        {
                            searchCondition += column + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                        }
                    }
                    searchCondition = searchCondition.Remove(searchCondition.Length - 3);
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

                if (model?.Sorting != null && !string.IsNullOrWhiteSpace(model?.Sorting?.FieldName) && !string.IsNullOrWhiteSpace(model?.Sorting?.Sort))
                {
                    string FieldName = "";

                    #region Select Field
                    if (string.Equals(model?.Sorting.FieldName, "WorkType", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "wt.Value";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "DivisionName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "tm.DivisionName";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "DistrictName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "tm.DistrictName";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "BidType", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "tm.BidType";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "WorkAmount", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "tm.TenderFinalAmount";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "WorkProgress", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "wm.WorkProgress";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "TenderNumber", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "tm.TenderNumber";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "MilestoneName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "ms.MilestoneName";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "PercentageCompleted", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "ms.PercentageCompleted";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "StartDate", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "tm.StartDate";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "EndDate", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "tm.EndDate";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "WorkCommencementDate", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "wm.WorkCommencementDate";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "WorkCompletionDate", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "wm.WorkCompletionDate";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "GONumber", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "go.GONumber";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "PercentageCompleted", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "ms.PercentageCompleted";
                    }
                    else
                    {
                        FieldName = "wtm.CreatedDate";
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
                    Condition += " ORDER BY CreatedDate ";
                }
                else
                {
                    Condition += " ORDER BY CreatedDate LIMIT " + model?.Take + " OFFSET " + model?.Skip;
                }

                Query += Condition;

                #endregion Build Query Conditions
            }

            return SqlMapper.Query<TenderMasterModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<TenderMasterModel>();
        }
        public string Tender_SaveUpdate(TenderMasterModel model)
        {
            dynamic @params = new
            {
                pId = model.Id,
                pGoId = model.GoId,
                pTenderNumber = model.TenderNumber,
                pStartDate = model.StartDate,
                pEndDate = model.EndDate,
                pDivision = model.Division,
                pDistrict = model.District,
                pDivisionName = model.DivisionName,
                pDistrictName = model.DistrictName,
                pClass = model.Class,
                pCategory = model.Category,
                pWorkValue = model.WorkValue,
                pMainCategory = model.MainCategory,
                pSubcategory = model.Subcategory,
                pBidType = model.BidType,
                pContractorName = model.ContractorName,
                pContractorDivision = model.ContractorDivision,
                pContractorDistrict = model.ContractorDistrict,
                pContractorCategory = model.ContractorCategory,
                pContractorMobile = model.ContractorMobile,
                pContractorEmail = model.ContractorEmail,
                pContractorAddress = model.ContractorAddress,
                pContractorAltMobile = model.ContractorAltMobile,
                pContractorId = model.ContractorId,
                pTenderFinalAmount = model.TenderFinalAmount,
                pDepartmentName = model.DepartmentName,
                pDepartmentCode = model.DepartmentCode,
                pDepartmentId = model.DepartmentId,
                pIsActive = true,

                pContractorCompanyAddress = model.ContractorCompanyAddress,
                pContractorCompanyName = model.ContractorCompanyName,
                pContractorCompanyPhone = model.ContractorCompanyPhone,
                pAPI_Responce = model.API_Responce,

                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = DateTime.Now,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Tender_Master_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }
        public string Tender_Update_Amount(TenderAmountUpdateModel model)
        {
            dynamic @params = new
            {
                pUpdatedNote = model.UpdatedNote,
                pTenderId = model.TenderId,
                pIncreasedAmount = model.IncreasedAmount,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Tender_Increase_WorkValue", @params, commandType: CommandType.StoredProcedure);
        }
        //public List<TenderMasterModel> Tender_SaveUpdate_Bulk(List<TenderMasterModel> list)
        //{
        //    if (list?.Count > 0)
        //    {
        //        list.ForEach(model =>
        //        {
        //            model.Id = Guid.NewGuid().ToString();
        //            string Id = SaveData(model);

        //            if (Id.Contains("@@1"))
        //            {
        //                model.Id = Id.Split("/")[0].ToString();
        //                SaveData(model);
        //            }
        //            else
        //            {
        //                model.Id = Id;
        //            }
        //        });

        //        return Tender_Get();
        //    }



        //    return new List<TenderMasterModel>();
        //}



        //Updated by Vijay on 12-11-2025 to send update and insert data count in the email

       
        public List<TenderMasterModel> Tender_SaveUpdate_Bulk(List<TenderMasterModel> list)
        {
            if (list?.Count > 0)
            {
                int insertedCount = 0;
                int updatedCount = 0;

                List<string> insertedTenderNumbers = new();
                List<string> updatedTenderNumbers = new();

                foreach (var model in list)
                {
                    model.Id = Guid.NewGuid().ToString();

                    string result = SaveData(model);

                    if (result.Contains("@@1"))  // duplicate -> update
                    {
                        string existingId = result.Split("/")[0];

                        // No second DB call — avoid timeout
                        model.Id = existingId;
                        updatedCount++;
                        updatedTenderNumbers.Add(model.TenderNumber);
                    }
                    else
                    {
                        // Insert
                        model.Id = result;
                        insertedCount++;
                        insertedTenderNumbers.Add(model.TenderNumber);
                    }
                }

                // store for email later
                TenderrecordsModel.InsertedTenderCount = insertedCount;
                TenderrecordsModel.UpdatedTenderCount = updatedCount;
                TenderrecordsModel.InsertedTenderNumbers = insertedTenderNumbers;
                TenderrecordsModel.UpdatedTenderNumbers = updatedTenderNumbers;

                return Tender_Get();
            }

            return new List<TenderMasterModel>();
        }


        public string SaveData(TenderMasterModel model)
        {
            dynamic @params = new
            {
                pId = model.Id,
                pGoId = model.GoId,
                pTenderNumber = model.TenderNumber,
                pStartDate = model.StartDate,
                pEndDate = model.EndDate,
                pDivision = model.Division,
                pDistrict = model.District,
                pDivisionName = model.DivisionName,
                pDistrictName = model.DistrictName,
                pClass = model.Class,
                pCategory = model.Category,
                pWorkValue = model.WorkValue,
                pMainCategory = model.MainCategory,
                pSubcategory = model.Subcategory,
                pBidType = model.BidType,
                pContractorName = model.ContractorName,
                pContractorDivision = model.ContractorDivision,
                pContractorDistrict = model.ContractorDistrict,
                pContractorCategory = model.ContractorCategory,
                pContractorMobile = model.ContractorMobile,
                pContractorEmail = model.ContractorEmail,
                pContractorAddress = model.ContractorAddress,
                pContractorAltMobile = model.ContractorAltMobile,
                pContractorId = model.ContractorId,
                pTenderFinalAmount = model.TenderFinalAmount,
                pDepartmentName = model.DepartmentName,
                pDepartmentCode = model.DepartmentCode,
                pDepartmentId = model.DepartmentId,
                pIsActive = true,

                pContractorCompanyAddress = model.ContractorCompanyAddress,
                pContractorCompanyName = model.ContractorCompanyName,
                pContractorCompanyPhone = model.ContractorCompanyPhone,
                pAPI_Responce = model.API_Responce,

                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = DateTime.Now,
                pTSchemeID = model.SchemeId,
                ptSchemeName = model.SchemeName,
                ptSchemeCode = model.SchemeCode,
                pWorkserialno = model.WorkSerialNumber,
                ptender_opening_date = model.TenderOpenedDate,
                pGo_package_No = model.Go_package_No,
                pservice_type_main_id=model.service_type_main_id,
                pcategory_type_main_id=model.category_type_main_id,
                pservice_type_main=model.service_type_main,
                pcategory_type_main = model.category_type_main,


                pWorkorder = model.Workorder,
                pNegotiation_signed_doc = model.Negotiation_signed_doc,
                pOthers_doc = model.Others_doc
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            string Id = SqlMapper.ExecuteScalar<string>(connection, "Tender_Master_SaveUpdate", @params, commandType: CommandType.StoredProcedure);

            return Id;
        }
     
        
        
        public List<string> Tender_Ids_Get_ByContractor(string ContractorId)
        {
            dynamic @params = new
            {
                pContractorId = ContractorId.Trim()
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<string>(connection, "Tender_Id_Get_ByContractor", @params, commandType: CommandType.StoredProcedure) ?? new List<string>();
        }
        public List<string> Mbook_Id_Get_ByContractor(string ContractorId)
        {
            dynamic @params = new
            {
                pContractorId = ContractorId.Trim()
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<string>(connection, "Mbook_Id_Get_ByContractor", @params, commandType: CommandType.StoredProcedure) ?? new List<string>();
        }
        public List<string> Division_Ids_Get_ByContractor(string ContractorId)
        {
            dynamic @params = new
            {
                pContractorId = ContractorId.Trim()
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<string>(connection, "Division_Id_Get_ByContractor", @params, commandType: CommandType.StoredProcedure) ?? new List<string>();
        }
        public List<string> DivisionId_getbyUserId(string pUserId)
        {
            dynamic @params = new
            {
                pUserId = pUserId.Trim()
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<string>(connection, "DivisionId_getbyUserId", @params, commandType: CommandType.StoredProcedure) ?? new List<string>();
        }
        public List<string> Department_Ids_Get_ByContractor(string ContractorId)
        {
            dynamic @params = new
            {
                pContractorId = ContractorId.Trim()
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<string>(connection, "Department_Id_Get_ByContractor", @params, commandType: CommandType.StoredProcedure) ?? new List<string>();
        }
        public string Tender_Update_Status(TenderMasterModel model)
        {
            dynamic @params = new
            {
                pId = model.Id,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = DateTime.Now,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Tender_Master_Update_Status", @params, commandType: CommandType.StoredProcedure);
        }
        public List<TenderRelatedIdModel> Get_TenderRelatedIds(string TenderId)
        {
            dynamic @params = new
            {
                pTenderId = TenderId
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<TenderRelatedIdModel>(connection, "Tender_Get_Related_Ids", @params, commandType: CommandType.StoredProcedure) ?? new List<TenderRelatedIdModel>();
        }
        public List<AccountUserModel> Tender_User_Get(string TenderId = "", string MBookId = "", string MilestoneId = "")
        {
            dynamic @params = new
            {
                pTenderId = TenderId?.Trim() ?? "",
                pMBookId = MBookId?.Trim() ?? "",
                pMilestoneId = MilestoneId?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<AccountUserModel>(connection, "Tender_User_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<AccountUserModel>();
        }
        #endregion Tender

        #region Work

        public List<WorkMasterModel> Work_Get(bool IsActive = true, string Id = "", string TenderId = "", string WorkNumber = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pIsActive = IsActive,
                pTenderId = TenderId?.Trim() ?? "",
                pWorkNumber = WorkNumber?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<WorkMasterModel>(connection, "Work_Master_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<WorkMasterModel>();
        }
        public List<WorkMasterModel> Work_Get_All(bool IsActive = true, string Id = "", string TenderId = "", string WorkNumber = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pIsActive = IsActive,
                pTenderId = TenderId?.Trim() ?? "",
                pWorkNumber = WorkNumber?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<WorkMasterModel>(connection, "Work_Master_Get_All", @params, commandType: CommandType.StoredProcedure) ?? new List<WorkMasterModel>();
        }
        public List<WorkMasterModel> Work_Get(WorkFilterModel model, out int TotalCount)
        {
            TotalCount = 0;
            // List<StatusMaster>? status_list = _settingDAL.Status_Get(Type: StatusTypeConst.Tender);
            StatusMaster? notStartedstatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.NotStarted).FirstOrDefault();
            StatusMaster? SlowProgressstatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.SlowProgress).FirstOrDefault();
            StatusMaster? OnHoldstatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.StartedbutStilled).FirstOrDefault();
            StatusMaster? CompletedStatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.Completed).FirstOrDefault();
            StatusMaster? InprogressStatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.Inprogress).FirstOrDefault();
            StatusMaster? NewStatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.New).FirstOrDefault();

            string Query = @"SELECT tm.Id, tm.TenderNumber, tm.StartDate, tm.EndDate, tm.Division, tm.District, tm.Class, tm.Category, tm.WorkValue, tm.MainCategory, tm.Subcategory,
                                tm.ContractorId,
                                tm.BidType, tm.ContractorName, tm.ContractorDivision, tm.ContractorDistrict, tm.ContractorCategory, tm.TenderFinalAmount, tm.TenderOpenedDate,tm.Go_Package_No,
                                tm.LocalTenderNumber, tm.DistrictName, tm.DivisionName, tm.ContractorCompanyName, tm.ContractorCompanyAddress, tm.ContractorCompanyPhone, tm.ContractorMobile,
                                wm.Id,tm.Id as TenderId, wm.WorkOrderId, wm.AgreementCopyId, wm.WorkTemplateId, wm.CalenderLeaveTypes, 
                                                            wm.LetterOfAcceptanceId,wm.IsActive, wm.Prefix, wm.Suffix, wm.RunningNumber, wm.WorkNumber, wm.GoId,tm.SchemeName,wm.WorkCommencementDate,wm.WorkCompletionDate,wm.DateDifference,
                                                            wm.CreatedBy, wm.CreatedDate, wm.CreatedByUserName, wm.ModifiedBy, wm.ModifiedDate, 
                                                            wm.ModifiedByUserName, wm.DeletedBy, wm.DeletedByUserName, wm.DeletedDate, wm.OtherFileId, wm.WorkStatus,
                                                          go.GONumber, go.GODate, go.GOCost, go.GOName, go.GODepartment, go.GORevisedAmount, go.GOTotalAmount,
                                 go.LocalGONumber, go.departmentId,

                                wtm.Id as Work_Template_Id, wtm.TemplateId, wtm.WorkId, wtm.WorkTemplateName, wtm.WorkTypeId,
                                tcc.Value as WorkType,
                                wtm.WorkTypeId, wtm.SubWorkTypeId, swt.Value as SubWorkType, wtm.Strength, wtm.TemplateCode,
                                wtm.WorkDurationInDays, wtm.WorkTemplateNumber,
                                   ifnull(tm.TenderStatus,'Not-Started') as TenderStatus,
                                   ifnull(tm.TenderStatus,'Not-Started') as WorkStatus,             
                                   ifnull(wsm.StatusName,'Not-Started') as WorkStatusName
                                from  tender_master tm 
                                LEFT JOIN work_master wm  ON tm.Id=wm.TenderId
                                LEFT JOIN go_master go ON go.Id = tm.GoId
                                LEFT JOIN work_template_master wtm ON wtm.WorkId = wm.Id  AND wtm.IsActive=1
                                LEFT JOIN two_column_configuration_values tcc ON tcc.Id = wtm.WorkTypeId
                                LEFT JOIN status_master wsm ON wsm.Id = wm.WorkStatus
                                LEFT JOIN two_column_configuration_values swt ON swt.Id = wtm.SubWorkTypeId";

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            if (model != null)
            {
                string Condition = " WHERE ";

                if (model.Where != null)
                {
                    PropertyInfo[] whereProperties = typeof(WorkWhereClauseProperties).GetProperties();
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
                            Condition += "wm." + property.Name + "='" + value.Replace('\'', '%').Trim() + "' AND ";
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";
                    List<string> whereProperties = new List<string>() { "go.GONumber", "tm.DivisionName", "tm.TenderNumber",
                        "tm.EndDate", "tm.StartDate", "tcc.Value", "tm.DistrictName", "tm.Category","tm.SchemeName", "tm.ContractorName" };
                    foreach (var property in whereProperties)
                    {
                        searchCondition += property + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                    }
                    searchCondition = searchCondition.Remove(searchCondition.Length - 3);
                    searchCondition += ") AND";

                    Condition += searchCondition;
                }

                if (string.Equals(model?.RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (model?.TenderIds?.Count > 0)
                    {
                        string tenderIds_str = "";
                        model.TenderIds.ForEach(x =>
                        {
                            tenderIds_str += "'" + x + "',";
                        });
                        tenderIds_str = tenderIds_str.Trim().Trim(',');
                        if (!string.IsNullOrWhiteSpace(tenderIds_str))
                        {
                            Condition += " tm.Id IN (" + tenderIds_str + ") AND ";
                        }
                    }
                }
                else
                {

                    if (model?.DistrictList?.Count > 0)
                    {
                        string districtIds_str = "";
                        model.DistrictList.ForEach(x =>
                        {
                            districtIds_str += "'" + x + "',";
                        });
                        districtIds_str = districtIds_str.Trim().Trim(',');
                        if (!string.IsNullOrWhiteSpace(districtIds_str))
                        {
                            Condition += " tm.District IN (" + districtIds_str + ") AND ";
                        }
                    }
                    if (model?.DepartmentList?.Count > 0)
                    {
                        string departmentIds_str = "";
                        model.DepartmentList.ForEach(x =>
                        {
                            departmentIds_str += "'" + x + "',";
                        });
                        departmentIds_str = departmentIds_str.Trim().Trim(',');
                        if (!string.IsNullOrWhiteSpace(departmentIds_str))
                        {
                            Condition += " tm.DepartmentId IN (" + departmentIds_str + ") AND ";
                        }
                    }

                    if (model?.PackageList?.Count > 0)
                    {
                        string PackageList_str = "";
                        model.PackageList.ForEach(x =>
                        {
                            PackageList_str += "'" + x + "',";
                        });
                        PackageList_str = PackageList_str.Trim().Trim(',');
                        if (!string.IsNullOrWhiteSpace(PackageList_str))
                        {
                            Condition += " tm.Go_Package_No IN (" + PackageList_str + ") AND ";
                        }
                    }

                    if (model?.DivisionList?.Count > 0)
                    {
                        string divisionIds_str = "";
                        model.DivisionList.ForEach(x =>
                        {
                            divisionIds_str += "'" + x + "',";

                        });
                        divisionIds_str = divisionIds_str.Trim().Trim(',');
                        if (!string.IsNullOrWhiteSpace(divisionIds_str))
                        {
                            Condition += " tm.Division IN (" + divisionIds_str + ") AND ";
                        }
                    }
                }
                if (model?.StatusList?.Count > 0)
                {
                    bool sts = false;
                    bool hasSlow = false;
                    bool hasOnHold = false;

                    if (notStartedstatus != null && model.StatusList.Contains(notStartedstatus.Id))
                    {
                        sts = true;
                    }

                    string statusIds_str = string.Join(",", model.StatusList.Select(x => $"'{x}'"));

                    if (SlowProgressstatus != null && model.StatusList.Contains(SlowProgressstatus.Id))
                    {
                        hasSlow = true;
                    }

                    if (OnHoldstatus != null && model.StatusList.Contains(OnHoldstatus.Id))
                    {
                        hasOnHold = true;
                    }

                    if (!string.IsNullOrWhiteSpace(statusIds_str))
                    {
                        // collect OR conditions in a group
                        List<string> orConds = new List<string>();

                        if (hasSlow)
                        {
                            orConds.Add(@"(wm.DateDifference >= CAST(SUBSTRING_INDEX(
                (SELECT Value FROM two_column_configuration_values WHERE Code = 'SLOW'), '-', 1
            ) AS UNSIGNED)
            AND wm.DateDifference <= CAST(SUBSTRING_INDEX(
                (SELECT Value FROM two_column_configuration_values WHERE Code = 'SLOW'), '-', -1
            ) AS UNSIGNED))");
                        }

                        if (hasOnHold)
                        {
                            orConds.Add(@"(wm.DateDifference >= CAST(
                (SELECT Value FROM two_column_configuration_values WHERE Code = 'ONHOLD') AS UNSIGNED
            ))");
                        }

                        if (sts)
                        {
                            orConds.Add($"(wm.WorkStatus IN ({statusIds_str}) OR wm.WorkStatus IS NULL)");
                        }
                        else if (model.StatusList.Contains(CompletedStatus.Id) ||
                                 model.StatusList.Contains(InprogressStatus.Id) ||
                                 model.StatusList.Contains(NewStatus.Id))
                        {
                            orConds.Add($"(wm.WorkStatus IN ({statusIds_str}))");
                        }

                        if (orConds.Count > 0)
                        {
                            Condition += "(" + string.Join(" OR ", orConds) + ") AND ";
                        }
                    }
                }


                if (model?.SchemeList?.Count() > 0)
                {
                    string schemestr = "";
                    model.SchemeList.ForEach(x =>
                    {
                        schemestr += "'" + x + "',";
                    });
                    schemestr = schemestr.Trim().Trim(',');
                    //if (!string.IsNullOrWhiteSpace(schemestr))
                    //{
                    //    Condition += " (tm.SchemeId IS NULL OR tm.SchemeId IN (" + schemestr + ")) AND ";
                    //}

                    Condition += "(";
                    foreach (var i in model.SchemeList)
                    {
                        Condition += " tm.SchemeId ='" +i+"'  or ";
                    }
                    if(model.SchemeList.Any(x=>x=="-1"))
                    {
                        Condition += " tm.SchemeId IS NULL  or ";

                    }
                    Condition = Condition.Remove(Condition.Length - 5);
                    Condition += ") AND ";
                }

                //if(model?.SchemeList)
                //{
                //    Condition += " tm.SchemeId IN (" + schemestr + ") AND ";
                //}

                if (model?.FromDate != null && model?.ToDate != null)
                {
                    //Condition += " (wm.WorkCommencementDate between '" + model.FromDate?.ToString("yyyy-MM-dd") + "' AND '" + model.ToDate?.ToString("yyyy-MM-dd") + "' OR ";
                    //Condition += " wm.WorkCompletionDate between '" + model.FromDate?.ToString("yyyy-MM-dd") + "' AND '" + model.ToDate?.ToString("yyyy-MM-dd") + "' OR ";
                    Condition += " ((tm.StartDate between '" + model.FromDate?.ToString("yyyy-MM-dd") + "' AND '" + model.ToDate?.ToString("yyyy-MM-dd") + "') OR ";
                    Condition += " (tm.EndDate between '" + model.FromDate?.ToString("yyyy-MM-dd") + "' AND '" + model.ToDate?.ToString("yyyy-MM-dd") + "') OR (tm.StartDate <= '" + model.FromDate?.ToString("yyyy-MM-dd") + "' AND  tm.EndDate>= '" + model.ToDate?.ToString("yyyy-MM-dd") + "' )) AND ";


                }
                if (!string.IsNullOrWhiteSpace(model?.Days))
                {
                    var Days = model?.Days.Split("-");
                    Condition += " wm.DateDifference >= " + Days[0] + " AND";
                    Condition += " wm.DateDifference <= " + Days[1] + " AND";
                }

                if (!string.IsNullOrWhiteSpace(model?.DivisionId))
                {
                    Condition += " tm.Division = '" + model.DivisionId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.DistrictId))
                {
                    Condition += " tm.District = '" + model.DistrictId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.WorkTypeId))
                {
                    Condition += " wtm.WorkTypeId = '" + model.WorkTypeId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.SubWorkTypeId))
                {
                    Condition += " wtm.SubWorkTypeId = '" + model.SubWorkTypeId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.MainCategory))
                {
                    Condition += " tm.mainCategory = '" + model.MainCategory + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.Subcategory))
                {
                    Condition += " tm.subcategory = '" + model.Subcategory + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.Strength))
                {
                    Condition += " wtm.Strength = '" + model.Strength + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.GoId))
                {
                    Condition += " go.Id = '" + model.GoId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.Status))
                {

                    Condition += " wm.WorkStatus = '" + model.Status + "' AND";

                }
                if (!string.IsNullOrWhiteSpace(model?.Contractor))
                {
                    Condition += " tm.ContractorId = '" + model.Contractor + "' AND";
                }


                if (!string.IsNullOrWhiteSpace(model?.ContractorDistrict))
                {
                    Condition += " tm.District = '" + model.ContractorDistrict + "' AND";
                }

                // ==============================================================================

                if (Condition.Substring(Condition.Length - 5) == " AND ")
                {
                    Condition = Condition.Remove(Condition.Length - 5);
                }
                if (Condition.Substring(Condition.Length - 4) == " AND")
                {
                    Condition = Condition.Remove(Condition.Length - 4);
                }
                if (Condition == " WHERE ")
                {
                    Condition = "";
                }

                TotalCount = (SqlMapper.Query<TenderMasterModel>(connection, Query + Condition, commandType: CommandType.Text)?.ToList() ?? new List<TenderMasterModel>()).Count();

                if (model?.Sorting != null && !string.IsNullOrWhiteSpace(model?.Sorting.FieldName) && !string.IsNullOrWhiteSpace(model?.Sorting.Sort))
                {
                    if (model?.Skip == 0 && model?.Take == 0)
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " ";
                    }
                    else
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                    }
                }
                else if (model?.Skip == 0 && model?.Take == 0)
                {
                    Condition += " ORDER BY CreatedDate ";
                }
                else
                {
                    Condition += " ORDER BY CreatedDate LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                }

                Query += Condition;
            }


            return SqlMapper.Query<WorkMasterModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<WorkMasterModel>();
        }
        public bool Work_Is_Completed(string WorkId)
        {
            dynamic @params = new
            {
                pWorkId = WorkId
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            string IsCompletedString = SqlMapper.ExecuteScalar<string>(connection, "Work_Is_Completed", @params, commandType: CommandType.StoredProcedure);

            return Convert.ToInt32(IsCompletedString) == 1;
        }
        public string Work_SaveUpdate(WorkMasterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
            }

            dynamic @params = new
            {
                pId = model.Id,
                pTenderId = model.TenderId,
                pWorkOrderId = model.WorkOrderId,
                pAgreementCopyId = model.AgreementCopyId,
                pWorkTemplateId = model.WorkTemplateId,
                pLetterOfAcceptanceId = model.LetterOfAcceptanceId,
                pCalenderLeaveTypes = model.CalenderLeaveTypes,
                pOtherFileId = model.OtherFileId,
                pIsActive = model.IsActive,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Work_Master_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }

        #endregion Work

        #region Work Template
        public List<WorkTemplateMasterModel> Work_Template_Get(bool IsActive = true, string Id = "", string WorkId = "", string WorkTypeId = "", string TemplateId = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pIsActive = IsActive,
                pWorkId = WorkId?.Trim() ?? "",
                pWorkTypeId = WorkTypeId?.Trim() ?? "",
                pTemplateId = TemplateId?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<WorkTemplateMasterModel>(connection, "Work_Template_Master_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<WorkTemplateMasterModel>();
        }
        public string Work_Template_SaveUpdate(WorkTemplateMasterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
            }
            dynamic @params = new
            {
                pId = model.Id,
                pTemplateId = model.TemplateId,
                pWorkId = model.WorkId,
                pWorkTemplateName = model.WorkTemplateName,
                pWorkTypeId = model.WorkTypeId,
                pserviceTypeId = model.serviceTypeId,
                pcategoryTypeId = model.categoryTypeId,
                pWorkDurationInDays = model.WorkDurationInDays,
                pSubWorkTypeId = model.SubWorkTypeId,
                pStrength = model.Strength,
                pTemplateCode = model.TemplateCode,
                pIsActive = model.IsActive,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Work_Template_Master_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }
        public string Work_Template_Submit(WorkTemplateMasterModel model)
        {
            dynamic @params = new
            {
                pId = model.Id,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Work_Template_Submit", @params, commandType: CommandType.StoredProcedure);
        }

        #endregion Work Template

        //TEMPLATE RE-ASSIGN by Indu on march 3 2025
        public string DeleteWorkTemplate(string WorkId, string SavedBy, string SavedByUserName, DateTime SavedDate)
        {
            dynamic @params = new
            {
                pWorkId = WorkId,
                pSavedBy = SavedBy,
                pSavedByUserName = SavedByUserName,
                pSavedDate = SavedDate,
            };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            string rowsAffected = SqlMapper.ExecuteScalar<string>(connection, "DeleteWorkTemplate", @params, commandType: CommandType.StoredProcedure);
            return rowsAffected;

        }

        public string UpdateDateDifference()
        {
            dynamic @params = new
            {

            };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            string rowsAffected = SqlMapper.ExecuteScalar<string>(connection, "UpdateDatedifference", @params, commandType: CommandType.StoredProcedure);
            return rowsAffected;

        }
        public string UpdateCameraDatabase()
        {
            dynamic @params = new
            {

            };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            string rowsAffected = SqlMapper.ExecuteScalar<string>(connection, "UpdateCameraDatabase", @params, commandType: CommandType.StoredProcedure);
            return rowsAffected;

        }





        #region Work Template Milestone
        public List<WorkTemplateMilestoneMasterModel> Work_Template_Milestone_Master_Get(bool IsActive = true, string Id = "",
            string WorkTemplateId = "", string WorkId = "", string TenderId = "", string DivisionId = "", string MilestoneStatusId = "", string PaymentStatusId = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pIsActive = IsActive,
                pWorkTemplateId = WorkTemplateId?.Trim() ?? "",
                pWorkId = WorkId?.Trim() ?? "",
                pPaymentStatusId = PaymentStatusId?.Trim() ?? "",
                pMilestoneStatusId = MilestoneStatusId?.Trim() ?? "",
                pDivisionId = DivisionId?.Trim() ?? "",
                pTenderId = TenderId?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<WorkTemplateMilestoneMasterModel>(connection, "Work_Template_Milestone_Master_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<WorkTemplateMilestoneMasterModel>();
        }
        public List<MilestoneReportModel> Milestone_Get(MilestoneFilterModel model, out int TotalCount)
        {
            TotalCount = 0;

            string Query = @"select
            wtm.WorkTypeId, tcc.Value as WorkType, wtm.SubWorkTypeId, swt.Value as SubWorkType, wtm.Strength,
            wtm.WorkTemplateName, wm.WorkNumber,
            tm.Division, tm.District, tm.DistrictName, tm.DivisionName, tm.TenderNumber,
            wmm.Id, wmm.WorkId, wmm.WorkTemplateId, wmm.MilestoneName, wmm.OrderNumber, 
            wmm.DurationInDays, (datediff(wmm.StartDate, CURDATE())) as DurationInDaysLeft,
            wmm.IsPaymentRequired, wmm.PaymentPercentage, wmm.StartDate, wmm.EndDate, wmm.PercentageCompleted,
            wmm.MilestoneCode, wmm.PaymentStatus, psm.StatusName as PaymentStatusName,
            wmm.MilestoneStatus, msm.StatusName as MilestoneStatusName, wmm.CompletedDate,
            wmm.MilestoneAmount, wmm.IsCompleted, wmm.ActualAmount,

            wmm.MilestoneAmount as PannedValue,
            wmm.ActualAmount as ActualValue,
            (CASE 
                WHEN wmm.IsCompleted = 1 THEN wmm.ActualAmount
                ELSE null
            END) as PaymentValue,

            IFNULL((SELECT CONCAT(Id, '|', SavedFileName) FROM file_master WHERE TypeId = wmm.Id AND Type='MilestoneImage' ORDER BY CreatedDate DESC LIMIT 1),'') as 'MilestoneFile1Saved',
            IFNULL((SELECT OriginalFileName FROM file_master WHERE TypeId = wmm.Id AND Type='MilestoneImage' ORDER BY CreatedDate DESC LIMIT 1),'') as 'MilestoneFile1Original',

            IFNULL((SELECT CONCAT(Id, '|', SavedFileName) FROM file_master WHERE TypeId = wmm.Id AND Type='MilestoneImage' ORDER BY CreatedDate DESC LIMIT 1 OFFSET 1),'') as 'MilestoneFile2Saved',
            IFNULL((SELECT OriginalFileName FROM file_master WHERE TypeId = wmm.Id AND Type='MilestoneImage' ORDER BY CreatedDate DESC LIMIT 1 OFFSET 1),'') as 'MilestoneFile2Original'

            from work_template_milestone_master wmm
            inner join work_master wm on wm.Id = wmm.WorkId
            inner join tender_master tm on tm.Id = wm.TenderId
            inner join work_template_master wtm on wtm.Id = wmm.WorkTemplateId
            left join two_column_configuration_values tcc on tcc.Id = wtm.WorkTypeId
            left join two_column_configuration_values swt on swt.Id = wtm.SubWorkTypeId
            left join status_master msm on msm.Id = wmm.MilestoneStatus
            left join status_master psm on psm.Id = wmm.PaymentStatus";

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            if (model != null)
            {
                //Template reassign
                //Filter only active milestones






                string Condition = " WHERE wtm.IsActive = 1 AND ";

                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";
                    List<string> whereProperties = new List<string>() { "tcc.Value", "swt.Value", "wtm.Strength", "wmm.MilestoneName",
                        "wtm.WorkTemplateName", "tm.TenderNumber", "wm.WorkNumber", "wmm.MilestoneCode", "psm.StatusName", "msm.StatusName",
                        "wmm.StartDate", "wmm.EndDate", "wmm.MilestoneAmount", "wmm.ActualAmount"
                    };
                    foreach (var property in whereProperties)
                    {
                        searchCondition += property + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                    }
                    searchCondition = searchCondition.Remove(searchCondition.Length - 3);
                    searchCondition += ") AND";

                    Condition += searchCondition;
                }


                if (model?.DistrictList?.Count > 0)
                {
                    string districtIds_str = "";
                    model.DistrictList.ForEach(x =>
                    {
                        districtIds_str += "'" + x + "',";
                    });
                    districtIds_str = districtIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(districtIds_str))
                    {
                        Condition += " tm.District IN (" + districtIds_str + ") AND ";
                    }
                }
                if (model?.DivisionList?.Count > 0)
                {
                    string divisionIds_str = "";
                    model.DivisionList.ForEach(x =>
                    {
                        divisionIds_str += "'" + x + "',";
                    });
                    divisionIds_str = divisionIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(divisionIds_str))
                    {
                        Condition += " tm.Division IN (" + divisionIds_str + ") AND ";
                    }
                }
                if (model?.DepartmentList?.Count > 0)
                {
                    string departmentIds_str = "";
                    model.DepartmentList.ForEach(x =>
                    {
                        departmentIds_str += "'" + x + "',";
                    });
                    departmentIds_str = departmentIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(departmentIds_str))
                    {
                        Condition += " tm.DepartmentId IN (" + departmentIds_str + ") AND ";
                    }
                }
                if (model?.StatusList?.Count > 0)
                {
                    string statusIds_str = "";
                    model.StatusList.ForEach(x =>
                    {
                        statusIds_str += "'" + x + "',";
                    });
                    statusIds_str = statusIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(statusIds_str))
                    {
                        Condition += " wmm.MilestoneStatus IN (" + statusIds_str + ") AND ";
                    }
                }
                if (model?.FromDate != null && model?.ToDate != null)
                {
                    Condition += " wmm.StartDate >= '" + model.FromDate?.ToString("yyyy-MM-dd") + "' AND";
                    Condition += " wmm.EndDate <= '" + model.ToDate?.ToString("yyyy-MM-dd") + "' AND";
                }

                if (!string.IsNullOrWhiteSpace(model?.DivisionId))
                {
                    Condition += " tm.Division = '" + model.DivisionId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.DistrictId))
                {
                    Condition += " tm.District = '" + model.DistrictId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.WorkTypeId))
                {
                    Condition += " wtm.WorkTypeId = '" + model.WorkTypeId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.SubWorkTypeId))
                {
                    Condition += " wtm.SubWorkTypeId = '" + model.SubWorkTypeId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.Strength))
                {
                    Condition += " wtm.Strength = '" + model.Strength + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.WorkId))
                {
                    Condition += " wmm.WorkId = '" + model.WorkId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.ApprovalStatusId))
                {
                    Condition += " wmm.MilestoneStatus = '" + model.ApprovalStatusId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.ApprovalStatusName))
                {
                    Condition += " msm.StatusName = '" + model.ApprovalStatusName + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.PaymentStatusId))
                {
                    Condition += " wmm.PaymentStatus = '" + model.PaymentStatusId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.PaymentStatusName))
                {
                    Condition += " psm.StatusName = '" + model.PaymentStatusName + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.Cost))
                {
                    Condition += " wmm.MilestoneAmount = '" + model.Cost + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.ActualCost))
                {
                    Condition += " wmm.ActualAmount = '" + model.ActualCost + "' AND";
                }

                // ==============================================================================

                if (Condition.Substring(Condition.Length - 5) == " AND ")
                {
                    Condition = Condition.Remove(Condition.Length - 5);
                }
                if (Condition.Substring(Condition.Length - 4) == " AND")
                {
                    Condition = Condition.Remove(Condition.Length - 4);
                }
                if (Condition == " WHERE ")
                {
                    Condition = "";
                }

                TotalCount = (SqlMapper.Query<MilestoneReportModel>(connection, Query + Condition, commandType: CommandType.Text)?.ToList() ?? new List<MilestoneReportModel>()).Count();

                if (model?.Sorting != null && !string.IsNullOrWhiteSpace(model?.Sorting.FieldName) && !string.IsNullOrWhiteSpace(model?.Sorting.Sort))
                {
                    if (model?.Skip == 0 && model?.Take == 0)
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " ";
                    }
                    else
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                    }
                }
                else if (model?.Skip == 0 && model?.Take == 0)
                {
                    Condition += " ORDER BY wmm.CreatedDate ";
                }
                else
                {
                    Condition += " ORDER BY wmm.CreatedDate LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                }

                Query += Condition;
            }


            return SqlMapper.Query<MilestoneReportModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<MilestoneReportModel>();
        }
        public string Work_Template_Milestone_Master_SaveUpdate(WorkTemplateMilestoneMasterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
            }
            dynamic @params = new
            {
                pId = model.Id,
                pWorkTemplateId = model.WorkTemplateId,
                pMilestoneName = model.MilestoneName,
                pOrderNumber = model.OrderNumber,
                pDurationInDays = model.DurationInDays,
                pIsPaymentRequired = model.IsPaymentRequired,
                pStartDate = model.StartDate,
                pEndDate = model.EndDate,
                pPaymentPercentage = model.PaymentPercentage,
                pPercentageCompleted = model.PercentageCompleted,
                pPaymentStatus = model.PaymentStatus,
                pMilestoneStatus = model.MilestoneStatus,
                pActualAmount = model.ActualAmount,
                pMilestoneCode = model.MilestoneCode,
                pIsActive = model.IsActive,
                pIsCompleted = model.IsCompleted,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
                pCompletedDate = model.CompletedDate
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Work_Template_Milestone_Master_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }
        public string SetWorkCompletionDate(string WorkTemplateId)
        {
            dynamic @params = new
            {
                pWorkTemplateId = WorkTemplateId,
            };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "SetWorkCompletionDate", @params, commandType: CommandType.StoredProcedure);
        }

        public string Work_Template_Milestone_Master_Delete_All(WorkTemplateMilestoneMasterModel model)
        {
            dynamic @params = new
            {
                pWorkTemplateId = model.WorkTemplateId,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Work_Template_Milestone_Master_Delete_All", @params, commandType: CommandType.StoredProcedure);
        }

        public string Update_Milestone_Completed_Percentage(MilestoneUpdateModel model)
        {
            dynamic @params = new
            {
                pWorkMilestoneId = model.WorkMilestoneId,
                pCompletedPercentage = model.CompletedPercentage,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Work_Milestone_UpdateCompletedPercentage", @params, commandType: CommandType.StoredProcedure);
        }

        #endregion Work Template Milestone

        #region M-Book
        // Get by without checking IsActive status
        public List<MBookMasterModel> Work_MBook_GetById(string Id = "", string WorkTemplateMilestoneId = "", string ActionableRoleId = "", string DivisionId = "",
            string StatusId = "", string WorkId = "", string TenderId = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pWorkTemplateMilestoneId = WorkTemplateMilestoneId?.Trim() ?? "",
                pActionableRoleId = ActionableRoleId?.Trim() ?? "",
                pDivisionId = DivisionId?.Trim() ?? "",
                pStatusId = StatusId?.Trim() ?? "",
                pWorkId = WorkId?.Trim() ?? "",
                pTenderId = TenderId?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<MBookMasterModel>(connection, "Work_MBook_Master_GetById", @params, commandType: CommandType.StoredProcedure) ?? new List<MBookMasterModel>();
        }
        public List<MBookMasterModel> Work_MBook_Get(bool IsActive = true, string Id = "", string WorkTemplateMilestoneId = "", string ActionableRoleId = "", string DivisionId = "",
            string StatusId = "", string WorkId = "", string TenderId = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pIsActive = IsActive,
                pWorkTemplateMilestoneId = WorkTemplateMilestoneId?.Trim() ?? "",
                pActionableRoleId = ActionableRoleId?.Trim() ?? "",
                pDivisionId = DivisionId?.Trim() ?? "",
                pStatusId = StatusId?.Trim() ?? "",
                pWorkId = WorkId?.Trim() ?? "",
                pTenderId = TenderId?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<MBookMasterModel>(connection, "Work_MBook_Master_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<MBookMasterModel>();
        }
        public List<MBookMasterModel> Work_MBook_Get(MBookFilterModel model, out int TotalCount)
        {
            TotalCount = 0;

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            string Query = @"SELECT wtm.Id, wtm.WorkTemplateMilestoneId, wtm.WorkTemplateId, wtm.PaymentStatusId, wtm.Length, wtm.Width,
                            wtm.Heigth, wtm.ActionableRoleId, wtm.DivisionId, wtm.StatusName, wtm.StatusId, wtm.PaymentStatusName, wtm.PaymentStatusCode,
                            wtm.IsActive, wtm.CreatedBy, wtm.CreatedDate, wtm.CreatedByUserName, wtm.ModifiedBy, wtm.ModifiedDate, wtm.ModifiedByUserName, 
                            wtm.DeletedBy, wtm.DeletedByUserName, wtm.DeletedDate,wtm.WorkId,
    
                            ms.MilestoneName as MilestoneName,
                            ms.PercentageCompleted,
                            tm.TenderNumber,
                            tm.DistrictName,
                            tm.DivisionName,
                            tm.DepartmentId,
                            go.GONumber,
                            go.GODepartment,
                            go.GOName,

                            ms.StartDate,
                            ms.EndDate,
                            ms.PaymentStatus,
                            ms.MilestoneStatus,
                            ms.DurationInDays,
                            ms.PaymentPercentage,

                            ms.MilestoneCode,
                            ms.MileStoneId,
                            ms.MilestoneAmount,
                            ms.IsCompleted,
    
                            wtv.Value as WorkType,
                            swtv.Value as SubWorkType,
                            wtm.WorkNotes,
                            wtm.Date,
                            wtm.StatusCode,
                            wtm.Prefix,
                            wtm.Suffix,
                            wtm.RunningNumber,
                            wtm.MBookNumber,
                            wtm.ActualAmount,

                            wt.Strength,
                            wt.TemplateCode,
                            wt.IsSubmitted,

                            ms.MilestoneAmount as PannedValue,
                            wtm.ActualAmount as ActualValue,
                            (CASE 
                                WHEN ms.IsCompleted = 1 THEN wtm.ActualAmount
                                ELSE null
                            END) as PaymentValue
    
                            FROM work_mbook_master wtm 
                            LEFT JOIN work_template_master wt ON wt.Id = wtm.WorkTemplateId
                            LEFT JOIN two_column_configuration_values wtv ON wtv.Id = wt.WorkTypeId
                            LEFT JOIN two_column_configuration_values swtv ON swtv.Id = wt.SubWorkTypeId
                            INNER JOIN work_template_milestone_master ms ON ms.Id = wtm.WorkTemplateMilestoneId
                            INNER JOIN status_master psm ON psm.Id = ms.PaymentStatus
                            LEFT JOIN work_master wm ON wm.Id = wt.WorkId
                            LEFT JOIN go_master go ON go.Id = wm.GoId
                            LEFT JOIN tender_master tm ON tm.Id = wm.TenderId";


            if (model != null)
            {
                #region Build Query Conditions

                string Condition = " WHERE ";


                if (model.MilestonePercentageShouldBeGreaterThan)
                {
                    Condition += " ms.IsActive=1 AND wtm.IsActive=1 AND ms.PercentageCompleted  =100.00 AND ";
                }
                if (!string.IsNullOrWhiteSpace(model.RoleId) && model.RoleCode!="ADM")
                {
                    Condition += " wtm.StatusCode != 'SAVED' AND (wtm.ActionableRoleId = '" + model.RoleId + "' OR wtm.ActionableRoleId = '-2'  OR wtm.ActionableRoleId is null) AND ";
                }
                if (model.Where != null)
                {
                    PropertyInfo[] whereProperties = typeof(MBookWhereClauseProperties).GetProperties();
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
                            Condition += " wtm." + property.Name + "='" + value.Replace('\'', '%').Trim() + "' AND ";
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
                            if (string.Equals(item.FieldName, "DivisionName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.DivisionName";
                            }
                            else if (string.Equals(item.FieldName, "DistrictName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.DistrictName";
                            }
                            else if (string.Equals(item.FieldName, "GONumber", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "go.GONumber";
                            }
                            else if (string.Equals(item.FieldName, "TenderNumber", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "tm.TenderNumber";
                            }
                            else if (string.Equals(item.FieldName, "WorkType", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "wtv.Value";
                            }
                            else if (string.Equals(item.FieldName, "SubWorkType", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "swtv.Value";
                            }
                            else if (string.Equals(item.FieldName, "Strength", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "wt.Strength";
                            }
                            else if (string.Equals(item.FieldName, "MBookNumber", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "wtm.MBookNumber";
                            }
                            else if (string.Equals(item.FieldName, "MilestoneName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "ms.MilestoneName";
                            }
                            else if (string.Equals(item.FieldName, "milestoneCode", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "ms.milestoneCode";
                            }
                            else if (string.Equals(item.FieldName, "StartDate", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "ms.StartDate";
                            }
                            else if (string.Equals(item.FieldName, "EndDate", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "ms.EndDate";
                            }
                            else if (string.Equals(item.FieldName, "PercentageCompleted", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "ms.PercentageCompleted";
                            }
                            else if (string.Equals(item.FieldName, "PaymentPercentage", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "ms.PaymentPercentage";
                            }
                            else if (string.Equals(item.FieldName, "PaymentStatusName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "psm.StatusName";
                            }
                            else if (string.Equals(item.FieldName, "StatusName", StringComparison.CurrentCultureIgnoreCase))
                            {
                                columnName = "wtm.StatusName";
                            }
                            #endregion Field Name Select

                            //Condition += " " + columnName + "='" + item.SearchString.Replace('\'', '%').Trim() + "' AND ";
                            Condition += " " + columnName + " LIKE " + "'%" + item.SearchString.Replace('\'', '%').Trim() + "%' AND ";
                        }
                    }
                }
                if (model?.DivisionIds?.Count > 0 && model.RoleCode != "ADM")
                {
                    string divisionIds_str = "";
                    model.DivisionIds.ForEach(x =>
                    {
                        divisionIds_str += "'" + x + "',";
                    });
                    divisionIds_str = divisionIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(divisionIds_str))
                    {
                        Condition += " wtm.DivisionId IN (" + divisionIds_str + ") AND ";
                    }
                }
                if (model?.TenderIds?.Count > 0)
                {
                    string tenderIds_str = "";
                    model.TenderIds.ForEach(x =>
                    {
                        tenderIds_str += "'" + x + "',";
                    });
                    tenderIds_str = tenderIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(tenderIds_str))
                    {
                        Condition += " tm.Id IN (" + tenderIds_str + ") AND ";
                    }
                }
                if (model?.DepartmentIds?.Count > 0 && model.RoleCode != "ADM")
                {
                    string departmentIds_str = "";
                    model.DepartmentIds.ForEach(x =>
                    {
                        departmentIds_str += "'" + x + "',";
                    });
                    departmentIds_str = departmentIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(departmentIds_str))
                    {
                        Condition += " tm.DepartmentId IN (" + departmentIds_str + ") AND ";
                    }
                }
                if (model?.Year?.Count > 0)
                {
                    //string searchCondition = " (";
                    //foreach (string year in model.Year)
                    //{
                    //    searchCondition += " (YEAR(ms.StartDate) = '" + year + "' || YEAR(ms.EndDate) = '" + year + "') OR ";
                    //}
                    //if (searchCondition.Substring(searchCondition.Length - 3) == "OR ")
                    //{
                    //    searchCondition = searchCondition.Remove(searchCondition.Length - 3);
                    //}
                    //searchCondition += ") AND ";

                    //Condition += searchCondition;

                    List<int> yearList = model.Year.Select(s => Convert.ToInt32(s)).OrderBy(x => x).ToList();
                    int fromYear = yearList?.FirstOrDefault() ?? 2020;
                    int toYear = yearList?.LastOrDefault() ?? 4000;

                    DateTime StartDate = new DateTime(Convert.ToInt32(fromYear), 4, 1);
                    DateTime EndDate = new DateTime(Convert.ToInt32(toYear), 3, 31);

                    //if (model.Year.Count > 1)
                    //{
                    //    Condition += " ((YEAR(ms.StartDate) >= " + StartDate.Year + " OR YEAR(ms.StartDate) <= " + EndDate.Year + ") AND (YEAR(ms.EndDate) >= " + StartDate.Year + " OR YEAR(ms.EndDate) <= " + EndDate.Year + ")) AND ";
                    //}
                    //else
                    //{
                    //    Condition += " ((YEAR(ms.StartDate) = " + StartDate.Year + " OR YEAR(ms.StartDate) = " + EndDate.Year + ") AND (YEAR(ms.EndDate) = " + StartDate.Year + " OR YEAR(ms.EndDate) = " + EndDate.Year + ")) AND ";
                    //}

                    Condition += " ((YEAR(ms.StartDate) BETWEEN " + StartDate.Year + " AND " + EndDate.Year + ") OR (YEAR(ms.EndDate)  BETWEEN " + StartDate.Year + " AND " + EndDate.Year + ")) AND ";
                }
                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";

                    List<string> columnsToSearch = new List<string>() { "wtm.StatusName", "wtm.PaymentStatusName",
                        "ms.MilestoneName", "ms.PercentageCompleted", "tm.TenderNumber", "tm.DistrictName", "tm.DivisionName", "go.GONumber", "go.GODepartment",
                        "go.GOName", "ms.PaymentStatus", "ms.MilestoneStatus", "wtv.Value", "swtv.Value", "wt.TemplateCode"
                    };
                    foreach (var column in columnsToSearch)
                    {
                        if (!string.IsNullOrEmpty(column))
                        {
                            searchCondition += column + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                        }
                    }

                    if (searchCondition.Substring(searchCondition.Length - 3) == "OR ")
                    {
                        searchCondition = searchCondition.Remove(searchCondition.Length - 3);
                    }
                    searchCondition += ") AND ";

                    Condition += searchCondition;
                }
                if (Condition.Substring(Condition.Length - 3) == "OR ")
                {
                    Condition = Condition.Remove(Condition.Length - 3);
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

                    #region Select Field
                    if (string.Equals(model?.Sorting.FieldName, "WorkType", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "wtv.Value";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "DivisionName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "tm.DivisionName";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "TenderNumber", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "tm.TenderNumber";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "MilestoneName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "ms.MilestoneName";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "PercentageCompleted", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "ms.PercentageCompleted";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "StartDate", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "ms.StartDate";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "EndDate", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "ms.EndDate";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "GONumber", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "go.GONumber";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "PercentageCompleted", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "ms.PercentageCompleted";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "PaymentPercentage", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "ms.PaymentPercentage";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "PaymentStatusName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "wtm.PaymentStatusName";
                    }
                    else if (string.Equals(model?.Sorting.FieldName, "statusName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FieldName = "wtm.StatusName";
                    }
                    else
                    {
                        FieldName = "wtm.CreatedDate";
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

            return SqlMapper.Query<MBookMasterModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<MBookMasterModel>();
        }
        public List<MBookReportModel> Work_MBook_Report_Get(MBookReportFilterModel model, out int TotalCount)
        {
            TotalCount = 0;
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            string Query = @"select
            wtm.WorkTypeId, tcc.Value as WorkType, wtm.SubWorkTypeId, swt.Value as SubWorkType, wtm.Strength,
            wtm.WorkTemplateName, wm.WorkNumber,

            tm.Division, tm.District, tm.DistrictName, tm.DivisionName, tm.TenderNumber,

            wmm.WorkId, wmm.WorkTemplateId, wmm.MilestoneName, wmm.OrderNumber, 
            wmm.DurationInDays, (datediff(wmm.StartDate, CURDATE())) as DurationInDaysLeft,
            wmm.IsPaymentRequired, wmm.PaymentPercentage, wmm.StartDate  as StartDate, 
            wmm.EndDate  as MilestoneEndDate, wmm.PercentageCompleted,
            wmm.MilestoneCode, wmm.CompletedDate as MilestoneCompletedDate,
            wmm.MilestoneAmount, wmm.IsCompleted as MilestoneIsCompleted, wmm.ActualAmount as MilestoneActualAmount,

            mbm.PaymentStatusId, psm.StatusName as PaymentStatusName,
            mbm.StatusId, msm.StatusName as StatusName, 

            mbm.Id, mbm.WorkNotes, mbm.SubmittedDate, mbm.CompletedDate, mbm.ActualAmount, mbm.Date,
            mbm.MBookNumber, mbm.ActionableRoleId,  ar.RoleName as ActionableRoleName, mbm.WorkNotes as Notes,
            
            wmm.MilestoneAmount as PannedValue,
            mbm.ActualAmount as ActualValue,
            (CASE 
                WHEN wmm.IsCompleted = 1 THEN mbm.ActualAmount
                ELSE null
            END) as PaymentValue,
            (CASE 
					WHEN (SELECT RoleId 
						  FROM approval_flow_master 
						  WHERE DepartmentId = tm.DepartmentId AND IsActive = 1 
						  ORDER BY OrderNumber DESC 
						  LIMIT 1) = mbm.ActionableRoleId 
					THEN 1
			END) AS IsWaitingForPayment,
 IFNULL((SELECT CONCAT(Id, '|', SavedFileName) FROM file_master WHERE TypeId = wmm.Id AND Type='MilestoneImage' ORDER BY CreatedDate DESC LIMIT 1),'') as 'MilestoneFile1Saved',
            IFNULL((SELECT OriginalFileName FROM file_master WHERE TypeId = wmm.Id AND Type='MilestoneImage' ORDER BY CreatedDate DESC LIMIT 1),'') as 'MilestoneFile1Original',

            IFNULL((SELECT CONCAT(Id, '|', SavedFileName) FROM file_master WHERE TypeId = wmm.Id AND Type='MilestoneImage' ORDER BY CreatedDate DESC LIMIT 1 OFFSET 1),'') as 'MilestoneFile2Saved',
            IFNULL((SELECT OriginalFileName FROM file_master WHERE TypeId = wmm.Id AND Type='MilestoneImage' ORDER BY CreatedDate DESC LIMIT 1 OFFSET 1),'') as 'MilestoneFile2Original'

            from work_mbook_master mbm 
            inner join work_template_milestone_master wmm on wmm.Id = mbm.WorkTemplateMilestoneId
            inner join work_master wm on wm.Id = wmm.WorkId
            inner join tender_master tm on tm.Id = wm.TenderId
            inner join work_template_master wtm on wtm.Id = wmm.WorkTemplateId
            left join two_column_configuration_values tcc on tcc.Id = wtm.WorkTypeId
            left join two_column_configuration_values swt on swt.Id = wtm.SubWorkTypeId
            left join status_master msm on msm.Id = mbm.StatusId
            left join status_master psm on psm.Id = mbm.PaymentStatusId
            left join account_role ar on ar.Id = mbm.ActionableRoleId";

            if (model != null)
            {
                //Template reassign
                //Filter only active milestones
                string Condition = " WHERE mbm.IsActive = 1 AND wmm.PercentageCompleted =100.00 AND ";

                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";
                    List<string> whereProperties = new List<string>() { "tcc.Value", "swt.Value", "wtm.Strength", "wmm.MilestoneName",
                        "wtm.WorkTemplateName", "tm.TenderNumber", "wm.WorkNumber", "wmm.MilestoneCode", "psm.StatusName", "msm.StatusName",
                        "wmm.StartDate", "wmm.EndDate", "wmm.MilestoneAmount", "wmm.ActualAmount", "ar.RoleName", "mbm.MBookNumber", "mbm.WorkNotes"
                    };
                    foreach (var property in whereProperties)
                    {
                        searchCondition += property + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                    }
                    searchCondition = searchCondition.Remove(searchCondition.Length - 3);
                    searchCondition += ") AND";

                    Condition += searchCondition;
                }

                if (model?.DistrictList?.Count > 0)
                {
                    string districtIds_str = "";
                    model.DistrictList.ForEach(x =>
                    {
                        districtIds_str += "'" + x + "',";
                    });
                    districtIds_str = districtIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(districtIds_str))
                    {
                        Condition += " tm.District IN (" + districtIds_str + ") AND ";
                    }
                }
                if (model?.DivisionList?.Count > 0)
                {
                    string divisionIds_str = "";
                    model.DivisionList.ForEach(x =>
                    {
                        divisionIds_str += "'" + x + "',";
                    });
                    divisionIds_str = divisionIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(divisionIds_str))
                    {
                        Condition += " tm.Division IN (" + divisionIds_str + ") AND ";
                    }
                }
                if (model?.DepartmentList?.Count > 0)
                {
                    string departmentIds_str = "";
                    model.DepartmentList.ForEach(x =>
                    {
                        departmentIds_str += "'" + x + "',";
                    });
                    departmentIds_str = departmentIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(departmentIds_str))
                    {
                        Condition += " tm.DepartmentId IN (" + departmentIds_str + ") AND ";
                    }
                }
                if (string.Equals(model?.RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (model?.MbookIds?.Count > 0)
                    {
                        string tenderIds_str = "";
                        model.MbookIds.ForEach(x =>
                        {
                            tenderIds_str += "'" + x + "',";
                        });
                        tenderIds_str = tenderIds_str.Trim().Trim(',');
                        if (!string.IsNullOrWhiteSpace(tenderIds_str))
                        {
                            Condition += " mbm.Id IN (" + tenderIds_str + ") AND ";
                        }
                    }
                }
                if (model?.StatusList?.Count > 0)
                {


                    string statusIds_str = "";
                    model.StatusList.ForEach(x =>
                    {
                        statusIds_str += "'" + x + "',";
                    });
                    statusIds_str = statusIds_str.Trim().Trim(',');
                    var list = statusIds_str.Split(",");
                    
                        if (list.Any(x => x == "'40625423-7b36-11ee-b363-fa163e14116e'"))
                        {
                            Condition += " mbm.PaymentStatusId = '" + "40625423-7b36-11ee-b363-fa163e14116e" + "' AND ";
                        }
                        if (list.Any(x => x == "'8e6b30f4-7b20-11ee-b363-fa163e14116e'"))
                        {
                            Condition += " mbm.PaymentStatusId = '" + "8e6b30f4-7b20-11ee-b363-fa163e14116e" + "' AND";
                        }
                    Condition += "(";
                        if(list.Any(x=>x == "'febc5537-50ec-11f0-9806-2c98117e76d0'"))
                    {
                        if (model.RoleCode=="ADM")
                        {
                            Condition += "mbm.StatusName IN ('Approved','Returned') OR";
                        }
                        else {
                            Condition += " mbm.ActionableRoleId='" + model.RoleId + "' OR";
                        }
                    }

                    if (list.Any(x => x == "'42b8efcc-50e6-11f0-9806-2c98117e76d0'"))
                    {
                        Condition += " (SELECT RoleId FROM approval_flow_master WHERE DepartmentId = tm.DepartmentId AND IsActive = 1 ORDER BY OrderNumber DESC LIMIT 1 ) = mbm.ActionableRoleId OR ";
                    }

                    if (!string.IsNullOrWhiteSpace(statusIds_str))
                    {
                        Condition += " mbm.StatusId IN (" + statusIds_str + ")) AND ";
                    }

                }
                if (model?.FromDate != null && model?.ToDate != null)
                {
                    //Condition += " wmm.StartDate >= '" + model.FromDate?.ToString("yyyy-MM-dd") + "' AND";
                    //Condition += " wmm.EndDate <= '" + model.ToDate?.ToString("yyyy-MM-dd") + "' AND";

                    Condition += " ((YEAR(wmm.StartDate) BETWEEN YEAR('" + model.FromDate?.ToString("yyyy-MM-dd") + "') AND YEAR('" + model.ToDate?.ToString("yyyy-MM-dd") + "')) " +
                 " OR (YEAR(wmm.EndDate) BETWEEN YEAR('" + model.FromDate?.ToString("yyyy-MM-dd") + "') AND YEAR('" + model.ToDate?.ToString("yyyy-MM-dd") + "'))) AND ";

                    //Condition += " ((YEAR(wmm.StartDate) BETWEEN YEAR('" + model?.FromDate + "') AND YEAR('" + model?.ToDate + "')) " +
                    //    "OR(YEAR(wmm.EndDate) BETWEEN YEAR('" + model?.FromDate + "') AND YEAR('" + model?.ToDate + "'))) AND ";
                }

                if (!string.IsNullOrWhiteSpace(model?.DivisionId))
                {
                    Condition += " tm.Division = '" + model.DivisionId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.DistrictId))
                {
                    Condition += " tm.District = '" + model.DistrictId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.WorkTypeId))
                {
                    Condition += " wtm.WorkTypeId = '" + model.WorkTypeId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.SubWorkTypeId))
                {
                    Condition += " wtm.SubWorkTypeId = '" + model.SubWorkTypeId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.Strength))
                {
                    Condition += " wtm.Strength = '" + model.Strength + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.WorkId))
                {
                    Condition += " wmm.WorkId = '" + model.WorkId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.StatusId))
                {
                    Condition += " mbm.StatusId = '" + model.StatusId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.StatusName))
                {
                    Condition += " msm.StatusName = '" + model.StatusName + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.PaymentStatusId))
                {
                    Condition += " wmm.PaymentStatus = '" + model.PaymentStatusId + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.PaymentStatusName))
                {
                    Condition += " psm.StatusName = '" + model.PaymentStatusName + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.Amount))
                {
                    Condition += " wmm.MilestoneAmount = '" + model.Amount + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.ActualAmount))
                {
                    Condition += " mbm.ActualAmount = '" + model.ActualAmount + "' AND";
                }
                if (!string.IsNullOrWhiteSpace(model?.ActionableRoleId))
                {
                    Condition += " mbm.ActionableRoleId = '" + model.ActionableRoleId + "' AND";
                }

                // ==============================================================================

                if (Condition.Substring(Condition.Length - 5) == " AND ")
                {
                    Condition = Condition.Remove(Condition.Length - 5);
                }
                if (Condition.Substring(Condition.Length - 4) == " AND")
                {
                    Condition = Condition.Remove(Condition.Length - 4);
                }
                if (Condition == " WHERE ")
                {
                    Condition = "";
                }

                TotalCount = (SqlMapper.Query<MilestoneReportModel>(connection, Query + Condition, commandType: CommandType.Text)?.ToList() ?? new List<MilestoneReportModel>()).Count();

                if (model?.Sorting != null && !string.IsNullOrWhiteSpace(model?.Sorting.FieldName) && !string.IsNullOrWhiteSpace(model?.Sorting.Sort))
                {
                    if (model.Sorting.FieldName == "notes")
                    {
                        model.Sorting.FieldName = "mbm.WorkNotes";
                    }
                    if (model?.Skip == 0 && model?.Take == 0)
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " ";
                    }
                    else
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                    }
                }
                else if (model?.Skip == 0 && model?.Take == 0)
                {
                    Condition += " ORDER BY wmm.CreatedDate ";
                }
                else
                {
                    Condition += " ORDER BY wmm.CreatedDate LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                }

                Query += Condition;
            }

            return SqlMapper.Query<MBookReportModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<MBookReportModel>();
        }
        public string Work_MBook_SaveUpdate(MBookMasterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
            }

            dynamic @params = new
            {
                pId = model.Id,
                pWorkTemplateMilestoneId = model.WorkTemplateMilestoneId,
                pPaymentStatusId = model.PaymentStatusId,
                pLength = model.Length,
                pWidth = model.Width,
                pHeigth = model.Heigth,
                pActionableRoleId = model.ActionableRoleId,
                pDivisionId = model.DivisionId,
                pStatusName = model.StatusName,
                pPaymentStatusName = model.PaymentStatusName,
                pWorkTemplateId = model.WorkTemplateId,
                pWorkId = model.WorkId,
                pWorkNotes = model.WorkNotes,
                pDate = model.Date,
                pStatusCode = model.StatusCode,
                pPaymentStatusCode = model.PaymentStatusCode,
                pActualAmount = model.ActualAmount,
                pStatusId = model.StatusId,
                pIsActive = model.IsActive,
                pIsReturned = model.IsReturned,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
                pSubmittedDate = model.SubmittedDate,
                pCompletedDate = model.CompletedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Work_MBook_Master_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }

        #endregion M-Book

        #region M-Book Approval_History
        public List<MBookApprovalHistoryModel> Work_MBook_Approval_History_Get(string Id = "", string MBookId = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pMBookId = MBookId,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<MBookApprovalHistoryModel>(connection, "Work_MBook_Approval_History_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<MBookApprovalHistoryModel>();
        }
        public string Work_MBook_Approval_History_SaveUpdate(MBookApprovalHistoryModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
            }
            dynamic @params = new
            {
                pId = model.Id,
                pMBookId = model.MBookId,
                pFromId = model.FromId,
                pToId = model.ToId,
                pStatusEnum = model.StatusEnum,
                pComments = model.Comments,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
                pDocumentname = model.DocumentName,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Work_MBook_Approval_History_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }
        #endregion M-Book Approval_History

        #region API Call

        public string TenderMaster_Update_Log_Save(TenderMasterUpdateLogModel model)
        {
            dynamic @params = new
            {
                pId = Guid.NewGuid().ToString(),
                pAddedRecordCount = model.AddedRecordCount,
                pResponceText = model.ResponceText,
                pSavedDate = DateTime.Now,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "TenderMaster_Update_Log_Save", @params, commandType: CommandType.StoredProcedure);
        }

        #endregion API Call

        #region Comment
        public string Comment_SaveUpdate(CommentMasterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
            }
            dynamic @params = new
            {
                pId = model.Id,
                pType = model.Type,
                pParentId = model.ParentId,
                pTypeId = model.TypeId,
                pCommentsFrom = model.CommentsFrom,
                pCommentsText = model.CommentsText,
                pSubjectText = model.SubjectText,
                pSavedBy = model.CreatedBy,
                pSavedByUserName = model.CreatedByUserName,
                pSavedDate = model.CreatedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Comment_Master_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }
        public List<CommentMasterModel> Comment_Get(string Type = "", string TypeId = "", string CommentsFrom = "", string ParentId = "")
        {
            dynamic @params = new
            {
                pType = Type?.Trim() ?? "",
                pParentId = ParentId?.Trim() ?? "",
                pTypeId = TypeId?.Trim() ?? "",
                pCommentsFrom = CommentsFrom?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<CommentMasterModel>(connection, "Comment_Master_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<CommentMasterModel>();
        }
        public List<CommentMasterModel> Comment_Get(CommentFilterModel model, out int TotalCount)
        {
            TotalCount = 0;
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            string Query = @"SELECT Id, Type, TypeId, ParentId, CommentsFrom, CommentsText, SubjectText, CommentDate, CreatedBy, 
                                CreatedByUserName, CreatedDate, Suffix, Prefix, RunningNumber, CommentNumber
                                FROM Comment_Master ";

            if (model != null)
            {
                #region Build Query Conditions

                string Condition = " WHERE ";

                if (model.Where != null)
                {
                    PropertyInfo[] whereProperties = typeof(CommentWhereClauseProperties).GetProperties();
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
                            Condition += " " + property.Name + "='" + value.Replace('\'', '%').Trim() + "' AND ";
                        }
                    }
                }
                if (model?.Ids?.Count > 0)
                {
                    string recordIds_str = "";
                    model.Ids.ForEach(x =>
                    {
                        recordIds_str += "'" + x + "',";
                    });
                    recordIds_str = recordIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(recordIds_str))
                    {
                        Condition += " TypeId IN (" + recordIds_str + ") AND ";
                    }
                }
                if (model?.Types?.Count > 0)
                {
                    string recordTypes_str = "";
                    model.Types.ForEach(x =>
                    {
                        recordTypes_str += "'" + x + "',";
                    });
                    recordTypes_str = recordTypes_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(recordTypes_str))
                    {
                        Condition += " Type IN (" + recordTypes_str + ") AND ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";
                    PropertyInfo[] whereProperties = typeof(CommentWhereClauseProperties).GetProperties();
                    foreach (var property in whereProperties)
                    {
                        if (property.PropertyType.Name.ToLower() != "boolean")
                        {
                            searchCondition += property.Name + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                        }
                    }
                    searchCondition += ") AND";

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
                    if (model?.Skip == 0 && model?.Take == 0)
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " ";
                    }

                    else
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                    }
                }
                else if (model?.Skip == 0 && model?.Take == 0)
{
    Condition += " ORDER BY CreatedDate ASC"; // ASC → oldest to newest
}
else
{
    Condition += " ORDER BY CreatedDate ASC LIMIT " + model?.Take + " OFFSET " + model?.Skip;
}


                Query += Condition;

                #endregion Build Query Conditions
            }

            return SqlMapper.Query<CommentMasterModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<CommentMasterModel>();
        }
        #endregion Comment

        #region Dashboard
        public List<TenderWorkBaseModel> DashboardGet_TenderChart(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            if (model.DivisionIds == null)
            {
                model.DivisionIds = new List<string>();
            }
            if (model.DepartmentIds == null)
            {
                model.DepartmentIds = new List<string>();
            }

            if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
            {
                List<string> TenderIds = Tender_Ids_Get_ByContractor(UserId);
                model.DivisionIds = Division_Ids_Get_ByContractor(UserId);
                model.DepartmentIds = Department_Ids_Get_ByContractor(UserId);
            }
            List<TenderWorkBaseModel> tender = TenderWorkBaseList_Get(model).Where(x => model.DivisionIds.Contains(x.Division) && model.DepartmentIds.Contains(x.DepartmentId)).ToList();
            if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
            {
                tender = tender.Where(x => x.ContractorId == UserId).ToList();
            }

            return tender;
        }
        public List<MbookBaseModel> DashboardGet_MbookChart(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            TenderChartModel responce = new TenderChartModel();

            if (model.DivisionIds == null)
            {
                model.DivisionIds = new List<string>();
            }
            if (model.DepartmentIds == null)
            {
                model.DepartmentIds = new List<string>();
            }
            List<string> TenderIds = new List<string>();
            if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
            {
                TenderIds = Tender_Ids_Get_ByContractor(UserId);
                model.DivisionIds = Division_Ids_Get_ByContractor(UserId);
                model.DepartmentIds = Department_Ids_Get_ByContractor(UserId);
            }

            List<MbookBaseModel> mbook = MbookBaseList_Get(model).ToList();
            mbook = mbook.Where(x => model.DivisionIds.Contains(x.DivisionId) && model.DepartmentIds.Contains(x.DepartmentId)).ToList();

            if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
            {
                mbook = mbook.Where(x => TenderIds.Contains(x.TenderId)).ToList();
            }
            else
            {
                mbook = mbook.Where(x => x.PercentageCompleted > 0).ToList();
            }

            return mbook;
        }

        // Modified by Indu on 23/03/2025 - to get the dashboard tender cards data
        public DashboardRecordCountCardModel DashboardGetCountData(DashboardFilterModel model, string RoleCode, string UserGroupName, string UserId, string RoleId)
        {
            DashboardRecordCountCardModel result = new DashboardRecordCountCardModel();

            #region Tender

            if (model.DivisionIds == null)
            {
                model.DivisionIds = new List<string>();
            }
            if (model.DepartmentIds == null)
            {
                model.DepartmentIds = new List<string>();
            }

            if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
            {
                List<string> TenderIds = Tender_Ids_Get_ByContractor(UserId);
                model.DivisionIds = Division_Ids_Get_ByContractor(UserId);
                model.DepartmentIds = Department_Ids_Get_ByContractor(UserId);
            }

            List<TenderWorkBaseModel> tender = TenderWorkBaseList_Get(model).ToList();
            tender = tender.Where(x => model.DivisionIds.Contains(x.Division) && model.DepartmentIds.Contains(x.DepartmentId)).ToList();

            List<MbookBaseModel> mbook = MbookBaseList_Get(model).ToList();
            mbook = mbook.Where(x => model.DivisionIds.Contains(x.DivisionId) && model.DepartmentIds.Contains(x.DepartmentId)).ToList();

            List<StatusModel> tenderstatus = Tenderget_Slowprogress(model).ToList();
            tenderstatus = tenderstatus.Where(x => model.DivisionIds.Contains(x.Division) && model.DepartmentIds.Contains(x.DepartmentId)).ToList();

            if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
            {
                tender = tender.Where(x => x.ContractorId == UserId).ToList();
                mbook = mbook.Where(x => tender.Select(y => y.TenderId).Contains(x.TenderId)).ToList();
                tenderstatus = tenderstatus.Where(x => x.ContractorId == UserId).ToList();
            }

            List<TenderWorkBaseModel> finishedTenderList = tender.Where(x => x.IsCompleted == true)?.ToList() ?? new List<TenderWorkBaseModel>();
            result.Project_Finished = finishedTenderList.Count();
            result.Project_Finished_Amount = Math.Round(finishedTenderList.Sum(x => x.TenderAmount), 2);
            result.Project_Finished_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Project_Finished_Amount) + " Crores";

            List<TenderWorkBaseModel> onGoingTenderList = tender.Where(x => x.WorkStatus == "198ccf78-8de2-11ee-aa46-fa163e14116e").ToList() ?? new List<TenderWorkBaseModel>();
            result.Project_OnGoing = onGoingTenderList.Count();
            result.Project_OnGoing_Amount = Math.Round(onGoingTenderList.Sum(x => x.TenderAmount), 2);
            result.Project_OnGoing_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Project_OnGoing_Amount) + " Crores";

            List<TenderWorkBaseModel> upComingTenderList = new List<TenderWorkBaseModel>();
            if (onGoingTenderList.Count > 0)
            {
                upComingTenderList = tender.Where(x => x.IsCompleted == false && !onGoingTenderList.Select(k => k.TenderId).Contains(x.TenderId))?.ToList() ?? new List<TenderWorkBaseModel>();
            }
            else
            {
                upComingTenderList = tender.Where(x => string.IsNullOrWhiteSpace(x.WorkTemplateId) || string.IsNullOrWhiteSpace(x.WorkId))?.ToList() ?? new List<TenderWorkBaseModel>();
            }
            result.Project_Upcoming = upComingTenderList.Count();
            result.Project_Upcoming_Amount = Math.Round(upComingTenderList.Sum(x => x.TenderAmount), 2);
            result.Project_Upcoming_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Project_Upcoming_Amount) + " Crores";


            //List<StatusModel> Status = Tenderget_Slowprogress(model) ?? new List<StatusModel>();
            List<StatusModel> Statuslist = tenderstatus.Where(x => x.Status == "ON HOLD").ToList();
            result.Project_OnHold = Statuslist.Count();
            result.Project_OnHold_Amount = Math.Round(Statuslist.Sum(x => x.TenderAmount), 2);
            result.Project_OnHold_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Project_OnHold_Amount) + " Crores";



            List<StatusModel> StatuslistSlow = tenderstatus.Where(x => x.Status == "SLOW PROGRESS").ToList();
            result.Project_Slowprogress = StatuslistSlow.Count();
            result.Project_Slowprogress_Amount = Math.Round(StatuslistSlow.Sum(x => x.TenderAmount), 2);
            result.Project_Slowprogress_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Project_Slowprogress_Amount) + " Crores";


            result.Total_Project = finishedTenderList.Count() + onGoingTenderList.Count() + upComingTenderList.Count();
            result.Total_Project_Amount = Math.Round(
                                            finishedTenderList.Sum(x => x.TenderAmount) +
                                            onGoingTenderList.Sum(x => x.TenderAmount) +
                                            upComingTenderList.Sum(x => x.TenderAmount),
                                            2
                                        );
            result.Total_Project_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Total_Project_Amount) + " Crores";


            //if (onGoingTenders != null)
            //{
            //    result.Project_Upcoming = tender.Where(x => !onGoingTenders.Contains(x.TenderId))?.Count() ?? 0;
            //}
            //else
            //{
            //    result.Project_Upcoming = tender.Where(x => string.IsNullOrWhiteSpace(x.WorkTemplateId) || string.IsNullOrWhiteSpace(x.WorkId))?.Count() ?? 0;
            //}

            #endregion Tender


            // Modified by Indu on 23/06/2025 - to get the dashboard mbook cards data
            #region MBook

            List<MbookBaseModel> mBookApprovedList = mbook.Where(x => x.StatusCode == StatusCodeConst.Completed && x.IsActive == true).ToList();
            List<MbookBaseModel> mBookReturnList = mbook.Where(x => x.StatusCode == StatusCodeConst.Return).ToList();
            List<MbookBaseModel> mBookUpcomingList = new List<MbookBaseModel>();
            List<MbookBaseModel> mBookInApprovalList = new List<MbookBaseModel>();
            List<MbookBaseModel> mbookNotUploaded = new List<MbookBaseModel>();
            List<MbookBaseModel> mbookUploaded = new List<MbookBaseModel>();
            List<MBookMasterModel> mbookNoActionTaken = new List<MBookMasterModel>();
            List<MbookBaseModel> mbookPaymentPending = new List<MbookBaseModel>();
            List<MbookBaseModel> TotalMbooks = mbook.Where(x => x.IsActive == true && (x.PercentageCompleted == 100)).ToList();

            // AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: UserId)?.FirstOrDefault() ?? new AccountUserModel();

            MBookFilterModel mbookNoAction = new MBookFilterModel();
            mbookNoAction.Where = new MBookWhereClauseProperties();
            mbookNoAction.Where.IsActive = true;
            mbookNoAction.IsForApproval = true;
            mbookNoAction.Year = model.Year;
            mbookNoAction.MilestonePercentageShouldBeGreaterThan = true;
            mbookNoAction.RoleId = RoleId;


            AccountUserModel currentUser = _settingDAL.User_Get(true, UserId: UserId)?.FirstOrDefault() ?? new AccountUserModel();


            int TotalCount;
            string roleId = RoleId ?? "";
            
            
            
            if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
            {

                if (mbookNoAction.DivisionIds?.Count > 0)
                    mbookNoAction.DivisionIds.Clear();

                
                mbookNoAction.DivisionIds = Division_Ids_Get_ByContractor(UserId);
                mbookNoAction.DepartmentIds = Department_Ids_Get_ByContractor(UserId);
                mbookNoAction.TenderIds = Tender_Ids_Get_ByContractor(UserId);
            }
            else
            {
                mbookNoAction.RoleCode = RoleCode;
                
                if (mbookNoAction.DivisionIds == null || mbookNoAction.DivisionIds.Count == 0)
                {
                    mbookNoAction.DivisionIds = currentUser.DivisionId.Split(',').ToList();

                }
            }

            
            List<MBookMasterModel> mBookList = Work_MBook_Get(mbookNoAction, out TotalCount);
            TotalCount = mBookList.Count;

            
            if ((mbookNoAction.Where?.IsActive ?? false) == true)
            {
                mBookList.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.ActionableRoleId) && (Constants.StaticRoles.Contains(RoleCode) || string.Equals(UserGroupName, UserGroupConst.Engineer, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        x.IsEditable = true;
                        x.IsActionable = false;
                    }

                    else if (roleId == x.ActionableRoleId )
                    {
                        if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (x.StatusCode != StatusCodeConst.Saved)
                            {
                                x.IsActionable = true;
                            }
                            else
                            {
                                x.IsActionable = false;
                            }

                            if (x.StatusCode == StatusCodeConst.Return || x.StatusCode == StatusCodeConst.Saved)
                            {
                                x.IsEditable = true;
                            }
                        }
                        else if (x.StatusCode == StatusCodeConst.Saved)
                        {
                            x.IsEditable = true;
                            x.IsActionable = false;
                        }
                        else if (x.StatusCode == StatusCodeConst.Return)
                        {
                            x.IsEditable = false;
                            x.IsActionable = true;
                        }
                        else if (x.StatusCode == StatusCodeConst.Submitted)
                        {
                            x.IsEditable = false;
                            x.IsActionable = true;
                        }
                        else if (x.StatusCode == StatusCodeConst.Approve)
                        {
                            x.IsEditable = false;
                            x.IsActionable = true;
                        }
                        else if (x.StatusCode == StatusCodeConst.Reject)
                        {
                            x.IsEditable = true;
                            x.IsActionable = false;
                        }
                        else
                        {
                            x.IsActionable = false;
                            x.IsEditable = false;
                        }
                    }


                    if (roleId == x.ActionableRoleId || string.Equals(UserGroupName, UserGroupConst.HQ, StringComparison.CurrentCultureIgnoreCase))
                    {
                        x.IsEditable = true;
                    }
                });
            }
            else
            {
                mBookList.ForEach(x =>
                {
                    x.IsEditable = false;
                    x.IsActionable = false;
                });
            }

            int actionableCount = mBookList.Count(x => x.IsActionable);



            if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
            {
                mBookUpcomingList = mbook.Where(x => x.StatusCode == StatusCodeConst.Saved && x.PercentageCompleted == 0 && x.IsActive == true).ToList();
                mBookInApprovalList = mbook.Where(x => x.StatusCode != StatusCodeConst.Completed && x.PercentageCompleted > 0 && x.IsActive == true).ToList();
                mbookNotUploaded = mbook.Where(x => x.StatusCode == StatusCodeConst.Saved && (x.PercentageCompleted == 100) && x.IsActive == true).ToList();
                mbookUploaded = mbook.Where(x => (x.StatusCode == StatusCodeConst.Submitted || x.StatusCode == StatusCodeConst.Return || x.StatusCode == StatusCodeConst.Approve)
                && (x.PercentageCompleted == 100) && x.IsActive == true).ToList();
                mbookNoActionTaken = mBookList.Where(x => x.IsActionable && x.IsActive == true).ToList();
                mbookPaymentPending = mbook.Where(x => x.IsWaitingForPayment == true && x.PaymentStatusCode == StatusCodeConst.PaymentInProgress).ToList();
            }
            else if (string.Equals(UserGroupName, UserGroupConst.Engineer, StringComparison.CurrentCultureIgnoreCase))
            {
                mBookUpcomingList = mbook.Where(x => x.StatusCode == StatusCodeConst.Submitted && x.PercentageCompleted > 0 && x.IsActive == true).ToList();
                mBookInApprovalList = mbook.Where(x => (x.StatusCode == StatusCodeConst.Approve || x.StatusCode == StatusCodeConst.Submitted || x.StatusCode == StatusCodeConst.Saved) &&
                x.PercentageCompleted > 0 && x.IsActive == true).ToList();

                mbookNotUploaded = mbook.Where(x => x.StatusCode == StatusCodeConst.Saved && (x.PercentageCompleted == 100) && x.IsActive == true).ToList();
                mbookUploaded = mbook.Where(x => (x.StatusCode == StatusCodeConst.Submitted)
                && (x.PercentageCompleted == 100) && x.IsActive == true).ToList();

                mbookNoActionTaken = mBookList.Where(x => x.IsActionable && x.IsActive == true).ToList();
                mbookPaymentPending = mbook.Where(x => x.IsWaitingForPayment == true && x.PaymentStatusCode == StatusCodeConst.PaymentInProgress).ToList();

            }
            else
            {
                mBookUpcomingList = mbook.Where(x => x.StatusCode == StatusCodeConst.Submitted && x.PercentageCompleted > 0 && x.IsActive == true).ToList();
                mBookInApprovalList = mbook.Where(x => (x.StatusCode == StatusCodeConst.Approve || x.StatusCode == StatusCodeConst.Submitted) &&
                x.IsActive == true && x.ActionableRoleId == RoleId).ToList();

                mbookNotUploaded = mbook.Where(x => x.StatusCode == StatusCodeConst.Saved && (x.PercentageCompleted == 100) && x.IsActive == true).ToList();
                mbookUploaded = mbook.Where(x => (x.StatusCode == StatusCodeConst.Submitted)
                && (x.PercentageCompleted == 100) && x.IsActive == true).ToList();
                mbookNoActionTaken = mBookList.Where(x => x.StatusCode != StatusCodeConst.Saved && x.StatusCode != StatusCodeConst.Completed && x.StatusCode != StatusCodeConst.Submitted && x.IsActive == true).ToList();
                mbookPaymentPending = mbook.Where(x => x.IsWaitingForPayment == true && x.PaymentStatusCode == StatusCodeConst.PaymentInProgress).ToList();
                
            }
            result.TotalMbooks = TotalMbooks.Count();
            result.Mbook_Total_Amount = Math.Round(TotalMbooks.Sum(x => x.MbookAmount), 2);
            result.Mbook_Total_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Mbook_Total_Amount) + " Crores";

            result.Mbook_Approved = mBookApprovedList.Count();
            result.Mbook_Approved_Amount = Math.Round(mBookApprovedList.Sum(x => x.ActualAmount), 2);
            result.Mbook_Approved_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Mbook_Approved_Amount) + " Crores";

            result.Mbook_Rejected = mBookReturnList.Count();
            result.Mbook_Rejected_Amount = Math.Round(mBookReturnList.Sum(x => x.MbookAmount), 2);
            result.Mbook_Rejected_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Mbook_Rejected_Amount) + " Crores";

            result.Mbook_Upcoming = mBookUpcomingList.Count();
            result.Mbook_Upcoming_Amount = Math.Round(mBookUpcomingList.Sum(x => x.MbookAmount), 2);
            result.Mbook_Upcoming_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Mbook_Upcoming_Amount) + " Crores";

            result.Mbook_InApproval = mBookInApprovalList.Count();
            result.Mbook_InApproval_Amount = Math.Round(mBookInApprovalList.Sum(x => x.MbookAmount), 2);
            result.Mbook_InApproval_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Mbook_InApproval_Amount) + " Crores";


            result.Mbook_NotUploaded = mbookNotUploaded.Count();
            result.Mbook_NotUploaded_Amount = Math.Round(mbookNotUploaded.Sum(x => x.MbookAmount), 2);
            result.Mbook_NotUploaded_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Mbook_NotUploaded_Amount) + " Crores";

            result.Mbook_Uploaded = mbookUploaded.Count();
            result.Mbook_Uploaded_Amount = Math.Round(mbookUploaded.Sum(x => x.MbookAmount), 2);
            result.Mbook_Uploaded_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Mbook_Uploaded_Amount) + " Crores";

            result.Mbook_No_Action_Taken = mbookNoActionTaken.Count();
            result.Mbook_No_Action_Taken_Amount = Math.Round(mbookNoActionTaken.Sum(x => x.MilestoneAmount), 2);
            result.Mbook_No_Action_Taken_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Mbook_No_Action_Taken_Amount) + " Crores";


            result.Mbook_PaymentPending = mbookPaymentPending.Count();
            result.Mbook_PaymentPending_Amount = Math.Round(mbookPaymentPending.Sum(x => x.MbookAmount), 2);
            result.Mbook_PaymentPending_Amount_Text = StringFunctions.ConvertRupeesToCrores((double)result.Mbook_PaymentPending_Amount) + " Crores";


            #endregion MBook

            return result;
        }

        public List<StatusModel> Tenderget_Slowprogress(DashboardFilterModel model)
        {


            if (model.Year?.Count > 0)
            {
                bool IsMultipleYear = false;

                if (model.Year.Count > 1)
                {
                    IsMultipleYear = true;
                }

                List<int> yearList = model.Year.Select(s => Convert.ToInt32(s)).OrderBy(x => x).ToList();

                int fromYear = yearList?.FirstOrDefault() ?? 2020;
                int toYear = yearList?.LastOrDefault() ?? 4000;

                DateTime StartDate = new DateTime(Convert.ToInt32(fromYear), 4, 1);
                DateTime EndDate = new DateTime(Convert.ToInt32(toYear), 3, 31);

                dynamic @params = new
                {
                    pStartDate = StartDate,
                    pEndDate = EndDate,
                    pIsMultipleYear = IsMultipleYear,
                };

                using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
                return SqlMapper.Query<StatusModel>(connection, "Getstatus", @params, commandType: CommandType.StoredProcedure) ?? new List<StatusModel>();
            }
            else
            {
                return new List<StatusModel>();
            }
        }



        public List<TenderWorkBaseModel> TenderWorkBaseList_Get(DashboardFilterModel model)
        {
            if (model.Year?.Count > 0)
            {
                bool IsMultipleYear = false;

                if (model.Year.Count > 1)
                {
                    IsMultipleYear = true;
                }

                List<int> yearList = model.Year.Select(s => Convert.ToInt32(s)).OrderBy(x => x).ToList();

                int fromYear = yearList?.FirstOrDefault() ?? 2020;
                int toYear = yearList?.LastOrDefault() ?? 4000;

                DateTime StartDate = new DateTime(Convert.ToInt32(fromYear), 4, 1);
                DateTime EndDate = new DateTime(Convert.ToInt32(toYear), 3, 31);

                dynamic @params = new
                {
                    pStartDate = StartDate,
                    pEndDate = EndDate,
                    pIsMultipleYear = IsMultipleYear,
                };

                using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
                return SqlMapper.Query<TenderWorkBaseModel>(connection, "Dashboard_Tender_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<TenderWorkBaseModel>();
            }
            else
            {
                return new List<TenderWorkBaseModel>();
            }
        }
        public List<MbookBaseModel> MbookBaseList_Get(DashboardFilterModel model)
        {
            if (model.Year?.Count > 0)
            {
                bool IsMultipleYear = false;

                if (model.Year.Count > 1)
                {
                    IsMultipleYear = true;
                }

                List<int> yearList = model.Year.Select(s => Convert.ToInt32(s)).OrderBy(x => x).ToList();

                int fromYear = yearList?.FirstOrDefault() ?? 2020;
                int toYear = yearList?.LastOrDefault() ?? 4000;

                DateTime StartDate = new DateTime(Convert.ToInt32(fromYear), 4, 1);
                DateTime EndDate = new DateTime(Convert.ToInt32(toYear), 3, 31);

                dynamic @params = new
                {
                    pStartDate = StartDate,
                    pEndDate = EndDate,
                    pIsMultipleYear = IsMultipleYear,
                };

                using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
                return SqlMapper.Query<MbookBaseModel>(connection, "Dashboard_MBook_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<MbookBaseModel>();
            }
            else
            {
                return new List<MbookBaseModel>();
            }
        }


        #endregion Dashboard

        #region Activity
        public List<WorkActivityModel> Work_Activity_Get(WorkActivityModel model)
        {
            dynamic @params = new
            {
                pId = model.Id?.Trim() ?? "",
                pType = model.Type.Trim() ?? "",
                pTypeId = model.TypeId?.Trim() ?? "",
                pParentType = model.ParentType?.Trim() ?? "",
                pParentId = model.ParentId?.Trim() ?? "",
                pActivitySubject = model.ActivitySubject?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<WorkActivityModel>(connection, "Work_Activity_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<WorkActivityModel>();
        }
        public List<WorkActivityModel> Work_Activity_Get_By_Ids(ActivityFilterModel model, out int TotalCount)
        {
            TotalCount = 0;
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            string Query = @"SELECT Id, Type, TypeId, ParentType, ParentId, ActivitySubject, ActivityMessage, CreatedBy, 
                                CreatedByUserName, CreatedDate FROM work_activity ";

            if (model != null)
            {
                #region Build Query Conditions

                string Condition = " WHERE ";

                if (model?.Ids?.Count > 0)
                {
                    string recordIds_str = "";
                    model.Ids.ForEach(x =>
                    {
                        recordIds_str += "'" + x + "',";
                    });
                    recordIds_str = recordIds_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(recordIds_str))
                    {
                        Condition += " TypeId IN (" + recordIds_str + ") AND ";
                    }
                }
                if (model?.Types?.Count > 0)
                {
                    string recordTypes_str = "";
                    model.Types.ForEach(x =>
                    {
                        recordTypes_str += "'" + x + "',";
                    });
                    recordTypes_str = recordTypes_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(recordTypes_str))
                    {
                        Condition += " Type IN (" + recordTypes_str + ") AND ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";
                    PropertyInfo[] whereProperties = typeof(CommentWhereClauseProperties).GetProperties();
                    foreach (var property in whereProperties)
                    {
                        if (property.PropertyType.Name.ToLower() != "boolean")
                        {
                            searchCondition += property.Name + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                        }
                    }
                    searchCondition += ") AND";

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
                    if (model?.Skip == 0 && model?.Take == 0)
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " ";
                    }
                    else
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                    }
                }
                else if (model?.Skip == 0 && model?.Take == 0)
                {
                    Condition += " ORDER BY CreatedDate ";
                }
                else
                {
                    Condition += " ORDER BY CreatedDate LIMIT " + model?.Take + " OFFSET " + model?.Skip;
                }

                Query += Condition;

                #endregion Build Query Conditions
            }

            return SqlMapper.Query<WorkActivityModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<WorkActivityModel>();
        }
        public string Work_Activity_SaveUpdate(WorkActivityModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
            }

            dynamic @params = new
            {
                pId = model.Id?.Trim() ?? "",
                pType = model.Type.Trim() ?? "",
                pTypeId = model.TypeId?.Trim() ?? "",
                pParentType = model.ParentType?.Trim() ?? "",
                pParentId = model.ParentId?.Trim() ?? "",
                pActivitySubject = model.ActivitySubject?.Trim() ?? "",
                pActivityMessage = model.ActivityMessage?.Trim() ?? "",
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "Work_Activity_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }
        #endregion Activity

        #region Alert
        public List<AlertTenderModel> Alert_GetWork()
        {
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<AlertTenderModel>(connection, "AlertTenderModel", commandType: CommandType.StoredProcedure)?.ToList() ?? new List<AlertTenderModel>();
        }
        public List<AlertTenderMilestoneModel> Alert_GetWorkMilestone(string TenderId = "", string WorkId = "")
        {
            dynamic @params = new
            {
                pTenderId = TenderId?.Trim() ?? "",
                pWorkId = WorkId.Trim() ?? ""
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<AlertTenderMilestoneModel>(connection, "Alert_GetWorkMilestone", @params, commandType: CommandType.StoredProcedure) ?? new List<AlertTenderMilestoneModel>();
        }
        public List<AlertTenderMBookModel> Alert_GetWorkMbook(string TenderId = "", string WorkId = "")
        {
            dynamic @params = new
            {
                pTenderId = TenderId?.Trim() ?? "",
                pWorkId = WorkId.Trim() ?? ""
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<AlertTenderMBookModel>(connection, "Alert_GetWorkMbook", @params, commandType: CommandType.StoredProcedure) ?? new List<AlertTenderMBookModel>();
        }
        public List<AlertMasterModel> Alert_Get(AlertFilterModel model, out int TotalCount)
        {
            TotalCount = 0;
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            string Query = @"SELECT ac.Id, ac.AlertId, ac.AlertCode, ac.Severity, ac.Message, ac.Type, ac.TypeId, ac.RoleId, ac.DivisionId, ac.DistrictId,
                            ac.ResolvedBy, ac.ResolvedByUserName, ac.ResolvedDate, ac.CreatedBy, ac.CreatedByUserName, ac.CreatedDate
                            FROM Alert_Master ac ";

            if (model != null)
            {
                #region Build Query Conditions

                string Condition = " WHERE ac.Isactive =1 AND ";

                if (model?.Types?.Count > 0)
                {
                    string type_str = "";
                    model.Types.ForEach(x =>
                    {
                        type_str += "'" + x + "',";
                    });
                    type_str = type_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(type_str))
                    {
                        Condition += " ac.Type IN (" + type_str + ") AND ";
                    }
                }
                if (model?.TypeIds?.Count > 0)
                {
                    string type_id_str = "";
                    model.TypeIds.ForEach(x =>
                    {
                        type_id_str += "'" + x + "',";
                    });
                    type_id_str = type_id_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(type_id_str))
                    {
                        Condition += " ac.TypeId IN (" + type_id_str + ") AND ";
                    }
                }
                if (model?.DivisionIds?.Count > 0)
                {
                    string divi_id_str = "";
                    model.DivisionIds.ForEach(x =>
                    {
                        divi_id_str += "'" + x + "',";
                    });
                    divi_id_str = divi_id_str.Trim().Trim(',');
                    if (!string.IsNullOrWhiteSpace(divi_id_str))
                    {
                        Condition += " ac.DivisionId IN (" + divi_id_str + ") AND ";
                    }
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
                    if (model?.Skip == 0 && model?.Take == 0)
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " ";
                    }
                    else
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                    }
                }
                else if (model?.Skip == 0 && model?.Take == 0)
                {
                    Condition += " ORDER BY CreatedDate ";
                }
                else
                {
                    Condition += " ORDER BY CreatedDate LIMIT " + model?.Take + " OFFSET " + model?.Skip;
                }

                Query += Condition;

                #endregion Build Query Conditions
            }

            return SqlMapper.Query<AlertMasterModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<AlertMasterModel>();
        }
        #endregion Alert

        public List<TenderDataModel> GetTenderData()
        {
            dynamic @params = new
            {

            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<TenderDataModel>(connection, "GetTenderData", @params, commandType: CommandType.StoredProcedure) ?? new List<TenderDataModel>();
        }

        public List<TenderDataModel> GetTenderData_ByContractor(string ContractorId)
        {
            dynamic @params = new
            {
                pContractorId = ContractorId
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<TenderDataModel>(connection, "GetTender_byContrator", @params, commandType: CommandType.StoredProcedure) ?? new List<TenderDataModel>();
        }
        public List<TenderDataModel> GetTenderData_ByDivision(string Division)
        {
            dynamic @params = new
            {
                pDivision = Division
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<TenderDataModel>(connection, "GetTender_byDivision", @params, commandType: CommandType.StoredProcedure) ?? new List<TenderDataModel>();
        }

        public List<DashboardDivisionCountModel> Dashboard_Dvision_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            if (model.Year?.Count > 0)
            {
                bool IsMultipleYear = false;

                if (model.Year.Count > 1)
                {
                    IsMultipleYear = true;
                }

                List<int> yearList = model.Year.Select(s => Convert.ToInt32(s)).OrderBy(x => x).ToList();

                int fromYear = yearList?.FirstOrDefault() ?? 2020;
                int toYear = yearList?.LastOrDefault() ?? 4000;

                DateTime StartDate = new DateTime(Convert.ToInt32(fromYear), 4, 1);
                DateTime EndDate = new DateTime(Convert.ToInt32(toYear), 3, 31);

                List<string> TenderIds = new List<string>();
                if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                {
                    TenderIds = Tender_Ids_Get_ByContractor(UserId);
                    model.DivisionIds = Division_Ids_Get_ByContractor(UserId);
                    model.DepartmentIds = Department_Ids_Get_ByContractor(UserId);
                }

                dynamic @params = new
                {
                    pStartDate = StartDate,
                    pEndDate = EndDate,
                    pIsMultipleYear = IsMultipleYear,
                    pDivisionId = string.Join(",", model.DivisionIds),
                    pDepartmentId = string.Join(",", model.DepartmentIds),
                    pContractorId = UserId,
                    pTenderIds = string.Join(",", TenderIds)
                };

                using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
                return SqlMapper.Query<DashboardDivisionCountModel>(connection, "GetDivision_Status_Count", @params, commandType: CommandType.StoredProcedure) ?? new List<DashboardDivisionCountModel>();
            }
            else
            {
                return new List<DashboardDivisionCountModel>();
            }
        }


        #region cameralive
        public (List<DashboardCameraModel> Data, int TotalCount)Dashboard_LiveStreaming(DashboardCameraFilterModel model)
        {
            var parameters = new
            {
                pDivisionId = string.Join(",", model.DivisionIds),
                pDistrictId = string.Join(",", model.DistrictIds),
                pWorkStatus = string.Join(",", model.WorkStatus),
                pMainCategory = string.Join(",", model.mainCategory),
                pSubcategory = string.Join(",", model.subcategory),
                pTenderNumber = string.Join(",", model.TenderNumber),
                pTake = model.Take,
                pSkip = model.Skip
            };

            using var connection =
                new MySqlConnection(_configuration.GetConnectionString(connectionId));

            using var multi = connection.QueryMultiple(
                "GetDashboard_streamingURLS",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var data = multi.Read<DashboardCameraModel>().ToList();
            var totalCount = multi.Read<int>().FirstOrDefault();

            return (data, totalCount);
        }

        public List<GetcameraStatusCountModel> Dashboard_CameraCount(GetcameraStatusCountModel model)
        {
            

               
                dynamic @params = new
                {
                    
                    pDivisionId = string.Join(",", model.DivisionIds),
                    pDistrictId = string.Join(",", model.DistrictIds),
                    
                   
                   
                   
                    
                };

                using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
                return SqlMapper.Query<GetcameraStatusCountModel>(connection, "GetDashboard_CameraCount", @params, commandType: CommandType.StoredProcedure) ?? new List<GetcameraStatusCountModel>();
            }
        //public List<DashboardCameraModel> Dashboard_CameraReport(DashboardCameraModel model)
        //{
            

               
        //        dynamic @params = new
        //        {
                    
        //            pDivisionId = string.Join(",", model.DivisionIds),
        //            pDistrictId = string.Join(",", model.DistrictIds),
        //            pWorkStatus = string.Join(",", model.WorkStatus),
        //            pMainCategory = string.Join(",", model.mainCategory),
        //            pSubcategory = string.Join(",", model.subcategory),
        //            pTenderNumber = string.Join(",", model.TenderNumber),
                   
                   
                   
                    
        //        };

        //        using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
        //        return SqlMapper.Query<DashboardCameraModel>(connection, "GetDashboard_CameraReport", @params, commandType: CommandType.StoredProcedure) ?? new List<DashboardCameraModel>();
        //    }

        public List<DashboardCameraModel> Dashboard_CameraReport(DashboardCameraFilterModel model, out int TotalCount)
        {
            TotalCount = 0;
            StatusMaster? notStartedstatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.NotStarted).FirstOrDefault();
            StatusMaster? SlowProgressstatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.SlowProgress).FirstOrDefault();
            StatusMaster? OnHoldstatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.StartedbutStilled).FirstOrDefault();
            StatusMaster? CompletedStatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.Completed).FirstOrDefault();
            StatusMaster? InprogressStatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.Inprogress).FirstOrDefault();
            StatusMaster? NewStatus = _settingDAL.Status_Get(StatusCode: StatusCodeConst.New).FirstOrDefault();

            string Query = @"select rp.TenderNumber,tm.Id as TenderId,RtspUrl,RtmpUrl,tm.DistrictName, tm.DivisionName,
    -- tm.Division as divisionIds, tm.District as districtIds,
    ifnull(tm.TenderStatus,'Not-Started') AS WorkStatus,wm.WorkStatus as WorkStatusId,
    tm.MainCategory,tm.Subcategory,
    rp.DivisionId,rp.DistrictId,tm.tender_final_awarded_value,tm.TipsTender_Id,tm.SchemeName,tm.Go_Package_No,tm.EndDate as AwardedDate,
	wm.WorkCommencementDate,wm.WorkCompletionDate,wm.DateDifference, tm.ContractorCompanyName

	from rtsp_details rp left join work_master wm on (select Id from tender_master where TenderNumber= rp.TenderNumber)=wm.TenderId
    left join tender_master tm on rp.TenderNumber = tm.TenderNumber";

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));


            if (model != null)
            {
                string Condition = " WHERE rp.IsActive = 1 AND ";
                if (!string.IsNullOrWhiteSpace(model.WorkStatus))
            {
                if (model.WorkStatus.Equals("Slow Progress", StringComparison.OrdinalIgnoreCase))
                {
                    Query += @"
        AND wm.DateDifference >= CAST(SUBSTRING_INDEX(
                (SELECT Value FROM two_column_configuration_values WHERE Code = 'SLOW'), '-', 1
        ) AS UNSIGNED)
        AND wm.DateDifference <= CAST(SUBSTRING_INDEX(
                (SELECT Value FROM two_column_configuration_values WHERE Code = 'SLOW'), '-', -1
        ) AS UNSIGNED)";
                }
                else if (model.WorkStatus.Equals("Started but Stilled", StringComparison.OrdinalIgnoreCase))
                {
                    Query += @"
        AND wm.DateDifference >= CAST(
                (SELECT Value FROM two_column_configuration_values WHERE Code = 'ONHOLD') AS UNSIGNED
        )";
                }
                else
                {
                        if (!string.IsNullOrWhiteSpace(model?.WorkStatus))
                            Condition += " tm.TenderStatus = '" + model.WorkStatus + "' AND ";
                    }
            }

                //if (!string.IsNullOrWhiteSpace(model?.WorkStatus))
                //    Condition += " tm.TenderStatus = '" + model.WorkStatus + "' AND ";

                if (!string.IsNullOrWhiteSpace(model?.mainCategory))
                    Condition += " tm.MainCategory = '" + model.mainCategory + "' AND ";

                if (!string.IsNullOrWhiteSpace(model?.subcategory))
                    Condition += " tm.Subcategory = '" + model.subcategory + "' AND ";

                if (!string.IsNullOrWhiteSpace(model?.TenderNumber))
                    Condition += " rp.TenderNumber = '" + model.TenderNumber + "' AND ";

                if (model.Where != null)
                {
                    PropertyInfo[] whereProperties = typeof(WorkWhereClauseProperties).GetProperties();
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
                            Condition += "wm." + property.Name + "='" + value.Replace('\'', '%').Trim() + "' AND ";
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";
                    List<string> whereProperties = new List<string>() { "tm.DivisionName", "tm.TenderNumber",
                 "tm.EndDate", "tm.StartDate","tm.DistrictName", "tm.Category","tm.SchemeName", "tm.ContractorName","tm.MainCategory",
                        "tm.Subcategory", "tm.ContractorCompanyName"};
                    foreach (var property in whereProperties)
                    {
                        searchCondition += property + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                    }
                    searchCondition = searchCondition.Remove(searchCondition.Length - 3);
                    searchCondition += ") AND";

                    Condition += searchCondition;
                }

                
                else
                {

                    if (model?.DistrictIds?.Count > 0)
                    {
                        string districtIds_str = "";
                        model.DistrictIds.ForEach(x =>
                        {
                            districtIds_str += "'" + x + "',";
                        });
                        districtIds_str = districtIds_str.Trim().Trim(',');
                        if (!string.IsNullOrWhiteSpace(districtIds_str))
                        {
                            Condition += " tm.District IN (" + districtIds_str + ") AND ";
                        }
                    }
                   

                   
                    if (model?.DivisionIds?.Count > 0)
                    {
                        string divisionIds_str = "";
                        model.DivisionIds.ForEach(x =>
                        {
                            divisionIds_str += "'" + x + "',";

                        });
                        divisionIds_str = divisionIds_str.Trim().Trim(',');
                        if (!string.IsNullOrWhiteSpace(divisionIds_str))
                        {
                            Condition += " tm.Division IN (" + divisionIds_str + ") AND ";
                        }
                    }
                }
                
                

             
                
                // ==============================================================================

                if (Condition.Substring(Condition.Length - 5) == " AND ")
                {
                    Condition = Condition.Remove(Condition.Length - 5);
                }
                if (Condition.Substring(Condition.Length - 4) == " AND")
                {
                    Condition = Condition.Remove(Condition.Length - 4);
                }
                if (Condition == " WHERE ")
                {
                    Condition = "";
                }

                TotalCount = (SqlMapper.Query<DashboardCameraModel>(connection, Query + Condition, commandType: CommandType.Text)?.ToList() ?? new List<DashboardCameraModel>()).Count();

                if (model?.Sorting != null && !string.IsNullOrWhiteSpace(model?.Sorting.FieldName) && !string.IsNullOrWhiteSpace(model?.Sorting.Sort))
                {
                    if (model?.Skip == 0 && model?.Take == 0)
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " ";
                    }
                    else
                    {
                        Condition += " ORDER BY " + model?.Sorting.FieldName + " " + model?.Sorting.Sort + " LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                    }
                }
                else if (model?.Skip == 0 && model?.Take == 0)
                {
                    Condition += " ORDER BY tm.EndDate ";
                }
                else
                {
                    Condition += " ORDER BY tm.EndDate LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                }

                Query += Condition;
            }


            return SqlMapper.Query<DashboardCameraModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<DashboardCameraModel>();
        }


        #endregion cameralive


        #region Filters


        public List<GetAllDivisionModel> GetAllDivision(DashboardCameraModel model)
        {



            dynamic @params = new
            {

                pDivisionId = string.Join(",", model.DivisionIds),
                pDistrictId = string.Join(",", model.DistrictIds),

            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<GetAllDivisionModel>(connection, "GetAllDivision", @params, commandType: CommandType.StoredProcedure) ?? new List<GetAllDivisionModel>();
        }




        public List<GetAllDistrictModel> GetAllDistrict(DashboardCameraModel model)
        {



            dynamic @params = new
            {

                pDivisionId = string.Join(",", model.DivisionIds),
                pDistrictId = string.Join(",", model.DistrictIds),

            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<GetAllDistrictModel>(connection, "GetAllDistrict", @params, commandType: CommandType.StoredProcedure) ?? new List<GetAllDistrictModel>();
        }



        public List<WorkTypeModel> GetAllMainCategory(DashboardCameraModel model)
        {

            dynamic @params = new
            {

                pDivisionId = string.Join(",", model.DivisionIds),
                pDistrictId = string.Join(",", model.DistrictIds),

            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<WorkTypeModel>(connection, "GetAllMainCategory", @params, commandType: CommandType.StoredProcedure) ?? new List<WorkTypeModel>();
        }



        public List<SubWorkTypeModel> GetAllSubCategory(DashboardCameraModel model)
        {



            dynamic @params = new
            {

                pDivisionId = string.Join(",", model.DivisionIds ?? new List<string>()),
                pDistrictId = string.Join(",", model.DistrictIds ?? new List<string>()),
                pMainCategory = model.mainCategory

            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<SubWorkTypeModel>(connection, "GetAllSubCategory", @params, commandType: CommandType.StoredProcedure) ?? new List<SubWorkTypeModel>();
        }



        //public List<SubWorkTypeModel> GetAllSubCategory(DashboardCameraModel model)
        //{
        //    dynamic @params = new
        //    {
        //        pDivisionId = string.Join(",", model.DivisionIds ?? new List<string>()),
        //        pDistrictId = string.Join(",", model.DistrictIds ?? new List<string>()),
        //        pMainCategory = model.mainCategory
        //    };

        //    using IDbConnection connection =
        //        new MySqlConnection(_configuration.GetConnectionString(connectionId));

        //    return SqlMapper.Query<SubWorkTypeModel>(
        //        connection,
        //        "GetAllSubCategory",
        //        @params,
        //        commandType: CommandType.StoredProcedure
        //    )?.ToList() ?? new List<SubWorkTypeModel>();
        //}



        public List<WorkStatusModel> GetAllWorkStatus(DashboardCameraModel model)
        {



            dynamic @params = new
            {

                pDivisionId = string.Join(",", model.DivisionIds ?? new List<string>()),
                pDistrictId = string.Join(",", model.DistrictIds ?? new List<string>()),
                pMainCategory = model.mainCategory,
                psubcategory = model.subcategory

            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<WorkStatusModel>(connection, "GetAllWorkStatus", @params, commandType: CommandType.StoredProcedure) ?? new List<WorkStatusModel>();
        }


        //public List<TenderNumberModel> GetAllTenderNumber(DashboardCameraModel model)
        //{



        //    dynamic @params = new
        //    {
        //        pDivisionId = string.Join(",", model.DivisionIds ?? new List<string>()),
        //        pDistrictId = string.Join(",", model.DistrictIds ?? new List<string>()),
        //        pMainCategory = model.mainCategory,
        //        psubcategory = model.subcategory,
        //        pTenderNumber= model.TenderNumber


        //    };

        //    using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
        //    return SqlMapper.Query<TenderNumberModel>(connection, "GetAllTenderNumber", @params, commandType: CommandType.StoredProcedure) ?? new List<TenderNumberModel>();
        //}



        public List<TenderNumberModel> GetAllTenderNumber(DashboardCameraModel model)
        {
            using IDbConnection connection =
                new MySqlConnection(_configuration.GetConnectionString(connectionId));

            string sql = @"
        SELECT DISTINCT
            tm.TenderNumber,
            tm.Id AS TenderId,
            wm.DateDifference
        FROM tender_master tm
        INNER JOIN rtsp_details rd
    ON tm.TenderNumber = rd.TenderNumber
        LEFT JOIN work_master wm ON tm.Id = wm.TenderId
        WHERE 
            tm.IsActive = 1 AND rd.IsActive = 1

            AND (IFNULL(@pDivisionId, '') = ''
                 OR FIND_IN_SET(tm.Division, @pDivisionId))

            AND (IFNULL(@pDistrictId, '') = ''
                 OR FIND_IN_SET(tm.District, @pDistrictId))

            AND (IFNULL(@pMainCategory, '') = ''
                 OR tm.MainCategory = @pMainCategory)

            AND (IFNULL(@pSubcategory, '') = ''
                 OR tm.SubCategory = @pSubcategory)";

            if (!string.IsNullOrWhiteSpace(model.WorkStatus))
            {
                if (model.WorkStatus.Equals("Slow Progress", StringComparison.OrdinalIgnoreCase))
                {
                    sql += @"
                AND wm.DateDifference >= CAST(SUBSTRING_INDEX(
                        (SELECT Value FROM two_column_configuration_values WHERE Code = 'SLOW'),
                        '-', 1
                ) AS UNSIGNED)

                AND wm.DateDifference <= CAST(SUBSTRING_INDEX(
                        (SELECT Value FROM two_column_configuration_values WHERE Code = 'SLOW'),
                        '-', -1
                ) AS UNSIGNED)";
                }
                else if (model.WorkStatus.Equals("Started but Stilled", StringComparison.OrdinalIgnoreCase))
                {
                    sql += @"
                AND wm.DateDifference >= CAST(
                        (SELECT Value
                         FROM two_column_configuration_values
                         WHERE Code = 'ONHOLD')
                    AS UNSIGNED)";
                }
                else
                {
                    // ✅ Correct variable name
                    sql += @"
                AND LOWER(REPLACE(REPLACE(tm.TenderStatus, '-', ''), ' ', '')) =
                    LOWER(REPLACE(REPLACE(@pWorkStatus, '-', ''), ' ', ''))";
                }
            }

            sql += " ORDER BY tm.TenderNumber;";

            var parameters = new
            {
                pDivisionId = string.Join(",", model.DivisionIds ?? new List<string>()),
                pDistrictId = string.Join(",", model.DistrictIds ?? new List<string>()),
                pMainCategory = model.mainCategory,
                pSubcategory = model.subcategory,
                pWorkStatus = model.WorkStatus
            };

            return connection.Query<TenderNumberModel>(sql, parameters).ToList();
        }




        #endregion





















        public List<DashboardDivisionCountModel> Dashboard_Dvision_district_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            if (model.Year?.Count > 0)
            {
                bool IsMultipleYear = false;

                if (model.Year.Count > 1)
                {
                    IsMultipleYear = true;
                }

                List<int> yearList = model.Year.Select(s => Convert.ToInt32(s)).OrderBy(x => x).ToList();

                int fromYear = yearList?.FirstOrDefault() ?? 2020;
                int toYear = yearList?.LastOrDefault() ?? 4000;

                DateTime StartDate = new DateTime(Convert.ToInt32(fromYear), 4, 1);
                DateTime EndDate = new DateTime(Convert.ToInt32(toYear), 3, 31);

                List<string> TenderIds = new List<string>();
                if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                {
                    TenderIds = Tender_Ids_Get_ByContractor(UserId);
                    model.DivisionIds = Division_Ids_Get_ByContractor(UserId);
                    model.DepartmentIds = Department_Ids_Get_ByContractor(UserId);
                }



                dynamic @params = new
                {
                    pStartDate = StartDate,
                    pEndDate = EndDate,
                    pIsMultipleYear = IsMultipleYear,
                    pDivisionId = string.Join(",", model.DivisionIds ?? new List<string>()),
                    pDepartmentId = string.Join(",", model.DepartmentIds ?? new List<string>()),
                    pDistrictId = string.Join(",", model.DistrictIds ?? new List<string>()),
                    pTenderIds = string.Join(",", TenderIds)
                };

                using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
                return SqlMapper.Query<DashboardDivisionCountModel>(connection, "GetDivision_District_Status_Count", @params, commandType: CommandType.StoredProcedure) ?? new List<DashboardDivisionCountModel>();
            }
            else
            {
                return new List<DashboardDivisionCountModel>();
            }
        }

        public List<DashboardDivisionCountModel> GetDivision_District_Mbook_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            if (model.Year?.Count > 0)
            {
                bool IsMultipleYear = false;

                if (model.Year.Count > 1)
                {
                    IsMultipleYear = true;
                }

                List<int> yearList = model.Year.Select(s => Convert.ToInt32(s)).OrderBy(x => x).ToList();

                int fromYear = yearList?.FirstOrDefault() ?? 2020;
                int toYear = yearList?.LastOrDefault() ?? 4000;

                DateTime StartDate = new DateTime(Convert.ToInt32(fromYear), 4, 1);
                DateTime EndDate = new DateTime(Convert.ToInt32(toYear), 3, 31);

                List<string> MbookIds = new List<string>();
                if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                {
                    MbookIds = Mbook_Id_Get_ByContractor(UserId);
                    model.DivisionIds = Division_Ids_Get_ByContractor(UserId);
                    model.DepartmentIds = Department_Ids_Get_ByContractor(UserId);
                }



                dynamic @params = new
                {
                    pStartDate = StartDate,
                    pEndDate = EndDate,
                    pIsMultipleYear = IsMultipleYear,
                    pDivisionId = string.Join(",", model.DivisionIds ?? new List<string>()),
                    pDepartmentId = string.Join(",", model.DepartmentIds ?? new List<string>()),
                    pDistrictId = string.Join(",", model.DistrictIds ?? new List<string>()),
                    pMbookIds = string.Join(",", MbookIds),
                    pRoleId= RoleId,
                    pRoleCode=RoleCode
                };

                using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
                return SqlMapper.Query<DashboardDivisionCountModel>(connection, "GetDivision_District_Mbook_Count", @params, commandType: CommandType.StoredProcedure) ?? new List<DashboardDivisionCountModel>();
            }
            else
            {
                return new List<DashboardDivisionCountModel>();
            }
        }



        public List<DashboardDivisionCountModel> GetDivision_Mbook_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            if (model.Year?.Count > 0)
            {
                bool IsMultipleYear = false;

                if (model.Year.Count > 1)
                {
                    IsMultipleYear = true;
                }

                List<int> yearList = model.Year.Select(s => Convert.ToInt32(s)).OrderBy(x => x).ToList();

                int fromYear = yearList?.FirstOrDefault() ?? 2020;
                int toYear = yearList?.LastOrDefault() ?? 4000;

                DateTime StartDate = new DateTime(Convert.ToInt32(fromYear), 4, 1);
                DateTime EndDate = new DateTime(Convert.ToInt32(toYear), 3, 31);

                List<string> MbookIds = new List<string>();
                if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                {
                    MbookIds = Mbook_Id_Get_ByContractor(UserId);
                    model.DivisionIds = Division_Ids_Get_ByContractor(UserId);
                    model.DepartmentIds = Department_Ids_Get_ByContractor(UserId);
                }



                dynamic @params = new
                {
                    pStartDate = StartDate,
                    pEndDate = EndDate,
                    pIsMultipleYear = IsMultipleYear,
                    pDivisionId = string.Join(",", model.DivisionIds ?? new List<string>()),
                    pDepartmentId = string.Join(",", model.DepartmentIds ?? new List<string>()),
                    //pDistrictId = string.Join(",", model.DistrictIds ?? new List<string>()),
                    pMbookIds = string.Join(",", MbookIds),
                    pRoleId = RoleId,
                    pRoleCode = RoleCode
                };

                using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
                return SqlMapper.Query<DashboardDivisionCountModel>(connection, "GetDivision_Mbook_Count", @params, commandType: CommandType.StoredProcedure) ?? new List<DashboardDivisionCountModel>();
            }
            else
            {
                return new List<DashboardDivisionCountModel>();
            }
        }




        public List<DashboardDivisionCountModel> Dashboard_Scheme_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            if (model.Year?.Count > 0)
            {
                bool IsMultipleYear = false;

                if (model.Year.Count > 1)
                {
                    IsMultipleYear = true;
                }

                List<int> yearList = model.Year.Select(s => Convert.ToInt32(s)).OrderBy(x => x).ToList();

                int fromYear = yearList?.FirstOrDefault() ?? 2020;
                int toYear = yearList?.LastOrDefault() ?? 4000;

                DateTime StartDate = new DateTime(Convert.ToInt32(fromYear), 4, 1);
                DateTime EndDate = new DateTime(Convert.ToInt32(toYear), 3, 31);

                List<string> TenderIds = new List<string>();
                if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                {
                    TenderIds = Tender_Ids_Get_ByContractor(UserId);
                    model.DivisionIds = Division_Ids_Get_ByContractor(UserId);
                    model.DepartmentIds = Department_Ids_Get_ByContractor(UserId);
                }

                dynamic @params = new
                {
                    pStartDate = StartDate,
                    pEndDate = EndDate,
                    pIsMultipleYear = IsMultipleYear,
                    pDivisionId = string.Join(",", model.DivisionIds),
                    pDepartmentId = string.Join(",", model.DepartmentIds),
                    pContractorId = UserId,
                    pTenderIds = string.Join(",", TenderIds)
                };

                using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
                return SqlMapper.Query<DashboardDivisionCountModel>(connection, "Get_Scheme_Status_Count", @params, commandType: CommandType.StoredProcedure) ?? new List<DashboardDivisionCountModel>();
            }
            else
            {
                return new List<DashboardDivisionCountModel>();
            }
        }

        public string IsTenderverified(string TenderId)
        {
            dynamic @params = new
            {
                pId = TenderId ?? "",


            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            string verify = SqlMapper.ExecuteScalar<string>(connection, "IsVerified", @params, commandType: CommandType.StoredProcedure);
            return verify;

        }

    }
}

