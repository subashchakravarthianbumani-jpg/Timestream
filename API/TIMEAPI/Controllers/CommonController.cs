using AutoMapper;
using BAL;
using BAL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model.Constants;
using Model.DomainModel;
using Model.ViewModel;
using Serilog;
using System.Reflection.Metadata;
using TIMEAPI.Infrastructure;
using Utils;
using Utils.Interface;

namespace TIMEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountBAL _accountBAL;
        private readonly ISettingBAL _settingsBAL;
        private readonly IGeneralBAL _generalBAL;
        private readonly IMapper _mapper;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IFTPHelpers _ftpHelper;
        private GeneralDetail _confg;

        public CommonController(ILogger<AccountController> logger,
            IAccountBAL accountBAL,
            IMapper mapper,
            IJwtAuthManager jwtAuthManager,
            ISettingBAL settingsBAL,
            IGeneralBAL generalBAL,
            IFTPHelpers ftpHelper, IOptions<GeneralDetail> confg)
        {
            _logger = logger;
            _accountBAL = accountBAL;
            _settingsBAL = settingsBAL;
            _mapper = mapper;
            _jwtAuthManager = jwtAuthManager;
            _ftpHelper = ftpHelper;
            _generalBAL = generalBAL;
            _confg = confg.Value;
        }

        #region File Upload/Download

        [HttpPost("[action]")]
        [Consumes("multipart/form-data")] // MBookDocument
        [AllowAnonymous]
        public ResponseViewModel UploadFile([FromForm] FileUploadFormModel model)
        
        {
            try
            {
                if (model.File == null || string.IsNullOrWhiteSpace(model.Type) || string.IsNullOrWhiteSpace(model.TypeId))
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "File, Type and TypeId is required, please send valid content"
                    };
                }
                else
                {
                    FTPModel fTPModel = new FTPModel();
                    fTPModel.file = model.File;
                    fTPModel.FolderName = model.Type;
                    fTPModel.FileName = Convert.ToString(Guid.NewGuid()) + Path.GetExtension(model.File?.FileName);

                    if (_ftpHelper.UploadFile(fTPModel))
                    {
                        FileMasterModel fileMasterModel = new FileMasterModel();

                        if (string.IsNullOrWhiteSpace(fileMasterModel.Id))
                        {
                            fileMasterModel.Id = Guid.NewGuid().ToString();
                        }
                        fileMasterModel.TypeId = model.TypeId;
                        fileMasterModel.Type = model.Type;
                        fileMasterModel.TypeName = model.TypeName;
                        fileMasterModel.FileType = Path.GetExtension(model.File?.FileName) ?? "";
                        fileMasterModel.OriginalFileName = model.File?.FileName ?? "";
                        fileMasterModel.SavedFileName = fTPModel.FileName;
                        fileMasterModel.IsActive = true;
                        fileMasterModel.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                        fileMasterModel.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                        fileMasterModel.SavedDate = DateTime.Now;

                        string res = _generalBAL.FileMaster_SaveUpdate(fileMasterModel);
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            //List<FileMasterModel> existRecords = _generalBAL.FileMaster_Get(true, Type: model.Type, TypeId: model.TypeId);
                            //if (existRecords?.Count > 0)
                            //{
                            //    existRecords?.ForEach(x =>
                            //    {
                            //        if (x.Id != res)
                            //        {
                            //            x.IsActive = false;
                            //            x.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                            //            x.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                            //            x.SavedDate = DateTime.Now;

                            //            _generalBAL.FileMaster_SaveUpdate(x);
                            //        }
                            //    });
                            //}

                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = fileMasterModel,
                                Message = "File saved successfully"
                            };
                        }
                    }

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Failed to save file"
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
        public ResponseViewModel FileMaster_SaveUpdate(FileMasterModel model)
        {
            try
            {
                FileMasterModel? fileObj = _generalBAL.FileMaster_Get(true, Id: model.Id).FirstOrDefault();
                if (fileObj != null)
                {
                    //fileObj.Type = model.Type;
                    //fileObj.TypeName = model.TypeName;
                    //fileObj.TypeId = model.TypeId;

                    fileObj.IsActive = model.IsActive;

                    fileObj.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    fileObj.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    fileObj.SavedDate = DateTime.Now;

                    string res = _generalBAL.FileMaster_SaveUpdate(fileObj);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = res,
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

        [HttpPost("[action]")]
        [Consumes("multipart/form-data")]
        public ResponseViewModel UploadUserProfile([FromForm] UserProfileUploadFormModel model)
        {
            try
            {
                if (model.File == null || string.IsNullOrWhiteSpace(model.UserId))
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "File  and UserId is required, please send valid content"
                    };
                }
                else
                {
                    FileUploadFormModel _model = new FileUploadFormModel();

                    _model.File = model.File;
                    _model.TypeId = model.UserId;
                    _model.Type = FTPFolderConstants.ProfileImagesFolder;

                    FTPModel fTPModel = new FTPModel();
                    fTPModel.file = _model.File;
                    fTPModel.FolderName = _model.Type;
                    fTPModel.FileName = Convert.ToString(Guid.NewGuid()) + Path.GetExtension(_model.File?.FileName);

                    if (_ftpHelper.UploadFile(fTPModel))
                    {
                        FileMasterModel fileMasterModel = new FileMasterModel();

                        if (string.IsNullOrWhiteSpace(fileMasterModel.Id))
                        {
                            fileMasterModel.Id = Guid.NewGuid().ToString();
                        }
                        fileMasterModel.TypeId = _model.TypeId;
                        fileMasterModel.Type = _model.Type;
                        fileMasterModel.FileType = Path.GetExtension(_model.File?.FileName) ?? "";
                        fileMasterModel.OriginalFileName = _model.File?.FileName ?? "";
                        fileMasterModel.SavedFileName = fTPModel.FileName;
                        fileMasterModel.IsActive = true;
                        fileMasterModel.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                        fileMasterModel.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                        fileMasterModel.SavedDate = DateTime.Now;

                        string res = _generalBAL.FileMaster_SaveUpdate(fileMasterModel);
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            List<FileMasterModel> existRecords = _generalBAL.FileMaster_Get(true, Type: _model.Type, TypeId: _model.TypeId);
                            if (existRecords?.Count > 0)
                            {
                                existRecords?.ForEach(x =>
                                {
                                    x.IsActive = false;
                                    x.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                                    x.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                                    x.SavedDate = DateTime.Now;

                                    _generalBAL.FileMaster_SaveUpdate(x);
                                });
                            }

                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = fileMasterModel,
                                Message = "File saved successfully"
                            };
                        }
                    }

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Failed to save file"
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
        public IActionResult DownloadImage(string fileId)
        {
            try
            {
                FileMasterModel? existRecord = _generalBAL.FileMaster_Get(true, Id: fileId).FirstOrDefault();
                if (existRecord != null)
                {
                    FileInfo info = new FileInfo(existRecord.SavedFileName);

                    string base64encodedstring = _ftpHelper.DownloadFile(new FTPModel() { FileName = existRecord.SavedFileName });
                    byte[] bytes = Convert.FromBase64String(base64encodedstring);
                    MemoryStream memory = new MemoryStream(bytes);

                    memory.Position = 0;
                    string MimeType = StringFunctions.GetMimeType(info.Extension);

                    return File(memory, MimeType, existRecord.OriginalFileName);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                return null;
            }
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public IActionResult File(string fileName)
        {
            try
            {
                FileMasterModel? existRecord = _generalBAL.FileMaster_Get(true, SavedFileName: fileName).FirstOrDefault();
                if (existRecord != null)
                {
                    FileInfo info = new FileInfo(fileName);

                    string base64encodedstring = _ftpHelper.DownloadFile(new FTPModel() { FileName = fileName });

                    byte[] bytes = Convert.FromBase64String(base64encodedstring);
                    MemoryStream memory = new MemoryStream(bytes);

                    memory.Position = 0;
                    string MimeType = StringFunctions.GetMimeType(info.Extension);

                    return File(memory, MimeType, existRecord.OriginalFileName);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return default;
            }
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public string ImageSource_Base64(string fileName)
        {
            try
            {
                FileMasterModel? existRecord = _generalBAL.FileMaster_Get(true, SavedFileName: fileName).FirstOrDefault();
                if (existRecord != null)
                {
                    FileInfo info = new FileInfo(fileName);

                    string MimeTypePrefix = "data:" + StringFunctions.GetMimeType(info.Extension) + ";base64, ";

                    string base64encodedstring = _ftpHelper.DownloadFile(new FTPModel() { FileName = fileName });

                    string image = MimeTypePrefix + base64encodedstring;

                    return image;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return default;
            }
        }

        #endregion File Upload/Download

        #region Key Contacts

        [HttpGet("[action]")]
        public ResponseViewModel KeyContacts()
        {
            try
            {
                string UserId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                AccountUserModel currentUser = _settingsBAL.User_Get(true, UserId: UserId)?[0] ?? new AccountUserModel();
                List<AccountUserModel> users = _generalBAL.GetKeyContacts(UserId, currentUser.RoleCode, currentUser.DivisionId, currentUser.DepartmentId);
                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Success,
                    Data = users,
                    TotalRecordCount = users?.Count ?? 0,
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


        #endregion Key Contacts

        #region QuickLink
        [HttpGet("[action]")]
        public ResponseViewModel QuickLink()
        {
            try
            {
                string UserId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                AccountUserModel currentUser = _settingsBAL.User_Get(true, UserId: UserId)?[0] ?? new AccountUserModel();
                List<QuickLinkMasterModel> list = _settingsBAL.QuickLink_Get_For_View(currentUser.UserGroup);
                List<QuickLinkMasterViewModel> viewList = new List<QuickLinkMasterViewModel>();
                if (list?.Count > 0)
                {
                    viewList = _mapper.Map<List<QuickLinkMasterViewModel>>(list);
                }
                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Success,
                    Data = viewList,
                    TotalRecordCount = viewList?.Count ?? 0,
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

        #endregion QuickLink

        #region RecordHistory
        [HttpPost("[action]")]
        public ResponseViewModel Record_History_Get(TableFilterModel model)
        {
            try
            {
                int TotalCount = 0;

                List<RecordHistoryModel> list = _generalBAL.GetRecordHistory(model, out TotalCount);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = list,
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
        #endregion RecordHistory

        #region Email SMS Log
        [HttpPost("[action]")]
        public ResponseViewModel Email_SMS_Log_Get(MailSMSLog model)
        {
            try
            {
                int TotalCount = 0;

                List<MailSMSLog> list = _generalBAL.MailSMSLog_Get(model);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = list,
                        TotalRecordCount = list.Count
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
        #endregion Email SMS Log

        [HttpGet("[action]")]
        [AllowAnonymous]
        public string DecryptPassword(string PasswordString)
        {
            try
            {
                return EncryptDecrypt.Decrypt(PasswordString);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return default;
            }
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public DateTime GetUTCDate()
        {
            try
            {
                var asdasd = DateTime.Now;

                return asdasd;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return default;
            }
        }


        [HttpGet("[action]")]
        [AllowAnonymous]
        public GeneralDetail GetGeneralDetails()
        {
            try
            {
                return _confg;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return default;
            }
        }


    }
}
