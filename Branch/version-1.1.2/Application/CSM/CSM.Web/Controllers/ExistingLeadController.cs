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
        private ICommonFacade _commonFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExistingLeadController));

        [CheckUserRole(ScreenCode.ViewCustomerLeads)]
        public ActionResult List(string encryptedString)
        {
            int? customerId = encryptedString.ToCustomerId();
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Recommended Campaign").Add("CustomerId", customerId).ToInputLogString());

            if (customerId == null)
            {
                return RedirectToAction("Search", "Customer");
            }

            try
            {
                ExistingLeadViewModel existingLeadVM = new ExistingLeadViewModel();
                CustomerInfoViewModel custInfoVM = this.MappingCustomerInfoView(customerId.Value);
                existingLeadVM.CustomerInfo = custInfoVM;
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
        public ActionResult ExistingLeadList(int? customerId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get ExistingLead List").Add("CustomerId", customerId).ToInputLogString());

            if (customerId == null)
            {
                return Json(new
                {
                    Valid = false,
                    RedirectUrl = Url.Action("Search", "Customer")
                });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _customerFacade = new CustomerFacade();
                    ExistingLeadViewModel existingLeadVM = new ExistingLeadViewModel();
                    CustomerInfoViewModel custInfoVM = this.MappingCustomerInfoView(customerId.Value);

                    if (!string.IsNullOrWhiteSpace(custInfoVM.CardNo))
                    {
                        _auditLog = new AuditLogEntity();
                        _auditLog.Module = Constants.Module.Customer;
                        _auditLog.Action = Constants.AuditAction.ExistingLead;
                        _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                        _auditLog.CreateUserId = this.UserInfo.UserId;

                        existingLeadVM.Ticket = _customerFacade.GetLeadList(_auditLog, custInfoVM.CardNo);
                    }

                    existingLeadVM.CustomerInfo = custInfoVM;

                    _commonFacade = new CommonFacade();
                    ViewBag.SLMUrlNewLead = _commonFacade.GetSLMUrlNewLead();
                    ViewBag.SLMUrlViewLead = _commonFacade.GetSLMUrlViewLead();
                    //                    ViewBag.SLMEncryptPassword = _commonFacade.GetSLMEncryptPassword();
                    ViewBag.SLMEncryptPassword = WebConfig.GetSLMEncryptPassword();

                    if (!string.IsNullOrEmpty(UserInfo.Username))
                        ViewBag.Username = UserInfo.Username.ToLower();

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
