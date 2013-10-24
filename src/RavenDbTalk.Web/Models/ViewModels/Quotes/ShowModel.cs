using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RavenDbTalk.Web.Models.Domain;

namespace RavenDbTalk.Web.Models.ViewModels.Quotes
{
    public class ShowModel
    {
        public ShowModel()
        {
            MoreLikeThis = new List<Quote>();
        }

        public Quote Quote { get; set; }
        public IList<Quote> MoreLikeThis { get; set; } 
    }
}