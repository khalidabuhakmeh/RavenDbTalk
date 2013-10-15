using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Raven.Abstractions.Extensions;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Tests.Helpers;
using RavenDbTalk.Tests.Models;
using Xunit;

namespace RavenDbTalk.Tests
{
    public class IndexesMapReduceTests : RavenTestBase
    {
        private readonly EmbeddableDocumentStore _documentStore;

        public IndexesMapReduceTests()
        {
            _documentStore = NewDocumentStore();

            // execute the indexes
            // IndexCreation.CreateIndexes(typeof(Example_Index).Assembly, _documentStore);

            // just seeding our database, nothing to see here
            using (var session = _documentStore.OpenSession())
            {
                var examples = Helper.GetExamples(200);
                examples.ForEach(session.Store);
                session.SaveChanges();
            }
        }

        [Fact]
        public void Can_create_a_dynamic_index()
        {
            using (var store = _documentStore)
            {
                store.DatabaseCommands.GetIndexes(1, 100).Count().Should().Be(0);

                using (var session = store.OpenSession())
                {
                    var result = session.Query<Example>()
                        .Where(x => x.Cost > 100)
                        .ToList();

                    store.DatabaseCommands.GetIndexes(1, 100).Count().Should().Be(1);
                }
            }
        }

        public class Example_Index : AbstractIndexCreationTask<Example, Example_Index.Result>
        {
            public class Result
            {
                public string Manufacturer { get; set; }
                public DateTimeOffset Date { get; set; }
                public decimal Cost { get; set; }
                public decimal Price { get; set; }
                public string[] Tags { get; set; }
            }

            public Example_Index()
            {
                Map = examples => from e in examples
                                  select new Result
                                  {
                                      Manufacturer = e.Manufacturer,
                                      Date = e.Date,
                                      Cost = e.Cost,
                                      Price = e.Price,
                                      Tags = e.Tags.ToArray()
                                  };
            }
        }

        [Fact]
        public void Can_create_an_index_for_a_set_of_documents()
        {
            using (var store = _documentStore)
            {
                new Example_Index().Execute(store);
                WaitForUserToContinueTheTest(store);

                // we are getting the index from the document store by name
                store.DatabaseCommands.GetIndex(new Example_Index().IndexName)
                    .Name.Should().BeEquivalentTo("example/index");
            }
        }

        public class Example_Tags : AbstractIndexCreationTask<Example, Example_Tags.Result>
        {
            public class Result
            {
                public string Tag { get; set; }
                public int Count { get; set; }
            }

            public Example_Tags()
            {
                Map = examples => from e in examples
                                  from t in e.Tags
                                  select new Result
                                  {
                                      Tag = t,
                                      Count = 1
                                  };

                Reduce = results => from r in results
                                    group r by r.Tag
                                        into g
                                        select new Result
                                        {
                                            Tag = g.Key,
                                            Count = g.Sum(x => x.Count)
                                        };

            }
        }

        [Fact]
        public void Can_create_a_map_reduce_over_a_set_of_documents()
        {
            using (var store = _documentStore)
            {
                new Example_Tags().Execute(store);
                WaitForIndexing(store);

                using (var session = store.OpenSession())
                {
                    var results = session.Query<Example_Tags.Result, Example_Tags>().ToList();

                    foreach (var result in results)
                        Debug.WriteLine(string.Format("{0} : {1}", result.Tag,  result.Count));

                    results.Any().Should().BeTrue();
                }
            }
        }

        public class Animals_Index : AbstractMultiMapIndexCreationTask<Animals_Index.Result>
        {
            public class Result
            {
                public string Name { get; set; }
                public string Owner { get; set; }
                public string Type { get; set; }
            }

            public Animals_Index()
            {
                AddMap<Cat>(cats => from c in cats select new { c.Name, c.Owner, Type = "cat" });
                AddMap<Dog>(dogs => from d in dogs select new { d.Name, d.Owner, Type = "dog" });
            }
        }

        [Fact]
        public void Can_create_an_index_over_multiple_documents()
        {
            using (var store = _documentStore)
            {
                new Animals_Index().Execute(store);

                using (var session = store.OpenSession())
                {
                    session.Store(new Cat { Name = "Grumpy Cat", Owner = "Khalid" });
                    session.Store(new Dog { Name = "Snoopy", Owner = "Khalid" });
                    session.SaveChanges();
                }

                WaitForIndexing(store);
                WaitForUserToContinueTheTest(store);

                using (var session = store.OpenSession())
                {
                    var results = session.Query<Animals_Index.Result, Animals_Index>()
                        .Where(a => a.Owner == "Khalid")
                        .OfType<object>()
                        .ToList();

                    foreach (var result in results)
                    {
                        if (result is Cat) ((Cat)result).Meooow();
                        if (result is Dog) ((Dog)result).Woof();
                    }

                    results.Count.Should().Be(2);
                }

            }
        }

        public override void Dispose()
        {
            _documentStore.Dispose();
        }
    }
}
