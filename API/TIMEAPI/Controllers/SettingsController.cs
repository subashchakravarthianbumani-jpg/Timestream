using AutoMapper;
using BAL;
using BAL.BackgroundWorkerService;
using BAL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Constants;
using Model.DomainModel;
using Model.MailTemplateHelper;
using Model.ViewModel;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Utils;
using Utils.UtilModels;

namespace TIMEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ISettingBAL _settingBAL;
        private readonly IGeneralBAL _generalBAL;
        private readonly IMapper _mapper;

        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SettingsController(ILogger<AccountController> logger, ISettingBAL settingBAL,
            IMapper mapper, IBackgroundTaskQueue backgroundTaskQueue, IServiceScopeFactory serviceScopeFactory,
            IGeneralBAL generalBAL)
        {
            _logger = logger;
            _settingBAL = settingBAL;
            _mapper = mapper;
            _backgroundTaskQueue = backgroundTaskQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _generalBAL = generalBAL;
        }

        #region Two Column Configuration
        [HttpGet("[action]")]
        public ResponseViewModel Configuration_Get(bool IsActive = true, string ConfigurationId = "", string CategoryId = "", string ParentConfigurationId = "", string CategoryCode = "", string DepartmentId = "")
        {
            try
            {
                List<ConfigurationModel> list = _settingBAL.Configuration_Get(IsActive, ConfigurationId, CategoryId, ParentConfigurationId, "", CategoryCode, DepartmentId);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = _mapper.Map<List<ConfigurationViewModel>>(list),
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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
        [HttpGet("[action]")]
        public ResponseViewModel Configuration_Category_Get(string CategoryCode)
        {
            try
            {
                //string loginId = User.Claims.Where(x => x.Type == Constants.LoginID)?.FirstOrDefault()?.Value ?? "";

                List<ConfigCategoryModel> list = _settingBAL.Configuration_Category_Get(CategoryCode);

                if (list != null)
                {
                    list.ForEach(configCategory =>
                    {
                        if (!string.IsNullOrEmpty(configCategory.ParentId))
                        {
                            configCategory.IsDependent = true;
                        }
                        else
                        {
                            configCategory.IsDependent = false;
                        }
                    });

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = _mapper.Map<List<ConfigCategoryViewModel>>(list),
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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
        [HttpPost("[action]")]
        public ResponseViewModel Configuration_SaveUpdate(ConfigurationModel fModel)
        {
            try
            {
                if (fModel != null)
                {
                    ConfigurationModel model = _mapper.Map<ConfigurationModel>(fModel);

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    if (model.Id == "" || model.Id == null)
                    {
                        model.Id = Guid.NewGuid().ToString();
                    }

                    string result = _settingBAL.Configuration_SaveUpdate(model);

                    if (result == "-1")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate value",
                            ErrorCode = ResponceErrorCodes.TCC_ConfigurationValueExist
                        };
                    }
                    else if (result == "-2")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate Code",
                            ErrorCode = ResponceErrorCodes.TCC_ConfigurationCodeExist
                        };
                    }
                    else if (result == "-3")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Dependent record exist, You can not set IsAcive 0",
                            ErrorCode = ResponceErrorCodes.TCC_ConfigurationDependentRecordExist
                        };
                    }
                    else
                    {
                        if (model.IsActive)
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
                                ErrorCode = ResponceErrorCodes.TCC_ConfigurationSaved
                            };
                        }
                        else
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
                                ErrorCode = ResponceErrorCodes.TCC_ConfigurationSaved
                            };
                        }
                    }
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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
        [HttpGet("[action]")]
        public ResponseViewModel Configuration_Department_Get()
        {
            try
            {
                List<SelectListItem> list = _settingBAL.GetDepartmentSelectList();

                if (list != null)
                {
                    if (list != null)
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Message = "Action completed successfully",
                            Data = list,
                        };
                    }
                    else
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Somthing went wrong"
                        };
                    }
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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
        [HttpGet("[action]")]
        public ResponseViewModel Configuration_UserGroup_Get()
        {
            try
            {
                List<SelectListItem> list = _settingBAL.GetUserGroupSelectList();

                if (list != null)
                {
                    if (list != null)
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Message = "Action completed successfully",
                            Data = list,
                        };
                    }
                    else
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Somthing went wrong"
                        };
                    }
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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
        [HttpGet("[action]")]
        public ResponseViewModel Configuration_Division_Get()
        {
            try
            {
                List<SelectListItem> list = _settingBAL.GetDivisionSelectList();

                if (list != null)
                {
                    if (list != null)
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Message = "Action completed successfully",
                            Data = list,
                        };
                    }
                    else
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Somthing went wrong"
                        };
                    }
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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


        [HttpGet("[action]")]
        public ResponseViewModel Configuration_List_Reports()
        {
            try
            {
                List<SelectListItem> DepartmentList = new List<SelectListItem>();
                List<SelectListItem> DivisionList = new List<SelectListItem>();
                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    List<SelectListItem> list = _settingBAL.GetDepartmentSelectList();
                    DepartmentList = list.Where(x => DepartmentIds.Split(",").Select(x => x.Trim()).ToList().Contains(x.Value)).ToList();
                }
                string DivisionIds = User.Claims.Where(x => x.Type == Constants.DivisionId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(DivisionIds))
                {
                    List<SelectListItem> list = _settingBAL.GetDivisionSelectList();
                    DivisionList = list.Where(x => DivisionIds.Split(",").Select(x => x.Trim()).ToList().Contains(x.Value)).ToList();
                }

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Success,
                    Message = "Action completed successfully",
                    Data = new { Departments = DepartmentList, Divisions = DivisionList },
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
        #endregion Two Column Configuration

        #region Role

        [HttpGet("[action]")]
        public ResponseViewModel Role_Get(string RoleId = "", bool IsActive = true)
        {
            try
            {
                List<AccountRoleModel> list = _settingBAL.Account_Role_Get(RoleId, IsActive);

                if (list != null)
                {
                    list.ForEach(r =>
                    {
                        if (Constants.StaticRoles.Contains(r.RoleCode))
                        {
                            r.IsChangeable = false;
                        }
                        else
                        {
                            r.IsChangeable = true;
                        }
                    });

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = _mapper.Map<List<AccountRoleViewModel>>(list),
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpPost("[action]")]
        public ResponseViewModel Role_SaveUpdate(AccountRoleViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    AccountRoleModel model = _mapper.Map<AccountRoleModel>(rModel);

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    if (model.Id == "" || model.Id == null)
                    {
                        model.Id = Guid.NewGuid().ToString();
                    }

                    string result = _settingBAL.Account_Role_Save(model);

                    if (result == "-1")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate role name",
                            ErrorCode = ResponceErrorCodes.ROLE_ConfigurationValueExist
                        };
                    }
                    else if (result == "-2")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate role code",
                            ErrorCode = ResponceErrorCodes.ROLE_ConfigurationCodeExist
                        };
                    }
                    else if (result == "-3")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Dependent record exist in user master, You can not set IsAcive 0",
                            ErrorCode = ResponceErrorCodes.ROLE_ConfigurationDependentRecordExist
                        };
                    }
                    else if (result == "-4")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Dependent record exist in role privillage, You can not set IsAcive 0",
                            ErrorCode = ResponceErrorCodes.ROLE_ConfigurationDependentRecordExist
                        };
                    }
                    else
                    {
                        if (model.IsActive)
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
                                ErrorCode = ResponceErrorCodes.ROLE_ConfigurationSaved
                            };
                        }
                        else
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
                                ErrorCode = ResponceErrorCodes.ROLE_ConfigurationSaved
                            };
                        }
                    }
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        #endregion Role

        #region Role Priilege
        [HttpGet("[action]")]
        public ResponseViewModel Role_Privilege_Get(string RoleId)
        {
            try
            {
                List<AccountPrivilegeByGroupModel> list = _settingBAL.Account_Role_Privilege_Get(RoleId);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = list,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpPost("[action]")]
        public ResponseViewModel Role_Privilege_Save(AccountPrivilegeSaveViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    AccountPrivilegeSaveModel model = _mapper.Map<AccountPrivilegeSaveModel>(rModel);

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;

                    string result = _settingBAL.Account_Role_Privilege_Save(model);

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = result,
                        Message = "Action completed successfully",
                        ErrorCode = ResponceErrorCodes.ROLE_ConfigurationSaved
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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
        #endregion Role Priilege

        #region User

        [HttpGet("[action]")]
        public ResponseViewModel User_Get(bool IsActive = true, string UserId = "", string DistrictId = "", string DivisionId = "", string UserGroup = "", string RoleId = "", string DepartmentId = "")
        {
            try
            {
                List<AccountUserModel> list = _settingBAL.User_Get(IsActive, UserId, DistrictId, DivisionId, UserGroup, RoleId, DepartmentId: DepartmentId);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = _mapper.Map<List<AccountUserViewModel>>(list),
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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
        [HttpGet("[action]")]
        public ResponseViewModel User_Activate(string UserId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(UserId))
                {
                    AccountUserModel model = new AccountUserModel();
                    model.UserId = UserId;
                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;

                    string userId = _settingBAL.User_Activate(model);

                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Message = "Action completed successfully",
                        };
                    }

                }
                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Failed,
                    Data = null,
                    Message = "Somthing went wrong"
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
        [HttpPost("[action]")]
        public ResponseViewModel User_SaveUpdate(AccountUserViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    AccountUserModel model = _mapper.Map<AccountUserModel>(rModel);

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    if (model.UserId == "")
                    {
                        model.UserId = Guid.NewGuid().ToString();
                    }
                    if (model.DivisionIdList?.Count > 0)
                    {
                        model.DivisionId = string.Join(',', model.DivisionIdList);
                    }
                    if (model.DistrictIdList?.Count > 0)
                    {
                        model.DistrictId = string.Join(',', model.DistrictIdList);
                    }
                    if (model.DepartmentIdList?.Count > 0)
                    {
                        model.DepartmentId = string.Join(',', model.DepartmentIdList);
                    }
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        model.Password = EncryptDecrypt.Encrypt(rModel.Password);
                    }

                    AccountUserModel result = _settingBAL.User_SaveUpdate(model);

                    if (result.UserId == "-1")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate email address",
                            ErrorCode = ResponceErrorCodes.USER_ConfigurationEmailExist
                        };
                    }
                    else if (result.UserId == "-2")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate mobile number",
                            ErrorCode = ResponceErrorCodes.USER_ConfigurationMobileExist
                        };
                    }
                    else
                    {
                        if (model.IsActive)
                        {
                            CurrentUserModel currentUser = new CurrentUserModel();
                            currentUser.UserName = model.SavedByUserName;
                            currentUser.UserId = model.SavedBy;

                            EmailTemplateModel template = WorkMailTemplate.GetEmailTemplate(EmailTemplateCode.UserCreate);
                            List<EmailModel> mailList = new List<EmailModel>()
                            {
                                new EmailModel()
                                {
                                    Body = template.Body,
                                    Subject = template.Subject + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:FFF"),
                                    To = new List<string>() { model.Email },
                                    BodyPlaceHolders = new Dictionary<string, string>() {
                                        { "{RECIPIENTFIRSTNAME}", model.FirstName },
                                        { "{RECIPIENTLASTNAME}", model.LastName },
                                        { "{USERNAME}", model.Email },
                                        { "{PASSWORD}", rModel.Password }
                                    },
                                    SavedBy = model.SavedBy,
                                    SavedByUserName = model.SavedByUserName,
                                    SavedDate = model.SavedDate,
                                    Type = "ACCOUNT",
                                    TypeId = model.UserId,
                                    ReceivedBy = model.UserId
                                }
                            };

                            if (mailList.Count > 0)
                            {
                                _generalBAL.SendMessage(mailList, new List<SMSModel>(), currentUser);
                            }

                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
                                ErrorCode = ResponceErrorCodes.USER_ConfigurationSaved
                            };
                        }
                        else
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
                                ErrorCode = ResponceErrorCodes.USER_ConfigurationSaved
                            };
                        }
                    }
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpGet("[action]")]
        public ResponseViewModel User_Form_Get()
        {
            try
            {
                AccountUserFormDetailModel model = _settingBAL.User_Form_Get();

                if (model != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = model,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpPost("[action]")]
        public ResponseViewModel User_GetList(UserFilterModel userFilter)
       {
            try
            {
                string UserId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: UserId)?.FirstOrDefault() ?? new AccountUserModel();
                int TotalCount = 0;
                List<AccountUserModel> list = _settingBAL.User_Get(userFilter, currentUser.DepartmentId, out TotalCount);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = _mapper.Map<List<AccountUserViewModel>>(list),
                        TotalRecordCount = TotalCount
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        #endregion User

        #region QuickLink

        [HttpGet("[action]")]
        public ResponseViewModel QuickLink_Get(bool IsActive = true, string Id = "", string FileType = "")
        {
            try
            {
                List<QuickLinkMasterModel> list = _settingBAL.QuickLink_Get(IsActive, Id, FileType);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = _mapper.Map<List<QuickLinkMasterViewModel>>(list),
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpPost("[action]")]
        public ResponseViewModel QuickLink_SaveUpdate(QuickLinkMasterViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    QuickLinkMasterModel model = _mapper.Map<QuickLinkMasterModel>(rModel);

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    if (model.Id == "" || model.Id == null)
                    {
                        model.Id = Guid.NewGuid().ToString();
                    }

                    string result = _settingBAL.QuickLink_SaveUpdate(model);

                    if (result == "-1")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate name",
                            ErrorCode = ResponceErrorCodes.QUICKLINK_ConfigurationNameExist
                        };
                    }
                    else if (result == "-2")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate link",
                            ErrorCode = ResponceErrorCodes.QUICKLINK_ConfigurationLinkExist
                        };
                    }
                    else
                    {
                        if (model.IsActive)
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
                                ErrorCode = ResponceErrorCodes.QUICKLINK_ConfigurationSaved
                            };
                        }
                        else
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
                                ErrorCode = ResponceErrorCodes.QUICKLINK_ConfigurationSaved
                            };
                        }
                    }
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        #endregion QuickLink

        #region WorkTemplate

        [HttpGet("[action]")]
        public ResponseViewModel Template_Get(bool IsActive = true, string Id = "", string WorkTypeId = "", string DepartmentId = "",string subcategory="", string mainCategory = "", string serviceType = "", string categoryType = "")
        {
            try
            {
                List<TemplateModel> list = _settingBAL.Template_Get(IsActive, Id, WorkTypeId, DepartmentId, subcategory, mainCategory, serviceType, categoryType);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = _mapper.Map<List<TemplateViewModel>>(list),
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpGet("[action]")]
        [AllowAnonymous]
        public ResponseViewModel Template_Get_With_Milestone(bool IsActive_template = true, bool IsActive_milestone = true, string Id = "", string WorkTypeId = "")
        {
            try
            {
                List<TemplateModel> list = _settingBAL.Template_Get(IsActive_template, Id, WorkTypeId);

                if (list != null)
                {
                    List<TemplateViewModel> templateViewList = _mapper.Map<List<TemplateViewModel>>(list);

                    templateViewList.ForEach(template =>
                    {
                        List<TemplateMilestoneModel> milestone_list = _settingBAL.TemplateMilestone_Get(IsActive_milestone, TemplateId: template.Id) ?? new List<TemplateMilestoneModel>();
                        template.templateMilestones = _mapper.Map<List<TemplateMilestoneViewModel>>(milestone_list)?.OrderBy(o => o.OrderNumber)?.ToList() ?? new List<TemplateMilestoneViewModel>();

                    });

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = templateViewList,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpPost("[action]")]
        public ResponseViewModel Template_SaveUpdate(TemplateViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    TemplateModel model = _mapper.Map<TemplateModel>(rModel);

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    if (model.Id == "" || model.Id == null)
                    {
                        model.Id = Guid.NewGuid().ToString();
                    }

                    string result = _settingBAL.Template_SaveUpdate(model);

                    if (result == "-1")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate name",
                            ErrorCode = ResponceErrorCodes.TEMPLATE_ConfigurationNameExist
                        };
                    }
                    if (result == "-2")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate Code",
                            ErrorCode = ResponceErrorCodes.TEMPLATE_ConfigurationNameExist
                        };
                    }
                    else
                    {
                        if (model.IsActive)
                        {
                            TemplateViewModel template = _mapper.Map<TemplateViewModel>(_settingBAL.Template_Get(true, Id: result)[0]);
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = template,
                                Message = "Action completed successfully",
                                ErrorCode = ResponceErrorCodes.TEMPLATE_ConfigurationSaved
                            };
                        }
                        else
                        {
                            TemplateViewModel template = _mapper.Map<TemplateViewModel>(_settingBAL.Template_Get(false, Id: result)[0]);
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = template,
                                Message = "Action completed successfully",
                                ErrorCode = ResponceErrorCodes.TEMPLATE_ConfigurationSaved
                            };
                        }
                    }
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpGet("[action]")]
        public ResponseViewModel TemplateMilestone_Get(bool IsActive = true, string Id = "", string TemplateId = "")
        {
            try
            {
                List<TemplateMilestoneModel> list = _settingBAL.TemplateMilestone_Get(IsActive, Id, TemplateId);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = _mapper.Map<List<TemplateMilestoneViewModel>>(list),
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpPost("[action]")]
        public ResponseViewModel TemplateMilestone_SaveUpdate(List<TemplateMilestoneViewModel> rModel)
        {
            try
            {
                if (rModel != null && rModel.Count > 0)
                {
                    bool IsPublished = rModel[0].IsPublished;
                    string templateId = rModel[0].TemplateId;

                    List<TemplateMilestoneModel> model_list = _mapper.Map<List<TemplateMilestoneModel>>(rModel);

                    List<string> milestone_name_list = model_list.Select(x => x.MilestoneName).Distinct().ToList();
                    List<string> milestone_code_list = model_list.Select(x => x.MilestoneCode).Distinct().ToList();

                    if (milestone_name_list.Count != model_list.Count)
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate name",
                            ErrorCode = ResponceErrorCodes.TEMPLATEMILESTONE_ConfigurationNameExist
                        };

                    }
                    else if (milestone_code_list.Count != model_list.Count)
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate code",
                            ErrorCode = ResponceErrorCodes.TEMPLATEMILESTONE_ConfigurationNameExist
                        };
                    }
                    else
                    {
                        foreach (TemplateMilestoneModel model in model_list)
                        {
                            model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                            model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                            model.SavedDate = DateTime.Now;
                            if (model.Id == "" || model.Id == null)
                            {
                                model.Id = Guid.NewGuid().ToString();
                            }

                            string result = _settingBAL.TemplateMilestone_SaveUpdate(model);

                        }

                        if (IsPublished)
                        {
                            TemplateModel publishModel = new TemplateModel();
                            publishModel.Id = templateId;
                            publishModel.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                            publishModel.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                            publishModel.SavedDate = DateTime.Now;

                            _settingBAL.Template_Publish(publishModel);
                        }

                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = null,
                            Message = "Action completed successfully",
                            ErrorCode = ResponceErrorCodes.TEMPLATE_ConfigurationSaved
                        };
                    }
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpGet("[action]")]
        public ResponseViewModel Template_GetWorkTypeList()
        {
            try
            {
                List<SelectListItem> list = _settingBAL.GetWorkTypeList();

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = list,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpGet("[action]")]
        public ResponseViewModel Template_ServiceTypeList()
        {
            try
            {
                List<SelectListItem> list = _settingBAL.Template_ServiceTypeList();

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = list,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpGet("[action]")]
        public ResponseViewModel Template_CategoryTypeList()
        {
            try
            {
                List<SelectListItem> list = _settingBAL.Template_CategoryTypeList();

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = list,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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


        [HttpGet("[action]")]
        public ResponseViewModel Template_Publish(string Id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    TemplateModel model = new TemplateModel();

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    model.Id = Id;

                    string result = _settingBAL.Template_Publish(model);

                    if (result == Id)
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = null,
                            Message = "Action completed successfully",
                            ErrorCode = ResponceErrorCodes.TEMPLATE_ConfigurationSaved
                        };
                    }
                }

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Failed,
                    Data = null,
                    Message = "Somthing went wrong"
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



        // to edit the default duration and strenght for every work
        [HttpPost("[action]")]
        public ResponseViewModel Work_Template_Edit(TemplateViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    TemplateViewModel model = _mapper.Map<TemplateViewModel>(rModel);

                    model.LastUpdatedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.LastUpdatedUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.LastUpdatedDate = DateTime.Now;
                    if (model.Id == "" || model.Id == null)
                    {
                        model.Id = Guid.NewGuid().ToString();
                    }

                    string result = _settingBAL.Work_Template_Edit(rModel);

                    if (result != null)
                    {
                        

                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = true,
                            Message = "Action completed successfully"
                        };
                    }
                }

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Failed,
                    Data = null,
                    Message = "Somthing went wrong"
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





        #endregion WorkTemplate

        #region BackgroundTask

        [HttpGet("[action]")]
        [AllowAnonymous]
        public ResponseViewModel SendMail()
        {
            try
            {
                _backgroundTaskQueue.QueueBackgroundWorkItem(workItem: token =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var scopedService = scope.ServiceProvider;
                        var serringBAL = scopedService.GetRequiredService<ISettingBAL>();
                        var logger = scopedService.GetRequiredService<ILogger<SettingsController>>();

                        try
                        {
                            DoBackgroundWork(serringBAL);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, ex.Message);
                        }
                    }

                    return Task.CompletedTask;
                });

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Failed,
                    Data = null,
                    Message = "Somthing went wrong"
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

        private void DoBackgroundWork(ISettingBAL settingBAL)
        {
            var list = settingBAL.Account_Role_Get("", true);
        }

        #endregion BackgroundTask

        #region Approval Flow
        [HttpGet("[action]")]
        public ResponseViewModel ApprovalFlow_GetRoleList(string DepartmentId)
        {
            try
            {
                List<SelectListItem> list = _settingBAL.GetApprovalflowRoleIdList(DepartmentId);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = list,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpGet("[action]")]
        public ResponseViewModel ApprovalFlow_Get(string DepartmentId)
        {
            try
            {
                List<ApprovalFlowMaster> list = _settingBAL.ApprovalFlow_Get(DepartmentId);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = _mapper.Map<List<ApprovalFlowViewMaster>>(list)?.OrderBy(o => o.OrderNumber)?.ToList() ?? new List<ApprovalFlowViewMaster>(),
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpPost("[action]")]
        public ResponseViewModel ApprovalFlow_Add_Role(ApprovalFlowAddRoleModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    ApprovalFlowAddRoleModel_API model = new ApprovalFlowAddRoleModel_API();
                    model.DepartmentId = rModel.DepartmentId;
                    model.RoleIds = rModel.RoleIds;
                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;

                    string result = _settingBAL.ApprovalFlow_Add_Role(model);

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = result,
                        Message = "Action completed successfully",
                        ErrorCode = ResponceErrorCodes.QUICKLINK_ConfigurationSaved
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpGet("[action]")]
        public ResponseViewModel ApprovalFlow_Remove_Role(string Id, string RoleId)
        {
            try
            {
                ApprovalFlowMaster model = new ApprovalFlowMaster();
                model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                model.SavedDate = DateTime.Now;
                model.RoleId = RoleId;
                model.Id = Id;

                string res = _settingBAL.ApprovalFlow_Remove_Role(model);

                if (res != null && res != "")
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = null,
                        Message = "Action completed successfully",
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpPost("[action]")]
        public ResponseViewModel ApprovalFlow_SaveUpdate(List<ApprovalFlowViewMaster> rModel)
        {
            try
            {
                if (rModel != null)
                {
                    List<ApprovalFlowMaster> model_list = _mapper.Map<List<ApprovalFlowMaster>>(rModel);

                    foreach (ApprovalFlowMaster model in model_list)
                    {
                        model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                        model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                        model.SavedDate = DateTime.Now;
                        if (model.Id == "" || model.Id == null)
                        {
                            model.Id = Guid.NewGuid().ToString();
                        }
                        model.IsActive = true;
                        string result = _settingBAL.ApprovalFlow_SaveUpdate(model);
                    }

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = null,
                        Message = "Action completed successfully",
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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
        #endregion Approval Flow

        #region Alert
        [HttpGet("[action]")]
        public ResponseViewModel Alert_Configuration_Form()
        {
            try
            {
                AlertConfigurationFormModel model = _settingBAL.Alert_Configuration_Form();

                if (model != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = model,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpGet("[action]")]
        public ResponseViewModel Alert_Primary_Get(string Id = "", string Type = "", string Object = "", string Name = "", string Department = "", bool IsActive = true)
        {
            try
            {
                List<AlertConfigurationPrimaryModel> list = _settingBAL.Alert_Primary_Get(Id, Type, Object, Name, Department, IsActive);

                if (list != null)
                {
                    List<AlertConfigurationPrimaryViewModel> viewList = _mapper.Map<List<AlertConfigurationPrimaryViewModel>>(list);

                    viewList.ForEach(x =>
                    {
                        x.SmsuserGroupsList = x.SmsuserGroups.Split(",")?.ToList() ?? new List<string>();
                        x.EmailuserGroupList = x.EmailuserGroups.Split(",")?.ToList() ?? new List<string>();

                        if (!string.IsNullOrWhiteSpace(Id) || IsActive == false)
                        {
                            x.AlertConfigurationSecondary = _mapper.Map<List<AlertConfigurationSecondaryViewModel>>(_settingBAL.Alert_Secondary_Get(PrimaryId: x.Id));
                        }
                    });

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = viewList,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpGet("[action]")]
        public ResponseViewModel Alert_Secondary_Get(string Id = "", string PrimaryId = "")
        {
            try
            {
                List<AlertConfigurationSecondaryModel> list = _settingBAL.Alert_Secondary_Get(Id, PrimaryId);

                if (list != null)
                {
                    List<AlertConfigurationSecondaryViewModel> viewList = _mapper.Map<List<AlertConfigurationSecondaryViewModel>>(list);

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Message = "Action completed successfully",
                        Data = viewList,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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

        [HttpPost("[action]")]
        public ResponseViewModel AlertPrimary_Config_SaveUpdate(AlertConfigurationPrimaryViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    AlertConfigurationPrimaryModel model = _mapper.Map<AlertConfigurationPrimaryModel>(rModel);

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;

                    model.SMSuserGroups = string.Join(",", model.SMSuserGroupsList ?? new List<string>())?.ToString() ?? "";
                    model.EmailuserGroups = string.Join(",", model.EmailuserGroupList ?? new List<string>())?.ToString() ?? "";

                    string result = _settingBAL.AlertPrimary_Config_SaveUpdate(model);

                    if (result.Contains("@@1"))
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate record"
                        };
                    }
                    else
                    {
                        string Id = "";
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            Id = result.Split("/")[0].ToString();

                            if (rModel.AlertConfigurationSecondary?.Count > 0)
                            {
                                foreach (AlertConfigurationSecondaryViewModel item in rModel.AlertConfigurationSecondary)
                                {
                                    AlertConfigurationSecondaryModel secondaryModel = _mapper.Map<AlertConfigurationSecondaryModel>(item);

                                    secondaryModel.SavedBy = model.SavedBy;
                                    secondaryModel.SavedByUserName = model.SavedByUserName;
                                    secondaryModel.SavedDate = model.SavedDate;
                                    secondaryModel.PrimaryId = Id;

                                    _settingBAL.AlertSecondary_Config_SaveUpdate(secondaryModel);
                                }
                            }
                        }

                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = Id,
                            Message = "Action completed successfully"
                        };
                    }
                }
                else
                {


                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Somthing went wrong"
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
        #endregion Alert


        #region Export

        [HttpGet("[action]")]
        [AllowAnonymous]
        public ResponseViewModel ExportAsHTML()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var renderer = scope.ServiceProvider.GetRequiredService<RazorView>();
                    var content = renderer.RenderViewToString("ResultView", new ResponseViewModel()
                    {
                        Data = "TestData",
                        Message = "TestMessage",
                        ErrorCode = "200",
                        TotalRecordCount = 10,
                        Status = "SUCCESS"

                    }).GetAwaiter().GetResult();

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = content,
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

        #endregion





        //[HttpGet("Live")]
        //[AllowAnonymous]
        //public async Task Live([FromQuery] string rtspUrl)
        //{
        //    if (string.IsNullOrEmpty(rtspUrl))
        //    {
        //        Response.StatusCode = 400;
        //        await Response.WriteAsync("RTSP URL is required");
        //        return;
        //    }

        //    // Important headers for multipart MJPEG
        //    Response.StatusCode = 200;
        //    Response.ContentType = "multipart/x-mixed-replace; boundary=frame";
        //    Response.Headers["Cache-Control"] = "no-cache";
        //    Response.Headers["Connection"] = "close";
        //    // If behind nginx you may want: Response.Headers["X-Accel-Buffering"] = "no";

        //    var ffmpegPath = @"C:\Users\Indu\Downloads\ffmpeg.exe";
        //    // adjust if needed

        //   // var ffmpegPath = "/usr/bin/ffmpeg";

        //    var psi = new ProcessStartInfo
        //    {
        //        FileName = ffmpegPath,



        //        Arguments = $"-rtsp_transport tcp -fflags nobuffer -flags low_delay -i \"{rtspUrl}\" -c:v mjpeg -an -f mjpeg -q:v 5 -",



        //        RedirectStandardOutput = true,
        //        RedirectStandardError = true,
        //        UseShellExecute = false,
        //        CreateNoWindow = true
        //    };

        //    using var ffmpeg = Process.Start(psi);
        //    if (ffmpeg == null)
        //    {
        //        Response.StatusCode = 500;
        //        await Response.WriteAsync("Failed to start ffmpeg");
        //        return;
        //    }


        //    var stderrTask = Task.Run(async () =>
        //    {
        //        try
        //        {
        //            var reader = ffmpeg.StandardError;
        //            string? line;
        //            while ((line = await reader.ReadLineAsync()) != null)
        //            {
        //                // optional: log the ffmpeg stderr somewhere
        //                Console.WriteLine("[ffmpeg] " + line);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("[ffmpeg-stderr] " + ex);
        //        }
        //    });

        //    var output = ffmpeg.StandardOutput.BaseStream;
        //    var cancellation = HttpContext.RequestAborted;

        //    var buffer = new byte[8192];
        //    var imageBuffer = new List<byte>();
        //    try
        //    {
        //        while (!cancellation.IsCancellationRequested)
        //        {
        //            int bytesRead = await output.ReadAsync(buffer, 0, buffer.Length, cancellation);
        //            if (bytesRead == 0)
        //                break;

        //            // Append new bytes
        //            for (int i = 0; i < bytesRead; i++)
        //                imageBuffer.Add(buffer[i]);

        //            // Try to find end-of-jpeg marker (0xFF 0xD9)
        //            int endIndex = FindJpegEnd(imageBuffer);
        //            while (endIndex >= 0)
        //            {
        //                // endIndex is the index of the 0xD9 byte; include it
        //                int frameLength = endIndex + 1;
        //                var imageBytes = imageBuffer.Take(frameLength).ToArray();

        //                // Remove the used bytes from buffer
        //                imageBuffer.RemoveRange(0, frameLength);

        //                // Write multipart frame headers + image
        //                var boundaryBytes = Encoding.ASCII.GetBytes("--frame\r\n");
        //                var headerBytes = Encoding.ASCII.GetBytes("Content-Type: image/jpeg\r\n\r\n");
        //                var footerBytes = Encoding.ASCII.GetBytes("\r\n");

        //                await Response.Body.WriteAsync(boundaryBytes, 0, boundaryBytes.Length, cancellation);
        //                await Response.Body.WriteAsync(headerBytes, 0, headerBytes.Length, cancellation);
        //                await Response.Body.WriteAsync(imageBytes, 0, imageBytes.Length, cancellation);
        //                await Response.Body.WriteAsync(footerBytes, 0, footerBytes.Length, cancellation);

        //                await Response.Body.FlushAsync(cancellation);

        //                // Look for another complete JPEG in the residual buffer
        //                endIndex = FindJpegEnd(imageBuffer);
        //            }
        //        }
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        // client disconnected
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[STREAM ERROR] {ex}");
        //        if (!cancellation.IsCancellationRequested)
        //        {
        //            Response.StatusCode = 500;
        //            try { await Response.WriteAsync("Error: " + ex.Message); } catch { }
        //        }
        //    }
        //    finally
        //    {
        //        try
        //        {
        //            if (!ffmpeg.HasExited)
        //            {
        //                ffmpeg.Kill(true);
        //                ffmpeg.WaitForExit(1000);
        //            }
        //        }
        //        catch { }

        //        // Wait for stderr reader to finish (optional)
        //        try { await stderrTask; } catch { }
        //    }
        //}





        //// Returns index of the 0xD9 byte if a 0xFF 0xD9 sequence exists, otherwise -1.
        //private static int FindJpegEnd(List<byte> buffer)
        //{
        //    for (int i = 1; i < buffer.Count; i++)
        //    {
        //        if (buffer[i - 1] == 0xFF && buffer[i] == 0xD9)
        //            return i;
        //    }
        //    return -1;
        //}




        [HttpGet("Live")]
        [AllowAnonymous]
        public async Task Live([FromQuery] string rtspUrl)
        {
            if (string.IsNullOrEmpty(rtspUrl))
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("RTSP URL is required");
                return;
            }

            // Important headers for multipart MJPEG
            Response.StatusCode = 200;
            Response.ContentType = "multipart/x-mixed-replace; boundary=frame";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "close";
            // If behind nginx you may want: Response.Headers["X-Accel-Buffering"] = "no";

            //var ffmpegPath = @"C:\Users\Indu\Downloads\ffmpeg.exe";

            //var ffmpegPath = @"C:\Users\vijay\Downloads\Links\ffmpeg.exe";

            // adjust if needed

            var ffmpegPath = "/usr/bin/ffmpeg";

            string arguments;
            if (rtspUrl.StartsWith("rtsp://", StringComparison.OrdinalIgnoreCase))
            {

                arguments = $"-rtsp_transport tcp -fflags nobuffer -flags low_delay -i \"{rtspUrl}\" -c:v mjpeg -an -f mjpeg -q:v 5 -";
            }
            else if (rtspUrl.StartsWith("rtmp://", StringComparison.OrdinalIgnoreCase))
            {
                // RTMP specific arguments - no rtsp_transport needed
                // Add -timeout and -rw_timeout for RTMP connection stability
                arguments = $"-fflags nobuffer -flags low_delay -i \"{rtspUrl}\" -c:v mjpeg -an -f mjpeg -q:v 5 -";

                // Alternative RTMP arguments with better buffering control:
                // arguments = $"-timeout 5000000 -rw_timeout 5000000 -i \"{rtspUrl}\" -c:v mjpeg -an -f mjpeg -q:v 5 -";
            }
            else
            {
                arguments = $"-fflags nobuffer -flags low_delay -i \"{rtspUrl}\" -c:v mjpeg -an -f mjpeg -q:v 5 -";
            }

            var psi = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                // Keep using mjpeg as ffmpeg outputs raw JPEGs; you can also try "-f mpjpeg" to let ffmpeg emit multipart
                //Arguments = $"-rtsp_transport tcp -fflags nobuffer -flags low_delay -bsf:v mjpeg2jpeg -i \"{rtspUrl}\" -vf scale=-1:-1 -c:v mjpeg -q:v 6 -preset ultrafast -bufsize 0 -an -fflags discardcorrupt+ignoragun -f mjpeg -";



                //  Arguments = $"-rtsp_transport tcp -fflags nobuffer -flags low_delay -i \"{rtspUrl}\" -c:v mjpeg -an -f mjpeg -q:v 5 -",
                Arguments = arguments,

                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var ffmpeg = Process.Start(psi);
            if (ffmpeg == null)
            {
                Response.StatusCode = 500;
                await Response.WriteAsync("Failed to start ffmpeg");
                return;
            }

            // Read stderr asynchronously so ffmpeg doesn't block if it writes a lot
            var stderrTask = Task.Run(async () =>
            {
                try
                {
                    var reader = ffmpeg.StandardError;
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        // optional: log the ffmpeg stderr somewhere
                        Console.WriteLine("[ffmpeg] " + line);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ffmpeg-stderr] " + ex);
                }
            });

            var output = ffmpeg.StandardOutput.BaseStream;
            var cancellation = HttpContext.RequestAborted;

            var buffer = new byte[8192];
            var imageBuffer = new List<byte>();
            try
            {
                while (!cancellation.IsCancellationRequested)
                {
                    int bytesRead = await output.ReadAsync(buffer, 0, buffer.Length, cancellation);
                    if (bytesRead == 0)
                        break;

                    // Append new bytes
                    for (int i = 0; i < bytesRead; i++)
                        imageBuffer.Add(buffer[i]);

                    // Try to find end-of-jpeg marker (0xFF 0xD9)
                    int endIndex = FindJpegEnd(imageBuffer);
                    while (endIndex >= 0)
                    {
                        // endIndex is the index of the 0xD9 byte; include it
                        int frameLength = endIndex + 1;
                        var imageBytes = imageBuffer.Take(frameLength).ToArray();

                        // Remove the used bytes from buffer
                        imageBuffer.RemoveRange(0, frameLength);

                        // Write multipart frame headers + image
                        var boundaryBytes = Encoding.ASCII.GetBytes("--frame\r\n");
                        var headerBytes = Encoding.ASCII.GetBytes("Content-Type: image/jpeg\r\n\r\n");
                        var footerBytes = Encoding.ASCII.GetBytes("\r\n");

                        await Response.Body.WriteAsync(boundaryBytes, 0, boundaryBytes.Length, cancellation);
                        await Response.Body.WriteAsync(headerBytes, 0, headerBytes.Length, cancellation);
                        await Response.Body.WriteAsync(imageBytes, 0, imageBytes.Length, cancellation);
                        await Response.Body.WriteAsync(footerBytes, 0, footerBytes.Length, cancellation);

                        await Response.Body.FlushAsync(cancellation);

                        // Look for another complete JPEG in the residual buffer
                        endIndex = FindJpegEnd(imageBuffer);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // client disconnected
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[STREAM ERROR] {ex}");
                if (!cancellation.IsCancellationRequested)
                {
                    Response.StatusCode = 500;
                    try { await Response.WriteAsync("Error: " + ex.Message); } catch { }
                }
            }
            finally
            {
                try
                {
                    if (!ffmpeg.HasExited)
                    {
                        ffmpeg.Kill(true);
                        ffmpeg.WaitForExit(1000);
                    }
                }
                catch { }

                // Wait for stderr reader to finish (optional)
                try { await stderrTask; } catch { }
            }
        }



        // Returns index of the 0xD9 byte if a 0xFF 0xD9 sequence exists, otherwise -1.
        private static int FindJpegEnd(List<byte> buffer)
        {
            for (int i = 1; i < buffer.Count; i++)
            {
                if (buffer[i - 1] == 0xFF && buffer[i] == 0xD9)
                    return i;
            }
            return -1;
        }



        [HttpPost("playback")]
        [AllowAnonymous]
        public IActionResult Playback([FromBody] RecordingRequest req)
        {
            if (req == null)
                return BadRequest("Request body is null");

            string basePath = "/mnt/recordings";
            string tempPath = Path.Combine(basePath, "temp");
            Directory.CreateDirectory(tempPath);

            string camPath = Path.Combine(basePath, req.CameraId);

            if (!Directory.Exists(camPath))
                return NotFound($"Camera folder not found: {req.CameraId}");

            var files = Directory.GetFiles(camPath, "*.mp4")
                .Where(f =>
                {
                    var name = Path.GetFileNameWithoutExtension(f);
                    return DateTime.TryParseExact(
                        name,
                        "yyyy-MM-dd_HH-mm-ss",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var t)
                        && t >= req.StartTime && t <= req.EndTime;
                })
                .OrderBy(f => f)
                .ToList();

            if (!files.Any())
                return NotFound("No recordings in selected time range");

            string playbackId = Guid.NewGuid().ToString("N");
            string mergedFile = $"{req.CameraId}_{playbackId}.mp4";
            string mergedPath = Path.Combine(tempPath, mergedFile);

            string listFile = Path.GetTempFileName();
            System.IO.File.WriteAllLines(listFile, files.Select(f => $"file '{f}'"));

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-f concat -safe 0 -i \"{listFile}\" -c copy \"{mergedPath}\"",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            if (process == null)
                return StatusCode(500, "Failed to start ffmpeg");

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                var error = process.StandardError.ReadToEnd();
                return StatusCode(500, error);
            }

            System.IO.File.Delete(listFile);

            return Ok(new
            {
                playbackId,
                videoUrl = $"http://3.108.9.185/recordings/temp/{mergedFile}"
            });
        }


        [HttpDelete("playback/{playbackId}")]
        public IActionResult DeletePlayback(string playbackId)
        {
            var tempPath = "/mnt/recordings/temp";
            var file = Directory
                .GetFiles(tempPath, $"*{playbackId}*.mp4")
                .FirstOrDefault();

            if (file != null)
                System.IO.File.Delete(file);

            return Ok();
        }






    }
}
