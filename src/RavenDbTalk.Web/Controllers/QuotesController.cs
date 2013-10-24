using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Raven.Client;
using RavenDbTalk.Web.Models.Domain;
using RavenDbTalk.Web.Models.Extensions;
using RavenDbTalk.Web.Models.Indexes;
using RavenDbTalk.Web.Models.ViewModels.Quotes;

namespace RavenDbTalk.Web.Controllers
{
    public class QuotesController : ApplicationController
    {
        public ActionResult Index(SearchModel search)
        {
            var model = new IndexModel(search);
            model.Quotes =
              Db.Query<Quotes_Search.Result, Quotes_Search>()
                .If(model.HasQuery, q => q.Search(x => x.Content, model.Query))
                .If(model.HasCategory, q => q.Where(x => x.Category == model.Category))
                .If(model.HasBy, q => q.Where(x => x.By == model.By))
                .OrderBy(x => x.By)
                .As<Quote>()
                .ToPagedList(model.Page, model.Size);

            return View(model);
        }
    }
}
