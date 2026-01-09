using DAL;
using Model.DomainModel;
using Model.ViewModel;

namespace BAL.Interface
{
    public interface IAccountBAL
    {
        public LoginModel? GetLogin(LoginRequestModel model);
        public string SaveLogin(LoginModel model);
        public string SaveLoginLog(AccountLoginLogModel model);
        public List<AccountLoginLogModel> GetLoginLog(string Id = "", string LoginId = "");
        public AccountUserModel? GetUser(bool IsActive = true, string UserId = "", string DistrictId = "", string DivisionId = "", string UserGroup = "", string RoleId = "", string MobileNumber = "");
        public string Login_OTP_Save(string UserId, string OTP, string PasswordToken);
        public string Login_OTP_Get(string UserId);
        public bool Login_Password_Save(string PasswordToken, string Password, string OTP);
    }
}
