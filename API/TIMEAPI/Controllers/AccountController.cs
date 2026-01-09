using AutoMapper;
using BAL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Constants;
using Model.DomainModel;
using Model.ViewModel;
using Serilog;
using System.Security.Claims;
using TIMEAPI.Infrastructure;
using Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Utils.Interface;
using Org.BouncyCastle.Asn1.Ocsp;
using OtpNet;

namespace TIMEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountBAL _accountBAL;
        private readonly ISettingBAL _settingsBAL;
        private readonly IMapper _mapper;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly ISMSHelper _smsHelper;
        

        public AccountController(ILogger<AccountController> logger, 
            IAccountBAL accountBAL, 
            IMapper mapper, 
            IJwtAuthManager jwtAuthManager, 
            ISettingBAL settingsBAL,
            ISMSHelper smsHelper)
        {
            _logger = logger;
            _accountBAL = accountBAL;
            _settingsBAL = settingsBAL;
            _mapper = mapper;
            _jwtAuthManager = jwtAuthManager;
            _smsHelper = smsHelper;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public ResponseViewModel Login(LoginRequestModel login)
        {
            try
            {
                login.Password = EncryptDecrypt.Encrypt(login.Password);

                LoginModel? _login = _accountBAL.GetLogin(login);

                if (_login != null)
                {
                    AccountUserModel user = _accountBAL.GetUser(UserId: _login.UserId) ?? new AccountUserModel();

                    AccountUserViewModel userViewData = _mapper.Map<AccountUserViewModel>(user);
                    userViewData.LoginId = _login.UserId;

                    var claims = new[]
                    {
                        //new Claim(ClaimTypes.Name,login.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(Constants.UserId, user.UserId),
                        new Claim(Constants.LoginId, _login.LoginId),
                        new Claim(Constants.Name, user.FirstName + " "+ user.LastName),
                        new Claim(Constants.RoleId, user.RoleId),
                        //new Claim(Constants.DistrictId, user.DistrictId),
                        new Claim(Constants.DivisionId, user.DivisionId),
                        new Claim(Constants.Mobile, user.Mobile),
                        //new Claim(Constants.RoleName, user.RoleName),
                        new Claim(Constants.RoleCode, user.RoleCode),
                        new Claim(Constants.DepartmentId, user.DepartmentId),
                        new Claim(Constants.UserGroup, user.UserGroup),
                        new Claim(Constants.UserGroupName, user.UserGroupName),
                        new Claim(JwtRegisteredClaimNames.NameId, user.UserNumber),
                    };

                    JwtAuthResult _tokenResult = _jwtAuthManager.GenerateTokens(login.UserName, claims, DateTime.Now);

                    _login.RefreshToken = _tokenResult.RefreshToken.TokenString;
                    _login.AccessToken = "Bearer " + _tokenResult.AccessToken;

                    _login.SavedByUserName = user.FirstName + " " + user.LastName;
                    _login.SavedBy = _login.LoginId;
                    _login.SavedDate = DateTime.Now;

                    LoginViewModel loginViewData = GetLoginData(_login, user);

                    AccountLoginLogModel _log = new AccountLoginLogModel();

                    _log.LoginId = _login.LoginId;
                    _log.Device = login.Device; 
                    _log.UserName = user.FirstName + " " + user.LastName;

                    _accountBAL.SaveLoginLog(_log);

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = loginViewData,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Invalid username and password"
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Error,
                    Data = null,
                    Message = ex.Message
                };
            }

        }

        [HttpGet("user")]
        [Authorize]
        public ActionResult GetCurrentUser()
        {
            return Ok(new LoginViewModel
            {
                UserName = User.Identity?.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            var userName = User.Identity?.Name;
            _jwtAuthManager.RemoveRefreshTokenByUserName(userName);
            return Ok();
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var userName = User.Identity?.Name;

                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var jwtResult = _jwtAuthManager.Refresh(request.RefreshToken, accessToken, DateTime.Now);
                return Ok(new LoginViewModel
                {
                    UserName = userName,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                    AccessToken = "Bearer " + jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString
                });
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }

        private LoginViewModel GetLoginData(LoginModel login, AccountUserModel user)
        {
            LoginViewModel model = _mapper.Map<LoginViewModel>(login);

            model.UserNumber = user.UserNumber;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;

            model.UserDetails = user;

            model.Password = "";
            model.UserDetails.Password = "";

            model.UserDetails.District = "";
            model.UserDetails.DistrictId = "";
            model.UserDetails.DistrictIdList = new List<string>();
            model.UserDetails.Division = "";
            model.UserDetails.DivisionId = "";
            model.UserDetails.DivisionIdList = new List<string>();
            model.UserDetails.Department = "";
            model.UserDetails.DepartmentId = "";
            model.UserDetails.DepartmentIdList = new List<string>();
            model.UserDetails.DepartmentNameList = new List<string>();

            if (user.RoleCode == "ADM")
            {
                model.Privillage = _settingsBAL.Account_Role_Privilege_Get_All(user.RoleId)?.Select(x => x.PrivilegeCode)?.ToList() ?? new List<string>();
            }
            else
            {
                model.Privillage = _settingsBAL.Account_Role_Privilege_Login_Get(user.RoleId)?.Select(x => x.PrivilegeCode)?.ToList() ?? new List<string>();
            }

            return model;
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public ResponseViewModel SendOtp(string MobileNumber = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(MobileNumber.Trim()) && MobileNumber.Length == 10)
                {
                    AccountUserModel? user = _accountBAL.GetUser(IsActive: true, MobileNumber: MobileNumber.Trim());

                    if (user != null)
                    {
                        string mobileNumber = user.Mobile;

                        if (!string.IsNullOrWhiteSpace(mobileNumber))
                        {
                            var OTP = _smsHelper.GenerateOtp();

                            string res = _accountBAL.Login_OTP_Save(user.UserId, OTP, "");


                            IDictionary<string, string> replaces = new Dictionary<string, string>();
                            replaces.Add("{#otp#}", OTP);

                            string _message = string.Empty;
                            _smsHelper.SentSMS(new List<string>() { mobileNumber }, "SENDOTP", replaces, out _message);

                            string mobileEndsWith = mobileNumber.Substring(mobileNumber.Length - 2);
                            if (res == OTP)
                            {
                                return new ResponseViewModel()
                                {
                                    Status = ResponseConstants.Success,
                                    Data = MobileNumber,
                                    Message = "OTP send to registered mobile number ends with ...." + mobileEndsWith
                                };
                            }
                        }
                        else
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Failed,
                                Data = null,
                                Message = "Mobile number is not saved for this user"
                            };
                        }
                    }
                    else
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Mobile number is not valid"
                        };
                    }
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Something went wrong"
                    };
                }

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Failed,
                    Data = null,
                    Message = "Something went wrong"
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Error,
                    Data = null,
                    Message = ex.Message
                };
            }
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public ResponseViewModel ValidateOtp(ValidateOtpModel model)
        {
            try
            {
                AccountUserModel? user = _accountBAL.GetUser(MobileNumber: model.MobileNumber.Trim());

                if (user != null)
                {
                    string savedOtp = _accountBAL.Login_OTP_Get(UserId: user.UserId);

                    if (!string.IsNullOrWhiteSpace(savedOtp) && savedOtp.Trim() == model.OTP.Trim())
                    {
                        Guid g = Guid.NewGuid();
                        string GuidString = Convert.ToBase64String(g.ToByteArray());
                        GuidString = GuidString.Replace("=", "");
                        GuidString = GuidString.Replace("+", "");

                        string res = _accountBAL.Login_OTP_Save(user.UserId, "", GuidString);

                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = GuidString,
                            Message = "OTP is valid"
                        };
                    }
                    else
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Invalid OTP"
                        };
                    }
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Mobile number is not valid"
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Error,
                    Data = null,
                    Message = ex.Message
                };
            }
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public ResponseViewModel SaveNewPassword(SaveNewPasswordModel model)
        {
            try
            {
                bool res = _accountBAL.Login_Password_Save(model.Token, EncryptDecrypt.Encrypt(model.Password), model.OTP);
                if (res)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = null,
                        Message = "New password saved"
                    };
                }

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Failed,
                    Data = null,
                    Message = "Somthing went wrog"
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Error,
                    Data = null,
                    Message = ex.Message
                };
            }
        }

        [HttpGet("[action]")]
        [Authorize]
        public ResponseViewModel GetLoginLog(string Id, string LoginId)
        {
            try
            {
                var res = _accountBAL.GetLoginLog(Id, LoginId);
                if (res != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = res,
                        Message = "Login log"
                    };
                }

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Failed,
                    Data = null,
                    Message = "Somthing went wrog"
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Error,
                    Data = null,
                    Message = ex.Message
                };
            }
        }
    }

    public class RefreshTokenRequest
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }

    public class ImpersonationRequest
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
}
