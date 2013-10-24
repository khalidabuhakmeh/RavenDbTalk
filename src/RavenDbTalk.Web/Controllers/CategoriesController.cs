using System.Linq;
using System.Web.Mvc;
using PagedList;
using Raven.Client;
using RavenDbTalk.Web.Models.Indexes;
using RavenDbTalk.Web.Models.ViewModels;
using RavenDbTalk.Web.Models.ViewModels.Categories;

namespace RavenDbTalk.Web.Controllers
{
    public class CategoriesController : ApplicationController
    {
        public ActionResult Index(SearchModel search)
        {
            var model = new IndexModel();

            model.Categories = Db.Query<Quotes_ByCategory.Result, Quotes_ByCategory>()
                .OrderByDescending(x => x.Count)
                .As<CategoryWithCount>()
                .ToPagedList(search.Page, search.Size);

            return View(model);
        }

    }
}
