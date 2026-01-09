using Dapper;
using Microsoft.Extensions.Configuration;
using Model.CustomeAttributes;
using Model.DomainModel;
using Model.ViewModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Utils;
using Utils.Interface;
using MySqlHelper = MySql.Data.MySqlClient.MySqlHelper;

namespace DAL
{
    public class GeneralDAL
    {
        private readonly IMySqlHelper _mySqlHelper;
        private readonly IConfiguration _configuration;
        private readonly IMySqlDapperHelper _mySqlDapperHelper;

        private readonly string connectionId = "Default";
        public GeneralDAL(IMySqlDapperHelper mySqlDapperHelper, IMySqlHelper mySqlHelper, IConfiguration configuration)
        {
            _mySqlHelper = mySqlHelper;
            _configuration = configuration;
            _mySqlDapperHelper = mySqlDapperHelper;
        }

        public string FileMaster_SaveUpdate(FileMasterModel model)
        {
            dynamic @params = new
            {
                pId = model.Id,
                pType = model.Type,
                pOriginalFileName = model.OriginalFileName,
                pSavedFileName = model.SavedFileName,
                pFileType = model.FileType,
                pTypeName = model.TypeName,
                pTypeId = model.TypeId,
                pIsActive = model.IsActive,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = model.SavedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.ExecuteScalar<string>(connection, "FileMaster_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }

        public List<FileMasterModel> FileMaster_Get(bool IsActive, string Id = "", string Type = "", string TypeId = "", string SavedFileName = "")
        {
            dynamic @params = new
            {
                pId = Id?.Trim() ?? "",
                pType = Type?.Trim() ?? "",
                pTypeId = TypeId?.Trim() ?? "",
                pIsActive = IsActive,
                pSavedFileName = SavedFileName?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<FileMasterModel>(connection, "FileMaster_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<FileMasterModel>();
        }

        #region RecordHistory
        public bool SaveRecordDifference(ObjectDifference model)
        {
            List<RecordHistoryModel> list = new List<RecordHistoryModel>();
            if (model.NewObjectType == model.OldObjectType)
            {
                TableInfoAttribute? tableInfoAttribute = Attribute.GetCustomAttribute(model.NewObjectType, typeof(TableInfoAttribute)) as TableInfoAttribute;
                PropertyInfo? uniqueIdProperty = model.Properties.Where(p => p.Name == (tableInfoAttribute?.KeyFieldName ?? ""))?.FirstOrDefault();
                foreach (PropertyInfo property in model.Properties.Where(p => p.Name != (uniqueIdProperty?.Name ?? "")))
                {
                    string oldValue = property.GetValue(model.OldObject)?.ToString()?.Trim() ?? "";
                    string newValue = property.GetValue(model.NewObject)?.ToString()?.Trim() ?? "";

                    if (property.PropertyType.Name.ToLower() == "int32")
                    {
                        oldValue = Convert.ToInt32(oldValue).ToString();
                        newValue = Convert.ToInt32(newValue).ToString();
                    }
                    else if (property.PropertyType.Name.ToLower() == "decimal")
                    {
                        oldValue = Convert.ToDecimal(oldValue).ToString();
                        newValue = Convert.ToDecimal(newValue).ToString() + ".00";
                    }

                    RecordHistoryModel item = new RecordHistoryModel();

                    if (tableInfoAttribute is not null)
                    {
                        item.TableName = tableInfoAttribute.TableName;
                    }
                    if (uniqueIdProperty is not null)
                    {
                        item.TableUniqueId = uniqueIdProperty.GetValue(model.OldObject)?.ToString() ?? "";
                        if (string.IsNullOrWhiteSpace(item.TableUniqueId.Trim()))
                        {
                            item.TableUniqueId = uniqueIdProperty.GetValue(model.NewObject)?.ToString() ?? "";
                        }
                    }
                    item.ColumnName = property.Name;
                    item.SavedBy = model.SavedBy;
                    item.SavedByUserName = model.SavedByUserName;
                    item.SavedDate = model.SavedDate;

                    if (!string.IsNullOrWhiteSpace(item.TableUniqueId))
                    {
                        if (model.IsDeleted)
                        {
                            item.OldValue = oldValue;
                            item.Action = "D";
                            list.Add(item);
                        }
                        else if (string.IsNullOrWhiteSpace(oldValue) && !string.IsNullOrWhiteSpace(newValue))
                        {
                            item.NewValue = newValue;
                            item.Action = "A";
                            list.Add(item);
                        }
                        else if (!string.IsNullOrWhiteSpace(oldValue) && !string.IsNullOrWhiteSpace(newValue) && oldValue != newValue)
                        {
                            item.OldValue = oldValue;
                            item.NewValue = newValue;
                            item.Action = "U";
                            list.Add(item);
                        }
                    }
                }

                if (list.FirstOrDefault()?.Action == "A")
                {
                    RecordHistoryModel? mod = list.FirstOrDefault();

                    if (mod != null)
                    {
                        mod.OldValue = "";
                        mod.NewValue = "";
                        list.Clear();
                        list.Add(mod);
                    }
                }

                return RecordHistory_SaveUpdate(list);
            }
            return false;
        }
        public bool RecordHistory_SaveUpdate(List<RecordHistoryModel> list)
        {
            if (list.Count > 0)
            {
                foreach (RecordHistoryModel model in list)
                {
                    dynamic @params = new
                    {
                        pId = Guid.NewGuid().ToString(),
                        pAction = model.Action,
                        pTableName = model.TableName ?? "",
                        pTableUniqueId = model.TableUniqueId ?? "",
                        pColumnName = model.ColumnName ?? "",
                        pOldValue = model.OldValue ?? "",
                        pNewValue = model.NewValue ?? "",
                        pSavedBy = model.SavedBy,
                        pSavedByUserName = model.SavedByUserName,
                        pSavedDate = model.SavedDate,
                    };

                    using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
                    string id = SqlMapper.ExecuteScalar<string>(connection, "Record_History_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
                }

                return true;
            }

            return false;
        }
        public List<RecordHistoryModel> GetRecordHistory(TableFilterModel model, out int TotalCount)
        {
            TotalCount = 0;
            string Query = @"SELECT Id, Action, TableName, TableUniqueId, ColumnName, OldValue, NewValue, CreatedBy, CreatedByUserName, CreatedDate FROM record_history ";

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            if (model != null)
            {
                string Condition = " WHERE ";

                if (!string.IsNullOrWhiteSpace(model?.SearchString))
                {
                    string searchCondition = " (";
                    List<string> whereProperties = new List<string>() { "Id", "Action", "TableName", "TableUniqueId", "ColumnName", "OldValue", "NewValue", "CreatedByUserName" };
                    foreach (var property in whereProperties)
                    {
                        searchCondition += property + " LIKE " + "'%" + model.SearchString.Trim() + "%' OR ";
                    }
                    searchCondition += ") AND";

                    Condition += searchCondition;
                }

                if (model?.ColumnSearch?.Count > 0)
                {
                    foreach (ColumnSearchModel item in model.ColumnSearch)
                    {
                        if (!string.IsNullOrWhiteSpace(item.SearchString?.Trim()) && !string.IsNullOrWhiteSpace(item.FieldName?.Trim()))
                        {
                            Condition += " " + item.FieldName + " LIKE " + "'%" + item.SearchString.Replace('\'', '%').Trim() + "%' AND ";
                        }
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
                    Condition += " ORDER BY CreatedDate LIMIT  " + model?.Take + "  OFFSET " + model?.Skip;
                }

                Query += Condition;
            }


            return SqlMapper.Query<RecordHistoryModel>(connection, Query, commandType: CommandType.Text)?.ToList() ?? new List<RecordHistoryModel>();
        }
        #endregion RecordHistory

        #region Mail SMS Log

        public string MailSMSLog_Save(MailSMSLog model)
        {
            dynamic @params = new
            {
                pRecordType = model.RecordType ?? "",
                pSentAddress = model.SentAddress ?? "",
                pSubject = model.Subject ?? "",
                pBody = model.Body ?? "",
                pType = model.Type ?? "",
                pTypeId = model.TypeId ?? "",
                pReceivedBy = model.ReceivedBy ?? "",
                pSavedBy = model.CreatedBy,
                pSavedByUserName = model.CreatedByUserName,
                pSavedDate = model.CreatedDate,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            string id = SqlMapper.ExecuteScalar<string>(connection, "Mail_SMS_Log_Save", @params, commandType: CommandType.StoredProcedure);

            return id;
        }

        public List<MailSMSLog> MailSMSLog_Get(MailSMSLog model)
        {
            dynamic @params = new
            {
                pId = model.Id ?? "",
                pRecordType = model.RecordType ?? "",
                pSentAddress = !string.IsNullOrEmpty(model.SentAddress)
        ? $"%{model.SentAddress}%"
        : "",
                pType = model.Type ?? "",
                pTypeId = model.TypeId ?? "",
                pReceivedBy = model.ReceivedBy ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<MailSMSLog>(connection, "Mail_SMS_Log_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<MailSMSLog>();
        }

        #endregion Mail SMS Log


        public async Task<AlertForNotificationModel> GetAlertsforBGProcess()
        {
            AlertForNotificationModel model = new();
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            SqlMapper.GridReader results = await connection.QueryMultipleAsync("Alert_GetforNotifications", null, null, null, commandType: CommandType.StoredProcedure);

            model.mBookMasters = results.Read<MBookMasterModel>().ToList();
            model.workTemplateMilestones = results.Read<WorkTemplateMilestoneMasterModel>().ToList();
            model.workMasters = results.Read<WorkMasterModel>().ToList();
            model.primaryModels = results.Read<AlertConfigurationPrimaryModel>().ToList();
            model.secondaryModels = results.Read<AlertConfigurationSecondaryModel>().ToList();
            model.statuses = results.Read<StatusMaster>().ToList();
            return model;
        }

        public bool Alert_SaveUpdate(List<AlertMasterModel> model, string CreatedBy, string CreatedByUserName)
        {
            StringBuilder sCommand = new StringBuilder("UPDATE alert_master set IsActive = 0 where IsActive = 1;" +
                " INSERT INTO `alert_master`(`Id`, `AlertId`, `Severity`, `Message`, `Type`, `TypeId`, `CreatedBy`, `CreatedByUserName`, `CreatedDate`,`IsActive`, `DivisionId`) VALUES ");

            using (MySqlConnection mConnection = new MySqlConnection(_configuration.GetConnectionString(connectionId)))
            {
                List<string> Rows = new List<string>();
                for (int i = 0; i < model.Count; i++)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},'{10}' )",
                        model[i].Id, model[i].AlertId, model[i].Severity, model[i].Message, model[i].Type, model[i].TypeId, CreatedBy, CreatedByUserName, DateTime.Now.ToString("yyyy-MM-dd"), 1, model[i].DivisionId));
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                mConnection.Open();
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), mConnection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                    return true;
                }
            }
            //using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            //return SqlMapper.ExecuteScalar<string>(connection, "FileMaster_SaveUpdate", @params, commandType: CommandType.StoredProcedure);
        }

        public bool Alert_Resolve(string id, string CreatedBy, string CreatedByUserName, string note)
        {
            dynamic @params = new
            {
                pid = id,
                pSavedBy = CreatedBy,
                pSavedByUserName = CreatedByUserName,
                pResolverNotes = note,
                pSavedDate = DateTime.Now,
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            int res = SqlMapper.ExecuteScalar<int>(connection, "Alert_Resolve", @params, commandType: CommandType.StoredProcedure);

            return res > 0 ? true : false;
        }

    }
}
