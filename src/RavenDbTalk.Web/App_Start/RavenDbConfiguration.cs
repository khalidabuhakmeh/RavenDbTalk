using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace RavenDbTalk.Web.App_Start
{
    public static class RavenDbConfiguration
    {
        public static readonly IDocumentStore DocumentStore = new DocumentStore
        {
            ConnectionStringName = "Quotely",
            Conventions =
            {
                IdentityPartsSeparator = "-"
            }
        }.Initialize();

        public static void Start()
        {
            // create indexes from this assembly
            IndexCreation.CreateIndexes(typeof(RavenDbConfiguration).Assembly, DocumentStore);

            /* Mvc Integration */
            Raven.Client.MvcIntegration.RavenProfiler.InitializeFor(DocumentStore);
        }
    }
}