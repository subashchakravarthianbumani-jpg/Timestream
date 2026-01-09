using DAL;
using Model.DomainModel;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.UtilModels;

namespace BAL.Interface
{
    public interface IGeneralBAL
    {
        public string FileMaster_SaveUpdate(FileMasterModel model);
        public List<FileMasterModel> FileMaster_Get(bool IsActive, string Id = "", string Type = "", string TypeId = "", string SavedFileName = "");
        public Task<byte[]> GetImage(string ImageName);
        public List<AccountUserModel> GetKeyContacts(string UserId, string RoleCode, string DivisionIds, string DepartmentId);
        public void SendMessage(List<EmailModel> email, List<SMSModel> sms, CurrentUserModel user);
        public List<RecordHistoryModel> GetRecordHistory(TableFilterModel model, out int TotalCount);
        public Task<List<AlertMasterModel>> GetAlertsforBGProcess(string CreatedBy, string CreatedByUserName);
        public bool Alert_Resolve(string id, string CreatedBy, string CreatedByUserName, string note);


        #region Mail SMS Log
        public string MailSMSLog_Save(MailSMSLog model);
        public List<MailSMSLog> MailSMSLog_Get(MailSMSLog model);
        #endregion Mail SMS Log
    }
}
