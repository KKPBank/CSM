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
    public class ExistingProductController : BaseController
    {
        private ICommonFacade _commonFacade;
        private ICustomerFacade _customerFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExistingProductController));

        [CheckUserRole(ScreenCode.ViewCustomerProducts)]
        public ActionResult List(string encryptedString)
        {
            int? customerId = encryptedString.ToCustomerId();
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List ExistingProduct").Add("CustomerId", customerId).ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();

                ExistingProductViewModel productVM = new ExistingProductViewModel();
                CustomerInfoViewModel custInfoVM = this.MappingCustomerInfoView(customerId.Value);
                productVM.CustomerInfo = custInfoVM;

                if (custInfoVM.CustomerId.HasValue)
                {
                    productVM.SearchFilter = new AccountSearchFilter
                    {
                        CustomerId = custInfoVM.CustomerId.Value,
                        PageNo = 1,
                        PageSize = _commonFacade.GetPageSizeStart(),
                        SortField = "ProductGroup",
                        SortOrder = "ASC"
                    };

                    productVM.AccountList = _customerFacade.GetAccountList(productVM.SearchFilter);
                    ViewBag.PageSize = productVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                    ViewBag.Message = string.Empty;
                }

                return View(productVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List ExistingProduct").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerProducts)]
        public ActionResult ExistingProductList(AccountSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("ExistingProductList").Add("CustomerId", searchFilter.CustomerId)
                .ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _customerFacade = new CustomerFacade();
                    ExistingProductViewModel productVM = new ExistingProductViewModel();
                    productVM.SearchFilter = searchFilter;

                    productVM.AccountList = _customerFacade.GetAccountList(productVM.SearchFilter);
                    ViewBag.PageSize = productVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("ExistingProductList").ToSuccessLogString());
                    return PartialView("~/Views/ExistingProduct/_ExistingProductList.cshtml", productVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("ExistingProductList").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerProducts)]
        public ActionResult InitViewDetailProduct(int? customerId, string product, string productGroup, string subscriptionCode, int? accountId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitViewDetailProduct").ToInputLogString());

            try
            {
                _customerFacade = new CustomerFacade();
                ExistingProductViewModel productVM = new ExistingProductViewModel();
                ExistingProductSearchFilter searchFilter = new ExistingProductSearchFilter
                {
                    CustomerId = customerId,
                    ProductType = product,
                    ProductGroup = productGroup,
                    SubscriptionCode = subscriptionCode,
                    AccountId = accountId
                };

                productVM.DetailProduct = _customerFacade.GetExistingProductDetail(searchFilter);

                return PartialView("~/Views/ExistingProduct/_DetailProduct.cshtml", productVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitViewDetailProduct").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}
