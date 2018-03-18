using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CSM.Business;
using System.Web.Security;
using CSM.Common.Utilities;

namespace CSM.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class CheckUserSessionAttribute : ActionFilterAttribute, IDisposable
    {
        private IUserFacade _userFacade;
        private readonly string[] _ignoreUri = new string[] { "/", FormsAuthentication.LoginUrl };

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Do not execute the filter logic for User/Login
            if (filterContext.RouteData.GetRequiredString("controller").Equals("User", StringComparison.CurrentCultureIgnoreCase) &&
                filterContext.RouteData.GetRequiredString("action").Equals("Login", StringComparison.CurrentCultureIgnoreCase))
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    return;
                }
                else
                {
                    filterContext.Result = new JsonResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new { Valid = false, RedirectUrl = FormsAuthentication.LoginUrl }
                    };
                }
            }

            HttpSessionStateBase session = filterContext.HttpContext.Session;
            var username = HttpContext.Current.User != null ? HttpContext.Current.User.Identity.Name : string.Empty;

            _userFacade = new UserFacade();
            _userFacade.CheckExceededMaxConcurrent(username, session);

            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (((session["sessionid"] == null) && (!session.IsNewSession)) || (session.IsNewSession))
                {
                    // Clear cache
                    var cacheKey = string.Format(CultureInfo.InvariantCulture, "{0}_user_info", username);
                    if (HttpRuntime.Cache[cacheKey] != null)
                    {
                        HttpRuntime.Cache.Remove(cacheKey);
                    }

                    session.RemoveAll();
                    session.Clear();
                    session.Abandon();

                    string returnUrl = GetReturnUri(filterContext);

                    RouteValueDictionary dict = new RouteValueDictionary();
                    dict.Add("controller", "User");
                    dict.Add("action", "Login");

                    if (!string.IsNullOrWhiteSpace(returnUrl))
                    {
                        dict.Add("returnUrl", returnUrl);
                    }

                    filterContext.Result = new RedirectToRouteResult(dict);
                }

                base.OnActionExecuting(filterContext);
            }
            else
            {
                if (((session["sessionid"] == null) && (!session.IsNewSession)) || (session.IsNewSession))
                {
                    // Clear cache
                    var cacheKey = string.Format(CultureInfo.InvariantCulture, "{0}_user_info", username);
                    if (HttpRuntime.Cache[cacheKey] != null)
                    {
                        HttpRuntime.Cache.Remove(cacheKey);
                    }

                    session.RemoveAll();
                    session.Clear();
                    session.Abandon();

                    filterContext.Result = new JsonResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new { Valid = false, RedirectUrl = FormsAuthentication.LoginUrl }
                    };
                }
            }
        }

        #region "Functions"

        private string GetReturnUri(ActionExecutingContext filterContext)
        {
            string returnUrl;

            if (filterContext.HttpContext.Request.UrlReferrer != null)
            {
                returnUrl = filterContext.HttpContext.Request.UrlReferrer.PathAndQuery;
            }
            else
            {
                returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery;
            }

            string decodeUrl = HttpUtility.UrlDecode(returnUrl);
            var dic = ApplicationHelpers.GetParams(decodeUrl);
            string callId = filterContext.RouteData.Values["callId"].ConvertToString();

            if (_ignoreUri.Contains(decodeUrl) || decodeUrl.IsQueryStringExists("returnUrl") || !string.IsNullOrWhiteSpace(callId))
            {
                // Reset routedata
                filterContext.RouteData.Values.Remove("callId");
                filterContext.RouteData.Values.Remove("phoneNo");
                returnUrl = string.Empty;
            }
            if (dic != null && dic.ContainsKey("encryptedstring"))
            {
                returnUrl = UrlHelper.GenerateContentUrl("~/Customer/Search", filterContext.HttpContext);
            }
            
            return returnUrl;
        }

        #endregion

        #region "IDisposable"

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_userFacade != null) { _userFacade.Dispose(); }
                }
            }
            _disposed = true;
        }

        #endregion
    }
}