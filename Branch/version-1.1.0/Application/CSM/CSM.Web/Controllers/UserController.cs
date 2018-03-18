using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CSM.Business;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Web.Models;
using CSM.Web.Filters;
using CSM.Web.Controllers.Common;
using log4net;

namespace CSM.Web.Controllers
{
    public class UserController : BaseController
    {
        private IUserFacade _userFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserController));

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            // So that the user can be referred back to where they were when they click logon
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = HttpUtility.UrlDecode(returnUrl);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel user, string returnUrl)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Login").Add("UserName", user.UserName).ToInputLogString());

            try
            {
                // Validate the input data
                IDictionary<string, object> errorList = null;

                if (ModelState.IsValid)
                {
                    if (user.IsValid(user.UserName, user.Password))
                    {
                        //returnURL needs to be decoded
                        string decodedUrl = string.Empty;
                        if (!string.IsNullOrEmpty(returnUrl))
                            decodedUrl = Server.UrlDecode(returnUrl);

                        // Login logic
                        var userId = SetupFormsAuthTicket(user.UserName, user.RememberMe);
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Login").Add("UserId", userId).ToSuccessLogString());

                        if (Url.IsLocalUrl(decodedUrl))
                        {
                            return Redirect(decodedUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }

                    user.ErrorMessage = Resource.Msg_LoginFailed;
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Login").Add("Error Message", user.ErrorMessage).ToFailLogString());
                    goto Outer;
                }

                // Failed Validation
                errorList = GetModelValidationErrors();
                user.ErrorMessage = String.Join("<br>", errorList.Select(o => o.Value));
            }
            catch (CustomException cex)
            {
                user.ErrorMessage = cex.Message == Resource.Msg_CannotConnectToAD ? Resource.Msg_LoginFailed : cex.Message;
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Login").Add("Error Message", user.ErrorMessage).ToFailLogString());
            }

        Outer:
            ViewBag.ReturnUrl = returnUrl;
            return View(user);
        }

        [HttpGet]
        [CheckUserSession]
        public ActionResult Logout()
        {
            string loginUrl = FormsAuthentication.LoginUrl;
            int? userId = this.UserInfo != null ? this.UserInfo.UserId : new Nullable<int>();

            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Logout").Add("UserID", userId).ToInputLogString());
                ClearSession();
                FormsAuthentication.SignOut();
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Logout").ToSuccessLogString());
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Logout").Add("Error Message", ex.Message).ToFailLogString());
            }
            return Redirect(loginUrl);
        }

        [HttpGet]
        [CheckUserSession]
        public ActionResult AccessDenied()
        {
            return View();
        }
        
        private int? SetupFormsAuthTicket(string userName, bool persistanceFlag)
        {
            _userFacade = new UserFacade();
            int? userId = _userFacade.GetUserIdByLogin(userName);
            var userData = userId.ConvertToString();
            var authTicket = new FormsAuthenticationTicket(1, //version
                                                        userName, // user name
                                                        DateTime.Now,             //creation
                                                        DateTime.Now.AddMinutes(30), //Expiration
                                                        persistanceFlag, //Persistent
                                                        userData);

            var encTicket = FormsAuthentication.Encrypt(authTicket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            return userId;
        }
    }
}
