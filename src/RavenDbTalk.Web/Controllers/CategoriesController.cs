using System.Linq;
using System.Web.Mvc;
using PagedList;
using Raven.Client;
using RavenDbTalk.Web.Models.Extensions;
using RavenDbTalk.Web.Models.Indexes;
using RavenDbTalk.Web.Models.ViewModels;
using RavenDbTalk.Web.Models.ViewModels.Categories;

namespace RavenDbTalk.Web.Controllers
{
    public class CategoriesController : ApplicationController
    {
        public ActionResult Index(SearchModel search)
        {
            var model = new IndexModel(search);

            var query = Db.Query<Quotes_ByCategory.Result, Quotes_ByCategory>()
                .If(model.HasQuery, q => q.Search(x => x.Category, model.Query, escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard))
                .OrderByDescending(x => x.Count);

            model.Categories =  query.As<CategoryWithCount>()
                .ToPagedList(search.Page, search.Size);

            if (!model.Categories.Any())
            {
                model.Suggestions = query.Suggest();
            }

            return View(model);
        }
    }
}
