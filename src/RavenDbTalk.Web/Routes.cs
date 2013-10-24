using System.Web.Mvc;
using System.Web.Routing;
using RestfulRouting;
using RavenDbTalk.Web.Controllers;

namespace RavenDbTalk.Web
{
    public class Routes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.DebugRoute("routedebug");
            map.Root<HomeController>(x => x.Index());

            map.Resources<QuotesController>();
            map.Resources<CategoriesController>();
            map.Resources<AuthorsController>();
            map.Resources<MenusController>();
            map.Resources<UserQuotesController>(uq =>
            {
                uq.As("user-quotes");
                uq.Only("create");
            });
            map.Resources<LiveQuotesController>(l =>
            {
                l.As("live");
                l.Only("index");
            });
        }

        public static void Start()
        {
            var routes = RouteTable.Routes;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoutes<Routes>();
        }
    }
}