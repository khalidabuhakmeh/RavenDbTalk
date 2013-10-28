using System.Linq;
using System.Web.Mvc;
using PagedList;
using Raven.Client;
using Raven.Client.Linq;
using RavenDbTalk.Web.Models.Extensions;
using RavenDbTalk.Web.Models.Indexes;
using RavenDbTalk.Web.Models.ViewModels;
using RavenDbTalk.Web.Models.ViewModels.Authors;

namespace RavenDbTalk.Web.Controllers
{
    public class AuthorsController : ApplicationController
    {
        public ActionResult Index(SearchModel search)
        {
            var model = new IndexModel(search);

            var query = Db.Query<Quotes_ByAuthor.Result, Quotes_ByAuthor>()
                .If(model.HasQuery, q => q.Search(x => x.By, model.Query, escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard))
                .OrderBy(x => x.By);

            model.Authors = query
                .As<AuthorWithCount>()
                .ToPagedList(search.Page, search.Size);

            if (model.CanGetSuggestions)
                model.Suggestions = query.Suggest();

            return View(model);
        }
    }
}
