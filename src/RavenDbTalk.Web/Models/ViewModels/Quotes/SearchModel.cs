using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RavenDbTalk.Web.Models.ViewModels.Quotes
{
    public class SearchModel : ViewModels.SearchModel
    {
        public string Category { get; set; }
        public string By { get; set; }
    }
}