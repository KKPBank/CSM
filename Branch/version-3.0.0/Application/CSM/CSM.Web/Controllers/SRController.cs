using System;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using System.Linq;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class SRController : BaseController
    {
        private ICommonFacade _commonFacade;
        private ICustomerFacade _customerFacade;
        private IUserFacade _userFacade; 
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SRController));

        [CheckUserRole(ScreenCode.ViewCustomerSR)]
        public ActionResult List(string encryptedString)
        {
            int? customerId = encryptedString.ToCustomerId();
            decimal? customerNumber = encryptedString.ToCustomerNumber();
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List SR").Add("CustomerId", customerId).Add("CustomerNumber", customerNumber).ToInputLogString());

            try
            {
                if (customerId == 0 && customerNumber == 0)
                {
                    return RedirectToAction("Search", "Customer");
                }

                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();
                ViewBag.userId = this.UserInfo.UserId;
                
                SrViewModel srVM = new SrViewModel();
                srVM.SearchFilter = new SrSearchFilter
                {
                    CustomerId = customerId,
                    CustomerNumber = customerNumber,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "CreateDate",
                    SortOrder = "ASC"
                };

                if (TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()] != null)
                {
                    srVM.CustomerInfo = (CustomerInfoViewModel)TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()];
                    TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()] = srVM.CustomerInfo;
                }

                srVM.SearchFilter = new SrSearchFilter
                {
                    CustomerId = customerId,
                    CustomerNumber = customerNumber,
                    SubscripTypeName = srVM.CustomerInfo.SubscriptType.SubscriptTypeName,
                    CardNo = srVM.CustomerInfo.CardNo,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "CreateDate",
                    SortOrder = "ASC"
                };

                // Get SR list in ajax
                ViewBag.PageSize = _commonFacade.GetPageSizeStart();
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View(srVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List SR").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerSR)]
        public ActionResult CustomerSrList(SrSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SR").Add("CustomerId", searchFilter.CustomerId).Add("CustomerNumber" ,searchFilter.CustomerNumber)
                .ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _customerFacade = new CustomerFacade();
                    _userFacade = new UserFacade();
                    SrViewModel srVM = new SrViewModel();
                    srVM.SearchFilter = searchFilter;

                    srVM.SrList = _customerFacade.GetCustomerSrList(srVM.SearchFilter);
                    ViewBag.PageSize = srVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    // Set ViewBag.OwnerList
                    var lstEmployeeUser = _userFacade.GetEmployees(this.UserInfo);
                    lstEmployeeUser.Add(this.UserInfo); // add current user
                    ViewBag.OwnerList = lstEmployeeUser.Select(x => x.UserId).ToList(); // for enabled btnEdit

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("CustomerSrList").ToSuccessLogString());
                    return PartialView("~/Views/SR/_CustomerSrList.cshtml", srVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SR").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}
