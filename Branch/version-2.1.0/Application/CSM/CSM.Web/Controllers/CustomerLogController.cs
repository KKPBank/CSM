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
    public class CustomerLogController : BaseController
    {
        private ICommonFacade _commonFacade;
        private ICustomerFacade _customerFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CustomerLogController));

        [CheckUserRole(ScreenCode.ViewCustomerLogging)]
        public ActionResult List(string encryptedString)
        {
            try
            {
                int? customerId = encryptedString.ToCustomerId();
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitList CustomerLog").Add("CustomerId", customerId).ToInputLogString());

                if (customerId == null)
                {
                    return RedirectToAction("Search", "Customer");
                }

                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();
                CustomerLogViewModel custLogVM = new CustomerLogViewModel();
                CustomerInfoViewModel custInfoVM = this.MappingCustomerInfoView(customerId.Value);
                custLogVM.CustomerInfo = custInfoVM;

                if (custInfoVM.CustomerId.HasValue)
                {
                    // CustomerLog list
                    custLogVM.SearchFilter = new CustomerLogSearchFilter
                    {
                        CustomerId = custInfoVM.CustomerId.Value,
                        PageNo = 1,
                        PageSize = _commonFacade.GetPageSizeStart(),
                        SortField = "CreateDate",
                        SortOrder = "DESC"
                    };

                    custLogVM.CustomerLogList = _customerFacade.GetCustomerLogList(custLogVM.SearchFilter);
                    ViewBag.PageSize = custLogVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                    ViewBag.Message = string.Empty;
                }

                return View(custLogVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitList CustomerLog").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerLogging)]
        public ActionResult CustomerLogList(CustomerLogSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List CustomerLog").Add("CustomerId", searchFilter.CustomerId)
                .ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _customerFacade = new CustomerFacade();
                    CustomerLogViewModel custLogVM = new CustomerLogViewModel();
                    custLogVM.SearchFilter = searchFilter;

                    custLogVM.CustomerLogList = _customerFacade.GetCustomerLogList(custLogVM.SearchFilter);
                    ViewBag.PageSize = custLogVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("CustomerLogList").ToSuccessLogString());
                    return PartialView("~/Views/CustomerLog/_CustomerLogList.cshtml", custLogVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List CustomerLog").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}
