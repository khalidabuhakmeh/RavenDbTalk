using System;
using System.Diagnostics;
using System.Threading;
using FluentAssertions;
using Raven.Tests.Helpers;
using Xunit;

namespace RavenDbTalk.Tests
{
    public class ChangesTests : RavenTestBase
    {
        public bool ChangesOccured = false;

        [Fact]
        public void Can_listen_to_changes_api()
        {
            using (var store = NewDocumentStore())
            {
                store.Changes()
                    .ForAllDocuments()
                    //.ForAllIndexes()
                    //.ForAllReplicationConflicts()
                    //.ForBulkInsert()
                    //.ForDocument(/*docId*/)
                    //.ForDocumentsStartingWith("features/")
                    //.ForIndex("indexName")
                    // uses the Reactive Extensions package [rx-main] from NuGet.org 
                    .Subscribe(change =>
                    {
                        Debug.WriteLine(string.Format("we changed document {0}", change.Id));
                        ChangesOccured = true;
                    });

                // we haven't changed anything yet
                ChangesOccured.Should().BeFalse();

                using (var session = store.OpenSession())
                {
                    session.Store(new { Id = "trigger/change", text = "yes we can!" });
                    session.SaveChanges();
                }

                Thread.Sleep(1000);

                // We made the change and 
                // we changed this field
                ChangesOccured.Should().BeTrue();
            }
        }
    }


}
