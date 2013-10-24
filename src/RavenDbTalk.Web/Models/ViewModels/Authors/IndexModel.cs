using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using Raven.Abstractions.Data;
using RavenDbTalk.Web.Models.ViewModels.Menus;

namespace RavenDbTalk.Web.Models.ViewModels.Authors
{
    public class IndexModel
    {
        public IndexModel()
        {
            Authors = new StaticPagedList<AuthorWithCount>(new List<AuthorWithCount>(), 1, 25, 0);
            Suggestions = new SuggestionQueryResult();
        }

        public IndexModel(SearchModel search)
        {
            Query = search.Q;
        }

        public IPagedList<AuthorWithCount> Authors { get; set; }
        public string Query { get; set; }

        public bool HasQuery { get { return !string.IsNullOrWhiteSpace(Query); } }
        public bool HasSuggestions { get { return !Authors.Any() && Suggestions.Suggestions.Any(); } }

        public string Header
        {
            get
            {
                var sb = new StringBuilder("");
                var list = new List<string>();
                
                if (HasQuery) list.Add(string.Format("\"{0}\"", Query));

                if (list.Any())
                {
                    sb.Append(string.Join("/", list));
                }
                else
                {
                    sb.Append("All");
                }

                return sb.ToString();
            }
        }

        public SuggestionQueryResult Suggestions { get; set; }
    }

    public class AuthorWithCount
    {
        public string By { get; set; }
        public int Count { get; set; }
    }
}