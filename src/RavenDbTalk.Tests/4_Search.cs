using System;
using System.Linq;
using FluentAssertions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Tests.Helpers;
using RavenDbTalk.Tests.Models;
using Xunit;

namespace RavenDbTalk.Tests
{
    public class SearchTests : RavenTestBase
    {
        public class Quotes_Search : AbstractIndexCreationTask<FamousQuote>
        {
            public Quotes_Search()
            {
                Map = quotes => from q in quotes
                                select new { q.Said, q.By };

                Index(x => x.Said, FieldIndexing.Analyzed);
                Index(x => x.By, FieldIndexing.Analyzed);
            }
        }

        [Fact]
        public void Can_search_based_on_a_single_field()
        {
            using (var store = NewDocumentStore())
            {
                new Quotes_Search().Execute(store);

                SeedWithQuotes(store);
                WaitForIndexing(store);
                WaitForUserToContinueTheTest(store);

                using (var session = store.OpenSession())
                {
                    var results = session.Query<FamousQuote, Quotes_Search>()
                        .Search(x => x.Said, "all")
                        .ToList();

                    results.Count.Should().Be(2);
                    results.All(x => x.By == "Walt Disney" || x.By == "Abraham Lincoln").Should().BeTrue();

                    foreach (var famousQuote in results)
                        Console.WriteLine(famousQuote);
                }
            }
        }

        public class Quotes_ComplexSearch : AbstractIndexCreationTask<FamousQuote, Quotes_ComplexSearch.Result>
        {
            public class Result
            {
                public object[] Content { get; set; }
            }

            public Quotes_ComplexSearch()
            {
                Map = quotes => from q in quotes
                                select new
                                {
                                    Content = new object[] { q.Said, q.By }
                                };

                Index(x => x.Content, FieldIndexing.Analyzed);
            }
        }

        [Fact]
        public void Can_search_based_on_multiple_fields()
        {
            using (var store = NewDocumentStore())
            {
                new Quotes_ComplexSearch().Execute(store);

                SeedWithQuotes(store);
                WaitForIndexing(store);
                WaitForUserToContinueTheTest(store);

                using (var session = store.OpenSession())
                {
                    var results = session.Query<Quotes_ComplexSearch.Result, Quotes_ComplexSearch>()
                        .Search(x => x.Content, "lincoln")
                        .OfType<FamousQuote>()
                        .ToList();

                    results.Count.Should().Be(1);
                    results.All(x => x.By == "Abraham Lincoln").Should().BeTrue();

                    foreach (var famousQuote in results)
                        Console.WriteLine(famousQuote);
                }
            }
        }

        [Fact]
        public void Can_do_wildcard_searches()
        {
            using (var store = NewDocumentStore())
            {
                new Quotes_ComplexSearch().Execute(store);

                SeedWithQuotes(store);
                WaitForIndexing(store);
                WaitForUserToContinueTheTest(store);

                using (var session = store.OpenSession())
                {
                    var results = session.Query<Quotes_ComplexSearch.Result, Quotes_ComplexSearch>()
                        .Search(x => x.Content, "dream*", escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard)
                        .OfType<FamousQuote>()
                        .ToList();

                    results.Count.Should().Be(2);

                    foreach (var famousQuote in results)
                        Console.WriteLine(famousQuote);
                }
            }
        }


        [Fact]
        public void Can_suggest_words_when_mispellings_occur()
        {
            using (var store = NewDocumentStore())
            {
                new Quotes_Search().Execute(store);

                SeedWithQuotes(store);
                WaitForIndexing(store);
                WaitForUserToContinueTheTest(store);

                using (var session = store.OpenSession())
                {
                    var result = session.Query<FamousQuote, Quotes_Search>()
                        .Where(x => x.By == "jarmes")
                        .Suggest();

                    result.Should().NotBeNull();
                    result.Suggestions.FirstOrDefault().Should().Be("james");

                    foreach (var suggestion in result.Suggestions)
                        Console.WriteLine(suggestion);
                }
            }
        }

        [Fact]
        public void Can_suggest_words_for_multiple_mispellings()
        {
            using (var store = NewDocumentStore())
            {
                new Quotes_Search().Execute(store);

                SeedWithQuotes(store);
                WaitForIndexing(store);
                WaitForUserToContinueTheTest(store);

                using (var session = store.OpenSession())
                {
                    var result = session.Query<FamousQuote, Quotes_Search>()
                        .Suggest(new SuggestionQuery
                        {
                            Field = "By",
                            Term = "<<jarmes abroham>>",
                            Accuracy = 0.4f,
                            MaxSuggestions = 5,
                            Distance = StringDistanceTypes.JaroWinkler,
                            Popularity = true
                        });

                    result.Should().NotBeNull();
                    result.Suggestions.Count().Should().BeGreaterOrEqualTo(1);

                    foreach (var suggestion in result.Suggestions)
                        Console.WriteLine(suggestion);
                }
            }
        }

        private static void SeedWithQuotes(EmbeddableDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                session.Store(new FamousQuote
                {
                    By = "Abraham Lincoln",
                    Said = "Better to remain silent and be thought a fool than to speak out and remove all doubt."
                });
                session.Store(new FamousQuote
                {
                    By = "Walt Disney",
                    Said = "All our dreams can come true, if we have the courage to pursue them."
                });
                session.Store(new FamousQuote
                {
                    By = "James Dean",
                    Said = "Dream as if you'll live forever. Live as if you'll die today."
                });

                session.SaveChanges();
            }
        }
    }
}
