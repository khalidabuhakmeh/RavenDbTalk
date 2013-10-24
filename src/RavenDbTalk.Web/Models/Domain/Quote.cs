using System.ComponentModel.DataAnnotations;

namespace RavenDbTalk.Web.Models.Domain
{
    public class Quote
    {
        public string Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string By { get; set; }
        public string Category { get; set; }
    }
}