using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace CSM.Web.Filters
{
    public sealed class HandleAntiForgeryError : ActionFilterAttribute, IExceptionFilter
    {
        #region "IExceptionFilter Members"

        public void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception as HttpAntiForgeryException;
            if (exception != null)
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var routeValues = new RouteValueDictionary();
                    routeValues["controller"] = "User";
                    routeValues["action"] = "Login";
                    filterContext.Result = new RedirectToRouteResult(routeValues);
                }
                else
                {
                    filterContext.Result = new JsonResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new { Valid = false, RedirectUrl = FormsAuthentication.LoginUrl }
                    };
                }

                filterContext.ExceptionHandled = true;
            }
        }

        #endregion
    }
}