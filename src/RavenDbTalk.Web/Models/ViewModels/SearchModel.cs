using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RavenDbTalk.Web.Models.ViewModels
{
    public class SearchModel
    {
        public SearchModel()
        {
            Page = 1;
            Size = 50;
        }

        public int Page { get; set; }
        public int Size { get; set; }
        public string Q { get; set; }
    }
}