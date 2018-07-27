namespace WebBot.Logic.XPath
{
    public class XPathAttribute : System.Attribute
    {
        public string XPath { get; private set; }
        public XPathFetchType FetchType { get; private set; }

        public XPathAttribute(string xpath, XPathFetchType fetchType)
        {
            this.XPath = xpath;
            this.FetchType = fetchType;
        }
    }
}
