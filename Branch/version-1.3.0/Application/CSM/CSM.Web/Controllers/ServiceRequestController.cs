using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CSM.Business;
using CSM.Common;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Entity.Common;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using Newtonsoft.Json;
using CSM.Data.DataAccess;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class ServiceRequestController : BaseController
    {
        private AuditLogEntity _auditLog;
        private ICommonFacade _commonFacade;
        private ServiceRequestFacade _srFacade;
        private ServiceRequestDataAccess _srDataAccess;
        private ICustomerFacade _customerFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceRequestController));

        #region == Search ==

        public ActionResult Index()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch ServiceRequest").ToInputLogString());

            try
            {
                SearchServiceRequestViewModel viewModel = new SearchServiceRequestViewModel();
                viewModel.SearchFilter = new ServiceRequestSearchFilter
                {
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "",
                    SortOrder = ""
                };

                _commonFacade = new CommonFacade();
                var defSearch = _commonFacade.GetShowhidePanelByUserId(this.UserInfo, Constants.Page.ServiceRequestPage);

                viewModel.IsShowAdvanceSearch = defSearch != null ? defSearch.IsSelectd : false;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch ServiceRequest").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult ServiceRequestList(ServiceRequestSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search ServiceRequest").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    SearchServiceRequestViewModel viewModel = new SearchServiceRequestViewModel();
                    viewModel.SearchFilter = searchFilter;

                    if (searchFilter.CurrentUserId != UserInfo.UserId)
                    {
                        // First Load OR Change User
                        searchFilter.CurrentUserId = UserInfo.UserId;
                        searchFilter.CanViewAllUsers = null;
                        searchFilter.CanViewUserIds = string.Empty;
                        searchFilter.CanViewSrPageIds = string.Empty;
                    }

                    if (searchFilter.CurrentUserRoleCode != UserInfo.RoleCode)
                    {
                        // First Load OR Change Role
                        searchFilter.CurrentUserRoleCode = UserInfo.RoleCode;
                    }

                    _srFacade = new ServiceRequestFacade();
                    viewModel.ServiceRequestList = _srFacade.SearchServiceRequest(viewModel.SearchFilter);

                    ViewBag.CanViewAllUsers = viewModel.SearchFilter.CanViewAllUsers;
                    ViewBag.CanViewUserIds = viewModel.SearchFilter.CanViewUserIds;
                    ViewBag.CanViewSrPageIds = viewModel.SearchFilter.CanViewSrPageIds;

                    ViewBag.PageSize = viewModel.SearchFilter.PageSize;

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Search ServiceRequest").ToSuccessLogString());

                    return PartialView("~/Views/ServiceRequest/_SearchServiceRequestList.cshtml", viewModel);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search ServiceRequest").Add("Error Message", ex.Message).ToFailLogString());
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
                _commonFacade.SaveShowhidePanel(expandValue, UserInfo.UserId, Constants.Page.ServiceRequestPage);
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

        #endregion

        #region == Create SR ==

        public ActionResult Create()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR)").ToInputLogString());

            try
            {
                var model = new CreateServiceRequestViewModel();
                return Create(model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR)").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        public ActionResult CreateWithPhoneNo(string phoneNo)
        {
            return Create(new CreateServiceRequestViewModel { PhoneNo = phoneNo, CallId = phoneNo });
        }

        public ActionResult CreateWithCustomerId(int id)
        {
            return Create(new CreateServiceRequestViewModel { CustomerId = id });
        }

        public ActionResult CreateWithAccountId(int id)
        {
            return Create(new CreateServiceRequestViewModel { AccountId = id });
        }

        public ActionResult CreateWithCustomerContactId(int id)
        {
            return Create(new CreateServiceRequestViewModel { CustomerContactId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateServiceRequestViewModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR)").ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();

                if (model.CommandButton == null || string.IsNullOrEmpty(model.CommandButton))
                {
                    return CreateStep1(model);
                }
                else if (model.CommandButton == "NextOnStep1")
                {
                    return CreateStep2(model);
                }
                else if (model.CommandButton == "NextOnStep2")
                {
                    return CreateStep3(model);
                }
                else if (model.CommandButton == "BackOnStep2")
                {
                    return CreateStep1(model);
                }
                else if (model.CommandButton == "BackOnStep3")
                {
                    var isBack = true;
                    return CreateStep2(model, isBack);
                }
                else
                {
                    throw new Exception("No have this case");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR)").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        /// <summary>
        /// สำหรับการ โหลดหน้าแรก
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ActionResult CreateStep1(CreateServiceRequestViewModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Create Step 1")
                .Add("CallId", model.CallId).Add("PhoneNo", model.PhoneNo).ToInputLogString());

            try
            {
                if (_srFacade == null)
                    _srFacade = new ServiceRequestFacade();

                #region == Phone No ==

                if (string.IsNullOrEmpty(model.PhoneNo))
                    model.PhoneNo = base.PhoneNo;

                if (string.IsNullOrEmpty(model.CallId))
                    model.CallId = base.CallId;

                //if (!string.IsNullOrEmpty(model.PhoneNo) && model.ChannelId == null)
                //{
                //    model.ChannelId = Constants.CallCenterChannelId;
                //}

                if (model.ChannelId == null)
                {
                    model.ChannelId = UserInfo.ChannelId;
                    model.ChannelName = UserInfo.ChannelName;
                }

                #endregion

                #region == Start With Parameter ==

                // Check is Not Draft or not saved
                if (!model.SrId.HasValue && !model.MappingProductId.HasValue)
                {
                    // Open New SR
                    if ((model.CustomerContactId ?? 0) != 0)
                    {
                        var customerContact = _srFacade.GetCustomerContact(model.CustomerContactId.Value);
                        if (customerContact != null
                            && customerContact.CustomerAccount != null
                            && customerContact.CustomerAccount.Customer != null
                            && customerContact.CustomerAccount.Account != null)
                        {
                            FillCustomerToModel(model, customerContact.CustomerAccount.Customer);
                            FillAccountToModel(model, customerContact.CustomerAccount.Account);
                            FillCustomerContactToModel(model, customerContact.Contact, customerContact.CustomerContact);
                        }
                    }
                    else if ((model.AccountId ?? 0) != 0)
                    {
                        var customerAccount = _srFacade.GetCustomerAccount(model.AccountId.Value);
                        if (customerAccount != null
                            && customerAccount.Customer != null
                            && customerAccount.Account != null)
                        {
                            FillCustomerToModel(model, customerAccount.Customer);
                            FillAccountToModel(model, customerAccount.Account);
                        }
                    }
                    else if ((model.CustomerId ?? 0) != 0)
                    {
                        var customer = _srFacade.GetCustomerByID(model.CustomerId.Value);
                        if (customer != null)
                        {
                            FillCustomerToModel(model, customer);
                        }
                    }

                }

                #endregion

                if (model.ServiceRequest != null)
                {
                    var sr = model.ServiceRequest;

                    model.CustomerId = sr.CustomerId;
                    model.AccountId = sr.AccountId;
                    model.CustomerId = sr.CustomerId;

                    model.ProductGroupId = sr.ProductGroupId;
                    model.ProductGroupName = sr.ProductGroupName;
                    model.ProductId = sr.ProductId;
                    model.ProductName = sr.ProductName;
                    model.CampaignServiceId = sr.CampaignServiceId;
                    model.CampaignServiceName = sr.CampaignServiceName;

                    model.AreaId = sr.AreaId;
                    model.AreaName = sr.AreaName;
                    model.SubAreaId = sr.SubAreaId;
                    model.SubAreaName = sr.SubAreaName;
                    model.TypeId = sr.TypeId;
                    model.TypeName = sr.TypeName;

                    if (sr.CreateUser != null)
                    {
                        model.CreatorBranchId = sr.CreateUser.BranchId ?? 0;
                        model.CreatorBranchCode = sr.CreateUser.BranchCode;
                        model.CreatorBranchName = sr.CreateUser.BranchName;
                        model.CreatorUserId = sr.CreateUser.UserId;
                        model.CreatorUserFullName = sr.CreateUser.FullName;
                    }
                    else
                    {
                        return Error(new HandleErrorInfo(new Exception("CreateUser is null"), "ServiceRequest", "Create"));
                    }
                }
                else
                {
                    model.CreatorBranchId = UserInfo.BranchId;
                    model.CreatorBranchCode = !string.IsNullOrEmpty(UserInfo.BranchCode) ? UserInfo.BranchCode.Trim() : string.Empty;
                    model.CreatorBranchName = UserInfo.BranchName.Trim();
                    model.CreatorUserId = UserInfo.UserId;
                    model.CreatorUserFullName = UserInfo.FullName.Trim();
                }

                _commonFacade = new CommonFacade();
                //customer informationlist
                model.SubscriptTypeList = new SelectList((IEnumerable)_commonFacade.GetSubscriptTypeSelectList(), "Key", "Value", string.Empty);
                model.TitleThaiList = new SelectList((IEnumerable)_commonFacade.GetTitleThaiSelectList(), "Key", "Value", string.Empty);
                model.TitleEnglishList = new SelectList((IEnumerable)_commonFacade.GetTitleEnglishSelectList(), "Key", "Value", string.Empty);
                model.PhoneTypeList = new SelectList((IEnumerable)_commonFacade.GetPhoneTypeSelectList(), "Key", "Value", string.Empty);

                model.Channels = _srFacade.GetChannels().Select(c => new SelectListItem() { Text = c.Name, Value = c.ChannelId + "", }).ToList();
                model.MediaSources = _srFacade.GetMediaSources().Select(c => new SelectListItem() { Text = c.Name, Value = c.MediaSourceId + "", }).ToList();

                if (model.CampaignServiceId.HasValue)
                {
                    if (string.IsNullOrEmpty(model.CampaignServiceName)
                        || string.IsNullOrEmpty(model.ProductName)
                        || string.IsNullOrEmpty(model.ProductGroupName))
                    {
                        var campaignService = _srFacade.GetCampaignService(model.CampaignServiceId.Value);
                        if (campaignService != null)
                        {
                            model.CampaignServiceName = campaignService.CampaignServiceName;
                            model.ProductGroupId = campaignService.ProductGroupId;
                            model.ProductGroupName = campaignService.ProductGroupName;
                            model.ProductId = campaignService.ProductId;
                            model.ProductName = campaignService.ProductName;
                        }
                    }
                }

                return View("Create", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Create Step 1").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        private void FillCustomerContactToModel(CreateServiceRequestViewModel model, ContactEntity contact, CustomerContactEntity customerContactEntity)
        {
            var phoneNo1 = string.Empty;
            var phoneNo2 = string.Empty;
            var phoneNo3 = string.Empty;
            var phoneTypeName1 = string.Empty;
            var phoneTypeName2 = string.Empty;
            var phoneTypeName3 = string.Empty;
            var fax = string.Empty;

            var phoneNoFaxes = contact.PhoneList.Where(c => c.PhoneTypeCode != Constants.PhoneTypeCode.Fax).ToList();
            if (phoneNoFaxes.Count > 0)
            {
                phoneNo1 = phoneNoFaxes[0].PhoneNo;
                phoneTypeName1 = phoneNoFaxes[0].PhoneTypeName;

                if (phoneNoFaxes.Count > 1)
                {
                    phoneNo2 = phoneNoFaxes[1].PhoneNo;
                    phoneTypeName2 = phoneNoFaxes[1].PhoneTypeName;

                    if (phoneNoFaxes.Count > 2)
                    {
                        phoneNo3 = phoneNoFaxes[2].PhoneNo;
                        phoneTypeName3 = phoneNoFaxes[2].PhoneTypeName;
                    }
                }
            }

            model.ContactId = contact.ContactId;
            model.ContactSubscriptionName = contact.SubscriptType != null ? contact.SubscriptType.SubscriptTypeName : string.Empty;
            model.ContactCardNo = contact.CardNo;
            model.ContactBirthDate = contact.BirthDateDisplay;
            model.ContactTitleTh = contact.TitleThai != null ? contact.TitleThai.TitleName : string.Empty;
            model.ContactTitleEn = contact.TitleEnglish != null ? contact.TitleEnglish.TitleName : string.Empty;
            model.ContactFirstNameTh = contact.FirstNameThai;
            model.ContactFirstNameEn = contact.FirstNameEnglish;
            model.ContactLastNameTh = contact.LastNameThai;
            model.ContactLastNameEn = contact.LastNameEnglish;
            model.ContactPhoneNo1 = phoneNo1;
            model.ContactPhoneNo2 = phoneNo2;
            model.ContactPhoneNo3 = phoneNo3;
            model.ContactPhoneTypeName1 = phoneTypeName1;
            model.ContactPhoneTypeName2 = phoneTypeName2;
            model.ContactPhoneTypeName3 = phoneTypeName3;
            model.ContactFax = contact.Fax;
            model.ContactEmail = contact.Email;

            model.ContactAccountNo = customerContactEntity.AccountNo;
            model.ContactRelationshipId = customerContactEntity.RelationshipId;
            model.ContactRelationshipName = customerContactEntity.RelationshipName;
        }

        private void FillCustomerToModel(CreateServiceRequestViewModel model, CustomerEntity customer)
        {
            if (_srFacade == null)
                _srFacade = new ServiceRequestFacade();

            var phoneNo1 = string.Empty;
            var phoneNo2 = string.Empty;
            var phoneNo3 = string.Empty;
            var phoneTypeName1 = string.Empty;
            var phoneTypeName2 = string.Empty;
            var phoneTypeName3 = string.Empty;
            var fax = string.Empty;

            var employeeCode = string.Empty;
            if (customer.EmployeeId.HasValue)
            {
                var user = _srFacade.GetUserById(customer.EmployeeId.Value);
                if (user != null)
                {
                    employeeCode = user.EmployeeCode;
                }
            }

            var phoneNoFaxes = customer.PhoneList.Where(c => c.PhoneTypeCode != Constants.PhoneTypeCode.Fax).ToList();
            if (phoneNoFaxes.Count > 0)
            {
                phoneNo1 = phoneNoFaxes[0].PhoneNo;
                phoneTypeName1 = phoneNoFaxes[0].PhoneTypeName;

                if (phoneNoFaxes.Count > 1)
                {
                    phoneNo2 = phoneNoFaxes[1].PhoneNo;
                    phoneTypeName2 = phoneNoFaxes[1].PhoneTypeName;

                    if (phoneNoFaxes.Count > 2)
                    {
                        phoneNo3 = phoneNoFaxes[2].PhoneNo;
                        phoneTypeName3 = phoneNoFaxes[2].PhoneTypeName;
                    }
                }
            }

            model.CustomerId = customer.CustomerId;
            model.CustomerSubscriptionName = customer.SubscriptType != null ? customer.SubscriptType.SubscriptTypeName : string.Empty;
            model.CustomerCardNo = customer.CardNo;
            model.CustomerBirthDate = customer.BirthDateDisplay;
            model.CustomerTitleTh = customer.TitleThai != null ? customer.TitleThai.TitleName : string.Empty;
            model.CustomerTitleEn = customer.TitleEnglish != null ? customer.TitleEnglish.TitleName : string.Empty;
            model.CustomerFirstNameTh = customer.FirstNameThai;
            model.CustomerFirstNameEn = customer.FirstNameEnglish;
            model.CustomerLastNameTh = customer.LastNameThai;
            model.CustomerLastNameEn = customer.LastNameEnglish;
            model.CustomerPhoneNo1 = phoneNo1;
            model.CustomerPhoneNo2 = phoneNo2;
            model.CustomerPhoneNo3 = phoneNo3;
            model.CustomerPhoneTypeName1 = phoneTypeName1;
            model.CustomerPhoneTypeName2 = phoneTypeName2;
            model.CustomerPhoneTypeName3 = phoneTypeName3;
            model.CustomerFax = customer.Fax;
            model.CustomerEmail = customer.Email;
            model.CustomerEmployeeCode = employeeCode;
        }

        private void FillAccountToModel(CreateServiceRequestViewModel model, AccountEntity account)
        {
            model.AccountId = account.AccountId;
            model.AccountNo = account.AccountNo;

            var status = string.IsNullOrEmpty(account.Status) ? "Inactive" : (account.Status.ToUpper(CultureInfo.InvariantCulture) == "A" ? "Active" : "Inactive");

            model.AccountStatus = status;
            model.AccountCarNo = account.CarNo;
            model.AccountProductGroupName = account.ProductGroup;
            model.AccountProductName = account.Product;
            model.AccountBranchName = account.BranchName;
        }

        private ActionResult CreateStep2(CreateServiceRequestViewModel model, bool isBack = false)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Create Step 2").ToInputLogString());

            try
            {
                if (model.MappingProduct == null)
                    model.MappingProduct = this.GetMapping(model);

                if (model.MappingProduct == null)
                {
                    ModelState.AddModelError("ErrorMessage", "ไม่พบข้อมูล Mapping 6 ฟิล์ด");
                    return CreateStep1(model);
                }

                if (model.MappingProduct.DefaultOwnerUser != null)
                {
                    var user = model.MappingProduct.DefaultOwnerUser;
                    model.DefaultOwnerUserId = user.UserId;
                    model.DefaultOwnerUserFullName = user.FullName;

                    if (model.MappingProduct.DefaultOwnerBranch != null)
                    {
                        var branch = model.MappingProduct.DefaultOwnerBranch;
                        model.DefaultOwnerBranchId = branch.BranchId;
                        model.DefaultOwnerBranchName = branch.BranchName;
                    }
                }
                else
                {
                    model.DefaultOwnerUserId = model.CreatorUserId;
                    model.DefaultOwnerUserFullName = model.CreatorUserFullName;
                    model.DefaultOwnerBranchId = model.CreatorBranchId;
                    model.DefaultOwnerBranchName = model.CreatorBranchName;
                }

                switch (model.MappingProduct.SrPageId)
                {
                    case 1:
                    case 2:
                        model.IsEmailDelegate = true;
                        break;
                    case 3:
                        model.IsEmailDelegate = false;
                        break;
                }

                model.IsVerify = model.MappingProduct.IsVerify;
                model.SrPageId = model.MappingProduct.SrPageId;
                model.SrPageCode = model.MappingProduct.SrPageCode;
                model.MappingProductId = model.MappingProduct.MappingProductId;

                if (model.IsVerify)
                {
                    return View("CreateStep2", model);
                }
                else
                {
                    if (!isBack)
                        return CreateStep3(model);
                    else
                        return CreateStep1(model);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Create Step 2").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        private ActionResult CreateStep3(CreateServiceRequestViewModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Create Step 3").ToInputLogString());

            try
            {
                if (model.MappingProduct == null)
                    model.MappingProduct = this.GetMapping(model);

                if (model.MappingProduct == null)
                {
                    ModelState.AddModelError("ErrorMessage", "ไม่พบข้อมูล Mapping 6 ฟิล์ด");
                    return CreateStep1(model);
                }

                if (_srFacade == null)
                    _srFacade = new ServiceRequestFacade();

                model.ActivityTypes = _srFacade.GetActivityTypes().Select(c => new SelectListItem() { Text = c.Name, Value = c.ActivityTypeId + "" }).ToList();
                model.SrEmailTemplates = _srFacade.GetSrEmailTemplates().Select(c => new SelectListItem() { Text = c.Name, Value = c.SrEmailTemplateId + "" }).ToList();
                model.NCBCheckStatuses = _srFacade.GetNCBCheckStatuses().Select(c => new SelectListItem() { Text = c.Key, Value = c.Value }).ToList();

                if (!model.NCBMarketingUserId.HasValue || string.IsNullOrEmpty(model.NCBMarketingName))
                {
                    var userMarketingEntity = _srFacade.GetUserMarketing(model.CustomerId.Value);
                    if (userMarketingEntity != null)
                    {
                        if (userMarketingEntity.UserEntity != null)
                        {
                            model.NCBMarketingUserId = userMarketingEntity.UserEntity.UserId;
                            model.NCBMarketingName = userMarketingEntity.UserEntity.FullName;
                        }

                        if (userMarketingEntity.BranchEntity != null)
                        {
                            model.NCBMarketingBranchId = userMarketingEntity.BranchEntity.BranchId;
                            model.NCBMarketingBranchName = userMarketingEntity.BranchEntity.BranchName;
                        }

                        if (userMarketingEntity.UpperBranch1 != null)
                        {
                            model.NCBMarketingBranchUpper1Id = userMarketingEntity.UpperBranch1.BranchId;
                            model.NCBMarketingBranchUpper1Name = userMarketingEntity.UpperBranch1.BranchName;
                        }

                        if (userMarketingEntity.UpperBranch2 != null)
                        {
                            model.NCBMarketingBranchUpper2Id = userMarketingEntity.UpperBranch2.BranchId;
                            model.NCBMarketingBranchUpper2Name = userMarketingEntity.UpperBranch2.BranchName;
                        }
                    }
                }

                model.SearchFilter = new SrAttachmentSearchFilter()
                {
                    SrId = null,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "field",
                    SortOrder = "ASC"
                };

                // Get Default Account Address
                if (model.AccountId.HasValue && (!model.AddressSendDocId.HasValue || model.AddressSendDocId.Value == -1))
                {
                    var result = GetDefaultAccountAddress(model.AccountId.Value);
                    if (result != null)
                    {
                        model.AddressSendDocText = result.AddressDiplay;    //result.AccountAddress;
                        model.AddressSendDocId = result.AddressId;
                    }
                }


                _commonFacade = new CommonFacade();
                model.SrDocumentFolder = _commonFacade.GetSrDocumentFolder();

                ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexFileExt);
                ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);

                int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();
                var singleLimitSize = limitSingleFileSize.HasValue ? (limitSingleFileSize / 1048576) : 0;
                model.AllowFileExtensionsDesc = string.Format(CultureInfo.InvariantCulture, param.ParamDesc, singleLimitSize);
                model.AllowFileExtensionsRegex = param.ParamValue;

                //get doc type list
                List<DocumentTypeEntity> docTypeList = null;
                docTypeList = _commonFacade.GetActiveDocumentTypes(Constants.DocumentCategory.ServiceRequest);

                if (docTypeList != null)
                {
                    //check if SR created from job.
                    if (model.AttachmentJson != null) {
                        //assign others doc type
                        docTypeList[23].IsChecked = true;
                    }

                    model.DocTypeCheckBoxes = docTypeList.Select(x => new CheckBoxes
                    {
                        Value = x.DocTypeId.ToString(),
                        Text = x.Name,
                        Checked = x.IsChecked
                    }).ToList();
                }

                //get userid
                ViewBag.CurrentUserId = UserInfo.UserId;

                model.OfficePhoneNo = _commonFacade.GetOfficePhoneNo();
                model.OfficeHour = _commonFacade.GetOfficeHour();
                var statusList = _commonFacade.GetStatusSelectList();
                model.StatusList = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                return View("CreateStep3", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Create Step 3").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        public AccountAddressEntity GetDefaultAccountAddress(int accountId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Default AccountAddress").Add("AccountId", accountId).ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                var result = _srFacade.GetDefaultAccountAddress(accountId);
                if (result == null)
                {
                    return null;
                }

                var tokens = new List<string>();
                tokens.Add(result.AddressNo);
                tokens.Add(result.Village);
                tokens.Add(result.Building);
                tokens.Add(result.FloorNo);
                tokens.Add(result.RoomNo);
                tokens.Add(result.Moo);
                tokens.Add(result.Street);
                tokens.Add(result.Soi);
                tokens.Add(result.SubDistrict);
                tokens.Add(result.District);
                tokens.Add(result.Province);
                tokens.Add(result.Country);
                tokens.Add(result.Postcode);

                return new AccountAddressEntity()
                {
                    AddressId = result.AddressId,
                    AccountAddress = string.Join(" ", tokens),
                    AddressNo = result.AddressNo,
                    Village = result.Village,
                    Building = result.Building,
                    FloorNo = result.FloorNo,
                    RoomNo = result.RoomNo,
                    Moo = result.Moo,
                    Street = result.Street,
                    Soi = result.Soi,
                    SubDistrict = result.SubDistrict,
                    District = result.District,
                    Province = result.Province,
                    Country = result.Country,
                    Postcode = result.Postcode
                };

            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Default AccountAddress").Add("Error Message", ex.Message).ToFailLogString());
                return null;
            }
        }

        private MappingProductEntity GetMapping(CreateServiceRequestViewModel model)
        {
            var campaignserviceId = model.CampaignServiceId ?? 0;
            var areaId = model.AreaId ?? 0;
            var subareaId = model.SubAreaId ?? 0;
            var typeId = model.TypeId ?? 0;

            if (campaignserviceId == 0 || areaId == 0 || subareaId == 0 || typeId == 0)
            {
                ModelState.AddModelError("CampaignServiceId", "CampaignServiceId, AreaId, SubAreaId, TypeId cannot be zero");
                return null;
            }

            if (_srFacade == null)
                _srFacade = new ServiceRequestFacade();

            return _srFacade.GetMapping(campaignserviceId, areaId, subareaId, typeId);
        }

        #endregion

        #region == Edit SR ==

        [HttpGet]
        public ActionResult Edit()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int srId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Service Request (SR)").Add("SrId", srId).ToInputLogString());

            try
            {
                if (_srFacade == null)
                    _srFacade = new ServiceRequestFacade();

                var sr = _srFacade.GetServiceRequest(srId);
                if (sr == null)
                    return RedirectToAction("Index");

                return Edit(sr);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Service Request (SR)").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit2(int srId)
        //{
        //    Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Service Request (SR)").ToInputLogString());

        //    try
        //    {
        //        if (_srFacade == null)
        //            _srFacade = new ServiceRequestFacade();

        //        var entity = _srFacade.GetServiceRequestNoDetail(srId);
        //        if (entity == null)
        //            return RedirectToAction("Index");

        //        if (!entity.CustomerId.HasValue)
        //        {
        //            ViewBag.ErrorMessage = "ข้อมูลในฐานข้อมูลไม่สมบูรณ์: ขาดข้อมูล CustomerId";
        //            return View("Edit2");
        //        }

        //        if (!entity.SrStatusId.HasValue)
        //        {
        //            ViewBag.ErrorMessage = "ข้อมูลในฐานข้อมูลไม่สมบูรณ์: ขาดข้อมูล SRStatusID";
        //            return View("Edit2");
        //        }

        //        var customerId = entity.CustomerId.Value;
        //        var srStatusId = entity.SrStatusId.Value;

        //        var model = new EditServiceRequestViewModel();
        //        model.Entity = entity;

        //        //init activity searchfilter
        //        model.ActivitySearchFilter = new ActivityTabSearchFilter()
        //        {
        //            SrId = srId,
        //            CustomerId = customerId,
        //            SrOnly = true,
        //            PageNo = 1,
        //            PageSize = 15,
        //            SortField = "",
        //            SortOrder = "ASC"
        //        };

        //        //init existing searchfilter
        //        model.ExistingSearchFilter = new ExistingSearchFilter()
        //        {
        //            CustomerId = customerId,
        //            PageNo = 1,
        //            PageSize = 15,
        //            SortField = "",
        //            SortOrder = "ASC"
        //        };

        //        //init document searchfilter
        //        model.DocumentSearchFilter = new DocumentSearchFilter()
        //        {
        //            SrId = srId,
        //            CustomerId = customerId,
        //            SrOnly = true,
        //            PageNo = 1,
        //            PageSize = 15,
        //            SortField = "",
        //            SortOrder = ""
        //        };

        //        //init logging searchfilter
        //        model.LoggingSearchFilter = new LoggingSearchFilter()
        //        {
        //            SrId = srId,
        //            PageNo = 1,
        //            PageSize = 15,
        //            SortField = "",
        //            SortOrder = ""
        //        };

        //        switch (srStatusId)
        //        {
        //            case Constants.SRStatusId.Draft:
        //                return View("Create");
        //                break;
        //            case Constants.SRStatusId.Closed:
        //            case Constants.SRStatusId.Cancelled:
        //                model.CanEdit = false;
        //                return View("Edit2", model);
        //                break;
        //            default:
        //                model.CanEdit = true;
        //                return View("Edit2", model);
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Service Request (SR)").ToFailLogString());
        //        return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
        //            this.ControllerContext.RouteData.Values["action"].ToString()));
        //    }
        //}

        public ActionResult View(string srNo)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("View Service Request by SR No").Add("SrNo", srNo).ToInputLogString());

            try
            {
                if (_srFacade == null)
                    _srFacade = new ServiceRequestFacade();

                var sr = _srFacade.GetServiceRequest(srNo);
                if (sr == null)
                    return RedirectToAction("Index");

                return Edit(sr);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("View Service Request by SR No").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        private ActionResult Edit(ServiceRequestForDisplayEntity sr)
        {
            var srId = sr.SrId;

            if (_srFacade == null)
                _srFacade = new ServiceRequestFacade();

            var isDraftSR = sr.SRStatus == null || sr.SRStatus.SRStatusCode == Constants.SRStatusCode.Draft;

            var model = new CreateServiceRequestViewModel();

            model.SrId = sr.SrId;

            if (!isDraftSR)
            {
                model.SrNo = sr.SrNo;
            }

            model.CallId = sr.CallId;
            model.PhoneNo = sr.PhoneNo;

            if (sr.Customer != null)
            {
                var customer = sr.Customer;

                model.CustomerId = customer.CustomerId;

                if (sr.CustomerSubscriptType != null)
                {
                    model.CustomerSubscriptionTypeCode = sr.CustomerSubscriptType.SubscriptTypeCode;
                    model.CustomerSubscriptionName = sr.CustomerSubscriptType.SubscriptTypeName;
                }

                model.CustomerCardNo = customer.CardNo;
                model.CustomerBirthDate = customer.BirthDateDisplay;
                model.CustomerTitleTh = customer.TitleThai != null ? customer.TitleThai.TitleName : string.Empty;
                model.CustomerTitleEn = customer.TitleEnglish != null ? customer.TitleEnglish.TitleName : string.Empty;
                model.CustomerFirstNameTh = customer.FirstNameThai;
                model.CustomerFirstNameEn = customer.FirstNameEnglish;
                model.CustomerLastNameTh = customer.LastNameThai;
                model.CustomerLastNameEn = customer.LastNameEnglish;

                var phoneNotFax = customer.PhoneList.Where(c => c.PhoneTypeCode != Constants.PhoneTypeCode.Fax).ToList();

                if (phoneNotFax.Count > 0)
                {
                    model.CustomerPhoneTypeName1 = phoneNotFax[0].PhoneTypeName;
                    model.CustomerPhoneNo1 = phoneNotFax[0].PhoneNo;

                    if (phoneNotFax.Count > 1)
                    {
                        model.CustomerPhoneTypeName2 = phoneNotFax[1].PhoneTypeName;
                        model.CustomerPhoneNo2 = phoneNotFax[1].PhoneNo;

                        if (phoneNotFax.Count > 2)
                        {
                            model.CustomerPhoneTypeName3 = phoneNotFax[2].PhoneTypeName;
                            model.CustomerPhoneNo3 = phoneNotFax[2].PhoneNo;
                        }
                    }
                }

                var phoneFax = customer.PhoneList.FirstOrDefault(c => c.PhoneTypeCode == Constants.PhoneTypeCode.Fax);

                model.CustomerFax = phoneFax != null ? phoneFax.PhoneNo : string.Empty;
                model.CustomerEmail = customer.Email ?? string.Empty;
            }

            model.CustomerEmployeeCode = sr.CustomerEmployeeCode ?? string.Empty;

            if (sr.Account != null)
            {
                var account = sr.Account;
                model.AccountId = account.AccountId;
                model.AccountNo = account.AccountNo;
                model.AccountStatus = account.Status;
                model.AccountCarNo = account.CarNo;
                model.AccountProductGroupName = account.ProductGroup;
                model.AccountProductName = account.Product;
                model.AccountBranchName = account.BranchName;
            }

            if (sr.Contact != null)
            {
                var contact = sr.Contact;

                model.ContactId = contact.ContactId;

                if (sr.ContactSubscriptType != null)
                    model.ContactSubscriptionName = sr.ContactSubscriptType.SubscriptTypeName;

                model.ContactCardNo = contact.CardNo;
                model.ContactBirthDate = contact.BirthDateDisplay;
                model.ContactTitleTh = contact.TitleThai != null ? contact.TitleThai.TitleName : string.Empty;
                model.ContactTitleEn = contact.TitleEnglish != null ? contact.TitleEnglish.TitleName : string.Empty;
                model.ContactFirstNameTh = contact.FirstNameThai;
                model.ContactFirstNameEn = contact.FirstNameEnglish;
                model.ContactLastNameTh = contact.LastNameThai;
                model.ContactLastNameEn = contact.LastNameEnglish;

                var phoneNotFax = contact.PhoneList.Where(c => c.PhoneTypeCode != Constants.PhoneTypeCode.Fax).ToList();

                if (phoneNotFax.Count > 0)
                {
                    model.ContactPhoneTypeName1 = phoneNotFax[0].PhoneTypeName;
                    model.ContactPhoneNo1 = phoneNotFax[0].PhoneNo;

                    if (phoneNotFax.Count > 1)
                    {
                        model.ContactPhoneTypeName2 = phoneNotFax[1].PhoneTypeName;
                        model.ContactPhoneNo2 = phoneNotFax[1].PhoneNo;

                        if (phoneNotFax.Count > 2)
                        {
                            model.ContactPhoneTypeName3 = phoneNotFax[2].PhoneTypeName;
                            model.ContactPhoneNo3 = phoneNotFax[2].PhoneNo;
                        }
                    }
                }

                var phoneFax = contact.PhoneList.FirstOrDefault(c => c.PhoneTypeCode == Constants.PhoneTypeCode.Fax);

                model.ContactFax = phoneFax != null ? phoneFax.PhoneNo : string.Empty;
                model.ContactEmail = contact.Email ?? string.Empty;
            }

            model.ContactAccountNo = sr.ContactAccountNo;
            model.ContactRelationshipId = sr.Relationship != null ? (int?)sr.Relationship.RelationshipId : (int?)null;
            model.ContactRelationshipName = sr.Relationship != null ? (sr.Relationship.RelationshipName ?? string.Empty) : string.Empty;

            if (sr.ProductGroup != null)
            {
                model.ProductGroupId = sr.ProductGroup.ProductGroupId;
                model.ProductGroupName = sr.ProductGroup.ProductGroupName;
            }

            if (sr.Product != null)
            {
                model.ProductId = sr.Product.ProductId;
                model.ProductName = sr.Product.ProductName;
            }

            if (sr.CampaignService != null)
            {
                model.CampaignServiceId = sr.CampaignService.CampaignServiceId;
                model.CampaignServiceName = sr.CampaignService.CampaignServiceName;
            }

            if (sr.Area != null)
            {
                model.AreaId = sr.Area.AreaId;
                model.AreaName = sr.Area.AreaName;
            }

            if (sr.SubArea != null)
            {
                model.SubAreaId = sr.SubArea.SubareaId;
                model.SubAreaName = sr.SubArea.SubareaName;
            }

            if (sr.Type != null)
            {
                model.TypeId = sr.Type.TypeId;
                model.TypeName = sr.Type.TypeName;
            }

            if (sr.Channel != null)
            {
                model.ChannelId = sr.Channel.ChannelId;
                model.ChannelName = sr.Channel.Name;
            }

            if (sr.MediaSource != null)
            {
                model.MediaSourceId = sr.MediaSource.MediaSourceId;
                model.MediaSourceName = sr.MediaSource.Name;
            }

            model.Subject = sr.Subject;
            model.Remark = ApplicationHelpers.StripHtmlTags(sr.Remark);

            if (_commonFacade == null)
                _commonFacade = new CommonFacade();

            ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexFileExt);
            ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);

            int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();
            var singleLimitSize = limitSingleFileSize.HasValue ? (limitSingleFileSize / 1048576) : 0;
            model.AllowFileExtensionsDesc = string.Format(CultureInfo.InvariantCulture, param.ParamDesc, singleLimitSize);

            model.MappingProductId = sr.MappingProductId;
            model.IsVerify = sr.IsVerify ?? false;
            model.IsVerifyPass = sr.IsVerifyPass;

            if (isDraftSR)
            {
                model.VerifyAnswerJson = isDraftSR ? sr.DraftVerifyAnswerJson : string.Empty;
            }
            else
            {
                model.VerifyGroups = _srFacade.GetVerifyGroup(srId);
            }

            model.SrEmailTemplateId = sr.DraftSrEmailTemplateId;
            model.ActivityDescription = sr.DraftActivityDescription;
            model.ActivityTypeId = sr.DraftActivityTypeId;
            model.SendMailSender = sr.DraftSendMailSender;
            model.SendMailTo = sr.DraftSendMailTo;
            model.SendMailCc = sr.DraftSendMailCc;
            model.SendMailSubject = sr.DraftSendMailSubject;
            model.SendMailBody = sr.DraftSendMailBody;

            if (isDraftSR)
            {
                model.IsEmailDelegate = sr.DraftIsEmailDelegate;
                model.IsClose = sr.DraftIsClose;
                model.VerifyAnswerJson = sr.DraftVerifyAnswerJson;
                model.AttachmentJson = sr.DraftAttachmentJson;
                model.SrEmailTemplateId = sr.DraftSrEmailTemplateId;
            }

            if (sr.CreateUser != null)
            {
                model.CreatorUserId = sr.CreateUser.UserId;
                model.CreatorUserFullName = sr.CreateUser.FullName;
            }

            if (sr.CreateBranch != null)
            {
                model.CreatorBranchId = sr.CreateBranch.BranchId;
                model.CreatorBranchName = sr.CreateBranch.BranchName;
            }

            if (sr.OwnerUser != null)
            {
                model.OwnerUserId = sr.OwnerUser.UserId;
                model.OwnerUserFullName = sr.OwnerUser.FullName;
            }

            if (sr.OwnerUserBranch != null)
            {
                model.OwnerBranchId = sr.OwnerUserBranch.BranchId;
                model.OwnerBranchName = sr.OwnerUserBranch.BranchName;
            }

            if (sr.DelegateUser != null)
            {
                model.DelegateUserId = sr.DelegateUser.UserId;
                model.DelegateUserFullName = sr.DelegateUser.FullName;
            }

            if (sr.DelegateUserBranch != null)
            {
                model.DelegateBranchId = sr.DelegateUserBranch.BranchId;
                model.DelegateBranchName = sr.DelegateUserBranch.BranchName;
            }

            if (sr.SrPageId.HasValue)
            {
                model.SrPageId = sr.SrPageId.Value;
                model.SrPageCode = sr.SrPageCode;

                if (sr.SrPageId.Value == Constants.SRPage.DefaultPageId)
                {
                    model.AddressSendDocId = sr.DraftAccountAddressId;
                    if (sr.AccountAddress != null)
                        model.AddressSendDocText = sr.AccountAddress.AddressDiplay;
                }
                else if (sr.SrPageId.Value == Constants.SRPage.AFSPageId)
                {
                    model.AfsAssetId = sr.AfsAssetId;
                    model.AfsAssetNo = sr.AfsAssetNo;
                    model.AfsAssetDesc = sr.AfsAssetDesc;
                }
                else if (sr.SrPageId.Value == Constants.SRPage.NCBPageId)
                {
                    model.NCBCheckStatuses = _srFacade.GetNCBCheckStatuses().Select(c => new SelectListItem() { Text = c.Key, Value = c.Value }).ToList();
                    model.NCBCheckStatus = sr.NCBCheckStatus;
                    model.NCBBirthDate = sr.NCBCustomerBirthDateDisplay;

                    model.NCBMarketingUserId = sr.NCBMarketingUserId;
                    model.NCBMarketingName = sr.NCBMarketingFullName;
                    model.NCBMarketingBranchId = sr.NCBMarketingBranchId;
                    model.NCBMarketingBranchName = sr.NCBMarketingBranchName;
                    model.NCBMarketingBranchUpper1Id = sr.NCBMarketingBranchUpper1Id;
                    model.NCBMarketingBranchUpper1Name = sr.NCBMarketingBranchUpper1Name;
                    model.NCBMarketingBranchUpper2Id = sr.NCBMarketingBranchUpper2Id;
                    model.NCBMarketingBranchUpper2Name = sr.NCBMarketingBranchUpper2Name;

                    //var userMarketing = _srFacade.GetUserMarketing(model.CustomerId.Value);
                    //if (userMarketing != null)
                    //{
                    //    model.NCBMarketingUserId = userMarketing.UserEntity.UserId;
                    //    model.NCBMarketingName = userMarketing.UserEntity.FullName;
                    //    model.NCBMarketingBranchId = userMarketing.BranchEntity.BranchId;
                    //    model.NCBMarketingBranchName = userMarketing.BranchEntity.BranchName;

                    //    if (userMarketing.UpperBranch1 != null)
                    //    {
                    //        model.NCBMarketingBranchUpper1Id = userMarketing.UpperBranch1.BranchId;
                    //        model.NCBMarketingBranchUpper1Name = userMarketing.UpperBranch1.BranchName;
                    //    }
                    //    if (userMarketing.UpperBranch2 != null)
                    //    {
                    //        model.NCBMarketingBranchUpper2Id = userMarketing.UpperBranch2.BranchId;
                    //        model.NCBMarketingBranchUpper2Name = userMarketing.UpperBranch2.BranchName;
                    //    }
                    //}
                }
            }

            if (isDraftSR)
            {
                return Create(model);
            }
            else
            {
                model.SRStatusId = sr.SRStatus.SRStatusId;
                model.SRStatusName = sr.SRStatus.SRStatusName;
                model.CloseDate = sr.CloseDate;

                if (model.SRStatusId == Constants.SRStatusId.Draft
                    || model.SRStatusId == Constants.SRStatusId.Open
                    || model.SRStatusId == Constants.SRStatusId.WaitingCustomer
                    || model.SRStatusId == Constants.SRStatusId.InProgress
                    || model.SRStatusId == Constants.SRStatusId.RouteBack)
                {
                    model.CanEdit = true;
                }
                else
                {
                    model.CanEdit = false;
                }

                //init activity searchfilter
                model.ActivitySearchFilter = new ActivityTabSearchFilter
                {
                    SrId = model.SrId,
                    SrNo = model.SrNo,
                    CustomerId = model.CustomerId,
                    CustomerCardNo = model.CustomerCardNo,
                    CustomerSubscriptionTypeCode = model.CustomerSubscriptionTypeCode,
                    SrOnly = true,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "",
                    SortOrder = "ASC"
                };

                //init existing searchfilter
                model.ExistingSearchFilter = new ExistingSearchFilter
                {
                    CustomerId = model.CustomerId,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "",
                    SortOrder = "ASC"
                };

                //init document searchfilter
                model.DocumentSearchFilter = new DocumentSearchFilter
                {
                    SrId = model.SrId,
                    CustomerId = model.CustomerId,
                    SrOnly = true,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "CreateDate",
                    SortOrder = "DESC"
                };

                //init logging searchfilter
                model.LoggingSearchFilter = new LoggingSearchFilter()
                {
                    SrId = model.SrId,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "",
                    SortOrder = ""
                };

                model.IsEdit = true;

                return View("Edit", model);
            }
        }

        #endregion

        #region == Copy SR ==

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Copy(int srId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Clone Service Request (SR)").ToInputLogString());

            try
            {
                if (_srFacade == null)
                    _srFacade = new ServiceRequestFacade();

                ServiceRequestForDisplayEntity sr = _srFacade.GetServiceRequest(srId);
                if (sr == null)
                    return RedirectToAction("Index");

                CreateServiceRequestViewModel model = new CreateServiceRequestViewModel(sr);

                return Create(model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Clone Service Request (SR)").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Copy(int? srId)
        //{
        //    if (srId.HasValue)
        //    {
        //        //_srPageFacade = new SrPageFacade();
        //        //var srPageList = _srPageFacade.GetSrPageList();

        //        _srFacade = new ServiceRequestFacade();
        //        ServiceRequestForDisplayEntity sr = _srFacade.GetServiceRequest(srId.Value);

        //        Logger.Info(_logMsg.Clear().SetPrefixMsg("Service Request Copy").ToInputLogString());
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                sr.CreateUser = UserInfo;

        //                var model = new CreateServiceRequestViewModel();

        //                model.SrId = sr.SrId;

        //                model.SrNo = sr.SrNo;
                        
        //                model.CallId = sr.CallId;
        //                model.PhoneNo = sr.PhoneNo;

        //                if (sr.Customer != null)
        //                {
        //                    var customer = sr.Customer;

        //                    model.CustomerId = customer.CustomerId;

        //                    if (sr.CustomerSubscriptType != null)
        //                    {
        //                        model.CustomerSubscriptionTypeCode = sr.CustomerSubscriptType.SubscriptTypeCode;
        //                        model.CustomerSubscriptionName = sr.CustomerSubscriptType.SubscriptTypeName;
        //                    }

        //                    model.CustomerCardNo = customer.CardNo;
        //                    model.CustomerBirthDate = customer.BirthDateDisplay;
        //                    model.CustomerTitleTh = customer.TitleThai != null ? customer.TitleThai.TitleName : string.Empty;
        //                    model.CustomerTitleEn = customer.TitleEnglish != null ? customer.TitleEnglish.TitleName : string.Empty;
        //                    model.CustomerFirstNameTh = customer.FirstNameThai;
        //                    model.CustomerFirstNameEn = customer.FirstNameEnglish;
        //                    model.CustomerLastNameTh = customer.LastNameThai;
        //                    model.CustomerLastNameEn = customer.LastNameEnglish;

        //                    var phoneNotFax = customer.PhoneList.Where(c => c.PhoneTypeCode != Constants.PhoneTypeCode.Fax).ToList();

        //                    if (phoneNotFax.Count > 0)
        //                    {
        //                        model.CustomerPhoneTypeName1 = phoneNotFax[0].PhoneTypeName;
        //                        model.CustomerPhoneNo1 = phoneNotFax[0].PhoneNo;

        //                        if (phoneNotFax.Count > 1)
        //                        {
        //                            model.CustomerPhoneTypeName2 = phoneNotFax[1].PhoneTypeName;
        //                            model.CustomerPhoneNo2 = phoneNotFax[1].PhoneNo;

        //                            if (phoneNotFax.Count > 2)
        //                            {
        //                                model.CustomerPhoneTypeName3 = phoneNotFax[2].PhoneTypeName;
        //                                model.CustomerPhoneNo3 = phoneNotFax[2].PhoneNo;
        //                            }
        //                        }
        //                    }

        //                    var phoneFax = customer.PhoneList.FirstOrDefault(c => c.PhoneTypeCode == Constants.PhoneTypeCode.Fax);

        //                    model.CustomerFax = phoneFax != null ? phoneFax.PhoneNo : string.Empty;
        //                    model.CustomerEmail = customer.Email ?? string.Empty;
        //                }

        //                model.CustomerEmployeeCode = sr.CustomerEmployeeCode ?? string.Empty;

        //                if (sr.Account != null)
        //                {
        //                    var account = sr.Account;
        //                    model.AccountId = account.AccountId;
        //                    model.AccountNo = account.AccountNo;
        //                    model.AccountStatus = account.Status;
        //                    model.AccountCarNo = account.CarNo;
        //                    model.AccountProductGroupName = account.ProductGroup;
        //                    model.AccountProductName = account.Product;
        //                    model.AccountBranchName = account.BranchName;
        //                }

        //                if (sr.Contact != null)
        //                {
        //                    var contact = sr.Contact;

        //                    model.ContactId = contact.ContactId;

        //                    if (sr.ContactSubscriptType != null)
        //                        model.ContactSubscriptionName = sr.ContactSubscriptType.SubscriptTypeName;

        //                    model.ContactCardNo = contact.CardNo;
        //                    model.ContactBirthDate = contact.BirthDateDisplay;
        //                    model.ContactTitleTh = contact.TitleThai != null ? contact.TitleThai.TitleName : string.Empty;
        //                    model.ContactTitleEn = contact.TitleEnglish != null ? contact.TitleEnglish.TitleName : string.Empty;
        //                    model.ContactFirstNameTh = contact.FirstNameThai;
        //                    model.ContactFirstNameEn = contact.FirstNameEnglish;
        //                    model.ContactLastNameTh = contact.LastNameThai;
        //                    model.ContactLastNameEn = contact.LastNameEnglish;

        //                    var phoneNotFax = contact.PhoneList.Where(c => c.PhoneTypeCode != Constants.PhoneTypeCode.Fax).ToList();

        //                    if (phoneNotFax.Count > 0)
        //                    {
        //                        model.ContactPhoneTypeName1 = phoneNotFax[0].PhoneTypeName;
        //                        model.ContactPhoneNo1 = phoneNotFax[0].PhoneNo;

        //                        if (phoneNotFax.Count > 1)
        //                        {
        //                            model.ContactPhoneTypeName2 = phoneNotFax[1].PhoneTypeName;
        //                            model.ContactPhoneNo2 = phoneNotFax[1].PhoneNo;

        //                            if (phoneNotFax.Count > 2)
        //                            {
        //                                model.ContactPhoneTypeName3 = phoneNotFax[2].PhoneTypeName;
        //                                model.ContactPhoneNo3 = phoneNotFax[2].PhoneNo;
        //                            }
        //                        }
        //                    }

        //                    var phoneFax = contact.PhoneList.FirstOrDefault(c => c.PhoneTypeCode == Constants.PhoneTypeCode.Fax);

        //                    model.ContactFax = phoneFax != null ? phoneFax.PhoneNo : string.Empty;
        //                    model.ContactEmail = contact.Email ?? string.Empty;
        //                }

        //                model.ContactAccountNo = sr.ContactAccountNo;
        //                model.ContactRelationshipId = sr.Relationship != null ? (int?)sr.Relationship.RelationshipId : (int?)null;
        //                model.ContactRelationshipName = sr.Relationship != null ? (sr.Relationship.RelationshipName ?? string.Empty) : string.Empty;

        //                if (sr.ProductGroup != null)
        //                {
        //                    model.ProductGroupId = sr.ProductGroup.ProductGroupId;
        //                    model.ProductGroupName = sr.ProductGroup.ProductGroupName;
        //                }

        //                if (sr.Product != null)
        //                {
        //                    model.ProductId = sr.Product.ProductId;
        //                    model.ProductName = sr.Product.ProductName;
        //                }

        //                if (sr.CampaignService != null)
        //                {
        //                    model.CampaignServiceId = sr.CampaignService.CampaignServiceId;
        //                    model.CampaignServiceName = sr.CampaignService.CampaignServiceName;
        //                }

        //                if (sr.Area != null)
        //                {
        //                    model.AreaId = sr.Area.AreaId;
        //                    model.AreaName = sr.Area.AreaName;
        //                }

        //                if (sr.SubArea != null)
        //                {
        //                    model.SubAreaId = sr.SubArea.SubareaId;
        //                    model.SubAreaName = sr.SubArea.SubareaName;
        //                }

        //                if (sr.Type != null)
        //                {
        //                    model.TypeId = sr.Type.TypeId;
        //                    model.TypeName = sr.Type.TypeName;
        //                }

        //                if (sr.Channel != null)
        //                {
        //                    model.ChannelId = sr.Channel.ChannelId;
        //                    model.ChannelName = sr.Channel.Name;
        //                }

        //                if (sr.MediaSource != null)
        //                {
        //                    model.MediaSourceId = sr.MediaSource.MediaSourceId;
        //                    model.MediaSourceName = sr.MediaSource.Name;
        //                }

        //                model.Subject = sr.Subject;
        //                model.Remark = ApplicationHelpers.StripHtmlTags(sr.Remark);

        //                return View("Copy", model);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Info(_logMsg.Clear().SetPrefixMsg("Service Request Copy").ToFailLogString());
        //            return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
        //                         this.ControllerContext.RouteData.Values["action"].ToString()));
        //            //return Json(new { is_success = false, message = string.Format(CultureInfo.InvariantCulture, "Error : {0}", ex.Message) });
        //        }
        //    }

        //    return View("Search");
        //}

        #endregion

        #region == Save Draft SR | Save SR | Update SR ==

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDraft(CreateServiceRequestViewModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Save Draft").ToInputLogString());

            try
            {
                var isSaveDraft = true;
                return CreateServiceRequest(model, isSaveDraft);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Save Draft").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(CreateServiceRequestViewModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Save").ToInputLogString());

            try
            {
                var isSaveDraft = false;
                return CreateServiceRequest(model, isSaveDraft);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Save").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        private ActionResult CreateServiceRequest(CreateServiceRequestViewModel model, bool isSaveDraft)
        {
            var validateResult = ValidateForSave(model, isSaveDraft);

            if (validateResult.IsValid)
            {
                var sr = new ServiceRequestForSaveEntity();

                sr.SrId = model.SrId;
                sr.IsClone = model.IsClone;

                sr.CallId = model.CallId ?? string.Empty;
                sr.PhoneNo = model.PhoneNo ?? string.Empty;

                sr.CustomerId = model.CustomerId.Value;
                sr.AccountId = model.AccountId.Value;
                sr.ContactId = model.ContactId.Value;

                sr.ContactAccountNo = model.ContactAccountNo;
                sr.ContactRelationshipId = model.ContactRelationshipId.Value;
                sr.ProductGroupId = model.ProductGroupId.Value;
                sr.ProductId = model.ProductId.Value;
                sr.CampaignServiceId = model.CampaignServiceId.Value;
                sr.AreaId = model.AreaId.Value;
                sr.SubAreaId = model.SubAreaId.Value;
                sr.TypeId = model.TypeId.Value;
                sr.ChannelId = model.ChannelId.Value;
                sr.MediaSourceId = model.MediaSourceId;
                sr.Subject = model.Subject ?? string.Empty;
                sr.Remark = !string.IsNullOrEmpty(model.Remark) ? ApplicationHelpers.StripHtmlTags(model.Remark) : string.Empty;

                sr.MappingProductId = model.MappingProductId.Value;
                sr.IsVerify = model.IsVerify;
                if (sr.IsVerify)
                {
                    sr.VerifyAnswerJson = model.VerifyAnswerJson;
                    sr.IsVerifyPass = model.IsVerifyPass ?? string.Empty;
                }

                sr.SrPageId = model.SrPageId.Value;

                if (model.SrPageId.Value == 1)
                {
                    sr.AddressSendDocId = model.AddressSendDocId;
                    sr.AddressSendDocText = model.AddressSendDocText ?? string.Empty;
                }
                else if (model.SrPageId.Value == 2)
                {
                    sr.AfsAssetId = model.AfsAssetId;
                    sr.AfsAssetNo = model.AfsAssetNo ?? string.Empty;
                    sr.AfsAssetDesc = model.AfsAssetDesc ?? string.Empty;
                }
                else if (model.SrPageId.Value == 3)
                {
                    sr.NCBBirthDate = DateTime.ParseExact(model.NCBBirthDate, "dd/MM/yyyy", new CultureInfo("th-TH"));
                    sr.NCBCheckStatus = model.NCBCheckStatus;

                    sr.NCBMarketingUserId = model.NCBMarketingUserId;
                    sr.NCBMarketingName = model.NCBMarketingName;
                    sr.NCBMarketingBranchId = model.NCBMarketingBranchId;
                    sr.NCBMarketingBranchName = model.NCBMarketingBranchName;
                    sr.NCBMarketingBranchUpper1Id = model.NCBMarketingBranchUpper1Id;
                    sr.NCBMarketingBranchUpper1Name = model.NCBMarketingBranchUpper1Name;
                    sr.NCBMarketingBranchUpper2Id = model.NCBMarketingBranchUpper2Id;
                    sr.NCBMarketingBranchUpper2Name = model.NCBMarketingBranchUpper2Name;
                }

                sr.OwnerBranchId = model.OwnerBranchId.Value;
                sr.OwnerUserId = model.OwnerUserId.Value;
                sr.DelegateBranchId = model.DelegateBranchId;
                sr.DelegateUserId = model.DelegateUserId;

                sr.SrEmailTemplateId = model.SrEmailTemplateId;

                if (sr.SrEmailTemplateId == null)
                {
                    sr.ActivityDescription = model.ActivityDescription;
                }
                else
                {
                    sr.SendMailSender = model.SendMailSender;
                    sr.SendMailTo = model.SendMailTo;
                    sr.SendMailCc = model.SendMailCc;
                    sr.SendMailSubject = FillEmailParametersPreProcessing(model.SendMailSubject, model);
                    sr.SendMailBody = FillEmailParametersPreProcessing(model.SendMailBody, model);
                    sr.SendMailBody = ApplicationHelpers.StripHtmlTags(sr.SendMailBody);
                }

                sr.ActivityTypeId = model.ActivityTypeId.Value;
                sr.IsEmailDelegate = model.IsEmailDelegate;
                sr.IsClose = model.IsClose;

                sr.AttachmentJson = model.AttachmentJson;

                sr.CreateBranchId = model.CreatorBranchId;
                sr.CreateUserId = model.CreatorUserId;

                if (!isSaveDraft)
                {
                    try
                    {
                        var decodeVerifyAnswerJson = WebUtility.UrlDecode(model.VerifyAnswerJson);
                        if (decodeVerifyAnswerJson != null)
                        {
                            sr.VerifyAnswers = JsonConvert.DeserializeObject<List<SrVerifyAnswerEntity>>(decodeVerifyAnswerJson);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Save").Add("Error Message", ex.Message).ToFailLogString());
                        Logger.Error("Exception occur:\n", ex);
                    }

                    try
                    {
                        var decodeAttachmentJson = WebUtility.UrlDecode(model.AttachmentJson);
                        if (decodeAttachmentJson != null)
                        {
                            sr.SrAttachments = JsonConvert.DeserializeObject<List<SrAttachmentEntity>>(decodeAttachmentJson);
                            sr.IsJob = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Save").Add("Error Message", ex.Message).ToFailLogString());
                        Logger.Error("Exception occur:\n", ex);
                    }
                }

                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.ServiceRequest;
                _auditLog.Action = Constants.AuditAction.ActivityLog;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.CreateUserId = this.UserInfo.UserId;

                _srFacade = new ServiceRequestFacade();
                var result = _srFacade.CreateServiceRequest(_auditLog, sr, isSaveDraft);
                return Json(new
                {
                    result.IsSuccess,
                    result.ErrorMessage,
                    result.WarningMessages,
                    IsSaveDraft = isSaveDraft,
                    result.SrNo,
                });
            }
            else
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Service Request (SR) - Save").Add("Error Message", validateResult.ErrorMessage).ToFailLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = validateResult.ErrorMessage,
                });
            }
        }

        private string FillEmailParametersPreProcessing(string original, CreateServiceRequestViewModel model)
        {
            var tmp = original;

            tmp = tmp.Replace("%CUSTOMER_FIRST_NAME%", model.CustomerFirstNameTh);
            tmp = tmp.Replace("%CUSTOMER_LAST_NAME%", model.CustomerLastNameTh);
            tmp = tmp.Replace("%CUSTOMER_PHONE_NO%", model.CustomerPhoneNo1);

            tmp = tmp.Replace("%ACCOUNT_NO%", model.AccountNo);

            tmp = tmp.Replace("%CONTACT_FIRST_NAME%", model.ContactFirstNameTh);
            tmp = tmp.Replace("%CONTACT_LAST_NAME%", model.ContactLastNameTh);
            tmp = tmp.Replace("%CONTACT_PHONE_NO%", model.ContactPhoneNo1);

            tmp = tmp.Replace("%BRANCH_CODE%", model.CreatorBranchCode);
            tmp = tmp.Replace("%BRANCH_NAME%", model.CreatorBranchName);

            tmp = tmp.Replace("%PRODUCTGROUP_NAME%", model.ProductGroupName);
            tmp = tmp.Replace("%PRODUCT_NAME%", model.ProductName);
            tmp = tmp.Replace("%CAMPAIGNSERVICE_NAME%", model.CampaignServiceName);
            tmp = tmp.Replace("%TYPE_NAME%", model.TypeName);
            tmp = tmp.Replace("%AREA_NAME%", model.AreaName);
            tmp = tmp.Replace("%SUBAREA_NAME%", model.SubAreaName);
            tmp = tmp.Replace("%CHANNEL_NAME%", model.ChannelName);
            tmp = tmp.Replace("%REMARK%", model.Remark);

            tmp = tmp.Replace("%OWNER%", model.OwnerUserFullName);
            tmp = tmp.Replace("%MARKETING_NAME%", model.NCBMarketingName);
            tmp = tmp.Replace("%CREATE_BY%", model.CreatorUserFullName);

            if (_commonFacade == null)
                _commonFacade = new CommonFacade();

            tmp = tmp.Replace("%OFFICE_PHONE_NO%", _commonFacade.GetOfficePhoneNo());
            tmp = tmp.Replace("%OFFICE_HOUR%", _commonFacade.GetOfficeHour());

            return tmp;
        }

        private ValidateResult ValidateForSave(CreateServiceRequestViewModel model, bool isSaveDraft)
        {
            if (model == null)
                return new ValidateResult(false, "Technical Error: Model is null");

            if (model.CustomerId == null || model.CustomerId.Value == 0)
                return new ValidateResult(false, "Technical Error: CustomerId is null of 0");

            if (model.AccountId == null || model.AccountId.Value == 0)
                return new ValidateResult(false, "Technical Error: AccountId is null of 0");

            if (model.ContactId == null || model.ContactId.Value == 0)
                return new ValidateResult(false, "Technical Error: ContactId is null of 0");

            if (model.ContactRelationshipId == null || model.ContactRelationshipId.Value == 0)
                return new ValidateResult(false, "Technical Error: ContactRelationshipId is null of 0");

            if (string.IsNullOrEmpty(model.Subject))
                return new ValidateResult(false, "Technical Error: Subject is null or empty");

            if (model.ProductGroupId == null || model.ProductGroupId.Value == 0)
                return new ValidateResult(false, "Technical Error: ProductGroupId is null of 0");

            if (model.ProductId == null || model.ProductId.Value == 0)
                return new ValidateResult(false, "Technical Error: ProductId is null of 0");

            if (model.CampaignServiceId == null || model.CampaignServiceId.Value == 0)
                return new ValidateResult(false, "Technical Error: CampaignServiceId is null of 0");

            if (model.AreaId == null || model.AreaId.Value == 0)
                return new ValidateResult(false, "Technical Error: AreaId is null of 0");

            if (model.SubAreaId == null || model.SubAreaId.Value == 0)
                return new ValidateResult(false, "Technical Error: SubAreaId is null of 0");

            if (model.TypeId == null || model.TypeId.Value == 0)
                return new ValidateResult(false, "Technical Error: AreaId is null of 0");

            if (model.ChannelId == null || model.ChannelId.Value == 0)
                return new ValidateResult(false, "Technical Error: ChannelId is null of 0");

            if (model.OwnerUserId == null || model.OwnerUserId.Value == 0)
                return new ValidateResult(false, "Technical Error: OwnerUserId is null of 0");

            if (model.MappingProductId == null || model.MappingProductId.Value == 0)
                return new ValidateResult(false, "Technical Error: MappingProductId is null of 0");

            if (model.IsVerify && string.IsNullOrEmpty(model.IsVerifyPass))
                return new ValidateResult(false, "Technical Error: IsVerifyPass is null or empty (Cannot null or empty when IsVerify is true)");

            if (model.ActivityTypeId == null || model.ActivityTypeId.Value == 0)
                return new ValidateResult(false, "Technical Error: ActivityTypeId is null of 0");

            if (model.SrEmailTemplateId == null)
            {
                // Not Send Email

                if (string.IsNullOrEmpty(model.ActivityDescription))
                    return new ValidateResult(false, "Technical Error: ActivityDescription is null or empty (Cannot null or empty when Not Send Email)");
            }
            else
            {
                // Send Email
                if (string.IsNullOrEmpty(model.SendMailSender))
                    return new ValidateResult(false, "Technical Error: SendMailSender is null or empty (Cannot null or empty when Send Email)");

                if (string.IsNullOrEmpty(model.SendMailTo))
                    return new ValidateResult(false, "Technical Error: SendMailTo is null or empty (Cannot null or empty when Send Email)");

                if (string.IsNullOrEmpty(model.SendMailSubject))
                    return new ValidateResult(false, "Technical Error: SendMailSubject is null or empty (Cannot null or empty when Send Email)");

                if (string.IsNullOrEmpty(model.SendMailBody))
                    return new ValidateResult(false, "Technical Error: SendMailBody is null or empty (Cannot null or empty when Send Email)");

                if (model.SendMailBody.Length > Constants.EmailBodyMaxLength)
                    return new ValidateResult(false, string.Format(Resource.ValErr_StringLength, "E-Mail Body", Constants.EmailBodyMaxLength));

            }

            if (model.SrPageId == null || model.SrPageId.Value == 0)
                return new ValidateResult(false, "Technical Error: SrPageId is null of 0");

            if (model.SrPageId.Value == 1)
            {
                // No Verify

            }
            else if (model.SrPageId.Value == 2)
            {
                if (model.AfsAssetId == null || model.AfsAssetId.Value == 0)
                    return new ValidateResult(false, "Technical Error: AfsAssetId is null of 0 (Cannot null or 0 when SRPage is AFS)");
            }
            else if (model.SrPageId.Value == 3)
            {
                if (string.IsNullOrEmpty(model.NCBBirthDate))
                    return new ValidateResult(false, "Technical Error: NCB Birth Date is null or empty (Cannot null or empty when SRPage is NCB)");

                try
                {
                    DateTime.ParseExact(model.NCBBirthDate, "dd/MM/yyyy", new CultureInfo("th-TH"));
                }
                catch (Exception ex)
                {
                    return new ValidateResult(false, "Technical Error: NCB Birth Date is wrong format. (dd/MM/yyyy with Thai Buddhist year) : " + model.NCBBirthDate);
                }

                if (string.IsNullOrEmpty(model.NCBCheckStatus))
                    return new ValidateResult(false, "Technical Error: NCB CheckStatus is null or empty (Cannot null or empty when SRPage is NCB)");
            }

            return new ValidateResult(true, string.Empty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateServiceRequest(CreateServiceRequestViewModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Update Service Request Information").ToInputLogString());

            var validateResult = ValidateForUpdate(model);

            if (validateResult.IsValid)
            {
                var sr = new ServiceRequestForSaveEntity();

                sr.SrId = model.SrId;

                sr.Subject = model.Subject ?? string.Empty;
                sr.Remark = !string.IsNullOrEmpty(model.Remark) ? ApplicationHelpers.StripHtmlTags(model.Remark) : string.Empty;

                sr.SrPageId = model.SrPageId.Value;

                if (model.SrPageId.Value == 1)
                {
                    sr.AddressSendDocId = model.AddressSendDocId;
                    sr.AddressSendDocText = model.AddressSendDocText ?? string.Empty;
                }
                else if (model.SrPageId.Value == 2)
                {
                    sr.AfsAssetId = model.AfsAssetId;
                    sr.AfsAssetNo = model.AfsAssetNo ?? string.Empty;
                    sr.AfsAssetDesc = model.AfsAssetDesc ?? string.Empty;
                }
                else if (model.SrPageId.Value == 3)
                {
                    sr.NCBBirthDate = DateTime.ParseExact(model.NCBBirthDate, "dd/MM/yyyy", new CultureInfo("th-TH"));
                    sr.NCBCheckStatus = model.NCBCheckStatus;

                    sr.NCBMarketingUserId = model.NCBMarketingUserId;
                    sr.NCBMarketingName = model.NCBMarketingName;
                    sr.NCBMarketingBranchId = model.NCBMarketingBranchId;
                    sr.NCBMarketingBranchName = model.NCBMarketingBranchName;
                    sr.NCBMarketingBranchUpper1Id = model.NCBMarketingBranchUpper1Id;
                    sr.NCBMarketingBranchUpper1Name = model.NCBMarketingBranchUpper1Name;
                    sr.NCBMarketingBranchUpper2Id = model.NCBMarketingBranchUpper2Id;
                    sr.NCBMarketingBranchUpper2Name = model.NCBMarketingBranchUpper2Name;
                }
                sr.CreateUserId = UserInfo.UserId;

                _srFacade = new ServiceRequestFacade();
                _srFacade.UpdateServiceRequest(sr);

                return Json(new
                {
                    IsSuccess = true,
                });
            }
            else
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Update Service Request").Add("Error Message", validateResult.ErrorMessage).ToFailLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = validateResult.ErrorMessage,
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CopyServiceRequest(CreateServiceRequestViewModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Copy Service Request Information").ToInputLogString());

            var entity = new ServiceRequestForSaveEntity();

            entity.SrId = model.SrId;

            entity.CreateUserId = UserInfo.UserId;
            entity.CreateDate = DateTime.Now;

            _srFacade = new ServiceRequestFacade();
            ServiceRequestSaveResult result=_srFacade.CopyServiceRequest(entity);

            if (result.IsSuccess)
            {
                return Json(new
                {
                    IsSuccess = true,
                });
            }
            else {
                return Json(new
                {
                    IsSuccess = false,
                    WarningMessages = result.WarningMessages,
                });
            }
        }

        private ValidateResult ValidateForUpdate(CreateServiceRequestViewModel model)
        {
            if (model == null)
                return new ValidateResult(false, "Technical Error: Model is null");

            if (model.SrId == null || model.SrId.Value == 0)
                return new ValidateResult(false, "Technical Error: SrId is null of 0");

            if (string.IsNullOrEmpty(model.Subject))
                return new ValidateResult(false, "Technical Error: Subject null of empty");

            if (model.SrPageId == null || model.SrPageId.Value == 0)
                return new ValidateResult(false, "Technical Error: SrPageId is null of 0");

            if (string.IsNullOrEmpty(model.SrPageCode))
                return new ValidateResult(false, "Technical Error: SrPageCode null of empty");

            if (model.SrPageId.Value == 1)
            {
                // No Verify

            }
            else if (model.SrPageId.Value == 2)
            {
                if (model.AfsAssetId == null || model.AfsAssetId.Value == 0)
                    return new ValidateResult(false, "Technical Error: AfsAssetId is null of 0 (Cannot null or 0 when SRPage is AFS)");
            }
            else if (model.SrPageId.Value == 3)
            {
                if (string.IsNullOrEmpty(model.NCBBirthDate))
                    return new ValidateResult(false, "Technical Error: NCB Birth Date is null or empty (Cannot null or empty when SRPage is NCB)");

                try
                {
                    DateTime.ParseExact(model.NCBBirthDate, "dd/MM/yyyy", new CultureInfo("th-TH"));
                }
                catch (Exception ex)
                {
                    return new ValidateResult(false, "Technical Error: NCB Birth Date is wrong format. (dd/MM/yyyy with Thai Buddhist year) : " + model.NCBBirthDate);
                }

                if (string.IsNullOrEmpty(model.NCBCheckStatus))
                    return new ValidateResult(false, "Technical Error: NCB CheckStatus is null or empty (Cannot null or empty when SRPage is NCB)");
            }

            return new ValidateResult(true, string.Empty);
        }

        #endregion

        #region == Create Activity ==

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveActivity(CreateServiceRequestViewModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Create SR Activity").ToInputLogString());

            try
            {
                var validateResult = ValidateForSaveActivity(model);

                if (validateResult.IsValid)
                {
                    var sr = new ServiceRequestForSaveEntity();

                    sr.SrId = model.SrId;

                    sr.OwnerBranchId = model.OwnerBranchId.Value;
                    sr.OwnerUserId = model.OwnerUserId.Value;
                    sr.DelegateBranchId = model.DelegateBranchId;
                    sr.DelegateUserId = model.DelegateUserId;
                    sr.SrStatusId = model.SRStatusId;
                    sr.IsEmailDelegate = model.IsEmailDelegate;

                    sr.SrEmailTemplateId = model.SrEmailTemplateId;
                    if (sr.SrEmailTemplateId == null)
                    {
                        sr.ActivityDescription = model.ActivityDescription;
                    }
                    else
                    {
                        sr.SendMailSender = model.SendMailSender;
                        sr.SendMailTo = model.SendMailTo;
                        sr.SendMailCc = model.SendMailCc;
                        sr.SendMailSubject = model.SendMailSubject;
                        sr.SendMailBody = ApplicationHelpers.StripHtmlTags(model.SendMailBody);
                    }

                    sr.ActivityTypeId = model.ActivityTypeId.Value;
                    sr.AttachmentJson = model.AttachmentJson;

                    sr.CreateBranchId = UserInfo.BranchId;
                    sr.CreateUserId = UserInfo.UserId;

                    if (!string.IsNullOrEmpty(model.AttachmentJson))
                    {
                        var decodeAttachmentJson = WebUtility.UrlDecode(model.AttachmentJson);
                        sr.SrAttachments = JsonConvert.DeserializeObject<List<SrAttachmentEntity>>(decodeAttachmentJson);
                    }

                    _auditLog = new AuditLogEntity();
                    _auditLog.Module = Constants.Module.ServiceRequest;
                    _auditLog.Action = Constants.AuditAction.ActivityLog;
                    _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                    _auditLog.CreateUserId = this.UserInfo.UserId;

                    _srFacade = new ServiceRequestFacade();
                    var result = _srFacade.CreateServiceRequestActivity(_auditLog, sr, false);
                    return Json(new
                    {
                        result.IsSuccess,
                        result.ErrorMessage,
                        result.WarningMessages,
                    });
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Create SR Activity").Add("Error Message", validateResult.ErrorMessage).ToFailLogString());
                    return Json(new
                    {
                        IsSuccess = false,
                        ErrorMessage = validateResult.ErrorMessage,
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Create SR Activity").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                });
            }
        }

        private ValidateResult ValidateForSaveActivity(CreateServiceRequestViewModel model)
        {
            if (model == null)
                return new ValidateResult(false, "Technical Error: Model is null");

            if (model.OwnerUserId == null || model.OwnerUserId.Value == 0)
                return new ValidateResult(false, "Technical Error: ActivityTypeId is null of 0");

            if (model.ActivityTypeId == null || model.ActivityTypeId.Value == 0)
                return new ValidateResult(false, "Technical Error: ActivityTypeId is null of 0");

            if (model.SrEmailTemplateId == null)
            {
                // Not Send Email

                if (string.IsNullOrEmpty(model.ActivityDescription))
                    return new ValidateResult(false, "Technical Error: ActivityDescription is null or empty (Cannot null or empty when Not Send Email)");
            }
            else
            {
                // Send Email
                if (string.IsNullOrEmpty(model.SendMailSender))
                    return new ValidateResult(false, "Technical Error: SendMailSender is null or empty (Cannot null or empty when Send Email)");

                if (string.IsNullOrEmpty(model.SendMailTo))
                    return new ValidateResult(false, "Technical Error: SendMailTo is null or empty (Cannot null or empty when Send Email)");

                if (string.IsNullOrEmpty(model.SendMailSubject))
                    return new ValidateResult(false, "Technical Error: SendMailSubject is null or empty (Cannot null or empty when Send Email)");

                if (string.IsNullOrEmpty(model.SendMailBody))
                    return new ValidateResult(false, "Technical Error: SendMailBody is null or empty (Cannot null or empty when Send Email)");

                if (model.SendMailBody.Length > Constants.EmailBodyMaxLength)
                    return new ValidateResult(false, string.Format(Resource.ValErr_StringLength, "E-Mail Body", Constants.EmailBodyMaxLength));

            }

            return new ValidateResult(true, string.Empty);
        }

        #endregion

        #region == Modal Search Customer ==

        public ActionResult SearchCustomer()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Customer").ToInputLogString());

            try
            {
                var model = new LookupCustomerViewModel();
                model.CustomerSearchFilter = new CustomerSearchFilter
                {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    CardNo = string.Empty,
                    Registration = string.Empty,
                    AccountNo = string.Empty,
                    PhoneNo = string.Empty,
                    PageSize = 15,
                    PageNo = 1,
                    SortField = "field",
                    SortOrder = "ASC"
                };
                return View(model);
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
        public ActionResult SearchCustomer(CustomerSearchFilter customerSearchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Customer").ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                var model = new LookupCustomerViewModel();

                model.CustomerSearchFilter = customerSearchFilter;
                model.CustomerTableList = _srFacade.SearchCustomer(customerSearchFilter);
                ViewBag.PageSize = customerSearchFilter.PageSize;

                return PartialView("~/Views/ServiceRequest/_LookupCustomerList.cshtml", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Customer").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCustomerAccount(int accountId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get CustomerAccount").Add("AccountId", accountId).ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                var result = _srFacade.GetCustomerAccount(accountId);
                if (result == null)
                {
                    return Json(new AjaxResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Not Found Data",
                    });
                }

                var customer = result.Customer;
                var account = result.Account;

                var employeeCode = string.Empty;
                if (customer.EmployeeId.HasValue)
                {
                    var user = _srFacade.GetUserById(customer.EmployeeId.Value);
                    if (user != null && !string.IsNullOrEmpty(user.EmployeeCode))
                    {
                        employeeCode = user.EmployeeCode;
                    }
                }

                var phoneNo1 = string.Empty;
                var phoneNo2 = string.Empty;
                var phoneNo3 = string.Empty;
                var phoneExt1 = string.Empty;
                var phoneExt2 = string.Empty;
                var phoneExt3 = string.Empty;
                var phoneTypeName1 = string.Empty;
                var phoneTypeName2 = string.Empty;
                var phoneTypeName3 = string.Empty;

                if (customer.PhoneList.Count > 0)
                {
                    phoneNo1 = customer.PhoneList[0].PhoneNo;
                    phoneTypeName1 = customer.PhoneList[0].PhoneTypeName;

                    if (customer.PhoneList.Count > 1)
                    {
                        phoneNo2 = customer.PhoneList[1].PhoneNo;
                        phoneTypeName2 = customer.PhoneList[1].PhoneTypeName;

                        if (customer.PhoneList.Count > 2)
                        {
                            phoneNo3 = customer.PhoneList[2].PhoneNo;
                            phoneTypeName3 = customer.PhoneList[2].PhoneTypeName;
                        }
                    }
                }

                return Json(new AjaxResult
                {
                    IsSuccess = true,
                    Data = new
                    {
                        AccountId = account.AccountId,
                        CustomerId = NumberUtil.ToStringAsInt(customer.CustomerId),
                        CustomerSubscriptionName = customer.SubscriptType != null ? customer.SubscriptType.SubscriptTypeName : string.Empty,
                        CustomerCardNo = customer.CardNo,
                        CustomerBirthDate = customer.BirthDateDisplay,
                        CustomerTitleTh = customer.TitleThai != null ? customer.TitleThai.TitleName : string.Empty,
                        CustomerFirstNameTh = customer.FirstNameThai,
                        CustomerLastNameTh = customer.LastNameThai,
                        CustomerTitleEn = customer.TitleEnglish != null ? customer.TitleEnglish.TitleName : string.Empty,
                        CustomerFirstNameEn = customer.FirstNameEnglish,
                        CustomerLastNameEn = customer.LastNameEnglish,
                        CustomerPhoneTypeName1 = phoneTypeName1,
                        CustomerPhoneTypeName2 = phoneTypeName2,
                        CustomerPhoneTypeName3 = phoneTypeName3,
                        CustomerPhoneNo1 = phoneNo1,
                        CustomerPhoneNo2 = phoneNo2,
                        CustomerPhoneNo3 = phoneNo3,
                        CustomerPhoneExt1 = phoneExt1,
                        CustomerPhoneExt2 = phoneExt2,
                        CustomerPhoneExt3 = phoneExt3,
                        CustomerFax = customer.Fax,
                        CustomerEmail = customer.Email,
                        CustomerEmployeeCode = employeeCode,
                        AccountNo = account.AccountNo,
                        AccountStatus = account.Status,
                        CarNo = account.CarNo,
                        ProductGroupName = account.ProductGroup,
                        ProductName = account.Product,
                        BranchName = account.BranchName,
                    },
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get CustomerAccount").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new AjaxResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                });
            }
        }

        #endregion

        #region == Modal Search Account ==

        public ActionResult SearchAccount()
        {
            var model = new LookupContractViewModel();
            model.ContactSearchFilter = new ContractSearchFilter()
            {
                AccountNo = string.Empty,
                BranchName = string.Empty,
                CarNo = string.Empty,
                ProductGroupName = string.Empty,
                ProductName = string.Empty,
                Status = string.Empty,
                PageNo = 1,
                PageSize = 15,
                SortField = "field",
                SortOrder = "ASC"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchAccount(ContractSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Account").ToInputLogString());
            try
            {
                _srFacade = new ServiceRequestFacade();
                var model = new LookupContractViewModel();

                model.ContactSearchFilter = searchFilter;
                model.ContractTableList = _srFacade.SearchAccount(searchFilter);
                ViewBag.PageSize = searchFilter.PageSize;

                return PartialView("~/Views/ServiceRequest/_LookupContractList.cshtml", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Account").Add("Error Message", ex).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        #endregion

        #region == Modal Search Contact ==

        public ActionResult SearchContact()
        {
            var model = new LookupCustomerContactViewModel();
            model.CustomerContactSearchFilter = new CustomerContactSearchFilter
            {
                AccountNo = string.Empty,
                CardNo = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                PhoneNo = string.Empty,
                PageNo = 1,
                PageSize = 15,
                SortField = "field",
                SortOrder = "ASC"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchContact(CustomerContactSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search CustomerContact").ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                var model = new LookupCustomerContactViewModel();

                model.CustomerContactSearchFilter = searchFilter;
                model.ContactTableList = _srFacade.SearchContact(searchFilter);

                ViewBag.PageSize = searchFilter.PageSize;

                return PartialView("~/Views/ServiceRequest/_LookupCustomerContactList.cshtml", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search CustomerContact").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCampaignService(int campaignserviceId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get CampaignService").Add("CampaignserviceId", campaignserviceId).ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                var campaignService = _srFacade.GetCampaignService(campaignserviceId);
                return Json(new
                {
                    IsSuccess = true,
                    CampaignServiceId = campaignService.CampaignServiceId,
                    CampaignServiceName = campaignService.CampaignServiceName,
                    ProductGroupId = campaignService.ProductGroupId,
                    ProductGroupName = campaignService.ProductGroupName,
                    ProductId = campaignService.ProductId,
                    ProductName = campaignService.ProductName,
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get CampaignService").Add("Error Message", ex.Message).ToInputLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        #endregion

        #region == Modal Search Account Address ==
        public ActionResult SearchAccountAddress(int customerId)
        {
            var model = new LookupAccountAddressViewModel();
            model.ContactAddressFilter = new AccountAddressSearchFilter()
            {
                CustomerId = customerId,
                Address = string.Empty,
                Type = string.Empty,
                PageNo = 1,
                PageSize = 15,
                SortField = "field",
                SortOrder = "ASC"
            };

            _srFacade = new ServiceRequestFacade();
            var list = _srFacade.AddressTypeList();

            model.AddressTypeList = list.Select(l => l.name).Distinct().Select(item => new SelectListItem
            {
                Text = item,
                Value = item,
            }).Distinct().ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchAccountAddress(AccountAddressSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Contract").ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                var model = new LookupAccountAddressViewModel();

                model.ContactAddressFilter = searchFilter;
                model.AddressAddressTableList = _srFacade.SearchAccountAddress(searchFilter);
                ViewBag.PageSize = searchFilter.PageSize;

                return PartialView("~/Views/ServiceRequest/_LookupAccountAddressList.cshtml", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Contract").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetAssetInfo(int afsAssetId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Get Asset Info").Add("AfsAssetId", afsAssetId).ToInputLogString());

            try
            {
                const string assetInfoTemplate =
                    "ประเภททรัพย์: {0}\n" +
                    "สถานะ: {1}\n" +
                    "ที่ตั้ง: {2} {3}\n" +
                    "Sale: {4},{5},{6},{7}";

                _srFacade = new ServiceRequestFacade();
                AfsAssetEntity result = _srFacade.GetAssetInfo(afsAssetId);

                if (result == null)
                    return Json(new { IsSuccess = false });

                var saleOwner = result.SaleOwnerUser;
                var saleOwnerBranch = result.SaleOwnerBranch;

                return Json(new
                {
                    IsSuccess = true,
                    AssetInfo = string.Format(CultureInfo.InvariantCulture, assetInfoTemplate,
                        result.ProjectDes,
                        result.StatusDesc,
                        result.Amphur,
                        result.Province,
                        result.SaleName,
                        result.PhoneNo,
                        result.MobileNo,
                        result.Email),
                    SaleOwnerUserId = (saleOwner != null) ? saleOwner.UserId + "" : "",
                    SaleOwnerUserFullName = (saleOwner != null) ? saleOwner.FullName : "",
                    SaleOwnerBranchId = (saleOwnerBranch != null) ? saleOwnerBranch.BranchId + "" : "",
                    SaleOwnerBranchName = (saleOwnerBranch != null) ? saleOwnerBranch.BranchName : "",
                });

            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Auto Complete :: Get Asset Info").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new { IsSuccess = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetSrEmailTemplate(int? id)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get SR EmailTemplate").Add("EmailTempId", id).ToInputLogString());

            try
            {
                if (!id.HasValue)
                    return Json(new { IsSuccess = false });

                _srFacade = new ServiceRequestFacade();
                var result = _srFacade.GetSrEmailTemplate(id.Value);
                if (result == null)
                {
                    return Json(new AjaxResult()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Not Found Data",
                    });
                }

                if (string.IsNullOrEmpty(result.Sender))
                {
                    result.Sender = UserInfo.Email;
                }

                return Json(new
                {
                    IsSuccess = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get SR EmailTemplate").Add("Error Message", ex.Message).ToInputLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult SaveSrAttachment(CreateServiceRequestViewModel model, int fileSize, string fileNameList)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Attachment").Add("FileSize", fileSize).Add("FileNameList", fileNameList).ToInputLogString());

            try
            {
                var attach = new AttachmentEntity();
                _commonFacade = new CommonFacade();

                if (model.DocName.Length > 200)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Valid = false,
                        Error = string.Empty,
                        Message = "ไม่สามารถบันทึก sr ได้ เนื่องจากชื่อเอกสารเกิน 200 ตัวอักษร"
                    });
                }

                var file = model.FileAttach;

                #region "Save file"

                if (file != null && file.ContentLength > 0)
                {
                    //check file size
                    ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);
                    int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();

                    if (file.ContentLength > limitSingleFileSize.Value)
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Message = string.Format(CultureInfo.InvariantCulture, Resource.ValError_SingleFileSizeExceedMaxLimit, (limitSingleFileSize.Value / 1048576))
                        });
                    }

                    // extract only the filename
                    var fileName = Path.GetFileName(file.FileName);
                    // validate file name
                    var list = new JavaScriptSerializer().Deserialize<FileNameViewModel[]>(fileNameList);
                    if (list.Any(item => model.DocName == item.Name))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Message = "ชื่อ Document มีอยู่ในระบบกรุณาตรวจสอบอีกครั้ง"
                        });
                    }

                    ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexFileExt);
                    Match match = Regex.Match(fileName, param.ParamValue, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        ModelState.AddModelError("FileAttach", Resource.ValError_FileExtension);
                        goto Outer;
                    }

                    var docFolder = _commonFacade.GetSrDocumentFolder();
                    int seqNo = _commonFacade.GetNextAttachmentSeq();

                    var fileNameUrl = ApplicationHelpers.GenerateFileName(docFolder, Path.GetExtension(file.FileName), seqNo, Constants.AttachmentPrefix.Sr);

                    var targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", docFolder, fileNameUrl);
                    file.SaveAs(targetFile);

                    attach.Url = fileNameUrl;
                    attach.Filename = fileName;
                    attach.ContentType = file.ContentType;
                    attach.FileSize = file.ContentLength;
                }

                #endregion

                var selectedAttachType = model.DocTypeCheckBoxes.Where(x => x.Checked)
                        .Select(x => new AttachmentTypeEntity
                        {
                            DocTypeId = x.Value.ToNullable<int>(),
                            Name = x.Text,
                            CreateUserId = this.UserInfo.UserId
                        }).ToList();

                // Validate total file size
                ParameterEntity paramTotalFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.TotalFileSize);
                int? limitTotalFileSize = paramTotalFileSize.ParamValue.ToNullable<int>();

                if (fileSize > limitTotalFileSize)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Message =
                            string.Format(CultureInfo.InvariantCulture, Resource.ValError_TotalFileSizeExceedMaxLimit,
                                (limitTotalFileSize.Value / 1048576))
                    });
                }

                return Json(new
                {
                    IsSuccess = true,
                    attach.Url,
                    attach.Filename,
                    attach.ContentType,
                    attach.FileSize,
                    model.DocDesc,
                    model.ExpiryDate,
                    model.AttachToEmail,
                    model.DocName,
                    Status = 1,
                    SelectedAttachType = selectedAttachType,
                    CreateDate = DateUtil.ToStringAsDateTime(DateTime.Now),
                    CreateUserId = UserInfo.UserId,
                    CreateUserName = UserInfo.Username
                });

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
                ModelState.AddModelError("FileAttach", ex.Message);
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Attachment").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    IsSuccess = false,
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCustomer(CustomerViewModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Customer").Add("PhoneNo1", model.PhoneNo1).ToInputLogString());
            try
            {
                //validate cardno
                if (!string.IsNullOrEmpty(model.SubscriptType))
                {
                    _commonFacade = new CommonFacade();
                    var subscriptTypePersonal = _commonFacade.GetSubscriptTypeByCode(Constants.SubscriptTypeCode.Personal);

                    if (string.IsNullOrEmpty(model.CardNo))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            IsDuplicate = false,
                            Message = "กรุณากรอกข้อมูล CardNo."
                        });
                    }
                    else if (model.SubscriptType.ToNullable<int>() == subscriptTypePersonal.SubscriptTypeId)
                    {
                        if (!ApplicationHelpers.ValidateCardNo(model.CardNo))
                        {
                            return Json(new
                            {
                                IsSuccess = false,
                                IsDuplicate = false,
                                Message = "Subscription ID ที่ระบุไม่ถูกต้อง กรุณาตรวจสอบข้อมูล"
                            });
                        }
                    }

                    #region "Check Duplicate CardNo"

                    _customerFacade = new CustomerFacade();
                    if (_customerFacade.IsDuplicateCardNo(model.CustomerId,
                        model.SubscriptType.ToNullable<int>(), model.CardNo))
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            IsDuplicate = false,
                            Message = Resource.ValError_DuplicateCardNo
                        });
                    }

                    #endregion
                }

                var customerEntity = new CustomerEntity
                {
                    CustomerId = model.CustomerId,
                    SubscriptType = new SubscriptTypeEntity
                    {
                        SubscriptTypeId = model.SubscriptType.ToNullable<int>()
                    },
                    CardNo = model.CardNo,
                    BirthDate = model.BirthDateValue,
                    TitleThai = new TitleEntity
                    {
                        TitleId = model.TitleThai.ToNullable<int>()
                    },
                    FirstNameThai = model.FirstNameThai,
                    LastNameThai = model.LastNameThai,
                    TitleEnglish = new TitleEntity
                    {
                        TitleId = model.TitleEnglish.ToNullable<int>()
                    },
                    FirstNameEnglish = model.FirstNameEnglish,
                    LastNameEnglish = model.LastNameEnglish,
                    Email = model.Email,
                    CreateUser = new UserEntity
                    {
                        UserId = this.UserInfo.UserId
                    },
                    UpdateUser = new UserEntity
                    {
                        UserId = this.UserInfo.UserId
                    }
                };

                #region "Phone and Fax"

                customerEntity.PhoneList = new List<PhoneEntity>();
                if (!string.IsNullOrEmpty(model.PhoneNo1))
                {
                    customerEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = model.PhoneType1.ToNullable<int>(), PhoneNo = model.PhoneNo1 });
                }
                if (!string.IsNullOrEmpty(model.PhoneNo2))
                {
                    customerEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = model.PhoneType2.ToNullable<int>(), PhoneNo = model.PhoneNo2 });
                }
                if (!string.IsNullOrEmpty(model.PhoneNo3))
                {
                    customerEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = model.PhoneType3.ToNullable<int>(), PhoneNo = model.PhoneNo3 });
                }
                // Fax
                if (!string.IsNullOrEmpty(model.Fax))
                {
                    _commonFacade = new CommonFacade();
                    var phoneTypeFax = _commonFacade.GetPhoneTypeByCode(Constants.PhoneTypeCode.Fax);
                    customerEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = phoneTypeFax.PhoneTypeId, PhoneNo = model.Fax });
                }
                #endregion

                _customerFacade = new CustomerFacade();
                _srFacade = new ServiceRequestFacade();

                // Validate duplicate phone
                if (model.IsConfirm != "true")
                {
                    List<string> lstPhoneNo = new List<string>();
                    if (!string.IsNullOrEmpty(model.PhoneNo1)) lstPhoneNo.Add(model.PhoneNo1);
                    if (!string.IsNullOrEmpty(model.PhoneNo2)) lstPhoneNo.Add(model.PhoneNo2);
                    if (!string.IsNullOrEmpty(model.PhoneNo3)) lstPhoneNo.Add(model.PhoneNo3);

                    if (lstPhoneNo.Count > 0)
                    {
                        var isPhoneDuplicate = _srFacade.CheckDuplicatePhoneNo(lstPhoneNo);
                        if (isPhoneDuplicate)
                        {
                            var customerAccounts = _srFacade.GetCustomerAccountByPhoneNo(lstPhoneNo);

                            var sb = new StringBuilder();
                            sb.Append("<table class='table table-hover datatable'>");
                            sb.Append(@"<thead>
                <tr>
                    <th>Action</th>
                    <th>Product</th>
                    <th>Subscription ID</th>
                    <th>ชื่อลูกค้า</th>
                    <th>นามสกุล<br>ลูกค้า</th>
                    <th>เลขที่บัญชี<br>/สัญญา</th>
                    <th>ทะเบียน<br>รถยนต์</th>
                    <th>เบอร์โทรศัพท์</th>
                    <th>สถานะบัญชี<br>/สัญญา</th>
                    <th>สาขา</th>
                    <th>Customer<br>Type</th>
                    <th>Subscription<br>Type</th>
                </tr>
            </thead>");
                            sb.Append("<tbody>");

                            foreach (var item in customerAccounts)
                            {
                                var status = string.IsNullOrEmpty(item.AccountStatus) ? "Inactive" : item.AccountStatus == "A" ? "Active" : "Inactive";

                                sb.AppendFormat(@"<tr>
                                    <td class='center'>
                                        <a class='btn btn-success btn-xs' onclick='onSelectCustomerClick({0}, {1})' href='javascript:;'>เลือก</a>
                                    </td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                    <td>{4}</td>
                                    <td>{5}</td>
                                    <td>{6}</td>
                                    <td>{7}</td>
                                    <td>{8}</td>
                                    <td>{9}</td>
                                    <td>{10}</td>
                                    <td>{11}</td>
                                    <td>{12}</td>
                                </tr>",
                                      item.CustomerId,
                                      item.AccountId,
                                      item.ProductName,
                                      item.CardNo,
                                      item.CustomerFirstName,
                                      item.CustomerLastName,
                                      item.AccountNo,
                                      item.CarNo,
                                      item.PhoneNo,
                                      status,
                                      item.BranchName,
                                      item.CustomerType,
                                      item.SubscriptionTypeName);

                            }
                            sb.Append("</tbody>");
                            sb.Append("</table>");

                            return Json(new
                            {
                                IsSuccess = false,
                                IsDuplicate = true,
                                ExistingCustomerAccounts = sb.ToString(),
                                Message = "พบข้อมูลลูกค้าในระบบซ้ำกับลูกค้าใหม่ กรุณาตรวจสอบข้อมูล หากต้องการเพิ่มลูกค้ากดปุ่ม Confirm"
                            });
                        }
                    }
                }

                bool isSuccess = _customerFacade.SaveCustomer(customerEntity);
                if (isSuccess)
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        customerEntity.CustomerId, // CustomerId ที่ได้จากการ Save
                        customerEntity.DummyAccountId, // dummy account id
                        customerEntity.DummyCustomerContactId // dummy customer contact id
                    });
                }

                return Json(new
                {
                    IsSuccess = false,
                    IsDuplicate = false,
                    Message = Resource.Error_SaveFailed
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Customer").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAccount(ContactEditViewModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Customer").Add("PhoneNo1", model.PhoneNo1).ToInputLogString());
            try
            {
                _customerFacade = new CustomerFacade();

                ContactEntity contactEntity = new ContactEntity
                {
                    ContactId = null,
                    SubscriptType = new SubscriptTypeEntity
                    {
                        SubscriptTypeId = model.SubscriptType.ToNullable<int>()
                    },
                    CardNo = model.CardNo,
                    BirthDate = model.BirthDateValue,
                    TitleThai = new TitleEntity
                    {
                        TitleId = model.TitleThai.ToNullable<int>()
                    },
                    FirstNameThai = model.FirstNameThai,
                    LastNameThai = model.LastNameThai,
                    TitleEnglish = new TitleEntity
                    {
                        TitleId = model.TitleEnglish.ToNullable<int>()
                    },
                    FirstNameEnglish = model.FirstNameEnglish,
                    LastNameEnglish = model.LastNameEnglish,
                    Email = model.Email,
                    CreateUser = new UserEntity
                    {
                        UserId = this.UserInfo.UserId
                    },
                    UpdateUser = new UserEntity
                    {
                        UserId = this.UserInfo.UserId
                    },
                    CustomerId = model.CustomerId, // for add CustomerLog
                    AccountId = model.AccountId,
                    RelationshipId = model.RelationshipId
                };

                #region "Phone and Fax"

                contactEntity.PhoneList = new List<PhoneEntity>();
                if (!string.IsNullOrEmpty(model.PhoneNo1))
                {
                    contactEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = model.PhoneType1.ToNullable<int>(), PhoneNo = model.PhoneNo1 });
                }
                if (!string.IsNullOrEmpty(model.PhoneNo2))
                {
                    contactEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = model.PhoneType2.ToNullable<int>(), PhoneNo = model.PhoneNo2 });
                }
                if (!string.IsNullOrEmpty(model.PhoneNo3))
                {
                    contactEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = model.PhoneType3.ToNullable<int>(), PhoneNo = model.PhoneNo3 });
                }
                // Fax
                if (!string.IsNullOrEmpty(model.Fax))
                {
                    _commonFacade = new CommonFacade();
                    var phoneTypeFax = _commonFacade.GetPhoneTypeByCode(Constants.PhoneTypeCode.Fax);
                    contactEntity.PhoneList.Add(new PhoneEntity { PhoneTypeId = phoneTypeFax.PhoneTypeId, PhoneNo = model.Fax });
                }
                #endregion

                // Save
                _customerFacade = new CustomerFacade();
                _srFacade = new ServiceRequestFacade();

                // Validate duplicate contact name
                var isDuplicate = _srFacade.CheckDuplicateContact(contactEntity);
                if (isDuplicate)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        IsDuplicate = true,
                        Message = "พบข้อมูลลูกค้าในระบบซ้ำกับลูกค้าใหม่ กรุณาตรวจสอบข้อมูล"
                    });
                }

                bool isSuccess = _customerFacade.SaveContactSr(contactEntity);
                if (isSuccess)
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        contactEntity.ContactId // ContactId ที่ได้จากการ Save
                    });
                }

                return Json(new
                {
                    IsSuccess = false,
                    IsDuplicate = false,
                    Message = Resource.Error_SaveFailed
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Customer").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        
        [HttpPost]
        public ActionResult GetAccountRelationSection(int customerId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Account Relation").Add("CustomerId", customerId).ToInputLogString());
            try
            {
                var model = new CreateServiceRequestViewModel();
                _customerFacade = new CustomerFacade();
                _commonFacade = new CommonFacade();

                //account informationlist
                model.AccountSubscriptTypeList = new SelectList((IEnumerable)_commonFacade.GetSubscriptTypeSelectList(), "Key", "Value", string.Empty);
                model.AccountTitleThaiList = new SelectList((IEnumerable)_commonFacade.GetTitleThaiSelectList(), "Key", "Value", string.Empty);
                model.AccountTitleEnglishList = new SelectList((IEnumerable)_commonFacade.GetTitleEnglishSelectList(), "Key", "Value", string.Empty);
                model.AccountPhoneTypeList = new SelectList((IEnumerable)_commonFacade.GetPhoneTypeSelectList(), "Key", "Value", string.Empty);

                var listAccount = _customerFacade.GetAccountByCustomerId(customerId);
                TempData["AccountList"] = listAccount; // keep AccountList
                model.AccountList = new SelectList((IEnumerable)listAccount.Select(l => new
                {
                    key = l.AccountId,
                    value =  l.ProductAndAccountNoDisplay //l.AccountNo
                }).ToDictionary(t => t.key, t => t.value), "Key", "Value", string.Empty);

                var listRelationship = _commonFacade.GetRelationshipSelectList();
                TempData["RelationshipList"] = listRelationship; // Keep RelationshipList
                model.RelationshipList = new SelectList((IEnumerable)listRelationship, "Key", "Value", string.Empty);

                return PartialView("~/Views/ServiceRequest/_AccountRelationSection.cshtml", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Account Relation").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        public ActionResult CreateActivity()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateActivity(int srId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Activity (Service Request)").Add("SrId", srId).ToInputLogString());

            try
            {
                var model = new SrActivityViewModel();
                _srFacade = new ServiceRequestFacade();

                var sr = _srFacade.GetServiceRequest(srId);
                if (sr == null)
                    throw new Exception("Not found Service Request in database. (SR ID=" + srId + ")");

                model.SrId = sr.SrId;
                model.SrNo = sr.SrNo;
                model.CallId = sr.CallId;
                model.PhoneNo = sr.PhoneNo;

                model.RuleAssignFlag = sr.RuleAssignFlag;

                if (sr.OwnerUser != null)
                {
                    model.OwnerUserId = sr.OwnerUser.UserId;
                    model.OwnerUserFullName = sr.OwnerUser.FullName;
                }

                if (sr.OwnerUserBranch != null)
                {
                    model.OwnerBranchId = sr.OwnerUserBranch.BranchId;
                    model.OwnerBranchName = sr.OwnerUserBranch.BranchName;
                }

                if (sr.DelegateUser != null)
                {
                    model.DelegateUserId = sr.DelegateUser.UserId;
                    model.DelegateUserFullName = sr.DelegateUser.FullName;
                }

                if (sr.DelegateUserBranch != null)
                {
                    model.DelegateBranchId = sr.DelegateUserBranch.BranchId;
                    model.DelegateBranchName = sr.DelegateUserBranch.BranchName;
                }

                model.IsEmailDelegate = true;

                if (sr.SRStatus != null)
                    model.SrStatusId = sr.SRStatus.SRStatusId;

                model.ActivityTypes = _srFacade.GetActivityTypes().Select(c => new SelectListItem { Text = c.Name, Value = c.ActivityTypeId + "" }).ToList();
                model.SrEmailTemplates = _srFacade.GetSrEmailTemplates().Select(c => new SelectListItem { Text = c.Name, Value = c.SrEmailTemplateId + "" }).ToList();

                var srStatuses = _srFacade.GetAvailableNextSrStatuses(srId);

                if (sr.SRStatus != null)
                {
                    if (!srStatuses.Any(s => s.SRStatusId == sr.SRStatus.SRStatusId))
                    {
                        var currStatus = sr.SRStatus;
                        srStatuses.Add(new SRStatusEntity()
                        {
                            SRStatusId = currStatus.SRStatusId,
                            SRStatusCode = currStatus.SRStatusCode,
                            SRStatusName = currStatus.SRStatusName,
                        });
                    }
                }

                model.SrStatuses = srStatuses.OrderBy(s => s.SRStatusId).Select(c => new SelectListItem { Text = c.SRStatusName, Value = c.SRStatusId + "" }).ToList();

                // Get doc type list
                _commonFacade = new CommonFacade();
                List<DocumentTypeEntity> docTypeList = null;
                docTypeList = _commonFacade.GetActiveDocumentTypes(Constants.DocumentCategory.ServiceRequest);

                if (docTypeList != null)
                {
                    // model.JsonAttachType = JsonConvert.SerializeObject(docTypeList);
                    model.DocTypeCheckBoxes = docTypeList.Select(x => new CheckBoxes
                    {
                        Value = x.DocTypeId.ToString(),
                        Text = x.Name,
                        Checked = x.IsChecked
                    }).ToList();
                }

                // Document table
                model.DocumentSearchFilter = new DocumentSearchFilter()
                {
                    CustomerId = sr.Customer.CustomerId,
                    SrId = model.SrId,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "",
                    SortOrder = "ASC"
                };

                // For Render Email Template
                model.CustomerFirstNameTh = sr.Customer.FirstName;
                model.CustomerLastNameTh = sr.Customer.LastName;

                var customerPhoneNo1 = sr.Customer.PhoneList.OrderBy(p => p.PhoneTypeId).FirstOrDefault();
                model.CustomerPhoneNo1 = customerPhoneNo1 != null ? customerPhoneNo1.PhoneNo : "";

                model.AccountNo = sr.Account.AccountNo;

                model.ContactFirstNameTh = sr.Contact.FirstName;
                model.ContactLastNameTh = sr.Contact.LastName;

                var contactPhoneNo1 = sr.Contact.PhoneList.OrderBy(p => p.PhoneTypeId).FirstOrDefault();
                model.ContactPhoneNo1 = contactPhoneNo1 != null ? contactPhoneNo1.PhoneNo : "";

                if (sr.CreateBranch != null) {
                    model.CreatorBranchCode = sr.CreateBranch.BranchCode;
                    model.CreatorBranchName = sr.CreateBranch.BranchName;
                }
                model.ProductGroupName = sr.ProductGroup.ProductGroupName;
                model.ProductName = sr.Product.ProductName;
                model.CampaignServiceName = sr.CampaignService.CampaignServiceName;
                model.TypeName = sr.Type.TypeName;
                model.AreaName = sr.Area.AreaName;
                model.SubAreaName = sr.SubArea.SubareaName;
                model.ChannelName = sr.Channel.Name;
                model.Remark = sr.Remark;
                //model.OwnerUserFullName = sr.AfsAssetDesc;
                model.NCBMarketingName = sr.NCBMarketingFullName;
                if (sr.CreateUser != null) model.CreatorUserFullName = sr.CreateUser.FullName;
                model.CreateDateForEmailTemplate = sr.CreateDate.HasValue ? string.Format(new CultureInfo("en-US"), "{0:yyyy-MM-dd HH:mm:ss}", sr.CreateDate.Value) : "";

                _commonFacade = new CommonFacade();
                model.OfficePhoneNo = _commonFacade.GetOfficePhoneNo();
                model.OfficeHour = _commonFacade.GetOfficeHour();

                ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexFileExt);
                ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);

                int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();
                var singleLimitSize = limitSingleFileSize.HasValue ? (limitSingleFileSize / 1048576) : 0;
                model.AllowFileExtensionsDesc = string.Format(CultureInfo.InvariantCulture, param.ParamDesc, singleLimitSize);
                model.AllowFileExtensionsRegex = param.ParamValue;
                model.SrDocumentFolder = _commonFacade.GetSrDocumentFolder();

                ViewBag.CurrentUserId = UserInfo.UserId;

                return View(model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Activity (Service Request)").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetTabDocumentList(DocumentSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Document").ToInputLogString());

            try
            {
                var model = new CreateServiceRequestViewModel();
                model.DocumentSearchFilter = searchFilter;

                _srFacade = new ServiceRequestFacade();
                model.DocumentList = _srFacade.GetTabDocumentList(model.DocumentSearchFilter);

                ViewBag.PageSize = model.DocumentSearchFilter.PageSize;

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Document").ToSuccessLogString());

                // Get doc type list
                _commonFacade = new CommonFacade();
                List<DocumentTypeEntity> docTypeList = null;
                docTypeList = _commonFacade.GetActiveDocumentTypes(Constants.DocumentCategory.ServiceRequest);

                if (docTypeList != null)
                {
                    model.DocTypeCheckBoxes = docTypeList.Select(x => new CheckBoxes
                    {
                        Value = x.DocTypeId.ToString(),
                        Text = x.Name,
                        Checked = x.IsChecked
                    }).ToList();

                    model.DocTypeCheckBoxesEdit = docTypeList.Select(x => new CheckBoxes
                    {
                        Value = x.DocTypeId.ToString(),
                        Text = x.Name,
                        Checked = x.IsChecked
                    }).ToList();
                }

                ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexFileExt);
                ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);

                int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();
                var singleLimitSize = limitSingleFileSize.HasValue ? (limitSingleFileSize / 1048576) : 0;
                model.AllowFileExtensionsDesc = string.Format(CultureInfo.InvariantCulture, param.ParamDesc, singleLimitSize);
                model.AllowFileExtensionsRegex = param.ParamValue;

                var statusList = _commonFacade.GetStatusSelectList();
                model.StatusList = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);
                ViewBag.CurrentUserId = UserInfo.UserId;

                return PartialView("~/Views/ServiceRequest/_SearchTabDocumentList.cshtml", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Document").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetTabLoggingList(LoggingSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Logging").ToInputLogString());

            try
            {
                var model = new CreateServiceRequestViewModel();
                model.LoggingSearchFilter = searchFilter;

                _srFacade = new ServiceRequestFacade();
                model.LoggingResultList = _srFacade.GetTabLoggingList(model.LoggingSearchFilter);
                ViewBag.PageSize = model.LoggingSearchFilter.PageSize;

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Document").ToSuccessLogString());
                return PartialView("~/Views/ServiceRequest/_SearchTabLoggingList.cshtml", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Logging").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetTabActivityList(ActivityTabSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Activity").ToInputLogString());

            try
            {
                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.ServiceRequest;
                _auditLog.Action = Constants.AuditAction.ActivityLog;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.CreateUserId = UserInfo.UserId;

                var model = new CreateServiceRequestViewModel();
                model.SrId = searchFilter.SrId;
                model.ActivitySearchFilter = searchFilter;

                _srFacade = new ServiceRequestFacade();
                model.ActivityList = _srFacade.GetTabActivityList(_auditLog, model.ActivitySearchFilter);

                ViewBag.PageSize = model.ActivitySearchFilter.PageSize;
                ViewBag.OnlineStatus = model.ActivitySearchFilter.IsOnline ? "online" : "offline";

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Activity").ToSuccessLogString());
                return PartialView("~/Views/ServiceRequest/_SearchTabActivityList.cshtml", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Activity").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult GetTabExistingList(ExistingSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Existing").ToInputLogString());

            try
            {
                var model = new CreateServiceRequestViewModel();
                model.ExistingSearchFilter = searchFilter;

                _srFacade = new ServiceRequestFacade();
                model.ExistingList = _srFacade.GetTabExistingList(model.ExistingSearchFilter);
                ViewBag.PageSize = model.ExistingSearchFilter.PageSize;

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Document").ToSuccessLogString());
                return PartialView("~/Views/ServiceRequest/_SearchTabExistingList.cshtml", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Tab Existing").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult SaveDocumentTab(CreateServiceRequestViewModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("New Document").ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                var attach = new AttachmentEntity();
                _commonFacade = new CommonFacade();
                var file = model.FileAttach;

                if (model.DocumentLevel == Constants.DocumentLevel.Customer)
                {
                    if (!model.SrAttachmentId.HasValue)
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Message = "Technical Error: Attachment Id is null",
                        });
                    }

                    // Case [DocumentLevel=Customer] Cannot Add, Edit Only
                    var attachmentId = model.SrAttachmentId.Value;
                    var documentDesc = model.DocDescEdit;
                    var expiryDate = model.ExpireDateEdit.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate);
                    var docTypeIds = model.DocTypeCheckBoxesEdit.Where(x => x.Checked == true)
                                            .Select(x => Convert.ToInt32(x.Value, CultureInfo.InvariantCulture)).ToList();
                    var updateUserId = UserInfo.UserId;

                    _srFacade.UpdateDocumentCustomerLevel(attachmentId, documentDesc, expiryDate, docTypeIds, updateUserId);

                    return Json(new
                    {
                        IsSuccess = true,
                        Message = "บันทึก Document สำเร็จ"
                    });
                }

                // Case [DocumentLevel=SR] Can Add, Can Edit

                attach.CreateUserId = UserInfo.UserId;
                attach.SrId = model.SrId;
                attach.CustomerId = model.CustomerId;
                attach.SrAttachmentId = model.SrAttachmentId;
                attach.Description = !attach.SrAttachmentId.HasValue ? model.DocDesc : model.DocDescEdit;
                attach.ExpiryDate = !attach.SrAttachmentId.HasValue
                    ? model.ExpiryDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate)
                    : model.ExpireDateEdit.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate);
                attach.Status = 1;

                if (!attach.SrAttachmentId.HasValue)
                {
                    #region "Save file"

                    if (file != null && file.ContentLength > 0)
                    {
                        ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);
                        int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();

                        //check file size
                        if (file.ContentLength > limitSingleFileSize.Value)
                        {
                            return Json(new
                            {
                                IsSuccess = false,
                                Message = string.Format(CultureInfo.InvariantCulture, Resource.ValError_SingleFileSizeExceedMaxLimit, (limitSingleFileSize.Value / 1048576))
                            });
                        }

                        // extract only the filename
                        var fileName = Path.GetFileName(file.FileName);
                        attach.Name = model.DocName;

                        // check duplicate file name
                        var isDuplicate = _srFacade.CheckDuplicateDocumentFilename(attach);
                        if (isDuplicate)
                        {
                            return Json(new
                            {
                                IsSuccess = false,
                                Message = "ชื่อ Document มีอยู่ในระบบกรุณาตรวจสอบอีกครั้ง"
                            });
                        }


                        ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexFileExt);
                        Match match = Regex.Match(fileName, param.ParamValue, RegexOptions.IgnoreCase);

                        if (!match.Success)
                        {
                            ModelState.AddModelError("FileAttach", Resource.ValError_FileExtension);
                            goto Outer;
                        }

                        var docFolder = _commonFacade.GetSrDocumentFolder();
                        int seqNo = _commonFacade.GetNextAttachmentSeq();
                        var fileNameUrl = ApplicationHelpers.GenerateFileName(docFolder,
                            Path.GetExtension(file.FileName), seqNo, Constants.AttachmentPrefix.Sr);

                        var targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", docFolder, fileNameUrl);
                        file.SaveAs(targetFile);
                        attach.Url = fileNameUrl;
                        attach.Filename = fileName;
                        attach.ContentType = file.ContentType;
                        attach.FileSize = file.ContentLength;
                    }

                    #endregion
                }

                //validate total file size
                ParameterEntity paramTotalFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.TotalFileSize);
                int? limitTotalFileSize = paramTotalFileSize.ParamValue.ToNullable<int>();

                var totalFileSize = _srFacade.GetServiceAttachTotalFileSize(attach.SrId.Value);

                if (totalFileSize > limitTotalFileSize.Value)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Message = string.Format(CultureInfo.InvariantCulture, Resource.ValError_TotalFileSizeExceedMaxLimit, (limitTotalFileSize.Value / 1048576))
                    });
                }


                #region "AttachType"

                var selectedAttachType = new List<AttachmentTypeEntity>();

                if (!model.SrAttachmentId.HasValue)
                {
                    // Insert
                    selectedAttachType = model.DocTypeCheckBoxes
                        .Where(x => x.Checked == true)
                        .Select(x => new AttachmentTypeEntity
                        {
                            DocTypeId = x.Value.ToNullable<int>(),
                            CreateUserId = this.UserInfo.UserId
                        }).ToList();
                }
                else
                {
                    // Update
                    selectedAttachType = model.DocTypeCheckBoxesEdit
                        .Where(x => x.Checked == true)
                        .Select(x => new AttachmentTypeEntity
                        {
                            DocTypeId = x.Value.ToNullable<int>(),
                            CreateUserId = this.UserInfo.UserId
                        }).ToList();
                }

                if (model.AttachTypeList != null && model.AttachTypeList.Count > 0)
                {
                    var prevAttachTypes = (from at in model.AttachTypeList
                                           select new AttachmentTypeEntity
                                           {
                                               AttachmentId = at.AttachmentId,
                                               Code = at.Code,
                                               Name = at.Name,
                                               DocTypeId = at.DocTypeId,
                                               IsDelete = !selectedAttachType.Select(x => x.DocTypeId).Contains(at.DocTypeId),
                                               Status = at.Status,
                                               CreateUserId = this.UserInfo.UserId
                                           }).ToList();

                    var dupeAttachTypes = new List<AttachmentTypeEntity>(selectedAttachType);
                    dupeAttachTypes.AddRange(prevAttachTypes);

                    var duplicates = dupeAttachTypes.GroupBy(x => new { x.DocTypeId })
                        .Where(g => g.Count() > 1)
                        .Select(g => (object)g.Key.DocTypeId);

                    if (duplicates.Any())
                    {
                        //Logger.Info(_logMsg.Clear().SetPrefixMsg("Duplicate ID in list")
                        //    .Add("IDs", StringHelpers.ConvertListToString(duplicates.ToList(), ",")).ToInputLogString());
                        prevAttachTypes.RemoveAll(x => duplicates.Contains(x.DocTypeId));
                    }

                    selectedAttachType.AddRange(prevAttachTypes);
                }

                attach.AttachTypeList = selectedAttachType;

                #endregion

                var isSuccess = _srFacade.SaveSrAttachment(attach);

                if (isSuccess)
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Message = "บันทึก Document สำเร็จ"
                    });
                }

                return Json(new
                {
                    IsSuccess = false,
                    Message = "บันทึก Document ไม่สำเร็จ"
                });
                
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("New Document").Add("Error Message", ex.Message).ToInputLogString());
                return Json(new { IsSuccess = false, ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteDocumentTab(int? SrAttchId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Document").Add("SrAttchId", SrAttchId).ToInputLogString());

            try
            {
                if (SrAttchId.HasValue)
                {
                    _srFacade = new ServiceRequestFacade();

                    var isSuccess = _srFacade.DeleteSrAttachment(SrAttchId);

                    if (isSuccess)
                    {
                        return Json(new
                        {
                            IsSuccess = true,
                            Message = "ลบ Document สำเร็จ"
                        });
                    }

                    return Json(new
                    {
                        IsSuccess = false,
                        Message = "ลบ Document ไม่สำเร็จ"
                    });
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Message = "ไม่พบข้อมูลการลบ"
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Document").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new { IsSuccess = false, ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetSrAttachmentById(int srAttachId, string documentLevel)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Document").Add("SrAttachId", srAttachId).Add("DocumentLevel", documentLevel).ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                AttachmentEntity attach = _srFacade.GetSrAttachmentById(srAttachId, documentLevel);

                _commonFacade = new CommonFacade();

                string docFolder;
                if (documentLevel.ToUpper(CultureInfo.InvariantCulture) == Constants.DocumentLevel.Sr.ToUpper(CultureInfo.InvariantCulture))
                {
                    // SR Path
                    docFolder = _commonFacade.GetSrDocumentFolder();
                }
                else
                {
                    // Customer Path
                    docFolder = _commonFacade.GetCSMDocumentFolder();
                }

                return Json(new
                {
                    IsSuccess = true,
                    attach.AttachmentId,
                    Url = docFolder + "\\" + attach.Url,
                    attach.Name,
                    attach.Filename,
                    attach.Description,
                    attach.ExpiryDateDisplay,
                    attach.AttachTypeList,
                    DocumentLevel = documentLevel,
                    attach.Status
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Document").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new { IsSuccess = false, ex.Message });
            }
        }
        
        [HttpPost]
        public JsonResult GetActivityDocumentList(int srId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Activity Document").Add("SrId", srId).ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                var activityDocumentList = _srFacade.GetActivityDocumentList(srId);
                return Json(new { IsSuccess = true, activityDocumentList });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Activity Document").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new { IsSuccess = false, ex.Message });
            }
        }
        
        [HttpPost]
        public ActionResult SaveSrActivityAttachment(SrActivityViewModel model, int fileSize)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Attachment").Add("DocName", model.DocName).Add("FileSize", fileSize).ToInputLogString());

            try
            {
                var attach = new AttachmentEntity();
                _commonFacade = new CommonFacade();

                if (model.DocName.Length > 200)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Valid = false,
                        Error = string.Empty,
                        Message = "ไม่สามารถบันทึก sr ได้ เนื่องจากชื่อเอกสารเกิน 200 ตัวอักษร"
                    });
                }

                #region "Save file"
                var file = model.FileAttach;
                if (file != null && file.ContentLength > 0)
                {
                    ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);
                    int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();
                    // extract only the filename
                    var fileName = Path.GetFileName(file.FileName);

                    //check file size
                    if (file.ContentLength > limitSingleFileSize.Value)
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Message = string.Format(CultureInfo.InvariantCulture, Resource.ValError_SingleFileSizeExceedMaxLimit, (limitSingleFileSize.Value / 1048576))
                        });
                    }

                    ParameterEntity paramTotalFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.TotalFileSize);
                    int? limitTotalFileSize = paramTotalFileSize.ParamValue.ToNullable<int>();
                    //chech total file size
                    fileSize = fileSize + file.ContentLength;
                    if (fileSize > limitTotalFileSize)
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            Message = string.Format(CultureInfo.InvariantCulture, Resource.ValError_TotalFileSizeExceedMaxLimit, (limitTotalFileSize.Value / 1048576))
                        });
                    }

                    ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexFileExt);
                    Match match = Regex.Match(fileName, param.ParamValue, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        ModelState.AddModelError("FileAttach", Resource.ValError_FileExtension);
                        goto Outer;
                    }

                    var docFolder = _commonFacade.GetSrDocumentFolder();
                    int seqNo = _commonFacade.GetNextAttachmentSeq();

                    var fileNameUrl = ApplicationHelpers.GenerateFileName(docFolder, Path.GetExtension(file.FileName), seqNo, Constants.AttachmentPrefix.Sr);

                    var targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", docFolder, fileNameUrl);
                    file.SaveAs(targetFile);

                    attach.Url = fileNameUrl;
                    attach.Filename = fileName;
                    attach.ContentType = file.ContentType;
                }

                #endregion
                var selectedAttachType = model.DocTypeCheckBoxes.Where(x => x.Checked)
                        .Select(x => new AttachmentTypeEntity
                        {
                            DocTypeId = x.Value.ToNullable<int>(),
                            Name = x.Text,
                            CreateUserId = this.UserInfo.UserId
                        }).ToList();

                return Json(new
                {
                    IsSuccess = true,
                    attach.Url,
                    attach.Filename,
                    attach.ContentType,
                    model.DocDesc,
                    model.ExpiryDate,
                    model.AttachToEmail,
                    model.DocName,
                    SelectedAttachType = selectedAttachType,
                    CreateDate = DateUtil.ToStringAsDateTime(DateTime.Now),
                    FileSize = file.ContentLength,
                    Status = 1,
                    CreateUserId = UserInfo.UserId,
                    CreateUserName = UserInfo.Username
                });

            Outer:
                return Json(new
                {
                    IsSuccess = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("FileAttach", ex.Message);
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Attachment").Add("Error Message", ex.Message).ToFailLogString());

                return Json(new
                {
                    IsSuccess = false,
                    Valid = false,
                    Error = string.Empty,
                    Message = ex.Message,
                    Errors = GetModelValidationErrors()
                });
            }
        }
        
        [HttpPost]
        public JsonResult GetCustomerContactById(int customerContactId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Contact").Add("CustomerContactId", customerContactId).ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();

                var contactData = _srFacade.GetCustomerContactById(customerContactId);
                var phoneNoFax = contactData.CustomerPhones.Where(c => c.PhoneTypeCode != Constants.PhoneTypeCode.Fax).ToList();
                var phoneFax = contactData.CustomerPhones.FirstOrDefault(c => c.PhoneTypeCode == Constants.PhoneTypeCode.Fax);

                var phoneNo1 = "";
                var phoneNo2 = "";
                var phoneNo3 = "";

                var phoneType1 = "";
                var phoneType2 = "";
                var phoneType3 = "";

                var faxNo = "";

                if (phoneFax != null)
                {
                    faxNo = phoneFax.PhoneNo;
                }

                if (phoneNoFax.Count > 0)
                {
                    phoneNo1 = phoneNoFax[0].PhoneNo;
                    phoneType1 = phoneNoFax[0].PhoneTypeName;
                }

                if (phoneNoFax.Count > 1)
                {
                    phoneNo2 = phoneNoFax[1].PhoneNo;
                    phoneType2 = phoneNoFax[1].PhoneTypeName;
                }

                if (phoneNoFax.Count > 2)
                {
                    phoneNo3 = phoneNoFax[2].PhoneNo;
                    phoneType3 = phoneNoFax[2].PhoneTypeName;
                }

                return Json(new
                {
                    IsSuccess = true,
                    contactData.ContactId,
                    contactData.SubscriptionType,
                    contactData.CardNo,
                    BirthDay = DateUtil.ToStringAsDate(contactData.BirthDay),
                    contactData.TitleTh,
                    contactData.FirstNameTh,
                    contactData.LastNameTh,
                    contactData.TitleEn,
                    contactData.FirstNameEn,
                    contactData.LastNameEn,
                    phoneType1,
                    phoneType2,
                    phoneType3,
                    phoneNo1,
                    phoneNo2,
                    phoneNo3,
                    faxNo,
                    contactData.Email,
                    contactData.AccountNo,
                    contactData.RelationshipId,
                    contactData.RelationName
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Contact").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult GetProductByAccountId(int accountId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Product By AccountId").Add("AccountId", accountId).ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                var customerAccount = _srFacade.GetCustomerAccount(accountId);

                if (customerAccount != null && customerAccount.Account != null && customerAccount.Account.Product != null)
                {
                    return Json(new
                    {
                        Valid = true,
                        Product = customerAccount.Account.Product
                    });
                }
                else
                {
                    return Json(new
                    {
                        Valid = true,
                        Product = ""
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Product By AccountId").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                });
            }
        }

        #region == Download Document ==



        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LoadFileAttachment(string pathFile)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Load FileAttachment").ToInputLogString());

            try
            {
                if (!string.IsNullOrEmpty(pathFile) && !System.IO.File.Exists(pathFile))
                {
                    return Json(new
                    {
                        Valid = false,
                        Error = "ไม่พบไฟล์ที่ต้องการ Download",
                        Errors = string.Empty
                    });
                }

                return Json(new
                {
                    Valid = true
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Load FileAttachment").Add("Error Message", ex.Message).ToFailLogString());
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
        public JsonResult LoadFileAttachmentById(int attachmentId, string documentLevel = "")
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Load FileAttachment").ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                var attachment = _srFacade.GetSrAttachmentById(attachmentId, documentLevel);

                _commonFacade = new CommonFacade();
                string docFolder;
                if (documentLevel.ToUpper(CultureInfo.InvariantCulture) == Constants.DocumentLevel.Sr.ToUpper(CultureInfo.InvariantCulture))
                {
                    docFolder = _commonFacade.GetSrDocumentFolder();
                }
                else if (documentLevel.ToUpper(CultureInfo.InvariantCulture) == Constants.DocumentLevel.Customer.ToUpper(CultureInfo.InvariantCulture))
                {
                    docFolder = _commonFacade.GetCSMDocumentFolder();
                }
                else
                {
                    return Json(new
                    {
                        Valid = false,
                        Error = "Technical Error: ไม่พบ Document Level ตามที่กำหนด จึงไม่สามารถอ่านไฟล์ได้ (DocumentLevel=" + documentLevel + ")",
                        Errors = string.Empty
                    });
                }

                var pathFile = docFolder + "\\" + attachment.Url;

                if (!string.IsNullOrEmpty(pathFile) && !System.IO.File.Exists(pathFile))
                {
                    return Json(new
                    {
                        Valid = false,
                        Error = "ไม่พบไฟล์ที่ต้องการ Download",
                        Errors = string.Empty
                    });
                }

                return Json(new
                {
                    Valid = true
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Load FileAttachment").Add("Error Message", ex.Message).ToFailLogString());
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
        public JsonResult LoadFileAttachmentByIdOrName(int? attachmentId, string pathFile)
        {
            var id = attachmentId ?? 0;
            if (id != 0)
            {
                return LoadFileAttachmentById(id, Constants.DocumentLevel.Sr);
            }
            else
            {
                return LoadFileAttachment(pathFile);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PreviewAttachmentByIdOrName(int? attachmentId, string pathFile, string contentType, string fileName)
        {
            var id = attachmentId ?? 0;
            if (id != 0)
            {
                return PreviewAttachmentById(id);
            }
            else
            {
                return PreviewAttachment(pathFile, contentType, fileName);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PreviewAttachment(string pathFile, string contentType, string fileName)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Preview Attachment").ToInputLogString());

            try
            {
                byte[] byteArray = System.IO.File.ReadAllBytes(pathFile);
                return File(byteArray, contentType, fileName);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Preview Attachment").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PreviewAttachmentById(int attachmentId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Preview Attachment").ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                var attachment = _srFacade.GetSrAttachmentById(attachmentId, Constants.DocumentLevel.Sr);

                _commonFacade = new CommonFacade();
                var docFolder = _commonFacade.GetSrDocumentFolder();

                var pathFile = docFolder + "\\" + attachment.Url;

                byte[] byteArray = System.IO.File.ReadAllBytes(pathFile);
                return File(byteArray, attachment.ContentType, attachment.Filename);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Preview Attachment").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetSingleMapProduct(int? campaignServiceId, int? areaId, int? subAreaId, int? typeId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Single MapProduct").Add("CampaignServiceId", campaignServiceId)
                .Add("AreaId", areaId).Add("SubAreaId", subAreaId).Add("TypeId", typeId).ToInputLogString());

            try
            {
                _srFacade = new ServiceRequestFacade();
                SingleMapProductEntity data = _srFacade.GetSingleMapProduct(campaignServiceId, areaId, subAreaId, typeId);
                return Json(new { IsSuccess = true, data });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Single MapProduct").Add("Error Message", ex.Message).ToInputLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }
    }
}
