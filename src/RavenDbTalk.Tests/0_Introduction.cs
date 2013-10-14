using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Raven.Client.Embedded;
using Raven.Database.Server.Responders;
using Raven.Tests.Helpers;
using Xunit;

namespace RavenDbTalk.Tests
{
    public class IntroductionTests : RavenTestBase
    {
        [Fact]
        public void Must_have_a_document_store()
        {
            // DocumentStores are at the core of RavenDB.
            // They allow you to access the database instance
            // of the application.
            // DocumentStores come in three flavors:
            //      - Remote
            //      - Embedded
            //      - In Memory


            // This is a RavenDB running in memory
            var documentStore = new EmbeddableDocumentStore {
                RunInMemory = true
            }
            // You need to initialize your documentstore
            // after you set the configuration above.
            .Initialize();

            // Remote example
            // var documentStore = new DocumentStore {
            //      ConnectionStringName = "RavenDb"
            //  }.Initialize();

            // Embedded example
            // var documentStore = new EmbeddableDocumentStore {
            //     ConnectionStringName = "RavenDb"
            // }

            documentStore.Should()
                .NotBeNull();

            // remember to dispose your document store
            // or let the application shutdown dispose it
            documentStore.Dispose();
        }

        [Fact]
        public void Must_use_a_session_to_execute_queries()
        {
            // DocumentSession is the way you execute
            // queries against your database. They are short
            // lived and should be disposed as soon as
            // you are done with using it.
            using (var documentStore = NewDocumentStore())
            {
                using (var session = documentStore.OpenSession())
                {
                    // we are executing a query using our session
                    // to get the objects in our database...
                    // oh there are none, we didn't do anything :)
                    var result = session.Query<object>().ToList();
                    result.Should().BeEmpty();
                }
            }
        }

        [Fact]
        public void Has_a_management_studio_built_right_in()
        {
            using (var documentStore = NewDocumentStore())
            {
                var studioUrl = documentStore.Configuration.ServerUrl;
                Debug.WriteLine(studioUrl);

                // put a breakpoint here and go to the
                // studioUrl location.
                WaitForUserToContinueTheTest(documentStore);

                // delete the document in the database to continue
            }

            const bool awesome = true;
            awesome.Should().BeTrue();
        }
    }
}
