using System;
using Microsoft.AspNet.SignalR;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using RavenDbTalk.Web.App_Start;
using RavenDbTalk.Web.Models.Domain;

namespace RavenDbTalk.Web.Models.Hubs
{
    public class LiveHub : Hub
    {
        static LiveHub()
        {
            var database = RavenDbConfiguration.DatabaseName;

            // RavenDB Changes code
            RavenDbConfiguration.DocumentStore
                       .Changes(database)
                       .ForDocumentsStartingWith("thoughts")
                       .Subscribe(change =>
                       {
                           var hub = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();

                           if (change.Type == DocumentChangeTypes.Put)
                           {
                               using (var session = RavenDbConfiguration.DocumentStore.OpenSession())
                               {
                                   var thought = session.Load<Thought>(change.Id);
                                   hub.Clients.All.addThought(new
                                   {
                                       name = thought.Name,
                                       text = thought.Text
                                   });
                               }
                           }
                       });
        }

        /* SignalR specific code */
        public void Submit(dynamic thought)
        {
            string text = thought.text;
            string name = thought.name;

            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(name))
            {
                Clients.Caller.addError("A name and a throught are required");
                return;
            }

            using (var session = RavenDbConfiguration.DocumentStore.OpenSession())
            {
                var newThought = new Thought
                {
                    Name = name,
                    Text = text,
                    CallerId = Context.ConnectionId
                };
                session.Store(newThought);
                session.SaveChanges();
            }
        }
    }
}