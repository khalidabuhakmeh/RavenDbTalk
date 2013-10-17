using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Abstractions.Indexing;
using Raven.Client.Bundles.MoreLikeThis;
using Raven.Client.Indexes;
using Raven.Tests.Helpers;
using RavenDbTalk.Tests.Models;
using Xunit;

namespace RavenDbTalk.Tests
{
    public class MoreLikeThisTests : RavenTestBase
    {
        public class FamoutQuotes_Index : AbstractIndexCreationTask<FamousQuote>
        {
            public FamoutQuotes_Index()
            {
                Map = quotes => from q in quotes
                                select new { q.Said  };

                Store(x => x.Said, FieldStorage.Yes);
            }
        }

        [Fact]
        public void Can_get_more_like_this_results()
        {
            using (var store = NewDocumentStore())
            {
                new FamoutQuotes_Index().Execute(store);

                using (var session = store.OpenSession())
                {
                    Helper.GetQuotes().ForEach(session.Store);
                    session.SaveChanges();
                }

                WaitForIndexing(store);

                using (var session = store.OpenSession())
                {
                    // I hard coded a document Id from the list of quotes
                    // I promise there is not extra magic happening with 
                    // the document Id.
                    var documentId = "famousquotes/not";
                    var result = session
                        .Advanced
                        // we specify how the "more like this" feature should behave
                        // and we provide the reference document
                        .MoreLikeThis<FamousQuote, FamoutQuotes_Index>(new MoreLikeThisQuery
                        {
                            DocumentId = documentId,
                            MinimumDocumentFrequency = 2,
                            MinimumTermFrequency = 1
                        });
                        // Do this when you have a larger data set
                        //.MoreLikeThis<FamousQuote, FamoutQuotes_Index>(documentId);

                    // more info at : http://cephas.net/blog/2008/03/30/how-morelikethis-works-in-lucene/

                    result.Count().Should().Be(3);

                    foreach (var famousQuote in result)
                        Debug.WriteLine("{0} said \"{1}\"", famousQuote.By, famousQuote.Said);
                }
            }
        }
    }
}
