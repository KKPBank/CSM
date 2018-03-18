using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using System.Globalization;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class UserMonitoringController : BaseController
    {
        private const int AutoCompleteMaxResult = 10;

        private IUserFacade _userFacade;
        private IUserMonitoringFacade _userMonitoringFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserMonitoringController));

        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public ActionResult Search()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch UserMonitoring").ToInputLogString());
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch UserMonitoring").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetUserAndBranch()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search Group Assign").ToInputLogString());

            try
            {
                _userFacade = new UserFacade();

                var userIds = new List<int>();
                userIds.Add(UserInfo.UserId);

                var subordinateUserIds = _userFacade.GetUserIdsBySupervisorIds(new List<int> { UserInfo.UserId });
                userIds.AddRange(subordinateUserIds);

                var branchIds = _userFacade.GetBranchIdsByUserIds(userIds);

                var branchUserIds = _userFacade.GetDummyUserIdsByUserIds(userIds);
                userIds.AddRange(branchUserIds);

                return Json(new
                {
                    IsSuccess = true,
                    UserIds = string.Join(",", userIds),
                    BranchIds = string.Join(",", branchIds),
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search GroupAssign").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                });
            }
        }

        [HttpPost]
        public ActionResult SearchGroupAssign(GroupAssignSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search GroupAssign").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _userMonitoringFacade = new UserMonitoringFacade();

                    var viewModel = new SearchGroupAssignViewModel();
                    viewModel.SearchFilter = searchFilter;
                    viewModel.ResultList = _userMonitoringFacade.SearchGroupAssign(searchFilter);
                    ViewBag.PageSize = viewModel.SearchFilter.PageSize;
                    using (var stFacadce = new SrStatusFacade())
                    {
                        ViewBag.SRStateList = stFacadce.GetSrState()
                                                .Where(x => !x.SRStateId.InList(Constants.SRStateId.Cancelled, Constants.SRStateId.Closed))
                                                .ToList();
                    }
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search GroupAssign").ToSuccessLogString());
                    return PartialView("~/Views/UserMonitoring/_SearchGroupAssign.cshtml", viewModel);
                }
                return Json(new
                {
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search GroupAssign").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult SearchUserAssign(UserAssignSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search UserAssign").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    searchFilter.CurrentUserId = UserInfo.UserId;

                    _userMonitoringFacade = new UserMonitoringFacade();

                    var viewModel = new SearchUserAssignViewModel();
                    viewModel.SearchFilter = searchFilter;
                    viewModel.ResultList = _userMonitoringFacade.SearchUserAssign(searchFilter);
                    ViewBag.PageSize = viewModel.SearchFilter.PageSize;
                    using (var stFacadce = new SrStatusFacade())
                    {
                        ViewBag.SRStateList = stFacadce.GetSrState()
                                                .Where(x => !x.SRStateId.InList(Constants.SRStateId.Cancelled, Constants.SRStateId.Closed))
                                                .ToList();
                    }

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search UserAssign").ToSuccessLogString());
                    return PartialView("~/Views/UserMonitoring/_SearchUserAssign.cshtml", viewModel);
                }

                return Json(new
                {
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search UserAssign").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult SearchServiceRequest(UserMonitoringSrSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search UserAssign").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _userMonitoringFacade = new UserMonitoringFacade();

                    var viewModel = new UserMonitoringSrViewModel();
                    viewModel.SearchFilter = searchFilter;
                    viewModel.ResultList = _userMonitoringFacade.SearchServiceRequest(searchFilter);
                    ViewBag.PageSize = viewModel.SearchFilter.PageSize;

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search UserAssign").ToSuccessLogString());
                    return PartialView("~/Views/UserMonitoring/_SearchServiceRequest.cshtml", viewModel);
                }

                return Json(new
                {
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search UserAssign").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteModalSearchBranch(string keyword, string userIds)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Branch").Add("Keyword", keyword)
                .Add("UserIds", userIds).ToInputLogString());

            try
            {
                _userFacade = new UserFacade();
                List<UserEntity> result = _userFacade.GetUsersBySupervisorIds(userIds.Split(',').Select(Int32.Parse).ToList());

                var data = result.Select(r => new
                {
                    r.BranchId,
                    r.BranchName
                }).ToList();

                data = data.Distinct().ToList();

                return Json(data);
            }
            catch (Exception ex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Branch").ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteModalSearchUser(string keyword, string ids, int branchId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search User").ToInputLogString());

            try
            {
                var idsString = ids.Split(',').ToList();
                var userIds = idsString.Select(Int32.Parse).ToList();

                if (_userMonitoringFacade == null)
                    _userMonitoringFacade = new UserMonitoringFacade();

                List<UserEntity> result = _userMonitoringFacade.AutoCompleteSearchUser(keyword, userIds, branchId, AutoCompleteMaxResult);

                var data = result.Select(r => new
                {
                    r.UserId,
                    r.FullName,
                    r.BranchId
                }).ToList();

                data = data.Distinct().ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search User").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteSearchBranch(string keyword)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Branch").Add("Keyword", keyword).ToInputLogString());

            if (_userMonitoringFacade == null)
            {
                _userMonitoringFacade = new UserMonitoringFacade();
            }

            try
            {
                List<BranchEntity> result = _userMonitoringFacade.AutoCompleteSearchBranch(keyword, AutoCompleteMaxResult);
                return Json(result.Select(r => new
                {
                    r.BranchId,
                    r.BranchName
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Branch").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteSearchProduct(string keyword, int? productGroupId, int? campaignServiceId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Product").Add("Keyword", keyword)
                .Add("ProductGroupId", productGroupId).Add("CampaignServiceId", campaignServiceId).ToInputLogString());

            if (_userMonitoringFacade == null)
            {
                _userMonitoringFacade = new UserMonitoringFacade();
            }

            try
            {
                List<ProductEntity> result = _userMonitoringFacade.AutoCompleteSearchProduct(keyword, productGroupId, AutoCompleteMaxResult);
                return Json(result.Select(r => new
                {
                    r.ProductId,
                    r.ProductName,
                    r.ProductGroupName
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Product").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteSearchCampaignService(string keyword, int? productGroupId, int? productId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Campaign/Service").Add("Keyword", keyword)
                .Add("ProductGroupId", productGroupId).Add("ProductId", productId).ToInputLogString());

            if (_userMonitoringFacade == null)
            {
                _userMonitoringFacade = new UserMonitoringFacade();
            }

            try
            {
                List<CampaignServiceEntity> result = _userMonitoringFacade.AutoCompleteSearchCampaignService(keyword, productGroupId, productId, AutoCompleteMaxResult);
                return Json(result.Select(r => new
                {
                    r.CampaignServiceId,
                    r.CampaignServiceName
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Campaign/Service").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteSearchArea(string keyword, int? subAreaId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Area").Add("Keyword", keyword).Add("SubAreaId", subAreaId).ToInputLogString());

            if (_userMonitoringFacade == null)
            {
                _userMonitoringFacade = new UserMonitoringFacade();
            }

            try
            {
                List<AreaItemEntity> result = _userMonitoringFacade.AutoCompleteSearchArea(keyword, subAreaId, AutoCompleteMaxResult);
                return Json(result.Select(r => new
                {
                    r.AreaId,
                    r.AreaName
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Area").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteSearchAreaOnMapping(string keyword, int? campaignServiceId, int? subAreaId, int? typeId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Area (on Mapping)").Add("Keyword", keyword)
                .Add("CampaignServiceId", campaignServiceId).Add("SubAreaId", subAreaId).Add("TypeId", typeId).ToInputLogString());

            if (_userMonitoringFacade == null)
            {
                _userMonitoringFacade = new UserMonitoringFacade();
            }

            try
            {
                List<AreaItemEntity> result = _userMonitoringFacade.AutoCompleteSearchAreaOnMapping(keyword, campaignServiceId, subAreaId, typeId, AutoCompleteMaxResult);
                return Json(result.Select(r => new
                {
                    r.AreaId,
                    r.AreaName
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search Area (on Mapping)").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteSearchSubArea(string keyword, int? areaId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search SubArea").Add("Keyword", keyword)
                .Add("AreaId", areaId).ToInputLogString());

            if (_userMonitoringFacade == null)
            {
                _userMonitoringFacade = new UserMonitoringFacade();
            }

            try
            {
                List<SubAreaItemEntity> result = _userMonitoringFacade.AutoCompleteSearchSubArea(keyword, areaId, AutoCompleteMaxResult);
                return Json(result.Select(r => new
                {
                    r.SubAreaId,
                    r.SubAreaName
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search SubArea").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteSearchSubAreaOnMapping(string keyword, int? campaignServiceId, int? areaId, int? typeId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search SubArea (on Mapping)").Add("Keyword", keyword)
                .Add("CampaignServiceId", campaignServiceId).Add("areaId", areaId).Add("TypeId", typeId).ToInputLogString());

            if (_userMonitoringFacade == null)
            {
                _userMonitoringFacade = new UserMonitoringFacade();
            }

            try
            {
                List<SubAreaItemEntity> result = _userMonitoringFacade.AutoCompleteSearchSubAreaOnMapping(keyword, campaignServiceId, areaId, typeId, AutoCompleteMaxResult);
                return Json(result.Select(r => new
                {
                    r.SubAreaId,
                    r.SubAreaName
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Search SubArea (on Mapping)").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetServiceRequestByUserId(int userId, string statusCode)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Get ServiceRequest by User ID").Add("UserId", userId)
                .Add("StatusCode", statusCode).ToInputLogString());

            IEnumerable<ServiceRequestEntity> result = new List<ServiceRequestEntity>();

            try
            {
                var users = new List<int>();
                users.Add(userId);

                _userMonitoringFacade = new UserMonitoringFacade();
                var searchRequest = new ServiceRequestSearchFilter
                {
                    OwnerUserId = userId,
                    DelegateUserId = userId,
                    StatusCode = statusCode
                };

                result = _userMonitoringFacade.GetServiceRequestList(searchRequest);

                return Json(new
                {
                    IsSuccess = true,
                    data = result,
                    Error = string.Empty
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Get ServiceRequest by User ID").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransferServiceRequest(string srIds, string transferTypes, int transferToUserId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Transfer ServiceRequest").Add("SrIds", srIds)
                .Add("TransferTypes", transferTypes).Add("TransferToUserId", transferToUserId).ToInputLogString());

            try
            {
                if (string.IsNullOrEmpty(srIds) || string.IsNullOrEmpty(transferTypes) || transferToUserId == 0)
                {
                    return Json(new
                    {
                        Valid = false,
                        data = string.Empty,
                        Error = "Technical Error: Invalid Argument."
                    });
                }

                var ids = srIds.Split(',').Select(Int32.Parse).ToList();
                var types = transferTypes.Split(',').ToList();

                if (ids.Count != types.Count)
                {
                    return Json(new
                    {
                        Valid = false,
                        data = string.Empty,
                        Error = "Technical Error: Invalid Argument. Count(SrIds) is not equals Count(TransferTypes)"
                    });
                }

                var transfers = new List<SrTransferEntity>();
                for (int i = 0; i < ids.Count; i++)
                {
                    transfers.Add(new SrTransferEntity
                    {
                        SrId = ids[i],
                        TransferToUserId = transferToUserId,
                        IsTransferOwner = (types[i].ToUpper(CultureInfo.InvariantCulture) == "OWNER")
                    });
                }

                _userMonitoringFacade = new UserMonitoringFacade();
                var result = _userMonitoringFacade.TransferServiceRequest(transfers, UserInfo.UserId);

                return Json(new
                {
                    IsSuccess = result.IsSuccess,
                    ErrorMessage = (!result.IsSuccess) ? string.Join("<br/>", result.ErrorMessages) : ""
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Transfer ServiceRequest").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    data = string.Empty,
                    Error = Resource.Error_System
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetServiceRequestByAllUser(string userId, string statusCode)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Get ServiceRequest by All UserID")
                .Add("UserId", userId).Add("StatusCode", statusCode).ToInputLogString());

            IEnumerable<ServiceRequestEntity> result = new List<ServiceRequestEntity>();

            try
            {
                var idsString = userId.Split(',').ToList();

                var ids = idsString.Select(Int32.Parse).ToList();

                _userMonitoringFacade = new UserMonitoringFacade();
                _userFacade = new UserFacade();

                var empDetail = _userFacade.GetUsersBySupervisorIds(ids);

                if (empDetail.Any())
                {
                    result = _userMonitoringFacade.GetServiceRequestListByUserIds(ids, statusCode);
                }

                return Json(new
                {
                    IsSuccess = true,
                    data = result,
                    Error = string.Empty
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Get ServiceRequest by All UserID")
                    .Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchServiceRequestGroup(string product, string campaign, string area, string subarea, string fromdate, string todate)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search SR Group").Add("Product", product).Add("Campaign", campaign)
                .Add("Subarea", subarea).Add("FromDate", fromdate).Add("ToDate", todate).ToInputLogString());

            int productId = 0;
            int campaignId = 0;
            int areaId = 0;
            int subareaId = 0;
            DateTime dateFrom = DateTime.MinValue;
            DateTime dateTo = DateTime.MinValue;

            if (!String.IsNullOrEmpty(product))
            {
                productId = Int32.Parse(product, CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrEmpty(campaign))
            {
                campaignId = Int32.Parse(campaign, CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrEmpty(area))
            {
                areaId = Int32.Parse(area, CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrEmpty(subarea))
            {
                subareaId = Int32.Parse(subarea, CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrEmpty(fromdate))
            {
                DateTime.TryParse(fromdate, out dateFrom);
            }

            if (!String.IsNullOrEmpty(todate))
            {
                DateTime.TryParse(todate, out dateTo);
            }

            try
            {
                UserMonitoringModel userAssign = new UserMonitoringModel();

                var cur = new List<int>();
                cur.Add(this.UserInfo.UserId);

                _userFacade = new UserFacade();
                var listUserEntities = _userFacade.GetUsersBySupervisorIds(cur);
                var current = _userMonitoringFacade.GetUserByLoginName(this.UserInfo.Username);

                userAssign.GroupAssignInformation = new List<GroupAssignInformationModel>();
                userAssign.Branchs = new List<BranchEntity>();

                if (listUserEntities != null)
                {
                    listUserEntities.Add(current);
                    listUserEntities.Sort((x, y) => String.Compare(x.BranchName, y.BranchName, StringComparison.Ordinal));

                    List<int> braches = listUserEntities.Select(x => x.BranchId ?? 0).ToList();
                    braches = braches.Distinct().ToList();
                    List<UserEntity> dummies = _userMonitoringFacade.GetDummyUserByBranchIds(braches);

                    foreach (var br in braches)
                    {
                        var dummy = dummies.FirstOrDefault(x => x.BranchId == br);
                        var grpAssign = new GroupAssignInformationModel();

                        if (dummy != null)
                        {
                            grpAssign.BranchCodeTeam = dummy.BranchCode;
                            grpAssign.BranchNameTeam = dummy.BranchName;
                            grpAssign.UserIds = string.Join(",", listUserEntities.Select(x => x.UserId).ToList());
                            grpAssign.UserId = dummy.UserId;
                            var srList = _userMonitoringFacade.GetServiceRequestList(new ServiceRequestSearchFilter { OwnerUserId = dummy.UserId, DelegateUserId = dummy.UserId });

                            if (productId != 0)
                            {
                                srList = srList.Where(p => p.ProductId == productId).ToList();
                            }

                            if (campaignId != 0)
                            {
                                srList = srList.Where(p => p.CampaignServiceId == campaignId).ToList();
                            }

                            if (areaId != 0)
                            {
                                srList = srList.Where(p => p.AreaId == areaId).ToList();
                            }

                            if (subareaId != 0)
                            {
                                srList = srList.Where(p => p.SubAreaId == subareaId).ToList();
                            }

                            if (dateFrom.CompareTo(DateTime.MinValue) != 0)
                            {
                                srList = srList.Where(p => p.CreateDate.HasValue && p.CreateDate.Value.CompareTo(dateFrom) >= 0).ToList();
                            }

                            if (dateTo.CompareTo(DateTime.MinValue) != 0)
                            {
                                srList = srList.Where(p => p.ClosedDate.HasValue && p.ClosedDate.Value.CompareTo(dateTo) <= 0).ToList();
                            }

                            grpAssign.ServiceRequests = srList;

                            userAssign.GroupAssignInformation.Add(grpAssign);
                            userAssign.Branchs.Add(new BranchEntity() { BranchId = br, BranchName = dummy.BranchName, });
                        }
                    }
                }

                return Json(new
                {
                    IsSuccess = true,
                    data = userAssign.GroupAssignInformation,
                    Error = string.Empty
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search SR Group").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchServiceRequestUser(string branch, string product, string campaign, string area, string subarea, string fromdate, string todate)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search SR User").Add("Branch", branch).Add("Product", product)
                .Add("Campaign", campaign).Add("Area", area).Add("Subarea", subarea).Add("FromDate", fromdate).Add("ToDate", todate).ToInputLogString());

            int branchId = 0;
            int productId = 0;
            int campaignId = 0;
            int areaId = 0;
            int subareaId = 0;

            DateTime dateFrom = DateTime.MinValue;
            DateTime dateTo = DateTime.MinValue;

            if (!String.IsNullOrEmpty(branch))
            {
                branchId = Int32.Parse(branch, CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrEmpty(product))
            {
                productId = Int32.Parse(product, CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrEmpty(campaign))
            {
                campaignId = Int32.Parse(campaign, CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrEmpty(area))
            {
                areaId = Int32.Parse(area, CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrEmpty(subarea))
            {
                subareaId = Int32.Parse(subarea, CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrEmpty(fromdate))
            {
                DateTime.TryParse(fromdate, out dateFrom);
            }

            if (!String.IsNullOrEmpty(todate))
            {
                DateTime.TryParse(todate, out dateTo);
            }

            try
            {
                UserMonitoringModel userAssign = new UserMonitoringModel();
                _userMonitoringFacade = new UserMonitoringFacade();

                var current = _userMonitoringFacade.GetUserByLoginName(this.UserInfo.Username);

                var cur = new List<int>();
                cur.Add(this.UserInfo.UserId);

                _userFacade = new UserFacade();
                var listUserEntities = _userFacade.GetUsersBySupervisorIds(cur);

                if (branchId != 0)
                {
                    listUserEntities = listUserEntities.Where(p => p.BranchId == branchId).ToList();
                }

                userAssign.UserAssignInformation = new List<UserAssignInformationModel>();

                if (current != null)
                {
                    var srList = _userMonitoringFacade.GetServiceRequestList(new ServiceRequestSearchFilter { OwnerUserId = current.UserId, DelegateUserId = current.UserId });

                    if (productId != 0)
                    {
                        srList = srList.Where(p => p.ProductId == productId).ToList();
                    }

                    if (campaignId != 0)
                    {
                        srList = srList.Where(p => p.CampaignServiceId == campaignId).ToList();
                    }

                    if (areaId != 0)
                    {
                        srList = srList.Where(p => p.AreaId == areaId).ToList();
                    }

                    if (subareaId != 0)
                    {
                        srList = srList.Where(p => p.SubAreaId == subareaId).ToList();
                    }

                    if (dateFrom.CompareTo(DateTime.MinValue) != 0)
                    {
                        srList = srList.Where(p => p.CreateDate.HasValue && p.CreateDate.Value.CompareTo(dateFrom) >= 0).ToList();
                    }

                    if (dateTo.CompareTo(DateTime.MinValue) != 0)
                    {
                        srList = srList.Where(p => p.ClosedDate.HasValue && p.ClosedDate.Value.CompareTo(dateTo) <= 0).ToList();
                    }

                    if (branchId == 0 || current.BranchId == branchId)
                    {
                        var currentUserAssign = new UserAssignInformationModel
                        {
                            Role = !String.IsNullOrEmpty(current.RoleCode) ? current.RoleCode : String.Empty,
                            BranchName = !String.IsNullOrEmpty(current.BranchName) ? current.BranchName : String.Empty,
                            UserId = current.UserId,
                            Username = !String.IsNullOrEmpty(current.Username) ? current.Username : String.Empty,
                            FullName = !String.IsNullOrEmpty(current.FullName) ? current.FullName : String.Empty,
                            ServiceRequests = srList,
                        };

                        userAssign.UserAssignInformation.Add(currentUserAssign);
                    }
                }

                if (listUserEntities != null && listUserEntities.Any())
                {
                    foreach (var emp in listUserEntities)
                    {
                        var srList = _userMonitoringFacade.GetServiceRequestList(new ServiceRequestSearchFilter { OwnerUserId = emp.UserId, DelegateUserId = emp.UserId });

                        if (productId != 0)
                        {
                            srList = srList.Where(p => p.ProductId == productId).ToList();
                        }

                        if (campaignId != 0)
                        {
                            srList = srList.Where(p => p.CampaignServiceId == campaignId).ToList();
                        }

                        if (areaId != 0)
                        {
                            srList = srList.Where(p => p.AreaId == areaId).ToList();
                        }

                        if (subareaId != 0)
                        {
                            srList = srList.Where(p => p.SubAreaId == subareaId).ToList();
                        }

                        if (dateFrom.CompareTo(DateTime.MinValue) != 0)
                        {
                            srList = srList.Where(p => p.CreateDate.HasValue && p.CreateDate.Value.CompareTo(dateFrom) >= 0).ToList();
                        }

                        if (dateTo.CompareTo(DateTime.MinValue) != 0)
                        {
                            srList = srList.Where(p => p.ClosedDate.HasValue && p.ClosedDate.Value.CompareTo(dateTo) <= 0).ToList();
                        }

                        var currentUserAssign = new UserAssignInformationModel
                        {
                            Role = !String.IsNullOrEmpty(emp.RoleCode) ? emp.RoleCode : String.Empty,
                            BranchName = !String.IsNullOrEmpty(emp.BranchName) ? emp.BranchName : String.Empty,
                            UserId = emp.UserId,
                            Username = !String.IsNullOrEmpty(emp.Username) ? emp.Username : String.Empty,
                            FullName = !String.IsNullOrEmpty(emp.FullName) ? emp.FullName : String.Empty,
                            ServiceRequests = srList
                        };

                        userAssign.UserAssignInformation.Add(currentUserAssign);
                    }
                }


                return Json(new
                {
                    IsSuccess = true,
                    data = userAssign.UserAssignInformation,
                    Error = string.Empty
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("User Monitoring :: Search SR User").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}