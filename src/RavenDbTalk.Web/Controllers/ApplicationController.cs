using System;
using System.Web.Mvc;
using MvcFlash.Core;
using Raven.Client;
using RavenDbTalk.Web.App_Start;
using RestfulRouting.Format;

namespace RavenDbTalk.Web.Controllers
{
    public abstract class ApplicationController : Controller
    {
        public IDocumentSession Db { get; private set; }
        public IFlashPusher Flash { get { return MvcFlash.Core.Flash.Instance; } }

        protected ActionResult RespondTo(Action<FormatCollection> format)
        {
            return new FormatResult(format);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Db = RavenDbConfiguration.DocumentStore.OpenSession();
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            using (Db)
            {
                if (filterContext.Exception == null ||  filterContext.ExceptionHandled)
                    Db.SaveChanges();
            }

            base.OnActionExecuted(filterContext);
        }
    }
}