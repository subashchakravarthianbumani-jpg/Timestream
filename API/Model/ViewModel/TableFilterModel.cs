using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class TableFilterModel
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string? SearchString { get; set; }
        public ColumnSortingModel? Sorting { get; set; }
        public List<ColumnSearchModel>? ColumnSearch { get; set; }
    }
    public class ColumnSortingModel
    {
        public string FieldName { get; set; } = string.Empty;
        public string Sort { get; set; } = string.Empty;
    }
    public class ColumnSearchModel
    {
        public string FieldName { get; set; } = string.Empty;
        public string SearchString { get; set; } = string.Empty;
    }

    #region GO
    public class GoReportFilterModel : TableFilterModel
    {
        public string? WorkStatus { get; set; }
        public string? GoNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<string>? DepartmentList { get; set; }

        public int? NumberOfTenders { get; set; }
        public int? CompletedWorks { get; set; }
        public int? PendingWorks { get; set; }
        public decimal? PannedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? PaymentValue { get; set; }
        public decimal? TotalValue { get; set; }

    }
    public class GoFilterModel : TableFilterModel
    {
        public GoWhereClauseProperties? Where { get; set; }
        public List<string>? Year { get; set; }
        public List<string>? DepartmentList { get; set; }
        public List<string>? DistrictList { get; set; }
        public List<string>? DivisionList { get; set; }
        public string? DivisionId { get; set; }
        public string? DistrictId { get; set; }
    }

