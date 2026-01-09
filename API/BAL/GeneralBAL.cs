using AutoMapper;
using BAL.BackgroundWorkerService;
using BAL.Interface;
using DAL;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Model.Constants;
using Model.CustomeAttributes;
using Model.DomainModel;
using Model.ViewModel;
using MySql.Data.MySqlClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Utils;
using Utils.Interface;
using Utils.UtilModels;

namespace BAL
{
    public class GeneralBAL : IGeneralBAL
    {
        private readonly GeneralDAL _generalDAL;
        private readonly SettingsDAL _settingsDAL;

        private readonly IMapper _mapper;
        private readonly IFTPHelpers _ftpHelper;

        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public GeneralBAL(IMySqlDapperHelper mySqlDapperHelper, IMySqlHelper mySqlHelper, IMapper mapper,
            IConfiguration configuration, IFTPHelpers ftpHelper, IBackgroundTaskQueue backgroundTaskQueue, IServiceScopeFactory serviceScopeFactory)
        {
            _generalDAL = new GeneralDAL(mySqlDapperHelper, mySqlHelper, configuration);
            _mapper = mapper;
            _ftpHelper = ftpHelper;
            _settingsDAL = new SettingsDAL(mySqlDapperHelper, mySqlHelper, configuration);

            _backgroundTaskQueue = backgroundTaskQueue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public string FileMaster_SaveUpdate(FileMasterModel model)
        {
            #region Save Difference
            FileMasterModel? exist = FileMaster_Get(true, model.Id)?.FirstOrDefault() ?? new FileMasterModel();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = FileMaster_Get(false, model.Id)?.FirstOrDefault() ?? new FileMasterModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<FileMasterModel, LogFieldAttribute>();
            if (model.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _generalDAL.FileMaster_SaveUpdate(model);
        }
        public List<FileMasterModel> FileMaster_Get(bool IsActive, string Id = "", string Type = "", string TypeId = "", string SavedFileName = "")
        {
            return _generalDAL.FileMaster_Get(IsActive, Id, Type, TypeId, SavedFileName);
        }

        public Task<byte[]> GetImage(string ImageName)
        {
            return _ftpHelper.DownloadFile_bytes(new Utils.FTPModel() { FileName = ImageName });
        }

        public List<AccountUserModel> GetKeyContacts(string UserId, string RoleCode, string DivisionIds, string DepartmentId)
        {
            List<ConfigurationModel> department = _settingsDAL.Configuration_Get(true, CategoryCode: ConfigurationCategory.Department);

            List<AccountUserModel> userList = new List<AccountUserModel>();
            if (string.Equals(RoleCode, RoleCodeConstants.Contractor, StringComparison.CurrentCultureIgnoreCase))
            {
                List<string> divisionIdList = _settingsDAL.User_GetContractorDivisions(UserId);
                List<AccountUserModel> list = new List<AccountUserModel>();
                DepartmentId?.Split(",")?.ToList()?.ForEach(x =>
                {
                    list.AddRange(_settingsDAL.User_Get(IsActive: true, DepartmentId: x));
                });
                if (list.Count > 0)
                {
                    list.ForEach(x =>
                    {
                        x.DivisionIdList = x.DivisionId?.Split(",")?.Select(y => y.Trim())?.ToList();
                        x.DepartmentIdList = x.DepartmentId?.Split(",")?.Select(y => y.Trim())?.ToList();
                    });
                    foreach (string divisionId in divisionIdList)
                    {
                        userList.AddRange(list.Where(x => x.IsContractor == false && x.UserId != UserId && (x.DivisionIdList?.Contains(divisionId) ?? false) == true)?.ToList() ?? new List<AccountUserModel>());
                    }
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(DivisionIds))
                {
                    List<string> divisionIdList = DivisionIds.Split(',').ToList();
                    List<AccountUserModel> list = new List<AccountUserModel>();
                    DepartmentId?.Split(",")?.ToList()?.ForEach(x =>
                    {
                        list.AddRange(_settingsDAL.User_Get(IsActive: true, DepartmentId: x));
                    });
                    if (list.Count > 0)
                    {
                        list.ForEach(x =>
                        {
                            x.DivisionIdList = x.DivisionId?.Split(",")?.Select(y => y.Trim())?.ToList();
                            x.DepartmentIdList = x.DepartmentId?.Split(",")?.Select(y => y.Trim())?.ToList();
                        });
                        foreach (string divisionId in divisionIdList)
                        {
                            userList.AddRange(list.Where(x => x.IsContractor == false && x.UserId != UserId && (x.DivisionIdList?.Contains(divisionId) ?? false) == true)?.ToList() ?? new List<AccountUserModel>());
                        }
                    }
                }
            }

            userList = userList?.GroupBy(g => g.UserId)?.Select(x => x.First())?.ToList() ?? new List<AccountUserModel>();

            userList.ForEach(x =>
            {
                if (x.DepartmentIdList is null)
                {
                    x.DepartmentIdList = new List<string>();
                }
                x.DepartmentNameList = department.Where(y => x.DepartmentIdList.Contains(y.Id)).Select(z => z.Value).ToList();
            });

            return userList;
        }

        public void SendMessage(List<EmailModel> email, List<SMSModel> sms, CurrentUserModel user)
        {
            try
            {
                _backgroundTaskQueue.QueueBackgroundWorkItem(workItem: token =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var scopedService = scope.ServiceProvider;
                        ISMSHelper _smsHelper = scopedService.GetRequiredService<ISMSHelper>();
                        IMailHelper _mailHelper = scopedService.GetRequiredService<IMailHelper>();

                        try
                        {
                            DoBackgroundWork(_smsHelper, _mailHelper, email, sms, user);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, ex.Message);
                        }
                    }

                    return Task.CompletedTask;
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        private void DoBackgroundWork(ISMSHelper smsHelper, IMailHelper mailHelper, List<EmailModel> email, List<SMSModel> sms, CurrentUserModel user)
        {
            if (email.Count > 0)
            {
                email.ForEach(x =>
                {

                    string _subject = string.Empty;
                    string _body = string.Empty;

                    bool res = mailHelper.SendMail(x, out _body, out _subject);
                    if (res)
                    {
                        MailSMSLog _log = new MailSMSLog();

                        _log.RecordType = "MAIL";
                        _log.SentAddress = string.Join(',', x.To).ToString();
                        _log.Subject = _subject;
                        _log.Body = _body;
                        _log.Type = x.Type;
                        _log.TypeId = x.TypeId;
                        _log.ReceivedBy = x.ReceivedBy;
                        _log.CreatedBy = x.SavedBy;
                        _log.CreatedByUserName = x.SavedByUserName;
                        _log.CreatedDate = x.SavedDate;

                        _generalDAL.MailSMSLog_Save(_log);
                    }
                });
            }
            if (sms.Count > 0)
            {
                sms.ForEach(x =>
                {
                    string _message = string.Empty;
                    bool res = smsHelper.SentSMS(x.MobileNumbers, x.TemplateCode, x.MessageReplaces, out _message);
                    if (res)
                    {
                        MailSMSLog _log = new MailSMSLog();

                        _log.RecordType = "SMS";
                        _log.SentAddress = string.Join(',', x.MobileNumbers).ToString();
                        _log.Subject = x.TemplateCode;
                        _log.Body = _message;
                        _log.Type = x.Type;
                        _log.TypeId = x.TypeId;
                        _log.ReceivedBy = x.ReceivedBy;
                        _log.CreatedBy = x.SavedBy;
                        _log.CreatedByUserName = x.SavedByUserName;
                        _log.CreatedDate = x.SavedDate;

                        _generalDAL.MailSMSLog_Save(_log);
                    }
                });
            }
        }

        public List<RecordHistoryModel> GetRecordHistory(TableFilterModel model, out int TotalCount)
        {
            return _generalDAL.GetRecordHistory(model, out TotalCount);
        }

        #region Mail SMS Log

        public string MailSMSLog_Save(MailSMSLog model)
        {
            return _generalDAL.MailSMSLog_Save(model);
        }

        public List<MailSMSLog> MailSMSLog_Get(MailSMSLog model)
        {
            return _generalDAL.MailSMSLog_Get(model);
        }


        #endregion Mail SMS Log


        public async Task<List<AlertMasterModel>> GetAlertsforBGProcess(string CreatedBy, string CreatedByUserName)
        {

            var model = await _generalDAL.GetAlertsforBGProcess();
            List<AlertMasterModel> list = new List<AlertMasterModel>();

            if (model.primaryModels != null && model.secondaryModels != null)
            {
                foreach (var primary in model.primaryModels.Where(x => x.Object == AlertConfigConstants.Object_Milestone))
                {
                    var secondayList = model.secondaryModels.Where(x => x.PrimaryId == primary.Id);
                    if (secondayList != null && model.workTemplateMilestones != null)
                    {
                        Console.WriteLine($"Alert Name : {primary.Name}");

                        foreach (var seconday in secondayList.OrderBy(x => x.Severity))
                        {
                            if ((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate || seconday.Field == AlertConfigConstants.Field_MilestoneEndDate) && seconday.BaseField == AlertConfigConstants.BaseField_CurrentDate)
                            {
                                var statustoRemove = model.statuses?.Where(x => x.StatusCode != "COMPLETED" || x.StatusCode != "REJECT").Select(y => y.Id).ToList();
                                if (statustoRemove != null)
                                {
                                    var activeMilestones = model.workTemplateMilestones.Where(x => statustoRemove.Contains(x.MilestoneStatus));
                                    list = Get_MilestoneStart_End_Current(activeMilestones, seconday, list, seconday.Severity, primary);
                                }

                            }
                            if (seconday.Field == AlertConfigConstants.Field_MilestoneProgress && seconday.BaseField == AlertConfigConstants.BaseField_WorkCost)
                            {
                                var activeMilestones = model.workTemplateMilestones.Where(x => x.IsActive);
                                list = Get_MilestoneCost_actuals(activeMilestones, seconday, list, seconday.Severity, primary);
                            }
                        }

                    }
                }

                foreach (var primary in model.primaryModels.Where(x => x.Object == AlertConfigConstants.Object_Work))
                {
                    var secondayList = model.secondaryModels.Where(x => x.PrimaryId == primary.Id);
                    if (secondayList != null && model.workMasters != null)
                    {
                        Console.WriteLine($"Alert Name : {primary.Name}");

                        foreach (var seconday in secondayList.OrderBy(x => x.Severity))
                        {
                            if ((seconday.Field == AlertConfigConstants.Field_WorkStartDate || seconday.Field == AlertConfigConstants.Field_WorkEndDate) && seconday.BaseField == AlertConfigConstants.BaseField_CurrentDate)
                            {
                                var statustoRemove = model.statuses?.Where(x => x.StatusCode != "COMPLETED" || x.StatusCode != "REJECT").Select(y => y.Id).ToList();
                                if (statustoRemove != null)
                                {
                                    var activeWorks = model.workMasters.Where(x => statustoRemove.Contains(x.WorkStatus) || string.IsNullOrEmpty(x.WorkStatus));
                                    list = Get_WorkStart_End_Current(activeWorks, seconday, list, seconday.Severity, primary);
                                }
                            }
                            if (seconday.Field == AlertConfigConstants.Field_WorkTotalCost && seconday.BaseField == AlertConfigConstants.BaseField_WorkCost)
                            {
                                var activeWorks = model.workMasters.Where(x => 1 == 1);
                                list = Get_WorkCost_actuals(activeWorks, seconday, list, seconday.Severity, primary);
                            }
                        }

                    }
                }

                foreach (var primary in model.primaryModels.Where(x => x.Object == AlertConfigConstants.Object_Mbook))
                {
                    var secondayList = model.secondaryModels.Where(x => x.PrimaryId == primary.Id);
                    if (secondayList != null && model.mBookMasters != null)
                    {
                        Console.WriteLine($"Alert Name : {primary.Name}");

                        foreach (var seconday in secondayList.OrderBy(x => x.Severity))
                        {
                            var activemBooks = model.mBookMasters.Where(x => x.IsActive);
                            if (seconday.Field == AlertConfigConstants.Field_MilestoneEndDate && seconday.BaseField == AlertConfigConstants.BaseField_CurrentDate)
                            {
                                list = Get_MbookStart_End_Current(activemBooks, seconday, list, seconday.Severity, primary);
                            }
                            if (seconday.Field == AlertConfigConstants.Field_ActiveMbookSubmittedDate && seconday.BaseField == AlertConfigConstants.BaseField_CurrentDate)
                            {
                                list = Get_Mbook_Approvals(activemBooks, seconday, list, seconday.Severity, primary);
                            }
                        }

                    }
                }

            }
            if (list != null && list.Count > 0)
            {
                _generalDAL.Alert_SaveUpdate(list, CreatedBy, CreatedByUserName);
            }
            return list;
        }

        private static List<AlertMasterModel> Get_MilestoneStart_End_Current(IEnumerable<WorkTemplateMilestoneMasterModel> model, AlertConfigurationSecondaryModel seconday, List<AlertMasterModel> list, string severityType, AlertConfigurationPrimaryModel primary)
        {
            if (seconday.Severity == severityType)
            {
                var milestonedelayTemplate = @"{0} -- Tender: {1}, Name: {2}, Code: {3}, Starts: {4}, Ends: {5}, Progress: {6}%";
                if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThan)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.PercentageCompleted <= 0 : (x.PercentageCompleted < 100 && x.PercentageCompleted > 0)) && ((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays > Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays}, PercentageCompleted {Math.Round(item.PercentageCompleted)}%  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x =>
                        new AlertMasterModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            DivisionId = x.Division,
                            AlertId = seconday.PrimaryId,
                            TypeId = x.Id,
                            Type = "MILESTONE",
                            Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MilestoneName, x.MilestoneCode, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd"), Math.Round(x.PercentageCompleted)),
                            Severity = severityType
                        }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThan)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.PercentageCompleted <= 0 : (x.PercentageCompleted < 100 && x.PercentageCompleted > 0)) && ((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays < Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays}, PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.Division,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "MILESTONE",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MilestoneName, x.MilestoneCode, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd"), Math.Round(x.PercentageCompleted)),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThanEqual)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.PercentageCompleted <= 0 : (x.PercentageCompleted < 100 && x.PercentageCompleted > 0)) && ((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays <= Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays}, PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.Division,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "MILESTONE",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MilestoneName, x.MilestoneCode, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd"), Math.Round(x.PercentageCompleted)),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThanEqual)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.PercentageCompleted <= 0 : (x.PercentageCompleted < 100 && x.PercentageCompleted > 0)) && ((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays >= Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays}, PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.Division,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "MILESTONE",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MilestoneName, x.MilestoneCode, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd"), Math.Round(x.PercentageCompleted)),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
            }

            return list;
        }

        private static List<AlertMasterModel> Get_MilestoneCost_actuals(IEnumerable<WorkTemplateMilestoneMasterModel> model, AlertConfigurationSecondaryModel seconday, List<AlertMasterModel> list, string severityType, AlertConfigurationPrimaryModel primary)
        {
            if (seconday.Severity == severityType)
            {
                var milestonedelayTemplate = @"{0} -- Tender: {1}, Name: {2}, Code: {3}, Planned Amount: {4}, Actual Amount: {5}, Progress: {6}%";
                if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThan)
                {
                    var milestoneList = model.Where(x => x.PercentageCompleted > 0 && x.IsPaymentRequired && x.MilestoneAmount > 0 && ((x.ActualAmount / x.MilestoneAmount) * 100) > seconday.CalculationNo);
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" MilestoneAmount {item.MilestoneAmount} ActualAmount {item.ActualAmount} , PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel { Id = Guid.NewGuid().ToString(), DivisionId = x.Division, AlertId = seconday.PrimaryId, TypeId = x.Id, Type = "MILESTONE", Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MilestoneName, x.MilestoneCode, Math.Round(x.MilestoneAmount), Math.Round(x.ActualAmount), Math.Round(x.PercentageCompleted)), Severity = severityType }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThan)
                {
                    var milestoneList = model.Where(x => x.PercentageCompleted > 0 && x.IsPaymentRequired && x.MilestoneAmount > 0 && ((x.ActualAmount / x.MilestoneAmount) * 100) < seconday.CalculationNo);
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" MilestoneAmount {item.MilestoneAmount} ActualAmount {item.ActualAmount} , PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel { Id = Guid.NewGuid().ToString(), DivisionId = x.Division, AlertId = seconday.PrimaryId, TypeId = x.Id, Type = "MILESTONE", Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MilestoneName, x.MilestoneCode, Math.Round(x.MilestoneAmount), Math.Round(x.ActualAmount), Math.Round(x.PercentageCompleted)), Severity = severityType }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThanEqual)
                {
                    var milestoneList = model.Where(x => x.PercentageCompleted > 0 && x.IsPaymentRequired && x.MilestoneAmount > 0 && ((x.ActualAmount / x.MilestoneAmount) * 100) <= seconday.CalculationNo);
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" MilestoneAmount {item.MilestoneAmount} ActualAmount {item.ActualAmount} , PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel { Id = Guid.NewGuid().ToString(), DivisionId = x.Division, AlertId = seconday.PrimaryId, TypeId = x.Id, Type = "MILESTONE", Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MilestoneName, x.MilestoneCode, Math.Round(x.MilestoneAmount), Math.Round(x.ActualAmount), Math.Round(x.PercentageCompleted)), Severity = severityType }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThanEqual)
                {
                    var milestoneList = model.Where(x => x.PercentageCompleted > 0 && x.IsPaymentRequired && x.MilestoneAmount > 0 && ((x.ActualAmount / x.MilestoneAmount) * 100) >= seconday.CalculationNo);
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" MilestoneAmount {item.MilestoneAmount} ActualAmount {item.ActualAmount} , PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel { Id = Guid.NewGuid().ToString(), DivisionId = x.Division, AlertId = seconday.PrimaryId, TypeId = x.Id, Type = "MILESTONE", Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MilestoneName, x.MilestoneCode, Math.Round(x.MilestoneAmount), Math.Round(x.ActualAmount), Math.Round(x.PercentageCompleted)), Severity = severityType }) ?? new List<AlertMasterModel>());
                }
            }

            return list;
        }

        private static List<AlertMasterModel> Get_WorkStart_End_Current(IEnumerable<WorkMasterModel> model, AlertConfigurationSecondaryModel seconday, List<AlertMasterModel> list, string severityType, AlertConfigurationPrimaryModel primary)
        {
            if (seconday.Severity == severityType)
            {
                var milestonedelayTemplate = @"{0} -- Tender: {1}, Starts: {2}, Ends: {3}";
                if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThan)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_WorkStartDate ? string.IsNullOrWhiteSpace(x.WorkTemplateId) : !string.IsNullOrWhiteSpace(x.WorkTemplateId)) && ((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays > Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} TenderNumber {item.TenderNumber} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_WorkStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays},  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x =>
                    new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.Division,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "WORK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThan)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_WorkStartDate ? string.IsNullOrWhiteSpace(x.WorkTemplateId) : !string.IsNullOrWhiteSpace(x.WorkTemplateId)) && ((seconday.Field == AlertConfigConstants.Field_WorkStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays < Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} TenderNumber {item.TenderNumber} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_WorkStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays},  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.Division,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "WORK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThanEqual)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_WorkStartDate ? string.IsNullOrWhiteSpace(x.WorkTemplateId) : !string.IsNullOrWhiteSpace(x.WorkTemplateId)) && ((seconday.Field == AlertConfigConstants.Field_WorkStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays <= Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} TenderNumber {item.TenderNumber} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_WorkStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays},  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.Division,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "WORK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThanEqual)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_WorkStartDate ? string.IsNullOrWhiteSpace(x.WorkTemplateId) : !string.IsNullOrWhiteSpace(x.WorkTemplateId)) && ((seconday.Field == AlertConfigConstants.Field_WorkStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays >= Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} TenderNumber {item.TenderNumber} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_WorkStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays},  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.Division,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "WORK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
            }

            return list;
        }

        private static List<AlertMasterModel> Get_WorkCost_actuals(IEnumerable<WorkMasterModel> model, AlertConfigurationSecondaryModel seconday, List<AlertMasterModel> list, string severityType, AlertConfigurationPrimaryModel primary)
        {
            if (seconday.Severity == severityType)
            {
                var milestonedelayTemplate = @"{0} -- Tender: {1}, Planned Cost: {2}, Actual Cost: {3}";
                if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThan)
                {
                    var milestoneList = model.Where(x => Convert.ToDecimal(x.WorkValue) > 0 && ((x.WorkValueIncreasedAmount / Convert.ToDecimal(x.WorkValue)) * 100) > seconday.CalculationNo);
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} WorkValueIncreasedAmount {item.WorkValueIncreasedAmount} , TenderFinalAmount {item.TenderFinalAmount} ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.Division,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "WORK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.GOTotalAmount + x.WorkValueIncreasedAmount, 0),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThan)
                {
                    var milestoneList = model.Where(x => Convert.ToDecimal(x.TenderFinalAmount) > 0 && ((x.WorkValueIncreasedAmount / Convert.ToDecimal(x.TenderFinalAmount))) < seconday.CalculationNo);
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} WorkValueIncreasedAmount {item.WorkValueIncreasedAmount} , TenderFinalAmount {item.TenderFinalAmount} ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.Division,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "WORK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.GOTotalAmount + x.WorkValueIncreasedAmount, 0),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThanEqual)
                {
                    var milestoneList = model.Where(x => Convert.ToDecimal(x.TenderFinalAmount) > 0 && ((x.WorkValueIncreasedAmount / Convert.ToDecimal(x.TenderFinalAmount))) <= seconday.CalculationNo);
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} WorkValueIncreasedAmount {item.WorkValueIncreasedAmount} , TenderFinalAmount {item.TenderFinalAmount} ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.Division,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "WORK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.GOTotalAmount + x.WorkValueIncreasedAmount, 0),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThanEqual)
                {
                    var milestoneList = model.Where(x => Convert.ToDecimal(x.TenderFinalAmount) > 0 && ((x.WorkValueIncreasedAmount / Convert.ToDecimal(x.TenderFinalAmount))) >= seconday.CalculationNo);
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} WorkValueIncreasedAmount {item.WorkValueIncreasedAmount} , TenderFinalAmount {item.TenderFinalAmount} ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.Division,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "WORK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.GOTotalAmount + x.WorkValueIncreasedAmount, 0),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
            }

            return list;
        }

        private static List<AlertMasterModel> Get_MbookStart_End_Current(IEnumerable<MBookMasterModel> model, AlertConfigurationSecondaryModel seconday, List<AlertMasterModel> list, string severityType, AlertConfigurationPrimaryModel primary)
        {
            if (seconday.Severity == severityType)
            {
                var milestonedelayTemplate = @"{0} -- Tender Number: {1}, M-Book Number: {2}, Starts: {3}, Ends: {4}";
                if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThan)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? !x.IsSubmitted : !x.IsCompleted) && ((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays > Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays}, PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x =>
                    new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.DivisionId,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "M-BOOK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MBookNumber, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThan)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? !x.IsSubmitted : !x.IsCompleted) && ((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays < Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays}, PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.DivisionId,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "M-BOOK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MBookNumber, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThanEqual)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? !x.IsSubmitted : !x.IsCompleted) && ((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays <= Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays}, PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.DivisionId,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "M-BOOK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MBookNumber, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThanEqual)
                {
                    var milestoneList = model.Where(x => (seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? !x.IsSubmitted : !x.IsCompleted) && ((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? x.StartDate : x.EndDate) - DateTime.Today).TotalDays >= Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" Diff of day - {((seconday.Field == AlertConfigConstants.Field_MilestoneStartDate ? item.StartDate : item.EndDate) - DateTime.Today).TotalDays}, PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.DivisionId,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "M-BOOK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MBookNumber, x.StartDate.ToString("yyyy-MM-dd"), x.EndDate.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
            }

            return list;
        }

        private static List<AlertMasterModel> Get_Mbook_Approvals(IEnumerable<MBookMasterModel> model, AlertConfigurationSecondaryModel seconday, List<AlertMasterModel> list, string severityType, AlertConfigurationPrimaryModel primary)
        {
            if (seconday.Severity == severityType)
            {
                var milestonedelayTemplate = @"{0} -- Tender Number: {1}, M-Book Number: {2}, Last Action Date: {3}";
                if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThan)
                {
                    var milestoneList = model.Where(x => !x.IsCompleted && (x.LastApprovedDate - DateTime.Today)?.TotalDays > Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" MilestoneAmount {item.MilestoneAmount} ActualAmount {item.ActualAmount} , PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x =>
                    new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.DivisionId,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "M-BOOK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MBookNumber, x.LastApprovedDate?.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThan)
                {
                    var milestoneList = model.Where(x => !x.IsCompleted && (x.LastApprovedDate - DateTime.Today)?.TotalDays < Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" MilestoneAmount {item.MilestoneAmount} ActualAmount {item.ActualAmount} , PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.DivisionId,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "M-BOOK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MBookNumber, x.LastApprovedDate?.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_LessThanEqual)
                {
                    var milestoneList = model.Where(x => !x.IsCompleted && (x.LastApprovedDate - DateTime.Today)?.TotalDays < Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" MilestoneAmount {item.MilestoneAmount} ActualAmount {item.ActualAmount} , PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.DivisionId,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "M-BOOK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MBookNumber, x.LastApprovedDate?.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
                else if (seconday.CalculationType == AlertConfigConstants.CalculationType_GreaterThanEqual)
                {
                    var milestoneList = model.Where(x => !x.IsCompleted && (x.LastApprovedDate - DateTime.Today)?.TotalDays < Convert.ToInt64(seconday.CalculationNo));
                    if (milestoneList != null && milestoneList.Count() > 0)
                        foreach (var item in milestoneList)
                        {
                            Console.WriteLine($"{seconday.CalculationType} ---- {severityType} MilestoneName {item.MilestoneName} , StartDate {item.StartDate} , EndDate{item.EndDate}," +
                                $" MilestoneAmount {item.MilestoneAmount} ActualAmount {item.ActualAmount} , PercentageCompleted {item.PercentageCompleted}  ");
                        }
                    list.AddRange(milestoneList?.Where(y => !list.Any(c => c.TypeId == y.Id && c.AlertId == seconday.PrimaryId && c.Severity == AlertConfigConstants.Severity_Red)).Select(x => new AlertMasterModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DivisionId = x.DivisionId,
                        AlertId = seconday.PrimaryId,
                        TypeId = x.Id,
                        Type = "M-BOOK",
                        Message = string.Format(milestonedelayTemplate, primary.Name, x.TenderNumber, x.MBookNumber, x.LastApprovedDate?.ToString("yyyy-MM-dd")),
                        Severity = severityType
                    }) ?? new List<AlertMasterModel>());
                }
            }

            return list;
        }

        public bool Alert_Resolve(string id, string CreatedBy, string CreatedByUserName, string note)
        {
            return _generalDAL.Alert_Resolve(id, CreatedBy, CreatedByUserName, note);
        }
    }
}
