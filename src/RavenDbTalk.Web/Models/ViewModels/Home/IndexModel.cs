using System.Collections.Generic;
using RavenDbTalk.Web.Models.Domain;
using RavenDbTalk.Web.Models.ViewModels.Authors;
using RavenDbTalk.Web.Models.ViewModels.Categories;

namespace RavenDbTalk.Web.Models.ViewModels.Home
{
    public class IndexModel
    {
        public IndexModel()
        {
            RandomAuthors = new List<AuthorWithCount>();
            RandomQuote = new Quote();
            RandomCategories = new List<CategoryWithCount>();
        }

        public IList<CategoryWithCount> RandomCategories { get; set; }
        public Quote RandomQuote { get; set; }
        public IList<AuthorWithCount> RandomAuthors { get; set; }
    }
}