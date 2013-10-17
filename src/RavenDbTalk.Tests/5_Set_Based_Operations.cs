using System.Linq;
using FluentAssertions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client.Indexes;
using Raven.Tests.Helpers;
using RavenDbTalk.Tests.Models;
using Xunit;

namespace RavenDbTalk.Tests
{
    public class SetBasedOperationsTests : RavenTestBase
    {
        [Fact]
        public void Can_delete_a_set_of_documents_based_on_a_index()
        {
            using (var store = NewDocumentStore())
            {
                var index = new RavenDocumentsByEntityName();
                index.Execute(store);

                using (var session = store.OpenSession())
                {
                    Helper.GetExamples(100).ForEach(session.Store);
                    session.SaveChanges();
                }

                WaitForIndexing(store);

                // now let's delete some documents!
                store.DatabaseCommands.DeleteByIndex(index.IndexName, new IndexQuery())
                    .WaitForCompletion();

                using (var session = store.OpenSession())
                {
                    session.Query<Example, RavenDocumentsByEntityName>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                        .Count()
                        .Should().Be(0);
                }
            }
        }

        [Fact]
        public void Can_update_a_set_of_documents_based_on_an_index()
        {
            using (var store = NewDocumentStore())
            {
                var index = new RavenDocumentsByEntityName();
                index.Execute(store);

                using (var session = store.OpenSession())
                {
                    Helper.GetExamples(100).ForEach(session.Store);
                    session.SaveChanges();
                }

                WaitForIndexing(store);

                // now let's update some documents!
                store.DatabaseCommands.UpdateByIndex(index.IndexName, new IndexQuery(),
                    new[] {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Add,
                            Name = "Tags",
                            Value = "magic"
                        }
                    }).WaitForCompletion();

                using (var session = store.OpenSession())
                {
                    session.Query<Example, RavenDocumentsByEntityName>()
                        .FirstOrDefault()
                        .Tags
                        .Should().Contain("magic");
                }
            }
        }
    }
}
