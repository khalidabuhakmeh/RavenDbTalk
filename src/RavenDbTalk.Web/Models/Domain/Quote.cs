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

    public sealed class InstructionalQuote : Quote
    {
        public InstructionalQuote()
        {
            Text = "Please remember to load the sample data found in the data direction of this repository";
            By = "Khalid Abuhakmeh";
            Category = "helpful hints";
        }
    }
}