using AutoMapper;
using BAL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Constants;
using Model.DomainModel;
using Model.MailTemplateHelper;
using Model.ViewModel;
using MySqlX.XDevAPI.Relational;
using Serilog;
using System.Diagnostics;
using Utils;
using Utils.Interface;
using Utils.UtilModels;

////TO RE-ASSIGN THE TEMPLATE (line 1057-1109) Modified by INDU on March 3,2025

namespace TIMEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ISettingBAL _settingBAL;
        private readonly IWorkBAL _workBAL;
        private readonly IMapper _mapper;
        private readonly IFTPHelpers _ftpHelper;
        private readonly IGeneralBAL _generalBAL;
        public readonly IConfiguration _configuration;

        public WorkController(ILogger<AccountController> logger, ISettingBAL settingBAL, IMapper mapper, IWorkBAL workBAL, IFTPHelpers ftpHelper, IGeneralBAL generalBAL, IConfiguration configuration)
        {
            _logger = logger;
            _settingBAL = settingBAL;
            _mapper = mapper;
            _workBAL = workBAL;
            _ftpHelper = ftpHelper;
            _generalBAL = generalBAL;
            _configuration = configuration;
        }

        #region GO

        [HttpGet("[action]")]
        public ResponseViewModel GO_Get(bool IsActive = true, string Id = "", string GONumber = "", string LocalGONumber = "", string DepartmentId = "")
        {
            try
            {
                List<GOMasterModel> list = _workBAL.GO_Get(IsActive, Id, GONumber, LocalGONumber, DepartmentId);

                if (list != null)
                {
                    List<GOMasterViewModel> responce_List = _mapper.Map<List<GOMasterViewModel>>(list);
                    responce_List.ForEach(x =>
                    {
                        x.TenderList = _mapper.Map<List<TenderMasterViewModel>>(_workBAL.Tender_Get(GOId: x.Id));
                    });

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce_List,
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
        public ResponseViewModel GO_Get(GoFilterModel model)
        {
            try
            {
                if (model.Where == null)
                {
                    model.Where = new GoWhereClauseProperties();
                }

                //string RoleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                //if (!string.Equals(RoleCode, "ADM", StringComparison.CurrentCultureIgnoreCase))
                //{
                //    string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                //    if (!string.IsNullOrWhiteSpace(DepartmentIds))
                //    {
                //        model.DepartmentList = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                //    }
                //}

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentList = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }
                string DivisionId = User.Claims.Where(x => x.Type == Constants.DivisionId)?.FirstOrDefault()?.Value ?? "";

                int TotalCount = 0;
                List<GOMasterModel> list = _workBAL.GO_Get(model, out TotalCount);

                TenderFilterModel tender_filter_Model = new TenderFilterModel();
                if (!string.IsNullOrWhiteSpace(DivisionId))
                {
                    tender_filter_Model.DivisionList = DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }
                tender_filter_Model.DepartmentList = model.DepartmentList;

                int tender_Count = 0;
                List<TenderMasterViewModel> tender_list = _mapper.Map<List<TenderMasterViewModel>>(_workBAL.Tender_Get(tender_filter_Model, out tender_Count));

                if (list != null)
                {
                    List<GOMasterViewModel> responce_List = _mapper.Map<List<GOMasterViewModel>>(list);
                    responce_List.ForEach(x =>
                    {
                        x.TenderList = tender_list.Where(y => y.GoId == x.Id)?.ToList() ?? new List<TenderMasterViewModel>();
                    });
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce_List,
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

        [HttpPost("[action]")]
        public ResponseViewModel GO_SaveUpdate(GOMasterViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    GOMasterModel model = _mapper.Map<GOMasterModel>(rModel);

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    if (model.Id == "" || model.Id == null)
                    {
                        model.Id = Guid.NewGuid().ToString();
                    }

                    string result = _workBAL.GO_SaveUpdate(model);

                    if (result == "-1")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate name",
                        };
                    }
                    else if (result == "-2")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate number",
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
                                Message = "Saved Successfully",
                            };
                        }
                        else
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Deleted Successfully",
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

        #endregion GO

        #region Tender

        [HttpGet("[action]")]
        public ResponseViewModel Tender_Get(bool IsActive = true, string Id = "", string TenderNumber = "", string LocalTenderNumber = "", string DepartmentId = "")
        {
            try
            {
                List<TenderMasterModel> list = _workBAL.Tender_Get(IsActive, Id, TenderNumber, LocalTenderNumber, DepartmentId);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = _mapper.Map<List<TenderMasterViewModel>>(list),
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
        public ResponseViewModel Tender_Get(TenderFilterModel model)
        {
            try
            {
                if (model.Where == null)
                {
                    model.Where = new TenderWhereClauseProperties();
                }

                //string RoleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                //if (!string.Equals(RoleCode, "ADM", StringComparison.CurrentCultureIgnoreCase))
                //{
                //    string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                //    if (!string.IsNullOrWhiteSpace(DepartmentIds))
                //    {
                //        model.DepartmentList = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                //    }
                //}
                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentList = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }

                string UserId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: UserId)?.FirstOrDefault() ?? new AccountUserModel();

                if (model.DivisionList == null || model.DivisionList?.Count == 0)
                {
                    model.DivisionList = currentUser.DivisionIdList?.ToList() ?? new List<string>();
                }
                string RoleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                {
                    model.TenderIds = _workBAL.Tender_Ids_Get_ByContractor(UserId);
                }

                model.RoleCode = RoleCode;

                int TotalCount = 0;

                List<TenderMasterModel> list = _workBAL.Tender_Get(model, out TotalCount);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = _mapper.Map<List<TenderMasterViewModel>>(list),
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

        [HttpPost("[action]")]
        public ResponseViewModel Tender_SaveUpdate(TenderMasterViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    TenderMasterModel model = _mapper.Map<TenderMasterModel>(rModel);

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    if (model.Id == "" || model.Id == null)
                    {
                        model.Id = Guid.NewGuid().ToString();
                    }

                    string result = _workBAL.Tender_SaveUpdate(model);

                    if (result == "-1")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate number",
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
                                Message = "Saved Successfully",
                            };
                        }
                        else
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Deleted Successfully",
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

        // Work create activity added in procedure 'Work_Master_SaveUpdate'
        [HttpGet("[action]")]
        public ResponseViewModel Tender_Create_Work(string TenderId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TenderId))
                {
                    WorkMasterModel model = new WorkMasterModel();

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(model.Id))
                    {
                        model.Id = Guid.NewGuid().ToString();
                    }
                    model.TenderId = TenderId;

                    string result = _workBAL.Work_SaveUpdate(model);

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        List<WorkMasterViewModel> work = _mapper.Map<List<WorkMasterViewModel>>(_workBAL.Work_Get_All(Id: result));

                        WorkMasterViewModel responce = new WorkMasterViewModel();

                        if (work.Count > 0)
                        {
                            responce = work.First();
                            responce.Files = _generalBAL.FileMaster_Get(IsActive: true, TypeId: responce.Id);
                        }

                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = responce,
                            Message = "Saved Successfully",
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
        [HttpGet("[action]")]
        public ResponseViewModel Tender_View_Get(string TenderId, string WorkId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TenderId) || !string.IsNullOrWhiteSpace(WorkId))
                {
                    List<WorkMasterViewModel> work = _mapper.Map<List<WorkMasterViewModel>>(_workBAL.Work_Get_All(Id: WorkId, TenderId: TenderId));

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = work.FirstOrDefault(),
                    };
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
        public ResponseViewModel Tender_Update_Amount(TenderAmountUpdateModel model)
        {
            try
            {
                if (model != null)
                {
                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;

                    string result = _workBAL.Tender_Update_Amount(model);

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = result,
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
        #endregion Tender

        #region Work

        [HttpPost("[action]")] // Type => MilestoneImage
        [Consumes("multipart/form-data")]
        public ResponseViewModel UploadMilestoneFile([FromForm] MilestoneFileUploadFormModel model)
        {
            try
            {
                string fileType = "MilestoneImage";

                DateTime now = DateTime.Now;

                if (model.Files == null || string.IsNullOrWhiteSpace(model.MilestoneId))
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "File, Type and MilestoneId is required, please send valid content"
                    };
                }
                else if (model.Files.Count > 0)
                {
                    List<FileMasterModel> fileMasterList = new List<FileMasterModel>();

                    foreach (var file in model.Files)
                    {
                        FTPModel fTPModel = new FTPModel();
                        fTPModel.file = file;
                        fTPModel.FolderName = fileType;
                        fTPModel.FileName = Convert.ToString(Guid.NewGuid()) + Path.GetExtension(file?.FileName);

                        if (_ftpHelper.UploadFile(fTPModel))
                        {
                            FileMasterModel fileMasterModel = new FileMasterModel();

                            if (string.IsNullOrWhiteSpace(fileMasterModel.Id))
                            {
                                fileMasterModel.Id = Guid.NewGuid().ToString();
                            }
                            fileMasterModel.TypeId = model.MilestoneId;
                            fileMasterModel.Type = fileType;
                            fileMasterModel.FileType = Path.GetExtension(file?.FileName) ?? "";
                            fileMasterModel.OriginalFileName = file?.FileName ?? "";
                            fileMasterModel.SavedFileName = fTPModel.FileName;
                            fileMasterModel.IsActive = true;
                            fileMasterModel.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                            fileMasterModel.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                            fileMasterModel.SavedDate = now;

                            string res = _generalBAL.FileMaster_SaveUpdate(fileMasterModel);
                            if (!string.IsNullOrWhiteSpace(res))
                            {
                                fileMasterList.Add(fileMasterModel);
                            }
                        }
                    }

                    if (fileMasterList.Count > 0)
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = fileMasterList,
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

        [HttpGet("[action]")] // Type => MilestoneImage
        public ResponseViewModel GetMilestoneFiles(string MilestoneId)
        {
            try
            {
                List<FileMasterModel> existRecords = _generalBAL.FileMaster_Get(true, Type: "MilestoneImage", TypeId: MilestoneId);
                if (existRecords?.Count > 0)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = existRecords,
                        Message = "File saved successfully"
                    };
                }
                else
                {
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

        [HttpGet("[action]")] // Type => MilestoneImage
        public ResponseViewModel DeleteMilestoneFile(string FileId, string MilestoneId)
        {
            try
            {
                List<FileMasterModel> existRecords = _generalBAL.FileMaster_Get(true, Id: FileId, Type: "MilestoneImage", TypeId: MilestoneId);
                if (existRecords?.Count > 0)
                {
                    existRecords.ForEach(x =>
                    {
                        x.IsActive = false;
                        x.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                        x.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                        x.SavedDate = DateTime.Now;

                        _generalBAL.FileMaster_SaveUpdate(x);
                    });

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = true,
                        Message = "File saved successfully"
                    };
                }
                else
                {
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


        [HttpPost("[action]")] // Type => LetterOfAcceptance, WorkOrder, AgreementCopy, Other
        [Consumes("multipart/form-data")]
        public ResponseViewModel Work_UploadFile([FromForm] WorkFileUploadFormModel model)
        {
            try
            {
                DateTime now = DateTime.Now;

                if (model.File == null || string.IsNullOrWhiteSpace(model.Type) || string.IsNullOrWhiteSpace(model.WorkId))
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "File, Type and WorkId is required, please send valid content"
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
                        fileMasterModel.TypeId = model.WorkId;
                        fileMasterModel.Type = model.Type;
                        fileMasterModel.FileType = Path.GetExtension(model.File?.FileName) ?? "";
                        fileMasterModel.OriginalFileName = model.File?.FileName ?? "";
                        fileMasterModel.SavedFileName = fTPModel.FileName;
                        fileMasterModel.IsActive = true;
                        fileMasterModel.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                        fileMasterModel.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                        fileMasterModel.SavedDate = now;

                        string res = _generalBAL.FileMaster_SaveUpdate(fileMasterModel);
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            WorkMasterModel? work = _workBAL.Work_Get(Id: model.WorkId).FirstOrDefault();
                            if (work != null)
                            {
                                string ActivitySubject = "";
                                string ActivityMessage = "";
                                if (string.Equals(model.Type.Trim(), UploadTypeCode.Work_LetterOfAcceptance, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    work.LetterOfAcceptanceId = fileMasterModel.Id;
                                    ActivitySubject = WorkActivityMessageConst.LetterOfAcceptance_Upload;
                                    ActivityMessage = WorkActivityMessageConst.LetterOfAcceptance_Upload + ": " + fileMasterModel.OriginalFileName;
                                }
                                else if (string.Equals(model.Type.Trim(), UploadTypeCode.Work_WorkOrder, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    work.WorkOrderId = fileMasterModel.Id;
                                    ActivitySubject = WorkActivityMessageConst.WorkOrder_Upload;
                                    ActivityMessage = WorkActivityMessageConst.WorkOrder_Upload + ": " + fileMasterModel.OriginalFileName;
                                }
                                else if (string.Equals(model.Type.Trim(), UploadTypeCode.Work_AgreementCopy, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    work.AgreementCopyId = fileMasterModel.Id;
                                    ActivitySubject = WorkActivityMessageConst.AgreementCopy_Upload;
                                    ActivityMessage = WorkActivityMessageConst.AgreementCopy_Upload + ": " + fileMasterModel.OriginalFileName;
                                }
                                else if (string.Equals(model.Type.Trim(), UploadTypeCode.Work_Other, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    work.OtherFileId = fileMasterModel.Id;
                                    ActivitySubject = WorkActivityMessageConst.OtherFile_Upload;
                                    ActivityMessage = WorkActivityMessageConst.OtherFile_Upload + ": " + fileMasterModel.OriginalFileName;
                                }

                                #region Save Activity
                                WorkActivityModel workActivity = new WorkActivityModel();
                                workActivity.Id = Guid.NewGuid().ToString();
                                workActivity.ParentType = "TENDER";
                                workActivity.ParentId = work.TenderId;
                                workActivity.Type = "WORK";
                                workActivity.TypeId = work.Id;
                                workActivity.ActivitySubject = ActivitySubject;
                                workActivity.ActivityMessage = ActivityMessage;
                                workActivity.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                                workActivity.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                                workActivity.SavedDate = now;
                                _workBAL.Work_Activity_SaveUpdate(workActivity);
                                #endregion Save Activity

                                work.IsActive = true;
                                work.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                                work.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                                work.SavedDate = now;

                                _workBAL.Work_SaveUpdate(work);

                            }

                            List<FileMasterModel> existRecords = _generalBAL.FileMaster_Get(true, Type: model.Type, TypeId: model.WorkId);
                            if (existRecords?.Count > 0)
                            {
                                existRecords?.ForEach(x =>
                                {
                                    if (x.Id != res)
                                    {
                                        x.IsActive = false;
                                        x.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                                        x.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                                        x.SavedDate = now;

                                        _generalBAL.FileMaster_SaveUpdate(x);
                                    }
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

        #endregion Work

        #region Work Template
        [HttpGet("[action]")]
        //[AllowAnonymous]
        public ResponseViewModel Work_Template_Create(string WorkId = "", string TemplateId = "")
        {
            try
            {
                TemplateModel? template = _settingBAL.Template_Get(Id: TemplateId).FirstOrDefault();

                if (template != null)
                {
                    WorkTemplateMasterModel model = new WorkTemplateMasterModel();

                    model.Id = Guid.NewGuid().ToString();
                    model.TemplateId = TemplateId;
                    model.WorkId = WorkId;
                    model.WorkTemplateName = template.Name;
                    model.WorkTypeId = template.WorkTypeId;
                    model.WorkDurationInDays = template.DurationInDays;
                    model.SubWorkTypeId = template.SubWorkTypeId;
                    model.categoryTypeId = template.categoryTypeId;
                    model.serviceTypeId = template.serviceTypeId;
                    model.Strength = template.Strength;
                    model.TemplateCode = template.TemplateCode;
                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    model.IsActive = true;

                    string workTemplateId = _workBAL.Work_Template_SaveUpdate(model);
                    if (!string.IsNullOrWhiteSpace(workTemplateId))
                    {
                        WorkTemplateMasterModel? model2 = _workBAL.Work_Template_Get(Id: workTemplateId).FirstOrDefault();

                        if (model2 != null)
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = _mapper.Map<WorkTemplateMasterViewModel>(model2),
                                Message = "Action completed successfully"
                            };
                        }
                        else
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = _mapper.Map<WorkTemplateMasterViewModel>(model),
                                Message = "Action completed successfully"
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
                        Message = "Wrong template Id (or) Template not available"
                    };
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
        public ResponseViewModel Work_Template_SaveUpdate(WorkTemplateMasterViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    WorkTemplateMasterModel model = _mapper.Map<WorkTemplateMasterModel>(rModel);

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    if (model.Id == "" || model.Id == null)
                    {
                        model.Id = Guid.NewGuid().ToString();
                    }

                    string result = _workBAL.Work_Template_SaveUpdate(model);

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        if (model.IsActive)
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
                            };
                        }
                        else
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
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
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Data is not valid"
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
        //TEMPLATE RE-ASSIGN  by Indu
        //STARTED

        [HttpGet("[action]")]
        public ResponseViewModel DeleteWorkTemplate(string WorkId)
        {
            try
            {
                if (!string.IsNullOrEmpty(WorkId))
                {
                    string SavedBy = User.Claims.FirstOrDefault(x => x.Type == Constants.UserId)?.Value ?? "";
                    string SavedByUserName = User.Claims.FirstOrDefault(x => x.Type == Constants.Name)?.Value ?? "";
                    DateTime SavedDate = DateTime.Now;

                    string isDeleted = _workBAL.DeleteWorkTemplate(WorkId, SavedBy, SavedByUserName, SavedDate);

                    if (!string.IsNullOrWhiteSpace(isDeleted))
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = isDeleted,
                            Message = "Action completed successfully",
                        };
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
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Data is not valid"
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

        //ENDED

        [HttpGet("[action]")]
        public ResponseViewModel Work_Template_Get(bool IsActive = true, string Id = "", string WorkId = "", string WorkTypeId = "", string TemplateId = "")
        {
            try
            {
                List<WorkTemplateMasterViewModel> template_view_list = new List<WorkTemplateMasterViewModel>();
                List<WorkTemplateMasterModel> masterList = _workBAL.Work_Template_Get(IsActive, Id, WorkId, WorkTypeId, TemplateId); ;
                if (masterList?.Count > 0)
                {
                    template_view_list = _mapper.Map<List<WorkTemplateMasterViewModel>>(masterList);
                }
                template_view_list.ForEach(item =>
                {
                    List<WorkTemplateMilestoneMasterModel> list = _workBAL.Work_Template_Milestone_Master_Get(WorkTemplateId: item.Id, WorkId: item.WorkId);
                    if (list.Count > 0)
                    {
                        item.Milestones = _mapper.Map<List<WorkTemplateMilestoneMasterViewModel>>(list).OrderBy(o => o.OrderNumber).ToList();
                    }
                });

                if (template_view_list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = template_view_list
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

        #endregion Work Template

        #region Work Template Milestone

        [HttpGet("[action]")]
        public ResponseViewModel Work_Template_Milestone_Get(bool IsActive = true, string Id = "", string WorkTemplateId = "", string WorkId = "")
        {
            try
            {
                List<WorkTemplateMilestoneMasterModel> list = _workBAL.Work_Template_Milestone_Master_Get(IsActive, Id, WorkTemplateId, WorkId);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = _mapper.Map<List<WorkTemplateMilestoneMasterViewModel>>(list)?.OrderBy(o => o.OrderNumber)?.ToList(),
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
        public ResponseViewModel Work_Template_Milestone_SaveUpdate(List<WorkTemplateMilestoneMasterViewModel> rModelList)
        {
            try
            {
                if (rModelList != null && rModelList.Count > 0)
                {
                    List<string> milestone_name_list = rModelList.Select(x => x.MilestoneName.Trim()).Distinct().ToList();
                    List<string> milestone_code_list = rModelList.Select(x => x.MilestoneCode.Trim()).Distinct().ToList();

                    if (milestone_name_list.Count != rModelList.Count)
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate name"
                        };

                    }
                    else if (milestone_code_list.Count != rModelList.Count)
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Duplicate code"
                        };
                    }

                    List<WorkTemplateMilestoneMasterModel> modelList = _mapper.Map<List<WorkTemplateMilestoneMasterModel>>(rModelList);

                    // InActive all milestones
                    WorkTemplateMilestoneMasterModel m = modelList?.FirstOrDefault() ?? new WorkTemplateMilestoneMasterModel();
                    m.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    m.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    m.SavedDate = DateTime.Now;
                    _workBAL.Work_Template_Milestone_Master_Delete_All(m);
                    // InActive all milestones
                    if (m.IsSubmitted)
                    {
                        WorkTemplateMasterModel template = new WorkTemplateMasterModel()
                        {
                            Id = m.WorkTemplateId,
                            SavedBy = m.SavedBy,
                            SavedByUserName = m.SavedByUserName,
                            SavedDate = DateTime.Now,
                        };
                        _workBAL.Work_Template_Submit(template);

                        #region Save Activity
                        WorkActivityModel workActivity = new WorkActivityModel();
                        workActivity.Id = Guid.NewGuid().ToString();
                        workActivity.ParentType = "";
                        workActivity.ParentId = "";
                        workActivity.Type = "WORKTEMPLATE";
                        workActivity.TypeId = m.Id;
                        workActivity.ActivitySubject = "Work template submitted";
                        workActivity.ActivityMessage = "Work template submitted";
                        workActivity.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                        workActivity.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                        workActivity.SavedDate = DateTime.Now;
                        _workBAL.Work_Activity_SaveUpdate(workActivity);
                        #endregion Save Activity
                    }

                    modelList?.ForEach(model =>
                    {
                        model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                        model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                        model.SavedDate = DateTime.Now;
                        if (model.Id == "" || model.Id == null)
                        {
                            model.Id = Guid.NewGuid().ToString();
                        }

                        _workBAL.Work_Template_Milestone_Master_SaveUpdate(model);
                        

                    });
                    string? workTemplateId = rModelList.Select(x => x.WorkTemplateId).FirstOrDefault();
                    if (!string.IsNullOrEmpty(workTemplateId))
                    {
                        _workBAL.SetWorkCompletionDate(workTemplateId);
                    }
                    else
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = null,
                            Message = "Data is not valid"
                        };
                    }

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = true,
                        Message = "Action completed successfully"
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Data is not valid"
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
        public ResponseViewModel Work_Template_Milestone_UpdatePercentage(MilestoneUpdateViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    MilestoneUpdateModel model = new MilestoneUpdateModel();

                    model.WorkMilestoneId = rModel.WorkMilestoneId;
                    model.CompletedPercentage = rModel.CompletedPercentage;
                    model.PercentageUpdateNote = rModel.PercentageUpdateNote;
                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;

                    string res = _workBAL.Update_Milestone_Completed_Percentage(model);

                    if (res != null)
                    {
                        if (!string.IsNullOrWhiteSpace(rModel.WorkMilestoneId))
                        {
                            CommentMasterModel comment = new CommentMasterModel();
                            comment.SubjectText = "Milestone";
                            comment.TypeId = rModel.WorkMilestoneId;
                            comment.Type = CommentTypeConst.Milestone;
                            comment.CommentsFrom = CommentTypeConst.Milestone;
                            comment.CommentsText = rModel.PercentageUpdateNote;
                            comment.CommentDate = model.SavedDate;
                            comment.CreatedDate = model.SavedDate;
                            comment.CreatedBy = model.SavedBy;
                            comment.CreatedByUserName = model.SavedByUserName;

                            _workBAL.Comment_SaveUpdate(comment);
                        }

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


        #endregion Work Template Milestone

        #region M-Book
        [HttpPost("[action]")]
        public ResponseViewModel MBook_Get(MBookFilterModel model)
        {
            try
            {
                string UserId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: UserId)?.FirstOrDefault() ?? new AccountUserModel();

                string RoleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";
                if (!string.Equals(RoleCode, "ADM", StringComparison.CurrentCultureIgnoreCase))
                {
                    string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                    if (!string.IsNullOrWhiteSpace(DepartmentIds))
                    {
                        model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                    }
                }
                model.MilestonePercentageShouldBeGreaterThan = true;
                model.RoleId = currentUser.RoleId;

                int TotalCount = 0;
                List<MBookMasterModel> list = _workBAL.Work_MBook_Get(model, UserId, currentUser.RoleCode, UserGroupName, currentUser.DivisionId, out TotalCount);

                if (list != null)
                {
                    List<MBookMasterViewModel> viewList = _mapper.Map<List<MBookMasterViewModel>>(list);

                    viewList.ForEach(x =>
                    {
                        if (x.Files is null)
                        {
                            x.Files = new List<FileMasterModel>();
                        }

                        x.Files.AddRange(_generalBAL.FileMaster_Get(IsActive: true, TypeId: x.Id));
                        x.ApprovalHistory = _workBAL.Work_MBook_Approval_History_Get(MBookId: x.Id);
                    });

                    if (model.IsForApproval && model.Where?.IsActive == true)
                    {
                        viewList = viewList.Where(x => x.IsActionable == true).ToList();
                    }

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = viewList,
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

        [HttpGet("[action]")]
        public ResponseViewModel MBook_GetById(string MBookId)
        {
            try
            {
                MBookMasterModel? model = _workBAL.Work_MBook_GetById(MBookId);

                if (model != null)
                {
                    model.RejectedMbooks = _workBAL.Work_MBook_Get(IsActive: false, WorkTemplateMilestoneId: model.WorkTemplateMilestoneId);
                    string UserId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: UserId)?.FirstOrDefault() ?? new AccountUserModel();
                    string RoleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                    string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

                    MBookMasterViewModel viewModel = _mapper.Map<MBookMasterViewModel>(model);

                    viewModel.Files = _generalBAL.FileMaster_Get(IsActive: true, TypeId: MBookId);
                    viewModel.ApprovalHistory = _workBAL.Work_MBook_Approval_History_Get(MBookId: MBookId);
                    var approvalHistoryList = _workBAL.Work_MBook_Approval_History_Get(MBookId: MBookId);
                    if (approvalHistoryList != null)
                    {
                        foreach (var approvalItem in approvalHistoryList)
                        {
                            approvalItem.Files = _generalBAL.FileMaster_Get(IsActive: true, TypeId: approvalItem.Id);
                        }

                        viewModel.ApprovalHistory = approvalHistoryList;
                    }
                    if (viewModel.ApprovalHistory != null && viewModel.ApprovalHistory.Count > 0)
                    {
                        viewModel.ApprovalHistory = viewModel.ApprovalHistory.OrderByDescending(x => x.LastUpdatedDate).ToList();
                        viewModel.ApprovalHistory.ForEach(x =>
                        {
                            if (x.StatusEnum.ToLower() == "approve")
                            {
                                x.StatusEnum = "RECOMMENDED";
                            }
                        });
                    }

                    if (currentUser.RoleId == viewModel.ActionableRoleId)
                    {
                        if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (viewModel.StatusCode == StatusCodeConst.Saved)
                            {
                                viewModel.IsActionable = true;
                            }
                            else
                            {
                                viewModel.IsActionable = false;
                            }
                        }
                        else if (string.Equals(UserGroupName, UserGroupConst.Engineer, StringComparison.CurrentCultureIgnoreCase))
                        {
                            viewModel.IsActionable = true;
                            viewModel.IsEditable = true;
                        }
                        else
                        {
                            viewModel.IsActionable = true;
                            viewModel.IsEditable = true;
                        }
                    }

                    if (viewModel.StatusCode == StatusCodeConst.Saved)
                    {
                        viewModel.IsActionable = false;
                        viewModel.IsEditable = true;
                    }

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = viewModel,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "MBook not available"
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
        public ResponseViewModel MBook_Get_Files(string MBookId)
        {
            try
            {
                List<SelectListItem> fileTypeList = _workBAL.MbookFileUploadTypeList();
                List<FileMasterModel>? fileList = new List<FileMasterModel>();
                foreach (var file in fileTypeList)
                {
                    fileList.AddRange(_generalBAL.FileMaster_Get(IsActive: true, Type: file.Value, TypeId: MBookId));
                }

                if (fileList != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = fileList,
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "MBook not available"
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
        public ResponseViewModel MBook_Get_FileTypes()
        {
            try
            {
                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Success,
                    Data = _workBAL.MbookFileUploadTypeList(),
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
        public ResponseViewModel MBook_SaveUpdate(MBookMasterViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    string MbookId = "";
                    MBookMasterModel? exist = _workBAL.Work_MBook_GetById(rModel.Id);
                    if (exist != null)
                    {
                        exist.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                        exist.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                        exist.SavedByRoleName = User.Claims.Where(x => x.Type == Constants.RoleName)?.FirstOrDefault()?.Value ?? "";
                        exist.SavedDate = DateTime.Now;
                        exist.WorkNotes = rModel.WorkNotes;
                        exist.Date = rModel.Date;
                        exist.ActualAmount = rModel.ActualAmount;
                        exist.IsActive = true;
                        exist.IsSubmitted = rModel.IsSubmitted;

                        List<StatusMaster> statusMasterList = _settingBAL.Status_Get(Type: StatusTypeConst.MBook);

                        string RoleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
                        string RoleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";

                        if (rModel.StatusCode == "ONLYSAVE")
                        {
                            MbookId = _workBAL.Work_MBook_SaveUpdate(exist);
                        }
                        else if (rModel.IsSubmitted)
                        {
                            StatusMaster? statusMaster = statusMasterList.Find(x => x.StatusCode == StatusCodeConst.Submitted);
                            exist.StatusId = statusMaster?.Id ?? "";
                            exist.StatusName = statusMaster?.StatusName ?? "";
                            exist.StatusCode = statusMaster?.StatusCode ?? "";

                            StatusMaster? statusMasterInprogress = statusMasterList.Find(x => x.StatusCode == StatusCodeConst.PaymentInProgress);
                            exist.PaymentStatusId = statusMasterInprogress?.Id ?? "";
                            exist.PaymentStatusName = statusMasterInprogress?.StatusName ?? "";
                            exist.PaymentStatusCode = statusMasterInprogress?.StatusCode ?? "";

                            //if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                            //{
                            //    ApprovalFlowMaster? approvalFlowMaster = _settingBAL.ApprovalFlow_Get(exist.DepartmentId, RoleId).FirstOrDefault();
                            //    exist.ActionableRoleId = approvalFlowMaster?.ApprovalFlowId ?? RoleId;
                            //}
                            //else
                            //{
                            //    exist.ActionableRoleId = RoleId;
                            //}

                            ApprovalFlowMaster? approvalFlowMaster = _settingBAL.ApprovalFlow_Get(exist.DepartmentId, RoleId).FirstOrDefault();
                            if (approvalFlowMaster == null || approvalFlowMaster?.ApprovalFlowId == null)
                            {
                                return new ResponseViewModel()
                                {
                                    Status = ResponseConstants.Failed,
                                    Data = false,
                                    Message = "Approval flow not configured, Please Try it later!"
                                };
                            }
                            else if (approvalFlowMaster != null && approvalFlowMaster.ApprovalFlowId != null)
                            {
                                exist.ActionableRoleId = approvalFlowMaster.ApprovalFlowId;
                                MbookId = _workBAL.Work_MBook_SaveUpdate(exist);
                            }
                        }
                        else
                        {
                            StatusMaster? statusMaster = statusMasterList.Find(x => x.StatusCode == StatusCodeConst.Saved);
                            exist.StatusId = statusMaster?.Id ?? "";
                            exist.StatusName = statusMaster?.StatusName ?? "";
                            exist.StatusCode = statusMaster?.StatusCode ?? "";
                            StatusMaster? statusMasterNotInitiate = statusMasterList.Find(x => x.StatusCode == StatusCodeConst.PaymentNotInitiated);
                            exist.PaymentStatusId = statusMasterNotInitiate?.Id ?? "";
                            exist.PaymentStatusName = statusMasterNotInitiate?.StatusName ?? "";
                            exist.PaymentStatusCode = statusMasterNotInitiate?.StatusCode ?? "";
                            exist.ActionableRoleId = RoleId;

                            MbookId = _workBAL.Work_MBook_SaveUpdate(exist);
                        }

                        if (!string.IsNullOrWhiteSpace(MbookId))
                        {
                            CommentMasterModel comment = new CommentMasterModel();
                            comment.SubjectText = "M-Book Work Note";
                            comment.TypeId = MbookId;
                            comment.Type = CommentTypeConst.Mbook;
                            comment.CommentsFrom = CommentTypeConst.Mbook;
                            comment.CommentsText = exist.WorkNotes;
                            comment.CommentDate = exist.SavedDate;
                            comment.CreatedDate = exist.SavedDate;
                            comment.CreatedBy = exist.SavedBy;
                            comment.CreatedByUserName = exist.SavedByUserName;

                            _workBAL.Comment_SaveUpdate(comment);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(MbookId))
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = true,
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
                        Message = "Data is not valid"
                    };
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
        public ResponseViewModel MBook_ApproveRejectReturn(MbookApprovalModel model)
        {
            try
            {
                if (model != null)
                {
                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByRoleName = User.Claims.Where(x => x.Type == Constants.RoleName)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    string RoleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
                    string res = _workBAL.Work_Approve_MBook(model, RoleId);
                    if (res == "OK")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = res,
                            Message = "Action completed successfully"
                        };
                    }
                    else if (res == "PERCENTAGE_NOT_100")
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Failed,
                            Data = res,
                            Message = "Completed percentage should be 100"
                        };
                    }

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = res,
                        Message = "Somthing went wrong"
                    };
                }
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Data is not valid"
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
        public ResponseViewModel MBook_Get_ApprovalStatusList(string mbookId)
        {
            try
            {
                MBookMasterModel? mBook = _workBAL.Work_MBook_GetById(mbookId);

                List<SelectListItem> list = new List<SelectListItem>();
                string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

                if (string.Equals(UserGroupName, UserGroupConst.Engineer, StringComparison.CurrentCultureIgnoreCase))
                {
                    //recommended changed to Approve 

                    list.Add(new SelectListItem() { Text = "Approve", Value = StatusCodeConst.Approve });
                    if (mBook != null && mBook.IsReturned == false)
                    {
                        list.Add(new SelectListItem() { Text = "Return", Value = StatusCodeConst.Return });
                    }
                    //list.Add(new SelectListItem() { Text = "Reject", Value = StatusCodeConst.Reject });
                }
                else if (string.Equals(UserGroupName, UserGroupConst.HQ, StringComparison.CurrentCultureIgnoreCase))
                {
                    //recommended changed to Approve by vijay

                    list.Add(new SelectListItem() { Text = "Approve", Value = StatusCodeConst.Approve });
                    list.Add(new SelectListItem() { Text = "Return", Value = StatusCodeConst.Return });
                    //list.Add(new SelectListItem() { Text = "Reject", Value = StatusCodeConst.Reject });
                }

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Success,
                    Data = list,
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
        #endregion M-Book

        #region Comment
        [HttpPost("[action]")]
        public ResponseViewModel Comment_Get(CommentFilterModel model)
        {
            try
            {
                if (model.Where?.Type == "TENDER")
                {
                    model.Ids = new List<string>();
                    model.Types = new List<string>();

                    List<TenderRelatedIdModel> ids = _workBAL.Get_TenderRelatedIds(model.Where?.TypeId ?? "");
                    if (ids.Count > 0)
                    {
                        model.Ids.AddRange(ids.Select(x => x.Id));
                        model.Types.AddRange(ids.Select(x => x.Type));
                    }
                    else
                    {
                        model.Ids.Add(model.Where?.TypeId ?? "");
                        model.Types.Add(model.Where?.Type ?? "");
                    }
                }
                else
                {
                    model.Ids = new List<string>();
                    model.Types = new List<string>();
                    model.Ids.Add(model.Where?.TypeId ?? "");
                    model.Types.Add(model.Where?.Type ?? "");
                }
                if (model.Where != null)
                {
                    model.Ids.Add(model.Where?.TypeId ?? "");
                    model.Types.Add(model.Where?.Type ?? "");

                    model.Where.Type = null;
                    model.Where.TypeId = null;
                }
                int TotalCount = 0;
                List<CommentMasterModel> list = _workBAL.Comment_Get(model, out TotalCount);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = list.OrderByDescending(x => x.CreatedDate).ToList(),
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
        [HttpGet("[action]")]
        public ResponseViewModel Comment_Get(string Type = "", string TypeId = "", string CommentsFrom = "", string ParentId = "")
        {
            try
            {
                List<CommentMasterModel> list = _workBAL.Comment_Get(Type, TypeId, CommentsFrom, ParentId);

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
        [HttpPost("[action]")]
        public ResponseViewModel Comment_SaveUpdate(CommentMasterModel model)
        {
            try
            {
                if (model != null)
                {
                    model.CreatedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.CreatedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.CreatedDate = DateTime.Now;
                    model.CommentDate = DateTime.Now;
                    model.CommentsFrom = model.Type;
                    string result = _workBAL.Comment_SaveUpdate(model);

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = result,
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
        #endregion Comment

        #region Activity
        [HttpPost("[action]")]
        public ResponseViewModel Work_Activity_Get(WorkActivityModel model)
        {
            try
            {

                List<WorkActivityModel> list = _workBAL.Work_Activity_Get(model);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = list
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
        public ResponseViewModel Work_Activity_Get_Post(ActivityFilterModel model)
        {
            try
            {
                if (model.Where?.Type == "TENDER")
                {
                    model.Ids = new List<string>();
                    model.Types = new List<string>();

                    List<TenderRelatedIdModel> ids = _workBAL.Get_TenderRelatedIds(model.Where?.TypeId ?? "");
                    if (ids.Count > 0)
                    {
                        model.Ids.AddRange(ids.Select(x => x.Id));
                        model.Types.AddRange(ids.Select(x => x.Type));
                    }
                    else
                    {
                        model.Ids.Add(model.Where?.TypeId ?? "");
                        model.Types.Add(model.Where?.Type ?? "");
                    }
                }
                else
                {
                    model.Ids = new List<string>();
                    model.Types = new List<string>();
                    model.Ids.Add(model.Where?.TypeId ?? "");
                    model.Types.Add(model.Where?.Type ?? "");
                }
                if (model.Where != null)
                {
                    model.Ids.Add(model.Where.TypeId ?? "");
                    model.Types.Add(model.Where.Type ?? "");

                    model.Where.Type = null;
                    model.Where.TypeId = null;
                }
                if (model.Ids?.Count > 0)
                {
                    model.Ids = model.Ids.Distinct().ToList();
                }
                if (model.Types?.Count > 0)
                {
                    model.Types = model.Types.Distinct().ToList();
                }

                int TotalCount = 0;
                List<WorkActivityModel> list = _workBAL.Work_Activity_Get_By_Ids(model, out TotalCount);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = list.OrderByDescending(x => x.CreatedDate).ToList(),
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
        #endregion Activity

        #region Scheduler Endpoints



        //Modified by Indu on 22-10-2025 for emailnotification on scheduler start and end

        [HttpGet("[action]")]
        [AllowAnonymous]
        public void SchedulerEndPoint()
        {
            try
            {
                string startTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt");
                _workBAL.SendSchedulerNotificationEmail("TIME Scheduler - Started ", "The scheduler process has started successfully at " + startTime);
               

                _workBAL.FetchDepartmentRecords();
                _workBAL.FetchDivisionRecords();
                _workBAL.FetchWorkTypeRecords();

             

                List<int> FromDate = (_configuration.GetSection("TenderGetStartDate")?.Value?.ToString() ?? "01/01/2000").Split("/").Select(x => Convert.ToInt32(x)).ToList();
                //var previousMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
                //TenderDataIntegrationResponceModel responce = _workBAL.FetchAwardedTenders(previousMonthStart, DateTime.Now);
                TenderDataIntegrationResponceModel responce = _workBAL.FetchAwardedTenders(new DateTime(2023, 09, 05), DateTime.Now);
                if (responce.NewContractorList?.Count > 0)
                {
                    CurrentUserModel currentUser = new CurrentUserModel();
                    currentUser.UserName = "System";
                    currentUser.UserId = "";
                        
                    List<EmailModel> mailList = new List<EmailModel>();
                    EmailTemplateModel? template = WorkMailTemplate.GetEmailTemplate(EmailTemplateCode.UserCreate);
                    if (template is not null)
                    {
                        responce.NewContractorList.ForEach(contractor =>
                        {
                            EmailModel mail = new EmailModel();

                            mail.Body = template.Body;
                            mail.Subject = template.Subject;
                            mail.To = new List<string>() { contractor.Email };
                            mail.BodyPlaceHolders = new Dictionary<string, string>() {
                            { "{RECIPIENTFIRSTNAME}", contractor.FirstName },
                            { "{RECIPIENTLASTNAME}", contractor.LastName },
                            { "{USERNAME}", contractor.Email },
                            { "{PASSWORD}", contractor.Password }
                        };

                            mailList.Add(mail);
                        });

                        if (mailList.Count > 0)
                        {
                            _generalBAL.SendMessage(mailList, new List<SMSModel>(), currentUser);
                        }
                    }
                }
                _workBAL.UpdateDatedifference();
                _workBAL.UpdateCameraDatabase();
                _workBAL.SendSchedulerNotificationEmail("TIME Scheduler - Completed ", "The scheduler process has completed successfully at " + startTime);


            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                _workBAL.SendSchedulerNotificationEmail("TIME Scheduler  - Failed", $"Scheduler failed with error: {ex.Message}");

            }

        }




        [HttpGet("[action]")]
        [AllowAnonymous]
        public void CCTVScheduler()
        {
            try
            {
                string startTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt");
                _workBAL.SendSchedulerNotificationEmail("CCTV Scheduler - Started ", "The scheduler process has started successfully at " + startTime);



                _workBAL.UpdateCameraDatabase();
                _workBAL.SendSchedulerNotificationEmail("CCTV Scheduler - Completed ", "The scheduler process has completed successfully at " + startTime);


            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                _workBAL.SendSchedulerNotificationEmail("CCTV Scheduler  - Failed", $"Scheduler failed with error: {ex.Message}");

            }

        }





        [HttpGet("[action]")]
        [AllowAnonymous]
        public void MakeAlert()
        {
            List<AlertTenderModel> work_list = _workBAL.Alert_GetWork();
            if (work_list.Count > 0)
            {


                foreach (AlertTenderModel alert in work_list)
                {

                }
            }
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public void SendTestMail()
        {
            List<EmailModel> email = new List<EmailModel>() {
                new EmailModel(){Body = "Test Mail body 1", Subject = "Test Mail subject 1", To = new List<string>() { "vsappsolutions@gmail.com" } },
                new EmailModel(){Body = "Test Mail body 2", Subject = "Test Mail subject 2", To = new List<string>() { "vsappsolutions@gmail.com" } },
                new EmailModel(){Body = "Test Mail body 3", Subject = "Test Mail subject 3", To = new List<string>() { "vsappsolutions@gmail.com" } }
            };
            _generalBAL.SendMessage(email, new List<SMSModel>(), new CurrentUserModel());
        }
        #endregion Scheduler Endpoints


        [HttpGet("[action]")]
        public ResponseViewModel Review_Type_Get()
        {
            try
            {
                List<SelectListItem> list = new List<SelectListItem>()
                {
                    new SelectListItem(){ Text = "M-Book", Value = CommentTypeConst.Mbook },
                    new SelectListItem(){ Text = "Tender", Value = CommentTypeConst.Tender },
                    new SelectListItem(){ Text = "Milestone", Value = CommentTypeConst.Milestone }
                };

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

        #region Not In Use
        // Not in use
        [HttpGet("[action]")]
        public ResponseViewModel Work_Get(bool IsActive = true, string Id = "", string TenderId = "", string WorkNumber = "")
        {
            try
            {
                List<WorkMasterModel> list = _workBAL.Work_Get(IsActive, Id, TenderId, WorkNumber);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = _mapper.Map<List<WorkMasterViewModel>>(list),
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
        // Not in use
        [HttpPost("[action]")]
        public ResponseViewModel Work_Get(WorkFilterModel model)
        {
            try
            {
                int TotalCount = 0;
                List<WorkMasterModel> list = _workBAL.Work_Get(model, out TotalCount);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = _mapper.Map<List<WorkMasterViewModel>>(list),
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
        // Not in use
        [HttpPost("[action]")]
        public ResponseViewModel Work_SaveUpdate(WorkMasterViewModel rModel)
        {
            try
            {
                if (rModel != null)
                {
                    WorkMasterModel model = _mapper.Map<WorkMasterModel>(rModel);

                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    if (model.Id == "" || model.Id == null)
                    {
                        model.Id = Guid.NewGuid().ToString();
                    }

                    string result = _workBAL.Work_SaveUpdate(model);

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        if (model.IsActive)
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
                            };
                        }
                        else
                        {
                            return new ResponseViewModel()
                            {
                                Status = ResponseConstants.Success,
                                Data = result,
                                Message = "Action completed successfully",
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
                else
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Failed,
                        Data = null,
                        Message = "Data is not valid"
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

        #endregion Not In Use

        [HttpGet("[action]")]
        [AllowAnonymous]
        public ResponseViewModel GetTenderData()
        {
            try
            {
                List<TenderDataModel> list = _workBAL.GetTender();

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = _mapper.Map<List<TenderDataModel>>(list),
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
        public ResponseViewModel GetTender_byDivision(string Division)
        {
            try
            {
                List<TenderDataModel> list = _workBAL.GetTenderbyDivision(Division);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = _mapper.Map<List<TenderDataModel>>(list),
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
        public ResponseViewModel GetTender_byContractor(string Contractor)
        {
            try
            {
                List<TenderDataModel> list = _workBAL.GetTenderbyContractor(Contractor);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = _mapper.Map<List<TenderDataModel>>(list),
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

        public ResponseViewModel Tender_Verified(string TenderId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TenderId))
                {
                   string work = _workBAL.IsTenderverified( TenderId: TenderId);

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = work,
                    };
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
     
       

    }
}
