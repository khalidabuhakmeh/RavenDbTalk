using System.Linq;
using System.Web.Mvc;
using MvcFlash.Core.Extensions;
using PagedList;
using Raven.Client;
using Raven.Client.Bundles.MoreLikeThis;
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

            var query = Db.Query<Quotes_Search.Result, Quotes_Search>()
                .If(model.HasQuery, q => q.Search(x => x.Content, model.Query, options: SearchOptions.And, escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard))
                .If(model.HasCategory, q => q.Where(x => x.Category == model.Category))
                .If(model.HasBy, q => q.Where(x => x.By == model.By))
                .OrderBy(x => x.By);

            model.Quotes = query
                .As<Quote>()
                .ToPagedList(model.Page, model.Size);

            if (!model.Quotes.Any())
            {
                model.Suggestions = query.Suggest();
            }

            return View(model);
        }

        public ActionResult Show(string id)
        {
            var quote = Db.Load<Quote>(id);

            if (quote == null)
            {
                Flash.Error("Ooops", "We didn't find the quote you were looking for");
                return RedirectToAction("index", "quotes");
            }

            var results = Db.Advanced.MoreLikeThis<Quote, Quotes_Search>(quote.Id);

            var model = new ShowModel
            {
                Quote = quote,
                MoreLikeThis = results.Take(5).ToList()
            };

            return View(model);
        }
    }
}
