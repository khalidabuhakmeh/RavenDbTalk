using System.Web.Mvc;
using MvcFlash.Core;
using RavenDbTalk.Web.App_Start;
using RestfulRouting;

namespace RavenDbTalk.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Routes.Start();
            RavenDbConfiguration.Start();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            /* View Engines */
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RestfulRoutingRazorViewEngine());
        }
    }
}