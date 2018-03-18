using System;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using System.Collections.Generic;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using CSM.Service.CBSCustomerService;
using CSM.Service.CBSAccountService;
using CSM.Service.CBSTermDepositService;
using CSM.Service.CBSLoanService;
using System.Linq;
using CSM.Service.Messages.Common;
using CSM.Service.Messages.Customer;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class ProductServiceController : BaseController
    {
        //
        // GET: /ProductService/
        private ICommonFacade _commonFacade;
        private ICustomerFacade _customerFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExistingProductController));

        [CheckUserRole(ScreenCode.ViewCustomerProducts)]
        public ActionResult List(string encryptedString)
        {
            int? customerId = encryptedString.ToCustomerId();
            decimal? customerNumber = encryptedString.ToCustomerNumber();
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List ProductService").Add("CustomerId", customerId).Add("CustomerNumber", customerNumber).ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();

                ProductServiceViewModel productVM = new ProductServiceViewModel();
                productVM.SearchFilter = new AccountSearchFilter
                {
                    CustomerId = customerId,
                    CustomerNumber = customerNumber,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "ProductGroup",
                    SortOrder = "ASC"
                };

                if (TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()] != null)
                {
                    productVM.CustomerInfo = (CustomerInfoViewModel)TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()]; //this.MappingCustomerInfoView(customerId.Value);
                    TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()] = productVM.CustomerInfo;
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

        public IEnumerable<AccountEntity> ProductServiceListWS(AccountSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("ProductServiceList").ToSuccessLogString());

            IEnumerable<AccountEntity> res = null;
            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "";
                    if (searchFilter.CustomerNumber > 0)
                    {
                        WebServicePagingEntity vPaging = new WebServicePagingEntity();
                        vPaging.PageNumber = 1;
                        vPaging.PageSize = CommonFacade.GetWSMaxReturnRows();

                        CBSAccountSummaryResponse custSumList = InquiryCustomerAccountSummaryWS(searchFilter.CustomerNumber.Value, vPaging);
                        if (custSumList.ResponseCode != "CBS-I-1000" && custSumList.ResponseCode != "CBS-M-2001")
                        {
                            ViewBag.ErrorMessage = custSumList.ResponseCode + " : " + custSumList.ResponseMessage + " (InquiryCustomerAccountSummary)";
                        }

                        CBSServiceWithBankResponse svBaknList = InquiryServiceWithBankWS(searchFilter.CustomerNumber.Value, vPaging);
                        if (svBaknList.ResponseCode != "CBS-I-1000" && svBaknList.ResponseCode != "CBS-M-2001")
                        {
                            ViewBag.ErrorMessage = svBaknList.ResponseCode + " : " + svBaknList.ResponseMessage + " (InquiryServiceWithBank)";
                        }

                        IEnumerable<AccountEntity> svInstructionList = null;
                        if (searchFilter.IsLookUpMode)
                            using (var instCtrl = new InstructionController())
                            {
                                svInstructionList = instCtrl.GetCustomerInstructionList(searchFilter.CustomerNumber.ToString(), vPaging);
                            }
                        else
                            svInstructionList = new List<AccountEntity>();

                        res = custSumList.AccountList.Union(svBaknList.AccountList).Union(svInstructionList);
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("ProductServiceList").Add("InquiryCustomerAccountSummary", custSumList.AccountList.Count()).Add("InquiryServiceWithBank", svBaknList.AccountList.Count()).ToSuccessLogString());
                    }
                    else
                    {
                        using (var facade = new CustomerFacade())
                        {
                            res = facade.GetAccountList(searchFilter);
                        }
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("ProductServiceList").ToSuccessLogString());
                    }
                    //Filter Product/Account
                    //if (searchFilter.)
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("ExistingProductList").Add("Error Message", ex.Message).ToFailLogString());
                //throw;
            }
            return (res ?? new List<AccountEntity>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerProducts)]
        public ActionResult ProductServiceList(AccountSearchFilter searchFilter)
        {
            //Call by ajax in /Views/ProductService/List.cshtml
            Logger.Info(_logMsg.Clear().SetPrefixMsg("ProductServiceList").Add("CustomerId", searchFilter.CustomerId).Add("CustomerNumber",searchFilter.CustomerNumber)
                .ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    ProductServiceViewModel productVM = new ProductServiceViewModel();
                    productVM.SearchFilter = searchFilter;

                    productVM.AccountList = ProductServiceListWS(searchFilter);

                    if (TempData["CustomerInfo" + searchFilter.CustomerId.ToString() + searchFilter.CustomerNumber.ToString()] != null)
                    {
                        productVM.CustomerInfo = (CustomerInfoViewModel)TempData["CustomerInfo" + searchFilter.CustomerId.ToString() + searchFilter.CustomerNumber.ToString()]; //this.MappingCustomerInfoView(customerId.Value);
                        TempData["CustomerInfo" + searchFilter.CustomerId.ToString() + searchFilter.CustomerNumber.ToString()] = productVM.CustomerInfo;
                    }

                    #region "Paging"
                    productVM.SearchFilter.TotalRecords = productVM.AccountList.ToList<AccountEntity>().Count;
                    int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
                    if (startPageIndex >= searchFilter.TotalRecords)
                    {
                        startPageIndex = 0;
                        searchFilter.PageNo = 1;
                    }

                    switch (searchFilter.SortField)
                    {
                        case "AccountTypeDescription":
                            productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                                ? productVM.AccountList.OrderBy(q => q.AccountTypeDescription)
                                : productVM.AccountList.OrderByDescending(q => q.AccountTypeDescription);
                            break;
                        case "Product":
                            productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                                ? productVM.AccountList.OrderBy(q => q.Product)
                                : productVM.AccountList.OrderByDescending(q => q.Product);
                            break;
                        //case "Grade":
                        //    productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        //        ? productVM.AccountList.OrderBy(q => q.Grade)
                        //        : productVM.AccountList.OrderByDescending(q => q.Grade);
                        //    break;
                        case "AccountNo":
                            productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                                ? productVM.AccountList.OrderBy(q => q.AccountNo)
                                : productVM.AccountList.OrderByDescending(q => q.AccountNo);
                            break;
                        case "CarNo":
                            productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                                ? productVM.AccountList.OrderBy(q => q.CarNo)
                                : productVM.AccountList.OrderByDescending(q => q.CarNo);
                            break;
                        case "BranchName":
                            productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                                ? productVM.AccountList.OrderBy(q => q.BranchName)
                                : productVM.AccountList.OrderByDescending(q => q.BranchName);
                            break;
                        case "EffectiveDate":
                            productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                                ? productVM.AccountList.OrderBy(q => q.EffectiveDate)
                                : productVM.AccountList.OrderByDescending(q => q.EffectiveDate);
                            break;
                        case "ExpiryDate":
                            productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                                ? productVM.AccountList.OrderBy(q => q.ExpiryDate)
                                : productVM.AccountList.OrderByDescending(q => q.ExpiryDate);
                            break;
                        case "Status":
                            productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                                ? productVM.AccountList.OrderBy(q => q.AccountStatus)
                                : productVM.AccountList.OrderByDescending(q => q.AccountStatus);
                            break;
                        default:
                            productVM.AccountList = productVM.AccountList.OrderByDescending(q => q.AccountNo);
                            break;
                    }

                    productVM.AccountList = productVM.AccountList.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<AccountEntity>();

                    ViewBag.PageSize = productVM.SearchFilter.PageSize;
                    using (var facade = new CommonFacade())
                    {
                        ViewBag.PageSizeList = facade.GetPageSizeList();
                    }
                    #endregion

                    return PartialView("~/Views/ProductService/_ProductServiceList.cshtml", productVM);
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
        public ActionResult ViewProductDetail(ViewProductDetailSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("View Product Detail " + searchFilter.ProductGroup).ToInputLogString());
            try {
                HttpContext.Response.AppendHeader("X-XSS-Protection", "0");
                _commonFacade = new CommonFacade();

                CustomerInfoViewModel custInfo = new CustomerInfoViewModel();
                if (TempData["CustomerInfo" + searchFilter.CustomerId.ToString() + searchFilter.CustomerNumber.ToString()] != null)
                {
                    custInfo = (CustomerInfoViewModel)TempData["CustomerInfo" + searchFilter.CustomerId.ToString() + searchFilter.CustomerNumber.ToString()];
                    TempData["CustomerInfo" + searchFilter.CustomerId.ToString() + searchFilter.CustomerNumber.ToString()] = custInfo;
                }

                if (searchFilter.ProductGroup == "LOAN")
                {
                    LoanAccountDetailViewModel LDetail = getLoanAccountOverview(searchFilter.AccountNumber, searchFilter.CustomerNumber.ToString(), custInfo.CustomerTypeDisplay, custInfo.BirthDateDisplay, custInfo.CardNo);
                    return View("LoanAccountDetail", LDetail);
                }
                else if (searchFilter.ProductGroup == "TD")
                {

                    TDAccountDetailViewModel TDModel = InquiryTDAccountDetail(searchFilter.AccountNumber, searchFilter.CustomerNumber.ToString(), custInfo.CustomerTypeDisplay, custInfo.BirthDateDisplay, custInfo.CardNo, custInfo.CountryOfCitizenship, custInfo.FirstName, custInfo.LastName, searchFilter);
                    if (TDModel != null)
                    {
                        searchFilter.PageNo = 1;
                        searchFilter.PageSize = _commonFacade.GetPageSizeStart();
                        searchFilter.SortField = "TDPlacementNumber";
                        searchFilter.SortOrder = "desc";
                        searchFilter.TotalRecords = (TDModel.TDAccountList == null ? 0 : TDModel.TDAccountList.ToList().Count);

                        int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
                        if (startPageIndex >= searchFilter.TotalRecords)
                        {
                            startPageIndex = 0;
                            searchFilter.PageNo = 1;
                        }

                        TDModel.SearchFilter = searchFilter;
                    }
                    ViewBag.PageSize = searchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    return View("TDAccountDetail", TDModel);
                }
                else if (searchFilter.ProductGroup == "CASA") {
                    CASAAccountDetailViewModel CASAModel = InquiryCASAAccountDetail(searchFilter.AccountNumber, searchFilter.CustomerNumber.ToString(), custInfo.CustomerTypeDisplay, custInfo.BirthDateDisplay, custInfo.CardNo, custInfo.CountryOfCitizenship, custInfo.FirstName, custInfo.LastName);
                    //CASAAccountDetailViewModel CASAModel = InquiryCASAAccountDetail("1000000151", searchFilter.CustomerNumber.ToString(), custInfo.CustomerTypeDisplay, custInfo.BirthDateDisplay, custInfo.CardNo, custInfo.CountryOfCitizenship, custInfo.FirstName, custInfo.LastName, searchFilter);
                    if (CASAModel != null)
                    {
                        //searchFilter.PageNo = searchFilter.PageNo;
                        //searchFilter.PageSize = _commonFacade.GetPageSizeStart();
                        if (CASAModel.TransactionList != null)
                        {
                            searchFilter.SortField = "TransactionDate";
                            searchFilter.SortOrder = "desc";
                            searchFilter.TotalRecords = (CASAModel.TransactionList == null ? 0 : CASAModel.TransactionList.ToList().Count);

                            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
                            if (startPageIndex >= searchFilter.TotalRecords)
                            {
                                startPageIndex = 0;
                                searchFilter.PageNo = 1;
                            }
                            CASAModel.TransactionList = CASAModel.TransactionList.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<CASATransactionEntity>();
                        }

                        CASAModel.SearchFilter = searchFilter;
                    }

                    ViewBag.PageSize = searchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                   
                    return View("CASAAccountDetail", CASAModel);
                }
                else
                {
                    throw new Exception("No have this case");
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("View Product Detail " + searchFilter.ProductGroup).Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
            

        }
        #region "Get Customer Account Summary from webservice"

        public CBSAccountSummaryResponse InquiryCustomerAccountSummaryWS(decimal CustomerNumber,  WebServicePagingEntity vPaging) {
            CBSAccountService ws = new CBSAccountService();
            //IEnumerable<AccountEntity> result = new List<AccountEntity>();
            InquiryCustomerAccountSummaryResponse res = new InquiryCustomerAccountSummaryResponse();
            CBSAccountSummaryResponse result = new CBSAccountSummaryResponse();

            try {
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryCustomerAccountSummary);

                InquiryCustomerAccountSummary req = new InquiryCustomerAccountSummary();
                req.Header = new InquiryCustomerAccountSummaryHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerAccountSummaryWS").Add("ReferenceNo:", req.Header.ReferenceNo).Add("CustomerNumber", CustomerNumber));

                req.InquiryCustomerAccountSummaryRequest = new InquiryCustomerAccountSummaryInquiryCustomerAccountSummaryRequest();
                req.InquiryCustomerAccountSummaryRequest.CustomerNumber = CustomerNumber.ToString();
                //req.InquiryCustomerAccountSummaryRequest.CustomerNumber = "3740000000008"; // CustomerNumber.ToString();

                req.Paging = new InquiryCustomerAccountSummaryPaging();
                req.Paging.PageNumber = 1;
                req.Paging.PageSize = vPaging.PageSize;
                SaveServiceRequestLog(req.SerializeObject());

                res = ws.InquiryCustomerAccountSummary(req);
                if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000" || res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001")
                {
                    result.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    result.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                    if (res.Header.AccountList != null)
                    {
                        result.AccountList = new List<AccountEntity>();
                        result.AccountList = (from c in res.Header.AccountList
                                              select new AccountEntity()
                                              {
                                                  CustomerId = 0,
                                                  CustomerNumber = CustomerNumber,
                                                  AccountNo = c.AccountNumber,
                                                  Product = c.ProductCode,
                                                  ProductGroup = c.AccountTypeCode,
                                                  //Grade =  c.AccountStatus,
                                                  BundleCode = c.BundleCode,
                                                  AccountType = c.AccountType,
                                                  AccountTypeDescription = _commonFacade.GetAccountTypeByCode(c.SystemCode, c.AccountTypeCode, c.AccountType).AccountTypeName,
                                                  AccountStatus = c.AccountStatus,
                                                  StatusDisplay = (c.AccountStatus == "" ? "" : _commonFacade.GetConfigAccountStatus(Convert.ToInt32(c.AccountStatus)).AccountStatusName),
                                                  CustomerName = c.AccountName,
                                                  InquiryServiceName = Constants.ServiceName.InquiryCustomerAccountSummary
                                              }).AsQueryable();

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerAccountSummaryWS").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found data " + result.AccountList.Count() + "Row(s)"));
                    }
                    else
                    {
                        result.AccountList = new List<AccountEntity>();
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerAccountSummaryWS").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                    }
                }
                else {
                    result.AccountList = new List<AccountEntity>();
                    result.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    result.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerAccountSummaryWS").Add("Error Message", ex.Message).ToFailLogString());
                result.ResponseCode = "Exception ";
                result.ResponseMessage = ex.Message;
            }
            SaveServiceResponseLog(res.SerializeObject());

            return result;
        }

        CBSServiceWithBankResponse InquiryServiceWithBankWS(decimal CustomerNumber,  WebServicePagingEntity vPaging)
        {
            CBSCustomerService ws = new CBSCustomerService();
            InquiryServiceWithBankResponse res = new InquiryServiceWithBankResponse();
            CBSServiceWithBankResponse result = new CBSServiceWithBankResponse();
            try
            {
                //InquiryServiceWithBank 
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryServiceWithBank);

                InquiryServiceWithBank req = new InquiryServiceWithBank();
                req.Header = new InquiryServiceWithBankHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryServiceWithBank").Add("ReferenceNo:", req.Header.ReferenceNo).Add("CustomerNumber", CustomerNumber));

                req.InquiryServiceWithBankRequest = new InquiryServiceWithBankInquiryServiceWithBankRequest();
                req.InquiryServiceWithBankRequest.CustomerNumber = CustomerNumber.ToString();

                req.Paging = new InquiryServiceWithBankPaging();
                req.Paging.PageNumber = 1;
                req.Paging.PageSize = vPaging.PageSize;
                SaveServiceRequestLog(req.SerializeObject());

                
                res = ws.InquiryServiceWithBank(req);
                if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000" || res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001")
                {
                    result.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    result.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                    if (res.Header.ServiceList != null)
                    {
                        result.AccountList = new List<AccountEntity>();
                        result.AccountList = (from c in res.Header.ServiceList
                                  select new AccountEntity
                                  {
                                      CustomerId = 0,
                                      CustomerNumber = CustomerNumber,
                                      AccountNo = c.AccountNumber,
                                      Product = c.ProductCode,
                                      ProductGroup = c.AccountTypeCode,
                                      Grade = c.AccountStatus,
                                      EffectiveDate = c.EffectiveDate.ConvertTimeZoneFromUtc(),
                                      ExpiryDate = c.ValidTill.ConvertTimeZoneFromUtc(),
                                      BundleCode = "",
                                      CustomerEmail = c.CustomerEMail,
                                      CustomerMobileNo = c.CustomerMobileNumber,
                                      AccountType = c.AccountType,
                                      AccountTypeDescription = _commonFacade.GetAccountTypeByCode(c.SystemCode, c.AccountTypeCode, c.AccountType).AccountTypeName,
                                      AccountStatus = c.AccountStatus,
                                      StatusDisplay = (c.AccountStatus == "" ? "" : _commonFacade.GetConfigAccountStatus(Convert.ToInt32(c.AccountStatus)).AccountStatusName),
                                      InquiryServiceName = Constants.ServiceName.InquiryServiceWithBank
                                      //AccountStatus = c.AccountStatus,
                                      //StatusDisplay = (c.SubscriptionStatus == "" ? "" : _commonFacade.GetConfigCasaAccountStatus(Convert.ToInt32(c.AccountStatus)).CasaAccountStatusName),
                                  });
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryServiceWithBank").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found data " + result.AccountList.Count<AccountEntity>() + " Row(s)"));
                    }
                    else
                    {
                        result.AccountList = new List<AccountEntity>();
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryServiceWithBank").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                    }
                }
                else {
                    //มี Error Code
                    result.AccountList = new List<AccountEntity>();
                    result.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    result.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getInquiryServiceWithBankWS").Add("Error Message", ex.Message).ToFailLogString());
                result.ResponseCode = "Exception ";
                result.ResponseMessage = ex.Message;
            }
            SaveServiceResponseLog(res.SerializeObject());

            return result;
        }
        #endregion

        #region "Get Loan Account Detail from web service"
        LoanAccountDetailViewModel getLoanAccountOverview(string AccountNumber, string CustomerNumber, string CustomerType, string BirthDate, string CardNo) {
            LoanAccountDetailViewModel result = null;
            try {
                InquiryLoanAccountOverviewResponse res = InquiryLoanAccountOverviewWS(AccountNumber, CustomerNumber);
                if (res.Header.LoanAccountInfo != null)
                {
                    _commonFacade = new CommonFacade();
                    result = (from c in res.Header.LoanAccountInfo
                              select new LoanAccountDetailViewModel()
                              {
                                  CustomerNumber = CustomerNumber,
                                  BirthDateDisplay = BirthDate,
                                  IDNumber = CardNo,
                                  PhoneNumber = c.PhoneNumber,
                                  Address = c.Address,
                                  //BankName = (!string.IsNullOrEmpty(c.BankNumber) ? _commonFacade.GetBankByBankNo(c.BankNumber).BankName : ""),
                                  AccountNumber = c.AccountNumber,
                                  AccountName = c.AccountName,
                                  ApprovalDate = c.ApprovalDate.ConvertTimeZoneFromUtc(),
                                  DrawingLimit = c.DrawingLimit,
                                  PaymentDayOfMonth = c.PaymentDayOfMonth,
                                  FirstPaymentDate = c.FirstPaymentDate.ConvertTimeZoneFromUtc(),
                                  LoanInterestRate = c.LoanInterestRate.ToString(),
                                  Term = c.Term.ToString(),
                                  PaymentAmount = c.PaymentAmount.FormatDecimal(),
                                  MaturityDate = c.MaturityDate.ConvertTimeZoneFromUtc(),
                                  StatusAccountId = _commonFacade.GetConfigAccountStatus(Convert.ToInt32(c.StatusAccountCode)).AccountStatusId.ToString(),
                                  StatusAccountCode = c.StatusAccountCode,
                                  StatusDescription = c.StatusDescription,
                                  Reference1 = c.Reference1,
                                  Reference2 = c.Reference2,
                                  AFTLoan = c.AFTLoan,
                                  InsuranceFlag = c.InsuranceFlag,
                              }).FirstOrDefault();
                }
                else
                {
                    //ถ้าไม่พบข้อมูลให้แสดงเฉพาะ Customer Profile
                    result = new LoanAccountDetailViewModel {
                        CustomerNumber = CustomerNumber,
                        AccountNumber = AccountNumber,
                        BirthDateDisplay = BirthDate,
                        IDNumber = CardNo,
                    };
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getLoanAccountDetailWS").Add("Error Message", ex.Message).ToFailLogString());
            }
            return result;
        }

        public InquiryLoanAccountDetailResponse InqueryLoanAccountDetailWS(string AccountNumber)
        {
            CBSLoanService ws = new CBSLoanService();
            //ws.Proxy = getWebProxy();
            InquiryLoanAccountDetailResponse result = null;
            try {
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryLoanAccountOverview);

                InquiryLoanAccountDetail req = new InquiryLoanAccountDetail();
                req.Header = new InquiryLoanAccountDetailHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InqueryLoanAccountDetailWS").Add("ReferenceNo:", req.Header.ReferenceNo).Add("AccountNumber", AccountNumber));

                req.LoanAccountDetailRequest = new InquiryLoanAccountDetailLoanAccountDetailRequest();
                req.LoanAccountDetailRequest.AccountNumber = AccountNumber;
                SaveServiceRequestLog(req.SerializeObject());

                result = new InquiryLoanAccountDetailResponse();
                result = ws.InquiryLoanAccountDetail(req);
                if (result.Header.LoanAccountInfo != null)
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InqueryLoanAccountDetailWS").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found loan account data"));
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InqueryLoanAccountDetailWS").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data foud"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InqueryLoanAccountDetailWS").Add("Error Message", ex.Message).ToFailLogString());
            }

            SaveServiceResponseLog(result.SerializeObject());

            return result;
        }

        public InquiryLoanAccountOverviewResponse InquiryLoanAccountOverviewWS(string AccountNumber, string CustomerNumber)
        {
            CBSLoanService ws = new CBSLoanService();
            InquiryLoanAccountOverviewResponse result = new InquiryLoanAccountOverviewResponse();
            try
            {
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryLoanAccountOverview);

                InquiryLoanAccountOverview req = new InquiryLoanAccountOverview();
                req.Header = new InquiryLoanAccountOverviewHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryLoanAccountOverviewWS").Add("ReferenceNo:", req.Header.ReferenceNo).Add("AccountNumber", AccountNumber).Add("CustomerNumber", CustomerNumber));

                req.InquiryLoanAccountOverviewRequest = new InquiryLoanAccountOverviewInquiryLoanAccountOverviewRequest();
                req.InquiryLoanAccountOverviewRequest.AccountNumber = AccountNumber;
                SaveServiceRequestLog(req.SerializeObject());

                result = ws.InquiryLoanAccountOverview(req);
                if (result.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000" || result.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001")
                {
                    if (result.Header.LoanAccountInfo != null)
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryLoanAccountOverviewWS").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found loan account data"));
                    }
                    else
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryLoanAccountOverviewWS").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data foud"));
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = result.Header.ResponseStatusInfo.ResponseStatus.ResponseCode + " : " + result.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryLoanAccountOverviewWS").Add("Error Message", ex.Message).ToFailLogString());
                ViewBag.ErrorMessage = "Exception : " + ex.Message;
            }

            SaveServiceResponseLog(result.SerializeObject());
            return result;
        }
        #endregion

        #region "Get CASA Account Detail from web service"
        CASAAccountDetailViewModel InquiryCASAAccountDetail(string AccountNumber, string CustomerNumber, string CustomerType, string BirthDate, string CardNo, string CountryOfCitizenship, string FirstName, string LastName)
        {
            CASAAccountDetailViewModel result = null;
            CBSCasaAccountDetailResponse res = InquiryCASAAccountDetailResponseWS(AccountNumber, CustomerNumber);
            if (res.ResponseCode == "CBS-I-1000" || res.ResponseCode == "CBS-M-2001")
            {
                if (res.CasaDetail.Header.AccountInfo != null)
                {
                    InquiryCASAAccountDetailResponseHeaderAccountInfoAccount acc = res.CasaDetail.Header.AccountInfo.Account;
                    result = new CASAAccountDetailViewModel()
                    {
                        CIF_ID = CustomerNumber,
                        CustomerType = CustomerType,
                        BirthDate = BirthDate,
                        IDNumber = CardNo,
                        CountryOfCitizenship = CountryOfCitizenship,
                        CustomerFirstName = FirstName,
                        CustomerLastName = LastName,
                        AccountAddress = getAccountAddress(AccountNumber),
                        AccountNumber = acc.AccountNumber,
                        AccountName = acc.AccountName,
                        LocalDescription = acc.LocalDescription,
                        AvailableBalance = acc.AvailableBalance.FormatDecimal(),
                        DateOpened = acc.OpenedDate.ConvertTimeZoneFromUtc(), //ใน Webservice ไม่มีฟิลด์ DateOpen
                        AccountStatus = _commonFacade.GetConfigAccountStatus(acc.Status).AccountStatusName,
                        Officer = acc.Officer,
                        PassbookFlag = acc.PassbookFlag,
                        SignatureConditionCode = acc.SignatureConditionCode,
                        SignatureConditionDescription = acc.SignatureConditionDescription,
                        BranchName = _commonFacade.GetCbsBranchByCode(acc.BranchNumber.ToString()).BranchName,
                        TransactionList = InquiryCASAStatementHistoryList(AccountNumber, acc.AccountType)
                    };
                }
                else
                {
                    //ถ้าไม่พบข้อมูลให้แสดงเฉพาะ Customer Profile
                    result = new CASAAccountDetailViewModel()
                    {
                        CIF_ID = CustomerNumber,
                        CustomerType = CustomerType,
                        BirthDate = BirthDate,
                        IDNumber = CardNo,
                        CountryOfCitizenship = CountryOfCitizenship,
                        CustomerFirstName = FirstName,
                        CustomerLastName = LastName,
                        AccountAddress = getAccountAddress(AccountNumber),
                        AccountNumber = AccountNumber,
                    };
                }
            }
            else
            {
                ViewBag.ErrorMessage = res.ResponseCode + " : " + res.ResponseMessage + " (InquiryCASAAccountDetail)";

                //ถ้าไม่พบข้อมูลให้แสดงเฉพาะ Customer Profile
                result = new CASAAccountDetailViewModel()
                {
                    CIF_ID = CustomerNumber,
                    CustomerType = CustomerType,
                    BirthDate = BirthDate,
                    IDNumber = CardNo,
                    CountryOfCitizenship = CountryOfCitizenship,
                    CustomerFirstName = FirstName,
                    CustomerLastName = LastName,
                    AccountAddress = getAccountAddress(AccountNumber),
                    AccountNumber = AccountNumber,
                };
            }

            return result;
        }

        public CBSCasaAccountDetailResponse InquiryCASAAccountDetailResponseWS(string AccountNumber, string CustomerNumber)
        {
            CBSAccountService ws = new CBSAccountService();
            //ws.Proxy = getWebProxy();
            CBSCasaAccountDetailResponse ret = new CBSCasaAccountDetailResponse();
            InquiryCASAAccountDetailResponse res = new InquiryCASAAccountDetailResponse();
            try
            {
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryCASAAccountDetail);

                InquiryCASAAccountDetail req = new InquiryCASAAccountDetail();
                req.Header = new InquiryCASAAccountDetailHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };
                req.InquiryCASAAccountDetailRequest = new InquiryCASAAccountDetailInquiryCASAAccountDetailRequest();
                req.InquiryCASAAccountDetailRequest.AccountNumber = AccountNumber;
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAAccountDetail").Add("ReferenceNo:", req.Header.ReferenceNo).Add("AccountNumber", AccountNumber).Add("CustomerNumber", CustomerNumber));
                SaveServiceRequestLog(req.SerializeObject());

                res = ws.InquiryCASAAccountDetail(req);
                if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000" || res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001")
                {
                    ret.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    ret.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                    if (res.Header.AccountInfo != null)
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAAccountDetail").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found CASA Account Detail Data"));
                    }
                    else
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAAccountDetail").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Data not found"));
                    }
                    ret.CasaDetail = res;
                }
                else
                {
                    ret.CasaDetail = new InquiryCASAAccountDetailResponse();
                    ret.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    ret.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getCASAAccountDetailWS").Add("Error Message", ex.Message).ToFailLogString());
                ret.ResponseCode = "Exception : ";
                ret.ResponseMessage = ex.Message;
            }

            SaveServiceResponseLog(res.SerializeObject());

            return ret;

        }

        IEnumerable<CASATransactionEntity> InquiryCASAStatementHistoryList(string AccountNumber, string AccountType)
        {
            CBSAccountService ws = new CBSAccountService();
            //ws.Proxy = getWebProxy();
            IEnumerable<CASATransactionEntity> result = new List<CASATransactionEntity>();
            InquiryCASAStatementHistoryResponse res = new InquiryCASAStatementHistoryResponse();
            try
            {
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryCASAStatementHistory);

                InquiryCASAStatementHistory req = new InquiryCASAStatementHistory();
                req.Header = new InquiryCASAStatementHistoryHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };

                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAStatementHistoryList").Add("ReferenceNo:", req.Header.ReferenceNo).Add("AccountNumber", AccountNumber).Add("AccountType", AccountType));

                DateTime HisStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddMonths(0 - _commonFacade.GetCASAStatementHisMonth());

                req.InquiryCASAStatementHistoryRequest = new InquiryCASAStatementHistoryInquiryCASAStatementHistoryRequest();
                req.InquiryCASAStatementHistoryRequest.AccountNumber = AccountNumber;
                req.InquiryCASAStatementHistoryRequest.AccountType = AccountType;
                req.InquiryCASAStatementHistoryRequest.StartDate = HisStartDate;
                req.InquiryCASAStatementHistoryRequest.EndDate = DateTime.Today;

                req.Paging = new InquiryCASAStatementHistoryPaging();
                req.Paging.PageNumber = 1; //searchFilter.PageNo;
                req.Paging.PageSize = _commonFacade.GetPageSizeStart();
                SaveServiceRequestLog(req.SerializeObject());

                res = ws.InquiryCASAStatementHistory(req);
                if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000" || res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001")
                {
                    if (res.Header.StatementList != null)
                    {
                        result = (from c in res.Header.StatementList
                                select new CASATransactionEntity()
                                {
                                    TransactionDate = c.TransactionDate.ConvertTimeZoneFromUtc(),
                                    PostingTimeStamp = (c.PostingTimestamp != null ? c.PostingTimestamp.ConvertTimeZoneFromUtc().Value.ToString("dd/MM/yyyy HH:mm:ss",new System.Globalization.CultureInfo("en-US")) : ""),
                                    TransactionDescription = c.TransactionDescription,
                                    ChequeNumber = c.ChequeNumber,   //เลขที่เช็ค ไม่น่าจะเป็น long นะ
                                    DebitCreditCode = (c.DebitCreditCode == "D" ? "Debit" : "Credit"),
                                    TransactionBranchDescription = c.TransactionBranchDescription,
                                    TransactionAmount = c.TransactionAmount.FormatDecimal(),
                                });
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAStatementHistoryList").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found Data " + result.ToList<CASATransactionEntity>().Count() + "Row(s)"));
                    }
                    else
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAStatementHistoryList").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                    }
                }
                else
                {
                    //มี Error Code
                    ViewBag.ErrorMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode + " : " + res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage + " (InquiryCASAStatementHistory)";
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAStatementHistoryList").Add("Error Message", ex.Message).ToFailLogString());
                ViewBag.ErrorMessage = "Exception : " + ex.Message;
            }
            SaveServiceResponseLog(res.SerializeObject());

            return result;
        }
        #endregion

        #region "Get TD Account Detail from web service"
        TDAccountDetailViewModel InquiryTDAccountDetail(string AccountNumber,string CustomerNumber, string CustomerType, string BirthDate, string CardNo, string CountryOfCitizenship, string FirstName, string LastName, ViewProductDetailSearchFilter searchFilter)
        {
            CBSAccountService ws = new CBSAccountService();
            //ws.Proxy = getWebProxy();
            TDAccountDetailViewModel result = new TDAccountDetailViewModel();
            try
            {
                WebServicePagingEntity vPaging = new WebServicePagingEntity() {
                    PageNumber = 1,
                    PageSize = CommonFacade.GetWSMaxReturnRows(),
                };

                result = new TDAccountDetailViewModel()
                {
                    CIF_ID=CustomerNumber,
                    CustomerType=CustomerType,
                    BirthDate=BirthDate,
                    IDNumber=CardNo,
                    CountryOfCitizenship=CountryOfCitizenship,
                    CustomerFirstName=FirstName,
                    CustomerLastName=LastName,
                    AccountAddress = getAccountAddress(AccountNumber),
                    TDAccountList= InquiryTDAccountList(AccountNumber, vPaging)
                };
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountDetail").Add("Error Message", ex.Message).ToFailLogString());
            }

            return result;
        }

        IEnumerable<TDAccountEntity> InquiryTDAccountList(string AccountNumber, WebServicePagingEntity vPaging)
        {
            CBSTermDepositService ws = new CBSTermDepositService();
            IEnumerable<TDAccountEntity> result = new List<TDAccountEntity>();
            InquiryTDAccountByGroupResponse res = new InquiryTDAccountByGroupResponse();

            try {
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryTDAccountByGroup);

                InquiryTDAccountByGroup req = new InquiryTDAccountByGroup();
                req.Header = new InquiryTDAccountByGroupHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountList").Add("ReferenceNo:", req.Header.ReferenceNo).Add("AccountNumber", AccountNumber));

                req.InquiryTDAccountByGroupRequest = new InquiryTDAccountByGroupInquiryTDAccountByGroupRequest();
                req.InquiryTDAccountByGroupRequest.TDGroupNumber = AccountNumber;

                req.Paging = new InquiryTDAccountByGroupPaging();
                req.Paging.PageNumber = vPaging.PageNumber;
                req.Paging.PageSize = vPaging.PageSize;
                req.Paging.SortColumn = vPaging.SortColumn;
                req.Paging.SortDirection = vPaging.SortDirection;
                SaveServiceRequestLog(req.SerializeObject());

                res = ws.InquiryTDAccountByGroup(req);
                if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000" || res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001")
                {
                    if (res.Header.PlacementList != null)
                    {
                        _commonFacade = new CommonFacade();
                        result = (from c in res.Header.PlacementList
                                  select new TDAccountEntity
                                  {
                                      CustomerNumber = c.CustomerNumber,
                                      TDGroupNumber = c.TDGroupNumber,
                                      TDPlacementNumber = c.TDPlacementNumber,
                                      TDAccountTypeDescription = c.AccountTypeDescription,
                                      RenewalCounter = c.RenewalCounter,
                                      ProductTerm = c.Term,
                                      ProductTermCode = c.TermCode,
                                      EffectiveDate = c.EffectiveDate.ConvertTimeZoneFromUtc(),
                                      MaturityDate = c.MaturityDate.ConvertTimeZoneFromUtc(),
                                      InterestRatePercentage = c.InterestRatePercentage,
                                      OriginalAmount = c.OriginalAmount.ToString("#,##0.00"),
                                      AccountStatus = c.AccountStatus,
                                      AccountStatusDescription = c.AccountStatusDescription,
                                      BranchName = _commonFacade.GetCbsBranchByCode(c.BranchNumber.ToString()).BranchName,
                                      ReceiptSerialNumber = c.ReceiptSerialNumber,
                                  });

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountList").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found data " + result.Count<TDAccountEntity>() + " Row(s)"));
                    }
                    else
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountList").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode + " : " + res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage + " (InquiryTDAccountByGroup)";
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountList").Add("Error Message", ex.Message).ToFailLogString());
                ViewBag.ErrorMessage = "Exception : " + ex.Message;
            }
            SaveServiceResponseLog(res.SerializeObject());

            return result;
        }

        //public InquiryTDAccountDetailResponse InquiryTDAccountDetailWS(string AccountNumber) {
        //    CBSTermDepositService ws = new CBSTermDepositService();
        //    InquiryTDAccountDetailResponse res = new InquiryTDAccountDetailResponse();
        //    try
        //    {
        //        _commonFacade = new CommonFacade();
        //        Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryTDAccountDetail);

        //        InquiryTDAccountDetail req = new InquiryTDAccountDetail();
        //        req.Header = new InquiryTDAccountDetailHeader
        //        {
        //            ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
        //            TransactionDateTime = DateTime.Now,
        //            ServiceName = profile.service_name,
        //            SystemCode = profile.system_code,
        //            ChannelID = profile.channel_id
        //        };
        //        req.InquiryTDAccountDetailRequest = new InquiryTDAccountDetailInquiryTDAccountDetailRequest();
        //        req.InquiryTDAccountDetailRequest.TDPlacementNumber = AccountNumber;
        //        //req.InquiryTDAccountDetailRequest.AccountType = Constants.AccountType.TermDepositAccount;
        //        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountDetail").Add("ReferenceNo:", req.Header.ReferenceNo).Add("AccountNumber", AccountNumber));
        //        SaveServiceRequestLog(req.SerializeObject());

        //        res = ws.InquiryTDAccountDetail(req);
        //        if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000" || res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001")
        //        {
        //            if (res.Header.PlacementInfo != null)
        //            {
        //                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountDetail").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found TD Account Detail Data"));
        //            }
        //            else
        //            {
        //                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountDetail").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Data not found"));
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.ErrorMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode + " : " + res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("Exception occur:\n", ex);
        //        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountDetailWS").Add("Error Message", ex.Message).ToFailLogString());

        //        ViewBag.ErrorMessage = "Exception : " + ex.Message;
        //    }

        //    SaveServiceResponseLog(res.SerializeObject());

        //    return res;
        //}

        InquiryAccountAddressEntity getAccountAddress(string AccountNumber) {
            CBSAccountService ws = new CBSAccountService();
            //ws.Proxy = getWebProxy();
            InquiryAccountAddressEntity result = new InquiryAccountAddressEntity();

            try {
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryAccountAddress);

                InquiryAccountAddress req = new InquiryAccountAddress();
                req.Header = new InquiryAccountAddressHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryAccountAddress").Add("ReferenceNo:", req.Header.ReferenceNo).Add("AccountNumber", AccountNumber));

                req.InquiryAccountAddressRequest = new InquiryAccountAddressInquiryAccountAddressRequest();
                req.InquiryAccountAddressRequest.AccountNumber = AccountNumber;
                SaveServiceRequestLog(req.SerializeObject());

                InquiryAccountAddressResponse res = new InquiryAccountAddressResponse();
                res = ws.InquiryAccountAddress(req);

                if (res.Header.AddressInfo[0].AddressFormat == Constants.AddressFormat.LocalAddress)
                {
                    if (res.Header.AddressInfo[0].LocalAddressInfo != null)
                    {
                        result = (from c in res.Header.AddressInfo[0].LocalAddressInfo
                                  select new InquiryAccountAddressEntity
                                  {
                                      AccountNumber = AccountNumber,
                                      AccountType = res.Header.AddressInfo[0].AccountType,
                                      AccountTypeDescription = res.Header.AddressInfo[0].AccountTypeDescription,
                                      PhoneNo = res.Header.AddressInfo[0].Mobile,
                                      EMail = res.Header.AddressInfo[0].Email,
                                      AddressFormat = res.Header.AddressInfo[0].AddressFormat,
                                      HouseNumber = c.HouseNumber,
                                      MooLabel = c.MooLabel,
                                      Moo = c.Moo,
                                      FloorNumberLabel = c.FloorNumberLabel,
                                      FloorNumber = c.FloorNumber,
                                      RoomNumberLabel = c.RoomNumberLabel,
                                      RoomNumber = c.RoomNumber,
                                      Building = c.Building,
                                      SoiLabel = c.SoiLabel,
                                      Soi = c.Soi,
                                      SubDistrict = _commonFacade.GetSubDistrictByCode(c.SubDistrictCode),
                                      District = _commonFacade.GetDistrictByCode(c.DistrictCode),
                                      Province = _commonFacade.GetProvinceByCode(c.ProvinceCode),
                                      PostalCode = c.PostalCode,
                                  }).FirstOrDefault();

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryAccountAddress").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found address data " + result.AddressDisplay));
                    }
                    else
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryAccountAddress").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                    }
                }
                else {
                    result = (from c in res.Header.AddressInfo
                              select new InquiryAccountAddressEntity
                              {
                                  AccountNumber = c.AccountNumber,
                                  AccountType = c.AccountType,
                                  AccountTypeDescription = c.AccountTypeDescription,
                                  PhoneNo = c.Mobile,
                                  EMail = c.Email,
                                  AddressFormat = res.Header.AddressInfo[0].AddressFormat,
                                  HouseNumber = c.AddressLine1,
                                  SoiLabel = (!string.IsNullOrWhiteSpace(c.AddressLine2) ? " ซอย" : ""),
                                  Soi = c.AddressLine2,
                                  RoadLabel = (!string.IsNullOrWhiteSpace(c.AddressLine3) ? " ถนน" : ""),
                                  Road = c.AddressLine3,
                                  AddressAreaDetail = c.AddressLine4 + " " + c.AddressLine5 + " " + c.AddressLine6 + " " + c.AddressLine7,
                                  PostalCode = c.PostalCode,
                              }).FirstOrDefault();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryAccountAddress").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found address data " + result.AddressDisplay));
                }

                SaveServiceResponseLog(res.SerializeObject());
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getAccountAddress").Add("Error Message", ex.Message).ToFailLogString());
            }

            return result;
        }
        #endregion
    }

}

