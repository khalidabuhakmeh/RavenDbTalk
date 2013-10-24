using System.Collections.Generic;
using RavenDbTalk.Web.Models.ViewModels.Categories;

namespace RavenDbTalk.Web.Models.ViewModels.Menus
{
    public class IndexModel
    {
        public IndexModel()
        {
            Categories = new List<CategoryWithCount>();
            Authors = new List<AuthorViewModel>();
        }

        public IList<CategoryWithCount> Categories { get; set; }
        public IList<AuthorViewModel> Authors { get; set; }
    }

    public class AuthorViewModel
    {
        public string By { get; set; }
        public int Count { get; set; }
    }
}