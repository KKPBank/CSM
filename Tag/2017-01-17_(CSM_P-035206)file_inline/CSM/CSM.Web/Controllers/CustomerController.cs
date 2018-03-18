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

                var customerTypeList = _commonFacade.GetCustomerTypeSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                custVM.CustomerTypeList = new SelectList((IEnumerable)customerTypeList, "Key", "Value", string.Empty);

                var statusList = _commonFacade.GetStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                custVM.StatusList = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

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
                        var customerId = lstCustomer.First();
                        if (customerId != null)
                        {
                            return InitCustomerNote(customerId.Value);
                        }
                    }
                    else
                    {
                        custVM.SearchFilter.PhoneNo = callInfo.PhoneNo;
                        custVM.CustomerList = _customerFacade.GetCustomerList(custVM.SearchFilter);
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

                if (!string.IsNullOrWhiteSpace(searchFilter.Registration) &&
                    searchFilter.Registration.ExtractString().Length < Constants.MinLenght.SearchTerm)
                {
                    ModelState["Registration"].Errors.Clear();
                    ModelState["Registration"].Errors.Add(string.Format(CultureInfo.InvariantCulture, Resource.ValErr_MinLength, Constants.MinLenght.SearchTerm));
                }

                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _customerFacade = new CustomerFacade();
                    CustomerViewModel custVM = new CustomerViewModel();
                    custVM.SearchFilter = searchFilter;

                    custVM.CustomerList = _customerFacade.GetCustomerList(custVM.SearchFilter);
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

                ViewBag.PageSize = custVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View(custVM);
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
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Notes").Add("FirstName", searchFilter.FirstName)
                .ToInputLogString());

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
                    _commonFacade = new CommonFacade();
                    _customerFacade = new CustomerFacade();

                    CustomerViewModel custVM = new CustomerViewModel();
                    custVM.SearchFilter = searchFilter;

                    custVM.CustomerList = _customerFacade.GetCustomerList(custVM.SearchFilter);
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
        public ActionResult AdminNote(int? customerId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Admin Notes").Add("CustomerId", customerId).ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();
                CustomerViewModel custVM = new CustomerViewModel();

                if (customerId.HasValue)
                {
                    CustomerEntity customerEntity = _customerFacade.GetCustomerByID(customerId.Value);
                    custVM.CustomerInfo = new CustomerInfoViewModel();
                    custVM.CustomerInfo.Account = customerEntity.Account;
                    custVM.CustomerInfo.AccountNo = customerEntity.AccountNo;
                    custVM.CustomerInfo.BirthDate = customerEntity.BirthDate;
                    custVM.CustomerInfo.CardNo = customerEntity.CardNo;
                    custVM.CustomerInfo.CreateUser = customerEntity.CreateUser;
                    custVM.CustomerInfo.CustomerId = customerEntity.CustomerId;
                    custVM.CustomerInfo.CustomerType = customerEntity.CustomerType;
                    custVM.CustomerInfo.Email = customerEntity.Email;
                    custVM.CustomerInfo.Fax = customerEntity.Fax;
                    custVM.CustomerInfo.FirstNameEnglish = customerEntity.FirstNameEnglish;
                    custVM.CustomerInfo.FirstNameThai = customerEntity.FirstNameThai;
                    custVM.CustomerInfo.LastNameEnglish = customerEntity.LastNameEnglish;
                    custVM.CustomerInfo.LastNameThai = customerEntity.LastNameThai;
                    custVM.CustomerInfo.FirstNameThaiEng = customerEntity.FirstNameThaiEng;
                    custVM.CustomerInfo.LastNameThaiEng = customerEntity.LastNameThaiEng;
                    custVM.CustomerInfo.PhoneList = customerEntity.PhoneList;
                    custVM.CustomerInfo.Registration = customerEntity.Registration;
                    custVM.CustomerInfo.SubscriptType = customerEntity.SubscriptType;
                    custVM.CustomerInfo.TitleEnglish = customerEntity.TitleEnglish;
                    custVM.CustomerInfo.TitleThai = customerEntity.TitleThai;
                    custVM.CustomerInfo.UpdateUser = customerEntity.UpdateUser;

                    // Note list
                    custVM.NoteSearchFilter = new NoteSearchFilter
                    {
                        CustomerId = customerId.Value,
                        PageNo = 1,
                        PageSize = _commonFacade.GetPageSizeStart(),
                        SortField = "UpdateDate",
                        SortOrder = "DESC"
                    };

                    custVM.NoteList = _customerFacade.GetNoteList(custVM.NoteSearchFilter);
                    ViewBag.PageSize = custVM.NoteSearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                    ViewBag.Message = string.Empty;
                }

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
        public ActionResult InitEditNote(int? noteId, int? customerId)
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
        public ActionResult InitCustomerNote(int? customerId = null)
        {
            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Customer Notes").Add("CustomerId", customerId).ToInputLogString());

                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();
                CustomerViewModel custVM = new CustomerViewModel();

                if (customerId.HasValue)
                {
                    custVM.CustomerInfo = this.MappingCustomerInfoView(customerId.Value);
                    //TempData["CustomerInfo"] = custVM.CustomerInfo;

                    // Note list
                    custVM.NoteSearchFilter = new NoteSearchFilter
                    {
                        CustomerId = customerId.Value,
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

        [HttpGet]
        [CheckUserRole(ScreenCode.ViewCustomerNote)]
        public ActionResult CustomerNote(string encryptedString)
        {
            try
            {
                // Redirect มาจากหน้า Edit Customer
                //if (TempData["CustomerId"] != null)
                //{
                //    customerId = (int)TempData["CustomerId"];
                //}
                //else if (!customerId.HasValue && TempData["CustomerInfo"] != null) // Redirect มาจากหน้า CustomerTab
                //{
                //    customerId = ((CustomerInfoViewModel)TempData["CustomerInfo"]).CustomerId;
                //}

                int? customerId = encryptedString.ToCustomerId();
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Customer Notes").Add("CustomerId", customerId).ToInputLogString());

                if(customerId == null) {
                    return RedirectToAction("Search", "Customer");
                }

                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();
                CustomerViewModel custVM = new CustomerViewModel();

                if (customerId.HasValue)
                {
                    // CustomerInfo                    
                    custVM.CustomerInfo = this.MappingCustomerInfoView(customerId.Value);
                    //TempData["CustomerInfo"] = custVM.CustomerInfo;

                    // Note list
                    custVM.NoteSearchFilter = new NoteSearchFilter
                    {
                        CustomerId = customerId.Value,
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
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search CustomerNotes").Add("CustomerId", searchFilter.CustomerId)
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
    }
}
