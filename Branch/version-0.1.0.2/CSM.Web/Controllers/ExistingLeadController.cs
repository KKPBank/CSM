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
    public class ExistingLeadController : BaseController
    {
        private AuditLogEntity _auditLog;
        private ICustomerFacade _customerFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExistingLeadController));

        [CheckUserRole(ScreenCode.ViewCustomerLeads)]
        public ActionResult List(int? customerId = null)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Recommended Campaign").ToInputLogString());

            _auditLog = new AuditLogEntity();
            _auditLog.Module = Constants.Module.Customer;
            _auditLog.Action = Constants.AuditAction.ExistingLead;
            _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
            _auditLog.Status = LogStatus.Success;
            _auditLog.CreateUserId = this.UserInfo.UserId;

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
                ExistingLeadViewModel existingLeadVM = new ExistingLeadViewModel();
                if (custInfoVM.CustomerId != null)
                {
                    existingLeadVM.CustomerInfo = custInfoVM;
                }

                return View(existingLeadVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Recommended Campaign").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerLeads)]
        public ActionResult ExistingLeadList()
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

            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get ExistingLead List").Add("CardNo", custInfoVM.CardNo.MaskCardNo()).ToInputLogString());

            _auditLog = new AuditLogEntity();
            _auditLog.Module = Constants.Module.Customer;
            _auditLog.Action = Constants.AuditAction.ExistingLead;
            _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
            _auditLog.Status = LogStatus.Success;
            _auditLog.CreateUserId = this.UserInfo.UserId;

            try
            {
                if (ModelState.IsValid)
                {
                    _customerFacade = new CustomerFacade();
                    ExistingLeadViewModel existingLeadVM = new ExistingLeadViewModel();

                    if (!string.IsNullOrWhiteSpace(custInfoVM.CardNo))
                    {
                        existingLeadVM.Ticket = _customerFacade.GetLeadList(_auditLog, custInfoVM.CardNo);
                    }

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Get ExistingLead List").ToSuccessLogString());
                    return PartialView("~/Views/ExistingLead/_ExistingLeadList.cshtml", existingLeadVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get ExistingLead List").Add("Error Message", cex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = cex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get ExistingLead List").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}
