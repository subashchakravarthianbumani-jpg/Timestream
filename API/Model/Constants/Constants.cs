using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Constants
{
    public static class Constants
    {
        public const string UserId = "UserID";
        public const string LoginId = "LoginID";
        public const string UserName = "UserName";
        public const string Name = "Name";
        public const string RoleId = "RoleId";
        public const string RoleCode = "RoleCode";
        public const string RoleName = "RoleName";
        public const string DistrictId = "DistrictId";
        public const string DivisionId = "DivisionId";
        public const string Mobile = "Mobile";
        public const string DepartmentId = "DepartmentId";
        public const string UserGroup = "UserGroup";
        public const string UserGroupName = "UserGroupName";

        public const string ConfigurationGeneralSchemeId = "dasdsad6546546432424133214";

        public static List<string> StaticRoles = new List<string>() { "CONTRACTOR", "ADM" };
    }

    public static class EmailTemplateCode
    {
        public const string UserCreate = "USERCREATE";
        public const string MBookStatusMail = "MBOOKSTATUSMAIL";
        public const string WorkCompletedMail = "WORKCOMPLETED";
        public const string MilestoneCompletedMail = "MILESTONECOMPLETED";
    }
    public static class UploadTypeCode
    {
        public const string MBookEntryDocument = "MBookEntryDocument";
        public const string MBookTestingDocument = "MBookTestingDocument";
        public const string MBookPhotos = "MBookPhotos";
        public const string MBookVideos = "MBookVideos";
        public const string MBookTypeDoc = "MBookTypeDoc";
        public const string MBookOtherDoc = "MBookOtherDoc";


        public const string Work_LetterOfAcceptance = "LetterOfAcceptance";
        public const string Work_WorkOrder = "WorkOrder";
        public const string Work_AgreementCopy = "AgreementCopy";
        public const string Work_Other = "Other";
    }
    public static class StatusCodeConst
    {
        // Status codes you have to send to Mbook approve reject API is 'APPROVE, RETURN, REJECT'
        public const string Approve = "APPROVE";
        public const string Return = "RETURN";
        public const string Reject = "REJECT";
        public const string Saved = "SAVED";
        public const string Submitted = "SUBMITTED";
        public const string Completed = "COMPLETED";
        public const string PaymentNotInitiated = "PAYMENTNOTINITIATED";
        public const string PaymentInProgress = "PAYMENTINPROGRESS";
        public const string PaymentDone = "PAYMENTDONE";
        public const string Inprogress = "INPROGRESS";
        public const string New = "NEW";   
        public const string NotStarted = "NOTSTARTED";
        public const string SlowProgress = "SLOWPROGRESS";
        public const string StartedbutStilled = "ONHOLD";
    }
    public static class StatusTypeConst
    {
        public const string MBook = "MBook";
        public const string Tender = "Tender";

    }

    public static class UserGroupConst
    {
        public const string Contractor = "Contractor";
        public const string Engineer = "Engineer";
        public const string HQ = "HQ";
        public const string Admin = "ADMIN";
    }

    public static class CommentTypeConst
    {
        public const string Mbook = "MBOOK";
        public const string Tender = "TENDER";
        public const string Milestone = "MILESTONE";
    }

    public static class ChartColorConst
    {
        public const string Color1 = "#a7e996";
        public const string Color2 = "#f7dd91";
        public const string Color3 = "#efbebe";
        public const string Color4 = "#b3daf3";
        public const string Color5 = "#a996e9";

        public static List<ColorCodeConstModel> Get()
        {
            return new List<ColorCodeConstModel>() {

                new ColorCodeConstModel(){ I = 1, ColorCode = Color1 },
                new ColorCodeConstModel(){ I = 2, ColorCode = Color2 },
                new ColorCodeConstModel(){ I = 3, ColorCode = Color3 },
                new ColorCodeConstModel(){ I = 4, ColorCode = Color4 },
                new ColorCodeConstModel(){ I = 5, ColorCode = Color5 }
            };
        }
    }

    public static class WorkActivityMessageConst
    {
        public const string WorkCreated = "Work created";
        public const string TenderAmountIncreased = "Tender value increased";
        public const string LetterOfAcceptance_Upload = "Letter of acceptance uploaded";
        public const string WorkOrder_Upload = "Work order uploaded";
        public const string AgreementCopy_Upload = "Agreement copy uploaded";
        public const string OtherFile_Upload = "Other files uploaded";
    }
}
