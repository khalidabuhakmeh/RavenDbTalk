using System.Web.Mvc;

namespace RavenDbTalk.Web.Controllers
{
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
