using System.Collections.Generic;
using PagedList;

namespace RavenDbTalk.Web.Models.ViewModels.Categories
{
    public class IndexModel
    {
        public IndexModel()
        {
            Categories = new StaticPagedList<CategoryWithCount>(new List<CategoryWithCount>(), 1, 25, 0);
        }

        public IPagedList<CategoryWithCount> Categories { get; set; }
    }
}