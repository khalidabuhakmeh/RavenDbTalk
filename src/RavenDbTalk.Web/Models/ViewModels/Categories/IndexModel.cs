﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using Raven.Abstractions.Data;

namespace RavenDbTalk.Web.Models.ViewModels.Categories
{
    public class IndexModel
    {
        public IndexModel()
        {
            Categories = new StaticPagedList<CategoryWithCount>(new List<CategoryWithCount>(), 1, 25, 0);
        }

        public IndexModel(SearchModel search)
        {
            Query = search.Q;
        }

        public IPagedList<CategoryWithCount> Categories { get; set; }
        public string Query { get; set; }

        public bool HasQuery { get { return !string.IsNullOrWhiteSpace(Query); } }
        public bool HasSuggestions { get { return !Categories.Any() && Suggestions.Suggestions.Any(); } }

        public string Header
        {
            get
            {
                var sb = new StringBuilder("");
                var list = new List<string>();

                if (HasQuery) list.Add(string.Format("\"{0}\"", Query));
                
                sb.Append(list.Any() ? string.Join("/", list) : "All");

                return sb.ToString();
            }
        }

        public SuggestionQueryResult Suggestions { get; set; }
    }
}