using System.Web.Mvc;
using MvcFlash.Core.Extensions;
using RavenDbTalk.Web.Models.Domain;

namespace RavenDbTalk.Web.Controllers
{
    public class UserQuotesController : ApplicationController
    {
        public ActionResult Create(Quote quote)
        {
            if (ModelState.IsValid)
            {
                quote.Category = "user quotes";
                Db.Store(quote);

                Flash.Success("Hooray!", "We have received you quote, look for it to show up soon");
            }
            else
            {
                Flash.Error("Doh!", "sorry it looks like your quote was incomplete, please try again");
            }

            return RedirectToAction("index", "home");
        }
    }
}
