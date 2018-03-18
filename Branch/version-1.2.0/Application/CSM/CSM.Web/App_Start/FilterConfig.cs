using System.Web.Mvc;
using CSM.Web.Filters;

namespace CSM.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogActionFilter());
            filters.Add(new ValidateInputAttribute(false)); //disable the default request validation
            filters.Add(new HandleAntiForgeryError());
        }
    }
}