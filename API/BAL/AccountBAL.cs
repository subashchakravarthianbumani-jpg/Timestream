using AutoMapper;
using BAL.Interface;
using DAL;
using Microsoft.Extensions.Configuration;
using Model.DomainModel;
using Model.ViewModel;
using Utils.Interface;

namespace BAL
{
    public class AccountBAL : IAccountBAL
    {
        private readonly AccountDAL _accountDAL;
        private readonly IMapper _mapper;

        public AccountBAL(IMySqlDapperHelper mySqlDapperHelper, IMySqlHelper mySqlHelper, IMapper mapper, IConfiguration configuration)
        {
            _accountDAL = new AccountDAL(mySqlDapperHelper, mySqlHelper, configuration);
            _mapper = mapper;
        }
        public LoginModel? GetLogin(LoginRequestModel model)
        {
            return _accountDAL.GetLogin(model);
        }
        public string SaveLogin(LoginModel model)
        {
            model.AccessToken = Utils.EncryptDecrypt.Encrypt(model.AccessToken);
            model.RefreshToken = Utils.EncryptDecrypt.Encrypt(model.RefreshToken);
            return _accountDAL.SaveLogin(model);
        }
        public string SaveLoginLog(AccountLoginLogModel model)
        {
            return _accountDAL.SaveLoginLog(model);
        }
        public List<AccountLoginLogModel> GetLoginLog(string Id = "", string LoginId = "")
        {
            return _accountDAL.GetLoginLog(Id, LoginId);
        }
        public AccountUserModel? GetUser(bool IsActive = true, string UserId = "", string DistrictId = "", string DivisionId = "", string UserGroup = "", string RoleId = "", string MobileNumber = "")
        {
            AccountUserModel? model = _accountDAL.GetUser(IsActive, UserId,DistrictId, DivisionId, UserGroup, RoleId, MobileNumber);
            if (model != null)
            {
                if (!string.IsNullOrWhiteSpace(model.DivisionId))
                {
                    model.DivisionIdList = model.DivisionId.Split(',').ToList();
                }
                if (!string.IsNullOrWhiteSpace(model.DepartmentId))
                {
                    model.DepartmentIdList = model.DepartmentId.Split(',').ToList();
                }
            }
            return model;
        }
        public string Login_OTP_Save(string UserId, string OTP, string PasswordToken)
        {
            return _accountDAL.Login_OTP_Save(UserId,OTP,PasswordToken);
        }
        public string Login_OTP_Get(string UserId)
        {
            return _accountDAL.Login_OTP_Get(UserId);
        }
        public bool Login_Password_Save(string PasswordToken, string Password, string OTP)
        {
            return _accountDAL.Login_Password_Save(PasswordToken, Password, OTP);
        }


    }
}