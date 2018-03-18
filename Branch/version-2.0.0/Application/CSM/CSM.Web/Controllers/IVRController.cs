using System;
using System.Web.Mvc;
using System.Web.Routing;
using CSM.Business;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using System.Globalization;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class IVRController : Controller
    {
        private ICustomerFacade _customerFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IVRController));

        private UserEntity UserInfo
        {
            get
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    UserIdentity id = HttpContext.User.Identity as UserIdentity;
                    return id.UserInfo;
                }

                return null;
            }
        }

        [HttpGet]
        public ActionResult ReceiveCall()
        {
            var dic = ApplicationHelpers.GetParams(Request.Url.Query);
            string callId = dic["callid"];
            string phoneNo = dic["phoneno"];
            string cardNo = dic["cardno"];
            string callType = dic["calltype"];
            string ivrLang = dic["ivrlang"];

            Logger.Info(_logMsg.Clear().SetPrefixMsg("Receive Call").Add("CallId", callId).Add("PhoneNo", phoneNo)
                .Add("CardNo", cardNo.MaskCardNo()).Add("CallType", callType).Add("RequestUrl", Request.Url.AbsoluteUri).ToInputLogString());

            try
            {
                // Reset routedata
                RouteData.Values.Remove("callId");
                RouteData.Values.Remove("phoneNo");

                if (string.IsNullOrWhiteSpace(callId) || string.IsNullOrWhiteSpace(phoneNo) 
                    || string.IsNullOrWhiteSpace(callType))
                {
                    goto Outer;
                }

                _customerFacade = new CustomerFacade();
                bool success = _customerFacade.SaveCallId(callId, phoneNo, cardNo, callType.ToUpper(CultureInfo.InvariantCulture)
                                                            , this.UserInfo.UserId, ivrLang.ToUpper(CultureInfo.InvariantCulture));
                if (success)
                {
                    RouteValueDictionary dict = new RouteValueDictionary();
                    dict.Add("callId", callId);
                    dict.Add("phoneNo", phoneNo);
                    return RedirectToAction("Search", "Customer", dict);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Receive Call").Add("Error Message", ex.Message).ToFailLogString());
            }

        Outer:
            return RedirectToAction("Search", "Customer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewCallInfo(string callId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("View CallInfo").Add("CallId", callId).ToInputLogString());

            try
            {
                _customerFacade = new CustomerFacade();
                var callInfo = _customerFacade.GetCallInfoByCallId(callId);
                if (callInfo != null)
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("View CallInfo").Add("CallID", callInfo.CallId)
                        .Add("CardNo", callInfo.CardNo.MaskCardNo())
                        .Add("CallType", callInfo.CallType)
                        .ToSuccessLogString());
                    return View("~/Views/Shared/_ViewCallInfo.cshtml", callInfo);
                }

                return Json(new
                {
                    Valid = false,
                    Error = Resource.Msg_NoRecords,
                    Errors = string.Empty
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("View CallInfo").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }
    }
}
