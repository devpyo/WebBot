using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using WebBot.Logic.Http;
using WebBot.Logic.Page;
using WebBot.Logic.PageDownload;

namespace WebBot.Logic
{
    public class WebBot
    {
        ILogger<WebBot> logger;
        PageDownloader pageDownloader;

        public WebBot(ILogger<WebBot> logger, PageDownloader pageDownloader)
        {
            this.logger = logger;
            this.pageDownloader = pageDownloader;
        }

        [Obsolete]
        public void Run(string startUrl)
        {
            logger.LogInformation($"WebBot Run : {startUrl}");

            pageDownloader.Download(startUrl).Do(content =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(content);
                var nodes = doc.DocumentNode.SelectNodes(@"//*[@id=""board_list""]/div/div[2]/table/tbody/tr[*]");

                //File.WriteAllText(@"d:/test.txt", driver.PageSource);

                var items = new List<string>();
                foreach (var node in nodes)
                {
                    var s = node.SelectSingleNode(@".//td[@class=""subject""]/div/a");
                    if (s == null)
                        continue;

                    items.Add(s.InnerText);
                }

                items.ForEach(x => logger.LogInformation(x));
            }).Wait();
        }

        void ReadPage<ModelType>(string baseUrl, ModelType model, object currentObject) where ModelType : IPageModel
        {
            var currentObjectType = currentObject.GetType();
            if (currentObjectType.HasElementType)
                currentObjectType = currentObjectType.GetElementType();
            var urlElements = currentObjectType.GetFields().Select(field => new { Field = field, Attribute = field.GetAttribute<UrlAttribute>() }).Where(x => x.Attribute != null).ToArray();
            var xpathElements = currentObjectType.GetFields().Select(field => new { Field = field, Attribute = field.GetAttribute<XPath.XPathAttribute>() }).Where(x => x.Attribute != null).ToArray();

            // currentObject의 타입 필드들의 속성을 검색한다. (배열일 경우, 단일 타입으로 확인한다)
            // url 속성 필드가 있다면, url을 캐시하고 해당 필드를 재귀함수로 현재 함수를 반복 실행한다.
                // 반복처리 할 때, model, currentObject와 currentType을 넘긴다.
                // currentObject가 비어있다면, 값을 생성해 필드에 SetValue를 한다.
            // xpath 속성 필드가 있다면, 웹페이지를 다운받고 xpath로 검색해 필드에 저장한다.

            urlElements.ToList().ForEach(x =>
            {
                ReadPage(ConcatUrl(baseUrl, x.Attribute.Url), model, x.Field.GetValue(model));
            });

            if (xpathElements.Length <= 0)
                return;

            // xpath 찾기
            var content = pageDownloader.DownloadText(baseUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            foreach (var element in xpathElements)
            {
                var nodes = doc.DocumentNode.SelectNodes(element.Attribute.XPath);
                foreach (var node in nodes)
                {
                    var s = node.SelectSingleNode(@".//td[@class=""subject""]/div/a");
                    if (s == null)
                        continue;

                    //items.Add(s.InnerText);
                }
            }

           
        }

        public Page<ModelType> ReadPage<ModelType>() where ModelType : IPageModel, new()
        {
            var model = new ModelType();
            var urlAttribute = typeof(ModelType).GetAttribute<UrlAttribute>();
            if (urlAttribute == null)
                throw new Exception($"not found url attribute from model: {typeof(ModelType).Name}");

            var url = urlAttribute.Url;
            logger.LogDebug($"ReadPage: {url}");

            ReadPage(url, model, model);

            return new Page<ModelType>(model);
        }

        string ConcatUrl(string baseUrl, string appendUrl)
        {
            return string.Concat(baseUrl, "/", appendUrl).Replace("//", "/");
        }
    }
}
