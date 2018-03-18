using System.Web.Mvc;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Models;
using log4net;
using System;
using System.Collections.Generic;

namespace CSM.Web.Controllers.Common
{
    public class BaseController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseController));

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.CallId = !string.IsNullOrEmpty(this.CallId) ? this.CallId : Constants.NotKnown;
            ViewBag.PhoneNo = !string.IsNullOrEmpty(this.PhoneNo) ? this.PhoneNo : Constants.NotKnown;
            base.OnActionExecuting(filterContext);
        }

        protected UserEntity UserInfo
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
        
        protected string CallId
        {
            get { return this.ControllerContext.RouteData.Values["callId"].ConvertToString(); }
        }
        protected string PhoneNo
        {
            get { return this.ControllerContext.RouteData.Values["phoneNo"].ConvertToString(); }
        }

        public Dictionary<string, object> GetModelValidationErrors()
        {
            var errors = new Dictionary<string, object>();
            foreach (var key in ModelState.Keys)
            {
                if (ModelState[key].Errors.Count > 0)
                    errors[key] = ModelState[key].Errors[0].ErrorMessage;
            }

            return errors;
        }

        public ActionResult Error(HandleErrorInfo handleError)
        {
            if (handleError != null)
            {
                var controllerName = handleError.ControllerName;
                var actionName = handleError.ActionName;
                Logger.Error("Exception occur: Controller Name-" + controllerName + " Action Name-" + actionName + "\n", handleError.Exception);
            }

            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System
                });
            }

            return View("~/Views/Shared/Error.cshtml", handleError);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            try
            {
                if (filterContext.ExceptionHandled)
                {
                    return;
                }

                filterContext.ExceptionHandled = true;
                Exception exception = filterContext.Exception;

                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];
                var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                if (Request.IsAjaxRequest())
                {
                    filterContext.Result = Json(new
                    {
                        Valid = false,
                        Error = Resource.Error_System
                    });
                }
                else
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/Error.cshtml",
                        ViewData = new ViewDataDictionary(model),
                        TempData = filterContext.Controller.TempData
                    };
                }

                Logger.Error("Exception occur:\n", exception);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
        }

        protected void ClearSession()
        {
            Session["sessionid"] = null;

            // Reset routedata
            RouteData.Values.Remove("callId");
            RouteData.Values.Remove("phoneNo");

            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
        }
    }
}
