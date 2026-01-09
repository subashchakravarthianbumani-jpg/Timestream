using Dapper;
using Microsoft.Extensions.Configuration;
using Model.DomainModel;
using Model.ViewModel;
using MySql.Data.MySqlClient;
using System.Data;
using Utils;
using Utils.Interface;

namespace DAL
{
    public class AccountDAL
    {
        private readonly IMySqlHelper _mySqlHelper;
        private readonly IConfiguration _configuration;
        private readonly IMySqlDapperHelper _mySqlDapperHelper;

        private readonly string connectionId = "Default";
        public AccountDAL(IMySqlDapperHelper mySqlDapperHelper, IMySqlHelper mySqlHelper, IConfiguration configuration)
        {
            _mySqlHelper = mySqlHelper;
            _configuration = configuration;
            _mySqlDapperHelper = mySqlDapperHelper;
        }
        public LoginModel? GetLogin(LoginRequestModel model)
        {
            dynamic @params = new { pUserName = model.UserName, pPassword = model.Password, pEmail = model.UserName };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.QueryFirstOrDefault<LoginModel>(connection, "Account_LoginGet", @params, commandType: CommandType.StoredProcedure);
        }
        public string SaveLogin(LoginModel model)
        {
            dynamic @params = new
            {
                pLoginId = model.LoginId,
                pUserId = model.UserId,
                pUserName = model.UserName,
                pPassword = model.Password,
                pEmail = model.Email,
                pAccessToken = model.AccessToken,
                pRefreshToken = model.RefreshToken,
                pIsActive = model.IsActive,
                pSavedBy = model.SavedBy,
                pSavedByUserName = model.SavedByUserName,
                pSavedDate = DateTime.Now,
            };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            return SqlMapper.ExecuteScalar<string>(connection, "Account_LoginSave", @params, commandType: CommandType.StoredProcedure);
        }
        public string SaveLoginLog(AccountLoginLogModel model)
        {
            dynamic @params = new
            {
                pId = Guid.NewGuid().ToString(),
                pLoginId = model.LoginId,
                pDevice = model.Device,
                pUserName = model.UserName,
                pSavedDate = DateTime.Now,
            };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            return SqlMapper.ExecuteScalar<string>(connection, "Account_Login_Log_Save", @params, commandType: CommandType.StoredProcedure);
        }
        public List<AccountLoginLogModel> GetLoginLog(string Id = "", string LoginId = "")
        {
            dynamic @params = new
            {
                pLoginId = LoginId?.Trim() ?? "",
                pId = Id?.Trim() ?? "",
            };

            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.Query<AccountLoginLogModel>(connection, "Account_Login_Log_Get", @params, commandType: CommandType.StoredProcedure) ?? new List<AccountLoginLogModel>();
        }
        public AccountUserModel? GetUser(bool IsActive = true, string UserId = "", string DistrictId = "", 
            string DivisionId = "", string UserGroup = "", string RoleId = "", string MobileNumber = "", string Email = "", string DepartmentId = "",
            string UserGroupName = "")
        {
            dynamic @params = new
            {
                pUserId = UserId?.Trim() ?? "",
                pIsActive = IsActive,
                pDistrictId = string.IsNullOrWhiteSpace(DistrictId) ? "" : ("%" + DistrictId?.Trim() + "%"),
                pDivisionId = string.IsNullOrWhiteSpace(DivisionId) ? "" : ("%" + DivisionId?.Trim() + "%"),
                pUserGroup = UserGroup?.Trim() ?? "",
                pUserGroupName = UserGroupName?.Trim() ?? "",
                pRoleId = RoleId?.Trim() ?? "",
                pMobile = MobileNumber?.Trim() ?? "",
                pDepartmentId = string.IsNullOrWhiteSpace(DepartmentId) ? "" : ("%" + DepartmentId?.Trim() + "%"),
                pEmail = Email?.Trim() ?? ""
            };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));
            return SqlMapper.QueryFirstOrDefault<AccountUserModel>(connection, "Account_User_Get", @params, commandType: CommandType.StoredProcedure);
        }
        public string Login_OTP_Save(string UserId, string OTP, string PasswordToken)
        {
            dynamic @params = new
            {
                pUserId = UserId,
                pOTP = OTP,
                pPasswordToken = PasswordToken,
            };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            return SqlMapper.ExecuteScalar<string>(connection, "Login_OTP_Save", @params, commandType: CommandType.StoredProcedure);
        }
        public string Login_OTP_Get(string UserId)
        {
            dynamic @params = new
            {
                pUserId = UserId,
            };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            return SqlMapper.ExecuteScalar<string>(connection, "Login_OTP_Get", @params, commandType: CommandType.StoredProcedure);
        }
        public bool Login_Password_Save(string PasswordToken, string Password, string OTP)
        {
            dynamic @params = new
            {
                pPasswordToken = PasswordToken,
                pPassword = Password,
                pOtp = OTP,
                pSavedDate = DateTime.Now
            };
            using IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString(connectionId));

            int res = SqlMapper.Execute(connection, "Login_Password_Save", @params, commandType: CommandType.StoredProcedure);

            if (res > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}