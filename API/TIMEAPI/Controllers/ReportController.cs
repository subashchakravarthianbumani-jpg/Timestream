using AutoMapper;
using BAL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Constants;
using Model.DomainModel;
using Model.ViewModel;
using Serilog;
using Utils.Interface;

namespace TIMEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ISettingBAL _settingBAL;
        private readonly IWorkBAL _workBAL;
        private readonly IMapper _mapper;
        private readonly IFTPHelpers _ftpHelper;
        private readonly IGeneralBAL _generalBAL;

        public ReportController(ILogger<AccountController> logger, ISettingBAL settingBAL, IMapper mapper, IWorkBAL workBAL, IFTPHelpers ftpHelper, IGeneralBAL generalBAL)
        {
            _logger = logger;
            _settingBAL = settingBAL;
            _mapper = mapper;
            _workBAL = workBAL;
            _ftpHelper = ftpHelper;
            _generalBAL = generalBAL;
        }

        [HttpPost("[action]")]
        public ResponseViewModel Work_Get(WorkFilterModel model)
        {
            try
            {
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

        [HttpPost("[action]")]
        public ResponseViewModel Milestone_Get(MilestoneFilterModel model)
        {
            try
            {
                string UserId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: UserId)?.FirstOrDefault() ?? new AccountUserModel();
                if (model.DivisionList == null || model.DivisionList?.Count == 0)
                {
                    model.DivisionList = currentUser.DivisionIdList?.ToList() ?? new List<string>();
                }
                int TotalCount = 0;
                List<MilestoneReportModel> list = _workBAL.Milestone_Get(model, out TotalCount);

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

        [HttpPost("[action]")]
        public ResponseViewModel MBook_Get(MBookReportFilterModel model)
        {
            try
            {
                string UserId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: UserId)?.FirstOrDefault() ?? new AccountUserModel();
                if (model.DivisionList == null || model.DivisionList?.Count == 0)
                {
                    model.DivisionList = currentUser.DivisionIdList?.ToList() ?? new List<string>();
                }


                
                string RoleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                if (string.Equals(RoleCode, "CONTRACTOR", StringComparison.CurrentCultureIgnoreCase))
                {
                    model.MbookIds = _workBAL.Mbook_Id_Get_ByContractor(UserId);
                }

                model.RoleCode = RoleCode;
                model.RoleId = currentUser.RoleId;


                int TotalCount = 0;
                List<MBookReportModel> list = _workBAL.Work_MBook_Report_Get(model, out TotalCount);

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

        [HttpPost("[action]")]
        public ResponseViewModel GO_Get(GoFilterModel model)
        {
            try
            {
                string UserId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: UserId)?.FirstOrDefault() ?? new AccountUserModel();
                
                int TotalCount = 0;
                List<GOReportViewModel> list = _workBAL.GO_Report_Get(model, out TotalCount);

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

        [HttpGet("[action]")]
        public ResponseViewModel Milestone_Get(bool IsActive = true, string Id = "", string WorkTemplateId = "", string WorkId = "",
            string TenderId = "", string DivisionId = "", string MilestoneStatusId = "", string PaymentStatusId = "")
        {
            try
            {
                List<WorkTemplateMilestoneMasterModel> list = _workBAL.Work_Template_Milestone_Master_Get(IsActive, Id, WorkTemplateId, WorkId, TenderId, DivisionId, MilestoneStatusId, PaymentStatusId);

                if (list != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = _mapper.Map<List<WorkTemplateMilestoneMasterViewModel>>(list)?.OrderBy(o => o.WorkId).OrderBy(oo => oo.OrderNumber)?.ToList(),
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
        public ResponseViewModel MBook_Get(bool IsActive = true, string Id = "", string WorkTemplateMilestoneId = "", string ActionableRoleId = "",
            string DivisionId = "", string StatusId = "", string WorkId = "", string TenderId = "")
        {
            try
            {
                List<MBookMasterModel> list = _workBAL.Work_MBook_Get(IsActive, Id, WorkTemplateMilestoneId, ActionableRoleId, DivisionId, StatusId, WorkId, TenderId);

                if (list != null)
                {
                    List<MBookMasterViewModel> viewList = _mapper.Map<List<MBookMasterViewModel>>(list);

                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
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

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<ResponseViewModel> AlertsforBGProcess()
        {
            try
            {
                string UserId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: UserId)?.FirstOrDefault() ?? new AccountUserModel();
                List<AlertMasterModel> model = await _generalBAL.GetAlertsforBGProcess(currentUser.UserId, currentUser.FirstName + ' ' + currentUser.LastName);

                if (model != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = model,
                        TotalRecordCount = model.Count()
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
        public ResponseViewModel Alert_Filter_Form()
        {
            try
            {
                ReportFilterForm model = new();

                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();
                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";

                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    List<SelectListItem> list = _settingBAL.GetDepartmentSelectList();
                    model.departments = list.Where(x => DepartmentIds.Split(",").Select(x => x.Trim()).ToList().Contains(x.Value)).ToList();
                }
                string DivisionIds = User.Claims.Where(x => x.Type == Constants.DivisionId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(DivisionIds))
                {
                    List<SelectListItem> list = _settingBAL.GetDivisionSelectList();
                    model.divisions = list.Where(x => DivisionIds.Split(",").Select(x => x.Trim()).ToList().Contains(x.Value)).ToList();
                }

                model.districts = _settingBAL.GetDistrictSelectList();

                var statuses = _settingBAL.Status_Get();
                if (statuses != null)
                {
                    model.statusList = new();
                    model.statusList.AddRange(statuses.Select(x => new SelectListItem { Text = x.StatusName, Value = x.Id }));
                }
                model.types = new List<SelectListItem>
                {
                    new SelectListItem { Text = "GO", Value = "GO" },
                    new SelectListItem { Text = "Tender", Value = "TENDER" },
                    new SelectListItem { Text = "Milestone", Value = "MILESTONE" },
                    new SelectListItem { Text = "M-Book", Value = "MBOOK" }
                };
                
                var schemes= _settingBAL.Scheme_GeT();
                var PackageNo = _settingBAL.PacakgeNo_Get();
                if (schemes != null)
                {
                    model.schemes = new();
                    model.schemes.AddRange(schemes.Select(x => new SelectListItem { Text = x.SchemeName, Value = x.SchemeId }));
                    model.schemes.Add(new SelectListItem
                    {
                        Text = "Others",
                        Value = "-1"
                    });
                }
                if (PackageNo != null)
                {
                    model.GoPackageNo = new();
                    model.GoPackageNo.AddRange(PackageNo.Select(x => new SelectListItem { Text = x.Go_Package_No, Value = x.Go_Package_No }));
                }
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

    }
}
