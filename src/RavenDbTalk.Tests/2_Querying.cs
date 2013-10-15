using System.Linq;
using System.Runtime.InteropServices;
using FluentAssertions;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Tests.Helpers;
using RavenDbTalk.Tests.Models;
using Xunit;

namespace RavenDbTalk.Tests
{
    public class QueryingTests : RavenTestBase
    {
        private readonly EmbeddableDocumentStore _documentStore;

        public QueryingTests()
        {
            _documentStore = NewDocumentStore();

            // just seeding our database, nothing to see here
            using (var session = _documentStore.OpenSession())
            {
                var examples = Helper.GetExamples(200);
                examples.ForEach(session.Store);
                session.SaveChanges();
            }
        }

        [Fact]
        public void Can_get_first_or_default_of_a_collection_using_linq()
        {
            // RavenDb allows you to query using linq, and
            // will create automatic indexes for you.
            using (var session = _documentStore.OpenSession())
            {
                var result = session.Query<Example>()
                    // we need to wait for indexes, this is not a good practice
                    // this is purely done for testing purposes
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    // we just want the first one
                    .FirstOrDefault();

                WaitForUserToContinueTheTest(_documentStore);
                result.Should().NotBeNull();
            }
        }

        [Fact]
        public void Can_use_where_clause_with_a_collection_using_linq()
        {
            // RavenDb allows you to query using linq, and
            // will create automatic indexes for you.
            using (var session = _documentStore.OpenSession())
            {
                var result = session.Query<Example>()
                    // we need to wait for indexes, this is not a good practice
                    // this is purely done for testing purposes
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    // we are quering using LINQ!
                    .Where(x => x.Quantity > 0).ToList();

                WaitForUserToContinueTheTest(_documentStore);

                // we got some results!
                result.Count.Should().BeGreaterThan(1);
            }
        }

        [Fact]
        public void Can_use_any_to_query_a_collection_within_a_collection()
        {
            // RavenDb allows you to query using linq, and
            // will create automatic indexes for you.
            using (var session = _documentStore.OpenSession())
            {
                var result = session.Query<Example>()
                    // we need to wait for indexes, this is not a good practice
                    // this is purely done for testing purposes
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    // we are quering using LINQ!
                    .Where(x => x.Tags.Any(t => t == "product")).ToList();

                WaitForUserToContinueTheTest(_documentStore);

                // we got some results!
                result.Count.Should().BeGreaterThan(1);
            }
        }

        [Fact]
        public void Can_use_an_index_to_query_precomputed_results()
        {
            // We need to execute the index against our datastore
            // this will initiate an indexing process
            new Examples_ByManufacturer().Execute(_documentStore);
            WaitForIndexing(_documentStore);

            using (var session = _documentStore.OpenSession())
            {
                var result = session.Query<Example>()
                    // we need to wait for indexes, this is not a good practice
                    // this is purely done for testing purposes
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    // we are quering using LINQ!
                    .Where(x => x.Tags.Any(t => t == "product")).ToList();

                WaitForUserToContinueTheTest(_documentStore);

                // we got some results!
                result.Count.Should().BeGreaterThan(1);
            }
        }

        [Fact]
        public void Can_query_using_lucene_query_syntax()
        {
            new Examples_ByManufacturer().Execute(_documentStore);
            WaitForIndexing(_documentStore);

            using (var session = _documentStore.OpenSession())
            {
                // you need an index to execute a lucene query
                var result = session.Advanced.LuceneQuery<Example, Examples_ByManufacturer>()
                    // we need to wait for indexes, this is not a good practice
                    // this is purely done for testing purposes
                    .WaitForNonStaleResultsAsOfLastWrite()
                    // we are quering using LINQ!
                    .Where("Manufacturer: \"company #2\"").ToList();

                WaitForUserToContinueTheTest(_documentStore);

                // we got some results!
                result.Count.Should().BeGreaterThan(1);
            }
        }

        public class Examples_ByManufacturer : AbstractIndexCreationTask<Example>
        {
            public Examples_ByManufacturer()
            {
                // to create an index, all you need
                // to do is create a projection of what is
                // is searchable in your model. Be sure to
                // only include what is important for searches.
                Map = examples => from e in examples
                                  select new { e.Manufacturer };
            }
        }

        [Fact]
        public void Can_only_get_128_documents_to_avoid_n_plus_one()
        {
            using (var store = _documentStore)
            {
                new Examples_ByManufacturer().Execute(store);
                WaitForIndexing(store);

                using (var session = store.OpenSession())
                {
                    RavenQueryStatistics stats;
                    var result = session.Query<Example, Examples_ByManufacturer>()
                        .Statistics(out stats)
                        .ToList();

                    result.Count.Should().Be(128);
                    stats.TotalResults.Should().Be(200);
                }
            }

        }

        public override void Dispose()
        {
            _documentStore.Dispose();
        }
    }
}
