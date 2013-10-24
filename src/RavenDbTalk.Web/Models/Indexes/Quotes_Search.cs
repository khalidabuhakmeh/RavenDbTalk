using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using RavenDbTalk.Web.Models.Domain;

namespace RavenDbTalk.Web.Models.Indexes
{
    public class Quotes_Search : AbstractIndexCreationTask<Quote, Quotes_Search.Result>
    {
        public class Result
        {
            public string Category { get; set; }
            public object[] Content { get; set; }
            public string By { get; set; }
        }

        public Quotes_Search()
        {
            Map = quotes =>
                from q in quotes
                select new
                {
                    q.Category,
                    q.By,
                    Content = new[] { q.By, q.Category, q.Text }
                };

            StoreAllFields(FieldStorage.Yes);
            Index(x => x.Content, FieldIndexing.Analyzed);
        }
    }
}