public class GoWhereClauseProperties
    {
        public bool IsActive { get; set; } = true;
        public string Id { get; set; } = string.Empty;
        public string GONumber { get; set; } = string.Empty;
        public string LocalGONumber { get; set; } = string.Empty;
        public string GODate { get; set; } = string.Empty;
        public string GOName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public decimal? GOCost { get; set; }
        public decimal? GORevisedAmount { get; set; }
        public decimal? GOTotalAmount { get; set; }
        public int? NumberOfTenders { get; set; }
        public decimal? PannedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public int? TotalWorkCount { get; set; }
        public int? CompletedWorkCount { get; set; }
        public int? RemainingWorks { get; set; }
    }
    #endregion GO

    #region Tender
    public class TenderFilterModel : TableFilterModel
    {
        public TenderWhereClauseProperties? Where { get; set; }
        public List<string>? DivisionList { get; set; }
        public List<string>? DistrictList { get; set; }
        public List<string>? DepartmentList { get; set; }
        public List<string>? WorkType { get; set; }
        public List<string>? TenderIds { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? QuickFilterCode { get; set; }
        public string? RoleCode { get; set; }
        public string? SelectionType { get; set; }
        public List<string>? Year { get; set; } 
    }
    public class TenderWhereClauseProperties
    {
        public bool IsActive { get; set; } = true;
        public string Id { get; set; } = string.Empty;
        public string TenderNumber { get; set; } = string.Empty;
        public string LocalTenderNumber { get; set; } = string.Empty;
        public string ContractorId { get; set; } = string.Empty;
    }
    #endregion Tender

    #region Work
    public class WorkFilterModel : TableFilterModel
    {
        public WorkWhereClauseProperties? Where { get; set; }
        public string? goNumber { get; set; }
        public string? GoId { get; set; }
        public string? WorkNumber { get; set; }
        public string? WorkTypeId { get; set; }
        public string? SubWorkTypeId { get; set; }
        public string? Strength { get; set; }
        public string? DivisionId { get; set; }
        public string? DistrictId { get; set; }
        public string? Duration { get; set; }
        public string? workStatus { get; set; }
        public string? Status { get; set; }
        public string? Cost { get; set; }
        public string? Delay { get; set; }
        public string Days {  get; set; }= string.Empty;
        public List<string>? TenderIds { get; set; }
        public string? RoleCode { get; set; }
        public string SchemeName {  get; set; } = string.Empty;
        public string go_Package_No {  get; set; } = string.Empty;
        public string? Contractor { get; set; }
        public string? ContractorDistrict { get; set; }
        public string? ContractorCompanyName { get; set; }
        public string? ContractorMobile { get; set; }


        public List<string>? DistrictList { get; set; }
        public List<string>? DivisionList { get; set; }
        public List<string>? StatusList { get; set; }
        public List<string>? DepartmentList { get; set; }
        public List<string>? SchemeList { get; set; }
        public List<string>? PackageList { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }


        public string? MainCategory { get; set; }

        public string? Subcategory { get; set; }
    }
    public class WorkWhereClauseProperties
    {
        public bool IsActive { get; set; } = true;
        public string Id { get; set; } = string.Empty;
        public string TenderId { get; set; } = string.Empty;
        public string WorkNumber { get; set; } = string.Empty;
    }
    #endregion Work

    #region Milestone
    public class MilestoneFilterModel : TableFilterModel
    {
        public string? WorkId { get; set; }
        public string? WorkTypeId { get; set; }
        public string? SubWorkTypeId { get; set; }
        public string? Strength { get; set; }
        public string? DivisionId { get; set; }
        public string? DistrictId { get; set; }
        public string? ApprovalStatusName { get; set; }
        public string? PaymentStatusName { get; set; }
        public string? ApprovalStatusId { get; set; }
        public string? PaymentStatusId { get; set; }
        public string? Cost { get; set; }
        public string? ActualCost { get; set; }

        public List<string>? DistrictList { get; set; }
        public List<string>? DivisionList { get; set; }
        public List<string>? StatusList { get; set; }
        public List<string>? DepartmentList { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public decimal? PannedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? PaymentValue { get; set; }

    }
    #endregion Milestone

    #region M-Book
    public class MBookReportFilterModel : TableFilterModel
    {
        public string? WorkId { get; set; }
        public string? WorkTypeId { get; set; }
        public string? SubWorkTypeId { get; set; }
        public string? Strength { get; set; }
        public string? DivisionId { get; set; }
        public string? DistrictId { get; set; }
        public string? StatusName { get; set; }
        public string? PaymentStatusName { get; set; }
        public string? StatusId { get; set; }
        public string? PaymentStatusId { get; set; }
        public string? ActionableRoleId { get; set; }
        public string? Amount { get; set; }
        public string? ActualAmount { get; set; }
        public List<string>? MbookIds { get; set; }
        public string? RoleCode { get; set; }
        public string? RoleId { get; set; }
        public List<string>? DistrictList { get; set; }
        public List<string>? DivisionList { get; set; }
        public List<string>? StatusList { get; set; }
        public List<string>? DepartmentList { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public decimal? PannedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? PaymentValue { get; set; }

    }
    public class MBookFilterModel : TableFilterModel
    {
        public MBookWhereClauseProperties? Where { get; set; }
        public List<string>? DivisionIds { get; set; }
        public List<string>? TenderIds { get; set; }
        public List<string>? DepartmentIds { get; set; }
        public List<string>? Year { get; set; }
        public string? RoleId { get; set; }
        public string? RoleCode { get; set; }
        public bool MilestonePercentageShouldBeGreaterThan { get; set; } = false;
        public string MBookId { get; set;} = string.Empty;  
        public bool IsForApproval { get; set;} = false;
    }
    public class MBookWhereClauseProperties
    {
        public bool IsActive { get; set; } = true;
    }
    #endregion M-Book

    #region Comment
    public class CommentFilterModel : TableFilterModel
    {
        public CommentWhereClauseProperties? Where { get; set; }
        public List<string>? Ids { get; set; }
        public List<string>? Types { get; set; }
    }
    public class CommentWhereClauseProperties
    {
        public string? Id { get; set; } = string.Empty;
        public string? Type { get; set; } = string.Empty;
        public string? TypeId { get; set; } = string.Empty;
        public string? ParentId { get; set; } = string.Empty;
        public string? CommentsFrom { get; set; } = string.Empty;
        public string? CommentsText { get; set; } = string.Empty;
        public string? SubjectText { get; set; } = string.Empty;
        public string? CommentNumber { get; set; } = string.Empty;
        public string? CreatedByUserName { get; set; } = string.Empty;
        public DateTime? CommentDate { get; set; }
    }
    #endregion Comment

    #region Activity
    public class ActivityFilterModel : TableFilterModel
    {
        public ActivityWhereClauseProperties? Where { get; set; }
        public List<string>? Ids { get; set; }
        public List<string>? Types { get; set; }
    }
    public class ActivityWhereClauseProperties
    {
        public string? Type { get; set; } = string.Empty;
        public string? TypeId { get; set; } = string.Empty;
    }
    #endregion Activity

    #region Dashboard
    public class DashboardFilterModel
    {
        public List<string>? DivisionIds { get; set; }
        public List<string>? DistrictIds { get; set; }
        public string? ContractorId { get; set; }
        public List<string>? DepartmentIds { get; set; }
        public List<string>? Year { get; set; }
        public string? SelectionType { get; set; }
        public string? CostOrCount { get; set; }
    }
    #endregion Dashboard

    #region Alert
    public class AlertFilterModel : TableFilterModel
    {
        public string? RoleId { get; set; }
        public List<string>? DivisionIds { get; set; }
        public List<string>? Types { get; set; }
        public List<string>? TypeIds { get; set; }
    }
    #endregion Alert


    #region Users
    public class UserFilterModel : TableFilterModel
    {
        public UserWhereClauseProperties? Where { get; set; }
    }
    public class UserWhereClauseProperties
    {
        public bool IsActive { get; set; } = true;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
    }
    #endregion



    #region dahsboardcamera
    public class DashboardCameraFilterModel : TableFilterModel
    {
        public WorkWhereClauseProperties? Where { get; set; }
        public List<string>? DivisionIds { get; set; }
        public List<string>? DistrictIds { get; set; }
        public List<string>? DepartmentIds { get; set; }
        public string TenderId { get; set; } = string.Empty;
        public string DivisionName { get; set; } = string.Empty;
        public string WorkStatus { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string TenderNumber { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string rtspUrl { get; set; } = string.Empty;
        public string LiveUrl { get; set; } = string.Empty;
        public string mainCategory { get; set; } = string.Empty;
        public string subcategory { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string tender_final_awarded_value { get; set; } = string.Empty;
        public string TipsTender_Id { get; set; } = string.Empty;
        public string SchemeName { get; set; } = string.Empty;
        public string Go_Package_No { get; set; } = string.Empty;
        public string AwardedDate { get; set; } = string.Empty;
        public string ContractorCompanyName { get; set; } = string.Empty;
       
        public string? WorkCommencementDate { get; set; } 
        public string? WorkCompletionDate { get; set; } 
        public bool IsRtspValid { get; set; } 
        public int rows { get; set; } 
        public int DateDifference { get; set; } 
        public int Total { get; set; } 


    }

    public class DashboardCameraModel 
    {
      
        public List<string>? DivisionIds { get; set; }
        public List<string>? DistrictIds { get; set; }
        public List<string>? DepartmentIds { get; set; }
        public string TenderId { get; set; } = string.Empty;
        public string DivisionName { get; set; } = string.Empty;
        public string WorkStatus { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string TenderNumber { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string rtspUrl { get; set; } = string.Empty;
        public string LiveUrl { get; set; } = string.Empty;
        public string RtmpUrl { get; set; } = string.Empty;
        public string mainCategory { get; set; } = string.Empty;
        public string subcategory { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string tender_final_awarded_value { get; set; } = string.Empty;
        public string TipsTender_Id { get; set; } = string.Empty;
        public string SchemeName { get; set; } = string.Empty;
        public string Go_Package_No { get; set; } = string.Empty;
        public string AwardedDate { get; set; } = string.Empty;
        public string ContractorCompanyName { get; set; } = string.Empty;
        public string? WorkCommencementDate { get; set; }
        public string? WorkCompletionDate { get; set; }
        public bool IsRtspValid { get; set; } = true;
        public bool IsRtspLive { get; set; } = true;
    
        public int rows { get; set; }
        public int DateDifference { get; set; }
      


    }
    public class GetcameraStatusCountModel
    {
        public List<string>? DivisionIds { get; set; }
        public List<string>? DistrictIds { get; set; }
        public string NotStarted { get; set; } = string.Empty;

        public string InProgress { get; set; } = string.Empty;
        public string SlowProgress { get; set; } = string.Empty;
        public string Completed { get; set; } = string.Empty;
        public string StartedButStilled { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
    }


    #endregion dahsboardcamera




    #region FilterModels


    public class  GetAllDivisionModel
    {
        public string Division { get; set; } = string.Empty;

        public string DivisionName { get; set; } = string.Empty;
    }

    public class GetAllDistrictModel
    {
        public string District { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
    }

    public class WorkTypeModel
    {
        public string MainCategory { get; set; } = string.Empty;
    }

    public class SubWorkTypeModel
    {
        public string Subcategory { get; set; } = string.Empty;
    }


    public class WorkStatusModel
    {
        public string WorkStatus { get; set; } = string.Empty;
    }

    public class TenderNumberModel
    {
        public string TenderId { get; set; } = string.Empty;
        public string TenderNumber { get; set; } = string.Empty;
    }
  

    #endregion FilterModels
}