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
                decimal? customerNumber = encryptedString.ToCustomerNumber();
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitList CustomerLog").Add("CustomerId", customerId).Add("CustomerNumber", customerNumber).ToInputLogString());

                if (customerId == 0 && customerNumber == 0)
                {
                    return RedirectToAction("Search", "Customer");
                }

                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();
                CustomerLogViewModel custLogVM = new CustomerLogViewModel();
                custLogVM.SearchFilter = new CustomerLogSearchFilter
                {
                    CustomerId = customerId,
                    CustomerNumber = customerNumber,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "CreateDate",
                    SortOrder = "DESC"
                };

                // CustomerInfo                    
                if (TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()] != null)
                {
                    custLogVM.CustomerInfo = (CustomerInfoViewModel)TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()];
                    TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()] = custLogVM.CustomerInfo;
                }

                //custLogVM.CustomerLogList = _customerFacade.GetCustomerLogList(custLogVM.SearchFilter);
                //ViewBag.PageSize = custLogVM.SearchFilter.PageSize;
                //ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                //ViewBag.Message = string.Empty;

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
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List CustomerLog").Add("CustomerId", searchFilter.CustomerId).Add("CustomerNumber", searchFilter.CustomerNumber)
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
