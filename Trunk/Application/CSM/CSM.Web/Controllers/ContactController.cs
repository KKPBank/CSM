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
using Newtonsoft.Json;
using System.Globalization;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class ContactController : BaseController
    {
        private ICustomerFacade _customerFacade;
        private ICommonFacade _commonFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ContactController));

        [CheckUserRole(ScreenCode.SearchContactRlat)]
        public ActionResult Search()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Contact").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                ContactViewModel contactVM = new ContactViewModel();

                contactVM.SearchFilter = new RelationshipSearchFilter
                {
                    Status = null,
                    RelationshipName = string.Empty,
                    RelationshipDesc = string.Empty,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "RelationshipId",
                    SortOrder = "DESC"
                };

                var statusList = _commonFacade.GetStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                contactVM.StatusList = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                ViewBag.PageSize = contactVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;
                return View(contactVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Contact").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.SearchContactRlat)]
        public ActionResult ContactList(RelationshipSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Contact").Add("Status", searchFilter.Status)
                .Add("RelationshipName", searchFilter.RelationshipName).Add("RelationshipDesc", searchFilter.RelationshipDesc));
            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _customerFacade = new CustomerFacade();
                    ContactViewModel contactVM = new ContactViewModel();
                    contactVM.SearchFilter = searchFilter;

                    contactVM.RelationshipList = _customerFacade.GetAllRelationships(searchFilter);
                    ViewBag.PageSize = contactVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    return PartialView("~/Views/Contact/_ContactList.cshtml", contactVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Contact").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ManageContactRlat)]
        public ActionResult InitEdit(int? relationshipId = null)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("IntEdit ContactRelationship").Add("RelationshipId", relationshipId).ToInputLogString());

            try
            {
                ContactViewModel contactVM = null;

                if (TempData["contactVM"] != null)
                {
                    contactVM = (ContactViewModel)TempData["contactVM"];
                }
                else
                {
                    contactVM = new ContactViewModel { RelationshipId = relationshipId };
                }

                _commonFacade = new CommonFacade();
                var statusList = _commonFacade.GetStatusSelectList();
                contactVM.StatusList = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                if (contactVM.RelationshipId != null)
                {
                    _customerFacade = new CustomerFacade();
                    RelationshipEntity relationshipEntity =
                        _customerFacade.GetRelationshipByID(contactVM.RelationshipId.Value);

                    contactVM.RelationshipId = relationshipEntity.RelationshipId;
                    contactVM.RelationshipName = relationshipEntity.RelationshipName;
                    contactVM.RelationshipDesc = relationshipEntity.RelationshipDesc;
                    contactVM.Status = relationshipEntity.Status;
                    contactVM.CreateUser = relationshipEntity.CreateUserDisplay;
                    contactVM.UpdateUser = relationshipEntity.UpdateUserDisplay;
                    contactVM.CreatedDate =
                        relationshipEntity.CreatedDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                    contactVM.UpdateDate =
                        relationshipEntity.Updatedate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                }
                else
                {
                    // default UserLogin
                    if (this.UserInfo != null)
                    {
                        var today = DateTime.Now;
                        contactVM.CreateUser = this.UserInfo.FullName;
                        contactVM.CreatedDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                        contactVM.UpdateDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                        contactVM.UpdateUser = this.UserInfo.FullName;
                    }
                }

                return View("~/Views/Contact/Edit.cshtml", contactVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("IntEdit ContactRelationship").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ManageContactRlat)]
        public ActionResult Edit(ContactViewModel contactVM) // Mater Relationship
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Save ContactRelationship").Add("Status", contactVM.Status)
                .Add("RelationshipName", contactVM.RelationshipName).Add("RelationshipDesc", contactVM.RelationshipDesc).ToInputLogString());
            try
            {
                if (ModelState.IsValid)
                {
                    RelationshipEntity relationEntity = new RelationshipEntity
                    {
                        RelationshipId = contactVM.RelationshipId.HasValue ? contactVM.RelationshipId.Value : 0,
                        Status = contactVM.Status,
                        RelationshipName = contactVM.RelationshipName,
                        RelationshipDesc = contactVM.RelationshipDesc,
                        CreateUser = UserInfo, //When save the progame will select to save this parameter
                        UpdateUser = UserInfo
                    };

                    _customerFacade = new CustomerFacade();
                    // Check Duplicate
                    if (_customerFacade.IsDuplicateRelationship(relationEntity) == true)
                    {
                        ViewBag.ErrorMessage = Resource.Error_SaveFailedDuplicate;
                        goto Outer;
                    }

                    bool success = _customerFacade.SaveContactRelation(relationEntity);
                    if (success)
                    {
                        return RedirectToAction("Search", "Contact");
                    }
                    ViewBag.ErrorMessage = Resource.Error_SaveFailed;
                }

            Outer:
                TempData["contactVM"] = contactVM;
                return InitEdit();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save ContactRelationship").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        #region "Use in Tab"

        [CheckUserRole(ScreenCode.ViewCustomerContact)]
        public ActionResult List(string encryptedString)
        {
            int? customerId = encryptedString.ToCustomerId();
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Contact").ToInputLogString());

            try
            {
                if (customerId == null)
                {
                    return RedirectToAction("Search", "Customer");
                }

                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Contact").Add("CustomerId", customerId).ToInputLogString());

                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();
                CustomerContactViewModel contactVM = new CustomerContactViewModel();
                CustomerInfoViewModel custInfoVM = this.MappingCustomerInfoView(customerId.Value);
                contactVM.CustomerInfo = custInfoVM;

                if (custInfoVM.CustomerId.HasValue)
                {
                    // Contact list
                    contactVM.SearchFilter = new ContactSearchFilter
                    {
                        CustomerId = custInfoVM.CustomerId.Value,
                        PageNo = 1,
                        PageSize = _commonFacade.GetPageSizeStart(),
                        SortField = "CardNo",
                        SortOrder = "ASC"
                    };

                    contactVM.ContactList = _customerFacade.GetContactList(contactVM.SearchFilter);
                    ViewBag.PageSize = contactVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                    ViewBag.Message = string.Empty;
                }

                return View(contactVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Contact").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerContact)]
        public ActionResult CustomerContactList(ContactSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search CustomerContactList").Add("CustomerId", searchFilter.CustomerId).ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _customerFacade = new CustomerFacade();
                    CustomerContactViewModel contactVM = new CustomerContactViewModel();
                    contactVM.SearchFilter = searchFilter;

                    contactVM.ContactList = _customerFacade.GetContactList(contactVM.SearchFilter);
                    ViewBag.PageSize = contactVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("CustomerContactList").ToSuccessLogString());
                    return PartialView("~/Views/Contact/_CustomerContactList.cshtml", contactVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search CustomerContactList").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerContact)]
        public JsonResult DeleteCustomerContact(int customerContactId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Contact").Add("ContactId", customerContactId).ToInputLogString());

            try
            {
                _customerFacade = new CustomerFacade();
                bool isSuccess = _customerFacade.DeleteCustomerContact(customerContactId, this.UserInfo.UserId);

                if (isSuccess)
                {
                    return Json(new
                    {
                        Valid = true
                    });
                }

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Contact").ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = "ไม่สามารถลบข้อมูลได้ เนื่องจากมีการใช้งานข้อมูลอยู่",
                    Errors = string.Empty
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Contact").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }

        #endregion

        #region "PopupEdit CustomerContact"

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerContact)]
        public ActionResult InitEditCustomerContact(int? contactId = null, int? customerId = null)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit CustomerContact").Add("ContactId", contactId)
                .Add("CustomerId", customerId).ToInputLogString());

            try
            {
                ContactEditViewModel contactEditVM = new ContactEditViewModel();

                _customerFacade = new CustomerFacade();

                if (contactId.HasValue && customerId.HasValue)
                {
                    var contactEntity = _customerFacade.GetContactByID(contactId.Value);
                    contactEditVM.ContactId = contactEntity.ContactId;
                    contactEditVM.SubscriptType = contactEntity.SubscriptType != null ? contactEntity.SubscriptType.SubscriptTypeId.ConvertToString() : string.Empty;
                    contactEditVM.TitleThai = contactEntity.TitleThai != null ? contactEntity.TitleThai.TitleId.ConvertToString() : string.Empty;
                    contactEditVM.TitleEnglish = contactEntity.TitleEnglish != null ? contactEntity.TitleEnglish.TitleId.ConvertToString() : string.Empty;
                    contactEditVM.FirstNameThai = contactEntity.FirstNameThai;
                    contactEditVM.LastNameThai = contactEntity.LastNameThai;
                    contactEditVM.TitleEnglish = contactEntity.TitleEnglish != null ? contactEntity.TitleEnglish.TitleId.ConvertToString() : string.Empty;
                    contactEditVM.FirstNameEnglish = contactEntity.FirstNameEnglish;
                    contactEditVM.LastNameEnglish = contactEntity.LastNameEnglish;
                    contactEditVM.CardNo = contactEntity.CardNo;
                    contactEditVM.BirthDate = contactEntity.BirthDateDisplay;
                    contactEditVM.Email = contactEntity.Email;
                    contactEditVM.Fax = contactEntity.Fax;

                    #region "Phone"

                    // Phone
                    if (contactEntity.PhoneList != null)
                    {
                        if (contactEntity.PhoneList.Count > 0)
                        {
                            contactEditVM.PhoneType1 = contactEntity.PhoneList[0].PhoneTypeId.ConvertToString();
                            contactEditVM.PhoneNo1 = contactEntity.PhoneList[0].PhoneNo;
                        }
                        else
                        {
                            contactEditVM.PhoneType1 = string.Empty;
                            contactEditVM.PhoneNo1 = string.Empty;
                        }

                        if (contactEntity.PhoneList.Count > 1)
                        {
                            contactEditVM.PhoneType2 = contactEntity.PhoneList[1].PhoneTypeId.ConvertToString();
                            contactEditVM.PhoneNo2 = contactEntity.PhoneList[1].PhoneNo;
                        }
                        else
                        {
                            contactEditVM.PhoneType2 = string.Empty;
                            contactEditVM.PhoneNo2 = string.Empty;
                        }

                        if (contactEntity.PhoneList.Count > 2)
                        {
                            contactEditVM.PhoneType3 = contactEntity.PhoneList[2].PhoneTypeId.ConvertToString();
                            contactEditVM.PhoneNo3 = contactEntity.PhoneList[2].PhoneNo;
                        }
                        else
                        {
                            contactEditVM.PhoneType3 = string.Empty;
                            contactEditVM.PhoneNo3 = string.Empty;
                        }
                    }

                    #endregion

                    contactEditVM.IsEdit = contactEntity.IsEdit.Value ? "1" : "0";
                    contactEditVM.CustomerId = customerId; // keep CustomerId

                    contactEditVM.IsConfirm = "1";

                    var contactRelationships = _customerFacade.GetContactRelationshipList(contactId.Value, customerId.Value);
                    contactEditVM.ContactRelationshipList = contactRelationships;
                    contactEditVM.JsonContactRelationship = JsonConvert.SerializeObject(contactRelationships);
                }
                else
                {
                    contactEditVM.IsEdit = "1"; // case New
                }

                // Get SelectList
                _commonFacade = new CommonFacade();
                contactEditVM.SubscriptTypeList = new SelectList((IEnumerable)_commonFacade.GetSubscriptTypeSelectList(), "Key", "Value", string.Empty);
                contactEditVM.TitleThaiList = new SelectList((IEnumerable)_commonFacade.GetTitleThaiSelectList(), "Key", "Value", string.Empty);
                contactEditVM.TitleEnglishList = new SelectList((IEnumerable)_commonFacade.GetTitleEnglishSelectList(), "Key", "Value", string.Empty);
                contactEditVM.PhoneTypeList = new SelectList((IEnumerable)_commonFacade.GetPhoneTypeSelectList(), "Key", "Value", string.Empty);

                return PartialView("~/Views/Contact/_EditCustomerContact.cshtml", contactEditVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit CustomerContact").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerContact)]
        public ActionResult ManageContactRlat(ContactEditViewModel contactEditVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Contact").Add("FirstNameThai", contactEditVM.FirstNameThai).ToInputLogString());

            try
            {
                #region "Validate CardNo"

                if (!string.IsNullOrEmpty(contactEditVM.SubscriptType))
                {
                    _commonFacade = new CommonFacade();
                    var subscriptTypePersonal =
                        _commonFacade.GetSubscriptTypeByCode(Constants.SubscriptTypeCode.Personal);
                    if (string.IsNullOrEmpty(contactEditVM.CardNo))
                    {
                        ModelState.AddModelError("CardNo", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_RequiredField, Resource.Lbl_CardNo_Passport));
                    }
                    else if (contactEditVM.SubscriptType.ToNullable<int>() == subscriptTypePersonal.SubscriptTypeId)
                    {
                        if (!ApplicationHelpers.ValidateCardNo(contactEditVM.CardNo))
                        {
                            ModelState.AddModelError("CardNo", Resource.ValErr_InvalidCardNo);
                        }
                    }
                }

                #endregion

                #region "Validate FirstName"

                if (string.IsNullOrEmpty(contactEditVM.FirstNameThai) &&
                    string.IsNullOrEmpty(contactEditVM.FirstNameEnglish))
                {
                    ModelState.AddModelError("FirstNameThai", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_RequiredField, Resource.Lbl_FirstNameThai_Contact));
                }


                #endregion

                #region  "Validate Phone"

                if (contactEditVM.IsEdit == "1")
                {
                    if (string.IsNullOrEmpty(contactEditVM.PhoneType2))
                    {
                        ModelState.Remove("PhoneNo2");
                    }
                    if (string.IsNullOrEmpty(contactEditVM.PhoneType3))
                    {
                        ModelState.Remove("PhoneNo3");
                    }

                    // Check duplicate phoneNo
                    if (!string.IsNullOrEmpty(contactEditVM.PhoneNo1) && !string.IsNullOrEmpty(contactEditVM.PhoneNo2))
                    {
                        if (contactEditVM.PhoneNo1.Equals(contactEditVM.PhoneNo2))
                        {
                            ModelState.AddModelError("PhoneNo1", Resource.ValError_DuplicatePhoneNo);
                            ModelState.AddModelError("PhoneNo2", Resource.ValError_DuplicatePhoneNo);
                        }
                    }

                    if (!string.IsNullOrEmpty(contactEditVM.PhoneNo1) && !string.IsNullOrEmpty(contactEditVM.PhoneNo3))
                    {
                        if (contactEditVM.PhoneNo1.Equals(contactEditVM.PhoneNo3))
                        {
                            ModelState.AddModelError("PhoneNo1", Resource.ValError_DuplicatePhoneNo);
                            ModelState.AddModelError("PhoneNo3", Resource.ValError_DuplicatePhoneNo);
                        }
                    }

                    if (!string.IsNullOrEmpty(contactEditVM.PhoneNo2) && !string.IsNullOrEmpty(contactEditVM.PhoneNo3))
                    {
                        if (contactEditVM.PhoneNo2.Equals(contactEditVM.PhoneNo3))
                        {
                            ModelState.AddModelError("PhoneNo2", Resource.ValError_DuplicatePhoneNo);
                            ModelState.AddModelError("PhoneNo3", Resource.ValError_DuplicatePhoneNo);
                        }
                    }

                }
                else
                {
                    // Phone
                    ModelState.Remove("PhoneType1");
                    ModelState.Remove("PhoneNo1");
                    ModelState.Remove("PhoneNo2");
                    ModelState.Remove("PhoneNo3");

                    // Name
                    ModelState.Remove("FirstNameThai");
                    ModelState.Remove("FirstNameEnglish");
                    ModelState.Remove("LastNameThai");
                    ModelState.Remove("LastNameEnglish");
                }

                #endregion

                if (!string.IsNullOrEmpty(contactEditVM.BirthDate) && !contactEditVM.BirthDateValue.HasValue)
                {
                    ModelState.AddModelError("BirthDate", Resource.ValErr_InvalidDate);
                }
                else if (!string.IsNullOrEmpty(contactEditVM.BirthDate) && contactEditVM.BirthDateValue.HasValue)
                {
                    if (contactEditVM.BirthDateValue.Value > DateTime.Now.Date)
                    {
                        ModelState.AddModelError("BirthDate", Resource.ValErr_InvalidDate_MustLessThanToday);
                    }
                }

                if (ModelState.IsValid)
                {
                    _customerFacade = new CustomerFacade();

                    #region "Check Duplicate PhoneNo"

                    List<string> lstPhoneNo = new List<string>();
                    if (!string.IsNullOrEmpty(contactEditVM.PhoneNo1)) lstPhoneNo.Add(contactEditVM.PhoneNo1);
                    if (!string.IsNullOrEmpty(contactEditVM.PhoneNo2)) lstPhoneNo.Add(contactEditVM.PhoneNo2);
                    if (!string.IsNullOrEmpty(contactEditVM.PhoneNo3)) lstPhoneNo.Add(contactEditVM.PhoneNo3);

                    if (contactEditVM.IsEdit != "0")
                    {
                        contactEditVM.ContactList = _customerFacade.GetContactByPhoneNo(contactEditVM.ContactId,
                            contactEditVM.FirstNameThai, contactEditVM.LastNameThai
                            , contactEditVM.FirstNameEnglish, contactEditVM.LastNameEnglish, lstPhoneNo);
                    }

                    if (contactEditVM.ContactList != null && contactEditVM.ContactList.Any())
                    {
                        if (contactEditVM.IsConfirm == "0") // Case New
                        {
                            // Show DuplicateList
                            return PartialView("~/Views/Contact/_ContactDuplicateList.cshtml", contactEditVM);
                        }
                        else // Case Edit
                        {
                            // Alert Duplicate
                            return Json(new
                            {
                                Valid = false,
                                Error = Resource.ValError_DuplicateContact,
                                Errors = string.Empty
                            });
                        }

                    }

                    #endregion

                    ContactEntity contactEntity = new ContactEntity
                    {
                        ContactId = contactEditVM.ContactId,
                        SubscriptType = new SubscriptTypeEntity
                        {
                            SubscriptTypeId = contactEditVM.SubscriptType.ToNullable<int>()
                        },
                        CardNo = contactEditVM.CardNo,
                        BirthDate = contactEditVM.BirthDateValue,
                        TitleThai = new TitleEntity
                        {
                            TitleId = contactEditVM.TitleThai.ToNullable<int>()
                        },
                        FirstNameThai = contactEditVM.FirstNameThai,
                        LastNameThai = contactEditVM.LastNameThai,
                        TitleEnglish = new TitleEntity
                        {
                            TitleId = contactEditVM.TitleEnglish.ToNullable<int>()
                        },
                        FirstNameEnglish = contactEditVM.FirstNameEnglish,
                        LastNameEnglish = contactEditVM.LastNameEnglish,
                        Email = contactEditVM.Email,
                        CreateUser = new UserEntity
                        {
                            UserId = this.UserInfo.UserId
                        },
                        UpdateUser = new UserEntity
                        {
                            UserId = this.UserInfo.UserId
                        },
                        CustomerId = contactEditVM.CustomerId // for add CustomerLog
                    };

                    #region "Phone & Fax"

                    // Phone & Fax
                    contactEntity.PhoneList = new List<PhoneEntity>();
                    if (!string.IsNullOrEmpty(contactEditVM.PhoneNo1))
                    {
                        contactEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = contactEditVM.PhoneType1.ToNullable<int>(), PhoneNo = contactEditVM.PhoneNo1 });
                    }
                    if (!string.IsNullOrEmpty(contactEditVM.PhoneNo2))
                    {
                        contactEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = contactEditVM.PhoneType2.ToNullable<int>(), PhoneNo = contactEditVM.PhoneNo2 });
                    }
                    if (!string.IsNullOrEmpty(contactEditVM.PhoneNo3))
                    {
                        contactEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = contactEditVM.PhoneType3.ToNullable<int>(), PhoneNo = contactEditVM.PhoneNo3 });
                    }
                    // Fax
                    if (!string.IsNullOrEmpty(contactEditVM.Fax))
                    {
                        _commonFacade = new CommonFacade();
                        var phoneTypeFax = _commonFacade.GetPhoneTypeByCode(Constants.PhoneTypeCode.Fax);
                        contactEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = phoneTypeFax.PhoneTypeId, PhoneNo = contactEditVM.Fax });
                    }
                    #endregion

                    // Save
                    var contactRelationship = contactEditVM.ContactRelationshipList.Where(x => x.IsEdit == true).ToList();

                    bool isSuccess = false;
                    if (contactEditVM.IsConfirm == "0")
                    {
                        isSuccess = _customerFacade.SaveContact(contactEntity, null); // save contact only
                    }
                    else
                    {
                        #region "Validate ContactRelationship"

                        if (contactEditVM.ContactRelationshipList.Count(x => x.CustomerId == contactEditVM.CustomerId) == 0)
                        {
                            // Alert
                            return Json(new
                            {
                                Valid = false,
                                Error = Resource.Msg_PleaseInputRelationship,
                                Errors = string.Empty
                            });
                        }


                        #endregion

                        isSuccess = _customerFacade.SaveContact(contactEntity, contactRelationship); // save contact & relationship
                    }

                    if (isSuccess)
                    {
                        return Json(new
                        {
                            Valid = true,
                            contactId = contactEntity.ContactId,
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Valid = false,
                            Error = Resource.Error_System,
                            Errors = string.Empty
                        });
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Contact").Add("Error Message", ex.Message).ToFailLogString());
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
        [CheckUserRole(ScreenCode.ViewCustomerContact)]
        public ActionResult ContactRelationshipList(ContactEditViewModel contactEditVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get ContactRelationship List")
                .Add("JsonContactRelationship", contactEditVM.JsonContactRelationship).ToInputLogString());

            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get ContactRelationship List").ToSuccessLogString());
                return PartialView("~/Views/Contact/_ContactRelationshipList.cshtml", contactEditVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get ContactRelationship List").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        #endregion

        #region "PopupEdit ContactRelationship"

        [CheckUserRole(ScreenCode.ViewCustomerContact)]
        public ActionResult InitEditContactRelationship(string jsonData)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit ContactRelationship").Add("JsonContactRelationship", jsonData).ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();
                var contactRelationVM = JsonConvert.DeserializeObject<ContactRelationshipViewModel>(jsonData);

                if (contactRelationVM.ListIndex != null)
                {
                    CustomerContactEntity selected =
                        contactRelationVM.ContactRelationshipList[contactRelationVM.ListIndex.Value];
                    contactRelationVM.AccountId = selected.AccountId;
                    contactRelationVM.RelationshipId = selected.RelationshipId;
                    contactRelationVM.Product = selected.Product;
                }

                if (contactRelationVM.CustomerId.HasValue)
                {
                    var listAccount = _customerFacade.GetAccountByCustomerId(contactRelationVM.CustomerId.Value);
                    TempData["AccountList"] = listAccount; // keep AccountList

                    contactRelationVM.AccountList = new SelectList((IEnumerable)listAccount.Select(x => new
                    {
                        key = x.AccountId,
                        value = x.ProductAndAccountNoDisplay
                    }).ToDictionary(t => t.key, t => t.value), "Key", "Value", string.Empty);
                }

                // Get SelectList
                var lstRelationship = _commonFacade.GetRelationshipSelectList();
                TempData["RelationshipList"] = lstRelationship; // Keep RelationshipList
                contactRelationVM.RelationshipList = new SelectList((IEnumerable)lstRelationship, "Key", "Value", string.Empty);

                return PartialView("~/Views/Contact/_EditContactRelationship.cshtml", contactRelationVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit ContactRelationship").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerContact)]
        public ActionResult EditContactRelationship(ContactRelationshipViewModel contactRelationVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit ContactRelationship").Add("AccountId", contactRelationVM.AccountId)
                .Add("RelationshipId", contactRelationVM.RelationshipId).ToInputLogString());

            try
            {
                CustomerContactEntity custContact = contactRelationVM.ListIndex != null ?
                    contactRelationVM.ContactRelationshipList[contactRelationVM.ListIndex.Value] : new CustomerContactEntity();

                if (ModelState.IsValid)
                {
                    // Check Duplicate
                    var objCheckDup =
                        contactRelationVM.ContactRelationshipList.FirstOrDefault(
                            x => x.AccountId == contactRelationVM.AccountId);
                    if (objCheckDup != null && custContact.AccountId != objCheckDup.AccountId)
                    {
                        ModelState.AddModelError("AccountId", Resource.Error_SaveDuplicateContact);
                        goto Outer;
                    }

                    custContact.AccountId = contactRelationVM.AccountId;
                    custContact.RelationshipId = contactRelationVM.RelationshipId;

                    if (TempData["AccountList"] != null)
                    {
                        var lstAccount = (List<AccountEntity>)TempData["AccountList"];
                        var objAccount = lstAccount.FirstOrDefault(x => x.AccountId == contactRelationVM.AccountId);
                        custContact.AccountNo = objAccount.AccountNo;
                        custContact.Product = objAccount.Product;
                        custContact.AccountDesc = objAccount.AccountDesc;
                        TempData["AccountList"] = lstAccount; // keep AccountList
                    }

                    if (TempData["RelationshipList"] != null)
                    {
                        var lstRelationship = (IDictionary<string, string>)TempData["RelationshipList"];
                        var objRelationship = lstRelationship.FirstOrDefault(x => x.Key == contactRelationVM.RelationshipId.ToString(CultureInfo.InvariantCulture));
                        custContact.RelationshipName = objRelationship.Value;
                        TempData["RelationshipList"] = lstRelationship; // keep RelationshipList
                    }

                    if (contactRelationVM.ListIndex == null)
                    {
                        custContact.CustomerId = contactRelationVM.CustomerId;
                        custContact.IsEdit = true;

                        #region "Customer Name"

                        var custInfoVM = this.MappingCustomerInfoView(custContact.CustomerId.Value);
                        custContact.CustomerFullNameThaiEng = custInfoVM.FirstNameThaiEng + " " + custInfoVM.LastNameThaiEng;

                        #endregion

                        contactRelationVM.ContactRelationshipList.Add(custContact);
                    }

                    return Json(new
                    {
                        Valid = true,
                        Data = contactRelationVM.ContactRelationshipList
                    });
                }

            Outer:
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit ContactRelationship").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }

        [CheckUserRole(ScreenCode.ViewCustomerContact)]
        public JsonResult GetProduct(int accountId)
        {
            if (TempData["AccountList"] != null)
            {
                var lstAccount = (List<AccountEntity>)TempData["AccountList"];
                var objAccount = lstAccount.FirstOrDefault(x => x.AccountId == accountId);

                TempData["AccountList"] = lstAccount; // Keep AccountList

                return Json(new
                {
                    Valid = true,
                    Data = objAccount.Product
                });
            }

            return Json(new
            {
                Valid = false,
                Error = Resource.Error_System,
                Errors = string.Empty
            });
        }

        #endregion
    }
}
