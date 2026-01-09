using AutoMapper;
using BAL.BackgroundWorkerService;
using BAL.Interface;
using DAL;
using DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Constants;
using Model.CustomeAttributes;
using Model.DomainModel;
using Model.MailTemplateHelper;
using Model.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Serilog;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Utils;
using Utils.Interface;
using Utils.UtilModels;

namespace BAL
{
    public class WorkBAL : IWorkBAL
    {
        private readonly SettingsDAL _settingDAL;
        private readonly GeneralDAL _generalDAL;
        private readonly WorkDAL _workDAL;
        private readonly IMapper _mapper;
        private readonly IFTPHelpers _fTPHelpers;
        private readonly IGeneralBAL _generalBAL;
        private readonly HttpClient _httpClient;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private object _workBAL;

        public WorkBAL(IMySqlDapperHelper mySqlDapperHelper, IMySqlHelper mySqlHelper, IMapper mapper,
            IConfiguration configuration, IFTPHelpers fTPHelpers, IGeneralBAL generalBAL, IBackgroundTaskQueue backgroundTaskQueue, IServiceScopeFactory serviceScopeFactory)
        {
           HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            _settingDAL = new SettingsDAL(mySqlDapperHelper, mySqlHelper, configuration);
            _generalDAL = new GeneralDAL(mySqlDapperHelper, mySqlHelper, configuration);
            _workDAL = new WorkDAL(mySqlDapperHelper, mySqlHelper, configuration);
            _configuration = configuration;
            _mapper = mapper;
            _fTPHelpers = fTPHelpers;
            _httpClient = new HttpClient(clientHandler);
            _generalBAL = generalBAL;
            _backgroundTaskQueue = backgroundTaskQueue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        #region GO

        public List<GOMasterModel> GO_Get(bool IsActive = true, string Id = "", string GONumber = "", string LocalGONumber = "", string DepartmentId = "")
        {
            return _workDAL.GO_Get(IsActive, Id, GONumber, LocalGONumber, DepartmentId);
        }

        public List<GOMasterModel> GO_Get(GoFilterModel model, out int TotalCount)
        {
            return _workDAL.GO_Get(model, out TotalCount);
        }
        public string GO_SaveUpdate(GOMasterModel model)
        {
            #region Save Difference
            GOMasterModel? exist = GO_Get(true, model.Id)?.FirstOrDefault() ?? new GOMasterModel();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = GO_Get(false, model.Id)?.FirstOrDefault() ?? new GOMasterModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<GOMasterModel, LogFieldAttribute>();
            if (model.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _workDAL.GO_SaveUpdate(model);
        }
        public List<GOReportViewModel> GO_Report_Get(GoFilterModel model, out int TotalCount)
        {
            return _workDAL.GO_Report_Get(model, out TotalCount);
        }

        #endregion GO

        #region Tender

        public List<TenderMasterModel> Tender_Get(bool IsActive = true, string Id = "", string TenderNumber = "", string LocalTenderNumber = "", string GOId = "", string DepartmentId = "")
        {
            return _workDAL.Tender_Get(IsActive, Id, TenderNumber, LocalTenderNumber, GOId, DepartmentId);
        }
        public List<TenderMasterModel> Tender_Get(TenderFilterModel model, out int TotalCount)
        {
            return _workDAL.Tender_Get(model, out TotalCount);
        }
        public string Tender_SaveUpdate(TenderMasterModel model)
        {
            #region Save Difference
            TenderMasterModel? exist = Tender_Get(true, model.Id)?.FirstOrDefault() ?? new TenderMasterModel();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = Tender_Get(false, model.Id)?.FirstOrDefault() ?? new TenderMasterModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<TenderMasterModel, LogFieldAttribute>();
            if (model.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _workDAL.Tender_SaveUpdate(model);
        }
        public string Tender_Update_Amount(TenderAmountUpdateModel model) // Record Log Not Required
        {
            WorkActivityModel workActivity = new WorkActivityModel();
            workActivity.Id = Guid.NewGuid().ToString();
            workActivity.ParentType = "TENDER";
            workActivity.ParentId = model.TenderId;
            workActivity.Type = "TENDER";
            workActivity.TypeId = model.TenderId;
            workActivity.ActivitySubject = WorkActivityMessageConst.TenderAmountIncreased;
            workActivity.ActivityMessage = WorkActivityMessageConst.TenderAmountIncreased + ": Amount = '" + model.IncreasedAmount.ToString() + "', Note: '" + model.UpdatedNote + "'";
            workActivity.SavedBy = model.SavedBy ?? "";
            workActivity.SavedByUserName = model.SavedByUserName ?? "";
            workActivity.SavedDate = model.SavedDate ?? new DateTime();

            _workDAL.Work_Activity_SaveUpdate(workActivity);

            return _workDAL.Tender_Update_Amount(model);
        }
        public List<TenderRelatedIdModel> Get_TenderRelatedIds(string TenderId)
        {
            return _workDAL.Get_TenderRelatedIds(TenderId);
        }
        public List<string> Tender_Ids_Get_ByContractor(string ContractorId)
        {
            return _workDAL.Tender_Ids_Get_ByContractor(ContractorId);
        }
        public List<string> Mbook_Id_Get_ByContractor(string ContractorId)
        {
            return _workDAL.Mbook_Id_Get_ByContractor(ContractorId);
        }
        #endregion Tender

        #region Work

        public List<WorkMasterModel> Work_Get(bool IsActive = true, string Id = "", string TenderId = "", string WorkNumber = "")
        {
            List<WorkMasterModel> list = _workDAL.Work_Get(IsActive, Id, TenderId, WorkNumber);

            list.ForEach(x =>
            {
                if (string.IsNullOrWhiteSpace(x.AgreementCopyId) || string.IsNullOrWhiteSpace(x.WorkOrderId) || string.IsNullOrWhiteSpace(x.AgreementCopyId))
                {

                }
            });

            return list;
        }
        public List<WorkMasterModel> Work_Get_All(bool IsActive = true, string Id = "", string TenderId = "", string WorkNumber = "")
        {
            List<WorkMasterModel> list = _workDAL.Work_Get_All(IsActive, Id, TenderId, WorkNumber);
            return list;
        }
        public List<WorkMasterModel> Work_Get(WorkFilterModel model, out int TotalCount)
        {
            return _workDAL.Work_Get(model, out TotalCount);
        }
        public string Work_SaveUpdate(WorkMasterModel model)
        {
            #region Save Difference
            if (model.IsActive == true)
            {
                WorkMasterModel? exist = Work_Get(true, model.Id)?.FirstOrDefault() ?? new WorkMasterModel();
                if (string.IsNullOrWhiteSpace(exist.Id))
                {
                    exist = Work_Get(false, model.Id)?.FirstOrDefault() ?? new WorkMasterModel();
                }
                ObjectDifference diff = new ObjectDifference(model, exist);
                diff.Properties = StringFunctions.GetPropertiesWithAttribute<WorkMasterModel, LogFieldAttribute>();
                diff.SavedBy = model.SavedBy;
                diff.SavedByUserName = model.SavedByUserName;
                diff.SavedDate = model.SavedDate;
                _generalDAL.SaveRecordDifference(diff);
            }
            #endregion Save Difference

            return _workDAL.Work_SaveUpdate(model);
        }

        #endregion Work

        #region Work Template
        public List<WorkTemplateMasterModel> Work_Template_Get(bool IsActive = true, string Id = "", string WorkId = "", string WorkTypeId = "", string TemplateId = "")
        {
            return _workDAL.Work_Template_Get(IsActive, Id, WorkId, WorkTypeId, TemplateId);
        }
        public string Work_Template_SaveUpdate(WorkTemplateMasterModel model)
        {
            #region Save Difference
            WorkTemplateMasterModel? exist = Work_Template_Get(true, model.Id)?.FirstOrDefault() ?? new WorkTemplateMasterModel();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = Work_Template_Get(false, model.Id)?.FirstOrDefault() ?? new WorkTemplateMasterModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<WorkTemplateMasterModel, LogFieldAttribute>();
            if (model.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _workDAL.Work_Template_SaveUpdate(model);
        }
        public string Work_Template_Submit(WorkTemplateMasterModel model)
        {
            return _workDAL.Work_Template_Submit(model);
        }
        #endregion Work Template

        //TEMPLATE RE-ASSIGN (LINE 237-240)
        public string DeleteWorkTemplate(string WorkId, string SavedBy, string SavedByUserName, DateTime SavedDate)
        {
            return _workDAL.DeleteWorkTemplate(WorkId, SavedBy, SavedByUserName, SavedDate);
        }
        public string UpdateDatedifference()
        {
            return _workDAL.UpdateDateDifference();
        }
        
        public string UpdateCameraDatabase()
        {
            return _workDAL.UpdateCameraDatabase();
        }
        





        #region Work Template Milestone
        public List<WorkTemplateMilestoneMasterModel> Work_Template_Milestone_Master_Get(bool IsActive = true, string Id = "", string WorkTemplateId = "", string WorkId = "",
            string TenderId = "", string DivisionId = "", string MilestoneStatusId = "", string PaymentStatusId = "")
        {
            return _workDAL.Work_Template_Milestone_Master_Get(IsActive, Id, WorkTemplateId, WorkId, TenderId, DivisionId, MilestoneStatusId, PaymentStatusId);
        }

        public List<MilestoneReportModel> Milestone_Get(MilestoneFilterModel model, out int TotalCount)
        {
            return _workDAL.Milestone_Get(model, out TotalCount);
        }
        public string Work_Template_Milestone_Master_SaveUpdate(WorkTemplateMilestoneMasterModel model)
        {
            #region Save Difference
            WorkTemplateMilestoneMasterModel? exist = Work_Template_Milestone_Master_Get(true, model.Id)?.FirstOrDefault() ?? new WorkTemplateMilestoneMasterModel();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = Work_Template_Milestone_Master_Get(false, model.Id)?.FirstOrDefault() ?? new WorkTemplateMilestoneMasterModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<WorkTemplateMilestoneMasterModel, LogFieldAttribute>();
            if (model.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _workDAL.Work_Template_Milestone_Master_SaveUpdate(model);
        }

        public string SetWorkCompletionDate(string WorkTemplateId)
        {
            return _workDAL.SetWorkCompletionDate(WorkTemplateId);
        }
        public string Work_Template_Milestone_Master_Delete_All(WorkTemplateMilestoneMasterModel model)
        {
            return _workDAL.Work_Template_Milestone_Master_Delete_All(model);
        }
        public string Update_Milestone_Completed_Percentage(MilestoneUpdateModel model) // Record Log Not Required
        {
            return _workDAL.Update_Milestone_Completed_Percentage(model);
        }
        #endregion Work Template Milestone

        #region M-Book
        public List<MBookMasterModel> Work_MBook_Get(MBookFilterModel model, string UserId, string RoleCode, string UserGroupName, string DivisionId, out int TotalCount)
        {
            string roleId = model.RoleId ?? "";
            if (Constants.StaticRoles.Contains(RoleCode) || string.Equals(UserGroupName, UserGroupConst.Engineer, StringComparison.CurrentCultureIgnoreCase))
            {
                model.RoleId = "";
            }
            List<MBookMasterModel> mBookList = new List<MBookMasterModel>();
            if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
            {
                if (model.DivisionIds?.Count > 0)
                {
                    model.DivisionIds.Clear();
                }
                if (!string.IsNullOrWhiteSpace(UserId))
                {
                    model.TenderIds = _workDAL.Tender_Ids_Get_ByContractor(UserId);
                }
                mBookList = _workDAL.Work_MBook_Get(model, out TotalCount);
            }
            else
            {
                if (model.DivisionIds == null || model.DivisionIds.Count == 0)
                {
                    model.DivisionIds = DivisionId.Split(',').ToList();
                }
                mBookList = _workDAL.Work_MBook_Get(model, out TotalCount);
            }
            if ((model.Where?.IsActive ?? false) == true)
            {
                mBookList.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.ActionableRoleId) && (Constants.StaticRoles.Contains(RoleCode) || string.Equals(UserGroupName, UserGroupConst.Engineer, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        x.IsEditable = true;
                        x.IsActionable = false;
                    }
                    else if (roleId == x.ActionableRoleId)
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

            return mBookList;
        }
        public List<MBookReportModel> Work_MBook_Report_Get(MBookReportFilterModel model, out int TotalCount)
        {
            return _workDAL.Work_MBook_Report_Get(model, out TotalCount);
        }
        public MBookMasterModel? Work_MBook_GetById(string MBookId)
        {
            return _workDAL.Work_MBook_GetById(Id: MBookId).FirstOrDefault();
        }
        public List<MBookMasterModel> Work_MBook_Get(bool IsActive = true, string Id = "", string WorkTemplateMilestoneId = "", string ActionableRoleId = "", string DivisionId = "",
            string StatusId = "", string WorkId = "", string TenderId = "")
        {
            return _workDAL.Work_MBook_Get(IsActive, Id, WorkTemplateMilestoneId, ActionableRoleId, DivisionId, StatusId, WorkId, TenderId);
        }
        public string Work_MBook_SaveUpdate(MBookMasterModel model, string message = "", string subject = "")
        {
            #region Save Difference
            MBookMasterModel? exist = Work_MBook_GetById(model.Id) ?? new MBookMasterModel();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = Work_MBook_GetById(model.Id) ?? new MBookMasterModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<MBookMasterModel, LogFieldAttribute>();
            if (model.IsActive == false)
            {
                diff.IsDeleted = true;
            }
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            #region Save Activity
            WorkActivityModel workActivity = new WorkActivityModel();
            workActivity.Id = Guid.NewGuid().ToString();
            workActivity.ParentType = "";
            workActivity.ParentId = "";
            workActivity.Type = "MBOOK";
            workActivity.TypeId = model.Id;
            if (string.IsNullOrWhiteSpace(message) && string.IsNullOrWhiteSpace(subject))
            {
                if (model.IsActive && model.IsSubmitted)
                {
                    workActivity.ActivitySubject = "M-Book (" + model.MilestoneCode + ") submitted";
                    workActivity.ActivityMessage = "M-Book (" + model.MilestoneCode + ") submitted";
                }
                else if (model.IsActive)
                {
                    workActivity.ActivitySubject = "M-Book (" + model.MilestoneCode + ") created/updated";
                    workActivity.ActivityMessage = "M-Book (" + model.MilestoneCode + ") created/updated";
                }
                else
                {
                    workActivity.ActivitySubject = "M-Book (" + model.MilestoneCode + ") deleted";
                    workActivity.ActivityMessage = "M-Book (" + model.MilestoneCode + ") deleted";
                }
            }
            else
            {
                workActivity.ActivitySubject = subject;
                workActivity.ActivityMessage = message;
            }
            workActivity.SavedBy = model.SavedBy;
            workActivity.SavedByUserName = model.SavedByUserName;
            workActivity.SavedDate = DateTime.Now;
            _workDAL.Work_Activity_SaveUpdate(workActivity);
            #endregion Save Activity

            string MilestoneCode = "";

            #region Update Milestone
            if (model.ActionableRoleId == "-2")
            {
                WorkTemplateMilestoneMasterModel? milestone = _workDAL.Work_Template_Milestone_Master_Get(true, Id: model.WorkTemplateMilestoneId).FirstOrDefault();
                if (milestone != null)
                {
                    StatusMaster? status_list = _settingDAL.Status_Get(StatusCode: StatusCodeConst.Completed).FirstOrDefault();
                    milestone.MilestoneStatus = status_list?.Id ?? "";
                    milestone.SavedBy = model.SavedBy;
                    milestone.SavedByUserName = model.SavedByUserName;
                    milestone.SavedDate = model.SavedDate;
                    milestone.CompletedDate = DateTime.Now;

                    MilestoneCode = model.MilestoneCode;

                    Work_Template_Milestone_Master_SaveUpdate(milestone);
                }

                model.CompletedDate = DateTime.Now;
            }
            else if (model.IsSubmitted)
            {
                WorkTemplateMilestoneMasterModel? milestone = _workDAL.Work_Template_Milestone_Master_Get(true, Id: model.WorkTemplateMilestoneId).FirstOrDefault();
                if (milestone != null)
                {
                    StatusMaster? status_list = _settingDAL.Status_Get(StatusCode: StatusCodeConst.Inprogress).FirstOrDefault();
                    milestone.MilestoneStatus = status_list?.Id ?? "";
                    milestone.SavedBy = model.SavedBy;
                    milestone.SavedByUserName = model.SavedByUserName;
                    milestone.SavedDate = model.SavedDate;
                    milestone.ActualAmount = model.ActualAmount;

                    MilestoneCode = model.MilestoneCode;

                    Work_Template_Milestone_Master_SaveUpdate(milestone);
                }

                model.SubmittedDate = DateTime.Now;
            }
            #endregion Update Milestone

            #region Mail
            if (model.IsSubmitted)
            {
                EmailTemplateModel? template = WorkMailTemplate.GetEmailTemplate(EmailTemplateCode.MBookStatusMail);
                List<EmailModel> email = new List<EmailModel>();
                List<SMSModel> sms = new List<SMSModel>();

                if (template != null)
                {
                    List<AccountUserModel> userList = _workDAL.Tender_User_Get(MBookId: model.Id);

                    if (userList.Count > 0)
                    {
                        foreach (AccountUserModel user in userList)
                        {
                            if (!string.IsNullOrWhiteSpace(user.Email))
                            {
                                email.Add(new EmailModel()
                                {
                                    Body = template.Body,
                                    Subject = template.Subject,
                                    To = new List<string>() { user.Email },
                                    BodyPlaceHolders = new Dictionary<string, string>() {
                                        { "{RECIPIENTFIRSTNAME}", user.FirstName },
                                        { "{RECIPIENTLASTNAME}", user.LastName },
                                        { "{MBOOKCODE}", model.MBookNumber },
                                        { "{MILESTONECODE}", MilestoneCode },
                                        { "{ACTIONBYROLE}", model.SavedByRoleName },
                                        { "{ACTIONBY}", model.SavedByUserName },
                                        { "{MBOOKSTATUS}", "Submitted" }
                                    },
                                    SubjectPlaceHolders = new Dictionary<string, string>() {
                                        { "{MBOOKCODE}", model.MBookNumber }
                                    },
                                    SavedBy = model.SavedBy,
                                    SavedByUserName = model.SavedByUserName,
                                    SavedDate = model.SavedDate,
                                    Type = "MBOOK",
                                    TypeId = model.Id,
                                    ReceivedBy = user.UserId
                                });
                            }
                            if (!string.IsNullOrWhiteSpace(user.Mobile))
                            {

                            }
                        }

                        SendMessage(email, sms);
                    }
                }
            }
            #endregion Mail

            return _workDAL.Work_MBook_SaveUpdate(model);
        }
        public string Work_Approve_MBook(MbookApprovalModel model, string RoleId)
        {
            MBookMasterModel? mBook = _workDAL.Work_MBook_Get(Id: model.MbookId).FirstOrDefault();
            if (mBook is not null)
            {
                ApprovalFlowMaster? approvalFlow = _settingDAL.ApprovalFlow_Get(mBook.DepartmentId, RoleId).FirstOrDefault();

                if (approvalFlow is not null)
                {
                    string MbookStatus = "";
                    List<string> UserGroupList = new List<string>();
                    List<AccountUserModel> userList = _workDAL.Tender_User_Get(MBookId: mBook.Id);

                    List<StatusMaster>? status_list = _settingDAL.Status_Get(Type: StatusTypeConst.MBook);

                    MBookApprovalHistoryModel historyModel = new MBookApprovalHistoryModel();
                    historyModel.Id = model.MbookApprovHistoryeId;
                    historyModel.MBookId = model.MbookId;
                    historyModel.FromId = mBook.ActionableRoleId;
                    historyModel.Comments = model.Comments;
                    historyModel.SavedBy = model.SavedBy;
                    historyModel.SavedByUserName = model.SavedByUserName;
                    historyModel.SavedDate = model.SavedDate;
                    historyModel.DocumentName = model.DocumentName;

                    StatusMaster? status = status_list.Find(x => x.StatusCode == model.StatusCode);
                    if (approvalFlow.ApprovalFlowId == "-2" && string.Equals(model.StatusCode, StatusCodeConst.Approve, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (mBook.PercentageCompleted == 100)
                        {
                            status = status_list.Find(x => x.StatusCode == StatusCodeConst.Completed);
                            WorkTemplateMilestoneMasterModel? milestone = Work_Template_Milestone_Master_Get(true, Id: mBook.WorkTemplateMilestoneId).FirstOrDefault();
                            if (milestone != null)
                            {
                                milestone.IsCompleted = true;
                                milestone.PaymentStatus = status_list.Find(x => x.StatusCode == StatusCodeConst.PaymentDone)?.Id ?? "";
                                milestone.MilestoneStatus = status?.Id ?? "";
                                milestone.SavedBy = model.SavedBy;
                                milestone.ActualAmount = mBook.ActualAmount;
                                milestone.SavedByUserName = model.SavedByUserName;
                                milestone.SavedDate = model.SavedDate;
                                milestone.CompletedDate = DateTime.Now;

                                Work_Template_Milestone_Master_SaveUpdate(milestone);

                                #region Save Activity
                                WorkActivityModel workActivity = new WorkActivityModel();
                                workActivity.Id = Guid.NewGuid().ToString();
                                workActivity.ParentType = "";
                                workActivity.ParentId = "";
                                workActivity.Type = "MILESTONE";
                                workActivity.TypeId = milestone.Id;
                                workActivity.ActivitySubject = "Milestone (" + milestone.MilestoneCode + ") completed";
                                workActivity.ActivityMessage = "Milestone (" + milestone.MilestoneCode + ") completed";
                                workActivity.SavedBy = model.SavedBy;
                                workActivity.SavedByUserName = model.SavedByUserName;
                                workActivity.SavedDate = DateTime.Now;
                                _workDAL.Work_Activity_SaveUpdate(workActivity);
                                #endregion Save Activity

                                #region Work/Milestone/Mbook Completed Mail
                                List<EmailModel> email = new List<EmailModel>();
                                List<SMSModel> sms = new List<SMSModel>();

                                WorkMasterModel? workMasterModel = _workDAL.Work_Get_All(true, Id: mBook.WorkId).FirstOrDefault();
                                if (workMasterModel != null && workMasterModel.IsCompleted)
                                {
                                    EmailTemplateModel? template = WorkMailTemplate.GetEmailTemplate(EmailTemplateCode.WorkCompletedMail);
                                    if (userList.Count > 0 && template != null)
                                    {
                                        foreach (AccountUserModel user in userList)
                                        {
                                            if (!string.IsNullOrWhiteSpace(user.Email))
                                            {
                                                email.Add(new EmailModel()
                                                {
                                                    Body = template.Body,
                                                    Subject = template.Subject,
                                                    To = new List<string>() { user.Email },
                                                    BodyPlaceHolders = new Dictionary<string, string>() {
                                                        { "{RECIPIENTFIRSTNAME}", user.FirstName },
                                                        { "{RECIPIENTLASTNAME}", user.LastName },
                                                        { "{WORKCODE}", workMasterModel.TenderNumber },
                                                        { "{ACTIONBYROLE}", model.SavedByRoleName },
                                                        { "{ACTIONBY}", model.SavedByUserName }
                                                    },
                                                    SubjectPlaceHolders = new Dictionary<string, string>() {
                                                        { "{WORKCODE}", workMasterModel.TenderNumber }
                                                    },
                                                    SavedBy = model.SavedBy,
                                                    SavedByUserName = model.SavedByUserName,
                                                    SavedDate = model.SavedDate,
                                                    Type = "WORK",
                                                    TypeId = mBook.Id,
                                                    ReceivedBy = user.UserId
                                                });
                                            }
                                            if (!string.IsNullOrWhiteSpace(user.Mobile))
                                            {

                                            }
                                        }
                                    }
                                }
                                if (milestone != null)
                                {
                                    EmailTemplateModel? template = WorkMailTemplate.GetEmailTemplate(EmailTemplateCode.MilestoneCompletedMail);
                                    List<string> UserGroupList_milestoneComp = new List<string>() { UserGroupConst.Contractor, UserGroupConst.Engineer };
                                    List<AccountUserModel> usersList22 = userList.Where(x => UserGroupList_milestoneComp.Contains(x.UserGroupName)).ToList();
                                    if (usersList22.Count > 0 && template != null)
                                    {
                                        foreach (AccountUserModel user in usersList22)
                                        {
                                            if (!string.IsNullOrWhiteSpace(user.Email))
                                            {
                                                email.Add(new EmailModel()
                                                {
                                                    Body = template.Body,
                                                    Subject = template.Subject,
                                                    To = new List<string>() { user.Email },
                                                    BodyPlaceHolders = new Dictionary<string, string>() {
                                                        { "{RECIPIENTFIRSTNAME}", user.FirstName },
                                                        { "{RECIPIENTLASTNAME}", user.LastName },
                                                        { "{MILESTONECODE}", milestone.MilestoneCode },
                                                        { "{ACTIONBYROLE}", model.SavedByRoleName },
                                                        { "{ACTIONBY}", model.SavedByUserName }
                                                    },
                                                    SubjectPlaceHolders = new Dictionary<string, string>() {
                                                        { "{MILESTONECODE}", milestone.MilestoneCode }
                                                    },
                                                    SavedBy = model.SavedBy,
                                                    SavedByUserName = model.SavedByUserName,
                                                    SavedDate = model.SavedDate,
                                                    Type = "MILESTONE",
                                                    TypeId = mBook.Id,
                                                    ReceivedBy = user.UserId
                                                });
                                            }
                                            if (!string.IsNullOrWhiteSpace(user.Mobile))
                                            {

                                            }
                                        }
                                    }
                                }

                                SendMessage(email, sms);

                                #endregion Work/Milestone/Mbook Completed Mail
                            }
                        }
                        else
                        {
                            return "PERCENTAGE_NOT_100";
                        }
                    }
                    if (status is not null)
                    {
                        mBook.StatusId = status.Id;
                        mBook.StatusName = status.StatusName;
                        mBook.StatusCode = status.StatusCode;
                        historyModel.StatusEnum = model.StatusCode;
                    }

                    mBook.SavedBy = model.SavedBy;
                    mBook.SavedByUserName = model.SavedByUserName;
                    mBook.SavedDate = model.SavedDate;
                    // Modified by indu for mbook approval comments log
                    if (string.Equals(model.StatusCode, StatusCodeConst.Approve, StringComparison.CurrentCultureIgnoreCase))
                    {
                        mBook.IsReturned = false;

                        mBook.ActionableRoleId = approvalFlow.ApprovalFlowId;
                        historyModel.ToId = approvalFlow.ApprovalFlowId;
                        historyModel.StatusEnum = StatusCodeConst.Approve;

                        string subject = "M-Book approved";
                        string message = "M-Book (" + mBook.MilestoneName + ":" + mBook.MBookNumber + ") approved by " + approvalFlow.RoleName + " and moved to " + approvalFlow.ApprovalFlowRoleName;

                        if (approvalFlow.ApprovalFlowId == "-2")
                        {
                            StatusMaster? status_payment_done = status_list.Find(x => x.StatusCode == StatusCodeConst.PaymentDone);

                            mBook.PaymentStatusId = status_payment_done?.Id ?? "8e6b314c-7b20-11ee-b363-fa163e14116e";
                            mBook.PaymentStatusCode = status_payment_done?.StatusCode ?? "PAYMENTDONE";
                            mBook.PaymentStatus = status_payment_done?.StatusName ?? "Payment Done";
                            mBook.PaymentStatusName = status_payment_done?.StatusName ?? "Payment Done";

                            StatusMaster? status_completed = status_list.Find(x => x.StatusCode == StatusCodeConst.Completed);

                            mBook.StatusId = status_completed?.Id ?? "";
                            mBook.StatusCode = status_completed?.StatusCode ?? "COMPLETED";
                            mBook.StatusName = status_completed?.StatusName ?? "Completed";
                            mBook.StatusName = status_completed?.StatusName ?? "Completed";

                            MbookStatus = "Completed";
                            UserGroupList.Add(UserGroupConst.Engineer);
                            UserGroupList.Add(UserGroupConst.Contractor);
                        }
                        else
                        {
                            MbookStatus = "Approved";
                            UserGroupList.Add(UserGroupConst.Engineer);
                        }

                        Work_MBook_SaveUpdate(mBook, message, subject);
                        MBookMasterModel? exist = Work_MBook_GetById(model.MbookId);
                        if (!string.IsNullOrWhiteSpace(model.MbookId))
                        {
                            CommentMasterModel comment = new CommentMasterModel();
                            comment.SubjectText = "M-Book Approved by "+ approvalFlow.RoleName;
                            comment.TypeId = model.MbookId;
                            comment.Type = CommentTypeConst.Mbook;
                            comment.CommentsFrom = CommentTypeConst.Mbook;
                            comment.CommentsText = model.Comments;
                            comment.CommentDate = model.SavedDate;
                            comment.CreatedDate = model.SavedDate;
                            comment.CreatedBy = model.SavedBy;
                            comment.CreatedByUserName = model.SavedByUserName;

                            Comment_SaveUpdate(comment);
                        }
                    }
                    else if (string.Equals(model.StatusCode, StatusCodeConst.Return, StringComparison.CurrentCultureIgnoreCase))
                    {
                        mBook.IsReturned = true;

                        mBook.ActionableRoleId = approvalFlow.ReturnFlowId;
                        historyModel.ToId = approvalFlow.ReturnFlowId;
                        historyModel.StatusEnum = StatusCodeConst.Return;

                        string subject = "M-Book returned";
                        string message = "M-Book (" + mBook.MilestoneName + ":" + mBook.MBookNumber + ") returned by " + model.SavedByUserName + " and moved to " + approvalFlow.ReturnFlowRoleName;

                        MbookStatus = "Returned";
                        UserGroupList.Add(UserGroupConst.Engineer);

                        Work_MBook_SaveUpdate(mBook, message, subject);
                        MBookMasterModel? exist = Work_MBook_GetById(model.MbookId);
                        if (!string.IsNullOrWhiteSpace(model.MbookId))
                        {
                            CommentMasterModel comment = new CommentMasterModel();
                            comment.SubjectText = "M-Book Returned by " + approvalFlow.RoleName;
                            comment.TypeId = model.MbookId;
                            comment.Type = CommentTypeConst.Mbook;
                            comment.CommentsFrom = CommentTypeConst.Mbook;
                            comment.CommentsText = model.Comments;
                            comment.CommentDate = model.SavedDate;
                            comment.CreatedDate = model.SavedDate;
                            comment.CreatedBy = model.SavedBy;
                            comment.CreatedByUserName = model.SavedByUserName;

                            Comment_SaveUpdate(comment);
                        }
                    }
                    else if (string.Equals(model.StatusCode, StatusCodeConst.Reject, StringComparison.CurrentCultureIgnoreCase))
                    {
                        MbookStatus = "Rejected";
                        UserGroupList.Add(UserGroupConst.Engineer);
                        UserGroupList.Add(UserGroupConst.Contractor);

                        historyModel.ToId = RoleId;
                        historyModel.StatusEnum = StatusCodeConst.Reject;

                        StatusMaster? status_payment_not_init = status_list.Find(x => x.StatusCode == StatusCodeConst.PaymentNotInitiated);

                        mBook.PaymentStatusId = status_payment_not_init?.Id ?? "3c74c9b9-7b36-11ee-8402-00090ffe0001";
                        mBook.PaymentStatusCode = status_payment_not_init?.StatusCode ?? "PAYMENTNOTINITIATED";
                        mBook.PaymentStatus = status_payment_not_init?.StatusName ?? "Payment Not Initiated";
                        mBook.PaymentStatusName = status_payment_not_init?.StatusName ?? "Payment Not Initiated";

                        // InActive m-book
                        mBook.IsActive = false;

                        string subject = "M-Book deleted";
                        string message = "M-Book (" + mBook.MilestoneName + ":" + mBook.MBookNumber + ") deleted by " + model.SavedByUserName + " and new M-Book will be created for this one";

                        Work_MBook_SaveUpdate(mBook, message, subject);

                        // Add new m-book with same info
                        mBook.Id = Guid.NewGuid().ToString();
                        mBook.ActionableRoleId = RoleId;
                        mBook.IsActive = true;
                        mBook.WorkNotes = "";
                        mBook.Date = null;

                        StatusMaster? status_saved = status_list.Find(x => x.StatusCode == StatusCodeConst.Saved);
                        StatusMaster? status_payment = status_list.Find(x => x.StatusCode == StatusCodeConst.PaymentNotInitiated);
                        if (status_saved is not null)
                        {
                            mBook.StatusId = status_saved.Id;
                            mBook.StatusName = status_saved.StatusName;
                            mBook.StatusCode = status_saved.StatusCode;
                        }
                        if (status_payment is not null)
                        {
                            mBook.PaymentStatusId = status_payment.Id;
                            mBook.PaymentStatusName = status_payment.StatusName;
                            mBook.PaymentStatusCode = status_payment.StatusCode;
                        }

                        string subject1 = "New M-Book (" + mBook.MilestoneCode + ") created";
                        string message1 = "New M-Book (" + mBook.MilestoneCode + ") created";

                        Work_MBook_SaveUpdate(mBook, message1, subject1);

                    }

                    #region Send Mail/SMS
                    if (!string.IsNullOrWhiteSpace(MbookStatus))
                    {
                        EmailTemplateModel? template = WorkMailTemplate.GetEmailTemplate(EmailTemplateCode.MBookStatusMail);
                        List<EmailModel> email = new List<EmailModel>();
                        List<SMSModel> sms = new List<SMSModel>();

                        if (template != null)
                        {
                            List<AccountUserModel> userList2 = userList?.Where(x => UserGroupList.Contains(x.UserGroupName))?.ToList() ?? new List<AccountUserModel>();

                            if (userList2.Count > 0)
                            {
                                foreach (AccountUserModel user in userList2)
                                {
                                    if (!string.IsNullOrWhiteSpace(user.Email))
                                    {
                                        email.Add(new EmailModel()
                                        {
                                            Body = template.Body,
                                            Subject = template.Subject,
                                            To = new List<string>() { user.Email },
                                            BodyPlaceHolders = new Dictionary<string, string>() {
                                                { "{RECIPIENTFIRSTNAME}", user.FirstName },
                                                { "{RECIPIENTLASTNAME}", user.LastName },
                                                { "{MBOOKCODE}", mBook.MBookNumber },
                                                { "{ACTIONBYROLE}", model.SavedByRoleName },
                                                { "{ACTIONBY}", model.SavedByUserName },
                                                { "{MBOOKSTATUS}", MbookStatus }
                                            },
                                            SubjectPlaceHolders = new Dictionary<string, string>() {
                                                { "{MBOOKCODE}", mBook.MBookNumber }
                                            },
                                            SavedBy = model.SavedBy,
                                            SavedByUserName = model.SavedByUserName,
                                            SavedDate = model.SavedDate,
                                            Type = "MBOOK",
                                            TypeId = mBook.Id,
                                            ReceivedBy = user.UserId
                                        });
                                    }
                                    if (!string.IsNullOrWhiteSpace(user.Mobile))
                                    {

                                    }
                                }

                                SendMessage(email, sms);
                            }
                        }
                    }
                    #endregion Send Mail/SMS

                    Work_MBook_Approval_History_SaveUpdate(historyModel);

                    return "OK";
                }

            }

            return "NOTOK";
        }
        public List<SelectListItem> MbookFileUploadTypeList()
        {
            List<SelectListItem>? fileTypeList = new List<SelectListItem>()
                {
                    new SelectListItem(){ Text = "M-Book", Value = UploadTypeCode.MBookEntryDocument },
                    new SelectListItem(){ Text = "Quality Testing document", Value = UploadTypeCode.MBookTestingDocument },
                    new SelectListItem(){ Text = "Photos", Value = UploadTypeCode.MBookPhotos },
                    new SelectListItem(){ Text = "Videos", Value = UploadTypeCode.MBookVideos },
                    new SelectListItem(){ Text = "Others", Value = UploadTypeCode.MBookOtherDoc },
                };

            return fileTypeList;
        }
        #endregion M-Book

        #region M-Book Approval_History
        public List<MBookApprovalHistoryViewModel> Work_MBook_Approval_History_Get(string Id = "", string MBookId = "")
        {
            List<MBookApprovalHistoryModel> list = _workDAL.Work_MBook_Approval_History_Get(Id, MBookId);
            if (list.Count > 0)
            {

                return _mapper.Map<List<MBookApprovalHistoryViewModel>>(list);
                
            }
            else
            {
                return new List<MBookApprovalHistoryViewModel>();
            }
        }
        public string Work_MBook_Approval_History_SaveUpdate(MBookApprovalHistoryModel model)
        {
            #region Save Difference
            MBookApprovalHistoryViewModel? exist = Work_MBook_Approval_History_Get(model.Id)?.FirstOrDefault() ?? new MBookApprovalHistoryViewModel();
            if (string.IsNullOrWhiteSpace(exist.Id))
            {
                exist = Work_MBook_Approval_History_Get(model.Id)?.FirstOrDefault() ?? new MBookApprovalHistoryViewModel();
            }
            ObjectDifference diff = new ObjectDifference(model, exist);
            diff.Properties = StringFunctions.GetPropertiesWithAttribute<MBookMasterModel, LogFieldAttribute>();
            diff.IsDeleted = false;
            diff.SavedBy = model.SavedBy;
            diff.SavedByUserName = model.SavedByUserName;
            diff.SavedDate = model.SavedDate;
            _generalDAL.SaveRecordDifference(diff);
            #endregion Save Difference

            return _workDAL.Work_MBook_Approval_History_SaveUpdate(model);
        }
        #endregion M-Book Approval_History

        #region API Call
        
      //  Modified by Indu on 22-10-2025 for sending email notifications
        public TenderDataIntegrationResponceModel FetchAwardedTenders(DateTime fromDate, DateTime ToDate)
        {
            try
            {
                string startTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt");
                SendSchedulerNotificationEmail("TIME Scheduler - Tender Sync Started ", "Syncing Tender records started successfully at " + startTime);

                TenderDataIntegrationResponceModel responce = new TenderDataIntegrationResponceModel();
                responce.NewContractorList = new List<AccountUserModel>();

                AwardedTenderAPIResponce? responce_model = new AwardedTenderAPIResponce();
                HttpResponseMessage message = new HttpResponseMessage();

                TenderAwardedRequestModel model = new TenderAwardedRequestModel();

                model.Fdate = fromDate.ToString("dd/MM/yyyy"); // "05/09/2023";
                model.Tdate = ToDate.ToString("dd/MM/yyyy"); // "13/10/2023";

       

               


                string jsonObject = JsonConvert.SerializeObject(model, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
                var Content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

                string _url = _configuration.GetSection("TenderAPIs").GetSection("Tender_GET_URL").Value.ToString();

                message = _httpClient.PostAsync(_url, Content).Result;

                if (message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string ResponceString = message.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(ResponceString))
                    {
                        TenderMasterUpdateLogModel log_model = new TenderMasterUpdateLogModel();

                        JObject obj = JObject.Parse(ResponceString);
                        responce_model = obj?.ToObject<AwardedTenderAPIResponce>();

                        log_model.ResponceText = "";

                        if (responce_model != null && responce_model?.data?.Count > 0)
                        {
                            log_model.AddedRecordCount = responce_model.data.Count;

                            string divisionCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.Division)?.FirstOrDefault()?.Id ?? "";
                            string districtCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.District)?.FirstOrDefault()?.Id ?? "";
                            string departmentCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.Department)?.FirstOrDefault()?.Id ?? "";
                            string userGroupCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.UserGroup)?.FirstOrDefault()?.Id ?? "";

                            string ContractorUserGroupId = _settingDAL.Configuration_Get(Value: UserGroupConst.Contractor, CategoryId: userGroupCategoryId)?.FirstOrDefault()?.Id ?? "";

                            responce_model.data.ForEach(x =>
                            {
                                if (!string.IsNullOrWhiteSpace(x.dept_name))
                                {
                                    x.dept_id_time = _settingDAL.Configuration_Get(Code: x.dept_code.Trim(), CategoryId: departmentCategoryId)?.FirstOrDefault()?.Id ?? "";
                                    if (string.IsNullOrEmpty(x.dept_id_time))
                                    {
                                        ConfigurationModel con = new ConfigurationModel()
                                        {
                                            Id = Guid.NewGuid().ToString(),
                                            CategoryId = departmentCategoryId,
                                            Value = x.dept_name.Trim(),
                                            Code = x.dept_code,
                                            IsActive = true,
                                            SavedBy = "",
                                            SavedByUserName = "System",
                                            SavedDate = DateTime.Now,
                                        };

                                        x.dept_id_time = _settingDAL.Configuration_SaveUpdate(con);
                                    }
                                }
                            });

                            List<TenderMasterModel> tenders = _mapper.Map<List<TenderMasterModel>>(responce_model.data)?.Distinct()?.ToList() ?? new List<TenderMasterModel>();
                            List<GOMasterModel> goList = _mapper.Map<List<GOMasterModel>>(responce_model.data)?.Distinct()?.ToList() ?? new List<GOMasterModel>();
                            if (goList.Count == 0)
                            {
                                responce_model.data.ForEach(x =>
                                {
                                    goList.Add(new GOMasterModel()
                                    {
                                        GONumber = string.IsNullOrWhiteSpace(x.go_number) ? "" : x.go_number,
                                        GODate = string.IsNullOrWhiteSpace(x.go_date) ? default : Convert.ToDateTime(x.go_date),
                                        GOCost = string.IsNullOrWhiteSpace(x.go_amount) ? 0 : Convert.ToDecimal(x.go_amount),
                                        GOName = x.govt_order_name,
                                        GORevisedAmount = string.IsNullOrWhiteSpace(x.go_revised_amount) ? 0 : Convert.ToDecimal(x.go_revised_amount),
                                        GODepartment = x.dept_name,
                                        DepartmentId = x.dept_id_time,
                                        DepartmentCode = x.dept_code,
                                        GOTotalAmount = string.IsNullOrWhiteSpace(x.go_final_amount) ? 0 : Convert.ToDecimal(x.go_final_amount),
                                        IsActive = true
                                    });
                                });

                                goList = goList.Distinct().ToList();
                            }
                            if (goList.Count > 0)
                            {
                                goList.ForEach(go =>
                                {
                                    ConfigurationModel? congig = _settingDAL.Configuration_Get(Code: go.DepartmentCode, CategoryId: departmentCategoryId)?.FirstOrDefault();
                                    if (congig != null)
                                    {
                                        go.DepartmentId = congig.Id;
                                        go.GODepartment = congig.Value;
                                    }
                                });

                                goList = _workDAL.GO_SaveUpdate_Bulk(goList);
                            }
                            if (tenders.Count > 0)
                            {
                                tenders.ForEach(model =>
                                {
                                    model.GoId = goList.Find(x => x.GONumber == model.GoNumber && x.GOName == model.GoName)?.Id ?? "";
                                    model.District = _settingDAL.Configuration_Get(Value: model.DistrictName, CategoryId: districtCategoryId)?.FirstOrDefault()?.Id ?? "";
                                    model.Division = _settingDAL.Configuration_Get(Value: model.DivisionName, CategoryId: divisionCategoryId)?.FirstOrDefault()?.Id ?? "";
                                    model.DepartmentId = _settingDAL.Configuration_Get(Code: model.DepartmentCode, CategoryId: departmentCategoryId)?.FirstOrDefault()?.Id ?? "";
                                });

                                AccountRoleModel role = _settingDAL.Account_Role_Get(IsActive: true, RoleCode: RoleCodeConstants.Contractor).FirstOrDefault() ?? new AccountRoleModel();

                                string defaultPassword = _configuration.GetSection("ContractorDefaultPassword").Value.ToString();
                               

                                tenders.ForEach(tender =>
                                {



                                    AccountUserModel accountUserModel = new AccountUserModel();
                                    accountUserModel.IsActive = true;
                                    accountUserModel.UserId = Guid.NewGuid().ToString();
                                    accountUserModel.RoleId = role.Id;
                                    accountUserModel.FirstName = tender.ContractorName ?? "";
                                    accountUserModel.LastName = "";
                                    accountUserModel.Email = tender.ContractorEmail ?? "";
                                    accountUserModel.Mobile = tender.ContractorMobile ?? "";
                                    accountUserModel.SavedDate = DateTime.Now;
                                    accountUserModel.SavedByUserName = "System";
                                    accountUserModel.DepartmentId = tender.DepartmentId;
                                    accountUserModel.IsContractor = true;
                                    accountUserModel.UserGroup = ContractorUserGroupId;
                                    accountUserModel.UserGroupName = UserGroupConst.Contractor;
                                    
                                    if (string.IsNullOrEmpty(defaultPassword))
                                    {
                                        defaultPassword = StringFunctions.CreateRandomPassword(8);
                                    }
                                    accountUserModel.Password = Utils.EncryptDecrypt.Encrypt(defaultPassword);
                                    accountUserModel.DistrictId = _settingDAL.Configuration_Get(Value: tender.ContractorDistrict, CategoryId: districtCategoryId)?.FirstOrDefault()?.Id ?? "";
                                    accountUserModel.DivisionId = _settingDAL.Configuration_Get(Value: tender.ContractorDivision, CategoryId: divisionCategoryId)?.FirstOrDefault()?.Id ?? "";

                                    AccountUserModel? contractor = _settingDAL.User_Get(MobileNumber: tender.ContractorMobile, Email: tender.ContractorEmail)?.FirstOrDefault();

                                    if (contractor != null)
                                    {
                                        accountUserModel.UserId = contractor.UserId;
                                        if (!string.IsNullOrEmpty(contractor.DivisionId))
                                        {
                                            List<string> divisionIds = contractor.DivisionId.Split(',').ToList();
                                            if (!divisionIds.Contains(tender.Division))
                                            {
                                                divisionIds.Add(tender.Division);
                                            }
                                            accountUserModel.DivisionId = string.Join(',', divisionIds);
                                        }
                                        if (!string.IsNullOrEmpty(contractor.DepartmentId))
                                        {
                                            List<string> departmentIds = contractor.DepartmentId.Split(',').ToList();
                                            if (!departmentIds.Contains(tender.DepartmentId))
                                            {
                                                departmentIds.Add(tender.DepartmentId);
                                            }
                                            accountUserModel.DepartmentId = string.Join(',', departmentIds);
                                        }
                                        tender.ContractorId = _settingDAL.User_SaveUpdate(accountUserModel, true);
                                    }
                                    else
                                    {
                                        List<string> divisionIds = new List<string>();
                                        if (!string.IsNullOrEmpty(accountUserModel.DivisionId) && !divisionIds.Contains(accountUserModel.DivisionId))
                                        {
                                            divisionIds.Add(accountUserModel.DivisionId);
                                        }
                                        if (!string.IsNullOrEmpty(tender.Division) && !divisionIds.Contains(tender.Division))
                                        {
                                            divisionIds.Add(tender.Division);
                                        }
                                        accountUserModel.DivisionId = string.Join(',', divisionIds);

                                        tender.ContractorId = _settingDAL.User_SaveUpdate(accountUserModel, true);
                                        responce.NewContractorList.Add(accountUserModel);
                                    }


                                });

                                DateTime today = DateTime.Today;

                                // last month start
                                DateTime lastMonthStart = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
                                tenders = tenders.Where(t => t.EndDate >= lastMonthStart && t.EndDate <= today).ToList();

                                tenders = _workDAL.Tender_SaveUpdate_Bulk(tenders.Where(i => i.GoId != "")?.ToList() ?? new List<TenderMasterModel>());






                            }
                        }
                        else
                        {
                            log_model.AddedRecordCount = 0;
                        }

                        _workDAL.TenderMaster_Update_Log_Save(log_model);

                    }
                }
                

                else if (message.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("unauthorized");
                }





               // SendSchedulerNotificationEmail("TIME Scheduler - Tender Sync Completed", "Tender data synced with " + responce_model.data.Count + " records successfully at " + startTime);

                int insertedCount = TenderrecordsModel.InsertedTenderCount;
                List<string> insertedNumbers = TenderrecordsModel.InsertedTenderNumbers;

                int updatedCount = TenderrecordsModel.UpdatedTenderCount;
                List<string> updatedNumbers = TenderrecordsModel.UpdatedTenderNumbers;



                SendSchedulerNotificationEmail("TIME Scheduler - Tender Sync Completed",
    $"Tender data synced successfully at {startTime}.\n" +
    $" Newly Inserted: {insertedCount}  records.\n" 
);
                //"TIME Scheduler - Tender Sync Completed",
                //$"Tender data synced successfully added {insertedCount} new records at {startTime}. " 



                return responce;
                
            }
            catch (Exception ex)
            {
                SendSchedulerNotificationEmail("TIME Scheduler - Tender Sync Failed.", $"Error: {ex.Message}");
                throw ex;
            }
        }

        public void FetchDivisionRecords()
        {
            try
            {
                string startTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt");
                SendSchedulerNotificationEmail("TIME Scheduler - Division Sync Started", "Sync Configuration:Division records started successfully at " + startTime);
                DivisionListAPIResponceMode? responce_model = new DivisionListAPIResponceMode();
                HttpResponseMessage message = new HttpResponseMessage();
                string _url = _configuration.GetSection("TenderAPIs").GetSection("District_Division_GET_URL").Value.ToString();
                message = _httpClient.PostAsync(_url, null).Result;
                if (message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string ResponceString = message.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(ResponceString))
                    {
                        TenderMasterUpdateLogModel log_model = new TenderMasterUpdateLogModel();
                        JObject obj = JObject.Parse(ResponceString);
                        responce_model = obj?.ToObject<DivisionListAPIResponceMode>();
                        if (responce_model?.data?.Count > 0)
                        {
                            string divisionCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.Division)?.FirstOrDefault()?.Id ?? "";
                            string districtCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.District)?.FirstOrDefault()?.Id ?? "";
                            string departmentCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.Department)?.FirstOrDefault()?.Id ?? "";
                            ConfigurationModel? department = _settingDAL.Configuration_Get(Value: "General", CategoryId: departmentCategoryId)?.FirstOrDefault();

                            responce_model?.data.ForEach(x =>
                            {
                                string configurationId = "";
                                ConfigurationModel? configuration = _settingDAL.Configuration_Get(Value: x.Division_name, CategoryId: divisionCategoryId)?.FirstOrDefault();
                                if (configuration == null)
                                {
                                    ConfigurationModel model = new ConfigurationModel();
                                    model.CategoryId = divisionCategoryId;
                                    model.Code = x.Division_name.Trim();
                                    model.Id = Guid.NewGuid().ToString();
                                    model.SavedDate = DateTime.Now;
                                    model.Value = x.Division_name.Trim();
                                    model.IsActive = true;
                                    model.DepartmentId = department?.Id ?? "";

                                    configurationId = _settingDAL.Configuration_SaveUpdate(model);
                                }
                                else
                                {
                                    configurationId = configuration.Id;
                                }

                                if (!string.IsNullOrWhiteSpace(configurationId) && x.Districts?.Count > 0)
                                {
                                    x.Districts.ForEach(y =>
                                    {
                                        ConfigurationModel? districtExists = _settingDAL.Configuration_Get(IsActive: true, Value: y.district_name.Trim(), CategoryId: districtCategoryId, ParentConfigurationId: configurationId)?.FirstOrDefault();

                                        if (districtExists is null)
                                        {
                                            ConfigurationModel model = new ConfigurationModel();
                                            model.CategoryId = districtCategoryId;
                                            model.ConfigurationId = configurationId;
                                            model.Code = y.district_code.Trim();
                                            model.Id = Guid.NewGuid().ToString();
                                            model.SavedDate = DateTime.Now;
                                            model.Value = y.district_name.Trim();
                                            model.IsActive = true;
                                            model.DepartmentId = department?.Id ?? "";

                                            string Id = _settingDAL.Configuration_SaveUpdate(model);
                                        }
                                    });
                                }
                            });

                        }
                    }
                }
                else if (message.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("unauthorized");
                }
                SendSchedulerNotificationEmail("TIME Scheduler - Division Sync Completed", "Sync Configuration:Division records updated successfully at " + startTime);
            }
            catch (Exception ex)
            {
                SendSchedulerNotificationEmail("TIME Scheduler - Division Sync Failed {startTime}.", $"Error occurred: {ex.Message}");
                throw ex;
            }
        }
        public void FetchDepartmentRecords()
        {
            string startTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt");
            SendSchedulerNotificationEmail("TIME Scheduler - Departments Sync Started", "Sync Configuration : Departments records started successfully at " + startTime);

            try
            {
                DepartmentAPIResponseModel? responce_model = new DepartmentAPIResponseModel();
                HttpResponseMessage message = new HttpResponseMessage();

                //ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                string _url = _configuration.GetSection("TenderAPIs").GetSection("Department_GET_URL").Value.ToString();

                message = _httpClient.PostAsync(_url, null).Result;
                if (message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string ResponceString = message.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(ResponceString))
                    {
                        TenderMasterUpdateLogModel log_model = new TenderMasterUpdateLogModel();
                        JObject obj = JObject.Parse(ResponceString);
                        responce_model = obj?.ToObject<DepartmentAPIResponseModel>();
                        if (responce_model?.data?.Count > 0)
                        {
                            string departmentCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.Department)?.FirstOrDefault()?.Id ?? "";

                            responce_model.data.Add(new Department() { department_name = "General", department_abv = "GENERAL" });

                            responce_model.data.ForEach(x =>
                            {
                                if (!string.IsNullOrWhiteSpace(x.department_name) && !string.IsNullOrWhiteSpace(x.department_abv))
                                {
                                    string configurationId = "";
                                    ConfigurationModel? configuration = _settingDAL.Configuration_Get(Value: x.department_name.Trim(), CategoryId: departmentCategoryId)?.FirstOrDefault();
                                    if (configuration == null)
                                    {
                                        ConfigurationModel model = new ConfigurationModel();
                                        model.CategoryId = departmentCategoryId;
                                        model.Code = x.department_abv;
                                        model.Id = Guid.NewGuid().ToString();
                                        model.SavedDate = DateTime.Now;
                                        model.Value = x.department_name;
                                        model.IsActive = true;

                                        configurationId = _settingDAL.Configuration_SaveUpdate(model);
                                    }
                                    else
                                    {
                                        configurationId = configuration.Id;
                                    }
                                }

                            });

                        }
                    }
                    SendSchedulerNotificationEmail("TIME Scheduler - Department Sync Completed", "Sync Configuration : Department records updated successfully at " + startTime);
                }
                else if (message.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                   
                    throw new Exception("unauthorized");
                }
            }
            catch (Exception ex)
            {
                SendSchedulerNotificationEmail("TIME Scheduler - Department Sync Failed .", $"Error occurred: {ex.Message}");
                throw ex;
            }
        }


        //Added by indu on 16-07-2025 for getting worktypes and subworktypes to store it in configuration table
        public void FetchWorkTypeRecords()
        {
            try
            {
                string startTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt");
          
                SendSchedulerNotificationEmail("TIME Scheduler - Configurations Sync started", "Syncing configurations records started successfully at " + startTime );
                WorkTypeApiModel? response_model = new WorkTypeApiModel();

                // Accept untrusted SSL (if needed)
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                string _url = _configuration.GetSection("TenderAPIs")["Configuration_Get_URL"];
                HttpResponseMessage message = _httpClient.GetAsync(_url).Result;

                if (message.StatusCode == HttpStatusCode.OK)
                {
                    string responseString = message.Content.ReadAsStringAsync().Result;

                    if (!string.IsNullOrEmpty(responseString))
                    {
                        JObject obj = JObject.Parse(responseString);
                        response_model = obj.ToObject<WorkTypeApiModel>();

                        if (response_model?.data?.WorkTypeDetails?.Count > 0)
                        {
                            string workTypeCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.WorkType)?.FirstOrDefault()?.Id ?? "";
                            string subWorkTypeCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.SubWorkType)?.FirstOrDefault()?.Id ?? "";
                            string serviceTypeCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.ServiceType)?.FirstOrDefault()?.Id ?? "";
                            string categoryTypeCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.CategoryType)?.FirstOrDefault()?.Id ?? "";


                            foreach (var workType in response_model.data.WorkTypeDetails)
                            {
                                if (string.IsNullOrWhiteSpace(workType.work_type_main)) continue;

                                // 1. Insert WorkType if not exists
                                string workTypeId = _settingDAL.Configuration_Get(Value: workType.work_type_main.Trim(), CategoryId: workTypeCategoryId)?.FirstOrDefault()?.Id;

                                if (string.IsNullOrEmpty(workTypeId))
                                {
                                    ConfigurationModel model = new ConfigurationModel
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        CategoryId = workTypeCategoryId,
                                        Code = workType.work_type_main_id?.Trim(),
                                        Value = workType.work_type_main?.Trim(),
                                        IsActive = true,
                                        SavedDate = DateTime.Now
                                    };

                                    workTypeId = _settingDAL.Configuration_SaveUpdate(model);
                                    Console.WriteLine($"Inserted WorkType: {model.Value}");
                                }

                                // 2. Insert SubWorkTypes
                                if (!string.IsNullOrEmpty(workTypeId) && !string.IsNullOrWhiteSpace(workType.work_type_sub))
                                {
                                    var existingSub = _settingDAL.Configuration_Get(
                                        IsActive: true,
                                        Value: workType.work_type_sub.Trim(),
                                        CategoryId: subWorkTypeCategoryId,
                                        ParentConfigurationId: workTypeId
                                    )?.FirstOrDefault();

                                    if (existingSub == null)
                                    {
                                        ConfigurationModel subModel = new ConfigurationModel
                                        {
                                            Id = Guid.NewGuid().ToString(),
                                            CategoryId = subWorkTypeCategoryId,
                                            ConfigurationId = workTypeId, // Parent = WorkType
                                            Code = workType.work_type_sub_code?.Trim(),
                                            Value = workType.work_type_sub?.Trim(),
                                            IsActive = true,
                                            SavedDate = DateTime.Now
                                        };

                                        _settingDAL.Configuration_SaveUpdate(subModel);
                                        Console.WriteLine($"Inserted SubWorkType: {subModel.Value} under {workType.work_type_main}");
                                    }
                                }

                            }

                            if (response_model?.data?.ServiceType?.Count > 0)
                            {
                                foreach (var service in response_model.data.ServiceType)
                                {
                                    if (string.IsNullOrWhiteSpace(service.service_type_main)) continue;

                                    var existing = _settingDAL.Configuration_Get(Value: service.service_type_main.Trim(), CategoryId: serviceTypeCategoryId)?.FirstOrDefault();

                                    if (existing == null)
                                    {
                                        ConfigurationModel model = new ConfigurationModel
                                        {
                                            Id = Guid.NewGuid().ToString(),
                                            CategoryId = serviceTypeCategoryId,
                                            Code = service.service_type_main_id?.Trim(),
                                            Value = service.service_type_main?.Trim(),
                                            IsActive = true,
                                            SavedDate = DateTime.Now
                                        };

                                        _settingDAL.Configuration_SaveUpdate(model);
                                        Console.WriteLine($"Inserted ServiceType: {model.Value}");
                                    }
                                }
                            }

                            // 3. CategoryType
                            if (response_model?.data?.CategoryType?.Count > 0)
                            {
                                foreach (var cat in response_model.data.CategoryType)
                                {
                                    if (string.IsNullOrWhiteSpace(cat.category_type_main)) continue;

                                    var existing = _settingDAL.Configuration_Get(Value: cat.category_type_main.Trim(), CategoryId: categoryTypeCategoryId)?.FirstOrDefault();

                                    if (existing == null)
                                    {
                                        ConfigurationModel model = new ConfigurationModel
                                        {
                                            Id = Guid.NewGuid().ToString(),
                                            CategoryId = categoryTypeCategoryId,
                                            Code = cat.category_type_main_id?.Trim(),
                                            Value = cat.category_type_main?.Trim(),
                                            IsActive = true,
                                            SavedDate = DateTime.Now
                                        };

                                        _settingDAL.Configuration_SaveUpdate(model);
                                        Console.WriteLine($"Inserted CategoryType: {model.Value}");
                                    }
                                }
                            }
                        
                    }
                    }
                    SendSchedulerNotificationEmail("TIME Scheduler - Configurations Sync Completed", "Syncing configurations records updated successfully at " + startTime);
                }
                else if (message.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Unauthorized access to configuration API");
                }
            }
            catch (Exception ex)
            {
                SendSchedulerNotificationEmail("TIME Scheduler - Configurations Sync Failed ", $"Error occurred: {ex.Message}");
                throw ex; // Log this if you have a logger
            }
        }

        //Written by indu on 22-10-2025 for scheduler emailnotification
        public void SendSchedulerNotificationEmail(string subject, string body)
        {
            try
            {
                // Get email addresses from appsettings.json
                var recipients = _configuration.GetSection("SchedulerSettings:NotificationEmails").Get<List<string>>();

                if (recipients == null || recipients.Count == 0)
                {
                    Log.Warning("No scheduler notification email addresses configured.");
                    return;
                }

                CurrentUserModel systemUser = new CurrentUserModel
                {
                    UserName = "System",
                    UserId = ""
                };

                EmailModel mail = new EmailModel
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                    SavedDate = DateTime.Now, 
                    SavedBy = "System",
                    SavedByUserName = "System"
                };

                _generalBAL.SendMessage(new List<EmailModel> { mail }, new List<SMSModel>(), systemUser);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to send scheduler notification email: {subject}");
            }
        }



        #endregion API Call

        #region Comment
        public string Comment_SaveUpdate(CommentMasterModel model)
        {
            return _workDAL.Comment_SaveUpdate(model);
        }
        public List<CommentMasterModel> Comment_Get(string Type = "", string TypeId = "", string CommentsFrom = "", string ParentId = "")
        {
            return _workDAL.Comment_Get(Type, TypeId, CommentsFrom, ParentId);
        }
        public List<CommentMasterModel> Comment_Get(CommentFilterModel model, out int TotalCount)
        {
            return _workDAL.Comment_Get(model, out TotalCount);
        }
        #endregion Comment

        #region Dashboard

        // Modified by Indu on 15/04/2025 - Added summary by division/ summay by division/district 
        public TenderChartModel DashboardGet_TenderChart(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            TenderChartModel result = new TenderChartModel();

            List<TenderWorkBaseModel> tenderList = _workDAL.DashboardGet_TenderChart(model, RoleCode, UserId, RoleId);
            List<ConfigurationModel> list = new();
            if (model.SelectionType == "DIVISION" || model.SelectionType == "SUMMARY_BY_DIVISION")
            {
                string divisionCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.Division)?.FirstOrDefault()?.Id ?? "";
                list = _settingDAL.Configuration_Get(CategoryId: divisionCategoryId).OrderBy(o => o.Value).ToList();
                result.Labels = list.Select(o => o.Value).ToList();
            }

            else if (model.SelectionType == "WORK_TYPE")
            {
                string WorkTypeCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.WorkType)?.FirstOrDefault()?.Id ?? "";
                list = _settingDAL.Configuration_Get(CategoryId: WorkTypeCategoryId).OrderBy(o => o.Value).ToList();
                result.Labels = list.Select(o => o.Value).ToList();
            }

            //add by vijay (23-10-2025) for scheme tenderchart

            else if (model.SelectionType == "SUMMARY_BY_SCHEME")
            {
                // Fetch all active schemes from DB
                var schemeList = _settingDAL.Scheme_Category_Get(IsActive: true)
                   .OrderBy(o => o.SchemeName)
                   .ToList();

                // Group and clean up scheme data
                list = _settingDAL.Scheme_Category_Get(IsActive: true);



                result.Labels = list.Select(o => o.SchemeName).ToList();
            }



            else
            {
                string districtCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.District)?.FirstOrDefault()?.Id ?? "";
                list = _settingDAL.Configuration_Get(CategoryId: districtCategoryId).OrderBy(o => o.Value).ToList();
                result.Labels = list.Select(o => o.Value).ToList();
            }
            List<ColorCodeConstModel> colors = ChartColorConst.Get();
            int i = 1;
            model.Year?.OrderBy(o => o)?.ToList()?.ForEach(year =>
            {
                TenderChartDatasetModel dataSet = new TenderChartDatasetModel();
                dataSet.Label = year;
                dataSet.BackgroundColor = colors?.Find(x => x.I == i)?.ColorCode ?? "#e996ba";
                dataSet.BorderColor = colors?.Find(x => x.I == i)?.ColorCode ?? "#e996ba";
                dataSet.BorderWidth = 2;
                dataSet.BorderRadius = 3;

                DateTime StartDate = new DateTime(Convert.ToInt32(year), 1, 1).Date;
                DateTime EndDate = new DateTime(Convert.ToInt32(year), 12, 31).AddDays(1).AddTicks(-1);

                if (model.SelectionType == "DIVISION" || model.SelectionType == "SUMMARY_BY_DIVISION")
                {
                    foreach (ConfigurationModel division in list)
                    {
                        long value = 0;
                        if (string.Equals(model.CostOrCount, "count", StringComparison.CurrentCultureIgnoreCase))
                        {
                            value = tenderList.Where(x => x.Division == division.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Count();
                        }
                        else
                        {
                            value = Convert.ToInt64(tenderList.Where(x => x.Division == division.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Select(y => y.TenderAmount).Sum());
                        }
                        dataSet.Data.Add(value);
                    }
                }

                else if (model.SelectionType == "WORK_TYPE")
                {
                    foreach (ConfigurationModel division in list)
                    {
                        long value = 0;
                        if (string.Equals(model.CostOrCount, "count", StringComparison.CurrentCultureIgnoreCase))
                        {
                            value = tenderList.Where(x => x.WorkTypeId == division.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Count();
                        }
                        else
                        {
                            value = Convert.ToInt64(tenderList.Where(x => x.WorkTypeId == division.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Select(y => y.TenderAmount).Sum());
                        }
                        dataSet.Data.Add(value);
                    }
                }

                else if (model.SelectionType == "SUMMARY_BY_SCHEME")
                {
                    foreach (ConfigurationModel division in list)
                    {
                        long value = 0;


                        if (string.Equals(model.CostOrCount, "count", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (!division.SchemeId.Equals("Others", StringComparison.OrdinalIgnoreCase))
                            {
                                value = tenderList.Where(x => x.SchemeId == division.SchemeId && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Count();
                            }
                            else
                            {
                                value = Convert.ToInt64(
                                    tenderList.Where(x => string.IsNullOrEmpty(x.SchemeId) && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Count());

                            }


                        }

                        else
                        {
                            if (!division.SchemeId.Equals("Others", StringComparison.OrdinalIgnoreCase))
                            {

                                value = Convert.ToInt64(tenderList.Where(x => x.SchemeId == division.SchemeId && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Select(y => y.TenderAmount).Sum());
                            }
                            else
                            {
                                value = Convert.ToInt64(tenderList.Where(x => string.IsNullOrEmpty(x.SchemeId) && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Select(y => y.TenderAmount).Sum());

                            }

                        }

                        dataSet.Data.Add(value);

                    }
                }


                else
                {
                    foreach (ConfigurationModel district in list)
                    {
                        long value = 0;
                        if (string.Equals(model.CostOrCount, "count", StringComparison.CurrentCultureIgnoreCase))
                        {
                            value = tenderList.Where(x => x.District == district.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Count();
                        }
                        else
                        {
                            value = Convert.ToInt64(tenderList.Where(x => x.District == district.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Select(y => y.TenderAmount).Sum());
                        }
                        dataSet.Data.Add(value);
                    }

                }
                result.Datasets.Add(dataSet);

                i++;
            });


            return result;
        }

        // Modified by Indu on 23/06/2025 - Added summary by division/ summay by division/district 
        public TenderChartModel DashboardGet_MbookChart(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            TenderChartModel result = new TenderChartModel();

            List<MbookBaseModel> mbookList = _workDAL.DashboardGet_MbookChart(model, RoleCode, UserId, RoleId);

            List<ConfigurationModel> list = new();
            if (model.SelectionType == "DIVISION" || model.SelectionType == "SUMMARY_BY_DIVISION")
            {
                string divisionCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.Division)?.FirstOrDefault()?.Id ?? "";
                list = _settingDAL.Configuration_Get(CategoryId: divisionCategoryId).OrderBy(o => o.Value).ToList();
                result.Labels = list.Select(o => o.Value).ToList();
            }
           
            else if (model.SelectionType == "WORK_TYPE")
            {
                string WorkTypeCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.WorkType)?.FirstOrDefault()?.Id ?? "";
                list = _settingDAL.Configuration_Get(CategoryId: WorkTypeCategoryId).OrderBy(o => o.Value).ToList();
                result.Labels = list.Select(o => o.Value).ToList();
            }
            else
            {
                string districtCategoryId = _settingDAL.Configuration_Category_Get(ConfigurationCategory.District)?.FirstOrDefault()?.Id ?? "";
                list = _settingDAL.Configuration_Get(CategoryId: districtCategoryId).OrderBy(o => o.Value).ToList();
                result.Labels = list.Select(o => o.Value).ToList();
            }

            List<ColorCodeConstModel> colors = ChartColorConst.Get();
            int i = 1;
            model.Year?.OrderBy(o => o)?.ToList()?.ForEach(year =>
            {
                TenderChartDatasetModel dataSet = new TenderChartDatasetModel();
                dataSet.Label = year;
                dataSet.BackgroundColor = colors?.Find(x => x.I == i)?.ColorCode ?? "#e996ba";
                dataSet.BorderColor = colors?.Find(x => x.I == i)?.ColorCode ?? "#e996ba";
                dataSet.BorderWidth = 2;
                dataSet.BorderRadius = 3;

                DateTime StartDate = new DateTime(Convert.ToInt32(year), 1, 1);
                DateTime EndDate = new DateTime(Convert.ToInt32(year), 12, 31).AddDays(1).AddTicks(-1);

                if (model.SelectionType == "DIVISION" || model.SelectionType == "SUMMARY_BY_DIVISION")
                {
                    foreach (ConfigurationModel division in list)
                    {
                        long value = 0;
                        if (string.Equals(model.CostOrCount, "count", StringComparison.CurrentCultureIgnoreCase))
                        {
                            value = mbookList.Where(x => x.DivisionId == division.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Count();
                        }
                        else
                        {
                            value = Convert.ToInt64(mbookList.Where(x => x.DivisionId == division.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Select(y => y.MbookAmount).Sum());
                        }
                        dataSet.Data.Add(value);
                    }
                }
                

                else if (model.SelectionType == "WORK_TYPE")
                {
                    foreach (ConfigurationModel division in list)
                    {
                        long value = 0;
                        if (string.Equals(model.CostOrCount, "count", StringComparison.CurrentCultureIgnoreCase))
                        {
                            value = mbookList.Where(x => x.WorkTypeId == division.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Count();
                        }
                        else
                        {
                            value = Convert.ToInt64(mbookList.Where(x => x.WorkTypeId == division.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Select(y => y.MbookAmount).Sum());
                        }
                        dataSet.Data.Add(value);
                    }
                }
                else
                {
                    foreach (ConfigurationModel district in list)
                    {
                        long value = 0;
                        if (string.Equals(model.CostOrCount, "count", StringComparison.CurrentCultureIgnoreCase))
                        {
                            value = mbookList.Where(x => x.DistrictId == district.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Count();
                        }
                        else
                        {
                            value = Convert.ToInt64(mbookList.Where(x => x.DistrictId == district.Id && (x.StartDate.Year.ToString() == year || x.EndDate.Year.ToString() == year)).Select(y => y.MbookAmount).Sum());
                        }
                        dataSet.Data.Add(value);
                    }
                }
                result.Datasets.Add(dataSet);

                i++;
            });

            return result;
        }
        public DashboardRecordCountCardModel DashboardGetCountData(DashboardFilterModel model, string RoleCode, string UserGroupName, string UserId, string RoleId)
        {
            return _workDAL.DashboardGetCountData(model, RoleCode, UserGroupName, UserId, RoleId);
        }
        #endregion Dashboard

        #region Activity
        public List<WorkActivityModel> Work_Activity_Get(WorkActivityModel model)
        {
            return _workDAL.Work_Activity_Get(model);
        }
        public List<WorkActivityModel> Work_Activity_Get_By_Ids(ActivityFilterModel model, out int TotalCount)
        {
            return _workDAL.Work_Activity_Get_By_Ids(model, out TotalCount);
        }
        public string Work_Activity_SaveUpdate(WorkActivityModel model)
        {
            return _workDAL.Work_Activity_SaveUpdate(model);
        }
        #endregion Activity

        #region Alert
        public List<AlertTenderModel> Alert_GetWork()
        {
            return _workDAL.Alert_GetWork();
        }
        public List<AlertTenderMilestoneModel> Alert_GetWorkMilestone(string TenderId = "", string WorkId = "")
        {
            return _workDAL.Alert_GetWorkMilestone(TenderId, WorkId);
        }
        public List<AlertTenderMBookModel> Alert_GetWorkMbook(string TenderId = "", string WorkId = "")
        {
            return _workDAL.Alert_GetWorkMbook(TenderId, WorkId);
        }
        public List<AlertMasterModel> Alert_Get(AlertFilterModel model, out int TotalCount)
        {
            return _workDAL.Alert_Get(model, out TotalCount);
        }
        #endregion Alert

        #region Background Mail Process
        public void SendMessage(List<EmailModel> email, List<SMSModel> sms)
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
                            DoBackgroundWork(_smsHelper, _mailHelper, email, sms);
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
        private void DoBackgroundWork(ISMSHelper smsHelper, IMailHelper mailHelper, List<EmailModel> email, List<SMSModel> sms)
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
        #endregion Background Mail Process


        public void InsertLog()
        {
            TenderMasterUpdateLogModel log_model = new TenderMasterUpdateLogModel();
            log_model.ResponceText = "Test";
            log_model.CreatedDate = DateTime.Now;
            log_model.AddedRecordCount = 0;
            _workDAL.TenderMaster_Update_Log_Save(log_model);
        }

        public List<TenderDataModel> GetTender()
        {
            return _workDAL.GetTenderData();
        }
        public List<TenderDataModel> GetTenderbyDivision(string Division)
        {
            return _workDAL.GetTenderData_ByDivision(Division);
        }
        public List<TenderDataModel> GetTenderbyContractor(string Contractor)
        {
            return _workDAL.GetTenderData_ByContractor(Contractor);
        }
        // Modified by indu for dahsboard record counts
        public List<DashboardDivisionCountModel> Dashboard_Dvision_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            return _workDAL.Dashboard_Dvision_Count(model, RoleCode, UserId, RoleId);
        }
        public  (List<DashboardCameraModel> Data, int TotalCount) Dashboard_LiveStreaming(DashboardCameraFilterModel model)
        {
            return _workDAL.Dashboard_LiveStreaming(model);
        }
        public List<GetcameraStatusCountModel> Dashboard_CameraCount(GetcameraStatusCountModel model)
        {
            return _workDAL.Dashboard_CameraCount(model);
        }
       

        public List<DashboardCameraModel> Dashboard_CameraReport(DashboardCameraFilterModel model, out int TotalCount)
        {
            return _workDAL.Dashboard_CameraReport(model, out TotalCount);
        }
        //public bool IsRtspStreamValid(string rtspUrl)
        //{
        //    try
        //    {
        //        var psi = new ProcessStartInfo
        //        {
        //            FileName = @"C:\Users\Indu\Downloads\ffmpeg-8.0-essentials_build\ffmpeg-8.0-essentials_build\bin\ffprobe.exe",
        //            Arguments = $"-rtsp_transport tcp -timeout 2000000 -i \"{rtspUrl}\" -show_streams",
        //            RedirectStandardError = true,
        //            RedirectStandardOutput = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        };

        //        using var process = Process.Start(psi);

        //        // Wait max 3 seconds
        //        process.WaitForExit(2500);

        //        string output = process.StandardOutput.ReadToEnd();
        //        string error = process.StandardError.ReadToEnd();

        //        string total = output + error;

        //        // ffprobe prints codec info into STDERR, so search both for indicators
        //        if (total.Contains("Video:") || total.Contains("h264") || total.Contains("H.264") || total.Contains("hevc"))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}


   


        

        private static readonly HttpClient _client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        public async Task<Dictionary<string, bool>> GetRtspStatusFromMediaMTX()
        {
            var auth = Convert.ToBase64String(
                Encoding.ASCII.GetBytes("apiuser:StrongPassword123"));

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", auth);

            var json = await _client.GetStringAsync(
                "http://3.108.9.185:9997/v3/paths/list");

            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("items")
                .EnumerateArray()
                .ToDictionary(
                    x => x.GetProperty("name").GetString()!,
                    x => x.GetProperty("ready").GetBoolean()
                );
        }

        public string GetPathFromLiveUrl(string liveUrl)
        {
            if (string.IsNullOrWhiteSpace(liveUrl))
                return string.Empty;

            return new Uri(liveUrl).AbsolutePath.Trim('/');
        }


        #region Filters

        public List<GetAllDivisionModel> GetAllDivision(DashboardCameraModel model)
        {
            return _workDAL.GetAllDivision(model);
        }


        public List<GetAllDistrictModel> GetAllDistrict(DashboardCameraModel model)
        {
            return _workDAL.GetAllDistrict(model);
        }




        public List<WorkTypeModel> GetAllMainCategory(DashboardCameraModel model)
        {
            return _workDAL.GetAllMainCategory(model);
        }



        public List<SubWorkTypeModel> GetAllSubCategory(DashboardCameraModel model)
        {
            return _workDAL.GetAllSubCategory(model);
        }



        public List<WorkStatusModel> GetAllWorkStatus(DashboardCameraModel model)
        {
            return _workDAL.GetAllWorkStatus(model);
        }


        public List<TenderNumberModel> GetAllTenderNumber(DashboardCameraModel model)
        {
            return _workDAL.GetAllTenderNumber(model);
        }

        #endregion Filters

        public List<DashboardDivisionCountModel> Dashboard_Division_district_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            return _workDAL.Dashboard_Dvision_district_Count(model, RoleCode, UserId, RoleId);
        }
        public List<DashboardDivisionCountModel> Dashboard_mbook_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            return _workDAL.GetDivision_District_Mbook_Count(model, RoleCode, UserId, RoleId);
        }
        public List<DashboardDivisionCountModel> GetDivision_Mbook_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            return _workDAL.GetDivision_Mbook_Count(model, RoleCode, UserId, RoleId);
        }

        public List<DashboardDivisionCountModel> Dashboard_Scheme_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            return _workDAL.Dashboard_Scheme_Count(model, RoleCode, UserId, RoleId);
        }

        public string IsTenderverified(string TenderId)
        {
            return _workDAL.IsTenderverified(TenderId);
        }
        

    }
}
