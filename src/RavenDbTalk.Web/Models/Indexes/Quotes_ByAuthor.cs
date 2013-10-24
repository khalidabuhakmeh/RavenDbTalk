using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using RavenDbTalk.Web.Models.Domain;

namespace RavenDbTalk.Web.Models.Indexes
{
    public class Quotes_ByAuthor : AbstractIndexCreationTask<Quote, Quotes_ByAuthor.Result>
    {
        public class Result
        {
            public string By { get; set; }
            public int Count { get; set; }
        }

        public Quotes_ByAuthor()
        {
            Map = quotes => from q in quotes
                            select new
                                   {
                                       q.By,
                                       Count = 1
                                   };

            Reduce = results => from r in results
                                group r by r.By
                                    into g
                                    select new
                                           {
                                               By = g.Key,
                                               Count = g.Sum(x => x.Count)
                                           };

            Index(x => x.By, FieldIndexing.Analyzed);
        }
    }
}