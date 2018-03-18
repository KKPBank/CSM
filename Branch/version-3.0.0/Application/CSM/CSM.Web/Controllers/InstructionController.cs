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
                        PageNumber = InstructionVM.SearchFilter.PageNo,
                        PageSize = InstructionVM.SearchFilter.PageSize
                    };
                    
                    InstructionVM.InstructionList = getCustomerInstructionListWS(customerNumber.ToString(), customerId.Value, vPaging); //GetInstructionList(InstructionVM.SearchFilter);

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
        IEnumerable<InstructionEntity> getCustomerInstructionListWS(string CustomerNumber, int CustomerId, WebServicePagingEntity vPaging) {
            CBSCustomerService ws = new CBSCustomerService();
            //ws.Proxy = getWebProxy();

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

                InquiryInstructionWithBankResponse res = new InquiryInstructionWithBankResponse();
                res = ws.InquiryInstructionWithBank(req);

                if (res.Header.InstructionList != null)
                {
                    result = (from c in res.Header.InstructionList
                              select new InstructionEntity
                              {
                                  CustomerNumber = Convert.ToDecimal(c.CustomerNumber),
                                  CustomerId = CustomerId,
                                  BankNumber = c.BankNumber,
                                  AccountNumber = c.AccountNumber,
                                  CustomerName = c.CustomerName,
                                  InstructionDescription = c.InstructionDescription,
                                  EffectiveDate = c.EffectiveDate,
                                  SubscriptionStatus = c.SubscriptionStatus
                              }).AsQueryable();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getCustomerInstruction").Add("Error Message", ex.Message).ToFailLogString());
            }
            return result;
        }
        #endregion
    }
}
