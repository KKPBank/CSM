using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CSM.Business;
using CSM.Entity;
using CSM.Web.Models;

namespace CSM.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class CheckUserRoleAttribute : ActionFilterAttribute, IDisposable
    {
        public int AccessRoles { get; set; }
        private ICommonFacade _commonFacade;

        public CheckUserRoleAttribute(string screenCode) : base()
        {
            _commonFacade = new CommonFacade();
            this.AccessRoles = _commonFacade.GetRoleValueByScreenCode(screenCode);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            UserIdentity id = (UserIdentity)HttpContext.Current.User.Identity;
            UserEntity user = id.UserInfo;

            if (session["sessionid"] != null && this.AccessRoles != 0 && (this.AccessRoles & user.RoleValue) == 0)
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "User" }, { "action", "AccessDenied" } });
                    base.OnActionExecuting(filterContext);
                }
                else {
                    filterContext.Result = new JsonResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new { Valid = false, RedirectUrl = UrlHelper.GenerateContentUrl("~/User/AccessDenied", filterContext.HttpContext) }
                    };
                }
            }
        }

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
                    if (_commonFacade != null) { _commonFacade.Dispose(); }
                }
            }
            _disposed = true;
        }

        #endregion
    }
}