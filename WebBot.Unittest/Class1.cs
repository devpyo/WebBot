using System.Linq;
using WebBot.Logic;
using WebBot.Logic.Http;
using WebBot.Logic.XPath;
using Xunit;

namespace WebBot.Unittest
{
    public class LazyHtmlDownloader
    {

    }

    public class ModelMapper
    {
        public ModelMapper(string baseUrl, object model, object currentObject)
        {
            var currentObjectType = currentObject.GetType();
            if (currentObjectType.HasElementType)
                currentObjectType = currentObjectType.GetElementType();
            var urlElements = currentObjectType.GetFields().Select(field => new { Field = field, Attribute = field.GetAttribute<UrlAttribute>() }).Where(x => x.Attribute != null).ToArray();
            var xpathElements = currentObjectType.GetFields().Select(field => new { Field = field, Attribute = field.GetAttribute<XPathAttribute>() }).Where(x => x.Attribute != null).ToArray();
        }
    }

    public class Class1
    {
        [Fact]
        public void Test()
        {
            
        }
    }
}
