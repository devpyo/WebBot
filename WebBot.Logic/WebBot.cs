﻿using Microsoft.Extensions.Logging;
using WebBot.Logic.PageDownload;
using System.Reactive.Linq;
using HtmlAgilityPack;
using System.Collections.Generic;
using System;
using WebBot.Logic.Page;

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

        public Page<ModelType> ReadPage<ModelType>(string url) where ModelType : IPageModel
        {
            logger.LogDebug($"ReadPage: {url}");

            var text = pageDownloader.DownloadText(url);
            return new Page<ModelType>(text);
        }
    }
}
