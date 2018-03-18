using System;
using System.Web;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using CSM.Common.Resources;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CSM.Entity.Common;
using System.Text;
using System.Globalization;
using CSM.Common.Securities;
using CSM.Service.CBSCustomerService;
using CSM.Service.Messages.Common;
using CSM.Service.HPContractService;
using CSM.Service.Messages.Customer;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class CustomerController : BaseController
    {
        private ICommonFacade _commonFacade;
        private ICustomerFacade _customerFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CustomerController));

        #region "Customer"

        [CheckUserRole(ScreenCode.SearchCustomer)]
        public ActionResult Search(string skip = "0")
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Customer").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                CustomerViewModel custVM = new CustomerViewModel();

                custVM.SearchFilter = new CustomerSearchFilter
                {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    ExactFirstName = false,
                    ExactLastName = false,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "CustomerId",
                    SortOrder = "ASC"
                };

                var defSearch = _commonFacade.GetShowhidePanelByUserId(this.UserInfo, Constants.Page.CustomerPage);

                if (defSearch != null)
                {
                    custVM.IsSelected = defSearch.IsSelectd ? "1" : "0";
                }
                else
                {
                    custVM.IsSelected = "0";
                }

                var customerProductList = _commonFacade.GetCustomerProductSelectList("");
                custVM.CustomerProductList = new SelectList((IEnumerable)customerProductList, "Key", "Value", string.Empty);

                var customerTypeList = _commonFacade.GetCustomerTypeSelectList("");
                custVM.CustomerTypeList = new SelectList((IEnumerable)customerTypeList, "Key", "Value", string.Empty);

                if (!string.IsNullOrWhiteSpace(this.CallId) && !skip.Equals("1"))
                {
                    _customerFacade = new CustomerFacade();
                    CallInfoEntity callInfo = _customerFacade.GetCallInfoByCallId(this.CallId);

                    //var lstCustomer = _customerFacade.GetCustomerIdWithCallId(callInfo.PhoneNo);

                    custVM.SearchFilter.PhoneNo = callInfo.PhoneNo;
                    custVM.SearchFilter.ExactPhoneNo = false;
                    custVM = SearchCustomerList(custVM.SearchFilter);
                    var recordFound = (custVM == null) ? 0 : custVM.CustomerList.Count();

                    // AuditLog
                    var logDetail = new StringBuilder("");
                    logDetail.AppendFormat("CallId = {0}\n", callInfo.CallId);
                    logDetail.AppendFormat("PhoneNo = {0}\n", callInfo.PhoneNo);
                    logDetail.AppendFormat("CardNo = {0}\n", callInfo.CardNo);
                    logDetail.AppendFormat("CallType = {0}\n", callInfo.CallType);
                    logDetail.AppendFormat("TotalRecords = {0}\n", recordFound);
                    var auditLog = new AuditLogEntity();
                    auditLog.Module = Constants.Module.Customer;
                    auditLog.Action = Constants.AuditAction.Search;
                    auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                    auditLog.Status = LogStatus.Success;
                    auditLog.Detail = logDetail.ToString();
                    auditLog.CreateUserId = this.UserInfo.UserId;
                    AppLog.AuditLog(auditLog);

                    if (recordFound == 1)
                    {
                        var cust = custVM.CustomerList.First();
                        string cbsCardTypeCode = "";
                        string cbsCardTypeName = "";
                        if (cust.CbsCardType != null)
                        {
                            cbsCardTypeCode = cust.CbsCardType.CardTypeCode;
                            cbsCardTypeName = cust.CbsCardType.CardTypeName;
                        }

                        return InitCustomerNote(cust.CustomerId.Value, cust.CustomerNumber.Value, cust.CustomerType.Value.ToString(), cust.BirthDateDisplay, cust.CardNo, cbsCardTypeCode, cbsCardTypeName, cbsCardTypeCode, cust.CountryOfCitizenship, 
                            cust.TitleThai.TitleName, cust.FirstNameThai, cust.LastNameThai,cust.TitleEnglish.TitleName,cust.FirstName,cust.LastName, cust.EmployeeCode);
                        //if (customerId != null)
                        //{
                        //    return InitCustomerNote(customerId.Value, 0, custVM);
                        //}
                    }
                    else
                    {
                        //custVM.SearchFilter.PhoneNo = callInfo.PhoneNo;
                        //custVM.SearchFilter.ExactPhoneNo = true;
                        //custVM = SearchCustomerList(custVM.SearchFilter);
                        custVM.CustomerProductList = new SelectList((IEnumerable)customerProductList, "Key", "Value", string.Empty);
                        custVM.CustomerTypeList = new SelectList((IEnumerable)customerTypeList, "Key", "Value", string.Empty);
                    }
                }

                ViewBag.PageSize = custVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View(custVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Customer").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ShowhidePanel(int expandValue)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("ShowhidePanel").Add("expand", expandValue).ToInputLogString());
            try
            {
                _commonFacade = new CommonFacade();
                int userId = this.UserInfo.UserId;
                _commonFacade.SaveShowhidePanel(expandValue, userId, Constants.Page.CustomerPage);

                return Json(new
                {
                    Valid = true
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("ShowhidePanel").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }

        private CustomerViewModel SearchCustomerList(CustomerSearchFilter searchFilter)
        {
            _commonFacade = new CommonFacade();
            _customerFacade = new CustomerFacade();
            CustomerViewModel custVM = new CustomerViewModel();
            custVM.SearchFilter = searchFilter;

            IEnumerable<CustomerEntity> wsList = new List<CustomerEntity>(); //ชื่อนามสกุลลูกค้า
            IEnumerable<CustomerEntity> hpList = new List<CustomerEntity>(); //เลขที่สัญญา (HP)
            IEnumerable<CustomerEntity> insList = new List<CustomerEntity>(); //เลขที่สัญญา (Insurance)
            IEnumerable<CustomerEntity> cbList = new List<CustomerEntity>(); //เลขที่สัญญา (Core Bank)
            IEnumerable<CustomerEntity> dbList = new List<CustomerEntity>(); //DB

            WebServicePagingEntity vPaging = new WebServicePagingEntity();
            vPaging.PageNumber = 1;
            vPaging.PageSize = CommonFacade.GetWSMaxReturnRows();
            custVM.InquiryCBSErrorMessage = "";

            if (custVM.SearchFilter.CustomerType == null || custVM.SearchFilter.CustomerType == Constants.CustomerType.Customer)
            {
                //ถ้ามี HP
                if (custVM.SearchFilter.Product == Constants.CustomerProduct.HP || searchFilter.Registration != null || searchFilter.CustomerDeptFirstName != null || searchFilter.CustomerDeptLastName != null)
                {
                    hpList = InquiryHPCustomerContractListWS(searchFilter.AccountNo, "", searchFilter.CustomerDeptFirstName, searchFilter.CustomerDeptLastName, "", searchFilter.CardNo, searchFilter.Registration, vPaging);
                    if (hpList.ToList().Count() > 0)
                    {
                        //ถ้า HP มีข้อมูลมากกว่า 0 รายการให้ Return เลย
                        custVM.CustomerHPCount = hpList.ToList().Count();
                        custVM.CustomerList = hpList;
                        custVM.SearchFilter.TotalRecords = custVM.CustomerHPCount;
                        return custVM;
                    }
                }
                else
                {
                    if (custVM.SearchFilter.Product == Constants.CustomerProduct.Insurance)
                    {
                        insList = getCustomerByInsuranceAccountWS(searchFilter.AccountNo, vPaging);
                    }
                    else if (custVM.SearchFilter.FirstName != null || custVM.SearchFilter.LastName != null || custVM.SearchFilter.PhoneNo != null || custVM.SearchFilter.CardNo != null)
                    {
                        //ถ้าใช้เงื่อนไขจาก Core Bank ให้เอาจาก Account กับ Customer มา Intersect
                        CBSCustomerByInformationResponse cstInfoList = getCustomerByInformationWS(custVM.SearchFilter, vPaging);
                        if (cstInfoList.ResponseCode == "CBS-I-1000" || cstInfoList.ResponseCode == "CBS-M-2001")
                        {
                            wsList = (cstInfoList.CustomerList == null ? new List<CustomerEntity>() : cstInfoList.CustomerList);
                            if (custVM.SearchFilter.Product == Constants.CustomerProduct.Loan || custVM.SearchFilter.Product == Constants.CustomerProduct.Funding)
                            {
                                CBSCustomerByAccountResponse cstAccList = getCustomerByLoanFundingAccountWS(searchFilter.AccountNo, vPaging, custVM.SearchFilter.Product);
                                if (cstAccList.ResponseCode == "CBS-I-1000" || cstAccList.ResponseCode == "CBS-M-2001")
                                {
                                    cbList = cstAccList.CustomerList;
                                }
                                else
                                {
                                    custVM.InquiryCBSErrorMessage = cstAccList.ResponseCode + " : " + cstAccList.ResponseMessage + " (InquiryCustomerByAccount)"; 
                                }
                            }

                            if (wsList.Count() != 0 && cbList.Count() != 0)
                            {
                                var cmp = new CustomerComparer();
                                wsList = wsList.Intersect(cbList, cmp).ToList();
                            }
                            else if (wsList.Count() == 0 && cbList.Count() != 0)
                                wsList = cbList;
                        }
                        else {
                            custVM.InquiryCBSErrorMessage = cstInfoList.ResponseCode + " : " + cstInfoList.ResponseMessage + " (InquiryCustomerByInfomation)";
                        }
                    }
                    else {
                        if (custVM.SearchFilter.Product == Constants.CustomerProduct.Loan || custVM.SearchFilter.Product == Constants.CustomerProduct.Funding)
                        {
                            CBSCustomerByAccountResponse cstAccList = getCustomerByLoanFundingAccountWS(searchFilter.AccountNo, vPaging, custVM.SearchFilter.Product);
                            if (cstAccList.ResponseCode == "CBS-I-1000" || cstAccList.ResponseCode == "CBS-M-2001")
                            {
                                wsList = cstAccList.CustomerList;
                            }
                            else
                            {
                                custVM.InquiryCBSErrorMessage = cstAccList.ResponseCode + " : " + cstAccList.ResponseMessage + " (InquiryCustomerByAccount)";
                            }
                        }
                    }
                }

                dbList = _customerFacade.SearchCustomer(custVM.SearchFilter);
                custVM.CustomerList = hpList.Union(insList).Union(wsList).Union(dbList);
            }
            else
            {
                dbList = _customerFacade.SearchCustomer(custVM.SearchFilter);
                custVM.CustomerList = dbList;
            }
            List<CustomerEntity> lst = custVM.CustomerList?.ToList();
            if (lst != null)
            {
                var phoneType = (new CustomerFacade()).GetPhoneType(code: "02").FirstOrDefault(); //เบอร์มือถือ
                //foreach (var cust in custVM.CustomerList)
                for (int i = 0; i < lst.Count; i++)
                {
                    var cust = lst[i];
                    if ((cust.CustomerNumber ?? 0) > 0)
                    {
                        var contact = CustomerFacade.WSInquiryCustomerContact(cust.CustomerNumber, new WebServicePagingEntity() { PageNumber = 1, PageSize = CommonFacade.GetWSMaxReturnRows() });
                        if (contact != null)
                        {
                            var mobile = contact.Where(x => x.ContactCode == "MBL").Select(x => x.ContactDetail);
                            if (mobile != null)
                            {
                                cust.PhoneList = new List<PhoneEntity>();
                                mobile.ToList().ForEach(x => cust.PhoneList.Add(new PhoneEntity()
                                {
                                    PhoneNo = x.Replace("+66", "0"),
                                    PhoneTypeCode = phoneType.PhoneTypeCode,
                                    PhoneTypeName = phoneType.PhoneTypeName,
                                    PhoneTypeId = phoneType.PhoneTypeId
                                }));
                            }
                        }
                    }
                }
                custVM.CustomerList = lst;
            }
            custVM.SearchFilter.TotalRecords = custVM.CustomerList.ToList().Count;

            #region "Paging and Sorting"
            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            switch (searchFilter.SortField)
            {
                case "CIF_ID":
                    custVM.CustomerList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? custVM.CustomerList.OrderBy(q => q.CIF_ID)
                        : custVM.CustomerList.OrderByDescending(q => q.CIF_ID);
                    break;
                case "FirstNameThai":
                    custVM.CustomerList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? custVM.CustomerList.OrderBy(q => q.FirstNameThai)
                        : custVM.CustomerList.OrderByDescending(q => q.FirstNameThai);
                    break;
                case "LastNameThai":
                    custVM.CustomerList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? custVM.CustomerList.OrderBy(q => q.LastNameThai)
                        : custVM.CustomerList.OrderByDescending(q => q.LastNameThai);
                    break;
                case "CardNo":
                    custVM.CustomerList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? custVM.CustomerList.OrderBy(q => q.CardNo)
                        : custVM.CustomerList.OrderByDescending(q => q.CardNo);
                    break;
                case "IDTypeCode":
                    custVM.CustomerList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? custVM.CustomerList.OrderBy(q => q.IDTypeCode)
                        : custVM.CustomerList.OrderByDescending(q => q.IDTypeCode);
                    break;
                case "CustomerCategory":
                    custVM.CustomerList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? custVM.CustomerList.OrderBy(q => q.CustomerCategory)
                        : custVM.CustomerList.OrderByDescending(q => q.CustomerCategory);
                    break;
                case "CustomerTypeDisplay":
                    custVM.CustomerList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? custVM.CustomerList.OrderBy(q => q.CustomerTypeDisplay)
                        : custVM.CustomerList.OrderByDescending(q => q.CustomerTypeDisplay);
                    break;
                case "AccountTypeDescription":
                    custVM.CustomerList = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? custVM.CustomerList.OrderBy(q => q.Account.AccountTypeDescription)
                        : custVM.CustomerList.OrderByDescending(q => q.Account.AccountTypeDescription);
                    break;
                default:
                    custVM.CustomerList = custVM.CustomerList.OrderByDescending(q => q.FirstNameThai);
                    break;
            }
            custVM.CustomerList = custVM.CustomerList.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<CustomerEntity>();
            #endregion

            return custVM;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.SearchCustomer)]
        public ActionResult CustomerList(CustomerSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Customers").Add("CardNo", searchFilter.CardNo.MaskCardNo())
                .Add("FirstName", searchFilter.FirstName).Add("LastName", searchFilter.LastName).ToInputLogString());

            try
            {
                if (!string.IsNullOrWhiteSpace(searchFilter.FirstName)
                    && searchFilter.FirstName.ExtractString().Length < Constants.MinLenght.SearchTerm)
                {
                    ModelState["FirstName"].Errors.Clear();
                    ModelState["FirstName"].Errors.Add(string.Format(CultureInfo.InvariantCulture, Resource.ValErr_MinLength, Constants.MinLenght.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(searchFilter.LastName) &&
                    searchFilter.LastName.ExtractString().Length < Constants.MinLenght.SearchTerm)
                {
                    ModelState["LastName"].Errors.Clear();
                    ModelState["LastName"].Errors.Add(string.Format(CultureInfo.InvariantCulture, Resource.ValErr_MinLength, Constants.MinLenght.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(searchFilter.CustomerDeptFirstName) &&
                    searchFilter.CustomerDeptFirstName.ExtractString().Length < Constants.MinLenght.SearchTerm)
                {
                    ModelState["CustomerDeptFirstName"].Errors.Clear();
                    ModelState["CustomerDeptFirstName"].Errors.Add(string.Format(CultureInfo.InvariantCulture, Resource.ValErr_MinLength, Constants.MinLenght.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(searchFilter.CustomerDeptLastName) &&
                    searchFilter.CustomerDeptLastName.ExtractString().Length < Constants.MinLenght.SearchTerm)
                {
                    ModelState["CustomerDeptLastName"].Errors.Clear();
                    ModelState["CustomerDeptLastName"].Errors.Add(string.Format(CultureInfo.InvariantCulture, Resource.ValErr_MinLength, Constants.MinLenght.SearchTerm));
                }

                if (ModelState.IsValid)
                {
                    CustomerViewModel custVM = SearchCustomerList(searchFilter);
                    if (custVM.CustomerHPCount > 0)
                    {
                        ViewBag.PageSize = custVM.SearchFilter.PageSize;
                        ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Search HP Customer").ToSuccessLogString());
                        //return PartialView("~/Views/Customer/_CustomerHPList.cshtml", custVM);

                        return Json(new
                        {
                            Valid = true,
                            CustomerHPCount = custVM.CustomerHPCount,
                            ResultView = ConvertViewToString("~/Views/Customer/_CustomerHPList.cshtml", custVM),
                            Error = string.Empty,
                            Errors = string.Empty
                        });
                    }
                    else {
                        if (custVM.CustomerList.Count() > 0)
                        {
                            if (custVM.CustomerList.Where(a => (a.CIF_ID == null || a.CIF_ID == 0)  && a.CustomerId == null && a.CustomerType == Constants.CustomerType.Customer).Any())
                            {
                                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Customers").Add("Error ", "Customer CIF ID Not found").ToFailLogString());
                                return Json(new
                                {
                                    Valid = false,
                                    Error = Resource.Error_SearchCustomerNoCIFID,
                                    Errors = GetModelValidationErrors()
                                });
                            }
                        }

                        ViewBag.PageSize = custVM.SearchFilter.PageSize;
                        ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                        custVM.IsAdminNoteSearch = searchFilter.IsAdminNoteSearch;

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Customer").ToSuccessLogString());
                        return PartialView("~/Views/Customer/_CustomerList.cshtml", custVM);
                    }
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Customers").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }


        private string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[CheckUserRole(ScreenCode.EditCustomer)]
        //public ActionResult SearchCustomerByHPList(CustomerSearchFilter searchFilter)
        //{
        //    Logger.Info(_logMsg.Clear().SetPrefixMsg("SearchCustomerByHPList").Add("AccountNo", searchFilter.AccountNo)
        //        .Add("DeptFirstName", searchFilter.CustomerDeptFirstName).Add("DeptLastName", searchFilter.CustomerDeptLastName).Add("LicensePlateNo", searchFilter.Registration).ToInputLogString());

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _commonFacade = new CommonFacade();
        //            WebServicePagingEntity vPaging = new WebServicePagingEntity
        //            {
        //                PageNumber = 1,
        //                PageSize = _commonFacade.GetPageSizeStart()
        //            };


        //            CustomerViewModel custVM = SearchCustomerList(searchFilter);



        //            //CustomerHPViewModel custHP = new CustomerHPViewModel();
        //            //custHP.SearchFilter = searchFilter;
        //            //custHP.CustomerList = InquiryHPCustomerContractListWS(searchFilter.AccountNo, "", searchFilter.CustomerDeptFirstName, searchFilter.CustomerDeptLastName, "", "", searchFilter.Registration, vPaging);

        //            //ViewBag.PageSize = vPaging.PageSize;
        //            //ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
        //            //custHP.SearchFilter.TotalRecords = custHP.CustomerList.Count();

        //            //Logger.Info(_logMsg.Clear().SetPrefixMsg("SearchCustomerHPList").ToSuccessLogString());
        //            //return PartialView("~/Views/Customer/_CustomerHPList.cshtml", custHP);
        //        }

        //        return Json(new
        //        {
        //            Valid = false,
        //            Error = string.Empty,
        //            Errors = GetModelValidationErrors()
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("Exception occur:\n", ex);
        //        Logger.Info(_logMsg.Clear().SetPrefixMsg("SearchCustomerByHPList").Add("Error Message", ex.Message).ToFailLogString());
        //        return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
        //            this.ControllerContext.RouteData.Values["action"].ToString()));
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.EditCustomer)]
        public ActionResult InitEditCustomer(int? customerId = null)
        {
            try
            {
                CustomerViewModel custVM = null;

                if (TempData["CustomerVM"] != null)
                {
                    custVM = (CustomerViewModel)TempData["CustomerVM"];
                }
                else
                {
                    custVM = new CustomerViewModel { CustomerId = customerId };
                }

                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit Customer").Add("CustomerId", custVM.CustomerId).ToInputLogString());
                _customerFacade = new CustomerFacade();

                if (customerId.HasValue)
                {
                    var customerEntity = _customerFacade.GetCustomerByID(customerId.Value);
                    custVM.CustomerId = customerEntity.CustomerId;
                    custVM.CardTypeId = customerEntity.CbsCardType != null ? customerEntity.CbsCardType.CardTypeID.ConvertToString() : "";
                    custVM.TitleThai = customerEntity.TitleThai != null ? customerEntity.TitleThai.TitleId.ConvertToString() : "";
                    custVM.TitleEnglish = customerEntity.TitleEnglish != null ? customerEntity.TitleEnglish.TitleId.ConvertToString() : "";
                    custVM.FirstNameThai = customerEntity.FirstNameThai;
                    custVM.LastNameThai = customerEntity.LastNameThai;
                    custVM.TitleEnglish = customerEntity.TitleEnglish != null ? customerEntity.TitleEnglish.TitleId.ConvertToString() : "";
                    custVM.FirstNameEnglish = customerEntity.FirstNameEnglish;
                    custVM.LastNameEnglish = customerEntity.LastNameEnglish;
                    custVM.CardNo = customerEntity.CardNo;
                    custVM.BirthDate = customerEntity.BirthDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate);
                    custVM.Email = customerEntity.Email;
                    custVM.Fax = customerEntity.Fax;
                    custVM.EmployeeCode = customerEntity.EmployeeCode;

                    // Phone
                    if (customerEntity.PhoneList != null)
                    {
                        if (customerEntity.PhoneList.Count > 0)
                        {
                            custVM.PhoneType1 = customerEntity.PhoneList[0].PhoneTypeId.ConvertToString();
                            custVM.PhoneNo1 = customerEntity.PhoneList[0].PhoneNo;
                        }
                        else
                        {
                            custVM.PhoneType1 = string.Empty;
                            custVM.PhoneNo1 = string.Empty;
                        }

                        if (customerEntity.PhoneList.Count > 1)
                        {
                            custVM.PhoneType2 = customerEntity.PhoneList[1].PhoneTypeId.ConvertToString();
                            custVM.PhoneNo2 = customerEntity.PhoneList[1].PhoneNo;
                        }
                        else
                        {
                            custVM.PhoneType2 = string.Empty;
                            custVM.PhoneNo2 = string.Empty;
                        }

                        if (customerEntity.PhoneList.Count > 2)
                        {
                            custVM.PhoneType3 = customerEntity.PhoneList[2].PhoneTypeId.ConvertToString();
                            custVM.PhoneNo3 = customerEntity.PhoneList[2].PhoneNo;
                        }
                        else
                        {
                            custVM.PhoneType3 = string.Empty;
                            custVM.PhoneNo3 = string.Empty;
                        }
                    }
                }

                // Get SelectList
                _commonFacade = new CommonFacade();
                custVM.CardTypeList = new SelectList((IEnumerable)_commonFacade.GetCbsCardTypeSelectList(), "Key", "Value", string.Empty);
                custVM.TitleThaiList = new SelectList((IEnumerable)_commonFacade.GetTitleThaiSelectList(), "Key", "Value", string.Empty);
                custVM.TitleEnglishList = new SelectList((IEnumerable)_commonFacade.GetTitleEnglishSelectList(), "Key", "Value", string.Empty);
                custVM.PhoneTypeList = new SelectList((IEnumerable)_commonFacade.GetPhoneTypeSelectList(), "Key", "Value", string.Empty);

                return View("~/Views/Customer/Edit.cshtml", custVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit Customer").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.EditCustomer)]
        public ActionResult Edit(CustomerViewModel customerVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Customer").Add("PhoneNo1", customerVM.PhoneNo1).ToInputLogString());

            try
            {
                #region "Validate CardNo"

                if (!string.IsNullOrEmpty(customerVM.CardTypeName))
                {
                    _commonFacade = new CommonFacade();
                    var cbsCardTypePersonal = _commonFacade.GetCbsCardTypeByCode(Constants.CbsCardTypeCode.Personal);

                    if (string.IsNullOrEmpty(customerVM.CardNo))
                    {
                        ModelState.AddModelError("CardNo", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_RequiredField, Resource.Lbl_CardNo_Passport));
                    }
                    else if (customerVM.CardTypeId == cbsCardTypePersonal.CardTypeID.ConvertToString())
                    {
                        if (!ApplicationHelpers.ValidateCardNo(customerVM.CardNo))
                        {
                            ModelState.AddModelError("CardNo", Resource.ValErr_InvalidCardNo);
                        }
                    }
                }

                #endregion

                #region "Validate FirstName"

                if (string.IsNullOrEmpty(customerVM.FirstNameThai) && string.IsNullOrEmpty(customerVM.FirstNameEnglish))
                {
                    ModelState.AddModelError("FirstNameThai", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_RequiredField, Resource.Lbl_FirstNameThai));
                }

                #endregion

                #region  "Validate Phone"

                //if (string.IsNullOrEmpty(customerVM.PhoneType1))
                //{
                //    ModelState.Remove("PhoneNo1");
                //}
                if (customerVM.NotValidatePhone1 || customerVM.IsConfirm == "1")
                {
                    ModelState.Remove("PhoneType1");
                    ModelState.Remove("PhoneNo1");
                }
                if (string.IsNullOrEmpty(customerVM.PhoneType2))
                {
                    ModelState.Remove("PhoneNo2");
                }
                if (string.IsNullOrEmpty(customerVM.PhoneType3))
                {
                    ModelState.Remove("PhoneNo3");
                }

                // Check duplicate phoneNo
                if (!string.IsNullOrEmpty(customerVM.PhoneNo1) && !string.IsNullOrEmpty(customerVM.PhoneNo2))
                {
                    if (customerVM.PhoneNo1.Equals(customerVM.PhoneNo2))
                    {
                        ModelState.AddModelError("PhoneNo1", Resource.ValError_DuplicatePhoneNo);
                        ModelState.AddModelError("PhoneNo2", Resource.ValError_DuplicatePhoneNo);
                    }
                }

                if (!string.IsNullOrEmpty(customerVM.PhoneNo1) && !string.IsNullOrEmpty(customerVM.PhoneNo3))
                {
                    if (customerVM.PhoneNo1.Equals(customerVM.PhoneNo3))
                    {
                        ModelState.AddModelError("PhoneNo1", Resource.ValError_DuplicatePhoneNo);
                        ModelState.AddModelError("PhoneNo3", Resource.ValError_DuplicatePhoneNo);
                    }
                }

                if (!string.IsNullOrEmpty(customerVM.PhoneNo2) && !string.IsNullOrEmpty(customerVM.PhoneNo3))
                {
                    if (customerVM.PhoneNo2.Equals(customerVM.PhoneNo3))
                    {
                        ModelState.AddModelError("PhoneNo2", Resource.ValError_DuplicatePhoneNo);
                        ModelState.AddModelError("PhoneNo3", Resource.ValError_DuplicatePhoneNo);
                    }
                }

                #endregion

                if (!string.IsNullOrEmpty(customerVM.BirthDate) && !customerVM.BirthDateValue.HasValue)
                {
                    ModelState.AddModelError("BirthDate", Resource.ValErr_InvalidDate);
                }
                else if (!string.IsNullOrEmpty(customerVM.BirthDate) && customerVM.BirthDateValue.HasValue)
                {
                    if (customerVM.BirthDateValue.Value > DateTime.Now.Date)
                    {
                        ModelState.AddModelError("BirthDate", Resource.ValErr_InvalidDate_MustLessThanToday);
                    }
                }

                if (ModelState.IsValid)
                {
                    _customerFacade = new CustomerFacade();
                    _commonFacade = new CommonFacade();

                    CbsCardTypeEntity stb = new CbsCardTypeEntity { CardTypeCode = "" };
                    TitleEntity tiTh = new TitleEntity { TitleName = "" };
                    TitleEntity tiEn = new TitleEntity { TitleName = "" };
                    if (string.IsNullOrEmpty(customerVM.CardTypeId) == false)
                    {
                        stb = _commonFacade.GetCbsCardTypeById(Convert.ToInt32(customerVM.CardTypeId));
                        if (stb != null) {
                            customerVM.CardTypeName = stb.CardTypeName;
                            customerVM.CardTypeCode = stb.CardTypeCode;
                        }
                    }

                    if (string.IsNullOrEmpty(customerVM.TitleThai) == false)
                        tiTh = _commonFacade.GetTitleById(Convert.ToInt32(customerVM.TitleThai), Constants.TitleLanguage.TitleTh);

                    if (string.IsNullOrEmpty(customerVM.TitleEnglish) == false)
                        tiEn = _commonFacade.GetTitleById(Convert.ToInt32(customerVM.TitleEnglish), Constants.TitleLanguage.TitleEn);

                    #region "Check Duplicate CardNo"
                    if (!string.IsNullOrEmpty(customerVM.CardTypeId))
                    {
                        //if (_customerFacade.IsDuplicateCardNo(customerVM.CustomerId,
                        //    customerVM.SubscriptType.ToNullable<int>(), customerVM.CardNo))
                        if (WS_IsDuplicateCardNo(customerVM.CustomerId, customerVM.CardTypeId, customerVM.CardNo))
                        {
                            ViewBag.ErrorMessage = Resource.ValError_DuplicateCardNo;
                            TempData["CustomerVM"] = customerVM;
                            return InitEditCustomer();
                        }
                    }

                    #endregion

                    if (customerVM.IsConfirm == null || customerVM.IsConfirm != "1")
                    {
                        #region "Check Duplicate PhoneNo"

                        List<string> lstPhoneNo = new List<string>();
                        if (!string.IsNullOrEmpty(customerVM.PhoneNo1)) lstPhoneNo.Add(customerVM.PhoneNo1);
                        if (!string.IsNullOrEmpty(customerVM.PhoneNo2)) lstPhoneNo.Add(customerVM.PhoneNo2);
                        if (!string.IsNullOrEmpty(customerVM.PhoneNo3)) lstPhoneNo.Add(customerVM.PhoneNo3);

                        if (lstPhoneNo.Count > 0)
                        {
                            //customerVM.CustomerList = _customerFacade.GetCustomerByPhoneNo(customerVM.CustomerId, lstPhoneNo);
                            customerVM.CustomerList = WS_GetCustomerByPhoneNo(customerVM.CustomerId, lstPhoneNo.ToArray());

                            if (customerVM.CustomerList != null && customerVM.CustomerList.Any())
                            {
                                customerVM.IsSubmit = "1";
                                TempData["CustomerVM"] = customerVM;
                                return InitEditCustomer();
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(customerVM.FirstNameThai))
                            {
                                //customerVM.CustomerList = _customerFacade.GetCustomerByName(customerVM.FirstNameThai);
                                customerVM.CustomerList = WS_GetCustomerByName(customerVM.CustomerId, customerVM.FirstNameThai);
                                if (customerVM.CustomerList != null && customerVM.CustomerList.Any())
                                {
                                    customerVM.IsSubmit = "1";
                                    TempData["CustomerVM"] = customerVM;
                                    return InitEditCustomer();
                                }
                            }
                        }

                        #endregion
                    }

                    // Save
                    bool isSuccess = SaveCustomer(customerVM);
                    if (isSuccess)
                    {
                        //TempData["CustomerId"] = customerVM.CustomerId;
                        //ฝฝcustomerId.ConvertToString() + "#" + customerNumber.ConvertToString()
                        string encryptedstring = StringCipher.Encrypt(customerVM.CustomerId.ConvertToString() + "#0", Constants.PassPhrase);
                        TempData["CustomerInfo" + customerVM.CustomerId.ConvertToString() + "0"] = CreateCustomerInfo(customerVM.CustomerId.Value, 0, Constants.CustomerType.Prospect.ToString(), customerVM.BirthDateValue.ToString(), customerVM.CardNo, stb.CardTypeCode, stb.CardTypeName, "", tiTh.TitleName, customerVM.FirstNameThai, customerVM.LastNameThai, tiEn.TitleName, customerVM.FirstNameEnglish, customerVM.LastNameEnglish, customerVM.EmployeeCode);

                        return RedirectToAction("CustomerNote", "Customer", new { encryptedString = encryptedstring });
                    }

                    ViewBag.ErrorMessage = Resource.Error_SaveFailed;
                }
                else
                {
                    customerVM.IsSubmit = "0";
                }

                TempData["CustomerVM"] = customerVM;
                return InitEditCustomer();

            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Customer").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        public bool WS_IsDuplicateCardNo(int? custId, string cbsCardTypeId, string cardNo)
        {
            var custVM = SearchCustomerList(new CustomerSearchFilter()
            {
                CardNo = cardNo,
                PageNo = 1,
                PageSize = 100
            });
            var custList = custVM.CustomerList?.Where(x => (!custId.HasValue || x.CustomerId != custId) && (x.CbsCardType != null && x.CbsCardType.CardTypeID.ToString() == cbsCardTypeId)).Count();
            return (custList ?? 0) > 0;
        }

        public IEnumerable<CustomerEntity> WS_GetCustomerByPhoneNo(int? customerId, params string[] phoneNos)
        {
            List<CustomerEntity> custList = new List<CustomerEntity>();
            if (phoneNos != null && phoneNos.Length > 0)
            {
                foreach (string phone in phoneNos)
                {
                    var custVM = SearchCustomerList(new CustomerSearchFilter()
                    {
                        PhoneNo = phone,
                        ExactPhoneNo = true,
                        PageNo = 1,
                        PageSize = 100
                    });
                    if (custVM?.CustomerList != null)
                    {
                        //กรองเอาเฉพาะลูกค้าที่ไม่ซ้ำกัน
                        var cifList = custList.Where(x => (x.CIF_ID ?? 0) > 0).Select(x => x.CIF_ID);
                        var custIdList = custList.Where(x => (x.CustomerId ?? 0) > 0).Select(x => x.CustomerId);
                        var newList = custVM.CustomerList.Where(x => ((x.CIF_ID ?? 0) == 0 || !cifList.Contains(x.CIF_ID))
                                                                    && ((x.CustomerId ?? 0) == 0 || !custIdList.Contains(x.CustomerId))).ToList();
                        custList = custList.Union(newList).ToList();
                    }
                }
            }
            return custList;
        }

        private IEnumerable<CustomerEntity> WS_GetCustomerByName(int? customerId, string firstNameThai)
        {
            var custVM = SearchCustomerList(new CustomerSearchFilter()
            {
                FirstName = firstNameThai,
                ExactPhoneNo = true,
                PageNo = 1,
                PageSize = 100
            });
            return custVM.CustomerList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.SearchCustomer)]
        public ActionResult ClearIVR()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Clear CallId").Add("CallId", this.CallId).Add("PhoneNo", this.PhoneNo).ToInputLogString());

            try
            {
                // Reset routedata
                ClearCallId();
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Clear CallId").ToSuccessLogString());
                return RedirectToAction("Search", "Customer");
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Clear CallId").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        private bool SaveCustomer(CustomerViewModel customerVM)
        {
            CustomerEntity customerEntity = new CustomerEntity
            {
                CustomerId = customerVM.CustomerId,
                CbsCardType = new CbsCardTypeEntity
                {
                    CardTypeID = Convert.ToInt32(customerVM.CardTypeId),
                    CardTypeCode = customerVM.CardTypeCode,
                    CardTypeName = customerVM.CardTypeName
                },
                CardNo = customerVM.CardNo,
                BirthDate = customerVM.BirthDateValue,
                TitleThai = new TitleEntity
                {
                    TitleId = customerVM.TitleThai.ToNullable<int>()
                },
                FirstNameThai = customerVM.FirstNameThai,
                LastNameThai = customerVM.LastNameThai,
                TitleEnglish = new TitleEntity
                {
                    TitleId = customerVM.TitleEnglish.ToNullable<int>()
                },
                FirstNameEnglish = customerVM.FirstNameEnglish,
                LastNameEnglish = customerVM.LastNameEnglish,
                Email = customerVM.Email,
                CreateUser = new UserEntity
                {
                    UserId = this.UserInfo.UserId
                },
                UpdateUser = new UserEntity
                {
                    UserId = this.UserInfo.UserId
                }
            };

            // Phone & Fax
            customerEntity.PhoneList = new List<PhoneEntity>();
            if (!string.IsNullOrEmpty(customerVM.PhoneNo1))
            {
                customerEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = customerVM.PhoneType1.ToNullable<int>(), PhoneNo = customerVM.PhoneNo1 });
            }
            if (!string.IsNullOrEmpty(customerVM.PhoneNo2))
            {
                customerEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = customerVM.PhoneType2.ToNullable<int>(), PhoneNo = customerVM.PhoneNo2 });
            }
            if (!string.IsNullOrEmpty(customerVM.PhoneNo3))
            {
                customerEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = customerVM.PhoneType3.ToNullable<int>(), PhoneNo = customerVM.PhoneNo3 });
            }
            // Fax
            if (!string.IsNullOrEmpty(customerVM.Fax))
            {
                _commonFacade = new CommonFacade();
                var phoneTypeFax = _commonFacade.GetPhoneTypeByCode(Constants.PhoneTypeCode.Fax);
                customerEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = phoneTypeFax.PhoneTypeId, PhoneNo = customerVM.Fax });
            }

            _customerFacade = new CustomerFacade();
            bool isSuccess = _customerFacade.SaveCustomer(customerEntity);
            if (isSuccess)
            {
                customerVM.CustomerId = customerEntity.CustomerId; // CustomerId ที่ได้จากการ Save
            }

            return isSuccess;
        }

        #endregion

        #region "Admin Note"

        [CheckUserRole(ScreenCode.SearchNoteForCustomer)]
        public ActionResult SearchNote()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Notes").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();

                CustomerViewModel custVM = new CustomerViewModel();
                custVM.SearchFilter = new CustomerSearchFilter
                {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "CustomerId",
                    SortOrder = "DESC"
                };

                var customerProductList = _commonFacade.GetCustomerProductSelectList("");
                custVM.CustomerProductList = new SelectList((IEnumerable)customerProductList, "Key", "Value", string.Empty);

                var customerTypeList = _commonFacade.GetCustomerTypeSelectList("");
                custVM.CustomerTypeList = new SelectList((IEnumerable)customerTypeList, "Key", "Value", string.Empty);

                ViewBag.PageSize = custVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View("~/Views/Customer/SearchCustomerNote.cshtml", custVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Notes").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.SearchNoteForCustomer)]
        public ActionResult AdminNoteList(CustomerSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Notes").Add("CardNo", searchFilter.CardNo.MaskCardNo())
                .Add("FirstName", searchFilter.FirstName).Add("LastName", searchFilter.LastName).ToInputLogString());

            try
            {
                if (!string.IsNullOrWhiteSpace(searchFilter.FirstName)
                    && searchFilter.FirstName.ExtractString().Length < Constants.MinLenght.SearchTerm)
                {
                    ModelState["FirstName"].Errors.Clear();
                    ModelState["FirstName"].Errors.Add(string.Format(CultureInfo.InvariantCulture, Resource.ValErr_MinLength, Constants.MinLenght.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(searchFilter.LastName) &&
                    searchFilter.LastName.ExtractString().Length < Constants.MinLenght.SearchTerm)
                {
                    ModelState["LastName"].Errors.Clear();
                    ModelState["LastName"].Errors.Add(string.Format(CultureInfo.InvariantCulture, Resource.ValErr_MinLength, Constants.MinLenght.SearchTerm));
                }

                if (ModelState.IsValid)
                {
                    CustomerViewModel custVM = SearchCustomerList(searchFilter);
                    if (custVM.CustomerHPCount > 0)
                    {
                        ViewBag.PageSize = custVM.SearchFilter.PageSize;
                        ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Search HP Customer").ToSuccessLogString());
                        //return PartialView("~/Views/Customer/_CustomerHPList.cshtml", custVM);

                        return Json(new
                        {
                            Valid = true,
                            CustomerHPCount = custVM.CustomerHPCount,
                            ResultView = ConvertViewToString("~/Views/Customer/_CustomerHPList.cshtml", custVM),
                            Error = string.Empty,
                            Errors = string.Empty
                        });
                    }
                    else
                    {
                        ViewBag.PageSize = custVM.SearchFilter.PageSize;
                        ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Notes").ToSuccessLogString());
                        return PartialView("~/Views/Customer/_AdminNoteList.cshtml", custVM);
                    }
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Notes").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpGet]
        public ActionResult AdminNote()
        {
            return RedirectToAction("SearchNote");
        }

        [HttpPost]
        [CheckUserRole(ScreenCode.SearchNoteForCustomer)]
        public ActionResult AdminNote(int customerId, decimal customerNumber, string CustomerType, string BirthDate, string IDNumber, string CbsCardTypeCode, string CbsCardTypeName, string CountryOfCitizenship, string Title, string PrimaryName, string PrimaryLastName, string AlternateTitle, string AlternateFirstName, string AlternateLastName, string emplCode)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Admin Notes").Add("CustomerId", customerId).ToInputLogString());

            try
            {
                CustomerViewModel custVM = new CustomerViewModel();

                //Get Customer Profile From Webservice
                custVM.CustomerId = customerId;
                custVM.CustomerNumber = customerNumber;

                custVM.CustomerInfo = new CustomerInfoViewModel();
                custVM.CustomerInfo.CustomerId = customerId;
                custVM.CustomerInfo.CustomerNumber = customerNumber;
                custVM.CustomerInfo.CustomerType = Convert.ToInt32(CustomerType);
                if (BirthDate.Equals("") == false)
                    custVM.CustomerInfo.BirthDate = Convert.ToDateTime(BirthDate);
                else
                    custVM.CustomerInfo.BirthDate = null;

                custVM.CustomerInfo.CbsCardType = new CbsCardTypeEntity { CardTypeCode = CbsCardTypeCode, CardTypeName = CbsCardTypeName };
                custVM.CustomerInfo.CardNo = IDNumber;
                custVM.CustomerInfo.CountryOfCitizenship = CountryOfCitizenship;
                custVM.CustomerInfo.TitleThai = new TitleEntity { TitleName = Title };
                custVM.CustomerInfo.TitleEnglish = new TitleEntity { TitleName = AlternateTitle };
                custVM.CustomerInfo.FirstNameThai = HttpUtility.HtmlDecode(PrimaryName);
                custVM.CustomerInfo.LastNameThai = HttpUtility.HtmlDecode(PrimaryLastName);
                custVM.CustomerInfo.FirstNameEnglish = HttpUtility.HtmlDecode(AlternateFirstName);
                custVM.CustomerInfo.LastNameEnglish = HttpUtility.HtmlDecode(AlternateLastName);
                custVM.CustomerInfo.EmployeeCode = emplCode;

                if (customerNumber > 0)
                    custVM.CustomerInfo.CustomerAddress = InquiryCustomerAddress(customerNumber.ToString());
                else
                {
                    custVM.CustomerInfo.CustomerAddress = new CBSCustomerAddressResponse();
                    custVM.CustomerInfo.CustomerAddress.ResponseCode = "CBS-I-1000";
                }

                _customerFacade = new CustomerFacade();
                CustomerContactEntity ccnt = _customerFacade.GetCustomerContact(CustomerType, customerId.ToString(), customerNumber.ToString());
                if (ccnt != null)
                {
                    custVM.CustomerInfo.Email = string.IsNullOrEmpty(ccnt.Email) ? "" : ccnt.Email;
                    custVM.CustomerInfo.Fax = string.IsNullOrEmpty(ccnt.Fax) ? "" : ccnt.Fax;
                    custVM.CustomerInfo.PhoneList = ccnt.PhoneList;
                }
                else
                {
                    custVM.CustomerInfo.Email = "";
                    custVM.CustomerInfo.Fax = "";
                    custVM.CustomerInfo.PhoneList = new List<PhoneEntity>();
                }

                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();

                custVM.NoteSearchFilter = new NoteSearchFilter
                {
                    CustomerId = customerId,
                    CustomerNumber = customerNumber,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "UpdateDate",
                    SortOrder = "DESC"
                };

                custVM.NoteList = _customerFacade.GetNoteList(custVM.NoteSearchFilter);
                ViewBag.PageSize = custVM.NoteSearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View(custVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Admin Notes").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.SearchNoteForCustomer)]
        public ActionResult NoteList(NoteSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Notes").Add("CustomerId", searchFilter.CustomerId)
                .ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _customerFacade = new CustomerFacade();
                    CustomerViewModel custVM = new CustomerViewModel();
                    custVM.NoteSearchFilter = searchFilter;

                    custVM.NoteList = _customerFacade.GetNoteList(custVM.NoteSearchFilter);
                    ViewBag.PageSize = custVM.NoteSearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Note List").ToSuccessLogString());
                    return PartialView("~/Views/Customer/_NoteList.cshtml", custVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Notes").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [CheckUserRole(ScreenCode.ManageNoteForCustomer)]
        public ActionResult InitEditNote(int? noteId, int? customerId, decimal? customerNumber)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit Note").Add("NoteId", noteId).Add("CustomerId", customerId).ToInputLogString());

            try
            {
                _customerFacade = new CustomerFacade();
                NoteViewModel noteVM = new NoteViewModel();

                if (noteId.HasValue)
                {
                    NoteEntity noteEntity = _customerFacade.GetNoteByID(noteId.Value);
                    noteVM.CustomerId = noteEntity.CustomerId;
                    noteVM.CustomerNumber = noteEntity.CustomerNumber;
                    noteVM.NoteId = noteEntity.NoteId;
                    noteVM.EffectiveDate = noteEntity.EffectiveDateDisplay;
                    noteVM.ExpiryDate = noteEntity.ExpiryDateDisplay;
                    noteVM.Detail = noteEntity.Detail;
                    noteVM.CreateUser = noteEntity.CreateUser.FullName;
                    noteVM.CreateDate = noteEntity.CreateDateDisplay;
                    noteVM.UpdateUser = noteEntity.UpdateUser.FullName;
                    noteVM.UpdateDate = noteEntity.UpdateDateDisplay;
                }
                else
                {
                    var today = DateTime.Now;
                    UserEntity userLogin = this.UserInfo;
                    noteVM.CreateUser = userLogin.FullName;
                    noteVM.CreateDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                    noteVM.UpdateUser = userLogin.FullName;
                    noteVM.UpdateDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                    noteVM.CustomerNumber = customerNumber;
                    noteVM.CustomerId = customerId;
                }

                return PartialView("~/Views/Customer/_EditNote.cshtml", noteVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit Note").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ManageNoteForCustomer)]
        public JsonResult SaveNote(NoteViewModel noteVM)
        {
            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Note").ToInputLogString());

                bool isValid = TryUpdateModel(noteVM);

                if (!string.IsNullOrEmpty(noteVM.EffectiveDate) && !noteVM.EffectiveDateValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("EffectiveDate", Resource.ValErr_InvalidDate);
                }
                if (!string.IsNullOrEmpty(noteVM.ExpiryDate) && !noteVM.ExpiryDateValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("ExpiryDate", Resource.ValErr_InvalidDate);
                }
                if (noteVM.EffectiveDateValue.HasValue && noteVM.ExpiryDateValue.HasValue
                    && noteVM.EffectiveDateValue.Value > noteVM.ExpiryDateValue.Value)
                {
                    isValid = false;
                    ModelState.AddModelError("EffectiveDate", Resource.ValErr_InvalidDateRange);
                    ModelState.AddModelError("ExpiryDate", "");
                }

                // Validate MaxLength
                if (!string.IsNullOrWhiteSpace(noteVM.Detail) && noteVM.Detail.Count() > Constants.MaxLength.Note)
                {
                    isValid = false;
                    ModelState.AddModelError("Detail", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_StringLength, Resource.Lbl_Detail, Constants.MaxLength.Note));
                }

                if (isValid)
                {
                    // Save
                    NoteEntity noteEntity = new NoteEntity
                    {
                        NoteId = noteVM.NoteId,
                        CustomerId = noteVM.CustomerId,
                        CustomerNumber = noteVM.CustomerNumber,
                        EffectiveDate = noteVM.EffectiveDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate),
                        ExpiryDate = noteVM.ExpiryDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate),
                        Detail = noteVM.Detail,
                        CreateUser = new UserEntity { UserId = this.UserInfo.UserId },
                        UpdateUser = new UserEntity { UserId = this.UserInfo.UserId }
                    };

                    _customerFacade = new CustomerFacade();
                    _customerFacade.SaveNote(noteEntity);

                    return Json(new
                    {
                        Valid = true
                    });
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Note").Add("Error Message", ex.Message).ToInputLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }

        #endregion

        #region "Customer Note"

        private CustomerInfoViewModel CreateCustomerInfo(int customerId, decimal customerNumber, string CustomerType, string BirthDate, string IDNumber, string CbsCardTypeCode, string CbsCardTypeName,  string CountryOfCitizenship, string Title, string PrimaryName, string PrimaryLastName, string AlternateTitle, string AlternateFirstName, string AlternateLastName, string emplCode)
        {
            CustomerInfoViewModel ret = new CustomerInfoViewModel();
            ret.CustomerId = customerId;
            ret.CustomerNumber = customerNumber;
            ret.CustomerType = Convert.ToInt32(CustomerType);
            if (string.IsNullOrEmpty(BirthDate) == false)
                ret.BirthDate = Convert.ToDateTime(BirthDate);
            else
                ret.BirthDate = null;

            ret.CbsCardType = new CbsCardTypeEntity { CardTypeCode = CbsCardTypeCode, CardTypeName = CbsCardTypeName };
            ret.CardNo = IDNumber;
            ret.CountryOfCitizenship = CountryOfCitizenship;
            ret.TitleThai = new TitleEntity { TitleName = Title };
            ret.TitleEnglish = new TitleEntity { TitleName = AlternateTitle };
            ret.FirstNameThai = HttpUtility.HtmlDecode(PrimaryName);
            ret.LastNameThai = HttpUtility.HtmlDecode(PrimaryLastName);
            ret.FirstNameEnglish = HttpUtility.HtmlDecode(AlternateFirstName);
            ret.LastNameEnglish = HttpUtility.HtmlDecode(AlternateLastName);
            ret.EmployeeCode = emplCode;

            if (customerNumber > 0)
                ret.CustomerAddress = InquiryCustomerAddress(customerNumber.ToString());
            else
            {
                ret.CustomerAddress = new CBSCustomerAddressResponse();
                ret.CustomerAddress.ResponseCode = "CBS-I-1000";
            }

            CustomerContactEntity ccnt = _customerFacade.GetCustomerContact(CustomerType, customerId.ToString(), customerNumber.ToString());
            if (ccnt != null)
            {
                ret.Email = string.IsNullOrEmpty(ccnt.Email) ? "" : ccnt.Email;
                ret.Fax = string.IsNullOrEmpty(ccnt.Fax) ? "" : ccnt.Fax;
                ret.PhoneList = ccnt.PhoneList;
            }
            else
            {
                ret.Email = "";
                ret.Fax = "";
                ret.PhoneList = new List<PhoneEntity>();
            }

            return ret;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerNote)]
        public ActionResult InitCustomerNote(int customerId,decimal customerNumber,string CustomerType, string BirthDate, string IDNumber, string SubscriptTypeCode, string SubscriptTypeName, string SubscriptTypeCodeCBS, string CountryOfCitizenship, string Title, string PrimaryName, string PrimaryLastName, string AlternateTitle, string AlternateFirstName, string AlternateLastName, string emplCode)
        {
            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Customer Notes").Add("CustomerId", customerId).ToInputLogString());

                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();
                CustomerViewModel custVM = new CustomerViewModel();
                custVM.CustomerId = customerId;
                custVM.CustomerNumber = customerNumber;
                custVM.CustomerInfo = CreateCustomerInfo(customerId, customerNumber, CustomerType, BirthDate, IDNumber, SubscriptTypeCode, SubscriptTypeName, CountryOfCitizenship, Title, PrimaryName, PrimaryLastName, AlternateTitle, AlternateFirstName, AlternateLastName, emplCode);

                TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()] = custVM.CustomerInfo;
                // Note list
                custVM.NoteSearchFilter = new NoteSearchFilter
                {
                    CustomerId = customerId,
                    CustomerNumber = customerNumber,
                    EffectiveDate = DateTime.Today, // สำหรับ filter ที่ยังไม่หมดอายุ
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "UpdateDate",
                    SortOrder = "DESC"
                };

                custVM.NoteList = _customerFacade.GetNoteList(custVM.NoteSearchFilter);
                ViewBag.PageSize = custVM.NoteSearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View("~/Views/Customer/CustomerNote.cshtml", custVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Customer Notes").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }


        [CheckUserRole(ScreenCode.ViewCustomerNote)]
        public ActionResult CustomerNote(string encryptedString)
        {
            try
            {
                int? customerId = encryptedString.ToCustomerId();
                decimal? customerNumber = encryptedString.ToCustomerNumber();
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Customer Notes").Add("CustomerId", customerId).ToInputLogString());

                if(customerId == 0 && customerNumber == 0) {
                    return RedirectToAction("Search", "Customer");
                }

                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();
                CustomerViewModel custVM = new CustomerViewModel();
                // Note list
                custVM.NoteSearchFilter = new NoteSearchFilter
                {
                    CustomerNumber = customerNumber.Value,
                    CustomerId = customerId.Value,
                    EffectiveDate = DateTime.Today, // สำหรับ filter ที่ยังไม่หมดอายุ
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "UpdateDate",
                    SortOrder = "DESC"
                };

                // CustomerInfo                    
                if (TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()] != null)
                {
                    custVM.CustomerInfo = (CustomerInfoViewModel)TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()];
                    TempData["CustomerInfo" + customerId.ToString() + customerNumber.ToString()] = custVM.CustomerInfo;
                }
                return View("~/Views/Customer/CustomerNote.cshtml", custVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Customer Notes").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerNote)]
        public ActionResult CustomerNoteList(NoteSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search CustomerNotes").Add("CustomerId", searchFilter.CustomerId).Add("CustomerNumber",searchFilter.CustomerNumber)
                .ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _customerFacade = new CustomerFacade();
                    CustomerViewModel custVM = new CustomerViewModel();

                    searchFilter.EffectiveDate = DateTime.Today; // สำหรับ filter ที่ยังไม่หมดอายุ
                    custVM.NoteSearchFilter = searchFilter;

                    custVM.NoteList = _customerFacade.GetNoteList(custVM.NoteSearchFilter);
                    ViewBag.PageSize = custVM.NoteSearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("CustomerNote List").ToSuccessLogString());
                    return PartialView("~/Views/Customer/_CustomerNoteList.cshtml", custVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search CustomerNotes").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [CheckUserRole(ScreenCode.ViewCustomerNote)]
        public ActionResult InitViewNote(int? noteId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitView Note").Add("NoteId", noteId).ToInputLogString());

            try
            {
                _customerFacade = new CustomerFacade();
                NoteViewModel noteVM = new NoteViewModel();

                if (noteId.HasValue)
                {
                    NoteEntity noteEntity = _customerFacade.GetNoteByID(noteId.Value);
                    noteVM.CustomerId = noteEntity.CustomerId;
                    noteVM.NoteId = noteEntity.NoteId;
                    noteVM.EffectiveDate = noteEntity.EffectiveDateDisplay;
                    noteVM.ExpiryDate = noteEntity.ExpiryDateDisplay;
                    noteVM.Detail = noteEntity.Detail;
                    noteVM.CreateUser = noteEntity.CreateUser.FullName;
                    noteVM.CreateDate = noteEntity.CreateDateDisplay;
                    noteVM.UpdateUser = noteEntity.UpdateUser.FullName;
                    noteVM.UpdateDate = noteEntity.UpdateDateDisplay;
                }

                return PartialView("~/Views/Customer/_ViewNote.cshtml", noteVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitView Note").Add("Error Message", ex).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        #endregion

        #region "AutoComplete"

        [HttpGet]
        public JsonResult SearchByBranchName(string searchTerm, int pageSize, int pageNum)
        {
            _customerFacade = new CustomerFacade();
            List<AccountEntity> accounts = _customerFacade.GetAccountBranchByName(searchTerm, pageSize, pageNum);
            int total = _customerFacade.GetAccountBranchCountByName(searchTerm);

            Select2PagedResult jsonPaged = new Select2PagedResult();
            jsonPaged.Results = new List<Select2Result>();
            foreach (AccountEntity acc in accounts)
            {
                jsonPaged.Results.Add(new Select2Result { id = acc.BranchDisplay, text = acc.BranchDisplay });
            }
            jsonPaged.Total = total;

            //Return the data as a jsonp result
            return Json(jsonPaged, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchByProduct(string searchTerm, int pageSize, int pageNum)
        {
            _customerFacade = new CustomerFacade();
            List<AccountEntity> accounts = _customerFacade.GetAccountProductByName(searchTerm, pageSize, pageNum);
            int total = _customerFacade.GetAccountProductCountByName(searchTerm);

            Select2PagedResult jsonPaged = new Select2PagedResult();
            jsonPaged.Results = new List<Select2Result>();
            foreach (AccountEntity acc in accounts)
            {
                jsonPaged.Results.Add(new Select2Result { id = acc.Product, text = acc.Product });
            }
            jsonPaged.Total = total;

            //Return the data as a jsonp result
            return Json(jsonPaged, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchByGrade(string searchTerm, string product, int pageSize, int pageNum)
        {
            _customerFacade = new CustomerFacade();
            List<AccountEntity> accounts = _customerFacade.GetAccountGradeByName(searchTerm, product, pageSize, pageNum);
            int total = _customerFacade.GetAccountGradeCountByName(searchTerm, product);

            Select2PagedResult jsonPaged = new Select2PagedResult();
            jsonPaged.Results = new List<Select2Result>();
            foreach (AccountEntity acc in accounts)
            {
                jsonPaged.Results.Add(new Select2Result { id = acc.Grade, text = acc.Grade });
            }
            jsonPaged.Total = total;

            //Return the data as a jsonp result
            return Json(jsonPaged, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Call Web Service Function"

        public IEnumerable<CustomerEntity> InquiryHPCustomerContractListWS(string AccountNo,string AccountName, string CustomerDeptFirstName, string CustomerDeptLastName, string CustomerType, string IDCardNo, string CarNo , WebServicePagingEntity vPaging)
        {
            //InquiryCustomerContract
            HPContractService ws = new HPContractService();
            //ws.Proxy = getWebProxy();

            _commonFacade = new CommonFacade();
            Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryCustomerContractList);

            inquiryCustomerContractListRequest req = new inquiryCustomerContractListRequest();
            req.header = new header
            {
                referenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                transactionDateTime = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceHPDateTime),
                serviceName = profile.service_name,
                systemCode = profile.system_code,
                channelId = profile.channel_id
            };

            req.paging = new paging
            {
                pageNumber = vPaging.PageNumber.ToString(),
                pageSize = vPaging.PageSize.ToString(),
                totalRecord = "0",
            };

            req.carLicensePlate = CarNo;
            req.cardID = IDCardNo;
            req.contractName = AccountName;
            req.contractNo = AccountNo;
            req.custFirstName = CustomerDeptFirstName;
            req.custLastName = CustomerDeptLastName;
            req.custType = "";

            Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryHPCustomerContractListWS").Add("ReferenceNo:", req.header.referenceNo).Add("AccountNo", AccountNo).Add("ContactName", AccountName).Add("CustomerDeptFirstName", CustomerDeptFirstName).Add("CustomerDeptLastName", CustomerDeptLastName).Add("CardNo", IDCardNo).Add("CarLicensePlate", CarNo));
            SaveServiceRequestLog(req.SerializeObject());

            IEnumerable<CustomerEntity> result = new List<CustomerEntity>();
            try {
                inquiryCustomerContractListResponse res = new inquiryCustomerContractListResponse();
                res = ws.InquiryCustomerContractList(req);
                if (res.inquiryCustomerContractListResponse1 != null)
                {
                    result = (from c in res.inquiryCustomerContractListResponse1
                              select new CustomerEntity
                              {
                                  CustomerNumber = (string.IsNullOrEmpty(c.cifId) ? 0 : Convert.ToDecimal(c.cifId)),
                                  CIF_ID = (string.IsNullOrEmpty(c.cifId) ? 0 : Convert.ToDecimal(c.cifId)),
                                  FirstNameThai = c.firstName,
                                  //BirthDate = c.cu,
                                  TitleThai = new TitleEntity { TitleName = c.titleName },
                                  TitleEnglish = new TitleEntity { TitleName = "" },
                                  //FirstNameEnglish = "FirstName",
                                  LastNameThai = c.lastName,
                                  //LastNameEnglish = "LastName",
                                  FirstNameThaiEng = c.firstName,
                                  LastNameThaiEng = c.lastName,
                                  CardNo = c.customerIdNo,
                                  IDTypeCode = c.customerCardTypeDescription,
                                  CbsCardType = new CbsCardTypeEntity
                                  {
                                      CardTypeCode = c.customerCardType,
                                      CardTypeName = c.customerCardTypeDescription
                                  },
                                  CustomerType = Constants.CustomerType.Customer,
                                  Registration = c.licenseNo + " " + c.licenseProvince,
                                  //CountryOfCitizenship =  c.customerCardCountryIssue,
                                  //CustomerAddress = InquiryCustomerAddress((string.IsNullOrEmpty(c.cifId) ? "0" : c.cifId)),
                                  AccountNo = "",
                                  Account = new AccountEntity {
                                      AccountNo = "",
                                      AccountType = "",
                                      AccountTypeDescription = ""
                                  },
                                  CustomerWebserviceType = Constants.CustomerWebServiceType.CustomerByHP
                              }).AsQueryable();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryHPCustomerContractListWS").Add("ReferenceNo:", req.header.referenceNo).SetSuffixMsg("Found data " + res.inquiryCustomerContractListResponse1.Length.ToString() + " Row(s)"));
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryHPCustomerContractListWS").Add("ReferenceNo:", req.header.referenceNo).SetSuffixMsg("No data found"));
                }
                SaveServiceResponseLog(res.SerializeObject());
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryHPCustomerContractListWS").Add("Error Message", ex.Message).ToFailLogString());
            }

            return result;
        }

        public IEnumerable<CustomerEntity> getCustomerByInsuranceAccountWS(string AccountNo,WebServicePagingEntity vPaging)
        {
            IEnumerable<CustomerEntity> result = new List<CustomerEntity>();
            try {

                //_commonFacade = new CommonFacade();
                //Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryCustomerByInformation);
                //SaveServiceRequestLog(req.SerializeObject());
                //req.Header = new InquiryCustomerByInformationHeader
                //{
                //    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                //    TransactionDateTime = DateTime.Now,
                //    ServiceName = Constants.ServiceName.InquiryCustomerByInformation,
                //    SystemCode = profile.system_code,
                //    ChannelID = profile.channel_id
                //};
                //CustomerWebserviceType = Constants.CustomerWebServiceType.CustomerByHP

                //Logger.Info(_logMsg.Clear().SetPrefixMsg().Add("ReferenceNo:", req.Header.ReferenceNo).Add("FirstName", searchFileter.FirstName).Add("LastName", searchFileter.LastName).Add("CardNo", searchFileter.CardNo).Add("PhoneNo", searchFileter.PhoneNo));
                //SaveServiceResponseLog(res.SerializeObject());
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getCustomerByHPAccountWS").Add("Error Message", ex.Message).ToFailLogString());
            }

            return result;
        }

        public CBSCustomerByAccountResponse getCustomerByLoanFundingAccountWS(string AccountNo, WebServicePagingEntity vPaging, string ProductType = "") {
            CBSCustomerService ws = new CBSCustomerService();
            //ws.Proxy = getWebProxy();

            CBSCustomerByAccountResponse result = new CBSCustomerByAccountResponse();
            InquiryCustomerByAccountResponse res = new InquiryCustomerByAccountResponse();
            try
            {
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryCustomerByAccount);
                
                InquiryCustomerByAccount req = new InquiryCustomerByAccount();
                req.Header = new InquiryCustomerByAccountHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };

                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByAccount").Add("ReferenceNo:", req.Header.ReferenceNo).Add("AccountNo", AccountNo));

                req.InquiryCustomerByAccountRequest = new InquiryCustomerByAccountInquiryCustomerByAccountRequest
                {
                    AccountNumber = string.IsNullOrEmpty(AccountNo) ? "" : AccountNo,
                    AccountType = "",
                    ExcludeClosedAccountFlag = ""
                };

                req.Paging = new InquiryCustomerByAccountPaging
                {
                    PageNumber = 1,
                    PageSize = vPaging.PageSize
                };
                SaveServiceRequestLog(req.SerializeObject());

                res = ws.InquiryCustomerByAccount(req);
                if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000" || res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001")
                {
                    result.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    result.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                    
                    if (res.Header.CustomerList != null)
                    {
                        var query = res.Header.CustomerList.AsQueryable();

                        switch (ProductType)
                        {
                            case Constants.CustomerProduct.Loan:
                                query = query.Where(a => a.AccountType == Constants.AccountType.LoanAccount);
                                break;
                            case Constants.CustomerProduct.Funding:
                                query = query.Where(a => a.AccountType != Constants.AccountType.LoanAccount);
                                break;
                            default:
                                break;
                        }

                        //เมื่อได้ข้อมูลจาก InquiryCustomerByAccount แล้ว ให้เอา
                        List<CustomerEntity> custList = new List<CustomerEntity>();
                        query.ToList().ForEach(x =>
                            custList.AddRange(getCustomerByInformationWS(new CustomerSearchFilter { CardNo = x.IDNumber, Product = ProductType }, vPaging).CustomerList)
                        );
                        result.CustomerList = new List<CustomerEntity>();
                        result.CustomerList = custList;

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByAccount").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found data " + result.CustomerList.Count<CustomerEntity>().ToString() + " Row(s)"));
                    }
                    else
                    {
                        result.CustomerList = new List<CustomerEntity>();
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByAccount").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                    }
                }
                else
                {
                    result.CustomerList = new List<CustomerEntity>();
                    result.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    result.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getCustomerByLoanFundingAccountWS").Add("Error Message", ex.Message).ToFailLogString());

                result.ResponseCode = "Exception ";
                result.ResponseMessage = ex.Message;
            }
            SaveServiceResponseLog(res.SerializeObject());

            return result;
        }

        public CBSCustomerByInformationResponse getCustomerByInformationWS(CustomerSearchFilter searchFileter, WebServicePagingEntity vPaging)
        {
            CBSCustomerService ws = new CBSCustomerService();
            //ws.Proxy = getWebProxy();
            CBSCustomerByInformationResponse result = new CBSCustomerByInformationResponse();
            InquiryCustomerByInformationResponse res = new InquiryCustomerByInformationResponse();
            try
            {
                InquiryCustomerByInformation req = new InquiryCustomerByInformation();

                #region "Request Parameter"
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryCustomerByInformation);

                req.Header = new InquiryCustomerByInformationHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = Constants.ServiceName.InquiryCustomerByInformation,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };

                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByInformation").Add("ReferenceNo:" ,req.Header.ReferenceNo).Add("FirstName", searchFileter.FirstName).Add("LastName", searchFileter.LastName).Add("CardNo", searchFileter.CardNo).Add("PhoneNo", searchFileter.PhoneNo));

                string searchFirstName = string.IsNullOrEmpty(searchFileter.FirstName) ? "" : searchFileter.FirstName;
                string searchLastName = string.IsNullOrEmpty(searchFileter.LastName) ? "" : searchFileter.LastName;
                req.InquiryCustomerByInformationRequest = new InquiryCustomerByInformationInquiryCustomerByInformationRequest();
                req.InquiryCustomerByInformationRequest.FirstName = "";
                req.InquiryCustomerByInformationRequest.ExactSearchFirstNameFlag = "";
                req.InquiryCustomerByInformationRequest.LastName = "";
                req.InquiryCustomerByInformationRequest.ExactSearchLastNameFlag = "";
                req.InquiryCustomerByInformationRequest.IDNumber = "";
                req.InquiryCustomerByInformationRequest.ElectronicAddress = "";
                req.InquiryCustomerByInformationRequest.ExactSearchFlag = "";
                req.InquiryCustomerByInformationRequest.CustomerType = "";

                if (searchFirstName != "")
                {
                    req.InquiryCustomerByInformationRequest.FirstName = searchFirstName;
                    req.InquiryCustomerByInformationRequest.ExactSearchFirstNameFlag = (searchFileter.ExactFirstName ? "Y" : "N"); // "N";
                }
               
                if (searchLastName != "")
                {
                    req.InquiryCustomerByInformationRequest.LastName = searchLastName;
                    req.InquiryCustomerByInformationRequest.ExactSearchLastNameFlag = (searchFileter.ExactLastName ? "Y" : "N");
                }
                
                if (string.IsNullOrEmpty(searchFileter.CardNo) == false) {
                    req.InquiryCustomerByInformationRequest.IDNumber =  searchFileter.CardNo;
                }

                if (string.IsNullOrEmpty(searchFileter.PhoneNo) == false)
                {
                    req.InquiryCustomerByInformationRequest.ElectronicAddress = string.IsNullOrEmpty(searchFileter.PhoneNo) ? "" : searchFileter.PhoneNo;
                    req.InquiryCustomerByInformationRequest.ExactSearchFlag = (searchFileter.ExactPhoneNo ? "Y" : "N");
                }

                //if (searchFileter.CustomerType.HasValue)
                //    req.InquiryCustomerByInformationRequest.CustomerType = searchFileter.CustomerType.Value.ToString();

                req.Paging = new InquiryCustomerByInformationPaging();
                req.Paging.PageNumber = vPaging.PageNumber;
                req.Paging.PageSize = vPaging.PageSize;
                SaveServiceRequestLog(req.SerializeObject());

                #endregion

                res = ws.InquiryCustomerByInformation(req);
                if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000" || res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001")
                {
                    result.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    result.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                    if (res.Header.CustomerList != null)
                    {
                        result.CustomerList = new List<CustomerEntity>();
                        result.CustomerList = (from c in res.Header.CustomerList
                                               select new CustomerEntity
                                               {
                                                   CustomerId = 0,
                                                   CIF_ID = Convert.ToDecimal(c.CustomerNumber),
                                                   CustomerNumber = Convert.ToDecimal(c.CustomerNumber),
                                                   FirstNameThai = c.PrimaryName,
                                                   FirstNameEnglish = c.AlternateFirstName,
                                                   LastNameThai = c.PrimaryLastName,
                                                   LastNameEnglish = c.AlternateLastName,
                                                   FirstNameThaiEng = !string.IsNullOrEmpty(c.PrimaryName) ? c.PrimaryName : "",
                                                   LastNameThaiEng = !string.IsNullOrEmpty(c.PrimaryLastName) ? c.PrimaryLastName : "",
                                                   BirthDate = c.BirthDate.ConvertTimeZoneFromUtc(),
                                                   TitleThai = _commonFacade.GetTitleByCode(c.Title, Constants.TitleLanguage.TitleTh),
                                                   TitleEnglish = _commonFacade.GetTitleByCode(c.AlternateTitle, Constants.TitleLanguage.TitleEn),
                                                   CardNo = c.IDNumber,
                                                   IDTypeCode = c.IDTypeCode,
                                                   CbsCardType = _commonFacade.GetCbsCardTypeByCode(c.IDTypeCode),
                                                   CountryOfCitizenship = _commonFacade.GetCbsCountryByCode(c.CountryOfCitizenship).CountryNameTH,
                                                   CustomerType = Constants.CustomerType.Customer,
                                                   CustomerCategory = _commonFacade.GetCbsCustTypeByCode(c.CustomerCategory).LongDescThai,
                                                   //EmployeeCode = c.OfficerCode,
                                                   //EmployeeName = c.OfficerShortName,
                                                   AccountNo = "",
                                                   Account = new AccountEntity
                                                   {
                                                       AccountNo = "",
                                                       AccountType = "",
                                                       AccountTypeDescription = searchFileter.Product
                                                   },
                                                   //CustomerAddress = InquiryCustomerAddress(c.CustomerNumber),
                                                   CustomerWebserviceType = Constants.CustomerWebServiceType.CustomerByInformation
                                               }).AsQueryable();

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByInformation").Add("ReferenceNo:", req.Header.ReferenceNo).Add("Found Data", result.CustomerList.Count<CustomerEntity>() + " Row(s)"));
                    }
                    else
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByInformation").Add("ReferenceNo:", req.Header.ReferenceNo).Add("Not Found Data", ""));
                    }
                }
                else
                {
                    //มี Error Code
                    result.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    result.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByInformation").Add("Error Message", ex.Message).ToFailLogString());
                result.ResponseCode = "Exception ";
                result.ResponseMessage = ex.Message;
            }

            SaveServiceResponseLog(res.SerializeObject());

            return result;
        }

        private CBSCustomerAddressResponse InquiryCustomerAddress(string customerNumber)
        {
            CBSCustomerService ws = new CBSCustomerService();
            InquiryCustomerAddressResponse res = new InquiryCustomerAddressResponse();
            CBSCustomerAddressResponse ret = new CBSCustomerAddressResponse();
            try
            {
                InquiryCustomerAddress req = new InquiryCustomerAddress();
                _commonFacade = new CommonFacade();
                Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryCustomerAddress);

                req.Header = new InquiryCustomerAddressHeader
                {
                    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = Constants.ServiceName.InquiryCustomerAddress,
                    SystemCode = profile.system_code,
                    ChannelID = profile.channel_id
                };

                req.Paging = new InquiryCustomerAddressPaging
                {
                    PageNumber = 1,
                    PageSize = _commonFacade.GetPageSizeStart()
                };

                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerAddress").Add("ReferenceNo:", req.Header.ReferenceNo).Add("customerNumber", customerNumber));

                req.InquiryCustomerAddressRequest = new InquiryCustomerAddressInquiryCustomerAddressRequest();
                req.InquiryCustomerAddressRequest.CustomerNumber = customerNumber;
                req.InquiryCustomerAddressRequest.InquiryMode = "";
                req.InquiryCustomerAddressRequest.SeqSpecified = true;
                req.InquiryCustomerAddressRequest.Seq = 0;
                SaveServiceRequestLog(req.SerializeObject());

                res = ws.InquiryCustomerAddress(req);
                if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000" || res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001")
                {
                    ret.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    ret.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                    if (res.Header.AddressList != null)
                    {
                        ret.CustomerAddress = new List<CustomerAddressEntity>();
                        var addrLocalList = (from addr in res.Header.AddressList
                                             where addr.AddressFormat == Constants.AddressFormat.LocalAddress
                                             select new CustomerAddressEntity
                                             {
                                                 CustomerNumber = Convert.ToDecimal(customerNumber),
                                                 AddressFormatCode = addr.AddressFormat,
                                                 AddressFormatName = (addr.AddressFormat != "" ? _commonFacade.GetAddressFormatByCode(addr.AddressFormat).LongDescTha : ""),
                                                 AddressTypeCode = addr.AddressType,
                                                 AddressTypeName = (addr.AddressType != "" ? _commonFacade.GetAddressTypeByCode(addr.AddressType).LongDescTha : ""),
                                                 Seq = addr.Seq,
                                                 HouseNumber = addr.LocalAddressInfo.LocalAddress.HouseNumber,
                                                 MooLabel = addr.LocalAddressInfo.LocalAddress.MooLabel,
                                                 Moo = addr.LocalAddressInfo.LocalAddress.Moo,
                                                 FloorNumberLabel = addr.LocalAddressInfo.LocalAddress.FloorNumberLabel,
                                                 FloorNumber = addr.LocalAddressInfo.LocalAddress.FloorNumber,
                                                 RoomNumberLabel = addr.LocalAddressInfo.LocalAddress.RoomNumberLabel,
                                                 RoomNumber = addr.LocalAddressInfo.LocalAddress.RoomNumber,
                                                 Building = addr.LocalAddressInfo.LocalAddress.Building,
                                                 SoiLabel = addr.LocalAddressInfo.LocalAddress.SoiLabel,
                                                 Soi = addr.LocalAddressInfo.LocalAddress.Soi,
                                                 RoadLabel = addr.LocalAddressInfo.LocalAddress.RoadLabel,
                                                 Road = addr.LocalAddressInfo.LocalAddress.Road,
                                                 SubDistrict = _commonFacade.GetSubDistrictByCode(addr.LocalAddressInfo.LocalAddress.SubDistrictCode),
                                                 District = _commonFacade.GetDistrictByCode(addr.LocalAddressInfo.LocalAddress.DistrictCode),
                                                 Province = _commonFacade.GetProvinceByCode(addr.LocalAddressInfo.LocalAddress.ProvinceCode),
                                                 PostalCode = addr.LocalAddressInfo.LocalAddress.PostalCode
                                            });

                        if (addrLocalList.Any() == true) {
                            ret.CustomerAddress.AddRange(addrLocalList);
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerAddress").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found Local address data " + addrLocalList.FirstOrDefault().AddressDisplay));
                        }
                            

                        var addrUnstrucList = (from addr in res.Header.AddressList
                                               where addr.AddressFormat != Constants.AddressFormat.LocalAddress
                                               select new CustomerAddressEntity
                                               {
                                                   CustomerNumber = Convert.ToDecimal(customerNumber),
                                                   AddressFormatCode = addr.AddressFormat,
                                                   AddressFormatName = (addr.AddressFormat != "" ? _commonFacade.GetAddressFormatByCode(addr.AddressFormat).LongDescTha : ""),
                                                   AddressTypeCode = addr.AddressType,
                                                   AddressTypeName = (addr.AddressType != "" ? _commonFacade.GetAddressTypeByCode(addr.AddressType).LongDescTha : ""),
                                                   Seq = addr.Seq,
                                                   HouseNumber = addr.AddressLine1,
                                                   SoiLabel = (!string.IsNullOrWhiteSpace(addr.AddressLine2) ? " ซอย" : ""),
                                                   Soi = addr.AddressLine2,
                                                   RoadLabel = (!string.IsNullOrWhiteSpace(addr.AddressLine3) ? " ถนน" : ""),
                                                   Road = addr.AddressLine3,
                                                   AddressAreaDetail = addr.AddressLine4 + " " + addr.AddressLine5 + " " + addr.AddressLine6 + " " + addr.AddressLine7,
                                                   PostalCode = addr.PostalCode,
                                               });

                        if (addrUnstrucList.Any() == true)
                        {
                            ret.CustomerAddress.AddRange(addrUnstrucList);
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerAddress").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found address data " + addrUnstrucList.FirstOrDefault().AddressDisplay));
                        }

                        ret.CustomerAddress = ret.CustomerAddress.OrderBy(x => x.Seq).ToList();
                    }
                }
                else
                {
                    ret.ResponseCode = res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode;
                    ret.ResponseMessage = res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerAddress").Add("Error Message", ex.Message).ToFailLogString());

                ret.ResponseCode = "Exception : ";
                ret.ResponseMessage = ex.Message;
            }

            SaveServiceResponseLog(res.SerializeObject());

            return ret;
        }

        #endregion
        
    }
}
