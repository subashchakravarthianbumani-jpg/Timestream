using Model.DomainModel;
using Model.ViewModel;

namespace BAL.Interface
{
    public interface IWorkBAL
    {
        #region GO
        public List<GOMasterModel> GO_Get(bool IsActive = true, string Id = "", string GONumber = "", string LocalGONumber = "", string DepartmentId = "");
        public List<GOMasterModel> GO_Get(GoFilterModel model, out int TotalCount);
        public string GO_SaveUpdate(GOMasterModel model);
        public List<GOReportViewModel> GO_Report_Get(GoFilterModel model, out int TotalCount);

        #endregion GO

        #region Tender
        public List<TenderMasterModel> Tender_Get(bool IsActive = true, string Id = "", string TenderNumber = "", string LocalTenderNumber = "", string GOId = "", string DepartmentId = "");
        public List<TenderMasterModel> Tender_Get(TenderFilterModel model, out int TotalCount);
        public string Tender_SaveUpdate(TenderMasterModel model);
        public string Tender_Update_Amount(TenderAmountUpdateModel model);
        public List<TenderRelatedIdModel> Get_TenderRelatedIds(string TenderId);
        public List<string> Tender_Ids_Get_ByContractor(string ContractorId);
        public List<TenderDataModel> GetTenderbyContractor(string Contractor);
             public List<TenderDataModel> GetTenderbyDivision(string Division);
            public List<TenderDataModel> GetTender();
        public string IsTenderverified(string TenderId);


        #endregion Tender

        #region Work
        public List<WorkMasterModel> Work_Get(bool IsActive = true, string Id = "", string TenderId = "", string WorkNumber = "");
        public List<WorkMasterModel> Work_Get_All(bool IsActive = true, string Id = "", string TenderId = "", string WorkNumber = "");
        public List<WorkMasterModel> Work_Get(WorkFilterModel model, out int TotalCount);
        public string Work_SaveUpdate(WorkMasterModel model);

        #endregion Work

        #region Work Template
        public List<WorkTemplateMasterModel> Work_Template_Get(bool IsActive = true, string Id = "", string WorkId = "", string WorkTypeId = "", string TemplateId = "");
        public string Work_Template_SaveUpdate(WorkTemplateMasterModel model);
        public string DeleteWorkTemplate(string WorkId, string SavedBy, string SavedByUserName, DateTime SavedDate);
        public string UpdateDatedifference();
        public string Work_Template_Submit(WorkTemplateMasterModel model);

        #endregion Work Template

        #region Work Template Milestone
        public List<WorkTemplateMilestoneMasterModel> Work_Template_Milestone_Master_Get(bool IsActive = true, string Id = "", string WorkTemplateId = "", string WorkId = "",
            string TenderId = "", string DivisionId = "", string MilestoneStatusId = "", string PaymentStatusId = "");
        public List<MilestoneReportModel> Milestone_Get(MilestoneFilterModel model, out int TotalCount);
        public string Work_Template_Milestone_Master_SaveUpdate(WorkTemplateMilestoneMasterModel model);
        public string SetWorkCompletionDate(string WorkTemplateId);
        public string Work_Template_Milestone_Master_Delete_All(WorkTemplateMilestoneMasterModel model);
        public string Update_Milestone_Completed_Percentage(MilestoneUpdateModel model);

        #endregion Work Template Milestone

        #region M-Book
        public List<MBookMasterModel> Work_MBook_Get(MBookFilterModel model, string UserId, string RoleCode, string UserGroupName, string DivisionId, out int TotalCount);
        public List<MBookReportModel> Work_MBook_Report_Get(MBookReportFilterModel model, out int TotalCount);
        public MBookMasterModel? Work_MBook_GetById(string MBookId);
        public List<MBookMasterModel> Work_MBook_Get(bool IsActive = true, string Id = "", string WorkTemplateMilestoneId = "", string ActionableRoleId = "", string DivisionId = "", string StatusId = "", string WorkId = "", string TenderId = "");
        public string Work_MBook_SaveUpdate(MBookMasterModel model, string message = "", string subject = "");
        public string Work_Approve_MBook(MbookApprovalModel model, string RoleId);
        public List<string> Mbook_Id_Get_ByContractor(string ContractorId);
        public List<SelectListItem> MbookFileUploadTypeList();

