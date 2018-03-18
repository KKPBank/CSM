using System;
using System.Diagnostics;
using System.Text;
using System.Web.Mvc;
using log4net;

namespace CSM.Web.Filters
{
    public sealed class LogActionFilter : ActionFilterAttribute
    {
        private const string StopwatchKey = "DebugLoggingStopWatch";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogActionFilter));

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (
                filterContext.RouteData.GetRequiredString("controller")
                    .Equals("MenuNavigator", StringComparison.CurrentCultureIgnoreCase) &&
                filterContext.RouteData.GetRequiredString("action")
                    .Equals("MainMenu", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            if (Logger.IsInfoEnabled)
            {
                var loggingWatch = Stopwatch.StartNew();
                filterContext.HttpContext.Items.Add(StopwatchKey, loggingWatch);

                var message = new StringBuilder();
                message.AppendFormat("I:--START--:--OnActionExecuting ({0}, {1})--",
                    filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    filterContext.ActionDescriptor.ActionName);

                ThreadContext.Properties["EventClass"] = filterContext.ActionDescriptor.ActionName;
                Logger.Info(message);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (
                filterContext.RouteData.GetRequiredString("controller")
                    .Equals("MenuNavigator", StringComparison.CurrentCultureIgnoreCase) &&
                filterContext.RouteData.GetRequiredString("action")
                    .Equals("MainMenu", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            if (Logger.IsInfoEnabled)
            {
                if (filterContext.HttpContext.Items[StopwatchKey] != null)
                {
                    var loggingWatch = (Stopwatch)filterContext.HttpContext.Items[StopwatchKey];
                    loggingWatch.Stop();

                    long timeSpent = loggingWatch.ElapsedMilliseconds;

                    var message = new StringBuilder();
                    message.AppendFormat("O:--OnActionExecuted ({0}, {1})--:TimeSpent/{2}",
                        filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                        filterContext.ActionDescriptor.ActionName, timeSpent);

                    Logger.Info(message);
                    filterContext.HttpContext.Items.Remove(StopwatchKey);
                }
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}