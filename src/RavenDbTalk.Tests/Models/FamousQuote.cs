namespace RavenDbTalk.Tests.Models
{
    public class FamousQuote
    {
        public string Id { get; set; }
        public string By { get; set; }
        public string Said { get; set; }

        public override string ToString()
        {
            return string.Format("{0} said : \"{1}\"", By, Said);
        }
    }
}