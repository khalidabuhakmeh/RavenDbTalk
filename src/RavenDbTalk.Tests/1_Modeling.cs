using System.Diagnostics;
using FluentAssertions;
using Raven.Tests.Helpers;
using RavenDbTalk.Tests.Models;
using Xunit;

namespace RavenDbTalk.Tests
{
    /// <summary>
    /// A.K.A. things your ORM can't do :P
    /// </summary>
    public class ModelingTests : RavenTestBase
    {
        [Fact]
        public void Must_have_an_id_on_a_document()
        {
            // RavenDb is a document database that operates
            // on conventions. One of the important conventions
            // is that all models have an "Id" property which is
            // a string.

            var document = new { Id = "", Name = "Khalid Abuhakmeh" };

            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(document);
                    session.SaveChanges();
                }
            }

            Debug.WriteLine(string.Format("document id: {0}", document.Id));
            document.Id.Should().NotBeBlank();
        }

        [Fact]
        public void Can_have_meaningful_ids_on_documents()
        {
            var city = new { Id = "usa/pa/philadelphia", Name = "Philadelphia", State = "Pennsylvania" };

            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(city);
                    session.SaveChanges();
                }

                // start another session for test purposes
                using (var session = store.OpenSession())
                {
                    var result = session.Load<dynamic>("usa/pa/philadelphia");
                    // we just loaded a document with a meaninful id
                    ((string)result.Name).Should().Be("Philadelphia");
                }
            }


        }

        [Fact]
        public void Can_generate_an_id_based_on_the_type_and_hilo_key_generator()
        {
            var document = new Example();

            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(document);

                    // the id is set 
                    document.Id.Should().Be("examples/1");
                }
            }
        }

        [Fact]
        public void Can_change_id_seperator_because_slashes_scare_me()
        {
            var document = new Example();
            using (var store = NewDocumentStore())
            {
                // most web developers change the identity seperator
                // to work with ASP.NET MVC routes by altering the convention
                store.Conventions.IdentityPartsSeparator = "-";

                using (var session = store.OpenSession())
                {
                    session.Store(document);

                    // the id is set 
                    document.Id.Should().Be("examples-1");
                }
            }
        }

        [Fact]
        public void Can_store_lists_in_my_objects()
        {
            var document = new Example
            {
                Id = "crazy-lists",
                Tags = new[] { "this", "is", "freaking", "awesome" }
            };

            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(document);
                    session.SaveChanges();

                    WaitForUserToContinueTheTest(store);
                }

                // we reload the document from the database
                // and we get the tags back.
                using (var session = store.OpenSession())
                {
                    session.Load<Example>("crazy-lists")
                        .Tags.Count
                        .Should().Be(4);
                }
            }
        }

        [Fact]
        public void Can_utlize_object_oriented_programming_correctly()
        {
            // http://ravendb.net/docs/theory/document-structure-design
            // - Documents are not flat
            // - Raven is not relational
            // - Entities and Aggregates are important
            // - Associations Management 
            //         (Aggregate Roots may contain all of their 
            //          children, but even Aggregates do not live in isolation)
            var order = new Order();
            var product = new Product { Id = "sku-12345", Name = "Peanut Butter", Price = 20.00m };

            // we are adding a product to an order
            order.Add(product, quantity: 1);

            // we created a new order line item
            order.Items.Count.Should().Be(1);

            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(order);
                    session.SaveChanges();
                }

                var storeUrl = store.Configuration.ServerUrl;
                WaitForUserToContinueTheTest(store);
            }

            order.Id.Should().NotBeEmpty();
        }
    }
}
