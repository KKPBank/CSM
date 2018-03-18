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
            IEnumerable<AccountEntity> res = null;
            try
            {
                if (ModelState.IsValid)
                {
                    if (searchFilter.CustomerNumber > 0)
                    {
                        WebServicePagingEntity vPaging = new WebServicePagingEntity();
                        vPaging.PageNumber = searchFilter.PageNo;
                        vPaging.PageSize = searchFilter.PageSize;

                        IEnumerable<AccountEntity> custSumList = InquiryCustomerAccountSummaryWS(searchFilter.CustomerNumber.Value, vPaging);
                        IEnumerable<AccountEntity> svBaknList = InquiryServiceWithBankWS(searchFilter.CustomerNumber.Value, vPaging);

                        res = custSumList.Union(svBaknList);

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("ProductServiceList").Add("InquiryCustomerAccountSummary", custSumList.Count()).Add("InquiryServiceWithBank", svBaknList.Count()).ToSuccessLogString());
                    }
                    else
                    {
                        using (var facade = new CustomerFacade())
                        {
                            res = facade.GetAccountList(searchFilter);
                        }
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("ProductServiceList").ToSuccessLogString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("ExistingProductList").Add("Error Message", ex.Message).ToFailLogString());
                throw;
            }
            return res;
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
                        case "ProductGroup":
                            productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                                ? productVM.AccountList.OrderBy(q => q.ProductGroup)
                                : productVM.AccountList.OrderByDescending(q => q.ProductGroup);
                            break;
                        case "LastNameThai":
                            productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                                ? productVM.AccountList.OrderBy(q => q.Product)
                                : productVM.AccountList.OrderByDescending(q => q.Product);
                            break;
                        case "Grade":
                            productVM.AccountList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                                ? productVM.AccountList.OrderBy(q => q.Grade)
                                : productVM.AccountList.OrderByDescending(q => q.Grade);
                            break;
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
                    CASAAccountDetailViewModel CASAModel = InquiryCASAAccountDetail(searchFilter.AccountNumber, searchFilter.CustomerNumber.ToString(), custInfo.CustomerTypeDisplay, custInfo.BirthDateDisplay, custInfo.CardNo, custInfo.CountryOfCitizenship, custInfo.FirstName, custInfo.LastName, searchFilter);
                    //CASAAccountDetailViewModel CASAModel = InquiryCASAAccountDetail("1000000151", searchFilter.CustomerNumber.ToString(), custInfo.CustomerTypeDisplay, custInfo.BirthDateDisplay, custInfo.CardNo, custInfo.CountryOfCitizenship, custInfo.FirstName, custInfo.LastName, searchFilter);

                    if (CASAModel != null)
                    {
                        searchFilter.PageNo = 1;
                        searchFilter.PageSize = _commonFacade.GetPageSizeStart();
                        searchFilter.SortField = "TransactionDate";
                        searchFilter.SortOrder = "desc";
                        searchFilter.TotalRecords = (CASAModel.TransactionList == null ? 0 : CASAModel.TransactionList.ToList().Count);

                        int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
                        if (startPageIndex >= searchFilter.TotalRecords)
                        {
                            startPageIndex = 0;
                            searchFilter.PageNo = 1;
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

        public IEnumerable<AccountEntity> InquiryCustomerAccountSummaryWS(decimal CustomerNumber,  WebServicePagingEntity vPaging) {
            CBSAccountService ws = new CBSAccountService();
            //ws.Proxy = getWebProxy();
            IEnumerable<AccountEntity> result = new List<AccountEntity>();
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
                req.Paging.PageNumber = vPaging.PageNumber;
                req.Paging.PageSize = vPaging.PageSize;

                InquiryCustomerAccountSummaryResponse res = new InquiryCustomerAccountSummaryResponse();
                res = ws.InquiryCustomerAccountSummary(req);

                if (res.Header.AccountList != null)
                {
                    result = (from c in res.Header.AccountList
                              select new AccountEntity()
                              {
                                  CustomerId = 0,
                                  CustomerNumber = CustomerNumber,
                                  AccountNo = c.AccountNumber,
                                  Product = c.ProductCode,
                                  Grade = c.AccountStatus,
                                  BundleCode = c.BundleCode,
                                  AccountType = c.AccountType,
                                  AccountStatus = (c.AccountStatus == "1" ? "A" : "I"),
                                  CustomerName = c.AccountName,
                                  InquiryServiceName = Constants.ServiceName.InquiryCustomerAccountSummary
                              }).AsQueryable();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerAccountSummaryWS").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found data " + result.Count() + "Row(s)"));
                }
                else {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerAccountSummaryWS").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerAccountSummaryWS").Add("Error Message", ex.Message).ToFailLogString());
            }
            return result;
        }

        IEnumerable<AccountEntity> InquiryServiceWithBankWS(decimal CustomerNumber,  WebServicePagingEntity vPaging)
        {
            CBSCustomerService ws = new CBSCustomerService();
            //ws.Proxy = getWebProxy();
            IEnumerable<AccountEntity> result = new List<AccountEntity>();
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
                req.Paging.PageNumber = vPaging.PageNumber;
                req.Paging.PageSize = vPaging.PageSize;
                
                InquiryServiceWithBankResponse res = new InquiryServiceWithBankResponse();
                res = ws.InquiryServiceWithBank(req);
                if (res.Header.ServiceList != null)
                {
                    result = (from c in res.Header.ServiceList
                              select new AccountEntity
                              {
                                  CustomerId = 0,
                                  CustomerNumber = CustomerNumber,
                                  AccountNo = c.AccountNumber,
                                  Product = c.ProductCode,
                                  Grade = c.AccountStatus,
                                  EffectiveDate = c.EffectiveDate,
                                  ExpiryDate = c.ValidTill,
                                  BundleCode = "",
                                  CustomerEmail = c.CustomerEMail,
                                  CustomerMobileNo = c.CustomerMobileNumber,
                                  AccountType = c.AccountType,
                                  AccountStatus = (c.AccountStatus == "1" ? "A" : "I"),
                                  InquiryServiceName = Constants.ServiceName.InquiryServiceWithBank
                              });
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryServiceWithBank").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found data " + result.Count<AccountEntity>() + " Row(s)"));
                }
                else {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryServiceWithBank").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getInquiryServiceWithBankWS").Add("Error Message", ex.Message).ToFailLogString());
            }

            return result;
        }
        #endregion

        #region "Get Loan Account Detail from web service"
        LoanAccountDetailViewModel getLoanAccountOverview(string AccountNumber, string CustomerNumber, string CustomerType, string BirthDate, string CardNo) {
            LoanAccountDetailViewModel result = null;
            try {
                //result = new LoanAccountDetailViewModel()
                //{
                //    CustomerNumber = "Simulate Data",
                //    BirthDate = new DateTime(2000, 1, 1),
                //    IDNumber = "IDNumber",
                //    PhoneNumber = "PhoneNumber",
                //    Address = "Address",
                //    BankNumber = "BankNumber",
                //    AccountNumber = AccountNumber,
                //    AccountName = "AccountName",
                //    ApprovalDate = DateTime.Now,
                //    DrawingLimit = (decimal)20000000.00,
                //    PaymentDayOfMonth = 1,
                //    FirstPaymentDate = DateTime.Now,
                //    LoanInterestRate = "3.75",
                //    Term = "Term",
                //    PaymentAmount = "50,000.00",
                //    MaturityDate = DateTime.Now.AddYears(1),
                //    StatusAccountCode = "StatusAccountCode",
                //    StatusDescription = "StatusDescription",
                //    Reference1 = "Reference1",
                //    Reference2 = "Reference2",
                //    AFTLoan = "AFTLoan",
                //    InsuranceFlag = "InsuranceFlag"
                //};

                InquiryLoanAccountOverviewResponse res = InquiryLoanAccountOverviewWS(AccountNumber, CustomerNumber);
                if (res.Header.LoanAccountInfo != null)
                {
                    result = (from c in res.Header.LoanAccountInfo
                              select new LoanAccountDetailViewModel()
                              {
                                  CustomerNumber = CustomerNumber,
                                  BirthDateDisplay = BirthDate,
                                  IDNumber = CardNo,
                                  PhoneNumber = c.PhoneNumber,
                                  Address = c.Address,
                                  BankNumber = c.BankNumber,
                                  AccountNumber = c.AccountNumber,
                                  AccountName = c.AccountName,
                                  ApprovalDate = c.ApprovalDate,
                                  DrawingLimit = c.DrawingLimit,
                                  PaymentDayOfMonth = c.PaymentDayOfMonth,
                                  FirstPaymentDate = c.FirstPaymentDate,
                                  LoanInterestRate = c.LoanInterestRate.FormatDecimal(),
                                  Term = c.Term.ToString(),
                                  PaymentAmount = c.PaymentAmount.FormatDecimal(),
                                  MaturityDate = c.MaturityDate,
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

            return result;
        }

        public InquiryLoanAccountOverviewResponse InquiryLoanAccountOverviewWS(string AccountNumber, string CustomerNumber)
        {
            CBSLoanService ws = new CBSLoanService();
            //ws.Proxy = getWebProxy();
            InquiryLoanAccountOverviewResponse result = null;
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

                result = new InquiryLoanAccountOverviewResponse();
                result = ws.InquiryLoanAccountOverview(req);
                if (result.Header.LoanAccountInfo != null)
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryLoanAccountOverviewWS").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found loan account data"));
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryLoanAccountOverviewWS").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data foud"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryLoanAccountOverviewWS").Add("Error Message", ex.Message).ToFailLogString());
            }
            return result;
        }
        #endregion

        #region "Get CASA Account Detail from web service"
        CASAAccountDetailViewModel InquiryCASAAccountDetail(string AccountNumber, string CustomerNumber, string CustomerType, string BirthDate, string CardNo, string CountryOfCitizenship, string FirstName, string LastName, ViewProductDetailSearchFilter searchFilter)
        {
            CASAAccountDetailViewModel result = null;
            InquiryCASAAccountDetailResponse res = InquiryCASAAccountDetailResponseWS(AccountNumber, CustomerNumber);
            if (res.Header.AccountInfo != null)
            {
                WebServicePagingEntity vPaging = new WebServicePagingEntity()
                {
                    PageNumber = searchFilter.PageNo,
                    PageSize = searchFilter.PageSize,
                };

                InquiryCASAAccountDetailResponseHeaderAccountInfoAccount acc = res.Header.AccountInfo.Account;
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
                    DateOpened = acc.OpenedDate, //ใน Webservice ไม่มีฟิลด์ DateOpen
                    AccountStatus = _commonFacade.GetConfigCasaAccountStatus(acc.Status).CasaAccountStatusName,
                    Officer = acc.Officer, 
                    PassbookFlag = acc.PassbookFlag,
                    SignatureConditionCode = acc.SignatureConditionCode,
                    SignatureConditionDescription = acc.SignatureConditionDescription,
                    BranchNumber = acc.BranchNumber,
                    TransactionList = InquiryCASAStatementHistoryList(AccountNumber, acc.AccountType, searchFilter)
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

            return result;
        }

        public InquiryCASAAccountDetailResponse InquiryCASAAccountDetailResponseWS(string AccountNumber, string CustomerNumber)
        {
            CBSAccountService ws = new CBSAccountService();
            //ws.Proxy = getWebProxy();
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

                res = ws.InquiryCASAAccountDetail(req);
                if (res.Header.AccountInfo != null)
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAAccountDetail").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found CASA Account Detail Data"));
                }
                else {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAAccountDetail").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Data not found"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getCASAAccountDetailWS").Add("Error Message", ex.Message).ToFailLogString());
            }

            return res;

        }

        IEnumerable<CASATransactionEntity> InquiryCASAStatementHistoryList(string AccountNumber, string AccountType,  ViewProductDetailSearchFilter searchFilter) {
            CBSAccountService ws = new CBSAccountService();
            //ws.Proxy = getWebProxy();
            IEnumerable<CASATransactionEntity> result = null;

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
                req.Paging.PageNumber = searchFilter.PageNo;
                req.Paging.PageSize = _commonFacade.GetPageSizeStart();


                InquiryCASAStatementHistoryResponse res = new InquiryCASAStatementHistoryResponse();
                res = ws.InquiryCASAStatementHistory(req);
                if (res.Header.StatementList != null)
                {
                    result = (from c in res.Header.StatementList
                              select new CASATransactionEntity()
                              {
                                  TransactionDate = c.TransactionDate,
                                  PostingTimeStamp = c.PostingTimestamp,  //PostingTimestamp เป็นค่าว่าง  แต่มีฟิลด์ PostingTime นั้นมีข้อมูล แต่ไม่ใช่ฟิลด์ที่ตรงตาม Spec
                                  TransactionDescription = c.TransactionDescription,
                                  ChequeNumber = c.ChequeNumber,   //เลขที่เช็ค ไม่น่าจะเป็น long นะ
                                  DebitCreditCode = c.DebitCreditCode,
                                  TransactionBranchDescription = c.TransactionBranchDescription,
                                  TransactionAmount = c.TransactionAmount.FormatDecimal(),
                              });
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAStatementHistoryList").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found Data " + result.ToList<CASATransactionEntity>().Count() + "Row(s)"));
                }
                else {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAStatementHistoryList").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCASAStatementHistoryList").Add("Error Message", ex.Message).ToFailLogString());
            }

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
                    PageNumber = searchFilter.PageNo,
                    PageSize = searchFilter.PageSize,
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
            //ws.Proxy = getWebProxy();
            IEnumerable<TDAccountEntity> result = new List<TDAccountEntity>();

            try {
                //result = new List<TDAccountEntity>() {
                //    new TDAccountEntity() {
                //              CustomerNumber = "Simulate Data",
                //              TDGroupNumber = "3500065479003",
                //              TDPlacementNumber = "0035000654790030001",
                //              TDAccountTypeDescription = "TDAccountTypeDescription",
                //              RenewalCounter = 0,
                //              ProductTerm = 1,
                //              ProductTermCode = "M",
                //              EffectiveDate = DateTime.Now,
                //              MaturityDate = DateTime.Now.AddYears(1),
                //              InterestRatePercentage = (decimal)1.125,
                //              OriginalAmount = 10000000,
                //              AccountStatus = 1,
                //              AccountStatusDescription = "AccountStatusDescription",
                //              BranchNumber = 1,
                //              ReceiptSerialNumber = "ReceiptSerialNumber"
                //    },

                //    new TDAccountEntity() {
                //              CustomerNumber = "20000111000",
                //              TDGroupNumber = "4585065479003",
                //              TDPlacementNumber = "0055000654790030003",
                //              TDAccountTypeDescription = "TDAccountTypeDescription",
                //              RenewalCounter = 0,
                //              ProductTerm = 1,
                //              ProductTermCode = "M",
                //              EffectiveDate = DateTime.Now,
                //              MaturityDate = DateTime.Now.AddYears(1),
                //              InterestRatePercentage = (decimal)1.125,
                //              OriginalAmount = 10000000,
                //              AccountStatus = 1,
                //              AccountStatusDescription = "AccountStatusDescription",
                //              BranchNumber = 1,
                //              ReceiptSerialNumber = "ReceiptSerialNumber"
                //    },
                //};

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

                InquiryTDAccountByGroupResponse res = new InquiryTDAccountByGroupResponse();
                res = ws.InquiryTDAccountByGroup(req);
                if (res.Header.PlacementList != null)
                {
                    result = (from c in res.Header.PlacementList
                              select new TDAccountEntity()
                              {
                                  CustomerNumber = c.CustomerNumber,
                                  TDGroupNumber = c.TDGroupNumber,
                                  TDPlacementNumber = c.TDPlacementNumber,
                                  TDAccountTypeDescription = c.AccountTypeDescription,
                                  RenewalCounter = c.RenewalCounter,
                                  ProductTerm = c.Term,
                                  ProductTermCode = c.TermCode,
                                  EffectiveDate = c.EffectiveDate,
                                  MaturityDate = c.MaturityDate,
                                  InterestRatePercentage = c.InterestRatePercentage,
                                  OriginalAmount = c.OriginalAmount.ToString("#,##0.00"),
                                  AccountStatus = c.AccountStatus,
                                  AccountStatusDescription = c.AccountStatusDescription,
                                  BranchNumber = c.BranchNumber,
                                  ReceiptSerialNumber = c.ReceiptSerialNumber,
                              });

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountList").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found data " +  result.Count<TDAccountEntity>() + " Row(s)"));
                }
                else {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountList").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountList").Add("Error Message", ex.Message).ToFailLogString());
            }

            return result;
        }

        public InquiryTDAccountDetailResponse InquiryTDAccountDetailWS(string AccountNumber) {
            CBSTermDepositService ws = new CBSTermDepositService();
            //ws.Proxy = getWebProxy();
            InquiryTDAccountDetailResponse res = new InquiryTDAccountDetailResponse();
            try
            {
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryTDAccountDetail);

                InquiryTDAccountDetail req = new InquiryTDAccountDetail();
                req.Header = new InquiryTDAccountDetailHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };
                req.InquiryTDAccountDetailRequest = new InquiryTDAccountDetailInquiryTDAccountDetailRequest();
                req.InquiryTDAccountDetailRequest.TDPlacementNumber = AccountNumber;
                //req.InquiryTDAccountDetailRequest.AccountType = Constants.AccountType.TermDepositAccount;
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountDetail").Add("ReferenceNo:", req.Header.ReferenceNo).Add("AccountNumber", AccountNumber));

                res = ws.InquiryTDAccountDetail(req);
                if (res.Header.PlacementInfo != null)
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountDetail").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found TD Account Detail Data"));
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountDetail").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Data not found"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryTDAccountDetailWS").Add("Error Message", ex.Message).ToFailLogString());
            }

            return res;
        }

        InquiryAccountAddressEntity getAccountAddress(string AccountNumber) {
            CBSAccountService ws = new CBSAccountService();
            //ws.Proxy = getWebProxy();
            InquiryAccountAddressEntity result = new InquiryAccountAddressEntity();

            try {
                ////Test Function
                //result = new InquiryAccountAddressEntity()
                //{
                //    AccountNumber = "Simulate Data",
                //    AccountType = "AccountType",
                //    AccountTypeDescription = "AccountTypeDescription",
                //    Seq = 1,
                //    PhoneNo = "PhoneNo",
                //    EMail = "Email",
                //    AddressFormat = "AddressFormat",
                //    AddressLine1 = "123",
                //    AddressLine2 = " ถ.พหลโยธิน 48 ",
                //    AddressLine3 = " แขวงอนุสาวรีย์",
                //    AddressLine4 = " เขตบางเขน",
                //    AddressLine5 = " กทม.",
                //    AddressLine6 = "",
                //    AddressLine7 = "",
                //    PostalCode = "10220",
                //    StateCode = "TH",
                //};

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

                InquiryAccountAddressResponse res = new InquiryAccountAddressResponse();
                res = ws.InquiryAccountAddress(req);

                if (res.Header.AddressInfo != null)
                {
                    result = (from c in res.Header.AddressInfo
                              select new InquiryAccountAddressEntity()
                              {
                                  AccountNumber = c.AccountNumber,
                                  AccountType = c.AccountType,
                                  AccountTypeDescription = c.AccountTypeDescription,
                                  Seq = (int)c.Seq,
                                  PhoneNo = c.Mobile,
                                  EMail = c.Email,
                                  AddressFormat = c.AddressFormat,
                                  AddressLine1 = c.AddressLine1,
                                  AddressLine2 = c.AddressLine2,
                                  AddressLine3 = c.AddressLine3,
                                  AddressLine4 = c.AddressLine4,
                                  AddressLine5 = c.AddressLine5,
                                  AddressLine6 = c.AddressLine6,
                                  AddressLine7 = c.AddressLine7,
                                  PostalCode = c.PostalCode,
                                  StateCode = c.StateCode,
                              }).FirstOrDefault();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryAccountAddress").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found address data " + result.AddressDisplay));
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryAccountAddress").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                }
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

