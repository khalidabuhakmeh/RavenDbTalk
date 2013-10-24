using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using RavenDbTalk.Web.Models.Domain;

namespace RavenDbTalk.Web.Models.Indexes
{
    public class Quotes_ByCategory : AbstractIndexCreationTask<Quote, Quotes_ByCategory.Result>
    {
        public class Result
        {
            public string Category { get; set; }
            public int Count { get; set; }
        }

        public Quotes_ByCategory()
        {
            Map = quotes => from q in quotes
                            select new
                                   {
                                       q.Category,
                                       Count = 1
                                   };

            Reduce = results => from r in results
                                group r by r.Category
                                    into g
                                    select new
                                    {
                                        Category = g.Key,
                                        Count = g.Sum(x => x.Count),
                                    };

            Index(x => x.Category, FieldIndexing.Analyzed);
        }
    }
}