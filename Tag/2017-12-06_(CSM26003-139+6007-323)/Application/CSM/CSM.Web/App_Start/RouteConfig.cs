using System.Web.Mvc;
using System.Web.Routing;

namespace CSM.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "OpenServiceRequest",                                                      // Route name
                "ServiceRequest/View/{srNo}",                                              // URL with parameters
                new { controller = "ServiceRequest", action = "View", srNo = UrlParameter.Optional }, // Parameter defaults
                new string[] { "CSM.Web.Controllers" }
            );

            routes.MapRoute(
                "PortalRoute",                                                             // Route name
                "{callId}-{phoneNo}/{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                new { callId = "^[0-9a-zA-Z]+$", phoneNo = "^[0-9]+$" },
                //  new { callId = "^[0-9]+$", phoneNo = "^[0-9]+$" },
                new string[] { "CSM.Web.Controllers" }
            );

            routes.MapRoute(
                 "Default",                                                                 // Route name
                 "{controller}/{action}/{id}",                                              // URL with parameters
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                 new string[] { "CSM.Web.Controllers" }
            );
        }
    }
}