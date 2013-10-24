using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using RavenDbTalk.Web.Models.Indexes;
using RavenDbTalk.Web.Models.ViewModels.Categories;
using RavenDbTalk.Web.Models.ViewModels.Menus;
using IndexModel = RavenDbTalk.Web.Models.ViewModels.Menus.IndexModel;

namespace RavenDbTalk.Web.Controllers
{
    public class MenusController : ApplicationController
    {
        [ChildActionOnly]
        public ActionResult Index()
        {
            var model = new IndexModel();
            
            var categories = Db.Query<Quotes_ByCategory.Result, Quotes_ByCategory>()
                .OrderByDescending(x => x.Count)
                .ThenByDescending(x => x.Category)
                .Take(5)
                .As<CategoryWithCount>()
                .Lazily();

            var authors  = Db.Query<Quotes_ByAuthor.Result, Quotes_ByAuthor>()
                .OrderByDescending(x => x.Count)
                .ThenByDescending(x => x.By)
                .Take(5)
                .As<AuthorViewModel>()
                .Lazily();

            Db.Advanced.Eagerly.ExecuteAllPendingLazyOperations();

            model.Categories = categories.Value.ToList();
            model.Authors = authors.Value.ToList();

            return PartialView(model);
        }
    }
}
