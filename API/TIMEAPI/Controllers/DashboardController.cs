using AutoMapper;
using BAL.Interface;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Constants;
using Model.DomainModel;
using Model.ViewModel;
using Serilog;
using System.Diagnostics;
using System.IO;
using Utils.Interface;

namespace TIMEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ISettingBAL _settingBAL;
        private readonly IWorkBAL _workBAL;
        private readonly IMapper _mapper;
        private readonly IFTPHelpers _ftpHelper;

        private readonly IGeneralBAL _generalBAL;

        public DashboardController(ILogger<AccountController> logger, ISettingBAL settingBAL, IMapper mapper,
            IWorkBAL workBAL, IFTPHelpers ftpHelper, IGeneralBAL generalBAL)
        {
            _logger = logger;
            _settingBAL = settingBAL;
            _mapper = mapper;
            _workBAL = workBAL;

            _ftpHelper = ftpHelper;
            _generalBAL = generalBAL;
        }
        // Modified by indu for dashboard record counts
        #region Dashboard
        [HttpPost("[action]")]
        public ResponseViewModel Dashboard_RecordCount_Get(DashboardFilterModel model)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
                string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }

                DashboardRecordCountCardModel responce = _workBAL.DashboardGetCountData(model, roleCode, UserGroupName, userId, roleId);



                if (responce != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
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
        public ResponseViewModel Dashboard_Tender_Chart(DashboardFilterModel model)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }

                TenderChartModel responce = _workBAL.DashboardGet_TenderChart(model, roleCode, userId, roleId);

                if (responce != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
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
        public ResponseViewModel Dashboard_Mbook_Chart(DashboardFilterModel model, string RoleCode, string UserId, string RoleId)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }

                TenderChartModel responce = _workBAL.DashboardGet_MbookChart(model, roleCode, userId, roleId);

                if (responce != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
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
        #endregion Dashboard

        #region Alert
        [HttpPost("[action]")]
        public ResponseViewModel Alert_Get(AlertFilterModel model)
        {
            try
            {
                string UserId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: UserId)?.FirstOrDefault() ?? new AccountUserModel();
                if (model.DivisionIds == null || model.DivisionIds?.Count == 0)
                {
                    model.DivisionIds = currentUser.DivisionIdList?.ToList() ?? new List<string>();
                }
                model.RoleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";

                int TotalCount = 0;
                List<AlertMasterModel> list = _workBAL.Alert_Get(model, out TotalCount);

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
        public ResponseViewModel Alert_Resolve(ResolveAlertModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.Id) && !string.IsNullOrWhiteSpace(model.UpdatedNote))
                {
                    model.SavedBy = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                    model.SavedByUserName = User.Claims.Where(x => x.Type == Constants.Name)?.FirstOrDefault()?.Value ?? "";
                    model.SavedDate = DateTime.Now;
                    bool result = _generalBAL.Alert_Resolve(model.Id, model.SavedBy, model.SavedByUserName, model.UpdatedNote);

                    if (result)
                    {
                        return new ResponseViewModel()
                        {
                            Status = ResponseConstants.Success,
                            Data = result,
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

        #endregion Alert



        #region dashboardcounts

        [HttpPost("[action]")]
        public ResponseViewModel Dashboard_Division_Count_Get(DashboardFilterModel model)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
                string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    List<string> UserDivision = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }


                List<string> selectedDivision = model.DivisionIds;


                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }

                model.DivisionIds = model.DivisionIds.Where(x => (selectedDivision != null && selectedDivision.Contains(x)) || selectedDivision == null || selectedDivision.Count() == 0).ToList();

                List<DashboardDivisionCountModel> responce = _workBAL.Dashboard_Dvision_Count(model, roleCode, userId, roleId);

                if (responce != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
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
        public ResponseViewModel Dashboard_Division_district_Count_Get(DashboardFilterModel model)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
                string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    List<string> UserDivision = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }


                List<string> selectedDivision = model.DivisionIds;
                List<string> selectedDistrict = model.DistrictIds;


                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }

                model.DivisionIds = model.DivisionIds.Where(x => (selectedDivision != null && selectedDivision.Contains(x)) || selectedDivision == null || selectedDivision.Count() == 0).ToList();

                if (model.DistrictIds != null)
                {
                    model.DistrictIds = model.DistrictIds.Where(x => (selectedDistrict != null && selectedDistrict.Contains(x)) || selectedDistrict == null || selectedDistrict.Count() == 0).ToList();
                }

                List<DashboardDivisionCountModel> responce = _workBAL.Dashboard_Division_district_Count(model, roleCode, userId, roleId);

                if (responce != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
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
        public ResponseViewModel Dashboard_Scheme_Count_Get(DashboardFilterModel model)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
                string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    List<string> UserDivision = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }


                List<string> selectedDivision = model.DivisionIds;


                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }

                model.DivisionIds = model.DivisionIds.Where(x => (selectedDivision != null && selectedDivision.Contains(x)) || selectedDivision == null || selectedDivision.Count() == 0).ToList();

                List<DashboardDivisionCountModel> responce = _workBAL.Dashboard_Scheme_Count(model, roleCode, userId, roleId);

                if (responce != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
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
        public ResponseViewModel Dashboard_Mbook_Count_Get(DashboardFilterModel model)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
                string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    List<string> UserDivision = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }


                List<string> selectedDivision = model.DivisionIds;
                List<string> selectedDistrict = model.DistrictIds;


                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }

                model.DivisionIds = model.DivisionIds.Where(x => (selectedDivision != null && selectedDivision.Contains(x)) || selectedDivision == null || selectedDivision.Count() == 0).ToList();

                if (model.DistrictIds != null)
                {
                    model.DistrictIds = model.DistrictIds.Where(x => (selectedDistrict != null && selectedDistrict.Contains(x)) || selectedDistrict == null || selectedDistrict.Count() == 0).ToList();
                }

                List<DashboardDivisionCountModel> responce = _workBAL.Dashboard_mbook_Count(model, roleCode, userId, roleId);

                if (responce != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
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
        public ResponseViewModel GetDivision_Mbook_Count(DashboardFilterModel model)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
                string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    List<string> UserDivision = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }


                List<string> selectedDivision = model.DivisionIds;
                List<string> selectedDistrict = model.DistrictIds;


                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }

                model.DivisionIds = model.DivisionIds.Where(x => (selectedDivision != null && selectedDivision.Contains(x)) || selectedDivision == null || selectedDivision.Count() == 0).ToList();

                if (model.DistrictIds != null)
                {
                    model.DistrictIds = model.DistrictIds.Where(x => (selectedDistrict != null && selectedDistrict.Contains(x)) || selectedDistrict == null || selectedDistrict.Count() == 0).ToList();
                }

                List<DashboardDivisionCountModel> responce = _workBAL.GetDivision_Mbook_Count(model, roleCode, userId, roleId);

                if (responce != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
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




        #endregion dashboardcounts


        #region cameralive

        [HttpPost("DashboardcameraLive")]
        public async Task<ResponseViewModel> DashboardcameraLive(DashboardCameraFilterModel model)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
                string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    List<string> UserDivision = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }


                List<string> selectedDivision = model.DivisionIds;

                List<string> selectedDistrict = model.DistrictIds;


                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DistrictId))
                {
                    model.DistrictIds = currentUser.DistrictId.Split(",").Select(x => x.Trim()).ToList();
                }
                int TotalCount = 0;



                if (model.DivisionIds != null && selectedDivision != null && selectedDivision.Any())
                {
                    model.DivisionIds =
                        model.DivisionIds
                        .Where(d => selectedDivision.Contains(d))
                        .ToList();
                }

                // District filter
                if (model.DistrictIds != null && selectedDistrict != null && selectedDistrict.Any())
                {
                    model.DistrictIds =
                        model.DistrictIds
                        .Where(d => selectedDistrict.Contains(d))
                        .ToList();
                }

                // model.DivisionIds = model.DivisionIds.Where(x => (selectedDivision != null && selectedDivision.Contains(x)) || selectedDivision == null || selectedDivision.Count() == 0).ToList();



                List<DashboardCameraModel> responce = new();
                
                if (model.type == "Camera")
                {
                    (responce, TotalCount) = _workBAL.Dashboard_LiveStreaming(model);
                    responce = responce;
                    TotalCount = TotalCount;

                }
                else if (model.type == "Alert")
                {

                    model.Skip = 0;
                    model.Take = int.MaxValue;



                    (responce, TotalCount) = _workBAL.Dashboard_LiveStreaming(model);
                    responce = responce;
                    TotalCount = TotalCount;


                    var rtspStatus = await _workBAL.GetRtspStatusFromMediaMTX();

                   
                    foreach (var cam in responce)
                    {
                        if (string.IsNullOrWhiteSpace(cam.LiveUrl) ||
                            !Uri.TryCreate(cam.LiveUrl, UriKind.Absolute, out var uri))
                        {
                            cam.IsRtspValid = false;
                            //cam.IsRtspLive = false;
                            continue;
                        }

                        var path = uri.AbsolutePath.Trim('/');

                        if (!rtspStatus.TryGetValue(path, out var isReady))
                        {
                            cam.IsRtspValid = false;
                            //cam.IsRtspLive = false;
                        }
                        else
                        {
                            cam.IsRtspValid = true;
                            cam.IsRtspValid = isReady;
                        }
                    }


                    responce = responce
                        .Where(c => !c.IsRtspValid || !c.IsRtspLive)
                        .ToList();


                    return new ResponseViewModel
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
                        TotalRecordCount = responce.Count
                    };
                }


                else

                {
                    responce = _workBAL.Dashboard_CameraReport(model, out TotalCount);
                }
                




                if (responce != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
                        TotalRecordCount = TotalCount
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


        [HttpPost("Dashboardcameracount")]
        public ResponseViewModel DashboardcameraCount(GetcameraStatusCountModel model)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
                string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    List<string> UserDivision = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }


                List<string> selectedDivision = model.DivisionIds;

                List<string> selectedDistrict = model.DistrictIds;


                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DistrictId))
                {
                    model.DistrictIds = currentUser.DistrictId.Split(",").Select(x => x.Trim()).ToList();
                }
                int TotalCount = 0;



                if (model.DivisionIds != null && selectedDivision != null && selectedDivision.Any())
                {
                    model.DivisionIds =
                        model.DivisionIds
                        .Where(d => selectedDivision.Contains(d))
                        .ToList();
                }

                // District filter
                if (model.DistrictIds != null && selectedDistrict != null && selectedDistrict.Any())
                {
                    model.DistrictIds =
                        model.DistrictIds
                        .Where(d => selectedDistrict.Contains(d))
                        .ToList();
                }

                // model.DivisionIds = model.DivisionIds.Where(x => (selectedDivision != null && selectedDivision.Contains(x)) || selectedDivision == null || selectedDivision.Count() == 0).ToList();



                 var  responce = _workBAL.Dashboard_CameraCount(model);
                    
              
              
                




                if (responce != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
                        TotalRecordCount = TotalCount
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





        #endregion cameralive



        #region Drop DownFilter



        [HttpPost("GetAllDivision")]

        public ResponseViewModel GetAllDivision(DashboardCameraModel model)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
                string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
                string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
                string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

                string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    List<string> UserDivision = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }


                List<string> selectedDivision = model.DivisionIds;


                if (!string.IsNullOrWhiteSpace(DepartmentIds))
                {
                    model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
                }
                if (!string.IsNullOrWhiteSpace(currentUser.DistrictId))
                {
                    model.DistrictIds = currentUser.DistrictId.Split(",").Select(x => x.Trim()).ToList();
                }

                model.DivisionIds = model.DivisionIds.Where(x => (selectedDivision != null && selectedDivision.Contains(x)) || selectedDivision == null || selectedDivision.Count() == 0).ToList();

                List<GetAllDivisionModel> responce = _workBAL.GetAllDivision(model);


                if (responce != null)
                {
                    return new ResponseViewModel()
                    {
                        Status = ResponseConstants.Success,
                        Data = responce,
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



        [HttpPost("GetAllDistrict")]
        public ResponseViewModel GetAllDistrict(DashboardCameraModel model)
        {
            try
            {
                string userId = User.Claims.Where(x => x.Type == Constants.UserId)
                                           .FirstOrDefault()?.Value ?? "";

                AccountUserModel currentUser =
                    _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault()
                    ?? new AccountUserModel();

                List<string> selectedDivision = model.DivisionIds;

                if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
                {
                    model.DivisionIds = currentUser.DivisionId
                                        .Split(",")
                                        .Select(x => x.Trim())
                                        .ToList();
                }

                
                if (selectedDivision != null && selectedDivision.Count > 0)
                {
                    model.DivisionIds = selectedDivision;
                }

                
                if (!string.IsNullOrWhiteSpace(currentUser.DistrictId))
                {
                    model.DistrictIds = currentUser.DistrictId
                                        .Split(",")
                                        .Select(x => x.Trim())
                                        .ToList();
                }
                else
                {
                    model.DistrictIds = new List<string>();
                }

                
                List<GetAllDistrictModel> response =
                    _workBAL.GetAllDistrict(model);

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Success,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                return new ResponseViewModel()
                {
                    Status = ResponseConstants.Error,
                    Message = ex.Message
                };
            }
        }



        [HttpPost("GetAllMainCategory")]
        public ResponseViewModel GetAllMainCategory(
       [FromQuery] string divisionId,
       [FromQuery] string districtId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == Constants.UserId)?.Value ?? "";

                AccountUserModel currentUser =
                    _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault()
                    ?? new AccountUserModel();

                
                var userDivisions = !string.IsNullOrWhiteSpace(currentUser.DivisionId)
                    ? currentUser.DivisionId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(x => x.Trim()).ToList()
                    : new List<string>();

                var userDistricts = !string.IsNullOrWhiteSpace(currentUser.DistrictId)
                    ? currentUser.DistrictId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(x => x.Trim()).ToList()
                    : new List<string>();

                
                var selectedDivisions = !string.IsNullOrWhiteSpace(divisionId)
                    ? divisionId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim()).ToList()
                    : new List<string>();

                var selectedDistricts = !string.IsNullOrWhiteSpace(districtId)
                    ? districtId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim()).ToList()
                    : new List<string>();

                
                var finalDivisions = selectedDivisions.Any()
                   ? userDivisions.Intersect(selectedDivisions).ToList()
                   : userDivisions;

                var finalDistricts = selectedDistricts.Any()
                   ? userDistricts.Intersect(selectedDistricts).ToList()
                   : userDistricts;

                
                var model = new DashboardCameraModel
                {
                    DivisionIds = finalDivisions,
                    DistrictIds = finalDistricts
                };

                // ----------------------------
                // ✅ DEPARTMENT FILTER
                // ----------------------------
                string departmentIdsClaim =
                    User.Claims.FirstOrDefault(x => x.Type == Constants.DepartmentId)?.Value ?? "";

                if (!string.IsNullOrWhiteSpace(departmentIdsClaim))
                {
                    model.DepartmentIds = departmentIdsClaim
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();
                }

                // ----------------------------
                // ✅ DATA CALL
                // ----------------------------
                var result = _workBAL.GetAllMainCategory(model);

                return new ResponseViewModel
                {
                    Status = ResponseConstants.Success,
                    Data = result ?? new List<WorkTypeModel>()
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                return new ResponseViewModel
                {
                    Status = ResponseConstants.Error,
                    Message = ex.Message
                };
            }
        }





        [HttpPost("GetAllSubCategory")]
        public ResponseViewModel GetAllSubCategory(
    [FromQuery] string divisionId,
    [FromQuery] string districtId,
    [FromQuery] string mainCategory)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == Constants.UserId)?.Value ?? "";

                AccountUserModel currentUser =
                    _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault()
                    ?? new AccountUserModel();

                // ----------------------------
                // ✅ USER ALLOWED AREAS
                // ----------------------------
                var userDivisions = !string.IsNullOrWhiteSpace(currentUser.DivisionId)
                    ? currentUser.DivisionId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(x => x.Trim()).ToList()
                    : new List<string>();

                var userDistricts = !string.IsNullOrWhiteSpace(currentUser.DistrictId)
                    ? currentUser.DistrictId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(x => x.Trim()).ToList()
                    : new List<string>();

                // ----------------------------
                // ✅ UI SELECTED FILTERS
                // ----------------------------
                var selectedDivisions = !string.IsNullOrWhiteSpace(divisionId)
                    ? divisionId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim()).ToList()
                    : new List<string>();

                var selectedDistricts = !string.IsNullOrWhiteSpace(districtId)
                    ? districtId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim()).ToList()
                    : new List<string>();

                // ----------------------------
                // ✅ FINAL FILTER (SECURE)
                // ----------------------------
                var finalDivisions = selectedDivisions.Any()
                   ? userDivisions.Intersect(selectedDivisions).ToList()
                   : userDivisions;

                var finalDistricts = selectedDistricts.Any()
                   ? userDistricts.Intersect(selectedDistricts).ToList()
                   : userDistricts;

                // ----------------------------
                // ✅ BUILD MODEL FOR DAL
                // ----------------------------
                var model = new DashboardCameraModel
                {
                    DivisionIds = finalDivisions,
                    DistrictIds = finalDistricts,
                    mainCategory = mainCategory ?? string.Empty
                };

                // ----------------------------
                // ✅ DEPARTMENT FILTER
                // ----------------------------
                string departmentIdsClaim =
                    User.Claims.FirstOrDefault(x => x.Type == Constants.DepartmentId)?.Value ?? "";

                if (!string.IsNullOrWhiteSpace(departmentIdsClaim))
                {
                    model.DepartmentIds = departmentIdsClaim
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();
                }

                // ----------------------------
                // ✅ DATA CALL
                // ----------------------------
                var result = _workBAL.GetAllSubCategory(model);

                return new ResponseViewModel
                {
                    Status = ResponseConstants.Success,
                    Data = result ?? new List<SubWorkTypeModel>()
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                return new ResponseViewModel
                {
                    Status = ResponseConstants.Error,
                    Message = ex.Message
                };
            }
        }



        //[HttpPost("GetAllSubCategory")]

        //public ResponseViewModel GetAllSubCategory(DashboardCameraModel model)
        //{
        //    try
        //    {
        //        string userId = User.Claims.Where(x => x.Type == Constants.UserId)?.FirstOrDefault()?.Value ?? "";
        //        string roleCode = User.Claims.Where(x => x.Type == Constants.RoleCode)?.FirstOrDefault()?.Value ?? "";
        //        string roleId = User.Claims.Where(x => x.Type == Constants.RoleId)?.FirstOrDefault()?.Value ?? "";
        //        string UserGroupName = User.Claims.Where(x => x.Type == Constants.UserGroupName)?.FirstOrDefault()?.Value ?? "";

        //        AccountUserModel currentUser = _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault() ?? new AccountUserModel();

        //        string DepartmentIds = User.Claims.Where(x => x.Type == Constants.DepartmentId)?.FirstOrDefault()?.Value ?? "";
        //        if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
        //        {
        //            List<string> UserDivision = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
        //        }


        //        List<string> selectedDivision = model.DivisionIds;


        //        if (!string.IsNullOrWhiteSpace(DepartmentIds))
        //        {
        //            model.DepartmentIds = DepartmentIds.Split(",").Select(x => x.Trim()).ToList();
        //        }
        //        if (!string.IsNullOrWhiteSpace(currentUser.DivisionId))
        //        {
        //            model.DivisionIds = currentUser.DivisionId.Split(",").Select(x => x.Trim()).ToList();
        //        }
        //        if (!string.IsNullOrWhiteSpace(currentUser.DistrictId))
        //        {
        //            model.DistrictIds = currentUser.DistrictId.Split(",").Select(x => x.Trim()).ToList();
        //        }

        //        model.DivisionIds = model.DivisionIds.Where(x => (selectedDivision != null && selectedDivision.Contains(x)) || selectedDivision == null || selectedDivision.Count() == 0).ToList();

        //        List<SubWorkTypeModel> responce = _workBAL.GetAllSubCategory(model);


        //        if (responce != null)
        //        {
        //            return new ResponseViewModel()
        //            {
        //                Status = ResponseConstants.Success,
        //                Data = responce,
        //            };
        //        }

        //        return new ResponseViewModel()
        //        {
        //            Status = ResponseConstants.Failed,
        //            Data = null,
        //            Message = "Somthing went wrong"
        //        };
        //    }

        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, ex.Message);

        //        return new ResponseViewModel()
        //        {
        //            Status = ResponseConstants.Error,
        //            Data = null,
        //            Message = ex.Message
        //        };
        //    }



        //}




        [HttpPost("GetAllWorkStatus")]

        public ResponseViewModel GetAllWorkStatus( string divisionId,
     string districtId,
  string mainCategory,
    string subcategory)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == Constants.UserId)?.Value ?? "";

                AccountUserModel currentUser =
                    _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault()
                    ?? new AccountUserModel();

                // ----------------------------
                // ✅ USER ALLOWED AREAS
                // ----------------------------
                var userDivisions = !string.IsNullOrWhiteSpace(currentUser.DivisionId)
                    ? currentUser.DivisionId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(x => x.Trim()).ToList()
                    : new List<string>();

                var userDistricts = !string.IsNullOrWhiteSpace(currentUser.DistrictId)
                    ? currentUser.DistrictId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(x => x.Trim()).ToList()
                    : new List<string>();

                // ----------------------------
                // ✅ UI SELECTED FILTERS
                // ----------------------------
                var selectedDivisions = !string.IsNullOrWhiteSpace(divisionId)
                    ? divisionId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim()).ToList()
                    : new List<string>();

                var selectedDistricts = !string.IsNullOrWhiteSpace(districtId)
                    ? districtId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim()).ToList()
                    : new List<string>();

                // ----------------------------
                // ✅ FINAL FILTER (SECURE)
                // ----------------------------
                var finalDivisions = selectedDivisions.Any()
                   ? userDivisions.Intersect(selectedDivisions).ToList()
                   : userDivisions;

                var finalDistricts = selectedDistricts.Any()
                   ? userDistricts.Intersect(selectedDistricts).ToList()
                   : userDistricts;

                // ----------------------------
                // ✅ BUILD MODEL FOR DAL
                // ----------------------------
                var model = new DashboardCameraModel
                {
                    DivisionIds = finalDivisions,
                    DistrictIds = finalDistricts,
                    mainCategory = mainCategory ?? string.Empty,
                    subcategory = subcategory ?? string.Empty
                };

                // ----------------------------
                // ✅ DEPARTMENT FILTER
                // ----------------------------
                string departmentIdsClaim =
                    User.Claims.FirstOrDefault(x => x.Type == Constants.DepartmentId)?.Value ?? "";

                if (!string.IsNullOrWhiteSpace(departmentIdsClaim))
                {
                    model.DepartmentIds = departmentIdsClaim
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();
                }

                // ----------------------------
                // ✅ DATA CALL
                // ----------------------------
                var result = _workBAL.GetAllWorkStatus(model);

                return new ResponseViewModel
                {
                    Status = ResponseConstants.Success,
                    Data = result ?? new List<WorkStatusModel>()
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                return new ResponseViewModel
                {
                    Status = ResponseConstants.Error,
                    Message = ex.Message
                };
            }
        }





        

        [HttpPost("GetAllTenderNumber")]

        public ResponseViewModel GetAllTenderNumber([FromQuery] string divisionId,
    [FromQuery] string districtId,
    [FromQuery] string mainCategory,
     [FromQuery] string subcategory,
     [FromQuery] string WorkStatus)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == Constants.UserId)?.Value ?? "";

                AccountUserModel currentUser =
                    _settingBAL.User_Get(true, UserId: userId)?.FirstOrDefault()
                    ?? new AccountUserModel();

                // ----------------------------
                // ✅ USER ALLOWED AREAS
                // ----------------------------
                var userDivisions = !string.IsNullOrWhiteSpace(currentUser.DivisionId)
                    ? currentUser.DivisionId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(x => x.Trim()).ToList()
                    : new List<string>();

                var userDistricts = !string.IsNullOrWhiteSpace(currentUser.DistrictId)
                    ? currentUser.DistrictId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(x => x.Trim()).ToList()
                    : new List<string>();

                // ----------------------------
                // ✅ UI SELECTED FILTERS
                // ----------------------------
                var selectedDivisions = !string.IsNullOrWhiteSpace(divisionId)
                    ? divisionId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim()).ToList()
                    : new List<string>();

                var selectedDistricts = !string.IsNullOrWhiteSpace(districtId)
                    ? districtId.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim()).ToList()
                    : new List<string>();

                // ----------------------------
                // ✅ FINAL FILTER (SECURE)
                // ----------------------------
                var finalDivisions = selectedDivisions.Any()
                   ? userDivisions.Intersect(selectedDivisions).ToList()
                   : userDivisions;

                var finalDistricts = selectedDistricts.Any()
                   ? userDistricts.Intersect(selectedDistricts).ToList()
                   : userDistricts;

                // ----------------------------
                // ✅ BUILD MODEL FOR DAL
                // ----------------------------
                var model = new DashboardCameraModel
                {
                    DivisionIds = finalDivisions,
                    DistrictIds = finalDistricts,
                    mainCategory = mainCategory ?? string.Empty,
                    subcategory = subcategory ?? string.Empty,
                    WorkStatus = WorkStatus ?? string.Empty
                };

                // ----------------------------
                // ✅ DEPARTMENT FILTER
                // ----------------------------
                string departmentIdsClaim =
                    User.Claims.FirstOrDefault(x => x.Type == Constants.DepartmentId)?.Value ?? "";

                if (!string.IsNullOrWhiteSpace(departmentIdsClaim))
                {
                    model.DepartmentIds = departmentIdsClaim
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();
                }

                // ----------------------------
                // ✅ DATA CALL
                // ----------------------------
                var result = _workBAL.GetAllTenderNumber(model);

                return new ResponseViewModel
                {
                    Status = ResponseConstants.Success,
                    Data = result ?? new List<TenderNumberModel>()
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                return new ResponseViewModel
                {
                    Status = ResponseConstants.Error,
                    Message = ex.Message
                };
            }
        }
























        #endregion Filter Drop DownFilter

    }
}