        #endregion M-Book

        #region M-Book Approval_History
        public List<MBookApprovalHistoryViewModel> Work_MBook_Approval_History_Get(string Id = "", string MBookId = "");
        public string Work_MBook_Approval_History_SaveUpdate(MBookApprovalHistoryModel model);
        #endregion M-Book Approval_History

        #region API Call
        public TenderDataIntegrationResponceModel FetchAwardedTenders(DateTime fromDate, DateTime ToDate);
        public void FetchDivisionRecords();
        public void FetchDepartmentRecords();
        public void FetchWorkTypeRecords();
        public void SendSchedulerNotificationEmail(string subject, string body);

        #endregion API Call

        #region Comment
        public string Comment_SaveUpdate(CommentMasterModel model);
        public List<CommentMasterModel> Comment_Get(string Type = "", string TypeId = "", string CommentsFrom = "", string ParentId = "");
        public List<CommentMasterModel> Comment_Get(CommentFilterModel model, out int TotalCount);
        #endregion Comment

        #region Dashboard
        public TenderChartModel DashboardGet_TenderChart(DashboardFilterModel model, string RoleCode, string UserId, string RoleId);
        public TenderChartModel DashboardGet_MbookChart(DashboardFilterModel model, string RoleCode, string UserId, string RoleId);
        public DashboardRecordCountCardModel DashboardGetCountData(DashboardFilterModel model, string RoleCode, string UserGroupName, string UserId, string RoleId);
        public List<DashboardDivisionCountModel> Dashboard_Dvision_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId);
        public List<DashboardDivisionCountModel> Dashboard_Division_district_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId);
        public List<DashboardDivisionCountModel> Dashboard_Scheme_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId);
        public List<DashboardDivisionCountModel> Dashboard_mbook_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId);
        public List<DashboardDivisionCountModel> GetDivision_Mbook_Count(DashboardFilterModel model, string RoleCode, string UserId, string RoleId);
        #endregion Dashboard

        #region Activity
        public List<WorkActivityModel> Work_Activity_Get(WorkActivityModel model);
        public List<WorkActivityModel> Work_Activity_Get_By_Ids(ActivityFilterModel model, out int TotalCount);
        public string Work_Activity_SaveUpdate(WorkActivityModel model);
        #endregion Activity

        #region Alert
        public List<AlertTenderModel> Alert_GetWork();
        public List<AlertTenderMilestoneModel> Alert_GetWorkMilestone(string TenderId = "", string WorkId = "");
        public List<AlertTenderMBookModel> Alert_GetWorkMbook(string TenderId = "", string WorkId = "");
        public List<AlertMasterModel> Alert_Get(AlertFilterModel model, out int TotalCount);
        #endregion Alert


        public (List<DashboardCameraModel> Data, int TotalCount) Dashboard_LiveStreaming(DashboardCameraFilterModel model);

        //public bool IsRtspStreamValid(string rtspUrl);
        Task<Dictionary<string, bool>> GetRtspStatusFromMediaMTX();
       
        public string GetPathFromLiveUrl(string liveUrl);
        public List<DashboardCameraModel> Dashboard_CameraReport(DashboardCameraFilterModel model, out int TotalCount);
        public List<GetcameraStatusCountModel> Dashboard_CameraCount(GetcameraStatusCountModel model);
        public string UpdateCameraDatabase();


        public List<GetAllDivisionModel> GetAllDivision(DashboardCameraModel model);


        public List<GetAllDistrictModel> GetAllDistrict(DashboardCameraModel model);


        public List<WorkTypeModel> GetAllMainCategory(DashboardCameraModel model);


        public List<SubWorkTypeModel> GetAllSubCategory(DashboardCameraModel model);


        public List<WorkStatusModel> GetAllWorkStatus(DashboardCameraModel model);


        public List<TenderNumberModel> GetAllTenderNumber(DashboardCameraModel model);


        public void InsertLog();


    }
}
