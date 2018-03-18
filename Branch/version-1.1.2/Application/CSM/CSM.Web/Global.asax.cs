using System;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using CSM.Business;
using CSM.Entity;
using CSM.Web.Models;
using log4net;
using CSM.Common.Utilities;
using System.Web;
using System.Collections.Generic;

namespace CSM.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private int _cacheTimeoutInMinute = 20;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_BeginRequest()
        {
            System.Web.HttpContext.Current.Response.AddHeader("P3P", "CP=\"NOI CURa ADMa DEVa TAIa OUR BUS IND UNI COM NAV INT\"");
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AuthConfig.RegisterAuth();
            //registering our custom model validation provider
            ModelValidatorProviders.Providers.Clear();
            ModelValidatorProviders.Providers.Add(new DisallowHtmlMetadataValidationProvider());

            ConfigureLogging();
        }

        protected void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            if (Context.Handler is IRequiresSessionState)
            {
                log4net.ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    log4net.ThreadContext.Properties["UserID"] = HttpContext.Current.User.Identity.Name;
                }
            }
        }
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            //Logger.Debug("--START--:--Get User--"); 

            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                string encTicket = authCookie.Value;
                if (!string.IsNullOrEmpty(encTicket))
                {
                    var ticket = FormsAuthentication.Decrypt(encTicket);
                    var id = new UserIdentity(ticket);
                    IUserFacade userFacade = null;

                    try
                    {
                        UserEntity userInfo = null;
                        var cacheKey = string.Format(CultureInfo.InvariantCulture, "{0}_user_info", id.Name);

                        if (HttpRuntime.Cache[cacheKey] != null)
                        {
                            userInfo = (UserEntity) HttpRuntime.Cache[cacheKey];
                        }
                        else
                        {
                            userFacade = new UserFacade();
                            userInfo = userFacade.GetUserByLogin(id.Name);
                            HttpRuntime.Cache.Insert(cacheKey, userInfo, null,
                                DateTime.Now.AddMinutes(_cacheTimeoutInMinute), Cache.NoSlidingExpiration);
                        }

                        id.UserInfo = userInfo;
                        var userRoles = new string[] {id.UserInfo.RoleValue.ConvertToString()};
                        var prin = new GenericPrincipal(id, userRoles);
                        HttpContext.Current.User = prin;
                        //Logger.DebugFormat("--SUCCESS--:--Get User--:Username/{0}", id.UserInfo.Username); 
                    }
                    catch (CustomException cex)
                    {
                        Logger.Error("CustomException occur:\n", cex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        if (userFacade != null)
                        {
                            userFacade.Dispose();
                        }
                    }
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var controllerName = Request.RequestContext.RouteData.Values["controller"].ToString();

                Exception exception = Server.GetLastError();
                Response.Clear();

                var httpException = exception as HttpException;

                if (httpException != null)
                {
                    string action = string.Empty;

                    switch (httpException.GetHttpCode())
                    {
                        // page not found
                        case 404:
                            action = MappingRouteAction(controllerName);
                            break;
                    }

                    // clear error on server
                    Server.ClearError();
                    if (!string.IsNullOrWhiteSpace(action))
                    {
                        Response.Redirect(String.Format("~/{0}/{1}", controllerName, action));
                    }
                    else
                    {
                        Response.Redirect(String.Format("~/{0}/{1}", "Home", "Index"));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            
        }

        private string MappingRouteAction(string controllerName)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Customer", "Search");
            dic.Add("Area", "Search");
            dic.Add("CommPool", "Search");
            dic.Add("Configuration", "Search");
            dic.Add("Job", "Search");
            dic.Add("MappingProductType", "Search");
            dic.Add("News", "Search");
            dic.Add("Product", "Search");
            dic.Add("Question", "Index");
            dic.Add("QuestionGroup", "Index");
            dic.Add("Sla", "Search");
            dic.Add("SubArea", "SearchSelectSubAreaList");
            dic.Add("Type", "Index");
            dic.Add("UserMonitoring", "Search");
            dic.Add("Contact", "Search");

            return dic.Where(x => x.Key == controllerName).Select(x => x.Value).FirstOrDefault();
              
        }

        private void ConfigureLogging()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();

                // Set logfile name and application name variables
                log4net.GlobalContext.Properties["ApplicationCode"] = Constants.SystemName.CSM;
                log4net.GlobalContext.Properties["ServerName"] = System.Environment.MachineName;

                // Record application startup
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
        }
    }
}