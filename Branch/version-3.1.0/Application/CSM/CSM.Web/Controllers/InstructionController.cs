using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using CSM.Service.CBSCustomerService;
using CSM.Service.Messages.Common;
using CSM.Service.Messages.Customer;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class InstructionController : BaseController
    {
        private ICommonFacade _commonFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExistingProductController));

        [CheckUserRole(ScreenCode.ViewCustomerInstruction)]

        public ActionResult List(string encryptedString) {
            int? customerId = encryptedString.ToCustomerId();
            decimal? customerNumber = encryptedString.ToCustomerNumber();
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Instruction").Add("CustomerId", customerId).Add("CustomerNumber", customerNumber).ToInputLogString());

            try {
                if (customerId == 0 && customerNumber == 0)
                {
                    return RedirectToAction("Search", "Customer");
                }

                ViewBag.ErrorMessage = "";
                _commonFacade = new CommonFacade();
                InstructionViewModel InstructionVM = new InstructionViewModel();

                InstructionVM.SearchFilter = new InstructionSearchFilter
                {
                    CustomerId = customerId.Value,
                    CustomerNumber = customerNumber.Value,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                };

                if (TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()] != null)
                {
                    InstructionVM.CustomerInfo = (CustomerInfoViewModel)TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()]; //this.MappingCustomerInfoView(customerId.Value);
                    TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()] = InstructionVM.CustomerInfo;
                }

                if (customerNumber.Value > 0)
                {
                    WebServicePagingEntity vPaging = new WebServicePagingEntity
                    {
                        PageNumber = 1,
                        PageSize = CommonFacade.GetWSMaxReturnRows()
                    };

                    CBSInstructionWithBankResponse insList = getCustomerInstructionListWS(customerNumber.ToString(), customerId.Value, vPaging);
                    if (insList.ResponseCode == "CBS-I-1000" || insList.ResponseCode == "CBS-M-2001")
                    {
                        InstructionVM.InstructionList = insList.InstructionList;
                        if (InstructionVM.InstructionList != null)
                        {
                            InstructionVM.SearchFilter.PageNo = 1;
                            InstructionVM.SearchFilter.PageSize = _commonFacade.GetPageSizeStart();
                            InstructionVM.SearchFilter.SortField = "InstructionVM.SearchFilter";
                            InstructionVM.SearchFilter.SortOrder = "asc";
                            InstructionVM.SearchFilter.TotalRecords = InstructionVM.InstructionList.Count<InstructionEntity>();

                            ViewBag.PageSize = InstructionVM.SearchFilter.PageSize;
                            ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                            ViewBag.Message = string.Empty;

                            int startPageIndex = (InstructionVM.SearchFilter.PageNo - 1) * InstructionVM.SearchFilter.PageSize;
                            if (startPageIndex >= InstructionVM.SearchFilter.TotalRecords)
                            {
                                startPageIndex = 0;
                                InstructionVM.SearchFilter.PageNo = 1;
                            }
                        }
                    }
                    else
                    {
                        InstructionVM.InstructionList = new List<InstructionEntity>();
                        ViewBag.ErrorMessage = insList.ResponseCode + " : " + insList.ResponseMessage;
                    }
                }
                else {
                    InstructionVM.InstructionList = new List<InstructionEntity>();
                }

                return View(InstructionVM);
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Instruction").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        #region "Call Web Service Function"
        InquiryInstructionWithBankResponse getCustomerInstructionWS(string CustomerNumber, WebServicePagingEntity vPaging)
        {
            CBSCustomerService ws = new CBSCustomerService();
            //ws.Proxy = getWebProxy();
            InquiryInstructionWithBankResponse res = new InquiryInstructionWithBankResponse();
            IEnumerable<InstructionEntity> result = new List<InstructionEntity>();
            try
            {
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryInstructionWithBank);

                InquiryInstructionWithBank req = new InquiryInstructionWithBank();
                req.Header = new InquiryInstructionWithBankHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryInstructionWithBank").Add("ReferenceNo", req.Header.ReferenceNo).Add("CustomerNumber", CustomerNumber));

                req.InquiryInstructionWithBankRequest = new InquiryInstructionWithBankInquiryInstructionWithBankRequest();
                req.InquiryInstructionWithBankRequest.CustomerNumber = CustomerNumber;
                //req.InquiryInstructionWithBankRequest.CustomerNumber = "3890000000002";

                req.Paging = new InquiryInstructionWithBankPaging();
                req.Paging.PageNumber = vPaging.PageNumber;
                req.Paging.PageSize = vPaging.PageSize;
                SaveServiceRequestLog(req.SerializeObject());

                res = ws.InquiryInstructionWithBank(req);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getCustomerInstruction").Add("Error Message", ex.Message).ToFailLogString());
                res = null;
            }
            SaveServiceResponseLog(res.SerializeObject());
            return res;
        }

        CBSInstructionWithBankResponse getCustomerInstructionListWS(string CustomerNumber, int CustomerId, WebServicePagingEntity vPaging)
        {
            CBSInstructionWithBankResponse result = new CBSInstructionWithBankResponse();
            try
            {
                InquiryInstructionWithBankResponse res = getCustomerInstructionWS(CustomerNumber, vPaging);
                if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000" || res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001")
                {
                    result.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    result.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                    if (res.Header.InstructionList != null)
                    {
                        _commonFacade = new CommonFacade();

                        result.InstructionList = new List<InstructionEntity>();
                        result.InstructionList = (from c in res.Header.InstructionList
                                                  select new InstructionEntity
                                                  {
                                                      CustomerNumber = Convert.ToDecimal(c.CustomerNumber),
                                                      CustomerId = CustomerId,
                                                      Bank = _commonFacade.GetBankByBankNo(c.BankNumber.ToString()),
                                                      AccountNumber = c.AccountNumber,
                                                      CustomerName = c.CustomerName,
                                                      InstructionDescription = c.InstructionDescription,
                                                      EffectiveDate = c.EffectiveDate.ConvertTimeZoneFromUtc(),
                                                      SubscriptionStatus = c.SubscriptionStatus,
                                                  }).AsQueryable();

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryInstructionWithBank").Add("ReferenceNo:", res.Header.ReferenceNo).SetSuffixMsg("Found data " + result.InstructionList.Count() + "Row(s)"));
                    }
                    else {
                        result.InstructionList = new List<InstructionEntity>();
                    }
                }
                else
                {
                    result.InstructionList = new List<InstructionEntity>();
                    result.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    result.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                } 
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryInstructionWithBank").Add("Error Message", ex.Message).ToFailLogString());
                result.ResponseCode = "Exception ";
                result.ResponseMessage = ex.Message;
            }
            return result;
        }

        
        public IEnumerable<AccountEntity> GetCustomerInstructionList(string CustomerNumber, WebServicePagingEntity vPaging)
        {
            IEnumerable<AccountEntity> result = new List<AccountEntity>();
            try
            {
                InquiryInstructionWithBankResponse res = getCustomerInstructionWS(CustomerNumber, vPaging);
                if (res.Header.InstructionList != null)
                {
                    result = (from c in res.Header.InstructionList
                              select new AccountEntity
                              {
                                  CustomerNumber = Convert.ToDecimal(c.CustomerNumber),
                                  CustomerId = 0,
                                  //BankNumber = c.BankNumber,
                                  AccountNo = c.AccountNumber,
                                  CustomerName = c.CustomerName,
                                  ProductGroup = "",
                                  Product = c.InstructionDescription,
                                  EffectiveDate = c.EffectiveDate.ConvertTimeZoneFromUtc(),
                                  AccountStatus = c.SubscriptionStatus
                              }).AsQueryable();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("GetCustomerInstructionList").Add("Error Message", ex.Message).ToFailLogString());
            }
            return result;
        }
        #endregion
    }
}
