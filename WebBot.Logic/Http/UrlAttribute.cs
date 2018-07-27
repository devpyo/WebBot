namespace WebBot.Logic.Http
{
    public class UrlAttribute : System.Attribute
    {
        public string Url { get; private set; }

        public UrlAttribute(string url)
        {
            this.Url = url;
        }
    }
}
