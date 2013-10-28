using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using PagedList;
using Raven.Abstractions.Data;
using RavenDbTalk.Web.Models.Domain;

namespace RavenDbTalk.Web.Models.ViewModels.Quotes
{
    public class IndexModel
    {
        public IndexModel()
        {
            Quotes = new PagedList<Quote>(new List<Quote>(), 1, 25);
            Suggestions = new SuggestionQueryResult();
        }

        public IndexModel(SearchModel search)
            : this()
        {
            Query = search.Q;
            Category = search.Category;
            By = search.By;
            Page = search.Page;
            Size = search.Size;
        }

        public int Page { get; set; }
        public int Size { get; set; }

        public IPagedList<Quote> Quotes { get; set; }
        public string Query { get; set; }
        public string Category { get; set; }
        public string By { get; set; }

        public bool HasQuery { get { return !string.IsNullOrWhiteSpace(Query); } }
        public bool HasCategory { get { return !string.IsNullOrWhiteSpace(Category); } }
        public bool HasBy { get { return !string.IsNullOrWhiteSpace(By); } }
        public bool HasSuggestions { get { return !Quotes.Any() && Suggestions != null && Suggestions.Suggestions.Any(); } }

        public string Header
        {
            get
            {
                var sb = new StringBuilder("");
                var list = new List<string>();

                if (HasCategory) list.Add(Category);
                if (HasBy) list.Add(By);
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

        public bool CanGetSuggestions
        {
            get
            {
                return (HasQuery || HasCategory || HasBy) && Quotes.Any();
            }
        }
    }
}