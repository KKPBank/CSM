using System;
using System.Collections;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Service.Messages.Campaign;
using CSM.Service.Messages.Common;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class CampaignController : BaseController
    {
        private IUserFacade _userFacade;
        private AuditLogEntity _auditLog;
        private ICommonFacade _commonFacade;
        private ICampaignFacade _campaignFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CampaignController));

        public ActionResult List(string encryptedString)
        {
            int? customerId = encryptedString.ToCustomerId();
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Recommended Campaign").Add("CustomerId", customerId).ToInputLogString());
            
            if (customerId == null)
            {
                return RedirectToAction("Search", "Customer");
            }

            try
            {
                CampaignViewModel campaignVM = new CampaignViewModel();
                CustomerInfoViewModel custInfoVM = this.MappingCustomerInfoView(customerId.Value);
                campaignVM.CustomerInfo = custInfoVM;
                return View(campaignVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Recommended Campaign").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerCampaigns)]
        public ActionResult CampaignList(int? customerId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Campaign").Add("CustomerId", customerId).ToInputLogString());

            if (customerId == null)
            {
                return Json(new
                {
                    Valid = false,
                    RedirectUrl = Url.Action("Search", "Customer")
                });
            }
            
            try
            {
                if (ModelState.IsValid)
                {
                    _campaignFacade = new CampaignFacade();
                    CampaignViewModel campaignVM = new CampaignViewModel();
                    CustomerInfoViewModel custInfoVM = this.MappingCustomerInfoView(customerId.Value);

                    if (!string.IsNullOrWhiteSpace(custInfoVM.CardNo))
                    {
                        _auditLog = new AuditLogEntity();
                        _auditLog.Module = Constants.Module.Customer;
                        _auditLog.Action = Constants.AuditAction.RecommendedCampaign;
                        _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                        _auditLog.CreateUserId = this.UserInfo.UserId;

                        campaignVM.CampaignList = _campaignFacade.GetCampaignListByCustomer(_auditLog, custInfoVM.CardNo,
                            Constants.CMTParamConfig.NoOffered, Constants.CMTParamConfig.NoInterested,
                            Constants.CMTParamConfig.RecommendCampaign, Constants.CMTParamConfig.NumRecommendCampaign);
                    }

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Campaign").ToSuccessLogString());
                    return PartialView("~/Views/Campaign/_CampaignList.cshtml", campaignVM);
                }

                return Json(new
                {
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                });
            }
            catch (CustomException cex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Campaign").Add("Error Message", cex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = cex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Campaign").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerCampaigns)]
        public ActionResult RecommendedCampaignList(int? customerId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Recommended Campaign").Add("CustomerId", customerId).ToInputLogString());

            if (customerId == null)
            {
                return Json(new
                {
                    Valid = false,
                    RedirectUrl = Url.Action("Search", "Customer")
                });
            }
            
            try
            {
                if (ModelState.IsValid)
                {
                    _campaignFacade = new CampaignFacade();
                    CampaignViewModel campaignVM = new CampaignViewModel();
                    CustomerInfoViewModel custInfoVM = this.MappingCustomerInfoView(customerId.Value);

                    if (!string.IsNullOrWhiteSpace(custInfoVM.CardNo))
                    {
                        _auditLog = new AuditLogEntity();
                        _auditLog.Module = Constants.Module.Customer;
                        _auditLog.Action = Constants.AuditAction.RecommendedCampaign;
                        _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                        _auditLog.CreateUserId = this.UserInfo.UserId;

                        campaignVM.RecommendedCampaignList = _campaignFacade.GetCampaignListByCustomer(_auditLog,
                            custInfoVM.CardNo, Constants.CMTParamConfig.Offered,
                            Constants.CMTParamConfig.Interested, Constants.CMTParamConfig.RecommendedCampaign,
                            Constants.CMTParamConfig.NumRecommendedCampaign);
                    }

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Campaign").ToSuccessLogString());
                    return PartialView("~/Views/Campaign/_RecommendedCampaignList.cshtml", campaignVM);
                }

                return Json(new
                {
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                });
            }
            catch (CustomException cex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Recommended Campaign").Add("Error Message", cex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = cex.Message,
                    Errors = string.Empty
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Recommended Campaign").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerCampaigns)]
        public ActionResult InitEdit(int? customerId, string campaignId, string campaignName, string contractNoRefer)
        {
            if (customerId == null)
            {
                return Json(new
                {
                    Valid = false,
                    RedirectUrl = Url.Action("Search", "Customer")
                });
            }

            CustomerInfoViewModel custInfoVM = this.MappingCustomerInfoView(customerId.Value);
            CampaignViewModel campaignVM = new CampaignViewModel();
            campaignVM.FirstName = custInfoVM.FirstName;
            campaignVM.LastName = custInfoVM.LastName;
            campaignVM.CustomerType = custInfoVM.CustomerType;
            campaignVM.CardNo = custInfoVM.CardNo;
            campaignVM.Email = custInfoVM.Email;
            campaignVM.CampaignId = campaignId;
            campaignVM.CampaignName = campaignName;
            campaignVM.CustomerType = custInfoVM.CustomerType;
            campaignVM.ChannelId = this.UserInfo.ChannelId;
            campaignVM.ChannelName = this.UserInfo.ChannelName;
            campaignVM.ContractNoRefer = contractNoRefer;

            if (custInfoVM.PhoneList != null && custInfoVM.PhoneList.Count > 0)
            {
                campaignVM.PhoneNo = custInfoVM.PhoneList[0].PhoneNo;
            }

            _commonFacade = new CommonFacade();
            var customerTypeList = _commonFacade.GetCustomerTypeSelectList();
            campaignVM.CustomerTypeList = new SelectList((IEnumerable)customerTypeList, "Key", "Value", string.Empty);

            return PartialView("~/Views/Campaign/_CampaignEdit.cshtml", campaignVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerCampaigns)]
        public ActionResult Edit(CampaignViewModel campaignVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Recommended Campaign").Add("CardNo", campaignVM.CardNo.MaskCardNo())
                .Add("HasInterested", campaignVM.Interested).Add("FirstName", campaignVM.FirstName).Add("LastName", campaignVM.LastName)
                .Add("PhoneNo", campaignVM.PhoneNo).ToInputLogString());
            
            try
            {
                if (ModelState.IsValid)
                {
                    _userFacade = new UserFacade();
                    _commonFacade = new CommonFacade();

                    var searchFilter = new CampaignSearchFilter
                    {
                        CampaignId = campaignVM.CampaignId,
                        HasOffered = Constants.CMTParamConfig.Offered,
                        IsInterested = campaignVM.Interested,
                        Comments = campaignVM.Comments,
                        UpdatedBy = this.UserInfo.Username,
                        FirstName = campaignVM.FirstName,
                        LastName = campaignVM.LastName,
                        PhoneNo = campaignVM.PhoneNo,
                        Email = campaignVM.Email,
                        CardNo = campaignVM.CardNo,
                        ChannelName = campaignVM.ChannelName,
                        AvailableTime = campaignVM.AvailableTime,
                        ContractNoRefer = campaignVM.ContractNoRefer
                    };

                    int? ownerLeadId = campaignVM.OwnerLead.ToNullable<int>();
                    if (ownerLeadId != null)
                    {
                        UserEntity ownerLead = _userFacade.GetUserById(ownerLeadId.Value);
                        searchFilter.OwnerLeadCode = ownerLead.EmployeeCode;
                    }

                    _auditLog = new AuditLogEntity();
                    _auditLog.Module = Constants.Module.Customer;
                    _auditLog.Action = Constants.AuditAction.RecommendedCampaign;
                    _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                    _auditLog.CreateUserId = this.UserInfo.UserId;

                    _campaignFacade = new CampaignFacade();

                    if (Constants.CMTParamConfig.Interested.Equals(campaignVM.Interested))
                    {
                        Ticket resLead = _campaignFacade.CreateLead(_auditLog, searchFilter);
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Create Lead").Add("ResponseCode", resLead.ResponseCode)
                            .Add("ResponseMessage", resLead.ResponseMessage).ToSuccessLogString());
                    }

                    UpdateCampaignFlagsResponse resCamp = _campaignFacade.SaveCampaignFlags(_auditLog, campaignVM.CardNo, searchFilter);
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Recommended Campaign").Add("UpdateStatus", resCamp.UpdateStatus).ToSuccessLogString());

                    // Call CMT and SLM Services
                    return Json(new
                    {
                        Valid = true,
                        Error = string.Empty,
                    });
                }

                return Json(new
                {
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                });
            }
            catch (CustomException cex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Campaign").Add("Error Message", cex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = cex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Campaign").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = ex.Message
                });
            }
        }
    }
}
