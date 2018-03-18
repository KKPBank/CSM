using System;
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

                var customerProductList = _commonFacade.GetCustomerProductSelectList(Resource.Ddl_CustomerProduct_All, Constants.ApplicationStatus.All);
                custVM.CustomerProductList = new SelectList((IEnumerable)customerProductList, "Key", "Value", string.Empty);

                var customerTypeList = _commonFacade.GetCustomerTypeSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                custVM.CustomerTypeList = new SelectList((IEnumerable)customerTypeList, "Key", "Value", string.Empty);

                if (!string.IsNullOrWhiteSpace(this.CallId) && !skip.Equals("1"))
                {
                    _customerFacade = new CustomerFacade();
                    CallInfoEntity callInfo = _customerFacade.GetCallInfoByCallId(this.CallId);

                    var lstCustomer = _customerFacade.GetCustomerIdWithCallId(callInfo.PhoneNo);
                    var recordFound = (lstCustomer == null) ? 0 : lstCustomer.Count;

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
                        //var customerId = lstCustomer.First();
                        //if (customerId != null)
                        //{
                        //    return InitCustomerNote(customerId.Value , 0, custVM);
                        //}
                    }
                    else
                    {
                        custVM.SearchFilter.PhoneNo = callInfo.PhoneNo;
                        custVM = SearchCustomerList(custVM.SearchFilter); //.CustomerList = _customerFacade.GetCustomerList(custVM.SearchFilter);
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
            IEnumerable<CustomerEntity> acList = new List<CustomerEntity>(); //เลขที่สัญญา 
            IEnumerable<CustomerEntity> dbList = new List<CustomerEntity>(); //DB

            WebServicePagingEntity vPaging = new WebServicePagingEntity();
            vPaging.PageNumber = searchFilter.PageNo;
            vPaging.PageSize = searchFilter.PageSize;

            //Search from Webservice CustomerByInformation
            if (custVM.SearchFilter.FirstName != null || custVM.SearchFilter.LastName != null || custVM.SearchFilter.PhoneNo != null || custVM.SearchFilter.CardNo != null)
            {
                wsList = getCustomerByInformationWS(custVM.SearchFilter, vPaging);

                //Search from CSM DB
                dbList = _customerFacade.SearchCustomer(custVM.SearchFilter);
            }

            //Search from Webservice CustomerByAccount
            //Sample Data AccountNumber 310011692320300000
            if (custVM.SearchFilter.AccountNo != null)
            {
                if (custVM.SearchFilter.Product == Constants.CustomerProduct.Loan || custVM.SearchFilter.Product == Constants.CustomerProduct.Funding)
                {
                    acList = getCustomerByLoanFundingAccountWS(searchFilter.AccountNo, vPaging);
                }
                else if (custVM.SearchFilter.Product == Constants.CustomerProduct.HP)
                {
                    acList = InquiryHPCustomerContractListWS(searchFilter.AccountNo,"","","","","", vPaging);
                }
                else if (custVM.SearchFilter.Product == Constants.CustomerProduct.Insurance)
                {
                    acList = getCustomerByInsuranceAccountWS(searchFilter.AccountNo, vPaging);
                }
                else if (custVM.SearchFilter.Product == Constants.ApplicationStatus.All.ToString())
                {
                    acList = getCustomerByLoanFundingAccountWS(searchFilter.AccountNo, vPaging);
                    acList = acList.Union(InquiryHPCustomerContractListWS(searchFilter.AccountNo, "",searchFilter.CustomerDeptFirstName + " " + searchFilter.CustomerDeptLastName,"", searchFilter.CardNo,searchFilter.Registration, vPaging));
                    acList = acList.Union(getCustomerByInsuranceAccountWS(searchFilter.AccountNo, vPaging));                   
                }
            }

            //Search from Webservice HPCustomerContractList
            if (searchFilter.Registration != null || searchFilter.CustomerDeptFirstName != null || searchFilter.CustomerDeptLastName != null)
            {
                acList = acList.Union(InquiryHPCustomerContractListWS(searchFilter.AccountNo, "", searchFilter.CustomerDeptFirstName + " " + searchFilter.CustomerDeptLastName, "", searchFilter.CardNo, searchFilter.Registration, vPaging));
            }

            custVM.CustomerList = wsList.Union(acList).Union(dbList);
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

                if (ModelState.IsValid)
                {
                    CustomerViewModel custVM = SearchCustomerList(searchFilter);

                    if (custVM.CustomerList.Count() > 0) {
                        if (custVM.CustomerList.Where(a => a.CIF_ID == null && a.CustomerId == null && a.CustomerType==Constants.CustomerType.Customer).Any())
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
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Customer").ToSuccessLogString());
                    return PartialView("~/Views/Customer/_CustomerList.cshtml", custVM);
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
                    custVM.SubscriptType = customerEntity.SubscriptType != null ? customerEntity.SubscriptType.SubscriptTypeId.ConvertToString() : "";
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
                custVM.SubscriptTypeList = new SelectList((IEnumerable)_commonFacade.GetSubscriptTypeSelectList(), "Key", "Value", string.Empty);
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

                if (!string.IsNullOrEmpty(customerVM.SubscriptType))
                {
                    _commonFacade = new CommonFacade();
                    var subscriptTypePersonal = _commonFacade.GetSubscriptTypeByCode(Constants.SubscriptTypeCode.Personal);

                    if (string.IsNullOrEmpty(customerVM.CardNo))
                    {
                        ModelState.AddModelError("CardNo", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_RequiredField, Resource.Lbl_CardNo_Passport));
                    }
                    else if (customerVM.SubscriptType.ToNullable<int>() == subscriptTypePersonal.SubscriptTypeId)
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

                    #region "Check Duplicate CardNo"
                    if (!string.IsNullOrEmpty(customerVM.SubscriptType))
                    {
                        if (_customerFacade.IsDuplicateCardNo(customerVM.CustomerId,
                            customerVM.SubscriptType.ToNullable<int>(), customerVM.CardNo))
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
                            customerVM.CustomerList = _customerFacade.GetCustomerByPhoneNo(customerVM.CustomerId, lstPhoneNo);

                            if (customerVM.CustomerList != null && customerVM.CustomerList.Any())
                            {
                                customerVM.IsSubmit = "1";
                                TempData["CustomerVM"] = customerVM;
                                return InitEditCustomer();
                            }
                        }
                        else
                        {
                            customerVM.CustomerList = _customerFacade.GetCustomerByName(customerVM.FirstNameThai);
                            if (customerVM.CustomerList != null && customerVM.CustomerList.Any())
                            {
                                customerVM.IsSubmit = "1";
                                TempData["CustomerVM"] = customerVM;
                                return InitEditCustomer();
                            }
                        }

                        #endregion
                    }

                    // Save
                    bool isSuccess = SaveCustomer(customerVM);
                    if (isSuccess)
                    {
                        //TempData["CustomerId"] = customerVM.CustomerId;
                        string encryptedstring = StringCipher.Encrypt(customerVM.CustomerId.ConvertToString(), Constants.PassPhrase);
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
                SubscriptType = new SubscriptTypeEntity
                {
                    SubscriptTypeId = customerVM.SubscriptType.ToNullable<int>()
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

                var customerProductList = _commonFacade.GetCustomerProductSelectList(Resource.Ddl_CustomerProduct_All, Constants.ApplicationStatus.All);
                custVM.CustomerProductList = new SelectList((IEnumerable)customerProductList, "Key", "Value", string.Empty);

                var customerTypeList = _commonFacade.GetCustomerTypeSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
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

                    ViewBag.PageSize = custVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Notes").ToSuccessLogString());
                    return PartialView("~/Views/Customer/_AdminNoteList.cshtml", custVM);
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
        public ActionResult AdminNote(int customerId, decimal customerNumber, string CustomerType, string BirthDate, string IDNumber, string SubscriptTypeCode, string SubscriptTypeName, string CountryOfCitizenship, string Title, string PrimaryName, string PrimaryLastName, string AlternateTitle, string AlternateFirstName, string AlternateLastName)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Admin Notes").Add("CustomerId", customerId).ToInputLogString());

            try
            {
                CustomerViewModel custVM = new CustomerViewModel();

                //Get Customer Profile From Webservice
                custVM.CustomerId = customerId;
                custVM.CustomerNumber = customerNumber;

                custVM.CustomerInfo = new CustomerInfoViewModel();
                //custVM.CustomerInfo.Account = customerEntity.Account;
                //custVM.CustomerInfo.AccountNo = customerEntity.AccountNo;
                if (BirthDate.Equals("") == false)
                    custVM.CustomerInfo.BirthDate = Convert.ToDateTime(BirthDate);
                else
                    custVM.CustomerInfo.BirthDate = null;

                custVM.CustomerInfo.SubscriptType = new SubscriptTypeEntity { SubscriptTypeCode = SubscriptTypeCode, SubscriptTypeName = SubscriptTypeName };
                custVM.CustomerInfo.CardNo = IDNumber;
                custVM.CustomerInfo.CountryOfCitizenship = CountryOfCitizenship;
                custVM.CustomerInfo.TitleThai = new TitleEntity { TitleName = Title };
                custVM.CustomerInfo.TitleEnglish = new TitleEntity { TitleName = AlternateTitle };
                custVM.CustomerInfo.FirstNameThai = PrimaryName;
                custVM.CustomerInfo.LastNameThai = PrimaryLastName;
                custVM.CustomerInfo.FirstNameEnglish = AlternateFirstName;
                custVM.CustomerInfo.LastNameEnglish = AlternateLastName;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerNote)]
        public ActionResult InitCustomerNote(int customerId,decimal customerNumber,string CustomerType, string BirthDate, string IDNumber, string SubscriptTypeCode, string SubscriptTypeName, string CountryOfCitizenship, string Title, string PrimaryName, string PrimaryLastName, string AlternateTitle, string AlternateFirstName, string AlternateLastName)
        {
            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Customer Notes").Add("CustomerId", customerId).ToInputLogString());

                _commonFacade = new CommonFacade(); 
                _customerFacade = new CustomerFacade();
                CustomerViewModel custVM = new CustomerViewModel();
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

                custVM.CustomerInfo.SubscriptType = new SubscriptTypeEntity { SubscriptTypeCode = SubscriptTypeCode, SubscriptTypeName = SubscriptTypeName };
                custVM.CustomerInfo.CardNo = IDNumber;
                custVM.CustomerInfo.CountryOfCitizenship = CountryOfCitizenship;
                custVM.CustomerInfo.TitleThai = new TitleEntity { TitleName = Title };
                custVM.CustomerInfo.TitleEnglish = new TitleEntity { TitleName = AlternateTitle };
                custVM.CustomerInfo.FirstNameThai = PrimaryName;
                custVM.CustomerInfo.LastNameThai = PrimaryLastName;
                custVM.CustomerInfo.FirstNameEnglish = AlternateFirstName;
                custVM.CustomerInfo.LastNameEnglish = AlternateLastName;

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

        public IEnumerable<CustomerEntity> InquiryHPCustomerContractListWS(string AccountNo,string AccountName, string CustomerDeptName, string CustomerType, string IDCardNo, string CarNo , WebServicePagingEntity vPaging)
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
                pageSize = vPaging.PageSize.ToString()
            };

            req.carLicensePlate = CarNo;
            req.cardID = IDCardNo;
            req.contractName = AccountName;
            req.contractNo = AccountNo;
            req.custName = CustomerDeptName;
            req.custType = CustomerType;

            Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryHPCustomerContractListWS").Add("ReferenceNo:", req.header.referenceNo).Add("AccountNo", AccountNo).Add("ContactName", AccountName).Add("CustomerDeptName", CustomerDeptName).Add("CardNo", IDCardNo).Add("CarLicensePlate", CarNo));

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
                                  FirstNameThai = c.firstName,
                                  //BirthDate = c.cu,
                                  TitleThai = new TitleEntity { TitleName = c.titleName },
                                  //TitleEnglish = new TitleEntity { TitleName = "MR." },
                                  //FirstNameEnglish = "FirstName",
                                  LastNameThai = c.lastName,
                                  //LastNameEnglish = "LastName",
                                  FirstNameThaiEng = c.firstName,
                                  LastNameThaiEng = c.lastName,
                                  CardNo = c.customerIdNo,
                                  IDTypeCode = c.customerCardTypeDescription,
                                  SubscriptType = new SubscriptTypeEntity
                                  {
                                      SubscriptTypeCode = c.customerCardType,
                                      SubscriptTypeName = c.customerCardTypeDescription
                                  },
                                  CustomerType = Constants.CustomerType.Customer,
                                  //CustomerCategory = c.customerCardType,
                                  Registration = c.licenseNo + " " + c.licenseProvince,
                                  CountryOfCitizenship = c.customerCardCountryIssue,
                              }).AsQueryable();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryHPCustomerContractListWS").Add("ReferenceNo:", req.header.referenceNo).SetSuffixMsg("Found data " + res.inquiryCustomerContractListResponse1.Length.ToString() + " Row(s)"));
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryHPCustomerContractListWS").Add("ReferenceNo:", req.header.referenceNo).SetSuffixMsg("No data found"));
                }
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

                //req.Header = new InquiryCustomerByInformationHeader
                //{
                //    ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
                //    TransactionDateTime = DateTime.Now,
                //    ServiceName = Constants.ServiceName.InquiryCustomerByInformation,
                //    SystemCode = profile.system_code,
                //    ChannelID = profile.channel_id
                //};

                //Logger.Info(_logMsg.Clear().SetPrefixMsg().Add("ReferenceNo:", req.Header.ReferenceNo).Add("FirstName", searchFileter.FirstName).Add("LastName", searchFileter.LastName).Add("CardNo", searchFileter.CardNo).Add("PhoneNo", searchFileter.PhoneNo));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getCustomerByHPAccountWS").Add("Error Message", ex.Message).ToFailLogString());
            }

            return result;
        }

        public IEnumerable<CustomerEntity> getCustomerByLoanFundingAccountWS(string AccountNo, WebServicePagingEntity vPaging) {
        CBSCustomerService ws = new CBSCustomerService();
        //ws.Proxy = getWebProxy();

        IEnumerable<CustomerEntity> result = new List<CustomerEntity>();
        try {
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
                    PageNumber = vPaging.PageNumber,
                    PageSize = vPaging.PageSize
                };

                InquiryCustomerByAccountResponse res = new InquiryCustomerByAccountResponse();
                res = ws.InquiryCustomerByAccount(req);
                if (res.Header.CustomerList != null)
                {
                    result = (from c in res.Header.CustomerList
                              select new CustomerEntity
                              {
                                  CustomerId = 0,
                                  CIF_ID = Convert.ToDecimal(c.CustomerNumber),
                                  CustomerNumber = Convert.ToDecimal(c.CustomerNumber),
                                  FirstNameThai = c.CustomerName,
                                  LastNameThai = "",
                                  FirstNameEnglish = c.AlternateName,
                                  LastNameEnglish = "",
                                  FirstNameThaiEng = !string.IsNullOrEmpty(c.CustomerName) ? c.CustomerName : c.AlternateName,
                                  LastNameThaiEng = "",
                                  BirthDate = c.BirthDate,
                                  TitleThai = new TitleEntity { TitleName = "" },
                                  TitleEnglish = new TitleEntity { TitleName = "" },
                                  CardNo = c.IDNumber,
                                  IDTypeCode = c.IDTypeCode,
                                  SubscriptType = new SubscriptTypeEntity { SubscriptTypeCode = c.IDTypeCode, SubscriptTypeName = c.IDTypeCode },
                                  CountryOfCitizenship = c.CountryOfCitizenship,
                                  CustomerType = Constants.CustomerType.Customer,
                                  CustomerCategory = c.CustomerCategory,
                                  EmployeeCode = c.OfficerCode,
                                  EmployeeName = c.OfficerShortName,
                                  AccountNo = c.AccountNumber,
                                  Account = new AccountEntity { AccountNo = c.AccountNumber, AccountType = c.AccountType }
                              }).AsQueryable();
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByAccount").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("Found data " + result.Count<CustomerEntity>().ToString() + " Row(s)"));
                }
                else {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByAccount").Add("ReferenceNo:", req.Header.ReferenceNo).SetSuffixMsg("No data found"));
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("getCustomerByLoanFundingAccountWS").Add("Error Message", ex.Message).ToFailLogString());
            }
            return result;
        }

        public IEnumerable<CustomerEntity> getCustomerByInformationWS(CustomerSearchFilter searchFileter, WebServicePagingEntity vPaging)
        {
            CBSCustomerService ws = new CBSCustomerService();
            //ws.Proxy = getWebProxy();
            IEnumerable<CustomerEntity> result = new List<CustomerEntity>();
            try
            {
                InquiryCustomerByInformation req = new InquiryCustomerByInformation();

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
                    if (searchFirstName.StartsWith("*") || searchFirstName.EndsWith("*"))
                    {
                        req.InquiryCustomerByInformationRequest.FirstName = searchFirstName.Replace("*", "");
                        req.InquiryCustomerByInformationRequest.ExactSearchFirstNameFlag = "N";
                    }
                    else
                    {
                        req.InquiryCustomerByInformationRequest.ExactSearchFirstNameFlag = "Y";
                    }
                }
               
                if (searchLastName != "")
                {
                    req.InquiryCustomerByInformationRequest.LastName = searchLastName;
                    if (searchLastName.StartsWith("*") || searchLastName.EndsWith("*"))
                    {
                        req.InquiryCustomerByInformationRequest.LastName = searchLastName.Replace("*", "");
                        req.InquiryCustomerByInformationRequest.ExactSearchLastNameFlag = "N";
                    }
                    else
                    {
                        req.InquiryCustomerByInformationRequest.ExactSearchLastNameFlag = "Y";
                    }
                }
                
                if (string.IsNullOrEmpty(searchFileter.CardNo) == false) {
                    req.InquiryCustomerByInformationRequest.IDNumber =  searchFileter.CardNo;
                }

                if (string.IsNullOrEmpty(searchFileter.PhoneNo) == false)
                {
                    req.InquiryCustomerByInformationRequest.ElectronicAddress = string.IsNullOrEmpty(searchFileter.PhoneNo) ? "" : searchFileter.PhoneNo;
                }

                req.Paging = new InquiryCustomerByInformationPaging();
                req.Paging.PageNumber = vPaging.PageNumber;
                req.Paging.PageSize = vPaging.PageSize;
                
                InquiryCustomerByInformationResponse res = new InquiryCustomerByInformationResponse();
                res = ws.InquiryCustomerByInformation(req);
                if (res.Header.CustomerList != null)
                {
                    result = (from c in res.Header.CustomerList
                              select new CustomerEntity
                              {
                                  CustomerId = 0,
                                  CIF_ID = Convert.ToDecimal(c.CustomerNumber),
                                  CustomerNumber = Convert.ToDecimal(c.CustomerNumber),
                                  FirstNameThai = c.PrimaryName,
                                  FirstNameEnglish = c.AlternateFirstName,
                                  LastNameThai = c.PrimaryLastName,
                                  LastNameEnglish = c.AlternateLastName,
                                  FirstNameThaiEng = !string.IsNullOrEmpty(c.PrimaryName) ? c.PrimaryName : c.AlternateFirstName,
                                  LastNameThaiEng = !string.IsNullOrEmpty(c.PrimaryLastName) ? c.PrimaryLastName : c.AlternateLastName,
                                  BirthDate = c.BirthDate,
                                  TitleThai = _commonFacade.GetTitleByCode(c.Title, Constants.TitleLanguage.TitleTh),  //new TitleEntity { TitleName = c.TitleCode },
                                  TitleEnglish = _commonFacade.GetTitleByCode(c.AlternateTitle, Constants.TitleLanguage.TitleEn),
                                  CardNo = c.IDNumber,
                                  IDTypeCode = c.IDTypeCode,
                                  SubscriptType = _commonFacade.GetSubscriptTypeByTypeCode(c.IDTypeCode), // new SubscriptTypeEntity { SubscriptTypeCode = c.IDTypeCode, SubscriptTypeName = c.IDTypeCode },
                                  CountryOfCitizenship = c.CountryOfCitizenship,
                                  CustomerType = Constants.CustomerType.Customer,
                                  CustomerCategory = _commonFacade.GetCbsCustomerCategory(c.CustomerCategory).LongDescThai,
                                  EmployeeCode = c.OfficerCode,
                                  EmployeeName = c.OfficerShortName,

                              }).AsQueryable();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByInformation").Add("ReferenceNo:", req.Header.ReferenceNo).Add("Found Data", result.Count<CustomerEntity>() + " Row(s)"));
                } else {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByInformation").Add("ReferenceNo:", req.Header.ReferenceNo).Add("Not Found Data", ""));
                }
            }
            catch (Exception ex) {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByInformation").Add("Error Message", ex.Message).ToFailLogString());
            }

            return result;
        }

        
        #endregion
    }
}
