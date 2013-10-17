using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Tests.Helpers;
using RavenDbTalk.Tests.Models;
using Xunit;

namespace RavenDbTalk.Tests
{
    public class Faceted_Search : RavenTestBase
    {
        public class Example_Index : AbstractIndexCreationTask<Example, IndexesMapReduceTests.Example_Index.Result>
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
                                  select new IndexesMapReduceTests.Example_Index.Result
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
        public void Can_perform_a_faceted_search()
        {
            using (var store = NewDocumentStore())
            {
                new Example_Index().Execute(store);

                // we need to define our facets, this is what documents
                // could potentially be categorized by. You can choose any
                // property that exists on your document.
                var facets = new List<Facet> {
                    // manufacturer's vary by name
                    new Facet<Example> {Name = x => x.Manufacturer},
                    // price's vary by decimal values
                    new Facet<Example>
                    {
                        Name = x => x.Price,
                        Ranges =
                        {
                            x => x.Price < 9.99m,
                            x => x.Price > 9.99m && x.Price < 49.99m,
                            x => x.Price > 49.99m && x.Price < 99.99m,
                            x => x.Price > 99.99m
                        }
                    },
                    // cost varies by decimal values
                    new Facet<Example>
                    {
                        Name = x => x.Cost,
                        Ranges =
                        {
                            x => x.Cost < 9.99m,
                            x => x.Cost > 9.99m && x.Cost < 49.99m,
                            x => x.Cost > 49.99m && x.Cost < 99.99m,
                            x => x.Cost > 99.99m
                        }
                    },
                };

                using (var session = store.OpenSession())
                {
                    Helper.GetExamples(100).ForEach(session.Store);
                    // Facets are saved using the FacetSetup class,
                    // and you will need to define an identifer.
                    // It is just a document in RavenDB
                    session.Store(new FacetSetup
                    {
                        Id = "facets/examples",
                        Facets = facets
                    });
                    session.SaveChanges();
                }

                WaitForIndexing(store);
                WaitForUserToContinueTheTest(store);

                using (var session = store.OpenSession())
                {
                    // using our index, we get the facets.
                    // additionally you can use linq to refine
                    // facets by using a where clause.
                    var results = session.Query<Example, Example_Index>()
                        .ToFacets("facets/examples");

                    results.Should().NotBeNull();

                    foreach (var facet in results.Results)
                    {
                        Debug.WriteLine(facet.Key + ":");
                        foreach (var value in facet.Value.Values)
                            Debug.WriteLine("   {0} : {1}", value.Range, value.Hits);

                    }
                }
            }
        }
    }
}
