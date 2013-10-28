using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using RavenDbTalk.Web.Models.Domain;
using RavenDbTalk.Web.Models.Indexes;
using RavenDbTalk.Web.Models.ViewModels.Authors;
using RavenDbTalk.Web.Models.ViewModels.Categories;
using IndexModel = RavenDbTalk.Web.Models.ViewModels.Home.IndexModel;

namespace RavenDbTalk.Web.Controllers
{
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            var model = new IndexModel();

            var quote = Db.Query<Quotes_Search.Result, Quotes_Search>()
                .Customize(x => x.RandomOrdering())
                .As<Quote>()
                .Take(1)
                .Lazily();

            var categories = Db.Query<Quotes_ByCategory.Result, Quotes_ByCategory>()
                .Customize(x => x.RandomOrdering())
                .As<CategoryWithCount>()
                .Take(6)
                .Lazily();

            var authors = Db.Query<Quotes_ByAuthor.Result, Quotes_ByAuthor>()
                .Customize(x => x.RandomOrdering())
                .As<AuthorWithCount>()
                .Take(6)
                .Lazily();

            /* execute all queries at once: Optimized! */
            Db.Advanced.Eagerly.ExecuteAllPendingLazyOperations();

            model.RandomAuthors = authors.Value.ToList();
            model.RandomQuote = quote.Value.FirstOrDefault() ?? new Quote();
            model.RandomCategories = categories.Value.ToList();

            return View(model);
        }
    }
}
