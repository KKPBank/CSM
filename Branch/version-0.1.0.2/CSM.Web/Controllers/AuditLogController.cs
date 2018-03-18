using System;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using System.Collections;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class AuditLogController : BaseController
    {
        private ICommonFacade _commonFacade;
        private IAuditLogFacade _auditlogFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AuditLogController));

        [CheckUserRole(ScreenCode.SearchAuditLog)]
        public ActionResult Search()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch AuditLog").ToInputLogString());
            try
            {
                _commonFacade = new CommonFacade();
                _auditlogFacade = new AuditLogFacade();
                AuditLogViewModel auditlogVM = new AuditLogViewModel();

                auditlogVM.SearchFilter = new AuditLogSearchFilter
                {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    DateFrom = string.Empty, 
                    DateTo = string.Empty,
                    Module = string.Empty,
                    Action = string.Empty,
                    Status = null,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "AuditLogId",
                    SortOrder = "DESC"
                };

                var moduleList = _auditlogFacade.GetModule(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.Module = new SelectList((IEnumerable)moduleList, "Key", "Value", string.Empty);                               

                var actionList = _auditlogFacade.GetAction(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.Action = new SelectList((IEnumerable)actionList, "Key", "Value", string.Empty);

                var statusList = _auditlogFacade.GetStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.Status = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                ViewBag.PageSize = auditlogVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View(auditlogVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch AuditLog").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            } 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.SearchAuditLog)]
        public ActionResult AuditLogList(AuditLogSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search AuditLog").Add("Name", searchFilter.FirstName)
               .Add("DateFrom", searchFilter.DateFrom).Add("DateTo", searchFilter.DateTo).Add("Module", searchFilter.Module)
               .Add("Action", searchFilter.Action).Add("Status", searchFilter.Status));
            try
            {
                #region "Validation"

                bool isValid = TryUpdateModel(searchFilter);
                if (!string.IsNullOrEmpty(searchFilter.DateFrom) && !searchFilter.DateFromValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("txtFromDate", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.DateFromValue.HasValue)
                {
                    if (searchFilter.DateFromValue.Value > DateTime.Now.Date)
                    {
                        isValid = false;
                        ModelState.AddModelError("txtFromDate", Resource.ValErr_InvalidDate_MustLessThanToday);
                    }
                }

                if (!string.IsNullOrEmpty(searchFilter.DateTo) && !searchFilter.DateToValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("txtToDate", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.DateToValue.HasValue)
                {
                    if (searchFilter.DateToValue.Value > DateTime.Now.Date)
                    {
                        isValid = false;
                        ModelState.AddModelError("txtToDate", Resource.ValErr_InvalidDate_MustLessThanToday);
                    }
                }

                if (searchFilter.DateFromValue.HasValue && searchFilter.DateToValue.HasValue
                    && searchFilter.DateFromValue.Value > searchFilter.DateToValue.Value)
                {
                    isValid = false;
                    ModelState.AddModelError("dvDateRange", Resource.ValErr_InvalidDateRange);
                }

                #endregion

                if (isValid)
                {
                    _commonFacade = new CommonFacade();
                    _auditlogFacade = new AuditLogFacade();
                    AuditLogViewModel auditlogVM = new AuditLogViewModel();
                    auditlogVM.SearchFilter = searchFilter;
                    auditlogVM.AuditLogList = _auditlogFacade.SearchAuditLogs(searchFilter);

                    ViewBag.PageSize = auditlogVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    return PartialView("~/Views/AuditLog/_AuditLogList.cshtml", auditlogVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search AuditLog").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            } 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LoadActionByModule(string module)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("LoadActionByModule").Add("module", module).ToInputLogString());
            try
            {
                _auditlogFacade = new AuditLogFacade();
               
                var actionList = _auditlogFacade.GetActionByModule(module,Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                var lstAction = new SelectList((IEnumerable)actionList, "Key", "Value", string.Empty);

                return Json(new
                {
                    Valid = true ,
                    lstAction = lstAction   
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("LoadActionByModule").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }
    }
}
