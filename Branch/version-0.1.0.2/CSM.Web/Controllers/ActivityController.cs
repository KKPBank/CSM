using System;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class ActivityController : BaseController
    {
        private AuditLogEntity _auditLog;
        private ICommonFacade _commonFacade;
        private IActivityFacade _activityFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ActivityController));

        [CheckUserRole(ScreenCode.ViewCustomerActivity)]
        public ActionResult List()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch ActivityLog").ToInputLogString());

            try
            {
                CustomerInfoViewModel custInfoVM = new CustomerInfoViewModel();

                if (TempData["CustomerInfo"] != null)
                {
                    custInfoVM = (CustomerInfoViewModel)TempData["CustomerInfo"];
                    TempData["CustomerInfo"] = custInfoVM; // Keep for change Tab
                }
                else
                {
                    return RedirectToAction("Search", "Customer");
                }

                ActivityViewModel activityVM = new ActivityViewModel();
                if (custInfoVM.CustomerId != null)
                {
                    activityVM.CustomerInfo = custInfoVM;
                }

                _commonFacade = new CommonFacade();
                _activityFacade = new ActivityFacade();

                var subsType = custInfoVM.SubscriptType;

                var today = DateTime.Today;
                var month = new DateTime(today.Year, today.Month, 1);
                var numMonthsActivity = _commonFacade.GetNumMonthsActivity();
                var activityStartDateValue = month.AddMonths(-1 * numMonthsActivity); //"2015-01-01".ParseDateTime("yyyy-MM-dd");

                activityVM.SearchFilter = new ActivitySearchFilter
                {
                    ActivityStartDateTime = activityStartDateValue.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime),
                    ActivityEndDateTime = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime),
                    CardNo = custInfoVM.CardNo,
                    SubsTypeCode = subsType != null ? subsType.SubscriptTypeCode : null,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "ActivityID",
                    SortOrder = "DESC"
                };

                ViewBag.PageSize = activityVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View(activityVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch ActivityLog").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerActivity)]
        public ActionResult ActivityList(ActivitySearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Activity").ToInputLogString());

            CustomerInfoViewModel custInfoVM = new CustomerInfoViewModel();

            if (TempData["CustomerInfo"] != null)
            {
                custInfoVM = (CustomerInfoViewModel)TempData["CustomerInfo"];
                TempData["CustomerInfo"] = custInfoVM; // keep for change Tab
            }
            else
            {
                return RedirectToAction("Search", "Customer");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _auditLog = new AuditLogEntity();
                    _auditLog.Module = Constants.Module.Customer;
                    _auditLog.Action = Constants.AuditAction.ActivityLog;
                    _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                    _auditLog.CreateUserId = this.UserInfo.UserId;

                    _commonFacade = new CommonFacade();
                    _activityFacade = new ActivityFacade();
                    ActivityViewModel activityVM = new ActivityViewModel();

                    //if (string.IsNullOrWhiteSpace(searchFilter.JsonActivities))
                    //{
                    //    var results = _activityFacade.GetActivityLogList(_auditLog, searchFilter);
                    //    searchFilter.JsonActivities = JsonConvert.SerializeObject(results);
                    //}

                    activityVM.SearchFilter = searchFilter;
                    activityVM.ActivityList = _activityFacade.GetActivityLogList(_auditLog, searchFilter);

                    activityVM.SearchFilter.IsConnect = 1;

                    ViewBag.PageSize = activityVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Activity").ToSuccessLogString());
                    return PartialView("~/Views/Activity/_ActivityList.cshtml", activityVM);
                }

                return Json(new
                {
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                });
            }
            catch (CustomException cex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Activity").Add("Error Message", cex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = cex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Activity").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerActivity)]
        public ActionResult SRActivityList(ActivitySearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SR Activity").ToInputLogString());

            CustomerInfoViewModel custInfoVM = new CustomerInfoViewModel();

            if (TempData["CustomerInfo"] != null)
            {
                custInfoVM = (CustomerInfoViewModel)TempData["CustomerInfo"];
                TempData["CustomerInfo"] = custInfoVM; // keep for change Tab

                searchFilter.SrOnly = false;
                searchFilter.CustomerId = custInfoVM.CustomerId;
            }
            else
            {
                return RedirectToAction("Search", "Customer");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _activityFacade = new ActivityFacade();
                    ActivityViewModel activityVM = new ActivityViewModel();

                    activityVM.SearchFilter = searchFilter;
                    activityVM.ActivityList = _activityFacade.GetSRActivityList(searchFilter);

                    ViewBag.PageSize = activityVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Activity").ToSuccessLogString());
                    return PartialView("~/Views/Activity/_ActivityList.cshtml", activityVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Activity").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}
