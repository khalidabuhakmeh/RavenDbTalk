using System.Diagnostics;
using FluentAssertions;
using Raven.Tests.Helpers;
using Xunit;

namespace RavenDbTalk.Tests
{
    public class BulkImportTests : RavenTestBase
    {
        [Fact]
        public void Can_do_a_bulk_insert_operation()
        {
            using (var store = NewDocumentStore())
            {
                // let's load the documents the regular
                // way, one at a time. Works, but really
                // inefficent and slow.
                var normal = new Stopwatch();
                normal.Start();
                
                // 1000 documents
                for (var i = 0; i < 1000; i++)
                {
                    using (var session = store.OpenSession())
                    {
                        session.Store(new { Id = null as string, Count = i });
                        session.SaveChanges();
                    }
                }
                normal.Stop();

                var bulk = new Stopwatch();
                bulk.Start();
                using (var bulkInsert = store.BulkInsert())
                {
                    // we are firehosing documents
                    // int ravendb, and forgoing the
                    // niceties of a session in return
                    // for SPEED!!!!!
                    for (var i = 0; i < 1000; i++)
                    {
                        bulkInsert.Store(new { Id = "fast/" + i, Count = i });
                    }
                }
                bulk.Stop();

                Debug.WriteLine(string.Format("normal : {0}", normal.Elapsed.TotalSeconds));
                Debug.WriteLine(string.Format("bulk : {0}", bulk.Elapsed.TotalSeconds));

                // Bulk insert is always faster
                bulk.Elapsed.Should().BeLessOrEqualTo(normal.Elapsed);

                // ## Limitations
                // - Entity Id must be provided at the client side. The client by default will use the HiLo generator in order to generate the Id.
                // - Transactions are per batch, not per operation and DTC transactions are not supported.
                // - Documents inserted using bulk-insert will not raise notifications. More about Changes API can be found here.
                // - Document Updates and Reference Checking must be explicitly turned on (see BulkInsertOptions).
                // - AfterCommit method in Put Triggers will be not executed in contrast to AllowPut, AfterPut and OnPut.
            }
        }
    }
}
