using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Abstractions.Data;

namespace RavenDbTalk.Web.Models.Extensions
{
    public static class SuggestionExtensions
    {
        public static bool NotEmpty(this SuggestionQueryResult result)
        {
            if (result == null) return false;
            if (result.Suggestions == null) return false;

            return result.Suggestions.Any();
        }
    }
}